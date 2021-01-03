using StringReloads.Engine;
using StringReloads.StringModifier;
using System.Linq;

namespace StringReloads.Hook
{
    public unsafe class GetGlyphOutlineW : Base.Hook<GetGlyphOutlineWDelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "GetGlyphOutlineW";

        public override void Initialize()
        {
            HookDelegate = GetGlyphOutline;
            Compile();
        }

        private uint GetGlyphOutline(void* hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, byte* lpvBuffer, ref MAT2 lpmat2)
        {
            uChar = (uint)EntryPoint.ProcessW((void*)uChar);

            if (Config.Default.GetGlyphOutlineWUndoRemap)
                uChar = Remaper.Default.Restore(((char)uChar).ToString()).First();

            if (Config.Default.GetGlyphOutlineWRemapAlt)
                uChar = RemaperAlt.Default.Apply(((char)uChar).ToString(), null).First();

            return Bypass(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);
        }
    }
}
