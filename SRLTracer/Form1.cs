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
            MessageBox.Show("Sorry but the AppVeyor build don't works, Please, build this project by yourself");
#else
#if DEBUG
            TBInput.Text = SRL.StringReloader.ProcessManaged(TBInput.Text);
#else
            TBInput.Text = SRLUnity.Wrapper.Process(TBInput.Text);
#endif
#endif
        }


        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

#if AppVeyor
            MessageBox.Show("Sorry but the AppVeyor build don't works, Please, build this project by yourself");
#else
            Wrapper Wrapper = new Wrapper();
            for (int i = 0; i < ofd.FileNames.Length; i++) {
                string Script = ofd.FileNames[i];
                Text = $"SRLTracer - Exporting {System.IO.Path.GetFileName(Script)} ({i} of {ofd.FileNames.Length} Files)";
                Application.DoEvents();
                try {
                    bool Export = false;
                    var Strings = Wrapper.Import(Script, TryLastPluginFirst: true);
                    for (uint x = 0; x < Strings.LongLength; x++) {
                        string Ori = Strings[x];
#if DEBUG
                        Strings[x] = SRL.StringReloader.ProcessManaged(Strings[x]);
#else
                        Strings[x] = SRLUnity.Wrapper.Process(Strings[x]);
#endif
                        if (Strings[x] != Ori)
                            Export = true;
                    }
                    if (Export)
                        Wrapper.Export(Strings, Script);
                } catch { }
            }
#endif
            Text = "SRL Engine Tracer Tool";
            MessageBox.Show("Task Finished.");
        }
    }
}
