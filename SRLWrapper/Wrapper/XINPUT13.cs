using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    /// <summary>
    /// This is a wrapper to the XINPUT1_3.dll
    /// </summary>
    public unsafe static class XINPUT13
    {
        public static void* RealHandler;
        static XINPUT13()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("xinput1_3.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            dDllMain = GetDelegate<RET_3>(RealHandler, "DllMain", true);
            dXInputGetState = GetDelegate<RET_2>(RealHandler, "XInputGetState", true);
            dXInputSetState = GetDelegate<RET_2>(RealHandler, "XInputSetState", true);
            dXInputGetCapabilities = GetDelegate<RET_3>(RealHandler, "XInputGetCapabilities", true);
            dXInputEnable = GetDelegate<RET_1>(RealHandler, "XInputEnable", true);
            dXInputGetBatteryInformation = GetDelegate<RET_3>(RealHandler, "XInputGetBatteryInformation", true);
            dXInputGetDSoundAudioDeviceGuids = GetDelegate<RET_3>(RealHandler, "XInputGetDSoundAudioDeviceGuids", true);
            dXInputGetKeystroke = GetDelegate<RET_3>(RealHandler, "XInputGetKeystroke", true);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllMain(IntPtr hinstDLL, IntPtr fdwReason, IntPtr lpvReserved)
        {
            return dDllMain(hinstDLL, fdwReason, lpvReserved);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr XInputGetState(IntPtr dwUserIndex, IntPtr pState)
        {
            return dXInputGetState(dwUserIndex, pState);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr XInputSetState(IntPtr dwUserIndex, IntPtr pVibration)
        {
            return dXInputSetState(dwUserIndex, pVibration);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr XInputGetCapabilities(IntPtr dwUserIndex, IntPtr dwFlags, IntPtr pCapabilities)
        {
            return dXInputGetCapabilities(dwUserIndex, dwFlags, pCapabilities);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr XInputEnable(IntPtr a1)
        {
            return dXInputEnable(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr XInputGetBatteryInformation(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            return dXInputGetBatteryInformation(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr XInputGetDSoundAudioDeviceGuids(IntPtr dwUserIndex, IntPtr pDSoundRenderGuid, IntPtr pDSoundCaptureGuid)
        {
            return dXInputGetDSoundAudioDeviceGuids(dwUserIndex, pDSoundRenderGuid, pDSoundCaptureGuid);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr XInputGetKeystroke(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            return dXInputGetKeystroke(a1, a2, a3);
        }


        static RET_3 dDllMain;
        static RET_2 dXInputGetState;
        static RET_2 dXInputSetState;
        static RET_3 dXInputGetCapabilities;
        static RET_1 dXInputEnable;
        static RET_3 dXInputGetBatteryInformation;
        static RET_3 dXInputGetDSoundAudioDeviceGuids;
        static RET_3 dXInputGetKeystroke;

    }
}
