using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SRLUnity {
    public static class Wrapper {
        public static string AssemblyDirectory {
            get {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string DataDirectory {
            get {
                return Path.GetDirectoryName(AssemblyDirectory);
            }
        }

        public static string GameDirectory {
            get {
                return Path.GetDirectoryName(DataDirectory);
            }
        }

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
                try {
                    string DLLName = Is64Bits ? "SRLx64.dll" : "SRLx32.dll";
                    string[] Paths = new string[] {
                        GameDirectory + "/",
                        DataDirectory + "/Managed/",
                        DataDirectory + "/Plugins/",
                    };

                    int Loop = 0;
                    IntPtr DLL = NativeMethods.LoadLibrary(DLLName);
                    while (DLL == IntPtr.Zero) {
                        if (!File.Exists(Paths[Loop] + DLLName)) {
                            Loop++;
                            continue;
                        }

                        DLL = NativeMethods.LoadLibrary(Paths[Loop] + DLLName);
                        if (DLL == IntPtr.Zero)
                            DLL = NativeMethods.LoadLibrary(Paths[Loop] + DLLName.Replace("/", "\\"));
                    }
                    IntPtr Func = NativeMethods.GetProcAddress(DLL, "ProcessStd");
                    Function = Marshal.GetDelegateForFunctionPointer(Func, typeof(ProcessStd)) as ProcessStd;
                } catch (Exception ex){
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static T[] AppendArray<T>(T[] Array, T Value) {
            T[] New = new T[Array.Length + 1];
            Array.CopyTo(New, 0);
            New[Array.Length] = Value;
            return New;
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
