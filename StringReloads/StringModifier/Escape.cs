using StringReloads.Engine.Interface;
using System;
using System.Text;

namespace StringReloads.StringModifier
{
    class Escape : IStringModifier
    {
        public static Escape Default => new Escape();

        public string Name => "Escape";

        public bool CanRestore => true;

        public string Apply(string String, string Original)
        {
            StringBuilder SB = new StringBuilder();
            foreach (char c in String)
            {
                if (c == '\n')
                    SB.Append("\\n");
                else if (c == '\\')
                    SB.Append("\\\\");
                else if (c == '\t')
                    SB.Append("\\t");
                else if (c == '\r')
                    SB.Append("\\r");
                else
                    SB.Append(c);
            }
            return SB.ToString();
        }

        public string Restore(string String)
        {
            StringBuilder SB = new StringBuilder();
            bool Escape = false;
            foreach (char c in String)
            {
                if (c == '\\' & !Escape)
                {
                    Escape = true;
                    continue;
                }
                if (Escape)
                {
                    switch (c.ToString().ToLower()[0])
                    {
                        case '\\':
                            SB.Append('\\');
                            break;
                        case 'n':
                            SB.Append('\n');
                            break;
                        case 't':
                            SB.Append('\t');
                            break;
                        case '"':
                            SB.Append('"');
                            break;
                        case '\'':
                            SB.Append('\'');
                            break;
                        case 'r':
                            SB.Append('\r');
                            break;
                        default:
                            throw new Exception("\\" + c + " Isn't a valid string escape.");
                    }
                    Escape = false;
                }
                else
                    SB.Append(c);
            }

            return SB.ToString();
        }
    }
}
