namespace StringReloads.Hook
{
    unsafe class CreateFontIndirectW : Base.Hook<CreateFontIndirectWDelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "CreateFontIndirectW";

        public override void Initialize()
        {
            HookDelegate = new CreateFontIndirectWDelegate(CreateFontIndirect);
            Compile();
        }

        public void* CreateFontIndirect(ref LOGFONTW Info) {
            var Remap = EntryPoint.SRL.Match.ResolveRemap(Info.lfFaceName, Info.lfWidth, Info.lfHeight, Info.lfCharSet);

            if (Remap != null) {
                Info.lfHeight = Remap.Value.Height;
                Info.lfWidth = Remap.Value.Width;
                Info.lfFaceName = Remap.Value.To;
                Info.lfCharSet = (byte)Remap.Value.Charset;
            }

            return Bypass(ref Info);
        }
    }
}
