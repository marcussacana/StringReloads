using StringReloads.Engine;
using StringReloads.Hook.Base;
using System;
using System.IO;

namespace StringReloads.Hook.Others
{
    unsafe class EntisGLS_eslHeapAllocate : Hook<EntisGLS_eslHeapAllocateDelegate>
    {
        public override string Library => Path.GetFileName(Config.Default.GameExePath);

        public override string Export => "eslHeapAllocate";

        public override void Initialize()
        {
            HookDelegate = eslHeapAllocate;
            Compile();
        }

        public Action<IntPtr, int> OnHeapAllocated;

        public void* eslHeapAllocate(void* a1, int Size, void* a3) {

            var Heap = Bypass(a1, Size, a3);
            
            if (Heap != null)
                OnHeapAllocated?.Invoke(new IntPtr(Heap), Size);
            
            return Heap;
        }
    }
}
