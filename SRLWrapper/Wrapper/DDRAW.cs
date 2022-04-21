using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    /// <summary>
    /// This is a wrapper to the DDRAW.dll
    /// </summary>
    public unsafe static class ddraw
    {
        public static void* RealHandler;
        static ddraw()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("ddraw.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            dAcquireDDThreadLock = GetDelegate<RET_0>(RealHandler, "AcquireDDThreadLock", true);
            dReleaseDDThreadLock = GetDelegate<RET_0>(RealHandler, "ReleaseDDThreadLock", true);
            dDirectDrawCreate = GetDelegate<RET_3>(RealHandler, "DirectDrawCreate", false);
            dGetSurfaceFromDC = GetDelegate<RET_3>(RealHandler, "GetSurfaceFromDC", false);
            dDirectDrawCreateEx = GetDelegate<RET_4>(RealHandler, "DirectDrawCreateEx", false);
            dRegisterSpecialCase = GetDelegate<RET_4>(RealHandler, "RegisterSpecialCase", true);
            dDDInternalUnlock = GetDelegate<RET_1>(RealHandler, "DDInternalUnlock", false);
            dDDInternalLock = GetDelegate<RET_2>(RealHandler, "DDInternalLock", false);
            dGetDDSurfaceLocal = GetDelegate<RET_3>(RealHandler, "GetDDSurfaceLocal", true);
            dCompleteCreateSysmemSurface = GetDelegate<RET_1>(RealHandler, "CompleteCreateSysmemSurface", true);
            dDirectDrawEnumerateExW = GetDelegate<RET_3>(RealHandler, "DirectDrawEnumerateExW", false);
            dD3DParseUnknownCommand = GetDelegate<RET_2>(RealHandler, "D3DParseUnknownCommand", true);
            dDirectDrawEnumerateW = GetDelegate<RET_2>(RealHandler, "DirectDrawEnumerateW", false);
            dDirectDrawEnumerateExA = GetDelegate<RET_3>(RealHandler, "DirectDrawEnumerateExA", false);
            dDirectDrawEnumerateA = GetDelegate<RET_2>(RealHandler, "DirectDrawEnumerateA", false);
            dDDGetAttachedSurfaceLcl = GetDelegate<RET_3>(RealHandler, "DDGetAttachedSurfaceLcl", true);
            dDSoundHelp = GetDelegate<RET_3>(RealHandler, "DSoundHelp", false);
            dDirectDrawCreateClipper = GetDelegate<RET_3>(RealHandler, "DirectDrawCreateClipper", false);
            dDllGetClassObject = GetDelegate<RET_3>(RealHandler, "DllGetClassObject", false);
            dDllCanUnloadNow = GetDelegate<RET_0>(RealHandler, "DllCanUnloadNow", false);
            dGetOLEThunkData = GetDelegate<RET_1>(RealHandler, "GetOLEThunkData", true);
            dSetAppCompatData = GetDelegate<RET_2>(RealHandler, "SetAppCompatData", true);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void AcquireDDThreadLock()
        {
            dAcquireDDThreadLock();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void ReleaseDDThreadLock()
        {
            dReleaseDDThreadLock();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectDrawCreate(IntPtr lpGUID, IntPtr lplpDD, IntPtr pUnkOuter)
        {
            return dDirectDrawCreate(lpGUID, lplpDD, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr GetSurfaceFromDC(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            return dGetSurfaceFromDC(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectDrawCreateEx(IntPtr lpGuid, IntPtr lplpDD, IntPtr iid, IntPtr pUnkOuter)
        {
            return dDirectDrawCreateEx(lpGuid, lplpDD, iid, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr RegisterSpecialCase(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            return dRegisterSpecialCase(a1, a2, a3, a4);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DDInternalUnlock(IntPtr a1)
        {
            return dDDInternalUnlock(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DDInternalLock(IntPtr a1, IntPtr a2)
        {
            return dDDInternalLock(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr GetDDSurfaceLocal(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            return dGetDDSurfaceLocal(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CompleteCreateSysmemSurface(IntPtr a1)
        {
            return dCompleteCreateSysmemSurface(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectDrawEnumerateExW(IntPtr lpCallback, IntPtr lpContext, IntPtr dwFlags)
        {
            return dDirectDrawEnumerateExW(lpCallback, lpContext, dwFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DParseUnknownCommand(IntPtr a1, IntPtr a2)
        {
            return dD3DParseUnknownCommand(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectDrawEnumerateW(IntPtr lpCallback, IntPtr lpContext)
        {
            return dDirectDrawEnumerateW(lpCallback, lpContext);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectDrawEnumerateExA(IntPtr lpCallback, IntPtr lpContext, IntPtr dwFlags)
        {
            return dDirectDrawEnumerateExA(lpCallback, lpContext, dwFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectDrawEnumerateA(IntPtr lpCallback, IntPtr lpContext)
        {
            return dDirectDrawEnumerateA(lpCallback, lpContext);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DDGetAttachedSurfaceLcl(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            return dDDGetAttachedSurfaceLcl(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DSoundHelp(IntPtr hWnd, IntPtr a2, IntPtr a3)
        {
            return dDSoundHelp(hWnd, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DirectDrawCreateClipper(IntPtr dwFlags, IntPtr lplpDDClipper, IntPtr pUnkOuter)
        {
            return dDirectDrawCreateClipper(dwFlags, lplpDDClipper, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllGetClassObject(IntPtr rclsid, IntPtr riid, IntPtr ppv)
        {
            return dDllGetClassObject(rclsid, riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllCanUnloadNow()
        {
            return dDllCanUnloadNow();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr GetOLEThunkData(IntPtr a1)
        {
            return dGetOLEThunkData(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr SetAppCompatData(IntPtr a1, IntPtr a2)
        {
            return dSetAppCompatData(a1, a2);
        }


        static RET_0 dAcquireDDThreadLock;
        static RET_0 dReleaseDDThreadLock;
        static RET_3 dDirectDrawCreate;
        static RET_3 dGetSurfaceFromDC;
        static RET_4 dDirectDrawCreateEx;
        static RET_4 dRegisterSpecialCase;
        static RET_1 dDDInternalUnlock;
        static RET_2 dDDInternalLock;
        static RET_3 dGetDDSurfaceLocal;
        static RET_1 dCompleteCreateSysmemSurface;
        static RET_3 dDirectDrawEnumerateExW;
        static RET_2 dD3DParseUnknownCommand;
        static RET_2 dDirectDrawEnumerateW;
        static RET_3 dDirectDrawEnumerateExA;
        static RET_2 dDirectDrawEnumerateA;
        static RET_3 dDDGetAttachedSurfaceLcl;
        static RET_3 dDSoundHelp;
        static RET_3 dDirectDrawCreateClipper;
        static RET_3 dDllGetClassObject;
        static RET_0 dDllCanUnloadNow;
        static RET_1 dGetOLEThunkData;
        static RET_2 dSetAppCompatData;

    }
}
