namespace MonitorTaskViewer
{
    partial class MonitorTaskViewForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorTaskViewForm));
            LightningChartLib.WinForms.Charting.ChartOptions chartOptions1 = new LightningChartLib.WinForms.Charting.ChartOptions();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCSVPath = new System.Windows.Forms.TextBox();
            this.buttonBrwseCSV = new System.Windows.Forms.Button();
            this.lightningChartUltimate1 = new LightningChartLib.WinForms.Charting.LightningChart();
            this.buttonProceed = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridViewUnused = new System.Windows.Forms.DataGridView();
            this.ColLbl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMoment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewObj = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUnused)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewObj)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Monitor Task CSV file path";
            // 
            // textBoxCSVPath
            // 
            this.textBoxCSVPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCSVPath.Location = new System.Drawing.Point(151, 18);
            this.textBoxCSVPath.Name = "textBoxCSVPath";
            this.textBoxCSVPath.Size = new System.Drawing.Size(693, 20);
            this.textBoxCSVPath.TabIndex = 1;
            // 
            // buttonBrwseCSV
            // 
            this.buttonBrwseCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrwseCSV.Location = new System.Drawing.Point(850, 18);
            this.buttonBrwseCSV.Name = "buttonBrwseCSV";
            this.buttonBrwseCSV.Size = new System.Drawing.Size(36, 23);
            this.buttonBrwseCSV.TabIndex = 2;
            this.buttonBrwseCSV.Text = "...";
            this.buttonBrwseCSV.UseVisualStyleBackColor = true;
            this.buttonBrwseCSV.Click += new System.EventHandler(this.buttonBrwseCSV_Click);
            // 
            // lightningChartUltimate1
            // 
            this.lightningChartUltimate1.BackColor = System.Drawing.Color.Gray;
            this.lightningChartUltimate1.ChartManager = null;
            this.lightningChartUltimate1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lightningChartUltimate1.Location = new System.Drawing.Point(0, 0);
            this.lightningChartUltimate1.MinimumSize = new System.Drawing.Size(110, 90);
            this.lightningChartUltimate1.Name = "lightningChartUltimate1";
            this.lightningChartUltimate1.Options = chartOptions1;
            this.lightningChartUltimate1.Size = new System.Drawing.Size(1044, 430);
            this.lightningChartUltimate1.TabIndex = 3;
            //this.lightningChartUltimate1.Background = ((Arction.WinForms.Charting.Fill)(resources.GetObject("lightningChartUltimate1.Background")));
            //this.lightningChartUltimate1.RenderOptions = ((Arction.WinForms.Charting.Views.RenderOptionsCommon)(resources.GetObject("lightningChartUltimate1.RenderOptions")));
            //this.lightningChartUltimate1.Title = ((Arction.WinForms.Charting.Titles.ChartTitle)(resources.GetObject("lightningChartUltimate1.Title")));
            //this.lightningChartUltimate1.HorizontalScrollBars = ((System.Collections.Generic.List<Arction.WinForms.Charting.HorizontalScrollBar>)(resources.GetObject("lightningChartUltimate1.HorizontalScrollBars")));
            //this.lightningChartUltimate1.VerticalScrollBars = ((System.Collections.Generic.List<Arction.WinForms.Charting.VerticalScrollBar>)(resources.GetObject("lightningChartUltimate1.VerticalScrollBars")));
            //this.lightningChartUltimate1.View3D = ((Arction.WinForms.Charting.Views.View3D.View3D)(resources.GetObject("lightningChartUltimate1.View3D")));
            //this.lightningChartUltimate1.ViewPie3D = ((Arction.WinForms.Charting.Views.ViewPie3D.ViewPie3D)(resources.GetObject("lightningChartUltimate1.ViewPie3D")));
            //this.lightningChartUltimate1.ViewPolar = ((Arction.WinForms.Charting.Views.ViewPolar.ViewPolar)(resources.GetObject("lightningChartUltimate1.ViewPolar")));
            //this.lightningChartUltimate1.ViewSmith = ((Arction.WinForms.Charting.Views.ViewSmith.ViewSmith)(resources.GetObject("lightningChartUltimate1.ViewSmith")));
            //this.lightningChartUltimate1.ViewXY = ((Arction.WinForms.Charting.Views.ViewXY.ViewXY)(resources.GetObject("lightningChartUltimate1.ViewXY")));
            // 
            // buttonProceed
            // 
            this.buttonProceed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonProceed.Location = new System.Drawing.Point(952, 15);
            this.buttonProceed.Name = "buttonProceed";
            this.buttonProceed.Size = new System.Drawing.Size(104, 28);
            this.buttonProceed.TabIndex = 1;
            this.buttonProceed.Text = "Proceed";
            this.buttonProceed.UseVisualStyleBackColor = true;
            this.buttonProceed.Click += new System.EventHandler(this.buttonProceed_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(624, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Unused task";
            // 
            // dataGridViewUnused
            // 
            this.dataGridViewUnused.AllowUserToAddRows = false;
            this.dataGridViewUnused.AllowUserToDeleteRows = false;
            this.dataGridViewUnused.AllowUserToResizeRows = false;
            this.dataGridViewUnused.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUnused.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUnused.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColLbl,
            this.ColMoment,
            this.ColTime,
            this.ColId});
            this.dataGridViewUnused.Location = new System.Drawing.Point(627, 36);
            this.dataGridViewUnused.Name = "dataGridViewUnused";
            this.dataGridViewUnused.ReadOnly = true;
            this.dataGridViewUnused.Size = new System.Drawing.Size(414, 146);
            this.dataGridViewUnused.TabIndex = 6;
            // 
            // ColLbl
            // 
            this.ColLbl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColLbl.HeaderText = "Lbl";
            this.ColLbl.Name = "ColLbl";
            this.ColLbl.ReadOnly = true;
            // 
            // ColMoment
            // 
            this.ColMoment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColMoment.HeaderText = "S / E";
            this.ColMoment.Name = "ColMoment";
            this.ColMoment.ReadOnly = true;
            // 
            // ColTime
            // 
            this.ColTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColTime.HeaderText = "Time";
            this.ColTime.Name = "ColTime";
            this.ColTime.ReadOnly = true;
            // 
            // ColId
            // 
            this.ColId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColId.HeaderText = "ID";
            this.ColId.Name = "ColId";
            this.ColId.ReadOnly = true;
            // 
            // dataGridViewObj
            // 
            this.dataGridViewObj.AllowUserToAddRows = false;
            this.dataGridViewObj.AllowUserToDeleteRows = false;
            this.dataGridViewObj.AllowUserToResizeRows = false;
            this.dataGridViewObj.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewObj.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewObj.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3,
            this.ColStart,
            this.ColEnd});
            this.dataGridViewObj.Location = new System.Drawing.Point(3, 36);
            this.dataGridViewObj.MultiSelect = false;
            this.dataGridViewObj.Name = "dataGridViewObj";
            this.dataGridViewObj.ReadOnly = true;
            this.dataGridViewObj.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewObj.Size = new System.Drawing.Size(615, 146);
            this.dataGridViewObj.TabIndex = 7;
            this.dataGridViewObj.SelectionChanged += new System.EventHandler(this.dataGridViewObj_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "Lbl";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "Duration";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // ColStart
            // 
            this.ColStart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColStart.HeaderText = "Start";
            this.ColStart.Name = "ColStart";
            this.ColStart.ReadOnly = true;
            // 
            // ColEnd
            // 
            this.ColEnd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColEnd.HeaderText = "End";
            this.ColEnd.Name = "ColEnd";
            this.ColEnd.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Summary";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Location = new System.Drawing.Point(12, 49);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lightningChartUltimate1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewUnused);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewObj);
            this.splitContainer1.Panel2MinSize = 60;
            this.splitContainer1.Size = new System.Drawing.Size(1044, 619);
            this.splitContainer1.SplitterDistance = 430;
            this.splitContainer1.TabIndex = 9;
            // 
            // MonitorTaskViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 680);
            this.Controls.Add(this.buttonProceed);
            this.Controls.Add(this.buttonBrwseCSV);
            this.Controls.Add(this.textBoxCSVPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MonitorTaskViewForm";
            this.Text = "LS Monitor Task Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MonitorTaskViewForm_FormClosing);
            this.Load += new System.EventHandler(this.MonitorTaskViewForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUnused)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewObj)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCSVPath;
        private System.Windows.Forms.Button buttonBrwseCSV;
        private LightningChartLib.WinForms.Charting.LightningChart lightningChartUltimate1;
        private System.Windows.Forms.Button buttonProceed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridViewUnused;
        private System.Windows.Forms.DataGridView dataGridViewObj;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLbl;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMoment;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColEnd;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

