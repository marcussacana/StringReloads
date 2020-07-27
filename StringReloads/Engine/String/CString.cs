using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;

namespace StringReloads.Engine.String
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public unsafe class CString : UnsafeString
    {
        Encoding _Enco = null;
        public override Encoding Encoding => _Enco ?? base.Encoding;

        private CString() { }

        public static byte[] _Termination;
        public static byte[] Termination => _Termination ??= Config.Default.ReadEncoding.GetTermination();

        public override long? FixedLength { get; set; }

        public static implicit operator CString(string Str) {

            byte[] Buffer = GetBytes(Str);
            if (Buffer == null)
                return new CString() { BasePtr = null };

            var Addr = Marshal.AllocHGlobal(Buffer.Length);
            Marshal.Copy(Buffer, 0, Addr, Buffer.Length);

            var CStr = new CString();
            CStr.BasePtr = CStr.CurrentPtr = (byte*)Addr.ToPointer();

            return CStr;
        }

        public static implicit operator CString(byte* Ptr) {
            var UCS = new CString();
            UCS.BasePtr = Ptr;
            UCS.CurrentPtr = Ptr;

            return UCS;
        }

        public static implicit operator CString(void* Ptr) {
            var UCS = new CString();
            UCS.BasePtr = (byte*)Ptr;
            UCS.CurrentPtr = (byte*)Ptr;

            return UCS;
        }

        public override byte? GetCurrent() {
            if (IsEnd(CurrentPtr))
            {
                return null;
            }
            return *CurrentPtr;
        }

        private bool IsEnd(byte* Address) {
            if (FixedLength.HasValue)
                return (Address - BasePtr) >= FixedLength.Value;

            for (int i = 0; i < Termination.Length; i++) {
                if (*(Address + i) != Termination[i])
                    return false;
            }
            return true;
        }
        public override bool MoveNext()
        {
            if (IsEnd(CurrentPtr))
                return false;

            CurrentPtr++;
            return true;
        }

        public static byte[] GetBytes(string Content)
        {
            if (Content == null)
                return null;

            byte[] Buffer = Config.Default.WriteEncoding.GetBytes(Content);
            return Buffer.Concat(Termination).ToArray();
        }

        public void CopyTo(void* NewAddress)
        {
            ((CString)NewAddress).SetString(this);
        }

        public void SetString(CString Content) => SetString((string)Content);
        public void SetString(string Content) {
            byte[] Buffer = Config.Default.WriteEncoding.GetBytes(Content + "\x0");
            SetBytes(Buffer);
        }
        public void SetBytes(byte[] Data) {
            for (int i = 0; i < Data.Length; i++) {
               BasePtr[i] = Data[i];
            }

            if (FixedLength.HasValue)
                FixedLength = Data.Length;
        }

        private string DebuggerDisplay { get {
                if (System.IntPtr.Size == 4)
                    return $"[0x{(ulong)BasePtr:X8}] {(string)this}";
                return $"[0x{(ulong)BasePtr:X16}] {(string)this}";
            }
        }
    }
}
