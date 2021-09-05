using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Overlay {
    internal interface IOverlay {

        bool AutoSize { get; set; }
        string Text { get; set; }
        Font Font { get; set; }

        Size Size { get; set; }
        Point Location { get; set; }

        Color TextBackColor { get; set; }
        Color TextForeColor { get; set; }

        IntPtr Handle { get; }

        ContentAlignment TextAlignment { get; set; }

        FormWindowState WindowState { get; set; }
        double Opacity { get; set; }

        bool CanInvoke();

        void Invoke(Delegate Method);
        void Close();
        void Show();
        void Dispose();
        void Hide();
        void Focus();
    }
}
