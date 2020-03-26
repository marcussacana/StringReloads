using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace StringReloads.Engine.String
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    unsafe class CString : UnsafeString
    {
        Encoding _Enco = null;
        public override Encoding Encoding => _Enco ?? base.Encoding;

        private CString() { }

        static byte[] _Termination;
        public static byte[] Termination => _Termination ?? (_Termination = Config.Default.ReadEncoding.GetBytes("\x0"));

        public static implicit operator CString(string Str) {

            byte[] Buffer = GetBytes(Str);
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

        public override byte? GetCurrent() {
            if (IsEnd(CurrentPtr))
            {
                return null;
            }
            return *CurrentPtr;
        }

        private bool IsEnd(byte* Address) {
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
            byte[] Buffer = Config.Default.WriteEncoding.GetBytes(Content);
            return Buffer.Concat(Termination).ToArray();
        }

        private string DebuggerDisplay { get {
                if (System.IntPtr.Size == 4)
                    return $"[0x{(ulong)BasePtr:X8}] {(string)this}";
                return $"[0x{(ulong)BasePtr:X16}] {(string)this}";
            }
        }
    }
}
