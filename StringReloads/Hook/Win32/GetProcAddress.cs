namespace StringReloads.Hook
{
    public unsafe class GetProcAddress : Base.Hook<GetProcAddressDelegate>
    {
        public override string Library => "kernel32.dll";

        public override string Export => "GetProcAddress";


        public override void Initialize()
        {
            HookDelegate = new GetProcAddressDelegate(hGetProcAddress);
            Compile();
        }

        public static event GetProc OnGetProcAddress;
        private void* hGetProcAddress(void* hModule, void* Proc) {
            var Rst = Bypass(hModule, Proc);

            if (OnGetProcAddress != null) {
                var ERst = OnGetProcAddress.Invoke(hModule, Proc, Rst);
                if (ERst != null)
                    return ERst;
            }

            return Rst;
        }

        public delegate void* GetProc(void* hModule, void* Proc, void* Result);
    }
}
