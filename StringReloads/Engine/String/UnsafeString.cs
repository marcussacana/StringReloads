using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringReloads.Engine.String
{
    internal unsafe abstract class UnsafeString : IEnumerable<byte>, IEnumerator<byte>
    {
        public virtual Encoding Encoding => Config.Default.ReadEncoding;

        public byte* BasePtr;
        public byte* CurrentPtr;

        public static implicit operator void*(UnsafeString Instance)
        {
            return Instance.BasePtr;
        }

        public static implicit operator byte*(UnsafeString Instance)
        {
            return Instance.BasePtr;
        }

        public static implicit operator char*(UnsafeString Instance)
        {
            return (char*)Instance.BasePtr;
        }

        public static implicit operator string(UnsafeString Instance)
        {
            if (Instance.BasePtr == null)
                return null;

            return new string((sbyte*)Instance.BasePtr, 0, Instance.Count(), Instance.Encoding);
        }

        public byte Current => GetCurrent().Value;

        object IEnumerator.Current => (GetCurrent() ?? null);

        public abstract byte? GetCurrent();

        public void Dispose() { }

        public virtual IEnumerator<byte> GetEnumerator()
        {
            Reset();
            return this;
        }

        public abstract bool MoveNext();

        public virtual void Reset()
        {
            CurrentPtr = BasePtr;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Reset();
            return this;
        }
    }
}
