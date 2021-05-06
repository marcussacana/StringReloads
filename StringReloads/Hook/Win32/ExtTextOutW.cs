using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System.Linq;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Win32
{
    unsafe class ExtTextOutW : Hook<ExtTextOutWDelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "ExtTextOutW";

        public override void Initialize()
        {
            HookDelegate = new ExtTextOutWDelegate(ExtTextOut);
            Compile();
        }

        bool ExtTextOut(void* hdc, int x, int y, uint options, void* lprect, byte* lpString, uint c, int* lpDx)
        {
            WCString OriStr = lpString;
            OriStr.FixedLength = c;

            WCString InStr = EntryPoint.ProcessW((WCString)(string)OriStr);

            if (Config.Default.ExtTextOutWUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.ExtTextOutWRemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return Bypass(hdc, x, y , options, lprect, InStr, (uint)InStr.LongCount()/2, lpDx);
        }
    }
}
