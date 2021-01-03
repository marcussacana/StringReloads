using StringReloads.Engine;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System.Linq;

namespace StringReloads.Hook.Win32
{
    public unsafe class GetCharABCWidthsFloatW : Hook<GetCharABCWidthsFloatWDelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "GetCharABCWidthsFloatW";

        public override void Initialize()
        {
            HookDelegate = hGetCharABCWidthsFloat;
            Compile();
        }

        private unsafe bool hGetCharABCWidthsFloat(void* hdc, uint iFirst, uint iLast, ABCFLOAT* lpABC)
        {
            uint Diff = iLast - iFirst;
            iFirst = (uint)EntryPoint.ProcessW((void*)iFirst);

            if (Config.Default.GetCharABCWidthsFloatWUndoRemap)
                iFirst = Remaper.Default.Restore(((char)iFirst).ToString()).First();

            if (Config.Default.GetCharABCWidthsFloatWRemapAlt)
                iFirst = RemaperAlt.Default.Apply(((char)iFirst).ToString(), null).First();

            iLast = iFirst + Diff;
            return Bypass(hdc, iFirst, iLast, lpABC);
        }
    }
}
