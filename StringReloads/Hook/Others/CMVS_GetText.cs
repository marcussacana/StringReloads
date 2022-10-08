using StringReloads.Hook.Base;

namespace StringReloads.Hook.Others
{
    unsafe class CMVS_GetText : Hook<CMVS_GetTextDelegate>
    {
        public CMVS_GetText(void* Address) : base(Address) { }

        public override string Library => throw new System.NotImplementedException();

        public override string Export => throw new System.NotImplementedException();

        public override void Initialize()
        {
            HookDelegate = new CMVS_GetTextDelegate(GetStrHook);
            Compile(Function);
        }

        void* GetStrHook(void* hScript, int StrID)
        {
            void* Str = Bypass(hScript, StrID);

            if (StrID >= 0)
                return EntryPoint.Process(Str);

            return Str;
        }
    }
}
