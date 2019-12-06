using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static SRL.StringReloader;

namespace SRL.Engine
{
    class SoftPal : IEngine
    {
        bool PostSetupMode = false;
        bool SetupMode = false;
        SoftPALSettings Settings;

        CallInterceptor Interceptor;

        bool HookReady {
            get {
                if (Settings.Checksum == 0)
                    return false;

                var EXE = File.ReadAllBytes(Application.ExecutablePath);
                PECSVal(EXE, out uint GameChecksum);
                return Settings.Checksum == GameChecksum;
            }
        }

        byte Method = byte.MaxValue;
        string HookMethod {
            get {
                switch (Method)
                {
                    case 0: return "v1.17";
                    case 1: return "v1.15";
                    case 2: return "v1.17b";
                    case 3: return "v1.15b";
                    default: return "JIT";
                }
            }
        }
        public string Name => $"SoftPAL ({HookMethod})";

        DrawText DrawTextHookDelegate;
        DrawText DrawTextRealDelegate;

        UnmanagedHook<DrawText> DrawTextHookManager;

        PalFontDrawText PalFontDrawTextHookDelegate;
        PalFontDrawText PalFontDrawTextRealDelegate;

        UnmanagedHook<PalFontDrawText> PalFontDrawTextHookManager;

        IntPtr PalHandler;
        IntPtr RealFunc;
        IntPtr HookFunc;

        IntPtr SetupFunc;

        public bool IsCompatible()
        {
            foreach (var Import in UnmanagedHook.GetImports())
            {
                if (Import.Module.ToLower() == "pal.dll" && Import.Function?.ToLower() == "drawtext")
                {
                    DrawTextRealDelegate = (DrawText)Marshal.GetDelegateForFunctionPointer(Import.FunctionAddress, typeof(DrawText));

                    DrawTextHookDelegate = DrawTextHook;

                    DrawTextHookManager = new UnmanagedHook<DrawText>(Import, DrawTextHookDelegate);

                    Method = 0;
                    return true;
                }

                if (Import.Module.ToLower() == "pal.dll" && Import.Function?.ToLower() == "palfontdrawtext")
                {
                    PalFontDrawTextRealDelegate = (PalFontDrawText)Marshal.GetDelegateForFunctionPointer(Import.FunctionAddress, typeof(PalFontDrawText));

                    PalFontDrawTextHookDelegate = PalFontDrawTextHook;

                    PalFontDrawTextHookManager = new UnmanagedHook<PalFontDrawText>(Import, PalFontDrawTextHookDelegate);

                    Method = 1;
                    return true;
                }
            }

            string DLL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll", "pal.dll");
            if (File.Exists(DLL))
            {
                PalHandler = LoadLibrary(DLL);
                if (PalHandler == IntPtr.Zero)
                    return false;

                OnLoadLibrary += (Name) => {
                    string Module = Path.GetFileName(Name).ToLower();
                    if (Module == "pal.dll")
                        return PalHandler;
                    return null;
                };

                OnGetProcAddressName += (Module, Func) =>
                {

                    if (Module == PalHandler)
                    {
                        switch (Func.ToLower())
                        {
                            case "drawtext":
                                if (HookFunc == IntPtr.Zero)
                                {
                                    Method = 2;
                                    RealFunc = GetProcAddress(PalHandler, Func);
                                    DrawTextRealDelegate = (DrawText)Marshal.GetDelegateForFunctionPointer(RealFunc, typeof(DrawText));
                                    DrawTextHookDelegate = DrawTextHook;
                                    HookFunc = Marshal.GetFunctionPointerForDelegate(DrawTextHookDelegate);

                                    if (Debugging || Verbose)
                                        Log($"{Name} Hook Initialized", true);
                                }
                                break;
                            case "palfontdrawtext":
                                if (HookFunc == IntPtr.Zero)
                                {
                                    Method = 3;
                                    RealFunc = GetProcAddress(PalHandler, Func);
                                    PalFontDrawTextRealDelegate = (PalFontDrawText)Marshal.GetDelegateForFunctionPointer(RealFunc, typeof(PalFontDrawText));
                                    PalFontDrawTextHookDelegate = PalFontDrawTextHook;
                                    HookFunc = Marshal.GetFunctionPointerForDelegate(PalFontDrawTextHookDelegate);

                                    if (SetupMode)
                                    {
                                        if (Interceptor == null)
                                        {
                                            Interceptor = new CallInterceptor(RealFunc, true, true);
                                            Interceptor.OnIntercepted += DrawTextInfoReciver;
                                            SetupFunc = Interceptor.HookAddress;
                                            Log($"{Name} Hook Setup Initialized", true);
                                        }
                                    }
                                    else if (Debugging || Verbose)
                                        Log($"{Name} Hook Initialized", true);
                                }
                                break;
                            default:
                                return null;
                        }

                        if (PostSetupMode || WaitingConfirmation)
                            return RealFunc;

                        if (SetupMode)
                            return SetupFunc;

                        return HookFunc;
                    }
                    return null;
                };

                Settings = new SoftPALSettings();
                AdvancedIni.FastOpen(out Settings, IniPath);

                InstallLoadLibraryHooks();
                Method = 3;

                if (!HookReady && !SetupMode)
                {
                    var Rst = MessageBox.Show("SRL is not configured to work with this game yet, do you want to configure it now?", "StringReloader", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Rst == DialogResult.No)
                    {
                        return false;
                    }

                    SetupMode = true;

                    if (GameLineBreaker != "<br>")
                    {
                        Rst = MessageBox.Show("Looks like you aren't using the tag <br> as breakline rigth now, You want use it?", "StringReloader Setup Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (Rst == DialogResult.Yes)
                        {
                            LiveSettings = false;
                            GameLineBreaker = "<br>";

                            System.Threading.Thread.Sleep(1000);

                            if (Ini.GetConfigStatus(CfgName, "GameBreakLine", IniPath) == Ini.ConfigStatus.Ok)
                                Ini.SetConfig(CfgName, "GameBreakLine", GameLineBreaker, IniPath);
                           else
                                Ini.SetConfig(CfgName, "BreakLine", GameLineBreaker, IniPath);

                        }
                    }

                    MessageBox.Show("Very well. SRL will now do the dirty work of setting up the game for you. Your input is still needed, however; please start up the game and make it display a piece of dialogue.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Interceptor = new CallInterceptor(new IntPtr(unchecked((int)(Settings.Offset + GameBaseAddress))), Settings.PersistentHook);
                    Interceptor.OnIntercepted += TextDrawInterception;
                }

                return true;
            }
            return false;
        }

        public void InstallStrHook()
        {
            switch (Method)
            {
                case 2:
                case 0:
                    DrawTextHookManager.Install();
                    break;
                case 1:
                    PalFontDrawTextHookManager.Install();
                    break;
                case 3:
                    Interceptor?.Install();
                    break;
                default:
                    throw new Exception("Hook ID Not Found");
            }
        }

        public void UninstallStrHook()
        {
            switch (Method)
            {
                case 2:
                case 0:
                    DrawTextHookManager.Uninstall();
                    break;
                case 1:
                    PalFontDrawTextHookManager.Uninstall();
                    break;
                case 3:
                    Interceptor?.Uninstall();
                    break;
                default:
                    throw new Exception("Hook ID Not Found");
            }
        }

        public void DrawTextHook(IntPtr Text, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15, IntPtr a16, IntPtr a17, IntPtr a18, IntPtr a19, IntPtr a20, IntPtr a21, IntPtr a22, IntPtr a23)
        {
            Text = ProcessReal(Text);
            DrawTextRealDelegate(Text, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18, a19, a20, a21, a22, a23);
        }

        public IntPtr PalFontDrawTextHook(IntPtr a1, IntPtr Text, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6)
        {
            Text = ProcessReal(Text);
            return PalFontDrawTextRealDelegate(a1, Text, a3, a4, a5, a6);
        }

        void TextDrawInterception(IntPtr Stack, IntPtr Return)
        {
            Stack = Stack.Sum(Settings.StackOffset * 4);
            IntPtr NewPtr = ProcessReal(Marshal.ReadIntPtr(Stack));
            Marshal.WriteIntPtr(Stack, NewPtr);
        }

        //Engine v1.17
        //PAL v1.10 - 2019
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DrawText(IntPtr Text, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15, IntPtr a16, IntPtr a17, IntPtr a18, IntPtr a19, IntPtr a20, IntPtr a21, IntPtr a22, IntPtr a23);

        //Engine v1.15
        //PAL v1.10 - 2006-2017 
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr PalFontDrawText(IntPtr a1, IntPtr Text, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6);

        struct SoftPALSettings
        {
            [FieldParmaters(DefaultValue = 0u, Name = "Checksum")]
            public uint Checksum;

            [FieldParmaters(DefaultValue = 0u, Name = "Offset")]
            public uint Offset;

            [FieldParmaters(DefaultValue = 0u, Name = "StackOffset")]
            public uint StackOffset;

            [FieldParmaters(DefaultValue = false, Name = "PersistentHook")]
            public bool PersistentHook;
        }

        void DrawTextInfoReciver(IntPtr Stack, IntPtr Return)
        {
            if (Verbose)
                Log("DrawText Called From {0:X8} (ESP: {1:X8})", true, Return.ToUInt32(), Stack.ToUInt32());

            if (!SetupMode)
                return;

            SetupMode = false;

            MessageBox.Show("Yes, you did well. Now press OK and wait for SRL to analyze the text rendering fuction of the game...", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
         
            bool? SupportedFunc = null;
            IntPtr Offset = IntPtr.Zero;
            int Sub = 0;
            while (Sub < 0x900)
            {
                Offset = Return.Sum(Sub++ * -1);
                if (CallInterceptor.IsInterceptable(ref Offset, out SupportedFunc))                
                    break;
            }

            if (!SupportedFunc.HasValue) {

                if (Verbose)
                    Warning("Unsupported SoftPAL Game Detected", true);

                MessageBox.Show("Hmm, looks like SRL can't perform Auto-Install in this game at the moment. Please report an issue in the GitHub repository.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Settings.PersistentHook = SupportedFunc.Value;
            Settings.StackOffset = 0;
            Settings.Offset = Offset.ToUInt32() - GameBaseAddress;

            var EXE = File.ReadAllBytes(Application.ExecutablePath);
            PECSVal(EXE, out uint GameChecksum);
            Settings.Checksum = GameChecksum;

            if (Verbose)
                Log($"Supported {(SupportedFunc.Value ? "Persistent" : "Non-Persistent")} Hook at 0x{Offset.ToUInt32():X8}", true);


            PostSetupMode = true;

            Interceptor = new CallInterceptor(Offset, false);
            Interceptor.OnIntercepted += PostSetupInterception;
            Interceptor.AfterInvoked += PostSetupAfterInvoke;
            Interceptor.Install();

            MessageBox.Show($"Very well, looks like SRL can perform Auto-Install in this game, {(SupportedFunc.Value ? "optimally!" : ", albeit non-optimally.")} SRL now needs to gather more intricate information from the game. Please press OK and continue to the next in-game dialogue.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static bool WaitingConfirmation;
        static int LastOffset = -1;
        static bool PostSetupInstructionViewed;
        void PostSetupInterception(IntPtr Stack, IntPtr Return)
        {
            if (WaitingConfirmation)
            {
                WaitingConfirmation = false;
                var Rst = MessageBox.Show("And then, you saw the confirmation message?", "StringReloader Setup Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Rst == DialogResult.Yes)                
                    return;
                
                PostSetupMode = true;
            }

            if (!PostSetupMode)
                return;

            if (Verbose)
                Log("SoftPAL Recived Stack: 0x{0:X8} (Return to: 0x{1:X8})", true, Stack.ToUInt32(), Return.ToInt32());

            if (!PostSetupInstructionViewed)
                MessageBox.Show("SRL will now need your help to confirm if the dialogue has been found.\nThe program will display the game dialogue; when the correct dialogue is shown, press YES. If nothing is shown or if you see corrupted text, press NO.\nTake note that if you don't set the proper game encoding inside SRL.ini, the correct dialogue will most likely never be shown.\nPress OK if you have read and understood the above.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);


            PostSetupInstructionViewed = true;

            int Offset = LastOffset;
            bool Loop = true;
            while (Loop)
            {
                int StackValue = Marshal.ReadInt32(Stack, (++Offset) * 4);
                IntPtr StrAddress = new IntPtr(StackValue);

                try
                {
                    if (IsBadCodePtr(StrAddress))
                    {
                        if (Verbose)
                            Log("Bad Pointer: 0x{0:X8}", true, StrAddress.ToUInt32());
                        continue;
                    }

                    if (Verbose)
                        Log("Guessing Offset: 0x{0:X8} (+{1}) (0x{2:X8})", true, Stack.Sum(Offset * 4).ToUInt32(), Offset, StrAddress.ToUInt32());

                    string Str = GetString(StrAddress, MaxLen: 501, Internal: true);
                    if (Str?.Length > 0 && Str?.Length < 500)
                    {
                        var Reply = MessageBox.Show(Str, "Is this the dialogue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        switch (Reply)
                        {
                            case DialogResult.Yes:
                                byte[] Buffer = WriteEncoding.GetBytes($"Looks Like everything is working,{GameLineBreaker}Continue to the next game dialogue!\x0");

                                IntPtr NewStr = Marshal.AllocHGlobal(Buffer.Length);
                                Marshal.Copy(Buffer, 0, NewStr, Buffer.Length);
                                Marshal.WriteInt32(Stack, Offset * 4, NewStr.ToInt32());

                                Settings.StackOffset = (uint)Offset;
                                LastOffset = Offset;

                                PostSetupMode = false;
                                WaitingConfirmation = true;
                                return;

                            case DialogResult.No:
                                break;
                            default:
                                Environment.Exit(0);
                                break;
                        }
                    }
                }
                catch { }
            }

        }

        void PostSetupAfterInvoke()
        {
            if (PostSetupMode)
                return;

            if (WaitingConfirmation)
            {
                MessageBox.Show("Very well, SRL will now try translating the game text to test settings.\nIf you don't see a message in-game confirming what has been set, then continue on to the next in-game dialogue and press NO; otherwise, press YES.", "StringReloader Setup Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AdvancedIni.FastSave(Settings, IniPath);
            MessageBox.Show($"Perfect! SRL is now ready to use! Enjoy!", "StringReloader", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Restart();
        }
    }
}
