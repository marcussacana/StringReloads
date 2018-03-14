using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SRL {
    partial class StringReloader {

        /// <summary>
        /// Read Lst file and return the content
        /// </summary>
        /// <param name="Path">Path to the LST file</param>
        /// <param name="In">Original Lines</param>
        /// <param name="Out">Target Lines</param>
        static void ReadDump(string Path, ref List<string> In, ref List<string> Out, bool IgnoreOutput = true) {
            using (TextReader Reader = File.OpenText(Path)) {
                while (Reader.Peek() != -1) {
                    try {
                        string L1 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");
                        string L2 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");
                        if ((L2 != L1 && !string.IsNullOrWhiteSpace(L2) && !In.Contains(L1)) || IgnoreOutput) {
                            In.Add(L1);
                            if (IgnoreOutput)
                                Out.Add(L2);
                        }
                    } catch {

                    }
                }
                Reader.Close();
            }
        }
        /// <summary>
        /// Append a Entry to a LST File
        /// </summary>
        /// <param name="L1">Original</param>
        /// <param name="L2">Result</param>
        /// <param name="LstPath">Path to the LST File</param>
        internal static void AppendLst(string L1, string L2, string LstPath) {
            try {
                using (TextWriter Writer = File.AppendText(LstPath)) {
                    try {
                        Writer.WriteLine(L1.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));
                        Writer.WriteLine(L2.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));

                    } catch (Exception ex) {
                        Error("Failed to append the string list, Reason:\n{0}", ex.Message);
                    }
                    Writer.Close();
                }
            } catch (Exception ex) {
                Error("Failed to append the string list, Reason:\n{0}", ex.Message);
            }
        }

        /// <summary>
        /// Dump a missmatch if debugging
        /// </summary>
        /// <param name="String">Missmatched String</param>
        internal static void MissMatch(string String) {
            try {
                if (!IsDialog(String) && DumpStrOnly)
                    return;

                string Txt = SimplfyMatch(String);

                if (ContainsMissed(Txt) || InCache(Txt))
                    return;

                if (LastInput.EndsWith(String))
                    return;

                if (Ranges != null && DialogCheck) {
                    uint Miss = 0;
                    foreach (char c in Txt) {
                        if (!InRange(c))
                            Miss++;
                    }
                    if (Miss >= Txt.Length - 3)
                        return;
                }

                AddMissed(Txt);
                Txt = TrimString(String);
                AppendLst(Txt, Txt, TLMapSrc + StrLstSufix);

            } catch { }
        }

        /// <summary>
        /// Load All Configs from the INI file.
        /// </summary>
        private static void LoadConfig() {
            SpecialLineBreaker = false;
            EnableWordWrap = false;
            AntiCrash = false;
            DecodeCharactersFromInput = false;
            InvalidateWindow = false;
            LiteralMaskMatch = false;
            DialogCheck = true;
            FreeOnExit = false;
            CachePointers = false;
            TrimRangeMissmatch = false;
            Unicode = false;
            MultipleDatabases = false;
            OverlayEnabled = false;
            NoReload = true;

            Log(Initialized ? "Reloading Settings..." : "Loading Settings...", true);

            if (Ini.GetConfigStatus(CfgName, "InEncoding;ReadEncoding;Encoding", IniPath) == Ini.Status.Ok) {
                Log("Loading Read Encoding Config...", true);
                ReadEncoding = ParseEncodingName(Ini.GetConfig(CfgName, "InEncoding;ReadEncoding;Encoding", IniPath, true));
            }

            if (Ini.GetConfigStatus(CfgName, "OutEncoding;WriteEncoding;Encoding", IniPath) == Ini.Status.Ok) {
                Log("Loading Write Encoding Config...", true);
                WriteEncoding = ParseEncodingName(Ini.GetConfig(CfgName, "OutEncoding;WriteEncoding;Encoding", IniPath, true));
                
                if (Debugging)
                    Console.OutputEncoding = WriteEncoding;
            }

            if (Ini.GetConfig(CfgName, "Wide;Unicode", IniPath, false).ToLower() == "true") {
                Log("Wide Character Mode Enabled...", true);
                Unicode = true;
            }
            
            if (Ini.GetConfig(CfgName, "Multithread;DisablePipe", IniPath, false).ToLower() == "true") {
                if (Initialized && !Multithread) {
                    Warning("The Multithread Settings Changed - Restart Required");
                } else {
                    Log("Multithread Support Enabled", true);
                    Multithread = true;
                }
            } else if (Initialized && Multithread) {
                Warning("The Multithread Settings Changed - Restart Required");
            }

            if (Ini.GetConfig(CfgName, "DenyChars;NoChars", IniPath) != string.Empty) {
                Log("Custom Denied Chars List Loaded...", true);
                DenyChars = Ini.GetConfig(CfgName, "DenyChars;NoChars", IniPath);
            }

            if (Ini.GetConfig(CfgName, "TrimRangeMissmatch;TrimRange", IniPath, false).ToLower() == "true") {
                Log("Trim missmatch Ranges Enabled...", true);
                TrimRangeMissmatch = true;
            }

            if (Ini.GetConfig(CfgName, "CachePointers;CachePointer;ReusePointer;ReusePointers", IniPath, false).ToLower() == "true") {
                Warning("Pointer Cache Enabled...", true);
                CachePointers = true;
            }

            if (Ini.GetConfig(CfgName, "FreeOnExit;FreePointers;FreeMemory", IniPath, false).ToLower() == "true") {
                Warning("Memory Leak Prevention Enabled...", true);
                FreeOnExit = true;
            }

            if (Ini.GetConfig(CfgName, "NoDiagCheck;DisableDiagCheck;DisableDialogCheck", IniPath, false).ToLower() == "true") {
                Warning("Dialog Check Disabled...", true);
                DialogCheck = false;
            }

            if (Ini.GetConfig(CfgName, "LiteralMask;MaskLiteralMatch;MaskMatch", IniPath, false).ToLower() == "true") {
                Log("Literal Mask Matching Enabled...", true);
                LiteralMaskMatch = true;
            }
            if (Ini.GetConfig(CfgName, "MultiDatabase;MultiDB;SplitDatabase;SplitDB", IniPath, false).ToLower() == "true") {
                Log("Multidatabase's Matching Method Enabled...", true);
                MultipleDatabases = true;
            }

            if (Ini.GetConfig(CfgName, "WindowHook;WindowReloader", IniPath, false).ToLower() == "true") {
                Log("Enabling Window Reloader...", true);
                new Thread(() => WindowHook()).Start();

                if (Ini.GetConfig(CfgName, "Invalidate;RedrawWindow", IniPath, false).ToLower() == "true") {
                    Log("Invalidate Window Mode Enabled.", true);
                    InvalidateWindow = true;
                }
            }

            if (Ini.GetConfigStatus(CfgName, "AcceptableRanges;AcceptableRange;ValidRange;ValidRanges", IniPath) == Ini.Status.Ok) {
                LoadRanges();
            }

            if (Ini.GetConfig(CfgName, "DecodeInputRemap;DecodeCharacterRemapFromInput;DecodeRemapChars", IniPath).ToLower() == "true") {
                DecodeCharactersFromInput = true;
            }

            if (Ini.GetConfig(CfgName, "ReadOnly;NoInjection;DisableReloader;NoReload", IniPath).ToLower() == "true") {
                NoReload = true;
                Warning("String Injection Disabled by User.");
            }

            if (Ini.GetConfig("Overlay", "EnableOverlay;Enabled;Enable;ShowOverlay", IniPath).ToLower() == "true") {
                OverlayEnabled = true;
                if (!File.Exists(OEDP))
                    Error("Can't Enabled the Overlay Because the Overlay.dll is missing.");
                else
                    Log("Overlay Allowed.", true);
            }

            if (Ini.GetConfigStatus("Overlay", "Padding", IniPath) == Ini.Status.Ok) {
                PaddingSeted = false;
                string Padding = Ini.GetConfig("Overlay", "Padding", IniPath, true);
                foreach (string Paramter in Padding.Split('|')) {
                    try {
                        string Border = Paramter.Split(':')[0].Trim().ToLower();
                        int Value = int.Parse(Paramter.Split(':')[1].Trim());
                        switch (Border) {
                            case "top":
                                OPaddingTop = Value;
                                break;
                            case "bottom":
                                OPaddinBottom = Value;
                                break;
                            case "rigth":
                                OPaddingRigth = Value;
                                break;
                            case "left":
                                OPaddinLeft = Value;
                                break;
                            default:
                                Error("\"{0}\" Isn't a valid Padding Border", Border);
                                break;
                        }
                    } catch {
                        Error("\"{0}\" Isn't a valid Padding Paramter", Paramter);
                    }
                }
            }

            if (Ini.GetConfig(CfgName, "LiveSettings;KeepSettingsUpdate;ReloadSettings", IniPath).ToLower() == "true") {
                if (SettingsWatcher == null) {
                    Log("Enabling Live Settings....", true);
                    SettingsWatcher = new Thread(() => {
                        DateTime Before = new FileInfo(IniPath).LastWriteTime;
                        while (true) {
                            DateTime Now = new FileInfo(IniPath).LastWriteTime;
                            if (Before != Now) {
                                Before = Now;
                                LoadConfig();
                            }
                            Thread.Sleep(500);
                        }
                    });
                    SettingsWatcher.Start();
                }
            } else if (SettingsWatcher != null) {
                SettingsWatcher.Abort();
                SettingsWatcher = null;
            }

            if (Ini.GetConfig(CfgName, "AntiCrash;CrashHandler", IniPath, false).ToLower() == "true") {
                if (!Initialized) {
                    Log("Enabling Crash Handler...", true);
                    System.Windows.Forms.Application.ThreadException += ProcessOver;
                    AppDomain.CurrentDomain.UnhandledException += ProcessOver;
                    AntiCrash = true;
                }
            }

            if (Ini.GetConfig("WordWrap", "Enable;Enabled", IniPath, false).ToLower() == "true") {
                Log("Wordwrap Enabled.", true);
                EnableWordWrap = true;
                string Width = Ini.GetConfig("WordWrap", "MaxWidth;Width;Length", IniPath, true);
                string Size = Ini.GetConfig("WordWrap", "Size;FontSize", IniPath, false);
                string FontName = Ini.GetConfig("WordWrap", "Face;FaceName;Font;FontName;FamilyName", IniPath, false);
                bool Bold = Ini.GetConfig("WordWrap", "Bold", IniPath, false) == "true";
                Monospaced = Ini.GetConfig("WordWrap", "Monospaced;FixedSize;FixedLength", IniPath, false).ToLower() == "true";

                if (!Monospaced) {
                    float FSize = float.Parse(Size);
                    Font = new System.Drawing.Font(FontName, FSize, Bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                }
                MaxWidth = uint.Parse(Width);
            }

            if (!string.IsNullOrEmpty(Ini.GetConfig(CfgName, "BreakLine", IniPath, false))) {
                GameLineBreaker = Ini.GetConfig(CfgName, "BreakLine", IniPath, false);
                SpecialLineBreaker = true;
            }

            string ExtraSimplify = Ini.GetConfig(CfgName, "MatchIgnore;IgnoreMatchs", IniPath, false);
            if (!string.IsNullOrWhiteSpace(ExtraSimplify)) {
                Log("Using Custom Ignore List...", true);
                MatchDel = new string[0];
                foreach (string str in ExtraSimplify.Split(','))
                    if (str.Trim().StartsWith("0x")) {
                        string Hex = str.Trim();
                        Hex = Hex.Substring(2, Hex.Length - 2);
                        byte[] Buffer = new byte[Hex.Length / 2];
                        for (int i = 0; i < Hex.Length / 2; i++) {
                            Buffer[i] = Convert.ToByte(Hex.Substring(i, 2), 16);
                        }
                        string Del = System.Text.Encoding.UTF8.GetString(Buffer);
                        AppendArray(ref MatchDel, Del, true);
                    } else
                        AppendArray(ref MatchDel, str.Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r"), true);
            }

            string ExtraTrim = Ini.GetConfig(CfgName, "TrimChars;TrimStrings", IniPath, false);
            if (!string.IsNullOrWhiteSpace(ExtraTrim)) {
                Log("Using Custom Trim List...", true);
                TrimChars = new string[0];
                foreach (string str in ExtraTrim.Split(',')) {
                    if (str.Trim().StartsWith("0x")) {
                        string Hex = str.Trim();
                        Hex = Hex.Substring(2, Hex.Length - 2);
                        byte[] Buffer = new byte[Hex.Length / 2];
                        for (int i = 0; i < Hex.Length / 2; i++) {
                            Buffer[i] = Convert.ToByte(Hex.Substring(i, 2), 16);
                        }
                        string Trim = System.Text.Encoding.UTF8.GetString(Buffer);
                        AppendArray(ref TrimChars, Trim, true);
                    } else
                        AppendArray(ref TrimChars, str.Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r"), true);
                }
            }

            Log("Settings Loaded.", true);

            if (Managed) {
                Log("Managed Mode Enabled, Enforcing Compatible Settings", true);
                OverlayEnabled = false;
                WriteEncoding = ReadEncoding = System.Text.Encoding.Unicode;
#if TRACE
                Multithread = true;
#endif
                if (Debugging)
                    LogFile = true;
            }
        }


        /// <summary>
        /// Load/Compile all Acceptable Character Ranges by the user config.
        /// </summary>
        private static void LoadRanges() {
            Ranges = new List<Range>();
            string RangeList = Ini.GetConfig(CfgName, "AcceptableRanges;AcceptableRange;ValidRange;ValidRanges", IniPath, true);
            for (int i = 0; i < RangeList.Length - 1;) {
                char c = RangeList[i];
                char c2 = RangeList[i + 1];
                if (c2 == '-') {
                    char c3 = RangeList[i + 2];
                    if (c <= c3) {
                        Range Range = new Range() {
                            Min = c,
                            Max = c3
                        };
                        if (!Ranges.Contains(Range)) {
                            Ranges.Add(Range);
                            Log("Range from {0} to {1} Added.", true, Range.Min, Range.Max);
                        } else {
                            Log("Range from {0} to {1} Conflited.", true, Range.Min, Range.Max);
                        }
                    }
                    i += 3;
                } else {
                    Range Range = new Range() {
                        Min = c,
                        Max = c
                    };
                    if (!Ranges.Contains(Range)) {
                        Ranges.Add(Range);
                        Log("Range from {0} to {1} Added.", true, Range.Min, Range.Max);
                    } else {
                        Log("Range from {0} to {1} Conflited.", true, Range.Min, Range.Max);
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// Decompile the Strings.srl
        /// </summary>
        /// <param name="TLMode">If true, Dump without the translation</param>
        static void DumpData(bool TLMode = false) {
            Log("Dumping Data...", true);
            if (File.Exists(TLMapSrc)) {
                File.Delete(TLMapSrc);
            }
            if (File.Exists(CharMapSrc)) {
                File.Delete(CharMapSrc);
            }
            SRLData2 Data = new SRLData2();
            StructReader Reader = new StructReader(TLMap);
            Reader.ReadStruct(ref Data);
            Reader.Close();

            if (Data.Databases.Length <= 1) {
                for (uint i = 0; i < Data.Databases[0].Original.LongLength; i++) {
                    string Str = Data.Databases[0].Original[i];
                    if (string.IsNullOrWhiteSpace(Str))
                        continue;

                    AppendLst(Str, TLMode ? Str : Data.Databases[0].Replace[i], TLMapSrc);
                }
            } else {
                int ID = 1;
                foreach (SRLDatabase DataBase in Data.Databases) {
                    for (uint i = 0; i < DataBase.Original.LongLength; i++) {
                        string Str = DataBase.Original[i];
                        if (string.IsNullOrWhiteSpace(Str))
                            continue;

                        AppendLst(Str, TLMode ? Str : DataBase.Replace[i], string.Format(TLMapSrcMsk, ID));
                    }
                    ID++;
                }
            }

            if (Data.OriLetters.LongLength + Data.UnkReps.LongLength != 0) {
                Log("Dumping Char Reloads...", true);
                using (TextWriter Output = File.CreateText(CharMapSrc)) {
                    for (uint i = 0; i < Data.OriLetters.LongLength; i++) {
                        Output.WriteLine("{0}={1}", Data.OriLetters[i], Data.MemoryLetters[i]);
                    }
                    for (uint i = 0; i < Data.UnkReps.LongLength; i++) {
                        Output.WriteLine("{0}=0x{1:X4}", Data.UnkReps[i], Data.UnkChars[i]);
                    }
                    Output.Close();
                }
            }

            if (Data.OriLetters.LongLength != 0) {
                Log("Dumping Replaces...", true);

                for (uint i = 0; i < Data.OriLetters.LongLength; i++) {
                    try {
                        string L1 = Data.RepOri[i];
                        string L2 = Data.RepTrg[i];
                        AppendLst(L1, L2, ReplLst);
                    } catch { }
                }
            }

            Log("Data Dumped...", true);
        }
    }
}
