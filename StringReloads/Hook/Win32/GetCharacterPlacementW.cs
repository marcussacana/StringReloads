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
    unsafe class GetCharacterPlacementW : Hook<GetCharacterPlacementWDelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "GetCharacterPlacementW";

        public override void Initialize()
        {
            HookDelegate = hGetCharacterPlacement;
            Compile();
        }

        uint hGetCharacterPlacement(void* hdc, byte* lpString, int nCount, int nMexExtent, GCP_RESULTSW* lpResult, uint dwFlags)
        {
            WCString InStr = lpString;
            InStr.FixedLength = (uint)nCount;

            InStr = EntryPoint.ProcessW((WCString)(string)InStr);//Ensure Null-Terminated

            if (Config.Default.GetCharacterPlacementWUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.GetCharacterPlacementWRemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return Bypass(hdc, InStr, InStr.Count(), nMexExtent, lpResult, dwFlags);
        }
    }
}
