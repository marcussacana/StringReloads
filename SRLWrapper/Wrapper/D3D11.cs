using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class D3D11
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("d3d11.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED     

            dD3D11CoreRegisterLayers = GetDelegate<RET_3>(RealHandler, "D3D11CoreRegisterLayers", false);
            dD3D11CoreGetLayeredDeviceSize = GetDelegate<RET_3>(RealHandler, "D3D11CoreGetLayeredDeviceSize", false);
            dD3D11CoreCreateLayeredDevice = GetDelegate<RET_5>(RealHandler, "D3D11CoreCreateLayeredDevice", false);
            dD3D11CoreCreateDevice = GetDelegate<RET_10>(RealHandler, "D3D11CoreCreateDevice", false);
            dD3D11CreateDeviceAndSwapChain = GetDelegate<RET_13>(RealHandler, "D3D11CreateDeviceAndSwapChain", false);
            dD3D11CreateDevice = GetDelegate<RET_11>(RealHandler, "D3D11CreateDevice", false);
            dD3DKMTWaitForVerticalBlankEvent = GetDelegate<RET_1>(RealHandler, "D3DKMTWaitForVerticalBlankEvent", false);
            dD3D11CreateDeviceForD3D12 = GetDelegate<RET_10>(RealHandler, "D3D11CreateDeviceForD3D12", false);
            dD3D11On12CreateDevice = GetDelegate<RET_11>(RealHandler, "D3D11On12CreateDevice", false);
            dD3DPerformance_BeginEvent = GetDelegate<RET_2>(RealHandler, "D3DPerformance_BeginEvent", false);
            dD3DPerformance_EndEvent = GetDelegate<RET_1>(RealHandler, "D3DPerformance_EndEvent", false);
            dD3DPerformance_GetStatus = GetDelegate<RET_1>(RealHandler, "D3DPerformance_GetStatus", false);
            dD3DPerformance_SetMarker = GetDelegate<RET_2>(RealHandler, "D3DPerformance_SetMarker", false);
            dD3DKMTQueryAdapterInfo = GetDelegate<RET_1>(RealHandler, "D3DKMTQueryAdapterInfo", false);
            dOpenAdapter10 = GetDelegate<RET_1>(RealHandler, "OpenAdapter10", false);
            dOpenAdapter10_2 = GetDelegate<RET_1>(RealHandler, "OpenAdapter10_2", false);
            dD3DKMTEscape = GetDelegate<RET_1>(RealHandler, "D3DKMTEscape", false);
            dD3DKMTGetDeviceState = GetDelegate<RET_1>(RealHandler, "D3DKMTGetDeviceState", false);
            dD3DKMTOpenAdapterFromHdc = GetDelegate<RET_1>(RealHandler, "D3DKMTOpenAdapterFromHdc", false);
            dD3DKMTQueryResourceInfo = GetDelegate<RET_1>(RealHandler, "D3DKMTQueryResourceInfo", false);
            dCreateDirect3D11DeviceFromDXGIDevice = GetDelegate<RET_2>(RealHandler, "CreateDirect3D11DeviceFromDXGIDevice", false);
            dCreateDirect3D11SurfaceFromDXGISurface = GetDelegate<RET_2>(RealHandler, "CreateDirect3D11SurfaceFromDXGISurface", false);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr D3D11CoreRegisterLayers(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            LoadRetail();
            return dD3D11CoreRegisterLayers(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr D3D11CoreGetLayeredDeviceSize(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            LoadRetail();
            return dD3D11CoreGetLayeredDeviceSize(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D11CoreCreateLayeredDevice(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5)
        {
            LoadRetail();
            return dD3D11CoreCreateLayeredDevice(a1, a2, a3, a4, a5);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D11CoreCreateDevice(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10)
        {
            LoadRetail();
            return dD3D11CoreCreateDevice(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr D3D11CreateDeviceAndSwapChain(IntPtr a0, IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr ArgList, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12)
        {
            LoadRetail();
            return dD3D11CreateDeviceAndSwapChain(a0, a1, a2, a3, a4, a5, a6, ArgList, a8, a9, a10, a11, a12);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr D3D11CreateDevice(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11)
        {
            LoadRetail();
            return dD3D11CreateDevice(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DKMTWaitForVerticalBlankEvent(IntPtr a1)
        {
            LoadRetail();
            return dD3DKMTWaitForVerticalBlankEvent(a1);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr D3D11CreateDeviceForD3D12(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10)
        {
            LoadRetail();
            return dD3D11CreateDeviceForD3D12(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr D3D11On12CreateDevice(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11)
        {
            LoadRetail();
            return dD3D11On12CreateDevice(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DPerformance_BeginEvent(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dD3DPerformance_BeginEvent(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DPerformance_EndEvent(IntPtr a1)
        {
            LoadRetail();
            return dD3DPerformance_EndEvent(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DPerformance_GetStatus(IntPtr a1)
        {
            LoadRetail();
            return dD3DPerformance_GetStatus(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void D3DPerformance_SetMarker(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            dD3DPerformance_SetMarker(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DKMTQueryAdapterInfo(IntPtr a1)
        {
            LoadRetail();
            return dD3DKMTQueryAdapterInfo(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr OpenAdapter10(IntPtr a1)
        {
            LoadRetail();
            return dOpenAdapter10(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr OpenAdapter10_2(IntPtr a1)
        {
            LoadRetail();
            return dOpenAdapter10_2(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DKMTEscape(IntPtr a1)
        {
            LoadRetail();
            return dD3DKMTEscape(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DKMTGetDeviceState(IntPtr a1)
        {
            LoadRetail();
            return dD3DKMTGetDeviceState(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DKMTOpenAdapterFromHdc(IntPtr a1)
        {
            LoadRetail();
            return dD3DKMTOpenAdapterFromHdc(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3DKMTQueryResourceInfo(IntPtr a1)
        {
            LoadRetail();
            return dD3DKMTQueryResourceInfo(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CreateDirect3D11DeviceFromDXGIDevice(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dCreateDirect3D11DeviceFromDXGIDevice(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CreateDirect3D11SurfaceFromDXGISurface(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dCreateDirect3D11SurfaceFromDXGISurface(a1, a2);
        }


        static RET_3 dD3D11CoreRegisterLayers;
        static RET_3 dD3D11CoreGetLayeredDeviceSize;
        static RET_5 dD3D11CoreCreateLayeredDevice;
        static RET_10 dD3D11CoreCreateDevice;
        static RET_13 dD3D11CreateDeviceAndSwapChain;
        static RET_11 dD3D11CreateDevice;
        static RET_1 dD3DKMTWaitForVerticalBlankEvent;
        static RET_10 dD3D11CreateDeviceForD3D12;
        static RET_11 dD3D11On12CreateDevice;
        static RET_2 dD3DPerformance_BeginEvent;
        static RET_1 dD3DPerformance_EndEvent;
        static RET_1 dD3DPerformance_GetStatus;
        static RET_2 dD3DPerformance_SetMarker;
        static RET_1 dD3DKMTQueryAdapterInfo;
        static RET_1 dOpenAdapter10;
        static RET_1 dOpenAdapter10_2;
        static RET_1 dD3DKMTEscape;
        static RET_1 dD3DKMTGetDeviceState;
        static RET_1 dD3DKMTOpenAdapterFromHdc;
        static RET_1 dD3DKMTQueryResourceInfo;
        static RET_2 dCreateDirect3D11DeviceFromDXGIDevice;
        static RET_2 dCreateDirect3D11SurfaceFromDXGISurface;

    }
}
