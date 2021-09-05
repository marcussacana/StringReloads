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
            this.TranslatePanel = new System.Windows.Forms.Panel();
            this.TranslateBox = new System.Windows.Forms.TextBox();
            this.LabelBackground.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.TranslatePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelBackground
            // 
            this.LabelBackground.BackColor = System.Drawing.Color.Fuchsia;
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
            this.DialogueBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClicked);
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ButtonPanel.Controls.Add(this.bntSettings);
            this.ButtonPanel.Location = new System.Drawing.Point(619, 8);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(72, 36);
            this.ButtonPanel.TabIndex = 1;
            // 
            // bntSettings
            // 
            this.bntSettings.Location = new System.Drawing.Point(4, 6);
            this.bntSettings.Name = "bntSettings";
            this.bntSettings.Size = new System.Drawing.Size(64, 23);
            this.bntSettings.TabIndex = 0;
            this.bntSettings.Text = "Settings";
            this.bntSettings.UseVisualStyleBackColor = true;
            this.bntSettings.Click += new System.EventHandler(this.bntSettings_Click);
            // 
            // TranslatePanel
            // 
            this.TranslatePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TranslatePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.TranslatePanel.Controls.Add(this.TranslateBox);
            this.TranslatePanel.Location = new System.Drawing.Point(15, 8);
            this.TranslatePanel.Name = "TranslatePanel";
            this.TranslatePanel.Size = new System.Drawing.Size(598, 36);
            this.TranslatePanel.TabIndex = 2;
            // 
            // TranslateBox
            // 
            this.TranslateBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TranslateBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TranslateBox.Location = new System.Drawing.Point(4, 5);
            this.TranslateBox.Name = "TranslateBox";
            this.TranslateBox.Size = new System.Drawing.Size(591, 26);
            this.TranslateBox.TabIndex = 0;
            this.TranslateBox.Text = "Waiting Text";
            this.TranslateBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownTB);
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.ClientSize = new System.Drawing.Size(706, 193);
            this.Controls.Add(this.TranslatePanel);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.LabelBackground);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Overlay";
            this.Opacity = 0.8D;
            this.Padding = new System.Windows.Forms.Padding(15);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.LabelBackground.ResumeLayout(false);
            this.ButtonPanel.ResumeLayout(false);
            this.TranslatePanel.ResumeLayout(false);
            this.TranslatePanel.PerformLayout();
            this.ResumeLayout(false);

		}

		internal Panel LabelBackground;
		internal Label DialogueBox;
		internal Panel ButtonPanel;
        private Button bntSettings;
        internal Panel TranslatePanel;
        private TextBox TranslateBox;
    }

}

#pragma warning restore 1591