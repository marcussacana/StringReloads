using System;
using System.Runtime.InteropServices;

namespace SRL
{
    static partial class StringReloader
    {
        static void InstallCreateFileHooks()
        {
            if (Managed)
                return;

            dGetFileAttrA = new GetFileAttributesADelegate(GetFileAttributes);
            dGetFileAttrW = new GetFileAttributesWDelegate(GetFileAttributes);
            dGetFileAttrExA = new GetFileAttributesExADelegate(GetFileAttributesEx);
            dGetFileAttrExW = new GetFileAttributesExWDelegate(GetFileAttributesEx);
            dCreateFileA = new CreateFileADelegate(CreateFile);
            dCreateFileW = new CreateFileWDelegate(CreateFile);

            hCreateFileA = AutoHookCreator("kernel32.dll", "CreateFileA", dCreateFileA);
            hCreateFileW = AutoHookCreator("kernel32.dll", "CreateFileW", dCreateFileW);
            hGetFileAttrA = AutoHookCreator("kernel32.dll", "GetFileAttributesA", dGetFileAttrA);
            hGetFileAttrW = AutoHookCreator("kernel32.dll", "GetFileAttributesW", dGetFileAttrW);
            hGetFileAttrExA = AutoHookCreator("kernel32.dll", "GetFileAttributesExA", dGetFileAttrExA);
            hGetFileAttrExW = AutoHookCreator("kernel32.dll", "GetFileAttributesExW", dGetFileAttrExW);


            hCreateFileA.Install();
            hCreateFileW.Install();
            hGetFileAttrA.Install();
            hGetFileAttrW.Install();
            hGetFileAttrExA.Install();
            hGetFileAttrExW.Install();

            Base = BaseDir.TrimEnd('\\', '/');
            if (Base == AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/'))
                Base += "\\Patch";

            Base += '\\';

            Log("CreateFile Hook Path: {0}", true, Base);
        }

        static bool GetFileAttributesEx(string FileName, IntPtr fInfoLevelId, IntPtr lpFileInformation)
        {
            FileName = ParsePath(FileName);

            return GetFileAttributesExW(FileName, fInfoLevelId, lpFileInformation);
        }

        static uint GetFileAttributes(string FileName)
        {
            FileName = ParsePath(FileName);

            return GetFileAttributesW(FileName);
        }
        static IntPtr CreateFile(string FileName, IntPtr Access, IntPtr Share, IntPtr Security, IntPtr Mode, IntPtr Flags, IntPtr TemplateFile)
        {
            FileName = ParsePath(FileName);

            return CreateFileW(FileName, Access, Share, Security, Mode, Flags, TemplateFile);
        }

        static string ParsePath(string Path)
        {

            string PatchPath = Base + System.IO.Path.GetFileName(Path);

            if (ValidPath(PatchPath))
            {
#if DEBUG
                Log("File Path Converted from:\n{0}\nto:\n{1}", true, Path, PatchPath);
#endif
                return PatchPath;
            }

            return Path;
        }

        static string Base;

        static bool ValidPath(string Path) => GetFileAttributesW(Path) != INVALID_FILE_ATTRIBUTES;

        const uint INVALID_FILE_ATTRIBUTES = 0xFFFFFFFF;

        static GetFileAttributesADelegate dGetFileAttrA;
        static GetFileAttributesWDelegate dGetFileAttrW;
        static GetFileAttributesExADelegate dGetFileAttrExA;
        static GetFileAttributesExWDelegate dGetFileAttrExW;
        static CreateFileADelegate dCreateFileA;
        static CreateFileWDelegate dCreateFileW;

        static UnmanagedHook hGetFileAttrA;
        static UnmanagedHook hGetFileAttrW;
        static UnmanagedHook hGetFileAttrExA;
        static UnmanagedHook hGetFileAttrExW;
        static UnmanagedHook hCreateFileA;
        static UnmanagedHook hCreateFileW;

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate uint GetFileAttributesADelegate([MarshalAs(UnmanagedType.LPStr)] string Filepath);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate uint GetFileAttributesWDelegate([MarshalAs(UnmanagedType.LPWStr)] string Filepath);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate bool GetFileAttributesExADelegate([MarshalAs(UnmanagedType.LPStr)] string Filepath, IntPtr fInfoLevelId, IntPtr lpFileInformation);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool GetFileAttributesExWDelegate([MarshalAs(UnmanagedType.LPWStr)] string Filepath, IntPtr fInfoLevelId, IntPtr lpFileInformation);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr CreateFileADelegate([MarshalAs(UnmanagedType.LPStr)] string filename, IntPtr Access, IntPtr Share, IntPtr Security, IntPtr Mode, IntPtr Flags, IntPtr templateFile);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr CreateFileWDelegate([MarshalAs(UnmanagedType.LPWStr)] string filename, IntPtr Access, IntPtr Share, IntPtr Security, IntPtr Mode, IntPtr Flags, IntPtr templateFile);


        [DllImport("kernelbase.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool GetFileAttributesExW(string lpFileName, IntPtr fInfoLevelId, IntPtr lpFileInformation);

        [DllImport("kernelbase.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern uint GetFileAttributesW(string lpFileName);

        [DllImport("kernelbase.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr CreateFileW(string FileName, IntPtr Access, IntPtr Share, IntPtr Security, IntPtr Mode, IntPtr Flags, IntPtr templateFile);

    }
}
