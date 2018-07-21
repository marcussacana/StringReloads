using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Overlay {
    public partial class TextOverlay : Form, IOverlay {

        Bitmap Content = new Bitmap(500, 500, PixelFormat.Format32bppArgb);
        public TextOverlay() {
            TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;

            TopLevel = true;

            MouseClick += MouseClicked;
        }

        private static bool Initializing = false;
        private static TextOverlay _DefaultInstance;

        public static TextOverlay DefaultInstance {
            get {
                while (Initializing) {
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                if (_DefaultInstance == null || _DefaultInstance.IsDisposed || !_DefaultInstance.CanInvoke()) {
                    if (!Exports.TextOnly)
                        return null;
                    Initializing = true;
                    _DefaultInstance = new TextOverlay();
                    _DefaultInstance.Show();
                    _DefaultInstance.Hide();
                    Initializing = false;
                }

                return _DefaultInstance;
            }
        }

        public bool CanInvoke() {
            try {
                Invoke(new MethodInvoker(() => { }));
                return true;
            } catch {
                return false;
            }

        }

        public new bool AutoSize = true;

        string _text = string.Empty;
        public new string Text {
            get {
                return _text;
            }
            set {
                if (Application.OpenForms.OfType<TextOverlay>().Count() == 0)
                    Show();

                _text = value;
                DrawBitmap();
            }
        }

        Color _oc = Color.Black;
        public Color TextBackColor {
            get {
                return _oc;
            }
            set {
                _oc = value;
                DrawBitmap();
            }
        }
        Color _tc = Color.White;
        public Color TextForeColor {
            get {
                return _tc;
            }
            set {
                _tc = value;
                DrawBitmap();
            }
        }
        private void DrawBitmap() {
            lock (Content) {
                Content = new Bitmap(Content.Width, Content.Height, PixelFormat.Format32bppArgb);
                Content.MakeTransparent(Color.Turquoise);

                Graphics g = Graphics.FromImage(Content);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Font Font = this.Font;
                if (AutoSize)
                    Font = GetBestSize(g);

                SizeF MeasuredSize = g.MeasureString(_text, Font);
                PointF TPos = new PointF((Size.Width / 2) - (MeasuredSize.Width / 2), (Size.Height / 2) - (MeasuredSize.Height / 2));

                using (var path = GetStringPath(Text, g.DpiY, new RectangleF(TPos, MeasuredSize), Font)) {
                    g.FillPath(new SolidBrush(_tc), path);
                    g.DrawPath(new Pen(_oc, 1f), path);
                    g.Flush();
                    g.Dispose();
                }

                SelectBitmap(Content);
            }
        }

        Font GetBestSize(Graphics g) {
            bool First = true;
            Font Font = this.Font;
            SizeF TextSize;

            do {
                if (!First)
                    Font = new Font(Font.FontFamily, Font.Size - 1, Font.Style);
                First = false;
                TextSize = g.MeasureString(Text, Font);
            } while ((TextSize.Width > Size.Width || TextSize.Height > Size.Height) & Font.Size > 5);

            return Font;
        }
        GraphicsPath GetStringPath(string s, float dpi, RectangleF rect, Font font) {
            GraphicsPath path = new GraphicsPath();
            float emSize = dpi * font.SizeInPoints / 72;
            path.AddString(s, font.FontFamily, (int)font.Style, emSize, rect, new StringFormat());

            return path;
        }

        public new Size Size {
            get {
                return Content.Size;
            }
            set {
                Content = new Bitmap(value.Width, value.Height);
                base.Size = value;
            }
        }


        public void SelectBitmap(Bitmap bitmap) {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb) {
                throw new ApplicationException("The bitmap must be 32bpp with alpha-channel.");
            }

            IntPtr screenDc = APIHelp.GetDC(IntPtr.Zero);
            IntPtr memDc = APIHelp.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hOldBitmap = IntPtr.Zero;

            try {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = APIHelp.SelectObject(memDc, hBitmap);

                APIHelp.Size newSize = new APIHelp.Size(bitmap.Width, bitmap.Height);
                APIHelp.Point sourceLocation = new APIHelp.Point(0, 0);
                APIHelp.Point newLocation = new APIHelp.Point(this.Left, this.Top);

                APIHelp.BLENDFUNCTION blend = new APIHelp.BLENDFUNCTION {
                    BlendOp = APIHelp.AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = 255,
                    AlphaFormat = APIHelp.AC_SRC_ALPHA
                };

                APIHelp.UpdateLayeredWindow(Handle, screenDc, ref newLocation, ref newSize, memDc, ref sourceLocation, 0, ref blend, APIHelp.ULW_ALPHA);
            } finally {
                APIHelp.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero) {
                    APIHelp.SelectObject(memDc, hOldBitmap);
                    APIHelp.DeleteObject(hBitmap);
                }
                APIHelp.DeleteDC(memDc);
            }
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= APIHelp.WS_EX_LAYERED;
                return createParams;
            }
        }
        /*
        protected override void WndProc(ref Message message) {
            if (message.Msg == APIHelp.WM_NCHITTEST) {
                message.Result = (IntPtr)APIHelp.HTCAPTION;
            } else {
                base.WndProc(ref message);
            }
        }
        */
        
        void IOverlay.Invoke(Delegate Method) {
            Invoke(Method);
        }

        void IOverlay.Focus() {
            Focus();
        }
        private void MouseClicked(object sender, MouseEventArgs e) {
            Point Pos = PointToScreen(e.Location);
            Exports.SendMouseClick(Exports.HookHandler, Pos.X, Pos.Y);
        }
    }
    class APIHelp {
        public const Int32 WS_EX_LAYERED = 0x80000;
        public const Int32 HTCAPTION = 0x02;
        public const Int32 WM_NCHITTEST = 0x84;
        public const Int32 ULW_ALPHA = 0x02;
        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;

        public enum Bool {
            False = 0,
            True = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Size {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteObject(IntPtr hObject);
    }
}
