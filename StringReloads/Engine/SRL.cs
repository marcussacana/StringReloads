using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using StringReloads.AutoInstall;
using StringReloads.Engine.Interface;
using StringReloads.Engine.Match;
using StringReloads.Engine.String;
using StringReloads.Engine.Unmanaged;
using StringReloads.Hook;
using StringReloads.Hook.Win32;
using StringReloads.Mods;
using StringReloads.StringModifier;

namespace StringReloads.Engine
{
    public unsafe class SRL
    {
        internal IPlugin[] _Plugins = null;
        public IPlugin[] Plugins => _Plugins ??=
            (from Asm in AppDomain.CurrentDomain.GetAssemblies()
             from Typ in Helpers.TryGet(Asm.GetTypes) ?? new Type[0]
             where typeof(IPlugin).IsAssignableFrom(Typ) && !Typ.IsInterface
             select (IPlugin)Activator.CreateInstance(Typ)).OrderBy(x => x.Name).ToArray();

        internal IMatch[] _Matchs = null;
        public IMatch[] Matchs => _Matchs ??= new IMatch[] {
            new RegexMatch(this),
            new BasicMatch(this),
            new TrimMatch(this)
        };

        internal Initializer Initializer = new Initializer();

        internal Config Settings = new Config();

        public List<Database> Databases = new List<Database>();

        public HashSet<string> Hashset = new HashSet<string>();

        internal IStringModifier[] _ReloadModifiers = null;
        public IStringModifier[] ReloadModifiers => _ReloadModifiers ??= new IStringModifier[] {
            new MonoWordWrap(this),
            new Remaper(this),
            new RemaperAlt(this),
            new Escape()
        };

        internal Hook.Base.Hook[] _Hooks = null;
        public Hook.Base.Hook[] Hooks => _Hooks ??= new Hook.Base.Hook[] {
            new CreateFontA(),
            new CreateFontW(),
            new CreateFontIndirectA(),
            new CreateFontIndirectW(),
            new ExtTextOutA(),
            new ExtTextOutW(),
            new GetCharABCWidthsFloatA(),
            new GetCharABCWidthsFloatW(),
            new GetCharacterPlacementA(),
            new GetCharacterPlacementW(),
            new GetGlyphOutlineA(),
            new GetGlyphOutlineW(),
            new GetTextExtentPoint32A(),
            new GetTextExtentPoint32W(),
            new lstrcpyA(),
            new lstrcpyW(),
            new SysAllocString(),
            new MultiByteToWideChar(),
            new WideCharToMultiByte(),
            new TextOutA(),
            new TextOutW()
        };

        internal IMod[] _Mods = null;
        public IMod[] Mods => _Mods ??= new IMod[] {
            new CoInitializeFix(),
            new ForceExit(),
            new PatchRedir()
        };

        internal IReloader[] _Reloads = null;
        public IReloader[] Reloads => _Reloads ??= new IReloader[] { };


        internal IAutoInstall[] _Installers = null;
        internal IAutoInstall[] Installers => _Installers ??= new IAutoInstall[] {
            new AdvHD(),
            new ExHIBIT(),
            new SoftPalMethodA(),
            new SoftPalMethodB(),
            new CMVS()
        };

        internal IEncoding[] Encodings = new IEncoding[0];

        internal int CurrentDatabaseIndex = 0;
        
        public Database CurrentDatabase {
            get {
                if (CurrentDatabaseIndex >= Databases.Count)
                    throw new Exception("No Database Loaded");

                return Databases[CurrentDatabaseIndex];
            }
        }

        public Queue<string> RecentOutput = new Queue<string>();

        internal Dictionary<char, char> CharRemap = new Dictionary<char, char>();
        internal Dictionary<char, char> CharRemapAlt = new Dictionary<char, char>();

        internal bool Initialized;

        internal byte* ProcessString(CString pString) {
            if (!Initialized)
                Initializer.Initialize(this);

            if (Settings.SanityCheck && SanityChecks.IsBadCodePtr(pString))
                return pString;

            var String = pString;
            foreach (var Rld in Reloads)
            {
                var New = Rld.BeforeReload(String, false);
                if (New != null)
                    String = New;
            }

            var Rst = ProcessString((string)String);
            if (Rst == null)
                return String;
            var Output = (CString)Rst;

            foreach (var Rld in Reloads)
            {
                var New = Rld.AfterReload(pString, Output, false);
                if (New != null)
                    Output = New;
            }

            if (Settings.Overwrite && Output != null && ((string)Output) != null)
                return (CString)Alloc.Overwrite(Output.ToArray(), pString);

            if (Settings.HeapAlloc && Output != null && ((string)Output) != null)
                return (CString)Alloc.CreateHeap(Output.ToArray());

            return Output;
        }
        internal byte* ProcessString(WCString pString) {
            if (!Initialized)
                Initializer.Initialize(this);

            if (Settings.SanityCheck && SanityChecks.IsBadCodePtr(pString))
                return pString;

            var String = pString;
            foreach (var Rld in Reloads) {
                var New = Rld.BeforeReload(String, true);
                if (New != null)
                    String = New;
            }

            var Rst = ProcessString((string)String);
            if (Rst == null)
                return String;
            var Output = (WCString)Rst;

            foreach (var Rld in Reloads)
            {
                var New = Rld.AfterReload(pString, Output, true);
                if (New != null)
                    Output = New;
            }

            if (Settings.Overwrite && Output != null && ((string)Output) != null)
                return (WCString)Alloc.Overwrite(Output.ToArray(), pString);

            if (Settings.HeapAlloc && Output != null && ((string)Output) != null)
                return (WCString)Alloc.CreateHeap(Output.ToArray());

            return Output;
        }

        public string ProcessString(string String)
        {
            if (Settings.CacheOutput && RecentOutput.Contains(String))
                return null;

            var Matched = MatchString(String);
            if (Matched == null) {
                Log.Trace($"Input: {String}");
                return null;
            }

            TriggerFlags(Matched?.OriginalFlags);
            TriggerFlags(Matched?.TranslationFlags);

            //Execute Reload Modifiers
            string Reloaded = Matched?.TranslationLine;

            string Prefix = String.GetStartTrimmed();
            string Sufix = String.GetEndTrimmed();

            if (!Reloaded.StartsWith(Prefix))
                Reloaded = Prefix + Reloaded;

            if (!Reloaded.EndsWith(Sufix))
                Reloaded += Sufix;

            foreach (var Modifier in ReloadModifiers)
                Reloaded = Modifier.Apply(Reloaded, Matched?.OriginalLine);

            Log.Debug($"Reload from:\r\n{Matched?.OriginalLine}\r\nTo:\r\n{Reloaded}");

            if (Settings.CacheOutput)
            {
                if (RecentOutput.Count == 100)
                    RecentOutput.Dequeue();
                RecentOutput.Enqueue(Reloaded);
            }

            return Reloaded;
        }

        public event Types.FlagTrigger OnFlagTriggered;
        private void TriggerFlags(LSTFlag[] Flags) {
            var Cancel = new CancelEventArgs();
            foreach (var Flag in Flags) {
                Log.Trace($"Triggering Flag {Flag.Name}");
                foreach (Types.FlagTrigger Trigger in OnFlagTriggered.GetInvocationList()) { 
                    if (Cancel.Cancel)
                        break;

                    Trigger(Flag, Cancel);
                }
            }
        }

        internal List<object> HasMatchLocks = new List<object>();
        public bool HasMatch(string String) => HasMatch(null, String);
        public bool HasMatch(IMatch This, string String)
        {
            if (This != null && !HasMatchLocks.Contains(This))
                HasMatchLocks.Add(This);

            foreach (var Match in Matchs)
            {
                if (HasMatchLocks.Contains(Match))
                    continue;

                var Rst = Match.HasMatch(String);
                if (Rst) {
                    if (HasMatchLocks.Contains(This))
                        HasMatchLocks.Remove(This);
                    return true;
                }
            }

            if (HasMatchLocks.Contains(This))
                HasMatchLocks.Remove(This);

            return false;
        }

        internal List<object> HasValueLocks = new List<object>();
        public bool HasValue(string String) => HasValue(null, String);
        public bool HasValue(IMatch This, string String)
        {
            if (This != null && !HasValueLocks.Contains(This))
                HasValueLocks.Add(This);

            foreach (var Match in Matchs)
            {
                if (HasValueLocks.Contains(Match))
                    continue;

                var Rst = Match.HasValue(String);
                if (Rst) {
                    if (HasValueLocks.Contains(This))
                        HasValueLocks.Remove(This);
                    return true;
                }
            }

            if (HasValueLocks.Contains(This))
                HasValueLocks.Remove(This);

            return false;
        }

        internal List<object> MatchStringLocks = new List<object>();
        public LSTEntry? MatchString(string String) => MatchString(null, String);
        public LSTEntry? MatchString(IMatch This, string String)
        {
            if (string.IsNullOrWhiteSpace(String))
                return null;

            if (This != null && !MatchStringLocks.Contains(This))
                MatchStringLocks.Add(This);

            foreach (var Match in Matchs)
            {
                if (MatchStringLocks.Contains(Match))
                    continue;

                var Rst = Match.MatchString(String);
                if (Rst != null) {
                    if (MatchStringLocks.Contains(This))
                        MatchStringLocks.Remove(This);
                    return Rst;
                }
            }

            if (MatchStringLocks.Contains(This))
                MatchStringLocks.Remove(This);

            return null;
        }
        internal List<object> ResolveRemapLocks = new List<object>();
        public char ResolveRemap(char Char) => ResolveRemap(null, Char);
        public char ResolveRemap(IMatch This, char Char)
        {
            if (!Initialized)
                Initializer.Initialize(this);

            if (This != null && !ResolveRemapLocks.Contains(This))
                ResolveRemapLocks.Add(This);

            foreach (var Match in Matchs)
            {
                if (ResolveRemapLocks.Contains(Match))
                    continue;

                var Rst = Match.ResolveRemap(Char);
                if (Rst != null) {
                    if (ResolveRemapLocks.Contains(This))
                        ResolveRemapLocks.Remove(This);
                    return Rst.Value;
                }
            }

            if (ResolveRemapLocks.Contains(This))
                ResolveRemapLocks.Remove(This);

            return Char;
        }

        public FontRemap? ResolveRemap(string Face, int Width, int Height, uint Charset) => ResolveRemap(null, Face, Width, Height, Charset);
        public FontRemap? ResolveRemap(IMatch This, string Face, int Width, int Height, uint Charset) {
            if (This != null && !ResolveRemapLocks.Contains(This))
                ResolveRemapLocks.Add(This);

            foreach (var Match in Matchs)
            {
                if (ResolveRemapLocks.Contains(Match))
                    continue;

                var Rst = Match.ResolveRemap(Face, Width, Height, Charset);
                if (Rst != null) {
                    if (ResolveRemapLocks.Contains(This))
                        ResolveRemapLocks.Remove(This);
                    return Rst.Value;
                }
            }

            if (ResolveRemapLocks.Contains(This))
                ResolveRemapLocks.Remove(This);

            return null;
        }

        public void EnableHook<T>(T NewHook) where T : Hook.Base.Hook {
            foreach (var ReadyHook in _Hooks)
                if (ReadyHook.Name == NewHook.Name)
                    return;

            EnableHook((Hook.Base.Hook)NewHook);
        }

        void EnableHook(Hook.Base.Hook Hook) {
            Array.Resize(ref _Hooks, _Hooks.Length + 1);
            _Hooks[_Hooks.Length - 1] = Hook;
            _Hooks[_Hooks.Length - 1].Install();
        }

        public string Minify(string String) => MinifyString(String);
        public static string MinifyString(string String) {
            return StringModifier.Minify.Default.Apply(String, null);
        }
    }
}
