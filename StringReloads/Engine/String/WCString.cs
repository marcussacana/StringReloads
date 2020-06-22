using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace StringReloads.Engine.String
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    unsafe class WCString : UnsafeString
    {
        public override Encoding Encoding => Encoding.Unicode;

        private WCString() { }

        public readonly static byte[] Termination = new byte[] { 0x00, 0x00 };

        public override long? FixedLength { get; set;}

        public static implicit operator WCString(string Str)
        {

            byte[] Buffer = GetBytes(Str);
            if (Buffer == null)
                return new WCString() { BasePtr = null };

            var Addr = Marshal.AllocHGlobal(Buffer.Length);
            Marshal.Copy(Buffer, 0, Addr, Buffer.Length);

            var CStr = new WCString();
            CStr.BasePtr = CStr.CurrentPtr = (byte*)Addr.ToPointer();

            return CStr;
        }

        public static implicit operator WCString(void* Ptr)
        {
            var UCS = new WCString();
            UCS.BasePtr = (byte*)Ptr;
            UCS.CurrentPtr = (byte*)Ptr;

            return UCS;
        }

        public static implicit operator WCString(char* Ptr)
        {
            var UCS = new WCString();
            UCS.BasePtr = (byte*)Ptr;
            UCS.CurrentPtr = (byte*)Ptr;

            return UCS;
        }

        public static implicit operator WCString(byte* Ptr)
        {
            var UCS = new WCString();
            UCS.BasePtr = Ptr;
            UCS.CurrentPtr = Ptr;

            return UCS;
        }


        public static implicit operator string(WCString Instance)
        {
            if (Instance.BasePtr == null)
                return null;

            byte[] Buffer = new byte[Instance.FixedLength ?? (Instance.Count() * 2)];
            Marshal.Copy(new IntPtr(Instance.BasePtr), Buffer, 0, Buffer.Length);

            return Instance.Encoding.GetString(Buffer);
        }

        public override byte? GetCurrent()
        {
            if (IsEnd(CurrentPtr))
            {
                return null;
            }
            return *CurrentPtr;
        }

        private bool IsEnd(byte* Address)
        {
            if (FixedLength.HasValue)
                return (Address - BasePtr) >= FixedLength.Value;

            for (int i = 0; i < Termination.Length; i++)
            {
                if (*(Address + i) != Termination[i])
                    return false;
            }
            return true;
        }
        public override bool MoveNext()
        {
            if (IsEnd(CurrentPtr))
                return false;

            CurrentPtr += 2;
            return true;
        }

        public static byte[] GetBytes(string Content)
        {
            if (Content == null)
                return null;

            byte[] Buffer = Encoding.Unicode.GetBytes(Content);
            return Buffer.Concat(Termination).ToArray();
        }

        public void CopyTo(void* NewAddress)
        {
            ((WCString)NewAddress).SetString(this);
        }

        public void SetString(WCString Content) => SetString((string)Content);
        public void SetString(string Content)
        {
            byte[] Buffer = Encoding.Unicode.GetBytes(Content + "\x0");
            SetBytes(Buffer);
        }
        public void SetBytes(byte[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                BasePtr[i] = Data[i];
            }

            if (FixedLength.HasValue)
                FixedLength = Data.Length / 2;
        }

        public override string ToString()
        {
            return this;
        }

        private string DebuggerDisplay
        {
            get
            {
                if (System.IntPtr.Size == 4)
                    return $"[0x{(ulong)BasePtr:X8}] {(string)this}";
                return $"[0x{(ulong)BasePtr:X16}] {(string)this}";
            }
        }
    }
}
