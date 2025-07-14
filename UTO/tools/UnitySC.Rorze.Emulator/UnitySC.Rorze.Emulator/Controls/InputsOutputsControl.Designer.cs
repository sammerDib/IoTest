
namespace UnitySC.Rorze.Emulator.Controls
{
    partial class InputsOutputsControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.paramDataGridView = new System.Windows.Forms.DataGridView();
            this.numberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionColums = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.paramDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // paramDataGridView
            // 
            this.paramDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.paramDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.paramDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.paramDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.numberColumn,
            this.nameColumn,
            this.descriptionColums,
            this.valueColumn});
            this.paramDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paramDataGridView.GridColor = System.Drawing.SystemColors.ScrollBar;
            this.paramDataGridView.Location = new System.Drawing.Point(0, 0);
            this.paramDataGridView.Name = "paramDataGridView";
            this.paramDataGridView.RowHeadersVisible = false;
            this.paramDataGridView.RowHeadersWidth = 30;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.paramDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.paramDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.paramDataGridView.RowTemplate.Height = 18;
            this.paramDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.paramDataGridView.Size = new System.Drawing.Size(410, 297);
            this.paramDataGridView.TabIndex = 5;
            this.paramDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParamDataGridView_CellValidated);
            // 
            // numberColumn
            // 
            this.numberColumn.HeaderText = "¹";
            this.numberColumn.Name = "numberColumn";
            this.numberColumn.ReadOnly = true;
            this.numberColumn.Width = 25;
            // 
            // nameColumn
            // 
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.ReadOnly = true;
            this.nameColumn.Width = 120;
            // 
            // descriptionColums
            // 
            this.descriptionColums.HeaderText = "Description";
            this.descriptionColums.Name = "descriptionColums";
            this.descriptionColums.ReadOnly = true;
            this.descriptionColums.Width = 220;
            // 
            // valueColumn
            // 
            this.valueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.valueColumn.HeaderText = "Value";
            this.valueColumn.Name = "valueColumn";
            this.valueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.valueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.valueColumn.Width = 21;
            // 
            // InputsOutputsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.paramDataGridView);
            this.Name = "InputsOutputsControl";
            this.Size = new System.Drawing.Size(410, 297);
            ((System.ComponentModel.ISupportInitialize)(this.paramDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridViewTextBoxColumn numberColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColums;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueColumn;
        protected System.Windows.Forms.DataGridView paramDataGridView;
    }
}
