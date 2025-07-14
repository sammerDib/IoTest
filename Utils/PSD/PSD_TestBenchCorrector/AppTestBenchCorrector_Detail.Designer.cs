namespace AppsTestBenchCorrector
{
    partial class AppTestBenchCorrector_Detail
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
        private void InitializeComponent(CDataPicture picture)
        {
            this.PBPictureDetail = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PBPictureDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // PBPictureDetail
            // 
            this.PBPictureDetail.Location = new System.Drawing.Point(1, 1);
            this.PBPictureDetail.Name = "PBPictureDetail";
            this.PBPictureDetail.Size = new System.Drawing.Size(picture.DisplayImage.Width, picture.DisplayImage.Height);
            this.PBPictureDetail.TabIndex = 0;
            this.PBPictureDetail.TabStop = false;
            this.PBPictureDetail.WaitOnLoad = true;
            // 
            // AppTestBenchCorrector_Detail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1084, 1061);
            this.Controls.Add(this.PBPictureDetail);
            this.Name = "AppTestBenchCorrector_Detail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AppTestBenchCorrector_Detail";
            ((System.ComponentModel.ISupportInitialize)(this.PBPictureDetail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PBPictureDetail;
    }
}
