using StringReloads.Engine.Unmanaged;
using StringReloads.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using StringReloads.Hook.Base;
using Iced.Intel;

namespace StringReloads.Tools
{
    public unsafe class Scanner
    {
        public static IEnumerable<long> SearchExportCall(string Function) => SearchExportCall(Function, null);

        public static IEnumerable<long> SearchExportCall(string Function, string Module)
        {
            var Imports = ModuleInfo.GetMainModuleImports();
            var matches = Imports.Where(x => string.Equals(x.Function, Function, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(Module))
                matches = matches.Where(x => string.Equals(x.Module, Module, StringComparison.OrdinalIgnoreCase) ||
                                             (x.Module != null && x.Module.IndexOf(Module, StringComparison.OrdinalIgnoreCase) >= 0));

            var func = matches.FirstOrDefault();
            ulong iatAddr = 0;
            ulong funcAddr = 0;
            unsafe
            {
                iatAddr = (ulong)func.ImportAddress;
                funcAddr = (ulong)func.FunctionAddress;
            }

            if (iatAddr == 0)
                yield break;

            foreach (var call in SearchImportCalls(iatAddr, funcAddr))
            {
                Log.Debug($"Call dword ptr ds: [&{Function}] - Found at 0x{call:X16}");
                yield return call;
            }
        }

        public static IEnumerable<long> SearchExportCallers(string Function) => SearchExportCallers(Function, null);

        public static IEnumerable<long> SearchExportCallers(string Function, string Module)
        {
            var Imports = ModuleInfo.GetMainModuleImports();
            var matches = Imports.Where(x => string.Equals(x.Function, Function, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(Module))
                matches = matches.Where(x => string.Equals(x.Module, Module, StringComparison.OrdinalIgnoreCase) ||
                                             (x.Module != null && x.Module.IndexOf(Module, StringComparison.OrdinalIgnoreCase) >= 0));

            var func = matches.FirstOrDefault();
            ulong iatAddr = 0;
            ulong funcAddr = 0;
            unsafe
            {
                iatAddr = (ulong)func.ImportAddress;
                funcAddr = (ulong)func.FunctionAddress;
            }

            if (iatAddr == 0)
                yield break;

            foreach (var caller in SearchCallers(iatAddr, funcAddr))
            {
                yield return caller;
            }
        }

        public unsafe static IEnumerable<long> SearchImportCalls(void* iatAddress, void* exportAddress = null)
        {
            return SearchImportCalls((ulong)iatAddress, (ulong)exportAddress);
        }

        public static IEnumerable<long> SearchImportCalls(ulong iatAddress, ulong exportAddress = 0)
        {
            var bitness = Environment.Is64BitProcess ? 64 : 32;
            bool is64 = bitness == 64;

            CodeInfo info;
            ulong codeStart;
            uint codeLen;
            unsafe
            {
                info = ModuleInfo.GetCodeInfo((byte*)Config.GameBaseAddress);
                codeStart = (ulong)info.CodeAddress;
                codeLen = info.CodeSize;
            }

            if (codeLen < 10)
                yield break;

            for (uint i = 0; i + 6 <= codeLen; i++)
            {
                ulong currentAddr = codeStart + i;
                byte b0, b1;
                unsafe
                {
                    byte* p = (byte*)currentAddr;
                    b0 = p[0];
                    b1 = p[1];
                }

                // Direct call to IAT: FF 15
                if (iatAddress != 0 && b0 == 0xFF && b1 == 0x15)
                {
                    ulong target;
                    unsafe
                    {
                        byte* p = (byte*)currentAddr;
                        target = is64
                            ? (ulong)p + 6 + (ulong)(long)*(int*)(p + 2)
                            : *(uint*)(p + 2);
                    }

                    if (target == iatAddress)
                        yield return (long)currentAddr;
                }

                // Call to export or thunk: E8
                if (b0 == 0xE8)
                {
                    ulong target;
                    unsafe
                    {
                        byte* p = (byte*)currentAddr;
                        target = (ulong)p + 5 + (ulong)(long)*(int*)(p + 1);
                    }

                    bool match = false;
                    if (exportAddress != 0 && target == exportAddress)
                    {
                        match = true;
                    }
                    else if (iatAddress != 0)
                    {
                        bool isContained = false;
                        unsafe
                        {
                            isContained = info.AddressIsContained((void*)target);
                        }

                        if (isContained)
                        {
                            unsafe
                            {
                                byte* pTarget = (byte*)target;
                                if (pTarget[0] == 0xFF && pTarget[1] == 0x25)
                                {
                                    ulong thunkTarget = is64
                                        ? (ulong)pTarget + 6 + (ulong)(long)*(int*)(pTarget + 2)
                                        : *(uint*)(pTarget + 2);

                                    if (thunkTarget == iatAddress)
                                        match = true;
                                }
                            }
                        }
                    }

                    if (match)
                        yield return (long)currentAddr;
                }
            }
        }

        public unsafe static IEnumerable<long> SearchCallers(void* iatAddress, void* exportAddress = null)
        {
            return SearchCallers((ulong)iatAddress, (ulong)exportAddress);
        }

        public static IEnumerable<long> SearchCallers(ulong iatAddress, ulong exportAddress = 0)
        {
            var bitness = Environment.Is64BitProcess ? 64 : 32;
            bool is64 = bitness == 64;

            CodeInfo info;
            ulong codeStart;
            uint codeLen;
            unsafe
            {
                info = ModuleInfo.GetCodeInfo((byte*)Config.GameBaseAddress);
                codeStart = (ulong)info.CodeAddress;
                codeLen = info.CodeSize;
            }

            if (codeLen < 10)
                yield break;

            for (uint i = 0; i + 6 <= codeLen; i++)
            {
                ulong currentAddr = codeStart + i;
                byte b0, b1;
                unsafe
                {
                    byte* p = (byte*)currentAddr;
                    b0 = p[0];
                    b1 = p[1];
                }

                // Direct call to IAT: FF 15
                if (iatAddress != 0 && b0 == 0xFF && b1 == 0x15)
                {
                    ulong target;
                    unsafe
                    {
                        byte* p = (byte*)currentAddr;
                        target = is64
                            ? (ulong)p + 6 + (ulong)(long)*(int*)(p + 2)
                            : *(uint*)(p + 2);
                    }

                    if (target == iatAddress)
                        yield return (long)(currentAddr + 6);
                }

                // Call to export or thunk: E8
                if (b0 == 0xE8)
                {
                    ulong target;
                    unsafe
                    {
                        byte* p = (byte*)currentAddr;
                        target = (ulong)p + 5 + (ulong)(long)*(int*)(p + 1);
                    }

                    bool match = false;
                    if (exportAddress != 0 && target == exportAddress)
                    {
                        match = true;
                    }
                    else if (iatAddress != 0)
                    {
                        bool isContained = false;
                        unsafe
                        {
                            isContained = info.AddressIsContained((void*)target);
                        }

                        if (isContained)
                        {
                            unsafe
                            {
                                byte* pTarget = (byte*)target;
                                if (pTarget[0] == 0xFF && pTarget[1] == 0x25)
                                {
                                    ulong thunkTarget = is64
                                        ? (ulong)pTarget + 6 + (ulong)(long)*(int*)(pTarget + 2)
                                        : *(uint*)(pTarget + 2);

                                    if (thunkTarget == iatAddress)
                                        match = true;
                                }
                            }
                        }
                    }

                    if (match)
                        yield return (long)(currentAddr + 5);
                }

                // In 64-bit: mov reg, [iatAddress] followed by call reg
                if (is64 && iatAddress != 0 && i + 9 <= codeLen)
                {
                    bool indirectMatch = false;
                    unsafe
                    {
                        byte* p = (byte*)currentAddr;
                        if (p[0] == 0x48 && p[1] == 0x8B && (p[2] & 0xC7) == 0x05)
                        {
                            ulong target = (ulong)p + 7 + (ulong)(long)*(int*)(p + 3);
                            if (target == iatAddress)
                            {
                                if (p[7] == 0xFF && (p[8] & 0xF8) == 0xD0)
                                    indirectMatch = true;
                            }
                        }
                    }

                    if (indirectMatch)
                        yield return (long)(currentAddr + 9);
                }
            }
        }

        public static IEnumerable<long> ScanMemory(byte?[] Pattern, ulong baseAddress, uint size)
        {
            ulong start = baseAddress;
            ulong maxOffset = size >= (uint)Pattern.Length ? size - (uint)Pattern.Length : 0;

            for (ulong i = 0; i <= maxOffset; i++)
            {
                bool isMatch = false;
                unsafe
                {
                    byte* pAddress = (byte*)(start + i);
                    isMatch = CheckPattern(pAddress, Pattern);
                }

                if (isMatch)
                    yield return (long)(start + i);
            }
        }

        public static IEnumerable<long> ScanModule(byte?[] Pattern, ulong hModule = 0)
        {
            if (hModule == 0)
            {
                unsafe
                {
                    hModule = (ulong)Config.GameBaseAddress;
                }
            }

            SectionInfo[] sections = null;
            unsafe
            {
                sections = ModuleInfo.GetModuleSections((byte*)hModule);
            }

            if (sections != null && sections.Length > 0)
            {
                foreach (var sec in sections)
                {
                    if (sec.Size == 0 || sec.AddressValue == 0) continue;
                    foreach (var addr in ScanMemory(Pattern, sec.AddressValue, sec.Size))
                        yield return addr;
                }
            }
            else
            {
                ModuleMemoryInfo info;
                unsafe
                {
                    info = ModuleInfo.GetModuleMemoryInfo((byte*)hModule);
                }
                foreach (var addr in ScanMemory(Pattern, info.BaseAddressValue, info.ImageSize))
                    yield return addr;
            }
        }

        public static IEnumerable<long> Scan(byte?[] Pattern, ulong? BeginAddress = null, bool Up = false)
        {
            ulong? Match = BeginAddress;
            do
            {
                if (Match != null)
                {

                    if (Up)
                        Match--;
                    else
                        Match++;

                }

                Match = Up ? ScanUp(Pattern, Match ?? ulong.MaxValue) : ScanDown(Pattern, Match ?? 0ul);

                if (Match != null)
                    yield return (long)Match;

            } while (Match != null);
        }

        private static ulong? ScanUp(byte?[] Pattern, ulong BeginAddress = ulong.MaxValue)
        {
            var Info = ModuleInfo.GetCodeInfo((byte*)Config.GameBaseAddress);

            if (BeginAddress == ulong.MaxValue)
                BeginAddress = (ulong)Info.CodeAddress + Info.CodeSize;

            ulong CodeAdd = (ulong)Info.CodeAddress;
            for (ulong i = BeginAddress - CodeAdd; i >= 0; i--)
            {
                byte* pAddress = (byte*)(CodeAdd + i);
                if (!CheckPattern(pAddress, Pattern))
                    continue;

                return (ulong)pAddress;
            }

            return null;
        }

        private static ulong? ScanDown(byte?[] Pattern, ulong BeginAddress = 0)
        {
            var Info = ModuleInfo.GetCodeInfo((byte*)Config.GameBaseAddress);

            ulong CodeAdd = (ulong)Info.CodeAddress;
            ulong CodeLen = Info.CodeSize;

            if (BeginAddress != 0)
                BeginAddress = BeginAddress - CodeAdd;

            for (ulong i = BeginAddress; i < CodeLen; i++)
            {
                byte* pAddress = (byte*)(CodeAdd + i);
                if (!CheckPattern(pAddress, Pattern))
                    continue;

                return (ulong)pAddress;
            }

            return null;
        }

        private static bool CheckPattern(byte* Buffer, byte?[] Pattern)
        {
            for (int i = 0; i < Pattern.Length; i++)
            {
                if (Pattern[i] == null)
                    continue;
                byte bPattern = Pattern[i].Value;
                if (bPattern != Buffer[i])
                    return false;
            }
            return true;
        }
    }
}
