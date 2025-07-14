namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.ImageViewer
{
    partial class ImgPreviewer
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelNavigation = new System.Windows.Forms.Panel();
            this.lblNavigation = new System.Windows.Forms.Label();
            this.tbNavigation = new System.Windows.Forms.TextBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.lblPreview = new System.Windows.Forms.Label();
            this.pbPanel = new System.Windows.Forms.PictureBox();
            this.panelNavigation.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // panelNavigation
            // 
            this.panelNavigation.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelNavigation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelNavigation.Controls.Add(this.lblNavigation);
            this.panelNavigation.Controls.Add(this.tbNavigation);
            this.panelNavigation.Controls.Add(this.btnBack);
            this.panelNavigation.Controls.Add(this.btnNext);
            this.panelNavigation.Location = new System.Drawing.Point(0, 156);
            this.panelNavigation.Name = "panelNavigation";
            this.panelNavigation.Size = new System.Drawing.Size(150, 29);
            this.panelNavigation.TabIndex = 16;
            this.panelNavigation.Visible = false;
            // 
            // lblNavigation
            // 
            this.lblNavigation.AutoSize = true;
            this.lblNavigation.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNavigation.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblNavigation.Location = new System.Drawing.Point(41, 5);
            this.lblNavigation.Name = "lblNavigation";
            this.lblNavigation.Size = new System.Drawing.Size(24, 18);
            this.lblNavigation.TabIndex = 1;
            this.lblNavigation.Text = "/ 0";
            // 
            // tbNavigation
            // 
            this.tbNavigation.Location = new System.Drawing.Point(4, 4);
            this.tbNavigation.Name = "tbNavigation";
            this.tbNavigation.Size = new System.Drawing.Size(33, 20);
            this.tbNavigation.TabIndex = 19;
            this.tbNavigation.Text = "0";
            this.tbNavigation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbNavigation.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbNavigation_KeyPress);
            // 
            // btnBack
            // 
            this.btnBack.Image = ResourceImgViewer.btnBack;
            this.btnBack.Location = new System.Drawing.Point(93, 1);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(25, 25);
            this.btnBack.TabIndex = 18;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Image = ResourceImgViewer.btnNext;
            this.btnNext.Location = new System.Drawing.Point(121, 1);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(25, 25);
            this.btnNext.TabIndex = 17;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // panelPreview
            // 
            this.panelPreview.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPreview.Controls.Add(this.lblPreview);
            this.panelPreview.Location = new System.Drawing.Point(0, 0);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(150, 29);
            this.panelPreview.TabIndex = 15;
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreview.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblPreview.Location = new System.Drawing.Point(3, 4);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(59, 18);
            this.lblPreview.TabIndex = 0;
            this.lblPreview.Text = "Preview";
            // 
            // pbPanel
            // 
            this.pbPanel.Location = new System.Drawing.Point(0, 34);
            this.pbPanel.Name = "pbPanel";
            this.pbPanel.Size = new System.Drawing.Size(150, 119);
            this.pbPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPanel.TabIndex = 14;
            this.pbPanel.TabStop = false;
            this.pbPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseDown);
            this.pbPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseMove);
            this.pbPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseUp);
            // 
            // ImgPreviewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelNavigation);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.pbPanel);
            this.Name = "ImgPreviewer";
            this.Size = new System.Drawing.Size(158, 192);
            this.MouseEnter += new System.EventHandler(this.ImgPreviewer_MouseEnter);
            this.panelNavigation.ResumeLayout(false);
            this.panelNavigation.PerformLayout();
            this.panelPreview.ResumeLayout(false);
            this.panelPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPanel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelNavigation;
        private System.Windows.Forms.Label lblNavigation;
        private System.Windows.Forms.TextBox tbNavigation;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.PictureBox pbPanel;
    }
}
