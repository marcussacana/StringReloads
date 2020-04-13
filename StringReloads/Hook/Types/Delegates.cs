using System;
using System.Runtime.InteropServices;

namespace StringReloads.Hook
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate void* CreateFontADelegate(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, int fdwItalic, int fdwUnderline, int fdwStrikeOut, uint fdwCharSet, int fdwOutputPrecision, int fdwClipPrecision, int fdwQuality, int fdwPitchAndFamily, string lpszFace);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate void* CreateFontWDelegate(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, int fdwItalic, int fdwUnderline, int fdwStrikeOut, uint fdwCharSet, int fdwOutputPrecision, int fdwClipPrecision, int fdwQuality, int fdwPitchAndFamily, string lpszFace);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate void* CreateFontIndirectADelegate(ref LOGFONTA lplf);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate void* CreateFontIndirectWDelegate(ref LOGFONTW lplf);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate uint GetGlyphOutlineADelegate(void* hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, byte* lpvBuffer, ref MAT2 lpmat2);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate uint GetGlyphOutlineWDelegate(void* hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, byte* lpvBuffer, ref MAT2 lpmat2);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate void* SysAllocStringDelegate(void* pStr);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate void* GetProcAddressDelegate(void* hModule, void* Proc);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate void* CreateFileADelegate(string FileName, EFileAccess Access, EFileShare Share, void* SecurityAttributes, ECreationDisposition CreationDisposition, EFileAttributes FlagsAndAttributes, void* TemplateFile);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate void* CreateFileWDelegate(string FileName, EFileAccess Access, EFileShare Share, void* SecurityAttributes, ECreationDisposition CreationDisposition, EFileAttributes FlagsAndAttributes, void* TemplateFile);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void SoftPAL_DrawTextDelegate(byte* Text, void* a2, void* a3, void* a4, void* a5, void* a6, void* a7, void* a8, void* a9, void* a10, void* a11, void* a12, void* a13, void* a14, void* a15, void* a16, void* a17, void* a18, void* a19, void* a20, void* a21, void* a22, void* a23);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate int MultiByteToWideCharDelegate(uint CodePage, uint dwFlags, byte* lpMultiByteStr, int cbMultiByte, char* lpWideCharStr, int cchWideChar);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate int WideCharToMultiByteDelegate(uint CodePage, uint dwFlags, byte* lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, byte* lpDefaultChar, out bool lpUsedDefaultChar);
}
