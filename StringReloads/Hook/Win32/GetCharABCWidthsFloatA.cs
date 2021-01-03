using StringReloads.Engine;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System.Linq;

namespace StringReloads.Hook.Win32
{
    public unsafe class GetCharABCWidthsFloatA : Hook<GetCharABCWidthsFloatADelegate>
    {

        public override string Library => "gdi32.dll";

        public override string Export => "GetCharABCWidthsFloatA";

        public override void Initialize()
        {
            HookDelegate = hGetCharABCWidthsFloat;
            Compile();
        }

        private unsafe bool hGetCharABCWidthsFloat(void* hdc, uint iFirst, uint iLast, ABCFLOAT* lpABC)
        {
            uint Diff = iLast - iFirst;

            iFirst = (uint)EntryPoint.Process((void*)iFirst);

            if (Config.Default.GetCharABCWidthsFloatAUndoRemap)
                iFirst = Remaper.Default.Restore(((char)iFirst).ToString()).First();

            if (Config.Default.GetCharABCWidthsFloatARemapAlt)
                iFirst = RemaperAlt.Default.Apply(((char)iFirst).ToString(), null).First();

            iLast = iFirst + Diff;
            return Bypass(hdc, iFirst, iLast, lpABC);
        }
    }
}
