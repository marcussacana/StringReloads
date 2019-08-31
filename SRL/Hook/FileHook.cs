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

        static void InstallLoadLibraryHooks()
        {

            dLoadLibraryA = new LoadLibraryADelegate(LoadLibraryHook);
            dLoadLibraryW = new LoadLibraryWDelegate(LoadLibraryHook);
            dLoadLibraryExA = new LoadLibraryExADelegate(LoadLibraryEx);
            dLoadLibraryExW = new LoadLibraryExWDelegate(LoadLibraryEx);

            hLoadLibraryA = new UnmanagedHook<LoadLibraryADelegate>("kernel32.dll", "LoadLibraryA", dLoadLibraryA, true);
            hLoadLibraryW = new UnmanagedHook<LoadLibraryWDelegate>("kernel32.dll", "LoadLibraryW", dLoadLibraryW, true);
            hLoadLibraryExA = new UnmanagedHook<LoadLibraryExADelegate>("kernel32.dll", "LoadLibraryExA", dLoadLibraryExA, true);
            hLoadLibraryExW = new UnmanagedHook<LoadLibraryExWDelegate>("kernel32.dll", "LoadLibraryExW", dLoadLibraryExW, true);

            hLoadLibraryA.AddFollower(hLoadLibraryW);
            hLoadLibraryExA.AddFollower(hLoadLibraryExW);

            hLoadLibraryA.Install();
            hLoadLibraryW.Install();
            hLoadLibraryExA.Install();
            hLoadLibraryExW.Install();
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
        static IntPtr LoadLibraryHook(string lpFileName)
        {
            if (LogAll)
                Log("LoadLibrary: {0}", true, lpFileName);

            if (lpFileName.ToLower() == Wrapper.Tools.CurrentDllPath.ToLower())
                return Wrapper.Tools.RealHandler;
            if (lpFileName.ToLower() == Wrapper.Tools.CurrentDllName.ToLower())
                return Wrapper.Tools.RealHandler;

            return LoadLibraryW(lpFileName);
        }
        static IntPtr LoadLibraryEx(string lpFileName, IntPtr ReservedNull, LoadLibraryFlags Flags)
        {
            bool AsResource = false;
            AsResource |= Flags.HasFlag(LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
            AsResource |= Flags.HasFlag(LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE);
            AsResource |= Flags.HasFlag(LoadLibraryFlags.LOAD_LIBRARY_AS_IMAGE_RESOURCE);

            if (LogAll)
                Log("LoadLibraryEx: {0}", true, lpFileName);

            if (!AsResource)
            {
                if (lpFileName.ToLower() == Wrapper.Tools.CurrentDllPath)
                    return Wrapper.Tools.RealHandler;
                if (lpFileName.ToLower() == Wrapper.Tools.CurrentDllName)
                    return Wrapper.Tools.RealHandler;
            }

            if (lpFileName.ToLower() == Wrapper.Tools.CurrentDllPath)
                lpFileName = Wrapper.Tools.RealDllPath;
            if (lpFileName.ToLower() == Wrapper.Tools.CurrentDllName)
                lpFileName = Wrapper.Tools.RealDllPath;

            return LoadLibraryExW(lpFileName, ReservedNull, Flags);
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

        static LoadLibraryADelegate dLoadLibraryA;
        static LoadLibraryWDelegate dLoadLibraryW;
        static LoadLibraryExADelegate dLoadLibraryExA;
        static LoadLibraryExWDelegate dLoadLibraryExW;

        static UnmanagedHook hGetFileAttrA;
        static UnmanagedHook hGetFileAttrW;
        static UnmanagedHook hGetFileAttrExA;
        static UnmanagedHook hGetFileAttrExW;
        static UnmanagedHook hCreateFileA;
        static UnmanagedHook hCreateFileW;

        static UnmanagedHook<LoadLibraryADelegate> hLoadLibraryA;
        static UnmanagedHook<LoadLibraryWDelegate> hLoadLibraryW;
        static UnmanagedHook<LoadLibraryExADelegate> hLoadLibraryExA;
        static UnmanagedHook<LoadLibraryExWDelegate> hLoadLibraryExW;

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


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr LoadLibraryADelegate(string lpFileName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr LoadLibraryWDelegate(string lpFileName);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr LoadLibraryExADelegate(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr LoadLibraryExWDelegate(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        [DllImport("kernelbase.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool GetFileAttributesExW(string lpFileName, IntPtr fInfoLevelId, IntPtr lpFileInformation);

        [DllImport("kernelbase.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern uint GetFileAttributesW(string lpFileName);

        [DllImport("kernelbase.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr CreateFileW(string FileName, IntPtr Access, IntPtr Share, IntPtr Security, IntPtr Mode, IntPtr Flags, IntPtr templateFile);



        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibraryW(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibraryExW(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        [Flags]
        enum LoadLibraryFlags : uint
        {
            None = 0,
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }
    }
}
