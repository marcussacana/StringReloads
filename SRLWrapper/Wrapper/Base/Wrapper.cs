using StringReloads;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SRLWrapper.Wrapper.Base
{
    static unsafe class Wrapper
    {

        public static string CurrentDllName = Path.GetFileName(GetSRLPath()).ToLower();
        public static string CurrentDllPath = Path.GetDirectoryName(GetSRLPath());

        public static string RealDllPath { get; internal set; } = null;
        public static void* RealHandler { get; internal set; }
        public static void* WrapperHandler { get; internal set; } = GetSRLModuleHandle();


        static bool WOW64 => !Environment.Is64BitProcess && Environment.Is64BitOperatingSystem;

        internal static void* LoadLibrary(string lpFileName, bool Required = true)
        {
            string DllPath = lpFileName;
            if (lpFileName.Length < 2 || lpFileName[1] != ':')
            {
                string DLL = Path.GetFileNameWithoutExtension(lpFileName);
                DllPath = Path.Combine(Environment.CurrentDirectory, $"{DLL}_ori.dll");


                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(Environment.CurrentDirectory, $"{DLL}.dll");

                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(CurrentDllPath, $"{DLL}_ori.dll");

                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(CurrentDllPath, $"{DLL}.dll.ori");

                if (!File.Exists(DllPath))
                {
                    DllPath = WOW64 ? Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) : Environment.SystemDirectory;
                    DllPath = Path.Combine(DllPath, $"{DLL}.dll");
                }
            }

            RealDllPath = DllPath;

            void* Handler = RealHandler = LoadLibraryW(DllPath);

            if (Handler == null && Required)
            {
                MessageBoxW(null, "Library: " + DllPath, "SRL WRAPPER ERROR", 0x10);
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED
            }

            return Handler;
        }


        internal static T GetDelegate<T>(void* Handler, string Function, bool Optional = true) where T : Delegate
        {
            var Address = GetProcAddress(Handler, Function);
            if (Address == null)
            {
                if (Optional)
                {
                    return null;
                }

                MessageBoxW(null, "Function: " + Function, "SRL WRAPPER ERROR", 0x10);
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED
            }
            return (T)Marshal.GetDelegateForFunctionPointer(new IntPtr(Address), typeof(T));
        }

        public static void InitializeSRL()
        {
            EntryPoint._CurrentDLL = GetSRLPath();
            EntryPoint.Process(null);
        }

        public static void WaitForExit(this Thread Thread)
        {
            while (Thread.IsRunning())            
                Thread.Sleep(100);            
        }

        private static bool IsRunning(this Thread Thread)
        {
            return Thread.ThreadState == ThreadState.Running || Thread.ThreadState == ThreadState.Background
                || Thread.ThreadState == ThreadState.WaitSleepJoin;
        }

        internal static int MessageBox(string Text, string Title, uint uType)
        {
            void* hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToPointer();
            return MessageBoxW(hWnd, Text, Title, uType);
        }

        static void* GetSRLModuleHandle()
        {
            var Modules = System.Diagnostics.Process.GetCurrentProcess().Modules.Cast<System.Diagnostics.ProcessModule>();
            foreach (var Module in Modules)
            {
                var hModule = Module.BaseAddress.ToPointer();
                var hProc = GetProcAddress(hModule, "GetDirectProcess");
                if (hProc != null)
                    return hModule;
            }
            throw new Exception("Failed to get the module handle");
        }

        internal static string GetSRLPath()
        {
            var Modules = System.Diagnostics.Process.GetCurrentProcess().Modules.Cast<System.Diagnostics.ProcessModule>();
            foreach (var Module in Modules)
            {
                var hModule = Module.BaseAddress.ToPointer();
                var hProc = GetProcAddress(hModule, "GetDirectProcess");
                if (hProc != null)
                    return Module.FileName;
            }
            throw new Exception("Failed to get the SRL path");
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern void* GetProcAddress(void* hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern void* LoadLibraryW(string lpFileName);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int MessageBoxW(void* hWnd, string lpText, string lpCaption, uint uType);

        const uint MB_YESNO = 0x00000004;
        const uint MB_RETRYCANCEL = 0x00000005;
        const uint MB_ICONERROR = 0x00000010;
        const uint MB_ICONQUESTION = 0x00000020;
        const uint MB_ICONWARNING = 0x00000030;
        const uint MB_ICONINFORMATION = 0x00000040;

        const uint IDYES = 6;
        const uint IDRETRY = 4;
    }
}
