//Original Repo of this class: https://github.com/marcussacana/RemoteControl
using System;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using static SRL.UnmanagedImports;
using System.Collections.Generic;
using System.Diagnostics;

namespace SRL
{
    public struct ImportEntry
    {

        /// <summary>
        /// The Imported Module Name
        /// </summary>
        public string Module;

        /// <summary>
        /// The Imported Function Name
        /// </summary>
        public string Function;

        /// <summary>
        /// The Import Ordinal Hint
        /// </summary>
        public ushort Ordinal;

        /// <summary>
        /// The Address of this Import in the IAT (Import Address Table)
        /// </summary>
        public IntPtr ImportAddress;

        /// <summary>
        /// The Address of the Imported Function
        /// </summary>
        public IntPtr FunctionAddress;
    }

    public class UnmanagedHook : UnmanagedHook<Delegate>
    {

        /// <summary>
        /// Create a new hook to the function in given pointer
        /// </summary>
        /// <param name="Original">The pointer of the unmanaged function to hook</param>
        /// <param name="Hook">The delegate with the hook function</param>
        public UnmanagedHook(IntPtr Original, Delegate Hook) : base(Original, Hook, false) { }

        /// Create a new Hook
        /// </summary>
        /// <param name="Library">The Library name, Ex: Kernel32.dll</param>
        /// <param name="Function">The Function name, Ex: CreateFile</param>
        /// <param name="Hook">The delegate wit the hook function</param>
        public UnmanagedHook(string Library, string Function, Delegate Hook) : base(Library, Function, Hook, false) { }

        /// Create a new Hook
        /// </summary>
        /// <param name="Import">The Import Entry to install the hook</param>
        /// <param name="Hook">The delegate wit the hook function</param>

        public UnmanagedHook(ImportEntry Import, Delegate Hook) : base(Import, Hook) { }

        /// <summary>
        /// Enumarete all imports of a specified process main module
        /// </summary>
        /// <param name="Process">The process that have loaded the module</param>
        /// <returns>All Import Entries</returns>
        public static ImportEntry[] GetImports() => GetModuleImports(Process.GetCurrentProcess().MainModule.BaseAddress);

        /// <summary>
        /// Get the Module Export Ordinal by the function name
        /// </summary>
        /// <param name="Module">The module name</param>
        /// <param name="Function">The Exported Function Name</param>
        /// <returns>The Ordinal</returns>
        public static ushort GetExportOrdinal(string Module, string Function) => SearchFunctionOridinal(Module, Function);

        /// <summary>
        /// Get the Module Export Ordinal by the function name
        /// </summary>
        /// <param name="Module">The module handler</param>
        /// <param name="Function">The Exported Function Name</param>
        /// <returns>The Ordinal</returns>
        public static ushort GetExportOrdinal(IntPtr Module, string Function) => SearchFunctionOridinal(Module, Function);

        /// <summary>
        /// Create a hook by Import if possible
        /// </summary>
        /// <param name="Library"></param>
        /// <param name="Function"></param>
        /// <param name="Hook"></param>
        /// <returns></returns>
        public static UnmanagedHook TryHookImport(string Library, string Function, Delegate Hook)
        {
            var Import = GetImport(Library, Function);
            if (Import.HasValue)
                return new UnmanagedHook(Import.Value, Hook);
            else
                return new UnmanagedHook(Library, Function, Hook);
        }

        public static ImportEntry? GetImport(string Module, string Function)
        {
            try
            {
                return (from x in GetImports() where (x.Module.ToLower() == Module.ToLower() && Function == x.Function) select x).Single();
            }
            catch
            {
                return null;
            }
        }

        public static ImportEntry? GetImport(string Module, ushort Ordinal)
        {
            try
            {
                return (from x in GetImports() where (x.Module.ToLower() == Module.ToLower() && Ordinal == x.Ordinal) select x).Single();
            }
            catch
            {
                return null;
            }
        }

        public bool ImportHook => base.ImportHook;
    }

    public class UnmanagedHook<T> : IDisposable, Hook where T : Delegate
    {

        static int nBytes = IntPtr.Size == 8 ? 12 : 5;

        IntPtr destination;
        IntPtr addr;
        Protection old;
        byte[] src = new byte[nBytes];
        byte[] dst = new byte[nBytes];
        bool AutoHook = false;
        public bool ImportHook { get; private set; } = false;

        public IntPtr HookFunctionAddress => destination;

        List<dynamic> Followers = new List<dynamic>();

        T RealDestination;
        T HookDestination;


        /// <summary>
        /// Create a new hook to the function in given pointer
        /// </summary>
        /// <param name="Original">The pointer of the unmanaged function to hook</param>
        /// <param name="Hook">The delegate with the hook function</param>
        /// <param name="AutoHook">When true the hook will be automatically uninstalled during he execution</param>
        public UnmanagedHook(IntPtr Original, T Hook, bool AutoHook)
        {
            this.AutoHook = AutoHook;
            RealDestination = Hook;

            if (AutoHook)
                GenerateMethod();

            destination = Marshal.GetFunctionPointerForDelegate(AutoHook ? HookDestination : RealDestination);

            VirtualProtect(Original, nBytes, Protection.PAGE_EXECUTE_READWRITE, out old);
            Marshal.Copy(Original, src, 0, nBytes);
            dst = AssembleJump(Original, destination);
            addr = Original;
        }

        /// <summary>
        /// Install a hook exclusive to a certain module (Using the IAT Method)
        /// </summary>
        /// <param name="Import">The Module Import To Hook</param>
        /// <param name="Hook">The Delegate with the hook function</param>
        public UnmanagedHook(ImportEntry Import, T Hook)
        {
            ImportHook = true;

            addr = Import.ImportAddress;

            src = new byte[IntPtr.Size];
            VirtualProtect(Import.ImportAddress, IntPtr.Size, Protection.PAGE_READWRITE, out old);
            Marshal.Copy(Import.ImportAddress, src, 0, IntPtr.Size);

            destination = Marshal.GetFunctionPointerForDelegate(Hook);
            dst = BitConverter.GetBytes(IntPtr.Size == 8 ? destination.ToUInt64() : destination.ToUInt32());
        }


        /// <summary>
        /// Create a new hook to the function in given pointer
        /// </summary>
        /// <param name="Original">The pointer of the unmanaged function to hook</param>
        /// <param name="Hook">The delegate with the hook function</param>
        public UnmanagedHook(IntPtr Original, T Hook) : this(Original, Hook, true) { }

        /// Create a new Hook
        /// </summary>
        /// <param name="Library">The Library name, Ex: Kernel32.dll</param>
        /// <param name="Function">The Function name, Ex: CreateFile</param>
        /// <param name="Hook">The delegate wit the hook function</param>
        public UnmanagedHook(string Library, string Function, T Hook) : this(GetProcAddress(LoadLibrary(Library), Function), Hook)
        {
        }


        /// Create a new Hook
        /// </summary>
        /// <param name="Library">The Library name, Ex: Kernel32.dll</param>
        /// <param name="Function">The Function name, Ex: CreateFile</param>
        /// <param name="Hook">The delegate wit the hook function</param>
        /// <param name="AutoHook">When true the hook will be automatically uninstalled during he execution</param>
        public UnmanagedHook(string Library, string Function, T Hook, bool AutoHook) : this(GetProcAddress(LoadLibrary(Library), Function), Hook, AutoHook)
        {

        }

        void GenerateMethod()
        {
            string ID = CurrentID++.ToString();
            SetInstance(ID, this);

            var ParametersInfo = RealDestination.Method.GetParameters();
            var Parameters = (from x in ParametersInfo select x.ParameterType).ToArray();

            List<Instruction> IL = new List<Instruction>();
            //Uninstall(IID);
            IL.Add(new Instruction(OpCodes.Ldstr, ID));
            IL.Add(new Instruction(OpCodes.Call, UninstallMI));

            //Invoke(Parameters);
            switch (Parameters.Length + 1)
            {
                case 1:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_1));
                    break;
                case 2:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_2));
                    break;
                case 3:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_3));
                    break;
                case 4:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_4));
                    break;
                case 5:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_5));
                    break;
                case 6:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_6));
                    break;
                case 7:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_7));
                    break;
                case 8:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_8));
                    break;
                default:
                    int Count = Parameters.Length + 1;
                    if (Count <= byte.MaxValue)
                        IL.Add(new Instruction(OpCodes.Ldc_I4_S, (byte)Count));
                    else
                        IL.Add(new Instruction(OpCodes.Ldc_I4, Count));
                    break;
            }
            IL.Add(new Instruction(OpCodes.Newarr, typeof(object)));

            for (int i = 0, ind = -1; i < Parameters.Length + 1; i++, ind++)
            {
                bool IsOut = ind >= 0 && ParametersInfo[ind].IsOut;
                bool IsRef = ind >= 0 && ParametersInfo[ind].ParameterType.IsByRef & !IsOut;
                if (IsOut)
                    continue;

                IL.Add(new Instruction(OpCodes.Dup));
                switch (i)
                {
                    case 0:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_0));
                        break;
                    case 1:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_1));
                        break;
                    case 2:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_2));
                        break;
                    case 3:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_3));
                        break;
                    case 4:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_4));
                        break;
                    case 5:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_5));
                        break;
                    case 6:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_6));
                        break;
                    case 7:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_7));
                        break;
                    case 8:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_8));
                        break;
                    default:
                        if (i <= byte.MaxValue)
                            IL.Add(new Instruction(OpCodes.Ldc_I4_S, (byte)i));
                        else
                            IL.Add(new Instruction(OpCodes.Ldc_I4, i));
                        break;
                }


                if (ind >= 0 && (ParametersInfo[ind].IsIn || ParametersInfo[ind].IsOut))
                {
                    if (ind <= byte.MaxValue)
                        IL.Add(new Instruction(OpCodes.Ldarga_S, (byte)ind));
                    else
                        IL.Add(new Instruction(OpCodes.Ldarga, (short)ind));
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            IL.Add(new Instruction(OpCodes.Ldstr, ID));
                            break;
                        case 1:
                            IL.Add(new Instruction(OpCodes.Ldarg_0));
                            break;
                        case 2:
                            IL.Add(new Instruction(OpCodes.Ldarg_1));
                            break;
                        case 3:
                            IL.Add(new Instruction(OpCodes.Ldarg_2));
                            break;
                        case 4:
                            IL.Add(new Instruction(OpCodes.Ldarg_3));
                            break;
                        default:
                            if (ind <= byte.MaxValue)
                                IL.Add(new Instruction(OpCodes.Ldarg_S, (byte)ind));
                            else
                                IL.Add(new Instruction(OpCodes.Ldarg, (short)ind));
                            break;
                    }
                }

                var Type = ind >= 0 ? Parameters[ind] : null;
                if (Type != null && IsRef)
                    Type = Parameters[ind].GetElementType();

                bool Cast = true;
                if (IsRef)
                {
                    var PType = Type;
                    if (PType.IsEnum && PType.IsValueType)
                        PType = Enum.GetUnderlyingType(PType);

                    switch (PType.FullName)
                    {
                        case "System.SByte":
                            IL.Add(new Instruction(OpCodes.Ldind_I1));
                            break;
                        case "System.Byte":
                            IL.Add(new Instruction(OpCodes.Ldind_U1));
                            break;
                        case "System.Int16":
                            IL.Add(new Instruction(OpCodes.Ldind_I2));
                            break;
                        case "System.Char":
                        case "System.UInt16":
                            IL.Add(new Instruction(OpCodes.Ldind_U2));
                            break;
                        case "System.Int32":
                            IL.Add(new Instruction(OpCodes.Ldind_I4));
                            break;
                        case "System.UInt32":
                            IL.Add(new Instruction(OpCodes.Ldind_U4));
                            break;
                        case "System.Int64":
                        case "System.UInt64":
                            IL.Add(new Instruction(OpCodes.Ldind_I8));
                            break;
                        case "System.Single":
                            IL.Add(new Instruction(OpCodes.Ldind_R4));
                            break;
                        case "System.Double":
                            IL.Add(new Instruction(OpCodes.Ldind_R8));
                            break;
                        case "System.IntPtr":
                        case "System.UIntPtr":
                            IL.Add(new Instruction(OpCodes.Ldind_I));
                            break;
                        default:
                            if ((PType.IsValueType && !PType.IsEnum) || PType.IsClass)
                            {
                                IL.Add(new Instruction(OpCodes.Ldobj, PType));
                            }
                            else
                            {
                                IL.Add(new Instruction(OpCodes.Ldind_Ref));
                                Cast = false;
                            }
                            break;
                    }
                }

                if (ind >= 0 && Cast)
                    IL.Add(new Instruction(OpCodes.Box, Type));

                IL.Add(new Instruction(OpCodes.Stelem_Ref));
            }

            IL.Add(new Instruction(OpCodes.Stloc_0));
            IL.Add(new Instruction(OpCodes.Ldloc_0));
            IL.Add(new Instruction(OpCodes.Call, InvokeMI));

            //Cast Return Type
            var RetType = RealDestination.Method.ReturnType;
            if (RetType.IsInterface)
                IL.Add(new Instruction(OpCodes.Castclass, RetType));
            else
            {
                IL.Add(new Instruction(OpCodes.Unbox_Any, RetType));
            }

            for (int i = 0, ind = -1; i < Parameters.Length + 1; i++, ind++)
            {
                bool IsOut = ind >= 0 && ParametersInfo[ind].IsOut;
                bool IsRef = ind >= 0 && ParametersInfo[ind].ParameterType.IsByRef & !IsOut;

                if (!IsRef && !IsOut || ind < 0)
                    continue;


                switch (ind)
                {
                    case 0:
                        IL.Add(new Instruction(OpCodes.Ldarg_0));
                        break;
                    case 1:
                        IL.Add(new Instruction(OpCodes.Ldarg_1));
                        break;
                    case 2:
                        IL.Add(new Instruction(OpCodes.Ldarg_2));
                        break;
                    case 3:
                        IL.Add(new Instruction(OpCodes.Ldarg_3));
                        break;
                    default:

                        if (ind <= byte.MaxValue)
                            IL.Add(new Instruction(OpCodes.Ldarg_S, (byte)ind));
                        else
                            IL.Add(new Instruction(OpCodes.Ldarg, (short)ind));
                        break;
                }

                IL.Add(new Instruction(OpCodes.Ldloc_0));
                switch (i)
                {
                    case 0:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_0));
                        break;
                    case 1:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_1));
                        break;
                    case 2:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_2));
                        break;
                    case 3:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_3));
                        break;
                    case 4:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_4));
                        break;
                    case 5:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_5));
                        break;
                    case 6:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_6));
                        break;
                    case 7:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_7));
                        break;
                    case 8:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_8));
                        break;
                    default:
                        if (ind <= byte.MaxValue)
                            IL.Add(new Instruction(OpCodes.Ldc_I4_S, (byte)ind));
                        else
                            IL.Add(new Instruction(OpCodes.Ldc_I4, ind));
                        break;
                }

                IL.Add(new Instruction(OpCodes.Ldelem_Ref));

                var Type = Parameters[ind];
                if (IsRef || IsOut)
                    Type = Type.GetElementType();

                if (Type.IsInterface)
                {
                    IL.Add(new Instruction(OpCodes.Castclass, Type));
                    IL.Add(new Instruction(OpCodes.Stind_Ref));
                }
                else
                {
                    IL.Add(new Instruction(OpCodes.Unbox_Any, Type));

                    if (Type.IsEnum && Type.IsValueType)
                        Type = Enum.GetUnderlyingType(Type);

                    switch (Type.FullName)
                    {
                        case "System.Byte":
                        case "System.SByte":
                            IL.Add(new Instruction(OpCodes.Stind_I1));
                            break;
                        case "System.Char":
                        case "System.UInt16":
                        case "System.Int16":
                            IL.Add(new Instruction(OpCodes.Stind_I2));
                            break;
                        case "System.UInt32":
                        case "System.Int32":
                            IL.Add(new Instruction(OpCodes.Stind_I4));
                            break;
                        case "System.Int64":
                        case "System.UInt64":
                            IL.Add(new Instruction(OpCodes.Stind_I8));
                            break;
                        case "System.Single":
                            IL.Add(new Instruction(OpCodes.Stind_R4));
                            break;
                        case "System.Double":
                            IL.Add(new Instruction(OpCodes.Stind_R8));
                            break;
                        case "System.IntPtr":
                        case "System.UIntPtr":
                            IL.Add(new Instruction(OpCodes.Stind_I));
                            break;
                        default:
                            if (Type.IsValueType && !Type.IsEnum)
                            {
                                IL.Add(new Instruction(OpCodes.Stobj, Type));
                            }
                            else
                            {
                                IL.Add(new Instruction(OpCodes.Stind_Ref));
                            }
                            break;
                    }
                }

            }


            //Install(IID);
            IL.Add(new Instruction(OpCodes.Ldstr, ID));
            IL.Add(new Instruction(OpCodes.Call, InstallMI));

            //return;
            IL.Add(new Instruction(OpCodes.Ret));

#if ILDEBUG
            var NewMethod = GenerateAssembly(ID, IL, ParametersInfo);
            HookDestination = (T)Delegate.CreateDelegate(typeof(T), NewMethod);
#else
            DynamicMethod Method = new DynamicMethod("UnmanagedHook", RealDestination.Method.ReturnType, Parameters, typeof(UnmanagedImports), true);



            var ILGen = Method.GetILGenerator();
            ILGen.DeclareLocal(typeof(object[]));

            foreach (var Pair in IL)
            {
                if (Pair.Value == null)
                    ILGen.Emit(Pair.Key);
                else
                    ILGen.Emit(Pair.Key, Pair.Value);
            }

            HookDestination = (T)Method.CreateDelegate(typeof(T));
#endif

        }

        MethodInfo InstallMI => (from x in typeof(UnmanagedImports).GetMethods(BindingFlags.Static | BindingFlags.Public) where x.Name == "Install" select x).First();
        MethodInfo UninstallMI => (from x in typeof(UnmanagedImports).GetMethods(BindingFlags.Static | BindingFlags.Public) where x.Name == "Uninstall" select x).First();
        MethodInfo InvokeMI => (from x in typeof(UnmanagedImports).GetMethods(BindingFlags.Static | BindingFlags.Public) where x.Name == "Invoke" select x).First();

        /// <summary>
        /// Install the hook
        /// </summary>
        public void Install()
        {
            Marshal.Copy(dst, 0, addr, ImportHook ? IntPtr.Size : nBytes);
        }


        /// <summary>
        /// Uninstall the hook
        /// </summary>
        public void Uninstall()
        {
            Marshal.Copy(src, 0, addr, ImportHook ? IntPtr.Size : nBytes);
        }

        Delegate Hook.GetDelegate()
        {
            return RealDestination;
        }

        /// <summary>
        /// Adds a hook to be disabled during the execution of the this hook
        /// </summary>
        /// <param name="Follower">The hook to be disabled</param>
        public void AddFollower(params object[] Followers)
        {
            if (!AutoHook)
                throw new Exception("The Auto Hook must be enabled");

            foreach (object Follower in Followers)
            {
                if (!(Follower is Hook))
                    throw new Exception(Follower.GetType().Name + " Isn't an UnmanagedHook Class");

                this.Followers.Add(Follower);
            }
        }

        /// <summary>
        /// Remove a hook from the Follower list
        /// </summary>
        /// <param name="Follower">The hook to be removed</param>
        public void RemoveFollower(params object[] Followers)
        {
            if (!AutoHook)
                throw new Exception("The Auto Hook must be enabled");

            foreach (object Follower in Followers)
            {
                if (!(Follower is Hook))
                    throw new Exception(Follower.GetType().Name + " Isn't an UnmanagedHook Class");

                this.Followers.Remove(Follower);
            }
        }

        object[] Hook.GetFollowers()
        {
            return Followers.ToArray();
        }

        public void Dispose()
        {
            Uninstall();
            Protection x;
            VirtualProtect(addr, nBytes, old, out x);
        }

    }

    internal static class UnmanagedImports
    {

        [DebuggerDisplay("{Key}      {Value}")]
        internal struct Instruction
        {
            public dynamic Key;
            public dynamic Value;

            public Instruction(dynamic Key, dynamic Value)
            {
                this.Key = Key;
                this.Value = Value;
            }
            public Instruction(dynamic Key)
            {
                this.Key = Key;
                Value = null;
            }
        }

        internal interface Hook
        {
            void Install();
            void Uninstall();

            dynamic[] GetFollowers();

            Delegate GetDelegate();
        }


        static Dictionary<string, object> InstanceMap = new Dictionary<string, object>();

        internal static long CurrentID = 0;

        /// <summary>
        /// INTERNAL UNMANAGED HOOK USAGE, DON'T TOUCH ME
        /// </summary>
        public static void Install(string ID)
        {
            Hook Hook = (Hook)InstanceMap[ID];
            Hook.Install();

            foreach (object dFollower in Hook.GetFollowers())
            {
                Hook Follower = (Hook)dFollower;
                Follower.Install();
            }
        }

        /// <summary>
        /// INTERNAL UNMANAGED HOOK USAGE, DON'T TOUCH ME
        /// </summary>
        public static void Uninstall(string ID)
        {
            Hook Hook = (Hook)InstanceMap[ID];
            Hook.Uninstall();

            foreach (object dFollower in Hook.GetFollowers())
            {
                Hook Follower = (Hook)dFollower;
                Follower.Uninstall();
            }
        }

        /// <summary>
        /// INTERNAL UNMANAGED HOOK USAGE, DON'T TOUCH ME
        /// </summary>
        public static object Invoke(object[] Parameters)
        {
            if (Parameters.Length == 0)
                throw new Exception("No Arguments Found");

            string ID = (string)Parameters.First();
            object[] Args = Parameters.Skip(1).ToArray();

            Hook Hook = (Hook)InstanceMap[ID];

            object Result = Hook.GetDelegate().DynamicInvoke(Args);
            Args.CopyTo(Parameters, 1);
            return Result;
        }


        internal static void SetInstance(string ID, object Instance)
        {
            InstanceMap[ID] = Instance;
        }
        internal static ImportEntry[] GetModuleImports(IntPtr Module)
        {
            if (Module == IntPtr.Zero)
                throw new Exception("Failed to catch the Main Module...");

            uint PtrSize = Environment.Is64BitProcess ? 8u : 4u;

            ulong OrdinalFlag = (1ul << (int)((8 * PtrSize) - 1));

            ulong PEStart = Read(Module.Sum(0x3C), 4).ToUInt32();
            ulong OptionalHeader = PEStart + 0x18;

            ulong ImageDataDirectoryPtr = OptionalHeader + (PtrSize == 8 ? 0x70u : 0x60u);

            ulong ImportTableEntry = ImageDataDirectoryPtr + 0x8;

            IntPtr RVA = ImportTableEntry.ToIntPtr();

            IntPtr ImportDesc = Module.Sum(Read(Module.Sum(RVA), 4).ToUInt32());

            if (ImportDesc == Module)
                return new ImportEntry[0];

            List<ImportEntry> Entries = new List<ImportEntry>();

            while (true)
            {
                uint OriginalFirstThunk = Read(ImportDesc.Sum(4 * 0), 4).ToUInt32();
                //uint TimeDateStamp  =     Read(ImportDesc.Sum(4 * 1), 4).ToUInt32();
                //uint ForwarderChain =     Read(ImportDesc.Sum(4 * 2), 4).ToUInt32();
                uint Name = Read(ImportDesc.Sum(4 * 3), 4).ToUInt32();
                uint FirstThunk = Read(ImportDesc.Sum(4 * 4), 4).ToUInt32();

                if (OriginalFirstThunk == 0x00)
                    break;

                string ModuleName = ReadString(Module.Sum(Name), false);

                IntPtr DataAddr = Module.Sum(OriginalFirstThunk);
                IntPtr IATAddr = Module.Sum(FirstThunk);
                while (true)
                {
                    IntPtr EntryPtr = Read(DataAddr, PtrSize).ToIntPtr();

                    if (EntryPtr == IntPtr.Zero)
                        break;

                    bool ImportByOrdinal = false;
                    if ((EntryPtr.ToUInt64() & OrdinalFlag) == OrdinalFlag)
                    {
                        EntryPtr = ((PtrSize == 8 ? EntryPtr.ToUInt64() : EntryPtr.ToUInt32()) ^ OrdinalFlag).ToIntPtr();
                        ImportByOrdinal = true;
                    }
                    else
                        EntryPtr = Module.Sum(EntryPtr);

                    ushort Hint = ImportByOrdinal ? (ushort)EntryPtr.ToUInt32() : Read(EntryPtr, 2).ToUInt16();
                    string ExportName = ImportByOrdinal ? null : ReadString(EntryPtr.Sum(2), false);

                    Entries.Add(new ImportEntry()
                    {
                        Function = ExportName,
                        Ordinal = Hint,
                        Module = ModuleName,
                        ImportAddress = IATAddr,
                        FunctionAddress = Read(IATAddr, PtrSize).ToIntPtr()
                    });

                    DataAddr = DataAddr.Sum(PtrSize);
                    IATAddr = IATAddr.Sum(PtrSize);
                }


                ImportDesc = ImportDesc.Sum(0x14);//sizeof(_IMAGE_IMPORT_DESCRIPTOR)
            }

            return Entries.ToArray();
        }

        internal static byte[] Read(IntPtr Address, uint Length)
        {
            byte[] Buffer = new byte[Length];
            if (!ChangeProtection(Address, Buffer.Length, Protection.PAGE_READWRITE, out Protection Original))
                throw new Exception($"Falied to change the R/W memory permissions at {Address.ToUInt32():X8}");
            Marshal.Copy(Address, Buffer, 0, Buffer.Length);
            if (!ChangeProtection(Address, Buffer.Length, Original))
                throw new Exception($"Falied to restore the memory permissions at {Address.ToUInt32():X8}");
            return Buffer;
        }
        internal static bool Write(IntPtr Address, byte[] Content, Protection? NewProtection = null)
        {
            ChangeProtection(Address, Content.Length, Protection.PAGE_READWRITE, out Protection Original);

            uint Saved = (uint)Content.LongLength;
            Marshal.Copy(Content, 0, Address, Content.Length);
            
            if (NewProtection.HasValue)
                ChangeProtection(Address, Content.Length, NewProtection.Value);
            else
                ChangeProtection(Address, Content.Length, Original);

            if (Saved != Content.Length)
                return false;

            return true;
        }

        internal static string ReadString(IntPtr Address, bool Unicode)
        {
            List<byte> Buffer = new List<byte>();
            IntPtr CPos = Address;
            do
            {
                byte[] Char = Read(CPos, Unicode ? 2u : 1u);
                if (Unicode && Char[0] == 0x00 && Char[1] == 0x00)
                    break;
                if (!Unicode && Char[0] == 0x00)
                    break;


                Buffer.AddRange(Char);

                CPos = CPos.Sum(Unicode ? 2u : 1u);
            } while (true);

            return Unicode ? System.Text.Encoding.Unicode.GetString(Buffer.ToArray()) : System.Text.Encoding.Default.GetString(Buffer.ToArray());
        }

        public static int JmpSize { get; private set; } = IntPtr.Size == 8 ? 12 : 5;
        public static byte[] AssembleJump(IntPtr From, IntPtr Destination)
        {            
            byte[] jmp = new byte[JmpSize];
            if (IntPtr.Size == 8)
            {
                //x64
                new byte[] { 0x48, 0xb8 }.CopyTo(jmp, 0);
                BitConverter.GetBytes(unchecked((ulong)Destination.ToInt64())).CopyTo(jmp, 2);
                new byte[] { 0xFF, 0xE0 }.CopyTo(jmp, 10);
            }
            else
            {
                //x86
                jmp[0] = 0xE9;
                int Result = (int)(Destination.ToInt64() - From.ToInt64() - JmpSize);
                BitConverter.GetBytes(Result).CopyTo(jmp, 1);
            }

            return jmp;
        }

        public static ushort SearchFunctionOridinal(string Module, string Function) => SearchFunctionOridinal(LoadLibrary(Module), Function);
        public static ushort SearchFunctionOridinal(IntPtr Module, string Function)
        {
            IntPtr ProcAddr = GetProcAddress(Module, Function);
            if (ProcAddr == IntPtr.Zero)
                throw new KeyNotFoundException("DLL Export Not Found");

            for (ushort i = 0; true; i++)
            {
                IntPtr Addr = GetProcAddress(Module, i);
                if (Addr == ProcAddr)
                    return i;
            }

            throw new KeyNotFoundException("DLL Export Ordignal Not Found");
        }

        internal static ulong ToUInt64(this IntPtr Ptr) => unchecked((ulong)Ptr.ToInt64());
        internal static uint ToUInt32(this IntPtr Ptr) => unchecked((uint)(Ptr.ToInt64() & 0xFFFFFFFF));

        internal static IntPtr ToIntPtr(this ulong Int) => new IntPtr(unchecked((long)Int));

        internal static IntPtr Sum(this IntPtr Pointer, IntPtr Value) => (Pointer.ToUInt64() + Value.ToUInt64()).ToIntPtr();
        internal static IntPtr Sum(this IntPtr Pointer, long Value) => (Pointer.ToUInt64() + (ulong)Value).ToIntPtr();

        internal static uint ToUInt32(this byte[] Data, int Address = 0) => BitConverter.ToUInt32(Data, Address);
        internal static ushort ToUInt16(this byte[] Data, int Address = 0) => BitConverter.ToUInt16(Data, Address);
        internal static int ToInt32(this byte[] Data, int Address = 0) => BitConverter.ToInt32(Data, Address);
        internal static long ToInt64(this byte[] Data, int Address = 0) => BitConverter.ToInt64(Data, Address);

        internal static IntPtr ToIntPtr(this byte[] Data, bool? x64 = null)
        {
            if (x64.HasValue)
                return new IntPtr(x64.Value ? Data.ToInt64() : Data.ToInt32());
            if (Data.Length >= 8)
                return new IntPtr(IntPtr.Size == 8 ? Data.ToInt64() : Data.ToInt32());
            return new IntPtr(Data.ToInt32());
        }


        internal static bool ChangeProtection(IntPtr Address, int Range, Protection Protection, out Protection OriginalProtection)
        {
            return VirtualProtect(Address, Range, Protection, out OriginalProtection);
        }

        internal static bool ChangeProtection(IntPtr Address, int Range, Protection Protection)
        {
            return VirtualProtect(Address, Range, Protection, out Protection OriginalProtection);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, Protection flNewProtect, out Protection lpflOldProtect);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, ushort Ordinal);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll")]
        static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, uint dwSize);

        internal static bool FlushInstructionCache(IntPtr Address, uint Size)
        {            
            IntPtr Handle = Process.GetCurrentProcess().Handle;
            return FlushInstructionCache(Handle, Address, Size);
        }

        internal enum Protection
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }
    }

    static partial class StringReloader
    {
        public static UnmanagedHook AutoHookCreator(string Module, string Function, Delegate Hook)
        {
            if (!ImportHook)
                return new UnmanagedHook(Module, Function, Hook);

            var RHook = UnmanagedHook.TryHookImport(Module, Function, Hook);
            if (Debugging && !RHook.ImportHook)
                Log("Import Hook Failed: {0} => {1}", true, Module, Function);

            return RHook;
        }
    }
}