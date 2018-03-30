using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

#pragma warning disable 1591
namespace Overlay {
    public partial class Overlay
	{
        private string ListFN = null;
        private bool TranslationMode = false;
        private bool AutoFontSize = false;
        private float FixedFontSize = 8f;

        static bool Initializing = false;
		internal Overlay()
		{
			InitializeComponent();

            TranslatePanel.Visible = TranslationMode;
        }       

        private void LB_Legenda_TextChanged(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(DialogueBox.Text))
                return;

            DialogueBox.Font = new Font(DialogueBox.Font.Name, FixedFontSize, DialogueBox.Font.Style);
            DialogueBox.BorderStyle = BorderStyle.Fixed3D;
            while (AutoFontSize) {
                Font New = new Font(DialogueBox.Font.Name, DialogueBox.Font.Size + 1, DialogueBox.Font.Style);
                using (var g = Graphics.FromHwnd(IntPtr.Zero)) {
                    SizeF Rst = g.MeasureString(DialogueBox.Text, New);
                    if (Rst.Width >= DialogueBox.Size.Width - 5 || Rst.Height >= DialogueBox.Size.Height - 5)
                        break;
                }

                DialogueBox.Font = New;
            }

            DialogueBox.BorderStyle = BorderStyle.None;
        }

        public string DialogueText {
            get { return DialogueBox.Text; }

            set {
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
        public void ShowText(string Text) {
            DialogueText = Text;
        }

		private static Overlay _DefaultInstance;

        public static Overlay DefaultInstance {
            get {
                while (Initializing) {
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                if (_DefaultInstance == null) {
                    Initializing = true;
                    _DefaultInstance = new Overlay();
                    Initializing = false;
                }

                return _DefaultInstance;
            }
        }

        private void bntSettings_Click(object sender, EventArgs e) {
            Settings Window = new Settings();
            Window.ShowDialog();

            TranslationMode = Window.TranslationMode;
            ListFN = Window.ListFile;
            AutoFontSize = Window.AutoResizeFont;
            FixedFontSize = Window.FixedFontSize;

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
                List.WriteLine(DialogueText.Replace("\n", BreakLineFlag).Replace("\r", ReturnLineFlag));
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
    }

}

#pragma warning restore 1591