//#define DebugPlugin
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SacanaWrapper
{
    public class Wrapper
    {
        string ImportPath = string.Empty;
        string ExportPath = string.Empty;
        string StrIP = string.Empty;
        string StrEP = string.Empty;
        private static string Lastest = string.Empty;
        DotNetVM Plugin;


        public string[] Import(string ScriptPath, bool PreventCorrupt = false, bool TryLastPluginFirst = false) {
            byte[] Script = File.ReadAllBytes(ScriptPath);
            string Extension = Path.GetExtension(ScriptPath);
            return Import(Script, Extension, PreventCorrupt, TryLastPluginFirst);
        }
        public string[] Import(byte[] Script, string Extension = null, bool PreventCorrupt = false, bool TryLastPluginFirst = false) {
            string[] Strings = null;
            string PluginDir = DotNetVM.AssemblyDirectory + "\\Plugins";       

            if (File.Exists(Lastest) && TryLastPluginFirst) {
#if !DebugPlugin
                try {
#endif
                    Strings = TryImport(Lastest, Script);
                    if (!Corrupted(Strings))
                        return Strings;
#if !DebugPlugin
            } catch { }
#endif
            }

            string[] Plugins = GetFiles(PluginDir, "*.inf|*.ini|*.cfg");

            List<string> Extensions = GetExtensions(Plugins);

            //Prepare Input Extension
            if (Extension != null && Extension.StartsWith(".")) {
                Extension = Extension.Substring(1, Extension.Length - 1);
            }
            if (Extension != null)
                Extension = Extension.ToLower();


            //Initial Detection
            if (Extension != null && CountMatch(Extensions, Extension) > 0) {
                uint Fails = 0;
                foreach (string Plugin in Plugins) {
                    string PExt = Ini.GetConfig("Plugin", "Extensions;Extension;Ext;Exts;extensions;extension;ext;exts", Plugin, false);
                    if (string.IsNullOrEmpty(PExt))
                        continue;
                    List<string> Exts = new List<string>(PExt.ToLower().Split('|'));
                    if (Exts.Contains(Extension)) {
#if !DebugPlugin
                        try {
#endif
                            Strings = TryImport(Plugin, Script);
                            if (Corrupted(Strings) && ++Fails < CountMatch(Extensions, Extension)) {
                                StrIP = ImportPath;
                                StrEP = ExportPath;
                                continue;
                            }
                            return Strings;
#if !DebugPlugin
                    } catch { }
#endif
                }
                }
            }

            //Brute Detection
            foreach (string Plugin in Plugins) {
#if !DebugPlugin
                try {
#endif
                    Strings = TryImport(Plugin, Script);
                    if (Corrupted(Strings)) {
                        StrIP = ImportPath;
                        StrEP = ExportPath;
                        continue;
                    }
                    return Strings;
#if !DebugPlugin
                } catch { }
#endif
            }
            if (Strings == null)
                throw new Exception("Supported Plugin Not Found.");

            if (Corrupted(Strings) && PreventCorrupt)
                return new string[0];
            ImportPath = StrIP;
            ExportPath = StrEP;
            return Strings;
        }

        private uint CountMatch(List<string> Strings, string Pattern) {
            return (uint)(from x in Strings where x == Pattern select x).LongCount(); ;
        }

        private List<string> GetExtensions(string[] Plugins) {
            List<string> Exts = new List<string>();
            foreach (string Plugin in Plugins) {
                string PExt = Ini.GetConfig("Plugin", "Extensions;Extension;Ext;Exts;extensions;extension;ext;exts", Plugin, false);
                if (string.IsNullOrEmpty(PExt))
                    continue;
                foreach (string ext in PExt.ToLower().Split('|'))
                    Exts.Add(ext);
            }
            return Exts;
        }

        private bool Corrupted(string[] Strings) {
            if (Strings.Length == 0)
                return true;

            char[] Corrupts = new char[] { '・' };

            uint Matchs = 0;
            foreach (string str in Strings) {
                if (str.Trim('\x0').Contains('\x0') || (from c in str.Trim('\x0') where (c & 0x7700) == 0x7700 || c < 10 || Corrupts.Contains(c) select c).Count() != 0)
                    Matchs++;
				else if (string.IsNullOrWhiteSpace(str))
					Matchs++;
            }

            if (Matchs > Strings.Length / 2)
                return true;

            return false;
        }

        public void Export(string[] Strings, string SaveAs) {
            byte[] Script = Export(Strings);
            File.WriteAllBytes(SaveAs, Script);
        }

        public byte[] Export(string[] Strings) {
            string[] Exp = ExportPath.Split('>');
            return (byte[])Plugin.Call(Exp[0], Exp[1], new object[] { Strings });
        }

        private string[] TryImport(string Plugin, byte[] Script) {
            ExportPath = Ini.GetConfig("Plugin", "Export;Exp;export;exp", Plugin, true);
            ImportPath = Ini.GetConfig("Plugin", "Import;Imp;import;imp", Plugin, true);
            string CustomSource = Ini.GetConfig("Plugin", "File;file;Archive;archive;Arc;arc", Plugin, false);

            string Path = System.IO.Path.GetDirectoryName(Plugin) + "\\",
             SourcePath = System.IO.Path.GetDirectoryName(Plugin) + "\\",
             SourcePath2 = System.IO.Path.GetDirectoryName(Plugin) + "\\";
             
            
            if (!string.IsNullOrWhiteSpace(CustomSource)){
                Path += CustomSource + ".dll";
                SourcePath += CustomSource + ".cs";
                SourcePath2 += CustomSource + ".vb";
            } else {
                Path += System.IO.Path.GetFileNameWithoutExtension(Plugin) + ".dll";
                SourcePath += System.IO.Path.GetFileNameWithoutExtension(Plugin) + ".cs";
                SourcePath2 += System.IO.Path.GetFileNameWithoutExtension(Plugin) + ".vb";
            }

            //Initialize Plugin
            bool InitializeWithScript = Ini.GetConfig("Plugin", "Initialize;InputOnCreate;initialize;inputoncreate", Plugin, false).ToLower() == "true";
            if (File.Exists(SourcePath))
                this.Plugin = new DotNetVM(File.ReadAllText(SourcePath, Encoding.UTF8), DotNetVM.Language.CSharp);
            else if (File.Exists(SourcePath2))
                this.Plugin = new DotNetVM(File.ReadAllText(SourcePath2, Encoding.UTF8), DotNetVM.Language.VisualBasic);
            else
                this.Plugin = new DotNetVM(File.ReadAllBytes(Path));

            //Import
            Lastest = Plugin;
            string[] Imp = ImportPath.Split('>');
            if (InitializeWithScript) {
                this.Plugin.StartInstance(Imp[0], Script);
                return (string[])this.Plugin.Call(Imp[0], Imp[1]);
            }
            return (string[])this.Plugin.Call(Imp[0], Imp[1], Script);

        }

        private static string[] GetFiles(string Dir, string Search) {
            string[] Result = new string[0];
            foreach (string pattern in Search.Split('|'))
                Result = Result.Union(Directory.GetFiles(Dir, pattern)).ToArray();
            return Result;
        }
    }
}
