using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Engine.Unmanaged;
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
                Hook = new CMVS_GetText((void*)((ulong)Config.Default.GameBaseAddress + Offset.Value));

            Hook.Install();
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

        byte?[][] Patterns => new[] {
            //x64
            new byte?[] {
                0x4C, null, null,             //mov ???, ???
                0x25, 0x00, 0x00, 0x00, 0xC0, //and eax, 0xC0000000
                0x74, null,                   //je ??
                0x3D, 0x00, 0x00, 0x00, 0x40, //cmp eax, 0x40000000
                0x74, null                    //je ??
            },
            //x32
            new byte?[] {
                0x25, 0x00, 0x00, 0x00, 0xC0, //and eax, 0xC0000000
                0x3D, 0x00, 0x00, 0x00, 0x80, //cmp eax, 0x80000000
                0x77, null,                   //ja ??
                0x74, null                    //je ??
            }
        };

        void SearchOffset()
        {
            byte* Address = null;
            foreach (var Pattern in Patterns)
            {
                if (Scan(out Address, Pattern))
                    break;
                Address = null;
            }

            if (Address == null)
            {
                Log.Error("Failed to find the Game injection point");
                return;
            }

            uint[] Prefixes = new uint[] { 0x4CC28BCC, 0xEC8B55CC };

            while (!Prefixes.Contains((*(uint*)Address)))
                Address--;

            Address++;//Skip int3

            Offset = ((ulong)Address) - (ulong)Config.Default.GameBaseAddress;
            Log.Debug($"CMVS Injection Offset Found: 0x{Offset:X16}");
        }

        private bool Scan(out byte* Address, byte?[] Pattern)
        {
            var Info = ModuleInfo.GetCodeInfo((byte*)Config.Default.GameBaseAddress);

            Address = null;
            long CodeAdd = (long)Info.CodeAddress;
            long CodeLen = Info.CodeSize - Pattern.Length;
            for (int i = 0; i < CodeLen; i++)
            {
                byte* pBuffer = (byte*)(Info.CodeAddress) + i;
                if (!CheckPattern(pBuffer, Pattern))
                    continue;
                Log.Debug($"CMVS Pattern Found At: 0x{(ulong)pBuffer:X16}");
                for (long x = (long)pBuffer; x > CodeAdd; x--)
                {
                    byte* pFunc = (byte*)x;
                    if (!CheckPattern(pFunc, Pattern))
                        continue;
                    Address = pFunc;
                    return true;
                }
            }
            return false;
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
