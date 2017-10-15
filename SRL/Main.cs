using System;
using System.Runtime.InteropServices;

namespace SRL {
    public partial class StringReloader {
        
        [DllExport]
        public static IntPtr Process(IntPtr Reload) {
            again:;
            int Tries = 0;
            try {
                DateTime Begin = DateTime.Now;
                int Ptr = ParsePtr(Reload);

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
                    return ProcessChar(Reload);
                }

                string Input = GetString(Reload);
                
                if (string.IsNullOrWhiteSpace(Input))
                    return Reload;

                string Reloaded = StrMap(Input, Reload);

                LastInput = Input;
                
                //Prevent inject a string already injected
                if (Input == Reloaded)
                    return Reload;
                else
                    CacheReply(Reloaded);

                TrimWorker(ref Reloaded, Input);

                IntPtr Output = GenString(Reloaded);
                
                AddPtr(Output);

                if (DelayTest)
                    Log("Delay - {0}ms", false, (DateTime.Now - Begin).TotalMilliseconds);

                return Output;
            } catch (Exception ex) {
                Log("Ops, a Bug...\n{0}\n======================\n{1}\n============================\n{2}", false, ex.Message, ex.StackTrace, ex.Data);

                if (Tries++ < 3)
                    goto again;
                Initialized = true;
            }
            return Reload;
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr Service(IntPtr hWnd, IntPtr hInst, IntPtr hCmdLine, int nCmdShow) {
            hConsole = hCmdLine;
            string Paramter = GetStringA(hCmdLine);
            ServiceCall(Paramter);
            return IntPtr.Zero;
        }
    }
}
