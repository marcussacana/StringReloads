using StringReloads.Engine;
using StringReloads.Engine.Interface;

namespace StringReloads.StringModifier
{
    public class Remaper : IStringModifier
    {

        static Remaper _Default = null;
        public static Remaper Default = _Default ?? new Remaper(EntryPoint.SRL);
        SRL Engine;
        public Remaper(SRL Main) => Engine = Main;

        public string Name => "Remaper";

        public bool CanRestore => true;

        public string Apply(string String, string Original)
        {
            string Result = string.Empty;
            foreach (var Char in String)
            {
                if (Engine.CharRemap.ContainsKey(Char))
                    Result += Engine.CharRemap[Char];
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
                if (Engine.CharRemap.ContainsValue(Char))
                    Result += Engine.CharRemap.ReverseMatch(Char);
                else
                    Result += Char;
            }

            return Result;
        }
    }
}
