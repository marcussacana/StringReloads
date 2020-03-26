namespace StringReloads.Hook
{
    unsafe class SoftPal_DrawText : Base.Hook<SoftPAL_DrawTextDelegate>
    {
        public override string Library => "pal.dll";

        public override string Export => "DrawText";

        public override void Initialize()
        {
            HookDelegate = new SoftPAL_DrawTextDelegate(DrawText);
            Compile();
        }

        void DrawText(byte* Text, void* a2, void* a3, void* a4, void* a5, void* a6, void* a7, void* a8, void* a9, void* a10, void* a11, void* a12, void* a13, void* a14, void* a15, void* a16, void* a17, void* a18, void* a19, void* a20, void* a21, void* a22, void* a23) {
            Text = (byte*)EntryPoint.Process(Text);
            DrawText(Text, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18, a19, a20, a21, a22, a23);
        }
    }
}
