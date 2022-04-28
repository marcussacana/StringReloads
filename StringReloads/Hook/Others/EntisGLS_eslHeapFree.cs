using StringReloads.Engine;
using StringReloads.Hook.Base;
using System;
using System.IO;

namespace StringReloads.Hook.Others
{
    unsafe class EntisGLS_eslHeapFree : Hook<EntisGLS_eslHeapFreeDelegate>
    {
        public override string Library => Path.GetFileName(Config.Default.GameExePath);

        public override string Export => "eslHeapFree";

        public override void Initialize()
        {
            HookDelegate = eslHeapFree;
            Compile();
        }

        public Action<IntPtr> OnHeapDisposed;

        public void eslHeapFree(void* a1, void* Heap, void* a3) {

            Bypass(a1, Heap, a3);

            OnHeapDisposed?.Invoke(new IntPtr(Heap));
        }
    }
}
