using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System;
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
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
        }

        bool Enabled = false;
        private void ProcessExit(object sender, EventArgs e)
        {
            if (!Enabled)
                return;

            Process.GetCurrentProcess().Kill();
        }

        public string Name => "ForceExit";

        Thread Watcher;
        public void Install() {
            Enabled = true;
            Watcher.Start();
        }
        public bool IsCompatible() => true;

        public void Uninstall() {
            Enabled = false;
            Watcher.Abort();
        }
        [DllImport("user32.dll")]
        static extern bool IsWindow(void* hWnd);
    }
}
