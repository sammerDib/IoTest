using System;
using System.Collections.Generic;
using System.Linq;

using AdcTools.Collection;

using Matrox.MatroxImagingLibrary;

namespace PatternInspectionTools
{

    public class PatternExecutor : ITakeable, IDisposable, ICloneable
    {

        public class InternComputePrm
        {
            public bool bOutList = false;
            public bool bOutBinary = false;
            public bool bOutRendering = false;

            public bool bUseCharacRoiID = false;
            public int nMinArea_px2 = -1;
            public int nMaxArea_px2 = -1;
        }

        #region Pattern Executor Data Member
        private MIL_ID m_MilSys = MIL.M_NULL;
        //PatObject m_oPatFileObj = null;

        private int m_nTrainPatternSize_X = 0;
        private int m_nTrainPatternSize_Y = 0;
        private bool m_bUseSobel = false;

        private object m_LockAlign = new object();
        private bool IsLocAlignRequested = false;
        private MIL_ID m_oPatAlign = MIL.M_NULL;

        private PatternLocAdvSettings m_oLocSettings; // settings for internal die alignement
        private PatternInspectSettings m_oInspSettings;

        private List<MIL_ID> m_ListMilRoiMaskID; // list that contains Ids of 1bitUnsigned buffers (m_oInspSettings.m_nNbRoi+1 Ids if inspection by roi, else one id only )
        private MIL_ID m_oPrediff_DARK;
        private MIL_ID m_oPrediff_BRIGHT;
        private MIL_ID m_oPreParams_DARK;
        private MIL_ID m_oPreParams_BRIGHT;
        private MIL_ID m_oRoiMask = MIL.M_NULL; // pour calcul des ROI ID dans un autre modules // ne doit pas etre FREE !!! MIL_ID partagé
        public MIL_ID GetRoiMask() { return m_oRoiMask; } // // ne doit pas etre FREE !!! MIL_ID partagé
        public void ForgetRoiMask() { m_oRoiMask = MIL.M_NULL; } // /!\ a utiliser avec précaution
        public int GetNbRoiRegion() { return m_oInspSettings.m_nNbROI; }

        // Noramalization Variables
        private List<double> m_dListNormalizationMean; //list (same size than m_ListMilRoiMaskID) contains mean of m_oMean in the corresponding roi
        private List<double> m_dListNormalizationStdDev;//list (same size than m_ListMilRoiMaskID) contains standard deviation of m_oMean in the corresponding roi
        private List<int> m_nListNormalizationMaxPeak;//list (same size than m_ListMilRoiMaskID) contains max histogram peak of m_oMean in the corresponding roi
        #endregion

        #region Inspection Run Data member
        //
        // Run Data - Variable Used in Inspect call for each instance of die (should not be shared between different thread hence it has been duplicated in CLone)
        // // RTI :for best performance results in MimArith buffer depth should be identical otherwise we could have some slower performances 
        private MIL_ID m_RD_MilAlign32FImageID = MIL.M_NULL;         // 32bit float image (size of pattern) which contains the translated image en normalized image
        private MIL_ID m_RD_MilBinPatternSizeResultID = MIL.M_NULL;  // Pattern size bin defect image - Size of input image - 32bit
        private MIL_ID m_RD_MilDiffID = MIL.M_NULL;              // Difference matrix In floating value (prediff - normalized inputimg) - Size of trained pattern - 32bits (for DARK Or BRIGHT)
        private MIL_ID m_RD_MilBinID = MIL.M_NULL;               // Binarized  differences for DARK Or BRIGHT  defect  - Size of trained pattern - 32bits (for DARK Or BRIGHT)
        private MIL_ID m_RD_MilBlobBinID = MIL.M_NULL;           // "blobarized" differences for DARK Or BRIGHT  defect  - Size of trained pattern - 1bits (for DARK Or BRIGHT)

        // ROI Inspect specific Run Data
        private MIL_ID m_RD_MilRoiDiffID = MIL.M_NULL;          // for roi INSPECT  - MilRoiDiffID buffer contains only roi pixel of m_RD_MilDiffID , others are set to 0
        private MIL_ID m_RD_MilRoiBinID = MIL.M_NULL;           // for roi INSPECT  - binarize buffer of MilRoiDiffID
        private MIL_ID m_RD_MilProvisoryImageID = MIL.M_NULL;


        private MIL_ID m_RD_MilStatContextID = MIL.M_NULL;
        private MIL_ID m_RD_MilStatResultID = MIL.M_NULL;
        private MIL_ID m_RD_MilBlobResultID = MIL.M_NULL;
        private MIL_ID m_RD_MilBlobContextFeatureListID = MIL.M_NULL;

        // Form max peak shift normalizatin algo
        private bool m_bUseMaxShiftPeakNormalization = false;
        private MIL_ID m_RD_MilHisto = MIL.M_NULL;
        private MIL_ID m_RD_MilImage8UBufID = MIL.M_NULL; // temporary buffer for conversion from float to 8 bits
        private MIL_ID m_RD_MilImageWithMaskID = MIL.M_NULL;

        private bool m_bIsCloned = false;

        #endregion // run data

        public PatternExecutor()
        {
            m_bIsCloned = false;
            m_MilSys = MIL.M_NULL;
        }

        public PatternExecutor(MIL_ID p_MilSys)
        {
            m_bIsCloned = false;
            m_MilSys = p_MilSys;
            m_ListMilRoiMaskID = new List<MIL_ID>();

            m_oPrediff_DARK = MIL.M_NULL;
            m_oPrediff_BRIGHT = MIL.M_NULL;
            m_oPreParams_DARK = MIL.M_NULL;
            m_oPreParams_BRIGHT = MIL.M_NULL;

            m_dListNormalizationMean = new List<double>();
            m_dListNormalizationStdDev = new List<double>();
            m_nListNormalizationMaxPeak = new List<int>();
        }


        public bool LoadFromPatFile(string p_sPatFilePath, out String p_sErrMsg)
        {
            bool bRes = true;
            string sShortFileName = System.IO.Path.GetFileNameWithoutExtension(p_sPatFilePath);
            p_sErrMsg = String.Empty;
            try
            {
                PatObject oPatFileObj = PatObject.OpenPatFile(m_MilSys, p_sPatFilePath);
                m_oInspSettings = (PatternInspectSettings)oPatFileObj.InspSettings.Clone();

                IsLocAlignRequested = (oPatFileObj.LocType == 1);

                m_nTrainPatternSize_X = oPatFileObj.TrainSize_X;
                m_nTrainPatternSize_Y = oPatFileObj.TrainSize_Y;
                m_bUseSobel = oPatFileObj.UseSobel;

                // Init et calcul matrice prédifference
                if (bRes)
                {
                    if (IsLocAlignRequested)
                    {
                        // Initialize Align Localisation Model finder object
                        m_oLocSettings = (PatternLocAdvSettings)oPatFileObj.LocSettings.Clone();
                        m_oPatAlign = oPatFileObj.InitAlign();
                    }

                    // Initialize Mil Buffers for Roi masks
                    if (!InitRoiMask(oPatFileObj.BufMask))
                    {
                        foreach (MIL_ID oMilMaskId in m_ListMilRoiMaskID)
                        {
                            if (oMilMaskId != MIL.M_NULL)
                            {
                                MIL.MbufFree(oMilMaskId);
                            }
                        }
                        m_ListMilRoiMaskID.Clear();
                        p_sErrMsg = String.Format("Load File Pattern Executor Exception for file <{0}> : Number of Roi in MaskData is not the same than the Number of Roi sets in the inpection parameters", sShortFileName);
                        bRes = false;
                    }
                    else
                    {
                        // Initialize Precomputed Difference matrices
                        InitPrediff(oPatFileObj.BufMean, oPatFileObj.BufStdDev);

                        if (!InitRunData())
                        {
                            p_sErrMsg = String.Format("Load File Pattern Executor Exception for file <{0}> : Problem in Run Data initialization", sShortFileName);
                            bRes = false;
                        }

                        // Initialize Normalization parameters and ROI
                        if (!InitNormalization(oPatFileObj.BufMean))
                        {
                            p_sErrMsg = String.Format("Load File Pattern Exception for file <{0}> : Problem in normalization initialization", sShortFileName);
                            bRes = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                p_sErrMsg = ex.Message;
                bRes = false;
            }
            finally
            {

            }

            return bRes;
        }

        private bool InitRoiMask(MIL_ID oMask)
        {
            // Clear m_ListMilRoiMaskID
            if (m_ListMilRoiMaskID.Count() > 0)
            {
                foreach (MIL_ID oMilMaskId in m_ListMilRoiMaskID)
                {
                    if (oMilMaskId != MIL.M_NULL)
                    {
                        MIL.MbufFree(oMilMaskId);
                    }
                }
                m_ListMilRoiMaskID.Clear();
            }
            // Variables
            MIL_ID MilCurrentRoiID = MIL.M_NULL;
            // Add Detection Roi mask to m_ListMilRoiMaskID
            MilCurrentRoiID = new MIL_ID();
            MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MilCurrentRoiID);
            MIL.MimBinarize(oMask, MilCurrentRoiID, MIL.M_NOT_EQUAL, 255, MIL.M_NULL);
            m_ListMilRoiMaskID.Add(MilCurrentRoiID);//Detection ROI is in the first place in the list (index 0)

            byte[] LutData = new byte[256];
            LutData[255] = (byte)255;

            //histogramme of m_oMask
            MIL_ID MilMaskHistID = MIL.M_NULL;
            MIL_INT[] HistMask = new MIL_INT[256];
            MIL.MimAllocResult(m_MilSys, 256, MIL.M_HIST_LIST, ref MilMaskHistID);
            MIL.MimHistogram(oMask, MilMaskHistID);
            MIL.MimGetResult(MilMaskHistID, MIL.M_VALUE, HistMask);
            if (MilMaskHistID != MIL.M_NULL)
            {
                MIL.MimFree(MilMaskHistID);
                MilMaskHistID = MIL.M_NULL;
            }

            if (m_oInspSettings.m_bInspectionByROI == 1) //Roi inspection ==> {m_nNbROI} Roi Masks are saved in the list (index 1 to m_nNbROI+1 in the list)
            {

                // Allocate mil buffers for each roi and Add their milId in the List
                for (int greyLevel = 0; greyLevel < 255; greyLevel++)
                {
                    if (HistMask[greyLevel] != 0)
                    {
                        MilCurrentRoiID = new MIL_ID();
                        MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT/*1 + MIL.M_UNSIGNED*/, MIL.M_IMAGE + MIL.M_PROC, ref MilCurrentRoiID);
                        MIL.MimBinarize(oMask, MilCurrentRoiID, MIL.M_EQUAL, greyLevel, MIL.M_NULL);
                        m_ListMilRoiMaskID.Add(MilCurrentRoiID);

                        LutData[greyLevel] = (byte)(m_ListMilRoiMaskID.Count - 2); //-2 car le premier est pris par le mask des detection et on veux l'indice à -1
                    }
                    else
                        LutData[greyLevel] = (byte)0;
                }
                if (m_ListMilRoiMaskID.Count() != m_oInspSettings.m_nNbROI + 1)
                {
                    // Error: Number of Roi in MaskData is not the same than the Number of Roi sets in the inpection parameters
                    return false;
                }

                for (int nRoiIndex = 1; nRoiIndex < m_ListMilRoiMaskID.Count; nRoiIndex++)
                {
                    if (m_oInspSettings.m_vnNormalizeMethod[nRoiIndex] == 2) // Meaning we are using "Max Peak Shift" normalization method
                    {
                        m_bUseMaxShiftPeakNormalization = true;
                        break;
                    }
                }
            }
            else
            {
                // pas de ROI inspection
                m_bUseMaxShiftPeakNormalization = (m_oInspSettings.m_vnNormalizeMethod[0] == 2);// Meaning we are using "Max Peak Shift" normalization method

                byte nIdx = 0;
                for (int greyLevel = 0; greyLevel < 255; greyLevel++)
                {
                    if (HistMask[greyLevel] != 0)
                    {
                        LutData[greyLevel] = nIdx;
                        nIdx++;
                    }
                    else
                        LutData[greyLevel] = (byte)0;
                }
            }

            MIL_ID milMskLut = MIL.M_NULL;
            MIL.MbufAlloc1d(m_MilSys, 256, 8 + MIL.M_UNSIGNED, MIL.M_LUT, ref milMskLut);
            MIL.MbufPut(milMskLut, LutData);
            MIL.MimLutMap(oMask, oMask, milMskLut);
            if (milMskLut != MIL.M_NULL)
            {
                MIL.MbufFree(milMskLut);
                milMskLut = MIL.M_NULL;
            }

            if (oMask != MIL.M_NULL)
            {
                MIL_INT nSizeW = MIL.MbufInquire(oMask, MIL.M_SIZE_X);
                MIL_INT nSizeH = MIL.MbufInquire(oMask, MIL.M_SIZE_Y);
                MIL.MbufAlloc2d(m_MilSys, nSizeW, nSizeH, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_oRoiMask);
                MIL.MbufCopy(oMask, m_oRoiMask);
                MIL.MimArith(m_oRoiMask, 1, m_oRoiMask, MIL.M_ADD_CONST); // le mask à 255 doit normalment passer à 0; c'est la region que l'on traite pas de toute façon
            }
            return true;
        }


        private bool InitNormalization(MIL_ID p_oMean)
        {
            bool bSuccess = true;
            m_dListNormalizationMean.Clear();
            m_dListNormalizationStdDev.Clear();
            m_nListNormalizationMaxPeak.Clear();

            if (m_oInspSettings.m_bInspectionByROI != 0) // normalization for each roi
            {
                bool bAllNone = true;
                for (int nRoiIndex = 1; nRoiIndex < m_ListMilRoiMaskID.Count; nRoiIndex++)
                    bAllNone &= (m_oInspSettings.m_vnNormalizeMethod[nRoiIndex] < 0 || m_oInspSettings.m_vnNormalizeMethod[nRoiIndex] > 2);

                if (bAllNone)
                    return true;
            }
            else if (m_oInspSettings.m_vnNormalizeMethod[0] < 0 || m_oInspSettings.m_vnNormalizeMethod[0] > 2)
            {
                return true;
            }

            //Alloc oMean 8 bit for MaxPeak measure
            MIL_ID MilMean8U = MIL.M_NULL;
            MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref MilMean8U);
            MIL.MbufCopy(p_oMean, MilMean8U);
            //Add parameters (mean, stdev and maxpeak)
            foreach (MIL_ID MilRoiMaskID in m_ListMilRoiMaskID)
            {
                // Mean / StdDev //
                double l_dMean = 0.0, l_dStdDev = 0.0;
                GetMeanStdDev(p_oMean, MilRoiMaskID, ref l_dMean, ref l_dStdDev);
                if (l_dStdDev <= 0.0 || l_dMean < 0)
                {
                    bSuccess = false;
                }
                else
                {
                    m_dListNormalizationMean.Add(l_dMean);
                    m_dListNormalizationStdDev.Add(l_dStdDev);
                }
                // Max Peak //
                if (m_bUseMaxShiftPeakNormalization == true)
                {
                    int nNormMaxPeak = MaxHistPeak(MilMean8U, MilRoiMaskID);
                    m_nListNormalizationMaxPeak.Add(nNormMaxPeak);
                }

            }
            //free
            if (MilMean8U != MIL.M_NULL)
            {
                MIL.MbufFree(MilMean8U);
                MilMean8U = MIL.M_NULL;
            }

            return bSuccess;

        }

        private bool InitPrediff(MIL_ID p_oMean, MIL_ID p_oStdDev)
        {
            // DARK //
            if (m_oInspSettings.m_nType == PatObject._DARK || m_oInspSettings.m_nType == PatObject._BOTH)
            {
                if (m_oPrediff_DARK != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oPrediff_DARK);
                    m_oPrediff_DARK = MIL.M_NULL;
                }
                if (m_oPreParams_DARK != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oPreParams_DARK);
                    m_oPreParams_DARK = MIL.M_NULL;
                }
                // Alloc Diff
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oPrediff_DARK);
                // Calculate Kdark*StdDev -- Ande Prepare Threshold Matrix
                if (m_oInspSettings.m_bInspectionByROI == 1) //Roi inspection
                {
                    MIL.MbufClear(m_oPrediff_DARK, 0.0); // need to be initialize for ROI inpsection

                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oPreParams_DARK);

                    for (int nRoiIndex = 1; nRoiIndex < m_ListMilRoiMaskID.Count; nRoiIndex++)
                    {
                        MIL_ID MilRoiMaskID = m_ListMilRoiMaskID[nRoiIndex];
                        MIL_ID MilRoiSensitivityID = MIL.M_NULL; //RoiSensitivity buffer will contain only K*StdDev pixel for the current Roi, others are set to 0

                        MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MilRoiSensitivityID);
                        MIL.MimArith(p_oStdDev, MilRoiMaskID, MilRoiSensitivityID, MIL.M_MULT); //set all pixel that are not in the current roi to 0 in MilRoiSensitivityID buffer
                        MIL.MimArith(MilRoiSensitivityID, m_oInspSettings.m_vfK[PatObject._DARK][nRoiIndex], MilRoiSensitivityID, MIL.M_MULT_CONST + MIL.M_SATURATION); // Multiply by the sensitivity K
                        MIL.MimArith(m_oPrediff_DARK, MilRoiSensitivityID, m_oPrediff_DARK, MIL.M_ADD + MIL.M_SATURATION); // Add this result to m_oPrediff_DARK

                        // Now RoiSensitivity buffer will contain only Threshold value for the current Roi, others are set to 0
                        MIL.MimArith(MilRoiMaskID, m_oInspSettings.m_vnTh[PatObject._DARK][nRoiIndex], MilRoiSensitivityID, MIL.M_MULT_CONST);
                        MIL.MimArith(m_oPreParams_DARK, MilRoiSensitivityID, m_oPreParams_DARK, MIL.M_ADD + MIL.M_SATURATION);

                        //free MilRoiSensitivityID
                        MIL.MbufFree(MilRoiSensitivityID);
                    }
                }
                else // Global inspection
                {
                    MIL.MimArith(p_oStdDev, m_oInspSettings.m_vfK[PatObject._DARK][0], m_oPrediff_DARK, MIL.M_MULT_CONST + MIL.M_SATURATION);
                }
                // Calculate Mean - K*StdDev
                MIL.MimArith(p_oMean, m_oPrediff_DARK, m_oPrediff_DARK, MIL.M_SUB + MIL.M_SATURATION);
            }
            // BRIGHT //
            if (m_oInspSettings.m_nType == PatObject._BRIGHT || m_oInspSettings.m_nType == PatObject._BOTH)
            {
                if (m_oPrediff_BRIGHT != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oPrediff_BRIGHT);
                    m_oPrediff_BRIGHT = MIL.M_NULL;
                }
                if (m_oPreParams_BRIGHT != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oPreParams_BRIGHT);
                    m_oPreParams_BRIGHT = MIL.M_NULL;
                }
                // Alloc Diff
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oPrediff_BRIGHT);
                // Calculate Kbright*StdDev
                if (m_oInspSettings.m_bInspectionByROI == 1) //Roi inspection
                {
                    MIL.MbufClear(m_oPrediff_BRIGHT, 0.0); // need to be initialize for ROI inpsection

                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oPreParams_BRIGHT);

                    for (int nRoiIndex = 1; nRoiIndex < m_ListMilRoiMaskID.Count; nRoiIndex++)
                    {
                        MIL_ID MilRoiMaskID = m_ListMilRoiMaskID[nRoiIndex];
                        MIL_ID MilRoiSensitivityID = MIL.M_NULL; //RoiSensitivity buffer will contain only K*StdDev pixel for the current Roi, others are set to 0
                        MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MilRoiSensitivityID);
                        MIL.MimArith(p_oStdDev, MilRoiMaskID, MilRoiSensitivityID, MIL.M_MULT); //set all pixel that are not in the current roi to 0 in MilRoiSensitivityID buffer
                        MIL.MimArith(MilRoiSensitivityID, m_oInspSettings.m_vfK[PatObject._BRIGHT][nRoiIndex], MilRoiSensitivityID, MIL.M_MULT_CONST + MIL.M_SATURATION); // Multiply by the sensitivity K
                        MIL.MimArith(m_oPrediff_BRIGHT, MilRoiSensitivityID, m_oPrediff_BRIGHT, MIL.M_ADD + MIL.M_SATURATION); // Add this result to m_oPrediff_BRIGHT

                        // Now RoiSensitivity buffer will contain only Threshold value for the current Roi, others are set to 0
                        MIL.MimArith(MilRoiMaskID, m_oInspSettings.m_vnTh[PatObject._BRIGHT][nRoiIndex], MilRoiSensitivityID, MIL.M_MULT_CONST);
                        MIL.MimArith(m_oPreParams_BRIGHT, MilRoiSensitivityID, m_oPreParams_BRIGHT, MIL.M_ADD + MIL.M_SATURATION);

                        //free MilRoiSensitivityID
                        MIL.MbufFree(MilRoiSensitivityID);
                    }
                }
                else // Global inspection
                {
                    MIL.MimArith(p_oStdDev, m_oInspSettings.m_vfK[PatObject._BRIGHT][0], m_oPrediff_BRIGHT, MIL.M_MULT_CONST + MIL.M_SATURATION);
                }
                // Calculate Mean + K*StdDev
                MIL.MimArith(p_oMean, m_oPrediff_BRIGHT, m_oPrediff_BRIGHT, MIL.M_ADD + MIL.M_SATURATION);
            }
            return true;
        }

        private bool InitRunData()
        {
            try
            {
                //alloc res      
                MIL.MimAlloc(m_MilSys, MIL.M_STATISTICS_CONTEXT, MIL.M_DEFAULT, ref m_RD_MilStatContextID);
                MIL.MimAllocResult(m_MilSys, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT, ref m_RD_MilStatResultID);

                // Pattern data
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilAlign32FImageID);
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilBinPatternSizeResultID);

                //alloc diff and bin image
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilDiffID);
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilBinID);
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilBlobBinID);

                if (m_oInspSettings.m_bInspectionByROI != 0)
                {
                    //alloc diff and bin image - for ROI Inspection
                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilRoiDiffID);
                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilRoiBinID);
                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilProvisoryImageID);

                }

                if (m_bUseMaxShiftPeakNormalization == true)
                {
                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilImage8UBufID);
                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_RD_MilImageWithMaskID);
                    MIL.MimAllocResult(m_MilSys, 256, MIL.M_HIST_LIST, ref m_RD_MilHisto);
                }

                //
                // Blob INitialisation
                //
                // Allocate a feature list.
                MIL.MblobAlloc(m_MilSys, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_RD_MilBlobContextFeatureListID);
                // Allocate a blob result buffer.
                MIL.MblobAllocResult(m_MilSys, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_RD_MilBlobResultID);

            }
            catch (Exception)
            {
                ReleaseRunData();
                return false;
            }
            return true;
        }

        private void GetMeanStdDev(MIL_ID p_MilSrcImageBufId, MIL_ID p_MilMaskBufId, ref double p_dMean, ref double p_dStdDev)
        {
            if (m_RD_MilStatResultID != MIL.M_NULL)
            {
                // avoid deleting existing source image when applying mask
                MIL_ID milSrc = MIL.M_NULL;
                MIL.MbufClone(p_MilSrcImageBufId, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, ref milSrc);

                MIL.MimArith(milSrc, p_MilMaskBufId, milSrc, MIL.M_MULT);

                MIL.MimControl(m_RD_MilStatContextID, MIL.M_STAT_MEAN, MIL.M_ENABLE);
                MIL.MimControl(m_RD_MilStatContextID, MIL.M_STAT_STANDARD_DEVIATION, MIL.M_ENABLE);
                MIL.MimControl(m_RD_MilStatContextID, MIL.M_CONDITION, MIL.M_GREATER);
                MIL.MimControl(m_RD_MilStatContextID, MIL.M_COND_LOW, 0.0);

                //mean/stddev calc
                MIL.MimStatCalculate(m_RD_MilStatContextID, milSrc, m_RD_MilStatResultID, MIL.M_DEFAULT);

                //get results
                MIL.MimGetResult(m_RD_MilStatResultID, MIL.M_STAT_MEAN, ref p_dMean);
                MIL.MimGetResult(m_RD_MilStatResultID, MIL.M_STAT_STANDARD_DEVIATION, ref p_dStdDev);

                //clean stats
                MIL.MimControl(m_RD_MilStatContextID, MIL.M_STAT_MEAN, MIL.M_DISABLE);
                MIL.MimControl(m_RD_MilStatContextID, MIL.M_STAT_STANDARD_DEVIATION, MIL.M_DISABLE);
                MIL.MimControl(m_RD_MilStatContextID, MIL.M_CONDITION, MIL.M_DISABLE);
                MIL.MimControl(m_RD_MilStatContextID, MIL.M_COND_LOW, MIL.M_DISABLE);

                MIL.MbufFree(milSrc);
            }
        }

        private int MaxHistPeak(MIL_ID p_MilSrcImageBufId, MIL_ID p_MilMaskBufId)
        {
            // PARAMETERS :
            // p_MilSrcImageBufId --> 8bits image
            // p_MilMaskBufId --> 8bits mask 0=dont care, 1 otherwise
            MIL.MimArith(p_MilSrcImageBufId, p_MilMaskBufId, m_RD_MilImageWithMaskID, MIL.M_MULT);

            //count 0 in mask
            MIL_INT nNumberZeroMask = 0;
            MIL.MimControl(m_RD_MilStatContextID, MIL.M_STAT_NUMBER, MIL.M_ENABLE);
            MIL.MimControl(m_RD_MilStatContextID, MIL.M_CONDITION, MIL.M_EQUAL);
            MIL.MimControl(m_RD_MilStatContextID, MIL.M_COND_LOW, 0);

            MIL.MimStatCalculate(m_RD_MilStatContextID, p_MilMaskBufId, m_RD_MilStatResultID, MIL.M_DEFAULT);

            MIL.MimGetResult(m_RD_MilStatResultID, MIL.M_STAT_NUMBER + MIL.M_TYPE_MIL_INT, ref nNumberZeroMask);
            MIL.MimControl(m_RD_MilStatContextID, MIL.M_STAT_NUMBER, MIL.M_DISABLE);
            MIL.MimControl(m_RD_MilStatContextID, MIL.M_CONDITION, MIL.M_DISABLE);
            MIL.MimControl(m_RD_MilStatContextID, MIL.M_COND_LOW, MIL.M_DISABLE);

            int nMaxPeak = 0;
            // Max Peak //
            MIL_INT[] nHist = new MIL_INT[256];
            //histo de maskdata
            MIL.MimHistogram(m_RD_MilImageWithMaskID, m_RD_MilHisto);
            MIL.MimGetResult(m_RD_MilHisto, MIL.M_VALUE, nHist);
            nHist[0] = nHist[0] - nNumberZeroMask;
            int nMax = 0;
            for (int greyLevel = 0; greyLevel < 256; greyLevel++)
            {
                if (nHist[greyLevel] > nMax)
                {
                    nMax = (int)nHist[greyLevel];
                    nMaxPeak = greyLevel;
                }
            }
            return nMaxPeak;
        }

        public bool Inspect(MIL_ID p_pMilImageSrc)
        {
            // on copie le binary dans la source
            return Inspect(p_pMilImageSrc, ref p_pMilImageSrc);
        }

        public bool Inspect(MIL_ID p_pMilImageSrc, ref MIL_ID p_pMilImageBinary)
        {
            // Warning :
            // Expected image is a 8Bits grayscale Image larger or equal to Trained pattern;
            // - Pattern should be trained, mask(s) and inspection parameter should be set prior to this inspection
            // --> Use Altatech Pattern Inspection Software suite (PatternTraining, PatterExplorer, RoiEditor, PatternInspect)...

            bool bSuccesInspection = true;
            //localization
            double dOffsetX = 0.0;          // alignment offset in image to match with input image ON X axis - in pixels
            double dOffsetY = 0.0;          // alignment offset in image to match with input image ON X axis - in pixels

            if (IsCancellationRequested)
                return false;

            ///////////
            // Align //
            ///////////
            if (IsLocAlignRequested)
            {
                if (!AlignMil(p_pMilImageSrc, ref dOffsetX, ref dOffsetY))
                    bSuccesInspection = false;
            }

            if ((dOffsetX == 0.0) && (dOffsetY == 0.0))
            {
                MIL.MbufCopy(p_pMilImageSrc, m_RD_MilAlign32FImageID);
            }
            else
            {
                // MIL.MbufClear(m_RD_MilAlign32FImageID, 0.0); // Reset pixel in Image Buffer //USELESS ?
                MIL.MimTranslate(p_pMilImageSrc, m_RD_MilAlign32FImageID, -dOffsetX, -dOffsetY, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR); //translate pixel unchanged are cleared
            }


            if (m_bUseSobel && (m_RD_MilAlign32FImageID != MIL.M_NULL))
            {
                MIL.MimEdgeDetect(m_RD_MilAlign32FImageID, m_RD_MilAlign32FImageID, MIL.M_NULL, MIL.M_SOBEL, MIL.M_DEFAULT, MIL.M_NULL);
            }

            ///////////////////
            // Normalization //
            ///////////////////
            if (m_RD_MilAlign32FImageID == MIL.M_NULL)
            {
                // pb lors de la translation ou allocation
                bSuccesInspection = false;
            }
            else if (!Normalize(m_RD_MilAlign32FImageID))
            {
                // normalization failure
                bSuccesInspection = false;
            }
            else
            {
                ////////////////
                // Inspection //
                ////////////////
                MIL.MbufClear(m_RD_MilBinPatternSizeResultID, 0);

                /////////////////////
                // Dark inspection //
                /////////////////////
                if (m_oPrediff_DARK != MIL.M_NULL && bSuccesInspection)
                {
                    //substraction (prediff dark - normalized image)
                    MIL.MimArith(m_oPrediff_DARK, m_RD_MilAlign32FImageID, m_RD_MilDiffID, MIL.M_SUB + MIL.M_SATURATION);
                    MIL.MimArith(m_RD_MilDiffID, m_ListMilRoiMaskID[0], m_RD_MilDiffID, MIL.M_MULT + MIL.M_SATURATION); // delete no detection roi (set to 0)

                    // Binarize //
                    if (m_oInspSettings.m_bInspectionByROI != 0)
                    {
                        // new optimize method - all threshold parameters of ROI are set in one matrix m_oPreParams_DARK
                        MIL.MimArith(m_RD_MilDiffID, m_oPreParams_DARK, m_RD_MilDiffID, MIL.M_SUB + MIL.M_SATURATION);
                        MIL.MimBinarize(m_RD_MilDiffID, m_RD_MilBinID, MIL.M_GREATER, 0, MIL.M_NULL);
                    }
                    else
                    {
                        MIL.MimBinarize(m_RD_MilDiffID, m_RD_MilBinID, MIL.M_GREATER, m_oInspSettings.m_vnTh[PatObject._DARK][0], MIL.M_NULL);
                    }

                    if (IsCancellationRequested)
                        return false;

                    /////////////////////////
                    // Dark Blob Exclusion //
                    /////////////////////////
                    PerformBlobExclusion(m_RD_MilBinID, m_oInspSettings.m_nBlobExArea[PatObject._DARK], (double)m_oInspSettings.m_fBlobExElongation[PatObject._DARK], m_oInspSettings.m_nBlobExBreadth[PatObject._DARK]);

                    // Add Result //
                    MIL.MimArith(m_RD_MilBinPatternSizeResultID, m_RD_MilBinID, m_RD_MilBinPatternSizeResultID, MIL.M_ADD);

                }//end dark inspection

                ///////////////////////
                // Bright inspection //
                ///////////////////////
                if (m_oPrediff_BRIGHT != MIL.M_NULL && bSuccesInspection)
                {

                    //substraction (normalized image - prediff bright)
                    MIL.MimArith(m_RD_MilAlign32FImageID, m_oPrediff_BRIGHT, m_RD_MilDiffID, MIL.M_SUB + MIL.M_SATURATION);
                    MIL.MimArith(m_RD_MilDiffID, m_ListMilRoiMaskID[0], m_RD_MilDiffID, MIL.M_MULT + MIL.M_SATURATION); // delete no detection roi (set to 0)

                    // Binarize //
                    if (m_oInspSettings.m_bInspectionByROI != 0)
                    {
                        // new optimize method - all threshold parameters of ROI are set in one matrix m_oPreParams_BRIGHT
                        MIL.MimArith(m_RD_MilDiffID, m_oPreParams_BRIGHT, m_RD_MilDiffID, MIL.M_SUB + MIL.M_SATURATION);
                        MIL.MimBinarize(m_RD_MilDiffID, m_RD_MilBinID, MIL.M_GREATER, 0, MIL.M_NULL);
                    }
                    else
                    {
                        MIL.MimBinarize(m_RD_MilDiffID, m_RD_MilBinID, MIL.M_GREATER, m_oInspSettings.m_vnTh[PatObject._BRIGHT][0], MIL.M_NULL);
                    }


                    if (IsCancellationRequested)
                        return false;


                    ///////////////////////////
                    // Bright Blob Exclusion //
                    ///////////////////////////
                    PerformBlobExclusion(m_RD_MilBinID, m_oInspSettings.m_nBlobExArea[PatObject._BRIGHT], (double)m_oInspSettings.m_fBlobExElongation[PatObject._BRIGHT], m_oInspSettings.m_nBlobExBreadth[PatObject._BRIGHT]);

                    // Add Result //
                    MIL.MimArith(m_RD_MilBinPatternSizeResultID, m_RD_MilBinID, m_RD_MilBinPatternSizeResultID, MIL.M_ADD);
                }//end bright inspection

                ////////////////////
                // Translate Back //
                ////////////////////
                if (p_pMilImageBinary == MIL.M_NULL)
                {
                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref p_pMilImageBinary);
                }
                if ((dOffsetX == 0.0) && (dOffsetY == 0.0))
                {
                    MIL.MbufCopy(m_RD_MilBinPatternSizeResultID, p_pMilImageBinary);
                }
                else
                {
                    MIL_INT iOffsetX = Convert.ToInt32(dOffsetX); // using integer value in MimTranslate avoids blob characteristic modifications
                    MIL_INT iOffsetY = Convert.ToInt32(dOffsetY);
                    MIL.MimTranslate(m_RD_MilBinPatternSizeResultID, m_RD_MilBinPatternSizeResultID, iOffsetX, iOffsetY, MIL.M_NEAREST_NEIGHBOR + MIL.M_OVERSCAN_CLEAR); //translate pixel unchanged are cleared
                    MIL.MimBinarize(m_RD_MilBinPatternSizeResultID, p_pMilImageBinary, MIL.M_NOT_EQUAL, 0, MIL.M_NULL);
                }
            }// end else (normalization ok)

            ///////////////////////
            // End of Inspection //
            ///////////////////////
            if (!bSuccesInspection) // if the inspection did not succeed, all pixel are set on defect (Could be alignment failure, or unexpected errors)
            {
                MIL.MbufClear(p_pMilImageBinary, 255.0);
            }

            return bSuccesInspection;
        }

        private bool AlignMil(MIL_ID p_MILImageSrc, ref double p_dOffsetX, ref double p_dOffsetY)
        {
            // First, Test that Regeion Of Interest for Localisation is not bigger than the image
            MIL_INT nSizeW = MIL.MbufInquire(p_MILImageSrc, MIL.M_SIZE_X);
            MIL_INT nSizeH = MIL.MbufInquire(p_MILImageSrc, MIL.M_SIZE_Y);

            if ((nSizeW < m_oLocSettings.m_nROIOrigin_X + m_oLocSettings.m_nROISize_sx) || (nSizeH < m_oLocSettings.m_nROIOrigin_Y + m_oLocSettings.m_nROISize_sy))
            {
                return false;
            }

            // Variables 
            bool bSuccess = true;
            MIL_ID imgChild = MIL.M_NULL;
            MIL_ID MilAlignResult = MIL.M_NULL;
            MIL_INT NumResults = 0L;
            double dXPosition = 0.0; /* Model X position.       */
            double dYPosition = 0.0; /* Model Y position.       */

            // Keep only the ROI in the image to locate the LocImage
            MIL.MbufChild2d(p_MILImageSrc, m_oLocSettings.m_nROIOrigin_X, m_oLocSettings.m_nROIOrigin_Y, m_oLocSettings.m_nROISize_sx, m_oLocSettings.m_nROISize_sy, ref imgChild);

            ///////////////////////////
            // MODEL GEOMETRY FINDER //
            ///////////////////////////

            // Alloc Result
            MIL.MmodAllocResult(m_MilSys, MIL.M_DEFAULT, ref MilAlignResult);

            // Find the model. 
            lock (m_LockAlign) // lock the alignment because m_oPatAlign cannot be shared in multithreading
            {
                MIL.MmodFind(m_oPatAlign, imgChild, MilAlignResult);
            }

            /* Get the number of models found. */
            MIL.MmodGetResult(MilAlignResult, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref NumResults);
            /* If a model was found above the acceptance threshold. */
            if ((NumResults >= 1))
            {
                /* Get the results of the single model. */
                MIL.MmodGetResult(MilAlignResult, MIL.M_DEFAULT, MIL.M_POSITION_X, ref dXPosition);
                MIL.MmodGetResult(MilAlignResult, MIL.M_DEFAULT, MIL.M_POSITION_Y, ref dYPosition);

                p_dOffsetX = dXPosition + m_oLocSettings.m_nROIOrigin_X;
                p_dOffsetY = dYPosition + m_oLocSettings.m_nROIOrigin_Y;
                bSuccess = true;
            }
            else
            {
                p_dOffsetX = 0.0;
                p_dOffsetY = 0.0;
                bSuccess = false;
            }

            //free Mil buffer
            if (imgChild != MIL.M_NULL)
            {
                MIL.MbufFree(imgChild);
                imgChild = MIL.M_NULL;
            }
            if (MilAlignResult != MIL.M_NULL)
            {
                MIL.MmodFree(MilAlignResult);
                MilAlignResult = MIL.M_NULL;
            }

            return bSuccess;
        }

        private bool Normalize(MIL_ID p_MilImageID)
        {
            bool bSuccessNormalize = true;

            if (m_bUseMaxShiftPeakNormalization)
                MIL.MbufCopy(p_MilImageID, m_RD_MilImage8UBufID);// conversion float -> 8bits

            ///////////////////////
            // ROI NORMALIZATION //
            ///////////////////////
            if (m_oInspSettings.m_bInspectionByROI != 0) // normalization for each roi
            {
                MIL.MbufClear(m_RD_MilProvisoryImageID, 0.0); //clear the buffer, each roi normalization is added successively

                for (int nRoiIndex = 1; nRoiIndex < m_ListMilRoiMaskID.Count; nRoiIndex++)
                {
                    if (IsCancellationRequested)
                        return false;

                    MIL_ID MilRoiMaskID = m_ListMilRoiMaskID[nRoiIndex];
                    MIL.MimArith(p_MilImageID, MilRoiMaskID, m_RD_MilRoiDiffID, MIL.M_MULT);

                    switch (m_oInspSettings.m_vnNormalizeMethod[nRoiIndex])
                    {
                        case 0:// Similar Mean / StdDev //
                            {
                                double l_dMean = 0.0, l_dStdDev = 0.0;
                                GetMeanStdDev(p_MilImageID, MilRoiMaskID, ref l_dMean, ref l_dStdDev);
                                if (l_dStdDev <= 0.0 || l_dMean < 0.0)
                                {
                                    bSuccessNormalize = false;
                                }
                                else
                                {
                                    //sub mean roi
                                    MIL.MimArith(m_RD_MilRoiDiffID, (double)(l_dMean), m_RD_MilRoiDiffID, MIL.M_SUB_CONST + MIL.M_SATURATION);
                                    //mult stddevPattern/stddevRoi
                                    MIL.MimArith(m_RD_MilRoiDiffID, (double)(m_dListNormalizationStdDev[nRoiIndex] / l_dStdDev), m_RD_MilRoiDiffID, MIL.M_MULT_CONST + MIL.M_SATURATION);
                                    //add mean pattern
                                    MIL.MimArith(m_RD_MilRoiDiffID, (double)(m_dListNormalizationMean[nRoiIndex]), m_RD_MilRoiDiffID, MIL.M_ADD_CONST + MIL.M_SATURATION);
                                }
                                break;
                            }
                        case 1:// Similar Mean //
                            {
                                double l_dMean = 0.0, l_dStdDev = 0.0;
                                GetMeanStdDev(p_MilImageID, MilRoiMaskID, ref l_dMean, ref l_dStdDev);
                                if (l_dMean < 0.0)
                                {
                                    bSuccessNormalize = false;
                                }
                                else
                                {
                                    //add (mean pattern - mean roi)
                                    MIL.MimArith(m_RD_MilRoiDiffID, (double)(m_dListNormalizationMean[nRoiIndex] - l_dMean), m_RD_MilRoiDiffID, MIL.M_ADD_CONST + MIL.M_SATURATION);
                                }
                                break;
                            }
                        case 2:// Max Peak Shift //
                            {
                                int nMaxPeakImage = 0;
                                nMaxPeakImage = MaxHistPeak(m_RD_MilImage8UBufID, MilRoiMaskID);
                                MIL.MimArith(m_RD_MilRoiDiffID, (double)(m_nListNormalizationMaxPeak[nRoiIndex] - nMaxPeakImage), m_RD_MilRoiDiffID, MIL.M_ADD_CONST + MIL.M_SATURATION);

                                break;
                            }
                        default:// No Normalization //
                            {
                                break;
                            }
                    }
                    // Add Normalized Roi
                    MIL.MimArith(m_RD_MilRoiDiffID, MilRoiMaskID, m_RD_MilRoiDiffID, MIL.M_MULT);//set all pixel of MilRoiImageID which are not in the roi to 0 before add it to m_RD_MilProvisoryImageID
                    MIL.MimArith(m_RD_MilRoiDiffID, m_RD_MilProvisoryImageID, m_RD_MilProvisoryImageID, MIL.M_ADD + MIL.M_SATURATION);//add it to m_RD_MilProvisoryImageID
                }

                MIL.MbufCopy(m_RD_MilProvisoryImageID, p_MilImageID);
            }
            //////////////////////////
            // GLOBAL NORMALIZATION //
            //////////////////////////
            else // global normalization
            {
                switch (m_oInspSettings.m_vnNormalizeMethod[0])
                {
                    case 0:// Mean / StdDev //
                        {
                            double l_dMean = 0.0, l_dStdDev = 0.0;
                            GetMeanStdDev(p_MilImageID, m_ListMilRoiMaskID[0], ref l_dMean, ref l_dStdDev);
                            if (l_dStdDev <= 0.0 || l_dMean < 0)
                            {
                                bSuccessNormalize = false;
                            }
                            else
                            {
                                //sub mean image
                                MIL.MimArith(p_MilImageID, (double)(l_dMean), p_MilImageID, MIL.M_SUB_CONST + MIL.M_SATURATION);
                                //mult stddevPattern/stddevImage
                                MIL.MimArith(p_MilImageID, (double)(m_dListNormalizationStdDev[0] / l_dStdDev), p_MilImageID, MIL.M_MULT_CONST + MIL.M_SATURATION);
                                //add mean pattern
                                MIL.MimArith(p_MilImageID, (double)(m_dListNormalizationMean[0]), p_MilImageID, MIL.M_ADD_CONST + MIL.M_SATURATION);
                            }
                            break;
                        }
                    case 1:// Similar Mean  //
                        {
                            double l_dMean = 0.0, l_dStdDev = 0.0;
                            GetMeanStdDev(p_MilImageID, m_ListMilRoiMaskID[0], ref l_dMean, ref l_dStdDev);
                            if (l_dMean < 0)
                            {
                                bSuccessNormalize = false;
                            }
                            else
                            {
                                // add (mean pattern - mean image)
                                MIL.MimArith(p_MilImageID, (double)(m_dListNormalizationMean[0] - l_dMean), p_MilImageID, MIL.M_ADD_CONST + MIL.M_SATURATION);
                            }
                            break;
                        }
                    case 2:// Max Peak //
                        {
                            int nMaxPeakImage = 0;
                            nMaxPeakImage = MaxHistPeak(m_RD_MilImage8UBufID, m_ListMilRoiMaskID[0]);
                            MIL.MimArith(p_MilImageID, (double)(nMaxPeakImage - m_nListNormalizationMaxPeak[0]), p_MilImageID, MIL.M_ADD_CONST + MIL.M_SATURATION);
                            break;
                        }
                    default:// No Normalization //
                        {
                            break;
                        }
                }
            }
            return bSuccessNormalize;
        }

        private void PerformBlobExclusion(MIL_ID p_MilBinImageID, int p_nBlobArea, double p_dBlobElongation, int p_nBlobBreadth)
        {
            // RTI : blob need a 8 or 1 bits buffer to perform we need to convert our float mask buffer !!!
            MIL.MbufCopy(p_MilBinImageID, m_RD_MilBlobBinID);

            MIL_INT nTotalBlobs = 0;

            /* Enable the Area and Center Of Gravity feature calculation. */
            if (p_dBlobElongation > 0.0)
                MIL.MblobControl(m_RD_MilBlobContextFeatureListID, MIL.M_FERET_ELONGATION, MIL.M_ENABLE);
            if (p_nBlobBreadth > 0)
                MIL.MblobControl(m_RD_MilBlobContextFeatureListID, MIL.M_BREADTH, MIL.M_ENABLE);
            // Calculate selected features for each blob. 
            MIL.MblobCalculate(m_RD_MilBlobBinID, MIL.M_NULL, m_RD_MilBlobContextFeatureListID, m_RD_MilBlobResultID);

            // Exclude blobs which do not fit settings
            if (p_nBlobArea > 0)
                MIL.MblobSelect(m_RD_MilBlobResultID, MIL.M_EXCLUDE, MIL.M_AREA, MIL.M_LESS_OR_EQUAL, p_nBlobArea, MIL.M_NULL);
            if (p_dBlobElongation > 0.0)
                MIL.MblobSelect(m_RD_MilBlobResultID, MIL.M_EXCLUDE, MIL.M_FERET_ELONGATION, MIL.M_GREATER_OR_EQUAL, p_dBlobElongation, MIL.M_NULL);
            if (p_nBlobBreadth > 0)
                MIL.MblobSelect(m_RD_MilBlobResultID, MIL.M_EXCLUDE, MIL.M_BREADTH, MIL.M_LESS_OR_EQUAL, p_nBlobBreadth, MIL.M_NULL);

            MIL.MbufClear(m_RD_MilBlobBinID, 0); // This image should have been cleared before add Included blob!
            //MIL 10
            //     MIL.MblobFill(m_RD_MilBlobResultID, m_RD_MilBlobBinID, MIL.M_INCLUDED_BLOBS, 255);
            //MIL X
            MIL.MblobDraw(MIL.M_DEFAULT, m_RD_MilBlobResultID, m_RD_MilBlobBinID, MIL.M_DRAW_BLOBS, MIL.M_INCLUDED_BLOBS, MIL.M_DEFAULT);
            // TO DO : check si les blobs sont a 255 et pas à 1 ou à TypeMax

            // put it back in float buffer
            MIL.MbufCopy(m_RD_MilBlobBinID, p_MilBinImageID);
        }

        #region IDisposable Members and Methods

        private bool m_hasDisposed = false;

        //Finalizer
        ~PatternExecutor()
        {
            // The object went out of scope and finalized is called. Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC runs the next time.
            Dispose(false);
        }
        public void Dispose()
        {
            // If this function is being called the user wants to release the resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_hasDisposed == false)
            {
                if (disposing == true)
                {
                    //someone want the deterministic release of all resources Let us release all the managed resources
                    ReleaseManagedResources();
                }
                else
                {
                    // Do nothing, no one asked a dispose, the object went out of scope and finalized is called so lets next round of GC 
                    // release these resources
                }

                // Release the unmanaged resource in any case as they will not be released by GC
                ReleaseUnmanagedResources();

                m_hasDisposed = true;
            }
            else
            {
                // Object already been disposed - avoid MS exception
            }
        }

        private void ReleaseManagedResources()
        {
            //Console.WriteLine("Releasing Managed Resources");

            //             if (tr != null)
            //             {
            //                 tr.Dispose();
            //                 tr = null;
            //             }
        }

        private void ReleaseRunData()
        {
            //free memory
            if (m_RD_MilStatResultID != MIL.M_NULL)
            {
                MIL.MimFree(m_RD_MilStatResultID);
                m_RD_MilStatResultID = MIL.M_NULL;
            }
            if (m_RD_MilHisto != MIL.M_NULL)
            {
                MIL.MimFree(m_RD_MilHisto);
                m_RD_MilHisto = MIL.M_NULL;
            }

            // Free MIL buffers
            if (m_RD_MilAlign32FImageID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilAlign32FImageID);
                m_RD_MilAlign32FImageID = MIL.M_NULL;
            }
            if (m_RD_MilBinPatternSizeResultID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilBinPatternSizeResultID);
                m_RD_MilBinPatternSizeResultID = MIL.M_NULL;
            }

            if (m_RD_MilDiffID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilDiffID);
                m_RD_MilDiffID = MIL.M_NULL;
            }
            if (m_RD_MilBinID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilBinID);
                m_RD_MilBinID = MIL.M_NULL;
            }
            if (m_RD_MilBlobBinID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilBlobBinID);
                m_RD_MilBlobBinID = MIL.M_NULL;
            }
            // ROI inspection run data
            if (m_RD_MilRoiDiffID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilRoiDiffID);
                m_RD_MilRoiDiffID = MIL.M_NULL;
            }
            if (m_RD_MilRoiBinID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilRoiBinID);
                m_RD_MilRoiBinID = MIL.M_NULL;
            }
            if (m_RD_MilProvisoryImageID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilProvisoryImageID);
                m_RD_MilProvisoryImageID = MIL.M_NULL;
            }

            if (m_RD_MilImage8UBufID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilImage8UBufID);
                m_RD_MilImage8UBufID = MIL.M_NULL;
            }
            if (m_RD_MilImageWithMaskID != MIL.M_NULL)
            {
                MIL.MbufFree(m_RD_MilImageWithMaskID);
                m_RD_MilImageWithMaskID = MIL.M_NULL;
            }

            // free blob memory
            if (m_RD_MilBlobResultID != MIL.M_NULL)
            {
                MIL.MblobFree(m_RD_MilBlobResultID);
                m_RD_MilBlobResultID = MIL.M_NULL;
            }
            if (m_RD_MilBlobContextFeatureListID != MIL.M_NULL)
            {
                MIL.MblobFree(m_RD_MilBlobContextFeatureListID);
                m_RD_MilBlobContextFeatureListID = MIL.M_NULL;
            }

        }

        private void ReleaseInitialData()
        {
            if (m_oPrediff_DARK != MIL.M_NULL)
            {
                MIL.MbufFree(m_oPrediff_DARK);
                m_oPrediff_DARK = MIL.M_NULL;
            }

            if (m_oPrediff_BRIGHT != MIL.M_NULL)
            {
                MIL.MbufFree(m_oPrediff_BRIGHT);
                m_oPrediff_BRIGHT = MIL.M_NULL;
            }

            if (m_oPreParams_DARK != MIL.M_NULL)
            {
                MIL.MbufFree(m_oPreParams_DARK);
                m_oPreParams_DARK = MIL.M_NULL;
            }

            if (m_oPreParams_BRIGHT != MIL.M_NULL)
            {
                MIL.MbufFree(m_oPreParams_BRIGHT);
                m_oPreParams_BRIGHT = MIL.M_NULL;
            }

            if (m_oRoiMask != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRoiMask);
                m_oRoiMask = MIL.M_NULL;
            }

            for (int i = 0; i < m_ListMilRoiMaskID.Count(); i++)
            {
                if (m_ListMilRoiMaskID[i] != MIL.M_NULL)
                {
                    MIL.MbufFree(m_ListMilRoiMaskID[i]);
                    m_ListMilRoiMaskID[i] = MIL.M_NULL;
                }
            }
            m_ListMilRoiMaskID.Clear();
        }

        private void ReleaseUnmanagedResources()
        {
            ReleaseRunData();

            ReleaseInitialData();

            if (!m_bIsCloned)
            {
                // ces données là sont partagée entre les threads

                /* // m_oRoiMask est partgé et le owner ship a été passé au module qui le disposera en temps voulu
                 * if (m_oRoiMask != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oRoiMask);
                    m_oRoiMask = MIL.M_NULL;
                }*/
            }
        }
        #endregion

        #region ICloneable Members and methods

        protected void CloneData(ref PatternExecutor cloned)
        {
            CopyInitialData(ref cloned);
            cloned.InitRunData();
        }

        protected void CopyInitialData(ref PatternExecutor cloned)
        {
            cloned.m_oRoiMask = MIL.M_NULL;
            cloned.m_ListMilRoiMaskID = new List<MIL_ID>();
            foreach (MIL_ID oMilMaskId in m_ListMilRoiMaskID)
            {
                if (oMilMaskId != MIL.M_NULL)
                {
                    MIL_ID MiClonedRoiID = new MIL_ID();
                    MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MiClonedRoiID);
                    MIL.MbufCopy(oMilMaskId, MiClonedRoiID);
                    cloned.m_ListMilRoiMaskID.Add(MiClonedRoiID);
                }
            }

            if (m_oPrediff_DARK != MIL.M_NULL)
            {
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref cloned.m_oPrediff_DARK);
                MIL.MbufCopy(m_oPrediff_DARK, cloned.m_oPrediff_DARK);
            }
            if (m_oPrediff_BRIGHT != MIL.M_NULL)
            {
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref cloned.m_oPrediff_BRIGHT);
                MIL.MbufCopy(m_oPrediff_BRIGHT, cloned.m_oPrediff_BRIGHT);
            }
            if (m_oPreParams_DARK != MIL.M_NULL)
            {
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref cloned.m_oPreParams_DARK);
                MIL.MbufCopy(m_oPreParams_DARK, cloned.m_oPreParams_DARK);
            }
            if (m_oPreParams_BRIGHT != MIL.M_NULL)
            {
                MIL.MbufAlloc2d(m_MilSys, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref cloned.m_oPreParams_BRIGHT);
                MIL.MbufCopy(m_oPreParams_BRIGHT, cloned.m_oPreParams_BRIGHT);
            }

            cloned.m_dListNormalizationMean = new List<double>();
            cloned.m_dListNormalizationMean.AddRange(m_dListNormalizationMean);
            cloned.m_dListNormalizationStdDev = new List<double>();
            cloned.m_dListNormalizationStdDev.AddRange(m_dListNormalizationStdDev);
            cloned.m_nListNormalizationMaxPeak = new List<int>();
            cloned.m_nListNormalizationMaxPeak.AddRange(m_nListNormalizationMaxPeak);
        }

        protected object DeepCopy()
        {
            PatternExecutor cloned = MemberwiseClone() as PatternExecutor;
            // ici on clone les instances nécéssaire au run (elles sont différentes et utlisé pour chaque run)
            CloneData(ref cloned);
            cloned.m_bIsCloned = true;
            if (!cloned.IsFree)
                cloned.Return();
            return cloned;
        }

        public virtual object Clone()
        {
            return DeepCopy();
        }

        #endregion

    }
}
