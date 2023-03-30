using StringReloads.Engine.String;

namespace StringReloads.Hook
{
    unsafe class SoftPal_PalSpriteCreateText : Base.Hook<SoftPAL_PalSpriteCreateTextDelegate>
    {
        public override string Library => "pal.dll";

        public override string Export => "PalSpriteCreateText";

        public override void Initialize()
        {
            HookDelegate = new SoftPAL_PalSpriteCreateTextDelegate(PalSpriteCreateText);
            Compile();
        }

        void* PalSpriteCreateText(int a1, byte* Text, int* a3, int* a4)
        {
            if (a1 != 0 && Text != null)
            {
                Text = (byte*)EntryPoint.Process(Text);
            }
            return Bypass(a1, Text, a3, a4);
        }
    }
}
