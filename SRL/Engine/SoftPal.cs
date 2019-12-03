using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static SRL.StringReloader;

namespace SRL.Engine
{
    class SoftPal : IEngine
    {
        bool PostSetupMode = false;
        bool SetupMode = false;
        SoftPALSettings Settings;

        bool HookReady {
            get {
                if (Settings.Checksum == 0)
                    return false;

                var EXE = File.ReadAllBytes(Application.ExecutablePath);
                PECSVal(EXE, out uint GameChecksum);
                return Settings.Checksum == GameChecksum;
            }
        }

        byte Method = byte.MaxValue;
        string HookMethod {
            get {
                switch (Method)
                {
                    case 0: return "v1.17";
                    case 1: return "v1.15";
                    case 2: return "v1.17b";
                    case 3: return "v1.15b";
                    default: return "JIT";
                }
            }
        }
        public string Name => $"SoftPAL ({HookMethod})";

        DrawText DrawTextHookDelegate;
        DrawText DrawTextRealDelegate;

        UnmanagedHook<DrawText> DrawTextHookManager;

        PalFontDrawText PalFontDrawTextHookDelegate;
        PalFontDrawText PalFontDrawTextRealDelegate;

        UnmanagedHook<PalFontDrawText> PalFontDrawTextHookManager;

        IntPtr PalHandler;
        IntPtr RealFunc;
        IntPtr HookFunc;

        IntPtr SetupFunc;

        public bool IsCompatible()
        {
            foreach (var Import in UnmanagedHook.GetImports())
            {
                if (Import.Module.ToLower() == "pal.dll" && Import.Function?.ToLower() == "drawtext")
                {
                    DrawTextRealDelegate = (DrawText)Marshal.GetDelegateForFunctionPointer(Import.FunctionAddress, typeof(DrawText));

                    DrawTextHookDelegate = DrawTextHook;

                    DrawTextHookManager = new UnmanagedHook<DrawText>(Import, DrawTextHookDelegate);

                    Method = 0;
                    return true;
                }

                if (Import.Module.ToLower() == "pal.dll" && Import.Function?.ToLower() == "palfontdrawtext")
                {
                    PalFontDrawTextRealDelegate = (PalFontDrawText)Marshal.GetDelegateForFunctionPointer(Import.FunctionAddress, typeof(PalFontDrawText));

                    PalFontDrawTextHookDelegate = PalFontDrawTextHook;

                    PalFontDrawTextHookManager = new UnmanagedHook<PalFontDrawText>(Import, PalFontDrawTextHookDelegate);

                    Method = 1;
                    return true;
                }
            }

            string DLL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll", "pal.dll");
            if (File.Exists(DLL))
            {
                PalHandler = LoadLibrary(DLL);
                if (PalHandler == IntPtr.Zero)
                    return false;

                OnLoadLibrary += (Name) => {
                    string Module = Path.GetFileName(Name).ToLower();
                    if (Module == "pal.dll")
                        return PalHandler;
                    return null;
                };

                OnGetProcAddressName += (Module, Func) =>
                {

                    if (Module == PalHandler)
                    {
                        switch (Func.ToLower())
                        {
                            case "drawtext":
                                if (HookFunc == IntPtr.Zero)
                                {
                                    Method = 2;
                                    RealFunc = GetProcAddress(PalHandler, Func);
                                    DrawTextRealDelegate = (DrawText)Marshal.GetDelegateForFunctionPointer(RealFunc, typeof(DrawText));
                                    DrawTextHookDelegate = DrawTextHook;
                                    HookFunc = Marshal.GetFunctionPointerForDelegate(DrawTextHookDelegate);

                                    if (Debugging || Verbose)
                                        Log($"{Name} Hook Initialized", true);
                                }
                                break;
                            case "palfontdrawtext":
                                if (HookFunc == IntPtr.Zero)
                                {
                                    Method = 3;
                                    RealFunc = GetProcAddress(PalHandler, Func);
                                    PalFontDrawTextRealDelegate = (PalFontDrawText)Marshal.GetDelegateForFunctionPointer(RealFunc, typeof(PalFontDrawText));
                                    PalFontDrawTextHookDelegate = PalFontDrawTextHook;
                                    HookFunc = Marshal.GetFunctionPointerForDelegate(PalFontDrawTextHookDelegate);

                                    if (!HookReady)
                                    {
                                        SetupFunc = Marshal.AllocHGlobal(SetupInfoBase.Length);
                                        if (!UnmanagedImports.Write(SetupFunc, SetupInfo, UnmanagedImports.Protection.PAGE_EXECUTE_READWRITE))
                                            Error("Failed to alloc the setup hook...");
                                        else if (Debugging || Verbose)
                                            Log($"{Name} Hook Setup" + (Verbose ? " Allocated at 0x{0:X8}" : ""), true, SetupFunc.ToUInt32());
                                    }
                                    else if (Debugging || Verbose)
                                        Log($"{Name} Hook Initialized", true);
                                }
                                break;
                            default:
                                return null;
                        }

                        if (PostSetupMode || WaitingConfirmation)
                            return RealFunc;

                        if (SetupMode)
                            return SetupFunc;

                        return HookFunc;
                    }
                    return null;
                };

                Settings = new SoftPALSettings();
                AdvancedIni.FastOpen(out Settings, IniPath);

                InstallLoadLibraryHooks();
                Method = byte.MaxValue - 1;

                if (!HookReady)
                {
                    var Rst = MessageBox.Show("SRL is not configured to work with this game yet, do you want to configure it now?", "StringReloader", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Rst == DialogResult.No)
                    {
                        return false;
                    }

                    SetupMode = true;

                    if (GameLineBreaker != "<br>")
                    {
                        Rst = MessageBox.Show("Looks like you aren't using the tag <br> as breakline rigth now, You want use it?", "StringReloader Setup Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (Rst == DialogResult.Yes)
                        {
                            LiveSettings = false;
                            GameLineBreaker = "<br>";

                            System.Threading.Thread.Sleep(1000);

                            if (Ini.GetConfigStatus(CfgName, "GameBreakLine", IniPath) == Ini.ConfigStatus.Ok)
                                Ini.SetConfig(CfgName, "GameBreakLine", GameLineBreaker, IniPath);
                           else
                                Ini.SetConfig(CfgName, "BreakLine", GameLineBreaker, IniPath);

                        }
                    }

                    MessageBox.Show("Very well. SRL will now do the dirty work of setting up the game for you. Your input is still needed, however; please start up the game and make it display a piece of dialogue.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Method = 3;
                    InstallDynamicHook();
                }

                return true;
            }
            return false;
        }

        public void InstallStrHook()
        {
            switch (Method)
            {
                case 2:
                case 0:
                    DrawTextHookManager.Install();
                    break;
                case 1:
                    PalFontDrawTextHookManager.Install();
                    break;
                case byte.MaxValue - 1:
                case 3:
                    InstallDynamicHook();
                    break;
                default:
                    throw new Exception("Hook ID Not Found");
            }
        }

        public void UninstallStrHook()
        {
            switch (Method)
            {
                case 2:
                case 0:
                    DrawTextHookManager.Uninstall();
                    break;
                case 1:
                    PalFontDrawTextHookManager.Uninstall();
                    break;

                case byte.MaxValue - 1:
                case 3:
                    UninstallDynamicHook();
                    break;
                default:
                    throw new Exception("Hook ID Not Found");
            }
        }

        IntPtr DynamicHookAddress;
        IntPtr DynamicRealAddress;
        byte[] DynamicHookData;
        byte[] DynamicJmpData;
        void InstallDynamicHook()
        {
            if (SetupMode)
                return;

            if (DynamicRealAddress == IntPtr.Zero)
            {
                if (Verbose)
                    Log("Initializing Dynamic Hooks (BaseAddress: {0:X8} + Offset: {1:X8})", true, GameBaseAddress, Settings.Offset);

                DynamicRealAddress = new IntPtr(unchecked((int)(GameBaseAddress + Settings.Offset)));
                DynamicHookData = UnmanagedImports.Read(DynamicRealAddress, 5);

                DynamicHookAddress = Marshal.AllocHGlobal(100);

                byte[] Function = Settings.PersistentHook ? PersistentHook : NonPersistentHook;

                UnmanagedImports.Write(DynamicHookAddress, Function, UnmanagedImports.Protection.PAGE_EXECUTE_READWRITE);
            
                DynamicJmpData = UnmanagedImports.AssembleJump(DynamicRealAddress, DynamicHookAddress);
            }

            UnmanagedImports.Write(DynamicRealAddress, DynamicJmpData);
            UnmanagedImports.FlushInstructionCache(DynamicHookAddress, (uint)DynamicJmpData.Length);
        }
        
        void UninstallDynamicHook()
        {
            UnmanagedImports.Write(DynamicRealAddress, DynamicHookData);
            UnmanagedImports.FlushInstructionCache(DynamicHookAddress, (uint)DynamicHookData.Length);
        }

        public void DrawTextHook(IntPtr Text, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15, IntPtr a16, IntPtr a17, IntPtr a18, IntPtr a19, IntPtr a20, IntPtr a21, IntPtr a22, IntPtr a23)
        {
            Text = ProcessReal(Text);
            DrawTextRealDelegate(Text, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18, a19, a20, a21, a22, a23);
        }

        public IntPtr PalFontDrawTextHook(IntPtr a1, IntPtr Text, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6)
        {
            Text = ProcessReal(Text);
            return PalFontDrawTextRealDelegate(a1, Text, a3, a4, a5, a6);
        }

        void BeginHook(IntPtr Stack, IntPtr Return)
        {
            HookReturnAddress = Return;
            IntPtr NewPtr = ProcessReal(Marshal.ReadIntPtr(Stack));
            Marshal.WriteIntPtr(Stack, NewPtr);
            UninstallDynamicHook();
        }

        IntPtr EndHook()
        {
            InstallDynamicHook();
            return HookReturnAddress;
        }

        //Engine v1.17
        //PAL v1.10 - 2019
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DrawText(IntPtr Text, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15, IntPtr a16, IntPtr a17, IntPtr a18, IntPtr a19, IntPtr a20, IntPtr a21, IntPtr a22, IntPtr a23);

        //Engine v1.15
        //PAL v1.10 - 2006-2017 
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr PalFontDrawText(IntPtr a1, IntPtr Text, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6);

        struct SoftPALSettings
        {
            [FieldParmaters(DefaultValue = 0u, Name = "Checksum")]
            public uint Checksum;

            [FieldParmaters(DefaultValue = 0u, Name = "Offset")]
            public uint Offset;

            [FieldParmaters(DefaultValue = 0u, Name = "StackOffset")]
            public uint StackOffset;

            [FieldParmaters(DefaultValue = 0u, Name = "PersistentHookSize")]
            public uint PersistentHookSize;

            [FieldParmaters(DefaultValue = false, Name = "PersistentHook")]
            public bool PersistentHook;
        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void InfoCatcher(IntPtr ReturnAddress, IntPtr ESP);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void HookBegin(IntPtr StackAddress, IntPtr ReturnAddress);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate IntPtr HookEnd();


        InfoCatcher CallInfoDelegate;

        void CallingInfoReciver(IntPtr Return, IntPtr Stack)
        {
            if (Verbose)
                Log("DrawText Called From {0:X8} (ESP: {1:X8})", true, Return.ToUInt32(), Stack.ToUInt32());

            if (!SetupMode)
                return;

            SetupMode = false;

            MessageBox.Show("Yes, you did well. Now press OK and wait for SRL to analyze the text rendering fuction of the game...", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
         
            bool? SupportedFunc = null;
            IntPtr Offset = IntPtr.Zero;
            int Sub = 0;
            while (Sub < 0x900)
            {
                Offset = Return.Sum(Sub++ * -1);
                byte[] Buff = UnmanagedImports.Read(Offset, 100);
                if (BufferEquals(Buff, SupportedFuncPrefixA))
                {
                    Settings.PersistentHookSize = 6;
                    SupportedFunc = true;
                    break;
                }
                if (BufferEquals(Buff, SupportedFuncPrefixB))
                {
                    Settings.PersistentHookSize = 6;
                    SupportedFunc = true;
                    break;
                }
                if (BufferEquals(Buff, UnsupportedFuncPrefix))
                {
                    SupportedFunc = false;
                    Offset.Sum(2);
                    break;
                }
            }

            if (!SupportedFunc.HasValue) {

                if (Verbose)
                    Warning("Unsupported SoftPAL Game Detected", true);

                MessageBox.Show("Hmm, looks like SRL can't perform Auto-Install in this game at the moment. Please report an issue in the GitHub repository.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Settings.PersistentHook = SupportedFunc.Value;
            Settings.StackOffset = 0;
            Settings.Offset = Offset.ToUInt32() - GameBaseAddress;

            var EXE = File.ReadAllBytes(Application.ExecutablePath);
            PECSVal(EXE, out uint GameChecksum);
            Settings.Checksum = GameChecksum;

            if (Verbose)
                Log($"Supported {(SupportedFunc.Value ? "Persistent" : "Non-Persistent")} Hook at 0x{Offset.ToUInt32():X8}", true);


            PostSetupMode = true;

            InstallDynamicHook();

            MessageBox.Show($"Very well, looks like SRL can perform Auto-Install in this game, {(SupportedFunc.Value ? "optimally!" : ", albeit non-optimally.")} SRL now needs to gather more intricate information from the game. Please press OK and continue to the next in-game dialogue.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static bool WaitingConfirmation;
        static int LastOffset = -1;
        static bool PostSetupInstructionViewed;
        void PostSetupBeginHook(IntPtr Stack, IntPtr Return)
        {
            HookReturnAddress = Return;

            if (WaitingConfirmation)
            {
                UninstallDynamicHook();

                WaitingConfirmation = false;
                var Rst = MessageBox.Show("And then, you saw the confirmation message?", "StringReloader Setup Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Rst == DialogResult.Yes)                
                    return;
                
                PostSetupMode = true;
            }

            if (!PostSetupMode)
                return;

            UninstallDynamicHook();

            if (Verbose)
                Log("SoftPAL Recived Stack: 0x{0:X8} (Return to: 0x{1:X8})", true, Stack.ToUInt32(), Return.ToInt32());

            if (!PostSetupInstructionViewed)
                MessageBox.Show("SRL will now need your help to confirm if the dialogue has been found.\nThe program will display the game dialogue; when the correct dialogue is shown, press YES. If nothing is shown or if you see corrupted text, press NO.\nTake note that if you don't set the proper game encoding inside SRL.ini, the correct dialogue will most likely never be shown.\nPress OK if you have read and understood the above.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);


            PostSetupInstructionViewed = true;

            int Offset = LastOffset;
            bool Loop = true;
            while (Loop)
            {
                int StackValue = Marshal.ReadInt32(Stack, (++Offset) * 4);
                IntPtr StrAddress = new IntPtr(StackValue);

                try
                {
                    if (IsBadCodePtr(StrAddress))
                    {
                        if (Verbose)
                            Log("Bad Pointer: 0x{0:X8}", true, StrAddress.ToUInt32());
                        continue;
                    }

                    if (Verbose)
                        Log("Guessing Offset: 0x{0:X8} (+{1}) (0x{2:X8})", true, Stack.Sum(Offset * 4).ToUInt32(), Offset, StrAddress.ToUInt32());

                    string Str = GetString(StrAddress, MaxLen: 501, Internal: true);
                    if (Str?.Length > 0 && Str?.Length < 500)
                    {
                        var Reply = MessageBox.Show(Str, "Is this the dialogue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        switch (Reply)
                        {
                            case DialogResult.Yes:
                                byte[] Buffer = WriteEncoding.GetBytes($"Looks Like everything is working,{GameLineBreaker}Continue to the next game dialogue!\x0");

                                IntPtr NewStr = Marshal.AllocHGlobal(Buffer.Length);
                                Marshal.Copy(Buffer, 0, NewStr, Buffer.Length);
                                Marshal.WriteInt32(Stack, Offset * 4, NewStr.ToInt32());

                                Settings.StackOffset = (uint)Offset;
                                LastOffset = Offset;

                                PostSetupMode = false;
                                WaitingConfirmation = true;
                                return;

                            case DialogResult.No:
                                break;
                            default:
                                Environment.Exit(0);
                                break;
                        }
                    }
                }
                catch { }
            }

        }

        IntPtr PostSetupEndHook()
        {
            if (PostSetupMode)
            {
                InstallDynamicHook();
                return HookReturnAddress;
            }

            if (WaitingConfirmation)
            {
                InstallDynamicHook();
                MessageBox.Show("Very well, SRL will now try translating the game text to test settings.\nIf you don't see a message in-game confirming what has been set, then continue on to the next in-game dialogue and press NO; otherwise, press YES.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return HookReturnAddress;
            }

            AdvancedIni.FastSave(Settings, IniPath);
            MessageBox.Show($"Perfect! SRL is now ready to use! Enjoy!", "StringReloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
            return HookReturnAddress;
        }

        IntPtr HookReturnAddress;
        HookBegin HookBeginDelegate;
        HookEnd HookEndDelegate;

        bool BufferEquals(byte[] Buffer, byte[] Data)
        {
            if (Buffer.Length < Data.Length)
                return true;
            for (int i = 0; i < Data.Length; i++)
                if (Buffer[i] != Data[i])
                    return false;
            return true;
        }

        byte[] SetupInfo { 
            get {
                CallInfoDelegate = new InfoCatcher(CallingInfoReciver);
                IntPtr Func = Marshal.GetFunctionPointerForDelegate(CallInfoDelegate);
                
                byte[] Buff = new byte[SetupInfoBase.Length];
                SetupInfoBase.CopyTo(Buff, 0);

                BitConverter.GetBytes(Func.ToUInt32()).CopyTo(Buff, 0x07);
                BitConverter.GetBytes(RealFunc.ToUInt32()).CopyTo(Buff, 0x0F);

                return Buff;
            }        
        }

        //+7 = Info Catcher; +f = DrawText
        byte[] SetupInfoBase = { 0x89, 0xE0, 0x60, 0x50, 0xFF, 0x30, 0xB8, 0x11, 0x11, 0x11, 0x11, 
                                 0xFF, 0xD0, 0x61, 0xB8, 0x22, 0x22, 0x22, 0x22, 0xFF, 0xE0 };


        byte[] NonPersistentHook { 
            get {
                HookBeginDelegate = PostSetupMode ? new HookBegin(PostSetupBeginHook) : new HookBegin(BeginHook);
                HookEndDelegate = PostSetupMode ? new HookEnd(PostSetupEndHook) : new HookEnd(EndHook);

                IntPtr BeginAddr = Marshal.GetFunctionPointerForDelegate(HookBeginDelegate);
                IntPtr EndAddr = Marshal.GetFunctionPointerForDelegate(HookEndDelegate);

                byte[] Buff = new byte[NonPersistentHookBase.Length];
                NonPersistentHookBase.CopyTo(Buff, 0);

                BitConverter.GetBytes(Settings.StackOffset * 4u).CopyTo(Buff, 0x07);

                BitConverter.GetBytes(BeginAddr.ToUInt32()).CopyTo(Buff, 0x0D);
                BitConverter.GetBytes(DynamicRealAddress.ToUInt32()).CopyTo(Buff, 0x16);
                BitConverter.GetBytes(EndAddr.ToUInt32()).CopyTo(Buff, 0x20);

                return Buff;
            }
        }


        //+7 = Stack Offset; +D = Hook Begin Address; +16 = Real Func Address; +20 = Hook End Address
        byte[] NonPersistentHookBase = { 0x60, 0x89, 0xE0, 0xFF, 0x70, 0x20, 0x05, 0x00, 0x01, 0x00, 0x00,
                                         0x50, 0xB8, 0x11, 0x11, 0x11, 0x11, 0xFF, 0xD0, 0x61, 0x58, 0xB8,
                                         0x22, 0x22, 0x22, 0x22, 0xFF, 0xD0, 0x6A, 0x00, 0x60, 0xB8, 0x33,
                                         0x33, 0x33, 0x33, 0xFF, 0xD0, 0x89, 0x44, 0x24, 0x20, 0x61, 0xC3 };

        byte[] PersistentHook {
            get {
                if (PostSetupMode)
                    return NonPersistentHook;

                List<byte> Buffer = new List<byte>();
                Buffer.AddRange(PersistentHookBase);
                Buffer.AddRange(UnmanagedImports.Read(DynamicRealAddress, Settings.PersistentHookSize));
                Buffer.AddRange(UnmanagedImports.AssembleJump(DynamicHookAddress.Sum(Buffer.Count), DynamicRealAddress.Sum(Settings.PersistentHookSize)));

                byte[] Buff = Buffer.ToArray();
                BitConverter.GetBytes(Settings.StackOffset * 4u).CopyTo(Buff, 0x04);
                BitConverter.GetBytes(GetDirectProcessReal().ToUInt32()).CopyTo(Buff, 0x0E);

                return Buff;
            }
        }

        //+4 = Stack Offset; +E = Process Address;
        byte[] PersistentHookBase = { 0x60, 0x89, 0xE0, 0xBB, 0x23, 0x01, 0x00, 0x00, 0x01, 0xD8, 0x50, 0xFF,
                                      0x30, 0xB8, 0x11, 0x11, 0x11, 0x11, 0xFF, 0xD0, 0x5B, 0x89, 0x03, 0x61 };


        byte[] SupportedFuncPrefixA = { 0x55, 0x8B, 0xEC, 0x83, 0xEC };
        byte[] SupportedFuncPrefixB = { 0x55, 0x8B, 0xEC, 0x83, 0xE4 };
        byte[] UnsupportedFuncPrefix = { 0xCC, 0xCC, 0x55, 0x8B, 0xEC };
    }
}
