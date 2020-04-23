using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StringReloads.Engine
{
    public class LSTParser : IDisposable
    {
        public string Name { get; private set; }
        TextReader Reader;
        public LSTParser(string LSTPath) : this(File.OpenText(LSTPath), Path.GetFileName(LSTPath)) { }

        public LSTParser(TextReader Reader, string Name)
        {
            this.Reader = Reader;

            this.Name = Name.Split('.').First();

            const string NamePrefix = "strings-";
            if (this.Name.ToLowerInvariant().StartsWith(NamePrefix))
                this.Name = this.Name.Substring(NamePrefix.Length);

        }

        ~LSTParser() {
            Dispose();
        }

        public IEnumerable<LSTEntry> GetEntries()
        {
            ((StreamReader)Reader).BaseStream.Position = 0;
            long CurrentLine = -1;
            bool InComment = false;
            while (Reader.Peek() != -1)
            {
                CurrentLine++;
                string LineA = Reader.ReadLine();
                if (LineA.Contains("/*"))
                {
                    InComment = true;
                }
                if (LineA.Contains("*/") && InComment)
                {
                    int CommentPos = LineA.IndexOf("/*");
                    int DecommentPos = LineA.IndexOf("*/");
                    if (DecommentPos > CommentPos)
                    {
                        InComment = false;
                        continue;
                    }
                }
                if (InComment || LineA.StartsWith("//"))
                    continue;

                string LineB = Reader.ReadLine();

                LSTEntry Entry = new LSTEntry(LineA, LineB);
                if (Entry.OriginalFlags.HasFlag("AssertTranslation"))
                    Log.Error($"LST Translation Assertion Failed: Line {CurrentLine}; File {Name})");

                if (Entry.TranslationFlags.HasFlag("AssertMatch"))
                    Log.Error($"LST Match Assertion Failed: Line {CurrentLine + 1}; File {Name})");

                yield return Entry;
            }
        }

        public void Dispose() {
            Reader?.Close();
            Reader?.Dispose();
        }
    }


    public struct LSTEntry
    {
        public LSTEntry(string LineAB) : this(LineAB, LineAB) { }
        public LSTEntry(string LineA, string LineB, LSTFlag[] FlagsA, LSTFlag[] FlagsB)
        {
            OriginalLine = LineA;
            TranslationLine = LineB;
            OriginalFlags = FlagsA;
            TranslationFlags = FlagsB;
        }
        public LSTEntry(string LineA, string LineB)
        {
            if (string.IsNullOrEmpty(LineA) || string.IsNullOrEmpty(LineB))
            {
                OriginalLine = string.Empty;
                TranslationLine = string.Empty;
                TranslationFlags = new LSTFlag[0];
                OriginalFlags = new LSTFlag[0];
                return;
            }
            OriginalFlags = new LSTFlag[0];
            while (LineA.StartsWith("::"))
            {
                string FlagContent = LineA.Substring(2, LineA.IndexOf("::", 2) - 2);
                string Name = FlagContent.Split('-').First();
                string Value = null;

                if (FlagContent.Contains("-"))
                    Value = FlagContent.Substring(FlagContent.IndexOf("-") + 1);

                OriginalFlags = OriginalFlags.Union(new LSTFlag[] { new LSTFlag() { Name = Name, Value = Value } }).ToArray();

                LineA = LineA.Substring(LineA.IndexOf("::", 2) + 2);
            }

            TranslationFlags = new LSTFlag[0];
            while (LineB.StartsWith("::"))
            {
                string FlagContent = LineB.Substring(2, LineB.IndexOf("::", 2) - 2);
                string Name = FlagContent.Split('-').First().Trim();
                string Value = null;

                if (FlagContent.Contains("-"))
                    Value = FlagContent.Substring(FlagContent.IndexOf("-") + 1).Trim();

                TranslationFlags = TranslationFlags.Union(new LSTFlag[] { new LSTFlag() { Name = Name, Value = Value } }).ToArray();

                LineB = LineB.Substring(LineB.IndexOf("::", 2) + 2);
            }

            OriginalLine = LineA;
            TranslationLine = LineB;
        }

        public LSTFlag[] Flags => OriginalFlags.Union(TranslationFlags).ToArray();
        public LSTFlag[] OriginalFlags;
        public LSTFlag[] TranslationFlags;
        public string OriginalLine;
        public string TranslationLine;
    }

    public struct LSTFlag
    {
        public string Name;
        public string Value;
    }

    internal static partial class Extensions
    {
        internal static bool HasFlag(this LSTFlag[] Flags, string Name)
        {
            foreach (var Flag in Flags)
            {
                if (Flag.Name.ToLowerInvariant() == Name.ToLowerInvariant())
                    return true;
            }
            return false;
        }

        internal static string GetFlags(this LSTFlag[] Flags)
        {
            string Buffer = string.Empty;
            foreach (var Flag in Flags)
            {
                if (Flag.Value != null)
                {
                    Buffer += $"::{Flag.Name}-{Flag.Value}::";
                    continue;
                }

                Buffer += $"::{Flag.Name}::";
            }
            return Buffer;
        }
    }
}