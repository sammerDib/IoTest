namespace UnitySC.Rorze.Emulator.Controls
{
    partial class SendControl
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
            this.postfixStaticLabel = new System.Windows.Forms.Label();
            this.commandStaticLabel = new System.Windows.Forms.Label();
            this.pefixStaticLabel = new System.Windows.Forms.Label();
            this.postfixTextBox = new System.Windows.Forms.TextBox();
            this.prefixTextBox = new System.Windows.Forms.TextBox();
            this.rawCommandChkBox = new System.Windows.Forms.CheckBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.outMessageTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // postfixStaticLabel
            // 
            this.postfixStaticLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.postfixStaticLabel.AutoSize = true;
            this.postfixStaticLabel.Location = new System.Drawing.Point(229, 5);
            this.postfixStaticLabel.Name = "postfixStaticLabel";
            this.postfixStaticLabel.Size = new System.Drawing.Size(38, 13);
            this.postfixStaticLabel.TabIndex = 31;
            this.postfixStaticLabel.Text = "Postfix";
            // 
            // commandStaticLabel
            // 
            this.commandStaticLabel.AutoSize = true;
            this.commandStaticLabel.Location = new System.Drawing.Point(38, 5);
            this.commandStaticLabel.Name = "commandStaticLabel";
            this.commandStaticLabel.Size = new System.Drawing.Size(54, 13);
            this.commandStaticLabel.TabIndex = 30;
            this.commandStaticLabel.Text = "Command";
            // 
            // pefixStaticLabel
            // 
            this.pefixStaticLabel.AutoSize = true;
            this.pefixStaticLabel.Location = new System.Drawing.Point(5, 5);
            this.pefixStaticLabel.Name = "pefixStaticLabel";
            this.pefixStaticLabel.Size = new System.Drawing.Size(33, 13);
            this.pefixStaticLabel.TabIndex = 29;
            this.pefixStaticLabel.Text = "Prefix";
            // 
            // postfixTextBox
            // 
            this.postfixTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.postfixTextBox.Enabled = false;
            this.postfixTextBox.Location = new System.Drawing.Point(220, 0);
            this.postfixTextBox.Name = "postfixTextBox";
            this.postfixTextBox.Size = new System.Drawing.Size(41, 20);
            this.postfixTextBox.TabIndex = 28;
            // 
            // prefixTextBox
            // 
            this.prefixTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.prefixTextBox.Enabled = false;
            this.prefixTextBox.Location = new System.Drawing.Point(0, 0);
            this.prefixTextBox.Name = "prefixTextBox";
            this.prefixTextBox.Size = new System.Drawing.Size(30, 20);
            this.prefixTextBox.TabIndex = 27;
            // 
            // rawCommandChkBox
            // 
            this.rawCommandChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rawCommandChkBox.AutoSize = true;
            this.rawCommandChkBox.Location = new System.Drawing.Point(271, 3);
            this.rawCommandChkBox.Name = "rawCommandChkBox";
            this.rawCommandChkBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.rawCommandChkBox.Size = new System.Drawing.Size(97, 17);
            this.rawCommandChkBox.TabIndex = 26;
            this.rawCommandChkBox.Text = "Raw command";
            this.rawCommandChkBox.UseVisualStyleBackColor = true;
            this.rawCommandChkBox.Click += new System.EventHandler(this.RawCommandChkBox_Click);
            // 
            // sendBtn
            // 
            this.sendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendBtn.Location = new System.Drawing.Point(273, 25);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(96, 23);
            this.sendBtn.TabIndex = 25;
            this.sendBtn.Text = "Send";
            this.sendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // outMessageTextBox
            // 
            this.outMessageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outMessageTextBox.Location = new System.Drawing.Point(30, 0);
            this.outMessageTextBox.Name = "outMessageTextBox";
            this.outMessageTextBox.Size = new System.Drawing.Size(190, 20);
            this.outMessageTextBox.TabIndex = 24;
            this.outMessageTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SendControl_KeyPress);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.outMessageTextBox);
            this.panel1.Controls.Add(this.postfixTextBox);
            this.panel1.Controls.Add(this.prefixTextBox);
            this.panel1.Location = new System.Drawing.Point(8, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(261, 20);
            this.panel1.TabIndex = 33;
            // 
            // SendControl
            // 
            this.AutoSize = true;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.postfixStaticLabel);
            this.Controls.Add(this.commandStaticLabel);
            this.Controls.Add(this.pefixStaticLabel);
            this.Controls.Add(this.rawCommandChkBox);
            this.Controls.Add(this.sendBtn);
            this.Name = "SendControl";
            this.Size = new System.Drawing.Size(375, 52);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SendControl_KeyPress);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label postfixStaticLabel;
        private System.Windows.Forms.Label commandStaticLabel;
        private System.Windows.Forms.Label pefixStaticLabel;
        private System.Windows.Forms.CheckBox rawCommandChkBox;
        private System.Windows.Forms.Button sendBtn;
        protected System.Windows.Forms.TextBox postfixTextBox;
        protected System.Windows.Forms.TextBox prefixTextBox;
        protected System.Windows.Forms.TextBox outMessageTextBox;
        private System.Windows.Forms.Panel panel1;
    }
}
