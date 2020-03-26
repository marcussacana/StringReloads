using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            if (Engine.Settings.Dump)
                DumpString(String);
            return null;
        }

        TextWriter DefaultLST = null;
        void DumpString(string String) {
            if (DefaultLST == null) {
                string LSTPath = Path.Combine(Engine.Settings.WorkingDirectory, "Strings.lst");
                DefaultLST = new StreamWriter(File.OpenWrite(LSTPath), Encoding.UTF8);
            }
            DefaultLST.WriteLine(String);
            DefaultLST.WriteLine(String);
            DefaultLST.Flush();
        }

        public FontRemap? ResolveRemap(string Facename, int Width, int Height, uint Charset) {
            var Remap = (from x in Config.Default.FontRemaps where 
                         CheckFontValue(x, "From",        Facename,           false) &&
                         CheckFontValue(x, "FromWidth",   Width.ToString(),   true ) &&
                         CheckFontValue(x, "FromHeight",  Height.ToString(),  true ) &&
                         CheckFontValue(x, "FromCharset", Charset.ToString(), false)
                         select x).FirstOrDefault();

            if (Remap == null)
                Remap = (from x in Config.Default.FontRemaps where
                         CheckFontValue(x, "From",        "*",                false) &&
                         CheckFontValue(x, "FromWidth",   Width.ToString(),   true ) &&
                         CheckFontValue(x, "FromHeight",  Height.ToString(),  true ) &&
                         CheckFontValue(x, "FromCharset", Charset.ToString(), false)
                         select x).FirstOrDefault();

            Log.Trace($"CreateFont -> Width: {Width:+00;-00}, Height: {Height:+00;-00}, Charset: 0x{Charset:X2}, Name: \"{Facename}\"");

            if (Remap == null)
                return null;

            FontRemap Rst = new FontRemap();
            Rst.From = Remap["from"];

            if (Remap.ContainsKey("to"))
                Rst.To = Remap["to"];
            else Rst.To = Facename;

            if (Remap.ContainsKey("charset"))
                Rst.Charset = Remap["charset"].ToUInt32();
            else Rst.Charset = Charset;

            if (Remap.ContainsKey("width"))
            {
                var nWidth = Remap["width"];

                if (nWidth.StartsWith("."))
                    Rst.Width = nWidth.Substring(1).ToInt32();
                else if (nWidth.StartsWith("+") || nWidth.StartsWith("-"))
                    Rst.Width = Width + nWidth.ToInt32();
                else
                    Rst.Width = nWidth.ToInt32();
            }
            else
                Rst.Width = Width;

            if (Remap.ContainsKey("height"))
            {
                var nHeight = Remap["height"];

                if (nHeight.StartsWith("."))
                    Rst.Height = nHeight.Substring(1).ToInt32();
                else if (nHeight.StartsWith("+") || nHeight.StartsWith("-"))
                    Rst.Height = Height + nHeight.ToInt32();
                else
                    Rst.Height = nHeight.ToInt32();
            }
            else
                Rst.Height = Height;


            Log.Debug($"Remaped    -> Width: {Rst.Width:+00;-00}, Height: {Rst.Height:+00;-00}, Charset: 0x{Rst.Charset:X2}, Name: \"{Rst.To}\"");

            return Rst;
        }

        private bool CheckFontValue(Dictionary<string, string> Dic, string Entry, string Expected, bool Evalaute = false) {
            if (!Dic.ContainsKey(Entry.ToLowerInvariant()))
                return true;

            string Value = Dic[Entry.ToLowerInvariant()];

            if (Evalaute) {
                var Mode = "=";

                if (Value.StartsWith(">=")) {
                    Mode = ">=";
                    Value = Value.Substring(2);
                } else if (Value.StartsWith("<=")) {
                    Mode = "<=";
                    Value = Value.Substring(2);
                } else if (Value.StartsWith(">")) {
                    Mode = ">";
                    Value = Value.Substring(1);
                } else if (Value.StartsWith("<")) {
                    Mode = "<";
                    Value = Value.Substring(1);
                }

                var Val = long.Parse(Value);
                var Exp = long.Parse(Expected);

                switch (Mode) {
                    case "=":
                        return Exp == Val;
                    case ">":
                        return Exp > Val;
                    case "<": 
                        return Exp < Val;
                    case ">=":
                        return Exp >= Val;
                    case "<=":
                        return Exp <= Val;
                }

            } else if (Value == Expected)
                return true;

            return false;
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
