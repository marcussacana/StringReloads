//using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;

namespace SRL {
    public partial class StringReloader {


        [DllExport]
        public static IntPtr Process(IntPtr Target) => ProcessReal(Target);

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr ProcessStd(IntPtr Target) => ProcessReal(Target);

        private static IntPtr ProcessReal(IntPtr Target) {
            again:;
            int Tries = 0;
            try {
                DateTime? Begin = DelayTest ? DateTime.Now : (DateTime?)null;
                dynamic Ptr = ParsePtr(Target);

                if (StrRld == null) {
                    try {
                        Init();
                        Log("Initiallized", true);
                    } catch (Exception ex) {
                        PreserveStackTrace(ex);
                        throw ex;
                    }
                    Initialized = true;
                }

                if (Ptr == 0)
                    return IntPtr.Zero;


#if DEBUG
                if (LogAll) {
                    Log("Target: {0} | Ptr: {1} | char.MaxValue {2} | Convert: {3}", true, Target.ToString(), Ptr, char.MaxValue, unchecked((uint)Ptr));
                }

#endif                

                if (!LiteMode) {
                    if (Ptr <= char.MaxValue) {
                        return ProcessChar(Target);
                    }

                    if (CachePointers) {
                        if (PtrCacheIn.Contains(Target))
                            return PtrCacheOut[PtrCacheIn.IndexOf(Target)];
                    }
                }

                if (IsBadCodePtr(Target) && Ptr >= char.MaxValue) {
                    if (LogAll) {
                        Log("BAD PTR: {0}", true, Ptr);
                    }
                    return Target;
                }

                string Input = GetString(Target);

                if (string.IsNullOrWhiteSpace(Input))
                    return Target;

                string Reloaded = StrMap(Input, Target, false);

                LastInput = Input;

                if (Input != Reloaded) {
                    Reloaded = UpdateOverlay(Reloaded);
                }

                //Prevent inject a string already injected
                if (Input == Reloaded) {
                    return Target;
                }

                DialogFound = true;

                if (!LiteMode) {
                    if (StringModifier != null) {
                        try {
                            Reloaded = StringModifier.Call("Modifier", "ResultHook", Reloaded);
                        } catch {
                            Log("Result Hook Error...", true);
                        }
                    }

                    CacheReply(Reloaded);
                    TrimWorker(ref Reloaded, Input);

                    if (NoReload)
                        return Target;

                    if (LogAll || LogOutput) {
                        Log("Output: {0}", true, Reloaded);
                    }

                }
                IntPtr Output = GenString(Reloaded);

                if (!LiteMode) {
                    AddPtr(Output);
                    AddPtr(Target);

                    if (CachePointers)
                        CachePtr(Target, Output);

                    if (DelayTest)
                        Log("Delay - {0}ms", false, (DateTime.Now - Begin)?.TotalMilliseconds);
                }

                return Output;
            } catch (Exception ex) {
                Error("Ops, a Bug...\n{0}\n======================\n{1}\n============================\n{2}", ex.Message, ex.StackTrace, ex.Data);

                if (Tries++ < 3)
                    goto again;
                Initialized = true;
            }
            return Target;
        }


        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr Service(IntPtr hWnd, IntPtr hInst, IntPtr hCmdLine, int nCmdShow) {
            hConsole = hCmdLine;
            string Parameter = GetStringA(hCmdLine);
            ServiceCall(Parameter);
            return IntPtr.Zero;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr ProcDelegate(IntPtr Target);
        private static ProcDelegate DirectProc = new ProcDelegate(ProcessReal);

        [DllExport]
        public static IntPtr GetDirectProcess() {
            try {
                DirectRequested = true;             
                return Marshal.GetFunctionPointerForDelegate(DirectProc);
            } catch { return IntPtr.Zero; }
        }

        public static string ProcessManaged(string Text) {
            Managed = true;
            IntPtr Ptr = Marshal.StringToHGlobalAuto(Text);
            IntPtr New = ProcessReal(Ptr);
            if (New == Ptr)
                return Text;
            Text = Marshal.PtrToStringAuto(New);
            Marshal.FreeHGlobal(Ptr);
            return Text;
        }

        static string ProcessManaged(string Text, bool Managed) {
            bool IsManaged = Managed;
            string Result = ProcessManaged(Text);
            if (!Managed)
                Managed = IsManaged;

            return Result;
        }

        public static char ProcessManaged(char Char) {
            Managed = true;
            IntPtr Result = ProcessReal(new IntPtr(Char));
            return (char)(Result.ToInt32() & 0xFFFF);
        }
    }
}
