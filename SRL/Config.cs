using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SRL
{
    partial class StringReloader
    {
        /// <summary>
        /// Load All Configs from the INI file.
        /// </summary>
        private static void LoadConfig()
        {
            SpecialLineBreaker = false;
            EnableWordWrap = false;
            AntiCrash = false;
            DecodeCharactersFromInput = false;
            InvalidateWindow = false;
            LiteralMaskMatch = false;
            DisableMasks = false;
            DialogCheck = true;
            FreeOnExit = false;
            CachePointers = false;
            TrimRangeMismatch = false;
            Unicode = false;
            MultipleDatabases = false;
            OverlayEnabled = false;
            NoReload = false;
            NoTrim = false;
            ReloadMaskParameters = false;
            LiteMode = false;
            RemoveIlegals = false;
            AsianInput = false;
            QuoteList = new Quote[0];
            Sensitivity = 3;
            UseDatabase = false;
            AutoUnks = false;
            UndoChars = false;
            CaseSensitive = false;
            ForceTrim = false;
            NotCachedOnly = false;
            AllowEmpty = false;
            ImportHook = false;
            HookCreateWindowEx = false;
            HookShowWindow = false;
            HookSetWindowPos = false;
            HookMoveWindow = false;
            CheckProportion = false;

            TagCleaner = false;
            IgnoreTag = false;

            Seconds = 0;
            MinSize = 0;

            DenyList = new string[0];
            IgnoreList = new string[0];
            TagChars = string.Empty;
            RldPrefix = string.Empty;
            RldSufix = string.Empty;

            SRLSettings Settings;
            OverlaySettings OverlaySettings;
            WordwrapSettings WordwrapSettings;
            FilterSettings FilterSettings;
            HookSettings HookSettings;
            IntroSettings IntroSettings;
            AdvancedIni.FastOpen(out Settings, IniPath);
            AdvancedIni.FastOpen(out OverlaySettings, IniPath);
            AdvancedIni.FastOpen(out WordwrapSettings, IniPath);
            AdvancedIni.FastOpen(out FilterSettings, IniPath);
            AdvancedIni.FastOpen(out HookSettings, IniPath);
            AdvancedIni.FastOpen(out IntroSettings, IniPath);

            Log(Initialized ? "Reloading Settings..." : "Loading Settings...", true);


            if (Settings.LiteMode)
            {
                LiteMode = true;
            }

            if (Settings.InEncoding != null)
            {
                Log("Loading Read Encoding Config...", true);
                ReadEncoding = ParseEncodingName(Settings.InEncoding);
            }

            if (Settings.OutEncoding != null)
            {
                Log("Loading Write Encoding Config...", true);
                WriteEncoding = ParseEncodingName(Settings.OutEncoding);

                if (Debugging)
                    Console.OutputEncoding = WriteEncoding;
            }

            if (Settings.Wide)
            {
                Log("Wide Character Mode Enabled...", true);
                Unicode = true;
            }

            if (Settings.Multithread)
            {
                if (Initialized && !Multithread)
                {
                    Warning("The Multithread Settings Changed - Restart Required");
                }
                else
                {
                    Log("Multithreaded Game Support Enabled", true);
                    Multithread = true;
                }
            }
            else if (Initialized && Multithread)
            {
                Warning("The Multithread Settings Changed - Restart Required");
            }

            if (!string.IsNullOrWhiteSpace(FilterSettings.DenyList))
            {
                Log("Custom Denied Chars List Loaded...", true);
                DenyList = FilterSettings.DenyList.Split(',');
            }

            if (FilterSettings.UseDB)
            {
                Log("Filter with Database Enabled", true);
                UseDatabase = true;
            }

            if (Settings.TrimRangeMismatch)
            {
                Log("Trim Mismatch Ranges Enabled...", true);
                TrimRangeMismatch = true;
            }

            if (Settings.CachePointers)
            {
                Warning("Pointer Cache Enabled...", true);
                CachePointers = true;
            }

            if (Settings.FreeOnExit)
            {
                Warning("Memory Leak Prevention Enabled...", true);
                FreeOnExit = true;
            }

            if (Settings.NoDialogCheck)
            {
                Warning("Dialog Check Disabled...", true);
                DialogCheck = false;
            }

            if (Settings.LiteralMaskMatch)
            {
                Log("Literal Mask Matching Enabled...", true);
                LiteralMaskMatch = true;
            }
            if (Settings.MultiDatabase)
            {
                Log("Multidatabase's Matching Method Enabled...", true);
                MultipleDatabases = true;
            }

            if (Settings.AsianInput)
            {
                Log("Asian Text Mode Enabled...", true);
                AsianInput = true;
            }

            if (Settings.AutoUnks)
            {
                Log("Auto Unk Char Reload Enabled...", true);
                AutoUnks = true;
            }

            if (Settings.DisableMask)
            {
                DisableMasks = true;
                Log("Masks Reloader Disabled...", true);
            }

            if (Settings.WindowHook)
            {
                Log("Enabling Window Reloader...", true);
                new Thread(() => WindowHook()).Start();

                if (Settings.InvalidateWindow)
                {
                    Log("Invalidate Window Mode Enabled.", true);
                    InvalidateWindow = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(Settings.AcceptableRanges))
            {
                LoadRanges();
            }

            if (Settings.DecodeFromInput)
            {
                Log("Enabling Character Reloader Decoding From Input...", true);
                DecodeCharactersFromInput = true;
            }

            if (Settings.NoTrim)
            {
                Log("Disabling Trim Service...", true);
                NoTrim = true;
            }

            if (Settings.ReloadMaskParameters)
            {
                Log("Enabling Mask Parameters Reloader...", true);
                ReloadMaskParameters = true;

                if (!Settings.Multithread)
                {
                    Warning("You can't use the PIPE service with the Mask Parameter Reloader Feature, Enabling Multithreaded Game Support...");
                    Settings.Multithread = true;
                    Multithread = true;
                }
            }

            if (Settings.NoReload)
            {
                NoReload = true;
                Warning("String Injection Disabled by User...", true);
            }

            if (Settings.RemoveViolations)
            {
                RemoveIlegals = true;
                Warning("Violation remover enabled, please, consider manual repair...");
            }

            if (Settings.NotCachedOnly)
            {
                NotCachedOnly = true;
                Log("Not Cached Only Reloader Mode Enabled", true);
            }

            if (Settings.AllowEmptyReloads)
            {
                AllowEmpty = true;
                Log("Empty Reloader Filter Disabled", true);
            }

            if (!Initialized && Settings.AllowDuplicates)
            {
                AllowDuplicates = true;
                Log("Duplicate Reload Support Enabled", true);
            }
            else if (Settings.AllowDuplicates != AllowDuplicates)
                Warning("Duplicate Reload Support Changed - Restart Required");

            if (Settings.SetOutputEncoding)
            {
                Log("Console Output Encoding Changed", false);
                if (!Debugging)
                {
                    ConsoleShowed = false;
                    HideConsole();
                }
            }

            RldPrefix = Settings.ReloadedPrefix;
            RldSufix = Settings.ReloadedSufix;

            CaseSensitive = Settings.CaseSensitive;

            if (OverlaySettings.Enable)
            {
                OverlayEnabled = true;
                ShowNonReloads = OverlaySettings.ShowNonReloaded;

                if (!File.Exists(OEDP))
                    Error("Can't Enabled the Overlay Because the Overlay.dll is missing.");
                else
                    Log("Overlay Allowed.", true);
            }

            if (!string.IsNullOrWhiteSpace(Settings.CustomCredits))
            {
                CustomCredits = Settings.CustomCredits;

                //Just if you wanna hide from kids trying fake it. :)
                if (CustomCredits.StartsWith("Fx"))
                {
                    byte[] Coded = ParseHex(CustomCredits.Replace("Fx", "0x"));
                    for (int i = 0; i < Coded.Length; i++)
                        Coded[i] ^= 0xFF;

                    CustomCredits = Encoding.UTF8.GetString(Coded);
                }

                if (CustomCredits.StartsWith("0x"))
                {
                    CustomCredits = Encoding.UTF8.GetString(ParseHex(CustomCredits));
                }

            }

            if (!string.IsNullOrWhiteSpace(OverlaySettings.Padding))
            {
                PaddingSeted = false;
                string Padding = OverlaySettings.Padding;
                foreach (string Parameter in Padding.Split('|'))
                {
                    try
                    {
                        string Border = Parameter.Split(':')[0].Trim().ToLower();
                        int Value = int.Parse(Parameter.Split(':')[1].Trim());
                        switch (Border)
                        {
                            case "top":
                                OPaddingTop = Value;
                                break;
                            case "bottom":
                                OPaddinBottom = Value;
                                break;
                            case "rigth":
                                OPaddingRigth = Value;
                                break;
                            case "left":
                                OPaddinLeft = Value;
                                break;
                            default:
                                Error("\"{0}\" Isn't a valid Padding Border", Border);
                                break;
                        }
                    }
                    catch
                    {
                        Error("\"{0}\" Isn't a valid Padding Parameter", Parameter);
                    }
                }
            }

            if (Settings.LiveSettings)
            {
                if (SettingsWatcher == null)
                {
                    Log("Enabling Live Settings....", true);
                    SettingsWatcher = new Thread(() =>
                    {
                        DateTime Before = new FileInfo(IniPath).LastWriteTime;
                        while (true)
                        {
                            DateTime Now = new FileInfo(IniPath).LastWriteTime;
                            if (Before != Now)
                            {
                                Before = Now;
                                Thread.Sleep(500);
                                LoadConfig();
                            }
                            Thread.Sleep(500);
                        }
                    });
                    SettingsWatcher.Start();
                }
            }
            else if (SettingsWatcher != null)
            {
                SettingsWatcher.Abort();
                SettingsWatcher = null;
            }

            if (Settings.AntiCrash)
            {
                if (!Initialized)
                {
                    Log("Enabling Crash Handler...", true);
                    System.Windows.Forms.Application.ThreadException += ProcessOver;
                    AppDomain.CurrentDomain.UnhandledException += ProcessOver;
                    AntiCrash = true;
                }
            }

            if (WordwrapSettings.Enabled)
            {
                Log("Wordwrap Enabled.", true);
                EnableWordWrap = true;
                MaxWidth = WordwrapSettings.Width;
                Monospaced = WordwrapSettings.Monospaced;
                FakeBreakLine = WordwrapSettings.FakeBreakLine;

                if (!Monospaced)
                    Font = new Font(WordwrapSettings.FontName, WordwrapSettings.Size, WordwrapSettings.Bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
            }

            if (!string.IsNullOrEmpty(Settings.GameLineBreaker))
            {
                GameLineBreaker = Settings.GameLineBreaker;
                SpecialLineBreaker = true;
            }

            if (!string.IsNullOrEmpty(FilterSettings.IgnoreList))
            {
                Log("Using Custom Ignore List...", true);
                string IgnoreList = FilterSettings.IgnoreList;
                if (!IgnoreList.StartsWith(">>"))
                    MatchDel = new string[0];
                else
                    IgnoreList = IgnoreList.Substring(2);
                foreach (string str in IgnoreList.Split(','))
                    if (str.Trim().StartsWith("0x"))
                    {
                        string Del = Encoding.UTF8.GetString(ParseHex(str.Trim()));
                        AppendArray(ref MatchDel, Del, true);
                    }
                    else
                        AppendArray(ref MatchDel, str.Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r"), true);
            }

            if (!string.IsNullOrEmpty(FilterSettings.TrimList))
            {
                Log("Using Custom Trim List...", true);
                TrimChars = new string[0];
                foreach (string str in FilterSettings.TrimList.Split(','))
                {
                    if (str.Trim().StartsWith("0x"))
                    {
                        string Trim = Encoding.UTF8.GetString(ParseHex(str.Trim()));
                        AppendArray(ref TrimChars, Trim, true);
                    }
                    else
                        AppendArray(ref TrimChars, str.Replace(BreakLineFlag, "\n").Replace(ReturnLineFlag, "\r"), true);
                }
            }
            if (!string.IsNullOrEmpty(FilterSettings.QuoteList))
            {
                Log("Using Custom Quotes...", true);
                foreach (string str in FilterSettings.TrimList.Split(','))
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    AppendArray(ref QuoteList, new Quote()
                    {
                        Start = str.First(),
                        End = str.Last()
                    }, true);
                    Ranges.Add(new Range()
                    {
                        Min = str.First(),
                        Max = str.First()
                    });
                    Ranges.Add(new Range()
                    {
                        Min = str.Last(),
                        Max = str.Last()
                    });
                }
            }

            if (FilterSettings.Sensitivity != 2)
            {
                Log("Dialogue Sensitivity Level Changed to {0}", true, FilterSettings.Sensitivity);
                Sensitivity = FilterSettings.Sensitivity;
            }

            if (!string.IsNullOrEmpty(FilterSettings.TagChars))
            {
                bool Set = true;
                if (FilterSettings.TagChars.Length != 2)
                {
                    if (FilterSettings.TagChars.Length < 2)
                    {
                        Error("Invalid Tag Char List.");
                        Set = false;
                    }
                    else {
                        Warning("Bad Tag Char List");
                    }
                }
                if (Set)
                {
                    TagChars = FilterSettings.TagChars;
                    Log("Tag Chars Set to {0} and {1}", true, TagChars[0], TagChars[1]);
                }
            }

            if (FilterSettings.ForceTrim)
            {
                Log("Dialogue Filter Trim Enforcement Enabled", true);
                ForceTrim = true;
            }

            if (FilterSettings.TagCleaner)
            {
                Log("Tag Cleaner Enabled", true);
                TagCleaner = true;
            }

            if (FilterSettings.IgnoreTag)
            {
                Log("Tag Ignore Mode Enabled", true);
                IgnoreTag = true;
            }

            if (!string.IsNullOrWhiteSpace(Settings.WorkDirectory))
            {
                CustomDir = Settings.WorkDirectory.TrimStart(' ', '\\', '/').Replace("/", "\\");
                if (!CustomDir.EndsWith("\\"))
                    CustomDir += '\\';

                if (!Directory.Exists(BaseDir))
                    Directory.CreateDirectory(BaseDir);

                Log("Custom Directory Loaded", true);
            }

            if (HookSettings.CreateFile)
            {
                if (!HookCreateFile)
                    InstallCreateFileHooks();
                HookCreateFile = true;
                Log("CreateFile Hook Enabled", true);
            }
            else if (HookGlyphOutline)
                Warning("CreateFile Hook Settings Changed - Restart Required");

            if (HookSettings.GetGlyphOutline)
            {
                if (!HookGlyphOutline)
                    InstallGlyphHooks();
                HookGlyphOutline = true;
                Log("GetGlyphOutline Hook Enabled", true);
            }
            else if (HookGlyphOutline)
                Warning("GetGlyphOutline Hook Settings Changed - Restart Required");

            if (HookSettings.TextOut)
            {
                if (!HookTextOut)
                    InstallTextOutHooks();
                HookTextOut = true;
                Log("TextOut Hook Enabled", true);
            }
            else if (HookTextOut)
                Warning("TextOut Hook Settings Changed - Restart Required");

            if (HookSettings.ExtTextOut)
            {
                if (!HookExtTextOut)
                    InstallExtTextOutHooks();
                HookExtTextOut = true;
                Log("ExtTextOut Hook Enabled", true);
            }
            else if (HookExtTextOut)
                Warning("ExtTextOut Hook Settings Changed - Restart Required");

            if (HookSettings.CreateFont)
            {
                if (!HookCreateFont)
                    InstallCreateFontHooks();
                HookCreateFont = true;
                Log("CreateFont Hook Enabled", true);
            }
            else if (HookCreateFont)
                Warning("CreateFont Hook Settings Changed - Restart Required");

            if (HookSettings.CreateFontIndirect)
            {
                if (!HookCreateFontIndirect)
                    InstallCreateFontIndirectHooks();
                HookCreateFontIndirect = true;
                Log("CreateFontIndirect Hook Enabled", true);
            }
            else if (HookCreateFontIndirect)
                Warning("CreateFontIndirect Hook Settings Changed - Restart Required");

#if DEBUG
            if (HookSettings.SendMessage) {
                if (!HookSendMessage)
                    InstallSendMessageHooks();
                HookSendMessage = true;
                Log("SendMessage Hook Enabled", true);
            } else if (HookSendMessage)
                Warning("SendMessage Hook Settings Changed - Restart Required");

            if (HookSettings.CreateWindow) {
                if (!HookCreateWindow)
                    InstallCreateWindowHooks();
                HookCreateWindow = true;
                Log("CreateWindow Hook Enabled", true);
            } else if (HookCreateWindow)
                Warning("CreateWindow Hook Settings Changed - Restart Required");

            if (HookSettings.CreateWindowEx) {
                if (!HookCreateWindowEx)
                    InstallCreateWindowExHooks();
                HookCreateWindowEx = true;
                Log("CreateWindowEx Hook Enabled", true);
            } else if (HookCreateWindowEx)
                Warning("CreateWindowEx Hook Settings Changed - Restart Required");
#endif
            if (HookSettings.SetWindowText)
            {
                if (!HookSetWindowText)
                    InstallSetWindowTextHooks();
                HookSetWindowText = true;
                Log("SetWindowText Hook Enabled", true);
            }
            else if (HookSetWindowText)
                Warning("SetWindowText Hook Settings Changed - Restart Required");

            if (HookSettings.LoadLibraryFix)
            {
                if (!LoadLibraryFix)
                    InstallLoadLibraryHooks();
                LoadLibraryFix = true;
                Log("LoadLibrary Hook Enabled", true);
            }
            else if (LoadLibraryFix)
                Warning("LoadLibrary Hook Settings Changed - Restart Required");

            new Thread(() =>
            {
                while (!DialogFound)
                    Thread.Sleep(100);
                if (HookSettings.MultiByteToWideChar)
                {
                    if (!HookMultiByteToWideChar)
                        InstallMultiByteToWideChar();
                    HookMultiByteToWideChar = true;
                    Log("MultiByteToWideChar Hook Enabled", true);
                }
                else if (HookMultiByteToWideChar)
                    Warning("MultiByteToWideChar Hook Settings Changed - Restart Required");
            }).Start();

            if (HookSettings.UndoChars)
            {
                UndoChars = true;
                Log("Hook Char Reloader Restoration Enabled", true);
            }

            if (HookSettings.FontCharset != 0)
            {
                FontCharset = HookSettings.FontCharset;
                Log("Font Hook Charset 0x{0:X2}", true, FontCharset);
            }

            if (!string.IsNullOrWhiteSpace(HookSettings.FaceName))
            {
                FontFaceName = HookSettings.FaceName;
                Log("Font Hook FaceName: {0}", true, FontFaceName);
            }
            else
            {
                FontFaceName = string.Empty;
                while (Ini.GetConfigStatus($"Hook.Font.{FontReplaces.Count}", "FromFont;From;SourceFont;Source", IniPath) == Ini.ConfigStatus.Ok)
                {
                    string KEY = $"Hook.Font.{FontReplaces.Count}";
                    string FN = Ini.GetConfig(KEY, "FromFont;From;SourceFont;Source", IniPath).Trim();
                    FontReplaces.Add(FN, new FontRedirect()
                    {
                        From = FN,
                        To = Ini.GetConfig(KEY, "ToFont;To;TargetFont;Target", IniPath),
                        Size = Ini.GetConfig(KEY, "ChageSize;Size;DiffSize;NewSize", IniPath)
                    });
                }
                if (FontReplaces.Count != 0)
                {
                    Log("{0} Font Replacement Loaded", true, FontReplaces.Count);
                }
            }

            if (HookSettings.AutoEngineHook && AutoEngine.IsCompatible())
            {
                Log("{0} Engine Detected, Auto SRL Installer Enabled", true, AutoEngine.Name);
                AutoEngine.InstallStrHook();
            }
            else if (HookSettings.AutoEngineHook)
            {
                Warning("No Supported Engine Detected, Auto SRL Installer Disabled", AutoEngine.Name);
            }
            else
            {
                if (AutoEngine.IsCompatible())
                    Log("{0} Engine Detected, Auto SRL Installer Disabled", true, AutoEngine.Name);

                AutoEngine.UninstallStrHook();
            }
            if (IntroSettings.CreateWindowEx)
            {
                HookCreateWindowEx = true;

                Log("Intro Injector (CreateWindowEx) Enabled", true);
            }

            if (IntroSettings.ShowWindow)
            {
                HookShowWindow = true;

                Log("Intro Injector (ShowWindow) Enabled", true);
            }

            if (IntroSettings.SetWindowPos)
            {
                HookSetWindowPos = true;

                Log("Intro Injector (SetWindowPos) Enabled", true);
            }
            if (IntroSettings.MoveWindow)
            {
                HookMoveWindow = true;

                Log("Intro Injector (MoveWindow) Enabled", true);
            }

            if (IntroSettings.CheckProportion)
            {
                CheckProportion = true;

                Log("Intro Injector Proportion Validator Enabled", true);
            }

            if (HookSettings.ImportHook)
            {
                ImportHook = true;
                Log("Import Table Hook Method Enabled", true);
            }

            if (IntroSettings.Seconds > 0)
                Seconds = IntroSettings.Seconds;

            if (IntroSettings.MinSize > 0)
                MinSize = IntroSettings.MinSize;

            Log("Settings Loaded.", true);

            if (Managed)
            {
                Log("Managed Mode Enabled, Enforcing Compatible Settings", true);
                //OverlayEnabled = false;
                WriteEncoding = ReadEncoding = Encoding.Unicode;
#if TRACE
                Multithread = true;
#endif
                if (Debugging)
                    LogFile = true;
            }
        }


        /// <summary>
        /// Load/Compile all Acceptable Character Ranges by the user config.
        /// </summary>
        private static void LoadRanges()
        {
            Ranges = new List<Range>();
            string RangeList = Ini.GetConfig(CfgName, "AcceptableRanges;AcceptableRange;ValidRange;ValidRanges", IniPath, true);
            for (int i = 0; i < RangeList.Length;)
            {
                char c = RangeList[i];
                char c2 = i + 1 < RangeList.Length ? RangeList[i + 1] : ' ';
                if (c2 == '-' && i + 2 < RangeList.Length)
                {
                    char c3 = RangeList[i + 2];
                    if (c <= c3)
                    {
                        Range Range = new Range()
                        {
                            Min = c,
                            Max = c3
                        };
                        if (!Ranges.Contains(Range))
                        {
                            Ranges.Add(Range);

                            if (LogAll)
                                Log("Range from {0} to {1} Added.", true, Range.Min, Range.Max);
                        }
                        else
                        {
                            Warning("Range from {0} to {1} Conflited.", true, Range.Min, Range.Max);
                        }
                    }
                    i += 3;
                }
                else
                {
                    Range Range = new Range()
                    {
                        Min = c,
                        Max = c
                    };
                    if (!Ranges.Contains(Range))
                    {
                        Ranges.Add(Range);
                        Log("Range from {0} to {1} Added.", true, Range.Min, Range.Max);
                    }
                    else
                    {
                        Log("Range from {0} to {1} Conflited.", true, Range.Min, Range.Max);
                    }
                    i++;
                }
            }
        }
    }
}
