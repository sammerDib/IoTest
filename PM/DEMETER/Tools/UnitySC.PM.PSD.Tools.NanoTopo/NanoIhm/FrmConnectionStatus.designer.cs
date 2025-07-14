namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    partial class FrmConnectionStatus
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConnectionStatus));
            this.LbTitle = new System.Windows.Forms.Label();
            this.LbModule2Name = new System.Windows.Forms.Label();
            this.LbModule1Name = new System.Windows.Forms.Label();
            this.PB_M1_OFF = new System.Windows.Forms.PictureBox();
            this.PB_M1_ON = new System.Windows.Forms.PictureBox();
            this.PB_M1_ABS = new System.Windows.Forms.PictureBox();
            this.CmdStart = new System.Windows.Forms.Button();
            this.CmdStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PB_M1_OFF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_M1_ON)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_M1_ABS)).BeginInit();
            this.SuspendLayout();
            // 
            // LbTitle
            // 
            this.LbTitle.BackColor = System.Drawing.SystemColors.Control;
            this.LbTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.LbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbTitle.Location = new System.Drawing.Point(0, 0);
            this.LbTitle.Name = "LbTitle";
            this.LbTitle.Size = new System.Drawing.Size(228, 16);
            this.LbTitle.TabIndex = 127;
            this.LbTitle.Text = "Connection serveur control";
            this.LbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LbModule2Name
            // 
            this.LbModule2Name.Location = new System.Drawing.Point(11, 54);
            this.LbModule2Name.Name = "LbModule2Name";
            this.LbModule2Name.Size = new System.Drawing.Size(72, 13);
            this.LbModule2Name.TabIndex = 122;
            this.LbModule2Name.Text = "Port#13900";
            this.LbModule2Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LbModule1Name
            // 
            this.LbModule1Name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LbModule1Name.AutoSize = true;
            this.LbModule1Name.Location = new System.Drawing.Point(125, 54);
            this.LbModule1Name.Name = "LbModule1Name";
            this.LbModule1Name.Size = new System.Drawing.Size(61, 13);
            this.LbModule1Name.TabIndex = 121;
            this.LbModule1Name.Text = "ClientName";
            // 
            // PB_M1_OFF
            // 
            this.PB_M1_OFF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_M1_OFF.BackColor = System.Drawing.SystemColors.Control;
            this.PB_M1_OFF.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PB_M1_OFF.BackgroundImage")));
            this.PB_M1_OFF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.PB_M1_OFF.Location = new System.Drawing.Point(98, 50);
            this.PB_M1_OFF.Name = "PB_M1_OFF";
            this.PB_M1_OFF.Size = new System.Drawing.Size(21, 21);
            this.PB_M1_OFF.TabIndex = 123;
            this.PB_M1_OFF.TabStop = false;
            // 
            // PB_M1_ON
            // 
            this.PB_M1_ON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_M1_ON.BackColor = System.Drawing.SystemColors.Control;
            this.PB_M1_ON.BackgroundImage = Properties.Resources.Voyant_Vert_ON;
            this.PB_M1_ON.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.PB_M1_ON.Location = new System.Drawing.Point(98, 50);
            this.PB_M1_ON.Name = "PB_M1_ON";
            this.PB_M1_ON.Size = new System.Drawing.Size(21, 21);
            this.PB_M1_ON.TabIndex = 125;
            this.PB_M1_ON.TabStop = false;
            this.PB_M1_ON.Visible = false;
            // 
            // PB_M1_ABS
            // 
            this.PB_M1_ABS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_M1_ABS.BackColor = System.Drawing.SystemColors.Control;
            this.PB_M1_ABS.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PB_M1_ABS.BackgroundImage")));
            this.PB_M1_ABS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.PB_M1_ABS.Location = new System.Drawing.Point(98, 50);
            this.PB_M1_ABS.Name = "PB_M1_ABS";
            this.PB_M1_ABS.Size = new System.Drawing.Size(21, 21);
            this.PB_M1_ABS.TabIndex = 128;
            this.PB_M1_ABS.TabStop = false;
            // 
            // CmdStart
            // 
            this.CmdStart.Location = new System.Drawing.Point(14, 23);
            this.CmdStart.Name = "CmdStart";
            this.CmdStart.Size = new System.Drawing.Size(60, 21);
            this.CmdStart.TabIndex = 129;
            this.CmdStart.Text = "Start";
            this.CmdStart.UseVisualStyleBackColor = true;
            this.CmdStart.Click += new System.EventHandler(this.CmdStart_Click);
            // 
            // CmdStop
            // 
            this.CmdStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CmdStop.Location = new System.Drawing.Point(147, 23);
            this.CmdStop.Name = "CmdStop";
            this.CmdStop.Size = new System.Drawing.Size(60, 21);
            this.CmdStop.TabIndex = 130;
            this.CmdStop.Text = "Stop";
            this.CmdStop.UseVisualStyleBackColor = true;
            this.CmdStop.Click += new System.EventHandler(this.CmdStop_Click);
            // 
            // FrmConnectionStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.CmdStop);
            this.Controls.Add(this.CmdStart);
            this.Controls.Add(this.LbTitle);
            this.Controls.Add(this.LbModule2Name);
            this.Controls.Add(this.LbModule1Name);
            this.Controls.Add(this.PB_M1_ABS);
            this.Controls.Add(this.PB_M1_OFF);
            this.Controls.Add(this.PB_M1_ON);
            this.Name = "FrmConnectionStatus";
            this.Size = new System.Drawing.Size(228, 79);
            ((System.ComponentModel.ISupportInitialize)(this.PB_M1_OFF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_M1_ON)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_M1_ABS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LbTitle;
        private System.Windows.Forms.Label LbModule2Name;
        private System.Windows.Forms.Label LbModule1Name;
        private System.Windows.Forms.PictureBox PB_M1_ON;
        private System.Windows.Forms.PictureBox PB_M1_ABS;
        private System.Windows.Forms.PictureBox PB_M1_OFF;
        private System.Windows.Forms.Button CmdStart;
        private System.Windows.Forms.Button CmdStop;
    }
}
