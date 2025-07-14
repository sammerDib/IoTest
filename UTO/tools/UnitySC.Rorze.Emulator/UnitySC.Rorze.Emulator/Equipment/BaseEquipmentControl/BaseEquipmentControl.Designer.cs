using UnitySC.Rorze.Emulator.Controls;

namespace UnitySC.Rorze.Emulator.Equipment.BaseEquipmentControl
{
    partial class BaseEquipmentControl
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
            this.commandPanel = new System.Windows.Forms.Panel();
            this.autorespButton = new System.Windows.Forms.CheckBox();
            this.tcpPanel = new System.Windows.Forms.Panel();
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tcpStatusBar = new System.Windows.Forms.StatusBar();
            this.tcpStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.serverRadioButton = new System.Windows.Forms.RadioButton();
            this.clientRadioButton = new System.Windows.Forms.RadioButton();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.portStaticLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.ipAddressStaticLabel = new System.Windows.Forms.Label();
            this.autoResponsePanel = new System.Windows.Forms.Panel();
            this.receiveGrpBox = new System.Windows.Forms.GroupBox();
            this.dataLogControl = new UnitySC.Rorze.Emulator.Controls.DataLogControl();
            this.sendControl = new UnitySC.Rorze.Emulator.Controls.SendControl();
            this.commandPanel.SuspendLayout();
            this.tcpPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpStatusBarPanel)).BeginInit();
            this.panel1.SuspendLayout();
            this.autoResponsePanel.SuspendLayout();
            this.receiveGrpBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandPanel
            // 
            this.commandPanel.Controls.Add(this.sendControl);
            this.commandPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.commandPanel.Location = new System.Drawing.Point(0, 399);
            this.commandPanel.Name = "commandPanel";
            this.commandPanel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.commandPanel.Size = new System.Drawing.Size(740, 57);
            this.commandPanel.TabIndex = 63;
            // 
            // autorespButton
            // 
            this.autorespButton.AutoSize = true;
            this.autorespButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.autorespButton.Location = new System.Drawing.Point(321, 3);
            this.autorespButton.Name = "autorespButton";
            this.autorespButton.Size = new System.Drawing.Size(108, 17);
            this.autorespButton.TabIndex = 65;
            this.autorespButton.Text = "AutoResponse";
            this.autorespButton.UseVisualStyleBackColor = true;
            this.autorespButton.CheckedChanged += new System.EventHandler(this.AutoResponseButton_CheckedChanged);
            // 
            // tcpPanel
            // 
            this.tcpPanel.Controls.Add(this.connectButton);
            this.tcpPanel.Controls.Add(this.disconnectButton);
            this.tcpPanel.Controls.Add(this.label2);
            this.tcpPanel.Controls.Add(this.tcpStatusBar);
            this.tcpPanel.Controls.Add(this.panel1);
            this.tcpPanel.Location = new System.Drawing.Point(0, 0);
            this.tcpPanel.Name = "tcpPanel";
            this.tcpPanel.Size = new System.Drawing.Size(429, 106);
            this.tcpPanel.TabIndex = 66;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(3, 46);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(96, 24);
            this.connectButton.TabIndex = 28;
            this.connectButton.Text = "Connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new System.Drawing.Point(3, 74);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(96, 24);
            this.disconnectButton.TabIndex = 29;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(0, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(202, 16);
            this.label2.TabIndex = 26;
            this.label2.Text = "TCP Connection Status";
            // 
            // tcpStatusBar
            // 
            this.tcpStatusBar.Dock = System.Windows.Forms.DockStyle.None;
            this.tcpStatusBar.Location = new System.Drawing.Point(0, 16);
            this.tcpStatusBar.Name = "tcpStatusBar";
            this.tcpStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.tcpStatusBarPanel});
            this.tcpStatusBar.ShowPanels = true;
            this.tcpStatusBar.Size = new System.Drawing.Size(424, 24);
            this.tcpStatusBar.TabIndex = 27;
            // 
            // tcpStatusBarPanel
            // 
            this.tcpStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.tcpStatusBarPanel.Name = "tcpStatusBarPanel";
            this.tcpStatusBarPanel.Width = 407;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.serverRadioButton);
            this.panel1.Controls.Add(this.clientRadioButton);
            this.panel1.Controls.Add(this.ipTextBox);
            this.panel1.Controls.Add(this.portStaticLabel);
            this.panel1.Controls.Add(this.portTextBox);
            this.panel1.Controls.Add(this.ipAddressStaticLabel);
            this.panel1.Location = new System.Drawing.Point(102, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(322, 59);
            this.panel1.TabIndex = 25;
            // 
            // serverRadioButton
            // 
            this.serverRadioButton.AutoSize = true;
            this.serverRadioButton.Checked = true;
            this.serverRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.serverRadioButton.Location = new System.Drawing.Point(248, 11);
            this.serverRadioButton.Name = "serverRadioButton";
            this.serverRadioButton.Size = new System.Drawing.Size(56, 17);
            this.serverRadioButton.TabIndex = 0;
            this.serverRadioButton.TabStop = true;
            this.serverRadioButton.Text = "Server";
            // 
            // clientRadioButton
            // 
            this.clientRadioButton.AutoSize = true;
            this.clientRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clientRadioButton.Location = new System.Drawing.Point(248, 34);
            this.clientRadioButton.Name = "clientRadioButton";
            this.clientRadioButton.Size = new System.Drawing.Size(51, 17);
            this.clientRadioButton.TabIndex = 1;
            this.clientRadioButton.Text = "Client";
            // 
            // ipTextBox
            // 
            this.ipTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ipTextBox.Location = new System.Drawing.Point(8, 27);
            this.ipTextBox.MaxLength = 15;
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(112, 22);
            this.ipTextBox.TabIndex = 30;
            this.ipTextBox.Text = "127.0.0.1";
            this.ipTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // portStaticLabel
            // 
            this.portStaticLabel.AutoSize = true;
            this.portStaticLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.portStaticLabel.Location = new System.Drawing.Point(146, 11);
            this.portStaticLabel.Name = "portStaticLabel";
            this.portStaticLabel.Size = new System.Drawing.Size(26, 13);
            this.portStaticLabel.TabIndex = 29;
            this.portStaticLabel.Text = "Port";
            this.portStaticLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // portTextBox
            // 
            this.portTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.portTextBox.Location = new System.Drawing.Point(149, 27);
            this.portTextBox.MaxLength = 5;
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(67, 22);
            this.portTextBox.TabIndex = 28;
            this.portTextBox.Text = "23";
            this.portTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ipAddressStaticLabel
            // 
            this.ipAddressStaticLabel.AutoSize = true;
            this.ipAddressStaticLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ipAddressStaticLabel.Location = new System.Drawing.Point(5, 11);
            this.ipAddressStaticLabel.Name = "ipAddressStaticLabel";
            this.ipAddressStaticLabel.Size = new System.Drawing.Size(58, 13);
            this.ipAddressStaticLabel.TabIndex = 27;
            this.ipAddressStaticLabel.Text = "IP Address";
            this.ipAddressStaticLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // autoResponsePanel
            // 
            this.autoResponsePanel.Controls.Add(this.autorespButton);
            this.autoResponsePanel.Location = new System.Drawing.Point(0, 379);
            this.autoResponsePanel.Name = "autoResponsePanel";
            this.autoResponsePanel.Size = new System.Drawing.Size(429, 20);
            this.autoResponsePanel.TabIndex = 67;
            // 
            // receiveGrpBox
            // 
            this.receiveGrpBox.Controls.Add(this.dataLogControl);
            this.receiveGrpBox.Location = new System.Drawing.Point(429, 0);
            this.receiveGrpBox.Name = "receiveGrpBox";
            this.receiveGrpBox.Size = new System.Drawing.Size(311, 453);
            this.receiveGrpBox.TabIndex = 68;
            this.receiveGrpBox.TabStop = false;
            this.receiveGrpBox.Text = "Communication log panel";
            // 
            // dataLogControl
            // 
            this.dataLogControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataLogControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataLogControl.Location = new System.Drawing.Point(3, 16);
            this.dataLogControl.Name = "dataLogControl";
            this.dataLogControl.Size = new System.Drawing.Size(305, 434);
            this.dataLogControl.TabIndex = 0;
            // 
            // sendControl
            // 
            this.sendControl.AutoSize = true;
            this.sendControl.Location = new System.Drawing.Point(0, 6);
            this.sendControl.Name = "sendControl";
            this.sendControl.OutMessage = "";
            this.sendControl.Size = new System.Drawing.Size(429, 51);
            this.sendControl.TabIndex = 0;
            this.sendControl.MessageSentPressed += new System.EventHandler<UnitySC.Rorze.Emulator.Controls.SendMessageEventArgs>(this.SendControl_MessageSentPressed);
            // 
            // BaseEquipmentControl
            // 
            this.Controls.Add(this.receiveGrpBox);
            this.Controls.Add(this.autoResponsePanel);
            this.Controls.Add(this.tcpPanel);
            this.Controls.Add(this.commandPanel);
            this.Name = "BaseEquipmentControl";
            this.Size = new System.Drawing.Size(740, 456);
            this.commandPanel.ResumeLayout(false);
            this.commandPanel.PerformLayout();
            this.tcpPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcpStatusBarPanel)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.autoResponsePanel.ResumeLayout(false);
            this.autoResponsePanel.PerformLayout();
            this.receiveGrpBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        protected System.Windows.Forms.Panel commandPanel;
        protected System.Windows.Forms.CheckBox autorespButton;
        private SendControl sendControl;
        private System.Windows.Forms.Panel tcpPanel;
        private System.Windows.Forms.Panel autoResponsePanel;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusBar tcpStatusBar;
        private System.Windows.Forms.StatusBarPanel tcpStatusBarPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton serverRadioButton;
        private System.Windows.Forms.RadioButton clientRadioButton;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Label portStaticLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label ipAddressStaticLabel;
        private System.Windows.Forms.GroupBox receiveGrpBox;
        private DataLogControl dataLogControl;
    }
}
