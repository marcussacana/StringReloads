using StringReloads.Engine.Interface;

namespace StringReloads.Engine.Match
{
    class TrimMatch : BasicMatch, IMatch
    {
        string[] Prefixes;
        string[] Sufixes;

        SRL Engine;
        public TrimMatch(SRL Engine) : base(Engine)
        {
            this.Engine = Engine;
            var Dic = Engine.Settings.GetValues("Trim");
            Prefixes = Dic["prefixes"].Unescape().Split('\n');
            Sufixes = Dic["sufixes"].Unescape().Split('\n');
        }

        bool IMatch.HasMatch(string String)
        {
            if (!Engine.Settings.TrimMatch)
                return false;

            return HasMatch(String);
        }

        LSTEntry? IMatch.MatchString(string String)
        {
            var Prefix = GetPrefix(String);
            var Sufix = GetSufix(String);
            var Match = MatchString(Trim(String));
            if (Match == null)
                return null;

            LSTEntry Result = Match.Value;

            Result.OriginalLine    = $"{Prefix}{Result.OriginalLine}{Sufix}";
            Result.TranslationLine = $"{Prefix}{Result.TranslationLine}{Sufix}";

            return Result;
        }

        string Trim(string String)
        {
            String = TrimEnd(String);
            return TrimStart(String);
        }
        string Trim(string String, string Pattern)
        {
            String = TrimEnd(String, Pattern);
            return TrimStart(String, Pattern);
        }

        string GetPrefix(string String)
        {
            var Trimmed = TrimStart(String);
            return String.Substring(0, String.Length - Trimmed.Length);
        }

        string GetSufix(string String)
        {
            var Trimmed = TrimEnd(String);
            return String.Substring(Trimmed.Length);
        }

        string TrimStart(string String)
        {
        Begin: foreach (var Prefix in Prefixes)
            {
                var NewStr = TrimStart(String, Prefix);
                bool MustRestart = String != NewStr;
                String = NewStr;
                if (MustRestart)
                    goto Begin;
            }
            return String;
        }
        string TrimStart(string String, string Pattern)
        {
            while (String.StartsWith(Pattern))
                String = String.Substring(Pattern.Length);
            return String;
        }

        string TrimEnd(string String)
        {
        Begin: foreach (var Sufix in Sufixes)
            {
                var NewStr = TrimEnd(String, Sufix);
                bool MustRestart = String != NewStr;
                String = NewStr;
                if (MustRestart)
                    goto Begin;
            }
            return String;
        }

        string TrimEnd(string String, string Pattern)
        {
            while (String.EndsWith(Pattern))
                String = String.Substring(0, String.Length - Pattern.Length);
            return String;
        }
    }
}
