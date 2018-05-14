using SRL;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SRLTracer {
    public partial class Main : Form {
        public Main() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (checkBox1.Checked)
                System.Diagnostics.Debugger.Break();
            if (rManaged.Checked) {
                Managed();
            } else
                TBInput.Text = Process(TBInput.Text);
        }

        private void Managed() {
            TBInput.Text = StringReloader.ProcessManaged(TBInput.Text);
        }

        [DllImport("SRL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern string Process(string Input);
    }
}
