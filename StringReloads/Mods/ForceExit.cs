using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace StringReloads.Mods
{
    unsafe class ForceExit : IMod
    {
        public ForceExit() {
            Watcher = new Thread(() => {
                while (Config.Default.MainWindow == null)
                    Thread.Sleep(500);

                while (true) {
                    if (!IsWindow(Config.Default.MainWindow))
                        Process.GetCurrentProcess().Kill();
                    Thread.Sleep(500);
                }
            });
            Watcher.IsBackground = true;
        }
        public string Name => "ForceExit";

        Thread Watcher;
        public void Install() => Watcher.Start();

        public bool IsCompatible() => true;

        public void Uninstall() => Watcher.Abort();

        [DllImport("user32.dll")]
        static extern bool IsWindow(void* hWnd);
    }
}
