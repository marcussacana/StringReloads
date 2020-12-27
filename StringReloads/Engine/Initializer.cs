using StringReloads.Engine.Unmanaged;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace StringReloads.Engine
{
    class Initializer
    {
        internal void Initialize(SRL Engine)
        {
            if (Engine.Initialized)
                return;

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//.net core

            if (Config.Default.Debug)
                System.Diagnostics.Debugger.Launch();

            Log.Information($"SRL - StringReloads v{Engine.Settings.SRLVersion}");
            Log.Information($"Created by Marcussacana");

            Log.Debug($"Working Directory: {Engine.Settings.WorkingDirectory}");
            Log.Debug("Initializing SRL...");

            if (File.Exists(Engine.Settings.CachePath))
            {
                var Cache = new Cache(Engine.Settings.CachePath);
                Engine.Databases = Cache.GetDatabases().ToList();
                Engine.CharRemap = Cache.GetRemaps().ToDictionary();
            }
            else
                BuildCache(Engine);

            Log.Debug($"{Engine.Databases.Count} Database(s) Loaded");
            Log.Debug($"{Engine.CharRemap.Count} Remap(s) Loaded");

            PluginsInitializer(Engine);

            if (Engine.Settings.LoadLocalFont)
                FontUtility.LoadLocalFonts();

            ModifiersInitializer(Engine);
            HooksInitializer(Engine);
            ModsInitializer(Engine);

            AutoInstall(Engine);

            EnableExceptionHandler(Engine);

            Engine.Initialized = true;
            Log.Information("SRL Initialized");
        }

        private void ModifiersInitializer(SRL Engine)
        {
            var Mods = Engine.ReloadModifiers;
            var Settings = Engine.Settings.Modifiers;
            for (int i = 0; i < Mods.Length; i++)
            {
                if (!Settings.ContainsKey(Mods[i].Name.ToLowerInvariant()))
                {
                    Log.Warning($"Modifier \"{Mods[i].Name}\" is without configuration.");
                    continue;
                }

                if (!Settings[Mods[i].Name.ToLowerInvariant()])
                {
                    Mods = Mods.Remove(Mods[i--]);
                    continue;
                }

                Log.Debug($"String Modifier \"{Mods[i].Name}\" Enabled.");
            }

            Engine._ReloadModifiers = Mods;
        }

        private void HooksInitializer(SRL Engine)
        {
            var Hooks = Engine.Hooks;
            var Settings = Engine.Settings.Hooks;
            for (int i = 0; i < Hooks.Length; i++)
            {
                if (!Settings.ContainsKey(Hooks[i].Name.ToLowerInvariant()))
                {
                    Log.Warning($"Hook \"{Hooks[i].Name}\" is without configuration.");
                    continue;
                }

                if (!Settings[Hooks[i].Name.ToLowerInvariant()])
                {
                    Hooks = Hooks.Remove(Hooks[i--]);
                    continue;
                }
            }

            Engine._Hooks = Hooks;

            for (int i = 0; i < Hooks.Length; i++)
            {
                Hooks[i].Install();

                Log.Debug($"Hook \"{Hooks[i].Name}\" Enabled.");
            }
        }

        private void ModsInitializer(SRL Engine)
        {
            var Mods = Engine.Mods;
            var Settings = Engine.Settings.Mods;
            for (int i = 0; i < Mods.Length; i++)
            {
                if (!Settings.ContainsKey(Mods[i].Name.ToLowerInvariant()))
                {
                    Log.Warning($"Mod \"{Mods[i].Name}\" is without configuration.");
                    continue;
                }

                if (!Settings[Mods[i].Name.ToLowerInvariant()])
                {
                    Mods = Mods.Remove(Mods[i--]);
                    continue;
                }
            }

            Engine._Mods = Mods;

            for (int i = 0; i < Mods.Length; i++)
            {
                if (!Engine.Mods[i].IsCompatible())
                    continue;

                Mods[i].Install();

                Log.Debug($"Mod \"{Mods[i].Name}\" Enabled.");
            }
        }

        private void AutoInstall(SRL Engine)
        {
            if (!Engine.Settings.AutoInstall)
                return;

            for (int i = 0; i < Engine.Installers.Length; i++)
            {
                if (!Engine.Installers[i].IsCompatible())
                    continue;

                Log.Information($"{Engine.Installers[i].Name} Engine Detected.");
                Engine.Installers[i].Install();
            }
        }

        private void PluginsInitializer(SRL Engine)
        {
            _ = Engine.ReloadModifiers;
            _ = Engine.Installers;
            _ = Engine.Reloads;
            _ = Engine.Matchs;
            _ = Engine.Hooks;
            _ = Engine.Mods;

            string PluginDir = Path.Combine(Path.GetDirectoryName(EntryPoint.CurrentDll), "Plugins");
            if (Directory.Exists(PluginDir))
            {
                foreach (string PluginPath in Directory.EnumerateFiles(PluginDir, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        AppDomain.CurrentDomain.Load(File.ReadAllBytes(PluginPath));
                    }
                    catch { }
                }
            }

            foreach (var Plugin in Engine.Plugins)
            {
                try
                {
                    Plugin.Initialize(Engine);
                    Log.Debug($"Plugin \"{Plugin.Name}\" Initialized.");

                    try
                    {
                        AppendArray(ref Engine._ReloadModifiers, Plugin.GetModifiers(), true);
                    }
                    catch { }
                    try
                    {
                        AppendArray(ref Engine._Installers, Plugin.GetAutoInstallers(), true);
                    }
                    catch { }
                    try
                    {
                        AppendArray(ref Engine._Matchs, Plugin.GetMatchs(), true);
                    }
                    catch { }
                    try
                    {
                        AppendArray(ref Engine._Hooks, Plugin.GetHooks(), true);
                    }
                    catch { }
                    try
                    {
                        AppendArray(ref Engine._Mods, Plugin.GetMods(), true);
                    }
                    catch { }
                    try
                    {
                        AppendArray(ref Engine._Reloads, Plugin.GetReloaders(), true);
                    }
                    catch { }
                    try
                    {
                        AppendArray(ref Engine.Encodings, Plugin.GetEncodings(), true);
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to Load the Plugin \"{Plugin.Name}\".\n{ex}");
                }
            }
        }

        private void AppendArray<T>(ref T[] Arr, T[] ArrAppend, bool InTop = false)
        {
            if (ArrAppend == null)
                return;

            if (InTop)
            {
                Array.Reverse(Arr);
                Array.Reverse(ArrAppend);
            }
            Array.Resize(ref Arr, Arr.Length + ArrAppend.Length);
            ArrAppend.CopyTo(Arr, Arr.Length - ArrAppend.Length);

            if (InTop)
            {
                Array.Reverse(Arr);
                Array.Reverse(ArrAppend);
            }
        }

        private void BuildCache(SRL Engine)
        {
            Log.Debug("Cache not found, Building database...");
            Engine.Databases = new List<Database>();
            Engine.CurrentDatabaseIndex = 0;
            Engine.CharRemap = new Dictionary<char, char>();

            if (!Directory.Exists(Engine.Settings.WorkingDirectory))
                Directory.CreateDirectory(Engine.Settings.WorkingDirectory);

            string[] SpecialLSTs = new string[] { "chars", "regex", "mtl" };

            foreach (string Lst in Directory.GetFiles(Engine.Settings.WorkingDirectory, "*.lst"))
            {
                var Parser = new LSTParser(Lst);

                if (SpecialLSTs.Contains(Parser.Name.ToLowerInvariant()))
                    continue;

                Database DB = new Database(Parser.Name);
                DB.AddRange(Parser.GetEntries());

                Log.Debug($"Database {Parser.Name} Initialized (ID: {Engine.Databases.Count})");
                Engine.Databases.Add(DB);
            }

            string CharsFile = Path.Combine(Engine.Settings.WorkingDirectory, "Chars.lst");

            if (File.Exists(CharsFile))
            {
                foreach (string Line in File.ReadAllLines(CharsFile))
                {
                    if (!Line.Contains('=') || string.IsNullOrWhiteSpace(Line))
                        continue;

                    string PartA = Line.Substring(0, Line.IndexOf('='));
                    string PartB = Line.Substring(Line.IndexOf('=') + 1);

                    if (PartA.Length > 1)
                        PartA = PartA.Trim();
                    if (PartB.Length > 1)
                        PartB = PartB.Trim();

                    char A, B;

                    if (PartA.ToLowerInvariant().StartsWith("0x"))
                    {
                        PartA = PartA.Substring(2);
                        A = (char)Convert.ToInt16(PartA, 16);
                    }
                    else
                        A = PartA.First();

                    if (PartB.ToLowerInvariant().StartsWith("0x"))
                    {
                        PartB = PartB.Substring(2);
                        B = (char)Convert.ToInt16(PartB, 16);
                    }
                    else
                        B = PartB.First();

                    Log.Debug($"Character Remap from {A} to {B}");
                    Engine.CharRemap[A] = B;
                }
            }

            Cache Builder = new Cache(Engine.Settings.CachePath);
            Builder.BuildDatabase(Engine.Databases.ToArray(), Engine.CharRemap.ToArray());
        }

        private void EnableExceptionHandler(SRL Engine)
        {
            if (Engine.Settings.Debug)
            {
                AppDomain.CurrentDomain.FirstChanceException += (sender, Args) => Log.Error(Args.Exception.ToString());
                AppDomain.CurrentDomain.UnhandledException += (sender, Args) =>
                {
                    if (Args.IsTerminating)
                        Log.Critical(Args.ExceptionObject.ToString());
                    else
                        Log.Error(Args.ExceptionObject.ToString());
                };
            }
        }
    }

    internal static partial class Extensions
    {
        public static Dictionary<char, char> ToDictionary(this IEnumerable<KeyValuePair<char, char>> Pairs)
        {
            var Dic = new Dictionary<char, char>();
            foreach (var Pair in Pairs)
                Dic.Add(Pair.Key, Pair.Value);
            return Dic;
        }

        public static T[] Remove<T>(this T[] Arr, T Item)
        {
            List<T> Rst = new List<T>();
            for (int i = 0; i < Arr.Length; i++)
            {
                if (Arr[i].Equals(Item))
                    continue;

                Rst.Add(Arr[i]);
            }
            return Rst.ToArray();
        }
    }
}
