using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

using StringReloads.AutoInstall;
using StringReloads.Engine.String;
using StringReloads.Hook;
using StringReloads.Mods;
using StringReloads.StringModifier;

namespace StringReloads.Engine
{
    unsafe class Main
    {
        internal Match _Match;
        internal Match Match => _Match ?? (_Match = new Match(this));

        internal Initializer Initializer = new Initializer();

        internal Config Settings;

        internal List<Database> Databases = new List<Database>();

        internal IStringModifier[] _ReloadModifiers = null;
        internal IStringModifier[] ReloadModifiers => _ReloadModifiers ?? (_ReloadModifiers = new IStringModifier[] {
            new MonoWordWrap(this),
            new Remaper(this),
            new Escape()
        });

        internal Hook.Base.Hook[] _Hooks = null;
        internal Hook.Base.Hook[] Hooks => _Hooks ?? (_Hooks = new Hook.Base.Hook[] {
            new CreateFontA(),
            new CreateFontW(),
            new CreateFontIndirectA(),
            new CreateFontIndirectW(),
            new GetGlyphOutlineA(),
            new GetGlyphOutlineW(),
            new SysAllocString(),
            new MultiByteToWideChar()
        });

        internal Mods.Base.IMod[] _Mods = null;
        internal Mods.Base.IMod[] Mods => _Mods ?? (_Mods = new Mods.Base.IMod[] {
            new PatchRedir()
        });


        internal AutoInstall.Base.IAutoInstall[] _Installers = null;
        internal AutoInstall.Base.IAutoInstall[] Installers => _Installers ?? (_Installers = new AutoInstall.Base.IAutoInstall[] {
            new AdvHD(),
            new SoftPalMethodA()
        });

        internal int CurrentDatabaseIndex = 0;
        
        internal Database CurrentDatabase {
            get {
                if (CurrentDatabaseIndex >= Databases.Count)
                    throw new Exception("No Database Loaded");

                return Databases[CurrentDatabaseIndex];
            }
        }

        internal Dictionary<char, char> CharRemap = new Dictionary<char, char>();

        internal bool Initialized;

        internal char ProcessChar(char Char)
        {
            if (!Initialized)
                Initializer.Initialize(this);

            return Match.ResolveRemap(Char);
        }

        internal byte* ProcessString(CString String)
        {

            if (!Initialized)
                Initializer.Initialize(this);

            var Matched = Match.MatchString(String);
            if (Matched == null) {
                Log.Trace($"Input: {(string)String}");
                return String;
            }

            TriggerFlags(Matched?.OriginalFlags);
            TriggerFlags(Matched?.TranslationFlags);

            //Execute Reload Modifiers
            string Reloaded = Matched?.TranslationLine;
            foreach (var Modifier in ReloadModifiers)
                Reloaded = Modifier.Apply(Reloaded); 
            
            Log.Trace($"Reload from:\r\n{Matched?.OriginalLine}\r\nTo:\r\n{Reloaded}");

            return (CString)Reloaded;
        }

        public event Types.FlagTrigger OnFlagTriggered;
        private void TriggerFlags(LSTParser.LSTFlag[] Flags) {
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

        public void EnableHook(Hook.Base.Hook Hook) {
            Array.Resize(ref _Hooks, _Hooks.Length + 1);
            _Hooks[_Hooks.Length - 1] = Hook;
            _Hooks[_Hooks.Length - 1].Install();
        }
    }
}
