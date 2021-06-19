using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System.Linq;

namespace StringReloads.Hook.Win32
{
    unsafe class GetTextExtentPoint32W : Hook<GetTextExtentPoint32WDelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "GetTextExtentPoint32W";

        public override void Initialize()
        {
            HookDelegate = hGetTextExtendPoint32;
            Compile();
        }

        private unsafe bool hGetTextExtendPoint32(void* hdc, byte* lpString, int c, SIZE* psize)
        {
            WCString InStr = lpString;
            InStr.FixedLength = (uint)c;

            InStr = EntryPoint.ProcessW((WCString)(string)InStr);//Ensure Null-Terminated

            if (Config.Default.GetTextExtentPoint32WUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.GetTextExtentPoint32WRemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return Bypass(hdc, InStr, InStr.Count(), psize);
        }
    }
}
