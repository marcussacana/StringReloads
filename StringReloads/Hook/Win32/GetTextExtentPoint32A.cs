using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.Hook.Win32
{
    unsafe class GetTextExtentPoint32A : Hook<GetTextExtentPoint32ADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "GetTextExtentPoint32A";

        public override void Initialize()
        {
            HookDelegate = hGetTextExtendPoint32;
            Compile();
        }

        private unsafe bool hGetTextExtendPoint32(void* hdc, byte* lpString, int c, SIZE* psize)
        {
            CString InStr = lpString;
            InStr.FixedLength = (uint)c;

            InStr = EntryPoint.Process((CString)(string)InStr);//Ensure Null-Terminated

            if (Config.Default.GetTextExtentPoint32AUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.GetTextExtentPoint32ARemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return Bypass(hdc, InStr, InStr.Count(), psize);
        }
    }
}
