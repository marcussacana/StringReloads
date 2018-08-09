using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SRL {
    static partial class StringReloader {

        static GetGlyphOutlineDelegate dOutlineA = null;
        static GetGlyphOutlineDelegate dOutlineW = null;
        static TextOutADelegate dTextOutA = null;
        static TextOutWDelegate dTextOutW = null;
        static ExtTextOutADelegate dExtTextOutA = null;
        static ExtTextOutWDelegate dExtTextOutW = null;

        static FxHook OutlineA;
        static FxHook OutlineW;
        static FxHook hTextOutA;
        static FxHook hTextOutW;
        static FxHook hExtTextOutA;
        static FxHook hExtTextOutW;

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
    }
}
