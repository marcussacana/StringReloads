using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#pragma warning disable 1591
namespace Overlay {
    public partial class Overlay : IOverlay
	{

        public Color TextBackColor { get {
                return DialogueBox.BackColor;
            }
            set {
                DialogueBox.BackColor = value;
            }
        }

        public Color TextForeColor {
            get {
                return DialogueBox.ForeColor;
            }
            set {
                DialogueBox.ForeColor = value;
            }
        }

        private string ListFN = null;
        private bool TranslationMode = false;
        public bool AutoFontSize = false;

        static bool Initializing = false;
		internal Overlay()
		{
			InitializeComponent();


            TopMost = true;
            TopLevel = true;

            TranslatePanel.Visible = TranslationMode;
            ButtonPanel.Visible = File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Strings.lst");
        }       

        private void LB_Legenda_TextChanged(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(DialogueBox.Text))
                return;

            DialogueBox.BorderStyle = BorderStyle.Fixed3D;
            Font New = Font;
            using (var g = Graphics.FromHwnd(IntPtr.Zero)) {
                bool First = true;
                if (AutoFontSize) {
                    SizeF ExpectedSize;
                    do {
                        if (!First)
                            New = new Font(New.Name, New.Size - 1, New.Style);
                        First = false;
                        ExpectedSize = g.MeasureString(Text, New);

                    } while (ExpectedSize.Width > DialogueBox.Width || ExpectedSize.Height > DialogueBox.Height);
                }
            }

            DialogueBox.Font = New;
            DialogueBox.BorderStyle = BorderStyle.None;
        }

        public new string Text {
            get { return DialogueBox.Text; }

            set {
                if (Application.OpenForms.OfType<Overlay>().Count() == 0)
                    Show();

                    Invoke(new MethodInvoker(() => {
                    DialogueBox.Text = value;
                    DialogueBox.Invalidate();

                    if (TranslationMode) {
                        string Tmp = value;
                        Encode(ref Tmp, true);
                        TranslateBox.Text = Tmp;
                    }
                }));
            }
        }

		private static Overlay _DefaultInstance;

        public static Overlay DefaultInstance {
            get {
                while (Initializing) {
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                if (_DefaultInstance == null || _DefaultInstance.IsDisposed || !_DefaultInstance.CanInvoke()) {
                    if (Exports.TextOnly)
                        return null;
                    Initializing = true;
                    _DefaultInstance = new Overlay();
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
        private void bntSettings_Click(object sender, EventArgs e) {
            Settings Window = new Settings();
            Window.ShowDialog();

            TranslationMode = Window.TranslationMode;
            ListFN = Window.ListFile;
            AutoFontSize = Window.AutoResizeFont;
            Font = new Font(Font.FontFamily, Window.FixedFontSize, Font.Style);

            TranslatePanel.Visible = TranslationMode;
        }

        private void KeyDownTB(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                e.Handled = true;
                e.SuppressKeyPress = true;
                AddTranslation(TranslateBox.Text);
            }
        }

        private void AddTranslation(string Text) {
            const string BreakLineFlag = "::BREAKLINE::";
            const string ReturnLineFlag = "::RETURNLINE::";
            Encode(ref Text, false);            

            //Initial Algoritm, Needs implement overwrite feature if the string is already present.
            using (StreamWriter List = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + ListFN)) {
                List.WriteLine(this.Text.Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
                List.WriteLine(Text.Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
            }
        }        

        //Enable = true: Encode Text, Enable = false: Decode Text
        private static void Encode(ref string String, bool Enable) {
            if (Enable) {
                string Result = string.Empty;
                foreach (char c in String) {
                    if (c == '\n')
                        Result += "\\n";
                    else if (c == '\\')
                        Result += "\\\\";
                    else if (c == '\t')
                        Result += "\\t";
                    else if (c == '\r')
                        Result += "\\r";
                    else
                        Result += c;
                }
                String = Result;
            } else {
                string Result = string.Empty;
                bool Special = false;
                foreach (char c in String) {
                    if (c == '\\' & !Special) {
                        Special = true;
                        continue;
                    }
                    if (Special) {
                        switch (c.ToString().ToLower()[0]) {
                            case '\\':
                                Result += '\\';
                                break;
                            case 'n':
                                Result += '\n';
                                break;
                            case 't':
                                Result += '\t';
                                break;
                            case 'r':
                                Result += '\r';
                                break;
                            default:
                                throw new Exception("\\" + c + " Isn't a valid string escape.");
                        }
                        Special = false;
                    } else
                        Result += c;
                }
                String = Result;
            }
        }

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

}

#pragma warning restore 1591