using System;
using Iced.Intel;
using System.Runtime.InteropServices;
using static StringReloads.Hook.Base.Extensions;
using System.Linq;
using System.Collections.Generic;
using StringReloads.Engine;
using StringReloads.Engine.Unmanaged;

namespace StringReloads.Hook.Base
{
    public abstract unsafe class Hook<T> : Hook where T : Delegate
    {
        public Hook() : base() { }
        public Hook(void* Function) : base(Function) { }

        private UnsafeDelegate HookInstance;
        private UnsafeDelegate<T> BypassInstance;

        public T Bypass
        {
            get
            {
                if (BypassInstance == null)
                    BypassInstance = BypassFunction;

                return BypassInstance;
            }
        }

        public virtual T HookDelegate
        {
            set
            {
                HookInstance = value;
                HookFunction = HookInstance;
            }
        }
    }

    public abstract unsafe class Hook
    {
        public Hook() { Initialize(); }

        public Hook(void* Function)
        {
            this.Function = Function;
            Initialize();
        }

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

        public void Compile(bool ImportHook = false)
        {
            var hModule = GetLibrary(Library);

            if (hModule == null)
                throw new DllNotFoundException(Library);

            if (Export != null)
                Function = GetProcAddress(hModule, Export);
            else
                Function = GetProcAddress(hModule, Ordinal);

            if (ImportHook)
                SetupImportHook();
            else
                AssemblyHook();

            if (Config.Default.LogLevel == Log.LogLevel.Trace)
                Log.Trace("Hook \"{0}->{1}\" {2}; Hook: 0x{3}; Bypass: 0x{4}", new string[] { Library, Export ?? Ordinal.ToString(), ImportHook ? "Ready" : "Compiled", ((ulong)HookFunction).ToString("X16"), ((ulong)BypassFunction).ToString("X16") });
            else
                Log.Debug("Hook \"{0}->{1}\" {2}", new string[] { Library, Export ?? Ordinal.ToString(), ImportHook ? "Ready" : "Compiled" });
        }

        public void Compile(void* Function)
        {
            this.Function = Function;
            AssemblyHook();

            if (Config.Default.LogLevel == Log.LogLevel.Trace)
                Log.Trace($"Anonymous Hook (0x{(ulong)Function:X16}) Compiled; Hook: 0x{(ulong)HookFunction:X16}; Bypass: 0x{(ulong)BypassFunction:X16}");
            else
                Log.Debug($"Anonymous Hook (0x{(ulong)Function:X16}) Compiled");
        }

#if x64
        uint? _JmpSize = null;
        public uint JmpSize => (_JmpSize ?? (_JmpSize = (uint)ulong.MaxValue.AssemblyJmp().GetEncodedSize(64))).Value;
        private void AssemblyHook()
        {
            //Copy Minimal Instructions Amount
            var Reader = new MemoryCodeReader(this.Function, 100);
            var Decoder = Iced.Intel.Decoder.Create(64, Reader);
            Decoder.IP = (ulong)Function;

            var Instructions = Decoder.DecodeMany(JmpSize);

            var RetAddr = Decoder.IP;

            //If the next instruction is a conditional jmp,
            //Will be more safe if we copy it to the bypass as well
            var NextInstruction = Decoder.PeekDecode();
            if (NextInstruction.IsCondJmp())
                Instructions.Add(NextInstruction);


            //Allocate Memory
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(64, Writer);

            int RHookSize = Instructions.GetEncodedSize(64, (ulong)Function);
            int BypassSize = Instructions.GetAutoEncodedSize(64, (ulong)Function);

            RealBuffer = new byte[RHookSize];
            BypassBuffer = new byte[BypassSize];

            DeprotectMemory(Function, (uint)BypassBuffer.LongLength);
            Marshal.Copy(new IntPtr(Function), RealBuffer, 0, RealBuffer.Length);


            //Assemble Bypass
            var phBuffer = BypassBuffer.AllocUnsafe();
            BypassFunction = phBuffer;

            Instructions.AddRange(RetAddr.AssemblyJmp());

            Compiler.AutoEncode(Instructions, (ulong)phBuffer);

            Writer.CopyTo(phBuffer, 0);

            //Assemble the Hook
            Writer = new MemoryCodeWriter();
            Compiler = Encoder.Create(64, Writer);

            Instructions = ((ulong)HookFunction).AssemblyJmp();

            Compiler.Encode(Instructions, (ulong)Function);

            HookBuffer = Writer.ToArray();
        }
#else
        public const int JmpSize = 5;
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

        private void SetupImportHook()
        {
            var Imports = ModuleInfo.GetModuleImports((byte*)Config.Default.GameBaseAddress);

            var Import = (from x in Imports where x.Function == Export && x.Module.ToLower() == Library.ToLower() select x).Single();

            BypassFunction = Function;
            Function = Import.ImportAddress;


#if x64
            HookBuffer = new byte[8];
            BitConverter.GetBytes((ulong)HookFunction).CopyTo(HookBuffer, 0);
            RealBuffer = new byte[8];
            BitConverter.GetBytes((ulong)BypassFunction).CopyTo(RealBuffer, 0);
#else
            HookBuffer = new byte[4];
            BitConverter.GetBytes((uint)HookFunction).CopyTo(HookBuffer, 0);
            RealBuffer = new byte[4];
            BitConverter.GetBytes((uint)BypassFunction).CopyTo(HookBuffer, 0);
#endif

            if (HookFunction == null)
                Log.Critical($"\"{Name}\" Null Hook Function");
        }

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
        public static int GetEncodedSize(this InstructionList List, int bitness, ulong IP = 0)
        {
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(bitness, Writer);
            foreach (var Instruction in List)
            {
                Compiler.Encode(Instruction, (ulong)Writer.Count + IP);
            }

            return Writer.Count;
        }
        public static int GetEncodedSize(this Instruction Instruction, int bitness, ulong IP = 0)
        {
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(bitness, Writer);
            return (int)Compiler.Encode(Instruction, (ulong)Writer.Count + IP);
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
        public static InstructionList DecodeAmount(this Decoder Decoder, uint Count)
        {
            var List = new InstructionList();
            while (List.Count < Count)
            {
                List.Add(Decoder.Decode());
            }
            return List;
        }
        public unsafe static Instruction PeekDecode(this Decoder RefDecoder)
        {
            var NewMem = new MemoryCodeReader((void*)RefDecoder.IP);
            var NewDecoder = Decoder.Create(RefDecoder.Bitness, NewMem);
            NewDecoder.IP = RefDecoder.IP;
            return NewDecoder.Decode();
        }
        public unsafe static InstructionList AssembleToList(this Assembler Assembler, ulong RIP)
        {
            var CWBuffer = new MemoryCodeWriter();
            Assembler.Assemble(CWBuffer, RIP);
            var Buffer = CWBuffer.ToArray();
            fixed (void* pBuffer = &Buffer[0])
            {
                using (var Reader = new MemoryCodeReader(pBuffer, (uint)Buffer.Length))
                {
                    var AsmDecoder = Decoder.Create(Assembler.Bitness, Reader);
                    return AsmDecoder.DecodeAmount((uint)Assembler.Instructions.Count);
                }
            }
        }

#if x64
        public static int GetAutoEncodedSize(this InstructionList List, int bitness, ulong IP = 0)
        {
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(bitness, Writer);
            return (int)Compiler.AutoEncode(List, IP);
        }
        public static int GetAutoEncodedSize(this Instruction Instruction, int bitness, ulong IP = 0)
        {
            var Writer = new MemoryCodeWriter();
            var Compiler = Encoder.Create(bitness, Writer);
            return Compiler.AutoEncode(Instruction, (ulong)Writer.Count + IP).GetEncodedSize(64);
        }
        public static uint AutoEncode(this Encoder Encoder, InstructionList List, ulong IP)
        {
            uint Len = 0;
            InstructionList NewList = new InstructionList();
            foreach (var Instruction in List)
            {
                var NewInstruction = Encoder.AutoEncode(Instruction, IP + Len);
                NewList.AddRange(NewInstruction);
                Len += (uint)NewInstruction.GetEncodedSize(64, IP + Len);
            }

            Len = 0;
            foreach (var Instruction in NewList)
                Len += Encoder.Encode(Instruction, IP + Len);

            return Len;
        }
        public static InstructionList AutoEncode(this Encoder Encoder, Instruction Instruction, ulong IP)
        {
            InstructionList List = new InstructionList();
            if (Encoder.Bitness <= 32)
            {
                List.Add(Instruction);
            }
            else
            {
                if (Instruction.IsJmp() && !Instruction.IsCallFar && Instruction.Op0Kind != OpKind.Register)
                {

                    if (!Instruction.IsANotJmp())
                        Instruction.NegateConditionCode();

                    var Jmp = Instruction.Immediate64.AssemblyJmp();

                    //Get Far Jmp Size + Short Conditional Jmp Size
                    var JmpSize = (uint)Jmp.GetAutoEncodedSize(64, IP);
                    JmpSize += (uint)Instruction.ConditionCode.ToShortJmp(IP + JmpSize, 64).GetEncodedSize(64, IP);

                    List.Add(Instruction.ConditionCode.ToShortJmp(IP + JmpSize, 64));
                    List.AddRange(Jmp);
                }
                else
                {
                    switch (Instruction.Code)
                    {
                        case Code.Mov_r64_rm64:
                            if (Instruction.Op1Kind != OpKind.Memory || Instruction.MemoryBase != Register.RIP)
                                goto default;
                            List.Add(Instruction.Create(Code.Mov_r64_imm64, Instruction.Op0Register, Instruction.IPRelativeMemoryAddress));
                            List.Add(Instruction.Create(Code.Mov_r64_rm64, Instruction.Op0Register, new MemoryOperand(Instruction.Op0Register)));
                            break;
                        default:
                            List.Add(Instruction);
                            break;
                    }
                }
            }
            return List;
        }

        public static bool IsCondJmp(this Instruction Instruction) => Instruction.ConditionCode switch
        {
            ConditionCode.None => false,
            _ => true
        };
        public static bool IsANotJmp(this Instruction Instruction) => Instruction.ConditionCode switch
        {
            ConditionCode.ne => true,
            ConditionCode.no => true,
            ConditionCode.np => true,
            ConditionCode.ns => true,
            _ => false
        };
        public static Instruction ToShortJmp(this ConditionCode Condition, ulong Address, byte bitness)
        {
            bool x64 = bitness == 64;
            return (Condition, bitness) switch
            {
                //x64
                (ConditionCode.a, 64) => Instruction.CreateBranch(Code.Ja_rel32_64, Address),
                (ConditionCode.ae, 64) => Instruction.CreateBranch(Code.Jae_rel32_64, Address),
                (ConditionCode.b, 64) => Instruction.CreateBranch(Code.Jb_rel32_64, Address),
                (ConditionCode.be, 64) => Instruction.CreateBranch(Code.Jbe_rel32_64, Address),
                (ConditionCode.e, 64) => Instruction.CreateBranch(Code.Je_rel32_64, Address),
                (ConditionCode.g, 64) => Instruction.CreateBranch(Code.Jg_rel32_64, Address),
                (ConditionCode.ge, 64) => Instruction.CreateBranch(Code.Jge_rel32_64, Address),
                (ConditionCode.l, 64) => Instruction.CreateBranch(Code.Jl_rel32_64, Address),
                (ConditionCode.ne, 64) => Instruction.CreateBranch(Code.Jne_rel32_64, Address),
                (ConditionCode.no, 64) => Instruction.CreateBranch(Code.Jno_rel32_64, Address),
                (ConditionCode.np, 64) => Instruction.CreateBranch(Code.Jnp_rel32_64, Address),
                (ConditionCode.ns, 64) => Instruction.CreateBranch(Code.Jns_rel32_64, Address),
                (ConditionCode.o, 64) => Instruction.CreateBranch(Code.Jo_rel32_64, Address),
                (ConditionCode.p, 64) => Instruction.CreateBranch(Code.Jp_rel32_64, Address),
                (ConditionCode.s, 64) => Instruction.CreateBranch(Code.Js_rel32_64, Address),

                //x32                
                (ConditionCode.a, 32) => Instruction.CreateBranch(Code.Ja_rel32_32, Address),
                (ConditionCode.ae, 32) => Instruction.CreateBranch(Code.Jae_rel32_32, Address),
                (ConditionCode.b, 32) => Instruction.CreateBranch(Code.Jb_rel32_32, Address),
                (ConditionCode.be, 32) => Instruction.CreateBranch(Code.Jbe_rel32_32, Address),
                (ConditionCode.e, 32) => Instruction.CreateBranch(Code.Je_rel32_32, Address),
                (ConditionCode.g, 32) => Instruction.CreateBranch(Code.Jg_rel32_32, Address),
                (ConditionCode.ge, 32) => Instruction.CreateBranch(Code.Jge_rel32_32, Address),
                (ConditionCode.l, 32) => Instruction.CreateBranch(Code.Jl_rel32_32, Address),
                (ConditionCode.ne, 32) => Instruction.CreateBranch(Code.Jne_rel32_32, Address),
                (ConditionCode.no, 32) => Instruction.CreateBranch(Code.Jno_rel32_32, Address),
                (ConditionCode.np, 32) => Instruction.CreateBranch(Code.Jnp_rel32_32, Address),
                (ConditionCode.ns, 32) => Instruction.CreateBranch(Code.Jns_rel32_32, Address),
                (ConditionCode.o, 32) => Instruction.CreateBranch(Code.Jo_rel32_32, Address),
                (ConditionCode.p, 32) => Instruction.CreateBranch(Code.Jp_rel32_32, Address),
                (ConditionCode.s, 32) => Instruction.CreateBranch(Code.Js_rel32_32, Address),

                //x16
                (ConditionCode.a, 16) => Instruction.CreateBranch(Code.Jae_rel16, Address),
                (ConditionCode.ae, 16) => Instruction.CreateBranch(Code.Jae_rel16, Address),
                (ConditionCode.b, 16) => Instruction.CreateBranch(Code.Jb_rel16, Address),
                (ConditionCode.be, 16) => Instruction.CreateBranch(Code.Jbe_rel16, Address),
                (ConditionCode.e, 16) => Instruction.CreateBranch(Code.Je_rel16, Address),
                (ConditionCode.g, 16) => Instruction.CreateBranch(Code.Jg_rel16, Address),
                (ConditionCode.ge, 16) => Instruction.CreateBranch(Code.Jge_rel16, Address),
                (ConditionCode.l, 16) => Instruction.CreateBranch(Code.Jl_rel16, Address),
                (ConditionCode.ne, 16) => Instruction.CreateBranch(Code.Jne_rel16, Address),
                (ConditionCode.no, 16) => Instruction.CreateBranch(Code.Jno_rel16, Address),
                (ConditionCode.np, 16) => Instruction.CreateBranch(Code.Jnp_rel16, Address),
                (ConditionCode.ns, 16) => Instruction.CreateBranch(Code.Jns_rel16, Address),
                (ConditionCode.o, 16) => Instruction.CreateBranch(Code.Jo_rel16, Address),
                (ConditionCode.p, 16) => Instruction.CreateBranch(Code.Jp_rel16, Address),
                (ConditionCode.s, 16) => Instruction.CreateBranch(Code.Js_rel16, Address),

                //Uncoditionall
                (ConditionCode.None, 64) => Instruction.CreateBranch(Code.Jmp_rel32_64, Address),
                (ConditionCode.None, 32) => Instruction.CreateBranch(Code.Jmp_rel32_32, Address),
                (ConditionCode.None, 16) => Instruction.CreateBranch(Code.Jmp_rel16, Address),

                _ => throw new InvalidOperationException()
            };
        }

        public static bool IsJmp(this Instruction Instruction) =>
            Instruction.IsJmpNear || Instruction.IsJmpNearIndirect ||
            Instruction.IsJmpShort || Instruction.IsJmpFar ||
            Instruction.IsJmpFarIndirect || Instruction.IsJmpShortOrNear ||
            Instruction.IsJccNear || Instruction.IsJccShort ||
            Instruction.IsJccShortOrNear;

        public static bool IsRet(this Instruction Instruction) => Instruction.Code switch
        {
            Code.Retfd => true,
            Code.Retfd_imm16 => true,
            Code.Retfq => true,
            Code.Retfq_imm16 => true,
            Code.Retfw => true,
            Code.Retfw_imm16 => true,
            Code.Retnd => true,
            Code.Retnd_imm16 => true,
            Code.Retnq => true,
            Code.Retnq_imm16 => true,
            Code.Retnw => true,
            Code.Retnw_imm16 => true,
            _ => false
        };
        public static InstructionList AssemblyJmp(this ulong Target)
        {
            var Instructions = new InstructionList();
            Instructions.Add(Instruction.Create(Code.Pushq_imm32, unchecked((int)(Target & uint.MaxValue))));
            Instructions.Add(Instruction.Create(Code.Mov_rm32_imm32, new MemoryOperand(Register.RSP, 4), (uint)(Target >> 8 * 4)));
            Instructions.Add(Instruction.Create(Code.Retnq));
            return Instructions;
        }
#endif

        public unsafe static byte* AllocUnsafe(uint Bytes)
        {
            return VirtualAlloc(null, Bytes, AllocationType.Reserve | AllocationType.Commit, MemoryProtection.ExecuteReadWrite); ;
        }
        public unsafe static byte* AllocUnsafe(this IEnumerable<byte> Buffer)
        {
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
        public unsafe static void* GetLibrary(string Library)
        {
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