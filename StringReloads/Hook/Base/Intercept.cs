using System;
using Iced.Intel;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Base
{
    public abstract unsafe class Intercept
    {
        public Intercept() { Initialize(); }

        ~Intercept() {
            Uninstall();
        }

        public abstract InterceptDelegate HookFunction { get; set; }

        void* Address;
        void* HookAddress;

        public byte[] RealBuffer;
        public byte[] HookBuffer;

        public void Compile(void* Address) {
            this.Address = Address;
            AssemblyHook();
        }

#if x64
        private void AssemblyHook()
        {
            throw new NotImplementedException();
        }
#else
        const int JmpSize = 5;
        private void AssemblyHook()
        {
            //Copy Minimal Instructions Amount
            var Instructions = new InstructionList();
            var Reader = new MemoryCodeReader(Address, 100);
            var Decoder = Iced.Intel.Decoder.Create(32, Reader);
            Decoder.IP = (ulong)Address;
            Instructions.AddRange(Decoder.DecodeMany(JmpSize));
            var RetAddr = Decoder.IP;

            //Allocate Memory
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(32, Writer);

            int InterceptSize = Instructions.GetEncodedSize(32);

            RealBuffer = new byte[InterceptSize];
            HookBuffer = new byte[InterceptSize + JmpSize];

            Extensions.DeprotectMemory(Address, (uint)RealBuffer.Length);
            Marshal.Copy(new IntPtr(Address), RealBuffer, 0, RealBuffer.Length);

            //Assemble Interceptor
            var IInstructions = new InstructionList
                {
                    Instruction.Create(Code.Pushad),
                    Instruction.Create(Code.Pushd_ES, Register.ESP),
                    Instruction.CreateBranch(Code.Call_rel32_32, (UnsafeDelegate)HookFunction),
                    Instruction.Create(Code.Popad)
                };

            Instructions.Add(Instruction.CreateBranch(Code.Jmp_rel32_32, RetAddr));

            //Encode Interceptor
            var phBuffer = Extensions.AllocUnsafe(new byte[IInstructions.GetEncodedSize(32)]);
            HookAddress = phBuffer;

            Compiler.Encode(Instructions, (ulong)phBuffer);

            Writer.CopyTo(phBuffer, 0);


            //Assemble the Hook
            Writer = new MemoryCodeWriter();
            Compiler = Encoder.Create(32, Writer);
            var jmp = Instruction.CreateBranch(Code.Jmp_rel32_32, (ulong)HookAddress);
            Compiler.Encode(jmp, (ulong)Address);

            HookBuffer = Writer.ToArray();
        }

#endif

        public void Install() {
            VirtualProtect(Address, (uint)HookBuffer.Length, PAGE_EXECUTE_READWRITE, out _);
            Marshal.Copy(HookBuffer, 0, new IntPtr(Address), HookBuffer.Length);
        }

        public void Uninstall() {
            VirtualProtect(Address, (uint)RealBuffer.Length, PAGE_EXECUTE_READWRITE, out _);
            Marshal.Copy(RealBuffer, 0, new IntPtr(Address), RealBuffer.Length);
        }

        public abstract void Initialize();

        const uint PAGE_EXECUTE_READWRITE = 0x40;

        [DllImport("kernel32.dll")]
        static extern bool VirtualProtect(void* lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate void InterceptDelegate(void* ESP);
}
