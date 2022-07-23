using static StringReloads.Hook.Base.Extensions;
using static StringReloads.Engine.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StringReloads.Engine;
using StringReloads.Hook;
using StringReloads.Hook.Base;
using StringReloads.Engine.String;
using StringReloads.AutoInstall.Patcher;
using StringReloads.Engine.Interface;

namespace StringReloads.AutoInstall
{
    unsafe class SoftPalMethodA : IAutoInstall
    {

        bool SetupMode = false;
        CallerTracer Tracer;
        Interceptor Intercepter;

        Config Config => Config.Default;
        Dictionary<string, string> SoftPalConfig;

        void* hPalFontDrawText = null;
        public string Name => "SoftPal#A";

        uint StackOffset = 0;

        public void Install()
        {
            if (SetupMode) {
                Tracer = new CallerTracer(hPalFontDrawText);
                Tracer.CallerCatched += SetupStepA;
                Tracer.Install(); 
                ShowMessageBox("Very well. SRL will now do the dirty work of setting up the game for you. Your input is still needed, however; please start up the game and make it display a piece of dialogue.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
                return;
            }

            StackOffset = SoftPalConfig["stackoffset"].ToUInt32();
            void* hFunc = (void*)((int)Config.GameBaseAddress + SoftPalConfig["hookoffset"].ToInt32());
            Intercepter = new Interceptor(hFunc, new InterceptDelegate(DrawTextHook));
            Intercepter.Install();
        }

        public bool IsCompatible()
        {
            var DLLPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll", "pal.dll");

            if (!File.Exists(DLLPath))
                return false;

            var hModule = GetLibrary(DLLPath);
            if (hModule == null)
                return false;

            hPalFontDrawText = GetProcAddress(hModule, "PalFontDrawText");
            if (hPalFontDrawText == null)
                return false;

            SoftPalConfig = Config.GetValues("SoftPal");

            if (SoftPalConfig != null && SoftPalConfig.ContainsKey("forcemethodb"))
            {
                if (SoftPalConfig["forcemethodb"].ToBoolean())
                    return false;
            }

            if (SoftPalConfig == null || !SoftPalConfig.ContainsKey("enginesize") || SoftPalConfig["enginesize"].ToInt64() != new FileInfo(Config.GameExePath).Length)
            {
                var Rst = ShowMessageBox("SRL is not configured to work with this game yet, do you want to configure it now?", "StringReloads Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.No)
                    return false;

                if (GetProcAddress(hModule, "DrawText") != null || GetProcAddress(hModule, "drawText") != null)
                {
                    Rst = ShowMessageBox("This game may use a alternative text draw method, you want try it first?", "StringReloads Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                    if (Rst == MBResult.Yes)
                    {
                        Config.SetValue("SoftPal", "ForceMethodB", "true");
                        Config.SaveSettings();

                        ShowMessageBox("If this method not works, delete the ForceMethodB in the SRL.ini file to retry the Method A.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
                        return false;
                    }
                }

                Tools.ApplyWrapperPatch();
                SetupMode = true;
            }

            return true;
        }

        public void Uninstall()
        {
            Intercepter.Uninstall();
        }

        void DrawTextHook(void* Stack) {
            uint* Address = ((uint*)Stack) + StackOffset;
            *Address = (uint)EntryPoint.Process((void*)*Address);
        }

        void SetupStepA(void* Caller) {
            Log.Debug($"PalFontDrawText called from 0x{(uint)Caller:X8}");
            if (!SetupMode)
                return;
            
            ShowMessageBox("Yes, you did well. Now press OK and wait for SRL to analyze the text rendering fuction of the game...", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);

            var hFunc = Tracer.SearchFuntionAddress((byte*)Caller);

            if (hFunc == null)
            {
                Log.Debug("Failed to search the function address.");
                SetupFailed();
            }

       
            Log.Debug($"PalFontDrawText Referenced by 0x{(uint)hFunc:X8}");

            SoftPalConfig = new Dictionary<string, string>();
            SoftPalConfig["HookOffset"]  = ((uint)hFunc - (uint)Config.GameBaseAddress).ToString();
            SoftPalConfig["EngineSize"]  = new FileInfo(Config.GameExePath).Length.ToString();

            Tracer.Uninstall();


            Intercepter = new Interceptor(hFunc, new InterceptDelegate(SetupStepB));
            Intercepter.Install();
            ShowMessageBox($"Very well, looks like SRL can perform Auto-Install in this game!\nSRL now needs to gather more intricate information from the game.\nPlease press OK and continue to the next in-game dialogue.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
        }

        int LastOffset = 0;
        bool WaitingConfirmation = false;
        bool FirstTry = true;
        void SetupStepB(void* ESP) {
            if (FirstTry)
                ShowMessageBox("SRL will now need your help to confirm if the dialogue has been found.\nThe program will display the game dialogue; when the correct dialogue is shown, press YES. If nothing is shown or if you see corrupted text, press NO.\nTake note that if you don't set the proper game encoding inside SRL.ini, the correct dialogue will most likely never be shown.\nPress OK if you have read and understood the above.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);

            if (WaitingConfirmation)
            {
                WaitingConfirmation = false;
                var Rst = ShowMessageBox("And then, you saw the confirmation message?", "StringReloads Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.Yes) {
                    FinishSetup();
                }

                WaitingConfirmation = false;
            }

            uint* Stack = (uint*)ESP;
            int StackOffset = LastOffset;
            while (StackOffset < 50) {
                var RStack = (Stack + StackOffset);
                if (IsBadCodePtr((void*)*RStack)) {
                    Log.Debug($"Bad Pointer: 0x{*RStack:X8}");
                    StackOffset++;
                    continue;
                }

                Log.Debug($"Guessing Offset: 0x{(uint)RStack:X8} (+{StackOffset}) (0x{*RStack:X8})");
                CString Str = (byte*)*RStack;
                if (Str.Count() > 0 && Str.Count() < 500)
                {
                    var Reply = ShowMessageBox(Str, "Is this the dialogue?", MBButtons.YesNo, MBIcon.Question);
                    if (Reply == MBResult.Yes)
                    {
                        Str = "Looks Like everything is working,<br>Continue to the next game dialogue!";
                        *RStack = (uint)(void*)Str;
                        WaitingConfirmation = true;
                        break;
                    }
                }

                StackOffset++;
                continue;
            }

            LastOffset = StackOffset;

            if (StackOffset == 50)
                SetupFailed();


            if (FirstTry) {
                FirstTry = false;
                ShowMessageBox("Very well, SRL will now try translating the game text to test settings.\nIf you don't see a message in-game confirming what has been set, then continue on to the next in-game dialogue and press NO; otherwise, press YES.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Information);
            }
        }

        private void FinishSetup() {
            SoftPalConfig["StackOffset"] = LastOffset.ToString();
            if (Config.BreakLine != "<br>") {
                var Rst = ShowMessageBox("Looks like you aren't using the tag <br> as breakline rigth now, You want use it?", "StringReloader Setup Wizard", MBButtons.YesNo, MBIcon.Question);
                if (Rst == MBResult.Yes)
                    Config.SetValue("BreakLine", "<br>");
            }

            Config.SetValues("SoftPal", SoftPalConfig);
            Config.SaveSettings();

            ShowMessageBox($"Perfect! SRL is now ready to use! Enjoy!", "StringReloads", MBButtons.Ok, MBIcon.Information);
            Tools.Restart();
        }
        private void SetupFailed() {
            ShowMessageBox("Hmm, looks like SRL can't perform Auto-Install in this game at the moment. Please report an issue in the GitHub repository.", "StringReloads Setup Wizard", MBButtons.Ok, MBIcon.Error);
            Tools.Restart();
        }
    }
}
