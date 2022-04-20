using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class D3D9
    {
        public static void* RealHandler;
        static D3D9()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("d3d9.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED            


            Create = GetDelegate<RET_1>(RealHandler, "Direct3DCreate9", false);
            CreateEx = GetDelegate<RET_2>(RealHandler, "Direct3DCreate9Ex", false);


            BeginEvent = GetDelegate<RET_2>(RealHandler, "D3DPERF_BeginEvent", true);
            EndEvent = GetDelegate<RET_0>(RealHandler, "D3DPERF_EndEvent", true);
            SetMarker = GetDelegate<NULL_2>(RealHandler, "D3DPERF_SetMarker", true);
            SetRegion = GetDelegate<NULL_2>(RealHandler, "D3DPERF_SetRegion", true);
            QueryRepeatFrame = GetDelegate<RET_0>(RealHandler, "D3DPERF_QueryRepeatFrame", true);
            SetOptions = GetDelegate<NULL_1>(RealHandler, "D3DPERF_SetOptions", true);
            GetStatus = GetDelegate<RET_0>(RealHandler, "D3DPERF_GetStatus", true);
            DbgSetLevel = GetDelegate<RET_0>(RealHandler, "DebugSetLevel", true);
            DbgSetMute = GetDelegate<RET_0>(RealHandler, "DebugSetMute", true);
            PSampleTexture = GetDelegate<RET_0>(RealHandler, "PSGPSampleTexture", true);
            PError = GetDelegate<RET_0>(RealHandler, "PSGPError", true);
            ShaderValidator = GetDelegate<RET_0>(RealHandler, "Direct3DShaderValidatorCreate9", true);
            EnableMaximizedWindowedModeShim = GetDelegate<RET_1>(RealHandler, "EnableMaximizedWindowedModeShim", true);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_BeginEvent(IntPtr Color, IntPtr WSName)
        {
            return BeginEvent(Color, WSName);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_EndEvent()
        {
            return EndEvent();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static void D3DPERF_SetMarker(IntPtr Color, IntPtr WSName)
        {
            SetMarker(Color, WSName);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static void D3DPERF_SetRegion(IntPtr Color, IntPtr WSName)
        {
            SetRegion(Color, WSName);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_QueryRepeatFrame()
        {
            return QueryRepeatFrame();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static void D3DPERF_SetOptions(IntPtr Options)
        {
            SetOptions(Options);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr D3DPERF_GetStatus()
        {
            return GetStatus();
        }


        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DebugSetLevel()
        {
            return DbgSetLevel();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr DebugSetMute()
        {
            return DbgSetMute();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr PSGPSampleTexture()
        {
            return PSampleTexture();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr PSGPError()
        {
            return PError();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr Direct3DShaderValidatorCreate9()
        {
            return ShaderValidator();
        }


        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr Direct3DCreate9(IntPtr SDKVersion)
        {
            return Create(SDKVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr Direct3DCreate9Ex(IntPtr SDKVersion, IntPtr ID3D9Ex)
        {
            return CreateEx(SDKVersion, ID3D9Ex);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]

        public static IntPtr Direct3D9EnableMaximizedWindowedModeShim(IntPtr A)
        {
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
        static RET_0 DbgSetMute;

        static RET_0 PSampleTexture;
        static RET_0 PError;

        static RET_0 ShaderValidator;
        static RET_1 Create;
        static RET_2 CreateEx;

        static RET_1 EnableMaximizedWindowedModeShim;
    }
}