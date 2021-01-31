using System;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;

namespace StringReloads.Hook.Win32
{
    unsafe class lstrcpyW : Hook<lstrcpyDelegate>
    {
        public lstrcpyW()
        {
            HookDelegate = hlstrcpyW;
            Compile();
        }
        public lstrcpyW(void* TargetModule) : this(new IntPtr(TargetModule)) { }
        public lstrcpyW(IntPtr TargetModule)
        {
            HookDelegate = hlstrcpyW;
            Compile(true, TargetModule);
        }
        public override string Library => "kernel32.dll";

        public override string Export => "lstrcpyA";

        public override void Initialize() { }

        private unsafe byte* hlstrcpyW(byte* lpString1, byte* lpString2)
        {
            if (!EntryPoint.SRL.Initialized)
                return Bypass(lpString1, lpString2);

            lpString2 = EntryPoint.SRL.ProcessString((WCString)lpString2);
            return Bypass(lpString1, lpString2);
        }
    }
}
