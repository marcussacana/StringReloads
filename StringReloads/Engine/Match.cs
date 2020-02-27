using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StringReloads.Engine
{
    class Match
    {
        Main Engine;
        public Match(Main Engine) => this.Engine = Engine;

        public char ResolveRemap(char Char) {
            if (!Engine.CharRemap.ContainsValue(Char))
                return Char;

            return Engine.CharRemap.ReverseMatch(Char);
        }

        public LSTParser.LSTEntry? MatchString(string String) {

            string Minified = String;

            foreach (var Modifier in Engine.ReloadModifiers) {
                if (Modifier.CanRestore)
                    Minified = Modifier.Restore(Minified);
            }

            Minified = Minify.Default.Apply(String);
            
            if (Engine.CurrentDatabase.HasKey(Minified))
                return Engine.CurrentDatabase[Minified];

            for (int i = 0; i < Engine.Databases.Count; i++) {
                if (Engine.Databases[i].HasKey(Minified)) {
                    Engine.CurrentDatabaseIndex = i;
                    Log.Trace($"Database Changed to {Engine.Databases[i].Name} (ID: {i})");
                    return Engine.Databases[i][Minified];
                }
            }

            return null;
        }
    }

    internal static partial class Extensions {
        static internal Key ReverseMatch<Key, Value>(this Dictionary<Key, Value> Dictionary, Value ValueToSearch)
        {
            if (!Dictionary.ContainsValue(ValueToSearch))
                throw new Exception("Value not Present in the Dictionary");

            int Index = Dictionary.Values.Select((value, index) => new { value, index })
                        .SkipWhile(pair => !pair.value.Equals(ValueToSearch)).FirstOrDefault().index;

            return Dictionary.Keys.ElementAt(Index);
        }

    }
}
