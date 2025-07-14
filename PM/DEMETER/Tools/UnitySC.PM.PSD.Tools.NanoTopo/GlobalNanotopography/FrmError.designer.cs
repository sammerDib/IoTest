namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    partial class FrmError
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
            this.CmdOK = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.LbTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LbErrorMsg2 = new System.Windows.Forms.Label();
            this.LbErrorMsg = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmdOK
            // 
            this.CmdOK.BackColor = System.Drawing.Color.White;
            this.CmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CmdOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CmdOK.Location = new System.Drawing.Point(243, 241);
            this.CmdOK.Name = "CmdOK";
            this.CmdOK.Size = new System.Drawing.Size(97, 53);
            this.CmdOK.TabIndex = 18;
            this.CmdOK.Text = "OK";
            this.CmdOK.UseVisualStyleBackColor = false;
            this.CmdOK.Click += new System.EventHandler(this.CmdOK_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.LbTitle);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(574, 50);
            this.panel3.TabIndex = 21;
            // 
            // LbTitle
            // 
            this.LbTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.LbTitle.Font = new System.Drawing.Font("Trebuchet MS", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbTitle.Location = new System.Drawing.Point(0, 0);
            this.LbTitle.Name = "LbTitle";
            this.LbTitle.Size = new System.Drawing.Size(572, 41);
            this.LbTitle.TabIndex = 0;
            this.LbTitle.Text = "Error";
            this.LbTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(574, 12);
            this.panel1.TabIndex = 24;
            // 
            // LbErrorMsg2
            // 
            this.LbErrorMsg2.Dock = System.Windows.Forms.DockStyle.Top;
            this.LbErrorMsg2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbErrorMsg2.Location = new System.Drawing.Point(0, 123);
            this.LbErrorMsg2.Name = "LbErrorMsg2";
            this.LbErrorMsg2.Size = new System.Drawing.Size(574, 61);
            this.LbErrorMsg2.TabIndex = 26;
            this.LbErrorMsg2.Text = "Error";
            this.LbErrorMsg2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LbErrorMsg
            // 
            this.LbErrorMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.LbErrorMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbErrorMsg.Location = new System.Drawing.Point(0, 62);
            this.LbErrorMsg.Name = "LbErrorMsg";
            this.LbErrorMsg.Size = new System.Drawing.Size(574, 61);
            this.LbErrorMsg.TabIndex = 25;
            this.LbErrorMsg.Text = "Error";
            this.LbErrorMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(574, 333);
            this.ControlBox = false;
            this.Controls.Add(this.LbErrorMsg2);
            this.Controls.Add(this.LbErrorMsg);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.CmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "FrmError";
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Darkfield Error";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmHandShakeError_FormClosing);
            this.Shown += new System.EventHandler(this.FrmError_Shown);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CmdOK;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label LbTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LbErrorMsg2;
        private System.Windows.Forms.Label LbErrorMsg;
    }
}
