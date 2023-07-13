using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Hook.Others;
using StringReloads.Hook.Win32;
using System;
using System.Diagnostics;
using System.Linq;

namespace StringReloads.AutoInstall
{
    class ExHIBIT : IAutoInstall
    {
        public string Name => "ExHIBIT";

        ExHIBIT_lstrcpyA lstrcpyAHook;

        ExHIBIT_Say10 Say10Hook;
        ExHIBIT_Say11 Say12Hook;
        ExHIBIT_PrintSub3 PrintSub3Hook;
        ExHIBIT_Question QuestionHook;
        public void Install()
        {
            if (Say10Hook == null && Say12Hook == null)
            {
                try
                {
                    Say10Hook = new ExHIBIT_Say10();
                }
                catch 
                { 
                    Say12Hook = new ExHIBIT_Say11();
                }
            }

            if (QuestionHook == null)
            {
                try
                {
                    QuestionHook = new ExHIBIT_Question();
                }
                catch { }
            }

            if (PrintSub3Hook == null)
                PrintSub3Hook = new ExHIBIT_PrintSub3();

            var Modules = Process.GetCurrentProcess().Modules.Cast<ProcessModule>();
            var Resident = (from x in Modules where x.ModuleName.ToLowerInvariant() == "resident.dll" select x.BaseAddress).Single();

            if (lstrcpyAHook == null)
                lstrcpyAHook = new ExHIBIT_lstrcpyA(Resident);

            Say10Hook?.Install();
            Say12Hook?.Install();
            PrintSub3Hook.Install();

            if (QuestionHook == null)
                lstrcpyAHook.Install();
            else
                QuestionHook.Install();
        }

        public bool IsCompatible()
        {
            if (!System.IO.File.Exists("resident.dll"))
                return false;

            if (Config.Default.HookEnabled("lstrcpyA"))
            {
                Log.Error("The ExHIBIT Auto-Install require to disable the lstrcpyA Hook.");
                return false;
            }

            return true;
        }

        public void Uninstall()
        {
            Say10Hook?.Uninstall();
            Say12Hook?.Uninstall();
            QuestionHook?.Uninstall();
            PrintSub3Hook.Uninstall();
            lstrcpyAHook.Uninstall();
        }
    }
}
