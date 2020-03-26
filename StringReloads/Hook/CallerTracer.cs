using StringReloads.Hook.Base;
using System;
using static StringReloads.Hook.Base.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.Hook
{
    unsafe class CallerTracer : Intercept
    {
        public CallerTracer(void* Function) : base(Function) {}
        public override InterceptDelegate HookFunction => new InterceptDelegate(Interception);

        public event OnCallerCatch CallerCatched;
        public void Interception(void* ESP) {
            if (Environment.Is64BitProcess) {
                throw new NotImplementedException();
            }
            else {
                uint* Stack = ((uint*)ESP) + 8;
                CallerCatched?.Invoke((void*)*Stack);
            }
        }
        
        public delegate void OnCallerCatch(void* Address);

        public void* SearchFuntionAddress(byte* At) {
            try
            {
                int Range = 0;
                while (Range < 0x1000) {
                    DeprotectMemory(At, 3);

                    if (Equals(FunctionPrefix, At - Range))
                        return At - Range;
                    if (Equals(EmptyPrefix, At - Range))
                        return null;

                    Range++;
                }
            }
            catch { }

            return null;
        }

        private bool Equals(byte[] Buffer, byte* Address) {
            for (int i = 0; i < Buffer.Length; i++)
                if (Buffer[i] != *Address++)
                    return false;
            return true;
        }

        static byte[] FunctionPrefix = { 0x55, 0x8B, 0xEC };
        static byte[] EmptyPrefix = { 0xCC, 0xCC, 0xCC };
    }
}
