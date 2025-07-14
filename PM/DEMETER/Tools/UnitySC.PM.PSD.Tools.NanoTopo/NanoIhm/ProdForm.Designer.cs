namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    partial class ProdForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProdForm));
            this.ProdLogPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxOutputPath = new System.Windows.Forms.TextBox();
            this.textBoxInputPath = new System.Windows.Forms.TextBox();
            this.labelOutputPath = new System.Windows.Forms.Label();
            this.labelInputPath = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.frmConnectionStatus1 = new NanoIhm.FrmConnectionStatus();
            this.SuspendLayout();
            // 
            // ProdLogPanel
            // 
            this.ProdLogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProdLogPanel.AutoSize = true;
            this.ProdLogPanel.BackColor = System.Drawing.SystemColors.Control;
            this.ProdLogPanel.Location = new System.Drawing.Point(12, 96);
            this.ProdLogPanel.Name = "ProdLogPanel";
            this.ProdLogPanel.Size = new System.Drawing.Size(780, 192);
            this.ProdLogPanel.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(670, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(645, 12);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(133, 24);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start Prod";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.Location = new System.Drawing.Point(645, 42);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(133, 24);
            this.buttonStop.TabIndex = 3;
            this.buttonStop.Text = "Stop Prod";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBoxOutputPath
            // 
            this.textBoxOutputPath.Location = new System.Drawing.Point(108, 40);
            this.textBoxOutputPath.Name = "textBoxOutputPath";
            this.textBoxOutputPath.Size = new System.Drawing.Size(382, 20);
            this.textBoxOutputPath.TabIndex = 11;
            // 
            // textBoxInputPath
            // 
            this.textBoxInputPath.Location = new System.Drawing.Point(108, 12);
            this.textBoxInputPath.Name = "textBoxInputPath";
            this.textBoxInputPath.Size = new System.Drawing.Size(382, 20);
            this.textBoxInputPath.TabIndex = 10;
            // 
            // labelOutputPath
            // 
            this.labelOutputPath.AutoSize = true;
            this.labelOutputPath.Location = new System.Drawing.Point(15, 42);
            this.labelOutputPath.Name = "labelOutputPath";
            this.labelOutputPath.Size = new System.Drawing.Size(64, 13);
            this.labelOutputPath.TabIndex = 9;
            this.labelOutputPath.Text = "Output Path";
            // 
            // labelInputPath
            // 
            this.labelInputPath.AutoSize = true;
            this.labelInputPath.Location = new System.Drawing.Point(15, 15);
            this.labelInputPath.Name = "labelInputPath";
            this.labelInputPath.Size = new System.Drawing.Size(56, 13);
            this.labelInputPath.TabIndex = 8;
            this.labelInputPath.Text = "Input Path";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(501, 14);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(135, 17);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Save precalibrate Data";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // frmConnectionStatus1
            // 
            this.frmConnectionStatus1.BackColor = System.Drawing.SystemColors.Control;
            this.frmConnectionStatus1.Location = new System.Drawing.Point(12, -1);
            this.frmConnectionStatus1.Name = "frmConnectionStatus1";
            this.frmConnectionStatus1.Size = new System.Drawing.Size(224, 79);
            this.frmConnectionStatus1.TabIndex = 2;
            this.frmConnectionStatus1.Visible = false;
            // 
            // ProdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 300);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textBoxOutputPath);
            this.Controls.Add(this.textBoxInputPath);
            this.Controls.Add(this.labelOutputPath);
            this.Controls.Add(this.labelInputPath);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.frmConnectionStatus1);
            this.Controls.Add(this.ProdLogPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 260);
            this.Name = "ProdForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProdForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProdForm_FormClosing);
            this.Load += new System.EventHandler(this.ProdForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel ProdLogPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;    
        private FrmConnectionStatus frmConnectionStatus1;
        private System.Windows.Forms.TextBox textBoxOutputPath;
        private System.Windows.Forms.TextBox textBoxInputPath;
        private System.Windows.Forms.Label labelOutputPath;
        private System.Windows.Forms.Label labelInputPath;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
