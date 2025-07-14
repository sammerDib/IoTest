using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AltaTools;
using AltaTools.ImageViewer;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Microsoft.Win32;
using Matrox.MatroxImagingLibrary;
using System.Threading;

namespace ADCCalibDMT
{
    public partial class MainForm : Form, IImgViewerStatus
    {
        public delegate void Evt_SearchMire();
        public event Evt_SearchMire DoSearchMire;
        public int m_nValidAreaClick;
        public List<AutoResetEvent> m_MireEvtClick;

        public int m_nModeOverlay = 0; // 2=removepoint
        public Rectangle m_rcOldRect = Rectangle.Empty;

        public MIL_ID m_MilApplication;
        public MIL_ID m_MilSystem;
  
        public MIL_ID m_MilSrcCalibImgID = MIL.M_NULL;

        public OpenFileDialog m_OpenImgFileDlg;
        public OpenFileDialog m_OpenXMLFileDlg;

        private ImgViewer m_oViewer;
        private ImgPreviewer m_oPreviewer;
        private Bitmap m_MaskSelect;
        float m_ffactor = 0.0075F;

        public float XBmpPixel { get; set; }
        public float YBmpPixel { get; set; }
        public int XMouse { get; set; }
        public int YMouse { get; set; }
        public Color BmpPixelColor { get; set; }

        protected WaferCalib m_oWaferCalib = null;

        private string calibXmlFilePath; //textBoxWaferCalibXmlPath.Text

        private string calibImageFilePath; //textBoxCalibImage.Text

        private string calibDestPSDFilePath;

        public MainForm()
            : base()
        {
            InitializeComponent();

            /// MIL INIT
            MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref m_MilApplication);
            MIL.MsysAlloc(m_MilApplication, MIL.M_SYSTEM_HOST, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_MilSystem);
            MIL.MappControl(m_MilApplication, MIL.M_ERROR, MIL.M_PRINT_DISABLE);

            // other init
            m_OpenImgFileDlg = new OpenFileDialog();
            m_OpenImgFileDlg.InitialDirectory = @".";
            m_OpenImgFileDlg.Filter = "TIF files (*.tif;*.tiff)|*.tif;*.tiff|BMP Files (*.bmp)|*.bmp|All files (*.*)|*.*";

            m_OpenXMLFileDlg = new OpenFileDialog();
            m_OpenXMLFileDlg.InitialDirectory = @".";
            m_OpenXMLFileDlg.Filter = "Xml Files (*.xml)|*.xml|All files (*.*)|*.*";

            // Init viewer panel-
            m_oViewer = new ImgViewer();

            m_oViewer.AllowDrop = false;
            m_oViewer.RotationsButtons = false;
            m_oViewer.GifAnimation = false;
            m_oViewer.OpenButton = false;
            m_oViewer.PreviewButton = false;
            m_oViewer.ShowPreview = true;

            m_oViewer.OnEvtImgViewerClick += new ImgViewer.EvtImgViewerClick(OnViewerClick);

            m_oPreviewer = new ImgPreviewer();
            tableLayoutPanelViewer.Controls.Add(m_oPreviewer, 1, 0);
            m_oPreviewer.Dock = DockStyle.Fill;

            m_oViewer.SetDistantPreviewBox(m_oPreviewer);

            m_oViewer.Dock = DockStyle.Fill;
            tableLayoutPanelViewer.Controls.Add(m_oViewer, 0, 0);
            tableLayoutPanelViewer.SetRowSpan(m_oViewer, 4);
         
        }

        public MainForm(string imageFilePath, string xmlFilePath, string destPSDFilePath):this()
        {
            calibImageFilePath = imageFilePath;
            calibXmlFilePath = xmlFilePath;
            calibDestPSDFilePath = destPSDFilePath;
      
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveRegistry();

            if (m_MilSrcCalibImgID != MIL.M_NULL)
            {
                MIL.MbufFree(m_MilSrcCalibImgID);
                m_MilSrcCalibImgID = MIL.M_NULL;
            }

            if (m_oWaferCalib != null)
            {
                m_oWaferCalib.Dispose();
                m_oWaferCalib = null;
            }
     
            if (m_MilSystem != MIL.M_NULL)
                MIL.MsysFree(m_MilSystem);

            if (m_MilApplication != MIL.M_NULL)
                 MIL.MappFree(m_MilApplication);
        }

        void IImgViewerStatus.DisplayCoordStatus()
        {
             textBoxViewerStatus.Text = String.Format("Img Coord [{0:F1};{1:F1}] in pixels", XBmpPixel, YBmpPixel);
        }

        void IImgViewerStatus.DisplayTextStatus(string sMessage)
        {
            textBoxViewerStatus.Text = sMessage;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.DoSearchMire += new Evt_SearchMire(SearchMirePts);

            InitRegistry();

            // If the file paths have been provided by the command line, we use them
            if (!string.IsNullOrEmpty(calibXmlFilePath))
                textBoxWaferCalibXmlPath.Text=calibXmlFilePath;
            if (!string.IsNullOrEmpty(calibImageFilePath))
                textBoxCalibImage.Text=calibImageFilePath;

            LoadThisImage(textBoxCalibImage.Text);

            m_oViewer.StatusView = this;
            m_oViewer.FitToScreen();

            m_oViewer.Invalidate();
            m_oViewer.Update();
            m_oViewer.Refresh();     
        }

        public void OnViewerClick(float fptX, float fptY)
        {
            if (checkBoxSelectDefectMode.Checked)
            {
                this.SafeInvoke(d => d.ActionClick(fptX, fptY));
            }
        }

        public void ActionClick(float fptX, float fptY)
        {
            int nSize = Convert.ToInt32(textBoxSearchAreaMireSize.Text);
            int nx = (int)fptX;
            int ny = (int)fptY;
            int nSizeMire = Convert.ToInt32(textBoxMireSize.Text);
            Rectangle rc;
            switch (m_nModeOverlay)
            {
                case 0: // handle rect areas
                    if (m_rcOldRect != null)
                        RemoveRect(m_rcOldRect);
                   
                    rc = new Rectangle(nx - nSize, ny - nSize, nSize * 2 + 1, nSize * 2 + 1);
                    rc = Rectangle.Intersect(rc, Pts.g_rcImage);
                    DrawRect(rc, Color.FromArgb(70, 50, 255));
                    break;
                case 1: // handle cross point
                if (m_rcOldRect != null)
                    RemoveRect(m_rcOldRect);
                    DrawCross(fptX,fptY,"Test",Color.Orange);
                break;
                case 2:
                    m_nValidAreaClick++;
                    int nSizeRemovePts = nSizeMire;
                    rc = new Rectangle(nx - nSizeRemovePts, ny - nSizeRemovePts, nSizeRemovePts * 2 + 1, nSizeRemovePts * 2 + 1);
                    rc = Rectangle.Intersect(rc, Pts.g_rcImage);
                    DrawRect(rc, Color.FromArgb(50, 100, 255));
                    ///m_rcOldRect = rc;
                    m_MireEvtClick[0].Set();
                break;

            }
        }

        private void buttonCalibImgBrwse_Click(object sender, EventArgs e)
        {
            DialogResult result = m_OpenImgFileDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxCalibImage.Text = m_OpenImgFileDlg.FileName;
                LoadThisImage(textBoxCalibImage.Text);
            }
        }

        private void buttonBrwseWaferCalibXML_Click(object sender, EventArgs e)
        {
            DialogResult result = m_OpenXMLFileDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxWaferCalibXmlPath.Text = m_OpenXMLFileDlg.FileName;
            }
        }

        private void LoadThisImage(string imageFilePath)
        {
            if (File.Exists(imageFilePath) == false)
                return;

            if (m_oWaferCalib != null)
            {
                CancelProcess();
                m_oWaferCalib.Dispose();
                m_oWaferCalib = null;
            }

            Bitmap bmp = new Bitmap(imageFilePath);
            m_oViewer.FitAfterLoad(false);
            m_oViewer.Image = bmp;
            m_oViewer.FitAfterLoad(true);

            m_oViewer.RemoveAllLayerImage();

            m_MaskSelect = new Bitmap(m_oViewer.Image.Width, m_oViewer.Image.Height);
            m_MaskSelect.MakeTransparent(Color.Transparent);
            using (Graphics g = Graphics.FromImage(m_MaskSelect))
            {
                g.FillRectangle(Brushes.Transparent, 0, 0, m_oViewer.Image.Width, m_oViewer.Image.Height);
            }
            m_oViewer.AddLayerImage(m_MaskSelect, true);
            m_oViewer.FocusOnMe();
            m_oViewer.FitToScreen();

            m_rcOldRect = Rectangle.Empty;

            if (m_MilSrcCalibImgID != MIL.M_NULL)
            {
                MIL.MbufFree(m_MilSrcCalibImgID);
                m_MilSrcCalibImgID = MIL.M_NULL;
            }
            MIL_INT iSizeX = 0;
            MIL.MbufDiskInquire(imageFilePath, MIL.M_SIZE_X, ref iSizeX);
            MIL_INT iSizeY = 0;
            MIL.MbufDiskInquire(imageFilePath, MIL.M_SIZE_Y, ref iSizeY);
            MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)iSizeX, (MIL_INT)iSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref m_MilSrcCalibImgID);
            MIL.MbufImport(imageFilePath, MIL.M_DEFAULT, MIL.M_LOAD, m_MilSystem, ref m_MilSrcCalibImgID);

            // on met à jour la zone image des poitn afin d'eviter des decoupage hors zone images
            Pts.g_rcImage = new Rectangle(0, 0, (int)iSizeX, (int)iSizeY);

            OnClearOverlay();
        }

        private void InitRegistry()
        {
            // initialize registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            string svalue;
            if (key == null)
                MessageBox.Show("Error cannot Open : HKCU\\Software", "ADCCalibDMT Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            key.CreateSubKey("UnitySC");
            if (key == null)
                MessageBox.Show("Error cannot Create : HKCU\\Software\\UnitySC", "ADCCalibDMT Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            key = key.OpenSubKey("UnitySC", true);
            if (key == null)
                MessageBox.Show("Error cannot Open : HKCU\\Software\\UnitySC", "ADCCalibDMT Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            key.CreateSubKey("ADCCalibDMT");
            if (key == null)
                MessageBox.Show("Error cannot Create : HKCU\\Software\\UnitySC\\ADCCalibDMT", "ADCCalibDMT Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            key = key.OpenSubKey("ADCCalibDMT", true);
            if (key == null)
                MessageBox.Show("Error cannot Open : HKCU\\Software\\UnitySC\\ADCCalibDMT", "ADCCalibDMT Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            svalue = (string) key.GetValue("CalibImagePath");
            if(svalue == null)
            {
                textBoxCalibImage.Text = "";
                key.SetValue("CalibImagePath", textBoxCalibImage.Text);
            }
            else
                textBoxCalibImage.Text = svalue;

            svalue = (string)key.GetValue("CalibWaferXmlPath");
            if(svalue == null)
            {
                textBoxWaferCalibXmlPath.Text = "";
                key.SetValue("CalibWaferXmlPath", textBoxWaferCalibXmlPath.Text);
            }
            else
                textBoxWaferCalibXmlPath.Text = svalue;

            svalue = (string)key.GetValue("SpecificAreaSearchSize_px");
            if (svalue == null)
            {
                textBoxSearchAreaMireSize.Text = "400";
                key.SetValue("SpecificAreaSearchSize_px", textBoxSearchAreaMireSize.Text);
            }
            else
                textBoxSearchAreaMireSize.Text = svalue;

            svalue = (string)key.GetValue("PtsAreaSearchSize_px");
            if (svalue == null)
            {
                textBoxMireSize.Text = "50";
                key.SetValue("PtsAreaSearchSize_px", textBoxMireSize.Text);
            }
            else
                textBoxMireSize.Text = svalue;

            svalue = (string)key.GetValue("MarginTop_um");
            if (svalue == null)
            {
                textBoxMarginTop.Text = "5000";
                key.SetValue("MarginTop_um", textBoxMarginTop.Text);
            }
            else
                textBoxMarginTop.Text = svalue;

            svalue = (string)key.GetValue("MarginBottom_um");
            if (svalue == null)
            {
                textBoxMarginBottom.Text = "5000";
                key.SetValue("MarginBottom_um", textBoxMarginBottom.Text);
            }
            else
                textBoxMarginBottom.Text = svalue;

            svalue = (string)key.GetValue("MarginRight_um");
            if (svalue == null)
            {
                textBoxMarginRight.Text = "5000";
                key.SetValue("MarginRight_um", textBoxMarginRight.Text);
            }
            else
                textBoxMarginRight.Text = svalue;

            svalue = (string)key.GetValue("MarginLeft_um");
            if (svalue == null)
            {
                textBoxMarginLeft.Text = "5000";
                key.SetValue("MarginLeft_um", textBoxMarginLeft.Text);
            }
            else
                textBoxMarginLeft.Text = svalue;
  
        }

        private void SaveRegistry()
        {
            // initialize registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\UnitySC\\ADCCalibDMT", true);
             if (key == null)
            {
                MessageBox.Show("Error cannot SAVE : HKCU\\Software\\UnitySC\\ADCCalibDMT", "ADCCalibDMT Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(File.Exists(textBoxCalibImage.Text))
                key.SetValue("CalibImagePath", textBoxCalibImage.Text);

            if (File.Exists(textBoxWaferCalibXmlPath.Text))
                key.SetValue("CalibWaferXmlPath", textBoxWaferCalibXmlPath.Text);

            if (String.IsNullOrWhiteSpace(textBoxSearchAreaMireSize.Text) == false)
                key.SetValue("SpecificAreaSearchSize_px", textBoxSearchAreaMireSize.Text);

            if (String.IsNullOrWhiteSpace(textBoxMireSize.Text) == false)
                key.SetValue("PtsAreaSearchSize_px", textBoxMireSize.Text);

            if (String.IsNullOrWhiteSpace(textBoxMarginTop.Text) == false)
                key.SetValue("MarginTop_um", textBoxMarginTop.Text);

            if (String.IsNullOrWhiteSpace(textBoxMarginBottom.Text) == false)
                key.SetValue("MarginBottom_um", textBoxMarginBottom.Text);

            if (String.IsNullOrWhiteSpace(textBoxMarginRight.Text) == false)
                key.SetValue("MarginRight_um", textBoxMarginRight.Text);

            if (String.IsNullOrWhiteSpace(textBoxMarginLeft.Text) == false)
                key.SetValue("MarginLeft_um", textBoxMarginLeft.Text);


        }

        private void InitNewWaferCalibSearchPoints()
        {
            OnClearOverlay();

            // Load Calib Wafer
            if (m_oWaferCalib != null)
            {
                m_oWaferCalib.Dispose();
                m_oWaferCalib = null;
            }

            try
            {
                m_oWaferCalib = new WaferCalib(textBoxWaferCalibXmlPath.Text, m_MilSystem);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("wafer calib init crash = " + ex.Message);
            }

            m_oWaferCalib.OnDrawRect += new WaferCalib.GenericDelegate_Rect(OnDrawRect);
            m_oWaferCalib.OnDrawCross += new WaferCalib.GenericDelegate_Cross(OnDrawCross);
            m_oWaferCalib.OnClearOverlay += new WaferCalib.GenericDelegate(OnClearOverlay);
            m_oWaferCalib.OnDrawGrid += new WaferCalib.GenericDelegate(OnDrawGrid);
            m_oWaferCalib.OnDisplayInfo += new WaferCalib.GenericDelegateString(OnDisplayInfo);
            m_oWaferCalib.OnAbort += new WaferCalib.GenericDelegate(OnAbort);


            m_oWaferCalib.m_MilSrcCalibImgID = m_MilSrcCalibImgID;

            m_oWaferCalib.m_SearchSize_px = Convert.ToInt32(textBoxMireSize.Text);

            m_nValidAreaClick = 0;
            m_nModeOverlay = 0;// rect mode 


            //  disable IHM boutons
            buttonAutoSearchPoints.Enabled = false;
            buttonSearchPoints.Enabled = false;
            buttonCalibration.Enabled = false;
            checkBoxReportCalibDebugImage.Enabled = false;
            buttonRemovePoint.Enabled = false;
            buttonCheckCalib.Enabled = false;
        }

        private void buttonSearchPoints_Click(object sender, EventArgs e)
        {
            InitNewWaferCalibSearchPoints();

            buttonValid.Enabled = true;
            checkBoxSelectDefectMode.Checked = true;

            this.BeginInvoke(DoSearchMire);
        }

        private Rectangle MakeRectangle(int nx, int ny, int nSearchSize)
        { 
            return new Rectangle(nx - nSearchSize, ny - nSearchSize, nSearchSize * 2 + 1, nSearchSize * 2 + 1);
        }

        private void buttonAutoSearchPoints_Click(object sender, EventArgs e)
        {
            InitNewWaferCalibSearchPoints();

            const float autoFacto = 2.0f;
            int nSearchSize = (int)(autoFacto * (float)Convert.ToInt32(textBoxSearchAreaMireSize.Text));

            Rectangle rc;
            int nImgSizeX = m_oViewer.Image.Width;
            int nImgSizeY = m_oViewer.Image.Height;
            int nx = 0;
            int ny = 0;
            if (m_oWaferCalib.m_MireName.Count > 4)
            {
                //CENTER
                // assumming its near image center
                nx = nImgSizeX / 2;
                ny = nImgSizeY / 2;
                rc = MakeRectangle(nx, ny, nSearchSize);
                rc = Rectangle.Intersect(rc, Pts.g_rcImage);

                m_oWaferCalib.m_MirePts[m_nValidAreaClick].SearchAreaInImage = rc;
                m_oWaferCalib.m_MireEvtClick[m_nValidAreaClick].Set();
                m_nValidAreaClick++;
            }

            // SOUTH
            // assumming its near the bottom image center
            nx = nImgSizeX / 2;
            ny = 3 * nImgSizeY / 4;
            rc = MakeRectangle(nx, ny, nSearchSize);
            rc = Rectangle.Intersect(rc, Pts.g_rcImage);

            m_oWaferCalib.m_MirePts[m_nValidAreaClick].SearchAreaInImage = rc;
            m_oWaferCalib.m_MireEvtClick[m_nValidAreaClick].Set();
            m_nValidAreaClick++;

            // NORTH
            // assumming its near the upper image center
            nx = nImgSizeX / 2;
            ny = nImgSizeY / 4;
            rc = MakeRectangle(nx, ny, nSearchSize);
            rc = Rectangle.Intersect(rc, Pts.g_rcImage);

            m_oWaferCalib.m_MirePts[m_nValidAreaClick].SearchAreaInImage = rc;
            m_oWaferCalib.m_MireEvtClick[m_nValidAreaClick].Set();
            m_nValidAreaClick++;

            //WEST
            // assumming its near the left image center
            nx = nImgSizeX / 4;
            ny = nImgSizeY / 2;
            rc = MakeRectangle(nx, ny, nSearchSize);
            rc = Rectangle.Intersect(rc, Pts.g_rcImage);

            m_oWaferCalib.m_MirePts[m_nValidAreaClick].SearchAreaInImage = rc;
            m_oWaferCalib.m_MireEvtClick[m_nValidAreaClick].Set();
            m_nValidAreaClick++;

            //EAST
            // assumming its near the right image center
            nx = 3 * nImgSizeX / 4;
            ny = nImgSizeY / 2;
            rc = MakeRectangle(nx, ny, nSearchSize);
            rc = Rectangle.Intersect(rc, Pts.g_rcImage);

            m_oWaferCalib.m_MirePts[m_nValidAreaClick].SearchAreaInImage = rc;
            m_oWaferCalib.m_MireEvtClick[m_nValidAreaClick].Set();
            m_nValidAreaClick++;

            checkBoxSelectDefectMode.Checked = true;

            this.BeginInvoke(DoSearchMire);


        }

        public void OnAbort()
        {
            this.SafeInvoke(d => d.Abort());
        }
        public void Abort()
        {
            buttonAutoSearchPoints.Enabled = true;
            buttonSearchPoints.Enabled = true;
            buttonRemovePoint.Enabled = false;
            buttonCalibration.Enabled = false;
            checkBoxReportCalibDebugImage.Enabled = false;

            buttonValid.Enabled = false;
            checkBoxSelectDefectMode.Checked = false;
        }

        public void OnDisplayInfo(String sMsg)
        {
            this.SafeInvoke(d => d.DisplayInfo(sMsg));
        }
        public void DisplayInfo(String sMsg)
        {
            textBoxDisplayInfo.Text = sMsg;
        }

        public void OnDrawCross(double dx, double dy, string sLabel, Color oColor)
        {
            this.SafeInvoke(d => d.DrawCross(dx,dy,sLabel,oColor));
        }

        public void DrawCross(double dx, double dy, string sLabel, Color oColor, bool isMire = false)
        {
            float fx = (float)dx;
            float fy = (float)dy;
            using (Graphics g = Graphics.FromImage(m_MaskSelect))
            {
                float fSize = (((float)Math.Max(m_oViewer.Image.Width, m_oViewer.Image.Height)) * m_ffactor);
                g.DrawLine(new Pen(oColor, 3), fx - fSize, fy, fx + fSize, fy);
                g.DrawLine(new Pen(oColor, 3), fx, fy - fSize, fx, fy + fSize);

                float fontSize = isMire ? 0.75F : 0.6f;
                using (var crossfont = new Font("Arial", fontSize * fSize, FontStyle.Regular))
                {
                    var stringSize = g.MeasureString(sLabel, crossfont);
                    var LabelLocation = new PointF(fx - (stringSize.Width * 0.5f), fy + (stringSize.Height * 0.5f));

                    ////Write your text.
                    g.DrawString(sLabel, crossfont, new SolidBrush(oColor), LabelLocation);
                }
                m_rcOldRect = new Rectangle((int)(fx - fSize), (int)(fy - fSize), (int)(fSize * 2.0F + 1.0F), (int)(fSize * 2.0F + 1.0F));
            }
        }

        public void OnDrawRect(Rectangle rc, Color oColor)
        {
            this.SafeInvoke(d => d.DrawRect(rc, oColor));
        }
        public void DrawRect(Rectangle rc, Color oColor)
        {
            using (Graphics g = Graphics.FromImage(m_MaskSelect))
            {
                if (rc != Rectangle.Empty)
                {
                    g.DrawRectangle(new Pen(oColor, 3), rc.X, rc.Y, rc.Width, rc.Height);
                    m_rcOldRect = rc;
                }
            }
        }

        public void RemoveRect(Rectangle rc)
        {
           /* using (Graphics g = Graphics.FromImage(m_MaskSelect))
            {
                if (rc != Rectangle.Empty)
                {
                    rc.Inflate(5, 5);
                    g.SetClip(rc);
                    g.Clear(Color.Transparent);
                    g.ResetClip();
                }
            }*/
        }

        public void OnClearOverlay()
        {
            this.SafeInvoke(d => d.ClearOverlay());
        }
        public void ClearOverlay()
        {
            using (Graphics g = Graphics.FromImage(m_MaskSelect))
            {
                //clear previous drawings
                g.FillRectangle(Brushes.Transparent, 0, 0, m_oViewer.Image.Width, m_oViewer.Image.Height);
                g.Clear(Color.Transparent);
            }

            m_oViewer.Invalidate();
            m_oViewer.Update();
            m_oViewer.Refresh();
        }

        public void OnDrawGrid()
        {
            this.SafeInvoke(d => d.DrawGrid());
        }
        public void DrawGrid()
        {
            if (m_oWaferCalib == null)
                return;

            using (Graphics g = Graphics.FromImage(m_MaskSelect))
            {
                //clear previous drawings
                g.FillRectangle(Brushes.Transparent, 0, 0, m_oViewer.Image.Width, m_oViewer.Image.Height);
                g.Clear(Color.Transparent);

                Pen areapen = new Pen(Color.FromArgb(255,255,128), 1);
                areapen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                foreach (Pts opts in m_oWaferCalib.m_GridPts)
                {
                    if (opts.SearchAreaInImage != Rectangle.Empty)
                    {
                        if (opts.IsGoodToCalibrate())
                        {
                            g.DrawRectangle(areapen, opts.SearchAreaInImage.X, opts.SearchAreaInImage.Y, opts.SearchAreaInImage.Width, opts.SearchAreaInImage.Height);

                            Color oColor = Color.DarkOrange;
                            String sLabel = String.Format("[{0},{1}]", opts.nxx, opts.nyy);
                            DrawCross(opts.dPosX_px, opts.dPosY_px, sLabel, oColor);
                        }
                        else
                        {
                            g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 0), 5), opts.SearchAreaInImage.X, opts.SearchAreaInImage.Y, opts.SearchAreaInImage.Width, opts.SearchAreaInImage.Height);
                        }
                    }

                }


                for (int i = 0; i < WaferCalib.NB_MIRES; i++)
                {
                    if (m_oWaferCalib.m_SpecificPts[i] != null)
                    {
                        if (m_oWaferCalib.m_SpecificPts[i].bFound)
                        {
                            string schar = "";

                            Color oColor = Color.LightGray;
                            switch (i)
                            {
                                case WaferCalib.SOUTH: schar = "(S)"; oColor = Color.FromArgb(0, 255, 128); break;
                                case WaferCalib.NORTH: schar = "(N)"; oColor = Color.FromArgb(0, 255, 255); break;
                                case WaferCalib.EAST: schar = "(E)";  oColor = Color.FromArgb(255, 0, 255); break;
                                case WaferCalib.WEST: schar = "(W)";  oColor = Color.FromArgb(128, 128, 255); break;
                                case WaferCalib.CENTER: schar = "(C)"; oColor = Color.FromArgb(0, 255, 0); break;
                            }

                            Pen Mirepen = new Pen(oColor, 5);
                            //Mirepen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;    
                            String sLabel = String.Format("[{0},{1}] {2}", m_oWaferCalib.m_SpecificPts[i].nxx, m_oWaferCalib.m_SpecificPts[i].nyy, schar);
                            DrawCross(m_oWaferCalib.m_SpecificPts[i].dPosX_px, m_oWaferCalib.m_SpecificPts[i].dPosY_px, sLabel, oColor, true);
                            g.DrawRectangle(Mirepen, m_oWaferCalib.m_SpecificPts[i].SearchAreaInImage.X, m_oWaferCalib.m_SpecificPts[i].SearchAreaInImage.Y, m_oWaferCalib.m_SpecificPts[i].SearchAreaInImage.Width, m_oWaferCalib.m_SpecificPts[i].SearchAreaInImage.Height);

                        }
                    }
                }
            }

            m_oViewer.Invalidate();
            m_oViewer.Update();
            m_oViewer.Refresh();

            // on remets les bouttone enalbel qui vont bien
            buttonAutoSearchPoints.Enabled = true;
            buttonSearchPoints.Enabled = true;
            buttonCalibration.Enabled = true;
            checkBoxReportCalibDebugImage.Enabled = true;
            buttonRemovePoint.Enabled = true;
            buttonCheckCalib.Enabled = true;

            buttonValid.Enabled = false;
            checkBoxSelectDefectMode.Checked = false;
        }


        private Thread m_GUIThread;
      

        public void SearchMirePts()
        {
            m_GUIThread = new Thread(new ThreadStart(m_oWaferCalib.SearchPoints));
            m_GUIThread.Name = "SearchPointsThread";
            m_GUIThread.Start();
        }

        private void buttonValid_Click(object sender, EventArgs e)
        {
            if (m_rcOldRect == Rectangle.Empty)
                return;

            if (m_oWaferCalib == null)
                return;
            
            if (m_nValidAreaClick >= m_oWaferCalib.m_MireEvtClick.Count)
                return;

            m_oWaferCalib.m_MirePts[m_nValidAreaClick].SearchAreaInImage = m_rcOldRect;
            m_oWaferCalib.m_MireEvtClick[m_nValidAreaClick].Set();
            m_nValidAreaClick++;
            
            m_rcOldRect = Rectangle.Empty;

        }

        public void CancelProcess()
        {
            if (m_oWaferCalib != null)
            {
                m_oWaferCalib.CancelProcess();
                foreach (AutoResetEvent evt in m_oWaferCalib.m_MireEvtClick)
                {
                    // on libère l'attente des clicks restants
                    evt.Set();
                }
                // on nettoye l'ecran
                OnClearOverlay();
            }
        }
        private void buttonCancelProcess_Click(object sender, EventArgs e)
        {
            CancelProcess();
        }

        private void buttonRemovePoint_Click(object sender, EventArgs e)
        {
            m_nValidAreaClick = 1;
            m_nModeOverlay = 2;
            checkBoxSelectDefectMode.Checked = true;
            OnDisplayInfo("Click on view to remove any wrong detection point");

            m_oWaferCalib.m_MireEvtClick.Clear();
            m_oWaferCalib.m_MireEvtClick.Add(new AutoResetEvent(false));
            m_MireEvtClick = m_oWaferCalib.m_MireEvtClick;

            m_GUIThread = new Thread(new ThreadStart(RemovePts));
            m_GUIThread.Name = "RemovePtsThread";
            m_GUIThread.Start();
        }

        public void RemovePts()
        {
            if (m_MireEvtClick[0].WaitOne() == true)
            {
                foreach (Pts opts in m_oWaferCalib.m_GridPts)
                {
                    if (opts.IsGoodToCalibrate())
                    {
                        if (m_rcOldRect.Contains((int)opts.dPosX_px, (int)opts.dPosY_px))
                        {
                            opts.bFound = false;
                            opts.dPosX_px = 0.0;
                            opts.dPosY_px = 0.0;
                            break;
                        }
                    }
                }
                OnDrawGrid();
                m_nModeOverlay = 0;
            }
        }

        private void buttonCalibration_Click(object sender, EventArgs e)
        {
            if (m_oWaferCalib != null)
            {
                double dMarginTop = Convert.ToDouble(textBoxMarginTop.Text);
                double dMarginBottom = Convert.ToDouble(textBoxMarginBottom.Text);
                double dMarginLeft = Convert.ToDouble(textBoxMarginLeft.Text);
                double dMarginRight = Convert.ToDouble(textBoxMarginRight.Text);
                m_oWaferCalib.Calibration(m_MilSrcCalibImgID, dMarginTop, dMarginBottom, dMarginRight, dMarginLeft, calibDestPSDFilePath, checkBoxReportCalibDebugImage.Checked);
            }
        }

        private void buttonExportView_Click(object sender, EventArgs e)
        {
            // When user clicks button, show the dialog.
            saveFileDialogExportView.FileName = String.Format("ExportView_{0:yyyy_MM_dd_hh_mm_ss}.png", DateTime.Now);
            saveFileDialogExportView.ShowDialog();     
        }

        private void saveFileDialogExportView_FileOk(object sender, CancelEventArgs e)
        {
            string name = saveFileDialogExportView.FileName;
            FileStream myFileOut = new FileStream(name, FileMode.OpenOrCreate);
            using (Bitmap target = new Bitmap(m_oViewer.Image))
            {
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver; // this is the default, but just to be clear
                    g.DrawImage(m_MaskSelect, 0, 0);
                }
                string sextension = Path.GetExtension(name);
                sextension = sextension.ToLower();
                switch (sextension)
                {
                    case ".bmp" :
                        target.Save(myFileOut, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".png":
                        target.Save(myFileOut, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".gif":
                        target.Save(myFileOut, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case ".tiff":
                    case ".tif": 
                        target.Save(myFileOut, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case ".jpg":
                    case ".jpeg": 
                        target.Save(myFileOut, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    default:
                        target.Save(myFileOut, System.Drawing.Imaging.ImageFormat.Png);
                        break;

                }
            }
            myFileOut.Close();
        }

        private void buttonCheckCalib_Click(object sender, EventArgs e)
        {
            string sPsdFileName = "";

            if (m_oWaferCalib != null)
            {
                if (String.IsNullOrEmpty(m_oWaferCalib.m_sLastSaveFile) == false)
                {
                    sPsdFileName = m_oWaferCalib.m_sLastSaveFile;
                }
            }

            CheckForm oCkfDlg = new CheckForm(m_MilSystem, textBoxCalibImage.Text, sPsdFileName);
            try
            {
                oCkfDlg.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }                
        }

        private void textBoxSearchAdv_KeyPress(object sender, KeyPressEventArgs e)
        {
            // on autorise que les chiffres numérique
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&  (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
};
