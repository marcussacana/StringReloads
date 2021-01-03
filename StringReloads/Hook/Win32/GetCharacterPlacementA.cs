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
    unsafe class GetCharacterPlacementA : Hook<GetCharacterPlacementADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "GetCharacterPlacementA";

        public override void Initialize()
        {
            HookDelegate = hGetCharacterPlacement;
            Compile();
        }

        uint hGetCharacterPlacement(void* hdc, byte* lpString, int nCount, int nMexExtent, GCP_RESULTSA* lpResult, uint dwFlags)
        {
            CString InStr = lpString;
            InStr.FixedLength = (uint)nCount;

            InStr = EntryPoint.Process((CString)(string)InStr);//Ensure Null-Terminated

            if (Config.Default.GetCharacterPlacementAUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.GetCharacterPlacementARemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return Bypass(hdc, InStr, InStr.Count(), nMexExtent, lpResult, dwFlags);
        }
    }
}
