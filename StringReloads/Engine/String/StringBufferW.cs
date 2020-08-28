using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StringReloads.Engine.String
{
    public unsafe abstract class StringBufferW : IEnumerable<byte>
    {
        public uint? FixedLength = null;
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

                yield return (byte)(Current >> 0x08);
                yield return (byte)(Current & 0xFF);

                if (FixedLength != null && Length++ > FixedLength)
                    yield break;
            }
        }

#if x64

        public StringBufferW(void* Address) => Ptr = (ulong)Address;
        public StringBufferW(byte* Address) => Ptr = (ulong)Address;

        ulong Ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort ReadNext(ref ulong Pointer)
        {
            var Word = *(ushort*)Pointer;
            Pointer += 2;
            return Word;
        }
#else

        public StringBufferW(void* Address) => Ptr = (uint)Address;
        public StringBufferW(byte* Address) => Ptr = (uint)Address;

        uint Ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort ReadNext(ref uint Pointer)
        {
            var Word = *(ushort*)Pointer;
            Pointer += 2;
            return Word;
        }

#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
