using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System;

namespace StringReloads.StringModifier
{
    class TrimRestore : IStringModifier
    {
        public static TrimRestore Default = new TrimRestore(Config.Default);

        public TrimRestore(Config Settings) {
            var Group = Settings.GetValues("Trim");

            if (Group == null)
            {
                Prefixes = Sufixes = new string[0];
                return;
            }

            Prefixes = Group["prefixes"].Unescape().Split('\n');
            Sufixes = Group["sufixes"].Unescape().Split('\n');
        }

        public TrimRestore(SRL Engine) : this(Engine.Settings) { }

        public string[] Prefixes;
        public string[] Sufixes;

        public string Name => "TrimRestore";

        public bool CanRestore => false;

        public string Apply(string String, string Original)
        {
            var Prefix = GetPrefix(Original);
            var Sufix = GetSufix(Original);

            var Result = Trim(String);

            return $"{Prefix}{Result}{Sufix}";
        }

        public string Restore(string String)
        {
            return String;
        }

        public string Trim(string String)
        {
            String = TrimEnd(String);
            return TrimStart(String);
        }

        public string Trim(string String, string Pattern)
        {
            String = TrimEnd(String, Pattern);
            return TrimStart(String, Pattern);
        }

        public string GetPrefix(string String)
        {
            var Trimmed = TrimStart(String);
            return String.Substring(0, String.Length - Trimmed.Length);
        }

        public string GetSufix(string String)
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
