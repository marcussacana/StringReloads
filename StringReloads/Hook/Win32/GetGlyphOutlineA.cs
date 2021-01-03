using StringReloads.Engine;
using StringReloads.StringModifier;
using System.Linq;

namespace StringReloads.Hook
{
    public unsafe class GetGlyphOutlineA : Base.Hook<GetGlyphOutlineADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "GetGlyphOutlineA";

        public override void Initialize()
        {
            HookDelegate = new GetGlyphOutlineADelegate(GetGlyphOutline);
            Compile();
        }

        private uint GetGlyphOutline(void* hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, byte* lpvBuffer, ref MAT2 lpmat2)
        {
            uChar = (uint)EntryPoint.Process((void*)uChar);

            if (Config.Default.GetGlyphOutlineAUndoRemap)
                uChar = Remaper.Default.Restore(((char)uChar).ToString()).First();

            if (Config.Default.GetGlyphOutlineARemapAlt)
                uChar = RemaperAlt.Default.Apply(((char)uChar).ToString(), null).First();

            return Bypass(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);
        }
    }
}
