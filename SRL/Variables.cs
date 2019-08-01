using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace SRL {
    partial class StringReloader {

        const string BreakLineFlag = "::BREAKLINE::";
        const string ReturnLineFlag = "::RETURNLINE::";
        const string AntiWordWrapFlag = "::NOWORDWRAP::";
        const string AntiMaskParser = "::NOMASK::";
        const string AntiPrefixFlag = "::NOPREFIX::";
        const string AntiSufixFlag = "::NOSUFIX::";
        const string MaskWordWrap = "::FULLWORDWRAP::";

        const string CfgName = "StringReloader";
        const string ServiceMask = "StringReloaderPipeID-{0}";

        const int CacheLength = 200;

        enum PipeCommands : byte {
            FindMissed = 0,
            AddMissed = 1,
            FindReload = 2,
            AddReload = 3,
            GetReload = 4,
            True = 5,
            False = 6,
            AddPtr = 7,
            GetPtrs = 8,
            EndPipe = 9,
            AddMask = 10,
            ChkMask = 11,
            RldMask = 12,
            AdvDB = 13,
            GetDBID = 14,
            SetDBID = 15
        }

        struct Range {
            internal uint Min;
            internal uint Max;
        }

        static int GamePID = System.Diagnostics.Process.GetCurrentProcess().Id;

        static Dictionary<ushort, char> CharRld;
        static Dictionary<ushort, char> UnkRld;
        static Dictionary<string, string> MskRld = null;
        static Dictionary<long, string> DBNames = null;
        static Dictionary<string, FontRedirect> FontReplaces = new Dictionary<string, FontRedirect>();


        static List<IntPtr> PtrCacheIn = new List<IntPtr>();
        static List<IntPtr> PtrCacheOut = new List<IntPtr>();
        static List<string> Missed = new List<string>();
        static List<string> Replys = new List<string>();
        static List<long> Ptrs = new List<long>();
        static List<Range> Ranges = null;


        static NamedPipeClientStream PipeClient = null;

        static bool Initialized = false;
        static bool LoadShowed = false;
        static bool ConsoleShowed = false;
        static bool DelayTest = false;
        static bool CmdLineChecked = false;
        static bool LogAll = false;
        static bool LogInput = false;
        static bool LogOutput = false;
        static bool DumpStrOnly = false;
        static bool DialogFound = false;
        static bool CloseEventAdded = false;
        static bool Unicode = false;
        static bool LogFile = false;
        static bool TrimRangeMismatch = false;
        static bool SpecialLineBreaker = false;
        static bool EnableWordWrap = false;
        static bool Multithread = false;
        static bool AntiCrash = false;
        static bool InvalidateWindow = false;
        static bool CachePointers = false;
        static bool FreeOnExit = false;
        static bool DialogCheck = true;
        static bool LiteralMaskMatch = false;
        static bool DecodeCharactersFromInput = false;
        static bool WindowHookRunning = false;
        static bool MultipleDatabases = false;
        static bool Managed = false;
        static bool NoReload = false;
        static bool NoTrim = false;
        static bool ReloadMaskParameters = false;
        static bool LiteMode = false;
        static bool RemoveIlegals = false;
        static bool AsianInput = false;
        static bool AutoUnks = false;
        static bool CaseSensitive = false;
        static bool NotCachedOnly = false;
        static bool AllowEmpty = false;

        static bool OverlayEnabled = false;
        static bool OverlayInitialized = false;
        static bool ShowNonReloads = false;
        static bool PaddingSeted = false;
        static int OPaddingTop;
        static int OPaddinLeft;
        static int OPaddinBottom;
        static int OPaddingRigth;

        static int ReplyPtr = 0;
        static int CacheArrPtr = 0;

        static int LogStack = 0;
        static int CursorX, CursorY;

        static string RldPrefix;
        static string RldSufix;

        static string[] DenyList;
        static string[] IgnoreList;
        static Quote[] QuoteList;
        static int Sensitivity;
        static bool UseDatabase;
        static bool ForceTrim;

#if DEBUG
        static bool HookCreateWindow;
        static bool HookSendMessage;
#endif
        static bool ImportHook;
        static bool HookCreateFile;
        static bool HookMultiByteToWideChar;
        static bool HookSetWindowText;
        static bool HookGlyphOutline;
        static bool HookTextOut;
        static bool HookExtTextOut;
        static bool HookCreateFont;
        static bool HookCreateFontIndirect;
        static byte FontCharset;
        static string FontFaceName;
        static bool UndoChars;

#if DEBUG
        static float LastDPI;
#endif

        static bool HookCreateWindowEx;
        static bool HookShowWindow;
        static bool HookSetWindowPos;
        static bool HookMoveWindow;
        static bool CheckProportion;
        static int Seconds;
        static int MinSize;

        static bool NoDatabase;

        static string StrLstSufix = string.Empty;
        static string SourceLang = string.Empty;
        static string TargetLang = string.Empty;
        static string LastInput = string.Empty;
        static string GameLineBreaker = "\n";
        static string LastOutput = string.Empty;
        static string CustomDir = string.Empty;
        static string CustomCredits = string.Empty;
        
        static System.Drawing.Font Font;
        static bool Monospaced;
        static bool FakeBreakLine;
        static uint MaxWidth;

        static DotNetVM TLIB = null;
        static DotNetVM EncodingModifier = null;
        static DotNetVM StringModifier = null;
        static DotNetVM Overlay = null;

        static bool DirectRequested = false;
        
        static string[] Replaces = new string[0];
        static string TLMap => BaseDir + "Strings.srl";
        static string TLMapSrc => BaseDir + "Strings.lst";
        static string TLMapSrcMsk => BaseDir + "Strings-{0}.lst";
        static string CharMapSrc => BaseDir + "Chars.lst";
        static string IntroMsk => BaseDir + "Intro{0}.{1}";
        static string TLDP => BaseDir + "TLIB.dll";
        static string OEDP => BaseDir + "Overlay.dll";
        static string IniPath => AppDomain.CurrentDomain.BaseDirectory + "Srl.ini";
        static string MTLCache => BaseDir + "MTL.lst";
        static string ReplLst =>  BaseDir + "Replaces.lst";
        static string SrlDll => System.Reflection.Assembly.GetCallingAssembly().Location;

        static string BaseDir => AppDomain.CurrentDomain.BaseDirectory + CustomDir;

        static Encoding ReadEncoding = Encoding.Default;
        static Encoding WriteEncoding = Encoding.Default;

        static BinaryReader PipeReader = null;
        static BinaryWriter PipeWriter = null;

        static int LastDBID = 0;
        static int DBID = 0;
        static List<Dictionary<string, string>> Databases = null;
        static Dictionary<string, string> StrRld {
            get {
                if (Databases == null)
                    Databases = new List<Dictionary<string, string>>() { null };

                if (DBID >= Databases.Count)
                    Databases.Add(new Dictionary<string, string>());

                if (DBID >= Databases.Count)
                    throw new Exception("GET - Invalid Database ID");

                return Databases[DBID];
            }
            set {
                if (Databases == null)
                    Databases = new List<Dictionary<string, string>>() { null };

                if (DBID >= Databases.Count)
                    Databases.Add(new Dictionary<string, string>());

                if (DBID >= Databases.Count)
                    throw new Exception("SET - Invalid Database ID");

                Databases[DBID] = value;
            }
        }

        static bool? DbgFlg = null;
        static bool _FrcDbg = false;
        static bool Debugging {
            get {
                if (_FrcDbg)
                    return true;

                if (DbgFlg.HasValue)
                    return DbgFlg.Value;

                DbgFlg = File.Exists(BaseDir + "DEBUG");

                return DbgFlg.Value;
            }
            set {
                _FrcDbg = value;
            }
        }

        
        private static IntPtr hConsole = IntPtr.Zero;
        private static bool _hdlFail = false;
        private static IntPtr _hdl = IntPtr.Zero;
        static IntPtr GameHandler {
            get {
                if (_hdl != IntPtr.Zero || _hdlFail)
                    return _hdl;

                Thread Work = new Thread(() => {
                    _hdl = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                    string title = WindowTitle;
                });
                Work.Start();

                try {
                    DateTime Begin = DateTime.Now;
                    while ((Work.IsAlive || Work.IsBackground) && (DateTime.Now - Begin).TotalSeconds <= 2)
                        continue;
                    Work?.Abort();
                } catch { }

                //Optimization
                if (_hdl == IntPtr.Zero)
                    _hdlFail = true;

                return _hdl;
            }
        }

        private static float DPI { get {
                var g = System.Drawing.Graphics.FromHwnd(GameHandler);
                return g.DpiX;
            }
        }


        private static string _tlt = string.Empty;
        private static string WindowTitle {
            get {
                if (_tlt == string.Empty) {
                    _tlt = System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle;
                }
                return _tlt;
            }
        }


        private static TextWriter _LogWriter = null;
        private static TextWriter LogWriter {
            get {
                if (_LogWriter == null) {
                    _LogWriter = File.AppendText(BaseDir + "SRL.log");
                }
                return _LogWriter;
            }
        }

        private static int _netstas = -1;
        private static DateTime LastTry = DateTime.Now;
        private static bool Online {
            get {
                if (_netstas == 1 || IsWine)
                    return true;
                if (_netstas == -1 || (DateTime.Now - LastTry).TotalMinutes > 10) {
                    Ping myPing = new Ping();
                    string host = "google.com";
                    byte[] buffer = new byte[32];
                    int timeout = 1000;
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                    _netstas = (reply.Status == IPStatus.Success ? 1 : 0);
                }
                LastTry = DateTime.Now;
                return _netstas == 1;
            }
            set {
                _netstas = value ? 1 : 0;
            }
        }

        static bool? isWine;
        internal static bool IsWine {
            get {
                if (isWine.HasValue) return isWine.Value;

                IntPtr hModule = GetModuleHandle(@"ntdll.dll");
                if (hModule == IntPtr.Zero)
                    isWine = false;
                else {
                    IntPtr fptr = GetProcAddress(hModule, @"wine_get_version");
                    isWine = fptr != IntPtr.Zero;
                }

                return isWine.Value;
            }
        }
        internal static string SRLVersion {
            get {
                var Version = System.Diagnostics.FileVersionInfo.GetVersionInfo(SrlDll);
                return Version.FileMajorPart + "." + Version.FileMinorPart;
            }
        }
        private static bool GameStarted() {
            try {
                return !string.IsNullOrWhiteSpace(System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle);
            } catch {
                return false;
            }
        }
        
        static Thread SettingsWatcher = null;

        static string[] MatchDel = new string[] {
            "\r", "\\r", "\n", "\\n", " ", "_r", "―", "-", "*", "♥", "①", "♪"
        };

        static string[] TrimChars = new string[] { "%K", "%LC", "♪", "%P" };


        static IntroContainer[] Introduction;
    }
}
