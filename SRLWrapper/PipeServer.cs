using System.Runtime.InteropServices;
using System;
using StringReloads;

namespace SRLWrapper
{
    unsafe static class PipeServer
    {

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr Server(IntPtr hWnd, IntPtr hInst, IntPtr hCmdLine, int nCmdShow)
        {
            EntryPoint._CurrentDLL = Wrapper.Base.Wrapper.GetSRLPath();
            EntryPoint.PipeServer(Marshal.PtrToStringAnsi(hCmdLine));
            return IntPtr.Zero;
        }
       
    }
}
