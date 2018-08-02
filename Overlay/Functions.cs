//#define DEBUG
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static Overlay.Imports;
using static Overlay.Exports;
using static Overlay.Events;


#pragma warning disable 1591
namespace Overlay {
    public static class Exports {

        public static bool TextOnly = true;
        internal static IOverlay DefaultInstance {
            get {
                return (TextOnly ? (IOverlay)TextOverlay.DefaultInstance : Overlay.DefaultInstance);
            }
        }
        public static string SetDialogue(string text) {
            try {
                if (text.StartsWith("::EVENT")) {
                    int EVEND = text.IndexOf("::", 2);
                    string EVIDS = text.Substring(0, EVEND);
                    EVIDS = EVIDS.Substring("::EVENT".Length);
                    uint EVID = uint.Parse(EVIDS);
                    text = text.Substring(EVEND + 2);

                    try {
                        TriggerEvent(EVID);
                    } catch (Exception ex) {
#if DEBUG
                        MessageBox.Show(ex.ToString());
#endif
                    }

                }

                if (EventList.Length == 0 || HookText)
                    DefaultInstance.Text = text;
            } catch (Exception ex) {
#if DEBUG
                MessageBox.Show(ex.ToString());
#endif
            }

            return text;
        }

        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public static void SendMouseClick(IntPtr WindowHandler, int X, int Y) {
            GetWindowArea(WindowHandler, out Point Pos, out Size Siz);

            X = X - Pos.X;
            Y = Y - Pos.Y;
            int lparm = (Y << 16) + X;
            int lngResult = SendMessage(WindowHandler, WM_LBUTTONDOWN, 0, lparm);
            int lngResult2 = SendMessage(WindowHandler, WM_LBUTTONUP, 0, lparm);
        }

        static bool Minimized = false;
        internal static void UpdateWindow(IntPtr WindowHandler) {
            if (!DefaultInstance.CanInvoke())
                return;

            var Rst = new WINDOWPLACEMENT();
            GetWindowPlacement(WindowHandler, ref Rst);
            if (Rst.showCmd == WMinimized) {
                if (DefaultInstance.WindowState == FormWindowState.Normal) {
                    Minimized = true;
                    DefaultInstance.Invoke(new MethodInvoker(() => {
                        DefaultInstance.WindowState = FormWindowState.Minimized;
                    }));
                }

                return;
            }


            if (Minimized) {
                Minimized = false;
                DefaultInstance.Invoke(new MethodInvoker(() => {
                    DefaultInstance.WindowState = FormWindowState.Normal;
                }));
            }
            
            DefaultInstance.Invoke(new MethodInvoker(() => {
                try {
                    GetPoint(WindowHandler, out Size Size, out Point Point);
                    if (Size != DefaultInstance.Size)
                        DefaultInstance.Size = Size;
                    if (Point != DefaultInstance.Location)
                        DefaultInstance.Location = Point;


                    SetWindowPos(DefaultInstance.Handle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                    SetWindowPos(DefaultInstance.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

                    DefaultInstance.Focus();
                    SetForegroundWindow(WindowHandler);
                } catch { }
            }));
        }


        internal static DockStyle Dock = DockStyle.Bottom;
        private static void GetPoint(IntPtr WindowHandler, out Size Size, out Point Point) {
            GetWindowArea(WindowHandler, out Point Pos, out Size Siz);

            switch (Dock) {
                default://bottom
                    Size = new Size(Siz.Width - (WindowDiff.Left + WindowDiff.Right), Siz.Height - (WindowDiff.Top + WindowDiff.Bottom));
                    Point = new Point(Pos.X + WindowDiff.Left, Pos.Y + WindowDiff.Top);
                    break;
                case DockStyle.Top:
                    Size = new Size(Siz.Width - (WindowDiff.Left + WindowDiff.Right), Siz.Height - (WindowDiff.Top + WindowDiff.Bottom));
                    Point = new Point(Pos.X + WindowDiff.Left, WindowDiff.Top);
                    break;
                case DockStyle.Fill://center
                    Size = new Size(Siz.Width - (WindowDiff.Left + WindowDiff.Right), Siz.Height - (WindowDiff.Top + WindowDiff.Bottom));
                    Point = new Point(Pos.X + WindowDiff.Left, ((Pos.Y + WindowDiff.Top) / 2) - (Size.Height / 2));
                    break;
            }
        }

        internal static IntPtr HookHandler = IntPtr.Zero;
        public static bool HookWindow(IntPtr WindowHandler) {
            try {
                Thread OverlayThread = new Thread(() => {
                    if (!IsWindow(WindowHandler))
                        return;

                    HookHandler = WindowHandler;

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    
                    DefaultInstance.Show();

                    new Thread(() => { SetDialogue("::EVENT0::"); }).Start();

                    while (IsWindow(WindowHandler)) {
                        try {
                            UpdateWindow(WindowHandler);
                        } catch (Exception ex){
#if DEBUG
                            MessageBox.Show(ex.ToString());
#endif
                        }

                        try {
                            for (int i = 0; i < 25; i++) {
                                Application.DoEvents();
                                Thread.Sleep(10);
                            }
                        } catch { }
                    }

                    //Maybe Envoriment.Exit will bee needed.
                    DefaultInstance.Close();
                });

                OverlayThread.SetApartmentState(ApartmentState.STA);
                OverlayThread.Start();

                return true;
            } catch {
                return false;
            }
        }

        internal static Padding WindowDiff = new Padding(0, 0, 0, 0);
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
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        internal static readonly IntPtr HWND_TOP = new IntPtr(0);
        internal const uint SWP_NOSIZE = 0x0001;
        internal const uint SWP_NOMOVE = 0x0002;
        internal const uint SWP_SHOWWINDOW = 0x0040;

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


        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
    }

    internal static class Events {

        internal static bool HookText = false;
        static string[] _evs = null;
        internal static string[] EventList {
            get {
                if (_evs == null) {
                    _evs = new string[0];
                    if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Events.ovs")) {
                        _evs = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Events.ovs");
                        CacheLabels();
                    }
                }
                return _evs;
            }
        }

        private static void CacheLabels() {
            for (long i = 0; i < EventList.LongLength; i++) {
                string CMD = EventList[i].Trim();
                if (string.IsNullOrWhiteSpace(CMD) || CMD.StartsWith(";") || CMD.StartsWith("//") || CMD.StartsWith("#"))
                    continue;

                if (CMD.StartsWith(":")) {
                    string SID = CMD.Substring(1);
                    if (uint.TryParse(SID, out uint ID)) {
                        Labels[ID] = i;
                    }
                }
            }
        }

        static Dictionary<uint, long> Labels = new Dictionary<uint, long>();
        internal static void TriggerEvent(uint ID) {
            bool InEvent = false;
            for (long i = Labels.ContainsKey(ID) ? Labels[ID] : 0; i < EventList.LongLength; i++) {
                string CMD = EventList[i].Trim();
                if (string.IsNullOrWhiteSpace(CMD) || CMD.StartsWith("#"))
                    continue;


                if (CMD == ":" + ID && !InEvent) {
                    InEvent = true;
                    continue;
                }

                if (!InEvent)
                    continue;


                string CMDN = CMD.Split(':')[0];
                string CMDV = null;
                if (CMD.Contains(":")) {
                    CMDV = CMD.Substring(CMD.IndexOf(":") + 1).Trim();
                }


                try {
                    switch (CMDN.ToLower()) {
                        case "show text":
                        case "subtitle":
                        case "set text":
                        case "text":
                            DefaultInstance.Text = CMDV.Replace("\\n", "\n");
                            break;
                        case "clear":
                        case "clear text":
                        case "remove text":
                        case "cls":
                            DefaultInstance.Text = string.Empty;
                            break;
                        case "hide":
                        case "close":
                        case "hide overlay":
                        case "close overlay":
                            DefaultInstance.Invoke(new MethodInvoker(() => {
                                DefaultInstance.Opacity = 0;
                                DefaultInstance?.Close();
                            }));
                            break;
                        case "show":
                        case "open":
                        case "show overlay":
                        case "open overlay":
                            DefaultInstance.Invoke(new MethodInvoker(() => {
                                DefaultInstance.Opacity = 1.0d;
                                DefaultInstance.Show();
                            }));
                            break;
                        case "window opacity":
                        case "overlay opacity":
                        case "global opacity":
                        case "opacity":
                            double Opacity = double.Parse(CMDV);
                            DefaultInstance.Invoke(new MethodInvoker(() => {
                                DefaultInstance.Opacity = Opacity;
                            }));
                            break;
                        case "set background color":
                        case "background color":
                        case "back color":
                        case "backcolor":
                            DefaultInstance.TextBackColor = LoadColor(CMDV, Color.Black);
                            break;
                        case "text color":
                        case "set text color":
                        case "fore color":
                        case "forecolor":
                                DefaultInstance.TextForeColor = LoadColor(CMDV, Color.FromArgb(40, 40, 40));
                            break;
                        case "set padding":
                        case "force padding":
                        case "change padding":
                        case "padding":
                            if (CMDV.ToLower() == "reset") {
                                WindowDiff = new Padding(0, 0, 0, 0);
                                break;
                            }
                            GetWindowArea(HookHandler, out Point WPoint, out Size WSize);
                            int Top = 0, Bottom = 0, Right = 0, Left = 0;
                            foreach (string Parameter in CMDV.Split('|')) {
                                string Border = Parameter.Split(':')[0].Trim().ToLower();
                                string SVal = Parameter.Split(':')[1].Trim();
                                bool Negative = false;
                                bool Percentage = false;
                                if (SVal.EndsWith("%")) {
                                    SVal = SVal.Substring(0, SVal.Length - 1);
                                    Percentage = true;
                                }
                                if (SVal.StartsWith("-")) {
                                    SVal = SVal.Substring(1);
                                    Negative = true;
                                }
                                int Value = int.Parse(SVal);
                                switch (Border) {
                                    case "top":
                                        Top = (int)(Percentage ? ((double)Value / 100) * WSize.Height : Value) * (Negative ? -1 : 1);
                                        break;
                                    case "bottom":
                                        Bottom = (int)(Percentage ? ((double)Value / 100) * WSize.Height : Value) * (Negative ? -1 : 1);
                                        break;
                                    case "rigth":
                                        Right = (int)(Percentage ? ((double)Value / 100) * WSize.Width : Value) * (Negative ? -1 : 1);
                                        break;
                                    case "left":
                                        Left = (int)(Percentage ? ((double)Value / 100) * WSize.Width : Value) * (Negative ? -1 : 1);
                                        break;
                                }
                            }
                            WindowDiff = new Padding(Left, Top, Right, Bottom);
                            UpdateWindow(HookHandler);
                            break;
                        case "font size":
                        case "set font size":
                        case "font resize":
                        case "resize font":
                            DefaultInstance.Font = new Font(DefaultInstance.Font.FontFamily, float.Parse(CMDV), DefaultInstance.Font.Style);
                            break;
                        case "auto font size":
                        case "auto resize text":
                        case "auto font resize":
                        case "resize text":
                            DefaultInstance.AutoSize = CMDV.ToLower() == "true";
                            break;
                        case "set dock":
                        case "change dock":
                        case "dock at":
                        case "dock":
                            switch (CMDV.Trim().ToLower()) {
                                case "bottom":
                                    Dock = DockStyle.Bottom;
                                    break;
                                case "top":
                                    Dock = DockStyle.Top;
                                    break;
                                case "center":
                                    Dock = DockStyle.Fill;
                                    break;
                            }
                            break;
                        case "text only":
                        case "no background":
                        case "subtitle mode":
                        case "hide background":
                            string Text = DefaultInstance?.Text;
                            DefaultInstance?.Close();
                            DefaultInstance?.Dispose();
                            TextOnly = CMDV.ToLower() == "true";
                            DefaultInstance.Show();
                            DefaultInstance.Text = Text;
                            UpdateWindow(HookHandler);
                            break;
                        case "centralize":
                        case "center":
                        case "align":
                        case "alignment":
                            bool Center = CMDV.ToLower() == "true";
                            DefaultInstance.TextAlignment = (Center ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft);
                            break;
                        case "hook text":
                        case "enable hook":
                        case "auto show":
                        case "auto hook":
                            HookText = true;
                            break;
                        case "call event":
                        case "call":
                        case "run event":
                        case "run":
                            TriggerEvent(uint.Parse(CMDV));
                            break;
                        case "call async":
                        case "async":
                        case "async invoke":
                        case "invoke async":
                            new Thread(() => { TriggerEvent(uint.Parse(CMDV)); }).Start();
                            break;
                        case "wait":
                        case "sleep":
                        case "delay":
                        case "suspend":
                            Thread.Sleep(int.Parse(CMDV));
                            break;
                        case "continue to event":
                        case "jump to event":
                        case "go to":
                        case "jump":
                            TriggerEvent(uint.Parse(CMDV));
                            return;
                        case "end event":
                        case "event end":
                        case "return":
                        case "die":
                            return;
                    }
                } catch (Exception ex) {
#if DEBUG
                    MessageBox.Show(ex.ToString());
#endif
                }
            }
        }

        internal static Color LoadColor(string ColorName, Color Default) {
            try {
                if (ColorName.StartsWith("#")) {
                    string Hex = ColorName.Trim('#', ' ');
                    int A, R, G, B;
                    switch (Hex.Length) {
                        case 3:
                            R = Convert.ToInt32(Hex[0].ToString() + Hex[0].ToString(), 16);
                            G = Convert.ToInt32(Hex[1].ToString() + Hex[1].ToString(), 16);
                            B = Convert.ToInt32(Hex[2].ToString() + Hex[2].ToString(), 16);
                            return Color.FromArgb(0xFF, R, G, B);
                        case 4:
                            A = Convert.ToInt32(Hex[0].ToString() + Hex[0].ToString(), 16);
                            R = Convert.ToInt32(Hex[1].ToString() + Hex[1].ToString(), 16);
                            G = Convert.ToInt32(Hex[2].ToString() + Hex[2].ToString(), 16);
                            B = Convert.ToInt32(Hex[3].ToString() + Hex[3].ToString(), 16);
                            return Color.FromArgb(A, R, G, B);
                        case 6:
                            R = Convert.ToInt32(Hex[0].ToString() + Hex[1].ToString(), 16);
                            G = Convert.ToInt32(Hex[2].ToString() + Hex[3].ToString(), 16);
                            B = Convert.ToInt32(Hex[4].ToString() + Hex[5].ToString(), 16);
                            return Color.FromArgb(0xFF, R, G, B);
                        case 8:
                            A = Convert.ToInt32(Hex[0].ToString() + Hex[1].ToString(), 16);
                            R = Convert.ToInt32(Hex[2].ToString() + Hex[3].ToString(), 16);
                            G = Convert.ToInt32(Hex[4].ToString() + Hex[5].ToString(), 16);
                            B = Convert.ToInt32(Hex[6].ToString() + Hex[7].ToString(), 16);
                            return Color.FromArgb(A, R, G, B);
                    }

                } else if (ColorName.Contains(";")) {
                    string[] Array = ColorName.Replace(@" ", "").Split(';');
                    int A, R, G, B;
                    switch (Array.Length) {
                        case 3:
                            R = int.Parse(Array[0].Trim());
                            G = int.Parse(Array[1].Trim());
                            B = int.Parse(Array[2].Trim());
                            return Color.FromArgb(0xFF, R, G, B);
                        case 4:
                            A = int.Parse(Array[0].Trim());
                            R = int.Parse(Array[1].Trim());
                            G = int.Parse(Array[2].Trim());
                            B = int.Parse(Array[3].Trim());
                            return Color.FromArgb(A, R, G, B);
                    }
                } else
                    return Color.FromName(ColorName);
            } catch { }

            return Default;
        }

    }
}

#pragma warning restore 1591