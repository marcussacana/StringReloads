using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.Engine.Interface
{
    public interface IMatch
    {
        public char? ResolveRemap(char Char);
        public bool HasMatch(string String);
        public LSTParser.LSTEntry? MatchString(string String);
        public FontRemap? ResolveRemap(string Facename, int Width, int Height, uint Charset);
    }
}
