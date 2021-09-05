using System;
using Iced.Intel;
using System.Runtime.InteropServices;
using static StringReloads.Hook.Base.Extensions;
using StringReloads.Engine;
using System.Runtime.Remoting.Contexts;

namespace StringReloads.Hook.Base
{
    public abstract unsafe class Intercept
    {
        public Intercept() { }

        public Intercept(void* Address)
        {
            this.Address = Address;
            AssemblyHook();
        }

        public Intercept(string Module, string Export) : this(GetProcAddress(LoadLibrary(Module), Export)) { }

        ~Intercept()
        {
            Uninstall();
        }

        public void Compile(void* Address)
        {
            this.Address = Address;
            AssemblyHook();

        }

        public virtual InterceptDelegate HookFunction { get => InterceptManager; }
        public virtual ManagedInterceptDelegate ManagedHookFunction { get => null; }

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

            DeprotectMemory(Address, (uint)RealBuffer.Length);
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
            var phBuffer = AllocUnsafe(BufferSize);
            DeprotectMemory(phBuffer, BufferSize);
            HookAddress = phBuffer;

            Log.Debug($"Intercept 0x{(ulong)Address:X8} With 0x{(ulong)phBuffer:X8}");

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

        public void Install()
        {
            DeprotectMemory(Address, (uint)HookBuffer.Length);
            Marshal.Copy(HookBuffer, 0, new IntPtr(Address), HookBuffer.Length);
        }

        public void Uninstall()
        {
            DeprotectMemory(Address, (uint)RealBuffer.Length);
            Marshal.Copy(RealBuffer, 0, new IntPtr(Address), RealBuffer.Length);
        }

        void InterceptManager(void* ESP)
        {
            uint* Stack = (uint*)ESP;

            ulong EDI = Stack[0];
            ulong ESI = Stack[1];
            ulong EBP = Stack[2];
            ulong EBX = Stack[4];
            ulong EDX = Stack[5];
            ulong ECX = Stack[6];
            ulong EAX = Stack[7];

            var OriESP = (void**)Stack[3];

            ManagedHookFunction(ref OriESP, ref EAX, ref ECX, ref EDX, ref EBX, ref EBP, ref ESI, ref EDI);

            Stack[0] = (uint)EDI;
            Stack[1] = (uint)ESI;
            Stack[2] = (uint)EBP;
            Stack[4] = (uint)EBX;
            Stack[5] = (uint)EDX;
            Stack[6] = (uint)ECX;
            Stack[7] = (uint)EAX;

            Stack[3] = (uint)OriESP;
        }

    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate void InterceptDelegate(void* ESP);

    public unsafe delegate void ManagedInterceptDelegate(ref void** ESP, ref ulong EAX, ref ulong ECX, ref ulong EDX, ref ulong EBX, ref ulong EBP, ref ulong ESI, ref ulong EDI);

}
