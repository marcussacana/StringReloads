using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.StringModifier
{
    internal class AntiIlegalChar : IStringModifier
    {
        public AntiIlegalChar(SRL Engine) 
        {
            Illegals = Engine.CharRemap.Values.ToArray();
        } 
        public string Name => "AntiIlegalChar";

        public bool CanRestore => false;


        char[] Illegals;

        public string Apply(string String, string Original)
        {
            foreach (var Char in Illegals)
                if (String.Contains(Char))
                    String = String.Replace(Char.ToString(), "");

            return String;
        }

        public string Restore(string String)
        {
            throw new NotImplementedException();
        }
    }
}
