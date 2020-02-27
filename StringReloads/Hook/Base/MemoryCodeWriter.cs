using Iced.Intel;
using System.Collections.Generic;

namespace StringReloads.Hook.Base
{
    public class MemoryCodeWriter : CodeWriter
    {
        List<byte> Buffer = new List<byte>();
        public override void WriteByte(byte value)
        {
            Buffer.Add(value);
        }

        byte this[int Address] {
            get {
                return Buffer[Address];
            }
            set {
                Buffer[Address] = value;
            }
        }

        public void CopyTo(byte[] Buffer, int Index)
        {
            for (int i = 0; i < this.Buffer.Count; i++)
                Buffer[Index + i] = this.Buffer[i];
        }
        public unsafe void CopyTo(byte* Buffer, int Index)
        {
            for (int i = 0; i < this.Buffer.Count; i++)
                *(Buffer + Index + i) = this.Buffer[i];
        }

        public byte[] ToArray() => Buffer.ToArray();

        public void Clear() => Buffer.Clear();

        public int Count => Buffer.Count;
    }
}
