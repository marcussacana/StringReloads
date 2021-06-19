using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using StringReloads.Engine.String;

namespace StringReloads.Engine
{
    public unsafe class Config
    {
        public static Config Default => EntryPoint.SRL.Settings;

        internal void* _MainWindow = null;
        public void* MainWindow => _MainWindow != null ? _MainWindow : (_MainWindow = Process.GetCurrentProcess().MainWindowHandle.ToPointer());

        bool? _AutoInstall = null;
        public bool AutoInstall => ((bool?)(_AutoInstall ??= GetValue("AutoInstall").ToBoolean())).Value;


        string _ConfigPath = null;
        public string ConfigPath => _ConfigPath ??= Path.Combine(EntryPoint.ApplicationDirectory, "SRL.ini");

        bool? _Debug = null;
        public bool Debug => ((bool?)(_Debug ??= GetValue("Debug").ToBoolean())).Value;

        bool? _CacheOutput = null;
        public bool CacheOutput => ((bool?)(_CacheOutput ??= GetValue("CacheOutput").ToBoolean())).Value;

        bool? _SanityCheck = null;
        public bool SanityCheck => ((bool?)(_SanityCheck ??= GetValue("SanityCheck").ToBoolean())).Value;

        string[] _IniLines = null;
        public string[] IniLines {
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
        public bool Dump => ((bool?)(_Dump ??= GetValue("Dump").ToBoolean())).Value;

        bool? _DumpRegex = null;
        public bool DumpRegex => ((bool?)(_DumpRegex ??= GetValue("DumpRegex").ToBoolean())).Value;


        bool? _ImportHook = null;
        public bool ImportHook => ((bool?)(_ImportHook ??= GetValue("ImportHook").ToBoolean())).Value;

        bool? _HeapAlloc = null;
        public bool HeapAlloc => ((bool?)(_HeapAlloc ??= GetValue("HeapAlloc").ToBoolean())).Value;
       
        bool? _Overwrite = null;
        public bool Overwrite => ((bool?)(_Overwrite ??= GetValue("Overwrite").ToBoolean())).Value;

        bool? _ReloadRegexCaptures = null;
        public bool ReloadRegexCaptures => ((bool?)(_ReloadRegexCaptures ??= GetValue("ReloadRegexCaptures").ToBoolean())).Value;

        bool? _TrimMatch = null;
        public bool TrimMatch => ((bool?)(_TrimMatch ??= GetValue("TrimMatch").ToBoolean())).Value;

        bool? _Log = null;
        public bool Log => ((bool?)(_Log ??= GetValue("Log").ToBoolean())).Value;

        bool? _LogFile = null;
        public bool LogFile => ((bool?)(_LogFile ??= GetValue("LogFile").ToBoolean())).Value;
       
        bool? _LoadLocalFont = null;
        public bool LoadLocalFont => ((bool?)(_LoadLocalFont ??= GetValue("LoadLocalFonts").ToBoolean())).Value;

        Log.LogLevel? _LogLevel = null;
        public Log.LogLevel LogLevel => ((Log.LogLevel?)(_LogLevel ??= GetValue("LogLevel").ToLogLevel())).Value;

        string _Workspace = null;
        public string Workspace => _Workspace ??= GetValue("Workspace");

        bool? _Hashset = null;
        public bool Hashset => _Hashset ??= GetValue("Hashset").ToBoolean();

        bool? _FastMode = null;
        public bool FastMode => _FastMode ??= GetValue("FastMode").ToBoolean();


        string _WorkingDirectory = null;
        public string WorkingDirectory {
            get {
                if (_WorkingDirectory != null)
                    return _WorkingDirectory;

                _WorkingDirectory = EntryPoint.ApplicationDirectory;
                if (Workspace != string.Empty)
                    _WorkingDirectory = Path.Combine(_WorkingDirectory, Workspace);

                return _WorkingDirectory;
            }
        }

        Version _SRLVersion = null;
        public Version SRLVersion {
            get {
                if (_SRLVersion != null)
                    return _SRLVersion;

                string DLLPath = EntryPoint.CurrentDll;
                return _SRLVersion = Version.Parse(FileVersionInfo.GetVersionInfo(DLLPath).FileVersion);
            }
        }

        Encoding _REncoding = null;
        public Encoding ReadEncoding {
            get {
                if (_REncoding != null)
                    return _REncoding;

                var GlobalEncoding = GetValue("Encoding");
                var Encoding = GetValue("ReadEncoding");

                return _REncoding = (Encoding ?? GlobalEncoding).ToEncoding();
            }
            set {
                _REncoding = value;
            }
        }

        Encoding _WEncoding = null;
        public Encoding WriteEncoding {
            get {
                if (_WEncoding != null)
                    return _WEncoding;

                var GlobalEncoding = GetValue("Encoding");
                var Encoding = GetValue("WriteEncoding");

                return _WEncoding = (Encoding ?? GlobalEncoding).ToEncoding();
            } set {
                _WEncoding = value;
            }
        }


        void* _GameBaseAddress = null;
        public void* GameBaseAddress => _GameBaseAddress != null ? _GameBaseAddress : (_GameBaseAddress = Process.GetCurrentProcess().MainModule.BaseAddress.ToPointer());

        string _GameExePath = null;
        public string GameExePath => _GameExePath ??= Process.GetCurrentProcess().MainModule.FileName;

        string _CachePath = null;
        public string CachePath => _CachePath ??= Path.Combine(WorkingDirectory, "Cache.srl");



        string _Breakline = null;
        public string BreakLine => _Breakline ??= GetValue("BreakLine").Unescape();



        string _RelativeWidth = null;
        public string RelativeWidth => _RelativeWidth ??= GetValue("RelativeWidth", "Wordwrap");

        int? _DefaultWidth = null;
        public int DefaultWidth => ((int?)(_DefaultWidth ??= GetValue("DefaultWidth", "Wordwrap").ToInt32())).Value;

        bool? _UseRelativeWidth = null;
        public bool UseRelativeWidth => ((bool?)(_UseRelativeWidth  ??= GetValue("UseRelativeWidth", "Wordwrap").ToBoolean())).Value;



        int? _MultiByteToWideCharCodePage = null;
        public int MultiByteToWideCharCodePage => ((int?)(_MultiByteToWideCharCodePage ??= GetValue("CodePage", "MultiByteToWideChar").ToInt32())).Value;

        int? _WideCharToMultiByteCodePage = null;
        public int WideCharToMultiByteCodePage => ((int?)(_WideCharToMultiByteCodePage ??= GetValue("CodePage", "WideCharToMultiByte").ToInt32())).Value;

        bool? _MultiByteToWideCharUndoRemap = null;
        public bool MultiByteToWideCharUndoRemap => ((bool?)(_MultiByteToWideCharUndoRemap ??= GetValue("UndoRemap", "MultiByteToWideChar").ToBoolean())).Value;

        bool? _WideCharToMultiByteUndoRemap = null;
        public bool WideCharToMultiByteUndoRemap => ((bool?)(_WideCharToMultiByteUndoRemap ??= GetValue("UndoRemap", "WideCharToMultiByte").ToBoolean())).Value;
       
        bool? _WideCharToMultiByteAutoEncoding = null;
        public bool WideCharToMultiByteAutoEncoding => ((bool?)(_WideCharToMultiByteAutoEncoding ??= GetValue("AutoEncoding", "WideCharToMultiByte").ToBoolean())).Value;

        bool? _MultiByteToWideCharAutoEncoding = null;
        public bool MultiByteToWideCharAutoEncoding => ((bool?)(_MultiByteToWideCharAutoEncoding ??= GetValue("AutoEncoding", "MultiByteToWideChar").ToBoolean())).Value;

        bool? _TextOutAUndoRemap = null;
        public bool TextOutAUndoRemap => ((bool?)(_TextOutAUndoRemap ??= GetValue("UndoRemap", "TextOutA").ToBoolean())).Value;

        bool? _TextOutWUndoRemap = null;
        public bool TextOutWUndoRemap => ((bool?)(_TextOutWUndoRemap ??= GetValue("UndoRemap", "TextOutW").ToBoolean())).Value;

        bool? _ExtTextOutAUndoRemap = null;
        public bool ExtTextOutAUndoRemap => ((bool?)(_ExtTextOutAUndoRemap ??= GetValue("UndoRemap", "ExtTextOutA").ToBoolean())).Value;

        bool? _ExtTextOutWUndoRemap = null;
        public bool ExtTextOutWUndoRemap => ((bool?)(_ExtTextOutWUndoRemap ??= GetValue("UndoRemap", "ExtTextOutW").ToBoolean())).Value;

        bool? _GetTextExtentPoint32WUndoRemap = null;
        public bool GetTextExtentPoint32WUndoRemap => ((bool?)(_GetTextExtentPoint32WUndoRemap ??= GetValue("UndoRemap", "GetTextExtentPoint32W").ToBoolean())).Value;

        bool? _GetTextExtentPoint32AUndoRemap = null;
        public bool GetTextExtentPoint32AUndoRemap => ((bool?)(_GetTextExtentPoint32AUndoRemap ??= GetValue("UndoRemap", "GetTextExtentPoint32A").ToBoolean())).Value;
        
        bool? _GetCharacterPlacementAUndoRemap = null;
        public bool GetCharacterPlacementAUndoRemap => ((bool?)(_GetCharacterPlacementAUndoRemap ??= GetValue("UndoRemap", "GetCharacterPlacementA").ToBoolean())).Value;

        bool? _GetCharacterPlacementWUndoRemap = null;
        public bool GetCharacterPlacementWUndoRemap => ((bool?)(_GetCharacterPlacementWUndoRemap ??= GetValue("UndoRemap", "_GetCharacterPlacementW").ToBoolean())).Value;

        bool? _MultiByteToWideCharRemapAlt = null;
        public bool MultiByteToWideCharRemapAlt => ((bool?)(_MultiByteToWideCharRemapAlt ??= GetValue("RemapAlt", "MultiByteToWideChar").ToBoolean())).Value;

        bool? _WideCharToMultiByteRemapAlt = null;
        public bool WideCharToMultiByteRemapAlt => ((bool?)(_WideCharToMultiByteRemapAlt ??= GetValue("RemapAlt", "WideCharToMultiByte").ToBoolean())).Value;

        bool? _TextOutARemapAlt = null;
        public bool TextOutARemapAlt => ((bool?)(_TextOutARemapAlt ??= GetValue("RemapAlt", "TextOutA").ToBoolean())).Value;

        bool? _TextOutWRemapAlt = null;
        public bool TextOutWRemapAlt => ((bool?)(_TextOutWRemapAlt ??= GetValue("RemapAlt", "TextOutW").ToBoolean())).Value;

        bool? _ExtTextOutARemapAlt = null;
        public bool ExtTextOutARemapAlt => ((bool?)(_ExtTextOutARemapAlt ??= GetValue("RemapAlt", "ExtTextOutA").ToBoolean())).Value;

        bool? _ExtTextOutWRemapAlt = null;
        public bool ExtTextOutWRemapAlt => ((bool?)(_ExtTextOutWRemapAlt ??= GetValue("RemapAlt", "ExtTextOutW").ToBoolean())).Value;


        bool? _GetTextExtentPoint32WRemapAlt = null;
        public bool GetTextExtentPoint32WRemapAlt => ((bool?)(_GetTextExtentPoint32WRemapAlt ??= GetValue("RemapAlt", "GetTextExtentPoint32W").ToBoolean())).Value;

        bool? _GetTextExtentPoint32ARemapAlt = null;
        public bool GetTextExtentPoint32ARemapAlt => ((bool?)(_GetTextExtentPoint32ARemapAlt ??= GetValue("RemapAlt", "GetTextExtentPoint32A").ToBoolean())).Value;

        bool? _GetCharacterPlacementARemapAlt = null;
        public bool GetCharacterPlacementARemapAlt => ((bool?)(_GetCharacterPlacementARemapAlt ??= GetValue("RemapAlt", "GetCharacterPlacementA").ToBoolean())).Value;

        bool? _GetCharacterPlacementWRemapAlt = null;
        public bool GetCharacterPlacementWRemapAlt => ((bool?)(_GetCharacterPlacementWRemapAlt ??= GetValue("RemapAlt", "GetCharacterPlacementW").ToBoolean())).Value;
        
        bool? _GetCharABCWidthsFloatWUndoRemap = null;
        public bool GetCharABCWidthsFloatWUndoRemap => ((bool?)(_GetCharABCWidthsFloatWUndoRemap ??= GetValue("UndoRemap", "GetCharABCWidthsFloatW").ToBoolean())).Value;

        bool? _GetCharABCWidthsFloatAUndoRemap = null;
        public bool GetCharABCWidthsFloatAUndoRemap => ((bool?)(_GetCharABCWidthsFloatAUndoRemap ??= GetValue("UndoRemap", "GetCharABCWidthsFloatA").ToBoolean())).Value;

        bool? _GetCharABCWidthsFloatWRemapAlt = null;
        public bool GetCharABCWidthsFloatWRemapAlt => ((bool?)(_GetCharABCWidthsFloatWRemapAlt ??= GetValue("RemapAlt", "GetCharABCWidthsFloatW").ToBoolean())).Value;

        bool? _GetCharABCWidthsFloatARemapAlt = null;
        public bool GetCharABCWidthsFloatARemapAlt => ((bool?)(_GetCharABCWidthsFloatARemapAlt ??= GetValue("RemapAlt", "GetCharABCWidthsFloatA").ToBoolean())).Value;

        bool? _GetGlyphOutlineWUndoRemap = null;
        public bool GetGlyphOutlineWUndoRemap => ((bool?)(_GetGlyphOutlineWUndoRemap ??= GetValue("UndoRemap", "GetGlyphOutlineW").ToBoolean())).Value;

        bool? _GetGlyphOutlineAUndoRemap = null;
        public bool GetGlyphOutlineAUndoRemap => ((bool?)(_GetGlyphOutlineAUndoRemap ??= GetValue("UndoRemap", "GetGlyphOutlineA").ToBoolean())).Value;

        bool? _GetGlyphOutlineWRemapAlt = null;
        public bool GetGlyphOutlineWRemapAlt => ((bool?)(_GetGlyphOutlineWRemapAlt ??= GetValue("RemapAlt", "GetGlyphOutlineW").ToBoolean())).Value;

        bool? _GetGlyphOutlineARemapAlt = null;
        public bool GetGlyphOutlineARemapAlt => ((bool?)(_GetGlyphOutlineARemapAlt ??= GetValue("RemapAlt", "GetGlyphOutlineA").ToBoolean())).Value;


        Dictionary<string, string>[] _FontRemaps;

        public Dictionary<string, string>[] FontRemaps { get {
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

        public Dictionary<string, bool> Modifiers {
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
        public Dictionary<string, bool> Hooks {
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
        public Dictionary<string, bool> Mods {
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
        public Filter Filter {
            get {
                if (_Filter.HasValue)
                    return _Filter.Value;

                _Filter = new Filter() { 
                    FromAsian = GetValue("FromAsian", "Filter").ToBoolean(),
                    DenyList = GetValue("DenyList", "Filter"),
                    IgnoreList = GetValue("IgnoreList", "Filter"),
                    QuoteList = GetValue("QuoteList", "Filter"),
                    Sensitivity = GetValue("Sensitivity", "Filter").ToInt32(),
                    UseDB = GetValue("UseDB", "Filter").ToBoolean(),
                    AllowNumbers = GetValue("AllowNumbers", "Filter").ToBoolean(),
                    RegexFilter = GetValue("RegexFilter", "Filter").ToBoolean(),
                    DumpFilter = GetValue("DumpFilter", "Filter").ToBoolean(),
                    DumpRegexFilter = GetValue("DumpRegexFilter", "Filter").ToBoolean(),
                    DumpAcceptableRange = GetValue("DumpAcceptableRange", "Filter").ToBoolean(),
                    AcceptableRange = CharacterRanges.GetRanges(GetValue("AcceptableRange", "Filter")).ToList()
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
                if (NewLines.Count > 0 && NewLines.Last().Trim() != string.Empty)
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
