namespace ADCCalibDMT
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanelViewer = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxSelectDefectMode = new System.Windows.Forms.CheckBox();
            this.textBoxViewerStatus = new System.Windows.Forms.TextBox();
            this.textBoxDisplayInfo = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonValid = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCalibImage = new System.Windows.Forms.TextBox();
            this.buttonCalibImgBrwse = new System.Windows.Forms.Button();
            this.textBoxWaferCalibXmlPath = new System.Windows.Forms.TextBox();
            this.buttonBrwseWaferCalibXML = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBoxMarginLeft = new System.Windows.Forms.TextBox();
            this.textBoxMarginRight = new System.Windows.Forms.TextBox();
            this.textBoxMarginBottom = new System.Windows.Forms.TextBox();
            this.textBoxMarginTop = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMireSize = new System.Windows.Forms.TextBox();
            this.textBoxSearchAreaMireSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonAutoSearchPoints = new System.Windows.Forms.Button();
            this.buttonExportView = new System.Windows.Forms.Button();
            this.buttonCancelProcess = new System.Windows.Forms.Button();
            this.buttonCheckCalib = new System.Windows.Forms.Button();
            this.buttonCalibration = new System.Windows.Forms.Button();
            this.buttonRemovePoint = new System.Windows.Forms.Button();
            this.buttonSearchPoints = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.saveFileDialogExportView = new System.Windows.Forms.SaveFileDialog();
            this.checkBoxReportCalibDebugImage = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelViewer.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl.SuspendLayout();
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
            this.tableLayoutPanelViewer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelViewer.Controls.Add(this.checkBoxSelectDefectMode, 1, 1);
            this.tableLayoutPanelViewer.Controls.Add(this.textBoxViewerStatus, 0, 4);
            this.tableLayoutPanelViewer.Controls.Add(this.textBoxDisplayInfo, 1, 3);
            this.tableLayoutPanelViewer.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanelViewer.Location = new System.Drawing.Point(12, 77);
            this.tableLayoutPanelViewer.Name = "tableLayoutPanelViewer";
            this.tableLayoutPanelViewer.RowCount = 5;
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 171F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelViewer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanelViewer.Size = new System.Drawing.Size(656, 526);
            this.tableLayoutPanelViewer.TabIndex = 14;
            // 
            // checkBoxSelectDefectMode
            // 
            this.checkBoxSelectDefectMode.AutoSize = true;
            this.checkBoxSelectDefectMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxSelectDefectMode.Location = new System.Drawing.Point(524, 174);
            this.checkBoxSelectDefectMode.Name = "checkBoxSelectDefectMode";
            this.checkBoxSelectDefectMode.Size = new System.Drawing.Size(129, 20);
            this.checkBoxSelectDefectMode.TabIndex = 25;
            this.checkBoxSelectDefectMode.Text = "Mouse Selection";
            this.checkBoxSelectDefectMode.UseVisualStyleBackColor = true;
            // 
            // textBoxViewerStatus
            // 
            this.textBoxViewerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxViewerStatus.Location = new System.Drawing.Point(3, 507);
            this.textBoxViewerStatus.Name = "textBoxViewerStatus";
            this.textBoxViewerStatus.ReadOnly = true;
            this.textBoxViewerStatus.Size = new System.Drawing.Size(515, 20);
            this.textBoxViewerStatus.TabIndex = 16;
            // 
            // textBoxDisplayInfo
            // 
            this.textBoxDisplayInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDisplayInfo.Location = new System.Drawing.Point(524, 287);
            this.textBoxDisplayInfo.Multiline = true;
            this.textBoxDisplayInfo.Name = "textBoxDisplayInfo";
            this.textBoxDisplayInfo.ReadOnly = true;
            this.textBoxDisplayInfo.Size = new System.Drawing.Size(129, 214);
            this.textBoxDisplayInfo.TabIndex = 27;
            this.textBoxDisplayInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonValid);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(524, 200);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(129, 81);
            this.flowLayoutPanel1.TabIndex = 28;
            // 
            // buttonValid
            // 
            this.buttonValid.Enabled = false;
            this.buttonValid.Location = new System.Drawing.Point(3, 3);
            this.buttonValid.Name = "buttonValid";
            this.buttonValid.Size = new System.Drawing.Size(126, 42);
            this.buttonValid.TabIndex = 26;
            this.buttonValid.Text = "Valid Click";
            this.buttonValid.UseVisualStyleBackColor = true;
            this.buttonValid.Click += new System.EventHandler(this.buttonValid_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(800, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(208, 54);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Calibration Image";
            // 
            // textBoxCalibImage
            // 
            this.textBoxCalibImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCalibImage.Location = new System.Drawing.Point(110, 16);
            this.textBoxCalibImage.Name = "textBoxCalibImage";
            this.textBoxCalibImage.Size = new System.Drawing.Size(519, 20);
            this.textBoxCalibImage.TabIndex = 18;
            // 
            // buttonCalibImgBrwse
            // 
            this.buttonCalibImgBrwse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCalibImgBrwse.Location = new System.Drawing.Point(635, 14);
            this.buttonCalibImgBrwse.Name = "buttonCalibImgBrwse";
            this.buttonCalibImgBrwse.Size = new System.Drawing.Size(33, 23);
            this.buttonCalibImgBrwse.TabIndex = 20;
            this.buttonCalibImgBrwse.Text = "...";
            this.buttonCalibImgBrwse.UseVisualStyleBackColor = true;
            this.buttonCalibImgBrwse.Click += new System.EventHandler(this.buttonCalibImgBrwse_Click);
            // 
            // textBoxWaferCalibXmlPath
            // 
            this.textBoxWaferCalibXmlPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWaferCalibXmlPath.Location = new System.Drawing.Point(136, 50);
            this.textBoxWaferCalibXmlPath.Name = "textBoxWaferCalibXmlPath";
            this.textBoxWaferCalibXmlPath.Size = new System.Drawing.Size(493, 20);
            this.textBoxWaferCalibXmlPath.TabIndex = 23;
            // 
            // buttonBrwseWaferCalibXML
            // 
            this.buttonBrwseWaferCalibXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrwseWaferCalibXML.Location = new System.Drawing.Point(635, 48);
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
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Calibration wafer xml file";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textBoxMarginLeft);
            this.tabPage3.Controls.Add(this.textBoxMarginRight);
            this.tabPage3.Controls.Add(this.textBoxMarginBottom);
            this.tabPage3.Controls.Add(this.textBoxMarginTop);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.textBoxMireSize);
            this.tabPage3.Controls.Add(this.textBoxSearchAreaMireSize);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(326, 506);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "Advanced Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textBoxMarginLeft
            // 
            this.textBoxMarginLeft.Location = new System.Drawing.Point(189, 188);
            this.textBoxMarginLeft.Name = "textBoxMarginLeft";
            this.textBoxMarginLeft.Size = new System.Drawing.Size(100, 20);
            this.textBoxMarginLeft.TabIndex = 11;
            this.textBoxMarginLeft.Text = "5000";
            this.textBoxMarginLeft.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchAdv_KeyPress);
            // 
            // textBoxMarginRight
            // 
            this.textBoxMarginRight.Location = new System.Drawing.Point(189, 161);
            this.textBoxMarginRight.Name = "textBoxMarginRight";
            this.textBoxMarginRight.Size = new System.Drawing.Size(100, 20);
            this.textBoxMarginRight.TabIndex = 10;
            this.textBoxMarginRight.Text = "5000";
            this.textBoxMarginRight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchAdv_KeyPress);
            // 
            // textBoxMarginBottom
            // 
            this.textBoxMarginBottom.Location = new System.Drawing.Point(189, 135);
            this.textBoxMarginBottom.Name = "textBoxMarginBottom";
            this.textBoxMarginBottom.Size = new System.Drawing.Size(100, 20);
            this.textBoxMarginBottom.TabIndex = 9;
            this.textBoxMarginBottom.Text = "5000";
            this.textBoxMarginBottom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchAdv_KeyPress);
            // 
            // textBoxMarginTop
            // 
            this.textBoxMarginTop.Location = new System.Drawing.Point(189, 108);
            this.textBoxMarginTop.Name = "textBoxMarginTop";
            this.textBoxMarginTop.Size = new System.Drawing.Size(100, 20);
            this.textBoxMarginTop.TabIndex = 8;
            this.textBoxMarginTop.Text = "5000";
            this.textBoxMarginTop.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchAdv_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 191);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "MarginLeft (µm)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 164);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Margin Right (µm)";
            // 
            // textBoxMireSize
            // 
            this.textBoxMireSize.Location = new System.Drawing.Point(189, 50);
            this.textBoxMireSize.Name = "textBoxMireSize";
            this.textBoxMireSize.Size = new System.Drawing.Size(100, 20);
            this.textBoxMireSize.TabIndex = 5;
            this.textBoxMireSize.Text = "50";
            this.textBoxMireSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchAdv_KeyPress);
            // 
            // textBoxSearchAreaMireSize
            // 
            this.textBoxSearchAreaMireSize.Location = new System.Drawing.Point(189, 22);
            this.textBoxSearchAreaMireSize.Name = "textBoxSearchAreaMireSize";
            this.textBoxSearchAreaMireSize.Size = new System.Drawing.Size(100, 20);
            this.textBoxSearchAreaMireSize.TabIndex = 4;
            this.textBoxSearchAreaMireSize.Text = "400";
            this.textBoxSearchAreaMireSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSearchAdv_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Pts Search Area (px)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Specific Pts Search Area (px)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Margin Bottom (µm)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Margin top (µm)";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.checkBoxReportCalibDebugImage);
            this.tabPage1.Controls.Add(this.buttonAutoSearchPoints);
            this.tabPage1.Controls.Add(this.buttonExportView);
            this.tabPage1.Controls.Add(this.buttonCancelProcess);
            this.tabPage1.Controls.Add(this.buttonCheckCalib);
            this.tabPage1.Controls.Add(this.buttonCalibration);
            this.tabPage1.Controls.Add(this.buttonRemovePoint);
            this.tabPage1.Controls.Add(this.buttonSearchPoints);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(326, 506);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Search Points";
            // 
            // buttonAutoSearchPoints
            // 
            this.buttonAutoSearchPoints.Location = new System.Drawing.Point(44, 21);
            this.buttonAutoSearchPoints.Name = "buttonAutoSearchPoints";
            this.buttonAutoSearchPoints.Size = new System.Drawing.Size(233, 50);
            this.buttonAutoSearchPoints.TabIndex = 28;
            this.buttonAutoSearchPoints.Text = "Auto Search Points";
            this.buttonAutoSearchPoints.UseVisualStyleBackColor = true;
            this.buttonAutoSearchPoints.Click += new System.EventHandler(this.buttonAutoSearchPoints_Click);
            // 
            // buttonExportView
            // 
            this.buttonExportView.Location = new System.Drawing.Point(107, 461);
            this.buttonExportView.Name = "buttonExportView";
            this.buttonExportView.Size = new System.Drawing.Size(105, 23);
            this.buttonExportView.TabIndex = 27;
            this.buttonExportView.Text = "Export View";
            this.buttonExportView.UseVisualStyleBackColor = true;
            this.buttonExportView.Click += new System.EventHandler(this.buttonExportView_Click);
            // 
            // buttonCancelProcess
            // 
            this.buttonCancelProcess.Location = new System.Drawing.Point(97, 144);
            this.buttonCancelProcess.Name = "buttonCancelProcess";
            this.buttonCancelProcess.Size = new System.Drawing.Size(115, 29);
            this.buttonCancelProcess.TabIndex = 6;
            this.buttonCancelProcess.Text = "Cancel Process";
            this.buttonCancelProcess.UseVisualStyleBackColor = true;
            this.buttonCancelProcess.Click += new System.EventHandler(this.buttonCancelProcess_Click);
            // 
            // buttonCheckCalib
            // 
            this.buttonCheckCalib.Location = new System.Drawing.Point(44, 375);
            this.buttonCheckCalib.Name = "buttonCheckCalib";
            this.buttonCheckCalib.Size = new System.Drawing.Size(233, 50);
            this.buttonCheckCalib.TabIndex = 4;
            this.buttonCheckCalib.Text = "Calibration Check";
            this.buttonCheckCalib.UseVisualStyleBackColor = true;
            this.buttonCheckCalib.Click += new System.EventHandler(this.buttonCheckCalib_Click);
            // 
            // buttonCalibration
            // 
            this.buttonCalibration.Enabled = false;
            this.buttonCalibration.Location = new System.Drawing.Point(44, 261);
            this.buttonCalibration.Name = "buttonCalibration";
            this.buttonCalibration.Size = new System.Drawing.Size(233, 50);
            this.buttonCalibration.TabIndex = 3;
            this.buttonCalibration.Text = "Calibration ";
            this.buttonCalibration.UseVisualStyleBackColor = true;
            this.buttonCalibration.Click += new System.EventHandler(this.buttonCalibration_Click);
            // 
            // buttonRemovePoint
            // 
            this.buttonRemovePoint.Enabled = false;
            this.buttonRemovePoint.Location = new System.Drawing.Point(97, 179);
            this.buttonRemovePoint.Name = "buttonRemovePoint";
            this.buttonRemovePoint.Size = new System.Drawing.Size(115, 29);
            this.buttonRemovePoint.TabIndex = 2;
            this.buttonRemovePoint.Text = "Remove Point";
            this.buttonRemovePoint.UseVisualStyleBackColor = true;
            this.buttonRemovePoint.Click += new System.EventHandler(this.buttonRemovePoint_Click);
            // 
            // buttonSearchPoints
            // 
            this.buttonSearchPoints.Location = new System.Drawing.Point(44, 77);
            this.buttonSearchPoints.Name = "buttonSearchPoints";
            this.buttonSearchPoints.Size = new System.Drawing.Size(233, 50);
            this.buttonSearchPoints.TabIndex = 0;
            this.buttonSearchPoints.Text = "Manual Search Points";
            this.buttonSearchPoints.UseVisualStyleBackColor = true;
            this.buttonSearchPoints.Click += new System.EventHandler(this.buttonSearchPoints_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Location = new System.Drawing.Point(674, 72);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(334, 532);
            this.tabControl.TabIndex = 22;
            // 
            // saveFileDialogExportView
            // 
            this.saveFileDialogExportView.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogExportView_FileOk);
            // 
            // checkBoxReportCalibDebugImage
            // 
            this.checkBoxReportCalibDebugImage.AutoSize = true;
            this.checkBoxReportCalibDebugImage.Location = new System.Drawing.Point(95, 317);
            this.checkBoxReportCalibDebugImage.Name = "checkBoxReportCalibDebugImage";
            this.checkBoxReportCalibDebugImage.Size = new System.Drawing.Size(182, 17);
            this.checkBoxReportCalibDebugImage.TabIndex = 29;
            this.checkBoxReportCalibDebugImage.Text = "Report Debug Calibration Images";
            this.checkBoxReportCalibDebugImage.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 615);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonBrwseWaferCalibXML);
            this.Controls.Add(this.textBoxWaferCalibXmlPath);
            this.Controls.Add(this.buttonCalibImgBrwse);
            this.Controls.Add(this.textBoxCalibImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tableLayoutPanelViewer);
            this.Name = "MainForm";
            this.Text = "DEMETER Module Calibration Tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tableLayoutPanelViewer.ResumeLayout(false);
            this.tableLayoutPanelViewer.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelViewer;
        private System.Windows.Forms.CheckBox checkBoxSelectDefectMode;
        private System.Windows.Forms.TextBox textBoxViewerStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCalibImage;
        private System.Windows.Forms.Button buttonCalibImgBrwse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxWaferCalibXmlPath;
        private System.Windows.Forms.Button buttonBrwseWaferCalibXML;
        private System.Windows.Forms.Button buttonValid;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button buttonCheckCalib;
        private System.Windows.Forms.Button buttonCalibration;
        private System.Windows.Forms.Button buttonRemovePoint;
        private System.Windows.Forms.Button buttonSearchPoints;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TextBox textBoxDisplayInfo;
        private System.Windows.Forms.TextBox textBoxSearchAreaMireSize;
        private System.Windows.Forms.TextBox textBoxMireSize;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonCancelProcess;
        private System.Windows.Forms.Button buttonExportView;
        private System.Windows.Forms.SaveFileDialog saveFileDialogExportView;
        private System.Windows.Forms.TextBox textBoxMarginLeft;
        private System.Windows.Forms.TextBox textBoxMarginRight;
        private System.Windows.Forms.TextBox textBoxMarginBottom;
        private System.Windows.Forms.TextBox textBoxMarginTop;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonAutoSearchPoints;
        private System.Windows.Forms.CheckBox checkBoxReportCalibDebugImage;
    }
}

