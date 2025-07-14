
namespace UnitySC.Rorze.Emulator.Controls.Dio0
{
    partial class Dio0LayingPlanLoadPortControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbWaferSize = new System.Windows.Forms.ComboBox();
            this.foupRemovedButton = new System.Windows.Forms.Button();
            this.foupPresentPlacedButton = new System.Windows.Forms.Button();
            this.cbProtrusionError = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbProtrusionError);
            this.groupBox1.Controls.Add(this.cbWaferSize);
            this.groupBox1.Controls.Add(this.foupRemovedButton);
            this.groupBox1.Controls.Add(this.foupPresentPlacedButton);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 222);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 75);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Carrier";
            // 
            // cbWaferSize
            // 
            this.cbWaferSize.FormattingEnabled = true;
            this.cbWaferSize.Items.AddRange(new object[] {
            "200mm",
            "150mm",
            "100mm"});
            this.cbWaferSize.Location = new System.Drawing.Point(277, 19);
            this.cbWaferSize.Name = "cbWaferSize";
            this.cbWaferSize.Size = new System.Drawing.Size(121, 21);
            this.cbWaferSize.TabIndex = 34;
            // 
            // foupRemovedButton
            // 
            this.foupRemovedButton.Location = new System.Drawing.Point(140, 19);
            this.foupRemovedButton.Name = "foupRemovedButton";
            this.foupRemovedButton.Size = new System.Drawing.Size(121, 23);
            this.foupRemovedButton.TabIndex = 33;
            this.foupRemovedButton.Text = "Remove Carrier";
            this.foupRemovedButton.UseVisualStyleBackColor = true;
            this.foupRemovedButton.Click += new System.EventHandler(this.foupRemovedButton_Click);
            // 
            // foupPresentPlacedButton
            // 
            this.foupPresentPlacedButton.Location = new System.Drawing.Point(6, 19);
            this.foupPresentPlacedButton.Name = "foupPresentPlacedButton";
            this.foupPresentPlacedButton.Size = new System.Drawing.Size(121, 23);
            this.foupPresentPlacedButton.TabIndex = 30;
            this.foupPresentPlacedButton.Text = "Place Carrier";
            this.foupPresentPlacedButton.UseVisualStyleBackColor = true;
            this.foupPresentPlacedButton.Click += new System.EventHandler(this.foupPresentPlacedButton_Click);
            // 
            // cbProtrusionError
            // 
            this.cbProtrusionError.AutoSize = true;
            this.cbProtrusionError.Location = new System.Drawing.Point(7, 49);
            this.cbProtrusionError.Name = "cbProtrusionError";
            this.cbProtrusionError.Size = new System.Drawing.Size(123, 17);
            this.cbProtrusionError.TabIndex = 35;
            this.cbProtrusionError.Text = "With Protrusion Error";
            this.cbProtrusionError.UseVisualStyleBackColor = true;
            // 
            // Dio0LayingPlanLoadPortControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "Dio0LayingPlanLoadPortControl";
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button foupRemovedButton;
        private System.Windows.Forms.Button foupPresentPlacedButton;
        private System.Windows.Forms.ComboBox cbWaferSize;
        private System.Windows.Forms.CheckBox cbProtrusionError;
    }
}
