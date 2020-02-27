using StringReloads.Engine;

namespace StringReloads.StringModifier
{
    class Remaper : IStringModifier
    {
        Main Main;

        public Remaper(Main Main) => this.Main = Main;

        public string Name => "Remaper";

        public bool CanRestore => true;

        public string Apply(string String)
        {
            string Result = string.Empty;
            foreach (var Char in String)
            {
                if (Main.CharRemap.ContainsKey(Char))
                    Result += Main.CharRemap[Char];
                else
                    Result += Char;
            }

            return Result;
        }

        public string Restore(string String)
        {
            string Result = string.Empty;
            foreach (var Char in String)
            {
                if (Main.CharRemap.ContainsValue(Char))
                    Result += Main.CharRemap.ReverseMatch(Char);
                else
                    Result += Char;
            }

            return Result;
        }
    }
}
