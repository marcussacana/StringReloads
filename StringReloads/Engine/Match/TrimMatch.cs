using StringReloads.Engine.Interface;
using StringReloads.StringModifier;

namespace StringReloads.Engine.Match
{
    class TrimMatch : BasicMatch, IMatch
    {
        SRL Engine;
        public TrimMatch(SRL Engine) : base(Engine)
        {
            this.Engine = Engine;
        }

        bool IMatch.HasMatch(string String)
        {
            if (!Engine.Settings.TrimMatch)
                return false;

            return HasMatch(String);
        }

        LSTEntry? IMatch.MatchString(string String)
        {
            var Prefix = TrimRestore.Default.GetPrefix(String);
            var Sufix = TrimRestore.Default.GetSufix(String);
            var Match = MatchString(TrimRestore.Default.Trim(String));
            if (Match == null)
                return null;

            LSTEntry Result = Match.Value;

            Result.OriginalLine = $"{Prefix}{Result.OriginalLine}{Sufix}";
            Result.TranslationLine = $"{Prefix}{Result.TranslationLine}{Sufix}";

            return Result;
        }
      
    }
}
