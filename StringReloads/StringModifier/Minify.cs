using System;
namespace StringReloads.StringModifier
{
    class Minify : IStringModifier
    {
        public static Minify Default => new Minify();

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
                        Rst += char.ToLowerInvariant(Char);
                        break;
                }
            }
            return Rst;
        }

        public string Restore(string String)
        {
            throw new NotImplementedException();
        }
    }
}
