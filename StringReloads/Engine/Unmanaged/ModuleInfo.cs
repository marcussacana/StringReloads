using StringReloads.Engine.String;
using System;
using System.Collections.Generic;

namespace StringReloads.Engine.Unmanaged
{
    public unsafe static class ModuleInfo
    {
        public unsafe static CodeInfo GetCodeInfo(byte* hModule)
        {
            ulong PEStart = *(uint*)(hModule + 0x3C) + (ulong)hModule;
            ulong OptionalHeader = PEStart + 0x18;

            uint SizeOfCode = *(uint*)(OptionalHeader + 0x04);
            uint EntryPoint = *(uint*)(OptionalHeader + 0x10);
            uint BaseOfCode = *(uint*)(OptionalHeader + 0x14);

            return new CodeInfo()
            {
                CodeAddress = hModule + BaseOfCode,
                EntryPoint = hModule + EntryPoint,
                CodeSize = SizeOfCode
            };
        }

        public unsafe static ImportEntry[] GetMainModuleImports() => GetModuleImports((byte*)Config.GameBaseAddress);
        public unsafe static ImportEntry[] GetModuleImports(byte* Module)
        {
            if (Module == null)
                throw new Exception("Invalid Module...");

            uint PtrSize = Environment.Is64BitProcess ? 8u : 4u;

            ulong OrdinalFlag = (1ul << (int)((8 * PtrSize) - 1));

            ulong PEStart = *(uint*)(Module + 0x3C);
            ulong OptionalHeader = PEStart + 0x18;

            ulong ImageDataDirectoryPtr = OptionalHeader + (PtrSize == 8 ? 0x70u : 0x60u);

            ulong ImportTableEntry = ImageDataDirectoryPtr + 0x8;

            uint RVA = (uint)ImportTableEntry;

            uint* ImportDesc = (uint*)(Module + *(uint*)(Module + RVA));

            if (ImportDesc == Module)
                return new ImportEntry[0];

            List<ImportEntry> Entries = new List<ImportEntry>();

            while (true)
            {
                uint OriginalFirstThunk = ImportDesc[0];
                uint Name = ImportDesc[3];
                uint FirstThunk = ImportDesc[4];

                if (OriginalFirstThunk == 0x00)
                    break;

                string ModuleName = (CString)(Module + Name);

                void** DataAddr = (void**)(Module + OriginalFirstThunk);
                void** IATAddr = (void**)(Module + FirstThunk);
                while (true)
                {
                    void* EntryPtr = *DataAddr;

                    if (EntryPtr == null)
                        break;

                    bool ImportByOrdinal = false;
                    if (((ulong)EntryPtr & OrdinalFlag) == OrdinalFlag)
                    {
                        EntryPtr = (void*)((ulong)EntryPtr ^ OrdinalFlag);
                        ImportByOrdinal = true;
                    }
                    else
                        EntryPtr = (void*)(Module + (ulong)EntryPtr);

                    ushort Hint = ImportByOrdinal ? (ushort)EntryPtr : *(ushort*)EntryPtr;

                    string ExportName = null;
                    if (!ImportByOrdinal)
                        ExportName = (CString)(void*)((ulong)EntryPtr + 2);

                    Entries.Add(new ImportEntry()
                    {
                        Function = ExportName,
                        Ordinal = Hint,
                        Module = ModuleName,
                        ImportAddress = IATAddr,
                        FunctionAddress = *IATAddr
                    });

                    DataAddr++;
                    IATAddr++;
                }


                ImportDesc += 5;//sizeof(_IMAGE_IMPORT_DESCRIPTOR)
            }

            return Entries.ToArray();
        }
    }
    public unsafe struct CodeInfo
    {
        /// <summary>
        /// The Begin Address of the module code
        /// </summary>
        public void* CodeAddress;
        /// <summary>
        /// The Size of the module code
        /// </summary>
        public uint CodeSize;
        /// <summary>
        /// The Entry Point of the module
        /// </summary>
        public void* EntryPoint;

        public void* EndCodeAddress => (void*)((ulong)CodeAddress + CodeSize);
        public bool AddressIsContained(void* Address) => Address >= CodeAddress && Address <= EndCodeAddress;
    }

    public unsafe struct ImportEntry
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
        public void* ImportAddress;

        /// <summary>
        /// The Address of the Imported Function
        /// </summary>
        public void* FunctionAddress;

    }

}