﻿using StringReloads;
using System.Runtime.InteropServices;

namespace SRLWrapper
{
    public static unsafe class Exports
    {
        static Exports() {
            EntryPoint._CurrentDLL = Wrapper.Base.Wrapper.GetSRLPath();
        }

        [DllExport(CallingConvention.StdCall)]
        public static void* Process(void* Value) => EntryPoint.Process(Value);

        [DllExport(CallingConvention.StdCall)]
        public static void* GetDirectProcess() => EntryPoint.GetDirectProcess();

        [DllExport(CallingConvention.StdCall)]
        public static void* ProcessW(void* Value) => EntryPoint.ProcessW(Value);

        [DllExport(CallingConvention.StdCall)]
        public static void* GetDirectProcessW() => EntryPoint.GetDirectProcessW();

    }
}
