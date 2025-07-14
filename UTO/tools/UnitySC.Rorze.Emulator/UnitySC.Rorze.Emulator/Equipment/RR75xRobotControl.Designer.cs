
namespace UnitySC.Rorze.Emulator.Equipment
{
    partial class RR75xRobotControl
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
            this.loadPortsTabPage = new System.Windows.Forms.TabPage();
            this.bLP4SetMapping = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tLP4Mapping = new System.Windows.Forms.TextBox();
            this.bLP3SetMapping = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tLP3Mapping = new System.Windows.Forms.TextBox();
            this.bLP2SetMapping = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tLP2Mapping = new System.Windows.Forms.TextBox();
            this.bLP1SetMapping = new System.Windows.Forms.Button();
            this.mappingLabel = new System.Windows.Forms.Label();
            this.tLP1Mapping = new System.Windows.Forms.TextBox();
            this.inputsOutputsTabpage = new System.Windows.Forms.TabPage();
            this.rR75xInputsOutputsControl1 = new UnitySC.Rorze.Emulator.Controls.RR75x.RR75xInputsOutputsControl();
            this.statusWordTabPage = new System.Windows.Forms.TabPage();
            this.rr75xStatusWordSpyControl = new UnitySC.Rorze.Emulator.Controls.RR75x.Rr75xStatusWordSpyControl();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.loadPortsTabPage.SuspendLayout();
            this.inputsOutputsTabpage.SuspendLayout();
            this.statusWordTabPage.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadPortsTabPage
            // 
            this.loadPortsTabPage.Controls.Add(this.bLP4SetMapping);
            this.loadPortsTabPage.Controls.Add(this.label4);
            this.loadPortsTabPage.Controls.Add(this.tLP4Mapping);
            this.loadPortsTabPage.Controls.Add(this.bLP3SetMapping);
            this.loadPortsTabPage.Controls.Add(this.label3);
            this.loadPortsTabPage.Controls.Add(this.tLP3Mapping);
            this.loadPortsTabPage.Controls.Add(this.bLP2SetMapping);
            this.loadPortsTabPage.Controls.Add(this.label1);
            this.loadPortsTabPage.Controls.Add(this.tLP2Mapping);
            this.loadPortsTabPage.Controls.Add(this.bLP1SetMapping);
            this.loadPortsTabPage.Controls.Add(this.mappingLabel);
            this.loadPortsTabPage.Controls.Add(this.tLP1Mapping);
            this.loadPortsTabPage.Location = new System.Drawing.Point(4, 22);
            this.loadPortsTabPage.Name = "loadPortsTabPage";
            this.loadPortsTabPage.Size = new System.Drawing.Size(418, 242);
            this.loadPortsTabPage.TabIndex = 3;
            this.loadPortsTabPage.Text = "LPs";
            this.loadPortsTabPage.UseVisualStyleBackColor = true;
            // 
            // bLP4SetMapping
            // 
            this.bLP4SetMapping.Location = new System.Drawing.Point(270, 162);
            this.bLP4SetMapping.Name = "bLP4SetMapping";
            this.bLP4SetMapping.Size = new System.Drawing.Size(121, 23);
            this.bLP4SetMapping.TabIndex = 36;
            this.bLP4SetMapping.Text = "Set mapping";
            this.bLP4SetMapping.UseVisualStyleBackColor = true;
            this.bLP4SetMapping.Click += new System.EventHandler(this.bLP4SetMapping_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(2, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "LP4 Mapping";
            // 
            // tLP4Mapping
            // 
            this.tLP4Mapping.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tLP4Mapping.Location = new System.Drawing.Point(3, 162);
            this.tLP4Mapping.MaxLength = 25;
            this.tLP4Mapping.Name = "tLP4Mapping";
            this.tLP4Mapping.Size = new System.Drawing.Size(258, 26);
            this.tLP4Mapping.TabIndex = 34;
            this.tLP4Mapping.Text = "0000000000000000000000000";
            // 
            // bLP3SetMapping
            // 
            this.bLP3SetMapping.Location = new System.Drawing.Point(270, 114);
            this.bLP3SetMapping.Name = "bLP3SetMapping";
            this.bLP3SetMapping.Size = new System.Drawing.Size(121, 23);
            this.bLP3SetMapping.TabIndex = 33;
            this.bLP3SetMapping.Text = "Set mapping";
            this.bLP3SetMapping.UseVisualStyleBackColor = true;
            this.bLP3SetMapping.Click += new System.EventHandler(this.bLP3SetMapping_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(2, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "LP3 Mapping";
            // 
            // tLP3Mapping
            // 
            this.tLP3Mapping.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tLP3Mapping.Location = new System.Drawing.Point(3, 114);
            this.tLP3Mapping.MaxLength = 25;
            this.tLP3Mapping.Name = "tLP3Mapping";
            this.tLP3Mapping.Size = new System.Drawing.Size(258, 26);
            this.tLP3Mapping.TabIndex = 31;
            this.tLP3Mapping.Text = "0000000000000000000000000";
            // 
            // bLP2SetMapping
            // 
            this.bLP2SetMapping.Location = new System.Drawing.Point(270, 65);
            this.bLP2SetMapping.Name = "bLP2SetMapping";
            this.bLP2SetMapping.Size = new System.Drawing.Size(121, 23);
            this.bLP2SetMapping.TabIndex = 30;
            this.bLP2SetMapping.Text = "Set mapping";
            this.bLP2SetMapping.UseVisualStyleBackColor = true;
            this.bLP2SetMapping.Click += new System.EventHandler(this.bLP2SetMapping_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(2, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "LP2 Mapping";
            // 
            // tLP2Mapping
            // 
            this.tLP2Mapping.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tLP2Mapping.Location = new System.Drawing.Point(3, 65);
            this.tLP2Mapping.MaxLength = 25;
            this.tLP2Mapping.Name = "tLP2Mapping";
            this.tLP2Mapping.Size = new System.Drawing.Size(258, 26);
            this.tLP2Mapping.TabIndex = 28;
            this.tLP2Mapping.Text = "0000000000000000000000000";
            // 
            // bLP1SetMapping
            // 
            this.bLP1SetMapping.Location = new System.Drawing.Point(271, 16);
            this.bLP1SetMapping.Name = "bLP1SetMapping";
            this.bLP1SetMapping.Size = new System.Drawing.Size(121, 23);
            this.bLP1SetMapping.TabIndex = 27;
            this.bLP1SetMapping.Text = "Set mapping";
            this.bLP1SetMapping.UseVisualStyleBackColor = true;
            this.bLP1SetMapping.Click += new System.EventHandler(this.bLP1SetMapping_Click);
            // 
            // mappingLabel
            // 
            this.mappingLabel.AutoSize = true;
            this.mappingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mappingLabel.Location = new System.Drawing.Point(3, 0);
            this.mappingLabel.Name = "mappingLabel";
            this.mappingLabel.Size = new System.Drawing.Size(70, 13);
            this.mappingLabel.TabIndex = 26;
            this.mappingLabel.Text = "LP1 Mapping";
            // 
            // tLP1Mapping
            // 
            this.tLP1Mapping.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tLP1Mapping.Location = new System.Drawing.Point(4, 16);
            this.tLP1Mapping.MaxLength = 25;
            this.tLP1Mapping.Name = "tLP1Mapping";
            this.tLP1Mapping.Size = new System.Drawing.Size(258, 26);
            this.tLP1Mapping.TabIndex = 25;
            this.tLP1Mapping.Text = "0000000000000000000000000";
            // 
            // inputsOutputsTabpage
            // 
            this.inputsOutputsTabpage.Controls.Add(this.rR75xInputsOutputsControl1);
            this.inputsOutputsTabpage.Location = new System.Drawing.Point(4, 22);
            this.inputsOutputsTabpage.Name = "inputsOutputsTabpage";
            this.inputsOutputsTabpage.Size = new System.Drawing.Size(418, 242);
            this.inputsOutputsTabpage.TabIndex = 2;
            this.inputsOutputsTabpage.Text = "Inputs/Outputs";
            // 
            // rR75xInputsOutputsControl1
            // 
            this.rR75xInputsOutputsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rR75xInputsOutputsControl1.Location = new System.Drawing.Point(0, 0);
            this.rR75xInputsOutputsControl1.Name = "rR75xInputsOutputsControl1";
            this.rR75xInputsOutputsControl1.Size = new System.Drawing.Size(418, 242);
            this.rR75xInputsOutputsControl1.TabIndex = 0;
            // 
            // statusWordTabPage
            // 
            this.statusWordTabPage.Controls.Add(this.rr75xStatusWordSpyControl);
            this.statusWordTabPage.Location = new System.Drawing.Point(4, 22);
            this.statusWordTabPage.Name = "statusWordTabPage";
            this.statusWordTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.statusWordTabPage.Size = new System.Drawing.Size(418, 242);
            this.statusWordTabPage.TabIndex = 1;
            this.statusWordTabPage.Text = "StatusWord";
            this.statusWordTabPage.UseVisualStyleBackColor = true;
            // 
            // rr75xStatusWordSpyControl
            // 
            this.rr75xStatusWordSpyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rr75xStatusWordSpyControl.Location = new System.Drawing.Point(3, 3);
            this.rr75xStatusWordSpyControl.Name = "rr75xStatusWordSpyControl";
            this.rr75xStatusWordSpyControl.Size = new System.Drawing.Size(412, 236);
            this.rr75xStatusWordSpyControl.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.statusWordTabPage);
            this.tabControl.Controls.Add(this.inputsOutputsTabpage);
            this.tabControl.Controls.Add(this.loadPortsTabPage);
            this.tabControl.Location = new System.Drawing.Point(3, 108);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(426, 268);
            this.tabControl.TabIndex = 69;
            // 
            // RR75xRobotControl
            // 
            this.Controls.Add(this.tabControl);
            this.Name = "RR75xRobotControl";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.Controls.SetChildIndex(this.commandPanel, 0);
            this.loadPortsTabPage.ResumeLayout(false);
            this.loadPortsTabPage.PerformLayout();
            this.inputsOutputsTabpage.ResumeLayout(false);
            this.statusWordTabPage.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage loadPortsTabPage;
        private System.Windows.Forms.TabPage inputsOutputsTabpage;
        private Controls.RR75x.RR75xInputsOutputsControl rR75xInputsOutputsControl1;
        private System.Windows.Forms.TabPage statusWordTabPage;
        private Controls.RR75x.Rr75xStatusWordSpyControl rr75xStatusWordSpyControl;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Button bLP4SetMapping;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tLP4Mapping;
        private System.Windows.Forms.Button bLP3SetMapping;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tLP3Mapping;
        private System.Windows.Forms.Button bLP2SetMapping;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tLP2Mapping;
        private System.Windows.Forms.Button bLP1SetMapping;
        private System.Windows.Forms.Label mappingLabel;
        private System.Windows.Forms.TextBox tLP1Mapping;
    }
}
