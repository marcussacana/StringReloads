using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static Overlay.Imports;


#pragma warning disable 1591
namespace Overlay {
    public static class Exports {
        public static bool SetDialogue(string text) {
            try {
                Overlay.DefaultInstance.ShowText(text);
                return true;
            } catch {
                return false;
            }
        }

        private static void UpdateWindow(IntPtr WindowHandler) {
            var Rst = new WINDOWPLACEMENT();
            GetWindowPlacement(WindowHandler, ref Rst);
            if (Rst.showCmd == WMinimized)
                return;

            Point Pos;
            Size Siz;
            GetWindowArea(WindowHandler, out Pos, out Siz);

            Overlay.DefaultInstance.Invoke(new MethodInvoker(() => {
                Overlay.DefaultInstance.Size = new Size(Siz.Width - (WindowDiff.Left + WindowDiff.Right), Siz.Height - (WindowDiff.Top + WindowDiff.Bottom));
                Overlay.DefaultInstance.Location = new Point(Pos.X + WindowDiff.Left, Pos.Y + WindowDiff.Top);

                Overlay.DefaultInstance.Focus();
                SetForegroundWindow(WindowHandler);

            }));
        }

        public static bool HookWindow(IntPtr WindowHandler) {
            try {
                Thread OverlayThread = new Thread(() => {
                    if (!IsWindow(WindowHandler))
                        return;

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    Overlay.DefaultInstance.Show();

                    while (IsWindow(WindowHandler)) {
                        try {
                            UpdateWindow(WindowHandler);
                        } catch { }

                        for (int i = 0; i < 50; i++) {
                            Application.DoEvents();
                            Thread.Sleep(10);
                        }
                    }
                    //Maybe Envoriment.Exit will bee needed.
                    Overlay.DefaultInstance.Close();
                });

                OverlayThread.SetApartmentState(ApartmentState.STA);
                OverlayThread.Start();

                return true;
            } catch {
                return false;
            }
        }

        private static Padding WindowDiff = new Padding(0, 0, 0, 0);
        public static void SetOverlayPadding(int Top, int Bottom, int Left, int Rigth) {
            WindowDiff = new Padding(Left, Top, Rigth, Bottom);
        }
        public static void GetWindowArea(IntPtr WindowHandler, out Point Point, out Size Size) {
            var RECT = new RECT();
            GetWindowRect(WindowHandler, out RECT);
            Point = new Point(RECT.Left, RECT.Top);
            Size = new Size(RECT.Right - RECT.Left, RECT.Bottom - RECT.Top);
        }
    }

    internal static class Imports {
        internal const int WNormal = 1;
        internal const int WMinimized = 2;
        internal const int WMaximized = 3;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        internal struct WINDOWPLACEMENT {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT {
            public int Left;        // x position of upper-left corner  
            public int Top;         // y position of upper-left corner  
            public int Right;       // x position of lower-right corner  
            public int Bottom;      // y position of lower-right corner  
        }
    }
}

#pragma warning restore 1591