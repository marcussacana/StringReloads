using static StringReloads.Hook.Base.Extensions;
using System;
using System.Runtime.InteropServices;

namespace StringReloads.Mods.Base
{
    public unsafe class MemoryPatching
    {
        public void* PatchAddress { get; private set; }
        public MemoryPatching(void* Address, long Assert) : this((void*)(((long)Address) + Assert)) { }
        public MemoryPatching(void* Address) {
            PatchAddress = Address;
        }

        public MemoryPatching(void* Address, long Assert, byte[] PatchedData, byte[] UnpatchedData) : this((void*)(((long)Address) + Assert), PatchedData, UnpatchedData) { }
        public MemoryPatching(void* Address, byte[] PatchedData, byte[] UnpatchedData) {
            this.PatchAddress = Address;
            this.PatchedData = PatchedData;
            this.UnpatchedData = UnpatchedData;
        }

        public virtual byte[] PatchedData { get; }
        public virtual byte[] UnpatchedData { get; }

        public bool IsCompatible() {
            var MaxSize = Math.Max(PatchedData.Length, UnpatchedData.Length);
            DeprotectMemory(PatchAddress, (uint)MaxSize);
            byte[] Buffer = new byte[MaxSize];
            Marshal.Copy(new IntPtr(PatchAddress), Buffer, 0, MaxSize);
            if (Equals(UnpatchedData, Buffer))
                return true;
            if (Equals(PatchedData, Buffer))
                return true;
            return false;
        }

        public void Enable() {
            DeprotectMemory(PatchAddress, (uint)PatchedData.Length);
            Marshal.Copy(PatchedData, 0, new IntPtr(PatchAddress), PatchedData.Length);
        }

        public void Disable() {
            DeprotectMemory(PatchAddress, (uint)UnpatchedData.Length);
            Marshal.Copy(UnpatchedData, 0, new IntPtr(PatchAddress), UnpatchedData.Length);
        }

        private bool Equals(byte[] BufferA, byte[] BufferB) {
            if (BufferA.Length > BufferB.Length)
                return false;
            for (int i = 0; i < BufferA.Length; i++)
                if (BufferA[i] != BufferB[i])
                    return false;
            return true;
        }
    }
}
