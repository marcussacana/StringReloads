namespace Overlay {
    partial class Settings {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.ckTransMode = new System.Windows.Forms.CheckBox();
            this.ckAutoSizeFont = new System.Windows.Forms.CheckBox();
            this.FontSizeTB = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ListFileTB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ckTransMode
            // 
            this.ckTransMode.AutoSize = true;
            this.ckTransMode.Location = new System.Drawing.Point(12, 12);
            this.ckTransMode.Name = "ckTransMode";
            this.ckTransMode.Size = new System.Drawing.Size(108, 17);
            this.ckTransMode.TabIndex = 0;
            this.ckTransMode.Text = "Translation Mode";
            this.ckTransMode.UseVisualStyleBackColor = true;
            this.ckTransMode.CheckedChanged += new System.EventHandler(this.ckTransMode_CheckedChanged);
            // 
            // ckAutoSizeFont
            // 
            this.ckAutoSizeFont.AutoSize = true;
            this.ckAutoSizeFont.Location = new System.Drawing.Point(12, 69);
            this.ckAutoSizeFont.Name = "ckAutoSizeFont";
            this.ckAutoSizeFont.Size = new System.Drawing.Size(95, 17);
            this.ckAutoSizeFont.TabIndex = 1;
            this.ckAutoSizeFont.Text = "Auto Size Font";
            this.ckAutoSizeFont.UseVisualStyleBackColor = true;
            this.ckAutoSizeFont.CheckedChanged += new System.EventHandler(this.ckAutoSizeFont_CheckedChanged);
            // 
            // FontSizeTB
            // 
            this.FontSizeTB.Location = new System.Drawing.Point(76, 86);
            this.FontSizeTB.Mask = "00.0";
            this.FontSizeTB.Name = "FontSizeTB";
            this.FontSizeTB.Size = new System.Drawing.Size(40, 20);
            this.FontSizeTB.TabIndex = 2;
            this.FontSizeTB.Text = "080";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Fixed Size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "List File:";
            // 
            // ListFileTB
            // 
            this.ListFileTB.Location = new System.Drawing.Point(60, 29);
            this.ListFileTB.Name = "ListFileTB";
            this.ListFileTB.Size = new System.Drawing.Size(103, 20);
            this.ListFileTB.TabIndex = 5;
            this.ListFileTB.Text = "Strings-Overlay.lst";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(175, 130);
            this.Controls.Add(this.ListFileTB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FontSizeTB);
            this.Controls.Add(this.ckAutoSizeFont);
            this.Controls.Add(this.ckTransMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Settings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckTransMode;
        private System.Windows.Forms.CheckBox ckAutoSizeFont;
        private System.Windows.Forms.MaskedTextBox FontSizeTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ListFileTB;
    }
}