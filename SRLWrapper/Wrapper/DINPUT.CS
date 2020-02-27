using System;
using System.Reflection;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    /// <summary>
    /// This is a wrapper to the dinput.dll
    /// </summary>
    public unsafe static class DINPUT
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary(CurrentDllName);

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            dDllGetClassObject = GetDelegate<RET_3>(RealHandler, "DllGetClassObject", false);
            dDllCanUnloadNow = GetDelegate<RET_0>(RealHandler, "DllCanUnloadNow", false);
            dDirectInputCreateA = GetDelegate<RET_4>(RealHandler, "DirectInputCreateA", false);
            dDirectInputCreateW = GetDelegate<RET_4>(RealHandler, "DirectInputCreateW", false);
            dDirectInputCreateEx = GetDelegate<RET_5>(RealHandler, "DirectInputCreateEx", false);
            dDllRegisterServer = GetDelegate<RET_0>(RealHandler, "DllRegisterServer", false);
            dDllUnregisterServer = GetDelegate<RET_0>(RealHandler, "DllUnregisterServer", false);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllGetClassObject(IntPtr rclsid, IntPtr riid, IntPtr ppv)
        {
            LoadRetail();
            return dDllGetClassObject(rclsid, riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllCanUnloadNow()
        {
            LoadRetail();
            return dDllCanUnloadNow();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectInputCreateA(IntPtr hinst, IntPtr dwVersion, IntPtr ppDI, IntPtr punkOuter)
        {
            LoadRetail();
            return dDirectInputCreateA(hinst, dwVersion, ppDI, punkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectInputCreateW(IntPtr hinst, IntPtr dwVersion, IntPtr ppDI, IntPtr punkOuter)
        {
            LoadRetail();
            return dDirectInputCreateW(hinst, dwVersion, ppDI, punkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectInputCreateEx(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr Target, IntPtr a5)
        {
            LoadRetail();
            return dDirectInputCreateEx(a1, a2, a3, Target, a5);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllRegisterServer()
        {
            LoadRetail();
            return dDllRegisterServer();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllUnregisterServer()
        {
            LoadRetail();
            return dDllUnregisterServer();
        }


        static RET_3 dDllGetClassObject;
        static RET_0 dDllCanUnloadNow;
        static RET_4 dDirectInputCreateA;
        static RET_4 dDirectInputCreateW;
        static RET_5 dDirectInputCreateEx;
        static RET_0 dDllRegisterServer;
        static RET_0 dDllUnregisterServer;

    }
}
