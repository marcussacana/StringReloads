using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.StringModifier;

using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace StringReloads
{
    public static class Extensions
    {
        public static bool ToBoolean(this string Value)
        {
            if (Value == null)
                return false;

            Value = Value.ToLowerInvariant();
            switch (Value)
            {
                case "1":
                case "true":
                case "yes":
                case "on":
                case "enable":
                case "enabled":
                    return true;
            }
            return false;
        }

        internal static Log.LogLevel ToLogLevel(this string ValueStr)
        {
            switch (ValueStr.ToLowerInvariant())
            {
                case "t":
                case "tra":
                case "trc":
                case "trace":
                    return Log.LogLevel.Trace;
                case "d":
                case "deb":
                case "dbg":
                case "debug":
                    return Log.LogLevel.Debug;
                case "i":
                case "inf":
                case "info":
                case "information":
                    return Log.LogLevel.Information;
                case "w":
                case "war":
                case "warn":
                case "warning":
                    return Log.LogLevel.Warning;
                case "e":
                case "err":
                case "erro":
                case "error":
                    return Log.LogLevel.Error;
                case "c":
                case "cri":
                case "crit":
                case "critical":
                    return Log.LogLevel.Critical;
            }

            return (Log.LogLevel)ValueStr.ToInt32();
        }

        public static uint ToUInt32(this string Value)
        {
            if (Value == null)
                return 0;

            if (Value.StartsWith("0x") && uint.TryParse(Value.Substring(2), NumberStyles.HexNumber, null, out uint Val))
                return Val;

            if (uint.TryParse(Value, out Val))
                return Val;

            return 0;
        }

        public static int ToInt32(this string Value)
        {
            if (Value == null)
                return 0;

            if (Value.StartsWith("0x") && int.TryParse(Value.Substring(2), NumberStyles.HexNumber, null, out int Val))
                return Val;

            if (int.TryParse(Value, out Val))
                return Val;

            return 0;
        }

        public static ulong ToUInt64(this string Value)
        {
            if (Value == null)
                return 0;

            if (Value.StartsWith("0x") && ulong.TryParse(Value.Substring(2), NumberStyles.HexNumber, null, out ulong Val))
                return Val;

            if (ulong.TryParse(Value, out Val))
                return Val;

            return 0;
        }

        public static long ToInt64(this string Value)
        {
            if (Value == null)
                return 0;

            if (Value.StartsWith("0x") && long.TryParse(Value.Substring(2), NumberStyles.HexNumber, null, out long Val))
                return Val;

            if (long.TryParse(Value, out Val))
                return Val;

            return 0;
        }

        public static Encoding ToEncoding(this string Value)
        {
            if (int.TryParse(Value, out int CP))
                return Encoding.GetEncoding(CP);

            return Value.ToLowerInvariant() switch
            {
                "sjis" => Encoding.GetEncoding(932),
                "shiftjis" => Encoding.GetEncoding(932),
                "shift-jis" => Encoding.GetEncoding(932),
                "unicode" => Encoding.Unicode,
                "utf16" => Encoding.Unicode,
                "utf16be" => Encoding.BigEndianUnicode,
                "utf16wb" => new UnicodeEncoding(false, true),
                "utf16wbbe" => new UnicodeEncoding(true, true),
                "utf16bewb" => new UnicodeEncoding(true, true),
                "utf8" => Encoding.UTF8,
                "utf8wb" => new UTF8Encoding(true),
                "utf7" => Encoding.UTF7,
                _ => Encoding.GetEncoding(Value)
            };
        }

        public static string Unescape(this string String)
        {
            return Escape.Default.Restore(String);
        }



        static string[] DenyList = Config.Default.Filter.DenyList.Unescape().Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        static string[] IgnoreList = Config.Default.Filter.IgnoreList.Unescape().Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        static Quote[] Quotes = Config.Default.Filter.QuoteList.Unescape().Split('\n')
                    .Where(x => x.Length == 2)
                    .Select(x => new Quote() { Start = x[0], End = x[1] }).ToArray();
        public static bool IsDialogue(this string String, int? Caution = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(String))
                    return false;

                if (Config.Default.Filter.UseDB && EntryPoint.SRL.HasMatch(String))
                    return true;

                string Str = String.Trim();
                foreach (string Ignore in IgnoreList)
                    Str = Str.Replace(Ignore, "");

                foreach (string Deny in DenyList) {
                    if (Str.ToLower().Contains(Deny.ToLower()))
                        return false;
                }

                Str = Str.Replace(Config.Default.BreakLine, "\n");


                if (string.IsNullOrWhiteSpace(Str))
                    return false;

                if (CharacterRanges.TotalMissmatch(Str, Config.Default.Filter.AcceptableRange) > 0)
                    return false;

                string[] Words = Str.Split(' ');

                char[] PontuationJapList = new char[] { '。', '？', '！', '…', '、', '―' };
                char[] SpecialList = new char[] { '_', '=', '+', '#', ':', '$', '@' };
                char[] PontuationList = new char[] { '.', '?', '!', '…', ',' };
                int Spaces = Str.Where(x => x == ' ' || x == '\t').Count();
                int Pontuations = Str.Where(x => PontuationList.Contains(x)).Count();
                int WordCount = Words.Where(x => x.Length >= 2 && !string.IsNullOrWhiteSpace(x)).Count();
                int Specials = Str.Where(x => char.IsSymbol(x)).Count();
                Specials += Str.Where(x => char.IsPunctuation(x)).Count() - Pontuations;
                int SpecialsStranges = Str.Where(x => SpecialList.Contains(x)).Count();

                int Uppers = Str.Where(x => char.IsUpper(x)).Count();
                int Latim = Str.Where(x => x >= 'A' && x <= 'z').Count();
                int Numbers = Str.Where(x => x >= '0' && x <= '9').Count();
                int NumbersJap = Str.Where(x => x >= '０' && x <= '９').Count();
                int JapChars = Str.Where(x => (x >= '、' && x <= 'ヿ') || (x >= '｡' && x <= 'ﾝ')).Count();
                int Kanjis = Str.Where(x => x >= '一' && x <= '龯').Count();


                bool IsCaps = Str.ToUpper() == Str;
                bool IsJap = JapChars + Kanjis > Latim / 2;


                //More Points = Don't Looks a Dialogue
                //Less Points = Looks a Dialogue
                int Points = 0;

                if (Str.Length > 4)
                {
                    string ext = Str.Substring(Str.Length - 4, 4);
                    try
                    {
                        if (System.IO.Path.GetExtension(ext).Trim('.').Length == 3)
                            Points += 2;
                    }
                    catch { }
                }

                bool BeginQuote = false;
                Quote? LineQuotes = null;
                foreach (Quote Quote in Quotes)
                {
                    BeginQuote |= Str.StartsWith(Quote.Start.ToString());

                    if (Str.StartsWith(Quote.Start.ToString()) && Str.EndsWith(Quote.End.ToString()))
                    {
                        Points -= 3;
                        LineQuotes = Quote;
                        break;
                    }
                    else if (Str.StartsWith(Quote.Start.ToString()) || Str.EndsWith(Quote.End.ToString()))
                    {
                        Points--;
                        LineQuotes = Quote;
                        break;
                    }
                }
                try
                {
                    char Last = (LineQuotes == null ? Str.Last() : Str.TrimEnd(LineQuotes.Value.End).Last());
                    if (IsJap && PontuationJapList.Contains(Last))
                        Points -= 3;

                    if (!IsJap && (PontuationList).Contains(Last))
                        Points -= 3;

                }
                catch { }
                try
                {
                    char First = (LineQuotes == null ? Str.First() : Str.TrimEnd(LineQuotes.Value.Start).First());
                    if (IsJap && PontuationJapList.Contains(First))
                        Points -= 3;

                    if (!IsJap && (PontuationList).Contains(First))
                        Points -= 3;

                }
                catch { }

                if (!IsJap)
                {
                    foreach (string Word in Words)
                    {
                        int WNumbers = Word.Where(c => char.IsNumber(c)).Count();
                        int WLetters = Word.Where(c => char.IsLetter(c)).Count();
                        if (WLetters > 0 && WNumbers > 0)
                        {
                            Points += 2;
                        }
                        if (Word.Trim(PontuationList).Where(c => PontuationList.Contains(c)).Count() != 0)
                        {
                            Points += 2;
                        }
                    }
                }

                if (!BeginQuote && !char.IsLetter(Str.First()))
                    Points += 2;

                if (Specials > WordCount)
                    Points++;

                if (Specials > Latim + JapChars)
                    Points += 2;

                if (SpecialsStranges > 0)
                    Points += 2;

                if (SpecialsStranges > 3)
                    Points++;

                if ((Pontuations == 0) && (WordCount <= 2) && !IsJap)
                    Points++;

                if (Uppers > Pontuations + 2 && !IsCaps)
                    Points++;

                if (Spaces > WordCount * 2)
                    Points++;

                if (Uppers > Spaces + 1 && !IsCaps)
                    Points++;

                if (IsJap && Spaces == 0)
                    Points--;

                if (!IsJap && Spaces == 0)
                    Points += 2;

                if (WordCount <= 2 && Numbers != 0)
                    Points += (int)(Str.PercentOf(Numbers) / 10);

                if (Str.Length <= 3 && !IsJap)
                    Points++;

                if (Numbers >= Str.Length)
                    Points += 3;

                if (IsJap && Kanjis / 2 > JapChars)
                    Points--;

                if (IsJap && JapChars > Kanjis)
                    Points--;

                if (IsJap && Latim != 0)
                    Points += (int)(Str.PercentOf(Latim) / 10) + 2;

                if (IsJap && NumbersJap != 0)
                    Points += (int)(Str.PercentOf(NumbersJap) / 10) + 2;

                if (IsJap && Numbers != 0)
                    Points += (int)(Str.PercentOf(Numbers) / 10) + 3;

                if (IsJap && Pontuations != 0)
                    Points += (int)(Str.PercentOf(Pontuations) / 10) + 2;

                if (Str.Trim() == string.Empty)
                    return false;

                if (Str.Trim().Trim(Str.Trim().First()) == string.Empty)
                    Points += 2;

                if (IsJap != Config.Default.Filter.FromAsian)
                    return false;

                bool Result = Points < (Caution ?? Config.Default.Filter.Sensitivity);
                return Result;
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#else
                return false;
#endif
            }
        }

        internal static double PercentOf(this string String, int Value)
        {
            var Result = Value / (double)String.Length;
            return Result * 100;
        }
    }
}
