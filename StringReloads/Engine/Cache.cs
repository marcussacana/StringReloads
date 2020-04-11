using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace StringReloads.Engine
{
    unsafe class Cache
    {
        const uint Signature = 0x364C5253;//SRL6

        Stream File;

        BinaryReader _Reader = null;
        BinaryReader Reader => _Reader ?? (_Reader = new BinaryReader(File, Encoding.Unicode));

        BinaryWriter _Writer = null;
        BinaryWriter Writer => _Writer ?? (_Writer = new BinaryWriter(File, Encoding.Unicode));

        public Cache(string Path) {
            File = System.IO.File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        ~Cache() {
            File?.Close();
            File?.Dispose();
        }

        Header GetHeader() {
            File.Position = 0;
            byte[] Buffer = new byte[sizeof(Header)];
            if (File.Read(Buffer, 0, Buffer.Length) != Buffer.Length)
                throw new InternalBufferOverflowException();

            fixed (byte* pBuffer = &Buffer[0]) {
                Header* pHeader = (Header*)pBuffer;

                if (pHeader->Signature != Signature)
                    throw new Exception("Invalid SRL Cache File");

                return *pHeader;
            }
        }

        public IEnumerable<Database> GetDatabases() {
            File.Position = GetHeader().DatabaseOffset;
            uint Entries = Reader.ReadUInt32();
            for (uint i = 0; i < Entries; i++) {
                string Name = Reader.ReadString();

                string[] LinesA = Reader.ReadArray<string>().ToArray();
                string[] LinesB = Reader.ReadArray<string>().ToArray();

                var DB = new Database(Name);
                for (uint x = 0; x < LinesA.Length; x++)
                    DB.Add(new LSTEntry(LinesA[x], LinesB[x]));

                yield return DB;
            }
        }

        public IEnumerable<KeyValuePair<char, char>> GetRemaps() {
            File.Position = GetHeader().CharsOffset;
            uint Entries = Reader.ReadUInt32();
            for (uint i = 0; i < Entries; i++) {
                char A = Reader.ReadChar();
                char B = Reader.ReadChar();

                yield return new KeyValuePair<char, char>(A, B);
            }
        }

        public void BuildDatabase(Database[] Databases, KeyValuePair<char, char>[] Remaps) {
            File.Position = 0;
            Header Header = new Header();
            Header* pHeader = &Header;

            pHeader->Signature = Signature;
            
            byte* Buffer = (byte*)pHeader;
            Writer.Write(Buffer, 0, sizeof(Header));
            Writer.Flush();

            pHeader->DatabaseOffset = (uint)Writer.BaseStream.Position;
            Writer.Write((uint)Databases.LongCount());
            foreach (var Database in Databases) {
                Writer.Write(Database.Name);

                List<string> LinesA = new List<string>();
                List<string> LinesB = new List<string>();

                foreach (var Entry in Database) {
                    LinesA.Add(Entry.OriginalFlags.GetFlags()    + Entry.OriginalLine);
                    LinesB.Add(Entry.TranslationFlags.GetFlags() + Entry.TranslationLine);
                }

                Writer.WriteArray(LinesA.ToArray());
                Writer.WriteArray(LinesB.ToArray());
            }

            Writer.Flush();
            pHeader->CharsOffset = (uint)Writer.BaseStream.Position;

            Writer.Write((uint)Remaps.Length);
            foreach (var Remap in Remaps) {
                Writer.Write(Remap.Key);
                Writer.Write(Remap.Value);
            }

            Writer.Flush();

            Writer.BaseStream.Position = 0;
            Writer.Write(Buffer, 0, sizeof(Header));
            Writer.Flush();
        }

        struct Header {
            public uint Signature;
            public uint DatabaseOffset;
            public uint CharsOffset;
        }
    }

    internal static partial class Extensions {
        public static IEnumerable<T> ReadArray<T>(this BinaryReader Reader)
        {
            uint Length = Reader.ReadUInt32();
            for (uint i = 0; i < Length; i++)
            {
                yield return Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.Boolean => (T)(object)Reader.ReadBoolean(),
                    TypeCode.Byte =>    (T)(object)Reader.ReadByte(),
                    TypeCode.SByte =>   (T)(object)Reader.ReadSByte(),
                    TypeCode.Int16 =>   (T)(object)Reader.ReadInt16(),
                    TypeCode.UInt16 =>  (T)(object)Reader.ReadUInt16(),
                    TypeCode.Int32 =>   (T)(object)Reader.ReadInt32(),
                    TypeCode.UInt32 =>  (T)(object)Reader.ReadUInt32(),
                    TypeCode.Int64 =>   (T)(object)Reader.ReadInt64(),
                    TypeCode.UInt64 =>  (T)(object)Reader.ReadUInt64(),
                    TypeCode.Char =>    (T)(object)Reader.ReadChar(),
                    TypeCode.String =>  (T)(object)Reader.ReadString(),
                    _ => throw new Exception("Invalid Array Type")
                };
            }
        }
        public static void WriteArray<T>(this BinaryWriter Writer, T[] Array)
        {
            Writer.Write((uint)Array.LongCount());
            for (uint i = 0; i < Array.Count(); i++)
            {
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Boolean: Writer.Write((bool)(object)Array[i]);   break;
                    case TypeCode.Byte:    Writer.Write((byte)(object)Array[i]);   break;
                    case TypeCode.SByte:   Writer.Write((sbyte)(object)Array[i]);  break;
                    case TypeCode.Int16:   Writer.Write((short)(object)Array[i]);  break;
                    case TypeCode.UInt16:  Writer.Write((ushort)(object)Array[i]); break;
                    case TypeCode.Int32:   Writer.Write((int)(object)Array[i]);    break;
                    case TypeCode.UInt32:  Writer.Write((uint)(object)Array[i]);   break;
                    case TypeCode.Int64:   Writer.Write((long)(object)Array[i]);   break;
                    case TypeCode.UInt64:  Writer.Write((ulong)(object)Array[i]);  break;
                    case TypeCode.Char:    Writer.Write((char)(object)Array[i]);   break;
                    case TypeCode.String:  Writer.Write((string)(object)Array[i]); break;
                    default: throw new Exception("Invalid Array Type");
                };
            }
        }

        public unsafe static void Write(this BinaryWriter Stream, byte* Buffer, int Offset, int Count) => Write(Stream.BaseStream, Buffer, Offset, Count);
        public unsafe static void Write(this Stream Stream, byte* Buffer, int Offset, int Count) {
            if (Stream is FileStream) {
                var FStream = (FileStream)Stream;
                if (!WriteFile(FStream.SafeFileHandle, Buffer + Offset, Count, out int Written, out _) || Written != Count)
                    throw new IOException("Failed to Write the Buffer");
                return;
            }

            byte[] MBuffer = new byte[Count];
            Marshal.Copy(new IntPtr(Buffer + Offset), MBuffer, 0, Count);
            Stream.Write(MBuffer, 0, Count);
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern unsafe bool WriteFile(SafeHandle handle, byte* buffer, int numBytesToWrite, out int numBytesWritten, out System.Threading.NativeOverlapped lpOverlapped);
    }
}
