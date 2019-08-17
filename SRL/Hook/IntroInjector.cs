using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SRL {

    static partial class StringReloader {

#if !DEBUG
        static CreateWindowExADelegate CreateWindowExADel;
        static CreateWindowExWDelegate CreateWindowExWDel;
        static UnmanagedHook CreateWindowExAHook;
        static UnmanagedHook CreateWindowExWHook;
#endif
        static ShowWindowDelegate ShowWindowDel;
        static SetWindowPosDelegate SetWindowPosDel;
        static MoveWindowDelegate MoveWindowDel;
        static UnmanagedHook ShowWindowHook;
        static UnmanagedHook SetWindowPosHook;
        static UnmanagedHook MoveWindowHook;

        static void InstallIntroInjector() {
            if (ShowWindowHook != null || Managed)
                return;
#if !DEBUG
            CreateWindowExADel = new CreateWindowExADelegate(hCreateWindowEx);
            CreateWindowExWDel = new CreateWindowExWDelegate(hCreateWindowEx);
            CreateWindowExAHook = AutoHookCreator("user32.dll", "CreateWindowExA", CreateWindowExADel);
            CreateWindowExWHook = AutoHookCreator("user32.dll", "CreateWindowExW", CreateWindowExWDel);
            if (HookCreateWindowEx)
                CreateWindowExADel = new CreateWindowExADelegate(hCreateWindowEx);
#endif

            ShowWindowDel = new ShowWindowDelegate(hShowWindow);
            ShowWindowHook = AutoHookCreator("user32.dll", "ShowWindow", ShowWindowDel);
            if (HookShowWindow)
                ShowWindowHook.Install();

            SetWindowPosDel = new SetWindowPosDelegate(hSetWindowPos);
            SetWindowPosHook = AutoHookCreator("user32.dll", "SetWindowPos", SetWindowPosDel);
            if (HookSetWindowPos)
                SetWindowPosHook.Install();

            MoveWindowDel = new MoveWindowDelegate(hMoveWindow);
            MoveWindowHook = AutoHookCreator("user32.dll", "MoveWindow", MoveWindowDel);
            if (HookMoveWindow)
                MoveWindowHook.Install();

            Log("Intro Injector Initialized...", true);
        }

        static bool IntroInitialized = false;
        static bool hShowWindow(IntPtr hWnd, int nCmdShow)
        {
            if (!ShowWindowHook.ImportHook)
                ShowWindowHook.Uninstall();

            bool Rst = ShowWindow(hWnd, nCmdShow);

            if (!ShowWindowHook.ImportHook)
                ShowWindowHook.Install();

            if (nCmdShow != SW_HIDE)
                ShowIntro(hWnd);

            return Rst;
        }

        static bool hSetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags)
        {
            if (!SetWindowPosHook.ImportHook)
                SetWindowPosHook.Uninstall();

            bool Rst = SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags);

            if (!SetWindowPosHook.ImportHook)
                SetWindowPosHook.Install();
            ShowIntro(hWnd);

            return Rst;
        }

        static bool hMoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint)
        {
            if (!MoveWindowHook.ImportHook)
                MoveWindowHook.Uninstall();

            bool Rst = MoveWindow(hWnd, X, Y, nWidth, nHeight, bRepaint);

            if (!MoveWindowHook.ImportHook)
                MoveWindowHook.Install();

            ShowIntro(hWnd);

            return Rst;
        }

#if !DEBUG
        static IntPtr hCreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam)
        {
            if (!CreateWindowExWHook.ImportHook)
                CreateWindowExWHook.Uninstall();

            IntPtr Result = CreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

            if (!CreateWindowExWHook.ImportHook)
                CreateWindowExWHook.Install();

            ShowIntro(Result);
            return Result;
        }
#endif
        static void ShowIntro(IntPtr hWnd) {
            if (IntroInitialized)
                return;
            if (hWnd == hConsole)
                return;

            IntroInitialized = true;
            try {

                var WindowSize = GetWindowSize(hWnd);
                if (WindowSize.Height < MinSize || WindowSize.Width < MinSize) {
                    IntroInitialized = false;
                    return;
                }

                if (CheckProportion && WindowSize.Width < WindowSize.Height) {
                    IntroInitialized = false;
                    return;
                }

                if (!IsWindowVisible(hWnd))
                    ShowWindow(hWnd, SW_SHOW);

                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);


                IntroHelper[] Intros = (from x in Introduction select LoadIntro(x, WindowSize)).ToArray();

                using (Graphics Window = Graphics.FromHwnd(hWnd)) {
                    var OriState = Window.Save();
                    Window.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    Window.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    Window.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    Window.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                    foreach (var Intro in Intros) {
                        Window.Clear(Intro.Background);

                        bool Loaded = false;
                        bool Finished = false;

                        if (Intro.Sound != null)
                            new Thread(() => {
                                try {
                                    using (var Stream = new MemoryStream(Intro.Sound))
                                    using (var Audio = new SoundPlayer(Stream)) {
                                        Audio.Load();
                                        Loaded = true;
                                        Audio.PlaySync();
                                        Finished = true;
                                    }
                                } catch (Exception ex) {
                                    Error("INTRO ERROR: {0}", ex);
                                }
                            }).Start();
                        else Loaded = true;

                        while (!Loaded)
                            Thread.Sleep(5);

                        bool Switch = false;
                        foreach (var Image in Intro.Fade) {
                            if (Finished)
                                break;
                            
                            Window.DrawImage(Image, Intro.Position);
                            Window.Flush();

                            DoEvents(hWnd);

                            if (Switch) {
                                Switch = false;
                                Thread.Sleep(10);
                            } else Switch = true;
                        }

                        if (Seconds == 0) {
                            while (!Finished) {
                                DoEvents(hWnd);

                                if (Switch) {
                                    Switch = false;
                                    Thread.Sleep(10);
                                } else Switch = true;
                            }
                        } else {
                            int Interval = Seconds > 0 ? Seconds * 1000 : 3000;
                            DateTime Begin = DateTime.Now;
                            while ((int)(DateTime.Now - Begin).TotalMilliseconds < Interval) {
                                DoEvents(hWnd);

                                if (Switch) {
                                    Switch = false;
                                    Thread.Sleep(10);
                                } else Switch = true;
                            }
                        }
                        

                        foreach (var Image in Intro.Fade.Reverse()) {
                            Window.DrawImage(Image, Intro.Position);
                            Window.Flush();

                            DoEvents(hWnd);

                            Image.Dispose();
                            if (Switch) {
                                Switch = false;
                                Thread.Sleep(10);
                            } else Switch = true;
                        }

                        Window.Restore(OriState);
                    }


                    Window.Dispose();
                }

            } catch (Exception ex) {
                Error("INTRO ERROR: {0}", ex);
            }
        }

        static void DoEvents(IntPtr hWnd) {
            RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, RDW_UPDATENOW);
            Application.DoEvents();
        }

        static IntroHelper LoadIntro(IntroContainer Intro, Size Size) {
            var Helper = new IntroHelper();
            int Steps = 100;

            Helper.Fade = new Bitmap[Steps];

            using (var Texture = Intro.Texture) {
                Color Pixel = Texture.GetPixel(0, 0);
                Helper.Background = Color.FromArgb(255, Pixel);

                Helper.Position = Texture.Size.GetCenterPoint(Size);
            }


            for (int Step = 0; Step < Steps; Step++) {
                using (var Texture = Intro.Texture) {
                    Bitmap Opacity = SetBitmapOpacity(Texture, ((float)Step) / Steps);
                    Bitmap Result = new Bitmap(Texture.Width, Texture.Height);
                    using (Graphics Draw = Graphics.FromImage(Result)) {
                        Draw.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        Draw.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        Draw.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                        Draw.Clear(Helper.Background);
                        Draw.DrawImage(Opacity, 0, 0);
                        Draw.Flush();
                        Helper.Fade[Step] = Result;
                    }
                    Opacity.Dispose();
                }
            }


            if (Intro.HasSound)
                Helper.Sound = Intro.Wav;
            else
                Helper.Sound = null;

            return Helper;
        }

        static Point GetCenterPoint(this Size Content, Size Background) {
            int X = (Background.Width / 2) - (Content.Width / 2);
            int Y = (Background.Height / 2) - (Content.Height / 2);

            return new Point(X, Y);
        }

        static Size GetWindowSize(IntPtr hWnd) {
            if (!GetWindowRect(hWnd, out RECT Rectangle))
                return new Size(1, 1);

            return Rectangle.Size;
        }

        //https://stackoverflow.com/questions/4779027/changing-the-opacity-of-a-bitmap-image
        static Bitmap SetBitmapOpacity(Bitmap image, float opacity) {
            try {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp)) {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            } catch (Exception ex) {
                return null;
            }
        }

        static bool ValidExport(string Module, string Function)
        {
            var hModule = LoadLibrary(Module);
            if (hModule == IntPtr.Zero)
                return false;
            return GetProcAddress(hModule, Function) != IntPtr.Zero;
        }

        struct IntroHelper {
            public Bitmap[] Fade;
            public Color Background;
            public Point Position;
            public byte[] Sound;

            public int Seconds => Sound == null ? -1 : 3;
        }

        [DllImport("win32u.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern bool NtUserShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("win32u.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern bool NtUserSetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("win32u.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool NtUserMoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]

        delegate bool ShowWindowDelegate(IntPtr hWnd, int nCmdShow);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]

        delegate bool SetWindowPosDelegate(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]

        delegate bool MoveWindowDelegate(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    }
}
