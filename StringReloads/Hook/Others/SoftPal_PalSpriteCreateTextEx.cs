using StringReloads.Engine.String;

namespace StringReloads.Hook
{
    unsafe class SoftPal_PalSpriteCreateTextEx : Base.Hook<SoftPAL_PalSpriteCreateTextExDelegate>
    {
        public override string Library => "pal.dll";

        public override string Export => "PalSpriteCreateTextEx";

        public override void Initialize()
        {
            HookDelegate = new SoftPAL_PalSpriteCreateTextExDelegate(PalSpriteCreateText);
            Compile();
        }

        void* PalSpriteCreateText(int a1, byte* Text, int* a3, int* a4, int* a5)
        {
            if (a1 != 0 && Text != null)
            {
                Text = (byte*)EntryPoint.Process(Text);
            }
            return Bypass(a1, Text, a3, a4, a5);
        }
    }
}
