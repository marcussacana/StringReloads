using System;
using System.Runtime.InteropServices;

namespace SRL {
    static partial class StringReloader {

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

        static FxHook OutlineA;
        static FxHook OutlineW;
        static FxHook hTextOutA;
        static FxHook hTextOutW;
        static FxHook hExtTextOutA;
        static FxHook hExtTextOutW;
        static FxHook hCreatFontA;
        static FxHook hCreatFontW;
        static FxHook hCreatFontIndirectA;
        static FxHook hCreatFontIndirectW;
#if DEBUG
        static FxHook hSendMessageA;
        static FxHook hSendMessageW;
        static FxHook hCreateWindowExA;
        static FxHook hCreateWindowExW;
        static FxHook hCreateWindowA;
        static FxHook hCreateWindowW;
#endif
        static FxHook hSetWindowTextA;
        static FxHook hSetWindowTextW;


        static FxHook hMultiByteToWideChar;

        static void InstallGlyphHooks() {
            dOutlineA = new GetGlyphOutlineDelegate(hGetGlyphOutlineA);
            dOutlineW = new GetGlyphOutlineDelegate(hGetGlyphOutlineW);

            OutlineA = new FxHook("gdi32.dll", "GetGlyphOutlineA", dOutlineA);
            OutlineW = new FxHook("gdi32.dll", "GetGlyphOutlineW", dOutlineW);

            OutlineA.Install();
            OutlineW.Install();
        }

        static void InstallTextOutHooks() {
            dTextOutA = new TextOutADelegate(hTextOut);
            dTextOutW = new TextOutWDelegate(hTextOut);

            hTextOutA = new FxHook("gdi32.dll", "TextOutA", dTextOutA);
            hTextOutW = new FxHook("gdi32.dll", "TextOutW", dTextOutW);

            hTextOutA.Install();
            hTextOutW.Install();
        }
        static void InstallExtTextOutHooks() {
            dExtTextOutA = new ExtTextOutADelegate(hExtTextOut);
            dExtTextOutW = new ExtTextOutWDelegate(hExtTextOut);

            hExtTextOutA = new FxHook("gdi32.dll", "ExtTextOutA", dExtTextOutA);
            hExtTextOutW = new FxHook("gdi32.dll", "ExtTextOutW", dExtTextOutW);

            hExtTextOutA.Install();
            hExtTextOutW.Install();
        }

        static void InstallCreateFontHooks() {
            dCreateFontA = new CreateFontADelegate(hCreateFont);
            dCreateFontW = new CreateFontWDelegate(hCreateFont);

            hCreatFontA = new FxHook("gdi32.dll", "CreateFontA", dCreateFontA);
            hCreatFontW = new FxHook("gdi32.dll", "CreateFontW", dCreateFontW);

            hCreatFontA.Install();
            hCreatFontW.Install();
        }

        static void InstallCreateFontIndirectHooks() {
            dCreateFontIndirectA = new CreateFontIndirectADelegate(hCreateFontIndirectA);
            dCreateFontIndirectW = new CreateFontIndirectWDelegate(hCreateFontIndirectW);


            hCreatFontIndirectA = new FxHook("gdi32.dll", "CreateFontIndirectA", dCreateFontIndirectA);
            hCreatFontIndirectW = new FxHook("gdi32.dll", "CreateFontIndirectW", dCreateFontIndirectW);

            hCreatFontIndirectA.Install();
            hCreatFontIndirectW.Install();
        }

#if DEBUG
        static void InstallSendMessageHooks() {
            dSendMessageA = new SendMessageADelegate(SendMessageAHook);
            dSendMessageW = new SendMessageWDelegate(SendMessageWHook);

            hSendMessageA = new FxHook("user32.dll", "SendMessageA", dSendMessageA);
            hSendMessageW = new FxHook("user32.dll", "SendMessageW", dSendMessageW);

            hSendMessageA.Install();
            hSendMessageW.Install();
        }

        static void InstallCreateWindowHooks() {
            dCreateWindowA = new CreateWindowADelegate(CreateWindow);
            dCreateWindowW = new CreateWindowWDelegate(CreateWindow);

            hCreateWindowA = new FxHook("user32.dll", "CreateWindowA", dCreateWindowA);
            hCreateWindowW = new FxHook("user32.dll", "CreateWindowW", dCreateWindowW);

            hCreateWindowA.Install();
            hCreateWindowW.Install();
        }

        static void InstallCreateWindowExHooks() {
            dCreateWindowExA = new CreateWindowExADelegate(CreateWindowEx);
            dCreateWindowExW = new CreateWindowExWDelegate(CreateWindowEx);

            hCreateWindowExA = new FxHook("user32.dll", "CreateWindowExA", dCreateWindowExA);
            hCreateWindowExW = new FxHook("user32.dll", "CreateWindowExW", dCreateWindowExW);

            hCreateWindowExA.Install();
            hCreateWindowExW.Install();
        }
#endif

        static void InstallSetWindowTextHooks() {
            dSetWindowTextA = new SetWindowTextADelegate(SetWindowTextHook);
            dSetWindowTextW = new SetWindowTextWDelegate(SetWindowTextHook);

            hSetWindowTextA = new FxHook("user32.dll", "SetWindowTextA", dSetWindowTextA);
            hSetWindowTextW = new FxHook("user32.dll", "SetWindowTextW", dSetWindowTextW);

            hSetWindowTextA.Install();
            hSetWindowTextW.Install();
        }

        static void InstallMultiByteToWideChar() {
            dMultiByteToWideChar = new MultiByteToWideCharDelegate(MultiByteToWideCharHook);

            hMultiByteToWideChar = new FxHook("kernel32.dll", "MultiByteToWideChar", dMultiByteToWideChar);

            hMultiByteToWideChar.Install();
        }

        public static uint hGetGlyphOutlineA(IntPtr hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2) {
            uChar = (uint)ParsePtr(ProcessReal(new IntPtr((int)uChar)));


#if DEBUG
            if (Debugging)
                Log("OutlineA Hooked, {0:X4}", true, uChar);
#endif

            OutlineA.Uninstall();
            uint Ret = GetGlyphOutlineA(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);
            OutlineA.Install();
            return Ret;
        }
        public static uint hGetGlyphOutlineW(IntPtr hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, IntPtr lpvBuffer, ref MAT2 lpmat2) {
            uChar = (uint)ParsePtr(ProcessReal(new IntPtr((int)uChar)));

#if DEBUG
            if (Debugging)
                Log("OutlineW Hooked, {0:X4}", true, uChar);
#endif

            OutlineW.Uninstall();
            uint Ret = GetGlyphOutlineW(hdc, uChar, uFormat, out lpgm, cbBuffer, lpvBuffer, ref lpmat2);
            OutlineW.Install();
            return Ret;
        }

        public static bool hTextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString) {
            lpString = ProcessManaged(lpString, false);
            if (UndoChars) {
                for (int i = 0; i < lpString.Length; i++) {
                    char C = lpString[i];
                    char OC = RestoreChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            } else {
                for (int i = 0; i < lpString.Length; i++) {
                    char C = lpString[i];
                    char OC = ProcessChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            }

#if DEBUG
            if (Debugging)
                Log("TextOut Hooked, {0}", true, lpString);
#endif

            hTextOutW.Uninstall();
            bool Rst = TextOutW(hdc, nXStart, nYStart, lpString, cbString);
            hTextOutW.Install();
            return Rst;
        }

        public static bool hExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions, ref RECT lprc, string lpString, uint cbCount, IntPtr lpDx) {
            lpString = ProcessManaged(lpString, false);
            if (UndoChars) {
                for (int i = 0; i < lpString.Length; i++) {
                    char C = lpString[i];
                    char OC = RestoreChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            } else {
                for (int i = 0; i < lpString.Length; i++) {
                    char C = lpString[i];
                    char OC = ProcessChar(C);
                    if (OC != C)
                        lpString = lpString.Replace(C, OC);
                }
            }

#if DEBUG
            if (Debugging)
                Log("ExtTextOut Hooked, {0}", true, lpString);
#endif

            hExtTextOutW.Uninstall();
            bool Rst = ExtTextOutW(hdc, X, Y, fuOptions, ref lprc, lpString, cbCount, lpDx);
            hExtTextOutW.Install();
            return Rst;
        }


        static IntPtr hCreateFont(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace) {
            if (RedirFontSize(lpszFace) != null) {
                string Change = RedirFontSize(lpszFace);
                if (Change.StartsWith("+")) {
                    nWidth += int.Parse(Change);
                } else {
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


#if DEBUG
            if (Debugging)
                Log("CreateFont Hooked, {0} 0x{1:X2}", true, lpszFace, fdwCharSet);
#endif

            hCreatFontW.Uninstall();
            var Result = CreateFontW(nHeight, nWidth, nEscapement, nOrientation, fnWeight, fdwItalic, fdwUnderline, fdwStrikeOut, fdwCharSet, fdwOutputPrecision, fdwClipPrecision, fdwQuality, fdwPitchAndFamily, lpszFace);
            hCreatFontW.Install();

            return Result;
        }

        static IntPtr hCreateFontIndirectA(ref LOGFONTA FontInfo) {
            if (RedirFontSize(FontInfo.lfFaceName) != null) {
                string Change = RedirFontSize(FontInfo.lfFaceName);
                if (Change.StartsWith("+")) {
                    FontInfo.lfWidth += int.Parse(Change);
                } else {
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


#if DEBUG
            if (Debugging)
                Log("CreateIndirectA Hooked, {0} 0x{1:X2}", true, FontInfo.lfFaceName, FontInfo.lfCharSet);
#endif

            hCreatFontIndirectA.Uninstall();
            var Result = CreateFontIndirectA(ref FontInfo);
            hCreatFontIndirectA.Install();

            return Result;
        }
        static IntPtr hCreateFontIndirectW(ref LOGFONTW FontInfo) {
            if (RedirFontSize(FontInfo.lfFaceName) != null) {
                string Change = RedirFontSize(FontInfo.lfFaceName);
                if (Change.StartsWith("+")) {
                    FontInfo.lfWidth += int.Parse(Change);
                } else {
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


#if DEBUG
            if (Debugging)
                Log("CreateIndirectW Hooked, {0} 0x{1:X2}", true, FontInfo.lfFaceName, FontInfo.lfCharSet);
#endif

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
            hSendMessageA.Uninstall();
            var Rst = SendMessageA(hWnd, Msg, wParam, lParam);
            hSendMessageA.Install();
            return Rst;
        }

        static IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam) {
            if (Debugging)
                Log("CreateWindowEx Hooked, {0}", true, lpWindowName);

            string Reload = StrMap(lpWindowName, IntPtr.Zero, true);

            hCreateWindowExW.Uninstall();
            var Rst = CreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
            hCreateWindowExW.Install();
            return Rst;
        }
        static IntPtr CreateWindow(string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam) {
            if (Debugging)
                Log("CreateWindow Hooked, {0}", true, lpWindowName);

            string Reload = StrMap(lpWindowName, IntPtr.Zero, true);

            hCreateWindowExW.Uninstall();
            var Rst = CreateWindowW(lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
            hCreateWindowExW.Install();
            return Rst;
        }
#endif
        static bool SetWindowTextHook(IntPtr hwnd, string lpString) { 
            lpString = StrMap(lpString, IntPtr.Zero, true);

            hSetWindowTextW.Uninstall();
            var Ret = SetWindowTextW(hwnd, lpString);
            hSetWindowTextW.Install();
            return Ret;
        }

        static int MultiByteToWideCharHook(int Codepage, uint dwFlags, IntPtr Input, int cbMultiByte, IntPtr Output, int cchWideChar) {
            hMultiByteToWideChar.Uninstall();
            if (!Initialized || cbMultiByte == 0) {
                int Rst = MultiByteToWideChar(Codepage, dwFlags, Input, cbMultiByte, Output, cchWideChar);
                hMultiByteToWideChar.Install();
                return Rst;
            }
            string Str = GetString(Input, Len: cbMultiByte == -1 ? null : (int?)cbMultiByte, CP: Codepage);
            string RStr = Str;
            RStr = StrMap(Str, Input, true);

            if (RStr == Str) {
                int Rst = MultiByteToWideChar(Codepage, dwFlags, Input, cbMultiByte, Output, cchWideChar);
                hMultiByteToWideChar.Install();
                return Rst;
            }

            Log("MBTWC Hook: {0}", true, RStr);

            Output = GenString(RStr, true, Output == IntPtr.Zero ? null : (IntPtr?)Output);

            hMultiByteToWideChar.Install();
            return cchWideChar;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct LOGFONTA {
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
        struct LOGFONTW {
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