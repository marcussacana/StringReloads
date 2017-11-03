using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;

namespace SRL {
    partial class StringReloader {

        /// <summary>
        /// Read Lst file and return the content
        /// </summary>
        /// <param name="Path">Path to the LST file</param>
        /// <param name="In">Original Lines</param>
        /// <param name="Out">Target Lines</param>
        static void ReadDump(string Path, ref List<string> In, ref List<string> Out) {
            using (TextReader Reader = File.OpenText(Path)) {
                while (Reader.Peek() != -1) {
                    try {
                        string L1 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");
                        string L2 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");
                        if (L2 != L1 && !string.IsNullOrWhiteSpace(L2) && !In.Contains(L1)) {
                            In.Add(L1);
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
                    Writer.WriteLine(L1.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));
                    Writer.WriteLine(L2.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));
                    Writer.Close();
                }
            } catch (Exception ex) {
                Log("Failed to append the string list, Reason:\n{0}", true, ex.Message);
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
                using (TextWriter Writer = File.AppendText(TLMapSrc + StrLstSufix)) {
                    string Txt = SimplfyMatch(String);
                    if (ContainsMissed(Txt) || Replys.Contains(Txt) || Txt.Length <= 2)
                        return;
                    if ((Replys.Count > 0 && Replys[ReplyPtr - 1].EndsWith(Txt)) || LastInput.EndsWith(String))
                        return;

                    if (Ranges != null) {
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
                    Writer.WriteLine(Txt.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));
                    Writer.WriteLine(Txt.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));
                    Writer.Close();
                }
            } catch (Exception ex) {
                Log("Failed to dump the string, Reason:\n{0}", true, ex.Message);
            }
        }

        /// <summary>
        /// Load All Configs from the INI file.
        /// </summary>
        private static void LoadConfig() {
            Log("Loading Settings...", true);

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

            if (Ini.GetConfig(CfgName, "TrimRangeMissmatch;TrimRange", IniPath, false).ToLower() == "true") {
                Log("Trim missmatch Ranges Enabled...", true);
                TrimRangeMissmatch = true;
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


            if (Ini.GetConfigStatus(CfgName, "AcceptableRanges;AcceptableRange;ValidRange;ValidRanges", IniPath) == Ini.Status.Ok) {
                LoadRanges();
            }


            if (!string.IsNullOrEmpty(Ini.GetConfig(CfgName, "BreakLine", IniPath, false))) {
                GameLineBreaker = Ini.GetConfig(CfgName, "BreakLine", IniPath, false);
                SpecialLineBreaker = true;
            }

            string ExtraSimplify = Ini.GetConfig(CfgName, "MatchIgnore;IgnoreMatchs", IniPath, false);
            if (!string.IsNullOrWhiteSpace(ExtraSimplify)) {
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
        }


        /// <summary>
        /// Load/Compile all Acceptable Character Ranges by the user config.
        /// </summary>
        private static void LoadRanges() {
            Ranges = new System.Collections.Generic.List<Range>();
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
            SRLData Data = new SRLData();
            using (TextWriter Output = File.CreateText(TLMapSrc)) {
                StructReader Reader = new StructReader(TLMap);
                Reader.ReadStruct(ref Data);
                Reader.Close();
                for (uint i = 0; i < Data.Original.LongLength; i++) {
                    string Str = Data.Original[i];
                    if (string.IsNullOrWhiteSpace(Str))
                        continue;
                    if (TLMode) {
                        Output.WriteLine(Str.Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
                        Output.WriteLine(Str.Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
                    } else {
                        Output.WriteLine(Str.Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
                        Output.WriteLine(Data.Replace[i].Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
                    }
                }
                Output.Close();
            }
            using (TextWriter Output = File.CreateText(CharMapSrc)) {
                for (uint i = 0; i < Data.OriLetters.LongLength; i++) {
                    Output.WriteLine("{0}={1}", Data.OriLetters[i], Data.MemoryLetters[i]);
                }
                for (uint i = 0; i < Data.UnkReps.LongLength; i++) {
                    Output.WriteLine("{0}=0x{1:X4}", Data.UnkReps[i], Data.UnkChars[i]);
                }
                Output.Close();
            }
            using (TextWriter Output = File.CreateText(ReplLst)) {
                for (uint i = 0; i < Data.OriLetters.LongLength; i++) {
                    Output.WriteLine(Data.RepOri[i]);
                    Output.WriteLine(Data.RepTrg[i]);
                }
                Output.Close();
            }
            Log("Data Dumped...", true);
        }
    }
}
