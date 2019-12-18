using System;
using System.IO;
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

            PatchBase = BaseDir.TrimEnd('\\', '/');
            if (PatchBase == AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/'))
                PatchBase += "\\Patch";

            PatchBase += '\\';

            Log("CreateFile Hook Path: {0}", true, PatchBase);
        }

        public static void InstallLoadLibraryHooks()
        {
            if (LoadLibHookInitialized)
                return;

            LoadLibHookInitialized = true;
            dLoadLibraryA = new LoadLibraryADelegate(LoadLibraryHook);
            dLoadLibraryW = new LoadLibraryWDelegate(LoadLibraryHook);
            dLoadLibraryExA = new LoadLibraryExADelegate(LoadLibraryEx);
            dLoadLibraryExW = new LoadLibraryExWDelegate(LoadLibraryEx);
            dGetModuleHandleA = new GetModuleHandleADelegate(GetModuleHandleHook);
            dGetModuleHandleW = new GetModuleHandleWDelegate(GetModuleHandleHook);
            dGetProcAddress = new GetProcAddressDelegate(GetProcAddressHook);

            hLoadLibraryA = new UnmanagedHook<LoadLibraryADelegate>("kernel32.dll", "LoadLibraryA", dLoadLibraryA, true);
            hLoadLibraryW = new UnmanagedHook<LoadLibraryWDelegate>("kernel32.dll", "LoadLibraryW", dLoadLibraryW, true);
            hLoadLibraryExA = new UnmanagedHook<LoadLibraryExADelegate>("kernel32.dll", "LoadLibraryExA", dLoadLibraryExA, true);
            hLoadLibraryExW = new UnmanagedHook<LoadLibraryExWDelegate>("kernel32.dll", "LoadLibraryExW", dLoadLibraryExW, true);
            hGetModuleHandleA = new UnmanagedHook<GetModuleHandleADelegate>("kernel32.dll", "GetModuleHandleA", dGetModuleHandleA, true);
            hGetModuleHandleW = new UnmanagedHook<GetModuleHandleWDelegate>("kernel32.dll", "GetModuleHandleW", dGetModuleHandleW, true);
            hGetProcAddress = new UnmanagedHook<GetProcAddressDelegate>("kernel32.dll", "GetProcAddress", dGetProcAddress, true);

            hLoadLibraryA.AddFollower(hLoadLibraryW);
            hLoadLibraryExA.AddFollower(hLoadLibraryExW);
            hGetModuleHandleA.AddFollower(hGetModuleHandleW);

            hLoadLibraryA.Install();
            hLoadLibraryW.Install();
            hLoadLibraryExA.Install();
            hLoadLibraryExW.Install();
            hGetModuleHandleA.Install();
            hGetModuleHandleW.Install();
            hGetProcAddress.Install();
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
            if (Verbose)
                Log("CreateFile: {0}", true, FileName);

            var Rst = OnCreateFile?.Invoke(FileName);
            if (Rst != null)
                return Rst.Value;

            FileName = ParsePath(FileName);

            return CreateFileW(FileName, Access, Share, Security, Mode, Flags, TemplateFile);
        }

        static string ParsePath(string Path)
        {

            string PatchPath = PatchBase + System.IO.Path.GetFileName(Path.Trim());

            if (ValidPath(PatchPath))
            {
                if (Verbose)
                    Log("File Path Converted from:\n{0}\nto:\n{1}", true, Path, PatchPath);

                return PatchPath;
            }

            return Path.Trim();
        }
        static IntPtr LoadLibraryHook(string lpFileName)
        {
            if (Verbose)
                Log("LoadLibrary: {0}", true, lpFileName);

            var Rst = OnLoadLibrary?.Invoke(lpFileName);
            if (Rst != null)
                return Rst.Value;

            var FileName = lpFileName?.ToLower();

            if (FileName == Wrapper.Tools.CurrentDllPath.ToLower())
                return Wrapper.Tools.RealHandler;
            if (FileName == Wrapper.Tools.CurrentDllName)
                return Wrapper.Tools.RealHandler;
            if (FileName == Path.GetFileNameWithoutExtension(Wrapper.Tools.CurrentDllName))
                return Wrapper.Tools.RealHandler;

            return LoadLibraryW(lpFileName);
        }

        static IntPtr LoadLibraryEx(string lpFileName, IntPtr ReservedNull, LoadLibraryFlags Flags)
        {
            bool AsResource = false;
            AsResource |= Flags.HasFlag(LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
            AsResource |= Flags.HasFlag(LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE);
            AsResource |= Flags.HasFlag(LoadLibraryFlags.LOAD_LIBRARY_AS_IMAGE_RESOURCE);

            if (Verbose)
                Log("LoadLibraryEx: {0}", true, lpFileName);

            var Rst = OnLoadLibrary?.Invoke(lpFileName);
            if (Rst != null)
                return Rst.Value;

            var FileName = lpFileName?.ToLower();

            if (!AsResource)
            {
                if (FileName == Wrapper.Tools.CurrentDllPath.ToLower())
                    return Wrapper.Tools.RealHandler;
                if (FileName == Wrapper.Tools.CurrentDllName)
                    return Wrapper.Tools.RealHandler;
            }

            if (FileName== Wrapper.Tools.CurrentDllPath.ToLower())
                lpFileName = Wrapper.Tools.RealDllPath;
            if (FileName == Wrapper.Tools.CurrentDllName)
                lpFileName = Wrapper.Tools.RealDllPath;
            if (FileName == Path.GetFileNameWithoutExtension(Wrapper.Tools.CurrentDllName))
                return Wrapper.Tools.RealHandler;

            return LoadLibraryExW(lpFileName, ReservedNull, Flags);
        }

        static IntPtr GetModuleHandleHook(string lpModuleName)
        {
            if (Verbose)
                Log("GetModuleHandle: {0}", true, lpModuleName);

            var Rst = OnGetModuleHandler?.Invoke(lpModuleName);
            if (Rst != null)
                return Rst.Value;

            var ModuleName = lpModuleName?.ToLower();

            if (ModuleName == Wrapper.Tools.CurrentDllPath.ToLower())
                return Wrapper.Tools.RealHandler;
            if (ModuleName == Wrapper.Tools.CurrentDllName)
                return Wrapper.Tools.RealHandler;
            if (ModuleName == Path.GetFileNameWithoutExtension(Wrapper.Tools.CurrentDllName))
                return Wrapper.Tools.RealHandler;


            return GetModuleHandleW(lpModuleName);
        }

        public static IntPtr GetProcAddressHook(IntPtr Module, IntPtr Function)
        {
            if (Module == Wrapper.Tools.WrapperHandler)
                Module = Wrapper.Tools.RealHandler;

            if (Function.ToUInt64() <= ushort.MaxValue)
            {
                ushort Ordinal = (ushort)Function.ToUInt64();

                if (Verbose)
                    Log("GetProcAddress: Ordinal#{0}", true, Ordinal);

                var Rst = OnGetProcAddressOrdinal?.Invoke(Module, Ordinal);
                if (Rst != null)
                    return Rst.Value;

                return GetProcAddress(Module, Ordinal);
            }
            else
            {
                string FuncName = GetStringA(Function, Internal: true);

                if (Verbose)
                    Log("GetProcAddress: {0}", true, FuncName);

                var Rst = OnGetProcAddressName?.Invoke(Module, FuncName);
                if (Rst != null)
                    return Rst.Value;

                return GetProcAddress(Module, FuncName);
            }
        }

        static bool LoadLibHookInitialized = false;
        static bool ValidPath(string Path) => GetFileAttributesW(Path) != INVALID_FILE_ATTRIBUTES;

        static string PatchBase;

        public static event FileAccessDelegate OnLoadLibrary;
        public static event FileAccessDelegate OnCreateFile;
        public static event FileAccessDelegate OnGetModuleHandler;
        public static event GetProcAddrOrdinalDelegate OnGetProcAddressOrdinal;
        public static event GetProcAddrNameDelegate OnGetProcAddressName;

        public delegate IntPtr? FileAccessDelegate(string Filename);
        public delegate IntPtr? GetProcAddrOrdinalDelegate(IntPtr Module, ushort Ordinal);
        public delegate IntPtr? GetProcAddrNameDelegate(IntPtr Module, string Name);


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
        static GetModuleHandleADelegate dGetModuleHandleA;
        static GetModuleHandleWDelegate dGetModuleHandleW;
        //static GetModuleHandleExADelegate dGetModuleHandleExA;
        //static GetModuleHandleExWDelegate dGetModuleHandleExW;
        static GetProcAddressDelegate dGetProcAddress;

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
        static UnmanagedHook<GetModuleHandleADelegate> hGetModuleHandleA;
        static UnmanagedHook<GetModuleHandleWDelegate> hGetModuleHandleW;
        //static UnmanagedHook<GetModuleHandleExADelegate> hGetModuleHandleExA;
        //static UnmanagedHook<GetModuleHandleExWDelegate> hGetModuleHandleExW;
        static UnmanagedHook<GetProcAddressDelegate> hGetProcAddress;

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


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr GetModuleHandleADelegate(string lpModuleName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr GetModuleHandleWDelegate(string lpModuleName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr GetModuleHandleExADelegate(uint Flags, string lpModuleName, IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr GetModuleHandleExWDelegate(uint Flags, string lpModuleName, IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        delegate IntPtr GetProcAddressDelegate(IntPtr hModule, IntPtr Proc);


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

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool GetModuleHandleExW(uint Flags, string lpModuleName, IntPtr hModule);

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
