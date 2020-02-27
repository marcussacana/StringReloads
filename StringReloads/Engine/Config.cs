using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace StringReloads.Engine
{
    unsafe class Config
    {
        internal static Config Default => EntryPoint.SRL.Settings;

        internal void* _MainWindow = null;
        internal void* MainWindow => _MainWindow != null ? _MainWindow : (_MainWindow = Process.GetCurrentProcess().MainWindowHandle.ToPointer());

        bool? _AutoInstall = null;
        internal bool AutoInstall => (_AutoInstall ?? (_AutoInstall = ToBoolean(GetValue("AutoInstall")))).Value;


        string _ConfigPath = null;
        internal string ConfigPath => _ConfigPath ?? (_ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SRL.ini"));


        string[] _IniLines = null;
        internal string[] IniLines {
            get {
                if (_IniLines != null)
                    return _IniLines;
                if (!File.Exists(ConfigPath)) {
                    throw new Exception($"\"{ConfigPath}\" not found.");
                }
                var IniContent = File.ReadAllText(ConfigPath, Encoding.UTF8).Replace("\r\n", "\n").Replace("\r", "\n");
                return _IniLines = IniContent.Split('\n');
            }
        }


        bool? _Log = null;
        internal bool Log => (_Log ?? (_Log = ToBoolean(GetValue("Log")))).Value;

        bool? _LogFile = null;
        internal bool LogFile => (_LogFile ?? (_LogFile = ToBoolean(GetValue("LogFile")))).Value;

        Log.LogLevel? _LogLevel = null;
        internal Log.LogLevel LogLevel => (_LogLevel ?? (_LogLevel = ToLogLevel(GetValue("LogLevel")))).Value;


        string _Workspace = null;
        internal string Workspace => _Workspace ?? (_Workspace = GetValue("Workspace"));


        string _WorkingDirectory = null;
        internal string WorkingDirectory {
            get {
                if (_WorkingDirectory != null)
                    return _WorkingDirectory;

                _WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (Workspace != string.Empty)
                    _WorkingDirectory = Path.Combine(_WorkingDirectory, Workspace);

                return _WorkingDirectory;
            }
        }

        Version _SRLVersion = null;
        internal Version SRLVersion {
            get {
                if (_SRLVersion != null)
                    return _SRLVersion;

                string DLLPath = EntryPoint.CurrentDll;
                return _SRLVersion = Version.Parse(FileVersionInfo.GetVersionInfo(DLLPath).FileVersion);
            }
        }

        Encoding _REncoding = null;
        internal Encoding ReadEncoding {
            get {
                if (_REncoding != null)
                    return _REncoding;

                var GlobalEncoding = GetValue("Encoding");
                var Encoding = GetValue("ReadEncoding");

                return _REncoding = ToEncoding(Encoding ?? GlobalEncoding);
            }
        }

        Encoding _WEncoding = null;
        internal Encoding WriteEncoding {
            get {
                if (_WEncoding != null)
                    return _WEncoding;

                var GlobalEncoding = GetValue("Encoding");
                var Encoding = GetValue("WriteEncoding");

                return _WEncoding = ToEncoding(Encoding ?? GlobalEncoding);
            }
        }



        string _CachePath = null;
        internal string CachePath => _CachePath ?? (_CachePath = Path.Combine(WorkingDirectory, "Cache.srl"));



        string _Breakline = null;
        internal string BreakLine => _Breakline ?? (_Breakline = ToUnescaped(GetValue("BreakLine")));
        


        int? _Width = null;
        internal int Width => (_Width ?? (_Width = ToInteger(GetValue("Width", "Wordwrap")))).Value;


        Dictionary<string, string>[] _FontRemaps;

        internal Dictionary<string, string>[] FontRemaps { get {
                if (_FontRemaps != null)
                    return _FontRemaps;

                List<Dictionary<string, string>> Remaps = new List<Dictionary<string, string>>();

                for (int i = 0; ; i++) {
                    var Remap = GetValues($"Font.{i}");
                    if (Remap == null || !Remap.ContainsKey("from"))
                        break;
                    Remaps.Add(Remap);
                }

                return _FontRemaps = Remaps.ToArray();
            }
        }

        internal Dictionary<string, bool> Modifiers {
            get {
                var Settings = GetValues("Modifiers");
                Dictionary<string, bool> Mods = new Dictionary<string, bool>();
                foreach (var Pair in Settings)
                {
                    Mods[Pair.Key] = ToBoolean(Pair.Value);
                }

                return Mods;
            }
        }
        internal Dictionary<string, bool> Hooks {
            get {
                var Settings = GetValues("Hooks");
                Dictionary<string, bool> Hks = new Dictionary<string, bool>();
                foreach (var Pair in Settings)
                {
                    Hks[Pair.Key] = ToBoolean(Pair.Value);
                }

                return Hks;
            }
        }
        internal Dictionary<string, bool> Mods {
            get {
                var Settings = GetValues("Mods");
                Dictionary<string, bool> Mods = new Dictionary<string, bool>();
                foreach (var Pair in Settings)
                {
                    Mods[Pair.Key] = ToBoolean(Pair.Value);
                }

                return Mods;
            }
        }

        #region IniParser

        private string GetValue(string Name, string Group = "StringReloads") {
            string CurrentGroup = null;
            foreach (string Line in IniLines) {
                if (Line.StartsWith("//") || Line.StartsWith(";") || Line.StartsWith("!"))
                    continue;

                if (Line.StartsWith("[") && Line.EndsWith("]")) {
                    CurrentGroup = Line.Substring(1, Line.Length - 2).Trim().ToLowerInvariant();
                    continue;
                }

                if (!Line.Contains("="))
                    continue;

                if (CurrentGroup != Group.Trim().ToLowerInvariant())
                    continue;

                string CurrentName = Line.Substring(0, Line.IndexOf('=')).Trim().ToLowerInvariant();
                string CurrentValue = Line.Substring(Line.IndexOf('=') + 1).Trim();

                if (CurrentName != Name.Trim().ToLowerInvariant())
                    continue;

                return CurrentValue;
            }

            return null;
        }

        private Dictionary<string, string> GetValues(string Group)
        {
            var Result = new Dictionary<string, string>();
            string CurrentGroup = null;
            foreach (string Line in IniLines)
            {
                if (Line.StartsWith("//") || Line.StartsWith(";") || Line.StartsWith("!"))
                    continue;

                if (Line.StartsWith("[") && Line.EndsWith("]"))
                {
                    CurrentGroup = Line.Substring(1, Line.Length - 2).Trim().ToLowerInvariant();
                    continue;
                }

                if (!Line.Contains("="))
                    continue;

                if (CurrentGroup != Group.Trim().ToLowerInvariant())
                    continue;

                string CurrentName = Line.Substring(0, Line.IndexOf('=')).Trim().ToLowerInvariant();
                string CurrentValue = Line.Substring(Line.IndexOf('=') + 1).Trim();

                Result[CurrentName] = CurrentValue;
            }

            if (Result.Count == 0)
                return null;

            return Result;
        }

        #endregion

        #region Convertions
        private bool ToBoolean(string Value)
        {
            if (Value == null)
                return false;

            Value = Value.ToLowerInvariant();
            switch (Value) {
                case "1":
                case "true":
                case "yes":
                case "on":
                case "enable":
                case "enabled":
                    return true;
            }
            return false;
        }

        private Log.LogLevel ToLogLevel(string ValueStr)
        {
            switch (ValueStr.ToLowerInvariant()) {
                case "t":
                case "tra":
                case "trc":
                case "trace":
                    return global::Log.LogLevel.Trace;
                case "d":
                case "deb":
                case "dbg":
                case "debug":
                    return global::Log.LogLevel.Debug;
                case "i":
                case "inf":
                case "info":
                case "information":
                    return global::Log.LogLevel.Information;
                case "w":
                case "war":
                case "warn":
                case "warning":
                    return global::Log.LogLevel.Warning;
                case "e":
                case "err":
                case "erro":
                case "error":
                    return global::Log.LogLevel.Error;
                case "c":
                case "cri":
                case "crit":
                case "critical":
                    return global::Log.LogLevel.Critical;
            }

            return (Log.LogLevel)ToInteger(ValueStr);
        }
        private int ToInteger(string Value) {
            if (Value == null)
                return 0;

            if (int.TryParse(Value, out int Val))
                return Val;

            return 0;
        }

        private Encoding ToEncoding(string Value) {
            if (int.TryParse(Value, out int CP))
                return Encoding.GetEncoding(CP);

            return Value.ToLowerInvariant() switch
            {
                "sjis" => Encoding.GetEncoding(932),
                "shiftjis" => Encoding.GetEncoding(932),
                "shift-jis" => Encoding.GetEncoding(932),
                "unicode" => Encoding.Unicode,
                "utf16" => Encoding.Unicode,
                "utf16be" => Encoding.BigEndianUnicode,
                "utf16wb" => new UnicodeEncoding(false, true),
                "utf16wbbe" => new UnicodeEncoding(true, true),
                "utf16bewb" => new UnicodeEncoding(true, true),
                "utf8" => Encoding.UTF8,
                "utf8wb" => new UTF8Encoding(true),
                "utf7" => Encoding.UTF7,
                _ => Encoding.GetEncoding(Value)
            };
        }

        private string ToUnescaped(string String) {
            return Escape.Default.Restore(String);
        }

        #endregion

        #region Tools
        public bool HookEnabled(string Name) {
            return Hooks.ContainsKey(Name.ToLowerInvariant()) && Hooks[Name.ToLowerInvariant()];
        }
        #endregion
    }
}
