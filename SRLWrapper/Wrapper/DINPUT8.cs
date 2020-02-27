using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class DINPUT8
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("dinput8.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED     


            Create = GetDelegate<RET_5>(RealHandler, "DirectInput8Create", false);
            GetJoystick = GetDelegate<RET_0>(RealHandler, "GetdfDIJoystick", false);

            CanUnload = GetDelegate<RET_0>(RealHandler, "DllCanUnloadNow");
            GetClassObj = GetDelegate<RET_3>(RealHandler, "DllGetClassObject");
            Register = GetDelegate<RET_0>(RealHandler, "DllRegisterServer");
            Unregister = GetDelegate<RET_0>(RealHandler, "DllUnregisterServer");

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectInput8Create(IntPtr Instance, IntPtr Version, IntPtr RefInterId, IntPtr VoidOut, IntPtr UnkOut)
        {
            LoadRetail();
            return Create(Instance, Version, RefInterId, VoidOut, UnkOut);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllCanUnloadNow()
        {
            LoadRetail();
            return CanUnload();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllGetClassObject(IntPtr RefClassId, IntPtr RefInterId, IntPtr VoidOut)
        {
            LoadRetail();
            return GetClassObj(RefClassId, RefInterId, VoidOut);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllRegisterServer()
        {
            LoadRetail();
            return Register();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllUnregisterServer()
        {
            LoadRetail();
            return Unregister();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr GetdfDIJoystick()
        {
            LoadRetail();
            return GetJoystick();
        }

        static RET_5 Create;

        static RET_0 CanUnload;
        static RET_3 GetClassObj;
        static RET_0 Register;
        static RET_0 Unregister;

        static RET_0 GetJoystick;
    }
}