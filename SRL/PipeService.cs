using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRL {
    static partial class StringReloader {        
        public static int ServiceCall(string ID) {
            string SName = string.Format(ServiceMask, ID);

            if (Debugging)
                Log("Debug Mode Enabled.");

            Task Running = Task.Factory.StartNew(() => {
                StrRld = new System.Collections.Generic.Dictionary<string, string>();
                Missed = new System.Collections.Generic.List<string>();

                NamedPipeServerStream Server = new NamedPipeServerStream(SName, PipeDirection.InOut, 2, PipeTransmissionMode.Byte);
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
                            if (!StrRld.ContainsKey(Original))
                                StrRld.Add(Original, Reloader);
                            Log("Command Finished, In: {0}, Out: {1}", true, 2, 0);
                            break;
                        case PipeCommands.FindReload:
                            OK = true;
                            if (StrRld.ContainsKey(Reader.ReadString()))
                                Writer.Write((byte)PipeCommands.True);
                            else
                                Writer.Write((byte)PipeCommands.False);
                            Writer.Flush();
                            Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                            break;
                        case PipeCommands.GetReload:
                            OK = true;
                            string RLD = Reader.ReadString();
                            try {
                                Writer.Write(StrRld[RLD]);
                            } catch (Exception ex) {
                                Writer.Write(RLD);
                                Log("Exception Handled\n=================\n{0}\n{1}", true, ex.Message, ex.StackTrace);
                            }
                            Writer.Flush();
                            Log("Command Finished, In: {0}, Out: {1}", true, 1, 1);
                            break;
                        case PipeCommands.AddMissed:
                            OK = true;
                            Missed.Add(Reader.ReadString());
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
                            Ptrs.Add(Reader.ReadInt64());
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
                            if (Debugging) {
                                File.WriteAllText("DEBUG", "CloseRecived");
                            }
                            Environment.Exit(0);
                            break;
                        default:
                            if (!OK)
                                MessageBox.Show("Recived Invalid Command to the pipe service...", "SRL Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            OK = false;
                            break;
                    }
                }
            });
            Running.Wait();
            return 0;
        }

        private static void EndPipe() {
            PipeWriter.Write((byte)PipeCommands.EndPipe);
            PipeWriter.Flush();
        }
        private static void AddPtr(IntPtr Ptr) {
            long Pointer = Ptr.ToInt64();
            PipeWriter.Write((byte)PipeCommands.AddPtr);
            PipeWriter.Write(Pointer);
            PipeWriter.Flush();
        }

        private static IntPtr[] GetPtrs() {
            PipeWriter.Write((byte)PipeCommands.GetPtrs);
            PipeWriter.Flush();

            long Count = PipeReader.ReadInt64();
            IntPtr[] Ptrs = new IntPtr[Count];

            for (long i = 0; i < Count; i++) {
                Ptrs[i] = new IntPtr(PipeReader.ReadInt64());
            }

            return Ptrs;
        }

        private static void AddMissed(string Line) {
            PipeWriter.Write((byte)PipeCommands.AddMissed);
            PipeWriter.Write(Line);
            PipeWriter.Flush();
        }

        private static bool ContainsMissed(string Line) {
            PipeWriter.Write((byte)PipeCommands.FindMissed);
            PipeWriter.Write(Line);
            PipeWriter.Flush();
            return PipeReader.ReadByte() == (byte)PipeCommands.True;
        }
        private static bool ContainsKey(string Line) {
            PipeWriter.Write((byte)PipeCommands.FindReload);
            PipeWriter.Write(Line);
            PipeWriter.Flush();
            return PipeReader.ReadByte() == (byte)PipeCommands.True;
        }

        private static void AddEntry(string Key, string Value) {
            PipeWriter.Write((byte)PipeCommands.AddReload);
            PipeWriter.Write(Key);
            PipeWriter.Write(Value);
            PipeWriter.Flush();
        }

        private static string GetEntry(string Key) {
            PipeWriter.Write((byte)PipeCommands.GetReload);
            PipeWriter.Write(Key);
            PipeWriter.Flush();
            return PipeReader.ReadString();
        }
        private static int GetPipeID() {
            return new Random().Next(0, int.MaxValue);
        }

        private static void StartPipe() {
            if (PipeClient == null) {
                Log("Starting Pipe Service...", true);

                int ServiceID = GetPipeID();
                string Service = string.Format(ServiceMask, ServiceID);
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
