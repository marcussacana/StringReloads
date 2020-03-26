using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper
{
    /// <summary>
    /// This is a wrapper to any kirikiri plugin
    /// </summary>
    public unsafe static class KIRIKIRI
    {
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary(CurrentDllName);

            if (RealHandler != null) {
                Link   = GetDelegate<RET_1>(RealHandler, "V2Link", false);
                Unlink = GetDelegate<RET_0>(RealHandler, "V2Unlink", false);
            }

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr V2Link(IntPtr Exporter)
        {
            LoadRetail();
            if (RealHandler == null)
                return IntPtr.Zero;
            return Link(Exporter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr V2Unlink()
        {
            LoadRetail();
            if (RealHandler == null)
                return IntPtr.Zero;
            return Unlink();
        }

        static RET_1 Link;
        static RET_0 Unlink;
    }
}
