namespace StringReloads.Hook
{
    public unsafe class SysAllocString : Base.Hook<SysAllocStringDelegate>
    {
        public override string Library => "OleAut32.dll";

        public override string Export => "SysAllocString";

        public override void Initialize()
        {
            HookDelegate = new SysAllocStringDelegate(hSysAllocString);
            Compile();
        }

        private void* hSysAllocString(void* pStr) {
            pStr = EntryPoint.Process(pStr);
            return Bypass(pStr);
        }
    }
}
