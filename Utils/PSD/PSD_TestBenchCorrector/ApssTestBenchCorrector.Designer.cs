namespace AppsTestBenchCorrector
{
    partial class ApssTestBenchCorrector
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.BTSelectPicture = new System.Windows.Forms.Button();
            this.TBSelectedFile = new System.Windows.Forms.TextBox();
            this.TBWaferSize = new System.Windows.Forms.TextBox();
            this.TBAngleValue = new System.Windows.Forms.ComboBox();
            this.TBCorrectionX = new System.Windows.Forms.TextBox();
            this.TBCorrectionY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TBThetaCorrection = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BTProcess = new System.Windows.Forms.Button();
            this.PBSummaryView = new System.Windows.Forms.PictureBox();
            this.BTDetail = new System.Windows.Forms.Button();
            this.TBPixelSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PBSummaryView)).BeginInit();
            this.SuspendLayout();
            // 
            // BTSelectPicture
            // 
            this.BTSelectPicture.Location = new System.Drawing.Point(12, 12);
            this.BTSelectPicture.Name = "BTSelectPicture";
            this.BTSelectPicture.Size = new System.Drawing.Size(214, 23);
            this.BTSelectPicture.TabIndex = 0;
            this.BTSelectPicture.Text = "Select Picture";
            this.BTSelectPicture.UseVisualStyleBackColor = true;
            this.BTSelectPicture.Click += new System.EventHandler(this.BTSelectPicture_Click);
            // 
            // TBSelectedFile
            // 
            this.TBSelectedFile.Location = new System.Drawing.Point(232, 14);
            this.TBSelectedFile.Name = "TBSelectedFile";
            this.TBSelectedFile.Size = new System.Drawing.Size(524, 20);
            this.TBSelectedFile.TabIndex = 1;
            // 
            // TBWaferSize
            // 
            this.TBWaferSize.Location = new System.Drawing.Point(232, 50);
            this.TBWaferSize.Name = "TBWaferSize";
            this.TBWaferSize.Size = new System.Drawing.Size(100, 20);
            this.TBWaferSize.TabIndex = 2;
            this.TBWaferSize.Text = "300";
            // 
            // TBAngleValue
            // 
            this.TBAngleValue.Location = new System.Drawing.Point(232, 76);
            this.TBAngleValue.Name = "TBAngleValue";
            this.TBAngleValue.Size = new System.Drawing.Size(100, 20);
            this.TBAngleValue.TabIndex = 3;
            this.TBAngleValue.Items.AddRange(new object[] { "Bottom", "Top", "Left", "Right" });
            this.TBAngleValue.SelectedIndex = 0;
            //
            // TBPositionCorrectionX
            // 
            this.TBCorrectionX.Location = new System.Drawing.Point(232, 193);
            this.TBCorrectionX.Name = "TBCorrectionX";
            this.TBCorrectionX.Size = new System.Drawing.Size(100, 20);
            this.TBCorrectionX.TabIndex = 4;
            // 
            // TBPositionCorrectionY
            // 
            this.TBCorrectionY.Location = new System.Drawing.Point(232, 219);
            this.TBCorrectionY.Name = "TBCorrectionY";
            this.TBCorrectionY.Size = new System.Drawing.Size(100, 20);
            this.TBCorrectionY.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Wafer Size (mm)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Notch location";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 196);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "X correction (µ)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 222);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Y correction (µ)";
            // 
            // TBThetaCorrection
            // 
            this.TBThetaCorrection.Location = new System.Drawing.Point(232, 245);
            this.TBThetaCorrection.Name = "TBThetaCorrection";
            this.TBThetaCorrection.Size = new System.Drawing.Size(100, 20);
            this.TBThetaCorrection.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 248);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Theta Correction (radian)";
            // 
            // BTProcess
            // 
            this.BTProcess.Location = new System.Drawing.Point(12, 145);
            this.BTProcess.Name = "BTProcess";
            this.BTProcess.Size = new System.Drawing.Size(214, 23);
            this.BTProcess.TabIndex = 12;
            this.BTProcess.Text = "Process";
            this.BTProcess.UseVisualStyleBackColor = true;
            this.BTProcess.Click += new System.EventHandler(this.BTProcess_Click);
            // 
            // PBSummaryView
            // 
            this.PBSummaryView.Location = new System.Drawing.Point(365, 50);
            this.PBSummaryView.Name = "PBSummaryView";
            this.PBSummaryView.Size = new System.Drawing.Size(391, 388);
            this.PBSummaryView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PBSummaryView.TabIndex = 13;
            this.PBSummaryView.TabStop = false;
            // 
            // BTDetail
            // 
            this.BTDetail.Location = new System.Drawing.Point(232, 415);
            this.BTDetail.Name = "BTDetail";
            this.BTDetail.Size = new System.Drawing.Size(127, 23);
            this.BTDetail.TabIndex = 14;
            this.BTDetail.Text = "Full view";
            this.BTDetail.UseVisualStyleBackColor = true;
            this.BTDetail.Click += new System.EventHandler(this.BTDetail_Click);
            // 
            // TBPixelSize
            // 
            this.TBPixelSize.Location = new System.Drawing.Point(232, 102);
            this.TBPixelSize.Name = "TBPixelSize";
            this.TBPixelSize.Size = new System.Drawing.Size(100, 20);
            this.TBPixelSize.TabIndex = 15;
            this.TBPixelSize.Text = "100";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Pixel Size (µ)";
            // 
            // ApssTestBenchCorrector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TBPixelSize);
            this.Controls.Add(this.BTDetail);
            this.Controls.Add(this.PBSummaryView);
            this.Controls.Add(this.BTProcess);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TBThetaCorrection);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TBCorrectionY);
            this.Controls.Add(this.TBCorrectionX);
            this.Controls.Add(this.TBAngleValue);
            this.Controls.Add(this.TBWaferSize);
            this.Controls.Add(this.TBSelectedFile);
            this.Controls.Add(this.BTSelectPicture);
            this.Name = "ApssTestBenchCorrector";
            this.Text = "AppsTestBenchCorrector";
            ((System.ComponentModel.ISupportInitialize)(this.PBSummaryView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTSelectPicture;
        private System.Windows.Forms.TextBox TBSelectedFile;
        private System.Windows.Forms.TextBox TBWaferSize;
        private System.Windows.Forms.ComboBox TBAngleValue;
        private System.Windows.Forms.TextBox TBCorrectionX;
        private System.Windows.Forms.TextBox TBCorrectionY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TBThetaCorrection;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BTProcess;
        private System.Windows.Forms.PictureBox PBSummaryView;
        private System.Windows.Forms.Button BTDetail;
        private System.Windows.Forms.TextBox TBPixelSize;
        private System.Windows.Forms.Label label6;
    }
}

