using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.ImageViewer;

using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;


using UnitySC.PM.PSD.Tools.NanoTopo.NanoViewer.Native;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoViewer.Ole;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoViewer
{
    public delegate void DelegOpenNewFile();

    public partial class NanoViewerForm : Form, IImgViewerStatus
    {
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(byte[] dest, uint[] destlen, byte[] source, uint sourcelen);
     
        public DelegOpenNewFile m_DelegOpenNewFile;

        private string m_sFilePath;
        private string m_sShortFileName;
        private ImgViewer m_oViewer;
        private ImgPreviewer m_oPreviewer;
        private double m_dPixelSize;
        private bool m_bToggleProfileChart;
        private bool m_bProfileMode;
        private MatrixFloatImage m_fHfilterMImg;
        private float m_fDefMax;
        private float m_fDefMin;
        private float m_fDefMean;
        private float m_fDefStdDev;

        private string m_sCurrentDir;
        private string[] m_sAdnFilesInCurrentDir;
        private int m_nCurrentAdnFileIndex;


        private float m_fMinInViewer;
        private float m_fMaxInViewer;
        private bool m_bOpenSuccess = false;

        private PointF m_ptCurrentProfileStart;
        private PointF m_ptCurrentProfileEnd;


        public float XBmpPixel { get; set; }
        public float YBmpPixel { get; set; }
        public int XMouse { get; set; }
        public int YMouse { get; set; }
        public Color BmpPixelColor { get; set; }
        void IImgViewerStatus.DisplayCoordStatus()
        {
            float fVal = 0.0f;
            if (m_bOpenSuccess)
                fVal = m_fHfilterMImg.GetValue(XBmpPixel, YBmpPixel);
            textBoxViewerStatus.Text = String.Format("Img Coord [{0:F1};{1:F1}] in px\t- Val = {2:F2} nm", XBmpPixel, YBmpPixel, fVal);

        }
        void IImgViewerStatus.DisplayTextStatus(string sMessage)
        {
            textBoxViewerStatus.Text = sMessage;
        }


        public float MinInViewer
        {
            set
            {
                m_fMinInViewer = value;
                numericUpDownMINView.Value = (decimal)m_fMinInViewer;
                m_oViewer.FitAfterLoad(false);
                m_oViewer.Image = m_fHfilterMImg.GetImage(m_fMinInViewer, m_fMaxInViewer);
                m_oViewer.FitAfterLoad(true);
            }
            get { return m_fMinInViewer; }
        }

        public float MaxInViewer
        {
            set
            {
                m_fMaxInViewer = value;
                numericUpDownMAXView.Value = (decimal)m_fMaxInViewer;
                m_oViewer.FitAfterLoad(false);
                m_oViewer.Image = m_fHfilterMImg.GetImage(m_fMinInViewer, m_fMaxInViewer);
                m_oViewer.FitAfterLoad(true);
            }
            get { return m_fMaxInViewer; }
        }

        public NanoViewerForm(string[] args)
        {
            InitializeComponent();

            float fZero = 0.0f;
            textBoxProfileSX.Text = fZero.ToString("F");
            textBoxProfileSY.Text = fZero.ToString("F");
            textBoxProfileEX.Text = fZero.ToString("F");
            textBoxProfileEY.Text = fZero.ToString("F");
            m_nCurrentAdnFileIndex = -1;
            m_sCurrentDir = "";

            m_DelegOpenNewFile = new DelegOpenNewFile(OpenNewFile);

            if (args.Length > 0)
            {
                m_sFilePath = args[0];
                m_sShortFileName = Path.GetFileNameWithoutExtension(m_sFilePath);
                m_sCurrentDir = Path.GetDirectoryName(m_sFilePath);
                m_sAdnFilesInCurrentDir = Directory.GetFiles(m_sCurrentDir, "*.adn");
                m_nCurrentAdnFileIndex = 0;

                foreach (string sfile in m_sAdnFilesInCurrentDir)
                {
                    if (sfile == m_sFilePath)
                        break;
                    m_nCurrentAdnFileIndex++;
                }
                if (m_nCurrentAdnFileIndex == m_sAdnFilesInCurrentDir.Length)
                    m_nCurrentAdnFileIndex = 0;

            }
            else
            {
                // choose your file
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "ADN Files (*.adn)|*.adn|TMP Files (*.tmpadn)|*.tmpadn; |All files (*.*)|*.*";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    m_sFilePath = openFileDialog.FileName;
                    m_sShortFileName = Path.GetFileNameWithoutExtension(m_sFilePath);
                    m_sCurrentDir = Path.GetDirectoryName(m_sFilePath);
                    m_sAdnFilesInCurrentDir = Directory.GetFiles(m_sCurrentDir, "*.adn");
                    m_nCurrentAdnFileIndex = 0;

                    foreach (string sfile in m_sAdnFilesInCurrentDir)
                    {
                        if (sfile == m_sFilePath)
                            break;
                        m_nCurrentAdnFileIndex++;
                    }
                    if (m_nCurrentAdnFileIndex == m_sAdnFilesInCurrentDir.Length)
                        m_nCurrentAdnFileIndex = 0;

                }
            }
            if (String.IsNullOrEmpty(m_sFilePath))
            {
                this.Text = "NanoViewer";
                m_sShortFileName = "";
                m_sCurrentDir = "";
            }
            else
                this.Text = "NanoViewer -- " + m_sFilePath;


            // Init viewer panel
            m_oViewer = new ImgViewer();

            m_oViewer.AllowDrop = false;
            m_oViewer.RotationsButtons = false;
            m_oViewer.GifAnimation = false;
            m_oViewer.OpenButton = false;
            m_oViewer.PreviewButton = false;
            m_oViewer.ShowPreview = true;

            m_oPreviewer = new ImgPreviewer();
            tableLayoutPanelViewer.Controls.Add(m_oPreviewer, 1, 0);
            m_oPreviewer.Dock = DockStyle.Fill;

            m_oViewer.AfterNewProfile += new ImgViewer.ImageViewerNewProfileEventHandler(OnNewProfileChart);
            m_oViewer.SetDistantPreviewBox(m_oPreviewer);

            tableLayoutPanelViewer.Controls.Add(m_oViewer, 0, 0);
            tableLayoutPanelViewer.SetRowSpan(m_oViewer, 4);
            m_oViewer.Dock = DockStyle.Fill;


            // fill profile chart with dummy data 
            //             double yValue = 0.02;
            //             Random random = new Random();
            //             for (int pointIndex = 0; pointIndex < 2000; pointIndex++)
            //             {
            //                 yValue = yValue + (random.NextDouble() * 0.01 - 0.005);
            //                 ProfileChart.Series[0].Points.AddXY(pointIndex,yValue);
            //             }

            m_bToggleProfileChart = true;
            m_bProfileMode = false;
            HideProfileChart();
            ProfileChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            ProfileChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            ProfileChart.ChartAreas[0].CursorY.IsUserEnabled = true;
            ProfileChart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            // fill PV chart with dummy data 
            //             yValue = 100.00;
            //             double xValue = 0.00;
            //             for (int pointIndex = 0; pointIndex < 2000; pointIndex++)
            //             {
            //                 xValue = xValue + random.NextDouble()*2;
            //                 yValue = yValue + (random.NextDouble() * -100/2000);
            //                 PVPlotschart.Series[0].Points.AddXY(xValue,yValue);
            //             }
            //             yValue = 100.00;
            //             xValue = 0.00;
            //             for (int pointIndex = 0; pointIndex < 2000; pointIndex++)
            //             {
            //                 xValue = xValue + random.NextDouble() * 0.75;
            //                 yValue = yValue + (random.NextDouble() * -100 / 1000);
            //                 PVPlotschart.Series[1].Points.AddY(yValue);
            //             }

            // Set fast line chart type
            //ProfileChart.Series[0].ChartType = SeriesChartType.FastLine;
            //chart2.Series[0].Points.AddXY(pointIndex, random.Next(5, 75));

            PVPlotschart.ChartAreas[0].AxisY.LabelStyle.Format = "{F}%";
            PVPlotschart.ChartAreas[0].AxisX.LabelStyle.Format = "F";

            m_bOpenSuccess = OpenADNFile(m_sFilePath);
        }

        private List<Point> InterBresenhamline(int x0, int y0, int x1, int y1)
        {
            List<Point> IndexList = new List<Point>();

            int x, y, dx, dy, e;
            dx = x1 - x0; dy = y1 - y0;
            int tmp;
            if (Math.Abs(dy) <= Math.Abs(dx))
            {
                if (x1 < x0)
                {
                    tmp = x0; x0 = x1; x1 = tmp;
                    tmp = y0; y0 = y1; y1 = tmp;
                }

                dx = x1 - x0; dy = y1 - y0;
                if (y0 <= y1)
                {
                    e = -dx;
                    x = x0; y = y0;
                    for (int i = 0; i <= dx; i++)
                    {
                        IndexList.Add(new Point(x, y)); //drawpixel(x, y);
                        x++; e = e + 2 * dy;
                        if (e >= 0)
                        {
                            y++;
                            e = e - 2 * dx;
                        }
                    }
                }
                else
                {
                    e = dx;
                    x = x0; y = y0;
                    for (int i = 0; i <= dx; i++)
                    {
                        IndexList.Add(new Point(x, y)); //drawpixel(x, y);
                        x++; e = e + 2 * dy;
                        if (e <= 0)
                        {
                            --y;
                            e = e + 2 * dx;
                        }
                    }
                }
            }
            else
            {
                if (y1 < y0)
                {
                    tmp = x0; x0 = x1; x1 = tmp;
                    tmp = y0; y0 = y1; y1 = tmp;
                }

                dx = x1 - x0; dy = y1 - y0;
                if (x0 <= x1)
                {
                    e = -dy;
                    x = x0; y = y0;
                    for (int i = 0; i <= dy; i++)
                    {
                        IndexList.Add(new Point(x, y)); //drawpixel(x, y);
                        y++; e = e + 2 * dx;
                        if (e >= 0)
                        {
                            x++;
                            e = e - 2 * dy;
                        }
                    }
                }
                else
                {
                    e = dy;
                    x = x0; y = y0;
                    for (int i = 0; i <= dy; i++)
                    {
                        IndexList.Add(new Point(x, y)); //drawpixel(x, y);
                        y++; e = e + 2 * dx;
                        if (e <= 0)
                        {
                            --x;
                            e = e + 2 * dy;
                        }
                    }
                    // we need to revert point list in order to have x correctly ordered 
                    IndexList.Reverse();
                }
            }

            return IndexList;
        }

        public void OnNewProfileChart(PointF ptStart, PointF ptEnd)
        {
            if (m_bOpenSuccess)
            {
                ProfileChart.Series[0].Points.Clear();

                float zValue = m_fHfilterMImg.GetValue(ptStart.X, ptStart.Y);
                float fDist = 0.0f;
                // ProfileChart.Series[0].Points.AddXY(fDist, zValue);

                List<Point> lidx = InterBresenhamline((int)Math.Round(ptStart.X, 0), (int)Math.Round(ptStart.Y, 0), (int)Math.Round(ptEnd.X, 0), (int)Math.Round(ptEnd.Y, 0));
                for (int i = 1; i < lidx.Count; i++) // Loop through List with for
                {
                    zValue = m_fHfilterMImg.GetValue(lidx[i].X, lidx[i].Y);
                    double distance = Math.Sqrt(Math.Pow(lidx[i].X - lidx[i - 1].X, 2) + Math.Pow(lidx[i].Y - lidx[i - 1].Y, 2));
                    //fDist += (float)(m_dPixelSize * distance);
                    fDist += (float)(distance);
                    ProfileChart.Series[0].Points.AddXY(fDist, zValue);
                }

                ProfileChart.ChartAreas[0].AxisY.LabelStyle.Format = "F";
                ProfileChart.ChartAreas[0].AxisX.LabelStyle.Format = "F";

                textBoxProfileSX.Text = ptStart.X.ToString("F");
                textBoxProfileSY.Text = ptStart.Y.ToString("F");
                textBoxProfileEX.Text = ptEnd.X.ToString("F");
                textBoxProfileEY.Text = ptEnd.Y.ToString("F");

                m_ptCurrentProfileStart = ptStart;
                m_ptCurrentProfileEnd = ptEnd;
            }

            if (m_bToggleProfileChart)
            {
                ShowProfileChart();
                m_bToggleProfileChart = false;
            }
        }

        public void ShowProfileChart()
        {
            tableLayoutPanelViewer.SetRowSpan(m_oViewer, 4);
            ProfileChart.Show();
            ProfileExport.Show();
        }
        public void HideProfileChart()
        {
            ProfileExport.Hide();
            ProfileChart.Hide();
            tableLayoutPanelViewer.SetRowSpan(m_oViewer, 5);
        }

        private void ProfileChart_MouseMove(object sender, MouseEventArgs e)
        {
            //              ProfileChart.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            //              ProfileChart.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            //             double pX = ProfileChart.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            //             double pY = ProfileChart.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor
            //             label1.Text = string.Format("({0}, {1}) ... ({2}, {3})", e.X, e.Y, pX, pY);

        }

        public bool OpenADNFile(string p_sADNPath)
        {
            // check if file exist
            if (!File.Exists(p_sADNPath))
            {
                string sMsg = "This file path {" + p_sADNPath + "} doesn't exist !";
                MessageBox.Show(sMsg, "File Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // check if extension is correct
            string sExt = System.IO.Path.GetExtension(p_sADNPath);
            if (false == (sExt.Equals(".adn", StringComparison.InvariantCultureIgnoreCase) || sExt.Equals(".tmpadn", StringComparison.InvariantCultureIgnoreCase)))
            {
                string sMsg = "This file extension cannot be read {" + p_sADNPath + "} !";
                MessageBox.Show(sMsg, "Wrong File Extension Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (NativeMethods.StgIsStorageFile(p_sADNPath) != 0)
            {
                string sMsg = "This file is not a storage file {" + p_sADNPath + "} !";
                MessageBox.Show(sMsg, "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            bool bRes = true;
            OleStorage storage = OleStorage.CreateInstance(p_sADNPath);
            try
            {
                // ROOT
                OleStream oStreamWaferID = storage.OpenStream("WaferID");
                int nSize = oStreamWaferID.ReadInt();
                byte[] strbuff = oStreamWaferID.ReadBuffer(nSize * sizeof(char));
                //string sWaferID = new string(strbuff);
                string sWaferID = ASCIIEncoding.ASCII.GetString(strbuff);
                textBoxLotID.Text = sWaferID;
                oStreamWaferID.Close();

                OleStream oStreamFoupID = storage.OpenStream("FoupID");
                if (oStreamFoupID != null)
                {
                    int nSize2 = oStreamFoupID.ReadInt();
                    byte[] strbuff2 = oStreamFoupID.ReadBuffer(nSize2 * sizeof(char));
                    //string sWaferID = new string(strbuff);
                    string sFoupID = ASCIIEncoding.ASCII.GetString(strbuff2);
                    textBoxFOUPID.Text = sFoupID;
                    oStreamFoupID.Close();
                }
                else
                    textBoxFOUPID.Text = "-";

                bool bIsDataCompressed = false;
                OleStream oStreamIsCompressData = storage.OpenStream("CompressStatus");
                if (oStreamIsCompressData != null)
                {
                    int nStatus = oStreamIsCompressData.ReadInt();
                    bIsDataCompressed = (nStatus != 0);
                    oStreamIsCompressData.Close();
                }

                OleStream oStreamStatData = storage.OpenStream("StatsData");
                if (oStreamStatData != null)
                {
                    float fPVGlobal = oStreamStatData.ReadFloat();
                    textBoxPVGlobal.Text = fPVGlobal.ToString("N3");
                    float fPeakGlobal = oStreamStatData.ReadFloat();
                    textBoxPeakGlobal.Text = fPeakGlobal.ToString("N3");
                    float fRMSGlobal = oStreamStatData.ReadFloat();
                    textBoxRMSGlobal.Text = fRMSGlobal.ToString("N3");
                    int nfiltertype = oStreamStatData.ReadInt();
                    textBoxFilterType.Text = nfiltertype.ToString();
                    oStreamStatData.Close();
                }
                else
                {
                    textBoxFilterType.Text = "-";
                    textBoxPVGlobal.Text = "-";
                    textBoxPeakGlobal.Text = "-";
                    textBoxRMSGlobal.Text = "-";
                }
                

                OleStream oStreamTHA = storage.OpenStream("THA");
                double dTHA10 = oStreamTHA.ReadDouble();
                textBoxTHA10.Text = dTHA10.ToString("N3");
                double dTHA2 = oStreamTHA.ReadDouble();
                textBoxTHA2.Text = dTHA2.ToString("N3");
                oStreamTHA.Close();

                OleStream oStreamParams = storage.OpenStream("Params");
                m_dPixelSize = oStreamParams.ReadDouble();
                int nBoolUseDisk = oStreamParams.ReadInt();
                bool bUseDisk = (nBoolUseDisk != 0);
                textBoxUseDiskPV.Text = bUseDisk.ToString();
                int nErodeRadiusInGenerateResults = oStreamParams.ReadInt();  // erode radius not use @this time
                double dEdgeEclusion_mm = oStreamParams.ReadDouble();
                textBoxEdgeExclusion.Text = dEdgeEclusion_mm.ToString();

                oStreamParams.Close();

                // Curves PV10
                OleStorage CurvesStoragePV10 = storage.OpenStorage("CurvesPV10");
                if (CurvesStoragePV10 != null)
                {
                    byte[] bufNbPts = CurvesStoragePV10.ReadStream("NbPts");
                    int nNbPoints = BitConverter.ToInt32(bufNbPts, 0);

                    OleStream oStreamData = CurvesStoragePV10.OpenStream("DataPts");
                    if (oStreamData != null)
                    {
                        if (bIsDataCompressed)
                        {
                            uint nBufLen = oStreamData.ReadUInt32();
                            byte[] buffertoDecompress = oStreamData.ReadBuffer((int)nBufLen);
                            byte[] buffer_out = new byte[nNbPoints * 2 * sizeof(float)];
                            uint[] destlen = new uint[1];
                            destlen[0] = (uint)(nNbPoints * 2 * sizeof(float));
                            int nzres = uncompress(buffer_out, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);

                            double xValue = 0.0;
                            double yValue = 0.0;
                            float fValConvert = 0.0f;
                            for (int pointIndex = 0; pointIndex < nNbPoints; pointIndex++)
                            {
                                fValConvert = BitConverter.ToSingle(buffer_out, pointIndex * 2 * sizeof(float));
                                xValue = (double)fValConvert;
                                fValConvert = BitConverter.ToSingle(buffer_out, (pointIndex * 2 + 1)* sizeof(float));
                                yValue = (double)fValConvert;
                                PVPlotschart.Series[0].Points.AddXY(xValue, yValue);
                            }
                        }
                        else
                        {
                            double xValue = 0.0;
                            double yValue = 0.0;
                            for (int pointIndex = 0; pointIndex < nNbPoints; pointIndex++)
                            {
                                xValue = (double)oStreamData.ReadFloat();
                                yValue = (double)oStreamData.ReadFloat();
                                PVPlotschart.Series[0].Points.AddXY(xValue, yValue);
                            }
                        }
                        oStreamData.Close();
                    }
                    else
                        bRes = false;
                    CurvesStoragePV10.Close();
                }


                // Curves PV2
                OleStorage CurvesStoragePV2 = storage.OpenStorage("CurvesPV2");
                if (CurvesStoragePV2 != null)
                {
                    byte[] bufNbPts = CurvesStoragePV2.ReadStream("NbPts");
                    int nNbPoints = BitConverter.ToInt32(bufNbPts, 0);

                    OleStream oStreamData = CurvesStoragePV2.OpenStream("DataPts");
                    if (oStreamData != null)
                    {
                        if (bIsDataCompressed)
                        {
                            uint nBufLen = oStreamData.ReadUInt32();
                            byte[] buffertoDecompress = oStreamData.ReadBuffer((int)nBufLen);
                            byte[] buffer_out = new byte[nNbPoints * 2 * sizeof(float)];
                            uint[] destlen = new uint[1];
                            destlen[0] = (uint)(nNbPoints * 2 * sizeof(float));
                            int nzres = uncompress(buffer_out, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);

                            double xValue = 0.0;
                            double yValue = 0.0;
                            float fValConvert = 0.0f;
                            for (int pointIndex = 0; pointIndex < nNbPoints; pointIndex++)
                            {
                                fValConvert = BitConverter.ToSingle(buffer_out, pointIndex * 2 * sizeof(float));
                                xValue = (double)fValConvert;
                                fValConvert = BitConverter.ToSingle(buffer_out, (pointIndex * 2 + 1) * sizeof(float));
                                yValue = (double)fValConvert;
                                PVPlotschart.Series[1].Points.AddXY(xValue, yValue);
                            }
                        }
                        else
                        {
                            double xValue = 0.0;
                            double yValue = 0.0;
                            for (int pointIndex = 0; pointIndex < nNbPoints; pointIndex++)
                            {
                                xValue = (double)oStreamData.ReadFloat();
                                yValue = (double)oStreamData.ReadFloat();
                                PVPlotschart.Series[1].Points.AddXY(xValue, yValue);
                            }
                        }
                        oStreamData.Close();
                    }
                    else
                        bRes = false;
                    CurvesStoragePV2.Close();
                }
                else
                    bRes = false;

                // HFilter
                OleStorage HFilterStorage = storage.OpenStorage("HFilter");
                if (HFilterStorage != null)
                {
                    OleStream oStreamSize = HFilterStorage.OpenStream("Size");
                    int nWidth = oStreamSize.ReadInt();
                    int nHeight = oStreamSize.ReadInt();
                    oStreamSize.Close();

                    OleStream oStreamStat = HFilterStorage.OpenStream("Stats");
                    double dMax = oStreamStat.ReadDouble();
                    double dMin = oStreamStat.ReadDouble();
                    double dMean = oStreamStat.ReadDouble();
                    double dStdDev = oStreamStat.ReadDouble();
                    oStreamStat.Close();

                    m_fDefMax = (float)dMax;
                    m_fDefMin = (float)dMin;
                    m_fDefMean = (float)dMean;
                    m_fDefStdDev = (float)dStdDev;

                    OleStream oStreamMaskPV = HFilterStorage.OpenStream("MaskPVData");
                  
                    if (oStreamMaskPV != null)
                    {
                        float PV10_TH_nm = oStreamMaskPV.ReadFloat();
                        float PV2_TH_nm = oStreamMaskPV.ReadFloat();
                        int nPVW = oStreamMaskPV.ReadInt();
                        int nPVH = oStreamMaskPV.ReadInt();

                        checkBoxUsePVMask10.Text = "PV10 (>" + PV10_TH_nm.ToString() + "nm)";
                        checkBoxUsePVMask2.Text = "PV2 (>" + PV2_TH_nm.ToString() + "nm)";

                        Bitmap bmp10 = new Bitmap(nPVW, nPVH);
                        using (Graphics grp10 = Graphics.FromImage(bmp10))
                        {
                            grp10.FillRectangle(Brushes.Transparent, 0, 0, nPVW, nPVH);
                        }
                        Bitmap bmp2 = new Bitmap(nPVW, nPVH);
                        using (Graphics grp2 = Graphics.FromImage(bmp2))
                        {
                            grp2.FillRectangle(Brushes.Transparent, 0, 0, nPVW, nPVH);
                        }
                        
                        byte[] bMskPVData = null;
                        if (bIsDataCompressed)
                        {
                            uint nBufLen = oStreamMaskPV.ReadUInt32();
                            byte[] buffertoDecompress = oStreamMaskPV.ReadBuffer((int)nBufLen);
                            bMskPVData = new byte[sizeof(byte) * (nHeight * nWidth)];
                            uint[] destlen = new uint[1];
                            destlen[0] = (uint)(sizeof(byte) * (nHeight * nWidth));
                            int nzres = uncompress(bMskPVData, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                        }
                        else
                        {
                            bMskPVData = oStreamMaskPV.ReadBuffer(sizeof(byte) * (nHeight * nWidth)); 
                        }
                        oStreamMaskPV.Close();
           
                        byte[] nAlphaRGB_10 = new byte[3] { 35, 230, 35 }; // vert 20 nm
                        byte[] nAlphaRGB_2  = new byte[3] { 185, 0, 165 }; // violat 10nm

                        checkBoxUsePVMask10.ForeColor = Color.FromArgb(nAlphaRGB_10[0], nAlphaRGB_10[1], nAlphaRGB_10[2]); ;
                        checkBoxUsePVMask2.ForeColor = Color.FromArgb(nAlphaRGB_2[0], nAlphaRGB_2[1], nAlphaRGB_2[2]);

                         unsafe
                         {
                            //lock the new bitmap in memory
                            BitmapData newData10 = bmp10.LockBits(new Rectangle(0, 0, nPVW, nPVH), ImageLockMode.WriteOnly, bmp10.PixelFormat);
                            BitmapData newData2 = bmp2.LockBits(new Rectangle(0, 0, nPVW, nPVH), ImageLockMode.WriteOnly, bmp2.PixelFormat);

                           // get source bitmap pixel format size
                           int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmp10.PixelFormat);
                           // Get color components count
                           int cCount = nDepth / 8;
                           Parallel.For(0, nPVH, y =>
                           //for (int y = 0; y < nPVH; y++)
                           {
                               //get the data from the new image
                               byte* nRow10 = (byte*)newData10.Scan0 + (y * newData10.Stride);
                               byte* nRow2 = (byte*)newData2.Scan0 + (y * newData2.Stride);

                               for (int x = 0; x < nPVW; x++)
                               {
                                   // convert float to index
                                   byte bymskVal = bMskPVData[y * nPVW + x];
                                   if( (bymskVal & 0x2) != 0 ) 
                                    {
                                       nRow10[x * cCount] = nAlphaRGB_10[2];        //B
                                       nRow10[x * cCount + 1] =  nAlphaRGB_10[1];   //G
                                       nRow10[x * cCount + 2] =  nAlphaRGB_10[0];   //R
                                       nRow10[x * cCount + 3] =  255;               //A
                                    }
                                   
                                   if( (bymskVal & ((byte)0x1)) != 0 )
                                   {
                                       nRow2[x * cCount] = nAlphaRGB_2[2];        //B
                                       nRow2[x * cCount + 1] =  nAlphaRGB_2[1];   //G
                                       nRow2[x * cCount + 2] =  nAlphaRGB_2[0];   //R
                                       nRow2[x * cCount + 3] =  255;               //A
                                   }
                                           
                               }
                           }); // Parallel.For

                           //unlock the bitmaps
                           bmp2.UnlockBits(newData2);
                           bmp10.UnlockBits(newData10);
                        }

                        m_oViewer.AddLayerImage(bmp10, checkBoxUsePVMask10.Checked);
                        m_oViewer.AddLayerImage(bmp2, checkBoxUsePVMask2.Checked);

                        checkBoxUsePVMask2.Enabled = true;
                        checkBoxUsePVMask10.Enabled = true;
                    }
                    else
                    {
                        checkBoxUsePVMask2.Enabled = false;
                        checkBoxUsePVMask10.Enabled = false;
                    }

                    OleStream oStreamData = HFilterStorage.OpenStream("Data");
                    float[] fData = new float[nHeight * nWidth];
                    byte[] byteArray = null;
                    if(bIsDataCompressed)
                    {
                        uint nBufLen = oStreamData.ReadUInt32();
                        byte[] buffertoDecompress = oStreamData.ReadBuffer((int)nBufLen);
                        byteArray = new byte[sizeof(float) * (nHeight * nWidth)];
                        uint[] destlen = new uint[1];
                        destlen[0] = (uint)(sizeof(float) * (nHeight * nWidth));
                        int nzres = uncompress(byteArray, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                    }
                    else
                    {
                        byteArray = oStreamData.ReadBuffer(sizeof(float) * (nHeight * nWidth));
                    }
                    
                    Buffer.BlockCopy(byteArray, 0, fData, 0, byteArray.Length);
                    oStreamData.Close();

                    string sColorMapPath = @"C:\Altasight\Nano\IniRep\colormap0.bmp";
                    m_fHfilterMImg = new MatrixFloatImage(nWidth, nHeight, fData, sColorMapPath);

                    /*   m_fMinInViewer = m_fDefMin;
                         m_fMaxInViewer = m_fDefMax;
                         numericUpDownMAXView.Value = (decimal) m_fMaxInViewer;
                         numericUpDownMINView.Value = (decimal)m_fMinInViewer;
                         m_oViewer.Image = m_fHfilterMImg.GetImage(m_fDefMin, m_fDefMax);*/
                    if (m_bOpenSuccess == false)
                    {
                        m_fMinInViewer = m_fDefMin;//-0.000001f;
                        m_fMaxInViewer = m_fDefMax;//0.000001f;
                        numericUpDownMAXView.Value = (decimal)m_fMaxInViewer;
                        numericUpDownMINView.Value = (decimal)m_fMinInViewer;
                        // m_oViewer.Image = m_fHfilterMImg.GetImage(m_fDefMin, m_fDefMax);
                        m_oViewer.Image = m_fHfilterMImg.GetImage(m_fMinInViewer, m_fMaxInViewer);
                    }
                    else
                    {
                        m_oViewer.Image = m_fHfilterMImg.GetImage(m_fMinInViewer, m_fMaxInViewer);
                    }

                    pictureBoxColorMap.Image = (Image)m_fHfilterMImg.GetColorMapBmp(true);
                    pictureBoxColorMap.Dock = System.Windows.Forms.DockStyle.Fill;

                }
                else
                    bRes = false;
            }
            catch (Exception e)
            {
                string sMsg = String.Format("Exception {0}", e);
                MessageBox.Show(sMsg, "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bRes = false;
            }
            finally
            {
                storage.Close();
            }

            return bRes;

        }

        private void buttonOpenNewAdn_Click(object sender, EventArgs e)
        {
            // test open new adn file

            // choose your file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ADN Files (*.adn)|*.adn|TMP Files (*.tmpadn)|*.tmpadn; |All files (*.*)|*.*";
            if (String.IsNullOrEmpty(m_sCurrentDir) == false)
                openFileDialog.InitialDirectory = m_sCurrentDir;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_sFilePath = openFileDialog.FileName;
                m_sShortFileName = Path.GetFileNameWithoutExtension(m_sFilePath);

                if (m_sCurrentDir != Path.GetDirectoryName(m_sFilePath))
                {
                    m_sCurrentDir = Path.GetDirectoryName(m_sFilePath);
                    m_sAdnFilesInCurrentDir = Directory.GetFiles(m_sCurrentDir, "*.adn");
                    m_nCurrentAdnFileIndex = 0;
                }
                foreach (string sfile in m_sAdnFilesInCurrentDir)
                {
                    if (sfile == m_sFilePath)
                        break;
                    m_nCurrentAdnFileIndex++;
                }
                if (m_nCurrentAdnFileIndex == m_sAdnFilesInCurrentDir.Length)
                    m_nCurrentAdnFileIndex = 0;
            }
            else
            {
                // on a annuler, on garde le précédent fichier ouvert
                return;
            }

            this.Invoke(m_DelegOpenNewFile);
        }

        private void buttonNextAdn_Click(object sender, EventArgs e)
        {
            // next file in directory
            if (m_sAdnFilesInCurrentDir != null)
            {
                int nCount = m_sAdnFilesInCurrentDir.Length;
                if (m_sAdnFilesInCurrentDir.Length > 1)
                {
                    m_nCurrentAdnFileIndex++;
                    if (m_nCurrentAdnFileIndex >= m_sAdnFilesInCurrentDir.Length || m_nCurrentAdnFileIndex < 0)
                    {
                        m_nCurrentAdnFileIndex = 0; // on repart du debut
                    }

                    m_sFilePath = m_sAdnFilesInCurrentDir[m_nCurrentAdnFileIndex];
                    m_sShortFileName = Path.GetFileNameWithoutExtension(m_sFilePath);
                    this.Invoke(m_DelegOpenNewFile);
                }
            }
        }

        private void buttonPreviousAdn_Click(object sender, EventArgs e)
        {
            // previous file in directory
            if (m_sAdnFilesInCurrentDir != null)
            {
                int nCount = m_sAdnFilesInCurrentDir.Length;
                if (m_sAdnFilesInCurrentDir.Length > 1)
                {
                    m_nCurrentAdnFileIndex--;
                    if (m_nCurrentAdnFileIndex >= m_sAdnFilesInCurrentDir.Length || m_nCurrentAdnFileIndex < 0)
                    {
                        m_nCurrentAdnFileIndex = m_sAdnFilesInCurrentDir.Length - 1; // on repart de la fin
                    }

                    m_sFilePath = m_sAdnFilesInCurrentDir[m_nCurrentAdnFileIndex];
                    m_sShortFileName = Path.GetFileNameWithoutExtension(m_sFilePath);
                    this.Invoke(m_DelegOpenNewFile);
                }
            }
        }

        private void NanoViewerForm_Load(object sender, EventArgs e)
        {
            if (m_bOpenSuccess == false)
            {
                // disabled some ctrls
                numericUpDownMAXView.Enabled = false;
                numericUpDownMINView.Enabled = false;
                buttonDef.Enabled = false;
                buttonExportImg.Enabled = false;
                buttonDef.Enabled = false;
                ToggleProfile.Enabled = false;
                SetProfile.Enabled = false;
                ProfileExport.Enabled = false;

                checkBoxUsePVMask2.Enabled = false;
                checkBoxUsePVMask10.Enabled = false;
            }
            m_oViewer.StatusView = this;

            m_oViewer.FitToScreen();

            m_oViewer.Select();
            m_oViewer.FocusOnMe();

        }

        private void numericUpDownMAXView_ValueChanged(object sender, EventArgs e)
        {
            if (m_bOpenSuccess)
            {
                if (m_fMaxInViewer != Convert.ToSingle(numericUpDownMAXView.Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat))
                    MaxInViewer = Convert.ToSingle(numericUpDownMAXView.Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        private void numericUpDownMINView_ValueChanged(object sender, EventArgs e)
        {
            if (m_bOpenSuccess)
            {
                if (m_fMinInViewer != Convert.ToSingle(numericUpDownMINView.Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat))
                    MinInViewer = Convert.ToSingle(numericUpDownMINView.Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        private void buttonDef_Click(object sender, EventArgs e)
        {
            if (m_bOpenSuccess)
            {
                m_fMinInViewer = m_fDefMin;
                m_fMaxInViewer = m_fDefMax;
                numericUpDownMINView.Value = (decimal)m_fMinInViewer;
                numericUpDownMAXView.Value = (decimal)m_fMaxInViewer;
                m_oViewer.FitAfterLoad(false);
                m_oViewer.Image = m_fHfilterMImg.GetImage(m_fMinInViewer, m_fMaxInViewer);
                m_oViewer.FitAfterLoad(true);
            }
        }

        private void ToggleProfile_Click(object sender, EventArgs e)
        {
            if (m_bToggleProfileChart)
            {
                ShowProfileChart();
            }
            else
            {
                HideProfileChart();
            }
            m_bToggleProfileChart = !m_bToggleProfileChart;
        }

        private void SetProfile_Click(object sender, EventArgs e)
        {
            m_bProfileMode = !m_bProfileMode;
            m_oViewer.ShowProfileLine = m_bProfileMode;
            m_oViewer.selectLineMode = m_bProfileMode;
            m_oViewer.SelectMode = m_bProfileMode;
            if (m_bProfileMode)
            {
                HideProfileChart();
                m_bToggleProfileChart = true;
                groupBoxProfileManual.Show();
            }
            else
                groupBoxProfileManual.Hide();
            m_oViewer.InvalidatePanel();
        }

        private void buttonExportImg_Click(object sender, EventArgs e)
        {
            if (m_bOpenSuccess)
            {
                // When user clicks button, show the dialog.
                saveFileDialog1.FileName = m_sShortFileName + "_" + m_fMinInViewer.ToString() + "_" + m_fMaxInViewer.ToString() + ".png";
                saveFileDialog1.ShowDialog();
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            // Get file name.
            string name = saveFileDialog1.FileName;
            FileStream myFileOut = new FileStream(name, FileMode.OpenOrCreate);
            m_oViewer.Image.Save(myFileOut, System.Drawing.Imaging.ImageFormat.Png);
            myFileOut.Close();

        }

        private void ProfileExport_Click(object sender, EventArgs e)
        {
            if (m_bOpenSuccess)
            {
                // When user clicks button, show the dialog.
                saveProfileExportDialog.FileName = m_sShortFileName + "_Profile.csv";
                saveProfileExportDialog.ShowDialog();
            }
        }

        private void saveProfileExportDialog_FileOk(object sender, CancelEventArgs e)
        {
            // Get file name.
            string name = saveProfileExportDialog.FileName;
            try
            {
                using (StreamWriter writer = new StreamWriter(name, false))
                {
                    writer.WriteLine("WaferID : " + textBoxLotID.Text + " ;");  // wafer ID
                    writer.WriteLine("Edge Exclusion  : " + textBoxEdgeExclusion.Text + " mm;");  // EE
                    writer.WriteLine("Profile start (px in image) : [" + m_ptCurrentProfileStart.X.ToString() + " , " + m_ptCurrentProfileStart.Y.ToString() + "];");
                    writer.WriteLine("Profile end (px in image) : [" + m_ptCurrentProfileEnd.X.ToString() + " , " + m_ptCurrentProfileEnd.Y.ToString() + "];");
                    writer.WriteLine("------------");
                    writer.WriteLine("");
                    writer.Flush();


                    writer.WriteLine("Distance (px); zValue (nm);");
                    for (int nIdx = 0; nIdx < ProfileChart.Series[0].Points.Count; nIdx++)
                    {
                        writer.WriteLine(ProfileChart.Series[0].Points[nIdx].XValue.ToString() + ";" + ProfileChart.Series[0].Points[nIdx].YValues[0].ToString() + ";");
                    }
                    writer.Close();
                }
            }
            catch
            {
                //error
                MessageBox.Show("Error : Could not export profile (" + name + ")");
            }
        }

        private void buttonManualProfileValidate_Click(object sender, EventArgs e)
        {
            if (m_bOpenSuccess)
            {
                try
                {
                    float fSx = Convert.ToSingle(textBoxProfileSX.Text);
                    float fSy = Convert.ToSingle(textBoxProfileSY.Text);
                    float fEx = Convert.ToSingle(textBoxProfileEX.Text);
                    float fEy = Convert.ToSingle(textBoxProfileEY.Text);

                    m_oViewer.m_PtProfileStart = new PointF(fSx, fSy);
                    m_oViewer.m_PtProfileEnd = new PointF(fEx, fEy);
                    OnNewProfileChart(m_oViewer.m_PtProfileStart, m_oViewer.m_PtProfileEnd);
                    m_oViewer.InvalidatePanel();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format Error : Some entries are empty or contain non numerical symbols", "Manual profile entries Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Overflow Error : Some entries exceed the range of a float value !", "Manual profile entries Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        public void OpenNewFile()
        {
            this.Cursor = Cursors.WaitCursor;       // Waiting / hour glass 

            // clear profile data
            ProfileChart.Series[0].Points.Clear();
            PVPlotschart.Series[0].Points.Clear();
            PVPlotschart.Series[1].Points.Clear();
            HideProfileChart();
            m_oViewer.RemoveAllLayerImage();

            if (String.IsNullOrEmpty(m_sFilePath))
            {
                this.Text = "NanoViewer";
            }
            else
                this.Text = "NanoViewer -- " + m_sFilePath;

            m_bOpenSuccess = OpenADNFile(m_sFilePath);

            // Enabled OR disabled some ctrls
            numericUpDownMAXView.Enabled = m_bOpenSuccess;
            numericUpDownMINView.Enabled = m_bOpenSuccess;
            buttonDef.Enabled = m_bOpenSuccess;
            buttonExportImg.Enabled = m_bOpenSuccess;
            buttonDef.Enabled = m_bOpenSuccess;
            ToggleProfile.Enabled = m_bOpenSuccess;
            SetProfile.Enabled = m_bOpenSuccess;
            ProfileExport.Enabled = m_bOpenSuccess;

            if (m_bOpenSuccess == false)
            {
                // clear data on screen
                textBoxLotID.Text = "-";
                textBoxFOUPID.Text = "-";
                textBoxTHA10.Text = "0.000";
                textBoxTHA2.Text = "0.000";
                textBoxUseDiskPV.Text = "-";
                textBoxEdgeExclusion.Text = "0.0";
                textBoxFilterType.Text = "-";
                textBoxPVGlobal.Text = "0.000";
                textBoxPeakGlobal.Text = "0.000";
                textBoxRMSGlobal.Text = "0.000";
                m_dPixelSize = 0.0;
                m_oViewer.Image = new Bitmap(100, 100);

                checkBoxUsePVMask2.Enabled = false;
                checkBoxUsePVMask10.Enabled = false;
            }

            m_oViewer.FitToScreen();
            m_oViewer.InvalidatePanel();

            this.Cursor = Cursors.Default;         // Back to normal   
        }

        private void checkBoxUsePVMask2_CheckedChanged(object sender, EventArgs e)
        {
            m_oViewer.DisplayLayerImage(1,checkBoxUsePVMask2.Checked);
            m_oViewer.InvalidatePanel();
        }

        private void checkBoxUsePVMask10_CheckedChanged(object sender, EventArgs e)
        {
            m_oViewer.DisplayLayerImage(0, checkBoxUsePVMask10.Checked);
            m_oViewer.InvalidatePanel();
        }
    }
}
