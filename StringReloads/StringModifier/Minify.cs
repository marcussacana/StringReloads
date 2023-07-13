using System;
using StringReloads.Engine;
using StringReloads.Engine.Interface;

namespace StringReloads.StringModifier
{
    class Minify : IStringModifier
    {
        public Minify() {
            Config.Default.Modifiers.TryGetValue("acceptablerangeminify", out AcceptableRange);
        }
        public static Minify Default = new Minify();

        bool AcceptableRange;
        public string Name => "Minify";

        public bool CanRestore => false;

        public string Apply(string String, string Original)
        {
            var Rst = string.Empty;
            foreach (char Char in String) {
                switch (Char) {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        break;
                    default:
                        if (char.IsWhiteSpace(Char))
                            break;

                        if (AcceptableRange)
                        {
                            bool Valid = false;
                            foreach (var Range in Config.Default.Filter.AcceptableRange)
                            {
                                if (Char >= Range.Begin && Char <= Range.End)
                                {
                                    Valid = true;
                                    break;
                                }
                            }

                            if (!Valid)
                                break;
                        }

                        Rst += char.ToLowerInvariant(Char);
                        break;
                }
            }
            return Rst.Trim().Replace(Config.Default.BreakLine.ToLowerInvariant(), "");
        }

        public string Restore(string String)
        {
            throw new NotImplementedException();
        }
    }
}
