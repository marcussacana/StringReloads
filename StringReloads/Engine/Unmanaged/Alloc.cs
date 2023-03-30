using Antlr.Runtime.Tree;
using StringReloads.Engine.String;
using System;
using System.Linq;
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

        public static CString Overwrite(CString String, CString OriString)
        {
            var Data = String.ToArray();

            Array.Resize(ref Data, Data.Length + 1);//Null Termination

            var OriginalSize = OriString.Count();

            return OveriteInternal(Data, OriString, OriginalSize);
        }

        public static WCString Overwrite(WCString String, WCString OriString)
        {
            var Data = String.ToArray();

            Array.Resize(ref Data, Data.Length + 2);//Null Termination

            var OriginalSize = OriString.Count();

            return OveriteInternal(Data, OriString, OriginalSize);
        }

        private static void* OveriteInternal(byte[] Data, void* OriString, int OriginalSize)
        {
            if (Data.Length < OriginalSize)
            {
                Array.Resize(ref Data, OriginalSize);
            }

            Marshal.Copy(Data, 0, new IntPtr(OriString), Data.Length);
            return OriString;
        }

        public static CString SafeOverwrite(CString String, CString OriString)
        {
            var Data = String.ToArray();
            
            Array.Resize(ref Data, Data.Length + 1);//Null Termination

            var OriginalSize = OriString.Count();

            return SafeOveriteInternal(Data, String, OriString, OriginalSize);
        }

        public static WCString SafeOverwrite(WCString String, WCString OriString)
        {
            var Data = String.ToArray();

            Array.Resize(ref Data, Data.Length + 2);//Null Termination

            var OriginalSize = OriString.Count();

            return SafeOveriteInternal(Data, String, OriString, OriginalSize);
        }

        private static void* SafeOveriteInternal(byte[] Data, void* String, void* OriString, int OriginalSize)
        {
            if (Data.Length < OriginalSize)
            {
                Array.Resize(ref Data, OriginalSize);
            }

            if (Data.Length > OriginalSize)
            {
                byte* pOriEnd = (byte*)OriString + OriginalSize;

                int EmptyBufferSize = 0;
                while (*pOriEnd == 0)
                {
                    EmptyBufferSize++;
                    pOriEnd++;
                }

                if (EmptyBufferSize + OriginalSize < Data.Length)
                {
                    return String;
                }
            }

            Marshal.Copy(Data, 0, new IntPtr(OriString), Data.Length);
            return OriString;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void* HeapCreate(uint flOptions, UIntPtr dwInitialSize, UIntPtr dwMaximumSize);
        
        [DllImport("kernel32.dll", SetLastError = false)]
        static extern void* HeapAlloc(void* hHeap, uint dwFlags, UIntPtr dwBytes);
    }
}
