using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

/// <summary>
/// Advanced Binary Tools - By Marcussacana
/// </summary>
namespace AdvancedBinary {

    public enum StringStyle {
        /// <summary>
        /// C-Style String (null terminated)
        /// </summary>
        CString,
        /// <summary>
        /// Unicode C-Style String (null terminated 2x)
        /// </summary>
        UCString,
        /// <summary>
        /// Pascal-Style String (Prefixed with the Length)
        /// </summary>
        PString
    }


    /// <summary>
    /// InvokeMethod While Reading
    /// </summary>
    /// <param name="Stream">Stream Instance</param>
    /// <param name="FromReader">Determine if the method is invoked from the StructReader or StructWriter</param>
    /// <param name="StructInstance">Struct instance reference</param>
    /// <return>New Struct Instance</return>
    public delegate dynamic FieldInvoke(Stream Stream, bool FromReader, dynamic StructInstance);

    /// <summary>
    /// Ignore Struct Field
    /// </summary>
    public class Ignore : Attribute { }

    /// <summary>
    /// C-Style String (null terminated)
    /// </summary>
    public class CString : Attribute { }

    /// <summary>
    /// Unicode C-Style String (null terminated 2x)
    /// </summary>
    public class UCString : Attribute { }
    /// <summary>
    /// Pascal-Style String (int32 Length Prefix)
    /// </summary>
    public class PString : Attribute {
        public string PrefixType = Const.UINT32;
        public bool UnicodeLength;
    }

    /// <summary>
    /// Pre-Defined String Length
    /// </summary>
    public class FString : Attribute {
        public long Length;
    }


    /// <summary>
    /// Fixed Length Array
    /// </summary>
    public class FArray : Attribute {
        public long Length;
    }

    /// <summary>
    /// Prefixed Length Array 
    /// </summary>
    public class PArray : Attribute {
        public string PrefixType = Const.UINT32;
    }

    /// <summary>
    /// Struct Field Type (required only to sub structs)
    /// </summary>
    public class StructField : Attribute { }
    public class Const {
        //Types
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

        //Attributes
        public const string PSTRING = "PString";
        public const string CSTRING = "CString";
        public const string UCSTRING = "UCString";
        public const string FSTRING = "FString";
        public const string STRUCT = "StructField";
        public const string IGNORE = "Ignore";
        public const string FARRAY = "FArray";
        public const string PARRAY = "PArray";
    }

    static class Tools {
        /*
        public static dynamic GetAttributePropertyValue<T>(T Struct, string FieldName, string AttributeName, string PropertyName) {
            Type t = Struct.GetType();
            FieldInfo[] Fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo Fld in Fields) {
                if (Fld.Name != FieldName)
                    continue;
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
            throw new Exception("Field Not Found");
        }*/
        
        public static void CopyStream(Stream input, Stream output) {
            int Readed = 0;
            byte[] Buffer = new byte[1024 * 1024];
            do {
                Readed = input.Read(Buffer, 0, Buffer.Length);
                output.Write(Buffer, 0, Readed);
            } while (Readed > 00);
        }

        public static dynamic Reverse(dynamic Data) {
            byte[] Arr = BitConverter.GetBytes(Data);
            Array.Reverse(Arr, 0, Arr.Length);
            string type = Data.GetType().FullName;
            switch (type) {
                case Const.INT8:
                case Const.UINT8:
                    return Data;
                case Const.INT16:
                    return BitConverter.ToInt16(Arr, 0);
                case Const.UINT16:
                    return BitConverter.ToUInt16(Arr, 0);
                case Const.INT32:
                    return BitConverter.ToInt32(Arr, 0);
                case Const.UINT32:
                    return BitConverter.ToUInt32(Arr, 0);
                case Const.INT64:
                    return BitConverter.ToInt64(Arr, 0);
                case Const.UINT64:
                    return BitConverter.ToUInt64(Arr, 0);
                case Const.DOUBLE:
                    return BitConverter.ToDouble(Arr, 0);
                case Const.FLOAT:
                    return BitConverter.ToSingle(Arr, 0);
                default:
                    throw new Exception("Unk Data Type.");
            }
        }

        public static dynamic GetAttributePropertyValue(FieldInfo Fld, string AttributeName, string PropertyName) {
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

        public static long GetStructLength<T>(T Struct) {
            Type type = Struct.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            long Length = 0;
            foreach (FieldInfo field in fields) {
                if (HasAttribute(field, Const.IGNORE))
                    continue;
                switch (field.FieldType.ToString()) {
                    case Const.INT8:
                    case Const.UINT8:
                        Length += 1;
                        break;
                    case Const.INT32:
                    case Const.FLOAT:
                    case Const.UINT32:
                        Length += 4;
                        break;
                    case Const.UINT64:
                    case Const.INT64:
                    case Const.DOUBLE:
                        Length += 8;
                        break;
                    case Const.STRING:
                        if (!HasAttribute(field, Const.FSTRING))
                            throw new Exception("You can't calculate struct length with strings");
                        else
                            Length += GetAttributePropertyValue(field, Const.FSTRING, "Length");
                        break;
                    default:
                        if (field.FieldType.BaseType.ToString() == Const.DELEGATE)
                            break;
                        if (HasAttribute(field, Const.IGNORE))
                            break;
                        throw new Exception("Unk Struct Field: " + field.FieldType.ToString());
                }
            }
            return Length;
        }

        internal static bool HasAttribute(FieldInfo Field, string Attrib) {
            foreach (Attribute attrib in Field.GetCustomAttributes(true))
                if (attrib.GetType().Name == Attrib)
                    return true;
            return false;
        }
        public static void ReadStruct<T>(byte[] Array, ref T Struct, bool IsBigEnddian = false, Encoding Encoding = null, long BaseOffset = 0) {
            MemoryStream Stream = new MemoryStream(Array);
            StructReader Reader = new StructReader(Stream, IsBigEnddian, Encoding);
            Reader.Seek(BaseOffset, SeekOrigin.Begin);
            Reader.ReadStruct(ref Struct);
            Reader.Close();
            Stream?.Close();
        }

        public static byte[] BuildStruct<T>(ref T Struct, bool BigEndian = false, Encoding Encoding = null) {
            MemoryStream Stream = new MemoryStream();
            StructWriter Writer = new StructWriter(Stream, BigEndian, Encoding);
            Writer.WriteStruct(ref Struct);
            byte[] Result = Stream.ToArray();
            Writer.Close();
            Stream?.Close();
            return Result;
        }

        internal static void CopyStruct<T>(T Input, ref T Output) {
            Type type = Input.GetType();
            object tmp = Output;
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++) {
                object value = fields[i].GetValue(Input);
                fields[i].SetValue(tmp, value);
            }
            Output = (T)tmp;
        }
    }

    public class StructWriter : BinaryWriter {

        internal bool BigEndian = false;
        internal Encoding Encoding;

        public StructWriter(Stream Output, bool BigEndian = false, Encoding Encoding = null) : base(Output) {
            if (Encoding == null)
                Encoding = Encoding.UTF8;
            this.BigEndian = BigEndian;
            this.Encoding = Encoding;
        }
        public StructWriter(string OutputFile, bool BigEndian = false, Encoding Encoding = null) : base(new StreamWriter(OutputFile).BaseStream) {
            if (Encoding == null)
                Encoding = Encoding.UTF8;
            this.BigEndian = BigEndian;
            this.Encoding = Encoding;
        }

        public void WriteStruct<T>(ref T Struct) {
            Type type = Struct.GetType();
            object tmp = Struct;
            WriteStruct(type, ref tmp);
            Struct = (T)tmp;
        }

        internal void WriteStruct(Type type, ref object Instance) {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields) {
                if (HasAttribute(field, Const.IGNORE))
                    continue;
                dynamic Value = field.GetValue(Instance);
                string Type = field.FieldType.ToString();
                if (!Type.EndsWith("[]")) {
                    WriteField(Value, Type, field, ref Instance);
                    continue;
                }
                if (HasAttribute(field, Const.FARRAY)) {
                    long BufferLen = Tools.GetAttributePropertyValue(field, Const.FARRAY, "Length");

                    if (Value.Length > BufferLen)
                        throw new Exception("Wrong Array Buffer Length");

                    byte[] Arr = new byte[BufferLen];
                    Value.CopyTo(Arr, 0);

                    Write(Arr);
                    continue;
                }

                if (!HasAttribute(field, Const.PARRAY))
                    throw new Exception("Bad Struct Array Configuration.");

                string PType = Tools.GetAttributePropertyValue(field, Const.PARRAY, "PrefixType");
                dynamic Length = ParseType(Value.LongLength, PType);

                if (BigEndian)
                    Length = Tools.Reverse(Length);
                Write(Length);

                for (long i = 0; i < Length; i++) {
                    WriteField(Value[i], Type.Substring(0, Type.Length-2), field, ref Instance);
                }
            }
        }

        internal dynamic ParseType(dynamic Value, string Type) {
            switch (Type) {
                case Const.INT8:
                    return (sbyte)Value;
                case Const.UINT8:
                    return (byte)Value;
                case Const.INT16:
                    return (short)Value;
                case Const.UINT16:
                    return (ushort)Value;
                case Const.INT32:
                    return (int)Value;
                case Const.UINT32:
                    return (uint)Value;
                case Const.INT64:
                    return (long)Value;
                case Const.UINT64:
                    return (ulong)Value;
                case Const.DOUBLE:
                    return (double)Value;
                case Const.FLOAT:
                    return (float)Value;
                default:
                    throw new Exception("Bad Struct Configuration");
            }
        }

        internal void WriteField(dynamic Value, string Type, FieldInfo field, ref object Instance) {
            switch (Type) {
                case Const.STRING:
                    if (HasAttribute(field, Const.CSTRING)) {
                        Write(Value, StringStyle.CString);
                        break;
                    }
                    if (HasAttribute(field, Const.UCSTRING)) {
                        Write(Value, StringStyle.UCString);
                        break;
                    }
                    if (HasAttribute(field, Const.PSTRING)) {
                        Write(field, Value, StringStyle.PString);
                        break;
                    }
                    if (HasAttribute(field, Const.FSTRING)) {
                        byte[] Buffer = new byte[Tools.GetAttributePropertyValue(field, Const.FSTRING, "Length")];
                        Encoding.GetBytes((string)Value).CopyTo(Buffer, 0);
                        Write(Buffer);
                        break;
                    }
                    throw new Exception("String Attribute Not Specified.");
                default:
                    if (HasAttribute(field, Const.STRUCT)) {
                        WriteStruct(field.FieldType, ref Value);
                    } else {
                        if (field.FieldType.BaseType.ToString() == Const.DELEGATE) {
                            FieldInvoke Invoker = ((FieldInvoke)Value);
                            if (Invoker == null)
                                break;
                            Instance = Invoker.Invoke(BaseStream, false, Instance);
                            field.SetValue(Instance, Invoker);
                        } else if (BigEndian)
                            Write(Tools.Reverse(Value));
                        else
                            Write(Value);
                    }
                    break;
            }
        }

        public void Write(string String, StringStyle Style) {
            switch (Style) {
                case StringStyle.UCString:
                case StringStyle.CString:
                    List<byte> Buffer = new List<byte>(Encoding.GetBytes(String + "\x0"));
                    base.Write(Buffer.ToArray(), 0, Buffer.Count);
                    break;
                default:
                    base.Write(String);
                    break;
            }
        }
        public void Write(FieldInfo Field, dynamic Value, StringStyle Style) {
            switch (Style) {
                case StringStyle.UCString:
                case StringStyle.CString:
                    List<byte> Buffer = new List<byte>(Encoding.GetBytes(Value + "\x0"));
                    base.Write(Buffer.ToArray(), 0, Buffer.Count);
                    break;
                case StringStyle.PString:
                    WritePString(Field, Value);
                    break;
            }
        }

        internal void WritePString(FieldInfo Field, dynamic Value) {
            byte[] Arr = Encoding.GetBytes(Value);

            string Prefix = Tools.GetAttributePropertyValue(Field, Const.PSTRING, "PrefixType");
            bool UnicodeLength = Tools.GetAttributePropertyValue(Field, Const.PSTRING, "UnicodeLength");


            long Length = Arr.LongLength;
            if (UnicodeLength)
                Length /= 2;

            switch (Prefix) {
                case Const.INT16:
                    if (BigEndian)
                        base.Write((short)Tools.Reverse((short)Length));
                    else
                        base.Write((short)Length);
                    break;
                case Const.UINT16:
                    if (BigEndian)
                        base.Write((ushort)Tools.Reverse((ushort)Length));
                    else
                        base.Write((ushort)Length);
                    break;
                case Const.UINT8:
                    if (BigEndian)
                        base.Write((byte)Tools.Reverse((byte)Length));
                    else
                        base.Write((byte)Length);
                    break;
                case Const.INT8:
                    if (BigEndian)
                        base.Write((sbyte)Tools.Reverse((sbyte)Length));
                    else
                        base.Write((sbyte)Length);
                    break;
                case Const.INT32:
                    if (BigEndian)
                        base.Write((int)Tools.Reverse((int)Length));
                    else
                        base.Write((int)Length);
                    break;
                case Const.UINT32:
                    if (BigEndian)
                        base.Write((uint)Tools.Reverse((uint)Length));
                    else
                        base.Write((uint)Length);
                    break;
                case Const.INT64:
                    if (BigEndian)
                        base.Write((long)Tools.Reverse(Length));
                    else
                        base.Write(Length);
                    break;
                default:
                    throw new Exception("Invalid Data Type");
            }
            base.Write(Arr);
        }

        internal bool HasAttribute(FieldInfo Field, string Attrib) {
            foreach (Attribute attrib in Field.GetCustomAttributes(true))
                if (attrib.GetType().Name == Attrib)
                    return true;
            return false;
        }

        internal void Seek(long Index, SeekOrigin Origin) {
            base.BaseStream.Seek(Index, Origin);
            base.BaseStream.Flush();
        }
    }

    class StructReader : BinaryReader {

        internal bool BigEndian = false;
        internal Encoding Encoding;
        public StructReader(Stream Input, bool BigEndian = false, Encoding Encoding = null) : base(Input) {
            if (Encoding == null)
                Encoding = Encoding.UTF8;
            this.BigEndian = BigEndian;
            this.Encoding = Encoding;
        }
        public StructReader(string Input, bool BigEndian = false, Encoding Encoding = null) : base(new StreamReader(Input).BaseStream) {
            if (Encoding == null)
                Encoding = Encoding.UTF8;
            this.BigEndian = BigEndian;
            this.Encoding = Encoding;
        }

        public void ReadStruct<T>(ref T Struct) {
            Type type = Struct.GetType();
            object tmp = Struct;
            ReadStruct(type, ref tmp);
            Struct = (T)tmp;
        }

        internal void ReadStruct(Type type, ref object Instance) {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++) {
                FieldInfo field = fields[i];
                if (Tools.HasAttribute(field, Const.IGNORE))
                    continue;

                string FType = field.FieldType.ToString();
                dynamic Value = null;

                if (!FType.EndsWith("[]")) {
                    Value = ReadField(FType, field, ref Instance);

                } else {
                    if (Tools.HasAttribute(field, Const.FARRAY)) {
                        byte[] Buffer = new byte[Tools.GetAttributePropertyValue(field, Const.FARRAY, "Length")];

                        if (Read(Buffer, 0, Buffer.Length) != Buffer.Length)
                            throw new Exception("Failed to Read");

                        Value = Buffer;

                    } else if (Tools.HasAttribute(field, Const.PARRAY)) {
                        FType = FType.Substring(0, FType.Length - 2);

                        string PType = Tools.GetAttributePropertyValue(field, Const.PARRAY, "PrefixType");
                        dynamic Count = ReadField(PType, field, ref Instance);

                        Value = CreateArrayInstance(FType, Count);
                        for (long x = 0; x < Count; x++) {
                            Value[x] = ReadField(FType, field, ref Instance);
                        }

                    } else
                        throw new Exception("Bad Struct Configuration");
                }                

                field.SetValue(Instance, Value);
            }
        }

        internal dynamic CreateArrayInstance(string TypeName, dynamic InitialLength) {/*
            switch (TypeName) {
                case Const.INT8:
                    return new sbyte[InitialLength];
                case Const.UINT8:
                    return new byte[InitialLength];
                case Const.INT16:
                    return new short[InitialLength];
                case Const.UINT16:
                    return new ushort[InitialLength];
                case Const.INT32:
                    return new int[InitialLength];
                case Const.UINT32:
                    return new uint[InitialLength];
                case Const.INT64:
                    return new long[InitialLength];
                case Const.UINT64:
                    return new ulong[InitialLength];
                case Const.DOUBLE:
                    return new double[InitialLength];
                case Const.FLOAT:
                    return new float[InitialLength];
                case Const.STRING:
                    return new string[InitialLength];
                default:
                    throw new Exception("Unk Variable Type");
            }*/
			Type ArrType = Type.GetType(TypeName);
			return Array.CreateInstance(ArrType, InitialLength);
			/*
            Type stringArrayType = Type.GetType(TypeName).MakeArrayType();
            return Activator.CreateInstance(stringArrayType, new object[] { InitialLength });*/
        }

        internal dynamic ReadField(string Type, FieldInfo field, ref object Instance) {
            bool IsNumber = true;
            dynamic Value = null;
            switch (Type) {
                case Const.INT8:
                    Value = base.ReadSByte();
                    break;
                case Const.INT16:
                    Value = base.ReadInt16();
                    break;
                case Const.CHAR:
                    Value = base.ReadChar();
                    break;
                case Const.UINT16:
                    Value = base.ReadUInt16();
                    break;
                case Const.UINT8:
                    Value = base.ReadByte();
                    break;
                case Const.INT32:
                    Value = base.ReadInt32();
                    break;
                case Const.UINT32:
                    Value = base.ReadUInt32();
                    break;
                case Const.DOUBLE:
                    Value = base.ReadDouble();
                    break;
                case Const.FLOAT:
                    Value = base.ReadSingle();
                    break;
                case Const.INT64:
                    Value = base.ReadInt64();
                    break;
                case Const.UINT64:
                    Value = base.ReadUInt64();
                    break;
                case Const.STRING:
                    IsNumber = false;
                    if (Tools.HasAttribute(field, Const.CSTRING) && Tools.HasAttribute(field, Const.PSTRING))
                        throw new Exception("You can't use CString and PString Attribute into the same field.");
                    if (Tools.HasAttribute(field, Const.CSTRING)) {
                        Value = ReadString(StringStyle.CString);
                        break;
                    }
                    if (Tools.HasAttribute(field, Const.UCSTRING)) {
                        Value = ReadString(StringStyle.UCString);
                        break;
                    }
                    if (Tools.HasAttribute(field, Const.PSTRING)) {
                        Value = ReadString(StringStyle.PString, field);
                        break;
                    }
                    if (Tools.HasAttribute(field, Const.FSTRING)) {
                        byte[] Bffr = new byte[Tools.GetAttributePropertyValue(field, Const.FSTRING, "Length")];
                        if (Read(Bffr, 0, Bffr.Length) != Bffr.Length)
                            throw new Exception("Failed to Read a String");
                        Value = Encoding.GetString(Bffr);
                        break;
                    }
                    throw new Exception("String Attribute Not Specified.");
                default:
                    IsNumber = false;
                    if (Tools.HasAttribute(field, Const.STRUCT)) {
                        Value = Activator.CreateInstance(field.FieldType);
                        ReadStruct(field.FieldType, ref Value);
                    } else {
                        if (field.FieldType.BaseType.ToString() == Const.DELEGATE) {
                            FieldInvoke Invoker = (FieldInvoke)field.GetValue(Instance);
                            Value = Invoker;
                            if (Invoker == null)
                                break;
                            Instance = Invoker.Invoke(BaseStream, true, Instance);
                            break;
                        }
                        throw new Exception("Unk Struct Field: " + field.FieldType.ToString());
                    }
                    break;
            }
            if (IsNumber && BigEndian) {
                Value = Tools.Reverse(Value);
            }
            return Value;
        }

        public string ReadString(StringStyle Style, FieldInfo Info = null) {
            List<byte> Buffer = new List<byte>();
            switch (Style) {
                case StringStyle.CString:
                    while (true) {
                        byte Byte = base.ReadByte();
                        if (Byte < 1)
                            break;
                        Buffer.Add(Byte);
                    }
                    return Encoding.GetString(Buffer.ToArray());
                case StringStyle.UCString:
                    while (true) {
                        byte Byte1 = base.ReadByte();
                        byte Byte2 = base.ReadByte();
                        if (Byte1 == 0x00 && Byte2 == 0x00)
                            break;
                        Buffer.Add(Byte1);
                        Buffer.Add(Byte2);
                    }
                    return Encoding.GetString(Buffer.ToArray());
                case StringStyle.PString:
                    if (Info != null) {
                        long Len;
                        string Prefix = Tools.GetAttributePropertyValue(Info, Const.PSTRING, "PrefixType");
                        bool UnicodeLength = Tools.GetAttributePropertyValue(Info, Const.PSTRING, "UnicodeLength");
                        switch (Prefix) {
                            case Const.INT16:
                                if (BigEndian)
                                    Len = Tools.Reverse(ReadInt16());
                                else
                                    Len = ReadInt16();
                                break;
                            case Const.UINT16:
                                if (BigEndian)
                                    Len = Tools.Reverse(ReadUInt16());
                                else
                                    Len = ReadUInt16();
                                break;
                            case Const.UINT8:
                                if (BigEndian)
                                    Len = Tools.Reverse(ReadByte());
                                else
                                    Len = ReadByte();
                                break;
                            case Const.INT8:
                                if (BigEndian)
                                    Len = Tools.Reverse(ReadSByte());
                                else
                                    Len = ReadSByte();
                                break;
                            case Const.INT32:
                                if (BigEndian)
                                    Len = Tools.Reverse(ReadInt32());
                                else
                                    Len = ReadInt32();
                                break;
                            case Const.UINT32:
                                if (BigEndian)
                                    Len = Tools.Reverse(ReadUInt32());
                                else
                                    Len = ReadUInt32();
                                break;
                            case Const.INT64:
                                if (BigEndian)
                                    Len = Tools.Reverse(ReadInt64());
                                else
                                    Len = ReadInt64();
                                break;
                            default:
                                throw new Exception("Invalid Data Type");
                        }
                        if (UnicodeLength)
                            Len *= 2;
                        if (Len > BaseStream.Length - BaseStream.Position)
                            throw new Exception("Invalid Length");
                        byte[] Buff = new byte[Len];
                        while (Len > 0)
                            Len -= BaseStream.Read(Buff, 0, Len > int.MaxValue ? int.MaxValue : (int)Len);
                        return Encoding.GetString(Buff);
                    } else
                        return ReadString();
                default:
                    throw new Exception("Unk Value Type");
            }
        }

        internal void Seek(long Index, SeekOrigin Origin) {
            base.BaseStream.Seek(Index, Origin);
            base.BaseStream.Flush();
        }

        internal int Peek() {
            int b = BaseStream.ReadByte();
            BaseStream.Position--;
            return b;
        }

        internal int PeekInt() {
            byte[] Buff = new byte[4];
            int i = BaseStream.Read(Buff, 0, Buff.Length);
            BaseStream.Position -= i;
            return BitConverter.ToInt32(Buff, 0);
        }
    }
}
