namespace StringReloads.Hook
{
    public unsafe class CreateFontA : Base.Hook<CreateFontADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "CreateFontA";

        public override void Initialize()
        {
            HookDelegate = new CreateFontADelegate(CreateFontHook);
            Compile();
        }

        void* CreateFontHook(uint nHeight, uint nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace) {
            var Remap = EntryPoint.SRL.GetFontRemap(lpszFace, nWidth, nHeight, fdwCharSet);

            if (Remap != null) {
                nHeight = Remap.Value.Height;
                nWidth = Remap.Value.Width;
                lpszFace = Remap.Value.To;
                fdwCharSet = Remap.Value.Charset;
            }

            return Bypass(nHeight, nWidth, nEscapement, nOrientation, fnWeight, fdwItalic, fdwUnderline, fdwStrikeOut, fdwCharSet, fdwOutputPrecision, fdwClipPrecision, fdwQuality, fdwPitchAndFamily, lpszFace);
        }
    }
}
