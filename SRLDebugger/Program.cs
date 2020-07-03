using StringReloads;
using StringReloads.Engine.String;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SRLDebugger
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
                args = new[] { "-help" };

            for (int i = 0; i < args.Length; i++) {
                string Arg = args[i].TrimStart('-', '/', '\\', ' ').ToLowerInvariant();
                switch (Arg) {
                    case "filter":
                        DbgFilter(args[++i]);
                        break;
                    case "lstunique":
                        DbgLSTUnique(args[++i], args[++i]);
                        break;
                    case "help":
                        Console.WriteLine("SRL Debug Tool");
                        Console.WriteLine($"{Path.GetFileName(Assembly.GetExecutingAssembly().Location)} -filter PlainText.txt\t\tRun the Dialogue filter in a file");
                        Console.WriteLine($"{Path.GetFileName(Assembly.GetExecutingAssembly().Location)} -LSTUnique FileA.lst FileB.lst\tAllow only a unique reload in both LSTs");
                        Console.ReadKey();
                        Console.WriteLine();
                        break;
                }
            }
        }

        static void DbgFilter(string FileName) {
            if (!File.Exists(FileName))
                return;

            string[] Lines = File.ReadAllLines(FileName);
            Lines = (from x in Lines where x.IsDialogue(UseDB: false) select x).ToArray();
            File.WriteAllLines(FileName, Lines);
        }

        static void DbgLSTUnique(string FileA, string FileB)
        {
            if (!File.Exists(FileA) || !File.Exists(FileB))
                return;

            List<string> LSTA = new List<string>();
            var LST = File.ReadAllLines(FileA);
            for (int i = 0; i < LST.Length; i += 2)
            {
                LSTA.Add(StringReloads.Engine.SRL.MinifyString(LST[i]));
            }
            LST = File.ReadAllLines(FileB);
            List<string> NewB = new List<string>();
            for (int i = 0; i < LST.Length; i += 2)
            {
                string LineA = StringReloads.Engine.SRL.MinifyString(LST[i]);
                string LineB = LST[i + 1];
                if (LSTA.Contains(LineA))
                    continue;
                NewB.Add(LST[i]);
                NewB.Add(LineB);
            }

            File.WriteAllLines(FileB, NewB.ToArray());
        }
    }
}
