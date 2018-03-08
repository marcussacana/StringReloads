namespace SRLTracer {
    partial class Main {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent() {
            this.TBInput = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.rUnmanaged = new System.Windows.Forms.RadioButton();
            this.rManaged = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // TBInput
            // 
            this.TBInput.Location = new System.Drawing.Point(12, 12);
            this.TBInput.Multiline = true;
            this.TBInput.Name = "TBInput";
            this.TBInput.Size = new System.Drawing.Size(723, 208);
            this.TBInput.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(660, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Reload";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rUnmanaged
            // 
            this.rUnmanaged.AutoSize = true;
            this.rUnmanaged.Location = new System.Drawing.Point(493, 229);
            this.rUnmanaged.Name = "rUnmanaged";
            this.rUnmanaged.Size = new System.Drawing.Size(83, 17);
            this.rUnmanaged.TabIndex = 2;
            this.rUnmanaged.Text = "Unmanaged";
            this.rUnmanaged.UseVisualStyleBackColor = true;
            // 
            // rManaged
            // 
            this.rManaged.AutoSize = true;
            this.rManaged.Checked = true;
            this.rManaged.Location = new System.Drawing.Point(584, 229);
            this.rManaged.Name = "rManaged";
            this.rManaged.Size = new System.Drawing.Size(70, 17);
            this.rManaged.TabIndex = 3;
            this.rManaged.TabStop = true;
            this.rManaged.Text = "Managed";
            this.rManaged.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(433, 232);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(54, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Break";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 261);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.rManaged);
            this.Controls.Add(this.rUnmanaged);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TBInput);
            this.Name = "Main";
            this.Text = "SRL Engine Tracer Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TBInput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton rUnmanaged;
        private System.Windows.Forms.RadioButton rManaged;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

