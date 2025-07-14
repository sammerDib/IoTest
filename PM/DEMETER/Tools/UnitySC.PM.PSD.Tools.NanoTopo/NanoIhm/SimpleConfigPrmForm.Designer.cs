namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    partial class SimpleConfigPrmForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleConfigPrmForm));
            this.checkBoxUseDiskPV = new System.Windows.Forms.CheckBox();
            this.groupBoxEE = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxThresh1 = new System.Windows.Forms.TextBox();
            this.textBoxThresh2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxSiteOffsetY = new System.Windows.Forms.TextBox();
            this.textBoxSiteOffsetX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSiteHeight = new System.Windows.Forms.TextBox();
            this.textBoxSiteWidth = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxSitePV = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxResumeLotEnabled = new System.Windows.Forms.CheckBox();
            this.groupBoxEE.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxUseDiskPV
            // 
            this.checkBoxUseDiskPV.AutoSize = true;
            this.checkBoxUseDiskPV.Location = new System.Drawing.Point(41, 27);
            this.checkBoxUseDiskPV.Name = "checkBoxUseDiskPV";
            this.checkBoxUseDiskPV.Size = new System.Drawing.Size(177, 17);
            this.checkBoxUseDiskPV.TabIndex = 0;
            this.checkBoxUseDiskPV.Text = "Use Disk PV (Square otherwise)";
            this.checkBoxUseDiskPV.UseVisualStyleBackColor = true;
            // 
            // groupBoxEE
            // 
            this.groupBoxEE.Controls.Add(this.radioButton3);
            this.groupBoxEE.Controls.Add(this.radioButton2);
            this.groupBoxEE.Controls.Add(this.radioButton1);
            this.groupBoxEE.Location = new System.Drawing.Point(12, 77);
            this.groupBoxEE.Name = "groupBoxEE";
            this.groupBoxEE.Size = new System.Drawing.Size(416, 61);
            this.groupBoxEE.TabIndex = 1;
            this.groupBoxEE.TabStop = false;
            this.groupBoxEE.Text = "Edge Exclusion";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(197, 28);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(50, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "3 mm";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(125, 28);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(50, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "2 mm";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(41, 28);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(59, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "1.5 mm";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxUseDiskPV);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 59);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PV Method";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(260, 335);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(348, 335);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.textBoxThresh1);
            this.groupBox2.Controls.Add(this.textBoxThresh2);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxSiteOffsetY);
            this.groupBox2.Controls.Add(this.textBoxSiteOffsetX);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxSiteHeight);
            this.groupBox2.Controls.Add(this.textBoxSiteWidth);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.checkBoxSitePV);
            this.groupBox2.Location = new System.Drawing.Point(18, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(410, 180);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Site PV Array ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(32, 119);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(103, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "PV Color Thresholds";
            // 
            // textBoxThresh1
            // 
            this.textBoxThresh1.Location = new System.Drawing.Point(84, 142);
            this.textBoxThresh1.Name = "textBoxThresh1";
            this.textBoxThresh1.Size = new System.Drawing.Size(28, 20);
            this.textBoxThresh1.TabIndex = 17;
            // 
            // textBoxThresh2
            // 
            this.textBoxThresh2.Location = new System.Drawing.Point(205, 143);
            this.textBoxThresh2.Name = "textBoxThresh2";
            this.textBoxThresh2.Size = new System.Drawing.Size(28, 20);
            this.textBoxThresh2.TabIndex = 16;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(239, 147);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "nm  <=";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(118, 146);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "nm  <=";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(186, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(13, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "<";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(65, 146);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "<";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Red;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(284, 145);
            this.label7.Name = "label7";
            this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label7.Size = new System.Drawing.Size(21, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "    ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Yellow;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(163, 146);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label6.Size = new System.Drawing.Size(21, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "    ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Lime;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(35, 145);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label5.Size = new System.Drawing.Size(21, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "    ";
            // 
            // textBoxSiteOffsetY
            // 
            this.textBoxSiteOffsetY.Location = new System.Drawing.Point(330, 87);
            this.textBoxSiteOffsetY.Name = "textBoxSiteOffsetY";
            this.textBoxSiteOffsetY.Size = new System.Drawing.Size(68, 20);
            this.textBoxSiteOffsetY.TabIndex = 8;
            // 
            // textBoxSiteOffsetX
            // 
            this.textBoxSiteOffsetX.Location = new System.Drawing.Point(330, 61);
            this.textBoxSiteOffsetX.Name = "textBoxSiteOffsetX";
            this.textBoxSiteOffsetX.Size = new System.Drawing.Size(68, 20);
            this.textBoxSiteOffsetX.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(226, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Site Offset Y (mm)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(226, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Site Offset X (mm)";
            // 
            // textBoxSiteHeight
            // 
            this.textBoxSiteHeight.Location = new System.Drawing.Point(136, 87);
            this.textBoxSiteHeight.Name = "textBoxSiteHeight";
            this.textBoxSiteHeight.Size = new System.Drawing.Size(68, 20);
            this.textBoxSiteHeight.TabIndex = 4;
            // 
            // textBoxSiteWidth
            // 
            this.textBoxSiteWidth.Location = new System.Drawing.Point(136, 61);
            this.textBoxSiteWidth.Name = "textBoxSiteWidth";
            this.textBoxSiteWidth.Size = new System.Drawing.Size(68, 20);
            this.textBoxSiteWidth.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Site Height (mm)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Site Width (mm)";
            // 
            // checkBoxSitePV
            // 
            this.checkBoxSitePV.AutoSize = true;
            this.checkBoxSitePV.Location = new System.Drawing.Point(35, 28);
            this.checkBoxSitePV.Name = "checkBoxSitePV";
            this.checkBoxSitePV.Size = new System.Drawing.Size(59, 17);
            this.checkBoxSitePV.TabIndex = 0;
            this.checkBoxSitePV.Text = "Enable";
            this.checkBoxSitePV.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxResumeLotEnabled);
            this.groupBox3.Location = new System.Drawing.Point(238, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(190, 59);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Lot Resume File generation";
            // 
            // checkBoxResumeLotEnabled
            // 
            this.checkBoxResumeLotEnabled.AutoSize = true;
            this.checkBoxResumeLotEnabled.Location = new System.Drawing.Point(41, 27);
            this.checkBoxResumeLotEnabled.Name = "checkBoxResumeLotEnabled";
            this.checkBoxResumeLotEnabled.Size = new System.Drawing.Size(59, 17);
            this.checkBoxResumeLotEnabled.TabIndex = 0;
            this.checkBoxResumeLotEnabled.Text = "Enable";
            this.checkBoxResumeLotEnabled.UseVisualStyleBackColor = true;
            // 
            // SimpleConfigPrmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 367);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxEE);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SimpleConfigPrmForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NanoTopography Configuration";
            this.groupBoxEE.ResumeLayout(false);
            this.groupBoxEE.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxUseDiskPV;
        private System.Windows.Forms.GroupBox groupBoxEE;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxSitePV;
        private System.Windows.Forms.TextBox textBoxSiteOffsetY;
        private System.Windows.Forms.TextBox textBoxSiteOffsetX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSiteHeight;
        private System.Windows.Forms.TextBox textBoxSiteWidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxThresh1;
        private System.Windows.Forms.TextBox textBoxThresh2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxResumeLotEnabled;
    }
}
