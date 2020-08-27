using StringReloads.Hook.Base;

namespace StringReloads.Hook.Others
{
    unsafe class CMVS32_GetText : Hook<CMVS32_GetTextDelegate>
    {
        public CMVS32_GetText(void* Address) : base(Address) { }

        public override string Library => throw new System.NotImplementedException();

        public override string Export => throw new System.NotImplementedException();

        public override void Initialize()
        {
            HookDelegate = new CMVS32_GetTextDelegate(GetStrHook);
            Compile(Function);
        }

        void* GetStrHook(void* hScript, uint StrID)
        {
            void* Str = Bypass(hScript, StrID);
            return EntryPoint.Process(Str);
        }
    }
}
