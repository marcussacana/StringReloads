using System;
using System.IO;
using StringReloads.Hook;
using StringReloads.Engine.Interface;
using StringReloads.AutoInstall.Patcher;
using static StringReloads.Hook.Base.Extensions;

namespace StringReloads.AutoInstall
{
    unsafe class SoftPalMethodB : IAutoInstall
    {
        SoftPal_DrawText Hook;
        public string Name => "SoftPal#B";

        public void Install() => Hook.Install();

        public bool IsCompatible()
        {
            var DLLPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll", "pal.dll");

            if (!File.Exists(DLLPath))
                return false;

            var hModule = GetLibrary(DLLPath);
            if (hModule == null)
                return false;

            var hDrawText = GetProcAddress(hModule, "DrawText");
            if (hDrawText == null)
            {
                hDrawText = GetProcAddress(hModule, "drawText");
                if (hDrawText == null)
                    return false;

                SoftPal_DrawText.AltName = true;
            }

            ExeTools.ApplyWrapperPatch();
            Hook = new SoftPal_DrawText();
            return true;
        }

        public void Uninstall() => Hook.Uninstall();
    }
}
