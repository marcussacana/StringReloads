namespace StringReloads.Hook
{
    unsafe class CreateFontIndirectA : Base.Hook<CreateFontIndirectADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "CreateFontIndirectA";

        public override void Initialize()
        {
            HookDelegate = new CreateFontIndirectADelegate(CreateFontIndirect);
            Compile();
        }

        public void* CreateFontIndirect(ref LOGFONTA Info) {
            var Remap = EntryPoint.SRL.ResolveRemap(Info.lfFaceName, Info.lfWidth, Info.lfHeight, Info.lfCharSet);

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
