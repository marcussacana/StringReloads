using System;
using Iced.Intel;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Base
{
    public abstract unsafe class Intercept
    {
        public Intercept() { }

        public Intercept(void* Address) {
            this.Address = Address;
            AssemblyHook();
        }

        ~Intercept() {
            Uninstall();
        }

        public void Compile(void* Address) {
            this.Address = Address;
            AssemblyHook();

        }

        public abstract InterceptDelegate HookFunction { get; }

        void* Address;
        void* HookAddress;

        public byte[] RealBuffer;
        public byte[] HookBuffer;

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
            var IInstructions = new InstructionList{
                Instruction.Create(Code.Pushad),
                Instruction.Create(Code.Push_r32, Register.ESP),
                Instruction.CreateBranch(Code.Call_rel32_32, (UnsafeDelegate)HookFunction),
                Instruction.Create(Code.Popad)
            };

            IInstructions.AddRange(Instructions);

            IInstructions.Add(Instruction.CreateBranch(Code.Jmp_rel32_32, RetAddr));

            //Encode Interceptor
            uint BufferSize = (uint)IInstructions.GetEncodedSize(32);
            var phBuffer = Extensions.AllocUnsafe(BufferSize);
            Extensions.DeprotectMemory(phBuffer, BufferSize);
            HookAddress = phBuffer;

            Compiler.Encode(IInstructions, (ulong)phBuffer);

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
            Extensions.DeprotectMemory(Address, (uint)HookBuffer.Length);
            Marshal.Copy(HookBuffer, 0, new IntPtr(Address), HookBuffer.Length);
        }

        public void Uninstall() {
            Extensions.DeprotectMemory(Address, (uint)RealBuffer.Length);
            Marshal.Copy(RealBuffer, 0, new IntPtr(Address), RealBuffer.Length);
        }

    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate void InterceptDelegate(void* ESP);
}
