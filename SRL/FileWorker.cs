using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;

namespace SRL
{
    partial class StringReloader
    {

        /// <summary>
        /// Read Lst file and return the content
        /// </summary>
        /// <param name="Path">Path to the LST file</param>
        /// <param name="In">Original Lines</param>
        /// <param name="Out">Target Lines</param>
        static void ReadDump(string Path, ref List<string> In, ref List<string> Out, bool IgnoreOutput = false, bool IgnoreMask = false)
        {
            using (TextReader Reader = File.OpenText(Path))
            {
                while (Reader.Peek() != -1)
                {
                    try
                    {
                        string L1 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");
                        string L2 = Reader.ReadLine().Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r");
                        if (IgnoreOutput)
                        {
                            In.Add(L1);
                            continue;
                        }

                        if (L2 != L1 || (IgnoreMask && IsMask(L1)))
                        {
                            if (!string.IsNullOrWhiteSpace(L2) || AllowEmpty)
                            {
                                if (!In.Contains(L1) || AllowDuplicates)
                                {
                                    In.Add(L1);
                                    Out.Add(L2);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
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
        internal static void AppendLst(string L1, string L2, string LstPath)
        {
            try
            {
                using (TextWriter Writer = File.AppendText(LstPath))
                {
                    try
                    {
                        Writer.WriteLine(L1.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));
                        Writer.WriteLine(L2.Replace("\n", "::BREAKLINE::").Replace("\r", "::RETURNLINE::"));

                    }
                    catch (Exception ex)
                    {
                        Error("Failed to append the string list, Reason:\n{0}", ex.Message);
                    }
                    Writer.Close();
                }
            }
            catch (Exception ex)
            {
                Error("Failed to append the string list, Reason:\n{0}", ex.Message);
            }
        }

        /// <summary>
        /// Dump a Mismatch if debugging
        /// </summary>
        /// <param name="String">Mismatched String</param>
        internal static void Missmatch(string String)
        {
            try
            {
                if (!String.IsDialog() && DumpStrOnly)
                    return;

                string Txt = SimplfyMatch(String);

                if (ContainsMissed(Txt) || InCache(Txt))
                    return;

                if (LastInput.EndsWith(String))
                    return;

                if (Ranges != null && DialogCheck)
                {
                    uint Miss = 0;
                    foreach (char c in Txt)
                    {
                        if (!InRange(c))
                            Miss++;
                    }
                    if (Miss >= Txt.Length - 3)
                        return;
                }

                AddMissed(Txt);
                Txt = TrimString(String);
                AppendLst(Txt, Txt, TLMapSrc + StrLstSufix);

            }
            catch { }
        }


        /// <summary>
        /// Decompile the Strings.srl
        /// </summary>
        /// <param name="TLMode">If true, Dump without the translation</param>
        static void DumpData(bool TLMode = false)
        {
            Log("Dumping Data...", true);
            if (File.Exists(TLMapSrc))
            {
                File.Delete(TLMapSrc);
            }
            if (File.Exists(CharMapSrc))
            {
                File.Delete(CharMapSrc);
            }
            using (StructReader Reader = new StructReader(TLMap))
            {
                SRLData3 Data = new SRLData3();
                Reader.ReadStruct(ref Data);

                if (Data.Databases.Length <= 1)
                {
                    for (uint i = 0; i < Data.Databases[0].Original.LongLength; i++)
                    {
                        string Str = Data.Databases[0].Original[i];
                        if (string.IsNullOrWhiteSpace(Str))
                            continue;

                        AppendLst(Str, TLMode ? Str : Data.Databases[0].Replace[i], TLMapSrc);
                    }
                }
                else
                {
                    foreach (SRLDatabase2 DataBase in Data.Databases)
                    {
                        for (uint i = 0; i < DataBase.Original.LongLength; i++)
                        {
                            string Str = DataBase.Original[i];
                            if (string.IsNullOrWhiteSpace(Str))
                                continue;

                            AppendLst(Str, TLMode ? Str : DataBase.Replace[i], string.Format(TLMapSrcMsk, DataBase.Name));
                        }
                    }
                }

                if (Data.OriLetters.LongLength + Data.UnkReps.LongLength != 0)
                {
                    Log("Dumping Char Reloads...", true);
                    using (TextWriter Output = File.CreateText(CharMapSrc))
                    {
                        for (uint i = 0; i < Data.OriLetters.LongLength; i++)
                        {
                            Output.WriteLine("{0}={1}", Data.OriLetters[i], Data.MemoryLetters[i]);
                        }
                        for (uint i = 0; i < Data.UnkReps.LongLength; i++)
                        {
                            Output.WriteLine("{0}=0x{1:X4}", Data.UnkReps[i], Data.UnkChars[i]);
                        }
                        Output.Close();
                    }
                }

                if (Data.OriLetters.LongLength != 0)
                {
                    Log("Dumping Replaces...", true);

                    for (uint i = 0; i < Data.OriLetters.LongLength; i++)
                    {
                        try
                        {
                            string L1 = Data.RepOri[i];
                            string L2 = Data.RepTrg[i];
                            AppendLst(L1, L2, ReplLst);
                        }
                        catch { }
                    }
                }

                if (Data.Version > 0)
                {
                    Log("Dumping Intros...", true);

                    SRLIntro Intros = new SRLIntro();
                    Reader.ReadStruct(ref Intros);

                    for (byte i = 0; i < Intros.Intros.Length; i++)
                    {
                        var Intro = Intros.Intros[i];
                        string sBitmap = string.Format(IntroMsk, i, "png");
                        string sSound = string.Format(IntroMsk, i, "wav");

                        File.WriteAllBytes(sBitmap, Intro.Bitmap);
                        if (Intro.HasSound)
                            File.WriteAllBytes(sSound, Intro.Wav);
                    }
                }

                Log("Data Dumped...", true);

                Reader.Close();
            }
        }
    }
}
