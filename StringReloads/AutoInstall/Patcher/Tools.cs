using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StringReloads.AutoInstall.Patcher
{
    static class Tools
    {
        static string CurrentDllName => Path.GetFileName(EntryPoint.CurrentDll);
        internal static bool ApplyWrapperPatch()
        {
            if (CurrentDllName.ToLower() == "srl.dll")
                return true;

            if (MessageBox.Show("The SRL need apply a patch in the game, apply now?\nIf yes the game will restart", "StringReloads", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return false;

            Retry:;
            string EXEPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            byte[] Data = File.ReadAllBytes(EXEPath);

            int Offset = IndexOf(Data, CurrentDllName);
            if (Offset == -1)
                Offset = IndexOf(Data, CurrentDllName.ToUpper());
            if (Offset == -1)
                Offset = IndexOf(Data, Path.GetFileName(EntryPoint.CurrentDll));

            if (Offset == -1)
            {
                var Rst = MessageBox.Show($"Failed to Patch, \"{Path.GetFileName(CurrentDllName)}\" occurrence not found in the game executable.", "StringReloads", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (Rst == DialogResult.Retry)
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
    }
}
