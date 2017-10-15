using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SRL {
    static partial class StringReloader {

        /// <summary>
        /// Build the String.srl
        /// </summary>
        internal static void CompileStrMap() {
            var In = new List<string>();
            var Out = new List<string>();
            var COri = new List<char>();
            var CFak = new List<char>();
            var UOri = new List<char>();
            var UErr = new List<ushort>();
            var ROri = new List<string>();
            var RNew = new List<string>();
            Log("Generating String Reload Database...");

            //Splited String Dump
            uint Cnt = 0;
            while (File.Exists(string.Format(TLMapSrcMsk, ++Cnt))) {
                string TMS = string.Format(TLMapSrcMsk, Cnt);
                ReadDump(TMS, ref In, ref Out);
                Log("{0} Found, Importing...", false, Path.GetFileName(TMS));
            }

            if (File.Exists(TLMapSrc)) {
                ReadDump(TLMapSrc, ref In, ref Out);
                Log("{0} Found, Importing...", false, Path.GetFileName(TLMapSrc));
            }

            Log("Database Generated.");
            if (File.Exists(CharMapSrc)) {
                Log("Compiling Char Reloads...");
                using (TextReader Reader = File.OpenText(CharMapSrc)) {
                    while (Reader.Peek() >= 0) {
                        string line = Reader.ReadLine();
                        if (line.Length == 3 && line[1] == '=') {
                            char cOri = line[0];
                            char cFak = line[2];
                            COri.Add(cOri);
                            CFak.Add(cFak);
                        }
                        if (line.Contains("0x") && line.Contains('=')) {
                            string hex = line.Split('=')[1].Split('x')[1];
                            ushort Val = ushort.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                            UOri.Add(line[0]);
                            UErr.Add(Val);
                        }
                    }
                    Reader.Close();
                }
            }

            if (File.Exists(ReplLst)) {
                Log("Compiling Replace List...");
                ReadDump(ReplLst, ref ROri, ref RNew);
            }

            Log("Building String Reloads...");
            SRLData Data = new SRLData() {
                Signature = "SRL",
                Original = In.ToArray(),
                Replace = Out.ToArray(),
                OriLetters = COri.ToArray(),
                MemoryLetters = CFak.ToArray(),
                UnkChars = UErr.ToArray(),
                UnkReps = UOri.ToArray(),
                RepOri = ROri.ToArray(),
                RepTrg = RNew.ToArray()
            };

            if (File.Exists(TLMap))
                File.Delete(TLMap);

            StructWriter Writer = new StructWriter(TLMap);
            Writer.WriteStruct(ref Data);
            Writer.Close();

            Log("Builded Successfully.");
        }

        
        /// <summary>
        /// Load the String.srl Data
        /// </summary>
        static void LoadData() {
            Log("Initializing String Reloads...", true);
            try {
                StartPipe();
                StructReader Reader = new StructReader(TLMap);
                if (Reader.PeekInt() == 0x43424C54) {
                    TLBCParser(Reader);
                    return;
                }
                if (Reader.PeekInt() != 0x4C5253) {
                    Log("Failed to Initialize - Corrupted Data");
                    Thread.Sleep(3000);
                    Environment.Exit(2);
                }
                var Data = new SRLData();
                Reader.ReadStruct(ref Data);
                Reader.Close();

                Log("Processing Char Reloads... 1/2", true);
                CharRld = new Dictionary<ushort, char>();
                for (uint i = 0; i < Data.OriLetters.LongLength; i++) {
                    char cOri = Data.OriLetters[i];
                    char cPrx = Data.MemoryLetters[i];
                    if (!CharRld.ContainsKey(cPrx)) {
                        CharRld.Add(cPrx, cOri);
                        AppendArray(ref Replaces, cOri.ToString());
                        AppendArray(ref Replaces, cPrx.ToString());
                    }
                }

                Log("Processing Char Reloads... 2/2", true);
                UnkRld = new Dictionary<ushort, char>();
                for (uint i = 0; i < Data.UnkChars.LongLength; i++) {
                    ushort c = Data.UnkChars[i];
                    if (!UnkRld.ContainsKey(c)) {
                        UnkRld.Add(c, Data.UnkReps[i]);
                    }
                }

                Log("Chars Reloads Initialized, Total entries: {0} + {1}", true, UnkRld.Count, CharRld.Count);
                Log("Processing String Reloads...", true);
                StrRld = new Dictionary<string, string>();
                for (uint i = 0; i < Data.Original.LongLength; i++) {
                    Application.DoEvents();
                    string str = SimplfyMatch(Data.Original[i]);
                    if (!ContainsKey(str))
                        AddEntry(str, ReplaceChars(Data.Replace[i]));
                }

                Log("String Reloads Initialized.", true);
                Log("Initializing Replaces...", true);
                for (uint i = 0; i < Data.RepOri.LongLength; i++) {
                    AppendArray(ref Replaces, Data.RepOri[i]);
                    AppendArray(ref Replaces, Data.RepTrg[i]);
                }
                Log("Loading Complete.", true);
            } catch (Exception ex) {
                Log("Failed to Execute: {0}\n=========\n{1}", false, ex.Message, ex.StackTrace);
                Thread.Sleep(3000);
                Environment.Exit(2);
            }
        }

        /// <summary>
        /// Convert TLBot Cache to SRL format
        /// </summary>
        /// <param name="Reader">Input Stream</param>
        private static void TLBCParser(StructReader Reader) {
            Log("TLBot Cache detected... Rebuilding...");
            var Cache = new TLBC();
            Reader.ReadStruct(ref Cache);
            Reader.Close();

            if (File.Exists(TLMapSrc))
                File.Delete(TLMapSrc);
            using (TextWriter Writer = File.CreateText(TLMapSrc)) {
                for (uint i = 0; i < Cache.Original.Length; i++) {
                    Writer.WriteLine(Cache.Original[i].Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
                    Writer.WriteLine(Cache.Replace[i].Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
                }
                Writer.Close();
            }
            File.Delete(TLMap);
            Log("Restarting...");
            Init();
        }
    }
}
