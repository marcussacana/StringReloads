using StringReloads.Engine.Interface;
using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StringReloads.Engine.Match
{
    class BasicMatch : IMatch
    {
        SRL Engine;
        public BasicMatch(SRL Engine) => this.Engine = Engine;

        public char? ResolveRemap(char Char)
        {
            if (!Engine.CharRemap.ContainsValue(Char))
                return Char;

            return Engine.CharRemap.ReverseMatch(Char);
        }

        public bool HasMatch(string String)
        {
            string Minified = String;

            if (!Engine.Settings.FastMode)
            {
                foreach (var Modifier in Engine.ReloadModifiers)
                {
                    if (Modifier.CanRestore)
                        Minified = Modifier.Restore(Minified);
                }
            }

            Minified = Engine.Minify(String);

            if (Engine.CurrentDatabase.HasKey(Minified))
                return true;

            if (Engine.Settings.Hashset && !Engine.Hashset.Contains(String) && !Engine.Hashset.Contains(Minified))
                return false;

            for (int i = 0; i < Engine.Databases.Count; i++)
            {
                if (Engine.Databases[i].HasKey(Minified))
                    return true;
            }

            return false;
        }
        public bool HasValue(string String)
        {
            string Minified = String;

            if (!Engine.Settings.FastMode)
            {
                foreach (var Modifier in Engine.ReloadModifiers)
                {
                    if (Modifier.CanRestore)
                        Minified = Modifier.Restore(Minified);
                }
            }

            Minified = Engine.Minify(String);

            if (Engine.CurrentDatabase.HasValue(Minified))
                return true;

            for (int i = 0; i < Engine.Databases.Count; i++)
            {
                if (Engine.Databases[i].HasValue(Minified))
                    return true;
            }

            return false;
        }
        public LSTEntry? MatchString(string String)
        {

            string Minified = String;
            string UnmodifiedMinified = null;

            if (!Engine.Settings.FastMode)
            {
                UnmodifiedMinified = Engine.Minify(String);
                foreach (var Modifier in Engine.ReloadModifiers)
                {
                    if (Modifier.CanRestore)
                        Minified = Modifier.Restore(Minified);
                }
            }

            Minified = Engine.Minify(Minified);

            if (Engine.CurrentDatabase.HasKey(Minified))
                return Engine.CurrentDatabase[Minified];

            if (UnmodifiedMinified != null && Engine.CurrentDatabase.HasKey(UnmodifiedMinified))
                return Engine.CurrentDatabase[UnmodifiedMinified];

            if (Engine.Settings.Hashset && !Engine.Hashset.Contains(String) && !Engine.Hashset.Contains(Minified) && !Engine.Hashset.Contains(UnmodifiedMinified))
                return null;

            for (int i = 0; i < Engine.Databases.Count; i++)
            {
                if (Engine.Databases[i].HasKey(Minified))
                {
                    Engine.CurrentDatabaseIndex = i;
                    Log.Trace($"Database Changed to {Engine.Databases[i].Name} (ID: {i})");
                    return Engine.Databases[i][Minified];
                }

                if (UnmodifiedMinified != null && Engine.Databases[i].HasKey(UnmodifiedMinified))
                {
                    Engine.CurrentDatabaseIndex = i;
                    Log.Trace($"Database Changed to {Engine.Databases[i].Name} (ID: {i})");
                    return Engine.Databases[i][UnmodifiedMinified];
                }
            }

            if (Engine.Settings.Dump)
                DumpString(String, Minified);

            return null;
        }

        List<string> DumpCache = new List<string>();
        TextWriter DefaultLST = null;
        void DumpString(string String, string Minified)
        {
            if (Engine.Settings.Filter.DumpFilter && !String.IsDialogue(UseAcceptableRange: Engine.Settings.Filter.DumpAcceptableRange))
                return;

            if (DefaultLST == null)
            {
                string LSTPath = Path.Combine(Engine.Settings.WorkingDirectory, "Strings.lst");
                if (File.Exists(LSTPath))
                {
                    using (var Reader = new StreamReader(File.Open(LSTPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                    {
                        while (Reader.Peek() != -1)
                        {
                            DumpCache.Add(Engine.Minify(Reader.ReadLine()));
                            Reader.ReadLine();
                        }
                        Reader.Close();
                    }
                }

                var FStream = File.Open(LSTPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                FStream.Seek(0, SeekOrigin.End);

                DefaultLST = new StreamWriter(FStream);
            }

            if (DumpCache.Contains(Minified))
                return;

            DumpCache.Add(Minified);

            String = String.Trim().Replace("\n", LSTParser.BreakLine).Replace("\r", LSTParser.ReturnLine);

            DefaultLST.WriteLine(String);
            DefaultLST.WriteLine(String);
            DefaultLST.Flush();
        }

        public FontRemap? ResolveRemap(string Facename, int Width, int Height, uint Charset)
        {
            var Remap = (from x in Config.Default.FontRemaps where
                                CheckFontValue(x, "From", Facename, false) &&
                                CheckFontValue(x, "FromWidth", Width.ToString(), true) &&
                                CheckFontValue(x, "FromHeight", Height.ToString(), true) &&
                                CheckFontValue(x, "FromCharset", Charset.ToString(), false)
                         select x).FirstOrDefault();

            if (Remap == null)
                Remap = (from x in Config.Default.FontRemaps where
                                CheckFontValue(x, "From", "*", false) &&
                                CheckFontValue(x, "FromWidth", Width.ToString(), true) &&
                                CheckFontValue(x, "FromHeight", Height.ToString(), true) &&
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

            var EvalKeys = new string[] { "Width", "Height", "Charset", "Name",   "Facename" };
            var EvalVals = new object[] {  Width,   Height,   Charset,  Facename,  Facename  };

            if (Remap.ContainsKey("width"))
            {
                var nWidth = Remap["width"];

                bool ForceAbsolute = nWidth.StartsWith(".");

                if (ForceAbsolute)
                    nWidth = nWidth.Substring(1);

                if (nWidth != "0")
                {
                    bool Relative = !ForceAbsolute && (nWidth.StartsWith("+") || nWidth.StartsWith("-"));
                    int Value = nWidth.ToInt32();
                    if (Value == 0 && !Relative)
                        Value = (int)nWidth.Evalaute(EvalKeys, EvalVals);
                  
                    if (Relative)
                        Rst.Width = Width + Value;
                    else
                        Rst.Width = Value;
                } else
                    Rst.Width = 0;
            }
            else
                Rst.Width = Width;

            if (Remap.ContainsKey("height"))
            {
                var nHeight = Remap["height"];

                bool ForceAbsolute = nHeight.StartsWith(".");

                if (ForceAbsolute)
                    nHeight = nHeight.Substring(1);

                if (nHeight != "0")
                {
                    bool Relative = !ForceAbsolute && (nHeight.StartsWith("+") || nHeight.StartsWith("-"));
                    int Value = nHeight.ToInt32();
                    if (Value == 0 && !Relative)
                        Value = (int)nHeight.Evalaute(EvalKeys, EvalVals);

                    if (Relative)
                        Rst.Height = Height + Value;
                    else
                        Rst.Height = Value;
                }
                else
                    Rst.Height = 0;
            }
            else
                Rst.Height = Height;


            Log.Trace($"Remaped    -> Width: {Rst.Width:+00;-00}, Height: {Rst.Height:+00;-00}, Charset: 0x{Rst.Charset:X2}, Name: \"{Rst.To}\"");

            return Rst;
        }

        private bool CheckFontValue(Dictionary<string, string> Dic, string Entry, string Expected, bool Evalaute = false)
        {
            if (!Dic.ContainsKey(Entry.ToLowerInvariant()))
                return true;

            string Value = Dic[Entry.ToLowerInvariant()];

            if (Evalaute)
            {
                var Mode = "=";

                if (Value.StartsWith(">="))
                {
                    Mode = ">=";
                    Value = Value.Substring(2);
                }
                else if (Value.StartsWith("<="))
                {
                    Mode = "<=";
                    Value = Value.Substring(2);
                }
                else if (Value.StartsWith(">"))
                {
                    Mode = ">";
                    Value = Value.Substring(1);
                }
                else if (Value.StartsWith("<"))
                {
                    Mode = "<";
                    Value = Value.Substring(1);
                }

                var Val = long.Parse(Value);
                var Exp = long.Parse(Expected);

                switch (Mode)
                {
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

            }
            else if (Value == Expected)
                return true;

            return false;
        }
    }

    internal static partial class Extensions
    {
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
