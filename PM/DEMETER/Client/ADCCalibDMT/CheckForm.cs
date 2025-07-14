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


using FloatDataFile;
using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.Shared.LibMIL;

namespace ADCCalibDMT
{
    public partial class CheckForm : Form, IImgViewerStatus
    {
        public MIL_ID m_MilSystem  = MIL.M_NULL;

        public bool m_bUse3DD = false;
        public float[] m_p3DABufDataRedressed = null;
    
        private Bitmap m_MaskSelect;
        float m_ffactor = 0.01F;
        public Rectangle m_rcOldRect = Rectangle.Empty;

        public OpenFileDialog m_OpenImgFileDlg;
        public OpenFileDialog m_OpenCalFileDlg;

        private ImgViewer m_oViewer;
        private ImgPreviewer m_oPreviewer;
      
        public float XBmpPixel { get; set; }
        public float YBmpPixel { get; set; }
        public int XMouse { get; set; }
        public int YMouse { get; set; }
        public Color BmpPixelColor { get; set; }

        private double m_dPixelSize = 0.0;
        private double m_dOffsetX_um = 0.0;
        private double m_dOffsetY_um = 0.0;

        public CheckForm(MIL_ID MilSystem, String sTestImgPath, String sCalibFile)
            : base()
        {
            InitializeComponent();

            m_MilSystem = MilSystem;

            textBoxCalibImage.Text = sTestImgPath;
            textBoxCalibfilePath.Text = sCalibFile;
             
            // other init
            m_OpenImgFileDlg = new OpenFileDialog();
            m_OpenImgFileDlg.InitialDirectory = @".";
            m_OpenImgFileDlg.Filter = "TIF files (*.tif;*.tiff)|*.tif;*.tiff|BMP Files (*.bmp)|*.bmp|3DA Files (*.3da)|*.3da|All files (*.*)|*.*";


            m_OpenCalFileDlg = new OpenFileDialog();
            m_OpenCalFileDlg.InitialDirectory = @".";
            m_OpenCalFileDlg.Filter = "psd Files (*.psd)|*.psd|All files (*.*)|*.*";


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

        private void CheckForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_bUse3DD = false;
            m_p3DABufDataRedressed = null;
        }

        void IImgViewerStatus.DisplayCoordStatus()
        {
            float fX_um = ((float)m_dPixelSize * XBmpPixel) + (float)m_dOffsetX_um;
            float fY_um = -1.0F * (((float)m_dPixelSize * YBmpPixel) + (float)m_dOffsetY_um);
            textBoxViewerStatus.Text = String.Format("Img Coord [{0:F1};{1:F1}] in pixels  -- Wafer Coord [{2:F1};{3:F1}] in mm", XBmpPixel, YBmpPixel, fX_um / 1000.0F, fY_um / 1000.0F);
        }

        void IImgViewerStatus.DisplayTextStatus(string sMessage)
        {
            textBoxViewerStatus.Text = sMessage;
        }

        private void CheckForm_Load(object sender, EventArgs e)
        {
            LoadAndTransformImage();

            m_oViewer.StatusView = this;
            m_oViewer.FitToScreen();

            m_oViewer.Invalidate();
            m_oViewer.Update();
            m_oViewer.Refresh();
        }

        public void OnViewerClick(float fptX, float fptY)
        {
            if (m_rcOldRect != null)
                RemoveRect(m_rcOldRect);
            OnDrawCross(fptX, fptY, "", Color.OrangeRed);

            float fX_um = ((float)m_dPixelSize * fptX) + (float)m_dOffsetX_um;
            float fY_um = -1.0F * (((float)m_dPixelSize * fptY) + (float)m_dOffsetY_um);
            textBoxImgCoord.Text = String.Format("[{0:F1};{1:F1}]", fptX, fptY);
            textBoxWaferCoord.Text = String.Format("[{0:F1};{1:F1}]", fX_um / 1000.0F, fY_um / 1000.0F);
        }

        private void buttonCalibImgBrwse_Click(object sender, EventArgs e)
        {
            DialogResult result = m_OpenImgFileDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxCalibImage.Text = m_OpenImgFileDlg.FileName;
                LoadAndTransformImage();
            }
        }

        private void buttonBrwseWaferCalibXML_Click(object sender, EventArgs e)
        {
            DialogResult result = m_OpenCalFileDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxCalibfilePath.Text = m_OpenCalFileDlg.FileName;
                LoadAndTransformImage();
            }
        }

        private void ConvertFloatToGreyLevel(MIL_ID milFloatImage, out MIL_ID milGreyLevelImage)
        {
            MIL_ID MilStatContext = MIL.M_NULL;
            MIL_ID MilStatResult = MIL.M_NULL;
            MIL_ID milImageGreyLvl = MIL.M_NULL;
            MIL_ID milmsk = MIL.M_NULL;

            try
            {
                MIL.MimAlloc(m_MilSystem, MIL.M_STATISTICS_CONTEXT, MIL.M_DEFAULT, ref MilStatContext);
                MIL.MimControl(MilStatContext, MIL.M_STAT_MIN, MIL.M_ENABLE);
                MIL.MimControl(MilStatContext, MIL.M_STAT_MAX, MIL.M_ENABLE);

                MIL.MimAllocResult(m_MilSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT, ref MilStatResult);

                MIL_INT iInSixeX = 0; MIL_INT iInSixeY = 0;
                MIL.MbufInquire(milFloatImage, MIL.M_SIZE_X, ref iInSixeX);
                MIL.MbufInquire(milFloatImage, MIL.M_SIZE_Y, ref iInSixeY);

                MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)iInSixeX, (MIL_INT)iInSixeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref milImageGreyLvl);
                MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)iInSixeX, (MIL_INT)iInSixeY, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref milmsk);

                MIL.MimBinarize(milFloatImage, milmsk, MIL.M_IN_RANGE, float.MinValue, float.MaxValue);

                // set buffer region - like previos MimStat M_MASK feature in previous MIL version
                MIL.MbufSetRegion(milFloatImage, milmsk, MIL.M_DEFAULT, MIL.M_RASTERIZE, MIL.M_DEFAULT);

                // // version (without NaN management)
                double dMin = 0.0;
                double dMax = 0.0;

                MIL.MimStatCalculate(MilStatContext, milFloatImage, MilStatResult, MIL.M_DEFAULT);

                MIL.MimGetResult(MilStatResult, MIL.M_MAX, ref dMax);
                MIL.MimGetResult(MilStatResult, MIL.M_MIN, ref dMin);

                // Calcul fonction affine de conversion (Y = aX + b)
                float a = 1.0f;
                float b = 0.0f;
                if ((dMax - dMin) != 0.0)
                {
                    a = 255.0f / (float)(dMax - dMin);
                    b = -(float)dMin * 255.0f / (float)(dMax - dMin);
                }
                MIL.MimArithMultiple(milFloatImage, a, b, 1, MIL.M_NULL, milImageGreyLvl, MIL.M_MULTIPLY_ACCUMULATE_1 + MIL.M_SATURATION, MIL.M_DEFAULT);


            }
            catch (System.Exception ex)
            {
                if (milImageGreyLvl != MIL.M_NULL)
                {
                    MIL.MbufFree(milImageGreyLvl);
                    milImageGreyLvl = MIL.M_NULL;
                }
                MessageBox.Show($"Error in ConvertFloatToGreyLevel : {ex.Message}");
            }
            finally
            {
                if (milmsk != MIL.M_NULL)
                    MIL.MbufFree(milmsk);

                if (MilStatResult != MIL.M_NULL)
                    MIL.MimFree(MilStatResult);

                if (MilStatContext != MIL.M_NULL)
                    MIL.MimFree(MilStatContext);
            }

            milGreyLevelImage = milImageGreyLvl;

        }

        private void ConvertAndSaveFloatToBmp( MIL_ID milFloatImage, String sFilePathName)
        {
          
            MIL_ID milImageGreyLvl = MIL.M_NULL;
            try
            {
                ConvertFloatToGreyLevel(milFloatImage, out milImageGreyLvl);
                if (milImageGreyLvl != MIL.M_NULL)
                    MIL.MbufExport(sFilePathName + "." + "bmp", MIL.M_BMP, milImageGreyLvl);               
            }
            catch
            {
                MessageBox.Show("Error in ConvertAndSaveFloatToBmp !!");
            }
            if(milImageGreyLvl != MIL.M_NULL)
                MIL.MbufFree(milImageGreyLvl);        
        }

        private void LoadAndTransformImage()
        {
            if (File.Exists(textBoxCalibfilePath.Text) == false || File.Exists(textBoxCalibImage.Text) == false)
                return;

           String sImgExt = Path.GetExtension(textBoxCalibImage.Text);
           m_bUse3DD = false;
           sImgExt = sImgExt.ToLowerInvariant();
           switch (sImgExt)
           {
               case ".3da":
               case ".3di":
                   m_bUse3DD = true;
                   break;
               default: 
                   break;;
            }
          
            try
            {

                // Load Transform
                //
                DMTTransform oTf = new DMTTransform(m_MilSystem, textBoxCalibfilePath.Text);
                // on recupere les donnée de conversion px->microns
                m_dPixelSize = oTf.PixelSize;
                m_dOffsetX_um = oTf.OffsetX_um;
                m_dOffsetY_um = oTf.OffsetY_um;
                textBoxPixelSize.Text = m_dPixelSize.ToString(".###");

                // Load Image to transform 
                //
                MIL_ID milSrcImgToredress = MIL.M_NULL;
                MIL_ID milOutImg = MIL.M_NULL;
                MIL_INT iSizeX = 0;
                MIL_INT iSizeY = 0;

                if (m_bUse3DD)
                {
                    // TO DO : should use UnitySC.Shared.Data.FormatFile.MatrixFloatFile instead
                    // Load 3DD (3da,  3dd, 3di) 
                    using (CFloatDataFile p3DaPicture = new CFloatDataFile())
                    {
                        int nSzX;
                        int nSzY;
                        List<float[]> fDataBuf = p3DaPicture.ReadFromFile(textBoxCalibImage.Text, out nSzX, out nSzY);
                        iSizeX = nSzX;
                        iSizeY = nSzY;

                        MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)iSizeX, (MIL_INT)iSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milSrcImgToredress);      
                        long lCount = 0L;
                        foreach (float[] chunk in fDataBuf)
                        {
                            MIL.MbufPut1d(milSrcImgToredress, lCount, chunk.LongLength, chunk);
                            lCount += chunk.LongLength; 
                        } 
                    }

                    MIL_ID FLOATOutImg = MIL.M_NULL;
                    MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)oTf.OutputSizeX, (MIL_INT)oTf.OutputSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref FLOATOutImg);

                    MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)oTf.OutputSizeX, (MIL_INT)oTf.OutputSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DIB + MIL.M_GDI, ref milOutImg);
                    if ((iSizeX == oTf.OutputSizeX) && (iSizeY == oTf.OutputSizeY))
                    {
                        // on considere que l'on nous donne une image déjà redressée donc on skip le redressement -- (cas particulier de test d'une image standard avec une calib)
                        MIL.MbufCopy(milSrcImgToredress, FLOATOutImg);
                    }
                    else
                    {
                        //oTf.Transform(milSrcImgToredress, FLOATOutImg);

                        MIL.MbufFree(FLOATOutImg);
                        milSrcImgToredress = (oTf.Transform(new MilImage(milSrcImgToredress,true))).MilId;
                        FLOATOutImg = milSrcImgToredress;
                        milSrcImgToredress = MIL.M_NULL;

                    }

                    m_p3DABufDataRedressed = new float[oTf.OutputSizeX * oTf.OutputSizeY];
                    MIL.MbufGet(FLOATOutImg, m_p3DABufDataRedressed); // on recupere direct le buffer en ligne de floatant

                    MIL_ID GreyLvlImg = MIL.M_NULL;
#if DEBUG
                    //if (false)
                    //{
                    //    ConvertFloatToGreyLevel(FLOATOutImg, out GreyLvlImg);
                    //}
                    //else
#endif
                    {
                        float max = -float.MaxValue;
                        float min = float.MaxValue;
                        byte[] pp = new byte[oTf.OutputSizeX * oTf.OutputSizeY];

                        foreach (float f in m_p3DABufDataRedressed)
                        {
                            if (float.IsNaN(f) == false)
                            {
                                if (f > max)
                                    max = f;
                                if (f < min)
                                    min = f;
                            }
                        }

                        long l = 0;
                        float a = 1.0f;
                        float b = 0.0f;
                        if (max != min)
                        {
                            a = 255.0f / (max - min);
                            b = -min * 255.0f / (max - min);
                        }
                        foreach (float f in m_p3DABufDataRedressed)
                        {
                            if (float.IsNaN(f))
                            {
                                pp[l] = 0;
                            }
                            else
                            {
                                pp[l] = (byte)(f * a + b);
                            }

                            l++;
                        }


                        MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)oTf.OutputSizeX, (MIL_INT)oTf.OutputSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref GreyLvlImg);
                        MIL.MbufPut(GreyLvlImg, pp);
                    }
                    if (GreyLvlImg != MIL.M_NULL)
                    {
                        // copy convert to 8Bit DIB image for display
                       MIL.MbufCopy(GreyLvlImg, milOutImg);
                       MIL.MbufFree(GreyLvlImg);
                       GreyLvlImg = MIL.M_NULL;
                    }

                    if (FLOATOutImg != MIL.M_NULL)
                    {
                        MIL.MbufFree(FLOATOutImg);
                        FLOATOutImg = MIL.M_NULL;
                    }
                }
                else
                {
                    // Load Image
                    MIL.MbufDiskInquire(textBoxCalibImage.Text, MIL.M_SIZE_X, ref iSizeX);
                    MIL.MbufDiskInquire(textBoxCalibImage.Text, MIL.M_SIZE_Y, ref iSizeY);
                    MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)iSizeX, (MIL_INT)iSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref milSrcImgToredress);
                    MIL.MbufImport(textBoxCalibImage.Text, MIL.M_DEFAULT, MIL.M_LOAD, m_MilSystem, ref milSrcImgToredress);

                    MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)oTf.OutputSizeX, (MIL_INT)oTf.OutputSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DIB + MIL.M_GDI, ref milOutImg);
                    if ((iSizeX == oTf.OutputSizeX) && (iSizeY == oTf.OutputSizeY))
                    {
                        // on considere que l'on nous donne une image déjà redressée donc on skip le redressement -- (cas particulier de test d'une image standard avec une calib)
                        MIL.MbufCopy(milSrcImgToredress, milOutImg);
                    }
                    else
                    {
                        oTf.Transform(new MilImage(milSrcImgToredress, false), new MilImage(milOutImg, false));
                    }
                }

                //          MIL.MbufExport("redress.bmp", MIL.M_BMP, m_MilSrcImgID);
                //          MIL_ID oid = oTf.Transform(milSrcImgToredress, true);
                //          MIL.MbufExport("oid.bmp",MIL.M_BMP,oid);

                // on recupere l'image redressé pour l'affichage dans le viewer
                Bitmap bmp = null;

                IntPtr hbitmap = MIL.MbufInquire(milOutImg, MIL.M_DIB_HANDLE, MIL.M_NULL);
                bmp = Bitmap.FromHbitmap(hbitmap);

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

                m_rcOldRect = Rectangle.Empty;
                textBoxImgCoord.Text = "";
                textBoxWaferCoord.Text = "";

                m_oViewer.FocusOnMe();
                m_oViewer.FitToScreen();

                if (milOutImg != MIL.M_NULL)
                {
                    MIL.MbufFree(milOutImg);
                    milOutImg = MIL.M_NULL;
                }

                if (milSrcImgToredress != MIL.M_NULL)
                {
                    MIL.MbufFree(milSrcImgToredress);
                    milSrcImgToredress = MIL.M_NULL;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in LoadAndTransformImage : " + ex.Message);
            }
        }

        public void OnDrawCross(double dx, double dy, string sLabel, Color oColor)
        {
            this.SafeInvoke(d => d.DrawCross(dx, dy, sLabel, oColor));
        }
        public void DrawCross(double dx, double dy, string sLabel, Color oColor)
        {
            using (Graphics g = Graphics.FromImage(m_MaskSelect))
            {
                float fSize = (((float)Math.Max(m_oViewer.Image.Width, m_oViewer.Image.Height)) * m_ffactor);
                g.DrawLine(new Pen(oColor, 3), (float)dx - fSize, (float)dy, (float)dx + fSize, (float)dy);
                g.DrawLine(new Pen(oColor, 3), (float)dx, (float)dy - fSize, (float)dx, (float)dy + fSize);

                if (String.IsNullOrEmpty(sLabel) == false)
                {
                    //Write your text.
                    g.DrawString(sLabel, new Font("Arial", 0.75F * fSize, FontStyle.Regular), new SolidBrush(oColor), new PointF((float)dx - (1.7F * fSize), (float)dy + (1.2F * fSize)));
                }

                m_rcOldRect = new Rectangle((int)((float)dx - fSize), (int)((float)dy - fSize), (int)(fSize * 2.0F + 1.0F), (int)(fSize * 2.0F + 1.0F));
            }
        }

        public void RemoveRect(Rectangle rc)
        {
             using (Graphics g = Graphics.FromImage(m_MaskSelect))
             {
                 if (rc != Rectangle.Empty)
                 {
                     rc.Inflate(5, 5);
                     g.SetClip(rc);
                     g.Clear(Color.Transparent);
                     g.ResetClip();
                 }
             }
        }

        private void buttonExportView_Click(object sender, EventArgs e)
        {
            // When user clicks button, show the dialog.
            saveFileDialogExportView.FileName = String.Format("ExportCheckView_{0:yyyy_MM_dd_hh_mm_ss}.png", DateTime.Now);
            saveFileDialogExportView.ShowDialog(); 
        }

        private void saveFileDialogExportView_FileOk(object sender, CancelEventArgs e)
        {
            string name = saveFileDialogExportView.FileName;
            FileStream myFileOut = new FileStream(name, FileMode.OpenOrCreate);
            using (Bitmap target = new Bitmap(m_oViewer.Image.Width, m_oViewer.Image.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver; // this is the default, but just to be clear
                    g.DrawImage(m_oViewer.Image, 0, 0);
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

            if (m_bUse3DD && (m_p3DABufDataRedressed != null))
            {
                string spathdir = Path.GetDirectoryName(name);
                string name3da = Path.GetFileNameWithoutExtension(name);
                name3da += ".3da";
                spathdir += @"\" + name3da;
                // TO DO : should use UnitySC.Shared.Data.FormatFile.MatrixFloatFile instead
                using (CFloatDataFile p3DaPicture = new CFloatDataFile())
                {
                    bool bCompressionFeature = true;
                    p3DaPicture.WriteInFile(spathdir, (int)m_oViewer.Image.Height, (int)m_oViewer.Image.Width, m_p3DABufDataRedressed, bCompressionFeature);           
                }   
            }
        }

        private void checkBoxShowCalibPoints_CheckedChanged(object sender, EventArgs e)
        {
            // to do : work in progess
        }
    }
}
