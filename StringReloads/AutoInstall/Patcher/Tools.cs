using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using static StringReloads.Engine.User;

namespace StringReloads.AutoInstall.Patcher
{
    static class Tools
    {
        static readonly string[] SupportedWrappers = new string[] {
            "version.dll", "d3d9.dll", "d3d10.dll", "d3d11.dll",
            "dinput.dll", "dinput8.dll", "dinput8.dll", "dsound.dll",
            "dxgi.dll"
        };
        static bool PatchAllowed = false;
        static string CurrentDllName => Path.GetFileName(EntryPoint.CurrentDll);
        internal static bool ApplyWrapperPatch(string Name = "SRL.dll")
        {
            if (!Engine.Config.Default.WrapperPatch)
                return true;

            if (CurrentDllName.ToLowerInvariant().StartsWith(Name.ToLowerInvariant()))
                return true;

            if (!PatchAllowed && ShowMessageBox("The SRL needs to apply a patch in the game, do you to want apply now?\nIf yes, the game will restart.", "StringReloads", MBButtons.YesNo, MBIcon.Warning) == MBResult.No)
                return false;
            
            PatchAllowed = true;

            Retry:;
            string UsedWrapper = null;
            string EXEPath = Process.GetCurrentProcess().MainModule.FileName;
            byte[] Data = File.ReadAllBytes(EXEPath);

            int Offset = -1;
            if (CurrentDllName.Length > Name.Length)
            {
                Offset = IndexOf(Data, CurrentDllName);
                if (Offset == -1)
                    Offset = IndexOf(Data, CurrentDllName.ToUpper());
            }

            if (Offset == -1)
            {
                foreach (var Wrapper in SupportedWrappers)
                {
                    if (Wrapper.Length < Name.Length)
                        continue;

                    UsedWrapper = Wrapper;
                    Offset = IndexOf(Data, Wrapper);
                    if (Offset != -1)
                        break;

                    Offset = IndexOf(Data, Wrapper.ToUpper());
                    if (Offset != -1)
                        break;

                    Offset = IndexOf(Data, Wrapper.ToUpper().Replace(".DLL", ".dll"));
                    if (Offset != -1)
                        break;
                }
            }

            if (Offset == -1)
            {
                var Rst = ShowMessageBox($"Failed to patch, \"{CurrentDllName}\" occurrence not found in the game executable.", "StringReloads", MBButtons.RetryCancel, MBIcon.Error);
                if (Rst == MBResult.Retry)
                    goto Retry;
                else
                    return false;
            }

            var Patch = Encoding.ASCII.GetBytes($"{Name}\x0");
            Array.Copy(Patch, 0, Data, Offset, Patch.Length);

            string Output = EXEPath;

            if (!TryRename(EXEPath, EXEPath + ".bak"))
                Output = Path.Combine(Path.GetDirectoryName(EXEPath), Path.GetFileNameWithoutExtension(EXEPath) + "-Patched.exe");

            File.WriteAllBytes(Output, Data);

            if (!TryRename(CurrentDllName, Name))
                File.Copy(CurrentDllName, Name);

            if (UsedWrapper != null)
            {
                Log.Debug($"Wrapper to \"{UsedWrapper}\" detected as supported and enabled");
            }

            Restart();
            return true;
        }

        public static bool ThirdPartyApplyPatch(string Executable, string Name = "SRL.dll")
        {
            var CurrentProc = Process.GetCurrentProcess();

            if (Executable.ToLowerInvariant() == CurrentProc.MainModule.FileName.ToLowerInvariant())
                return false;
            
            if (!PatchAllowed && ShowMessageBox("The SRL needs to apply a patch in the game, do you to want apply now?\nIf yes, the game will restart.", "StringReloads", MBButtons.YesNo, MBIcon.Warning) == MBResult.No)
                return false;

            PatchAllowed = true;

            var Procs = (from x in Process.GetProcessesByName(Executable.GetFilenameNoExt())
                         where x.Id != CurrentProc.Id
                         select x);

            foreach (var Proc in Procs)
                Proc.Kill();

            Retry:;
            string UsedWrapper = null;
            byte[] Data = File.ReadAllBytes(Executable);

            int Offset = -1;

            if (CurrentDllName.Length > Name.Length)
            {
                Offset = IndexOf(Data, CurrentDllName);
                if (Offset == -1)
                    Offset = IndexOf(Data, CurrentDllName.ToUpper());
            }

            if (Offset == -1)
            {

                foreach (var Wrapper in SupportedWrappers)
                {
                    if (Wrapper.Length < Name.Length)
                        continue;

                    UsedWrapper = Wrapper;
                    Offset = IndexOf(Data, Wrapper);
                    if (Offset != -1)
                        break;

                    Offset = IndexOf(Data, Wrapper.ToUpper());
                    if (Offset != -1)
                        break;

                    Offset = IndexOf(Data, Wrapper.ToUpper().Replace(".DLL", ".dll"));
                    if (Offset != -1)
                        break;
                }
            }

            if (Offset == -1)
            {
                var Rst = ShowMessageBox($"Failed to patch, \"{CurrentDllName}\" occurrence not found in the game executable.", "StringReloads", MBButtons.RetryCancel, MBIcon.Error);
                if (Rst == MBResult.Retry)
                    goto Retry;
                else
                    return false;
            }

            var Patch = Encoding.ASCII.GetBytes($"{Name}\x0");
            Array.Copy(Patch, 0, Data, Offset, Patch.Length);

            string Output = Executable;

            if (!TryRename(Executable, Executable + ".bak"))
                Output = Path.Combine(Path.GetDirectoryName(Executable), Path.GetFileNameWithoutExtension(Executable) + "-Patched.exe");

            File.WriteAllBytes(Output, Data);

            if (UsedWrapper != null)
            {
                Log.Debug($"Wrapper to \"{UsedWrapper}\" detected as supported and enabled");
            }

            return true;
        }

        public static void Restart()
        {
            Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & \"" + Process.GetCurrentProcess().MainModule.FileName + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
            Process.GetCurrentProcess().Kill();
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
    }
}
