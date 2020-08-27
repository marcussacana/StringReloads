using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Engine.Unmanaged;
using StringReloads.Hook.Others;
using System;
using System.Collections.Generic;
using System.IO;

namespace StringReloads.AutoInstall
{
    unsafe class CMVS32 : IAutoInstall
    {
        uint? Offset = null;
        CMVS32_GetText Hook;

        public CMVS32()
        {
            var CFG = Config.Default.GetValues("CMVS");
            if (CFG == null)
                return;

            var Size = CFG["FileSize"].ToInt64();

            if (Size != new FileInfo(Config.Default.GameExePath).Length)
                return;

            Offset = CFG["Offset"].ToUInt32();
        }

        public string Name => "CMVS32";

        public void Install()
        {
            if (Offset == null)
            {
                SearchOffset();
                
                if (Offset == null)
                    return;

                var Dic = new Dictionary<string, string>();
                Dic["FileSize"] = new FileInfo(Config.Default.GameExePath).Length.ToString();
                Dic["Offset"] = Offset.Value.ToString();

                Config.Default.SetValues("CMVS", Dic);
            }

            if (Hook == null) 
                Hook = new CMVS32_GetText((void*)((uint)Config.Default.GameBaseAddress + Offset.Value));

            Hook.Install();
        }

        byte?[] Pattern => new byte?[] {
            0x25, 0x00, 0x00, 0x00, 0xC0, //and eax, 0xC0000000
            0x3D, 0x00, 0x00, 0x00, 0x80, //cmp eax, 0x80000000
            0x77, null,                   //ja ??
            0x74, null                    //je ??
        };

        void SearchOffset()
        {
            if (!Scan(out byte* Address))
            {
                Log.Error("Failed to find the Game injection point");
                return;
            }

            uint Prefix = 0xEC8B55CC;
            while ((*(uint*)Address) != Prefix)
                Address--;

            Address++;

            Offset = ((uint)Address) - (uint)Config.Default.GameBaseAddress;
            Log.Debug($"CMVS32 Injection Offset Found: 0x{Offset:X8}");
        }

        private bool Scan(out byte* Address)
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
                Log.Debug($"CMVS32 Pattern Found At: 0x{(ulong)pBuffer:X8}");
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

        public bool IsCompatible() => Config.Default.GameExePath.GetFilenameNoExt().ToLowerInvariant() == "cmvs32";

        public void Uninstall()
        {
            Hook?.Uninstall();
        }
    }
}
