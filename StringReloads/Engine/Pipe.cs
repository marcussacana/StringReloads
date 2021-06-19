using StringReloads.Engine.String;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace StringReloads.Engine
{
    unsafe static class Pipe
    {
        internal static void Run(string PipeName) {
            var Server = new NamedPipeServerStream("SRLPipe." + PipeName, PipeDirection.InOut);
            Server.WaitForConnection();
            while (Server.IsConnected)
            {
                var Mode = Server.ReadByte();
                switch (Mode) {
                    case 1:
                        Server.ProcessStr();
                        break;
                    case 2:
                        Server.ProcessWStr();
                        break;
                    default:
                        continue;
                }
            }
        }
        static void ProcessStr(this NamedPipeServerStream Server)
        {
            var Buffer = Server.ReadStringBuffer();
            fixed (void* pStr = &Buffer[0])
            {
                var Rst = EntryPoint.Process(pStr);
                if (Rst == pStr)
                {
                    Server.WriteI32(0);
                    Server.Flush();
                    return;
                }

                CString NewStr = Rst;
                var StrData = NewStr.ToArray();
                StrData = StrData.Concat(new byte[1]).ToArray();

                Server.WriteI32(StrData.Length);
                Server.Write(StrData, 0, StrData.Length);
                Server.Flush();
            }
        }

        static void ProcessWStr(this NamedPipeServerStream Server)
        {
            var Buffer = Server.ReadStringBuffer();
            fixed (void* pStr = &Buffer[0])
            {
                var Rst = EntryPoint.ProcessW(pStr);
                if (Rst == pStr)
                {
                    Server.WriteI32(0);
                    Server.Flush();
                    return;
                }

                WCString NewStr = Rst;
                var StrData = NewStr.ToArray();
                StrData = StrData.Concat(new byte[2]).ToArray();

                Server.WriteI32(StrData.Length);
                Server.Write(StrData, 0, StrData.Length);
                Server.Flush();
            }
        }
        static void WriteI32(this Stream Strm, int Value)
        {
            var Data = BitConverter.GetBytes(Value);
            Strm.Write(Data, 0, Data.Length);
        }
        static byte[] ReadStringBuffer(this Stream Strm)
        {
            List<byte> Buffer = new List<byte>();
            while (true)
            {
                var Byt = Strm.ReadByte();
                if (Byt <= 0)
                    break;
                Buffer.Add((byte)Byt);
            }
            Buffer.Add(0);
            return Buffer.ToArray();
        }
        static byte[] ReadWStringBuffer(this Stream Strm)
        {
            List<byte> Buffer = new List<byte>();
            while (true)
            {
                var Byt = Strm.ReadByte();
                if (Byt < 0 || (Byt == 0 && Buffer.Last() == 0))
                    break;
                Buffer.Add((byte)Byt);
            }
            Buffer.Add(0);
            return Buffer.ToArray();
        }
    }
}
