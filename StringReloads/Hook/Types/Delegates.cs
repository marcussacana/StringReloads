using System.Runtime.InteropServices;

namespace StringReloads.Hook
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate void* CreateFontADelegate(uint nHeight, uint nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate void* CreateFontWDelegate(uint nHeight, uint nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

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


    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate void* CreateFileADelegate(string FileName, EFileAccess Access, EFileShare Share, void* SecurityAttributes, ECreationDisposition CreationDisposition, EFileAttributes FlagsAndAttributes, void* TemplateFile);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate void* CreateFileWDelegate(string FileName, EFileAccess Access, EFileShare Share, void* SecurityAttributes, ECreationDisposition CreationDisposition, EFileAttributes FlagsAndAttributes, void* TemplateFile);

}
