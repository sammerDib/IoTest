
namespace UnitySC.Rorze.Emulator.Equipment
{
    partial class Dio1Control
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
            this.dio1InputsOutputsControl = new System.Windows.Forms.TabControl();
            this.statusWordTabPage = new System.Windows.Forms.TabPage();
            this.dio1StatusWordSpyControl1 = new UnitySC.Rorze.Emulator.Controls.Dio1.Dio1StatusWordSpyControl();
            this.inputsOutputsTabpage = new System.Windows.Forms.TabPage();
            this.dio1InputsOutputsControl1 = new UnitySC.Rorze.Emulator.Controls.Dio1.Dio1InputsOutputsControl();
            this.dio1InputsOutputsControl.SuspendLayout();
            this.statusWordTabPage.SuspendLayout();
            this.inputsOutputsTabpage.SuspendLayout();
            this.SuspendLayout();
            // 
            // dio1InputsOutputsControl
            // 
            this.dio1InputsOutputsControl.Controls.Add(this.statusWordTabPage);
            this.dio1InputsOutputsControl.Controls.Add(this.inputsOutputsTabpage);
            this.dio1InputsOutputsControl.Location = new System.Drawing.Point(3, 108);
            this.dio1InputsOutputsControl.Name = "dio1InputsOutputsControl";
            this.dio1InputsOutputsControl.SelectedIndex = 0;
            this.dio1InputsOutputsControl.Size = new System.Drawing.Size(426, 268);
            this.dio1InputsOutputsControl.TabIndex = 69;
            // 
            // statusWordTabPage
            // 
            this.statusWordTabPage.Controls.Add(this.dio1StatusWordSpyControl1);
            this.statusWordTabPage.Location = new System.Drawing.Point(4, 22);
            this.statusWordTabPage.Name = "statusWordTabPage";
            this.statusWordTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.statusWordTabPage.Size = new System.Drawing.Size(418, 242);
            this.statusWordTabPage.TabIndex = 1;
            this.statusWordTabPage.Text = "StatusWord";
            this.statusWordTabPage.UseVisualStyleBackColor = true;
            // 
            // dio1StatusWordSpyControl1
            // 
            this.dio1StatusWordSpyControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dio1StatusWordSpyControl1.Location = new System.Drawing.Point(3, 3);
            this.dio1StatusWordSpyControl1.Name = "dio1StatusWordSpyControl1";
            this.dio1StatusWordSpyControl1.Size = new System.Drawing.Size(412, 236);
            this.dio1StatusWordSpyControl1.TabIndex = 0;
            // 
            // inputsOutputsTabpage
            // 
            this.inputsOutputsTabpage.Controls.Add(this.dio1InputsOutputsControl1);
            this.inputsOutputsTabpage.Location = new System.Drawing.Point(4, 22);
            this.inputsOutputsTabpage.Name = "inputsOutputsTabpage";
            this.inputsOutputsTabpage.Size = new System.Drawing.Size(418, 242);
            this.inputsOutputsTabpage.TabIndex = 2;
            this.inputsOutputsTabpage.Text = "Inputs/Outputs";
            this.inputsOutputsTabpage.UseVisualStyleBackColor = true;
            // 
            // dio1InputsOutputsControl1
            // 
            this.dio1InputsOutputsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dio1InputsOutputsControl1.Location = new System.Drawing.Point(0, 0);
            this.dio1InputsOutputsControl1.Name = "dio1InputsOutputsControl1";
            this.dio1InputsOutputsControl1.Size = new System.Drawing.Size(418, 242);
            this.dio1InputsOutputsControl1.TabIndex = 0;
            // 
            // Dio1Control
            // 
            this.Controls.Add(this.dio1InputsOutputsControl);
            this.Name = "Dio1Control";
            this.Controls.SetChildIndex(this.commandPanel, 0);
            this.Controls.SetChildIndex(this.dio1InputsOutputsControl, 0);
            this.dio1InputsOutputsControl.ResumeLayout(false);
            this.statusWordTabPage.ResumeLayout(false);
            this.inputsOutputsTabpage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl dio1InputsOutputsControl;
        private System.Windows.Forms.TabPage statusWordTabPage;
        private System.Windows.Forms.TabPage inputsOutputsTabpage;
        private Controls.Dio1.Dio1StatusWordSpyControl dio1StatusWordSpyControl1;
        private Controls.Dio1.Dio1InputsOutputsControl dio1InputsOutputsControl1;
    }
}
