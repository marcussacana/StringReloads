using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SRL.Wrapper {
    static class Tools {

        internal static string CurrentDllName = Path.GetFileName(Assembly.GetExecutingAssembly().Location).ToLower();
        internal static string CurrentDllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static bool WOW64 => !Environment.Is64BitProcess && Environment.Is64BitOperatingSystem;


        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_28(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J, IntPtr K, IntPtr L, IntPtr M, IntPtr N, IntPtr O, IntPtr P, IntPtr Q, IntPtr R, IntPtr S, IntPtr T, IntPtr U, IntPtr V, IntPtr W, IntPtr X, IntPtr Y, IntPtr Z, IntPtr AA, IntPtr AB);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_12(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J, IntPtr K, IntPtr L);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_11(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J, IntPtr K);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_10(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_9(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_8(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_7(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_6(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_5(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_4(IntPtr A, IntPtr B, IntPtr C, IntPtr D);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_3(IntPtr A, IntPtr B, IntPtr C);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_2(IntPtr A, IntPtr B);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_1(IntPtr A);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr RET_0();


        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_12(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J, IntPtr K, IntPtr L);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_8(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_5(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_4(IntPtr A, IntPtr B, IntPtr C, IntPtr D);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_3(IntPtr A, IntPtr B, IntPtr C);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_2(IntPtr A, IntPtr B);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_1(IntPtr A);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void NULL_0();

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibraryW(string lpFileName);

        internal static IntPtr LoadLibrary(string lpFileName) {
            string DllPath = lpFileName;
            if (lpFileName.Length < 2 || lpFileName[1] != ':') {
                string DLL = Path.GetFileNameWithoutExtension(lpFileName);
                DllPath = Path.Combine(Environment.CurrentDirectory, $"{DLL}_ori.dll");


                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(Environment.CurrentDirectory, $"{DLL}.dll");

                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(CurrentDllPath, $"{DLL}_ori.dll");

                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(CurrentDllPath, $"{DLL}.dll.ori");

                if (!File.Exists(DllPath)) {
                    DllPath = WOW64 ? Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) : Environment.SystemDirectory;
                    DllPath = Path.Combine(DllPath, $"{DLL}.dll");
                }
            }

            IntPtr Handler = LoadLibraryW(DllPath);

            if (Handler == IntPtr.Zero)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            return Handler;
        }

        internal static T GetDelegate<T>(IntPtr Handler, string Function, bool Optional = true) where T : Delegate {
            IntPtr Address = GetProcAddress(Handler, Function);
            if (Address == IntPtr.Zero) {
                if (Optional) {
                    return null;
                }

                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED
            }
            return (T)Marshal.GetDelegateForFunctionPointer(Address, typeof(T));
        }

        public static bool IsWow64(Process Target) {
            try {
                if (IsWow64Process(Target.Handle, out bool WOW64)) {
                    if (Environment.Is64BitOperatingSystem && WOW64)
                        return true;
                }
            } catch { }
            return false;
        }


        static ulong ToUInt64(this IntPtr Ptr) => unchecked((ulong)Ptr.ToInt64());
        static uint ToUInt32(this IntPtr Ptr) => unchecked((uint)(Ptr.ToInt64() & 0xFFFFFFFF));

        static IntPtr ToIntPtr(this ulong Int) => new IntPtr(unchecked((long)Int));

        static IntPtr Sum(this IntPtr Pointer, IntPtr Value) => (Pointer.ToUInt64() + Value.ToUInt64()).ToIntPtr();
        static IntPtr Sum(this IntPtr Pointer, long Value) => (Pointer.ToUInt64() + (ulong)Value).ToIntPtr();

        static uint ToUInt32(this byte[] Data, int Address = 0) => BitConverter.ToUInt32(Data, Address);
        static ushort ToUInt16(this byte[] Data, int Address = 0) => BitConverter.ToUInt16(Data, Address);
        static int ToInt32(this byte[] Data, int Address = 0) => BitConverter.ToInt32(Data, Address);
        static long ToInt64(this byte[] Data, int Address = 0) => BitConverter.ToInt64(Data, Address);

        static IntPtr ToIntPtr(this byte[] Data, bool? x64 = null) {
            if (x64.HasValue)
                return new IntPtr(x64.Value ? Data.ToInt64() : Data.ToInt32());
            if (Data.Length >= 8)
                return new IntPtr(IntPtr.Size == 8 ? Data.ToInt64() : Data.ToInt32());
            return new IntPtr(Data.ToInt32());
        }


        static bool ChangeProtection(IntPtr Address, int Range, Protection Protection, out Protection OriginalProtection) {
            return VirtualProtect(Address, Range, Protection, out OriginalProtection);
        }

        static bool ChangeProtection(IntPtr Address, int Range, Protection Protection) {
            return VirtualProtect(Address, Range, Protection, out Protection OriginalProtection);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, Protection flNewProtect, out Protection lpflOldProtect);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, [Out] IntPtr lphModule, uint cb, out uint lpcbNeeded, DwFilterFlag dwff);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, int nSize);

        enum Protection {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }
        public enum DwFilterFlag : uint {
            LIST_MODULES_DEFAULT = 0x0,
            LIST_MODULES_32BIT = 0x01,
            LIST_MODULES_64BIT = 0x02,
            LIST_MODULES_ALL = (LIST_MODULES_32BIT | LIST_MODULES_64BIT)
        }

    }

    struct ImportEntry {

        /// <summary>
        /// The Imported Module Name
        /// </summary>
        public string Module;

        /// <summary>
        /// The Imported Function Name
        /// </summary>
        public string Function;

        /// <summary>
        /// The Import Ordinal Hint
        /// </summary>
        public ushort Ordinal;

        /// <summary>
        /// The Address of this Import in the IAT (Import Address Table)
        /// </summary>
        public IntPtr ImportAddress;

        /// <summary>
        /// The Address of the Imported Function
        /// </summary>
        public IntPtr FunctionAddress;
    }
}
