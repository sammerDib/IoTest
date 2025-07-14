namespace UnitySC.Rorze.Emulator.Controls
{
    partial class BitCheckControl
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
            this.numberLabel = new System.Windows.Forms.Label();
            this.chkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // numberLabel
            // 
            this.numberLabel.AutoSize = true;
            this.numberLabel.Location = new System.Drawing.Point(0, 1);
            this.numberLabel.Name = "numberLabel";
            this.numberLabel.Size = new System.Drawing.Size(15, 13);
            this.numberLabel.TabIndex = 55;
            this.numberLabel.Text = "N";
            // 
            // chkBox
            // 
            this.chkBox.AutoSize = true;
            this.chkBox.Location = new System.Drawing.Point(4, 19);
            this.chkBox.Name = "chkBox";
            this.chkBox.Size = new System.Drawing.Size(15, 14);
            this.chkBox.TabIndex = 54;
            this.chkBox.CheckedChanged += new System.EventHandler(this.ChkBox_CheckedChanged);
            // 
            // BitCheckControl
            // 
            this.Controls.Add(this.numberLabel);
            this.Controls.Add(this.chkBox);
            this.Name = "BitCheckControl";
            this.Size = new System.Drawing.Size(39, 42);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CheckBoxDown_Paint);
            this.SizeChanged += new System.EventHandler(this.CheckBoxDown_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label numberLabel;
        private System.Windows.Forms.CheckBox chkBox;
    }
}
