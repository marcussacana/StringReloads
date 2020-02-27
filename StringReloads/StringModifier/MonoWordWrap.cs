using StringReloads.Engine;
using System;
using System.Linq;

namespace StringReloads.StringModifier
{
    class MonoWordWrap : IStringModifier
    {
        int? Size = null;
        int Width => (Size ?? Config.Default.Width);
        public MonoWordWrap(Main Main) {
            Main.OnFlagTriggered += (Flag, Cancel) =>
            {
                string Name = Flag.Name.ToUpperInvariant();
                if (Name != "MAXWIDTH" && Name != "WIDTH")
                    return;
                
                Cancel.Cancel = true;
                if (int.TryParse(Flag.Value, out int Size))
                    this.Size = Size;
                else
                    this.Size = null;
            };
        }

        public string Name => "MonoWordWrap";

        public bool CanRestore => true;

        public string Apply(string String)
        {
            int CurrentLength = 0;
            string Result = string.Empty;
            string CurrentWord = string.Empty;
            string BreakLine = Config.Default.BreakLine;


            for (int Index = 0; Index < String.Length; Index++) { 
                char Char = String[Index];
                if (Index + BreakLine.Length < String.Length && String.Substring(Index, BreakLine.Length) == BreakLine) {
                    Char = '\n';
                    Index += BreakLine.Length - 1;
                }
                switch (Char) {
                    case '\n':
                    case '\r':
                        Result += CurrentWord;
                        CurrentLength = 0;
                        CurrentWord = string.Empty;
                        goto default;
                    case ' ':
                        if (CurrentWord != string.Empty) {
                            if (CurrentLength + CurrentWord.Length > Width) {
                                Result += BreakLine;
                                Result += CurrentWord;
                                CurrentLength = CurrentWord.Length;
                                CurrentWord = string.Empty;
                            } else {
                                Result += CurrentWord;
                                CurrentLength += CurrentWord.Length;
                            }
                            break;
                        }
                        goto default;
                    default:
                        CurrentWord += Char;
                        break;
                }
            }

            return Result;
        }

        public string Restore(string String)
        {
            return String.Replace(Config.Default.BreakLine, " ");
        }
    }
}
