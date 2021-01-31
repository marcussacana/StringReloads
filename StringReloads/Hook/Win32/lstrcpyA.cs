using System;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;

namespace StringReloads.Hook.Win32
{
    unsafe class lstrcpyA : Hook<lstrcpyDelegate>
    {
        public lstrcpyA()
        {
            HookDelegate = hlstrcpyA;
            Compile();
        }
        public lstrcpyA(void* TargetModule) : this (new IntPtr(TargetModule)) { }
        public lstrcpyA(IntPtr TargetModule)
        {
            HookDelegate = hlstrcpyA;
            Compile(true, TargetModule);
        }
        public override string Library => "kernel32.dll";

        public override string Export => "lstrcpyA";

        public override void Initialize() { }

        private unsafe byte* hlstrcpyA(byte* lpString1, byte* lpString2)
        {
            if (!EntryPoint.SRL.Initialized)
                return Bypass(lpString1, lpString2);

            lpString2 = EntryPoint.SRL.ProcessString((CString)lpString2);
            return Bypass(lpString1, lpString2);
        }
    }
}
