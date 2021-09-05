using System;
using Iced.Intel;
using System.Runtime.InteropServices;
using System.Linq;
using StringReloads.Engine;
using StringReloads.Engine.Unmanaged;
using static StringReloads.Hook.Base.Extensions;

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

        public bool Enabled { get; private set; } = false;
        public bool Persistent;

        public void Compile(bool ImportHook = false, IntPtr? TargetModule = null)
        {
            var hModule = GetLibrary(Library);

            if (hModule == null)
                throw new DllNotFoundException(Library);

            if (Export != null)
                Function = GetProcAddress(hModule, Export);
            else
                Function = GetProcAddress(hModule, Ordinal);

            if (ImportHook)
                SetupImportHook(TargetModule == null ? Config.GameBaseAddress : TargetModule.Value.ToPointer());
            else
                AssemblyHook();

            Log.Debug("Hook \"{0}->{1}\" {2}; Hook: 0x{3}; Bypass: 0x{4}", new string[] { Library, Export ?? Ordinal.ToString(), ImportHook ? "Ready" : "Compiled", ((ulong)HookFunction).ToString("X16"), ((ulong)BypassFunction).ToString("X16") });
        }

        public void Compile(void* Function)
        {
            this.Function = Function;
            AssemblyHook();

            Log.Debug($"Anonymous Hook (0x{(ulong)Function:X16}) Compiled; Hook: 0x{(ulong)HookFunction:X16}; Bypass: 0x{(ulong)BypassFunction:X16}");
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

            if (Instructions.IsDangerousToHook()) {
                Log.Warning($"Dangerous Hook {Library}->{Export ?? Ordinal.ToString()} Found; If Crash, try enable the ImportHook");
            }

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


            if (Instructions.IsDangerousToHook()) {
                Log.Warning($"Dangerous Hook {Library}->{Export ?? Ordinal.ToString()} Found; If Crash, try enable the ImportHook");
            }

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

        private void SetupImportHook(void* BaseAddress)
        {
            var Imports = ModuleInfo.GetModuleImports((byte*)BaseAddress);

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
            BitConverter.GetBytes((uint)BypassFunction).CopyTo(RealBuffer, 0);
#endif

            if (HookFunction == null)
                Log.Critical($"\"{Name}\" Null Hook Function");
            else
                Log.Debug($"Import to Hook Address: 0x{(ulong)Function:X16}");
        }

        public void Install()
        {
            Enabled = true;
            DeprotectMemory(Function, (uint)HookBuffer.LongLength);
            Marshal.Copy(HookBuffer, 0, new IntPtr(Function), HookBuffer.Length);
        }

        public void Uninstall()
        {
            if (!Enabled || Persistent)
                return;

            Enabled = false;
            DeprotectMemory(Function, (uint)RealBuffer.LongLength);
            Marshal.Copy(RealBuffer, 0, new IntPtr(Function), RealBuffer.Length);
        }

        public abstract void Initialize();
    }
}