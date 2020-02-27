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

        internal static void* LoadLibrary(string lpFileName)
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

            if (Handler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            return Handler;
        }

        internal static bool ApplyWrapperPatch()
        {
            if (CurrentDllName == "srl.dll")
                return true;

            if (MessageBox("The SRL need apply a patch in the game, apply now?\nIf yes the game will restart", "StringReloader", MB_YESNO | MB_ICONQUESTION) != IDYES)
                return false;

            Retry:;
            string EXEPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            byte[] Data = File.ReadAllBytes(EXEPath);

            int Offset = IndexOf(Data, CurrentDllName);
            if (Offset == -1)
                Offset = IndexOf(Data, CurrentDllName.ToUpper());
            if (Offset == -1)
                Offset = IndexOf(Data, Path.GetFileName(GetSRLPath()));

            if (Offset == -1)
            {
                var Rst = MessageBox($"Failed to Patch, \"{Path.GetFileName(CurrentDllName)}\" occurrence not found in the game executable.", "StringReloader", MB_RETRYCANCEL | MB_ICONERROR);
                if (Rst == IDRETRY)
                    goto Retry;
                else
                    return false;
            }

            var Patch = Encoding.ASCII.GetBytes("SRL.dll\x0");
            Array.Copy(Patch, 0, Data, Offset, Patch.Length);

            string Output = EXEPath;

            if (!TryRename(EXEPath, EXEPath + ".bak"))
                Output = Path.Combine(Path.GetDirectoryName(EXEPath), Path.GetFileNameWithoutExtension(EXEPath) + "-Patched.exe");

            File.WriteAllBytes(Output, Data);

            if (!TryRename(CurrentDllName, "SRL.dll"))
                File.Copy(CurrentDllName, "SRL.dll");

            Restart();
            return true;
        }
        public static void Restart()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & \"" + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\"",
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        internal static int IndexOf(byte[] Array, string String) => IndexOf(Array, Encoding.ASCII.GetBytes(String));

        internal static int IndexOf(byte[] Array, byte[] SubArray)
        {
            if (SubArray.Length > Array.Length)
                return -1;

            for (int i = 0, x = 0; i < Array.Length; i++)
            {
                if (Array[i] == SubArray[x])
                    x++;
                else
                    x = 0;
                if (x == SubArray.Length)
                    return i - x + 1;
            }
            return -1;
        }

        internal static bool TryRename(string FileName, string NewName)
        {
            int Tries = 20;
            while (Tries > 0)
            {
                try
                {
                    var OutPath = Path.Combine(Path.GetDirectoryName(FileName), NewName);
                    if (OutPath.ToLower() == FileName.ToLower())
                        return false;
                    if (File.Exists(OutPath))
                        File.Delete(OutPath);
                    File.Move(FileName, OutPath);
                    return true;
                }
                catch
                {
                    Thread.Sleep(100);
                    Tries--;
                }
            }
            return false;
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

        static string GetSRLPath()
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
