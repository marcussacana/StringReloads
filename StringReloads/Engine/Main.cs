using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StringReloads.AutoInstall;
using StringReloads.Engine.Interface;
using StringReloads.Engine.String;
using StringReloads.Hook;
using StringReloads.Mods;
using StringReloads.StringModifier;

namespace StringReloads.Engine
{
    public unsafe class Main
    {
        internal IPlugin[] _Plugins = null;
        public IPlugin[] Plugins => _Plugins ??=
            (from Asm in AppDomain.CurrentDomain.GetAssemblies()
             from Typ in Asm.GetTypes()
             where typeof(IPlugin).IsAssignableFrom(Typ) && !Typ.IsInterface
             select (IPlugin)Activator.CreateInstance(Typ)).OrderBy(x => x.Name).ToArray();

        internal IMatch[] _Matchs = null;
        public IMatch[] Matchs => _Matchs ??= new IMatch[] {
            new RegexMatch(this),
            new Match(this)
        };

        internal Initializer Initializer = new Initializer();

        internal Config Settings = new Config();

        public List<Database> Databases = new List<Database>();

        internal IStringModifier[] _ReloadModifiers = null;
        public IStringModifier[] ReloadModifiers => _ReloadModifiers ??= new IStringModifier[] {
            new MonoWordWrap(this),
            new Remaper(this),
            new Escape()
        };

        internal Hook.Base.Hook[] _Hooks = null;
        public Hook.Base.Hook[] Hooks => _Hooks ??= new Hook.Base.Hook[] {
            new CreateFontA(),
            new CreateFontW(),
            new CreateFontIndirectA(),
            new CreateFontIndirectW(),
            new GetGlyphOutlineA(),
            new GetGlyphOutlineW(),
            new SysAllocString(),
            new MultiByteToWideChar(),
            new WideCharToMultiByte()
        };

        internal Mods.Base.IMod[] _Mods = null;
        public Mods.Base.IMod[] Mods => _Mods ??= new Mods.Base.IMod[] {
            new PatchRedir()
        };


        internal AutoInstall.Base.IAutoInstall[] _Installers = null;
        internal AutoInstall.Base.IAutoInstall[] Installers => _Installers ??= new AutoInstall.Base.IAutoInstall[] {
            new AdvHD(),
            new SoftPalMethodA()
        };

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

        internal bool Initialized;

        internal byte* ProcessString(CString String) {
            var Rst = ProcessString((string)String);
            if (Rst == null)
                return String;
            return (CString)Rst;
        }
        internal byte* ProcessString(WCString String) {
            var Rst = ProcessString((string)String);
            if (Rst == null)
                return String;
            return (WCString)Rst;
        }
        public string ProcessString(string String)
        {
            if (!Initialized)
                Initializer.Initialize(this);

            if (Settings.CacheOutput && RecentOutput.Contains(String))
                return null;

            var Matched = MatchString(String);
            if (Matched == null)
            {
                Log.Trace($"Input: {String}");
                return null;
            }

            TriggerFlags(Matched?.OriginalFlags);
            TriggerFlags(Matched?.TranslationFlags);

            //Execute Reload Modifiers
            string Reloaded = Matched?.TranslationLine;
            foreach (var Modifier in ReloadModifiers)
                Reloaded = Modifier.Apply(Reloaded, Matched?.OriginalLine);

            Log.Trace($"Reload from:\r\n{Matched?.OriginalLine}\r\nTo:\r\n{Reloaded}");

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

        public bool HasMatch(string String) => HasMatch(null, String);
        public bool HasMatch(IMatch This, string String)
        {
            foreach (var Match in Matchs)
            {
                if (Match == This)
                    continue;

                var Rst = Match.HasMatch(String);
                if (Rst)
                    return true;
            }
            return false;
        }
        public bool HasValue(string String) => HasValue(null, String);
        public bool HasValue(IMatch This, string String)
        {
            foreach (var Match in Matchs)
            {
                if (Match == This)
                    continue;

                var Rst = Match.HasValue(String);
                if (Rst)
                    return true;
            }
            return false;
        }

        public LSTEntry? MatchString(string String) => MatchString(null, String);
        public LSTEntry? MatchString(IMatch This, string String)
        {
            foreach (var Match in Matchs) {
                if (Match == This)
                    continue;

                var Rst = Match.MatchString(String);
                if (Rst != null)
                    return Rst;
            }
            return null;
        }
        public char ResolveRemap(char Char) => ResolveRemap(null, Char);
        public char ResolveRemap(IMatch This, char Char)
        {
            if (!Initialized)
                Initializer.Initialize(this);

            foreach (var Match in Matchs) {
                if (Match == This)
                    continue;

                var Rst = Match.ResolveRemap(Char);
                if (Rst != null)
                    return Rst.Value;
            }
            return Char;
        }

        public FontRemap? ResolveRemap(string Face, int Width, int Height, uint Charset) => ResolveRemap(null, Face, Width, Height, Charset);
        public FontRemap? ResolveRemap(IMatch This, string Face, int Width, int Height, uint Charset) {
            foreach (var Match in Matchs) {
                if (Match == This)
                    continue;

                var Rst = Match.ResolveRemap(Face, Width, Height, Charset);
                if (Rst != null)
                    return Rst.Value;
            }
            return null;
        }

        public void EnableHook(Hook.Base.Hook Hook) {
            Array.Resize(ref _Hooks, _Hooks.Length + 1);
            _Hooks[_Hooks.Length - 1] = Hook;
            _Hooks[_Hooks.Length - 1].Install();
        }

        public string Minify(string String) {
            return StringModifier.Minify.Default.Apply(String, null);
        }
    }
}
