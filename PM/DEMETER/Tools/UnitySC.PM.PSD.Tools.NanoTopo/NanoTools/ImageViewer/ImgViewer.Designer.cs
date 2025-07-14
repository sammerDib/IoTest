namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.ImageViewer
{
    partial class ImgViewer
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
            DisposeControl();

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
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnMode = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.cbZoom = new System.Windows.Forms.ComboBox();
            this.btnFitToScreen = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnRotate270 = new System.Windows.Forms.Button();
            this.btnRotate90 = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.sbVert = new System.Windows.Forms.VScrollBar();
            this.sbHoriz = new System.Windows.Forms.HScrollBar();
            this.sbPanel = new System.Windows.Forms.Panel();
            this.Previewpanel = new System.Windows.Forms.Panel();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMenu.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelMenu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMenu.Controls.Add(this.btnMode);
            this.panelMenu.Controls.Add(this.btnPreview);
            this.panelMenu.Controls.Add(this.cbZoom);
            this.panelMenu.Controls.Add(this.btnFitToScreen);
            this.panelMenu.Controls.Add(this.btnZoomIn);
            this.panelMenu.Controls.Add(this.btnZoomOut);
            this.panelMenu.Controls.Add(this.btnRotate270);
            this.panelMenu.Controls.Add(this.btnRotate90);
            this.panelMenu.Controls.Add(this.btnOpen);
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(307, 29);
            this.panelMenu.TabIndex = 11;
            // 
            // btnMode
            // 
            this.btnMode.Image = ResourceImgViewer.btnSelect;
            this.btnMode.Location = new System.Drawing.Point(142, 1);
            this.btnMode.Name = "btnMode";
            this.btnMode.Size = new System.Drawing.Size(25, 25);
            this.btnMode.TabIndex = 16;
            this.btnMode.UseVisualStyleBackColor = true;
            this.btnMode.Click += new System.EventHandler(this.btnMode_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Image = ResourceImgViewer.btnPreview;
            this.btnPreview.Location = new System.Drawing.Point(198, 1);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(25, 25);
            this.btnPreview.TabIndex = 15;
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Visible = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // cbZoom
            // 
            this.cbZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbZoom.FormattingEnabled = true;
            this.cbZoom.Location = new System.Drawing.Point(240, 3);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(62, 21);
            this.cbZoom.TabIndex = 14;
            this.cbZoom.SelectedIndexChanged += new System.EventHandler(this.cbZoom_SelectedIndexChanged);
            this.cbZoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbZoom_KeyPress);
            // 
            // btnFitToScreen
            // 
            this.btnFitToScreen.Image = ResourceImgViewer.btnFitToScreen;
            this.btnFitToScreen.Location = new System.Drawing.Point(58, 1);
            this.btnFitToScreen.Name = "btnFitToScreen";
            this.btnFitToScreen.Size = new System.Drawing.Size(25, 25);
            this.btnFitToScreen.TabIndex = 13;
            this.btnFitToScreen.UseVisualStyleBackColor = true;
            this.btnFitToScreen.Click += new System.EventHandler(this.btnFitToScreen_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Image = ResourceImgViewer.btnZoomIn;
            this.btnZoomIn.Location = new System.Drawing.Point(2, 1);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(25, 25);
            this.btnZoomIn.TabIndex = 12;
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Image = ResourceImgViewer.btnZoomOut;
            this.btnZoomOut.Location = new System.Drawing.Point(30, 1);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(25, 25);
            this.btnZoomOut.TabIndex = 11;
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnRotate270
            // 
            this.btnRotate270.Image = ResourceImgViewer.btnRotate270;
            this.btnRotate270.Location = new System.Drawing.Point(86, 1);
            this.btnRotate270.Name = "btnRotate270";
            this.btnRotate270.Size = new System.Drawing.Size(25, 25);
            this.btnRotate270.TabIndex = 10;
            this.btnRotate270.UseVisualStyleBackColor = true;
            this.btnRotate270.Click += new System.EventHandler(this.btnRotate270_Click);
            // 
            // btnRotate90
            // 
            this.btnRotate90.Image = ResourceImgViewer.btnRotate90;
            this.btnRotate90.Location = new System.Drawing.Point(114, 1);
            this.btnRotate90.Name = "btnRotate90";
            this.btnRotate90.Size = new System.Drawing.Size(25, 25);
            this.btnRotate90.TabIndex = 9;
            this.btnRotate90.UseVisualStyleBackColor = true;
            this.btnRotate90.Click += new System.EventHandler(this.btnRotate90_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Image = ResourceImgViewer.btnOpen;
            this.btnOpen.Location = new System.Drawing.Point(170, 1);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(25, 25);
            this.btnOpen.TabIndex = 8;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Visible = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // sbVert
            // 
            this.sbVert.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sbVert.Location = new System.Drawing.Point(276, 0);
            this.sbVert.Name = "sbVert";
            this.sbVert.Size = new System.Drawing.Size(17, 253);
            this.sbVert.TabIndex = 0;
            this.sbVert.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbVert_Scroll);
            // 
            // sbHoriz
            // 
            this.sbHoriz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sbHoriz.Location = new System.Drawing.Point(0, 252);
            this.sbHoriz.Name = "sbHoriz";
            this.sbHoriz.Size = new System.Drawing.Size(276, 17);
            this.sbHoriz.TabIndex = 1;
            this.sbHoriz.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbHoriz_Scroll);
            // 
            // sbPanel
            // 
            this.sbPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbPanel.BackColor = System.Drawing.SystemColors.Info;
            this.sbPanel.Location = new System.Drawing.Point(277, 253);
            this.sbPanel.Name = "sbPanel";
            this.sbPanel.Size = new System.Drawing.Size(16, 16);
            this.sbPanel.TabIndex = 2;
            // 
            // Previewpanel
            // 
            this.Previewpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Previewpanel.Location = new System.Drawing.Point(308, 0);
            this.Previewpanel.Name = "Previewpanel";
            this.Previewpanel.Size = new System.Drawing.Size(155, 219);
            this.Previewpanel.TabIndex = 14;
            // 
            // ImgViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Previewpanel);
            this.Controls.Add(this.panelMenu);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MinimumSize = new System.Drawing.Size(454, 157);
            this.Name = "ImgViewer";
            this.Size = new System.Drawing.Size(466, 310);
            this.Load += new System.EventHandler(this.ImageViewer_Load);
            this.Click += new System.EventHandler(this.ImageViewer_Click);
            this.MouseEnter += new System.EventHandler(this.ImgViewer_MouseEnter);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ImageViewer_MouseWheel);
            this.Resize += new System.EventHandler(this.ImageViewer_Resize);
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private PanelDoubleBuffered pbFull;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnRotate270;
        private System.Windows.Forms.Button btnRotate90;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnFitToScreen;
        private System.Windows.Forms.ComboBox cbZoom;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnMode;
        private System.Windows.Forms.Panel sbPanel;
        private System.Windows.Forms.HScrollBar sbHoriz;
        private System.Windows.Forms.VScrollBar sbVert;
        private System.Windows.Forms.Panel Previewpanel;
    }
}
