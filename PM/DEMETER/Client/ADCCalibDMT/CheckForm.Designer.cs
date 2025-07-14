namespace ADCCalibDMT
{
    partial class CheckForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckForm));
            this.tableLayoutPanelViewer = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxViewerStatus = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonExportView = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPixelSize = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxImgCoord = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxWaferCoord = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCalibImage = new System.Windows.Forms.TextBox();
            this.buttonCalibImgBrwse = new System.Windows.Forms.Button();
            this.textBoxCalibfilePath = new System.Windows.Forms.TextBox();
            this.buttonBrwseWaferCalibXML = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.saveFileDialogExportView = new System.Windows.Forms.SaveFileDialog();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxImageSize = new System.Windows.Forms.TextBox();
            this.checkBoxShowCalibPoints = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelViewer.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanelViewer
            // 
            this.tableLayoutPanelViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelViewer.AutoSize = true;
            this.tableLayoutPanelViewer.ColumnCount = 2;
            this.tableLayoutPanelViewer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelViewer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanelViewer.Controls.Add(this.textBoxViewerStatus, 0, 4);
            this.tableLayoutPanelViewer.Controls.Add(this.flowLayoutPanel2, 1, 2);
            this.tableLayoutPanelViewer.Controls.Add(this.flowLayoutPanel1, 1, 3);
            this.tableLayoutPanelViewer.Controls.Add(this.checkBoxShowCalibPoints, 1, 1);
            this.tableLayoutPanelViewer.Location = new System.Drawing.Point(12, 73);
            this.tableLayoutPanelViewer.Name = "tableLayoutPanelViewer";
            this.tableLayoutPanelViewer.RowCount = 5;
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanelViewer.Size = new System.Drawing.Size(850, 530);
            this.tableLayoutPanelViewer.TabIndex = 14;
            // 
            // textBoxViewerStatus
            // 
            this.textBoxViewerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxViewerStatus.Location = new System.Drawing.Point(3, 511);
            this.textBoxViewerStatus.Name = "textBoxViewerStatus";
            this.textBoxViewerStatus.ReadOnly = true;
            this.textBoxViewerStatus.Size = new System.Drawing.Size(709, 20);
            this.textBoxViewerStatus.TabIndex = 16;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.buttonExportView);
            this.flowLayoutPanel2.Controls.Add(this.label3);
            this.flowLayoutPanel2.Controls.Add(this.textBoxPixelSize);
            this.flowLayoutPanel2.Controls.Add(this.label11);
            this.flowLayoutPanel2.Controls.Add(this.textBoxImageSize);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(718, 129);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(129, 114);
            this.flowLayoutPanel2.TabIndex = 29;
            // 
            // buttonExportView
            // 
            this.buttonExportView.Location = new System.Drawing.Point(3, 3);
            this.buttonExportView.Name = "buttonExportView";
            this.buttonExportView.Size = new System.Drawing.Size(105, 23);
            this.buttonExportView.TabIndex = 27;
            this.buttonExportView.Text = "Export View";
            this.buttonExportView.UseVisualStyleBackColor = true;
            this.buttonExportView.Click += new System.EventHandler(this.buttonExportView_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "ADC Matrix Pixel Size";
            // 
            // textBoxPixelSize
            // 
            this.textBoxPixelSize.Location = new System.Drawing.Point(3, 45);
            this.textBoxPixelSize.Name = "textBoxPixelSize";
            this.textBoxPixelSize.ReadOnly = true;
            this.textBoxPixelSize.Size = new System.Drawing.Size(114, 20);
            this.textBoxPixelSize.TabIndex = 25;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.label9);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.textBoxImgCoord);
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.label7);
            this.flowLayoutPanel1.Controls.Add(this.label8);
            this.flowLayoutPanel1.Controls.Add(this.textBoxWaferCoord);
            this.flowLayoutPanel1.Controls.Add(this.label10);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(718, 249);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(129, 218);
            this.flowLayoutPanel1.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 15);
            this.label4.TabIndex = 30;
            this.label4.Text = "Cross Coordinates :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "                                       ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Image";
            // 
            // textBoxImgCoord
            // 
            this.textBoxImgCoord.Location = new System.Drawing.Point(3, 44);
            this.textBoxImgCoord.Name = "textBoxImgCoord";
            this.textBoxImgCoord.ReadOnly = true;
            this.textBoxImgCoord.Size = new System.Drawing.Size(126, 20);
            this.textBoxImgCoord.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "In pixels";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 26);
            this.label7.TabIndex = 34;
            this.label7.Text = "                                                                           ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "Wafer";
            // 
            // textBoxWaferCoord
            // 
            this.textBoxWaferCoord.Location = new System.Drawing.Point(3, 122);
            this.textBoxWaferCoord.Name = "textBoxWaferCoord";
            this.textBoxWaferCoord.ReadOnly = true;
            this.textBoxWaferCoord.Size = new System.Drawing.Size(126, 20);
            this.textBoxWaferCoord.TabIndex = 37;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 145);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 38;
            this.label10.Text = "In mm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Test Image";
            // 
            // textBoxCalibImage
            // 
            this.textBoxCalibImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCalibImage.Location = new System.Drawing.Point(110, 16);
            this.textBoxCalibImage.Name = "textBoxCalibImage";
            this.textBoxCalibImage.ReadOnly = true;
            this.textBoxCalibImage.Size = new System.Drawing.Size(497, 20);
            this.textBoxCalibImage.TabIndex = 18;
            // 
            // buttonCalibImgBrwse
            // 
            this.buttonCalibImgBrwse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCalibImgBrwse.Location = new System.Drawing.Point(613, 14);
            this.buttonCalibImgBrwse.Name = "buttonCalibImgBrwse";
            this.buttonCalibImgBrwse.Size = new System.Drawing.Size(33, 23);
            this.buttonCalibImgBrwse.TabIndex = 20;
            this.buttonCalibImgBrwse.Text = "...";
            this.buttonCalibImgBrwse.UseVisualStyleBackColor = true;
            this.buttonCalibImgBrwse.Click += new System.EventHandler(this.buttonCalibImgBrwse_Click);
            // 
            // textBoxCalibfilePath
            // 
            this.textBoxCalibfilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCalibfilePath.Location = new System.Drawing.Point(110, 46);
            this.textBoxCalibfilePath.Name = "textBoxCalibfilePath";
            this.textBoxCalibfilePath.ReadOnly = true;
            this.textBoxCalibfilePath.Size = new System.Drawing.Size(497, 20);
            this.textBoxCalibfilePath.TabIndex = 23;
            // 
            // buttonBrwseWaferCalibXML
            // 
            this.buttonBrwseWaferCalibXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrwseWaferCalibXML.Location = new System.Drawing.Point(613, 43);
            this.buttonBrwseWaferCalibXML.Name = "buttonBrwseWaferCalibXML";
            this.buttonBrwseWaferCalibXML.Size = new System.Drawing.Size(33, 23);
            this.buttonBrwseWaferCalibXML.TabIndex = 23;
            this.buttonBrwseWaferCalibXML.Text = "...";
            this.buttonBrwseWaferCalibXML.UseVisualStyleBackColor = true;
            this.buttonBrwseWaferCalibXML.Click += new System.EventHandler(this.buttonBrwseWaferCalibXML_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "PSD Transform file ";
            // 
            // saveFileDialogExportView
            // 
            this.saveFileDialogExportView.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogExportView_FileOk);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(654, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(208, 54);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 24;
            this.pictureBox2.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 68);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "Image Size";
            // 
            // textBoxImageSize
            // 
            this.textBoxImageSize.Location = new System.Drawing.Point(3, 84);
            this.textBoxImageSize.Name = "textBoxImageSize";
            this.textBoxImageSize.ReadOnly = true;
            this.textBoxImageSize.Size = new System.Drawing.Size(114, 20);
            this.textBoxImageSize.TabIndex = 29;
            // 
            // checkBoxShowCalibPoints
            // 
            this.checkBoxShowCalibPoints.AutoSize = true;
            this.checkBoxShowCalibPoints.Location = new System.Drawing.Point(718, 103);
            this.checkBoxShowCalibPoints.Name = "checkBoxShowCalibPoints";
            this.checkBoxShowCalibPoints.Size = new System.Drawing.Size(108, 17);
            this.checkBoxShowCalibPoints.TabIndex = 31;
            this.checkBoxShowCalibPoints.Text = "Show Test points";
            this.checkBoxShowCalibPoints.UseVisualStyleBackColor = true;
            this.checkBoxShowCalibPoints.CheckedChanged += new System.EventHandler(this.checkBoxShowCalibPoints_CheckedChanged);
            // 
            // CheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 615);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.buttonBrwseWaferCalibXML);
            this.Controls.Add(this.textBoxCalibfilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCalibImgBrwse);
            this.Controls.Add(this.textBoxCalibImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tableLayoutPanelViewer);
            this.Name = "CheckForm";
            this.Text = "DEMETER Check Calibration Tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CheckForm_FormClosed);
            this.Load += new System.EventHandler(this.CheckForm_Load);
            this.tableLayoutPanelViewer.ResumeLayout(false);
            this.tableLayoutPanelViewer.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelViewer;
        private System.Windows.Forms.TextBox textBoxViewerStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCalibImage;
        private System.Windows.Forms.Button buttonCalibImgBrwse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCalibfilePath;
        private System.Windows.Forms.Button buttonBrwseWaferCalibXML;
        private System.Windows.Forms.Button buttonExportView;
        private System.Windows.Forms.SaveFileDialog saveFileDialogExportView;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPixelSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxImgCoord;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxWaferCoord;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxImageSize;
        private System.Windows.Forms.CheckBox checkBoxShowCalibPoints;
    }
}

