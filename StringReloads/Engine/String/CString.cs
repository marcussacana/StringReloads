using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace StringReloads.Engine.String
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public unsafe class CString : StringBufferA
    {
        private CString(byte* Ptr) : base(Ptr) { }

        public Encoding ReadEncoding = Config.Default.ReadEncoding;
        public Encoding WriteEncoding = Config.Default.WriteEncoding;

        public static implicit operator CString(byte* Ptr) => new CString(Ptr);
        public static implicit operator CString(void* Ptr) => new CString((byte*)Ptr);
        public static implicit operator CString(IntPtr Ptr) => new CString((byte*)Ptr.ToPointer());

        public static implicit operator void*(CString Str) => Str.Address;
        public static implicit operator byte*(CString Str) => (byte*)Str.Address;

        public static implicit operator string(CString Str)
        {
            var Len =  Str.FixedLength ?? Str.Count();
            return new string(Str.Address, 0, (int)Len, Str.ReadEncoding);
        }

        public static implicit operator CString(string Content)
        {
            var Buffer = Config.Default.WriteEncoding.GetBytes(Content + "\x0");
            var Ptr = (byte*)Marshal.AllocHGlobal(Buffer.Length).ToPointer();

            for (int i = 0; i < Buffer.Length; i++)
                Ptr[i] = Buffer[i];

            return Ptr;
        }

        public void SetContent(string Content)
        {
            unchecked
            {
                var Buffer = WriteEncoding.GetBytes(Content + "\x0");
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