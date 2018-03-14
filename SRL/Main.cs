using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;

namespace SRL {
    public partial class StringReloader {
        
        [DllExport]
        public static IntPtr Process(IntPtr Target) {
            again:;
            int Tries = 0;
            try {
                DateTime Begin = DelayTest ? DateTime.Now : DateTime.FromFileTimeUtc(1512840324); //wtf this is causing a exception
                dynamic Ptr = ParsePtr(Target);

                if (StrRld == null) {
                    try {
                        Init();
                        Log("Initiallized", true);
                    } catch (Exception ex) {
                        throw ex;
                    }
                    Initialized = true;                    
                }
                
                if (Ptr == 0)
                    return IntPtr.Zero;

                if (Ptr <= char.MaxValue) {
                    return ProcessChar(Target);
                }

                if (CachePointers) {
                    if (PtrCacheIn.Contains(Target))
                        return PtrCacheOut[PtrCacheIn.IndexOf(Target)];
                }

                string Input = GetString(Target);
                
                if (string.IsNullOrWhiteSpace(Input))
                    return Target;

                string Reloaded = StrMap(Input, Target, false);

                LastInput = Input;
                
                //Prevent inject a string already injected
                if (Input == Reloaded)
                    return Target;

                CacheReply(Reloaded);
                TrimWorker(ref Reloaded, Input);

                UpdateOverlay(Reloaded);

                if (NoReload)
                    return Target;

                if (LogString) {
                    Log("Output: {0}", true, Reloaded);
                }

                IntPtr Output = GenString(Reloaded);
                
                AddPtr(Output);
                AddPtr(Target);

                if (CachePointers)
                    CachePtr(Target, Output);

                if (DelayTest)
                    Log("Delay - {0}ms", false, (DateTime.Now - Begin).TotalMilliseconds);

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
            string Paramter = GetStringA(hCmdLine);
            ServiceCall(Paramter);
            return IntPtr.Zero;
        }

        public static string ProcessManagerd(string Text) {
            Managed = true;
            IntPtr Ptr = Marshal.StringToHGlobalAuto(Text);
            IntPtr New = Process(Ptr);
            Text = Marshal.PtrToStringAuto(New);
            Marshal.FreeHGlobal(Ptr);
            return Text;
        }
        public static char ProcessManagerd(char Char) {
            Managed = true;
            return (char)(Process(new IntPtr(Char)).ToInt32() & 0xFFFF);
        }
    }
}
