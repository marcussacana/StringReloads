using System;
using System.Collections;
using System.Text;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;

namespace StringReloads.Hook.Others
{
    unsafe class ExHIBIT_lstrcpyA : Hook<lstrcpyDelegate>
    {
        public ExHIBIT_lstrcpyA()
        {
            HookDelegate = hlstrcpyA;
            Compile();
        }
        public ExHIBIT_lstrcpyA(void* TargetModule) : this (new IntPtr(TargetModule)) { }
        public ExHIBIT_lstrcpyA(IntPtr TargetModule)
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

            string Str = (CString)lpString2;

            if (Minify(Str).Length == 0)
                return Bypass(lpString1, lpString2);

            lpString2 = EntryPoint.SRL.ProcessString((CString)lpString2);
            return Bypass(lpString1, lpString2);
        }

        private string Minify(string Input) {
            StringBuilder Builder = new StringBuilder();
            foreach (var Char in Input) {
                if (Char >= '0' && Char <= '9')
                    continue;
                if (Char >= ',' && Char <= '.')
                    continue;
                if (Char == ';' || char.IsWhiteSpace(Char))
                    continue;
                Builder.Append(Char);
            }
            return Builder.ToString().Replace("NULL", "").Trim();
        }
    }
}
