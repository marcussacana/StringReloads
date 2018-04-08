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
        static void ReadDump(string Path, ref List<string> In, ref List<string> Out, bool IgnoreOutput = true, bool IgnoreMask = false) {
            using (TextReader Reader = File.OpenText(Path)) {
                while (Reader.Peek() != -1) {
                    try {
                        string L1 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");
                        string L2 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");

                        if (L2 != L1 || (IgnoreMask && IsMask(L1))) {
                            if (IgnoreOutput || (!string.IsNullOrWhiteSpace(L2) && !In.Contains(L1))) {
                                In.Add(L1);
                                if (IgnoreOutput)
                                    Out.Add(L2);
                            }
                        }
                    } catch (Exception ex){
                        Warning("Read Dump Exception: {0}", ex.Message);
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
        /// Dump a Mismatch if debugging
        /// </summary>
        /// <param name="String">Mismatched String</param>
        internal static void Mismatch(string String) {
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
            TrimRangeMismatch = false;
            Unicode = false;
            MultipleDatabases = false;
            OverlayEnabled = false;
            NoReload = false;
            NoTrim = true;
            ReloadMaskParameters = false;

            SRLSettings Settings;
            OverlaySettings OverlaySettings;
            WordwrapSettings WordwrapSettings;
            AdvancedIni.FastOpen(out Settings, IniPath);
            AdvancedIni.FastOpen(out OverlaySettings, IniPath);
            AdvancedIni.FastOpen(out WordwrapSettings, IniPath);


            Log(Initialized ? "Reloading Settings..." : "Loading Settings...", true);

            if (Settings.InEncoding != null) {
                Log("Loading Read Encoding Config...", true);
                ReadEncoding = ParseEncodingName(Settings.InEncoding);
            }

            if (Settings.OutEncoding != null) {
                Log("Loading Write Encoding Config...", true);
                WriteEncoding = ParseEncodingName(Settings.OutEncoding);
                
                if (Debugging)
                    Console.OutputEncoding = WriteEncoding;
            }

            if (Settings.Wide) {
                Log("Wide Character Mode Enabled...", true);
                Unicode = true;
            }
            
            if (Settings.Multithread) {
                if (Initialized && !Multithread) {
                    Warning("The Multithread Settings Changed - Restart Required");
                } else {
                    Log("Multithreaded Game Support Enabled", true);
                    Multithread = true;
                }
            } else if (Initialized && Multithread) {
                Warning("The Multithread Settings Changed - Restart Required");
            }

            if (!string.IsNullOrWhiteSpace(Settings.DenyChars)) {
                Log("Custom Denied Chars List Loaded...", true);
                DenyChars = Settings.DenyChars;
            }

            if (Settings.TrimRangeMismatch) {
                Log("Trim Mismatch Ranges Enabled...", true);
                TrimRangeMismatch = true;
            }

            if (Settings.CachePointers) {
                Warning("Pointer Cache Enabled...", true);
                CachePointers = true;
            }

            if (Settings.FreeOnExit) {
                Warning("Memory Leak Prevention Enabled...", true);
                FreeOnExit = true;
            }

            if (Settings.NoDialogCheck) {
                Warning("Dialog Check Disabled...", true);
                DialogCheck = false;
            }

            if (Settings.LiteralMaskMatch) {
                Log("Literal Mask Matching Enabled...", true);
                LiteralMaskMatch = true;
            }
            if (Settings.MultiDatabase) {
                Log("Multidatabase's Matching Method Enabled...", true);
                MultipleDatabases = true;
            }

            if (Settings.WindowHook) {
                Log("Enabling Window Reloader...", true);
                new Thread(() => WindowHook()).Start();

                if (Settings.InvalidateWindow) {
                    Log("Invalidate Window Mode Enabled.", true);
                    InvalidateWindow = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(Settings.AcceptableRanges)) {
                LoadRanges();
            }

            if (Settings.DecodeFromInput) {
                Log("Enabling Character Reloader Decoding From Input...");
                DecodeCharactersFromInput = true;
            }

            if (Settings.NoTrim) {
                Log("Disabling Trim Service...", true);
                NoTrim = true;
            }

            if (Settings.ReloadMaskParameters) {
                Log("Enabling Mask Parameters Reloader...", true);
                ReloadMaskParameters = true;

                if (!Settings.Multithread) {
                    Warning("You can't use the PIPE service with the Mask Parameter Reloader Feature, Enabling Multithreaded Game Support...");
                    Settings.Multithread = true;
                    Multithread = true;
                }
            }

            if (Settings.NoReload) {
                NoReload = true;
                Warning("String Injection Disabled by User...");
            }

            if (OverlaySettings.Enable) {
                OverlayEnabled = true;
                ShowNonReloads = OverlaySettings.ShowNonReloaded;

                if (!File.Exists(OEDP))
                    Error("Can't Enabled the Overlay Because the Overlay.dll is missing.");
                else
                    Log("Overlay Allowed.", true);
            }

            if (!string.IsNullOrWhiteSpace(OverlaySettings.Padding)) {
                PaddingSeted = false;
                string Padding = OverlaySettings.Padding;
                foreach (string Parameter in Padding.Split('|')) {
                    try {
                        string Border = Parameter.Split(':')[0].Trim().ToLower();
                        int Value = int.Parse(Parameter.Split(':')[1].Trim());
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
                        Error("\"{0}\" Isn't a valid Padding Parameter", Parameter);
                    }
                }
            }

            if (Settings.LiveSettings) {
                if (SettingsWatcher == null) {
                    Log("Enabling Live Settings....", true);
                    SettingsWatcher = new Thread(() => {
                        DateTime Before = new FileInfo(IniPath).LastWriteTime;
                        while (true) {
                            DateTime Now = new FileInfo(IniPath).LastWriteTime;
                            if (Before != Now) {
                                Before = Now;
                                Thread.Sleep(500);
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

            if (Settings.AntiCrash) {
                if (!Initialized) {
                    Log("Enabling Crash Handler...", true);
                    System.Windows.Forms.Application.ThreadException += ProcessOver;
                    AppDomain.CurrentDomain.UnhandledException += ProcessOver;
                    AntiCrash = true;
                }
            }

            if (WordwrapSettings.Enabled) {
                Log("Wordwrap Enabled.", true);
                EnableWordWrap = true;
                MaxWidth = WordwrapSettings.Width;
                Monospaced = WordwrapSettings.Monospaced;
                FakeBreakLine = WordwrapSettings.FakeBreakLine;

                if (!Monospaced)
                    Font = new System.Drawing.Font(WordwrapSettings.FontName, WordwrapSettings.Size, WordwrapSettings.Bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            }

            if (!string.IsNullOrEmpty(Settings.GameLineBreaker)) {
                GameLineBreaker = Settings.GameLineBreaker;
                SpecialLineBreaker = true;
            }

            if (!string.IsNullOrWhiteSpace(Settings.MatchIgnore)) {
                Log("Using Custom Ignore List...", true);
                MatchDel = new string[0];
                foreach (string str in Settings.MatchIgnore.Split(','))
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

            if (!string.IsNullOrWhiteSpace(Settings.TrimChars)) {
                Log("Using Custom Trim List...", true);
                TrimChars = new string[0];
                foreach (string str in Settings.TrimChars.Split(',')) {
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

            if (!string.IsNullOrWhiteSpace(Settings.WorkDirectory)) {
                CustomDir = Settings.WorkDirectory.TrimStart(' ', '\\', '/').Replace("/", "\\");
                if (!CustomDir.EndsWith("\\"))
                    CustomDir += '\\';

                if (!Directory.Exists(BaseDir))
                    Directory.CreateDirectory(BaseDir);

                Log("Custom Directory Loaded", true);
            }

            Log("Settings Loaded.", true);

            if (Managed) {
                Log("Managed Mode Enabled, Enforcing Compatible Settings", true);
                //OverlayEnabled = false;
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
                            Warning("Range from {0} to {1} Conflited.", true, Range.Min, Range.Max);
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
