using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System.Linq;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Win32
{
    unsafe class ExtTextOutA : Hook<ExtTextOutADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "ExtTextOutA";

        public override void Initialize()
        {
            HookDelegate = new ExtTextOutADelegate(ExtTextOut);
            Compile();
        }

        bool ExtTextOut(void* hdc, int x, int y, uint options, void* lprect, byte* lpString, uint c, int* lpDx)
        {
            CString OriStr = lpString;
            OriStr.FixedLength = c;

            WCString InStr = EntryPoint.ProcessW((WCString)(string)OriStr);

            if (Config.Default.ExtTextOutAUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.ExtTextOutARemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return ExtTextOutW(hdc, x, y , options, lprect, InStr, (uint)InStr.LongCount()/2, lpDx);
        }

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        extern static bool ExtTextOutW(void* hdc, int x, int y, uint options, void* lprect, byte* lpString, uint c, int* lpDx);
    }
}
