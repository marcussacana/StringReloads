//INSTANT C# NOTE: Formerly VB project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

#pragma warning disable 1591
namespace Overlay
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	public partial class Overlay : System.Windows.Forms.Form
	{
		//Descartar substituições de formulário para limpar a lista de componentes.
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

#pragma warning disable 0649
        //Exigido pelo Windows Form Designer
        private System.ComponentModel.IContainer components;
#pragma warning restore 0649

        //OBSERVAÇÃO: o procedimento a seguir é exigido pelo Windows Form Designer
        //Pode ser modificado usando o Windows Form Designer.  
        //Não o modifique usando o editor de códigos.
        [System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            this.LabelBackground = new System.Windows.Forms.Panel();
            this.DialogueBox = new System.Windows.Forms.Label();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.bntSettings = new System.Windows.Forms.Button();
            this.LabelBackground.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelBackground
            // 
            this.LabelBackground.Controls.Add(this.DialogueBox);
            this.LabelBackground.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LabelBackground.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LabelBackground.Location = new System.Drawing.Point(15, 54);
            this.LabelBackground.Name = "LabelBackground";
            this.LabelBackground.Size = new System.Drawing.Size(676, 124);
            this.LabelBackground.TabIndex = 0;
            // 
            // DialogueBox
            // 
            this.DialogueBox.AutoEllipsis = true;
            this.DialogueBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.DialogueBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DialogueBox.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DialogueBox.ForeColor = System.Drawing.Color.White;
            this.DialogueBox.Location = new System.Drawing.Point(0, 0);
            this.DialogueBox.Name = "DialogueBox";
            this.DialogueBox.Size = new System.Drawing.Size(676, 124);
            this.DialogueBox.TabIndex = 0;
            this.DialogueBox.Text = "Waiting Text";
            this.DialogueBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DialogueBox.TextChanged += new System.EventHandler(this.LB_Legenda_TextChanged);
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ButtonPanel.Controls.Add(this.bntSettings);
            this.ButtonPanel.Location = new System.Drawing.Point(619, 18);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(72, 30);
            this.ButtonPanel.TabIndex = 1;
            // 
            // bntSettings
            // 
            this.bntSettings.Location = new System.Drawing.Point(4, 3);
            this.bntSettings.Name = "bntSettings";
            this.bntSettings.Size = new System.Drawing.Size(64, 23);
            this.bntSettings.TabIndex = 0;
            this.bntSettings.Text = "Settings";
            this.bntSettings.UseVisualStyleBackColor = true;
            this.bntSettings.Click += new System.EventHandler(this.bntSettings_Click);
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.ClientSize = new System.Drawing.Size(706, 193);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.LabelBackground);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Overlay";
            this.Opacity = 0.8D;
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Text = "Overlay";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.LabelBackground.ResumeLayout(false);
            this.ButtonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		internal Panel LabelBackground;
		internal Label DialogueBox;
		internal Panel ButtonPanel;
        private Button bntSettings;
    }

}

#pragma warning restore 1591