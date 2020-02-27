using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class DSOUND
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("dsound.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED     


            Create = GetDelegate<RET_3>(RealHandler, "DirectSoundCreate", false);

            SoundEnumerateA = GetDelegate<RET_2>(RealHandler, "DirectSoundEnumerateA", false);
            SoundEnumerateW = GetDelegate<RET_2>(RealHandler, "DirectSoundEnumerateW", false);
            SoundCaptureEnumerateA = GetDelegate<RET_2>(RealHandler, "DirectSoundCaptureEnumerateA", false);
            SoundCaptureEnumerateW = GetDelegate<RET_2>(RealHandler, "DirectSoundCaptureEnumerateW", false);

            SoundCaptureCreate = GetDelegate<RET_3>(RealHandler, "DirectSoundCaptureCreate", false);

            SoundCreate8 = GetDelegate<RET_3>(RealHandler, "DirectSoundCreate8");
            SoundCaptureCreate8 = GetDelegate<RET_3>(RealHandler, "DirectSoundCaptureCreate8");
            SoundFullDuplexCreate = GetDelegate<RET_10>(RealHandler, "DirectSoundFullDuplexCreate");
            DeviceID = GetDelegate<RET_2>(RealHandler, "GetDeviceID");


            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundCreate(IntPtr pcGuidDevice, IntPtr ppDS, IntPtr pUnkOuter)
        {
            LoadRetail();
            return Create(pcGuidDevice, ppDS, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundCaptureCreate(IntPtr pcGuidDevice, IntPtr ppDS, IntPtr pUnkOuter)
        {
            LoadRetail();
            return SoundCaptureCreate(pcGuidDevice, ppDS, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundCaptureCreate8(IntPtr pcGuidDevice, IntPtr ppDSC8, IntPtr pUnkOuter)
        {
            LoadRetail();
            return SoundCaptureCreate8(pcGuidDevice, ppDSC8, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DllGetClassObject(IntPtr pcGuidDevice, IntPtr ppDS8, IntPtr pUnkOuter)
        {
            LoadRetail();
            return SoundCreate8(pcGuidDevice, ppDS8, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundFullDuplexCreate(IntPtr pcGuidCaptureDevice, IntPtr pcGuidRenderDevice, IntPtr pcDSCBufferDesc, IntPtr pcDSBufferDesc, IntPtr hWnd, IntPtr dwLevel, IntPtr ppDSFD, IntPtr ppDSCBuffer8, IntPtr ppDSBuffer8, IntPtr pUnkOuter)
        {
            LoadRetail();
            return SoundFullDuplexCreate(pcGuidCaptureDevice, pcGuidRenderDevice, pcDSCBufferDesc, pcDSBufferDesc, hWnd, dwLevel, ppDSFD, ppDSCBuffer8, ppDSBuffer8, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundFullDuplexCreate8(IntPtr pcGuidCaptureDevice, IntPtr pcGuidRenderDevice, IntPtr pcDSCBufferDesc, IntPtr pcDSBufferDesc, IntPtr hWnd, IntPtr dwLevel, IntPtr ppDSFD, IntPtr ppDSCBuffer8, IntPtr ppDSBuffer8, IntPtr pUnkOuter)
        {
            LoadRetail();
            return SoundFullDuplexCreate(pcGuidCaptureDevice, pcGuidRenderDevice, pcDSCBufferDesc, pcDSBufferDesc, hWnd, dwLevel, ppDSFD, ppDSCBuffer8, ppDSBuffer8, pUnkOuter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr GetDeviceID(IntPtr pGuidSrc, IntPtr pGuidDest)
        {
            LoadRetail();
            return DeviceID(pGuidSrc, pGuidDest);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundEnumerateA(IntPtr pDSEnumCallback, IntPtr pContext)
        {
            LoadRetail();
            return SoundEnumerateA(pDSEnumCallback, pContext);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundEnumerateW(IntPtr pDSEnumCallback, IntPtr pContext)
        {
            LoadRetail();
            return SoundEnumerateW(pDSEnumCallback, pContext);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundCaptureEnumerateA(IntPtr pDSEnumCallback, IntPtr pContext)
        {
            LoadRetail();
            return SoundCaptureEnumerateA(pDSEnumCallback, pContext);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DirectSoundCaptureEnumerateW(IntPtr pDSEnumCallback, IntPtr pContext)
        {
            LoadRetail();
            return SoundCaptureEnumerateW(pDSEnumCallback, pContext);
        }

        static RET_3 Create;

        static RET_3 SoundCaptureCreate;
        static RET_3 SoundCaptureCreate8;
        static RET_3 SoundCreate8;

        static RET_10 SoundFullDuplexCreate;
        static RET_2 DeviceID;

        static RET_2 SoundEnumerateA;
        static RET_2 SoundEnumerateW;
        static RET_2 SoundCaptureEnumerateA;
        static RET_2 SoundCaptureEnumerateW;
    }
}