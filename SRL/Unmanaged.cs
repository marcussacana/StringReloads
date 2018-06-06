using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SRL {
    partial class StringReloader {
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
