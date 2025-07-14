using UnitySC.Rorze.Emulator.Controls;
using UnitySC.Rorze.Emulator.Controls.RV101;

namespace UnitySC.Rorze.Emulator.Equipment
{
    partial class Rv101LoadPortControl
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
            this.mappingButton = new System.Windows.Forms.Button();
            this.mappingLabel = new System.Windows.Forms.Label();
            this.mappingTextBox = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.mainTabPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.foupRemovedButton = new System.Windows.Forms.Button();
            this.foupNoPresentPlacedButton = new System.Windows.Forms.Button();
            this.foupPresentNoPlacedButton = new System.Windows.Forms.Button();
            this.foupPresentPlacedButton = new System.Windows.Forms.Button();
            this.statusWordTabPage = new System.Windows.Forms.TabPage();
            this.loadPortStatusWordSpyControl1 = new UnitySC.Rorze.Emulator.Controls.RV101.Rv101StatusWordSpyControl();
            this.inputsOutputsTabpage = new System.Windows.Forms.TabPage();
            this.rv101InputsOutputsControl1 = new UnitySC.Rorze.Emulator.Controls.RV101.Rv101InputsOutputsControl();
            this.tabControl.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusWordTabPage.SuspendLayout();
            this.inputsOutputsTabpage.SuspendLayout();
            this.SuspendLayout();
            // 
            // mappingButton
            // 
            this.mappingButton.Location = new System.Drawing.Point(274, 16);
            this.mappingButton.Name = "mappingButton";
            this.mappingButton.Size = new System.Drawing.Size(121, 23);
            this.mappingButton.TabIndex = 24;
            this.mappingButton.Text = "Set mapping";
            this.mappingButton.UseVisualStyleBackColor = true;
            this.mappingButton.Click += new System.EventHandler(this.MappingButton_Click);
            // 
            // mappingLabel
            // 
            this.mappingLabel.AutoSize = true;
            this.mappingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mappingLabel.Location = new System.Drawing.Point(6, 0);
            this.mappingLabel.Name = "mappingLabel";
            this.mappingLabel.Size = new System.Drawing.Size(48, 13);
            this.mappingLabel.TabIndex = 23;
            this.mappingLabel.Text = "Mapping";
            // 
            // mappingTextBox
            // 
            this.mappingTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mappingTextBox.Location = new System.Drawing.Point(7, 16);
            this.mappingTextBox.MaxLength = 25;
            this.mappingTextBox.Name = "mappingTextBox";
            this.mappingTextBox.Size = new System.Drawing.Size(258, 26);
            this.mappingTextBox.TabIndex = 22;
            this.mappingTextBox.Text = "0000000000000000000000000";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.mainTabPage);
            this.tabControl.Controls.Add(this.statusWordTabPage);
            this.tabControl.Controls.Add(this.inputsOutputsTabpage);
            this.tabControl.Location = new System.Drawing.Point(3, 108);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(426, 268);
            this.tabControl.TabIndex = 28;
            // 
            // mainTabPage
            // 
            this.mainTabPage.Controls.Add(this.groupBox1);
            this.mainTabPage.Controls.Add(this.mappingButton);
            this.mainTabPage.Controls.Add(this.mappingLabel);
            this.mainTabPage.Controls.Add(this.mappingTextBox);
            this.mainTabPage.Location = new System.Drawing.Point(4, 22);
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mainTabPage.Size = new System.Drawing.Size(418, 242);
            this.mainTabPage.TabIndex = 0;
            this.mainTabPage.Text = "Main";
            this.mainTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.foupRemovedButton);
            this.groupBox1.Controls.Add(this.foupNoPresentPlacedButton);
            this.groupBox1.Controls.Add(this.foupPresentNoPlacedButton);
            this.groupBox1.Controls.Add(this.foupPresentPlacedButton);
            this.groupBox1.Location = new System.Drawing.Point(7, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 71);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FOUP";
            // 
            // foupRemovedButton
            // 
            this.foupRemovedButton.Location = new System.Drawing.Point(3, 14);
            this.foupRemovedButton.Name = "foupRemovedButton";
            this.foupRemovedButton.Size = new System.Drawing.Size(121, 23);
            this.foupRemovedButton.TabIndex = 33;
            this.foupRemovedButton.Text = "Remove Carrier";
            this.foupRemovedButton.UseVisualStyleBackColor = true;
            this.foupRemovedButton.Click += new System.EventHandler(this.FoupRemovedButton_Click);
            // 
            // foupNoPresentPlacedButton
            // 
            this.foupNoPresentPlacedButton.Location = new System.Drawing.Point(134, 14);
            this.foupNoPresentPlacedButton.Name = "foupNoPresentPlacedButton";
            this.foupNoPresentPlacedButton.Size = new System.Drawing.Size(121, 23);
            this.foupNoPresentPlacedButton.TabIndex = 32;
            this.foupNoPresentPlacedButton.Text = "No Pres\\Plac";
            this.foupNoPresentPlacedButton.UseVisualStyleBackColor = true;
            this.foupNoPresentPlacedButton.Click += new System.EventHandler(this.FoupNoPresentPlacedButton_Click);
            // 
            // foupPresentNoPlacedButton
            // 
            this.foupPresentNoPlacedButton.Location = new System.Drawing.Point(134, 43);
            this.foupPresentNoPlacedButton.Name = "foupPresentNoPlacedButton";
            this.foupPresentNoPlacedButton.Size = new System.Drawing.Size(121, 23);
            this.foupPresentNoPlacedButton.TabIndex = 31;
            this.foupPresentNoPlacedButton.Text = "Pres\\No Plac";
            this.foupPresentNoPlacedButton.UseVisualStyleBackColor = true;
            this.foupPresentNoPlacedButton.Click += new System.EventHandler(this.FoupPresentNoPlacedButton_Click);
            // 
            // foupPresentPlacedButton
            // 
            this.foupPresentPlacedButton.Location = new System.Drawing.Point(3, 43);
            this.foupPresentPlacedButton.Name = "foupPresentPlacedButton";
            this.foupPresentPlacedButton.Size = new System.Drawing.Size(121, 23);
            this.foupPresentPlacedButton.TabIndex = 30;
            this.foupPresentPlacedButton.Text = "Place Carrier";
            this.foupPresentPlacedButton.UseVisualStyleBackColor = true;
            this.foupPresentPlacedButton.Click += new System.EventHandler(this.FoupPresentPlacedButton_Click);
            // 
            // statusWordTabPage
            // 
            this.statusWordTabPage.Controls.Add(this.loadPortStatusWordSpyControl1);
            this.statusWordTabPage.Location = new System.Drawing.Point(4, 22);
            this.statusWordTabPage.Name = "statusWordTabPage";
            this.statusWordTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.statusWordTabPage.Size = new System.Drawing.Size(421, 120);
            this.statusWordTabPage.TabIndex = 1;
            this.statusWordTabPage.Text = "StatusWord";
            this.statusWordTabPage.UseVisualStyleBackColor = true;
            // 
            // loadPortStatusWordSpyControl1
            // 
            this.loadPortStatusWordSpyControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadPortStatusWordSpyControl1.Location = new System.Drawing.Point(3, 3);
            this.loadPortStatusWordSpyControl1.Name = "loadPortStatusWordSpyControl1";
            this.loadPortStatusWordSpyControl1.Size = new System.Drawing.Size(415, 114);
            this.loadPortStatusWordSpyControl1.TabIndex = 27;
            // 
            // inputsOutputsTabpage
            // 
            this.inputsOutputsTabpage.Controls.Add(this.rv101InputsOutputsControl1);
            this.inputsOutputsTabpage.Location = new System.Drawing.Point(4, 22);
            this.inputsOutputsTabpage.Name = "inputsOutputsTabpage";
            this.inputsOutputsTabpage.Size = new System.Drawing.Size(421, 120);
            this.inputsOutputsTabpage.TabIndex = 2;
            this.inputsOutputsTabpage.Text = "Inputs/Outputs";
            this.inputsOutputsTabpage.UseVisualStyleBackColor = true;
            // 
            // rE201InputsOutputsControl1
            // 
            this.rv101InputsOutputsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rv101InputsOutputsControl1.Location = new System.Drawing.Point(0, 0);
            this.rv101InputsOutputsControl1.Name = "rE201InputsOutputsControl1";
            this.rv101InputsOutputsControl1.Size = new System.Drawing.Size(421, 120);
            this.rv101InputsOutputsControl1.TabIndex = 0;
            // 
            // Re201SmifLoadPortControl
            // 
            this.Controls.Add(this.tabControl);
            this.Name = "Re201SmifLoadPortControl";
            this.Controls.SetChildIndex(this.commandPanel, 0);
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.tabControl.ResumeLayout(false);
            this.mainTabPage.ResumeLayout(false);
            this.mainTabPage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.statusWordTabPage.ResumeLayout(false);
            this.inputsOutputsTabpage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button mappingButton;
        private System.Windows.Forms.Label mappingLabel;
        private System.Windows.Forms.TextBox mappingTextBox;
        private Rv101StatusWordSpyControl loadPortStatusWordSpyControl1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage mainTabPage;
        private System.Windows.Forms.TabPage statusWordTabPage;
        private System.Windows.Forms.Button foupPresentPlacedButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button foupRemovedButton;
        private System.Windows.Forms.Button foupNoPresentPlacedButton;
        private System.Windows.Forms.Button foupPresentNoPlacedButton;
        private System.Windows.Forms.TabPage inputsOutputsTabpage;
        private Rv101InputsOutputsControl rv101InputsOutputsControl1;
    }
}
