using System;
using System.Runtime.InteropServices;

namespace SRLUnity {
    public static class Wrapper
    {
        static ProcessStd Function = null;
        public static bool Is64Bits = IntPtr.Size == 8;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr ProcessStd(IntPtr Ptr);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr GetDirectProcess();

        public static string Process(string Text) {
            Initialize();

            IntPtr Ptr = Marshal.StringToHGlobalAuto(Text);
            IntPtr New = Function(Ptr);
            Text = Marshal.PtrToStringAuto(New);
            Marshal.FreeHGlobal(Ptr);
            return Text;
        }
        public static char Process(char Char) {
            Initialize();
            IntPtr Result = Function(new IntPtr(Char));
            return (char)(Result.ToInt32() & 0xFFFF);
        }

        private static void Initialize() {
            if (Function == null) {
                IntPtr DLL = NativeMethods.LoadLibrary(Is64Bits ? "SRLx64.dll" : "SRLx86.dll");
                IntPtr Func = NativeMethods.GetProcAddress(DLL, "ProcessStd");
                Function = Marshal.GetDelegateForFunctionPointer(Func, typeof(ProcessStd)) as ProcessStd;
            }
        }

    }
    static class NativeMethods {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

}
