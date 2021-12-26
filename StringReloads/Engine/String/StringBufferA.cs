using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StringReloads.Engine.String
{
    public unsafe abstract class StringBufferA : IEnumerable<byte>
    {
        public long? FixedLength = null;
        public sbyte* Address => (sbyte*)Ptr;
        public IEnumerator<byte> GetEnumerator()
        {
            int Length = 0;
            var Ptr = this.Ptr;
            while (true)
            {
                var Current = ReadNext(ref Ptr);
                if (Current == 0)
                    yield break;

                yield return Current;

                if (FixedLength != null && Length++ > FixedLength)
                    yield break;
            }
        }

#if x64

        public StringBufferA(void* Address) => Ptr = (ulong)Address;
        public StringBufferA(byte* Address) => Ptr = (ulong)Address;

        ulong Ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte ReadNext(ref ulong Pointer) => *(byte*)Pointer++;
#else

        public StringBufferA(void* Address) => Ptr = (uint)Address;
        public StringBufferA(byte* Address) => Ptr = (uint)Address;

        uint Ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte ReadNext(ref uint Pointer) => *(byte*)Pointer++;

#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
