using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class DXGI
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("dxgi.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED     

            dCreateDXGIFactory = GetDelegate<RET_2>(RealHandler, "CreateDXGIFactory", false);
            dCreateDXGIFactory1 = GetDelegate<RET_2>(RealHandler, "CreateDXGIFactory1", false);
            dCompatValue = GetDelegate<RET_2>(RealHandler, "CompatValue", true);
            dCompatString = GetDelegate<RET_4>(RealHandler, "CompatString", true);
            dApplyCompatResolutionQuirking = GetDelegate<RET_3>(RealHandler, "ApplyCompatResolutionQuirking", true);
            dCreateDXGIFactory2 = GetDelegate<RET_3>(RealHandler, "CreateDXGIFactory2", true);
            dDXGID3D10RegisterLayers = GetDelegate<RET_2>(RealHandler, "DXGID3D10RegisterLayers", true);
            dPIXEndCapture = GetDelegate<RET_1>(RealHandler, "PIXEndCapture", true);
            dDXGID3D10GetLayeredDeviceSize = GetDelegate<RET_2>(RealHandler, "DXGID3D10GetLayeredDeviceSize", true);
            dDXGID3D10CreateLayeredDevice = GetDelegate<RET_5>(RealHandler, "DXGID3D10CreateLayeredDevice", true);
            dDXGID3D10CreateDevice = GetDelegate<RET_6>(RealHandler, "DXGID3D10CreateDevice", true);
            dDXGIReportAdapterConfiguration = GetDelegate<RET_1>(RealHandler, "DXGIReportAdapterConfiguration", true);
            dSetAppCompatStringPointer = GetDelegate<RET_2>(RealHandler, "SetAppCompatStringPointer", true);
            dUpdateHMDEmulationStatus = GetDelegate<RET_2>(RealHandler, "UpdateHMDEmulationStatus", true);
            dDXGIGetDebugInterface1 = GetDelegate<RET_3>(RealHandler, "DXGIGetDebugInterface1", true);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CreateDXGIFactory(IntPtr riid, IntPtr ppFactory)
        {
            LoadRetail();
            return dCreateDXGIFactory(riid, ppFactory);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CreateDXGIFactory1(IntPtr riid, IntPtr ppFactory)
        {
            LoadRetail();
            return dCreateDXGIFactory1(riid, ppFactory);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CompatValue(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dCompatValue(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CompatString(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            LoadRetail();
            return dCompatString(a1, a2, a3, a4);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr ApplyCompatResolutionQuirking(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            LoadRetail();
            return dApplyCompatResolutionQuirking(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CreateDXGIFactory2(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            LoadRetail();
            return dCreateDXGIFactory2(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DXGID3D10RegisterLayers(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dDXGID3D10RegisterLayers(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr PIXEndCapture(IntPtr a1)
        {
            LoadRetail();
            return dPIXEndCapture(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DXGID3D10GetLayeredDeviceSize(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dDXGID3D10GetLayeredDeviceSize(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DXGID3D10CreateLayeredDevice(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5)
        {
            LoadRetail();
            return dDXGID3D10CreateLayeredDevice(a1, a2, a3, a4, a5);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DXGID3D10CreateDevice(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6)
        {
            LoadRetail();
            return dDXGID3D10CreateDevice(a1, a2, a3, a4, a5, a6);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DXGIReportAdapterConfiguration(IntPtr a1)
        {
            LoadRetail();
            return dDXGIReportAdapterConfiguration(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr SetAppCompatStringPointer(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dSetAppCompatStringPointer(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr UpdateHMDEmulationStatus(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return dUpdateHMDEmulationStatus(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DXGIGetDebugInterface1(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            LoadRetail();
            return dDXGIGetDebugInterface1(a1, a2, a3);
        }


        static RET_2 dCreateDXGIFactory;
        static RET_2 dCreateDXGIFactory1;
        static RET_2 dCompatValue;
        static RET_4 dCompatString;
        static RET_3 dApplyCompatResolutionQuirking;
        static RET_3 dCreateDXGIFactory2;
        static RET_2 dDXGID3D10RegisterLayers;
        static RET_1 dPIXEndCapture;
        static RET_2 dDXGID3D10GetLayeredDeviceSize;
        static RET_5 dDXGID3D10CreateLayeredDevice;
        static RET_6 dDXGID3D10CreateDevice;
        static RET_1 dDXGIReportAdapterConfiguration;
        static RET_2 dSetAppCompatStringPointer;
        static RET_2 dUpdateHMDEmulationStatus;
        static RET_3 dDXGIGetDebugInterface1;

    }
}
