using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SRL {
    partial class StringReloader {

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern IntPtr CreateFontIndirectA([In] ref LOGFONTA FontInfo);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr CreateFontIndirectW([In] ref LOGFONTW FontInfo);

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern IntPtr CreateFontA(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CreateFontW(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern uint GetGlyphOutlineA(IntPtr hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetGlyphOutlineW(IntPtr hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2);

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi)]
        static extern bool TextOutA(IntPtr hdc, int nXStart, int nYStart, [MarshalAs(UnmanagedType.LPStr)] string lpString, int cbString);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        static extern bool TextOutW(IntPtr hdc, int nXStart, int nYStart, [MarshalAs(UnmanagedType.LPWStr)] string lpString, int cbString);

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi)]
        static extern bool ExtTextOutA(IntPtr hdc, int X, int Y, uint fuOptions, [In] ref RECT lprc, [MarshalAs(UnmanagedType.LPStr)] string lpString, uint cbCount, [In] IntPtr lpDx);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        static extern bool ExtTextOutW(IntPtr hdc, int X, int Y, uint fuOptions, [In] ref RECT lprc, [MarshalAs(UnmanagedType.LPWStr)] string lpString, uint cbCount, [In] IntPtr lpDx);

        [DllImport("kernel32.dll")]
        public static extern bool IsBadCodePtr(IntPtr Ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);        

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern int EnumWindows(CallBack x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, CallBack lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hWnd);


        [DllImport(@"kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport(@"kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, ref MENUITEMINFO lpmii);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, [In] ref MENUITEMINFO lpmii);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr rec, IntPtr recptr, uint Flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MENUITEMINFO {
            public uint cbSize;
            public uint fMask;
            public uint fType;
            public uint fState;
            public uint wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public string dwTypeData;
            public uint cch;
            public IntPtr hbmpItem;

            // return the size of the structure
            public static uint SizeOf {
                get { return (uint)Marshal.SizeOf(typeof(MENUITEMINFO)); }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TTPOLYGONHEADER {

            public int cb;
            public int dwType;
            [MarshalAs(UnmanagedType.Struct)] public POINTFX pfxStart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TTPOLYCURVEHEADER {
            public short wType;
            public short cpfx;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FIXED {
            public short fract;
            public short value;
        }


        public struct MAT2 {
            [MarshalAs(UnmanagedType.Struct)] public FIXED eM11;
            [MarshalAs(UnmanagedType.Struct)] public FIXED eM12;
            [MarshalAs(UnmanagedType.Struct)] public FIXED eM21;
            [MarshalAs(UnmanagedType.Struct)] public FIXED eM22;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTFX {
            [MarshalAs(UnmanagedType.Struct)] public FIXED x;
            [MarshalAs(UnmanagedType.Struct)] public FIXED y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GLYPHMETRICS {
            public int gmBlackBoxX;
            public int gmBlackBoxY;
            [MarshalAs(UnmanagedType.Struct)] public POINT gmptGlyphOrigin;
            public short gmCellIncX;
            public short gmCellIncY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom) {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r) {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r) {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2) {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2) {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r) {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj) {
                if (obj is RECT)
                    return Equals((RECT)obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public override int GetHashCode() {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString() {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }

        public const uint WM_SETTEXT = 0x000C;
        

        public const uint MIM_MAXHEIGHT = 0x00000001;
        public const uint MIM_BACKGROUND = 0x00000002;
        public const uint MIM_HELPID = 0x00000004;
        public const uint MIM_MENUDATA = 0x00000008;
        public const uint MIM_STYLE = 0x00000010;
        public const uint MIM_APPLYTOSUBMENUS = 0x80000000;
        public const uint MIIM_STRING = 0x00000040;
        public const uint MFT_STRING = 0x00000000;
        public const uint MIIM_SUBMENU = 0x00000004;
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_ERASE = 0x0004;



        public delegate bool CallBack(IntPtr hwnd, int lParam);
    }
}
