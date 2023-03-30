using StringReloads.Engine.String;
using System;
using System.Runtime.InteropServices;

namespace StringReloads.Hook
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate void* CreateFontADelegate(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, int fdwItalic, int fdwUnderline, int fdwStrikeOut, uint fdwCharSet, int fdwOutputPrecision, int fdwClipPrecision, int fdwQuality, int fdwPitchAndFamily, byte* lpszFace);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate void* CreateFontWDelegate(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, int fdwItalic, int fdwUnderline, int fdwStrikeOut, uint fdwCharSet, int fdwOutputPrecision, int fdwClipPrecision, int fdwQuality, int fdwPitchAndFamily, string lpszFace);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate void* CreateFontIndirectADelegate(ref LOGFONTA lplf);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate void* CreateFontIndirectWDelegate(ref LOGFONTW lplf);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate uint GetCharacterPlacementADelegate(void* hdc, byte* lpString, int nCount, int nMexExtent, GCP_RESULTSA* lpResult, uint dwFlags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate uint GetCharacterPlacementWDelegate(void* hdc, byte* lpString, int nCount, int nMexExtent, GCP_RESULTSW* lpResult, uint dwFlags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate uint GetGlyphOutlineADelegate(void* hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, byte* lpvBuffer, ref MAT2 lpmat2);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate uint GetGlyphOutlineWDelegate(void* hdc, uint uChar, uint uFormat, out GLYPHMETRICS lpgm, uint cbBuffer, byte* lpvBuffer, ref MAT2 lpmat2);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate bool GetCharABCWidthsFloatADelegate(void* hdc, uint iFirst, uint iLast, ABCFLOAT* lpABC);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate bool GetCharABCWidthsFloatWDelegate(void* hdc, uint iFirst, uint iLast, ABCFLOAT* lpABC);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate bool GetTextExtentPoint32ADelegate(void* hdc, byte* lpString, int c, SIZE* psize);
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate bool GetTextExtentPoint32WDelegate(void* hdc, byte* lpString, int c, SIZE* psize);

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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void* SoftPAL_PalSpriteCreateTextDelegate(int a1, byte* Text, int* a3, int* a4);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate void* CMVS_GetTextDelegate(void* hScript, int ID);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate void ExHIBIT_Say10Delegate(void* This, void* a1, void* a2, byte* Text, void* a4, void* a5, void* a6, void* a7, void* a8, void* a9, void* a10);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate void ExHIBIT_PrintSub3Delegate(void* This, void* Text, void* a2, void* a3);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void* EntisGLS_eslHeapAllocateDelegate(void* a1, int HeapSize, void* a3);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void EntisGLS_eslHeapFreeDelegate(void* a1, void* Heap, void* a3);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate byte* lstrcpyDelegate(byte* lpString1, byte* lpString2);


    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate int MultiByteToWideCharDelegate(uint CodePage, uint dwFlags, byte* lpMultiByteStr, int cbMultiByte, char* lpWideCharStr, int cchWideChar);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate int WideCharToMultiByteDelegate(uint CodePage, uint dwFlags, byte* lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, byte* lpDefaultChar, out bool lpUsedDefaultChar);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate bool TextOutADelegate(void* dc, int xStart, int yStart, byte* pStr, int strLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate bool TextOutWDelegate(void* dc, int xStart, int yStart, byte* pStr, int strLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public unsafe delegate bool ExtTextOutADelegate(void* hdc, int x, int y, uint options, void* lprect, byte* lpString, uint c, int* lpDx);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    public unsafe delegate bool ExtTextOutWDelegate(void* hdc, int x, int y, uint options, void* lprect, byte* lpString, uint c, int* lpDx);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int CoInitializeDelegate(IntPtr Reserved);


    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate void* LoadResourceDelegate(void* hModule, void* hResourceInfo);
}
