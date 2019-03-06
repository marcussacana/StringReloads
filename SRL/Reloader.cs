using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        static char ProcessChar(char Char) => (char)ProcessChar(new IntPtr(Char)).ToInt32();
        static char RestoreChar(char Char) {
            if (CharRld.ContainsValue(Char))
                return (char)CharRld.ReverseMatch(Char);
            if (UnkRld.ContainsValue(Char))
                return (char)UnkRld.ReverseMatch(Char);

            return Char;
        }

        static internal Key ReverseMatch<Key, Value>(this Dictionary<Key, Value> Dictionary, Value ValueToSearch) {
            if (!Dictionary.ContainsValue(ValueToSearch))
                throw new Exception("Value not Present in the Dictionary");

            int Index = Dictionary.Values.Select((value, index) => new { value, index })
                        .SkipWhile(pair => !pair.value.Equals(ValueToSearch)).FirstOrDefault().index;

            return Dictionary.Keys.ElementAt(Index);
        }

        internal static dynamic ParsePtr(IntPtr IntPtr) {
            
            if (Environment.Is64BitProcess)
                return unchecked((ulong)IntPtr.ToInt64());
            else
                return unchecked((uint)IntPtr.ToInt32());
        }

        internal static string RedirFaceName(string FaceName) {
            if (LogAll) {
                Log("Font Redirect Request: {0}", true, FaceName);
            }
            if (!FontReplaces.ContainsKey(FaceName.Trim())) {
                if (FontReplaces.ContainsKey("*"))
                    return FontReplaces["*"].To;

                if (!string.IsNullOrWhiteSpace(FontFaceName))
                    return FontFaceName;
                return FaceName;
            }
            return FontReplaces[FaceName.Trim()].To;
        }
        internal static string RedirFontSize(string FaceName) {
            if (!FontReplaces.ContainsKey(FaceName.Trim())) {
                return null;
            }
            if (FontReplaces.ContainsKey("*"))
                return FontReplaces["*"].Size;
            return FontReplaces[FaceName.Trim()].Size;
        }


        /// <summary>
        /// Reload a String
        /// </summary>
        /// <param name="Input">Original String</param>
        /// <param name="InputPtr">Original String Pointer, Use IntPtr.Zero don't have one</param>
        /// <param name="Native">When true, will return the string without char reloads.</param>
        /// <returns></returns>
        internal static string StrMap(string Input, IntPtr InputPtr, bool Native) {
            if (DecodeCharactersFromInput)
                Input = ReplaceChars(Input, true);

            bool IsDialog = Input.IsDialog();

            if (!DialogFound && !IsDialog)
                return Input;          

            if (string.IsNullOrWhiteSpace(Input))
                return Input;

            string Str = SimplfyMatch(Input);

            if ((LogAll || LogInput) && (!DumpStrOnly || IsDialog)) {
                if (!DumpStrOnly || !InCache("LOG: " + Str)) {
                    Log("[{1}] Input: {0}", true, Input, IsDialog ? "D" : "S");
                    if (DumpStrOnly)
                        CacheReply("LOG: " + Str);
                }
            }

            if (InCache(Str) && DialogFound && NotCachedOnly) {
                return Input;
            }

            if (ContainsKey(Str)) {
                string Entry = GetEntry(Str);
                
                string Rst = EnableWordWrap ? WordWrap(Entry) : Entry;
                if (Native)
                    return ReplaceChars(Rst, true);


                return Rst;
            }

            if (InputPtr != IntPtr.Zero) {
                Str = GetString(InputPtr, false);

                if (DecodeCharactersFromInput)
                    Str = ReplaceChars(Str, true);

                Str = SimplfyMatch(Str);
                if (ContainsKey(Str)) {

                    string Entry = GetEntry(Str);  
                    
                    string Rst = EnableWordWrap ? WordWrap(Entry) : Entry;
                    if (Native)
                        return ReplaceChars(Rst, true);
                    return Rst;
                }
            }

            if (ValidateMask(Input)) {
                try {

                    string Result = ProcesMask(Input);

                    if (Result.StartsWith(MaskWordWrap)) {
                        Result = Result.Substring(MaskWordWrap.Length, Result.Length - MaskWordWrap.Length);
                        Result = WordWrap(Result);
                    }

                    if (Native)
                        return ReplaceChars(Result, true);
                    return Result;
                } catch (Exception ex) {
                    Warning(ex.ToString());
                }
            }


            if (Debugging)
                Missmatch(Input);

            if (TLIB != null) {
                Str = TrimString(Input);
                if (Str.IsDialog()) {
                    string Ori = MergeLines(Str);

                    string TL = null;
                    if (Online) {
                        try {
                            Log("Translating: \"{0}\"", true, Str);
                            TL = TLIB.Call("TLIB.Google", "Translate", Ori, SourceLang, TargetLang);
                        } catch {
                            Log("Connection Failed, Disabling MTL for 30m", true);
                            Online = false;
                        }
                    }

                    if (!Online)
                        return Input;

                    if (SimplfyMatch(Ori) == SimplfyMatch(TL))
                        TL = Ori;

                    if (Str != TL)
                        AppendLst(Str, TL, MTLCache);

                    if (EnableWordWrap)
                        TL = WordWrap(TL);

                    if (!Native)
                        TL = ReplaceChars(TL);

                    AddEntry(SimplfyMatch(Str), TL);
                    return TL;
                }
            }

            if (SpecialLineBreaker)
                return Input.Replace("\n", GameLineBreaker);

            return Input;
        }

        internal static string UpdateOverlay(string Text) {
            try {
                if (Overlay != null && OverlayEnabled) {
                    if (!OverlayInitialized) {
                        OverlayInitialized = true;
                        Overlay.Call("Overlay.Exports", "HookWindow", GameHandler);
                    }

                    if (!PaddingSeted) {
                        PaddingSeted = true;
                        Overlay.Call("Overlay.Exports", "SetOverlayPadding", OPaddingTop, OPaddinBottom, OPaddinLeft, OPaddingRigth);
                    }

                    string ret = Overlay.Call("Overlay.Exports", "SetDialogue", ReplaceChars(Text, true).Replace(GameLineBreaker, "\n"));
                    ret = ReplaceChars(ret).Replace("\n", GameLineBreaker);
                    return ret;
                } else if (Text.StartsWith("::EVENT")) {
                    Text = Text.Substring(Text.IndexOf("::", 2) + 2);
                }
            } catch { }
            return Text;
        }

        internal static void Init() {
            try {
                if (Initialized) {
                    Log("Ops, Initialization Requested... But, is already initialized...", true);
                    return;
                }

                if (!CloseEventAdded) {
                    CloseEventAdded = true;
                    AppDomain.CurrentDomain.ProcessExit += ProcessOver;
                    new Thread(ShowLoading).Start();
                }

                CheckArguments();
                LoadConfig();

                if (Debugging) {
                    Log("Strings Reloads - v1.0");
                    Log("Soft-Translation Engine - By Marcussacana");
                    Log("Debug Mode Enabled...");
                }

                if (!DirectRequested)
                    Warning("You are using SRL through the old function, it is recommended to use GetDirectProcess");


                if (File.Exists(BaseDir + "EncodingModifier.cs")) {
                    Log("Enabling Encoding Modifier...", true);
                    try {
                        DotNetVM VM = new DotNetVM(File.ReadAllText(BaseDir + "EncodingModifier.cs", Encoding.UTF8));
                        EncodingModifier = VM;
                        Log("Encoding Modifier Compiled", true);
                    } catch (Exception ex) {
                        Error("Failed to compile the Encoding Modifier\n===========\n{0}\n===========\n{1}", ex.Message, ex.Source);
                    }
                }

                if (File.Exists(BaseDir + "StringModifier.cs")) {
                    Log("Enabling String Modifier...", true);
                    try {
                        DotNetVM VM = new DotNetVM(File.ReadAllText(BaseDir + "StringModifier.cs", Encoding.UTF8));
                        StringModifier = VM;
                        Log("String Modifier Compiled", true);
                    } catch (Exception ex) {
                        Error("Failed to compile the String Modifier\n===========\n{0}\n===========\n{1}", ex.Message, ex.Source);
                    }
                }

                //I Implement This to prevent 
                if (!PECSVal(File.ReadAllBytes(SrlDll))) {
#if DEBUG
                    Warning("SRL Engine - Unauthenticated Debug Build");
#else
                    Error("SRL Engine - Unauthenticated Public Build");
                    return;
#endif
                }

                if (File.Exists(OEDP) && Overlay == null) {
                    Overlay = new DotNetVM(OEDP);
                    Log("Overlay Enabled.", true);
                }

                if (File.Exists(TLDP) && TLIB == null) {
                    if (Ini.GetConfigStatus("MTL", "SourceLang", IniPath) == Ini.ConfigStatus.Ok) {
                        if (Ini.GetConfigStatus("MTL", "TargetLang", IniPath) == Ini.ConfigStatus.Ok) {
                            SourceLang = Ini.GetConfig("MTL", "SourceLang", IniPath, true);
                            TargetLang = Ini.GetConfig("MTL", "TargetLang", IniPath, true);
                            TLIB = new DotNetVM(TLDP);
                            Log("Machine Translation Enabled", true);
                        }
                    }
                }

                if (!File.Exists(TLMap) || Ini.GetConfig(CfgName, "Rebuild", IniPath, false).ToLower() == "true") {
                    Log("Unabled to load the {0}", false, TLMap);
                    bool ContainsSplitedList = Directory.GetFiles(BaseDir, Path.GetFileName(string.Format(TLMapSrcMsk, "*"))).Length != 0;
                    if (File.Exists(TLMapSrc) || ContainsSplitedList) {
                        Log("Compiling String Reloads, Please Wait...");
                        CompileStrMap();
                    } else {
                        Error("Can't Compile Strings because the Strings.lst has not found.");
                        Thread.Sleep(5000);
                        Environment.Exit(2);
                    }
                }

                LoadData();
                InstallIntroInjector();

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
                PreserveStackTrace(ex);
                throw ex;
            }
        }

        private static string GetDBNameById(long ID) {
            return DBNames[ID];
        }

        private static void WindowHook() {
            if (WindowHookRunning)
                return;

            WindowHookRunning = true;
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

        private static bool ProcessWindow(IntPtr Handler, int Parameters) {
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