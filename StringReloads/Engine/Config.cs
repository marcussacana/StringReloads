using StringReloads.StringModifier;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace StringReloads.Engine
{
    unsafe class Config
    {
        internal static Config Default => EntryPoint.SRL.Settings;

        internal void* _MainWindow = null;
        internal void* MainWindow => _MainWindow != null ? _MainWindow : (_MainWindow = Process.GetCurrentProcess().MainWindowHandle.ToPointer());

        bool? _AutoInstall = null;
        internal bool AutoInstall => (_AutoInstall ?? (_AutoInstall = GetValue("AutoInstall").ToBoolean())).Value;


        string _ConfigPath = null;
        internal string ConfigPath => _ConfigPath ?? (_ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SRL.ini"));

        bool? _Debug = null;
        internal bool Debug => (_Debug ?? (_Debug = GetValue("Debug").ToBoolean())).Value;

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

        bool? _Dump = null;
        internal bool Dump => (_Dump ?? (_Dump = GetValue("Dump").ToBoolean())).Value;


        bool? _DumpFilter = null;
        internal bool DumpFilter => (_DumpFilter ?? (_DumpFilter = GetValue("DumpFilter").ToBoolean())).Value;

        bool? _ImportHook = null;
        internal bool ImportHook => (_ImportHook ?? (_ImportHook = GetValue("ImportHook").ToBoolean())).Value;


        bool? _Log = null;
        internal bool Log => (_Log ?? (_Log = GetValue("Log").ToBoolean())).Value;

        bool? _LogFile = null;
        internal bool LogFile => (_LogFile ?? (_LogFile = GetValue("LogFile").ToBoolean())).Value;

        Log.LogLevel? _LogLevel = null;
        internal Log.LogLevel LogLevel => (_LogLevel ?? (_LogLevel = GetValue("LogLevel").ToLogLevel())).Value;


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

                return _REncoding = (Encoding ?? GlobalEncoding).ToEncoding();
            }
        }

        Encoding _WEncoding = null;
        internal Encoding WriteEncoding {
            get {
                if (_WEncoding != null)
                    return _WEncoding;

                var GlobalEncoding = GetValue("Encoding");
                var Encoding = GetValue("WriteEncoding");

                return _WEncoding = (Encoding ?? GlobalEncoding).ToEncoding();
            }
        }


        void* _GameBaseAddress = null;
        internal void* GameBaseAddress => _GameBaseAddress != null ? _GameBaseAddress : (_GameBaseAddress = Process.GetCurrentProcess().MainModule.BaseAddress.ToPointer());

        string _GameExePath = null;
        internal string GameExePath => _GameExePath ?? (_GameExePath = Process.GetCurrentProcess().MainModule.FileName);

        string _CachePath = null;
        internal string CachePath => _CachePath ?? (_CachePath = Path.Combine(WorkingDirectory, "Cache.srl"));



        string _Breakline = null;
        internal string BreakLine => _Breakline ?? (_Breakline = GetValue("BreakLine").Unescape());
        


        int? _Width = null;
        internal int Width => (_Width ?? (_Width = GetValue("Width", "Wordwrap").ToInt32())).Value;


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
                    Mods[Pair.Key] = Pair.Value.ToBoolean();
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
                    Hks[Pair.Key] = Pair.Value.ToBoolean();
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
                    Mods[Pair.Key] = Pair.Value.ToBoolean();
                }

                return Mods;
            }
        }

        internal Filter? _Filter = null;
        internal Filter Filter {
            get {
                if (_Filter.HasValue)
                    return _Filter.Value;

                _Filter = new Filter() { 
                    FromAsian = GetValue("FromAsian", "Filter").ToBoolean(),
                    DenyList = GetValue("DenyList", "Filter"),
                    IgnoreList = GetValue("IgnoreList", "Filter"),
                    QuoteList = GetValue("QuoteList", "Filter"),
                    Sensitivity = GetValue("Sensitivity", "Filter").ToInt32(),
                    UseDB = GetValue("UseDB", "Filter").ToBoolean()
                };

                return _Filter.Value;
            }
        }

        #region IniParser

        public string GetValue(string Name, string Group = "StringReloads") {
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

        public Dictionary<string, string> GetValues(string Group)
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

        public void SetValues(string Group, Dictionary<string, string> Entries) {
            foreach (KeyValuePair<string, string> Entry in Entries) {
                SetValue(Group, Entry.Key, Entry.Value);
            }
        }

        public void SetValue(string Name, string Value) => SetValue("StringReloads", Name, Value);
        public void SetValue(string Group, string Name, string Value)
        {
            int GroupBegin = -1;
            int GroupEnd = IniLines.Length;

            string CurrentGroup = null;
            for (int i = 0; i < IniLines.Length; i++) {
                string Line = IniLines[i];
                if (Line.StartsWith("//") || Line.StartsWith(";") || Line.StartsWith("!"))
                    continue;

                if (Line.StartsWith("[") && Line.EndsWith("]"))
                {
                    CurrentGroup = Line.Substring(1, Line.Length - 2).Trim().ToLowerInvariant();
                    if (CurrentGroup == Group.ToLowerInvariant()) {
                        GroupBegin = i;
                    }
                    continue;
                }

                if (!Line.Contains("="))
                    continue;

                if (CurrentGroup != Group.Trim().ToLowerInvariant())
                    continue;

                string CurrentName = Line.Substring(0, Line.IndexOf('=')).Trim().ToLowerInvariant();

                GroupEnd = i;

                if (CurrentName != Name.Trim().ToLowerInvariant())
                    continue;

                IniLines[i] = $"{Name}={Value}";
                return;
            }

            List<string> NewLines = new List<string>(IniLines);
            if (GroupBegin >= 0) {
                if (GroupEnd + 1 < IniLines.Length)
                    NewLines.Insert(GroupEnd + 1, $"{Name}={Value}");
                else
                    NewLines.Add($"{Name}={Value}");
            } else {
                if (NewLines.Last().Trim() != string.Empty)
                    NewLines.Add(string.Empty);

                NewLines.Add($"[{Group}]");
                NewLines.Add($"{Name}={Value}");
            }
            _IniLines = NewLines.ToArray();
        }

        public void SaveSettings() {
            File.WriteAllLines(ConfigPath, IniLines);
        }

        #endregion

        #region Tools
        public bool HookEnabled(string Name) {
            return Hooks.ContainsKey(Name.ToLowerInvariant()) && Hooks[Name.ToLowerInvariant()];
        }
        #endregion
    }
}
