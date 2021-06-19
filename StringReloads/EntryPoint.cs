using StringReloads.Engine;
using StringReloads.Engine.String;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StringReloads
{

    public static unsafe class EntryPoint
    {

        public static string _CurrentDLL = null;
        public static string CurrentDll => _CurrentDLL ?? Assembly.GetExecutingAssembly().Location;

        public static string ApplicationDirectory => System.IO.Path.GetDirectoryName(CurrentDll);

        internal static SRL SRL = new SRL();
        static ProcessDelegate ProcessInstance;
        static ProcessDelegate ProcessWInstance;

        public static void* Process(void* Value) {
            if (Value == null && SRL.Initialized)
                return null;

            int Retries = 0;
        Retry:;
            try
            {
                if ((ulong)Value <= ushort.MaxValue)
                {
                    var NewRemap = (ushort)SRL.ResolveRemap((char)(ushort)Value);
                    Log.Trace($"Character Remaped from {(char)(ushort)Value} to {(char)NewRemap}");
                    return (void*)NewRemap;
                }

                return SRL.ProcessString((CString)Value);
            }
            catch (Exception ex) {
                SRL.HasMatchLocks = new List<object>();
                SRL.HasValueLocks = new List<object>();
                SRL.MatchStringLocks = new List<object>();
                SRL.ResolveRemapLocks = new List<object>();
                Log.Error(ex.ToString());
                if (Retries++ < 5)
                    goto Retry;
                Log.Critical(ex.ToString());
                throw;
            }
        }
        public static void* ProcessW(void* Value) {
            if (Value == null && SRL.Initialized)
                return null;

            int Retries = 0;
        Retry:;
            try
            {
                if ((ulong)Value <= ushort.MaxValue)
                {
                    var NewRemap = (ushort)SRL.ResolveRemap((char)(ushort)Value);
                    Log.Trace($"Character Remaped from {(char)(ushort)Value} to {(char)NewRemap}");
                    return (void*)NewRemap;
                }

                return SRL.ProcessString((WCString)Value);
            }
            catch (Exception ex) {
                SRL.HasMatchLocks = new List<object>();
                SRL.HasValueLocks = new List<object>();
                SRL.MatchStringLocks = new List<object>();
                SRL.ResolveRemapLocks = new List<object>();
                Log.Error(ex.ToString());
                if (Retries++ < 5)
                    goto Retry;
                Log.Critical(ex.ToString());
                throw;
            }
        }

        public static void* GetDirectProcess() {
            ProcessInstance = new ProcessDelegate(Process);
            return Marshal.GetFunctionPointerForDelegate(ProcessInstance).ToPointer();
        }
        public static void* GetDirectProcessW()
        {
            ProcessWInstance = new ProcessDelegate(ProcessW);
            return Marshal.GetFunctionPointerForDelegate(ProcessWInstance).ToPointer();
        }

        public static void PipeServer(string PipeName)
        {
            System.Diagnostics.Debugger.Launch();
            Pipe.Run(PipeName); 
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void* ProcessDelegate(void* Value);
    }
}
