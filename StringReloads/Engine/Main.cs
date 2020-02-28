using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            new SysAllocString()
        });

        internal Mods.Base.IMod[] _Mods = null;
        internal Mods.Base.IMod[] Mods => _Mods ?? (_Mods = new Mods.Base.IMod[] {
            new PatchRedir()
        });


        internal AutoInstall.Base.IAutoInstall[] _Installers = null;
        internal AutoInstall.Base.IAutoInstall[] Installers => _Installers ?? (_Installers = new AutoInstall.Base.IAutoInstall[] {
            new AdvHD()
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
            
            Log.Trace($"Reload from:\n{Matched?.OriginalLine}\nTo:\n{Reloaded}");

            //Generate Output String
            var Buffer = CString.GetBytes(Reloaded);
            var pBuffer = Marshal.AllocHGlobal(Buffer.Length);
            Marshal.Copy(Buffer, 0, pBuffer, Buffer.Length);

            return (byte*)pBuffer;
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


        public FontRemap? GetFontRemap(string Facename, int Width, int Height, uint Charset) {
            var Remap = (from x in Config.Default.FontRemaps where x["from"] == Facename select x).FirstOrDefault();

            if (Remap == null)
                Remap = (from x in Config.Default.FontRemaps where x["from"] == "*" select x).FirstOrDefault();

            Log.Trace($"Font \"{Facename}\", Width: {Width}, Height: {Height}, Charset: 0x{Charset:X2}");

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
                    Rst.Height = Width + nHeight.ToInt32();
                else
                    Rst.Height = nHeight.ToInt32();
            }
            else
                Rst.Height = Height;


            Log.Debug($"Font Remaped to {Rst.To}, Width: {Rst.Width}, Height: {Rst.Height}, Charset: 0x{Rst.Charset:X2}");

            return Rst;
        }

        public void EnableHook(Hook.Base.Hook Hook) {
            Array.Resize(ref _Hooks, _Hooks.Length + 1);
            _Hooks[_Hooks.Length - 1] = Hook;
            _Hooks[_Hooks.Length - 1].Install();
        }
    }
}
