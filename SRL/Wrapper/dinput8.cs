using System;
using System.Runtime.InteropServices;
using static SRL.Wrapper.Tools;

namespace SRL.Wrapper {

    /// <summary>
    /// This is a wrapper to the dinput8.dll
    /// </summary>
    public static class DInput8 {
        public static IntPtr RealHandler;
        public static void LoadRetail() {
            if (RealHandler != IntPtr.Zero)
                return;

            try {
                StringReloader.ProcessReal(IntPtr.Zero);
            } catch { }

            RealHandler = LoadLibrary("dinput8.dll");

            if (RealHandler == IntPtr.Zero)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED     


            Create =      GetDelegate<RET_5>(RealHandler, "DirectInput8Create", false);
            GetJoystick = GetDelegate<RET_0>(RealHandler, "GetdfDIJoystick", false);

            CanUnload =   GetDelegate<RET_0>(RealHandler, "DllCanUnloadNow");
            GetClassObj = GetDelegate<RET_3>(RealHandler, "DllGetClassObject");
            Register =    GetDelegate<RET_0>(RealHandler, "DllRegisterServer");
            Unregister =  GetDelegate<RET_0>(RealHandler, "DllUnregisterServer");
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectInput8Create(IntPtr Instance, IntPtr Version, IntPtr RefInterId, IntPtr VoidOut, IntPtr UnkOut) {
            LoadRetail();
            return Create(Instance, Version, RefInterId, VoidOut, UnkOut);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllCanUnloadNow() {
            LoadRetail();
            return CanUnload();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllGetClassObject(IntPtr RefClassId, IntPtr RefInterId, IntPtr VoidOut) {
            LoadRetail();
            return GetClassObj(RefClassId, RefInterId, VoidOut);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllRegisterServer() {
            LoadRetail();
            return Register();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllUnregisterServer() {
            LoadRetail();
            return Unregister();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr GetdfDIJoystick() {
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
