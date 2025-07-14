using UnitySC.Rorze.Emulator.Controls;
using UnitySC.Rorze.Emulator.Equipment;

namespace UnitySC.Rorze.Emulator
{
    partial class EquipmentTestForm
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
            this.deviceSelectionControlTab = new System.Windows.Forms.TabControl();
            this.tpRobot = new System.Windows.Forms.TabPage();
            this.rR75xRobotControl1 = new UnitySC.Rorze.Emulator.Equipment.RR75xRobotControl();
            this.tpAligner = new System.Windows.Forms.TabPage();
            this.rA420AlignerControl1 = new UnitySC.Rorze.Emulator.Equipment.RA420AlignerControl();
            this.tpSmifLP1 = new System.Windows.Forms.TabPage();
            this.re201SmifLoadPortControl2 = new UnitySC.Rorze.Emulator.Equipment.Re201SmifLoadPortControl();
            this.tpSmifLP2 = new System.Windows.Forms.TabPage();
            this.re201SmifLoadPortControl1 = new UnitySC.Rorze.Emulator.Equipment.Re201SmifLoadPortControl();
            this.tpDio0 = new System.Windows.Forms.TabPage();
            this.dio0Control1 = new UnitySC.Rorze.Emulator.Equipment.Dio0Control();
            this.tpDio1 = new System.Windows.Forms.TabPage();
            this.dio1Control1 = new UnitySC.Rorze.Emulator.Equipment.Dio1Control();
            this.tpDio2 = new System.Windows.Forms.TabPage();
            this.dio2Control1 = new UnitySC.Rorze.Emulator.Equipment.Dio2Control();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this._rv101LoadPortControl1 = new UnitySC.Rorze.Emulator.Equipment.Rv101LoadPortControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this._rv101LoadPortControl2 = new UnitySC.Rorze.Emulator.Equipment.Rv101LoadPortControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dio1MediumSizeEfemControl1 = new UnitySC.Rorze.Emulator.Equipment.Dio1MediumSizeEfemControl();
            this.deviceSelectionControlTab.SuspendLayout();
            this.tpRobot.SuspendLayout();
            this.tpAligner.SuspendLayout();
            this.tpSmifLP1.SuspendLayout();
            this.tpSmifLP2.SuspendLayout();
            this.tpDio0.SuspendLayout();
            this.tpDio1.SuspendLayout();
            this.tpDio2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // deviceSelectionControlTab
            // 
            this.deviceSelectionControlTab.Controls.Add(this.tpRobot);
            this.deviceSelectionControlTab.Controls.Add(this.tpAligner);
            this.deviceSelectionControlTab.Controls.Add(this.tpSmifLP1);
            this.deviceSelectionControlTab.Controls.Add(this.tpSmifLP2);
            this.deviceSelectionControlTab.Controls.Add(this.tpDio0);
            this.deviceSelectionControlTab.Controls.Add(this.tpDio1);
            this.deviceSelectionControlTab.Controls.Add(this.tpDio2);
            this.deviceSelectionControlTab.Controls.Add(this.tabPage1);
            this.deviceSelectionControlTab.Controls.Add(this.tabPage2);
            this.deviceSelectionControlTab.Controls.Add(this.tabPage3);
            this.deviceSelectionControlTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceSelectionControlTab.Location = new System.Drawing.Point(0, 0);
            this.deviceSelectionControlTab.Name = "deviceSelectionControlTab";
            this.deviceSelectionControlTab.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.deviceSelectionControlTab.SelectedIndex = 0;
            this.deviceSelectionControlTab.Size = new System.Drawing.Size(874, 529);
            this.deviceSelectionControlTab.TabIndex = 29;
            // 
            // tpRobot
            // 
            this.tpRobot.Controls.Add(this.rR75xRobotControl1);
            this.tpRobot.Location = new System.Drawing.Point(4, 22);
            this.tpRobot.Name = "tpRobot";
            this.tpRobot.Size = new System.Drawing.Size(866, 503);
            this.tpRobot.TabIndex = 15;
            this.tpRobot.Text = "RR75x";
            this.tpRobot.UseVisualStyleBackColor = true;
            // 
            // rR75xRobotControl1
            // 
            this.rR75xRobotControl1.AutoResponseEnabled = true;
            this.rR75xRobotControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rR75xRobotControl1.Location = new System.Drawing.Point(0, 0);
            this.rR75xRobotControl1.Name = "rR75xRobotControl1";
            this.rR75xRobotControl1.Size = new System.Drawing.Size(866, 503);
            this.rR75xRobotControl1.TabIndex = 0;
            // 
            // tpAligner
            // 
            this.tpAligner.Controls.Add(this.rA420AlignerControl1);
            this.tpAligner.Location = new System.Drawing.Point(4, 22);
            this.tpAligner.Name = "tpAligner";
            this.tpAligner.Size = new System.Drawing.Size(866, 503);
            this.tpAligner.TabIndex = 14;
            this.tpAligner.Text = "RA420";
            this.tpAligner.UseVisualStyleBackColor = true;
            // 
            // rA420AlignerControl1
            // 
            this.rA420AlignerControl1.AutoResponseEnabled = false;
            this.rA420AlignerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rA420AlignerControl1.Location = new System.Drawing.Point(0, 0);
            this.rA420AlignerControl1.Name = "rA420AlignerControl1";
            this.rA420AlignerControl1.Size = new System.Drawing.Size(866, 503);
            this.rA420AlignerControl1.TabIndex = 0;
            // 
            // tpSmifLP1
            // 
            this.tpSmifLP1.Controls.Add(this.re201SmifLoadPortControl2);
            this.tpSmifLP1.Location = new System.Drawing.Point(4, 22);
            this.tpSmifLP1.Name = "tpSmifLP1";
            this.tpSmifLP1.Size = new System.Drawing.Size(866, 503);
            this.tpSmifLP1.TabIndex = 12;
            this.tpSmifLP1.Text = "SMIF LP1";
            this.tpSmifLP1.UseVisualStyleBackColor = true;
            // 
            // re201SmifLoadPortControl2
            // 
            this.re201SmifLoadPortControl2.AutoResponseEnabled = false;
            this.re201SmifLoadPortControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.re201SmifLoadPortControl2.InstanceId = 0;
            this.re201SmifLoadPortControl2.Location = new System.Drawing.Point(0, 0);
            this.re201SmifLoadPortControl2.Name = "re201SmifLoadPortControl2";
            this.re201SmifLoadPortControl2.Size = new System.Drawing.Size(866, 503);
            this.re201SmifLoadPortControl2.TabIndex = 0;
            // 
            // tpSmifLP2
            // 
            this.tpSmifLP2.Controls.Add(this.re201SmifLoadPortControl1);
            this.tpSmifLP2.Location = new System.Drawing.Point(4, 22);
            this.tpSmifLP2.Name = "tpSmifLP2";
            this.tpSmifLP2.Size = new System.Drawing.Size(866, 503);
            this.tpSmifLP2.TabIndex = 13;
            this.tpSmifLP2.Text = "SMIF LP2";
            this.tpSmifLP2.UseVisualStyleBackColor = true;
            // 
            // re201SmifLoadPortControl1
            // 
            this.re201SmifLoadPortControl1.AutoResponseEnabled = true;
            this.re201SmifLoadPortControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.re201SmifLoadPortControl1.InstanceId = 2;
            this.re201SmifLoadPortControl1.Location = new System.Drawing.Point(0, 0);
            this.re201SmifLoadPortControl1.Name = "re201SmifLoadPortControl1";
            this.re201SmifLoadPortControl1.Size = new System.Drawing.Size(866, 503);
            this.re201SmifLoadPortControl1.TabIndex = 1;
            // 
            // tpDio0
            // 
            this.tpDio0.Controls.Add(this.dio0Control1);
            this.tpDio0.Location = new System.Drawing.Point(4, 22);
            this.tpDio0.Name = "tpDio0";
            this.tpDio0.Size = new System.Drawing.Size(866, 503);
            this.tpDio0.TabIndex = 16;
            this.tpDio0.Text = "DIO 0";
            this.tpDio0.UseVisualStyleBackColor = true;
            // 
            // dio0Control1
            // 
            this.dio0Control1.AutoResponseEnabled = false;
            this.dio0Control1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dio0Control1.Location = new System.Drawing.Point(0, 0);
            this.dio0Control1.Name = "dio0Control1";
            this.dio0Control1.Size = new System.Drawing.Size(866, 503);
            this.dio0Control1.TabIndex = 0;
            // 
            // tpDio1
            // 
            this.tpDio1.Controls.Add(this.dio1Control1);
            this.tpDio1.Location = new System.Drawing.Point(4, 22);
            this.tpDio1.Name = "tpDio1";
            this.tpDio1.Size = new System.Drawing.Size(866, 503);
            this.tpDio1.TabIndex = 17;
            this.tpDio1.Text = "DIO 1";
            this.tpDio1.UseVisualStyleBackColor = true;
            // 
            // dio1Control1
            // 
            this.dio1Control1.AutoResponseEnabled = false;
            this.dio1Control1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dio1Control1.Location = new System.Drawing.Point(0, 0);
            this.dio1Control1.Name = "dio1Control1";
            this.dio1Control1.Size = new System.Drawing.Size(866, 503);
            this.dio1Control1.TabIndex = 0;
            // 
            // tpDio2
            // 
            this.tpDio2.Controls.Add(this.dio2Control1);
            this.tpDio2.Location = new System.Drawing.Point(4, 22);
            this.tpDio2.Name = "tpDio2";
            this.tpDio2.Size = new System.Drawing.Size(866, 503);
            this.tpDio2.TabIndex = 18;
            this.tpDio2.Text = "DIO 2";
            this.tpDio2.UseVisualStyleBackColor = true;
            // 
            // dio2Control1
            // 
            this.dio2Control1.AutoResponseEnabled = false;
            this.dio2Control1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dio2Control1.Location = new System.Drawing.Point(0, 0);
            this.dio2Control1.Name = "dio2Control1";
            this.dio2Control1.Size = new System.Drawing.Size(866, 503);
            this.dio2Control1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._rv101LoadPortControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(866, 503);
            this.tabPage1.TabIndex = 19;
            this.tabPage1.Text = "RV101 LP1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // _rv101LoadPortControl1
            // 
            this._rv101LoadPortControl1.AutoResponseEnabled = false;
            this._rv101LoadPortControl1.AutoSize = true;
            this._rv101LoadPortControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rv101LoadPortControl1.InstanceId = 1;
            this._rv101LoadPortControl1.Location = new System.Drawing.Point(3, 3);
            this._rv101LoadPortControl1.Name = "_rv101LoadPortControl1";
            this._rv101LoadPortControl1.Size = new System.Drawing.Size(860, 497);
            this._rv101LoadPortControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this._rv101LoadPortControl2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(866, 503);
            this.tabPage2.TabIndex = 20;
            this.tabPage2.Text = "RV101 LP2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // _rv101LoadPortControl2
            // 
            this._rv101LoadPortControl2.AutoResponseEnabled = false;
            this._rv101LoadPortControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rv101LoadPortControl2.InstanceId = 2;
            this._rv101LoadPortControl2.Location = new System.Drawing.Point(3, 3);
            this._rv101LoadPortControl2.Name = "_rv101LoadPortControl2";
            this._rv101LoadPortControl2.Size = new System.Drawing.Size(860, 497);
            this._rv101LoadPortControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dio1MediumSizeEfemControl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(866, 503);
            this.tabPage3.TabIndex = 21;
            this.tabPage3.Text = "DIO1 MediumSize";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dio1MediumSizeEfemControl1
            // 
            this.dio1MediumSizeEfemControl1.AutoResponseEnabled = false;
            this.dio1MediumSizeEfemControl1.Location = new System.Drawing.Point(0, 0);
            this.dio1MediumSizeEfemControl1.Name = "dio1MediumSizeEfemControl1";
            this.dio1MediumSizeEfemControl1.Size = new System.Drawing.Size(740, 456);
            this.dio1MediumSizeEfemControl1.TabIndex = 0;
            // 
            // EquipmentTestForm
            // 
            this.ClientSize = new System.Drawing.Size(874, 529);
            this.Controls.Add(this.deviceSelectionControlTab);
            this.DoubleBuffered = true;
            this.Name = "EquipmentTestForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "UnitySC.Rorze.Emulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HandleEquipmentTestFormClosing);
            this.deviceSelectionControlTab.ResumeLayout(false);
            this.tpRobot.ResumeLayout(false);
            this.tpAligner.ResumeLayout(false);
            this.tpSmifLP1.ResumeLayout(false);
            this.tpSmifLP2.ResumeLayout(false);
            this.tpDio0.ResumeLayout(false);
            this.tpDio1.ResumeLayout(false);
            this.tpDio2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl deviceSelectionControlTab;
        private System.Windows.Forms.TabPage tpSmifLP1;
        private System.Windows.Forms.TabPage tpSmifLP2;
        private Re201SmifLoadPortControl re201SmifLoadPortControl1;
        private System.Windows.Forms.TabPage tpAligner;
        private RA420AlignerControl rA420AlignerControl1;
        private System.Windows.Forms.TabPage tpRobot;
        private RR75xRobotControl rR75xRobotControl1;
        private Re201SmifLoadPortControl re201SmifLoadPortControl2;
        private System.Windows.Forms.TabPage tpDio0;
        private Dio0Control dio0Control1;
        private System.Windows.Forms.TabPage tpDio1;
        private Dio1Control dio1Control1;
        private System.Windows.Forms.TabPage tpDio2;
        private Dio2Control dio2Control1;
        private System.Windows.Forms.TabPage tabPage1;
        private Rv101LoadPortControl _rv101LoadPortControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private Rv101LoadPortControl _rv101LoadPortControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private Dio1MediumSizeEfemControl dio1MediumSizeEfemControl1;
    }
}
