using System;
using System.Runtime.InteropServices;

namespace SRL
{
    static partial class StringReloader
    {

        static GetGlyphOutlineDelegate dOutlineA = null;
        static GetGlyphOutlineDelegate dOutlineW = null;
        static TextOutADelegate dTextOutA = null;
        static TextOutWDelegate dTextOutW = null;
        static ExtTextOutADelegate dExtTextOutA = null;
        static ExtTextOutWDelegate dExtTextOutW = null;
        static CreateFontADelegate dCreateFontA = null;
        static CreateFontWDelegate dCreateFontW = null;
        static CreateFontIndirectADelegate dCreateFontIndirectA = null;
        static CreateFontIndirectWDelegate dCreateFontIndirectW = null;
        static MultiByteToWideCharDelegate dMultiByteToWideChar = null;
#if DEBUG
        static SendMessageADelegate dSendMessageA = null;
        static SendMessageWDelegate dSendMessageW = null;
        static CreateWindowExADelegate dCreateWindowExA = null;
        static CreateWindowExWDelegate dCreateWindowExW = null;
        static CreateWindowADelegate dCreateWindowA = null;
        static CreateWindowWDelegate dCreateWindowW = null;
#endif
        static SetWindowTextADelegate dSetWindowTextA;
        static SetWindowTextWDelegate dSetWindowTextW;

        static UnmanagedHook OutlineA;
        static UnmanagedHook OutlineW;
        static UnmanagedHook hTextOutA;
        static UnmanagedHook hTextOutW;
        static UnmanagedHook hExtTextOutA;
        static UnmanagedHook hExtTextOutW;
        static UnmanagedHook hCreateFontA;
        static UnmanagedHook hCreateFontW;
        static UnmanagedHook hCreatFontIndirectA;
        static UnmanagedHook hCreatFontIndirectW;
#if DEBUG
        static UnmanagedHook hSendMessageA;
        static UnmanagedHook hSendMessageW;
        static UnmanagedHook hCreateWindowExA;
        static UnmanagedHook hCreateWindowExW;
        static UnmanagedHook hCreateWindowA;
        static UnmanagedHook hCreateWindowW;
#endif
        static UnmanagedHook hSetWindowTextA;
        static UnmanagedHook hSetWindowTextW;


        static UnmanagedHook hMultiByteToWideChar;

        static void InstallGlyphHooks()
        {
            if (Managed)
                return;

            dOutlineA = new GetGlyphOutlineDelegate(hGetGlyphOutlineA);
            dOutlineW = new GetGlyphOutlineDelegate(hGetGlyphOutlineW);

            OutlineA = AutoHookCreator("gdi32.dll", "GetGlyphOutlineA", dOutlineA);
            OutlineW = AutoHookCreator("gdi32.dll", "GetGlyphOutlineW", dOutlineW);

            OutlineA.Install();
            OutlineW.Install();
        }

        static void InstallTextOutHooks()
        {
            if (Managed)
                return;

            dTextOutA = new TextOutADelegate(hTextOut);
            dTextOutW = new TextOutWDelegate(hTextOut);

            hTextOutA = AutoHookCreator("gdi32.dll", "TextOutA", dTextOutA);
            hTextOutW = AutoHookCreator("gdi32.dll", "TextOutW", dTextOutW);

            hTextOutA.Install();
            hTextOutW.Install();
        }

        static void InstallExtTextOutHooks()
        {
            if (Managed)
                return;

            dExtTextOutA = new ExtTextOutADelegate(hExtTextOut);
            dExtTextOutW = new ExtTextOutWDelegate(hExtTextOut);

            hExtTextOutA = AutoHookCreator("gdi32.dll", "ExtTextOutA", dExtTextOutA);
            hExtTextOutW = AutoHookCreator("gdi32.dll", "ExtTextOutW", dExtTextOutW);

            hExtTextOutA.Install();
            hExtTextOutW.Install();
        }

        static void InstallCreateFontHooks()
        {
            if (Managed)
                return;

            dCreateFontA = new CreateFontADelegate(hCreateFont);
            dCreateFontW = new CreateFontWDelegate(hCreateFont);

            hCreateFontA = AutoHookCreator("gdi32.dll", "CreateFontA", dCreateFontA);
            hCreateFontW = AutoHookCreator("gdi32.dll", "CreateFontW", dCreateFontW);

            hCreateFontA.Install();
            hCreateFontW.Install();
        }

        static void InstallCreateFontIndirectHooks()
        {
            if (Managed)
                return;

            dCreateFontIndirectA = new CreateFontIndirectADelegate(hCreateFontIndirectA);
            dCreateFontIndirectW = new CreateFontIndirectWDelegate(hCreateFontIndirectW);


            hCreatFontIndirectA = AutoHookCreator("gdi32.dll", "CreateFontIndirectA", dCreateFontIndirectA);
            hCreatFontIndirectW = AutoHookCreator("gdi32.dll", "CreateFontIndirectW", dCreateFontIndirectW);

            hCreatFontIndirectA.Install();
            hCreatFontIndirectW.Install();
        }

#if DEBUG
        static void InstallSendMessageHooks() {
            if (Managed)
                return;
            dSendMessageA = new SendMessageADelegate(SendMessageAHook);
            dSendMessageW = new SendMessageWDelegate(SendMessageWHook);

            hSendMessageA = AutoHookCreator("user32.dll", "SendMessageA", dSendMessageA);
            hSendMessageW = AutoHookCreator("user32.dll", "SendMessageW", dSendMessageW);

            hSendMessageA.Install();
            hSendMessageW.Install();
        }

        static void InstallCreateWindowHooks() {
            if (Managed)
                return;
            dCreateWindowA = new CreateWindowADelegate(CreateWindow);
            dCreateWindowW = new CreateWindowWDelegate(CreateWindow);

            hCreateWindowA = AutoHookCreator("user32.dll", "CreateWindowA", dCreateWindowA);
            hCreateWindowW = AutoHookCreator("user32.dll", "CreateWindowW", dCreateWindowW);

            hCreateWindowA.Install();
            hCreateWindowW.Install();
        }

        static void InstallCreateWindowExHooks() {
            if (Managed)
                return;
            dCreateWindowExA = new CreateWindowExADelegate(CreateWindowEx);
            dCreateWindowExW = new CreateWindowExWDelegate(CreateWindowEx);

            hCreateWindowExA = AutoHookCreator("user32.dll", "CreateWindowExA", dCreateWindowExA);
            hCreateWindowExW = AutoHookCreator("user32.dll", "CreateWindowExW", dCreateWindowExW);

            hCreateWindowExA.Install();
            hCreateWindowExW.Install();
        }
#endif

        static void InstallSetWindowTextHooks()
        {
            if (Managed)
                return;

            dSetWindowTextA = new SetWindowTextADelegate(SetWindowTextHook);
            dSetWindowTextW = new SetWindowTextWDelegate(SetWindowTextHook);

            hSetWindowTextA = AutoHookCreator("user32.dll", "SetWindowTextA", dSetWindowTextA);
            hSetWindowTextW = AutoHookCreator("user32.dll", "SetWindowTextW", dSetWindowTextW);

            hSetWindowTextA.Install();
            hSetWindowTextW.Install();
        }

        static void InstallMultiByteToWideChar()
        {
            if (Managed)
                return;

            dMultiByteToWideChar = new MultiByteToWideCharDelegate(MultiByteToWideCharHook);

            hMultiByteToWideChar = AutoHookCreator("kernel32.dll", "MultiByteToWideChar", dMultiByteToWideChar);

            hMultiByteToWideChar.Install();
        }

        public static uint hGetGlyphOutlineA(IntPtr hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2)
        {
            uChar = (uint)ParsePtr(ProcessReal(new IntPtr((int)uChar)));


#if DEBUG
            if (Debugging)
                Log("OutlineA Hooked, {0:X4}", true, uChar);
#endif
            if (OutlineA.ImportHook)
                return GetGlyphOutlineA(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);

            OutlineA.Uninstall();
            uint Ret = GetGlyphOutlineA(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);
            OutlineA.Install();
            return Ret;
        }
        public static uint hGetGlyphOutlineW(IntPtr hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2)
        {
            uChar = (uint)ParsePtr(ProcessReal(new IntPtr((int)uChar)));

#if DEBUG
            if (Debugging)
                Log("OutlineW Hooked, {0:X4}", true, uChar);
#endif

            if (OutlineW.ImportHook)
                return GetGlyphOutlineW(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);

            OutlineW.Uninstall();
            uint Ret = GetGlyphOutlineW(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);
            OutlineW.Install();
            return Ret;
        }

        public static bool hTextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString)
        {
            lpString = Process(lpString);
            if (UndoChars)
            {
                for (int i = 0; i < lpString.Length; i++)
                {
                    char C = lpString[i];
                    char OC = RestoreChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            }
            else
            {
                for (int i = 0; i < lpString.Length; i++)
                {
                    char C = lpString[i];
                    char OC = ProcessChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            }

            if (LogInput)
                Log("TextOut Hooked, {0}", true, lpString);

            if (hTextOutW.ImportHook)
                return TextOutW(hdc, nXStart, nYStart, lpString, cbString);

            hTextOutW.Uninstall();
            bool Rst = TextOutW(hdc, nXStart, nYStart, lpString, cbString);
            hTextOutW.Install();
            return Rst;
        }

        public static bool hExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions, ref RECT lprc, string lpString, uint cbCount, IntPtr lpDx)
        {
            lpString = Process(lpString);
            if (UndoChars)
            {
                for (int i = 0; i < lpString.Length; i++)
                {
                    char C = lpString[i];
                    char OC = RestoreChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            }
            else
            {
                for (int i = 0; i < lpString.Length; i++)
                {
                    char C = lpString[i];
                    char OC = ProcessChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            }


            if (LogInput)
                Log("ExtTextOut Hooked, {0}", true, lpString);


            if (hExtTextOutW.ImportHook)
                return ExtTextOutW(hdc, X, Y, fuOptions, ref lprc, lpString, cbCount, lpDx);

            hExtTextOutW.Uninstall();
            bool Rst = ExtTextOutW(hdc, X, Y, fuOptions, ref lprc, lpString, cbCount, lpDx);
            hExtTextOutW.Install();
            return Rst;
        }


        static IntPtr hCreateFont(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace)
        {
            if (RedirFontSize(lpszFace) != null)
            {
                string Change = RedirFontSize(lpszFace);
                if (Change.StartsWith("+"))
                {
                    nWidth += int.Parse(Change);
                }
                else
                {
                    int Val = int.Parse(Change);
                    if (Val <= 0)
                        nWidth += Val;
                    else
                        nWidth = Val;
                }
            }
            lpszFace = RedirFaceName(lpszFace);

            if (FontCharset != 0)
                fdwCharSet = FontCharset;


            if (LogInput)
                Log("CreateFont Hooked, {0} 0x{1:X2}", true, lpszFace, fdwCharSet);

            if (hCreateFontW.ImportHook)
                return CreateFontW(nHeight, nWidth, nEscapement, nOrientation, fnWeight, fdwItalic, fdwUnderline, fdwStrikeOut, fdwCharSet, fdwOutputPrecision, fdwClipPrecision, fdwQuality, fdwPitchAndFamily, lpszFace);

            hCreateFontW.Uninstall();
            var Result = CreateFontW(nHeight, nWidth, nEscapement, nOrientation, fnWeight, fdwItalic, fdwUnderline, fdwStrikeOut, fdwCharSet, fdwOutputPrecision, fdwClipPrecision, fdwQuality, fdwPitchAndFamily, lpszFace);
            hCreateFontW.Install();

            return Result;
        }

        static IntPtr hCreateFontIndirectA(ref LOGFONTA FontInfo)
        {
            if (RedirFontSize(FontInfo.lfFaceName) != null)
            {
                string Change = RedirFontSize(FontInfo.lfFaceName);
                if (Change.StartsWith("+"))
                {
                    FontInfo.lfWidth += int.Parse(Change);
                }
                else
                {
                    int Val = int.Parse(Change);
                    if (Val <= 0)
                        FontInfo.lfWidth += Val;
                    else
                        FontInfo.lfWidth = Val;
                }
            }
            FontInfo.lfFaceName = RedirFaceName(FontInfo.lfFaceName);


            if (FontCharset != 0)
                FontInfo.lfCharSet = FontCharset;

            if (LogInput)
                Log("CreateIndirectA Hooked, {0} 0x{1:X2}", true, FontInfo.lfFaceName, FontInfo.lfCharSet);

            if (hCreatFontIndirectA.ImportHook)
                return CreateFontIndirectA(ref FontInfo);

            hCreatFontIndirectA.Uninstall();
            var Result = CreateFontIndirectA(ref FontInfo);
            hCreatFontIndirectA.Install();

            return Result;
        }
        static IntPtr hCreateFontIndirectW(ref LOGFONTW FontInfo)
        {
            if (RedirFontSize(FontInfo.lfFaceName) != null)
            {
                string Change = RedirFontSize(FontInfo.lfFaceName);
                if (Change.StartsWith("+"))
                {
                    FontInfo.lfWidth += int.Parse(Change);
                }
                else
                {
                    int Val = int.Parse(Change);
                    if (Val <= 0)
                        FontInfo.lfWidth += Val;
                    else
                        FontInfo.lfWidth = Val;
                }
            }
            FontInfo.lfFaceName = RedirFaceName(FontInfo.lfFaceName);

            if (FontCharset != 0)
                FontInfo.lfCharSet = FontCharset;

            if (LogInput)
                Log("CreateIndirectW Hooked, {0} 0x{1:X2}", true, FontInfo.lfFaceName, FontInfo.lfCharSet);

            if (hCreatFontIndirectW.ImportHook)
                return CreateFontIndirectW(ref FontInfo);

            hCreatFontIndirectW.Uninstall();
            var Result = CreateFontIndirectW(ref FontInfo);
            hCreatFontIndirectW.Install();

            return Result;
        }

#if DEBUG
        static Int32 SendMessageWHook(int hWnd, int Msg, int wParam, IntPtr lParam) {
            if (Msg == WM_SETTEXT) {
                string Text = GetStringW(lParam, ForceUnicode: true);

                if (Debugging)
                    Log("SendMessage Hooked, WM_SETTEXT: {0}", true, Text);

                string Reload = StrMap(Text, IntPtr.Zero, true);
                if (Text != Reload) {
                    lParam = GenString(Reload, true);
                }
            }

            if (hSendMessageW.ImportHook)
                return SendMessageW(hWnd, Msg, wParam, lParam);

            hSendMessageW.Uninstall();
            var Rst = SendMessageW(hWnd, Msg, wParam, lParam);
            hSendMessageW.Install();
            return Rst;
        }
        static Int32 SendMessageAHook(int hWnd, int Msg, int wParam, IntPtr lParam) {
            if (Msg == WM_SETTEXT) {
                string Text = GetStringA(lParam);

                if (Debugging)
                    Log("WM_SETTEXT: {0}", true, Text);

                string Reload = StrMap(Text, IntPtr.Zero, true);
                if (Text != Reload) {
                    lParam = GenString(Reload);
                }
            }

            if (hSendMessageA.ImportHook)
                return SendMessageA(hWnd, Msg, wParam, lParam);

            hSendMessageA.Uninstall();
            var Rst = SendMessageA(hWnd, Msg, wParam, lParam);
            hSendMessageA.Install();
            return Rst;
        }

        static IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam) {
            if (Debugging)
                Log("CreateWindowEx Hooked, {0}", true, lpWindowName);

            string Reload = StrMap(lpWindowName, IntPtr.Zero, true);

            if (hCreateWindowExW.ImportHook)
                return CreateWindowExW(dwExStyle, lpClassName, Reload, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

            hCreateWindowExW.Uninstall();
            var Rst = CreateWindowExW(dwExStyle, lpClassName, Reload, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
            hCreateWindowExW.Install();
            return Rst;
        }
        static IntPtr CreateWindow(string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam) {
            if (Debugging)
                Log("CreateWindow Hooked, {0}", true, lpWindowName);

            string Reload = StrMap(lpWindowName, IntPtr.Zero, true);

            if (hCreateWindowExW.ImportHook)
                return CreateWindowW(lpClassName, Reload, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

            hCreateWindowExW.Uninstall();
            var Rst = CreateWindowW(lpClassName, Reload, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
            hCreateWindowExW.Install();
            return Rst;
        }
#endif
        static bool SetWindowTextHook(IntPtr hwnd, string lpString)
        {
            lpString = StrMap(lpString, IntPtr.Zero, true);

            if (hSetWindowTextW.ImportHook)
                return SetWindowTextW(hwnd, lpString);

            hSetWindowTextW.Uninstall();
            var Ret = SetWindowTextW(hwnd, lpString);
            hSetWindowTextW.Install();
            return Ret;
        }

        static int MultiByteToWideCharHook(int Codepage, uint dwFlags, IntPtr Input, int cbMultiByte, IntPtr Output, int cchWideChar)
        {

            if (!hMultiByteToWideChar.ImportHook)
                hMultiByteToWideChar.Uninstall();

            if (!Initialized || cbMultiByte == 0)
            {
                int Rst = MultiByteToWideChar(Codepage, dwFlags, Input, cbMultiByte, Output, cchWideChar);
                if (!hMultiByteToWideChar.ImportHook)
                    hMultiByteToWideChar.Install();
                return Rst;
            }
            string Str = GetString(Input, Len: cbMultiByte == -1 ? null : (int?)cbMultiByte, CP: Codepage);
            string RStr = Str;
            RStr = StrMap(Str, Input, true);

            if (RStr == Str)
            {
                int Rst = MultiByteToWideChar(Codepage, dwFlags, Input, cbMultiByte, Output, cchWideChar);
                if (!hMultiByteToWideChar.ImportHook)
                    hMultiByteToWideChar.Install();
                return Rst;
            }

            if (LogInput)
                Log("MBTWC Hook: {0}", true, RStr);

            Output = GenString(RStr, Input, true, Output == IntPtr.Zero ? null : (IntPtr?)Output);

            if (!hMultiByteToWideChar.ImportHook)
                hMultiByteToWideChar.Install();

            return cchWideChar;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct LOGFONTA
        {
            public const int LF_FACESIZE = 32;
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
            public string lfFaceName;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct LOGFONTW
        {
            public const int LF_FACESIZE = 32;
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
            public string lfFaceName;

        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate bool TextOutADelegate(IntPtr hdc, int nXStart, int nYStart, [MarshalAs(UnmanagedType.LPStr)] string lpString, int cbString);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate bool TextOutWDelegate(IntPtr hdc, int nXStart, int nYStart, [MarshalAs(UnmanagedType.LPWStr)] string lpString, int cbString);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate bool ExtTextOutADelegate(IntPtr hdc, int X, int Y, uint fuOptions, [In] ref RECT lprc, [MarshalAs(UnmanagedType.LPStr)] string lpString, uint cbCount, [In] IntPtr lpDx);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate bool ExtTextOutWDelegate(IntPtr hdc, int X, int Y, uint fuOptions, [In] ref RECT lprc, [MarshalAs(UnmanagedType.LPWStr)] string lpString, uint cbCount, [In] IntPtr lpDx);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate uint GetGlyphOutlineDelegate(IntPtr hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr CreateFontIndirectADelegate([In] ref LOGFONTA FontInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr CreateFontIndirectWDelegate([In] ref LOGFONTW FontInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate IntPtr CreateFontADelegate(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, [MarshalAs(UnmanagedType.LPStr)] string lpszFace);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate IntPtr CreateFontWDelegate(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, [MarshalAs(UnmanagedType.LPWStr)] string lpszFace);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int SendMessageADelegate(int hWnd, int Msg, int wParam, IntPtr lParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int SendMessageWDelegate(int hWnd, int Msg, int wParam, IntPtr lParam);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate IntPtr CreateWindowExADelegate(WindowStylesEx dwExStyle, [MarshalAs(UnmanagedType.LPStr)] string lpClassName, [MarshalAs(UnmanagedType.LPStr)] string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate IntPtr CreateWindowExWDelegate(WindowStylesEx dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate IntPtr CreateWindowADelegate([MarshalAs(UnmanagedType.LPStr)] string lpClassName, [MarshalAs(UnmanagedType.LPStr)] string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate IntPtr CreateWindowWDelegate([MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate bool SetWindowTextADelegate(IntPtr hwnd, [MarshalAs(UnmanagedType.LPStr)] string lpString);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate bool SetWindowTextWDelegate(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string lpString);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int MultiByteToWideCharDelegate(int CodePage, uint dwFlags, IntPtr lpMultiByteStr, int cbMultiByte, IntPtr lpWideCharStr, int cchWideChar);
    }
}