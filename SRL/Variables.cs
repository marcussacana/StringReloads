using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        const string CfgName = "StringReloader";
        const string ServiceMask = "StringReloaderPipeID-{0}";

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

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
            EndPipe = 9
        }

        struct Range {
            internal uint Min;
            internal uint Max;
        }


        static Dictionary<string, string> StrRld = null;
        static Dictionary<ushort, char> CharRld;
        static Dictionary<ushort, char> UnkRld;
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
        static bool LogString = false;
        static bool DumpStrOnly = false;
        static bool DialogFound = false;
        static bool CloseEventAdded = false;
        static bool Unicode = false;
        static bool LogFile = false;
        static bool TrimRangeMissmatch = false;
        static bool SpecialLineBreaker = false;
        static bool EnableWordWrap = false;
        static bool Multithread = false;

        static int ReplyPtr = 0;

        static string StrLstSufix = string.Empty;
        static string SourceLang = string.Empty;
        static string TargetLang = string.Empty;
        static string LastInput = string.Empty;
        static string GameLineBreaker = "\n";
        
        static System.Drawing.Font Font;
        static bool Monospaced;
        static uint MaxWidth;

        static DotNetVM TLIB = null;
        static DotNetVM Modifier = null;

        static string[] Replaces = new string[0];
        static string TLMap => AppDomain.CurrentDomain.BaseDirectory + "Strings.srl";
        static string TLMapSrc => AppDomain.CurrentDomain.BaseDirectory + "Strings.lst";
        static string TLMapSrcMsk => AppDomain.CurrentDomain.BaseDirectory + "Strings-{0}.lst";
        static string CharMapSrc => AppDomain.CurrentDomain.BaseDirectory + "Chars.lst";
        static string TLDP => AppDomain.CurrentDomain.BaseDirectory + "TLIB.dll";
        static string IniPath => AppDomain.CurrentDomain.BaseDirectory + "Srl.ini";
        static string MTLCache => AppDomain.CurrentDomain.BaseDirectory + "MTL.lst";
        static string ReplLst => AppDomain.CurrentDomain.BaseDirectory + "Replaces.lst";
        static string SrlDll => System.Reflection.Assembly.GetCallingAssembly().Location;

        static Encoding ReadEncoding = Encoding.Default;
        static Encoding WriteEncoding = Encoding.Default;

        static BinaryReader PipeReader = null;
        static BinaryWriter PipeWriter = null;

        static bool _FrcDbg = false;
        static bool Debugging {
            get {
                if (_FrcDbg)
                    return true;
                return File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DEBUG");
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
                Thread Task = new Thread(() => {
                    string title = WindowTitle;
                    _hdl = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                });
                Task.Start();
                try {
                    DateTime Begin = DateTime.Now;
                    while ((Task.IsAlive || Task.IsBackground) && (DateTime.Now - Begin).TotalSeconds <= 2)
                        continue;
                    Task?.Abort();
                } catch { }

                //Optimization
                if (_hdl == IntPtr.Zero)
                    _hdlFail = true;

                return _hdl;
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
                    _LogWriter = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "SRL.log");
                }
                return _LogWriter;
            }
        }

        private static int _netstas = -1;
        private static DateTime LastTry = DateTime.Now;
        private static bool Online {
            get {
                if (_netstas == 1)
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

        private static bool GameStarted() {
            try {
                return !string.IsNullOrWhiteSpace(System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle);
            } catch {
                return false;
            }
        }

        static string[] MatchDel = new string[] {
            "\r", "\\r", "\n", "\\n", " ", "_r", "―", "-", "*", "♥", "①", "♪"
        };

        static string[] TrimChars = new string[] {
            " ", "'", "<", "(", "[", "“", "［", "《", "«",
            "「", "『", "【", "]", "”", "］", "》",
            "»", "」", "』", "】", ")", ">", "‘", "’", "〃", "″",
            "～", "~", "―", "-", "%K", "%LC", "♪", "%P"
        };

        static string DenyChars = "@,§,$,_,<,>,/,[,],#";
    }
}
