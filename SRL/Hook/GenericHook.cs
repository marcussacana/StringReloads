using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static SRL.StringReloader;

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
            dCreateFontA = new CreateFontADelegate(hCreateFontA);
            dCreateFontW = new CreateFontWDelegate(hCreateFontW);

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
                    char OC = (char)ParsePtr(RestoreChar(new IntPtr(C)));
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
                    char OC = (char)ParsePtr(RestoreChar(new IntPtr(C)));
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


        static IntPtr hCreateFontW(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace) {
            if (!string.IsNullOrEmpty(FontFaceName))
                lpszFace = FontFaceName;

            if (FontCharset != 0)
                fdwCharSet = FontCharset;


#if DEBUG
            if (Debugging)
                Log("CreateFontW Hooked, {0} 0x{1:X2}", true, lpszFace, fdwCharSet);
#endif

            hCreatFontW.Uninstall();
            var Result = CreateFontW(nHeight, nWidth, nEscapement, nOrientation, fnWeight, fdwItalic, fdwUnderline, fdwStrikeOut, fdwCharSet, fdwOutputPrecision, fdwClipPrecision, fdwQuality, fdwPitchAndFamily, lpszFace);
            hCreatFontW.Install();

            return Result;
        }


        static IntPtr hCreateFontA(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, [MarshalAs(UnmanagedType.LPTStr)] string lpszFace) {
            if (!string.IsNullOrEmpty(FontFaceName))
                lpszFace = FontFaceName;

            if (FontCharset != 0)
                fdwCharSet = FontCharset;


#if DEBUG
            if (Debugging)
                Log("CreateFontA Hooked, {0} 0x{1:X2}", true, lpszFace, fdwCharSet);
#endif

            hCreatFontA.Uninstall();
            var Result = CreateFontA(nHeight, nWidth, nEscapement, nOrientation, fnWeight, fdwItalic, fdwUnderline, fdwStrikeOut, fdwCharSet, fdwOutputPrecision, fdwClipPrecision, fdwQuality, fdwPitchAndFamily, lpszFace);
            hCreatFontA.Install();

            return Result;
        }
        static IntPtr hCreateFontIndirectA(ref LOGFONTA FontInfo) {
            if (!string.IsNullOrEmpty(FontFaceName))
                FontInfo.lfFaceName = FontFaceName;

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
            if (!string.IsNullOrEmpty(FontFaceName))
                FontInfo.lfFaceName = FontFaceName;

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
    
    }
}