using SacanaWrapper;
using System;
using System.Windows.Forms;

namespace SRLTracer {
    public partial class Main : Form {
        public Main() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
#if AppVeyor
            TBInput.Text = SRLUnity.Wrapper.Process(TBInput.Text);
#endif
        }


        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

#if AppVeyor
            Wrapper Wrapper = new Wrapper();
            for (int i = 0; i < ofd.FileNames.Length; i++) {
                string Script = ofd.FileNames[i];
                Text = $"SRLTracer - Exporting {System.IO.Path.GetFileName(Script)} ({i} of {ofd.FileNames.Length} Files)";
                Application.DoEvents();

                var Strings = Wrapper.Import(Script, TryLastPluginFirst: true);
                for (uint x = 0; x < Strings.LongLength; x++)
                    Strings[x] = SRLUnity.Wrapper.Process(Strings[x]);
                Wrapper.Export(Strings, Script);
            }
#endif
            Text = "SRL Engine Tracer Tool";
            MessageBox.Show("Task Finished.");
        }
    }
}
