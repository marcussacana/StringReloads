using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Iced.Intel;
using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Engine.Unmanaged;
using StringReloads.Hook.Base;
using StringReloads.Hook.Others;

namespace StringReloads.AutoInstall
{
    /*
         How to found the GetStr Function Manually
         1 - Find for the function that call the ShellExecuteA
         2 - Scroll up (Ignore the Indirect Call)
         3 - You will found 3 calls of a same function, this function is our GetStr
     */
    unsafe class CMVS : IAutoInstall
    {
        ulong? Offset = null;
        CMVS_GetText Hook;

        public CMVS()
        {
            var CFG = Config.Default.GetValues("CMVS");
            if (CFG == null)
                return;

            var Size = CFG["filesize"].ToInt64();

            if (Size != new FileInfo(Config.Default.GameExePath).Length)
                return;

            Offset = CFG["offset"].ToUInt64();
        }

        public string Name => $"CMVS{(Environment.Is64BitProcess ? 64 : 32)}";

        public void Install()
        {
            if (Offset == null)
            {
                SearchOffset();

                if (Offset == null)
                    return;

                EnsureMultiArch();

                var Dic = new Dictionary<string, string>();
                Dic["FileSize"] = new FileInfo(Config.Default.GameExePath).Length.ToString();
                Dic["Offset"] = Offset.Value.ToString();

                Config.Default.SetValues("CMVS", Dic);
                Config.Default.SaveSettings();
            }

            if (Hook == null)
                Hook = new CMVS_GetText((void*)((ulong)Config.GameBaseAddress + Offset.Value));

            Hook.Install();
            
            /*// Read all files from directory (Must extract all packages before)
            foreach (var Match in EntryPoint.SRL.Matchs)
            {
                if (Match is not RegexMatch)
                    continue;
                
                RegexMatch RMatch = Match as RegexMatch;
                RMatch.AddRegex(@"([A-z\0-9\\\/]*)\.cpz", "$1");
            }
            */
        }

        private void EnsureMultiArch()
        {
            if (EntryPoint.CurrentDll.GetFilename().ToLowerInvariant().StartsWith("srlx"))
                return;

            Log.Debug("CMVS Multiarch Patch Not Applied.");

            var CurrentExe = Config.Default.GameExePath;
            var AltExe = Path.Combine(Path.GetDirectoryName(Config.Default.GameExePath), Environment.Is64BitProcess ? "cmvs32.exe" : "cmvs64.exe");

            var CurrentSRL = Environment.Is64BitProcess ? "SRLx64.dll" : "SRLx32.dll";
            var AltSRL = Environment.Is64BitProcess ? "SRLx32.dll" : "SRLx64.dll";

            if (!File.Exists(AltExe))
                Log.Warning($"Failed to Find the {Path.GetFileName(AltExe)}.");

            bool AltSRLAvailable = File.Exists(Path.Combine(Path.GetDirectoryName(CurrentExe), AltSRL));

            if (!AltSRLAvailable)
                Log.Warning($"Failed to Find the {AltSRL}.");
            else
                Patcher.Tools.ThirdPartyApplyPatch(AltExe, AltSRL);

            Patcher.Tools.ApplyWrapperPatch(CurrentSRL);
        }

        void SearchOffset()
        {
            var Imports = ModuleInfo.GetModuleImports((byte*)Config.GameBaseAddress);
            var ShellExec = Imports.Where(x => x.Function == "ShellExecuteA").Single();

            var Bitness = Environment.Is64BitProcess ? 64 : 32;

            ulong? Address = null;
            byte?[] Pattern = new byte?[] { 0xFF, 0x15 };
            foreach (var lAddress in Scan(Pattern))
            {
                Decoder Dissassembler = Decoder.Create(Bitness, new MemoryCodeReader((byte*)lAddress));
                Dissassembler.IP = (ulong)lAddress;
                var Call = Dissassembler.PeekDecode();
                var MemAddress = Call.IPRelativeMemoryAddress;
                //if (Environment.Is64BitProcess)
                //    MemAddress = Call.IPRelativeMemoryAddress + (ulong)lAddress + 6ul;

                if (MemAddress == (ulong)ShellExec.ImportAddress)
                {
                    Log.Debug($"Call dword ptr ds: [&ShellExecuteA] - Found at 0x{lAddress:X16}");
                    Pattern = new byte?[] { 0xE8 };
                    foreach (var lgetTextAddress in Scan(Pattern, (ulong)lAddress, true))
                    {
                        Dissassembler = Decoder.Create(Bitness, new MemoryCodeReader((byte*)lgetTextAddress));
                        Dissassembler.IP = (ulong)lgetTextAddress;
                        Call = Dissassembler.PeekDecode();

                        var Immediate = Environment.Is64BitProcess ? Call.MemoryDisplacement64 : Call.MemoryDisplacement32;

                        if (Address != Immediate)
                        {
                            Address = Immediate;
                            continue;
                        } 
                        else
                            break;
                    }
                    break;
                }
            }

            if (Address == null)
            {
                Log.Error("Failed to find the Game injection point");
                return;
            }

            Offset = ((ulong)Address) - (ulong)Config.GameBaseAddress;
            Log.Debug($"CMVS Injection Offset Found: 0x{Offset:X16}");
        }

        private IEnumerable<long> Scan(byte?[] Pattern, ulong? BeginAddress = null, bool Up = false)
        {
            ulong? Match = BeginAddress;
            do
            {
                if (Match != null) {
                    
                    if (Up)
                        Match--;
                    else
                        Match++;

                }

                Match =  Up ? ScanUp(Pattern, Match ?? ulong.MaxValue) : ScanDown(Pattern, Match ?? 0ul);

                if (Match != null)
                    yield return (long)Match;

            } while (Match != null);
        }

        private unsafe ulong? ScanUp(byte?[] Pattern, ulong BeginAddress = ulong.MaxValue)
        {
            var Info = ModuleInfo.GetCodeInfo((byte*)Config.GameBaseAddress);

            if (BeginAddress == ulong.MaxValue)
                BeginAddress = (ulong)Info.CodeAddress + Info.CodeSize;

            ulong CodeAdd = (ulong)Info.CodeAddress;
            for (ulong i = BeginAddress - CodeAdd; i >= 0; i--)
            {
                byte* pAddress = (byte*)(CodeAdd + i);
                if (!CheckPattern(pAddress, Pattern))
                    continue;

                return (ulong)pAddress;
            }

            return null;
        }

        private unsafe ulong? ScanDown(byte?[] Pattern, ulong BeginAddress = 0)
        {
            var Info = ModuleInfo.GetCodeInfo((byte*)Config.GameBaseAddress);

            ulong CodeAdd = (ulong)Info.CodeAddress;
            ulong CodeLen = Info.CodeSize;

            if (BeginAddress != 0)
                BeginAddress = BeginAddress - CodeAdd;

            for (ulong i = BeginAddress; i < CodeLen; i++)
            {
                byte* pAddress = (byte*)(CodeAdd + i);
                if (!CheckPattern(pAddress, Pattern))
                    continue;

                return (ulong)pAddress;
            }

            return null;
        }

        private bool CheckPattern(byte* Buffer, byte?[] Pattern)
        {
            for (int i = 0; i < Pattern.Length; i++)
            {
                if (Pattern[i] == null)
                    continue;
                byte bPattern = Pattern[i].Value;
                if (bPattern != Buffer[i])
                    return false;
            }
            return true;
        }

        public bool IsCompatible() => Config.Default.GameExePath.GetFilenameNoExt().ToLowerInvariant().StartsWith("cmvs");

        public void Uninstall()
        {
            Hook?.Uninstall();
        }
    }
}
