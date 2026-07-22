using Iced.Intel;
using StringReloads.AutoInstall.Patcher;
using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Engine.String;
using StringReloads.Engine.Unmanaged;
using StringReloads.Hook;
using StringReloads.Hook.Base;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using static StringReloads.Engine.User;
using static StringReloads.Hook.Base.Extensions;

namespace StringReloads.AutoInstall
{
    unsafe class SoftPalMethodC : IAutoInstall
    {

        bool SetupMode = false;
        CallerTracer Tracer;
        Intercept Intercepter;
        SoftPal_PalSpriteCreateText FontDrawTextHook;
        SoftPal_PalSpriteCreateTextEx FontDrawTextExHook;

        List<int> Invalids = new List<int>();
        Config Config => Config.Default;
        Dictionary<string, string> SoftPalConfig;

        void* GetGlyphOutlineA = null;
        public string Name => "SoftPal#C";

        uint StackOffset = 0;
        uint StackOffsetB = 0;
        Dictionary<uint, int> BufferOffsets = new Dictionary<uint, int>();
        const int PrefixScanLimit = 16;

        public void Install()
        {
            if (SetupMode)
            {
                Tracer = new CallerTracer(GetGlyphOutlineA);
                Tracer.CallerCatched += SetupStepA;
                Tracer.Install();
                ShowMessageBox("Very well. SRL will now do the dirty work of setting up the game for you. Your input is still needed, however; please start up the game and make it display a piece of dialogue.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
                return;
            }

            if (SoftPalConfig == null || !SoftPalConfig.ContainsKey("hookmodule") || !SoftPalConfig.ContainsKey("hookoffset") || !SoftPalConfig.ContainsKey("stackoffset") || !SoftPalConfig.ContainsKey("stackoffsetb") || !SoftPalConfig.ContainsKey("fromebp") || !SoftPalConfig.ContainsKey("enginesize"))
            {
                Log.Debug("SoftPal configuration is incomplete, restarting setup flow.");
                SetupMode = true;
                Tracer = new CallerTracer(GetGlyphOutlineA);
                Tracer.CallerCatched += SetupStepA;
                Tracer.Install();
                ShowMessageBox("Very well. SRL will now do the dirty work of setting up the game for you. Your input is still needed, however; please start up the game and make it display a piece of dialogue.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
                return;
            }

            StackOffset = SoftPalConfig["stackoffset"].ToUInt32();
            StackOffsetB = SoftPalConfig["stackoffsetb"].ToUInt32();
            BufferOffsets.Clear();
            LoadBufferOffset(StackOffset, "bufferoffset");
            LoadBufferOffset(StackOffsetB, "bufferoffsetb");
            LoadDynamicBufferOffsets();

            FromEBP = SoftPalConfig["fromebp"].ToBoolean();
            void* hookModuleBase = ResolveHookModuleBase(SoftPalConfig["hookmodule"]);
            void* hFunc = (void*)((ulong)hookModuleBase + SoftPalConfig["hookoffset"].ToUInt64());
            Intercepter = new ManagedInterceptor(hFunc, new ManagedInterceptDelegate(DrawTextHook));
            Intercepter.Install();

            FontDrawTextHook = new SoftPal_PalSpriteCreateText();
            FontDrawTextHook.Install();


            try
            {
                FontDrawTextExHook = new SoftPal_PalSpriteCreateTextEx();
                FontDrawTextExHook.Install();
            }
            catch
            {
                FontDrawTextExHook = null;
            }
        }

        public bool IsCompatible()
        {
            var DLLPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll", "pal.dll");

            if (!File.Exists(DLLPath))
                return false;

            var hModule = GetLibrary(DLLPath);
            if (hModule == null)
                return false;

            GetGlyphOutlineA = GetProcAddress(GetLibrary("gdi32.dll"), "GetGlyphOutlineA");

            if (GetProcAddress(hModule, "PalFontDrawText") == null)
                return false;

            SoftPalConfig = Config.GetValues("SoftPal");

            if (SoftPalConfig != null && (SoftPalConfig.ContainsKey("hookoffset_d") || SoftPalConfig.ContainsKey("forcemethodd")))
                return false;

            if (SoftPalConfig != null && SoftPalConfig.ContainsKey("forcemethodb"))
            {
                if (SoftPalConfig["forcemethodb"].ToBoolean())
                    return false;
            }

            if (SoftPalConfig == null || !SoftPalConfig.ContainsKey("enginesize") || SoftPalConfig["enginesize"].ToInt64() != new FileInfo(Config.GameExePath).Length)
            {
                var Rst = ShowMessageBox("SRL is not configured to work with this game yet, do you want to configure it now?", "StringReloads Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.No)
                    return false;

                ExeTools.ApplyWrapperPatch();
                SetupMode = true;
            }

            return true;
        }

        public void Uninstall()
        {
            Intercepter.Uninstall();
            FontDrawTextHook.Uninstall();
            FontDrawTextExHook?.Uninstall();
        }

        void DrawTextHook(ref ulong ESP, ref ulong EAX, ref ulong ECX, ref ulong EDX, ref ulong EBX, ref ulong EBP, ref ulong ESI, ref ulong EDI)
        {
            uint* BasePtr = (FromEBP ? ((uint*)EBP) : ((uint*)ESP));

            ProcessGameString(BasePtr, StackOffset);

            if (StackOffsetB != StackOffset && StackOffsetB != 0)
                ProcessGameString(BasePtr, StackOffsetB);
        }

        private void ProcessGameString(uint* BasePtr, uint Offset)
        {
            uint* Address = BasePtr + Offset;
            var SourcePtr = (byte*)*Address;
            if (SourcePtr == null || Engine.Unmanaged.SanityChecks.IsBadPtr(SourcePtr))
                return;

            CString SourceStr = SourcePtr;
            var OriginalText = (string)SourceStr;
            if (string.IsNullOrEmpty(OriginalText))
                return;

            var ProcessedPtr = (byte*)EntryPoint.Process((void*)*Address);
            if (ProcessedPtr == null || Engine.Unmanaged.SanityChecks.IsBadPtr(ProcessedPtr))
                return;

            CString ProcessedStr = ProcessedPtr;
            var ProcessedText = (string)ProcessedStr;
            if (string.IsNullOrEmpty(ProcessedText) || ProcessedText == OriginalText)
                return;

            *Address = (uint)ProcessedPtr;

            if (!TryGetBufferOffset(Offset, out var BufferOffset))
                BufferOffset = ResolveBufferOffset(BasePtr, Offset, OriginalText);

            if (BufferOffset >= 0)
                RepairPrefixBuffer(BasePtr, (uint)BufferOffset, OriginalText, ProcessedPtr);
        }

        private void RepairPrefixBuffer(uint* BasePtr, uint StackOffset, string OriginalText, byte* ProcessedPtr)
        {
            uint* CandidateAddress = BasePtr + StackOffset;
            var CandidatePtr = (byte*)*CandidateAddress;
            if (CandidatePtr == null || Engine.Unmanaged.SanityChecks.IsBadPtr(CandidatePtr))
                return;

            CString CandidateStr = CandidatePtr;
            var CandidateText = (string)CandidateStr;
            if (string.IsNullOrEmpty(CandidateText))
                return;

            if (CandidateText.Length > 4)
                return;

            if (!OriginalText.StartsWith(CandidateText, StringComparison.Ordinal))
                return;

            int bufferLen = CandidateText.Length + 1;

            ZeroBuffer(CandidatePtr, CandidateText.Length + 1);

            CString ReturnStr = ProcessedPtr;
            var ReturnedText = (string)ReturnStr;
            if (!string.IsNullOrEmpty(ReturnedText))
            {
                var Prefixed = new string(' ', bufferLen) + ReturnedText;
                if (Prefixed.Length < FindStrBufferSize(ProcessedPtr))
                    ReturnStr.SetContent(Prefixed);
            }
        }

        private int ResolveBufferOffset(uint* BasePtr, uint StackOffset, string OriginalText)
        {
            var FoundOffset = DetectPrefixBufferOffset(BasePtr, StackOffset, OriginalText);
            if (FoundOffset < 0)
                return -1;

            SetBufferOffset(StackOffset, FoundOffset, true);
            return FoundOffset;
        }

        private bool TryGetBufferOffset(uint StackOffset, out int BufferOffset)
        {
            if (BufferOffsets.TryGetValue(StackOffset, out BufferOffset))
                return BufferOffset >= 0;

            var Key = GetBufferOffsetKey(StackOffset);
            if (SoftPalConfig != null && SoftPalConfig.ContainsKey(Key))
            {
                BufferOffset = SoftPalConfig[Key].ToInt32();
                BufferOffsets[StackOffset] = BufferOffset;
                return BufferOffset >= 0;
            }

            BufferOffset = -1;
            return false;
        }

        private void SetBufferOffset(uint StackOffset, int BufferOffset, bool Persist)
        {
            BufferOffsets[StackOffset] = BufferOffset;
            Log.Debug($"SoftPal prefix buffer offset for stack +{StackOffset} resolved at +{BufferOffset}");

            if (!Persist || SoftPalConfig == null)
                return;

            SoftPalConfig[GetBufferOffsetKey(StackOffset)] = BufferOffset.ToString();
            Config.SetValues("SoftPal", SoftPalConfig);
            Config.SaveSettings();
        }

        private string GetBufferOffsetKey(uint StackOffset)
        {
            if (StackOffset == this.StackOffset)
                return "bufferoffset";

            if (StackOffset == this.StackOffsetB)
                return "bufferoffsetb";

            return $"bufferoffset_{StackOffset}";
        }

        private void LoadBufferOffset(uint StackOffset, string Key)
        {
            if (SoftPalConfig == null || !SoftPalConfig.ContainsKey(Key))
                return;

            if (!int.TryParse(SoftPalConfig[Key], out var BufferOffset))
                return;

            BufferOffsets[StackOffset] = BufferOffset;
        }

        private void LoadDynamicBufferOffsets()
        {
            if (SoftPalConfig == null)
                return;

            foreach (var Entry in SoftPalConfig)
            {
                if (!Entry.Key.StartsWith("bufferoffset_", StringComparison.OrdinalIgnoreCase))
                    continue;

                var RawStackOffset = Entry.Key.Substring("bufferoffset_".Length);
                if (!uint.TryParse(RawStackOffset, out var StackOffset))
                    continue;

                if (!int.TryParse(Entry.Value, out var BufferOffset))
                    continue;

                BufferOffsets[StackOffset] = BufferOffset;
            }
        }

        private void PersistBufferOffset(uint StackOffset)
        {
            if (SoftPalConfig == null)
                return;

            if (!BufferOffsets.TryGetValue(StackOffset, out var BufferOffset))
                BufferOffset = -1;

            SoftPalConfig[GetBufferOffsetKey(StackOffset)] = BufferOffset.ToString();
        }

        private void SaveHookLocation(void* HookAddress)
        {
            var HookModule = ResolveHookModule(HookAddress);
            void* HookModuleBase = Config.GameBaseAddress;
            if (HookModule != null)
                HookModuleBase = HookModule.BaseAddress.ToPointer();
            var HookModuleName = HookModule != null ? Path.GetFileName(HookModule.FileName) : Path.GetFileName(Config.GameExePath);

            SoftPalConfig["hookmodule"] = HookModuleName;
            SoftPalConfig["hookoffset"] = ((ulong)HookAddress - (ulong)HookModuleBase).ToString();

            Log.Debug($"SoftPal hook module resolved to {HookModuleName} with offset 0x{SoftPalConfig["hookoffset"]}");
        }

        private void* ResolveHookModuleBase(string ModulePath)
        {
            var ModuleName = Path.GetFileName(ModulePath);
            var Module = Config.Modules.FirstOrDefault(x =>
                x.ModuleName.Equals(ModuleName, StringComparison.OrdinalIgnoreCase) ||
                Path.GetFileName(x.FileName).Equals(ModuleName, StringComparison.OrdinalIgnoreCase));

            if (Module == null)
                return Config.GameBaseAddress;

            return Module.BaseAddress.ToPointer();
        }

        private ProcessModule ResolveHookModule(void* HookAddress)
        {
            foreach (var Module in Config.Modules)
            {
                try
                {
                    var Info = ModuleInfo.GetCodeInfo((byte*)Module.BaseAddress);
                    if (Info.AddressIsContained(HookAddress))
                        return Module;
                }
                catch
                {
                }
            }

            return Config.Modules.FirstOrDefault(x =>
            {
                var Start = (ulong)x.BaseAddress;
                var End = Start + (ulong)x.ModuleMemorySize;
                var Address = (ulong)HookAddress;
                return Address >= Start && Address < End;
            });
        }

        private void ZeroBuffer(byte* Buffer, int Length)
        {
            for (int i = 0; i < Length; i++)
                Buffer[i] = 0;
        }

        void SetupStepA(void* Caller)
        {
            Log.Debug($"GetGlyphOutlineA called from 0x{(uint)Caller:X8}");
            if (!SetupMode)
                return;

            ShowMessageBox("Yes, you did well. Now press OK and wait for SRL to analyze the text rendering fuction of the game...", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);

            var hFunc = Tracer.SearchFuntionAddress((byte*)Caller);

            if (hFunc == null)
            {
                Log.Debug("Failed to search the function address.");
                SetupFailed();
            }


            Log.Debug($"GetGlyphOutlineA Referenced by 0x{(uint)hFunc:X8}");


            var MemReader = new MemoryCodeReader(hFunc);
            var Diassembler = Decoder.Create(32, MemReader);
            for (int i = 0, x = 0; i < 30; i++)
            {
                var CurrentOffset = Diassembler.IP;
                var Instruction = Diassembler.Decode();
                switch (x)
                {
                    case 0:
                        if (Instruction.IsCallNear && Instruction.IPRelativeMemoryAddress == Diassembler.IP)
                            x++;
                        break;
                    case 1:
                        if (Instruction.Code == Code.Pop_r32)
                            x++;
                        else
                            x = 0;
                        break;
                    case 2:
                        if (Instruction.Code == Code.Add_EAX_imm32)
                            x++;
                        else
                            x = 0;
                        break;
                    case 3:
                        if (Instruction.IsJmpNearIndirect)
                            x++;
                        else
                            x = 0;
                        break;
                    case 4:
                        if (Instruction.Code == Code.Nopd)
                            break;

                        Log.Debug("SoftPAL Wordwrap Patched Engine Detected");
                        hFunc = (byte*)hFunc + CurrentOffset;
                        x++;
                        break;
                }
            }



            SoftPalConfig = new Dictionary<string, string>();
            SaveHookLocation(hFunc);
            SoftPalConfig["enginesize"] = new FileInfo(Config.GameExePath).Length.ToString();

            Tracer.Uninstall();

            Tracer = new CallerTracer(hFunc);
            Tracer.CallerCatched += SetupStepB;
            Tracer.Install();
        }

        void SetupStepB(void* Caller)
        {
            Log.Debug($"Function called from 0x{(uint)Caller:X8}");
            if (!SetupMode)
                return;

            var hFunc = Tracer.SearchFuntionAddress((byte*)Caller);

            if (hFunc == null)
            {
                Log.Debug("Failed to search the function address.");
                SetupFailed();
            }


            Log.Debug($"GetGlyphOutlineA Caller Referenced by 0x{(uint)hFunc:X8}");


            SoftPalConfig = new Dictionary<string, string>();
            SaveHookLocation(hFunc);
            SoftPalConfig["enginesize"] = new FileInfo(Config.GameExePath).Length.ToString();

            Tracer.Uninstall();
            
            Intercepter = new ManagedInterceptor(hFunc, new ManagedInterceptDelegate(SetupStepC));
            Intercepter.Install();
            ShowMessageBox($"Very well, looks like SRL can perform Auto-Install in this game!\nSRL now needs to gather more intricate information from the game.\nPlease press OK and continue to the next in-game dialogue.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
        }

        int CallSkips = 0;
        int FoundCount = 0;
        bool FromEBP;
        int LastOffset = 0;
        bool WaitingConfirmation = false;
        DateTime WaitingSince = DateTime.MinValue;
        bool FirstTry = true;
        void SetupStepC(ref ulong ESP, ref ulong EAX, ref ulong ECX, ref ulong EDX, ref ulong EBX, ref ulong EBP, ref ulong ESI, ref ulong EDI)
        {
            if (FirstTry)
                ShowMessageBox("SRL will now need your help to confirm if the dialogue has been found.\nThe program will display the game dialogue; when the correct dialogue is shown, press YES. If nothing is shown or if you see corrupted text, press NO.\nTake note that if you don't set the proper game encoding inside SRL.ini, the correct dialogue will most likely never be shown.\nPress OK if you have read and understood the above.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);

            if (WaitingConfirmation)
            {
                if ((DateTime.Now - WaitingSince).TotalSeconds < 2)
                    return;
                WaitingConfirmation = false;
                var Rst = ShowMessageBox("And then, you saw the confirmation message?", "StringReloads Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.Yes)
                {
                    switch (FoundCount++)
                    {
                        case 0:
                            FinishStepA();
                            break;
                        case 1:
                            FinishStepB();
                            FinishSetup();
                            break;
                        default:
                            FinishSetup();
                            break;

                    }
                }

                WaitingConfirmation = false;
            }

            uint* Stack = (uint*)ESP;
            uint* BaseStack = (uint*)EBP;
            int StackOffset = LastOffset;

            const int StackLimit = 40;

            while (StackOffset < StackLimit)
            {
                var RStack = (Stack + StackOffset);
                var BStack = (BaseStack + StackOffset);

                if (GuessOffset(RStack, StackOffset))
                {
                    FromEBP = false;
                    break;
                }

                if (GuessOffset(BStack, StackOffset))
                {
                    FromEBP = true;
                    break;
                }

                StackOffset++;
                continue;
            }


            LastOffset = StackOffset;


            if (StackOffset >= StackLimit)
            {
                if (CallSkips++ < 10)
                {
                    LastOffset = 0;
                    return;
                }

                SetupFailed();
            }


            if (FirstTry)
            {
                FirstTry = false;
                ShowMessageBox("Very well, SRL will now try translating the game text to test settings.\nIf you don't see a message in-game confirming what has been set, then continue on to the next in-game dialogue and press NO; otherwise, press YES.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
            }
        }

        private bool GuessOffset(uint* Address, int Offset)
        {
            if (IsBadCodePtr((void*)*Address))
            {
                return false;
            }

            Log.Debug($"Guessing Offset: 0x{(uint)Address:X8} (+{StackOffset}) (0x{*Address:X8})");
            CString Str = (byte*)*Address;
            if (Str.Count() > 0 && Str.Count() < 500)
            {
                var hash = ((string)Str).GetHashCode();

                if (Invalids.Contains(hash))
                {
                    return false;
                }

                var Reply = ShowMessageBox(Str, "Is this the dialogue?", MBButtons.YesNo, MBIcon.Question);
                if (Reply == MBResult.Yes)
                {
                    uint* BasePtr = Address - Offset;
                    var BufferOffset = DetectPrefixBufferOffset(BasePtr, (uint)Offset, (string)Str);
                    if (BufferOffset >= 0)
                    {
                        SetBufferOffset((uint)Offset, BufferOffset, false);
                        Log.Debug($"SoftPal prefix buffer offset found at +{BufferOffset}");
                    }
                    else
                        Log.Debug("SoftPal prefix buffer offset was not found during setup.");

                    var bufferSize = FindStrBufferSize((byte*)*Address);

                    var msg = "Looks Like everything is working,<br>Continue to the next game dialogue!";
                    if (msg.Length >= bufferSize)
                    {
                        var current = msg.Substring(0, bufferSize - 1);
                    }

                    Str.SetContent(msg);

                    WaitingConfirmation = true;
                    WaitingSince = DateTime.Now;
                    return true;
                } 
                else
                {
                    Invalids.Add(hash);
                }
            }

            return false;
        }

        private int FindStrBufferSize(byte* ptr)
        {
            const int MaxScan = 0x10000;

            int offset = 0;

            // Procura primeiro byte nulo
            while (offset < MaxScan && ptr[offset] != 0)
            {
                offset++;
            }

            if (offset == MaxScan)
            {
                throw new Exception("Nenhum byte nulo encontrado.");
            }

            // Conta sequência de bytes nulos
            int nullCount = 0;
            while ((offset + nullCount) < MaxScan &&
                   ptr[offset + nullCount] == 0)
            {
                nullCount++;
            }

            return offset + nullCount - 1;
        }

        private int DetectPrefixBufferOffset(uint* BasePtr, uint StackOffset, string OriginalText)
        {
            var ScanEnd = StackOffset + (uint)PrefixScanLimit;

            for (uint Offset = StackOffset + 1; Offset > 0; Offset--)
            {
                uint* CandidateAddress = BasePtr + Offset;
                var CandidatePtr = (byte*)*CandidateAddress;
                if (CandidatePtr == null || StringReloads.Engine.Unmanaged.SanityChecks.IsBadPtr(CandidatePtr))
                    continue;

                CString CandidateStr = CandidatePtr;
                var CandidateText = (string)CandidateStr;
                if (string.IsNullOrEmpty(CandidateText))
                    continue;

                if (CandidateText.Length > 4)
                    continue;

                if (!OriginalText.StartsWith(CandidateText, StringComparison.Ordinal))
                    continue;

                return (int)Offset;
            }

            return -1;
        }

        private void FinishStepA()
        {
            SoftPalConfig["stackoffset"] = LastOffset.ToString();
            PersistBufferOffset((uint)LastOffset);

            SoftPalConfig["fromebp"] = FromEBP ? "true" : "false";

            if (Config.BreakLine != "<br>")
            {
                var Rst = ShowMessageBox("Looks like you aren't using the tag <br> as breakline rigth now, You want use it?", "StringReloader Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.Yes)
                    Config.SetValue("BreakLine", "<br>");
            }

            if (!Config.SafeOverwrite)
            {
                var Rst = ShowMessageBox("It looks like you are not in safe memory overwrite mode, which is probably needed for this game, do you want to enable safe memory overwrite mode?", "StringReloader Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.Yes)
                    Config.SetValue("SafeOverwrite", "true");
            }

        }

        private void FinishStepB()
        {
            SoftPalConfig["stackoffsetb"] = LastOffset.ToString();
            PersistBufferOffset((uint)LastOffset);

            SoftPalConfig["fromebp"] = FromEBP ? "true" : "false";

            if (Config.BreakLine != "<br>")
            {
                var Rst = ShowMessageBox("Looks like you aren't using the tag <br> as breakline rigth now, You want use it?", "StringReloader Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.Yes)
                    Config.SetValue("BreakLine", "<br>");
            }

            if (!Config.SafeOverwrite)
            {
                var Rst = ShowMessageBox("It looks like you are not in safe memory overwrite mode, which is probably needed for this game, do you want to enable safe memory overwrite mode?", "StringReloader Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.Yes)
                    Config.SetValue("SafeOverwrite", "true");
            }

        }

        private void FinishSetup()
        {
            if (SoftPalConfig != null)
            {
                Config.SetValues("SoftPal", SoftPalConfig);
                Config.SaveSettings();
            }

            ShowMessageBox($"Perfect! SRL is now ready to use! Enjoy!", "StringReloads", MBButtons.Ok, MBIcon.Information);
            ExeTools.Restart();
        }
        private void SetupFailed()
        {
            ShowMessageBox("Hmm, looks like SRL can't perform Auto-Install in this game at the moment. Please report an issue in the GitHub repository.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Error);
            ExeTools.Restart();
        }
    }
}
