using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

class FieldParmaters : Attribute {
    public string Name;
    public object DefaultValue;
}
class AdvancedIni {
    byte[] INI;

    public AdvancedIni() {
        INI = new byte[0];
    }
    public AdvancedIni(string[] Ini) {
        StringBuilder SB = new StringBuilder();
        foreach (string Line in Ini)
            SB.AppendLine(Line);

        INI = Encoding.UTF8.GetBytes(SB.ToString());
    }

    public AdvancedIni(string IniFile) {
        INI = File.ReadAllBytes(IniFile);
    }

    public AdvancedIni(byte[] Data) {
        INI = Data;
    }

    public static void FastOpen<T>(out T Struct, byte[] Data) {
        AdvancedIni Instance = new AdvancedIni(Data);
        Instance.Open(out Struct);
    }
    public static void FastOpen<T>(out T Struct, string[] Lines) {
        AdvancedIni Instance = new AdvancedIni(Lines);
        Instance.Open(out Struct);
    }
    public static void FastOpen<T>(out T Struct, string File) {
        AdvancedIni Instance = new AdvancedIni(File);
        Instance.Open(out Struct);
    }
    public static void FastSave<T>(T Struct, out byte[] Data) {
        AdvancedIni Instance = new AdvancedIni();
        Instance.Save(Struct, out Data);
    }
    public static void FastSave<T>(T Struct, out string[] Lines) {
        AdvancedIni Instance = new AdvancedIni();
        Instance.Save(Struct, out Lines);
    }
    public static void FastSave<T>(T Struct, string File) {
        AdvancedIni Instance = new AdvancedIni();
        Instance.Save(Struct, File);
    }
    public void Open<T>(out T Struct) {
        object OutStruct = (T)Activator.CreateInstance(default(T).GetType());
        Type StructType = OutStruct.GetType();
        string BlockName = StructType.Name;
        if (HasAttribute(StructType, "FieldParmaters")) {
            BlockName = GetAttributePropertyValue(StructType, "FieldParmaters", "Name");
        }

        FieldInfo[] Fields = StructType.GetFields();
        for (int x = 0; x < Fields.Length; x++) {
            FieldInfo Field = Fields[x];
            string Name = Field.Name;
            if (HasAttribute(Field, "FieldParmaters")) {
                Name = GetAttributePropertyValue(Field, "FieldParmaters", "Name");
            }

            var Status = Ini.GetConfigStatus(BlockName, Name, INI);
            if (Status != Ini.ConfigStatus.Ok) {
                if (HasAttribute(Field, "FieldParmaters")) {
                    Field.SetValue(OutStruct, GetAttributePropertyValue(Field, "FieldParmaters", "DefaultValue"));
                } else {
                    continue;
                }
            } else {
                string EncodedValue = Ini.GetConfig(BlockName, Name, INI, true);
                if (Field.FieldType.FullName.EndsWith("[]")) {
                    Type RealType = Type.GetType(Field.FieldType.FullName.Substring(0, Field.FieldType.FullName.Length - 2));
                    long Length = long.Parse(EncodedValue);
                    dynamic Content = Array.CreateInstance(RealType, Length);
                    for (long i = 0; i < Length; i++) {
                        Content[i] = GetInstance(RealType, Ini.GetConfig(BlockName, Name.Split(';')[0] + $".{i}", INI, true));
                    }

                    Field.SetValue(OutStruct, Content);
                } else {
                    Field.SetValue(OutStruct, GetInstance(Field.FieldType, EncodedValue));
                }
            }
        }

        Struct = (T)OutStruct;
    }

    public void Save<T>(T Struct, out byte[] Data) {
        Data = new byte[INI.LongLength];
        INI.CopyTo(Data, 0);
        Type StructType = Struct.GetType();
        string BlockName = StructType.Name;
        if (HasAttribute(StructType, "FieldParmaters")) {
            BlockName = GetAttributePropertyValue(StructType, "FieldParmaters", "Name");
        }
        FieldInfo[] Fields = StructType.GetFields();
        for (int x = 0; x < Fields.Length; x++) {
            FieldInfo Field = Fields[x];
            string Name = Field.Name;
            if (HasAttribute(Field, "FieldParmaters")) {
                Name = GetAttributePropertyValue(Field, "FieldParmaters", "Name");
            }
            dynamic Value = Field.GetValue(Struct);

            if (Field.FieldType.FullName.EndsWith("[]")) {
                Type RealType = Type.GetType(Field.FieldType.FullName.Substring(0, Field.FieldType.FullName.Length - 2));

                Data = Ini.SetConfig(BlockName, Name, Value.Length.ToString(), Data);

                for (long i = 0; i < Value.LongLength; i++) {
                    Data = Ini.SetConfig(BlockName, Name.Split(';')[0] + $".{i}", Value[i].ToString(), Data);
                }
            } else {
                Data = Ini.SetConfig(BlockName, Name, Value.ToString(), Data);
            }
        }

        INI = Data;
    }

    public void Save<T>(T Struct, out string[] Lines) {
        byte[] Output = new byte[0];
        Save(Struct, out Output);

        Lines = Encoding.UTF8.GetString(Output).Replace("\r\n", "\n").Split('\n');
    }

    public void Save<T>(T Struct, string File) {
        byte[] Output = new byte[0];
        Save(Struct, out Output);

        System.IO.File.WriteAllBytes(File, Output);
    }
    private dynamic GetInstance(Type Format, string Value) {
        if (string.IsNullOrEmpty(Value) && Format.FullName != Const.STRING)
            throw new Exception("Invalid Value");
        bool Hex = false;
        if (Value.Trim().StartsWith("0x")) {
            Hex = true;
            Value = Value.Trim().Substring(2);
        }
        switch (Format.FullName) {
            case Const.CHAR:
                return Value[0];
            case Const.DOUBLE:
                if (Hex)
                    return double.Parse(Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                return double.Parse(Value, CultureInfo.InvariantCulture);
            case Const.FLOAT:
                if (Hex)
                    throw new Exception("Float values can't be a hex");
                return float.Parse(Value, CultureInfo.InvariantCulture);
            case Const.INT16:
                if (Hex)
                    return short.Parse(Value, NumberStyles.HexNumber);
                return short.Parse(Value);
            case Const.INT32:
                if (Hex)
                    return int.Parse(Value, NumberStyles.HexNumber);
                return int.Parse(Value);
            case Const.INT64:
                if (Hex)
                    return long.Parse(Value, NumberStyles.HexNumber);
                return long.Parse(Value);
            case Const.INT8:
                if (Hex)
                    return sbyte.Parse(Value, NumberStyles.HexNumber);
                return sbyte.Parse(Value);
            case Const.STRING:
                return Value;
            case Const.UINT16:
                if (Hex)
                    return ushort.Parse(Value, NumberStyles.HexNumber);
                return ushort.Parse(Value);
            case Const.UINT32:
                if (Hex)
                    return uint.Parse(Value, NumberStyles.HexNumber);
                return uint.Parse(Value);
            case Const.UINT64:
                if (Hex)
                    return ulong.Parse(Value, NumberStyles.HexNumber);
                return ulong.Parse(Value);
            case Const.UINT8:
                if (Hex)
                    return byte.Parse(Value, NumberStyles.HexNumber);
                return byte.Parse(Value);
            case Const.BOOLEAN:
                string Lower = Value.ToLower();
                return (new string[] { "true", "yes", "on", "enable", "active", "enabled", "actived", "1" }).Contains(Lower);

            default:
                throw new Exception("Unexpected Field Type");
        }
    }
    private dynamic GetAttributePropertyValue(FieldInfo Fld, string AttributeName, string PropertyName) {
        foreach (Attribute tmp in Fld.GetCustomAttributes(true)) {
            Type Attrib = tmp.GetType();
            if (Attrib.Name != AttributeName)
                continue;
            foreach (FieldInfo Field in Attrib.GetFields()) {
                if (Field.Name != PropertyName)
                    continue;
                return Field.GetValue(tmp);
            }
            throw new Exception("Property Not Found");
        }
        throw new Exception("Attribute Not Found");
    }

    private dynamic GetAttributePropertyValue<T>(T Struct, string AttributeName, string PropertyName) {
        Type t = Struct.GetType();
        PropertyInfo Info = (from x in t.GetProperties() where x.Name.Contains("CustomAttributes") select x).First();
        dynamic Rst = Info.GetValue(Struct, new object[0]);

        foreach (dynamic tmp in Rst) {
            if (tmp.AttributeType.Name != AttributeName)
                continue;
            foreach (dynamic Field in tmp.NamedArguments) {
                if (Field.MemberName != PropertyName)
                    continue;
                return Field.TypedValue.Value;
            }
            throw new Exception("Property Not Found");
        }
        throw new Exception("Attribute Not Found");
    }


    private bool HasAttribute(FieldInfo Field, string Attrib) {
        foreach (Attribute attrib in Field.GetCustomAttributes(true))
            if (attrib.GetType().Name == Attrib)
                return true;
        return false;
    }
    private bool HasAttribute<T>(T Struct, string Attrib) {
        Type t = Struct.GetType();
        PropertyInfo Info = (from x in t.GetProperties() where x.Name.Contains("CustomAttributes") select x).First();
        dynamic Rst = Info.GetValue(Struct, new object[0]);
        foreach (dynamic attrib in Rst)
            if (attrib.AttributeType.Name == Attrib)
                return true;
        return false;
    }
}

internal class Const {
    public const string INT8 = "System.SByte";
    public const string UINT8 = "System.Byte";
    public const string INT16 = "System.Int16";
    public const string UINT16 = "System.UInt16";
    public const string INT32 = "System.Int32";
    public const string UINT32 = "System.UInt32";
    public const string DOUBLE = "System.Double";
    public const string FLOAT = "System.Single";
    public const string INT64 = "System.Int64";
    public const string UINT64 = "System.UInt64";
    public const string STRING = "System.String";
    public const string DELEGATE = "System.MulticastDelegate";
    public const string CHAR = "System.Char";
    public const string BOOLEAN = "System.Boolean";
}

class Ini {
    internal static string GetConfig(string Key, string Name, string Path, bool Required = true) {
        if (GetConfigStatus(Key, Name, Path) == ConfigStatus.NoFile)
            return string.Empty;

        byte[] CFG = File.ReadAllBytes(Path);
        return GetConfig(Key, Name, CFG, Required);
    }
    internal static string GetConfig(string Key, string Name, byte[] File, bool Required = true) {
        VerifyHeader(ref File);

        string[] Lines = Encoding.UTF8.GetString(File).Replace("\r\n", "\n").Split('\n');
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

    internal static void VerifyHeader(ref byte[] Content) {
        if (EqualsAt(Content, new byte[] { 0xEF, 0xBB, 0xBF }, 0)) {
            byte[] Tmp = new byte[Content.Length - 3];
            for (uint i = 3; i < Content.LongLength; i++)
                Tmp[i - 3] = Content[i];

            Content = Tmp;
        }
    }

    internal static bool EqualsAt(byte[] Arr, byte[] Arr2, uint At) {
        if (Arr2.LongLength + At >= Arr.LongLength)
            return false;

        for (uint i = 0; i < Arr2.LongLength; i++) {
            if (Arr2[i] != Arr[i + At])
                return false;
        }

        return true;
    }


    internal static void SetConfig(string Key, string Name, string Value, string CfgPath) {
        if (GetConfigStatus(Key, Name, CfgPath) == ConfigStatus.NoFile) {
            File.WriteAllText(CfgPath, $"[{Key}]\r\n{Name}={Value}", Encoding.UTF8);
        } else {
            File.WriteAllBytes(CfgPath, SetConfig(Key, Name, Value, File.ReadAllBytes(CfgPath)));
        }
    }
    internal static byte[] SetConfig(string Key, string Name, string Value, byte[] Data) {
        VerifyHeader(ref Data);


        ConfigStatus cfg = GetConfigStatus(Key, Name, Data);
        string[] Lines = Encoding.UTF8.GetString(Data).Replace("\r\n", "\n").Split('\n');
        string AtualKey = string.Empty;
        if (cfg == ConfigStatus.Ok) {
            for (int i = 0; i < Lines.Length; i++) {
                string Line = Lines[i];
                if (Line.StartsWith("[") && Line.EndsWith("]"))
                    AtualKey = Line.Substring(1, Line.Length - 2);
                if (Line.StartsWith("!") || string.IsNullOrWhiteSpace(Line) || !Line.Contains("=") || Line.StartsWith("#") || Line.StartsWith(";"))
                    continue;
                string AtualName = Line.Split('=')[0].Trim().ToLower();
                if (Name.Split(';').Any(N => N.Trim().ToLower() == AtualName) && AtualKey == Key) {
                    Lines[i] = string.Format("{0}={1}", Name, Value);
                    break;
                }
            }
        }
        if (cfg == ConfigStatus.NoName) {
            List<string> Cfgs = new List<string>();
            int KeyPos = 0;
            for (int i = 0; i < Lines.Length; i++) {
                if (string.Format("[{0}]", Key) == Lines[i])
                    KeyPos = i;
                Cfgs.Add(Lines[i]);
            }
            Cfgs.Insert(KeyPos + 1, string.Format("{0}={1}", Name.Split(';')[0], Value));
            Lines = Cfgs.ToArray();
        }
        if (cfg == ConfigStatus.NoKey) {
            string[] NewLines = new string[Lines.Length + 3];
            Lines.CopyTo(NewLines, 0);
            NewLines[Lines.Length + 1] = string.Format("[{0}]", Key);
            NewLines[Lines.Length + 2] = string.Format("{0}={1}", Name.Split(';')[0], Value);
            Lines = NewLines;
        }

        StringBuilder SB = new StringBuilder();
        foreach (string str in Lines)
            SB.AppendLine(str);

        return Encoding.UTF8.GetBytes(SB.ToString().Trim('\r', '\n'));
    }

    internal enum ConfigStatus {
        NoFile, NoKey, NoName, Ok
    }

    internal static ConfigStatus GetConfigStatus(string Key, string Name, string CfgPath) {
        if (!File.Exists(CfgPath))
            return ConfigStatus.NoFile;
        return GetConfigStatus(Key, Name, File.ReadAllBytes(CfgPath));
    }
    internal static ConfigStatus GetConfigStatus(string Key, string Name, byte[] Data) {
        VerifyHeader(ref Data);

        string[] Lines = Encoding.UTF8.GetString(Data).Replace("\r\n", "\n").Split('\n');
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
                return ConfigStatus.Ok;
        }
        return KeyFound ? ConfigStatus.NoName : ConfigStatus.NoKey;
    }
}