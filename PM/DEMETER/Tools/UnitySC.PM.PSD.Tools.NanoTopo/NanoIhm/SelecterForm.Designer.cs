namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    partial class NanoIHMSelecter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NanoIHMSelecter));
            this.LaunchProductionButton = new System.Windows.Forms.Button();
            this.LaunchManualButton = new System.Windows.Forms.Button();
            this.ConfigParametersButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LaunchProductionButton
            // 
            this.LaunchProductionButton.Location = new System.Drawing.Point(24, 18);
            this.LaunchProductionButton.Name = "LaunchProductionButton";
            this.LaunchProductionButton.Size = new System.Drawing.Size(401, 29);
            this.LaunchProductionButton.TabIndex = 0;
            this.LaunchProductionButton.Text = "Start Nanotopography Production";
            this.LaunchProductionButton.UseVisualStyleBackColor = true;
            this.LaunchProductionButton.Click += new System.EventHandler(this.LaunchProductionButton_Click);
            // 
            // LaunchManualButton
            // 
            this.LaunchManualButton.Location = new System.Drawing.Point(24, 73);
            this.LaunchManualButton.Name = "LaunchManualButton";
            this.LaunchManualButton.Size = new System.Drawing.Size(401, 29);
            this.LaunchManualButton.TabIndex = 1;
            this.LaunchManualButton.Text = "Start Local Nanotopography";
            this.LaunchManualButton.UseVisualStyleBackColor = true;
            this.LaunchManualButton.Click += new System.EventHandler(this.LaunchManualButton_Click);
            // 
            // ConfigParametersButton
            // 
            this.ConfigParametersButton.Location = new System.Drawing.Point(24, 126);
            this.ConfigParametersButton.Name = "ConfigParametersButton";
            this.ConfigParametersButton.Size = new System.Drawing.Size(401, 29);
            this.ConfigParametersButton.TabIndex = 3;
            this.ConfigParametersButton.Text = "Configuration Parameters";
            this.ConfigParametersButton.UseVisualStyleBackColor = true;
            this.ConfigParametersButton.Click += new System.EventHandler(this.ConfigParametersButton_Click);
            // 
            // NanoIHMSelecter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 182);
            this.Controls.Add(this.ConfigParametersButton);
            this.Controls.Add(this.LaunchManualButton);
            this.Controls.Add(this.LaunchProductionButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NanoIHMSelecter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NanoIHM ";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LaunchProductionButton;
        private System.Windows.Forms.Button LaunchManualButton;
        private System.Windows.Forms.Button ConfigParametersButton;
    }
}
