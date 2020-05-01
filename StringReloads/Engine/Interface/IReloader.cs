namespace StringReloads.Engine.Interface
{
    public unsafe interface IReloader
    {
        public string Name { get; }

        public void* BeforeReload(void* pInput, bool WideMode);

        public void* AfterReload(void* pInput, void* pOutput, bool WideMode);
    }
}
