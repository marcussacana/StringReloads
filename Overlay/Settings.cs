using System;
using System.Windows.Forms;

namespace Overlay {
    public partial class Settings : Form {
        public bool TranslationMode = false;
        public bool AutoResizeFont = false;
        public float FixedFontSize = 12.0f;
        public string ListFile = null;
        public Settings() {
            InitializeComponent();
        }

        private void ckAutoSizeFont_CheckedChanged(object sender, EventArgs e) {
            FontSizeTB.Enabled = !ckAutoSizeFont.Checked;
        }

        private void ckTransMode_CheckedChanged(object sender, EventArgs e) {
            ListFileTB.Enabled = ckTransMode.Checked;
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e) {
            TranslationMode = ckTransMode.Checked;
            AutoResizeFont = ckAutoSizeFont.Checked;
            FixedFontSize = float.Parse(FontSizeTB.Text);
            ListFile = ListFileTB.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
