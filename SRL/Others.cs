using System.Diagnostics;
using System.Windows.Forms;

namespace SRL
{
    partial class StringReloader
    {
        public static void Restart()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & \"" + Application.ExecutablePath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
