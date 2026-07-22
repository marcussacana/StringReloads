using static StringReloads.Hook.Base.Extensions;
using static StringReloads.Engine.User;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StringReloads.Engine;
using StringReloads.Hook;
using StringReloads.Hook.Base;
using StringReloads.Engine.String;
using StringReloads.Engine.Interface;
using StringReloads.Tools;
using StringReloads.Engine.Unmanaged;
using Iced.Intel;
using Decoder = Iced.Intel.Decoder;

namespace StringReloads.AutoInstall
{
    unsafe class SoftPalMethodD : IAutoInstall
    {
        Intercept Intercepter;
        Config Config => Config.Default;
        Dictionary<string, string> SoftPalConfig;
        void* HookAddress = null;

        public string Name => "SoftPal#D";

        public bool IsCompatible()
        {
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll", "pal.dll");
            if (!File.Exists(dllPath) && GetLibrary("pal.dll") == null)
            {
                var mainImports = ModuleInfo.GetMainModuleImports();
                bool referencesPal = mainImports.Any(x => x.Module != null && x.Module.Equals("pal.dll", StringComparison.OrdinalIgnoreCase));
                if (!referencesPal && !File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Text.dat")))
                    return false;
            }

            SoftPalConfig = Config.GetValues("SoftPal") ?? new Dictionary<string, string>();

            if (SoftPalConfig.ContainsKey("hookoffset_d"))
            {
                ulong offset = SoftPalConfig["hookoffset_d"].ToUInt64();
                HookAddress = (void*)((ulong)Config.GameBaseAddress + offset);
                return true;
            }

            HookAddress = FindTextResolverHookAddress();
            if (HookAddress != null)
            {
                ulong offset = (ulong)HookAddress - (ulong)Config.GameBaseAddress;
                SoftPalConfig["hookoffset_d"] = offset.ToString();
                SoftPalConfig["forcemethodd"] = "true";
                SoftPalConfig["enginesize"] = new FileInfo(Config.GameExePath).Length.ToString();
                Config.SetValues("SoftPal", SoftPalConfig);
                Config.SaveSettings();
                return true;
            }

            return false;
        }

        public void Install()
        {
            if (HookAddress == null)
            {
                SoftPalConfig = Config.GetValues("SoftPal") ?? new Dictionary<string, string>();
                if (SoftPalConfig.ContainsKey("hookoffset_d"))
                {
                    ulong offset = SoftPalConfig["hookoffset_d"].ToUInt64();
                    HookAddress = (void*)((ulong)Config.GameBaseAddress + offset);
                }
            }

            if (HookAddress == null)
            {
                Log.Error("SoftPalMethodD: Text resolver hook address not found.");
                return;
            }

            Log.Debug($"SoftPalMethodD: Hooking at 0x{(ulong)HookAddress:X8}");
            Intercepter = new ManagedInterceptor(HookAddress, new ManagedInterceptDelegate(TextBufferHook));
            Intercepter.Install();
        }

        public void Uninstall()
        {
            Intercepter?.Uninstall();
        }

        void TextBufferHook(ref ulong ESP, ref ulong EAX, ref ulong ECX, ref ulong EDX, ref ulong EBX, ref ulong EBP, ref ulong ESI, ref ulong EDI)
        {
            byte* bufferPtr = (byte*)EDI;
            if (bufferPtr == null || SanityChecks.IsBadPtr(bufferPtr))
                return;


            if (*(uint*)bufferPtr == 0) return;

            CString sourceStr = &bufferPtr[4];
            var originalText = (string)sourceStr;
            if (string.IsNullOrEmpty(originalText))
                return;

            var modPtr = (CString)EntryPoint.Process((void*)sourceStr);
            if (modPtr == null || SanityChecks.IsBadPtr(modPtr))
                return;

            if (string.IsNullOrEmpty(modPtr) || modPtr == originalText)
                return;

            sourceStr.SetContent(modPtr);
        }

        private void* FindTextResolverHookAddress()
        {
            try
            {
                byte?[] strPattern = Encoding.ASCII.GetBytes("AdvGetVarString").Cast<byte?>().ToArray();

                long? strMatch = Scanner.ScanModule(strPattern).FirstOrDefault();
                if (strMatch == null)
                    return null;

                ulong strAddr = (ulong)strMatch.Value;
                byte[] strBytes = BitConverter.GetBytes((uint)strAddr);
                byte?[] pushPattern = new byte?[] { 0x68, strBytes[0], strBytes[1], strBytes[2], strBytes[3] };

                var pushMatches = Scanner.ScanModule(pushPattern);
                if (!pushMatches.Any())
                    return null;

                List<ulong> excludePushes = new List<ulong>();
                string[] forbiddenStrings = new string[] { "entry filename buffer over", "get_private_profile_string" };

                foreach (var forbidden in forbiddenStrings)
                {
                    byte?[] fPattern = Encoding.ASCII.GetBytes(forbidden).Cast<byte?>().ToArray();
                    long? fMatch = Scanner.ScanModule(fPattern).FirstOrDefault();
                    if (fMatch != null)
                    {
                        byte[] fBytes = BitConverter.GetBytes((uint)fMatch.Value);
                        byte?[] fPushPattern = new byte?[] { 0x68, fBytes[0], fBytes[1], fBytes[2], fBytes[3] };
                        foreach (long p in Scanner.ScanModule(fPushPattern))
                        {
                            excludePushes.Add((ulong)p);
                        }
                    }
                }

                foreach (long pushMatch in pushMatches)
                {
                    ulong pushAddr = (ulong)pushMatch;

                    bool hasImmediate10M = false;
                    ulong searchStart = pushAddr >= 0x100 ? pushAddr - 0x100 : pushAddr;

                    if (excludePushes.Any(exAddr => exAddr >= searchStart && exAddr <= (pushAddr + 0x250)))
                    {
                        Log.Debug($"SoftPalMethodD: Skipping candidate at 0x{pushAddr:X8} (references forbidden string).");
                        continue;
                    }
                    var reader = new MemoryCodeReader((void*)searchStart, 0x250);
                    var decoder = Decoder.Create(32, reader);
                    decoder.IP = searchStart;

                    for (int i = 0; i < 120; i++)
                    {
                        var ins = decoder.Decode();
                        if (ins.IsInvalid) break;

                        if (HasImmediate(ins, 0x10000000))
                        {
                            hasImmediate10M = true;
                            break;
                        }
                    }

                    if (!hasImmediate10M)
                    {
                        byte* pCheck = (byte*)searchStart;
                        for (int i = 0; i < 0x200; i++)
                        {
                            if (pCheck[i] == 0x00 && pCheck[i + 1] == 0x00 && pCheck[i + 2] == 0x00 && pCheck[i + 3] == 0x10)
                            {
                                hasImmediate10M = true;
                                break;
                            }
                        }
                    }

                    if (!hasImmediate10M)
                        continue;

                    var forwardReader = new MemoryCodeReader((void*)pushAddr, 0x200);
                    var forwardDecoder = Decoder.Create(32, forwardReader);
                    forwardDecoder.IP = pushAddr;

                    int toSkip = 1;

                    for (int i = 0; i < 60; i++)
                    {
                        var ins = forwardDecoder.Decode();

                        if (ins.Code == Code.Add_rm32_imm8 || ins.Code == Code.Add_rm32_imm32)
                        {
                            if (ins.Op0Register == Register.ESP && HasImmediate(ins, 12))
                            {
                                if (toSkip > 0)
                                {
                                    toSkip--;
                                    continue;
                                }
                                return (void*)ins.IP;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"SoftPalMethodD: Error during scan: {ex.Message}");
            }

            return null;
        }

        private static bool HasImmediate(in Instruction ins, ulong value)
        {
            for (int i = 0; i < ins.OpCount; i++)
            {
                var kind = ins.GetOpKind(i);
                if (kind == OpKind.Immediate8 || kind == OpKind.Immediate16 ||
                    kind == OpKind.Immediate32 || kind == OpKind.Immediate64 ||
                    kind == OpKind.Immediate8to16 || kind == OpKind.Immediate8to32 ||
                    kind == OpKind.Immediate8to64 || kind == OpKind.Immediate32to64)
                {
                    if (ins.GetImmediate(i) == value)
                        return true;
                }
            }
            return false;
        }
    }
}
