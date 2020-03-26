using System;
using Iced.Intel;
using System.Runtime.InteropServices;

using static StringReloads.Hook.Base.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace StringReloads.Hook.Base
{
    public abstract unsafe class Hook<T> : Hook where T : Delegate
    {
        private UnsafeDelegate HookInstance;
        private UnsafeDelegate<T> BypassInstance;

        public T Bypass
        {
            get {
                if (BypassInstance == null)
                    BypassInstance = BypassFunction;

                return BypassInstance;
            }
        }

        public virtual T HookDelegate {
            set {
                HookInstance = value;
                HookFunction = HookInstance;
            }
        }
    }

    public abstract unsafe class Hook
    {
        public Hook() { Initialize(); }

        ~Hook()
        {
            Uninstall();
        }

        public abstract string Library { get; }
        public abstract string Export { get; }
        public virtual ushort Ordinal => 0;

        public virtual string Name => Export;

        /// <summary>
        /// The hook function address
        /// </summary>
        public virtual void* HookFunction { get; set; }

        /// <summary>
        /// The target function Export
        /// </summary>
        public void* Function { get; private set; }

        /// <summary>
        /// The address of the bypass hook function
        /// </summary>
        public void* BypassFunction { get; private set; }

        public byte[] BypassBuffer;
        public byte[] RealBuffer;
        public byte[] HookBuffer;

        public void Compile()
        {
            var hModule = GetLibrary(Library);

            if (hModule == null)
                throw new DllNotFoundException(Library);

            if (Export != null)
                Function = GetProcAddress(hModule, Export);
            else
                Function = GetProcAddress(hModule, Ordinal);

            Log.Debug("Hook \"{0}->{1}\" Compiled.", new string[] { Library, Export ?? Ordinal.ToString() });

            AssemblyHook();
        }

        public void Compile(void* Function)
        {
            this.Function = Function;
            AssemblyHook();
        }

#if x64
        const int JmpSize = 12;
        private void AssemblyHook()
        {
            //Copy Minimal Instructions Amount
            var Reader = new MemoryCodeReader(this.Function, 100);
            var Decoder = Iced.Intel.Decoder.Create(64, Reader);

            Decoder.IP = (ulong)Function;
            var Instructions = Decoder.DecodeMany(JmpSize);

            var RetAddr = Decoder.IP;



            //Allocate Memory
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(64, Writer);

            int HookSize = Instructions.GetEncodedSize(64);

            RealBuffer = new byte[HookSize];
            BypassBuffer = new byte[HookSize + JmpSize];

            DeprotectMemory(Function, (uint)BypassBuffer.LongLength);
            Marshal.Copy(new IntPtr(Function), RealBuffer, 0, RealBuffer.Length);



            //Assemble Bypass
            var phBuffer = BypassBuffer.AllocUnsafe();
            BypassFunction = phBuffer;

            Instructions.Add(Instruction.Create(Code.Mov_r64_imm64, Register.RAX, RetAddr));
            Instructions.Add(Instruction.Create(Code.Jmp_rm64, Register.RAX));

            Compiler.Encode(Instructions, (ulong)phBuffer);

            Writer.CopyTo(phBuffer, 0);



            //Assemble the Hook
            Writer = new MemoryCodeWriter();
            Compiler = Encoder.Create(64, Writer);

            Instructions.Clear();
            Instructions.Add(Instruction.Create(Code.Mov_r64_imm64, Register.RAX, (ulong)HookFunction));
            Instructions.Add(Instruction.Create(Code.Jmp_rm64, Register.RAX));
            Compiler.Encode(Instructions, (ulong)Function);

            HookBuffer = Writer.ToArray();
        }
#else
        const int JmpSize = 5;
        private void AssemblyHook()
        {
            //Copy Minimal Instructions Amount
            var Instructions = new InstructionList();
            var Reader = new MemoryCodeReader(this.Function, 100);
            var Decoder = Iced.Intel.Decoder.Create(32, Reader);
            Decoder.IP = (ulong)Function;
            Instructions.AddRange(Decoder.DecodeMany(JmpSize));
            var RetAddr = Decoder.IP;



            //Allocate Memory
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(32, Writer);

            int HookSize = Instructions.GetEncodedSize(32);

            RealBuffer = new byte[HookSize];
            BypassBuffer = new byte[HookSize + JmpSize];

            DeprotectMemory(Function, (uint)BypassBuffer.LongLength);
            Marshal.Copy(new IntPtr(Function), RealBuffer, 0, RealBuffer.Length);


            //Assemble Bypass
            var phBuffer = BypassBuffer.AllocUnsafe();
            BypassFunction = phBuffer;

            Instructions.Add(Instruction.CreateBranch(Code.Jmp_rel32_32, RetAddr));
            Compiler.Encode(Instructions, (ulong)phBuffer);

            Writer.CopyTo(phBuffer, 0);


            //Assemble the Hook
            Writer = new MemoryCodeWriter();
            Compiler = Encoder.Create(32, Writer);
            var jmp = Instruction.CreateBranch(Code.Jmp_rel32_32, (ulong)HookFunction);
            Compiler.Encode(jmp, (ulong)Function);

            HookBuffer = Writer.ToArray();
        }

#endif

        public void Install()
        {
            DeprotectMemory(Function, (uint)HookBuffer.LongLength);
            Marshal.Copy(HookBuffer, 0, new IntPtr(Function), HookBuffer.Length);
        }

        public void Uninstall()
        {
            DeprotectMemory(Function, (uint)RealBuffer.LongLength);
            Marshal.Copy(RealBuffer, 0, new IntPtr(Function), RealBuffer.Length);
        }

        public abstract void Initialize();
    }

    public static partial class Extensions
    {
        public static int GetEncodedSize(this InstructionList List, int bitness)
        {
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(bitness, Writer);
            foreach (var Instruction in List)
            {
                Compiler.Encode(Instruction, (ulong)Writer.Count);
            }

            return Writer.Count;
        }
        public static int GetEncodedSize(this Instruction Instruction, int bitness)
        {
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(bitness, Writer);
            return (int)Compiler.Encode(Instruction, (ulong)Writer.Count);
        }

        public static uint Encode(this Encoder Encoder, InstructionList List, ulong IP)
        {
            uint Len = 0;
            foreach (var Instruction in List)
                Len += Encoder.Encode(Instruction, IP + Len);
            return Len;
        }

        public static InstructionList DecodeMany(this Decoder Decoder, uint MinLength)
        {
            var List = new InstructionList();
            var Begin = Decoder.IP;
            while (Decoder.IP < Begin + MinLength)
            {
                List.Add(Decoder.Decode());
            }
            return List;
        }

        public unsafe static byte* AllocUnsafe(uint Bytes)
        {
            return VirtualAlloc(null, Bytes, AllocationType.Reserve | AllocationType.Commit, MemoryProtection.ExecuteReadWrite);;
        }
        public unsafe static byte* AllocUnsafe(this IEnumerable<byte> Buffer) {
            var tmp = Buffer.ToArray();
            var Addr = VirtualAlloc(null, (uint)tmp.Length, AllocationType.Reserve | AllocationType.Commit, MemoryProtection.ExecuteReadWrite);
            for (int i = 0; i < tmp.Length; i++)
                *(Addr + i) = tmp[i];
            return Addr;
        }

        public unsafe static void DeprotectMemory(void* Buffer, uint Length, bool ExecutableOnly = false)
        {
            VirtualProtect(Buffer, Length, ExecutableOnly ? MemoryProtection.ExecuteRead : MemoryProtection.ExecuteReadWrite, out _);
        }
        public unsafe static void* GetLibrary(string Library) {
            var hModule = GetModuleHandle(Library);
            if (hModule != null)
                return hModule;
            return LoadLibrary(Library);
        }

        public unsafe static void* LoadLibrary(string Library) => LoadLibraryW(Library);
        public unsafe static void* GetModuleHandle(string Library) => GetModuleHandleW(Library);
        public unsafe static void* GetProcAddress(void* hModule, string ProcName) => GetProcAddressExt(hModule, ProcName);
        public unsafe static void* GetProcAddress(void* hModule, ushort ProcOrd) => GetProcAddressExt(hModule, ProcOrd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        unsafe static extern void* LoadLibraryW(string Library);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        unsafe static extern void* GetModuleHandleW(string Library);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        unsafe static extern void* GetProcAddressExt(void* hModule, string ProcName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        unsafe static extern void* GetProcAddressExt(void* hModule, ushort ProcOrdinal);

        [DllImport("kernel32.dll")]
        unsafe static extern bool VirtualProtect(void* lpAddress, uint dwSize, MemoryProtection flNewProtect, out AllocationType lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        unsafe static extern byte* VirtualAlloc(void* lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        public unsafe static extern bool IsBadCodePtr(void* Ptr);

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
    }
}