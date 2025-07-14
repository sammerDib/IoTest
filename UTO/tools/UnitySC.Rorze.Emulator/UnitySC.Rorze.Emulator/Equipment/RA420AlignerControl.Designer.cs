
namespace UnitySC.Rorze.Emulator.Equipment
{
    partial class RA420AlignerControl
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
            this.inputsOutputsTabpage = new System.Windows.Forms.TabPage();
            this.rA420InputsOutputsControl = new UnitySC.Rorze.Emulator.Controls.RA420.RA420InputsOutputsControl();
            this.ra420StatusWordSpyControl1 = new UnitySC.Rorze.Emulator.Controls.RA420.Ra420StatusWordSpyControl();
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
            this.statusWordTabPage.Controls.Add(this.ra420StatusWordSpyControl1);
            this.statusWordTabPage.Location = new System.Drawing.Point(4, 22);
            this.statusWordTabPage.Name = "statusWordTabPage";
            this.statusWordTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.statusWordTabPage.Size = new System.Drawing.Size(418, 242);
            this.statusWordTabPage.TabIndex = 1;
            this.statusWordTabPage.Text = "StatusWord";
            this.statusWordTabPage.UseVisualStyleBackColor = true;
            // 
            // inputsOutputsTabpage
            // 
            this.inputsOutputsTabpage.Controls.Add(this.rA420InputsOutputsControl);
            this.inputsOutputsTabpage.Location = new System.Drawing.Point(4, 22);
            this.inputsOutputsTabpage.Name = "inputsOutputsTabpage";
            this.inputsOutputsTabpage.Size = new System.Drawing.Size(418, 242);
            this.inputsOutputsTabpage.TabIndex = 2;
            this.inputsOutputsTabpage.Text = "Inputs/Outputs";
            this.inputsOutputsTabpage.UseVisualStyleBackColor = true;
            // 
            // rA420InputsOutputsControl
            // 
            this.rA420InputsOutputsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rA420InputsOutputsControl.Location = new System.Drawing.Point(0, 0);
            this.rA420InputsOutputsControl.Name = "rA420InputsOutputsControl";
            this.rA420InputsOutputsControl.Size = new System.Drawing.Size(418, 242);
            this.rA420InputsOutputsControl.TabIndex = 0;
            // 
            // ra420StatusWordSpyControl1
            // 
            this.ra420StatusWordSpyControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ra420StatusWordSpyControl1.Location = new System.Drawing.Point(3, 3);
            this.ra420StatusWordSpyControl1.Name = "ra420StatusWordSpyControl1";
            this.ra420StatusWordSpyControl1.Size = new System.Drawing.Size(412, 236);
            this.ra420StatusWordSpyControl1.TabIndex = 0;
            // 
            // RA420AlignerControl
            // 
            this.Controls.Add(this.tabControl);
            this.Name = "RA420AlignerControl";
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
        private Controls.RA420.Ra420StatusWordSpyControl ra420StatusWordSpyControl1;
        private Controls.RA420.RA420InputsOutputsControl rA420InputsOutputsControl;
    }
}
