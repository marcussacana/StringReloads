using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SRL {
    partial class StringReloader {

        static IntPtr ProcessChar(IntPtr CharInt) {
            ushort c = (ushort)ParsePtr(CharInt);
            if (CharRld.ContainsKey(c)) {
                return new IntPtr(CharRld[c]);
            }
            if (UnkRld.ContainsKey(c)) {
                return new IntPtr(UnkRld[c]);
            }
        
            if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) && Debugging)
                Log("Char Missed... l={0}|n=0x{1:X4}", true, (char)c, c);

            return CharInt;
        }

        internal static int ParsePtr(IntPtr IntPtr) {
            if (Environment.Is64BitProcess)
                return (int)(IntPtr.ToInt64() & int.MaxValue);
            else
                return IntPtr.ToInt32();
        }       
       
        internal static string StrMap(string Input, IntPtr InputPtr) {
            if (!DialogFound && !IsDialog(Input)) {
                return Input;
            }
            string Str = SimplfyMatch(Input);
            if (string.IsNullOrWhiteSpace(Str))
                return Input;
            if (LogString) {
                Log("Simplified: {0}", false, Str);
            }

            if (Replys.Contains(Str) && DialogFound) {
                return Input;
            }

            if (ContainsKey(Str)) {
                DialogFound = true;
                return GetEntry(Str);
            }

            Str = GetString(InputPtr, false);
            Str = SimplfyMatch(Str);
            if (ContainsKey(Str)) {
                DialogFound = true;
                return GetEntry(Str);
            }

            if (Debugging)
                MissMatch(Input);

            if (TLIB != null) {
                Str = TrimString(Input);
                if (IsDialog(Str)) {
                    string TL = TLIB.Call("TLIB.Google", "Translate", Str, SourceLang, TargetLang);
                    AppendLst(Str, TL, MTLCache);
                    TL = ReplaceChars(TL);
                    Log("\"{0}\" Automatically Transalted.", true, Str);
                    AddEntry(SimplfyMatch(Str), TL);
                    return TL;
                }
            }

            if (SpecialLineBreaker)
                return Input.Replace("\n", GameLineBreaker);

            return Input;
        }

        internal static void Init() {
            try {
                if (Initialized)
                    return;

                if (!CloseEventAdded) {
                    CloseEventAdded = true;
                    AppDomain.CurrentDomain.ProcessExit += ProcessOver;
                    new Thread(ShowLoading).Start();
                }

                CheckArguments();
                if (Debugging) {
                    Log("Strings Reloads - v1.0");
                    Log("Soft-Translation Engine - By Marcussacana");
                    Log("Debug Mode Enabled...");

                }

                if (File.Exists("Modifier.cs")) {
                    Log("Enabling String Modifier...", true);
                    try {
                        DotNetVM VM = new DotNetVM(File.ReadAllText("Modifier.cs", System.Text.Encoding.UTF8));
                        Modifier = VM;
                        Log("Modifier Compiled");
                    } catch (Exception ex) {
                        Log("Failed to compile the Modifier\n===========\n{0}\n===========\n{1}", false, ex.Message, ex.Source);
                    }
                }

                if (!PECSVal(File.ReadAllBytes(SrlDll))) {
                    return;
                }

                if (File.Exists(TLDP) && TLIB == null) {
                    if (Ini.GetConfigStatus("MTL", "SourceLang", IniPath) == Ini.Status.Ok) {
                        if (Ini.GetConfigStatus("MTL", "TargetLang", IniPath) == Ini.Status.Ok) {
                            SourceLang = Ini.GetConfig("MTL", "SourceLang", IniPath, true);
                            TargetLang = Ini.GetConfig("MTL", "TargetLang", IniPath, true);
                            TLIB = new DotNetVM(TLDP);
                            Log("Machine Translation Enabled", true);
                        }
                    }
                }

                if (!File.Exists(TLMap) || Ini.GetConfig(CfgName, "Rebuild", IniPath, false).ToLower() == "true") {
                    Log("Unabled to load the {0}", false, TLMap);
                    if (File.Exists(TLMapSrc) || File.Exists(string.Format(TLMapSrcMsk, 1))) {
                        Log("Compiling String Reloads, Please Wait...");
                        CompileStrMap();
                    } else {
                        Log("Can't Compile Strings because the Strings.lst has not found.");
                        Thread.Sleep(3000);
                        Environment.Exit(2);
                    }
                }

                LoadConfig();
                LoadData();

                if (Debugging && File.Exists(TLMapSrc)) {
                    Log("Loading Dumped Data...");
                    uint cnt = 0;
                    using (TextReader Reader = File.OpenText(TLMapSrc)) {
                        while (Reader.Peek() >= 0) {
                            AddMissed(SimplfyMatch(Reader.ReadLine()));
                            Reader.ReadLine();
                            cnt++;
                        }
                        Reader.Close();
                    }
                    Log("Dumped Data Loaded, {0} Entries Loaded.", false, cnt);
                }

                if (TLIB != null && File.Exists(MTLCache)) {
                    Log("Loading MTL Cache...", true);
                    List<string> Ori = new List<string>();
                    List<string> TL = new List<string>();
                    ReadDump(MTLCache, ref Ori, ref TL);

                    for (int i = 0; i < Ori.Count; i++) {
                        AddEntry(SimplfyMatch(Ori[i]), ReplaceChars(TL[i]));
                    }
                }

            } catch (Exception ex) {
                Log("Failed to Initialize...");
                throw ex;
            }
        }

        private static void ProcessOver(object sender, EventArgs e) {
            Log("Exiting Process...", true);
            try {
                IntPtr[] Ptrs = GetPtrs();
                foreach (IntPtr Ptr in Ptrs) {
                    try {
                        Marshal.FreeHGlobal(Ptr);
                    } catch { }
                }
            } catch { }
            EndPipe();

            if (Ini.GetConfig(CfgName, "AntiCrash", IniPath, false).ToLower() == "true") {
                Log("Forcing OK Process end Signal...", true);
                Environment.Exit(0);
            }
        }
                
    }
}


