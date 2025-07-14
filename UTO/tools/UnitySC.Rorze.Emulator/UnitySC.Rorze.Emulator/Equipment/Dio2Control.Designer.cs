
namespace UnitySC.Rorze.Emulator.Equipment
{
    partial class Dio2Control
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.statusWordTabPage = new System.Windows.Forms.TabPage();
            this.dio2StatusWordSpyControl1 = new UnitySC.Rorze.Emulator.Controls.Dio2.Dio2StatusWordSpyControl();
            this.inputsOutputsTabpage = new System.Windows.Forms.TabPage();
            this.dio2InputsOutputsControl1 = new UnitySC.Rorze.Emulator.Controls.Dio2.Dio2InputsOutputsControl();
            this.tabControl.SuspendLayout();
            this.statusWordTabPage.SuspendLayout();
            this.inputsOutputsTabpage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.statusWordTabPage);
            this.tabControl.Controls.Add(this.inputsOutputsTabpage);
            this.tabControl.Location = new System.Drawing.Point(3, 108);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(426, 268);
            this.tabControl.TabIndex = 69;
            // 
            // statusWordTabPage
            // 
            this.statusWordTabPage.Controls.Add(this.dio2StatusWordSpyControl1);
            this.statusWordTabPage.Location = new System.Drawing.Point(4, 22);
            this.statusWordTabPage.Name = "statusWordTabPage";
            this.statusWordTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.statusWordTabPage.Size = new System.Drawing.Size(418, 242);
            this.statusWordTabPage.TabIndex = 1;
            this.statusWordTabPage.Text = "StatusWord";
            this.statusWordTabPage.UseVisualStyleBackColor = true;
            // 
            // dio2StatusWordSpyControl1
            // 
            this.dio2StatusWordSpyControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dio2StatusWordSpyControl1.Location = new System.Drawing.Point(3, 3);
            this.dio2StatusWordSpyControl1.Name = "dio2StatusWordSpyControl1";
            this.dio2StatusWordSpyControl1.Size = new System.Drawing.Size(412, 236);
            this.dio2StatusWordSpyControl1.TabIndex = 0;
            // 
            // inputsOutputsTabpage
            // 
            this.inputsOutputsTabpage.Controls.Add(this.dio2InputsOutputsControl1);
            this.inputsOutputsTabpage.Location = new System.Drawing.Point(4, 22);
            this.inputsOutputsTabpage.Name = "inputsOutputsTabpage";
            this.inputsOutputsTabpage.Size = new System.Drawing.Size(418, 242);
            this.inputsOutputsTabpage.TabIndex = 2;
            this.inputsOutputsTabpage.Text = "Inputs/Outputs";
            this.inputsOutputsTabpage.UseVisualStyleBackColor = true;
            // 
            // dio2InputsOutputsControl1
            // 
            this.dio2InputsOutputsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dio2InputsOutputsControl1.Location = new System.Drawing.Point(0, 0);
            this.dio2InputsOutputsControl1.Name = "dio2InputsOutputsControl1";
            this.dio2InputsOutputsControl1.Size = new System.Drawing.Size(418, 242);
            this.dio2InputsOutputsControl1.TabIndex = 0;
            // 
            // Dio2Control
            // 
            this.Controls.Add(this.tabControl);
            this.Name = "Dio2Control";
            this.Controls.SetChildIndex(this.commandPanel, 0);
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.tabControl.ResumeLayout(false);
            this.statusWordTabPage.ResumeLayout(false);
            this.inputsOutputsTabpage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage statusWordTabPage;
        private System.Windows.Forms.TabPage inputsOutputsTabpage;
        private Controls.Dio2.Dio2StatusWordSpyControl dio2StatusWordSpyControl1;
        private Controls.Dio2.Dio2InputsOutputsControl dio2InputsOutputsControl1;
    }
}
