using StringReloads.Engine.String;

namespace StringReloads.Hook
{
    public unsafe class CreateFontA : Base.Hook<CreateFontADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "CreateFontA";

#if x64
        public override bool ProtectRAX => true;
#endif

        public override void Initialize()
        {
            HookDelegate = new CreateFontADelegate(CreateFontHook);
            Compile();
        }

        void* CreateFontHook(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, int fdwItalic, int fdwUnderline, int fdwStrikeOut, uint fdwCharSet, int fdwOutputPrecision, int fdwClipPrecision, int fdwQuality, int fdwPitchAndFamily, byte* lpszFace) {
            var Remap = EntryPoint.SRL.ResolveRemap((CString)lpszFace, nWidth, nHeight, fdwCharSet);

            if (Remap != null) {
                nHeight = Remap.Value.Height;
                nWidth = Remap.Value.Width;
                lpszFace = (CString)Remap.Value.To;
                fdwCharSet = Remap.Value.Charset;
            }

            return Bypass(nHeight, nWidth, nEscapement, nOrientation, fnWeight, fdwItalic, fdwUnderline, fdwStrikeOut, fdwCharSet, fdwOutputPrecision, fdwClipPrecision, fdwQuality, fdwPitchAndFamily, lpszFace);
        }
    }
}
