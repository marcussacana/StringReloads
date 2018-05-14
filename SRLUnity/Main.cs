using System;
using System.Runtime.InteropServices;

namespace SRLUnity {
    public static class Wrapper
    {
        public static bool Is64Bits = IntPtr.Size == 8;
#if x86
        public static bool Is64BitsBuild = false;

        [DllImport("SRLx32.dll", CallingConvention = CallingConvention.StdCall)]
#endif
#if x64
        public static bool Is64BitsBuild = true;
        [DllImport("SRLx64.dll", CallingConvention = CallingConvention.FastCall)]
#endif
        private extern static IntPtr Process(IntPtr Ptr);

        public static string Process(string Text) {
            TestBuild();

            IntPtr Ptr = Marshal.StringToHGlobalAuto(Text);
            IntPtr New = Process(Ptr);
            Text = Marshal.PtrToStringAuto(New);
            Marshal.FreeHGlobal(Ptr);
            return Text;
        }
        public static char Process(char Char) {
            TestBuild();
            IntPtr Result = Process(new IntPtr(Char));
            return (char)(Result.ToInt32() & 0xFFFF);
        }

        private static void TestBuild() {
            if (Is64Bits != Is64BitsBuild) {
                string Error = $"You can't Use {(Is64BitsBuild ? "x64" : "x86")} Builds in {(Is64Bits ? "x64" : "x86")} Games";
                throw new Exception(Error);
            }
        }
    }
}
