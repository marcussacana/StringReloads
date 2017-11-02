using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

static class Ini {
    internal static string GetConfig(string Key, string Name, string CfgFile, bool Required = true) {
        string[] Lines = File.ReadAllLines(CfgFile, Encoding.UTF8);
        string AtualKey = string.Empty;
        foreach (string Line in Lines) {
            if (Line.StartsWith("[") && Line.EndsWith("]"))
                AtualKey = Line.Substring(1, Line.Length - 2);
            if (Line.StartsWith("!") || string.IsNullOrWhiteSpace(Line) || !Line.Contains("=") || Line.StartsWith("#") || Line.StartsWith(";"))
                continue;
            string[] Splited = Line.Split('=');
            string AtualName = Splited[0].Trim().ToLower();
            string Value = Splited[1];
            for (int i = 2; i < Splited.Length; i++)
                Value += '=' + Splited[i];
            if (Name.Split(';').Any(N => N.Trim().ToLower() == AtualName) && AtualKey == Key)
                return Value;
        }
        if (!Required)
            return string.Empty;
        throw new Exception(string.Format("Config Error:\n[{0}]\n{1}=...", Key, Name));
    }
    internal static void SetConfig(string Key, string Name, string Value, string CfgFile) {
        Status cfg = GetConfigStatus(Key, Name, CfgFile);
        if (cfg == Status.NoFile) {
            File.WriteAllText(CfgFile, "[" + Key + "]");
            cfg = Status.NoName;
        }
        string[] Lines = File.ReadAllLines(CfgFile, Encoding.UTF8);
        string AtualKey = string.Empty;
        if (cfg == Status.Ok) {
            for (int i = 0; i < Lines.Length; i++) {
                string Line = Lines[i];
                if (Line.StartsWith("[") && Line.EndsWith("]"))
                    AtualKey = Line.Substring(1, Line.Length - 2);
                if (Line.StartsWith("!") || string.IsNullOrWhiteSpace(Line) || !Line.Contains("=") || Line.StartsWith("#") || Line.StartsWith(";"))
                    continue;
                string AtualName = Line.Split('=')[0].Trim();
                if (Name.Split(';').Any(N => N.Trim() == AtualName.Trim()) && AtualKey == Key) {
                    Lines[i] = string.Format("{0}={1}", Name, Value);
                    break;
                }
            }
        }
        if (cfg == Status.NoName) {
            List<string> Cfgs = new List<string>();
            int KeyPos = 0;
            for (int i = 0; i < Lines.Length; i++) {
                if (string.Format("[{0}]", Key) == Lines[i])
                    KeyPos = i;
                Cfgs.Add(Lines[i]);
            }
            Cfgs.Insert(KeyPos + 1, string.Format("{0}={1}", Name, Value));
            Lines = Cfgs.ToArray();
        }
        if (cfg == Status.NoKey) {
            string[] NewLines = new string[Lines.Length + 3];
            Lines.CopyTo(NewLines, 0);
            NewLines[Lines.Length + 1] = string.Format("[{0}]", Key);
            NewLines[Lines.Length + 2] = string.Format("{0}={1}", Name, Value);
            Lines = NewLines;
        }
        File.WriteAllLines(CfgFile, Lines, Encoding.UTF8);
    }

    internal enum Status {
        NoFile, NoKey, NoName, Ok
    }
    internal static Status GetConfigStatus(string Key, string Name, string CfgFile) {
        if (!File.Exists(CfgFile))
            return Status.NoFile;
        string[] Lines = File.ReadAllLines(CfgFile, Encoding.UTF8);
        bool KeyFound = false;
        string AtualKey = string.Empty;
        foreach (string Line in Lines) {
            if (Line.StartsWith("[") && Line.EndsWith("]"))
                AtualKey = Line.Substring(1, Line.Length - 2);
            if (AtualKey == Key)
                KeyFound = true;
            if (Line.StartsWith("!") || string.IsNullOrWhiteSpace(Line) || !Line.Contains("=") || Line.StartsWith("#") || Line.StartsWith(";"))
                continue;

            string AtualName = Line.Split('=')[0].Trim().ToLower();
            if (Name.Split(';').Any(N => N.Trim().ToLower() == AtualName) && AtualKey == Key)
                return Status.Ok;
        }
        return KeyFound ? Status.NoName : Status.NoKey;
    }
}