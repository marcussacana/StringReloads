using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SRL {
    partial class StringReloader {
        internal static void ShowLoading() {            
            if (LoadShowed)
                return;
            LoadShowed = true;
            const string WaitMsg = "SRL - String Reloader\nSoft-Translation Engine by Marcussacana\nInitializing, Please Wait...";
            Graphics g = Graphics.FromHwnd(GameHandler);
            string Title = WindowTitle;

            SetWindowText(GameHandler, string.Format("StringReloader - Initializing... [{0}]", Title));

            PrintMessage(WaitMsg, -1);

            SetWindowText(GameHandler, Title + " - [SRL Initialized]");

            Thread.Sleep(4000);
            SetWindowText(GameHandler, Title);
        }

        private static void PrintMessage(string Text, int Seconds) {
            Font F = new Font("Consolas", 20.5f, FontStyle.Bold);
            Brush B = Brushes.BlueViolet;
            PointF Pos = new PointF(0, 0);
            var Render = Graphics.FromHwnd(GameHandler);
            long Time = -1;
            if (Seconds != -1)
                Time = Seconds * 1000;
            if (Time == -1) {
                while (!Initialized) {
                    try {
                        Render.DrawString(Text, F, B, Pos);
                        Render.Flush();
                        Application.DoEvents();
                        Thread.Sleep(1);
                    } catch { }
                }
            } else {
                while (Time > 0) {
                    try {
                        Time--;
                        Render.DrawString(Text, F, B, Pos);
                        Render.Flush();
                        Application.DoEvents();
                        Thread.Sleep(1);
                    } catch { }
                }
            }
            Render.Dispose();
            ForcePaint(GameHandler);
        }

        internal static void CheckArguments() {
            if (CmdLineChecked)
                return;
            CmdLineChecked = true;
            string[] Commands = Environment.GetCommandLineArgs();

            if (Ini.GetConfig(CfgName, "Debug", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-debug");
            if (Ini.GetConfig(CfgName, "Delay", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-delay");
            if (Ini.GetConfig(CfgName, "Log", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-log");
            if (Ini.GetConfig(CfgName, "Unsafe", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-unsafe");
            if (Ini.GetConfig(CfgName, "LogFile", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-logfile");
            if (Ini.GetConfig(CfgName, "DetectText", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-detectstr");
            if (Ini.GetConfig(CfgName, "Dump", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-dump");
            if (Ini.GetConfig(CfgName, "DumpRetail", IniPath, false).ToLower() == "true")
                AppendArray(ref Commands, "-dumpretail");

            if (Commands?.Length == 0)
                return;

            foreach (string Command in Commands)
                switch (Command.ToLower().TrimStart('-', '/', '\\', ' ')) {
                    case "showlogwindow":
                    case "showconsole":
                    case "console":
                    case "debug":
                        Debugging = true;
                        break;
                    case "logdelay":
                    case "delaytest":
                    case "pingtest":
                    case "delay":
                        Log("Delay Test Mode - Enabled");
                        DelayTest = true;
                        break;
                    case "recovery":
                    case "recoverytl":
                    case "dumptranslation":
                    case "dump":
                        if (!File.Exists(TLMap)) {
                            Log("How you want dump the string if you don't have one?");
                            continue;
                        }
                        DumpData();
                        break;
                    case "dumporiginal":
                    case "restorelist":
                    case "retranslate":
                    case "dumptotl":
                    case "dumpretail":
                        if (!File.Exists(TLMap)) {
                            Log("How you want dump the string if you don't have one?");
                            continue;
                        }
                        DumpData(true);
                        break;
                    case "reloaddb":
                    case "updatedb":
                    case "refreshdb":
                    case "refreshdatabase":
                    case "reconstruct":
                    case "rebuild":
                        if (!File.Exists(TLMapSrc))
                            continue;
                        if (File.Exists(TLMap))
                            File.Delete(TLMap);
                        CompileStrMap();
                        break;
                    case "unsafe":
                    case "nodialogcheck":
                    case "noverifyfrist":
                        DialogFound = true;
                        break;
                    case "log":
                    case "debugstring":
                    case "debugencoding":
                    case "encodingtest":
                        Log("String Deubugger Enabled");
                        LogString = true;
                        break;
                    case "dumptext":
                    case "dumpstr":
                    case "detectstr":
                    case "detectstring":
                    case "detecttext":
                    case "detecttxt":
                        Debugging = true;
                        Log("String Dumping Filter Enabled");
                        DumpStrOnly = true;
                        break;
                    case "about":
                    case "credits":
                        Log("Credits:\n-String Reloader - Soft-Translation Engine\nCreated by Marcussacana");
                        break;
                    case "logfile":
                    case "fileoutput":
                    case "debugfile":
                        LogFile = true;
                        break;
                    case "whatisthis":
                    case "wtf":
                    case "help":
                    case "?":
                        Log("Help:\nParamters:\tAction:\nDump\t\tDump all content of the Strings.srl to the game directory\nUnsafe\t\tDon't ignore strings while don't recive a text line.\nDumpText\tEnable Dialog Filter in the debugger dump (Latim only)\nDumpRetail\tDump the text to Retranslation\nRebuild\t\tForce rebuild the Strings.srl from the Strings.lst\nLogFile\tWrite debug output to a plain text file.\n\nDelay Test\tShow the delay from a string reload\nLog\t\tLog all string input/output bytes\nCredits\t\tShow credits message\nHelp\t\tShow this message");
                        break;
                }
        }

        internal static bool PECSVal(byte[] Data) {
            int PEStart = BitConverter.ToInt32(Data, 0x3c);
            int PECoffStart = PEStart + 4;
            int PEOptionalStart = PECoffStart + 20;
            int PECheckSum = PEOptionalStart + 64;
            uint checkSumInFile = BitConverter.ToUInt32(Data, PECheckSum);
            long checksum = 0;
            var top = Math.Pow(2, 32);

            for (var i = 0; i < Data.Length / 4; i++) {
                if (i == PECheckSum / 4) {
                    continue;
                }
                var dword = BitConverter.ToUInt32(Data, i * 4);
                checksum = (checksum & 0xffffffff) + dword + (checksum >> 32);
                if (checksum > top) {
                    checksum = (checksum & 0xffffffff) + (checksum >> 32);
                }
            }

            checksum = (checksum & 0xffff) + (checksum >> 16);
            checksum = (checksum) + (checksum >> 16);
            checksum = checksum & 0xffff;

            checksum += (uint)Data.LongLength;
            if (checkSumInFile != checksum)
                return false;
            return true;
        }

        internal static void Error(string Message, params object[] Format) {
            ConsoleColor Color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Log(Message, false, Format);
            Console.ForegroundColor = Color;
        }
        internal static void Warning(string Message, params object[] Format) {
            ConsoleColor Color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(Message, true, Format);
            Console.ForegroundColor = Color;
        }

        internal static void Log(string Message, bool Optional = false, params object[] Format) {
            try {
                if (LogFile) {
                    LogWriter.WriteLine("{0}: {1}", DateTime.Now.ToShortTimeString(), string.Format(Message, Format));
                    LogWriter.Flush();
                }
                if (!ConsoleShowed && (!Optional || Debugging)) {
                    IntPtr discard = GameHandler;//Initialize the handler before open the console
                    discard = IntPtr.Zero;
                    ConsoleShowed = true;
                    AllocConsole();
                    Console.Title = "SRL Engine - Debug Output";
                    Console.OutputEncoding = WriteEncoding;
                    hConsole = GetConsoleWindow();
                }
                if (!ConsoleShowed && !Debugging && Optional) {
                    return;
                }
                ShowWindow(hConsole, SW_SHOW);
                string Msg = string.Format(Message, Format);
                Console.WriteLine("{0}: {1}", DateTime.Now.ToShortTimeString(), Msg);
            } catch {

            }
        }

        public static void ForcePaint(IntPtr Handle) {
            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, (uint)(Flags.RDW_ERASE | Flags.RDW_INVALIDATE));
        }

        public enum Flags : uint {
            RDW_INVALIDATE = 0x0001,
            RDW_ERASE = 0x0004,
        }

        [DllImport("User32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr rec, IntPtr recptr, uint Flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
