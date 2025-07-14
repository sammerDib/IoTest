using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace BasicModules.KlarfEditor
{
    public class CWaferResult
    {
        private bool m_bLoaded = false;
        public bool IsLoaded() { return m_bLoaded; }
        public int RoughBinArea = 999999; // this roughbin defines the particular type of defect "Area"

        private CWaferParameters m_WaferParameters;
        private string m_KlarfFileData;
        private Dictionary<int, List<CWaferDefectData>> m_WaferDefectDico = new Dictionary<int, List<CWaferDefectData>>(); // keys == roughbin
                                                                                                                           //List<int> m_sRoughBinKeyList = new List<int>();// unnecessary

        private Dictionary<int, List<RectangleF>> m_WaferDefectRect = new Dictionary<int, List<RectangleF>>();
        private Dictionary<int, DefectValues> m_defectLabelAndColorDico = new Dictionary<int, DefectValues>();
        private SortedList<int, int> m_defectOrder = new SortedList<int, int>();// order for defect caption in report image : SortedList<order, roughbin>

        public CWaferResult(String strKlarfFilePathName, int roughBinArea)
        {
            RoughBinArea = roughBinArea;
            if (!File.Exists(strKlarfFilePathName))
            {
                //throw new FileLoadException("File does not exist");
                // string sMsg = "This file path {" + strKlarfFilePathName + "} doesn't exist !";
                // MessageBox.Show(sMsg, "File Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_bLoaded = false;
                return;
            }
            int pos = strKlarfFilePathName.LastIndexOf('\\');
            String lName = strKlarfFilePathName.Remove(0, pos + 1);
            StreamReader sr = new StreamReader(strKlarfFilePathName);
            try
            {
                m_KlarfFileData = sr.ReadToEnd();
                bool bResultWaferAccepted = SetWaferResult(m_KlarfFileData);
                if (bResultWaferAccepted == false)
                {
                    // MessageBox.Show(String.Format("Could not Load Wafer Results.\nFile may be corrupted check <{0}>", strKlarfFilePathName), "File Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_bLoaded = false;
                    return;
                }
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            m_bLoaded = true;
        }

        public double m_dViewPixelSizeX = 1.0; // for View conversion from px to µm (X axis) 
        public double m_dViewPixelSizeY = 1.0; // for View conversion from px to µm (Y axis)  
        public int m_nViewMarginpx = 12;
        public int m_nViewSizepx = 3024;
        public float m_fViewPenSizepx = 5.0F;

        public bool SetWaferResult(String strData)
        {
            bool lbWaferResultAccepted = false;
            String[] sLine = strData.Split(';');
            String strLine;
            if (m_WaferParameters == null)
                m_WaferParameters = new CWaferParameters();
            for (int i = 0; i < sLine.Length; i++)
            {
                strLine = sLine[i];
                String[] sTab = strLine.Split(' ');
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[0].ToUpper()))
                {
                    // FileVersion
                    if (sTab.Length >= 3)
                        m_WaferParameters.FileVersion = "V" + sTab[1] + "." + sTab[2];
                    else
                        m_WaferParameters.FileVersion = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[1].ToUpper()))
                {
                    // FileTimeStamp
                    if (sTab.Length >= 3)
                        m_WaferParameters.FileTimeStamp = sTab[1] + " " + sTab[2];
                    else
                        m_WaferParameters.FileTimeStamp = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[2].ToUpper()))
                {
                    // TiffFileName
                    if (sTab.Length >= 2)
                        m_WaferParameters.TiffFileName = sTab[1];
                    else
                        m_WaferParameters.TiffFileName = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[3].ToUpper()))
                {
                    // InspectionStationID
                    if (sTab.Length >= 4)
                        m_WaferParameters.InspectionStationID = sTab[1].Replace('\"', ' ').Trim() + "-" + sTab[2].Replace('\"', ' ').Trim() + "-" + sTab[3].Replace('\"', ' ').Trim();
                    else
                        m_WaferParameters.InspectionStationID = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[4].ToUpper()))
                {
                    // SampleType
                    if (sTab.Length >= 2)
                        m_WaferParameters.SampleType = sTab[1];
                    else
                        m_WaferParameters.SampleType = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[5].ToUpper()))
                {
                    // ResultTimeStamp
                    if (sTab.Length >= 3)
                        m_WaferParameters.ResultTimeStamp = sTab[1] + " " + sTab[2];
                    else
                        m_WaferParameters.ResultTimeStamp = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[6].ToUpper()))
                {
                    // LotID
                    if (sTab.Length >= 2)
                        m_WaferParameters.LotID = sTab[1].Replace('\"', ' ').Trim();
                    else
                        m_WaferParameters.LotID = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[7].ToUpper()))
                {
                    // SampleSize
                    if (sTab.Length >= 3)
                        m_WaferParameters.SampleSize = sTab[2];
                    else
                        m_WaferParameters.SampleSize = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[8].ToUpper()))
                {
                    // DeviceID
                    if (sTab.Length >= 2)
                        m_WaferParameters.DeviceID = sTab[1].Replace('\"', ' ').Trim();
                    else
                        m_WaferParameters.DeviceID = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[9].ToUpper()))
                {
                    // SetupID

                    if (sTab.Length >= 2)
                    {
                        for (int j = 1; j < sTab.Length; j++)
                        {
                            m_WaferParameters.SetupID += sTab[j].Replace('\"', ' ').Trim();
                            if (j < sTab.Length - 1)
                                m_WaferParameters.SetupID += " - ";
                        }
                    }
                    else
                        m_WaferParameters.SetupID = "Unknown";

                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[10].ToUpper()))
                {
                    // StepID
                    if (sTab.Length >= 2)
                        m_WaferParameters.StepID = sTab[1].Replace('\"', ' ').Trim();
                    else
                        m_WaferParameters.StepID = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[11].ToUpper()))
                {
                    // SampleOrientationMarkType
                    if (sTab.Length >= 2)
                        m_WaferParameters.SampleOrientationMarkType = sTab[1];
                    else
                        m_WaferParameters.SampleOrientationMarkType = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[12].ToUpper()))
                {
                    // OrientationMarkLocation
                    if (sTab.Length >= 2)
                        m_WaferParameters.OrientationMarkLocation = sTab[1];
                    else
                        m_WaferParameters.OrientationMarkLocation = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[13].ToUpper()))
                {
                    // WaferID
                    if (sTab.Length >= 2)
                        m_WaferParameters.WaferID = sTab[1].Replace('\"', ' ').Trim();
                    else
                        m_WaferParameters.WaferID = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[14].ToUpper()))
                {
                    // SlotID
                    if (sTab.Length >= 2)
                        m_WaferParameters.SlotID = sTab[1];
                    else
                        m_WaferParameters.SlotID = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[15].ToUpper()))
                {
                    // OrientationInstruction
                    if (sTab.Length >= 2)
                        m_WaferParameters.OrientationInstructions = sTab[1].Replace('\"', ' ').Trim();
                    else
                        m_WaferParameters.OrientationInstructions = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[16].ToUpper()))
                {
                    // InspectionOrientation
                    if (sTab.Length >= 2)
                        m_WaferParameters.InspectionOrientation = sTab[1];
                    else
                        m_WaferParameters.InspectionOrientation = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[17].ToUpper()))
                {
                    // SampleTestPlan
                    String[] lStringSeparator = new String[] { "\n" };
                    strLine = strLine.Trim().Replace("\n\r", "");
                    strLine = strLine.Trim().Replace("\r", " ");
                    sTab = strLine.Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                    if (sTab.Length >= 2)
                    {
                        int lNbDies = 0;
                        for (int k = 0; k < sTab.Length; k++)
                        {
                            String[] sTabDiesIndexes = sTab[k].Trim().Split(' ');
                            if (sTabDiesIndexes[0].Contains("SampleTestPlan"))
                            {
                                lNbDies = Convert.ToInt32(sTabDiesIndexes[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                                continue;
                            }
                            Point lNewPoint = new Point(Convert.ToInt32(sTabDiesIndexes[0].Trim(), CultureInfo.InvariantCulture.NumberFormat), Convert.ToInt32(sTabDiesIndexes[1].Trim(), CultureInfo.InvariantCulture.NumberFormat));
                            m_WaferParameters.DiesIndexes.Add(lNewPoint);
                        }
                    }
                    // else
                    //     ;//m_WaferParameters.SampleTestPlan = "Unknown";
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[18].ToUpper()))
                {
                    // DefectRecordSpec
                    String[] lStringSeparator = new String[] { "DEFECTID" };
                    sTab = strLine.Trim().Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                    lStringSeparator = new String[] { " " };
                    String[] sTabHeader = sTab[0].Trim().Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                    if (sTabHeader.Length >= 2)
                    {
                        int lNbColumns = Convert.ToInt32(sTabHeader[1], CultureInfo.InvariantCulture.NumberFormat);
                        String[] strNameTab = sTab[1].Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (strNameTab.Length + 1 == lNbColumns)
                        {
                            m_WaferParameters.ColumnNameList.Add("DEFECTID");
                            m_WaferParameters.ColumnNameList.AddRange(strNameTab);
                        }
                    }
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[19].ToUpper()))
                {
                    // DiePitch
                    if (sTab.Length >= 3)
                        m_WaferParameters.DiePitch = new PointF((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat), (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                    else
                        m_WaferParameters.DiePitch = new PointF(0, 0);
                    continue;
                }
                if (strLine.ToUpper().Contains(CWaferParameters.WafersParametersName[20].ToUpper()))
                {
                    // DieOrigin
                    if (sTab.Length >= 3)
                        m_WaferParameters.DieOrigin = new PointF((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat), (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                    else
                        m_WaferParameters.DieOrigin = new PointF(0, 0);
                    continue;
                }
                if (strLine.ToUpper().Contains("DEFECTLIST"))
                {
                    foreach (List<CWaferDefectData> list in m_WaferDefectDico.Values)
                    {
                        list.Clear();
                    }
                    m_WaferDefectDico.Clear();

                    String[] lSeparator = new String[] { "\r\n" };
                    String[] lWaferData = strLine.Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
                    for (int iWaferDataIndex = 1; iWaferDataIndex < lWaferData.Length; iWaferDataIndex++)
                    {
                        //lock (m_pSynchroDefectDataTypeFileAccess)
                        {
                            int lWaferSize = Convert.ToInt32(m_WaferParameters.SampleSize, CultureInfo.InvariantCulture.NumberFormat) * 1000;
                            CWaferDefectData NewWaferDefectData = new CWaferDefectData(lWaferSize, lWaferData[iWaferDataIndex], m_WaferParameters);
                            if (m_WaferDefectDico.ContainsKey(NewWaferDefectData.RoughBinNumber))
                            {
                                m_WaferDefectDico[NewWaferDefectData.RoughBinNumber].Add(NewWaferDefectData);
                            }
                            else
                            {
                                m_WaferDefectDico.Add(NewWaferDefectData.RoughBinNumber, new List<CWaferDefectData>());
                                m_WaferDefectDico[NewWaferDefectData.RoughBinNumber].Add(NewWaferDefectData);
                            }
                        }
                    }

                    lbWaferResultAccepted = true;
                    break;
                }
            }
            return lbWaferResultAccepted;
        }
        public String LotID
        {
            get { return m_WaferParameters.LotID.Replace("\"", ""); }
        }

        public CWaferParameters WaferParameters
        {
            get { return m_WaferParameters; }
        }

        public Dictionary<int, List<CWaferDefectData>> WaferDefectDico
        {
            get { return m_WaferDefectDico; }
        }

        public bool bIsDieModeUsed
        {
            get { return (m_WaferParameters.DiesIndexes.Count > 1); }
        }

        public void SetDefectLabelsAndColors(Dictionary<int, DefectValues> defectLabelAndColorDico, Dictionary<String, int> defectRoughbin)
        {
            m_defectLabelAndColorDico.Clear();
            m_defectOrder.Clear();

            // perform the link between bin array and additionnal array to set a color with a RoughBin Value
            int Order = 0;
            foreach (int roughbinNumber in defectRoughbin.Values)
            {
                DefectValues values;
                if (defectLabelAndColorDico.TryGetValue(Order, out values))
                {
                    m_defectLabelAndColorDico.Add(Order, values);
                    m_defectOrder.Add(Order++, roughbinNumber);
                }
            }
        }

        private void DrawWaferShape(Graphics p_gImage, int p_nMarginpx, int p_nSizepx, float p_dDiamNotchpx, float p_fViewPenSizepx)
        {
            Rectangle WaferRectImage = new Rectangle(p_nMarginpx, p_nMarginpx, p_nSizepx - 2 * p_nMarginpx, p_nSizepx - 2 * p_nMarginpx); // pixel margin
            Pen MyWhitePen = new Pen(Color.White, p_fViewPenSizepx);
            p_gImage.DrawEllipse(MyWhitePen, WaferRectImage);
            float Cx = p_nMarginpx + (float)WaferRectImage.Width * 0.5F;
            float Cy = p_nMarginpx + (float)WaferRectImage.Height * 0.5F;

            int nSampleOrientationMarkType = 0; //0 notch | 1 flat | 2 double flat

            if (string.Equals(WaferParameters.SampleOrientationMarkType, "NOTCH", StringComparison.CurrentCultureIgnoreCase))
                nSampleOrientationMarkType = 0; //0 notch |
            else if (string.Equals(WaferParameters.SampleOrientationMarkType, "FLAT", StringComparison.CurrentCultureIgnoreCase))
                nSampleOrientationMarkType = 1; // 1 flat 
            else if (string.Equals(WaferParameters.SampleOrientationMarkType, "DFLAT", StringComparison.CurrentCultureIgnoreCase))
                nSampleOrientationMarkType = 2; // 2 double flat 
            else if (false == Int32.TryParse(WaferParameters.SampleOrientationMarkType, out nSampleOrientationMarkType)) // certaine version de klarf (non compatible klarity n'utilise qu'un ID)
                nSampleOrientationMarkType = 0; //par défaut NOTCH


            switch (nSampleOrientationMarkType)
            {
                case 0: // notch
                    {
                        RectangleF rectNotch = new RectangleF((Cx - p_dDiamNotchpx * 0.5F), (p_nSizepx - p_dDiamNotchpx * 0.5F - p_nMarginpx), p_dDiamNotchpx, p_dDiamNotchpx);
                        p_gImage.FillEllipse(Brushes.Black, rectNotch);
                        float startAngle = 180.0F;
                        float sweepAngle = 180.0F;
                        p_gImage.DrawArc(MyWhitePen, rectNotch, startAngle, sweepAngle);
                    }
                    break;
                case 1: // Flat
                    {  // draw "meplat" -- grosso merdo 2.6% du diametre du wafer bouffer par le méplat.
                        float p = 2.6F / 100.0F;
                        float g = p_nMarginpx + ((float)WaferRectImage.Height * (1.0F - p));
                        float r = (float)WaferRectImage.Width * 0.5F;
                        float zeta = r * r - (Cx * Cx) - (Cy * Cy) + g * (2.0F * Cy - g); // intersection droite et cercle (x-Cx)² + (y-Cy)² = r² (où y=g=(1 - p)2r)
                        float Discriminant = 4.0F * (Cx * Cx + zeta);
                        float x1 = (2.0F * Cx - (float)Math.Sqrt((double)Discriminant)) / 2.0F;
                        float x2 = (2.0F * Cx + (float)Math.Sqrt((double)Discriminant)) / 2.0F;

                        // point intersection meplat et cercle wafer
                        PointF TLm = new PointF(x1, g);
                        PointF TRm = new PointF(x2, g);
                        PointF BLm = new PointF(x1, p_nSizepx - 1.0F);
                        PointF BRm = new PointF(x2, p_nSizepx - 1.0F);
                        p_gImage.FillRectangle(Brushes.Black, x1, g, (float)p_nSizepx, (float)p_nSizepx);
                        p_gImage.DrawLine(MyWhitePen, TLm, TRm);
                    }
                    break;
                case 2: // double flat
                    {
                        // draw "meplat" Bas -- grosso merdo 2.6% du diametre du wafer bouffer par le méplat.
                        //
                        float p = 2.6F / 100.0F;
                        float g = p_nMarginpx + ((float)WaferRectImage.Height * (1.0F - p));
                        float r = (float)WaferRectImage.Width * 0.5F;
                        float zeta = r * r - (Cx * Cx) - (Cy * Cy) + g * (2.0F * Cy - g); // intersection droite et cercle (x-Cx)² + (y-Cy)² = r² (où y=g=(1 - p)2r)
                        float Discriminant = 4.0F * (Cx * Cx + zeta);
                        float x1 = (2.0F * Cx - (float)Math.Sqrt((double)Discriminant)) / 2.0F;
                        float x2 = (2.0F * Cx + (float)Math.Sqrt((double)Discriminant)) / 2.0F;

                        // point intersection meplat et cercle wafer
                        PointF TLmx = new PointF(x1, g);
                        PointF TRmx = new PointF(x2, g);
                        PointF BLmx = new PointF(x1, p_nSizepx - 1.0F);
                        PointF BRmx = new PointF(x2, p_nSizepx - 1.0F);
                        p_gImage.FillRectangle(Brushes.Black, x1, g, (float)p_nSizepx, (float)p_nSizepx);
                        p_gImage.DrawLine(MyWhitePen, TLmx, TRmx);

                        // draw "meplat" Gauche -- grosso merdo 1% du diametre du wafer bouffer par le méplat.
                        //
                        p = 1.0F / 100.0F;
                        g = p_nMarginpx + ((float)WaferRectImage.Width * p);
                        r = (float)WaferRectImage.Height * 0.5F;
                        zeta = r * r - (Cy * Cy) - (Cx * Cx) + g * (2.0F * Cx - g); // intersection droite et cercle (x-Cx)² + (y-Cy)² = r² (où x=g=p2r)
                        Discriminant = 4.0F * (Cy * Cy + zeta);
                        float y1 = (2.0F * Cy - (float)Math.Sqrt((double)Discriminant)) / 2.0F;
                        float y2 = (2.0F * Cy + (float)Math.Sqrt((double)Discriminant)) / 2.0F;

                        // point intersection meplat et cercle wafer
                        PointF TLmy = new PointF(1.0F, y1);
                        PointF TRmy = new PointF(g, y1);
                        PointF BLmy = new PointF(1.0F, y2);
                        PointF BRmy = new PointF(g, y2);
                        p_gImage.FillRectangle(Brushes.Black, 1.0F, y1, (float)g, (y2 - y1));
                        p_gImage.DrawLine(MyWhitePen, TRmy, BRmy);

                    }
                    break;
            }
        }

        /// <summary>
        /// Dessine l'image du wafer avec les défauts en haut à gauche de l'image
        /// </summary>
        public void DrawWaferDefects(Graphics p_gImage, int p_nMarginpx, int p_nSizepx, float p_dDiamNotchpx, float p_fViewPenSizepx, int p_squareDefectSizepx)
        {
            DrawWaferShape(p_gImage, p_nMarginpx, p_nSizepx, p_dDiamNotchpx, p_fViewPenSizepx);
            // get wafer diameter in image in pixels
            float diameterInImage = p_nSizepx - 2 * p_nMarginpx;
            // draw defects
            //int Order = 0;
            foreach (int order in m_defectOrder.Keys)
            {
                int roughBin = m_defectOrder[order];
                // get color and label for this type of defect                
                string label = m_defectLabelAndColorDico[order].Label;
                Color color = m_defectLabelAndColorDico[order].Color;
                using (Brush brush = new SolidBrush(color))
                {
                    List<CWaferDefectData> defectList;
                    if (m_WaferDefectDico.TryGetValue(roughBin, out defectList))
                    {
                        foreach (CWaferDefectData defect in defectList)
                        {
                            // position du défaut dans l'image
                            float xPos = (float)(defect.GetXPosBetween0and1() * diameterInImage) + p_nMarginpx - p_squareDefectSizepx / 2;
                            float yPos = (float)(diameterInImage - defect.GetYPosBetween0and1() * diameterInImage) + p_nMarginpx - p_squareDefectSizepx / 2;
                            // dessin du défaut
                            p_gImage.FillRectangle(brush, xPos, yPos, p_squareDefectSizepx, p_squareDefectSizepx);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dessine la légende (labels, couleurs et count) des défauts dans le rectangle passé en paramètre
        /// </summary>
        public void DrawDefectCaption(Graphics gfx, Rectangle rect)
        {
            float startPosColorX = rect.X;
            float startPosCaptionX = startPosColorX + rect.Width * 0.05f;
            float startPosValueX = startPosCaptionX + rect.Width * 0.8f;
            float startPosY = rect.Y;
            float deltaY = (float)rect.Height / 10f;
            float fontSize = deltaY / 2.4f;

            if (m_defectOrder.Keys.Count > 10)
            {
                deltaY = (float)rect.Height / (float)m_defectOrder.Keys.Count;
                fontSize = deltaY / 2.4f;
            }

            using (Font fontReg = new System.Drawing.Font("Arial", fontSize, FontStyle.Regular))
            {
                using (Font fontBold2 = new System.Drawing.Font("Arial", fontSize, FontStyle.Bold))
                {
                    Brush brushWhite = Brushes.White;

                    float squareSize = gfx.MeasureString("S", fontReg).Height; // le carré de couleur est de la taille du texte


                    //OPI m_defectOrder est vide : il faut le remplir. 
                    foreach (int order in m_defectOrder.Keys)
                    {
                        int roughBin = m_defectOrder[order];
                        string defectLabel = m_defectLabelAndColorDico[order].Label;
                        // label exception : do not draw haze label !! (demande de YTO)
                        if (String.Equals(defectLabel, "haze", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;// skip this label
                        }

                        // draw label
                        Brush brushColor = new SolidBrush(m_defectLabelAndColorDico[order].Color);
                        // carré de couleur
                        gfx.FillRectangle(brushColor, startPosColorX, startPosY, squareSize, squareSize);
                        // label
                        gfx.DrawString(defectLabel, fontReg, brushWhite, startPosCaptionX, startPosY);
                        // count
                        int defectCount = 0;
                        List<CWaferDefectData> defectList;
                        if (m_WaferDefectDico.TryGetValue(roughBin, out defectList))
                        {
                            defectCount = defectList.Count;
                        }
                        gfx.DrawString(": " + defectCount.ToString(), fontBold2, brushWhite, startPosValueX, startPosY);

                        startPosY += deltaY;
                    }
                }
            }

        }

        /// <summary>
        /// Dessine l'histogramme des diametres des défauts dans le rectangle passé en paramètre
        /// </summary>
        public void DrawBarChart(Graphics gfx, Rectangle rect, Pen normalPen, Font font, Brush highlightBrush, Brush axisBrush, Brush barBrush, int iNumberOfBars)
        {
            List<double> diameters = new List<double>();
            double dMinDiameterAuthorized = 0.01;//if we put 0 the logaritm won't be calculate and it will generate errors
            double dMin = double.MaxValue, dMax = dMinDiameterAuthorized;

            List<int> roughBinList = new List<int>();

            // get all defects and min/max values
            foreach (int defectRoughbin in m_WaferDefectDico.Keys)
            {
                if (defectRoughbin == RoughBinArea)
                    continue;

                List<CWaferDefectData> defectList = m_WaferDefectDico[defectRoughbin];
                foreach (CWaferDefectData defect in defectList)
                {
                    diameters.Add(defect.DiameterSize);
                    if (defect.DiameterSize < dMin)
                        dMin = defect.DiameterSize >= dMinDiameterAuthorized ? defect.DiameterSize : dMinDiameterAuthorized;
                    if (defect.DiameterSize > dMax)
                        dMax = defect.DiameterSize;
                }
            }

            // chart cannot be drawn if there is no default
            if (diameters.Count == 0)
                return;

            // special case if Min >= Max
            if (dMin >= dMax)
            {
                dMax = dMin + 1;
            }
            //dMin = dMin - 1; // met un petit decalage dechelle pour voir la première série de count 

            double logMin = Math.Log10(dMin);
            double logMax = Math.Log10(dMax);

            double[] xAxisBigScale = new double[8]; // big scale is for X axis graduation
            xAxisBigScale[0] = dMin;
            xAxisBigScale[7] = dMax;
            double scaleStep = (logMax - logMin) / 7;
            for (int i = 1; i < 7; i++)
            {
                xAxisBigScale[i] = FindScale(Math.Pow(10, logMin + scaleStep * i), true);
            }

            // set dicoCountPerBar that contains all bar count
            Dictionary<double, int> dicoCountPerBar = new Dictionary<double, int>();// key = min value of the bar interval, value = count
            double[] xAxisLittleScale = new double[iNumberOfBars];// contains min values of intervals
            double littleStep = (logMax - logMin) / iNumberOfBars;
            for (int i = 0; i < iNumberOfBars; i++)
            {
                xAxisLittleScale[i] = Math.Pow(10, logMin + littleStep * i);
                dicoCountPerBar.Add(xAxisLittleScale[i], 0);
            }

            // get all count per internal
            foreach (double diameter in diameters)
            {
                int pos = 0;
                while (pos < iNumberOfBars && diameter > xAxisLittleScale[pos])
                    pos++;
                if (pos == 0)
                    pos = 1;
                dicoCountPerBar[xAxisLittleScale[pos - 1]]++; // this diameter is in this bar
            }

            // max bar count value
            int maxCount = 0;
            foreach (int count in dicoCountPerBar.Values)
            {
                if (count > maxCount)
                    maxCount = count;
            }

            // max size for a string
            SizeF stringSize = gfx.MeasureString("9999", font);

            // pixel values
            float chartStartX = (int)(rect.X + stringSize.Height + stringSize.Width);
            float chartStartY = (int)rect.Y;
            float chartMaxWidth = rect.Width - (stringSize.Height + stringSize.Width);
            int chartBarStep = Math.Max((int)(chartMaxWidth / iNumberOfBars), 1);
            float chartWidth = (int)(chartBarStep * iNumberOfBars);
            float chartHeight = (int)(rect.Height - 2 * stringSize.Height);


            // draw axes
            gfx.DrawLine(normalPen, chartStartX, chartStartY, chartStartX, chartStartY + chartHeight);
            gfx.DrawLine(normalPen, chartStartX, chartStartY + chartHeight, chartStartX + chartWidth + 2 * normalPen.Width, chartStartY + chartHeight);

            // draw X axis(diameters) graduation
            float ratio = chartWidth / (float)(logMax - logMin);
            for (int i = 1; i < 7; i++)
            {
                if (xAxisBigScale[i] <= xAxisBigScale[0] || xAxisBigScale[i] >= xAxisBigScale[7])
                    continue;// do not draw scale out of range
                float figurePos = chartStartX + (int)(ratio * (Math.Log10(xAxisBigScale[i]) - logMin));
                gfx.DrawLine(normalPen, figurePos, chartStartY + chartHeight, figurePos, chartStartY + chartHeight + 5 * normalPen.Width);
                string sFigure = xAxisBigScale[i].ToString();

                gfx.DrawString(sFigure, font, axisBrush, figurePos - gfx.MeasureString(sFigure, font).Width / 2, chartStartY + chartHeight + 5 * normalPen.Width);
            }

            // highlight captions of X axis ("Diameter (µm)", min and max values)
            string sDiameter = "Size (µm)";
            stringSize = gfx.MeasureString(sDiameter, font);
            gfx.DrawString(sDiameter, font, highlightBrush, chartStartX + chartWidth / 2 - stringSize.Width / 2, chartStartY + chartHeight + stringSize.Height);
            string sMin = dMin > 1 ? ((int)dMin).ToString() : dMin.ToString();
            stringSize = gfx.MeasureString(sMin, font);
            gfx.DrawString(sMin, font, highlightBrush, chartStartX - stringSize.Width / 2, chartStartY + chartHeight + stringSize.Height);
            string sMax = dMax > 1 ? ((int)dMax + 1).ToString() : dMax.ToString();
            stringSize = gfx.MeasureString(sMax, font);
            gfx.DrawString(sMax, font, highlightBrush, chartStartX + chartWidth - stringSize.Width / 2, chartStartY + chartHeight + stringSize.Height);


            // draw axis Y (count) graduation
            int firstGraduation = (int)FindScale((double)maxCount / 2, true);
            int secondGraduation = firstGraduation * 2;
            if (maxCount <= 1)
            {
                firstGraduation = 1;
                secondGraduation = 0;
            }
            float firstPosY = chartStartY + (1 - (float)firstGraduation / (float)maxCount) * chartHeight;
            float secondPosY = chartStartY + (1 - (float)secondGraduation / (float)maxCount) * chartHeight;
            float startX = chartStartX - 5 * normalPen.Width;
            gfx.DrawLine(normalPen, startX, firstPosY, chartStartX, firstPosY);
            gfx.DrawLine(normalPen, startX, secondPosY, chartStartX, secondPosY);
            string sFirst = firstGraduation.ToString();
            stringSize = gfx.MeasureString(sFirst, font);
            gfx.DrawString(sFirst, font, axisBrush, startX - stringSize.Width - 1, firstPosY - stringSize.Height / 2);
            string sSecond = secondGraduation.ToString();
            stringSize = gfx.MeasureString(sSecond, font);
            gfx.DrawString(sSecond, font, axisBrush, startX - stringSize.Width - 1, secondPosY - stringSize.Height / 2);
            //write "Count" horizontally
            gfx.RotateTransform(-90);
            string sCount = "Count";
            stringSize = gfx.MeasureString(sCount, font);
            gfx.DrawString(sCount, font, highlightBrush, -chartStartY - chartHeight / 2 - stringSize.Width / 2, rect.X + 1);
            gfx.RotateTransform(90); // rotation back to draw the rest of the graphic

            // draw bars
            float barStartX = chartStartX + normalPen.Width;
            for (int i = 0; i < iNumberOfBars; i++)
            {
                if (dicoCountPerBar[xAxisLittleScale[i]] > 0)
                {
                    float barHeight = (int)Math.Max(2, chartHeight * (float)dicoCountPerBar[xAxisLittleScale[i]] / (float)maxCount);
                    float barStartY = chartStartY + chartHeight - barHeight - 1;

                    gfx.FillRectangle(barBrush, barStartX, barStartY, chartBarStep, barHeight);
                }
                barStartX += chartBarStep;
            }

        }

        /// <summary>
        /// Dessine l'histogramme des défauts de type Area dans le rectangle passé en paramètre
        /// </summary>
        public void DrawAreaChart(Graphics gfx, Rectangle rect, Pen normalPen, Font font, Brush highlightBrush, Brush axisBrush)
        {
            if (m_WaferDefectDico.ContainsKey(RoughBinArea))
            {
                // calculate the total of area defect in mm2
                double totalAreaUm2 = 0;
                foreach (CWaferDefectData defect in m_WaferDefectDico[RoughBinArea])
                {
                    totalAreaUm2 += defect.DefectArea;
                }
                double totalAreaMm2 = totalAreaUm2 / 1.0E6;
                if (totalAreaMm2 > 10000)
                    totalAreaMm2 = 10000;

                // max size for a string
                SizeF stringSize = gfx.MeasureString("10000", font);

                // pixel values
                const int nbOfScale = 3;
                int chartStartX = (int)(rect.X + stringSize.Width);
                int chartWidth = (int)(rect.Width - (stringSize.Height + stringSize.Width));
                int topHeight = (int)(1.5 * stringSize.Height);
                int botHeight = (int)(2 * stringSize.Height);
                int chartMaxHeight = (int)(rect.Height - (topHeight + botHeight));
                int deltaYscale = (int)(chartMaxHeight / nbOfScale);
                int chartHeight = nbOfScale * deltaYscale;
                int chartStartY = (int)(rect.Y + (chartMaxHeight - chartHeight) + topHeight);

                // draw axes
                gfx.DrawLine(normalPen, chartStartX + chartWidth, chartStartY, chartStartX + chartWidth, chartStartY + chartHeight);
                gfx.DrawLine(normalPen, chartStartX, chartStartY + chartHeight, chartStartX + chartWidth, chartStartY + chartHeight);

                int yValue = 1;
                int xPos1 = (int)(chartStartX + chartWidth - 4 * normalPen.Width);
                int xPos2 = (int)(chartStartX + chartWidth + 5 * normalPen.Width);
                for (int i = 0; i <= nbOfScale; i++)
                {
                    int yPos = (int)(chartStartY + chartHeight - i * deltaYscale);
                    gfx.DrawLine(normalPen, xPos1, yPos, xPos2, yPos);
                    gfx.DrawString(yValue.ToString(), font, axisBrush, xPos2, yPos - stringSize.Height / 2);

                    yValue *= 10;
                }
                int yPosCaption = (int)(chartStartY + 1.5 * deltaYscale - stringSize.Height / 2);
                gfx.DrawString("mm²", font, highlightBrush, xPos2, yPosCaption);
                gfx.DrawString("Area", font, highlightBrush, chartStartX + chartWidth, rect.Y);

                // draw area rectangle
                Color areaColor = Color.Red;
                if (m_defectLabelAndColorDico.ContainsKey(RoughBinArea))
                {
                    areaColor = m_defectLabelAndColorDico[RoughBinArea].Color;
                }

                using (Brush areaBrush = new SolidBrush(areaColor))
                {
                    float barHeight = (float)Math.Max(Math.Log10(totalAreaMm2) * deltaYscale, 1);
                    gfx.FillRectangle(areaBrush, chartStartX, chartStartY + chartHeight - barHeight, chartWidth, barHeight);
                }
            }
        }


        /// <summary>
        /// Retourne l'echelle arrondie la plus proche (inférieure si bFloorValue == true, supérieure sinon).
        /// Exemples : 452-> 400 ou 500(si bFloorValue == false) ; 0.001345 -> 0.001 ou 0.002(si bFloorValue == false)
        /// </summary>
        public static double FindScale(double dValue, bool bFloorValue)
        {
            double dNewValue = 0;
            if (dValue < 1)
            {
                int multiplier = 10;
                int d = (int)(dValue * multiplier);
                while (d < 1)
                {
                    multiplier *= 10;
                    d = (int)(dValue * multiplier);
                }
                dNewValue = (double)(d + (bFloorValue ? 0 : 1)) / (double)multiplier;
            }
            else
            {
                int divisor = 1;
                int d = (int)dValue;
                while (d > 10)
                {
                    divisor *= 10;
                    d = (int)(dValue / (double)divisor);
                }
                dNewValue = (d + (bFloorValue ? 0 : 1)) * divisor;
            }
            return dNewValue;
        }

        #region Unused functions
        //public Bitmap GetThumbnail() //NOTE DE RTI normalement ici on devrait pas avoir à s'en servir
        //{
        //    int nMarginpx = 1;
        //    int nThumbSizepx = 256;
        //    Bitmap ThumbBmp = new Bitmap(nThumbSizepx, nThumbSizepx);
        //    Graphics gImage = Graphics.FromImage(ThumbBmp);
        //    gImage.FillRectangle(Brushes.Black, 0, 0, ThumbBmp.Width, ThumbBmp.Height);

        //    float dDiamNotchpx = 3.0F;
        //    DrawWaferShape(gImage, nMarginpx, nThumbSizepx, dDiamNotchpx, 2.0F);

        //    Rectangle WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nThumbSizepx - 2 * nMarginpx, nThumbSizepx - 2 * nMarginpx); // pixel margin
        //    float Cx = nMarginpx + (float)WaferRectImage.Width * 0.5F;
        //    float Cy = nMarginpx + (float)WaferRectImage.Height * 0.5F;

        //    //
        //    // On dessine les defects
        //    //
        //    double lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / Convert.ToInt32(WaferParameters.SampleSize, CultureInfo.InvariantCulture.NumberFormat) / 1000;
        //    double lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / Convert.ToInt32(WaferParameters.SampleSize, CultureInfo.InvariantCulture.NumberFormat) / 1000;
        //    foreach (int sRoughBin in m_WaferDefectDico.Keys)
        //    {
        //        foreach (CWaferDefectData lWaferDefect in WaferDefectDico[sRoughBin])
        //        {
        //            if (!bIsDieModeUsed)
        //            {

        //                double lXRelatifFromWaferCenter = lWaferDefect.XRelatifFromWaferCenter;
        //                double XPos = lWaferDefect.XRelatifFromWaferCenter * lXConversionMicronToPixelAdjusted + Cx;
        //                double YPos = lWaferDefect.YRelatifFromWaferCenter * -1.0 * lYConversionMicronToPixelAdjusted + ThumbBmp.Height - Cy;
        //                Brush lBrush = new Pen(lWaferDefect.MyColor).Brush;
        //                Size lDefectSizeDisplayed;
        //                lDefectSizeDisplayed = new Size((int)lWaferDefect.XSize, (int)lWaferDefect.YSize); // TODO HUGO /!\ ici on utilisé les bon size voir comment on peux les gerer ?
        //                // par defaut en attendant on utilise la taille donnée 

        //                double dSizeX = lDefectSizeDisplayed.Width * lXConversionMicronToPixelAdjusted;
        //                if (dSizeX <= 0.0)
        //                    dSizeX = 1.0;
        //                double dSizeY = lDefectSizeDisplayed.Height * lYConversionMicronToPixelAdjusted;
        //                if (dSizeY <= 0.0)
        //                    dSizeY = 1.0;
        //                gImage.FillRectangle(lBrush, (float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY);
        //            }
        //            else
        //            {
        //                double iVectorDieOriginX = WaferParameters.DieOrigin.X - Convert.ToInt32(WaferParameters.SampleSize, CultureInfo.InvariantCulture.NumberFormat) * 1000 / 2;
        //                double iVectorDieOriginY = WaferParameters.DieOrigin.Y - Convert.ToInt32(WaferParameters.SampleSize, CultureInfo.InvariantCulture.NumberFormat) * 1000 / 2;
        //                double XPos = (lWaferDefect.XIndex * WaferParameters.DiePitch.X + iVectorDieOriginX + (double)lWaferDefect.XRelatif - WaferParameters.DiePitch.X / 2) * lXConversionMicronToPixelAdjusted + Cx;
        //                double YPos = (lWaferDefect.YIndex * -1 * WaferParameters.DiePitch.Y + iVectorDieOriginY - (double)lWaferDefect.YRelatif + WaferParameters.DiePitch.Y / 2) * lYConversionMicronToPixelAdjusted + ThumbBmp.Height - Cy;
        //                Brush lBrush = new Pen(lWaferDefect.MyColor).Brush;
        //                Size lDefectSizeDisplayed;
        //                lDefectSizeDisplayed = new Size((int)lWaferDefect.XSize, (int)lWaferDefect.YSize);// TODO HUGO /!\ ici on utilisé les bon size voir comment on peux les gerer ?
        //                // par defaut en attendant on utilise la taille donnée 
        //                double dSizeX = lDefectSizeDisplayed.Width * lXConversionMicronToPixelAdjusted;
        //                if (dSizeX <= 0.0)
        //                    dSizeX = 1.0;
        //                double dSizeY = lDefectSizeDisplayed.Height * lYConversionMicronToPixelAdjusted;
        //                if (dSizeY <= 0.0)
        //                    dSizeY = 1.0;
        //                gImage.FillRectangle(lBrush, (float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY);
        //            }
        //        }
        //    }

        //    return ThumbBmp;

        //}

        //public Bitmap GetView(Dictionary<int, bool> bDisplayedLayerDico)
        //{
        //    int nMarginpx = m_nViewMarginpx;
        //    int nSizepx = m_nViewSizepx;

        //    Bitmap ViewBmp = new Bitmap(nSizepx, nSizepx);
        //    Graphics gImage = Graphics.FromImage(ViewBmp);
        //    gImage.FillRectangle(Brushes.Black, 0, 0, ViewBmp.Width, ViewBmp.Height);

        //    Rectangle WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nSizepx - 2 * nMarginpx, nSizepx - 2 * nMarginpx); // pixel margin
        //    double lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / Convert.ToInt32(WaferParameters.SampleSize, CultureInfo.InvariantCulture.NumberFormat) / 1000;
        //    double lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / Convert.ToInt32(WaferParameters.SampleSize, CultureInfo.InvariantCulture.NumberFormat) / 1000;
        //    m_dViewPixelSizeX = 1.0 / (lXConversionMicronToPixelAdjusted * 1000);
        //    m_dViewPixelSizeY = 1.0 / (lYConversionMicronToPixelAdjusted * 1000);

        //    float Cx = nMarginpx + (float)WaferRectImage.Width * 0.5F;
        //    float Cy = nMarginpx + (float)WaferRectImage.Height * 0.5F;

        //    float dDiamNotchpx = 2500.0F * (float)lYConversionMicronToPixelAdjusted;
        //    DrawWaferShape(gImage, nMarginpx, nSizepx, dDiamNotchpx, m_fViewPenSizepx);

        //    //
        //    // On dessine les defects
        //    //
        //    m_WaferDefectRect.Clear();
        //    foreach (int sRoughBinKey in m_WaferDefectDico.Keys)
        //    {
        //        if (m_WaferDefectRect.ContainsKey(sRoughBinKey) == false)
        //            m_WaferDefectRect.Add(sRoughBinKey, new List<RectangleF>());
        //        if (bDisplayedLayerDico[sRoughBinKey])
        //        {
        //            foreach (CWaferDefectData lWaferDefect in m_WaferDefectDico[sRoughBinKey])
        //            {
        //                if (!bIsDieModeUsed)
        //                {
        //                    double lXRelatifFromWaferCenter = lWaferDefect.XRelatifFromWaferCenter;
        //                    double XPos = lWaferDefect.XRelatifFromWaferCenter * lXConversionMicronToPixelAdjusted + Cx;
        //                    double YPos = lWaferDefect.YRelatifFromWaferCenter * -1.0 * lYConversionMicronToPixelAdjusted + ViewBmp.Height - Cy;
        //                    Brush lBrush = new Pen(lWaferDefect.MyColor).Brush;
        //                    Size lDefectSizeDisplayed;
        //                    lDefectSizeDisplayed = new Size((int)lWaferDefect.XSize, (int)lWaferDefect.YSize); // TODO HUGO /!\ ici on utilisé les bon size voir comment on peux les gerer ?
        //                    // par defaut en attendant on utilise la taille donnée 
        //                    double dSizeX = lDefectSizeDisplayed.Width * lXConversionMicronToPixelAdjusted;
        //                    if (dSizeX <= 0.0)
        //                        dSizeX = 1.0;
        //                    double dSizeY = lDefectSizeDisplayed.Height * lYConversionMicronToPixelAdjusted;
        //                    if (dSizeY <= 0.0)
        //                        dSizeY = 1.0;

        //                    XPos -= dSizeX * 0.5;
        //                    YPos -= dSizeY * 0.5;
        //                    gImage.FillRectangle(lBrush, (float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY);
        //                    m_WaferDefectRect[sRoughBinKey].Add(new RectangleF((float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY));
        //                }
        //                else
        //                {
        //                    double iVectorDieOriginX = WaferParameters.DieOrigin.X - Convert.ToInt32(WaferParameters.SampleSize) * 1000 / 2;
        //                    double iVectorDieOriginY = WaferParameters.DieOrigin.Y - Convert.ToInt32(WaferParameters.SampleSize) * 1000 / 2;
        //                    double XPos = (lWaferDefect.XIndex * WaferParameters.DiePitch.X + iVectorDieOriginX + (double)lWaferDefect.XRelatif - WaferParameters.DiePitch.X / 2) * lXConversionMicronToPixelAdjusted + Cx;
        //                    double YPos = (lWaferDefect.YIndex * -1 * WaferParameters.DiePitch.Y + iVectorDieOriginY - (double)lWaferDefect.YRelatif + WaferParameters.DiePitch.Y / 2) * lYConversionMicronToPixelAdjusted + ViewBmp.Height - Cy;
        //                    Brush lBrush = new Pen(lWaferDefect.MyColor).Brush;
        //                    Size lDefectSizeDisplayed;
        //                    lDefectSizeDisplayed = new Size((int)lWaferDefect.XSize, (int)lWaferDefect.YSize); // TODO HUGO /!\ ici on utilisé les bon size voir comment on peux les gerer ?
        //                    // par defaut en attendant on utilise la taille donnée 
        //                    double dSizeX = lDefectSizeDisplayed.Width * lXConversionMicronToPixelAdjusted;
        //                    if (dSizeX <= 0.0)
        //                        dSizeX = 1.0;
        //                    double dSizeY = lDefectSizeDisplayed.Height * lYConversionMicronToPixelAdjusted;
        //                    if (dSizeY <= 0.0)
        //                        dSizeY = 1.0;
        //                    XPos -= dSizeX * 0.5;
        //                    YPos -= dSizeY * 0.5;
        //                    gImage.FillRectangle(lBrush, (float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY);
        //                    m_WaferDefectRect[sRoughBinKey].Add(new RectangleF((float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY));
        //                }
        //            }
        //        }
        //    }
        //    return ViewBmp;
        //}



        //public bool FindDefectInView(float fX_px, float fY_px, out CWaferDefectData Def, out RectangleF DefRect) //NOTE DE RTI normalement ici on devrait pas avoir à s'en servir
        //{
        //    bool bDefectFind = false;
        //    Def = null;
        //    DefRect = RectangleF.Empty;

        //    // on recherche l'ensemble des defauts qui contient le point et on retourne celui dont le centre est le plus proche de notre point
        //    float fDistMinimum = float.MaxValue;
        //    foreach (int iRoughBin in m_WaferDefectDico.Keys)
        //    {
        //        int nIdx = 0;
        //        foreach (RectangleF rc in m_WaferDefectRect[iRoughBin])
        //        {
        //            if (rc.Contains(fX_px, fY_px))
        //            {
        //                bDefectFind = true;
        //                float fdist = (float)Math.Sqrt(Math.Pow(rc.X + rc.Width * 0.5F - fX_px, 2) + Math.Pow(rc.Y + rc.Height * 0.5F - fY_px, 2));
        //                if (fdist <= fDistMinimum)
        //                {
        //                    fDistMinimum = fdist;
        //                    Def = m_WaferDefectDico[iRoughBin][nIdx];
        //                    DefRect = rc;
        //                }
        //            }
        //            nIdx++;
        //        }
        //    }

        //    return bDefectFind;
        //}
        #endregion

        public int SumOfDefects
        {
            get
            {
                int totalDef = 0;
                foreach (List<CWaferDefectData> defList in m_WaferDefectDico.Values)
                {
                    totalDef += defList.Count;
                }
                return totalDef;
            }
        }
    }
}
