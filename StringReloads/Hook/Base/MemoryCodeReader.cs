using Iced.Intel;
using System;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Base
{
    unsafe class MemoryCodeReader : CodeReader, IDisposable
    {
        byte* Address;
        void* End;

        uint Length = 0;

        public MemoryCodeReader(void* Address) : this((byte*)Address) { }
        public MemoryCodeReader(byte* Address) : this(Address, 0) { }
        public MemoryCodeReader(void* Address, uint Length) : this((byte*)Address, Length) { }
        public MemoryCodeReader(byte* Address, uint Length)
        {
            this.Address = Address;
            End = null;
            this.Length = Length;
            if (Length > 0)
            {
                End = this.Address + Length;
                VirtualProtect(Address, Length, PAGE_EXECUTE_READ, out OldProtection);
            }
        }

        ~MemoryCodeReader()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (End != null)
                VirtualProtect(Address, Length, OldProtection, out _);
        }

        public override int ReadByte()
        {
            if (End != null && Address >= End)
                return -1;

            return *Address++;
        }

        uint OldProtection = 0;
        const uint PAGE_EXECUTE_READ = 0x20;


        [DllImport("kernel32.dll")]
        static extern bool VirtualProtect(void* lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
    }
}
