using StringReloads.Engine.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.Engine.Unmanaged
{
    static unsafe class SanityChecks
    {
        public static bool IsBadPtr(void* Pointer) => IsBadCodePtr(Pointer);
        public static bool IsBadPtr(byte* Pointer) => IsBadCodePtr(Pointer);
        public static bool IsBadPtr(ushort* Pointer) => IsBadCodePtr(Pointer);
        public static bool IsBadPtr(CString Pointer) => IsBadCodePtr(Pointer);
        public static bool IsBadPtr(WCString Pointer) => IsBadCodePtr(Pointer);

        [DllImport("kernel32.dll")]
        public static extern bool IsBadCodePtr(void* Pointer);
    }
}
