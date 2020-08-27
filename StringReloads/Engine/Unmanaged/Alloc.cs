using System;
using System.Runtime.InteropServices;

namespace StringReloads.Engine.Unmanaged
{
    unsafe static class Alloc
    {
        const uint HEAP_ZERO_MEMORY = 0x00000008;
        public static void* CreateHeap(byte[] Data) {
            var hHeap = HeapCreate(0, new UIntPtr((uint)Data.Length), new UIntPtr((uint)Data.Length));
            var hAlloc = HeapAlloc(hHeap, HEAP_ZERO_MEMORY, new UIntPtr((uint)Data.Length));
            Marshal.Copy(Data, 0, new IntPtr(hAlloc), Data.Length);
            return hAlloc;
        }

        public static void* Overwrite(byte[] Data, void* Address) {
            Marshal.Copy(Data, 0, new IntPtr(Address), Data.Length);
            return Address;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void* HeapCreate(uint flOptions, UIntPtr dwInitialSize, UIntPtr dwMaximumSize);
        
        [DllImport("kernel32.dll", SetLastError = false)]
        static extern void* HeapAlloc(void* hHeap, uint dwFlags, UIntPtr dwBytes);
    }
}
