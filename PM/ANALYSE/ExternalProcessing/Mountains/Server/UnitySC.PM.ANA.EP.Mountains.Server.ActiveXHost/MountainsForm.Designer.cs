namespace UnitySC.PM.ANA.EP.Mountains.Server.ActiveXHost
{
    partial class MountainsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MountainsForm));
            this.groupBoxMountains = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // groupBoxMountains
            // 
            this.groupBoxMountains.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMountains.Location = new System.Drawing.Point(0, 0);
            this.groupBoxMountains.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBoxMountains.Name = "groupBoxMountains";
            this.groupBoxMountains.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBoxMountains.Size = new System.Drawing.Size(120, 61);
            this.groupBoxMountains.TabIndex = 0;
            this.groupBoxMountains.TabStop = false;
            this.groupBoxMountains.Text = "Mountains ActiveX";
            // 
            // MountainsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(120, 61);
            this.Controls.Add(this.groupBoxMountains);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MountainsForm";
            this.Text = "MountainsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MountainsForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMountains;

    }
}

