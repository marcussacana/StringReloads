using StringReloads.Engine.Unmanaged;
using StringReloads.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using StringReloads.Hook.Base;
using Iced.Intel;

namespace StringReloads.Tools
{
    internal unsafe class Scanner
    {
        public static IEnumerable<long> SearchExportCall(string Function)
        {
            var Imports = ModuleInfo.GetMainModuleImports();
            var Func = Imports.Where(x => x.Function == Function).Single();

            var Bitness = Environment.Is64BitProcess ? 64 : 32;

            byte?[] Pattern = new byte?[] { 0xFF, 0x15 };
            foreach (var lAddress in Scan(Pattern))
            {
                Decoder Dissassembler = Decoder.Create(Bitness, new MemoryCodeReader(lAddress));
                Dissassembler.IP = (ulong)lAddress;
                var Call = Dissassembler.PeekDecode();
                var MemAddress = Call.IPRelativeMemoryAddress;

                if (MemAddress == GetAddress(Func))
                {
                    Log.Debug($"Call dword ptr ds: [&{Function}] - Found at 0x{lAddress:X16}");
                    yield return lAddress;
                }
            }
        }
        private unsafe static ulong GetAddress(ImportEntry Entry) => (ulong)Entry.ImportAddress;

        internal static IEnumerable<long> Scan(byte?[] Pattern, ulong? BeginAddress = null, bool Up = false)
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
