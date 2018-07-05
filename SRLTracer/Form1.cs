using SacanaWrapper;
using System;
using System.Windows.Forms;

namespace SRLTracer {
    public partial class Main : Form {
        public Main() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
                TBInput.Text = SRLUnity.Wrapper.Process(TBInput.Text);
        }


        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

#if !AppVeyor
            Wrapper Wrapper = new Wrapper();
            foreach (string Script in ofd.FileNames) {
                var Strings = Wrapper.Import(Script, TryLastPluginFirst: true);
                for (uint i = 0; i < Strings.LongLength; i++)
                    Strings[i] = SRLUnity.Wrapper.Process(Strings[i]);
                Wrapper.Export(Strings, Script);
            }
#endif
            MessageBox.Show("Task Finished.");
        }
    }
}
