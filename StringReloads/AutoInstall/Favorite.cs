using Iced.Intel;
using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Engine.String;
using StringReloads.Engine.Unmanaged;
using StringReloads.Hook;
using StringReloads.Hook.Base;
using StringReloads.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringReloads.AutoInstall
{
    internal class Favorite : IAutoInstall
    {
        long Address = 0;
        Register Register = Register.None;
        Intercept Interceptor = null;

        public string Name => "FavoriteViewPoint";

        public void Install()
        {
            DetectHookConfig();
            Interceptor.Install();
        }
        unsafe void DetectHookConfig()
        {
            var Settings = Config.Default.GetValues("Favorite");

            if (Settings == null)
            {
                foreach (var Import in Scanner.SearchExportCall("lstrlenA"))
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var Addr = Import - i;
                        var Dissassembler = Decoder.Create(32, new MemoryCodeReader((byte*)Addr));
                        Dissassembler.IP = (ulong)Addr;
                        var Instruction = Dissassembler.PeekDecode();

                        switch (Instruction.Code)
                        {
                            case Code.Push_r32:
                                Register = Instruction.Op0Register;
                                break;
                            default:
                                continue;
                        }

                        Address = Import;
                        break;
                    }
                }

                Settings = new Dictionary<string, string>();
                Settings["Address"] = Address.ToString();
                Settings["Register"] = ((int)Register).ToString();
                Config.Default.SetValues("Favorite", Settings);
                Config.Default.SaveSettings();
            }
            else
            {
                Address = long.Parse(Settings["address"]);
                Register = (Register)int.Parse(Settings["register"]);
            }

            Interceptor = new ManagedInterceptor((void*)Address, onIntercepted);
            Log.Information($"Favorite Engine Intercepted at 0x{Address:X8}");
        }

        private unsafe void onIntercepted(ref ulong ESP, ref ulong EAX, ref ulong ECX, ref ulong EDX, ref ulong EBX, ref ulong EBP, ref ulong ESI, ref ulong EDI)
        {
            switch (Register)
            {
                case Register.EBX:
                    EBX = (ulong)EntryPoint.Process((void*)EBX);
                    EAX = (ulong)((CString)EBX).Count();
                    break;
                case Register.ECX:
                    ECX = (ulong)EntryPoint.Process((void*)ECX);
                    EAX = (ulong)((CString)ECX).Count();
                    break;
                case Register.EDX:
                    EDX = (ulong)EntryPoint.Process((void*)EDX);
                    EAX = (ulong)((CString)EDX).Count();
                    break;
                case Register.EBP:
                    EBP = (ulong)EntryPoint.Process((void*)EBP);
                    EAX = (ulong)((CString)EBP).Count();
                    break;
                case Register.ESI:
                    ESI = (ulong)EntryPoint.Process((void*)ESI);
                    EAX = (ulong)((CString)ESI).Count();
                    break;
                case Register.EDI:
                    EDI = (ulong)EntryPoint.Process((void*)EDI);
                    EAX = (ulong)((CString)EDI).Count();
                    break;
            }
        }

        public bool IsCompatible()
        {
            return Directory.GetFiles(".\\", "*.hcb").Any();
        }

        public void Uninstall()
        {
            Interceptor.Uninstall();
        }
    }
}
