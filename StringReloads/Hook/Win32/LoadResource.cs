using StringReloads.Hook.Base;
using System;

namespace StringReloads.Hook.Win32
{
    unsafe class LoadResource : Hook<LoadResourceDelegate>
    {
        public override string Library => "kernel32";

        public override string Export => "LoadResource";

        public override void Initialize()
        {
            HookDelegate = hLoadResource;
            Compile();
        }

        public Action OnCalled = null;

        public void* hLoadResource(void* hModule, void* hResInfo)
        {
            OnCalled?.Invoke();
            return Bypass(hModule, hResInfo);
        }
    }
}
