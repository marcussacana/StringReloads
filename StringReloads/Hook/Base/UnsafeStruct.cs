using System;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Base
{
    public unsafe class UnsafeStruct<T> : IDisposable where T : struct
    {
        private UnsafeStruct(void* Address) { this.Address = new IntPtr(Address); }

        ~UnsafeStruct()
        {
            Dispose();
        }

        IntPtr Address = IntPtr.Zero;

        private T Structure;


        public static implicit operator UnsafeStruct<T>(void* Address)
        {
            return new UnsafeStruct<T>(Address);
        }

        public static implicit operator T(UnsafeStruct<T> Struct)
        {
            if (Struct.Address != IntPtr.Zero)
                Struct.Structure = (T)Marshal.PtrToStructure(Struct.Address, typeof(T));
            else
                Struct.Structure = default;

            GC.SuppressFinalize(Struct.Structure);
            return Struct.Structure;
        }

        public static implicit operator void*(UnsafeStruct<T> Struct) {
            if (Struct.Address == IntPtr.Zero)
                Struct.Address =  Marshal.AllocHGlobal(Marshal.SizeOf(Struct.Structure));

            Marshal.StructureToPtr(Struct.Structure, Struct.Address, true);
            return Struct.Address.ToPointer();
        }

        public static implicit operator ulong(UnsafeStruct<T> Struct) {
            return (ulong)(void*)Struct;
        }

        public void SetData(T NewData) {
            Structure = NewData;
            Marshal.StructureToPtr(Structure, Address, true);
        }

        public void Dispose()
        {
            GC.ReRegisterForFinalize(Structure);
        }
    }
}
