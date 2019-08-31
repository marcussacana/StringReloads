using System;
using System.Reflection;
using System.Runtime.InteropServices;
using static SRL.Wrapper.Tools;

namespace SRL.Wrapper
{

    /// <summary>
    /// This is a wrapper to any kirikiri plugin
    /// </summary>
    public static class KirKiri
    {
        public static IntPtr RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != IntPtr.Zero)
                return;

            RealHandler = LoadLibrary(CurrentDllName);

            if (RealHandler == IntPtr.Zero)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            Link = GetDelegate<RET_1>(RealHandler, "V2Link", false);
            Unlink = GetDelegate<RET_0>(RealHandler, "V2Unlink", false);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr V2Link(IntPtr Exporter)
        {
            LoadRetail();
            return Link(Exporter);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi)]
        public static IntPtr V2Unlink()
        {
            LoadRetail();
            return Unlink();
        }

        static RET_1 Link;
        static RET_0 Unlink;
    }
}
