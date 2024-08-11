using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace StringReloads.Engine.String
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public unsafe class WCString : StringBufferW
    {
        private WCString(byte* Ptr) : base(Ptr) { }

        public static implicit operator WCString(byte* Ptr) => new WCString(Ptr);
        public static implicit operator WCString(long Ptr) => new WCString((byte*)Ptr);
        public static implicit operator WCString(ulong Ptr) => new WCString((byte*)Ptr);
        public static implicit operator WCString(void* Ptr) => new WCString((byte*)Ptr);
        public static implicit operator WCString(IntPtr Ptr) => new WCString((byte*)Ptr.ToPointer());

        public static implicit operator void*(WCString Str) => Str.Address;
        public static implicit operator byte*(WCString Str) => (byte*)Str.Address;

        public static implicit operator string(WCString Str)
        {
            var Len = Str.FixedLength ?? Str.Count();
            return new string(Str.Address, 0, (int)Len, Encoding.Unicode);
        }

        public static implicit operator WCString(string Content)
        {
            var Buffer = Encoding.Unicode.GetBytes(Content + "\x0");
            var Ptr = (byte*)Marshal.AllocHGlobal(Buffer.Length).ToPointer();

            for (int i = 0; i < Buffer.Length; i++)
                Ptr[i] = Buffer[i];

            return Ptr;
        }

        public void SetContent(string Content)
        {
            unchecked
            {
                var Buffer = Encoding.Unicode.GetBytes(Content + "\x0");
                for (int i = 0; i < Buffer.Length; i++)
                    Address[i] = (sbyte)Buffer[i];
            }
        }

        private string DebuggerDisplay
        {
            get
            {
                if (Environment.Is64BitProcess)
                    return $"[0x{(ulong)Address:X16}] {(string)this}";
                return $"[0x{(uint)Address:X8}] {(string)this}";
            }
        }
    }
}