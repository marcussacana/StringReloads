using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Engine.Match;

namespace StringReloads.StringModifier
{
    public class RemaperAlt : IStringModifier
    {

        static RemaperAlt _Default = null;
        public static RemaperAlt Default = _Default ??= new RemaperAlt(EntryPoint.SRL);
        SRL Engine;
        public RemaperAlt(SRL Main) => Engine = Main;

        public string Name => "RemaperAlt";

        public bool CanRestore => true;

        public string Apply(string String, string Original)
        {
            string Result = string.Empty;
            foreach (var Char in String)
            {
                if (Engine.CharRemapAlt.ContainsKey(Char))
                    Result += Engine.CharRemapAlt[Char];
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
                if (Engine.CharRemapAlt.ContainsValue(Char))
                    Result += Engine.CharRemapAlt.ReverseMatch(Char);
                else
                    Result += Char;
            }

            return Result;
        }
    }
}
