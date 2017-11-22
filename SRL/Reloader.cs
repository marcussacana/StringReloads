using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
                Warning("Char Missed... l={0}|n=0x{1:X4}", (char)c, c);

            return CharInt;
        }

        internal static int ParsePtr(IntPtr IntPtr) {
            if (Environment.Is64BitProcess)
                return (int)(IntPtr.ToInt64() & int.MaxValue);
            else
                return IntPtr.ToInt32();
        }       
       
        internal static string StrMap(string Input, IntPtr InputPtr, bool Native) {
            if (!DialogFound && !IsDialog(Input)) {
                return Input;
            }

            string Str = SimplfyMatch(Input);
            if (string.IsNullOrWhiteSpace(Str))
                return Input;

            if (LogString) {
                Log("Input: {0}", false, Input);
            }

            if (InCache(Str) && DialogFound) {
                return Input;
            }

            if (ContainsKey(Str)) {
                DialogFound = true;
                string Rst = EnableWordWrap ? WordWrap(GetEntry(Str)) : GetEntry(Str);
                if (Native)
                    return ReplaceChars(Rst, true);
                return Rst;
            }

            if (InputPtr != IntPtr.Zero) {
                Str = GetString(InputPtr, false);
                Str = SimplfyMatch(Str);
                if (ContainsKey(Str)) {
                    DialogFound = true;

                    string Rst = EnableWordWrap ? WordWrap(GetEntry(Str)) : GetEntry(Str);
                    if (Native)
                        return ReplaceChars(Rst, true);
                    return Rst;
                }
            }

            if (ValidateMask(Input)) {
                DialogFound = true;
                string Result = ProcesMask(Input);
                if (Native)
                    return ReplaceChars(Result, true);
                return Result;
            }

            if (Debugging)
                MissMatch(Input);

            if (TLIB != null) {
                Str = TrimString(Input);
                if (IsDialog(Str)) {
                    string Ori = MergeLines(Str);

                    string TL = null;
                    if (Online) {
                        try {
                            TL = TLIB.Call("TLIB.Google", "Translate", Ori, SourceLang, TargetLang);
                        } catch {
                            Log("Connection Failed, Disabling MTL for 30m", true);
                            Online = false;
                        }
                    }

                    if (!Online)
                        return Input;

                    if (Str != TL)
                        AppendLst(Str, TL, MTLCache);

                    if (EnableWordWrap)
                        TL = WordWrap(TL);

                    if (!Native)
                        TL = ReplaceChars(TL);

                    Log("\"{0}\" Automatically Translated.", true, Str);
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
                        Log("Modifier Compiled", true);
                    } catch (Exception ex) {
                        Error("Failed to compile the Modifier\n===========\n{0}\n===========\n{1}", ex.Message, ex.Source);
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
                        Error("Can't Compile Strings because the Strings.lst has not found.");
                        Thread.Sleep(5000);
                        Environment.Exit(2);
                    }
                }

                LoadConfig();
                LoadData();

                if (Debugging && File.Exists(TLMapSrc)) {
                    Log("Loading Dumped Data...");

                    var Strs = new List<string>();
                    var Ign = new List<string>();
                    ReadDump(TLMapSrc, ref Strs, ref Ign, true);

                    foreach (string str in Strs)
                        AddMissed(SimplfyMatch(str));

                    Log("Dumped Data Loaded, {0} Entries Loaded.", false, Strs.Count);
                }

                if (TLIB != null && File.Exists(MTLCache)) {
                    Log("Loading MTL Cache...", true);
                    List<string> Ori = new List<string>();
                    List<string> TL = new List<string>();
                    ReadDump(MTLCache, ref Ori, ref TL);

                    for (int i = 0; i < Ori.Count; i++) {
                        string Match = SimplfyMatch(Ori[i]);
                        if (!ContainsKey(Match))
                            AddEntry(Match, ReplaceChars(TL[i]));
                    }
                }

            } catch (Exception ex) {
                Error("Failed to Initialize...");
                throw ex;
            }
        }

        private static void WindowHook() {
            while (!Initialized)
                Thread.Sleep(100);            

            while (GameHandler == IntPtr.Zero)
                Thread.Sleep(1000);            

            while (true) {
                Thread.Sleep(100);

                var CB = new CallBack(ProcessWindow);
                EnumWindows(CB, 0);

                IntPtr Handler = GetMenu(GameHandler);
                ProcessMenu(Handler);
            }
        }

        private static bool ProcessWindow(IntPtr Handler, int Paramters) {
            int Len = GetWindowTextLength(Handler);
            StringBuilder sb = new StringBuilder(Len + 1);
            GetWindowText(Handler, sb, sb.Capacity);

            string Ori = sb.ToString();

            if (Replys.Contains(SimplfyMatch(Ori)))
                return true;

            uint HandlePID;
            GetWindowThreadProcessId(Handler, out HandlePID);

            if (HandlePID == GamePID) {
                var CB = new CallBack(ProcessWindow);
                EnumChildWindows(Handler, CB, IntPtr.Zero);
            } else
                return true;

            string Reload = StrMap(Ori, IntPtr.Zero, true);
            CacheReply(Reload);

            if (Ori == Reload)
                return true;
            

            HandleRef href = new HandleRef(null, Handler);
            SendMessage(href, WM_SETTEXT, IntPtr.Zero, Reload);

            if (InvalidateWindow)
                ForcePaint(Handler);

            return true;
        }
        private static void ProcessMenu(IntPtr Handler) {
            int MenuCount = GetMenuItemCount(Handler);
            if (MenuCount == -1)
                return;
            var MenuInfo = new MENUITEMINFO();
            for (int i = 0; i < MenuCount; i++) {
                MenuInfo = new MENUITEMINFO() {
                    cbSize = MENUITEMINFO.SizeOf,
                    fMask = MIIM_STRING | MIIM_SUBMENU,
                    fType = MFT_STRING,
                    dwTypeData = new string(new char[1024]),
                    cch = 1025
                };

                bool Sucess = GetMenuItemInfo(Handler, i, true, ref MenuInfo);

                string Ori = MenuInfo.dwTypeData;

                if (Replys.Contains(SimplfyMatch(Ori)))
                    continue;

                string Reload = StrMap(Ori, IntPtr.Zero, true);
                CacheReply(Reload);

                if (MenuInfo.hSubMenu != IntPtr.Zero)
                    ProcessMenu(MenuInfo.hSubMenu);

                if (!Sucess)
                    continue;

                if (Ori == Reload) {
                    continue;
                }

                MenuInfo.dwTypeData = Reload;

                Sucess = SetMenuItemInfo(Handler, i, true, ref MenuInfo);

                if (InvalidateWindow)
                    ForcePaint(Handler);
            }
        }

        private static void ProcessOver(object sender, EventArgs e) {
            if (FreeOnExit)
                try {
                    Log("Exiting Process...", true);
                    IntPtr[] Ptrs = GetPtrs();
                    foreach (IntPtr Ptr in Ptrs) {
                        try {
                            Marshal.FreeHGlobal(Ptr);
                        } catch { }
                    }
                } catch { }


            EndPipe();

            if (AntiCrash) {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
                
    }
}