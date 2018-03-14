using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

#pragma warning disable 1591
namespace Overlay {
    public partial class Overlay
	{
        static bool Initializing = false;
		internal Overlay()
		{
			InitializeComponent();
		}       

        private void LB_Legenda_TextChanged(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(DialogueBox.Text))
                return;

            DialogueBox.Font = new Font(DialogueBox.Font.Name, 8F, DialogueBox.Font.Style);
            DialogueBox.BorderStyle = BorderStyle.Fixed3D;
            while (true) {
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
            get => DialogueBox.Text;
            set => Invoke(new MethodInvoker(() => { DialogueBox.Text = value; DialogueBox.Invalidate(); } ));
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
            MessageBox.Show("Not Implemented Yet\nMaybe in the next update.", "String Reloader Overlay", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}

#pragma warning restore 1591