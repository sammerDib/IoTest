using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.ImageViewer
{
 	public enum ATZoom
    {
        ZoomIn,
        ZoomOut
    }

    public partial class ImgViewer : UserControl
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern short GetKeyState(int key);

        protected IVDrawEngine drawEngine;
        protected IVDrawObject drawing;
        protected Bitmap preview;
        private IImgViewerStatus _StatusView;  
        private ImgPreviewer m_oDistantPreview;
        private ImgPreviewer m_oLocalPreview;
        private ImgPreviewer m_oCurrentPreview;
        private bool m_bIsPreviewDistant = false;
        protected List<Bitmap> ImgLayer;
        protected List<bool> ImgLayerDisplay;
      

        public int AddLayerImage(Bitmap p_Img, bool p_bDisplay = true)
        {
            ImgLayer.Add(p_Img);
            ImgLayerDisplay.Add(p_bDisplay);
            return ImgLayer.Count - 1;
        }

        public void DisplayLayerImage(int p_nId, bool p_bDisplay = true)
        {
            if (p_nId >= 0 && p_nId < ImgLayer.Count)
            {
                ImgLayerDisplay[p_nId] = p_bDisplay;
            }
        }

        public Bitmap GetLayerImage(int p_nId)
        {
            if (p_nId < 0 || p_nId >= ImgLayer.Count)
                return null;
            return ImgLayer[p_nId];
        }

        public bool IsLayerImageDisplayed(int p_nId)
        {
            if (p_nId < 0 || p_nId >= ImgLayerDisplay.Count)
                return false;
            return ImgLayerDisplay[p_nId];
        }

        public bool RemoveLayerImage(int p_nId)
        {
            if (p_nId < 0 || p_nId >= ImgLayerDisplay.Count)
                return false;

            ImgLayer.RemoveAt(p_nId);
            ImgLayerDisplay.RemoveAt(p_nId);
            return true;
        }
       
        public void RemoveAllLayerImage()
        {
            ImgLayer.Clear();
            ImgLayerDisplay.Clear();
        }

        public IImgViewerStatus StatusView
        {
            set
            {
                _StatusView = value;
            }
            get
            {
                return _StatusView;
            }
        }

        private bool isScrolling = false;
        private bool scrollbars = false;
        private double fps = 15.0;
        private bool animationEnabled = false;
        private bool selectMode = false;
        private bool shiftSelecting = false;
        private Point ptSelectionStart = new Point();
        private Point ptSelectionEnd = new Point();
        public bool selectLineMode = false;

        private bool panelDragging = false;
        private bool showPreview = true;
        private Cursor grabCursor = null;
        private Cursor dragCursor = null;
        private bool bShowProfileLine = false;
        public bool ShowProfileLine
        {
            set { bShowProfileLine = value; }
            get {return bShowProfileLine; }
        }
        public PointF m_PtProfileStart;
        public PointF m_PtProfileEnd;

        public bool SelectMode
        {
            set
            {
                selectMode = value;
                if (selectMode)
                {
                    this.btnMode.Image = ResourceImgViewer.btnDrag;
                }
                else
                {
                    this.btnMode.Image = ResourceImgViewer.btnSelect;
                }
            }
            get { return selectMode; }
        }

        public delegate void ImageViewerRotationEventHandler(object sender, ImageViewerRotationEventArgs e);
        public event ImageViewerRotationEventHandler AfterRotation;

        public delegate void ImageViewerNewProfileEventHandler(PointF ptStart, PointF ptEnd);
        public event ImageViewerNewProfileEventHandler AfterNewProfile;

        protected virtual void OnRotation(ImageViewerRotationEventArgs e)
        {
            if (AfterRotation != null)
            {
                AfterRotation(this, e);
            }
        }

        protected virtual void OnNewProfile()
        {
            if (AfterNewProfile != null)
            {
                AfterNewProfile(m_PtProfileStart, m_PtProfileEnd);
            }
        }

        public void SetDistantPreviewBox(ImgPreviewer pDistantPreviewPanel)
        {
            m_bIsPreviewDistant = true;

            m_oDistantPreview = pDistantPreviewPanel;
            m_oDistantPreview.m_dlgNavKeyPress += new KeyPressEventHandler(tbNavigation_KeyPress);
            m_oDistantPreview.m_dlgNavNext += new EventHandler(btnNext_Click);
            m_oDistantPreview.m_dlgNavBack += new EventHandler(btnBack_Click);
            m_oDistantPreview.m_dlgPanelMouseEnter += new EventHandler(ImgViewer_MouseEnter);
            m_oDistantPreview.m_dlgPanelMouseUp += new MouseEventHandler(pbPanel_MouseUp);
            m_oDistantPreview.m_dlgPanelMouseMove += new MouseEventHandler(pbPanel_MouseMove);
            m_oDistantPreview.m_dlgPanelMouseDown += new MouseEventHandler(pbPanel_MouseDown);

            m_oCurrentPreview = m_oDistantPreview;

            // Hide the local preview and extend full panel to the max
            Previewpanel.Hide();
            m_oLocalPreview.HidePreview(false);
            pbFull.Width = pbFull.Width + Previewpanel.Width;
            panelMenu.Width = pbFull.Width;
            InitControl();
            drawing.AvoidOutOfScreen();
            pbFull.Refresh();

            UpdatePanels(true);


//             this.m_currentPBox = pDistantPreviewPanel;
//             this.m_currentPBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseDown);
//             this.m_currentPBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseMove);
//             this.m_currentPBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPanel_MouseUp);
  //          this.Invalidate();
        }

        private void DrawProfileLine()
        {
            Pen myPen = new Pen(Color.GreenYellow);
            myPen.Width = 2.0f;
            int nCrosSz = 7;

            // calculate line position in client zone
            float ax = (this.drawing.BoundingBox.X + (float)drawing.Zoom * m_PtProfileStart.X);
            float ay = (this.drawing.BoundingBox.Y + (float)drawing.Zoom * m_PtProfileStart.Y);
            float bx = (this.drawing.BoundingBox.X + (float)drawing.Zoom * m_PtProfileEnd.X);
            float by = (this.drawing.BoundingBox.Y + (float)drawing.Zoom * m_PtProfileEnd.Y);

            Point Ax1, Ax2;
            Point A = new Point((int)Math.Round(ax, 0), (int)Math.Round(ay, 0));
            Point B = new Point((int)Math.Round(bx, 0), (int)Math.Round(by, 0));
          
            // Draw Line
            drawEngine.g.DrawLine(myPen, B, A);

            // Draw cross B
            Ax1 = new Point(B.X - nCrosSz, B.Y);
            Ax2 = new Point(B.X + nCrosSz, B.Y);
            drawEngine.g.DrawLine(myPen, Ax1, Ax2);
            Ax1 = new Point(B.X, B.Y - nCrosSz);
            Ax2 = new Point(B.X, B.Y + nCrosSz);
            drawEngine.g.DrawLine(myPen, Ax1, Ax2);

            myPen.Color = Color.Violet;
            // draw cross A
            Ax1 = new Point(A.X - nCrosSz, A.Y);
            Ax2 = new Point(A.X + nCrosSz, A.Y);
            drawEngine.g.DrawLine(myPen, Ax1, Ax2);
            Ax1 = new Point(A.X, A.Y - nCrosSz);
            Ax2 = new Point(A.X, A.Y + nCrosSz);
            drawEngine.g.DrawLine(myPen, Ax1, Ax2); 
        }

        private void CreateProfileLine(Point PtStart, Point PtEnd)
        {
            ShowProfileLine = true; 
            float fCoef = (float) (1.0 / this.drawing.Zoom);
            m_PtProfileStart = new PointF( fCoef * (PtStart.X - this.drawing.BoundingBox.X),  fCoef * (PtStart.Y - this.drawing.BoundingBox.Y));
            m_PtProfileEnd = new PointF(fCoef * (PtEnd.X - this.drawing.BoundingBox.X), fCoef * (PtEnd.Y - this.drawing.BoundingBox.Y));

            OnNewProfile();
        }

        public int PanelWidth
        {
            get
            {
                if (pbFull != null)
                {
                    return pbFull.Width;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int PanelHeight
        {
            get
            {
                if (pbFull != null)
                {
                    return pbFull.Height;
                }
                else
                {
                    return 0;
                }
            }
        }

        public delegate void ImageViewerZoomEventHandler(object sender, ImageViewerZoomEventArgs e);
        public event ImageViewerZoomEventHandler AfterZoom;

        protected virtual void OnZoom(ImageViewerZoomEventArgs e)
        {
            if (AfterZoom != null)
            {
                AfterZoom(this, e);
            }
        }

        public void InvalidatePanel()
        {
            this.pbFull.Invalidate();
            this.Previewpanel.Invalidate();
        }

        public bool Scrollbars
        {
            get { return scrollbars; }
            set
            {
                scrollbars = value;
                DisplayScrollbars();
                SetScrollbarValues();
            }
        }

        public double GifFPS
        {
            get
            {
                return fps;
            }
            set
            {
                if (value <= 30.0 && value > 0.0)
                {
                    fps = value;
                    if (this.drawing.Gif != null)
                    {
                        this.drawing.Gif.FPS = fps;
                    }
                }
            }
        }

        public bool GifAnimation
        {
            get 
            {
                return animationEnabled;
            }
            set
            {
                animationEnabled = value;
                if (this.drawing.Gif != null)
                {
                    this.drawing.Gif.AnimationEnabled = animationEnabled;
                }
            }
        }

        private bool IsKeyPressed(int key)
        {
            bool keyPressed = false;
            short result = GetKeyState(key);

            switch (result)
            {
                case 0:
                    // Not pressed and not toggled
                    keyPressed = false;
                    break;

                case 1:
                    // Not presses but toggled
                    keyPressed = false;
                    break;

                default:
                    // Pressed
                    keyPressed = true;
                    break;
            }

            return keyPressed;
        }

        public bool OpenButton
        {
            get { return btnOpen.Visible; }
            set
            {
                if (value)
                {
                    btnOpen.Show();
                }
                else
                {
                    btnOpen.Hide();
                }

                btnOpen.Location = new Point(btnMode.Location.X + 28, btnPreview.Location.Y);
                if (btnOpen.Visible == true)
                {
                    // Making sure it's aligned properly
                    btnPreview.Location = new Point(btnOpen.Location.X+28, btnPreview.Location.Y);
                }
                else
                {
                    // Making sure it's aligned properly
                    btnPreview.Location = new Point(btnOpen.Location.X, btnPreview.Location.Y);
                }
            }
        }

        public bool RotationsButtons
        {
            get { return (btnRotate270.Visible & btnRotate90.Visible);}
            set
            {
                if (value)
                {
                    btnRotate270.Show();
                    btnRotate90.Show();

                }
                else
                {
                    btnRotate270.Hide();
                    btnRotate90.Hide();
                }

                if (btnRotate270.Visible == true)
                {
                    // Making sure it's aligned properly
                    btnMode.Location = new Point(142, btnPreview.Location.Y);
                    btnOpen.Location = new Point(170, btnPreview.Location.Y);
                    OpenButton = btnOpen.Visible;
                    PreviewButton = btnPreview.Visible;
                }
                else
                {
                    // Making sure it's aligned properly
                    int ModeOldLocX = btnMode.Location.X;
                    btnMode.Location = new Point(btnRotate270.Location.X, btnPreview.Location.Y);
                    if (btnOpen.Visible)
                    {
                        int OpenOldLocX = btnOpen.Location.X;
                        btnOpen.Location = new Point(btnRotate90.Location.X, btnPreview.Location.Y);
                        btnPreview.Location = new Point(ModeOldLocX, btnPreview.Location.Y);
                    }
                    else
                    {  
                        btnPreview.Location = new Point(btnRotate90.Location.X, btnPreview.Location.Y);
                    }
                }
            }
        }

        public bool PreviewButton
        {
            get { return btnPreview.Visible; }
            set
            {
                if (value)
                {
                    if (btnOpen.Visible == true)
                    {
                        // Making sure it's aligned properly
                        btnPreview.Location = new Point(198, btnPreview.Location.Y);
                    }
                    else
                    {
                        // Making sure it's aligned properly
                        btnPreview.Location = new Point(btnOpen.Location.X, btnPreview.Location.Y);
                    }

                    btnPreview.Show();
                }
                else
                {
                    btnPreview.Hide();
                }
            }
        }

        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                this.pbFull.AllowDrop = value;
                base.AllowDrop = value;
            }
        }

        public double Zoom
        {
            get { return Math.Round(drawing.Zoom * 100, 0); }
            set
            {
                if (value > 0)
                {
                    // Make it a double!
                    double zoomDouble = (double)value / (double)100;

                    drawing.SetZoom(zoomDouble);
                    UpdatePanels(true);

                    btnZoomIn.Focus();
                }
            }
        }

        public Size OriginalSize
        {
            get { return drawing.OriginalSize; }
        }

        public Size CurrentSize
        {
            get { return drawing.CurrentSize; }
        }

        public Color MenuColor
        {
            get { return panelMenu.BackColor; }
            set
            {
                panelMenu.BackColor = value;
                m_oCurrentPreview.MenuColor = value;
            }
        }

        public Color MenuPanelColor
        {
            get { return panelMenu.BackColor; }
            set
            {
                panelMenu.BackColor = value;
            }
        }

        public Color NavigationPanelColor
        {
            get { return m_oCurrentPreview.NavigationPanelColor; }
            set
            {
                m_oCurrentPreview.NavigationPanelColor = value;
            }
        }

        public Color PreviewPanelColor
        {
            get { return m_oCurrentPreview.PreviewPanelColor; }
            set
            {
                m_oCurrentPreview.PreviewPanelColor = value;
            }
        }

        public Color NavigationTextColor
        {
            get { return m_oCurrentPreview.NavigationTextColor; }
            set { m_oCurrentPreview.NavigationTextColor = value; }
        }

        public Color TextColor
        {
            get { return m_oCurrentPreview.TextColor; }
            set
            {
                m_oCurrentPreview.TextColor = value;
                m_oCurrentPreview.NavigationTextColor = value;
            }
        }

        public Color PreviewTextColor
        {
            get { return m_oCurrentPreview.PreviewTextColor; }
            set { m_oCurrentPreview.PreviewTextColor = value; }
        }

        public Color BackgroundColor
        {
            get { return pbFull.BackColor; }
            set { pbFull.BackColor = value; }
        }

        public string PreviewText
        {
            get { return m_oCurrentPreview.PreviewText; }
            set { m_oCurrentPreview.PreviewText = value; }
        }

        public string ImagePath
        {
            set 
            { 
                drawing.ImagePath = value;

                UpdatePanels(true);
                ToggleMultiPage();

                // scrollbars
                DisplayScrollbars();
                SetScrollbarValues();
            }
        }
        
        public Bitmap Image
        {
            get
            {
                return drawing.Image;
            }
            set
            {
                drawing.Image = value;

                UpdatePanels(true);
                ToggleMultiPage();

                // scrollbars
                DisplayScrollbars();
                SetScrollbarValues();

            }
        }

        public int Rotation
        {
            get { return drawing.Rotation; }
            set
            {
                // Making sure the rotation is 0, 90, 180 or 270 degrees!
                if (value == 90 || value == 180 || value == 270 || value == 0)
                {
                     drawing.Rotation = value;
                }
            }
        }

        private void Preview()
        {
            // Hide preview panel mechanics
            // Making sure that UpdatePanels doesn't get called when it's hidden!

            if (showPreview != m_oCurrentPreview.IsPreviewVisible())
            {
                if (showPreview == false)
                {
                    m_oCurrentPreview.HidePreview(drawing.MultiPage);

                    if (!m_bIsPreviewDistant)
                    {
                        Previewpanel.Hide();
                        pbFull.Width = pbFull.Width + (4 + Previewpanel.Width);

                        if ( ! drawing.MultiPage)
                        {
                            panelMenu.Width = pbFull.Width;
                        }
                        
                    }

                    InitControl();
                    drawing.AvoidOutOfScreen();
                    pbFull.Refresh();
                }
                else
                {
                    m_oCurrentPreview.ShowPreview(drawing.MultiPage);

                    if (!m_bIsPreviewDistant)
                    {
                        Previewpanel.Show();
                        pbFull.Width = pbFull.Width - (4 + Previewpanel.Width);
                        if ( ! drawing.MultiPage)
                        {
                            panelMenu.Width = pbFull.Width;
                        }
                    }
                                        
                    InitControl();
                    drawing.AvoidOutOfScreen();
                    pbFull.Refresh();

                    UpdatePanels(true);
                }
            }
        }

        public bool ShowPreview
        {
            get { return showPreview; }
            set
            {
                if (showPreview != value)
                {
                    showPreview = value;
                    Preview();
                }
            }
        }

        public ImgViewer()
        {
            // DrawEngine & DrawObject initialization
            drawEngine = new IVDrawEngine();
            drawing = new IVDrawObject(this);

            ImgLayer = new List<Bitmap>();
            ImgLayerDisplay = new List<bool>();

            // Stream to initialize the cursors.
            Stream imgStream = null;

            try
            {
                Assembly a = Assembly.GetExecutingAssembly();

                imgStream = a.GetManifestResourceStream("AltaTools.Resources.Grab.cur");
                if (imgStream != null)
                {
                    grabCursor = new Cursor(imgStream);
                    imgStream = null;
                }

                imgStream = a.GetManifestResourceStream("AltaTools.Resources.Drag.cur");
                if (imgStream != null)
                {
                    dragCursor = new Cursor(imgStream);
                    imgStream = null;
                }
            }
            catch
            {
                // Cursors could not be found
            }

            InitializeComponent();
 
            //
            // pbFull
            // 
            this.pbFull = new NanoTools.ImageViewer.PanelDoubleBuffered();
            this.pbFull.SuspendLayout();
           
            this.pbFull.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFull.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pbFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbFull.Controls.Add(this.sbVert);
            this.pbFull.Controls.Add(this.sbHoriz);
            this.pbFull.Controls.Add(this.sbPanel);
            this.pbFull.Location = new System.Drawing.Point(2, 36);
            this.pbFull.Name = "pbFull";
            this.pbFull.Size = new System.Drawing.Size(295, 271);
            this.pbFull.TabIndex = 13;
            this.pbFull.Click += new System.EventHandler(this.pbFull_Click);
            this.pbFull.DragDrop += new System.Windows.Forms.DragEventHandler(this.pbFull_DragDrop);
            this.pbFull.DragEnter += new System.Windows.Forms.DragEventHandler(this.pbFull_DragEnter);
            this.pbFull.Paint += new System.Windows.Forms.PaintEventHandler(this.pbFull_Paint);
            this.pbFull.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseDoubleClick);
            this.pbFull.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseDown);
            this.pbFull.MouseEnter += new System.EventHandler(this.pbFull_MouseEnter);
            this.pbFull.MouseLeave += new System.EventHandler(this.pbFull_MouseLeave);
            this.pbFull.MouseHover += new System.EventHandler(this.pbFull_MouseHover);
            this.pbFull.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseMove);
            this.pbFull.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbFull_MouseUp);
            this.Controls.Add(this.pbFull);
            this.pbFull.ResumeLayout(false);

            // Init viewer panel
            m_oLocalPreview = new ImgPreviewer();
            m_oLocalPreview.m_dlgNavKeyPress += new KeyPressEventHandler(tbNavigation_KeyPress);
            m_oLocalPreview.m_dlgNavNext += new EventHandler(btnNext_Click);
            m_oLocalPreview.m_dlgNavBack += new EventHandler(btnBack_Click);
            m_oLocalPreview.m_dlgPanelMouseEnter += new EventHandler(ImgViewer_MouseEnter);
            m_oLocalPreview.m_dlgPanelMouseUp += new MouseEventHandler(pbPanel_MouseUp);
            m_oLocalPreview.m_dlgPanelMouseMove += new MouseEventHandler(pbPanel_MouseMove);
            m_oLocalPreview.m_dlgPanelMouseDown += new MouseEventHandler(pbPanel_MouseDown);

            Previewpanel.Controls.Add(m_oLocalPreview);

            m_oCurrentPreview = m_oLocalPreview;

            InitControl();

            Preview();
        }

        private void DisposeControl()
        {
            // No memory leaks here
            if (drawing != null)
            {
                drawing.Dispose();
            }

            if (drawEngine != null)
            {
                drawEngine.Dispose();
            }

            if (preview != null)
            {
                preview.Dispose();
            }
        }

        public void InitControl()
        {
            // Make sure panel is DoubleBuffering
            drawEngine.CreateDoubleBuffer(pbFull.CreateGraphics(), pbFull.Size.Width, pbFull.Size.Height);

            if (!scrollbars)
            {
                sbHoriz.Visible = false;
                sbVert.Visible = false;
                sbPanel.Visible = false;
            }
        }

        public void FocusOnMe()
        {
            // Do not lose focus! ("Fix" for the Scrolling issue)
            this.Focus();
        }

        protected virtual void DisplayScrollbars()
        {
            if (scrollbars)
            {
                if (this.Image != null)
                {
                    int perPercent = this.CurrentSize.Width / 100;

                    if (this.CurrentSize.Width - perPercent > this.pbFull.Width)
                    {
                        this.sbHoriz.Visible = true;
                    }
                    else
                    {
                        this.sbHoriz.Visible = false;
                    }

                    if (this.CurrentSize.Height - perPercent > this.pbFull.Height)
                    {
                        this.sbVert.Visible = true;
                    }
                    else
                    {
                        this.sbVert.Visible = false;
                    }

                    if (this.sbVert.Visible == true && this.sbHoriz.Visible == true)
                    {
                        this.sbPanel.Visible = true;
                        this.sbVert.Height = this.pbFull.Height - 18;
                        this.sbHoriz.Width = this.pbFull.Width - 18;
                    }
                    else
                    {
                        this.sbPanel.Visible = false;

                        if (this.sbVert.Visible)
                        {
                            this.sbVert.Height = this.pbFull.Height;
                        }
                        else
                        {
                            this.sbHoriz.Width = this.pbFull.Width;
                        }
                    }
                }
                else
                {
                    this.sbHoriz.Visible = false;
                    this.sbVert.Visible = false;
                    this.sbPanel.Visible = false;
                }
            }
            else
            {
                this.sbHoriz.Visible = false;
                this.sbVert.Visible = false;
                this.sbPanel.Visible = false;
            }
        }

        protected virtual void SetScrollbarValues()
        {
            if (scrollbars)
            {
                if (sbHoriz.Visible)
                {
                    isScrolling = true;
                    double perPercent = (double)this.CurrentSize.Width / 101.0;
                    double totalPercent = (double)this.pbFull.Width / perPercent;

                    sbHoriz.Minimum = 0;
                    sbHoriz.Maximum = 100;
                    sbHoriz.LargeChange = Convert.ToInt32(Math.Round(totalPercent, 0));

                    double value = (double)((-this.drawing.BoundingBox.X) / perPercent);

                    if (value > sbHoriz.Maximum) { sbHoriz.Value = (sbHoriz.Maximum - sbHoriz.LargeChange) + ((sbHoriz.LargeChange > 0) ? 1 : 0); }
                    else if (value < 0) { sbHoriz.Value = 0; }
                    else
                    {
                        sbHoriz.Value = Convert.ToInt32(Math.Round(value, 0));
                    }
                    isScrolling = false;
                }

                if (sbVert.Visible)
                {
                    isScrolling = true;
                    double perPercent = (double)this.CurrentSize.Height / 101.0;
                    double totalPercent = (double)this.pbFull.Height / perPercent;

                    sbVert.Minimum = 0;
                    sbVert.Maximum = 100;
                    sbVert.LargeChange = Convert.ToInt32(Math.Round(totalPercent, 0));

                    double value = (double)((-this.drawing.BoundingBox.Y) / perPercent);

                    if (value > sbVert.Maximum) { sbVert.Value = (sbVert.Maximum - sbVert.LargeChange) + ((sbVert.LargeChange > 0) ? 1 : 0); }
                    else if (value < 0) { sbVert.Value = 0; }
                    else
                    {
                        sbVert.Value = Convert.ToInt32(Math.Round(value, 0));
                    }
                    isScrolling = false;
                }
            }
            else
            {
                sbHoriz.Visible = false;
                sbVert.Visible = false;
            }
        }

        private void ImageViewer_Load(object sender, EventArgs e)
        {
            // Loop for ComboBox Items! Increments by 5% - [5 <-> 125]
            for (double z = 0.05; z <= 1.25; z = z + 0.05)
            {
                cbZoom.Items.Add(z * 100 + "%");
            }
            for (double z = 1.50; z <= 4.0; z = z + 0.25)
            {
                cbZoom.Items.Add(z * 100 + "%");
            }
            for (double z = 5.0; z <= 10.0; z = z + 1.0)
            {
                cbZoom.Items.Add(z * 100 + "%");
            }

            cbZoom.SelectedIndex = 5;
        }

        protected virtual void ToggleMultiPage()
        {
            m_oCurrentPreview.ToggleMultiPage(drawing.MultiPage, drawing.Pages, drawing.CurrentPage + 1, showPreview, m_bIsPreviewDistant);

            if (drawing.MultiPage)
            {
                
                if (!showPreview)
                {
                    if (!m_bIsPreviewDistant)
                    {
                        panelMenu.Width = Previewpanel.Right - 2 - (4 + Previewpanel.Width);
                        pbFull.Width = Previewpanel.Right - 2;
                    }
                }
                else
                {
       
                    if (!m_bIsPreviewDistant)
                    {
                        panelMenu.Width = Previewpanel.Right - 2 - (4 + Previewpanel.Width);
                        pbFull.Width = Previewpanel.Right - 2 - (4 + Previewpanel.Width);
                    }
                }
            }
            else
            {
                if (!showPreview)
                {
                    panelMenu.Width = Previewpanel.Right - 2;
                }
                else
                {
                    panelMenu.Width = pbFull.Width;
                }
            }
        }

        protected virtual void ImageViewer_Resize(object sender, EventArgs e)
        {
            InitControl();
            drawing.AvoidOutOfScreen();
            UpdatePanels(true);
        }

        protected virtual void pbFull_Paint(object sender, PaintEventArgs e)
        {
            // Can I double buffer?
            if (drawEngine.CanDoubleBuffer())
            {
                // Yes I can!
                drawEngine.g.FillRectangle(new SolidBrush(pbFull.BackColor), e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
                
                // Drawing to backBuffer
                drawing.Draw(drawEngine.g);

                for (int i = 0; i < ImgLayerDisplay.Count; i++ )
                {
                    if (IsLayerImageDisplayed(i))
                    {
                        drawEngine.g.DrawImage(GetLayerImage(i), drawing.BoundingBox);
                    }
                }

//                 System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
//                 System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
//                 cm.Matrix33=0.5f;
//                 ia.SetColorMatrix(cm);
//                 drawEngine.g.DrawImage(m_tpng, drawing.BoundingBox, 0,0,3000,3000,GraphicsUnit.Pixel,ia);
          
                if (ShowProfileLine)
                {
                    DrawProfileLine();
                }

                // Drawing to Panel
                drawEngine.Render(e.Graphics);
            }
        }

        protected virtual void pbFull_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Left Shift or Right Shift pressed? Or is select mode one?
                if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1) || selectMode == true)
                {
                    // Fancy cursor
                    pbFull.Cursor = Cursors.Cross;

                    shiftSelecting = true;

                    // Initial selection
                    ptSelectionStart.X = e.X;
                    ptSelectionStart.Y = e.Y;

                    // No selection end
                    ptSelectionEnd.X = -1;
                    ptSelectionEnd.Y = -1;
                }
                else
                {
                    // Start dragging
                    drawing.BeginDrag(new Point(e.X, e.Y));

                    // Fancy cursor
                    if (grabCursor != null)
                    {
                        pbFull.Cursor = grabCursor;
                    }
                }
            }
        }

        protected virtual void pbFull_MouseUp(object sender, MouseEventArgs e)
        {
            // Am i dragging or selecting?
            if (shiftSelecting == true)
            {
                if (selectLineMode)
                {
                    CreateProfileLine(ptSelectionStart, ptSelectionEnd);

                    // Clear the selection points
                    ptSelectionEnd.X = -1;
                    ptSelectionEnd.Y = -1;
                    ptSelectionStart.X = -1;
                    ptSelectionStart.Y = -1;

                    // Stop selecting
                    shiftSelecting = false;
                }
                else
                {
                    // Calculate my selection rectangle
                    Rectangle rect = CalculateReversibleRectangle(ptSelectionStart, ptSelectionEnd);

                    // Clear the selection rectangle
                    ptSelectionEnd.X = -1;
                    ptSelectionEnd.Y = -1;
                    ptSelectionStart.X = -1;
                    ptSelectionStart.Y = -1;

                    // Stop selecting
                    shiftSelecting = false;

                    // Position of the panel to the screen
                    Point ptPbFull = PointToScreen(pbFull.Location);

                    // Zoom to my selection
                    drawing.ZoomToSelection(rect, ptPbFull);
                }

                // Refresh my screen & update my preview panel
                pbFull.Refresh();
                UpdatePanels(true);
            }
            else
            {
                // Stop dragging and update my panels
                drawing.EndDrag();
                UpdatePanels(true);

                // Fancy cursor
                if (dragCursor != null)
                {
                    pbFull.Cursor = dragCursor;
                }
            }
        }

        protected virtual void pbFull_MouseMove(object sender, MouseEventArgs e)
        {
            if (_StatusView != null)
            {
                _StatusView.XMouse = e.X;
                _StatusView.YMouse = e.Y;
                _StatusView.XBmpPixel = (float)(1.0 / this.drawing.Zoom) * (e.X - this.drawing.BoundingBox.X);
                _StatusView.YBmpPixel = (float)(1.0 / this.drawing.Zoom) * (e.Y - this.drawing.BoundingBox.Y);
                _StatusView.DisplayCoordStatus();
            }

            // Am I dragging or selecting?
            if (shiftSelecting == true)
            {
                // Keep selecting
                ptSelectionEnd.X = e.X;
                ptSelectionEnd.Y = e.Y;
                
                Rectangle pbFullRect = new Rectangle(0, 0, pbFull.Width - 1, pbFull.Height - 1);

                // Am I still selecting within my panel?
                if (pbFullRect.Contains(new Point(e.X, e.Y)))
                {
                    if (selectLineMode)
                    {
                        // draw revervisble line
                        Point ptSelectStart = ptSelectionStart;
                        Point ptSelectEnd = ptSelectionEnd;
                        ptSelectStart = pbFull.PointToScreen(ptSelectStart);
                        ptSelectEnd = pbFull.PointToScreen(ptSelectEnd);
                        pbFull.Refresh();
                        ControlPaint.DrawReversibleLine(ptSelectStart, ptSelectEnd, Color.LightGray);
                    }
                    else
                    {
                         // If so, draw my Rubber Band Rectangle!
                         Rectangle rect = CalculateReversibleRectangle(ptSelectionStart, ptSelectionEnd);
                         DrawReversibleRectangle(rect);
                    }
                 
                }
            }
            else
            {
                // Keep dragging
                drawing.Drag(new Point(e.X, e.Y));
                if (drawing.IsDragging)
                {
                    UpdatePanels(false);
                }
                else
                {
                    // I'm not dragging OR selecting
                    // Make sure if left or right shift is pressed to change cursor

                    if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1) || selectMode == true) 
                    {
                        // Fancy Cursor
                        if (pbFull.Cursor != Cursors.Cross)
                        {
                            pbFull.Cursor = Cursors.Cross;
                        }
                    }
                    else
                    {
                        // Fancy Cursor
                        if (pbFull.Cursor != dragCursor)
                        {
                            pbFull.Cursor = dragCursor;
                        }
                    }
                }
            }
        }

        protected virtual void ImageViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            drawing.Scroll(sender, e);

            if (drawing.Image != null)
            {
                if (e.Delta < 0)
                {
                    OnZoom(new ImageViewerZoomEventArgs(drawing.Zoom, ATZoom.ZoomOut));
                }
                else
                {
                    OnZoom(new ImageViewerZoomEventArgs(drawing.Zoom, ATZoom.ZoomIn));
                }
            }

            UpdatePanels(true);
        }

        protected virtual void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\AltaSight\Nano";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.wmf;*.emf|JPEG Files (*.jpg)|*.jpg;*.jpeg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png|TIF files (*.tif;*.tiff)|*.tif;*.tiff|EMF/WMF Files (*.wmf;*.emf)|*.wmf;*.emf|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.ImagePath = openFileDialog.FileName;
            }

            UpdatePanels(true);
        }

        protected virtual void btnRotate270_Click(object sender, EventArgs e)
        {
            if (drawing != null)
            {
                drawing.Rotate270();

                // AfterRotation Event
                OnRotation(new ImageViewerRotationEventArgs(drawing.Rotation));
                UpdatePanels(true);
                ToggleMultiPage();
            }
        }

        protected virtual void btnRotate90_Click(object sender, EventArgs e)
        {
            if (drawing != null)
            {
                drawing.Rotate90();

                // AfterRotation Event
                OnRotation(new ImageViewerRotationEventArgs(drawing.Rotation));
                UpdatePanels(true);
                ToggleMultiPage();
            }
        }

        public void Rotate90()
        {
            if (drawing != null)
            {
                drawing.Rotate90();

                // AfterRotation Event
                OnRotation(new ImageViewerRotationEventArgs(drawing.Rotation));
                UpdatePanels(true);
                ToggleMultiPage();
            }
        }

        public void Rotate180()
        {
            if (drawing != null)
            {
                drawing.Rotate180();

                // AfterRotation Event
                OnRotation(new ImageViewerRotationEventArgs(drawing.Rotation));
                UpdatePanels(true);
                ToggleMultiPage();
            }
        }

        public void Rotate270()
        {
            if (drawing != null)
            {
                drawing.Rotate270();

                // AfterRotation Event
                OnRotation(new ImageViewerRotationEventArgs(drawing.Rotation));
                UpdatePanels(true);
                ToggleMultiPage();
            }
        }

        protected virtual void btnZoomOut_Click(object sender, EventArgs e)
        {
            drawing.ZoomOut();

            // AfterZoom Event
            if (drawing.Image != null)
            {
                OnZoom(new ImageViewerZoomEventArgs(drawing.Zoom, ATZoom.ZoomOut));
            }
            UpdatePanels(true);
        }

        protected virtual void btnZoomIn_Click(object sender, EventArgs e)
        {
            drawing.ZoomIn();

            // AfterZoom Event
            if (drawing.Image != null)
            {
                OnZoom(new ImageViewerZoomEventArgs(drawing.Zoom, ATZoom.ZoomIn));
            }
            UpdatePanels(true);
        }

        public void FitToScreen()
        {
            drawing.FitToScreen();
            UpdatePanels(true);
        }

        public void FitAfterLoad(bool bFitEnable)
        {
            drawing.FitAfterLoad = bFitEnable;
        }

        protected virtual void btnFitToScreen_Click(object sender, EventArgs e)
        {
            FitToScreen();
        }

        protected virtual void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nZoomPerct = 0;
            int.TryParse(cbZoom.Text.Replace("%", ""), out nZoomPerct);

            double zoom = ((double)nZoomPerct)/100.0;
            double originalZoom = drawing.Zoom;

            if (drawing.Zoom != zoom && zoom != 0.0)
            {
                drawing.SetZoom(zoom);

                if (drawing.Image != null)
                {
                    if (zoom > originalZoom)
                    {
                        OnZoom(new ImageViewerZoomEventArgs(drawing.Zoom, ATZoom.ZoomIn));
                    }
                    else
                    {
                        OnZoom(new ImageViewerZoomEventArgs(drawing.Zoom, ATZoom.ZoomOut));
                    }
                }

                UpdatePanels(true);
            }
        }

        protected virtual void UpdatePanels(bool updatePreview)
        {
            if (drawing.CurrentSize.Width > 0 && drawing.OriginalSize.Width > 0)
            {
                // scrollbars
                DisplayScrollbars();
                SetScrollbarValues();

                // Make sure panel is up to date
                pbFull.Refresh();

                // Calculate zoom
                double zoom = Math.Round(((double)drawing.CurrentSize.Width / (double)drawing.OriginalSize.Width), 2);

                // Display zoom in percentages
                cbZoom.Text = (int)(zoom * 100) + "%";

                if (updatePreview && drawing.PreviewImage != null && m_oCurrentPreview.Visible == true)
                {
                    // No memory leaks here
                    if (preview != null)
                    {
                        preview.Dispose();
                        preview = null;
                    }

                    // New preview
                    preview = new Bitmap(drawing.PreviewImage.Size.Width, drawing.PreviewImage.Size.Height);

                    // Make sure panel is the same size as the bitmap
                    if (m_oCurrentPreview.PicBoxSize != drawing.PreviewImage.Size)
                    {
                        m_oCurrentPreview.PicBoxSize = drawing.PreviewImage.Size;
                    }

                    // New Graphics from the new bitmap we created (Empty)
                    using (Graphics g = Graphics.FromImage(preview))
                    {
                        // Draw the image on the bitmap
                        g.DrawImage(drawing.PreviewImage, 0, 0, drawing.PreviewImage.Size.Width, drawing.PreviewImage.Size.Height);

                        double ratioX = (double)drawing.PreviewImage.Size.Width / (double)drawing.CurrentSize.Width;
                        double ratioY = (double)drawing.PreviewImage.Size.Height / (double)drawing.CurrentSize.Height;

                        double boxWidth = pbFull.Width * ratioX;
                        double boxHeight = pbFull.Height * ratioY;
                        double positionX = ((drawing.BoundingBox.X - (drawing.BoundingBox.X * 2)) * ratioX);
                        double positionY = ((drawing.BoundingBox.Y - (drawing.BoundingBox.Y * 2)) * ratioY);

                        // Making the red pen
                        Pen pen = new Pen(Color.Red, 1);

                        if (boxHeight >= drawing.PreviewImage.Size.Height)
                        {
                            boxHeight = drawing.PreviewImage.Size.Height - 1;
                        }
                        else if ((boxHeight + positionY) > drawing.PreviewImage.Size.Height)
                        {
                            boxHeight = drawing.PreviewImage.Size.Height - (positionY);
                        }

                        if (boxWidth >= drawing.PreviewImage.Size.Width)
                        {
                            boxWidth = drawing.PreviewImage.Size.Width - 1;
                        }
                        else if ((boxWidth + positionX) > drawing.PreviewImage.Size.Width)
                        {
                            boxWidth = drawing.PreviewImage.Size.Width - (positionX);
                        }

                        // Draw the rectangle on the bitmap
                        g.DrawRectangle(pen, new Rectangle((int)positionX, (int)positionY, (int)boxWidth, (int)boxHeight));
                    }

                    // Display the bitmap
                    m_oCurrentPreview.Image = preview;
                }
            }
        }

        protected virtual void pbPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (panelDragging == false)
            {
                Size sz = m_oCurrentPreview.PicBoxSize;
                drawing.JumpToOrigin(e.X, e.Y, sz.Width, sz.Height, pbFull.Width, pbFull.Height);
                UpdatePanels(true);

                panelDragging = true;
            }
        }

        protected virtual void pbFull_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            drawing.JumpToOrigin(e.X + (drawing.BoundingBox.X - (drawing.BoundingBox.X * 2)), e.Y + (drawing.BoundingBox.Y - (drawing.BoundingBox.Y * 2)), pbFull.Width, pbFull.Height);
            UpdatePanels(true);
        }

        protected virtual void pbFull_MouseHover(object sender, EventArgs e)
        {
            // Left shift or Right shift!
            if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1))
            {
                // Fancy cursor
                pbFull.Cursor = Cursors.Cross;
            }
            else
            {
                // Fancy cursor if not dragging
                if (!drawing.IsDragging)
                {
                    pbFull.Cursor = dragCursor;
                }
            }
        }

        protected virtual void ImageViewer_Click(object sender, EventArgs e)
        {
            FocusOnMe();
        }

        protected virtual void pbFull_Click(object sender, EventArgs e)
        {
            FocusOnMe();
        }

        protected virtual void pbPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (panelDragging)
            {
                Size sz = m_oCurrentPreview.PicBoxSize;
                drawing.JumpToOrigin(e.X, e.Y, sz.Width, sz.Height, pbFull.Width, pbFull.Height);
                UpdatePanels(true);
            }
        }

        protected virtual void pbPanel_MouseUp(object sender, MouseEventArgs e)
        {
            panelDragging = false;
        }

        protected virtual void pbFull_MouseEnter(object sender, EventArgs e)
        {
            FocusOnMe();
            if (this.IsKeyPressed(0xA0) || this.IsKeyPressed(0xA1) || selectMode == true)
            {
                pbFull.Cursor = Cursors.Cross;
            }
            else
            {
                if (dragCursor != null)
                {
                    pbFull.Cursor = dragCursor;
                }
            }
        }

        protected virtual void pbFull_MouseLeave(object sender, EventArgs e)
        {
            pbFull.Cursor = Cursors.Default;
        }

        protected virtual void btnPreview_Click(object sender, EventArgs e)
        {
            if (this.ShowPreview)
            {
                this.ShowPreview = false;
            }
            else
            {
                this.ShowPreview = true;
            }
        }

        protected virtual void cbZoom_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
                if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back)
                {
                    // If enter is pressed apply the entered zoom
                    if (e.KeyChar == (char)Keys.Return)
                    {
                        int zoom = 0;

                        // Make sure the percent sign is out of the cbZoom.Text
                        int.TryParse(cbZoom.Text.Replace("%", ""), out zoom);

                        // If zoom is higher than zero
                        if (zoom > 0)
                        {
                            // Make it a double!
                            double zoomDouble = (double)zoom / (double)100;

                            drawing.SetZoom(zoomDouble);
                            UpdatePanels(true);

                            btnZoomIn.Focus();
                        }
                    }

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        private Rectangle CalculateReversibleRectangle(Point ptSelectStart, Point ptSelectEnd)
        {
            Rectangle rect = new Rectangle();

            ptSelectStart = pbFull.PointToScreen(ptSelectStart);
            ptSelectEnd = pbFull.PointToScreen(ptSelectEnd);

            if (ptSelectStart.X < ptSelectEnd.X)
            {
                rect.X = ptSelectStart.X;
                rect.Width = ptSelectEnd.X - ptSelectStart.X;
            }
            else
            {
                rect.X = ptSelectEnd.X;
                rect.Width = ptSelectStart.X - ptSelectEnd.X;
            }
            if (ptSelectStart.Y < ptSelectEnd.Y)
            {
                rect.Y = ptSelectStart.Y;
                rect.Height = ptSelectEnd.Y - ptSelectStart.Y;
            }
            else
            {
                rect.Y = ptSelectEnd.Y;
                rect.Height = ptSelectStart.Y - ptSelectEnd.Y;
            }

            return rect;
        }

        private void DrawReversibleRectangle(Rectangle rect)
        {
            pbFull.Refresh();
            ControlPaint.DrawReversibleFrame(rect, Color.LightGray, FrameStyle.Dashed);
        }


        protected virtual void pbFull_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // Get The file(s) you dragged into an array. (We'll just pick the first image anyway)
                string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                
                for (int f = 0; f < FileList.Length; f++)
                {
                    // Make sure the file exists!
                    if (System.IO.File.Exists(FileList[f]))
                    {
                        string ext = (System.IO.Path.GetExtension(FileList[f])).ToLower();

                        // Checking the extensions to be Image formats
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".wmf" || ext == ".emf" || ext == ".bmp" || ext == ".png" || ext == ".tif" || ext == ".tiff")
                        {
                            try
                            {
                                // Try to load it into a bitmap
                                //newBmp = Bitmap.FromFile(FileList[f]);
                                this.ImagePath = FileList[f];

                                // If succeeded stop the loop
                                if (this.Image != null)
                                {
                                    break;
                                }
                            }
                            catch
                            {
                                // Not an image?
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        protected virtual void pbFull_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // Drop the file
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    // I'm not going to accept this unknown format!
                    e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        protected virtual void btnMode_Click(object sender, EventArgs e)
        {
            if (selectMode == false)
            {
                selectMode = true;
                this.btnMode.Image = ResourceImgViewer.btnDrag;
            }
            else
            {
                selectMode = false;
                this.btnMode.Image = ResourceImgViewer.btnSelect;
            }
        }

        protected virtual void btnNext_Click(object sender, EventArgs e)
        {
            drawing.NextPage();
            m_oCurrentPreview.NavigationText = (drawing.CurrentPage + 1).ToString(); 

            pbFull.Refresh();
            UpdatePanels(true);
        }

        protected virtual void btnBack_Click(object sender, EventArgs e)
        {
            drawing.PreviousPage();
            m_oCurrentPreview.NavigationText = (drawing.CurrentPage + 1).ToString(); 

            pbFull.Refresh();
            UpdatePanels(true);
        }

        protected virtual void tbNavigation_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
                if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back)
                {
                    // If enter is pressed apply the entered zoom
                    if (e.KeyChar == (char)Keys.Return)
                    {
                        int page = 0;

                        int.TryParse(m_oCurrentPreview.NavigationText, out page);
                        
                        // If zoom is higher than zero
                        if (page > 0 && page <= drawing.Pages)
                        {
                            drawing.SetPage(page);
                            UpdatePanels(true);

                            btnZoomIn.Focus();
                        }
                        else
                        {
                            m_oCurrentPreview.NavigationText = drawing.CurrentPage.ToString();
                        }
                    }

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        protected virtual void sbVert_Scroll(object sender, ScrollEventArgs e)
        {
            if (!isScrolling)
            {
                double perPercent = (double)this.CurrentSize.Height / 101.0;

                double value = e.NewValue * perPercent;

                this.drawing.SetPositionY(Convert.ToInt32(Math.Round(value, 0)));

                this.drawing.AvoidOutOfScreen();

                pbFull.Invalidate();

                UpdatePanels(true);
            }
        }

        protected virtual void sbHoriz_Scroll(object sender, ScrollEventArgs e)
        {
            if (!isScrolling)
            {
                double perPercent = (double)this.CurrentSize.Width / 101.0;

                double value = e.NewValue * perPercent;

                this.drawing.SetPositionX(Convert.ToInt32(Math.Round(value, 0)));

                this.drawing.AvoidOutOfScreen();

                pbFull.Invalidate();

                UpdatePanels(true);
            }
        }

        protected virtual void ImgViewer_MouseEnter(object sender, EventArgs e)
        {
            FocusOnMe();
        }
    }

    public class ImageViewerRotationEventArgs : EventArgs
    {
        private int rotation;
        public int Rotation
        {
            get { return rotation; }
        }

        public ImageViewerRotationEventArgs(int rotation)
        {
            this.rotation = rotation;
        }
    }

    public class ImageViewerZoomEventArgs : EventArgs
    {
        private int zoom;
        public int Zoom
        {
            get { return zoom; }
        }

        private ATZoom inOut;
        public ATZoom InOut
        {
            get { return inOut; }
        }

        public ImageViewerZoomEventArgs(double zoom, ATZoom inOut)
        {
            this.zoom = Convert.ToInt32(Math.Round((zoom * 100), 0));
            this.inOut = inOut;
        }
    }
}
