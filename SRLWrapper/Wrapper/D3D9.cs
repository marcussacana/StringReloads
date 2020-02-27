using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class D3D9
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("d3d9.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED            


            Create = GetDelegate<RET_1>(RealHandler, "Direct3DCreate9", false);
            CreateEx = GetDelegate<RET_2>(RealHandler, "Direct3DCreate9Ex", false);


            BeginEvent = GetDelegate<RET_2>(RealHandler, "D3DPERF_BeginEvent");
            EndEvent = GetDelegate<RET_0>(RealHandler, "D3DPERF_EndEvent");
            SetMarker = GetDelegate<NULL_2>(RealHandler, "D3DPERF_SetMarker");
            SetRegion = GetDelegate<NULL_2>(RealHandler, "D3DPERF_SetRegion");
            QueryRepeatFrame = GetDelegate<RET_0>(RealHandler, "D3DPERF_QueryRepeatFrame");
            SetOptions = GetDelegate<NULL_1>(RealHandler, "D3DPERF_SetOptions");
            GetStatus = GetDelegate<RET_0>(RealHandler, "D3DPERF_GetStatus");
            DbgSetLevel = GetDelegate<RET_0>(RealHandler, "DebugSetLevel");
            DbgSetMute = GetDelegate<RET_1>(RealHandler, "DebugSetMute");
            PSampleTexture = GetDelegate<RET_0>(RealHandler, "PSGPSampleTexture");
            PError = GetDelegate<RET_0>(RealHandler, "PSGPError");
            ShaderValidator = GetDelegate<RET_0>(RealHandler, "Direct3DShaderValidatorCreate9");
            EnableMaximizedWindowedModeShim = GetDelegate<RET_1>(RealHandler, "EnableMaximizedWindowedModeShim");

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_BeginEvent(IntPtr Color, IntPtr WSName)
        {
            LoadRetail();
            return BeginEvent(Color, WSName);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_EndEvent()
        {
            LoadRetail();
            return EndEvent();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static void D3DPERF_SetMarker(IntPtr Color, IntPtr WSName)
        {
            LoadRetail();
            SetMarker(Color, WSName);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static void D3DPERF_SetRegion(IntPtr Color, IntPtr WSName)
        {
            LoadRetail();
            SetRegion(Color, WSName);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_QueryRepeatFrame()
        {
            LoadRetail();
            return QueryRepeatFrame();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static void D3DPERF_SetOptions(IntPtr Options)
        {
            LoadRetail();
            SetOptions(Options);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_GetStatus()
        {
            LoadRetail();
            return GetStatus();
        }


        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DebugSetLevel()
        {
            LoadRetail();
            return DbgSetLevel();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DebugSetMute(IntPtr Mute)
        {
            LoadRetail();
            return DbgSetMute(Mute);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr PSGPSampleTexture()
        {
            LoadRetail();
            return PSampleTexture();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr PSGPError()
        {
            LoadRetail();
            return PError();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr Direct3DShaderValidatorCreate9()
        {
            LoadRetail();
            return ShaderValidator();
        }


        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr Direct3DCreate9(IntPtr SDKVersion)
        {
            LoadRetail();
            return Create(SDKVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr Direct3DCreate9Ex(IntPtr SDKVersion, IntPtr ID3D9Ex)
        {
            LoadRetail();
            return CreateEx(SDKVersion, ID3D9Ex);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]

        public static IntPtr Direct3D9EnableMaximizedWindowedModeShim(IntPtr A)
        {
            LoadRetail();
            return EnableMaximizedWindowedModeShim(A);
        }

        static RET_2 BeginEvent;
        static RET_0 EndEvent;
        static RET_0 GetStatus;
        static RET_0 QueryRepeatFrame;

        static NULL_2 SetMarker;
        static NULL_2 SetRegion;
        static NULL_1 SetOptions;

        static RET_0 DbgSetLevel;
        static RET_1 DbgSetMute;

        static RET_0 PSampleTexture;
        static RET_0 PError;

        static RET_0 ShaderValidator;
        static RET_1 Create;
        static RET_2 CreateEx;

        static RET_1 EnableMaximizedWindowedModeShim;
    }
}