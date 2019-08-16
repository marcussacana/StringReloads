using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRL {
    static partial class StringReloader {        
        public static int ServiceCall(string ID) {
            if (ID.Contains(ServiceDuplicateFlag)) {
                ID = ID.Split('|').First();
                AllowDuplicates = true;
            }
                
            string SName = string.Format(ServiceMask, ID);

            if (Debugging)
                Log("Debug Mode Enabled.");

            Task.Factory.StartNew(() => Service(SName)).Wait();           
            return 0;
        }

        private static void Service(string ServiceName) {
            StrRld = CreateDictionary();
            MskRld = CreateDictionary();
            Missed = new List<string>();

            NamedPipeServerStream Server = new NamedPipeServerStream(ServiceName, PipeDirection.InOut, 2, PipeTransmissionMode.Byte);
            Server.WaitForConnection();

            BinaryReader Reader = new BinaryReader(Server);
            BinaryWriter Writer = new BinaryWriter(Server);

            bool OK = true;
            while (Server.IsConnected) {
                PipeCommands Command = (PipeCommands)Reader.ReadByte();
                Log("Command Recived: {0}", true, Command.ToString());
                switch (Command) {
                    case PipeCommands.AddReload:
                        OK = true;
                        string Original = Reader.ReadString();
                        string Reloader = Reader.ReadString();
                        if (!StrRld.ContainsKey(Original) || AllowDuplicates)
                            StrRld.Add(Original, Reloader);
                        Log("Command Finished, In: {0}, Out: {1}", true, 2, 0);
                        break;
                    case PipeCommands.FindReload:
                        OK = true;
                        bool Enforce = Reader.ReadByte() == (byte)PipeCommands.True;
                        string Key = Reader.ReadString();

                        if (StrRld.ContainsKey(Key))
                            Writer.Write((byte)PipeCommands.True);
                        else if (Enforce)
                            Writer.Write((byte)PipeCommands.False);
                        else if ((from x in Databases where x.ContainsKey(Key) select x).Count() > 0)
                            Writer.Write((byte)PipeCommands.True);
                        else
                            Writer.Write((byte)PipeCommands.False);

                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 2, 1);
                        break;
                    case PipeCommands.GetReload:
                        OK = true;
                        string RLD = Reader.ReadString();
                        try {
                            string Rst = null;
                            if (StrRld.ContainsKey(RLD))
                                Rst = StrRld[RLD];
                            else
                                for (DBID = 0; DBID < Databases.Count; DBID++) {
                                    if (StrRld.ContainsKey(RLD)) {
                                        Rst = StrRld[RLD];
                                        break;
                                    }
                                }
                            Writer.Write(Rst);
                        } catch (Exception ex) {
                            Writer.Write(RLD);
                            Log("Exception Handled\n=================\n{0}\n{1}", true, ex.Message, ex.StackTrace);
                        }
                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                        break;
                    case PipeCommands.AddMissed:
                        OK = true;
                        try {
                            Missed.Add(Reader.ReadString());
                        } catch { }
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                        break;
                    case PipeCommands.FindMissed:
                        OK = true;
                        if (Missed.Contains(Reader.ReadString()))
                            Writer.Write((byte)PipeCommands.True);
                        else
                            Writer.Write((byte)PipeCommands.False);
                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                        break;
                    case PipeCommands.AddPtr:
                        OK = true;
                        try {
                            Ptrs.Add(Reader.ReadInt64());
                        } catch { }
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 0);
                        break;
                    case PipeCommands.GetPtrs:
                        OK = true;
                        uint Replys = 1;
                        Writer.Write(Ptrs.Count);
                        foreach (long Ptr in Ptrs) {
                            Writer.Write(Ptr);
                            Replys++;
                        }
                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, Replys);
                        break;
                    case PipeCommands.EndPipe:
                        OK = true;
                        Log("Exit Command Recived...", true);
                        Environment.Exit(0);
                        break;
                    case PipeCommands.AddMask:
                        OK = true;
                        string Input = Reader.ReadString();
                        string Reload = Reader.ReadString();
                        if (!MskRld.ContainsKey(Input) || AllowDuplicates)
                            MskRld.Add(Input, Reload);
                        Log("Command Finished, In {0}, Out: {1}", true, 2, 0);
                        break;
                    case PipeCommands.ChkMask:
                        string String = Reader.ReadString();
                        
                        string[] Result = (from x in MskRld.Keys where MaskMatch(x, String) select x).ToArray();

                        Writer.Write((byte)((Result.Length > 0) ? PipeCommands.True : PipeCommands.False));
                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                        break;
                    case PipeCommands.RldMask:
                        string Ori = Reader.ReadString();
                        string Mask = (from x in MskRld.Keys where MaskMatch(x, Ori) select x).FirstOrDefault();
                        string Rld = MskRld[Mask];

                        Writer.Write(MaskReplace(Mask, Ori, Rld));
                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 2, 1);
                        break;
                    case PipeCommands.AdvDB:
                        if (DBID >= Databases.Count)
                            return;

                        LastDBID++;
                        DBID = LastDBID;
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 0);
                        break;
                    case PipeCommands.GetDBID:
                        Writer.Write(DBID);
                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                        break;
                    case PipeCommands.SetDBID:
                        DBID = Reader.ReadInt32();
                        Log("Command Finished, In: {0}, Out: {1}", true, 2, 0);
                        break;
                    case PipeCommands.GetDBIndex:
                        Writer.Write(((DuplicableDictionary<string, string>)StrRld).LastKeyIndex);
                        Writer.Flush();
                        Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                        break;
                    default:
                        if (!OK)
                            MessageBox.Show("Recived Invalid Command to the pipe service...", "SRL Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        OK = false;
                        break;
                }
            }
            Server.Close();
            Reader.Close();
            Writer.Close();
        }
        private static void EndPipe() {
            if (Multithread)
                return;

            PipeWriter.Write((byte)PipeCommands.EndPipe);
            PipeWriter.Flush();
        }
        private static void AddPtr(IntPtr Ptr) {
            long Pointer = Ptr.ToInt64();
            if (Multithread) {
                if (!Ptrs.Contains(Pointer))
                    Ptrs.Add(Pointer);
                return;
            }

            PipeWriter.Write((byte)PipeCommands.AddPtr);
            PipeWriter.Write(Pointer);
            PipeWriter.Flush();
        }

        private static IntPtr[] GetPtrs() {            
            if (Multithread) {
                long[] Arr = Ptrs.ToArray();
                IntPtr[] Rst = new IntPtr[Arr.LongLength];
                for (long i = 0; i < Rst.LongLength; i++) {
                    Rst[i] = new IntPtr(Arr[i]);
                }
                return Rst;
            }

            PipeWriter.Write((byte)PipeCommands.GetPtrs);
            PipeWriter.Flush();

            long Count = PipeReader.ReadInt64();
            IntPtr[] RstPtrs = new IntPtr[Count];

            for (long i = 0; i < Count; i++) {
                RstPtrs[i] = new IntPtr(PipeReader.ReadInt64());
            }

            return RstPtrs;
        }

        private static void AddMissed(string Line) {
            if (Multithread) {
                if (!Missed.Contains(Line))
                    Missed.Add(Line);
                return;
            }

            PipeWriter.Write((byte)PipeCommands.AddMissed);
            PipeWriter.Write(Line);
            PipeWriter.Flush();
        }

        private static bool ContainsMissed(string Line) {
            if (Multithread) {
                return Missed.Contains(Line);
            }

            PipeWriter.Write((byte)PipeCommands.FindMissed);
            PipeWriter.Write(Line);
            PipeWriter.Flush();
            return PipeReader.ReadByte() == (byte)PipeCommands.True;
        }
        private static bool ContainsKey(string Line, bool EnforceAtualDatabase = false) {
            if (Multithread) {
                if (StrRld.ContainsKey(Line))
                    return true;

                if (EnforceAtualDatabase)
                    return false;

                return (from x in Databases where x.ContainsKey(Line) select x).Count() > 0;
            }

            PipeWriter.Write((byte)PipeCommands.FindReload);
            PipeWriter.Write((byte)(EnforceAtualDatabase ? PipeCommands.True : PipeCommands.False));
            PipeWriter.Write(Line);
            PipeWriter.Flush();
            return PipeReader.ReadByte() == (byte)PipeCommands.True;
        }

        private static void AddMask(string ReadMask, string WriteMask) {
            if (Multithread) {
                MskRld.Add(ReadMask, WriteMask);
                return;
            }

            PipeWriter.Write((byte)PipeCommands.AddMask);
            PipeWriter.Write(ReadMask);
            PipeWriter.Write(WriteMask);
            PipeWriter.Flush();
        }

        private static bool ValidateMask(string String) {
            if (Multithread) {
                string[] Result = (from x in MskRld.Keys where MaskMatch(x, String) select x).ToArray();
                if (Result.Length > 0)
                    return true;
                return false;
            }

            PipeWriter.Write((byte)PipeCommands.ChkMask);
            PipeWriter.Write(String);
            PipeWriter.Flush();

            return PipeReader.ReadByte() == (byte)PipeCommands.True;
        }


        private static string ProcessMask(string Original) {
            if (Multithread) {
                string Mask = (from x in MskRld.Keys where MaskMatch(x, Original) select x).FirstOrDefault();
                return MaskReplace(Mask, Original, MskRld[Mask]);
            }

            PipeWriter.Write((byte)PipeCommands.RldMask);
            PipeWriter.Write(Original);
            PipeWriter.Flush();

            return PipeReader.ReadString();
        }

        private static void AddEntry(string Key, string Value) {
            if (Multithread) {
                StrRld.Add(Key, Value);
                return;
            }

            PipeWriter.Write((byte)PipeCommands.AddReload);
            PipeWriter.Write(Key);
            PipeWriter.Write(Value);
            PipeWriter.Flush();
        }
        
        private static void FinishDatabase() {
            if (Multithread) {
                if (DBID >= Databases.Count)
                    return;

                LastDBID++;
                DBID = LastDBID;
                return;
            }

            PipeWriter.Write((byte)PipeCommands.AdvDB);
            PipeWriter.Flush();
        }

        private static string GetEntry(string Key) {
            if (Multithread) {
                if (StrRld.ContainsKey(Key))
                    return StrRld[Key];

                string Result = string.Empty;
                for (DBID = 0; DBID < Databases.Count; DBID++) {
                    if (StrRld.ContainsKey(Key)) {
                        Result = StrRld[Key];
                        break;
                    }
                }

                if (Debugging)
                    Log("Database Changed to {0}, ID: {1}", true, GetDBNameById(DBID), DBID);

                return Result;
            }

            int OID = 0;

            if (Debugging) {
                OID = GetDBID();
            }

            PipeWriter.Write((byte)PipeCommands.GetReload);
            PipeWriter.Write(Key);
            PipeWriter.Flush();
            string Return = PipeReader.ReadString();

            if (Debugging) {
                int NID = GetDBID();
                if (OID != NID) {
                    Log("Database Changed to {0}, ID: {1}", true, GetDBNameById(NID), NID);
                }
            }

            return Return;
        }

        private static int GetDBID()
        {
            PipeWriter.Write((byte)PipeCommands.GetDBID);
            PipeWriter.Flush();
            return PipeReader.ReadInt32();
        }

        private static void SetDBID(int ID)
        {
            if (Debugging)
            {
                Log("Database Changed to {0}, ID: {1}", true, GetDBNameById(ID), ID);
            }

            if (Multithread)
            {
                DBID = ID;
                return;
            }

            PipeWriter.Write((byte)PipeCommands.SetDBID);
            PipeWriter.Write(ID);
            PipeWriter.Flush();
        }

        private static int GetCurrentDBIndex() {
            if (!AllowDuplicates)
                return -1;

            if (Multithread) {
                return ((DuplicableDictionary<string, string>)StrRld).LastKeyIndex;
            }

            PipeWriter.Write((byte)PipeCommands.GetDBIndex);
            PipeWriter.Flush();
            return PipeReader.ReadInt32();
        }
        private static int GetPipeID() {
            return new Random().Next(0, int.MaxValue);
        }

        private static void StartPipe() {
            if (Multithread) {
                Log("Pipe Service Disabled.", true);
                StrRld = CreateDictionary();
                MskRld = CreateDictionary();
                Missed = new List<string>();
                return;
            }

            if (PipeClient == null) {
                Log("Starting Pipe Service...", true);

                int ServiceID = GetPipeID();
                string Service = string.Format(ServiceMask, ServiceID);

                if (AllowDuplicates)
                    Service += ServiceDuplicateFlag;

#if DEBUG
                new System.Threading.Thread(() => ServiceCall(ServiceID.ToString())).Start();
#else
                string Args = string.Format("{0},{1} {2}", Path.GetFileName(SrlDll), "Service", ServiceID);
            
                var Proc = new System.Diagnostics.Process() {
                    StartInfo = new System.Diagnostics.ProcessStartInfo() {
                        UseShellExecute = false,
                        FileName = "rundll32",
                        Arguments = Args,
                        WorkingDirectory = Path.GetDirectoryName(SrlDll)
                    }
                };
                Proc.Start();
#endif
                while (PipeClient == null) {
                    try {
                        PipeClient = new NamedPipeClientStream(Service);
                        PipeClient.Connect();
                    } catch (Exception ex){
                        PipeClient = null;
                        Log("Pipe Exception: {0}", true, ex.Message);
                    }
                }
                PipeReader = new BinaryReader(PipeClient);
                PipeWriter = new BinaryWriter(PipeClient);
                Log("Pipe Service Started.", true);
            }
        }

    }
}
