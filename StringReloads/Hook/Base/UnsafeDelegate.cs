using System;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Base
{
    public unsafe class UnsafeDelegate<T> : IDisposable where T : Delegate {
        private UnsafeDelegate(void* Address) { this.Address = new IntPtr(Address); }

        ~UnsafeDelegate() {
            Dispose();
        }

        IntPtr Address = IntPtr.Zero;

        private T Delegate;


        public static implicit operator UnsafeDelegate<T>(void* Address) {
            return new UnsafeDelegate<T>(Address);
        }

        public static implicit operator T(UnsafeDelegate<T> Del) {
            if (Del.Delegate == null)
                Del.Delegate = (T)Marshal.GetDelegateForFunctionPointer(Del.Address, typeof(T));
            GC.SuppressFinalize(Del.Delegate);
            return Del.Delegate;
        }

        public void Dispose()
        {
            GC.ReRegisterForFinalize(Delegate);
        }
    }
    public unsafe class UnsafeDelegate : IDisposable {

        Delegate Base;

        public UnsafeDelegate(Delegate Base) { this.Base = Base; }

        ~UnsafeDelegate() {
            Dispose();
        }

        public void Dispose()
        {
            GC.ReRegisterForFinalize(Base);
        }

        public static implicit operator UnsafeDelegate(Delegate Del) {
            GC.SuppressFinalize(Del);
            return new UnsafeDelegate(Del);
        }
        public static implicit operator void*(UnsafeDelegate Function) {
            return Marshal.GetFunctionPointerForDelegate(Function.Base).ToPointer();
        }
        public static implicit operator ulong(UnsafeDelegate Function) {
            return (ulong)(void*)Function;
        }
    }
}
