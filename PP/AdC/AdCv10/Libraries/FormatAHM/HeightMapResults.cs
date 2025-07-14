using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace FormatAHM
{
    public class HeightMapResults : ICloneable
    {
        private const int FORMAT_VERSION = 1;

        private const int g_nMaxRange = 32;
        static public Bitmap g_ColorMapHeight = null;
        static public Bitmap g_ColorMapCopla = null;
        static public Bitmap g_ColorMapCoplaSubstrate = null;
        static public object g_LockColorMap = new object();

        public enum eWaferType { Round_Notch = 0, Round_Flat, Round_DblFlat, Square };
        public class MapRangePrm : ICloneable
        {
            private static int FORMAT_VERSION = 0;
            public float Min { set; get; }
            public float Max { set; get; }
            public float Step { set; get; }
            //private int CalculationMethod = 0; // reserved for later use

            public MapRangePrm()
            {
                Min = 0.0f;
                Max = 0.0f;
                Step = 0.0f;
            }
            public MapRangePrm(float fMin, float fMax, float fStep)
            {
                Min = fMin;
                Max = fMax;
                Step = fStep;
            }
            public void Read(BinaryReader br)
            {
                int lVersion = br.ReadInt32();
                if (lVersion < 0 || lVersion > FORMAT_VERSION)
                    throw new Exception("Bad file format version number in MapRangePrm. Reading failed.");
                switch (lVersion)
                {
                    case 0:
                        {
                            Min = br.ReadSingle();
                            Max = br.ReadSingle();
                            Step = br.ReadSingle();
                        }
                        break;
                }
            }
            public void Write(BinaryWriter bw)
            {
                bw.Write(MapRangePrm.FORMAT_VERSION);
                bw.Write(Min);
                bw.Write(Max);
                bw.Write(Step);
            }

            #region ICloneable Members

            protected object DeepCopy()
            {
                MapRangePrm cloned = MemberwiseClone() as MapRangePrm;
                return cloned;
            }

            public virtual object Clone()
            {
                return DeepCopy();
            }

            #endregion

        }

        #region attributes
        public eWaferType WaferType { get; set; }
        public float WaferSizeX_um { get; set; }
        public float WaferSizeY_um { get; set; }
        public float PixelSizeX { get; set; }
        public float PixelSizeY { get; set; }

        public float DieOriginX_um { get; set; } //position en microns du Die Origin par rapport au centre du wafer repère orthogonal
        public float DieOriginY_um { get; set; }
        public float DiePitchX_um { get; set; } // ecart entre 2 die en microns en X
        public float DiePitchY_um { get; set; } // ecart entre 2 die en microns en X

        public int NbDies { get { return m_DiesHMList.Count; } }
        public List<DieHMResults> DieMeasuresList { get { return m_DiesHMList; } }
        private List<DieHMResults> m_DiesHMList = null;
        public int DieGrid_MinIndex_X { get; private set; }
        public int DieGrid_MinIndex_Y { get; private set; }
        public int DieGrid_MaxIndex_X { get; private set; }
        public int DieGrid_MaxIndex_Y { get; private set; }

        public bool HeightMapGenerationEnable { set; get; }
        private MapRangePrm m_HeightMapRange = null;
        public MapRangePrm HeightMapRange { get { return m_HeightMapRange; } }
        public bool CoplaMapGenerationEnable { set; get; }
        private MapRangePrm m_CoplanarityMapRange = null;
        public MapRangePrm CoplanarityMapRange { get { return m_CoplanarityMapRange; } }

        public bool SubstrateCoplaMapGenerationEnable { set; get; }
        private MapRangePrm m_SubstrateCoplanarityMapRange = null;
        public MapRangePrm SubstrateCoplanarityMapRange { get { return m_SubstrateCoplanarityMapRange; } }
        #endregion attributes

        public HeightMapResults()
        {
            m_DiesHMList = new List<DieHMResults>();
            HeightMapGenerationEnable = false;
            CoplaMapGenerationEnable = false;
            SubstrateCoplaMapGenerationEnable = false;
        }

        public void SetWaferParameters(eWaferType p_wtype, float p_WaferSizeXum, float p_WaferSizeYum, float p_PixelSizeX, float p_PixelSizeY)
        {
            WaferType = p_wtype;
            WaferSizeX_um = p_WaferSizeXum;
            WaferSizeY_um = p_WaferSizeYum;
            PixelSizeX = p_PixelSizeX;
            PixelSizeY = p_PixelSizeY;
        }

        public void SetDieGridParameters(float fDieOriginPosX_um, float fDieOriginPosY_um, float fDiePitchX_um, float fDiePitchY_um)
        {
            DieOriginX_um = fDieOriginPosX_um;
            DieOriginY_um = fDieOriginPosY_um;
            DiePitchX_um = fDiePitchX_um;
            DiePitchY_um = fDiePitchY_um;
        }

        public void SetHeightMapParameters(bool bEnable, float fMin, float fMax, float fStep)
        {
            if (m_HeightMapRange == null)
            {
                m_HeightMapRange = new MapRangePrm(fMin, fMax, fStep);
                HeightMapGenerationEnable = bEnable;
            }
            else
            {
                m_HeightMapRange.Min = fMin;
                m_HeightMapRange.Max = fMax;
                m_HeightMapRange.Step = fStep;
                HeightMapGenerationEnable = bEnable;
            }
        }

        public void SetCoplanarityParameters(bool bEnable, float fMin, float fMax, float fStep)
        {
            if (m_CoplanarityMapRange == null)
            {
                m_CoplanarityMapRange = new MapRangePrm(fMin, fMax, fStep);
                CoplaMapGenerationEnable = bEnable;
            }
            else
            {
                m_CoplanarityMapRange.Min = fMin;
                m_CoplanarityMapRange.Max = fMax;
                m_CoplanarityMapRange.Step = fStep;
                CoplaMapGenerationEnable = bEnable;
            }
        }

        public void SetSubstrateCoplanarityParameters(bool bEnable, float fMin, float fMax, float fStep)
        {
            if (m_SubstrateCoplanarityMapRange == null)
            {
                m_SubstrateCoplanarityMapRange = new MapRangePrm(fMin, fMax, fStep);
                SubstrateCoplaMapGenerationEnable = bEnable;
            }
            else
            {
                m_SubstrateCoplanarityMapRange.Min = fMin;
                m_SubstrateCoplanarityMapRange.Max = fMax;
                m_SubstrateCoplanarityMapRange.Step = fStep;
                SubstrateCoplaMapGenerationEnable = bEnable;
            }
        }

        public void AddDieHMResult(DieHMResults p_result)
        {
            m_DiesHMList.Add(p_result);
        }

        #region Read/Write in file

        public bool WriteInFile(String pFilePathName, out String sError)
        {
            sError = "";
            FileStream lStream = null;
            BinaryWriter lBinaryWriter = null;
            bool bSuccess = false;
            try
            {
                lStream = new FileStream(pFilePathName, FileMode.Create);
                lBinaryWriter = new BinaryWriter(lStream);
                lBinaryWriter.Write(HeightMapResults.FORMAT_VERSION);

                //Wafer Type
                int nWaferType = (int)WaferType;
                lBinaryWriter.Write(nWaferType);
                lBinaryWriter.Write(WaferSizeX_um);
                lBinaryWriter.Write(WaferSizeY_um);
                lBinaryWriter.Write(PixelSizeX);
                lBinaryWriter.Write(PixelSizeY);

                // Die grid parameters
                lBinaryWriter.Write(DieOriginX_um);
                lBinaryWriter.Write(DieOriginY_um);
                lBinaryWriter.Write(DiePitchX_um);
                lBinaryWriter.Write(DiePitchY_um);

                // Height map parameters
                lBinaryWriter.Write(HeightMapGenerationEnable ? 1 : 0);
                if (HeightMapGenerationEnable)
                {
                    m_HeightMapRange.Write(lBinaryWriter);
                }

                // Coplanarity parameters
                lBinaryWriter.Write(CoplaMapGenerationEnable ? 1 : 0);
                if (CoplaMapGenerationEnable)
                {
                    m_CoplanarityMapRange.Write(lBinaryWriter);
                }

                // Stats
                UpdateStats();
                lBinaryWriter.Write(DieGrid_MinIndex_X);
                lBinaryWriter.Write(DieGrid_MaxIndex_X);
                lBinaryWriter.Write(DieGrid_MinIndex_Y);
                lBinaryWriter.Write(DieGrid_MaxIndex_Y);

                //results list
                lBinaryWriter.Write((int)NbDies);
                foreach (DieHMResults dieHM in m_DiesHMList)
                {
                    dieHM.Write(lBinaryWriter);
                }

                // Substrate Coplanarity parameters
                lBinaryWriter.Write(SubstrateCoplaMapGenerationEnable ? 1 : 0);
                if (SubstrateCoplaMapGenerationEnable)
                {
                    m_SubstrateCoplanarityMapRange.Write(lBinaryWriter);
                }

                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (lBinaryWriter != null)
                    lBinaryWriter.Close();
                if (lStream != null)
                    lStream.Close();
            }
            return bSuccess;
        }

        public bool ReadFromFile(String pFilePathName, out String sError)
        {
            bool bSuccess = false;
            sError = "";
            FileStream lStream = null;
            BinaryReader lBinaryReader = null;

            try
            {
                lStream = new FileStream(pFilePathName, FileMode.Open, FileAccess.Read, FileShare.Read);
                lBinaryReader = new BinaryReader(lStream);
                int lVersion = lBinaryReader.ReadInt32();
                if (lVersion < 0 || lVersion > FORMAT_VERSION)
                    throw new Exception("Bad file format version number. HeightMapResults Reading failed.");

                //Wafer Type
                int nWaferType = lBinaryReader.ReadInt32();
                if (typeof(eWaferType).IsEnumDefined(nWaferType))
                    WaferType = (eWaferType)nWaferType;
                else
                    throw new Exception("Bad Wafer Type enum in  HeightMapResults. Reading failed.");
                WaferSizeX_um = lBinaryReader.ReadSingle();
                WaferSizeY_um = lBinaryReader.ReadSingle();
                PixelSizeX = lBinaryReader.ReadSingle();
                PixelSizeY = lBinaryReader.ReadSingle();

                // Die grid parameters
                DieOriginX_um = lBinaryReader.ReadSingle();
                DieOriginY_um = lBinaryReader.ReadSingle();
                DiePitchX_um = lBinaryReader.ReadSingle();
                DiePitchY_um = lBinaryReader.ReadSingle();

                // Height map parameters
                HeightMapGenerationEnable = (0 != lBinaryReader.ReadInt32());
                if (HeightMapGenerationEnable)
                {
                    m_HeightMapRange = new MapRangePrm();
                    m_HeightMapRange.Read(lBinaryReader);
                }

                // Coplanarity parameters
                CoplaMapGenerationEnable = (0 != lBinaryReader.ReadInt32());
                if (CoplaMapGenerationEnable)
                {
                    m_CoplanarityMapRange = new MapRangePrm();
                    m_CoplanarityMapRange.Read(lBinaryReader);
                }

                // read Die grid stats
                DieGrid_MinIndex_X = lBinaryReader.ReadInt32();
                DieGrid_MaxIndex_X = lBinaryReader.ReadInt32();
                DieGrid_MinIndex_Y = lBinaryReader.ReadInt32();
                DieGrid_MaxIndex_Y = lBinaryReader.ReadInt32();

                //HM Results list
                int iNBdiesINList = lBinaryReader.ReadInt32();
                if (m_DiesHMList != null)
                    m_DiesHMList.Clear();
                m_DiesHMList = new List<DieHMResults>(iNBdiesINList);
                for (int i = 0; i < iNBdiesINList; i++)
                {
                    DieHMResults dieHM = new DieHMResults();
                    dieHM.Read(lBinaryReader);
                    m_DiesHMList.Add(dieHM);
                }

                if (lVersion >= 1)
                {
                    SubstrateCoplaMapGenerationEnable = (0 != lBinaryReader.ReadInt32());
                    if (SubstrateCoplaMapGenerationEnable)
                    {
                        m_SubstrateCoplanarityMapRange = new MapRangePrm();
                        m_SubstrateCoplanarityMapRange.Read(lBinaryReader);
                    }
                }

                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (lBinaryReader != null)
                    lBinaryReader.Close();
                if (lStream != null)
                    lStream.Close();
            }
            return bSuccess;
        }

        #endregion

        private void UpdateStats()
        {
            if (NbDies > 0)
            {
                DieGrid_MaxIndex_X = -int.MaxValue; DieGrid_MinIndex_X = int.MaxValue;
                DieGrid_MaxIndex_Y = -int.MaxValue; DieGrid_MinIndex_Y = int.MaxValue;
                foreach (DieHMResults dieHM in m_DiesHMList)
                {
                    DieGrid_MinIndex_X = Math.Min(DieGrid_MinIndex_X, dieHM.indexX);
                    DieGrid_MaxIndex_X = Math.Max(DieGrid_MaxIndex_X, dieHM.indexX);

                    DieGrid_MinIndex_Y = Math.Min(DieGrid_MinIndex_Y, dieHM.indexY);
                    DieGrid_MaxIndex_Y = Math.Max(DieGrid_MaxIndex_Y, dieHM.indexY);

                    dieHM.UpdateStats(); // calcul des données statistiques sur les mesure de hauteurs !
                }
            }
            else
            {
                DieGrid_MaxIndex_X = 0; DieGrid_MinIndex_X = 0;
                DieGrid_MaxIndex_Y = 0; DieGrid_MinIndex_Y = 0;
            }
        }

        #region Draw Map Image

        static public int _ImgMapSize_X = 4000;
        static public int _ImgMapSize_Y = 4000;
        static public int _ImgMapMargin_X = 99;
        static public int _ImgMapMargin_Y = 99;

        static public Image DrawHeightMap(HeightMapResults hmres, bool bDrawLegend, List<Tuple<int, String, Color>> CumulArray)
        {
            if (HeightMapResults.g_ColorMapHeight == null)
            {
                lock (g_LockColorMap)
                {
                    // on retest histoire de pas le refaire
                    if (HeightMapResults.g_ColorMapHeight == null)
                    {
                        // l'ordre doit forcement etre du plus petit float au plsu grand entre 0 et 1
                        int nbColor = 6;
                        List<Tuple<Color, float>> clrmap = new List<Tuple<Color, float>>(nbColor);
                        float fstep = 1.0f / ((float)nbColor - 1.0f);
                        int i = 0;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(112, 48, 160), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(0, 176, 240), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(0, 176, 80), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 255, 0), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(228, 109, 10), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 0, 0), i * fstep)); i++;

                        HeightMapResults.g_ColorMapHeight = MakeColorMap(clrmap);
                    }
                }
            }


            Image img = DrawWaferBackGround(hmres);

            float Cx_px = ((float)img.Size.Width) * 0.5F; // WaferCenter X en pixels;
            float Cy_px = ((float)img.Size.Height) * 0.5F; // WaferCenter Y en pixels;       
            float fPixelSize_Map_X = (hmres.WaferSizeX_um) / ((float)_ImgMapSize_X);
            float fPixelSize_Map_Y = (hmres.WaferSizeY_um) / ((float)_ImgMapSize_Y);
            float RatioPixSize_X = hmres.PixelSizeX / fPixelSize_Map_X;
            float RatioPixSize_Y = hmres.PixelSizeY / fPixelSize_Map_Y;

            MapRangePrm mr = hmres.HeightMapRange;
            if (mr.Step <= 0.0f)
            {
                mr.Step = mr.Max - mr.Min;
                if (mr.Step <= 0.0f)
                    mr.Step = 1.0f; // FUCK faudrait voir à ce que l'utiisateur parametrer correctement !!!
            }
            if ((mr.Max - mr.Min) == 0.0f)
            {
                mr.Max += mr.Step / 2.0f;
                mr.Min -= mr.Step / 2.0f;
            }
            if (mr.Step > mr.Max - mr.Min)
            {
                mr.Step = mr.Max - mr.Min;
            }

            float fA = 1.0f / (mr.Max - mr.Min); // pente pour linéarisation 
            float fB = 1.0f - (fA * mr.Max);                 // ordonnée à l'originepour linéarisation 

            int NbRange = (int)Math.Round((double)(2.0f + (mr.Max - mr.Min) / mr.Step));
            float fColorStep = 1.0f / (NbRange - 2);
            if (NbRange > g_nMaxRange)
            {
                NbRange = g_nMaxRange;
                fColorStep = 1.0f / (NbRange - 2);
                mr.Step = (mr.Max - mr.Min) / (float)(NbRange - 2);
            }
            else
            {
                mr.Step = fColorStep * (mr.Max - mr.Min);
            }

            Color[] lColor = new Color[NbRange];
            int[] lCumul = new int[NbRange];
            List<Brush> lBrush = new List<Brush>(NbRange);
            float fCur = 0.0f;
            float fMult = (float)g_nMaxRange - 1.0f;
            float fColorStepforMAP = 1.0f / (float)(NbRange - 1);
            for (int i = 0; i < NbRange; i++)
            {
                fCur = i * fColorStepforMAP;
                int nColorStep = (int)(Math.Round(fCur * fMult));
                Color clr = HeightMapResults.g_ColorMapHeight.GetPixel(nColorStep, 0);
                lColor[i] = clr;
                lCumul[i] = 0;
                lBrush.Add(new SolidBrush(clr));
            }

            using (Graphics graphics = Graphics.FromImage(img))
            {

                if (bDrawLegend)
                {
                    SolidBrush captionBrush = new SolidBrush(Color.White);
                    Font captionFontTitle = new Font("Arial", 36.0f);
                    Font captionFont = new Font("Arial", 18.0f);

                    float fRectX = 50.0f;
                    float fRectY = 30.0f;
                    float StartX_px = 30.0f;
                    float StartY_px = 30.0f;
                    float StartFontY_px = StartY_px + (float)captionFont.Size;
                    float fInterligne = 10.0f;

                    graphics.DrawString("Height mapping", captionFontTitle, captionBrush, 450.0f, StartY_px + 10.0f);
                    graphics.DrawString("in μm", captionFont, captionBrush, StartX_px + fRectX + 10.0f, 0.0f);
                    graphics.FillRectangle(lBrush[NbRange - 1], StartX_px, StartY_px, fRectX, fRectY);
                    graphics.DrawString(mr.Max.ToString("###0.#"), captionFont, captionBrush, StartX_px + fRectX, StartFontY_px);
                    for (int i = NbRange - 2; i > 0; i--)
                    {
                        graphics.FillRectangle(lBrush[i], StartX_px, StartY_px + ((NbRange - 1) - i) * (fRectY + fInterligne), fRectX, fRectY);
                        float fH = mr.Max - ((NbRange - 1) - i) * mr.Step;
                        graphics.DrawString(fH.ToString("###0.#"), captionFont, captionBrush,
                                                           StartX_px + fRectX, StartFontY_px + ((NbRange - 1) - i) * (fRectY + fInterligne));
                    }
                    graphics.FillRectangle(lBrush[0], StartX_px, StartY_px + (NbRange - 1) * (fRectY + fInterligne), fRectX, fRectY);
                }


                // HOL : il y a un offset entre la position des die dans l'image d'entrée (m_lsDieLayout[i].valueDiePosX_px) et leur position dans l'image du wafer
                // Pour calculer cet offset, on se base sur le die origin dont on connait la position dans les 2 images
                int iPosOffsetX_px = 0;// offset x en pixels
                int iPosOffsetY_px = 0;// offset y en pixels
                DieHMResults vDieOrigin = hmres.m_DiesHMList.FirstOrDefault(s => s.indexX == 0 && s.indexY == 0);
                if (vDieOrigin != null)
                {
                    float DieOriginPosWaferX_um = (hmres.WaferSizeX_um * 0.5f) + hmres.DieOriginX_um;  //pos x dans le wafer
                    float DieOriginPosWaferY_um = (hmres.WaferSizeY_um * 0.5f) - hmres.DieOriginY_um;  //pos y dans le wafer
                    float DieOriginPosImageX_um = vDieOrigin.positionX * hmres.PixelSizeX; //pos x dans l'image
                    float DieOriginPosImageY_um = vDieOrigin.positionY * hmres.PixelSizeY; //pos y dans l'image
                    iPosOffsetX_px = (int)((DieOriginPosWaferX_um - DieOriginPosImageX_um) / fPixelSize_Map_X); // calcul de l'offset x en pixels
                    iPosOffsetY_px = (int)((DieOriginPosWaferY_um - DieOriginPosImageY_um) / fPixelSize_Map_Y); // calcul de l'offset y en pixels
                }

                double dfontsize = 0.25 * Math.Min(hmres.m_DiesHMList[0].imageSizeX, hmres.m_DiesHMList[0].imageSizeY) * Math.Min(RatioPixSize_X, RatioPixSize_Y);
                if (dfontsize < 4.0)
                    dfontsize = 4.0;
                Font drawFont = new Font("Arial", (float)dfontsize);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                Pen drawPen = new Pen(drawBrush);

                int iPosDieX = 0; int iDieSizeX = 0;
                int iPosDieY = 0; int iDieSizeY = 0;
                string sTxt = "";
                foreach (DieHMResults diehm in hmres.m_DiesHMList)
                {
                    iPosDieX = (int)(RatioPixSize_X * (float)diehm.positionX) + iPosOffsetX_px + _ImgMapMargin_X;
                    iPosDieY = (int)(RatioPixSize_Y * (float)diehm.positionY) + iPosOffsetY_px + _ImgMapMargin_Y;

                    sTxt = diehm.indexX.ToString() + "/" + diehm.indexY.ToString();

                    iDieSizeX = (int)(RatioPixSize_X * (float)diehm.imageSizeX);
                    iDieSizeY = (int)(RatioPixSize_Y * (float)diehm.imageSizeY);

                    float fLinVal = diehm.MeanHeight_um * fA + fB;
                    int nSelectRange = 0;
                    if (fLinVal < 0.0f)
                        nSelectRange = 0;
                    else if (fLinVal > 1.0f)
                        nSelectRange = NbRange - 1;
                    else
                        nSelectRange = (int)Math.Floor(fLinVal / fColorStep) + 1;

                    lCumul[nSelectRange] += 1;
                    graphics.FillRectangle(lBrush[nSelectRange], iPosDieX, iPosDieY, iDieSizeX, iDieSizeY);
                    graphics.DrawRectangle(drawPen, iPosDieX, iPosDieY, iDieSizeX, iDieSizeY);

                    // calul de la position du string identifiant le die 
                    float fStrPosX = iPosDieX + Math.Max(0, (iDieSizeX - sTxt.Length * drawFont.Size * 0.92F) / 2.0F);
                    float fStrPosY = iPosDieY + Math.Max(0, (iDieSizeY - drawFont.GetHeight()) / 2.0F);
                    graphics.DrawString(sTxt, drawFont, drawBrush, fStrPosX, fStrPosY);
                }
            }

            if (CumulArray != null)
            {
                String sLbl = "";
                for (int i = 0; i < NbRange; i++)
                {
                    if (i == 0)
                    {
                        sLbl = String.Format("Below {0}", mr.Min);
                    }
                    else if (i == NbRange - 1)
                    {
                        sLbl = String.Format("Above {0}", mr.Max);
                    }
                    else
                    {
                        sLbl = String.Format("{0} - {1}", mr.Min + (i - 1) * mr.Step, mr.Min + (i) * mr.Step);
                    }
                    CumulArray.Add(new Tuple<int, String, Color>(lCumul[i], sLbl, lColor[i]));
                }
            }
            return img;
        }

        static public Image DrawCoplanarityMap(HeightMapResults hmres, bool bDrawLegend, List<Tuple<int, String, Color>> CumulArray)
        {

            if (HeightMapResults.g_ColorMapCopla == null)
            {
                lock (g_LockColorMap)
                {
                    // on retest histoire de pas le refaire
                    if (HeightMapResults.g_ColorMapCopla == null)
                    {
                        // l'ordre doit forcement etre du plus petit float au plsu grand entre 0 et 1
                        int nbColor = 4;
                        List<Tuple<Color, float>> clrmap = new List<Tuple<Color, float>>(nbColor);
                        float fstep = 1.0f / ((float)nbColor - 1.0f);
                        int i = 0;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(0, 255, 0), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 255, 0), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 128, 0), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 0, 0), i * fstep)); i++;

                        HeightMapResults.g_ColorMapCopla = MakeColorMap(clrmap);
                    }
                }
            }

            Image img = DrawWaferBackGround(hmres);

            float fPixelSize_Map_X = (hmres.WaferSizeX_um) / ((float)_ImgMapSize_X);
            float fPixelSize_Map_Y = (hmres.WaferSizeY_um) / ((float)_ImgMapSize_Y);
            float RatioPixSize_X = hmres.PixelSizeX / fPixelSize_Map_X;
            float RatioPixSize_Y = hmres.PixelSizeY / fPixelSize_Map_Y;

            MapRangePrm mr = hmres.CoplanarityMapRange;
            if (mr.Step <= 0.0f)
            {
                mr.Step = mr.Max - mr.Min;
                if (mr.Step <= 0.0f)
                    mr.Step = 1.0f; // FUCK faudrait voir à ce que l'utiisateur parametrer correctement !!!
            }
            if ((mr.Max - mr.Min) == 0.0f)
            {
                mr.Max += mr.Step / 2.0f;
                mr.Min -= mr.Step / 2.0f;
            }
            if (mr.Step > mr.Max - mr.Min)
            {
                mr.Step = mr.Max - mr.Min;
            }

            float fA = 1.0f / (mr.Max - mr.Min); // pente pour linéarisation 
            float fB = 1.0f - (fA * mr.Max);                 // ordonnée à l'originepour linéarisation 

            int NbRange = (int)Math.Round((double)(2.0f + (mr.Max - mr.Min) / mr.Step));
            float fColorStep = 1.0f / (NbRange - 2);
            if (NbRange > g_nMaxRange)
            {
                NbRange = g_nMaxRange;
                fColorStep = 1.0f / (NbRange - 2);
                mr.Step = (mr.Max - mr.Min) / (float)(NbRange - 2);
            }
            else
            {
                mr.Step = fColorStep * (mr.Max - mr.Min);
            }

            Color[] lColor = new Color[NbRange];
            int[] lCumul = new int[NbRange];
            List<Brush> lBrush = new List<Brush>(NbRange);
            float fCur = 0.0f;
            float fMult = (float)g_nMaxRange - 1.0f;
            float fColorStepforMAP = 1.0f / (float)(NbRange - 1);
            for (int i = 0; i < NbRange; i++)
            {
                fCur = i * fColorStepforMAP;
                int nColorStep = (int)(Math.Round(fCur * fMult));
                Color clr = HeightMapResults.g_ColorMapCopla.GetPixel(nColorStep, 0);
                lColor[i] = clr;
                lCumul[i] = 0;
                lBrush.Add(new SolidBrush(clr));
            }


            using (Graphics graphics = Graphics.FromImage(img))
            {
                if (bDrawLegend)
                {

                    SolidBrush captionBrush = new SolidBrush(Color.White);
                    Font captionFontTitle = new Font("Arial", 36.0f);
                    Font captionFont = new Font("Arial", 18.0f);

                    float fRectX = 50.0f;
                    float fRectY = 30.0f;
                    float StartX_px = 30.0f;
                    float StartY_px = 30.0f;
                    float StartFontY_px = StartY_px + (float)captionFont.Size;
                    float fInterligne = 10.0f;

                    graphics.DrawString("Coplanarity mapping", captionFontTitle, captionBrush, 450.0f, StartY_px + 10.0f);
                    graphics.DrawString("in μm", captionFont, captionBrush, StartX_px + fRectX + 10.0f, 0.0f);
                    graphics.FillRectangle(lBrush[NbRange - 1], StartX_px, StartY_px, fRectX, fRectY);
                    graphics.DrawString(mr.Max.ToString("###0.#"), captionFont, captionBrush, StartX_px + fRectX, StartY_px + fInterligne);
                    for (int i = NbRange - 2; i > 0; i--)
                    {
                        graphics.FillRectangle(lBrush[i], StartX_px, StartY_px + ((NbRange - 1) - i) * (fRectY + fInterligne), fRectX, fRectY);
                        float fH = mr.Max - ((NbRange - 1) - i) * mr.Step;
                        graphics.DrawString(fH.ToString("###0.#"), captionFont, captionBrush,
                                            StartX_px + fRectX, StartFontY_px + ((NbRange - 1) - i) * (fRectY + fInterligne));
                    }
                    graphics.FillRectangle(lBrush[0], StartX_px, StartY_px + (NbRange - 1) * (fRectY + fInterligne), fRectX, fRectY);
                }

                // HOL : il y a un offset entre la position des die dans l'image d'entrée (m_lsDieLayout[i].valueDiePosX_px) et leur position dans l'image du wafer
                // Pour calculer cet offset, on se base sur le die origin dont on connait la position dans les 2 images
                int iPosOffsetX_px = 0;// offset x en pixels
                int iPosOffsetY_px = 0;// offset y en pixels
                DieHMResults vDieOrigin = hmres.m_DiesHMList.FirstOrDefault(s => s.indexX == 0 && s.indexY == 0);
                if (vDieOrigin != null)
                {
                    float DieOriginPosWaferX_um = (hmres.WaferSizeX_um * 0.5f) + hmres.DieOriginX_um;  //pos x dans le wafer
                    float DieOriginPosWaferY_um = (hmres.WaferSizeY_um * 0.5f) - hmres.DieOriginY_um;  //pos y dans le wafer 
                    float DieOriginPosImageX_um = vDieOrigin.positionX * hmres.PixelSizeX; //pos x dans l'image
                    float DieOriginPosImageY_um = vDieOrigin.positionY * hmres.PixelSizeY; //pos y dans l'image
                    iPosOffsetX_px = (int)((DieOriginPosWaferX_um - DieOriginPosImageX_um) / fPixelSize_Map_X); // calcul de l'offset x en pixels
                    iPosOffsetY_px = (int)((DieOriginPosWaferY_um - DieOriginPosImageY_um) / fPixelSize_Map_Y); // calcul de l'offset y en pixels
                }

                double dfontsize = 0.25 * Math.Min(hmres.m_DiesHMList[0].imageSizeX, hmres.m_DiesHMList[0].imageSizeY) * Math.Min(RatioPixSize_X, RatioPixSize_Y);
                if (dfontsize < 4.0)
                    dfontsize = 4.0;
                Font drawFont = new Font("Arial", (float)dfontsize);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                Pen drawPen = new Pen(drawBrush);

                int iPosDieX = 0; int iDieSizeX = 0;
                int iPosDieY = 0; int iDieSizeY = 0;
                string sTxt = "";
                foreach (DieHMResults diehm in hmres.m_DiesHMList)
                {
                    iPosDieX = (int)(RatioPixSize_X * (float)diehm.positionX) + iPosOffsetX_px + _ImgMapMargin_X;
                    iPosDieY = (int)(RatioPixSize_Y * (float)diehm.positionY) + iPosOffsetY_px + _ImgMapMargin_Y;

                    sTxt = diehm.indexX.ToString() + "/" + diehm.indexY.ToString();

                    iDieSizeX = (int)(RatioPixSize_X * (float)diehm.imageSizeX);
                    iDieSizeY = (int)(RatioPixSize_Y * (float)diehm.imageSizeY);

                    float fLinVal = diehm.Coplanarity * fA + fB;
                    int nSelectRange = 0;
                    if (fLinVal < 0.0f)
                        nSelectRange = 0;
                    else if (fLinVal > 1.0f)
                        nSelectRange = NbRange - 1;
                    else
                        nSelectRange = (int)Math.Floor(fLinVal / fColorStep) + 1;

                    lCumul[nSelectRange] += 1;
                    graphics.FillRectangle(lBrush[nSelectRange], iPosDieX, iPosDieY, iDieSizeX, iDieSizeY);
                    graphics.DrawRectangle(drawPen, iPosDieX, iPosDieY, iDieSizeX, iDieSizeY);

                    // calul de la position du string identifiant le die 
                    float fStrPosX = iPosDieX + Math.Max(0, (iDieSizeX - sTxt.Length * drawFont.Size * 0.92F) / 2.0F);
                    float fStrPosY = iPosDieY + Math.Max(0, (iDieSizeY - drawFont.GetHeight()) / 2.0F);
                    graphics.DrawString(sTxt, drawFont, drawBrush, fStrPosX, fStrPosY);
                }
            }

            if (CumulArray != null)
            {
                string sLbl = "";
                for (int i = 0; i < NbRange; i++)
                {
                    if (i == 0)
                    {
                        sLbl = String.Format("Below {0}", mr.Min);
                    }
                    else if (i == NbRange - 1)
                    {
                        sLbl = String.Format("Above {0}", mr.Max);
                    }
                    else
                    {
                        sLbl = String.Format("{0} - {1}", mr.Min + (i - 1) * mr.Step, mr.Min + (i) * mr.Step);
                    }
                    CumulArray.Add(new Tuple<int, String, Color>(lCumul[i], sLbl, lColor[i]));
                }
            }

            return img;
        }

        static public Image DrawSubstrateCoplanarityMap(HeightMapResults hmres, bool bDrawLegend, List<Tuple<int, String, Color>> CumulArray)
        {

            if (HeightMapResults.g_ColorMapCoplaSubstrate == null)
            {
                lock (g_LockColorMap)
                {
                    // on retest histoire de pas le refaire
                    if (HeightMapResults.g_ColorMapCoplaSubstrate == null)
                    {
                        // l'ordre doit forcement etre du plus petit float au plsu grand entre 0 et 1
                        int nbColor = 4;
                        List<Tuple<Color, float>> clrmap = new List<Tuple<Color, float>>(nbColor);
                        float fstep = 1.0f / ((float)nbColor - 1.0f);
                        int i = 0;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(0, 255, 0), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 255, 0), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 128, 0), i * fstep)); i++;
                        clrmap.Add(new Tuple<Color, float>(Color.FromArgb(255, 0, 0), i * fstep)); i++;

                        HeightMapResults.g_ColorMapCoplaSubstrate = MakeColorMap(clrmap);
                    }
                }
            }

            Image img = DrawWaferBackGround(hmres);

            float fPixelSize_Map_X = (hmres.WaferSizeX_um) / ((float)_ImgMapSize_X);
            float fPixelSize_Map_Y = (hmres.WaferSizeY_um) / ((float)_ImgMapSize_Y);
            float RatioPixSize_X = hmres.PixelSizeX / fPixelSize_Map_X;
            float RatioPixSize_Y = hmres.PixelSizeY / fPixelSize_Map_Y;

            MapRangePrm mr = hmres.SubstrateCoplanarityMapRange;
            if (mr.Step <= 0.0f)
            {
                mr.Step = mr.Max - mr.Min;
                if (mr.Step <= 0.0f)
                    mr.Step = 1.0f; // FUCK faudrait voir à ce que l'utiisateur parametrer correctement !!!
            }
            if ((mr.Max - mr.Min) == 0.0f)
            {
                mr.Max += mr.Step / 2.0f;
                mr.Min -= mr.Step / 2.0f;
            }
            if (mr.Step > mr.Max - mr.Min)
            {
                mr.Step = mr.Max - mr.Min;
            }

            float fA = 1.0f / (mr.Max - mr.Min); // pente pour linéarisation 
            float fB = 1.0f - (fA * mr.Max);                 // ordonnée à l'originepour linéarisation 

            int NbRange = (int)Math.Round((double)(2.0f + (mr.Max - mr.Min) / mr.Step));
            float fColorStep = 1.0f / (NbRange - 2);
            if (NbRange > g_nMaxRange)
            {
                NbRange = g_nMaxRange;
                fColorStep = 1.0f / (NbRange - 2);
                mr.Step = (mr.Max - mr.Min) / (float)(NbRange - 2);
            }
            else
            {
                mr.Step = fColorStep * (mr.Max - mr.Min);
            }

            Color[] lColor = new Color[NbRange];
            int[] lCumul = new int[NbRange];
            List<Brush> lBrush = new List<Brush>(NbRange);
            float fCur = 0.0f;
            float fMult = (float)g_nMaxRange - 1.0f;
            float fColorStepforMAP = 1.0f / (float)(NbRange - 1);
            for (int i = 0; i < NbRange; i++)
            {
                fCur = i * fColorStepforMAP;
                int nColorStep = (int)(Math.Round(fCur * fMult));
                Color clr = HeightMapResults.g_ColorMapCoplaSubstrate.GetPixel(nColorStep, 0);
                lColor[i] = clr;
                lCumul[i] = 0;
                lBrush.Add(new SolidBrush(clr));
            }


            using (Graphics graphics = Graphics.FromImage(img))
            {
                if (bDrawLegend)
                {

                    SolidBrush captionBrush = new SolidBrush(Color.White);
                    Font captionFontTitle = new Font("Arial", 36.0f);
                    Font captionFont = new Font("Arial", 18.0f);

                    float fRectX = 50.0f;
                    float fRectY = 30.0f;
                    float StartX_px = 30.0f;
                    float StartY_px = 30.0f;
                    float StartFontY_px = StartY_px + (float)captionFont.Size;
                    float fInterligne = 10.0f;

                    graphics.DrawString("Substrate Coplanarity mapping", captionFontTitle, captionBrush, 450.0f, StartY_px + 10.0f);
                    graphics.DrawString("in μm", captionFont, captionBrush, StartX_px + fRectX + 10.0f, 0.0f);
                    graphics.FillRectangle(lBrush[NbRange - 1], StartX_px, StartY_px, fRectX, fRectY);
                    graphics.DrawString(mr.Max.ToString("###0.#"), captionFont, captionBrush, StartX_px + fRectX, StartY_px + fInterligne);
                    for (int i = NbRange - 2; i > 0; i--)
                    {
                        graphics.FillRectangle(lBrush[i], StartX_px, StartY_px + ((NbRange - 1) - i) * (fRectY + fInterligne), fRectX, fRectY);
                        float fH = mr.Max - ((NbRange - 1) - i) * mr.Step;
                        graphics.DrawString(fH.ToString("###0.#"), captionFont, captionBrush,
                                            StartX_px + fRectX, StartFontY_px + ((NbRange - 1) - i) * (fRectY + fInterligne));
                    }
                    graphics.FillRectangle(lBrush[0], StartX_px, StartY_px + (NbRange - 1) * (fRectY + fInterligne), fRectX, fRectY);
                }

                // HOL : il y a un offset entre la position des die dans l'image d'entrée (m_lsDieLayout[i].valueDiePosX_px) et leur position dans l'image du wafer
                // Pour calculer cet offset, on se base sur le die origin dont on connait la position dans les 2 images
                int iPosOffsetX_px = 0;// offset x en pixels
                int iPosOffsetY_px = 0;// offset y en pixels
                DieHMResults vDieOrigin = hmres.m_DiesHMList.FirstOrDefault(s => s.indexX == 0 && s.indexY == 0);
                if (vDieOrigin != null)
                {
                    float DieOriginPosWaferX_um = (hmres.WaferSizeX_um * 0.5f) + hmres.DieOriginX_um;  //pos x dans le wafer
                    float DieOriginPosWaferY_um = (hmres.WaferSizeY_um * 0.5f) - hmres.DieOriginY_um;  //pos y dans le wafer 
                    float DieOriginPosImageX_um = vDieOrigin.positionX * hmres.PixelSizeX; //pos x dans l'image
                    float DieOriginPosImageY_um = vDieOrigin.positionY * hmres.PixelSizeY; //pos y dans l'image
                    iPosOffsetX_px = (int)((DieOriginPosWaferX_um - DieOriginPosImageX_um) / fPixelSize_Map_X); // calcul de l'offset x en pixels
                    iPosOffsetY_px = (int)((DieOriginPosWaferY_um - DieOriginPosImageY_um) / fPixelSize_Map_Y); // calcul de l'offset y en pixels
                }

                double dfontsize = 0.25 * Math.Min(hmres.m_DiesHMList[0].imageSizeX, hmres.m_DiesHMList[0].imageSizeY) * Math.Min(RatioPixSize_X, RatioPixSize_Y);
                if (dfontsize < 4.0)
                    dfontsize = 4.0;
                Font drawFont = new Font("Arial", (float)dfontsize);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                Pen drawPen = new Pen(drawBrush);

                int iPosDieX = 0; int iDieSizeX = 0;
                int iPosDieY = 0; int iDieSizeY = 0;
                string sTxt = "";
                foreach (DieHMResults diehm in hmres.m_DiesHMList)
                {
                    iPosDieX = (int)(RatioPixSize_X * (float)diehm.positionX) + iPosOffsetX_px + _ImgMapMargin_X;
                    iPosDieY = (int)(RatioPixSize_Y * (float)diehm.positionY) + iPosOffsetY_px + _ImgMapMargin_Y;

                    sTxt = diehm.indexX.ToString() + "/" + diehm.indexY.ToString();

                    iDieSizeX = (int)(RatioPixSize_X * (float)diehm.imageSizeX);
                    iDieSizeY = (int)(RatioPixSize_Y * (float)diehm.imageSizeY);

                    float fLinVal = diehm.SubstrateCoplanarity * fA + fB;
                    int nSelectRange = 0;
                    if (fLinVal < 0.0f)
                        nSelectRange = 0;
                    else if (fLinVal > 1.0f)
                        nSelectRange = NbRange - 1;
                    else
                        nSelectRange = (int)Math.Floor(fLinVal / fColorStep) + 1;

                    lCumul[nSelectRange] += 1;
                    graphics.FillRectangle(lBrush[nSelectRange], iPosDieX, iPosDieY, iDieSizeX, iDieSizeY);
                    graphics.DrawRectangle(drawPen, iPosDieX, iPosDieY, iDieSizeX, iDieSizeY);

                    // calul de la position du string identifiant le die 
                    float fStrPosX = iPosDieX + Math.Max(0, (iDieSizeX - sTxt.Length * drawFont.Size * 0.92F) / 2.0F);
                    float fStrPosY = iPosDieY + Math.Max(0, (iDieSizeY - drawFont.GetHeight()) / 2.0F);
                    graphics.DrawString(sTxt, drawFont, drawBrush, fStrPosX, fStrPosY);
                }
            }

            if (CumulArray != null)
            {
                string sLbl = "";
                for (int i = 0; i < NbRange; i++)
                {
                    if (i == 0)
                    {
                        sLbl = String.Format("Below {0}", mr.Min);
                    }
                    else if (i == NbRange - 1)
                    {
                        sLbl = String.Format("Above {0}", mr.Max);
                    }
                    else
                    {
                        sLbl = String.Format("{0} - {1}", mr.Min + (i - 1) * mr.Step, mr.Min + (i) * mr.Step);
                    }
                    CumulArray.Add(new Tuple<int, String, Color>(lCumul[i], sLbl, lColor[i]));
                }
            }

            return img;
        }


        static private Image DrawWaferBackGround(HeightMapResults hmres)
        {
            Image img = new Bitmap(_ImgMapSize_X + 2 * _ImgMapMargin_X, _ImgMapSize_Y + 2 * _ImgMapMargin_Y);

            float Cx_px = ((float)img.Size.Width) * 0.5F; // WaferCenter X en pixels;
            float Cy_px = ((float)img.Size.Height) * 0.5F; // WaferCenter Y en pixels;       

            float fPixelSize_Map_X = (hmres.WaferSizeX_um) / ((float)_ImgMapSize_X);
            float fPixelSize_Map_Y = (hmres.WaferSizeY_um) / ((float)_ImgMapSize_Y);

            double RatioPixSize_X = hmres.PixelSizeX / fPixelSize_Map_X;
            double RatioPixSize_Y = hmres.PixelSizeY / fPixelSize_Map_Y;

            using (Graphics graphics = Graphics.FromImage(img))
            {
                // on se crée le fond tout en black
                graphics.FillRectangle(Brushes.Black, 0, 0, img.Width, img.Height);

                if (hmres.WaferType == eWaferType.Square)
                {
                    // on se cree un wafer rectangulaire
                    graphics.FillRectangle(Brushes.Gray, _ImgMapMargin_X, _ImgMapMargin_Y, _ImgMapSize_X, _ImgMapSize_Y);
                }
                else
                {
                    // on se cree un wafer rond
                    graphics.FillEllipse(Brushes.Gray, new Rectangle(_ImgMapMargin_X, _ImgMapMargin_Y, _ImgMapSize_X, _ImgMapSize_Y));

                    switch (hmres.WaferType)
                    {
                        case eWaferType.Round_Notch:
                            {
                                float fNotchdiameter_px = (float)(2850.0 / fPixelSize_Map_Y);
                                float fNotchRadius_px = fNotchdiameter_px * 0.5F;
                                RectangleF rectNotch = new RectangleF((Cx_px - fNotchRadius_px), (img.Size.Height - _ImgMapMargin_Y - fNotchRadius_px), (float)fNotchdiameter_px, fNotchdiameter_px);
                                graphics.FillEllipse(Brushes.Black, rectNotch);
                            }
                            break;
                        case eWaferType.Round_Flat:
                            {
                                // primary flat length selon SEMI
                                // pour 100mm = 32.5mm
                                // pour 150mm/200mm = 57.5mm
                                // pour 300mm = N/A

                                // longueur de corde [AB] = 2*radius*sin(alpha/2) où alpahe = angle AOB ou 0 est centre du cercle (A et B sont des pts du cercle)
                                // on veux la distance OH où H milieu de AB
                                // alpha = 2 * ArcSin(AB / 2R); et h = R*cos(alpha/2);
                                float fAB = 32500.0f;
                                if (hmres.WaferSizeY_um > 100000.0f)
                                    fAB = 57500.0f;
                                fAB /= /*2.0f * */fPixelSize_Map_Y;
                                double dRadius_Y_px = (double)hmres.WaferSizeY_um / (2.0f * fPixelSize_Map_Y);
                                double dAlpha_rd = 2.0 * Math.Asin(fAB / (2.0 * dRadius_Y_px));
                                double dH_px = dRadius_Y_px * Math.Cos(dAlpha_rd / 2.0);
                                graphics.FillRectangle(Brushes.Black, 0, Cy_px + (float)dH_px, img.Size.Width - 1, img.Size.Height - 1);
                            }
                            break;
                        case eWaferType.Round_DblFlat:
                            {
                                // primary flat length selon SEMI
                                // pour 100mm = 32.5mm
                                // pour 150mm/200mm = 57.5mm
                                // pour 300mm = N/A

                                // longueur de corde [AB] = 2*radius*sin(alpha/2) où alpahe = angle AOB ou 0 est centre du cercle (A et B sont des pts du cercle)
                                // on veux la distance OH où H milieu de AB
                                // alpha = 2 * ArcSin(AB / 2R); et h = R*cos(alpha/2);
                                float fAB = 32500.0f;
                                if (hmres.WaferSizeY_um > 100000.0f)
                                    fAB = 57500.0f;
                                fAB /= /*2.0f * */fPixelSize_Map_Y;
                                double dRadius_px = (double)hmres.WaferSizeY_um / (2.0f * fPixelSize_Map_Y);
                                double dAlpha_rd = 2.0 * Math.Asin(fAB / (2.0 * dRadius_px));
                                double dH_px = dRadius_px * Math.Cos(dAlpha_rd / 2.0);
                                graphics.FillRectangle(Brushes.Black, 0, Cy_px + (float)dH_px, img.Size.Width - 1, img.Size.Height - 1);

                                // Secondary flat length selon SEMI
                                // pour 100mm = 18.5mm
                                // pour 150mm = 37.5mm
                                // pour 200mm/300mm = N/A
                                fAB = 18500.0f;
                                if (hmres.WaferSizeY_um > 100000.0f)
                                    fAB = 37500.0f;
                                fAB /= /*2.0f * */fPixelSize_Map_X;
                                dRadius_px = (double)hmres.WaferSizeX_um / (2.0f * fPixelSize_Map_X);
                                dAlpha_rd = 2.0 * Math.Asin(fAB / (2.0 * dRadius_px));
                                dH_px = dRadius_px * Math.Cos(dAlpha_rd / 2.0);
                                graphics.FillRectangle(Brushes.Black, 0, 0, Cx_px - (float)dH_px, img.Size.Height - 1);
                            }
                            break;
                    }
                }
            }
            return img;
        }

        private static Bitmap MakeColorMap(List<Tuple<Color, float>> dtWColors)
        {
            int nX = HeightMapResults.g_nMaxRange;
            int nY = 1;
            Bitmap img = new Bitmap(nX, nY);
            using (Graphics graphics = Graphics.FromImage(img))
            {
                LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, nX, nY), Color.Black, Color.Black, 0, false);
                ColorBlend cb = new ColorBlend();
                cb.Positions = new float[dtWColors.Count];
                cb.Colors = new Color[dtWColors.Count];

                for (int i = 0; i < dtWColors.Count; i++)
                {
                    cb.Colors[i] = dtWColors[i].Item1;
                    cb.Positions[i] = dtWColors[i].Item2;
                }
                br.InterpolationColors = cb;

                graphics.FillRectangle(br, 0, 0, nX, nY);
            }
            //img.Save(@"E:\AltaData\Gradient.bmp", ImageFormat.Bmp);
            return img;
        }
        #endregion

        protected object DeepCopy()
        {
            HeightMapResults cloned = MemberwiseClone() as HeightMapResults;
            return cloned;
        }

        public virtual object Clone()
        {
            return DeepCopy();
        }
    }
}
