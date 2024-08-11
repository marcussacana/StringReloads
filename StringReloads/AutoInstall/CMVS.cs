using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Iced.Intel;
using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Engine.Unmanaged;
using StringReloads.Hook.Base;
using StringReloads.Hook.Others;
using StringReloads.Tools;

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
            var CFG = Config.Default.GetValues(Name);
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

                Config.Default.SetValues(Name, Dic);
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
                Patcher.ExeTools.ThirdPartyApplyPatch(AltExe, AltSRL);

            Patcher.ExeTools.ApplyWrapperPatch(CurrentSRL);
        }

        void SearchOffset()
        {
            ulong? Address = null;
            var Bitness = Environment.Is64BitProcess ? 64 : 32;
            foreach (var lAddress in Scanner.SearchExportCall("ShellExecuteA"))
            {
                var Pattern = new byte?[] { 0xE8 };
                foreach (var lgetTextAddress in Scanner.Scan(Pattern, (ulong)lAddress, true))
                {
                    var Dissassembler = Decoder.Create(Bitness, new MemoryCodeReader((byte*)lgetTextAddress));
                    Dissassembler.IP = (ulong)lgetTextAddress;
                    var Call = Dissassembler.PeekDecode();

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

            if (Address == null)
            {
                Log.Error("Failed to find the Game injection point");
                return;
            }

            Offset = ((ulong)Address) - (ulong)Config.GameBaseAddress;
            Log.Debug($"CMVS Injection Offset Found: 0x{Offset:X16}");
        }


        public bool IsCompatible() => Config.Default.GameExePath.GetFilenameNoExt().ToLowerInvariant().StartsWith("cmvs");

        public void Uninstall()
        {
            Hook?.Uninstall();
        }
    }
}
