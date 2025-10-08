using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.StringModifier
{
    internal class PreventInvalidChar : IStringModifier
    {
        public PreventInvalidChar(SRL Engine) 
        {
            Illegals = Engine.CharRemap.Values.ToArray();
        } 
        public string Name => "PreventInvalidChar";

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
