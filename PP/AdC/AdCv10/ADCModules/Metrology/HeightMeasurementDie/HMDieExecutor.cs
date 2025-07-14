using System;
using System.Collections.Generic;
using System.Linq;

using AdcBasicObjects;

using AdcTools;
using AdcTools.Collection;

using BasicModules.Sizing;

using FormatAHM;

using FormatRHM;

using Matrox.MatroxImagingLibrary;

namespace HeightMeasurementDieModule
{
    public class HMDieExecutor : ITakeable, IDisposable, ICloneable
    {
        private bool _bIsCloned = false;
        private int _algoStatRun = MIL.M_MEAN;

        #region SharedData
        // data à reférence unique pour l'ensemble de la mesure, non copier lors du clonage
        private MIL_ID m_MilSys = MIL.M_NULL;
        private MIL_ID m_oMaskMeasure = MIL.M_NULL;
        private HeightMapRecipe m_hmRecipe = null;

        private MIL_ID m_oRunBlobResults = MIL.M_NULL;
        #endregion SharedData

        #region RunData
        // data utile au run, elles sont copié par clonage si multithreading
        private MIL_ID m_oRunOneBlobMask = MIL.M_NULL;
        private MIL_ID m_oRunStatsContext = MIL.M_NULL;
        private MIL_ID m_oRunStats = MIL.M_NULL;
        private MIL_ID m_oRunMaskBackground = MIL.M_NULL;
        private double[] m_ardRunBlobLabels = null;

        private HeightMeasureRun[] m_arRunBlobMeasures = null;

        private MIL_ID m_oRunMaskBackground_CHILD = MIL.M_NULL;
        private MIL_ID m_oRunMaskMeasures_CHILD = MIL.M_NULL;
        private MIL_ID m_oRunNoNanMask_CHILD = MIL.M_NULL;

        private MIL_ID m_oRunNoNanMask_Background = MIL.M_NULL;
        private MIL_ID m_oRunNoNanMask_Measure = MIL.M_NULL;
        private MIL_ID m_oRunNoNanMask = MIL.M_NULL;
        #endregion RunData


        private int _nDieImgSizeX_px = 0;
        private int _nDieImgSizeY_px = 0;
        private int _nWindowMarginX_px = 0;
        private int _nWindowMarginY_px = 0;

        internal class CoplaRunCoefficient
        {
            //
            // pour le calcul de la coplanarity
            //
            //Soit z=a.x+b.y+c l'équation du plan .
            //
            //la solution est donné par la résolution du système : 
            //(Sxx).a+(Sxy).b+(Sx).c = (Sxz)
            //(Sxy).a+(Syy).b+(Sy).c = (Syz)
            //(Sx).a+(Sy).b+n.c = (Sz)
            // Equivalence :
            //  a = (Sxz - (Sx * Sz / n) - ((Sxy - Sy * Sx / n) * (Syz - Sy * Sz / n)) / (Syy - Sy * Sy / n))
            //    / (Sxx - (Sx * Sx / n) - ((Sxy - Sy * Sx / n) * (Sxy - Sx * Sy / n)) / (Syy - Sy * Sy / n));
            //  b = (Syz - Sy * Sz / n) / (Syy - Sy * Sy / n) - (Sxy - Sx * Sy / n) / (Syy - Sy * Sy / n) * a;
            //  c = (Sz - Sx * a - Sy * b) / n;

            //Les coefficients étant calculés préalablement par :
            //Sxx = Somme des (xk)² pour k=1 à k=n
            //Sxy = Somme des (xk)(yk)
            //Sx = Somme des (xk)
            //Sxz = Somme de (xk)(zk)
            //Syy = Somme de (yk)²
            //Syz = Somme de (yk)(zk)
            //Szz = Somme de (zk)²
            //Sz = Somme de (zk) 
            //Sy = Somme de (yk) 

            public double Sxx = 0.0;
            public double Sxy = 0.0;
            public double Sxz = 0.0;
            public double Syy = 0.0;
            public double Syz = 0.0;
            public double Szz = 0.0;
            public double Sx = 0.0;
            public double Sy = 0.0;
            public double Sz = 0.0;
            public double n = 0.0; // nombre d'accumulation de mesure

            public CoplaRunCoefficient()
            {
                n = 0.0;
            }
            public void Add(double dX, double dY, double dZ)
            {
                Sxx += dX * dX;
                Sxy += dX * dY;
                Sxz += dX * dZ;
                Syy += dY * dY;
                Syz += dY * dZ;
                Szz += dZ * dZ;
                Sx += dX;
                Sy += dY;
                Sz += dZ;
                n++;
            }

            public bool GetPlaneCoefcient(out double a, out double b, out double c)
            {
                a = 0.0; b = 0.0; c = 0.0;
                if ((Sxx - Sx * Sx / n) == 0.0 || (Syy - Sy * Sy / n) == 0.0)
                    return false;

                a = (Sxz - (Sx * Sz / n) - ((Sxy - Sy * Sx / n) * (Syz - Sy * Sz / n)) / (Syy - Sy * Sy / n)) / (Sxx - (Sx * Sx / n) - ((Sxy - Sy * Sx / n) * (Sxy - Sx * Sy / n)) / (Syy - Sy * Sy / n));
                b = (Syz - Sy * Sz / n) / (Syy - Sy * Sy / n) - (Sxy - Sx * Sy / n) / (Syy - Sy * Sy / n) * a;
                c = (Sz - Sx * a - Sy * b) / n;
                return true;
            }
        }

        internal class HeightMeasureRun : ICloneable
        {
            public double X_px = 0.0; // pixel in image die ref
            public double Y_px = 0.0; // pixel in iùage die ref
            public double Z_um = 0.0; // micron height in absolute sensors ref

            public double DataBlob_Left = 0.0;   // pixel in image die ref
            public double DataBlob_Right = 0.0;  // pixel in image die ref
            public double DataBlob_Top = 0.0;    // pixel in image die ref
            public double DataBlob_Bottom = 0.0; // pixel in image die ref

            public HeightMeasureRun()
            {

            }

            public HeightMeasureRun(double p_dx, double p_dy, double p_dz, double p_Left, double p_Right, double p_Top, double p_Bottom)
            {
                X_px = p_dx;
                Y_px = p_dy;
                Z_um = p_dz;
                DataBlob_Left = p_Left;
                DataBlob_Right = p_Right;
                DataBlob_Top = p_Top;
                DataBlob_Bottom = p_Bottom;
            }

            protected object DeepCopy()
            {
                HeightMeasureRun cloned = MemberwiseClone() as HeightMeasureRun;
                return cloned;
            }

            public virtual object Clone()
            {
                return DeepCopy();
            }
        }


        public HMDieExecutor()
        {
            _bIsCloned = false;
            m_MilSys = MIL.M_NULL;
            _algoStatRun = MIL.M_STAT_MEAN;
        }

        public HMDieExecutor(MIL_ID p_MilSys)
        {
            _bIsCloned = false;
            m_MilSys = p_MilSys;
            _algoStatRun = MIL.M_STAT_MEAN;
        }

        public bool LoadFromFile(string p_sPathRecipeFile)
        {
            // Load HM Recipe
            m_hmRecipe = new HeightMapRecipe();
            String sErrorMsg;
            bool bSuccess = m_hmRecipe.ReadFromFile(p_sPathRecipeFile, out sErrorMsg);
            if (!bSuccess)
            {
                throw new ApplicationException(String.Format("HMDieExecutor:LoadFromFile FAIL = {0}", sErrorMsg));
            }
            else
            {
                // check buffer validity
                if (m_hmRecipe.MaskMeasureBuffer == null)
                {
                    throw new ApplicationException(String.Format("HMDieExecutor:LoadFromFile FAIL = Null Measure mask"));
                }
                if (m_hmRecipe.MaskMeasureBuffer == null)
                {
                    throw new ApplicationException(String.Format("HMDieExecutor:LoadFromFile FAIL = Null Background mask"));
                }

                // transfom buffer to MIL_ID
                bSuccess = InitSharedData();

                // Init Run Data 
                bSuccess &= InitRunData();
            }

            return bSuccess;
        }

        public bool InitRunData()
        {
            bool bSuccess = false;
            MIL_ID milBlobFeatureId = MIL.M_NULL;
            try
            {
                switch (m_hmRecipe.AlgoMethod)
                {
                    case 0:
                        _algoStatRun = MIL.M_STAT_MEAN; break;
                    case 1:
                        _algoStatRun = MIL.M_STAT_MAX; break;
                    default:
                        _algoStatRun = MIL.M_STAT_MEAN; break;
                }


                // features - center of gravity in binary ONLY
                MIL.MblobAlloc(m_MilSys, MIL.M_DEFAULT, MIL.M_DEFAULT, ref milBlobFeatureId);
                MIL.MblobControl(milBlobFeatureId, MIL.M_CENTER_OF_GRAVITY + MIL.M_BINARY, MIL.M_ENABLE);
                MIL.MblobControl(milBlobFeatureId, MIL.M_BOX, MIL.M_ENABLE);


                // init reference measure mask blob analysis
                MIL.MblobCalculate(m_oMaskMeasure, MIL.M_NULL, milBlobFeatureId, m_oRunBlobResults);
                MIL_INT iNbBlobs = 0;
                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref iNbBlobs);

                // init labels
                m_ardRunBlobLabels = new double[iNbBlobs];
                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_LABEL_VALUE, m_ardRunBlobLabels);

                // init measures localisation bounding box and centers
                double[] arcenters_X = new double[iNbBlobs];
                double[] arcenters_Y = new double[iNbBlobs];
                double[] arBlob_Left = new double[iNbBlobs];
                double[] arBlob_Right = new double[iNbBlobs];
                double[] arBlob_Top = new double[iNbBlobs];
                double[] arBlob_Bottom = new double[iNbBlobs];

                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_CENTER_OF_GRAVITY_X + MIL.M_BINARY + MIL.M_TYPE_DOUBLE, arcenters_X);
                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_CENTER_OF_GRAVITY_Y + MIL.M_BINARY + MIL.M_TYPE_DOUBLE, arcenters_Y);
                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_BOX_X_MIN + MIL.M_TYPE_DOUBLE, arBlob_Left);
                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_BOX_X_MAX + MIL.M_TYPE_DOUBLE, arBlob_Right);
                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_BOX_Y_MIN + MIL.M_TYPE_DOUBLE, arBlob_Top);
                MIL.MblobGetResult(m_oRunBlobResults, MIL.M_DEFAULT, MIL.M_BOX_Y_MAX + MIL.M_TYPE_DOUBLE, arBlob_Bottom);

                // create our Hmeasuresrun data
                m_arRunBlobMeasures = new HeightMeasureRun[iNbBlobs];
                for (int k = 0; k < iNbBlobs; k++)
                {
                    m_arRunBlobMeasures[k] = new HeightMeasureRun(arcenters_X[k], arcenters_Y[k], 0.0, arBlob_Left[k], arBlob_Right[k], arBlob_Top[k], arBlob_Bottom[k]);
                }

                // init temporary buffer need for conversion
                MIL_ID milTmpInitBuffer = MIL.M_NULL;
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref milTmpInitBuffer);

                // Init background mask & its child (transfom buffer to MIL_ID)
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunMaskBackground);
                MIL.MbufPut(milTmpInitBuffer, m_hmRecipe.MaskBackgroundBuffer);
                MIL.MbufCopy(milTmpInitBuffer, m_oRunMaskBackground);
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunNoNanMask_Background);
                MIL.MbufChild2d(m_oRunNoNanMask_Background, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, ref m_oRunMaskBackground_CHILD);

                // init run blob mask 
                MIL.MbufClear(milTmpInitBuffer, 0.0);

                // blob fiil
                var oldcolor = MIL.MgraInquire(MIL.M_DEFAULT, MIL.M_COLOR);
                MIL.MgraColor(MIL.M_DEFAULT, 1);
                MIL.MblobDraw(MIL.M_DEFAULT, m_oRunBlobResults, milTmpInitBuffer, MIL.M_DRAW_BLOBS, MIL.M_ALL_BLOBS, MIL.M_DEFAULT);
                MIL.MgraColor(MIL.M_DEFAULT, oldcolor);

                // convert blob mask 8b to 32b float
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunOneBlobMask);
                MIL.MbufCopy(milTmpInitBuffer, m_oRunOneBlobMask);
                // let binarize floats !=0 into 1.0 such as MimArith could work without any saturation
                MIL.MimBinarize(m_oRunOneBlobMask, m_oRunOneBlobMask, MIL.M_FIXED + MIL.M_NOT_EQUAL, 0.0, MIL.M_NULL);


                // delete temporary buffer
                MIL.MbufFree(milTmpInitBuffer);
                milTmpInitBuffer = MIL.M_NULL;

                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunNoNanMask_Measure);
                MIL.MbufChild2d(m_oRunNoNanMask_Measure, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, ref m_oRunMaskMeasures_CHILD);

                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunNoNanMask);
                MIL.MbufChild2d(m_oRunNoNanMask, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, ref m_oRunNoNanMask_CHILD);

                // init run STATS object id 
                MIL.MimAlloc(m_MilSys, MIL.M_STATISTICS_CONTEXT, MIL.M_DEFAULT, ref m_oRunStatsContext);
                MIL.MimAllocResult(m_MilSys, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT, ref m_oRunStats);

                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(String.Format("HMDieExecutor:InitRunData FAIL = {0}", ex.Message));
            }
            finally
            {
                if (milBlobFeatureId != MIL.M_NULL)
                {
                    MIL.MblobFree(milBlobFeatureId);
                    milBlobFeatureId = MIL.M_NULL;
                }
            }
            return bSuccess;
        }

        [Obsolete]
        public bool CloneRunData()
        {
            bool bSuccess = false;
            try
            {
                // init temporary buffer need for conversion
                MIL_ID milTmpInitBuffer = MIL.M_NULL;
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref milTmpInitBuffer);

                // Init background mask & its child (transfom buffer to MIL_ID)
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunMaskBackground);
                MIL.MbufPut(milTmpInitBuffer, m_hmRecipe.MaskBackgroundBuffer);
                MIL.MbufCopy(milTmpInitBuffer, m_oRunMaskBackground);
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunNoNanMask_Background);
                MIL.MbufChild2d(m_oRunNoNanMask_Background, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, ref m_oRunMaskBackground_CHILD);

                // init run blob mask 
                MIL.MbufClear(milTmpInitBuffer, 0.0);

                // blob fiil
                var oldcolor = MIL.MgraInquire(MIL.M_DEFAULT, MIL.M_COLOR);
                MIL.MgraColor(MIL.M_DEFAULT, 1);
                MIL.MblobDraw(MIL.M_DEFAULT, m_oRunBlobResults, milTmpInitBuffer, MIL.M_DRAW_BLOBS, MIL.M_ALL_BLOBS, MIL.M_DEFAULT);
                MIL.MgraColor(MIL.M_DEFAULT, oldcolor);

                // convert blob mask 8b to 32b float
                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunOneBlobMask);
                MIL.MbufCopy(milTmpInitBuffer, m_oRunOneBlobMask);
                // let binarize floats !=0 into 1.0 such as MimArith could work without any saturation
                MIL.MimBinarize(m_oRunOneBlobMask, m_oRunOneBlobMask, MIL.M_FIXED + MIL.M_NOT_EQUAL, 0.0, MIL.M_NULL);

                // delete temporary buffer
                MIL.MbufFree(milTmpInitBuffer);
                milTmpInitBuffer = MIL.M_NULL;

                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunNoNanMask_Measure);
                MIL.MbufChild2d(m_oRunNoNanMask_Measure, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, ref m_oRunMaskMeasures_CHILD);

                MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oRunNoNanMask);
                MIL.MbufChild2d(m_oRunNoNanMask, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, ref m_oRunNoNanMask_CHILD);

                // init run STATS object id 
                MIL.MimAllocResult(m_MilSys, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT, ref m_oRunStats);

                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(String.Format("HMDieExecutor:InitRunData FAIL = {0}", ex.Message));
            }
            finally
            {

            }
            return bSuccess;
        }

        public bool InitSharedData()
        {
            //transfom buffer to MIL_ID
            if (m_oMaskMeasure != MIL.M_NULL)
            {
                MIL.MbufFree(m_oMaskMeasure);
                m_oMaskMeasure = MIL.M_NULL;
            }
            // ici on peut rester en 8bit unsigned tant que le mask measures n'est utiliser que pour le blobcalcultate du initrundata
            MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_oMaskMeasure);
            MIL.MbufPut(m_oMaskMeasure, m_hmRecipe.MaskMeasureBuffer);

            MIL.MblobAllocResult(m_MilSys, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_oRunBlobResults);

            return true;
        }

        private void ComputeNanMask(MIL_ID p_InputDieImg)
        {
            MIL.MimBinarize(p_InputDieImg, m_oRunNoNanMask, MIL.M_IN_RANGE, float.MinValue, float.MaxValue);
            MIL.MimArith(m_oRunOneBlobMask, m_oRunNoNanMask, m_oRunNoNanMask_Measure, MIL.M_MULT);
            MIL.MimArith(m_oRunMaskBackground, m_oRunNoNanMask, m_oRunNoNanMask_Background, MIL.M_MULT);
        }

        private void InitInputChilds(MIL_ID p_InputDieImg, ref MIL_ID p_ImgChild, ref MIL_ID p_ImgChild_Blob)
        {
            MIL.MbufChild2d(p_InputDieImg, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, ref p_ImgChild);
            MIL.MbufChild2d(p_InputDieImg, 0, 0, 1, 1, ref p_ImgChild_Blob);
        }


        private double ComputeWholeBackground(MIL_ID p_InputDieImg, bool p_bComputeSubstrateCoplanarity, ref List<HeightMeasure> p_substrateHeights, ref CoplaRunCoefficient p_SubCopla)
        {
            MIL.MbufChildMove(m_oRunMaskBackground_CHILD, 0, 0, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, MIL.M_DEFAULT);

            // calculate backgroud height value
            double dbackgroundHeight_um = 0.0;

            // MIL 10 pp3
            //MIL.MimStat(p_InputDieImg, m_oRunStats, MIL.M_MEAN, MIL.M_MASK, m_oRunNoNanMask_Background, MIL.M_NULL);
            //MIL.MimGetResult(m_oRunStats, MIL.M_MEAN + MIL.M_TYPE_DOUBLE, ref dbackgroundHeight_um);

#warning ** USP ** MIL10 to MILX need to deal with a remplacemetn of M_MASK feature

            MIL.MimControl(m_oRunStatsContext, MIL.M_STAT_MEAN, MIL.M_ENABLE);
            MIL.MimStatCalculate(m_oRunStatsContext, p_InputDieImg, m_oRunStats, MIL.M_DEFAULT);
            MIL.MimGetResult(m_oRunStats, MIL.M_STAT_MEAN, ref dbackgroundHeight_um);


            //double StdDev_background = 0.0;
            //MIL.MimStat(p_InputDieImg, m_oRunStats, MIL.M_STANDARD_DEVIATION, MIL.M_MASK, m_oRunMaskBackground_CHILD, MIL.M_NULL);
            //MIL.MimGetResult(m_oRunStats, MIL.M_STANDARD_DEVIATION + MIL.M_TYPE_DOUBLE, ref StdDev_background);

            if (p_bComputeSubstrateCoplanarity)
            {
                // CALCULER LA COPLA DU SUBSTRAT -- ici on on calcul pixel par pixel -- note : on est plus semnsisble au bruit qu'avace la method par window
                p_SubCopla = new CoplaRunCoefficient();

                int nbuffLegnth = m_hmRecipe.BaseImageSizeX * m_hmRecipe.BaseImageSizeY;
                float[] mskBufnoNaN = new float[nbuffLegnth];
                float[] dieimghbuff = new float[nbuffLegnth];
                MIL.MbufGet(m_oRunNoNanMask_Background, mskBufnoNaN);
                MIL.MbufGet(p_InputDieImg, dieimghbuff);
                double dX = 0.0; double dY = 0.0; double dZ = 0.0;
                p_substrateHeights = new List<HeightMeasure>(nbuffLegnth);
                for (int k = 0; k < nbuffLegnth; k++)
                {
                    if (IsCancellationRequested)
                        break;

                    if (mskBufnoNaN[k] != 0.0f)
                    {
                        dX = (double)(k % m_hmRecipe.BaseImageSizeX);
                        dY = (double)(k / m_hmRecipe.BaseImageSizeX);
                        dZ = dieimghbuff[k];
                        p_SubCopla.Add(dX, dY, dZ);
                        p_substrateHeights.Add(new HeightMeasure(k, (float)dZ, (int)dX, (int)dY));
                    }
                }
            }

            return dbackgroundHeight_um;
        }

        private double ComputeWindowBackground(int p_iBlobIdx, MIL_ID p_ImgChild, double p_dCenterX_px, double p_dCenterY_px, bool p_bComputeSubstrateCoplanarity, List<HeightMeasure> p_substrateHeights, CoplaRunCoefficient p_SubCopla)
        {
            double dbackgroundHeight_um = 0.0;

            // on calcule et applique la region de window  autrou du blob
            int originX = (int)p_dCenterX_px - _nWindowMarginX_px;
            int originY = (int)p_dCenterY_px - _nWindowMarginY_px;
            int childSizeX = 2 * _nWindowMarginX_px;
            int childSizeY = 2 * _nWindowMarginY_px;
            if (originX < 0)
            {
                childSizeX = childSizeX + originX;
                originX = 0;
            }
            if (originY < 0)
            {
                childSizeY = childSizeY + originY;
                originY = 0;
            }
            if (originX + childSizeX > _nDieImgSizeX_px)
            {
                childSizeX = _nDieImgSizeX_px - originX;
            }
            if (originY + childSizeY > _nDieImgSizeY_px)
            {
                childSizeY = _nDieImgSizeY_px - originY;
            }

            MIL.MbufChildMove(m_oRunMaskBackground_CHILD, originX, originY, childSizeX, childSizeY, MIL.M_DEFAULT);
            MIL.MbufChildMove(p_ImgChild, originX, originY, childSizeX, childSizeY, MIL.M_DEFAULT);

            // calculate backgroud height value
            // MIL 10
            //MIL.MimStat(p_ImgChild, m_oRunStats, MIL.M_MEAN, MIL.M_MASK, m_oRunMaskBackground_CHILD, MIL.M_NULL);
            //MIL.MimGetResult(m_oRunStats, MIL.M_MEAN + MIL.M_TYPE_DOUBLE, ref dbackgroundHeight_um);

#warning ** USP ** MIL10 to MILX need to deal with a remplacemetn of M_MASK feature
            MIL.MimControl(m_oRunStatsContext, MIL.M_STAT_MEAN, MIL.M_ENABLE);
            MIL.MimStatCalculate(m_oRunStatsContext, p_ImgChild, m_oRunStats, MIL.M_DEFAULT);
            MIL.MimGetResult(m_oRunStats, MIL.M_STAT_MEAN, ref dbackgroundHeight_um);

            //double StdDev_background = 0.0;
            //MIL.MimStat(milImageChildId, m_oRunStats, MIL.M_STANDARD_DEVIATION, MIL.M_MASK, m_oRunMaskBackground_CHILD, MIL.M_NULL);
            //MIL.MimGetResult(m_oRunStats, MIL.M_STANDARD_DEVIATION + MIL.M_TYPE_DOUBLE, ref StdDev_background);

            if (p_bComputeSubstrateCoplanarity)
            {
                // CALCULER LA COPLA DU SUBSTRAT -- ici on on calcul mesure par measure --
                p_SubCopla.Add(p_dCenterX_px, p_dCenterY_px, dbackgroundHeight_um);
                p_substrateHeights.Add(new HeightMeasure(p_iBlobIdx, (float)dbackgroundHeight_um, (int)p_dCenterX_px, (int)p_dCenterY_px));
            }
            return dbackgroundHeight_um;
        }

        private bool HeightBlobMeasure(HeightMeasureRun p_BlobMeasure, MIL_ID p_ImgChild_Blob)
        {
            bool bSuccess = true;
            try
            {
                MIL.MbufChildMove(m_oRunMaskMeasures_CHILD, (int)p_BlobMeasure.DataBlob_Left, (int)p_BlobMeasure.DataBlob_Top, (int)(p_BlobMeasure.DataBlob_Right - p_BlobMeasure.DataBlob_Left + 1.0), (int)(p_BlobMeasure.DataBlob_Bottom - p_BlobMeasure.DataBlob_Top + 1.0), MIL.M_DEFAULT);
                MIL.MbufChildMove(p_ImgChild_Blob, (int)p_BlobMeasure.DataBlob_Left, (int)p_BlobMeasure.DataBlob_Top, (int)(p_BlobMeasure.DataBlob_Right - p_BlobMeasure.DataBlob_Left + 1.0), (int)(p_BlobMeasure.DataBlob_Bottom - p_BlobMeasure.DataBlob_Top + 1.0), MIL.M_DEFAULT);

                // calculate bump height value
                // MIL10
                //MIL.MimStat(p_ImgChild_Blob, m_oRunStats, m_AlgoStatRun, MIL.M_MASK, m_oRunMaskMeasures_CHILD, MIL.M_NULL);
                //MIL.MimGetResult(m_oRunStats, m_AlgoStatRun + MIL.M_TYPE_DOUBLE, ref p_BlobMeasure.Z_um);

#warning ** USP ** MIL10 to MILX need to deal with a remplacemetn of M_MASK feature
                MIL.MimControl(m_oRunStatsContext, MIL.M_STAT_MEAN, MIL.M_ENABLE);
                if (_algoStatRun == MIL.M_STAT_MEAN)
                {
                    MIL.MimStatCalculate(MIL.M_STAT_CONTEXT_MEAN, p_ImgChild_Blob, m_oRunStats, MIL.M_DEFAULT);
                    MIL.MimGetResult(m_oRunStats, MIL.M_STAT_MEAN, ref p_BlobMeasure.Z_um);
                }
                else if (_algoStatRun == MIL.M_STAT_MAX)
                {
                    MIL.MimStatCalculate(MIL.M_STAT_CONTEXT_MAX, p_ImgChild_Blob, m_oRunStats, MIL.M_DEFAULT);
                    MIL.MimGetResult(m_oRunStats, MIL.M_STAT_MAX, ref p_BlobMeasure.Z_um);
                }

                // ici on calcule la largeur du blob de mesure rel pour diametre et surtout la position à voir si on recalcule plus tard la hauteur
                /*    MIL.MbufChildMove(milImageChildId_BLOB, (int)DataBlob_Left-nDistanceNeightbord_px, (int)DataBlob_Top-nDistanceNeightbord_px, (int)(DataBlob_Right - DataBlob_Left + 2.0 * (double)nDistanceNeightbord_px +1.0), (int)(DataBlob_Bottom - DataBlob_Top + 2.0 * (double)nDistanceNeightbord_px + 1.0), MIL.M_DEFAULT);
                    MIL.MbufChildMove(m_oRunMaskBackground_CHILD, (int)DataBlob_Left - nDistanceNeightbord_px, (int)DataBlob_Top - nDistanceNeightbord_px, (int)(DataBlob_Right - DataBlob_Left + 2.0 * (double)nDistanceNeightbord_px + 1.0), (int)(DataBlob_Bottom - DataBlob_Top + 2.0 * (double)nDistanceNeightbord_px + 1.0), MIL.M_DEFAULT);             
                    MIL.MbufChildMove(maskBlobRecentered_CHILD, (int)DataBlob_Left - nDistanceNeightbord_px, (int)DataBlob_Top - nDistanceNeightbord_px, (int)(DataBlob_Right - DataBlob_Left + 2.0 * (double)nDistanceNeightbord_px + 1.0), (int)(DataBlob_Bottom - DataBlob_Top + 2.0 * (double)nDistanceNeightbord_px + 1.0), MIL.M_DEFAULT);


                    MIL.MbufChildMove(m_oRunNoNanMask_CHILD, (int)DataBlob_Left - nDistanceNeightbord_px, (int)DataBlob_Top - nDistanceNeightbord_px, (int)(DataBlob_Right - DataBlob_Left + 2.0 * (double)nDistanceNeightbord_px + 1.0), (int)(DataBlob_Bottom - DataBlob_Top + 2.0 * (double)nDistanceNeightbord_px + 1.0), MIL.M_DEFAULT);


                    backgroundroof_height -= dbackgroundHeight_um;
                    double stddev = backgroundroof_height;
                    backgroundroof_height += dbackgroundHeight_um;

                    double dThresh3D = dbackgroundHeight_um + 3.0 * stddev;
                    //dThresh3D = Math.Max(backgroundroof_height, (dMeasureHeight_um - dbackgroundHeight_um)/ 3.0 + dbackgroundHeight_um);

                    double dMean = 0.0; double sigmat = 0.0;
                    MIL.MbufChildMove(m_oRunMaskMeasures_CHILD, (int)DataBlob_Left - nDistanceNeightbord_px, (int)DataBlob_Top - nDistanceNeightbord_px, (int)(DataBlob_Right - DataBlob_Left + 2.0 * (double)nDistanceNeightbord_px + 1.0), (int)(DataBlob_Bottom - DataBlob_Top + 2.0 * (double)nDistanceNeightbord_px + 1.0), MIL.M_DEFAULT);
                    MIL.MimStat(milImageChildId_BLOB, m_oRunStats, MIL.M_MEAN+ MIL.M_STANDARD_DEVIATION, MIL.M_MASK, m_oRunMaskMeasures_CHILD, MIL.M_NULL);
                    MIL.MimGetResult(m_oRunStats, MIL.M_MEAN + MIL.M_TYPE_DOUBLE, ref dMean);
                    MIL.MimGetResult(m_oRunStats, MIL.M_STANDARD_DEVIATION + MIL.M_TYPE_DOUBLE, ref sigmat);

                    dThresh3D = dMean - sigmat;

                    MIL.MimBinarize(milImageChildId_BLOB, maskBlobRecentered_CHILD, MIL.M_GREATER_OR_EQUAL, dThresh3D, MIL.M_NULL);
                    MIL.MimArith(maskBlobRecentered_CHILD, m_oRunMaskBackground_CHILD, maskBlobRecentered_CHILD, MIL.M_SUB + MIL.M_SATURATION);
                    MIL.MimArith(maskBlobRecentered_CHILD, m_oRunNoNanMask, maskBlobRecentered_CHILD, MIL.M_MULT);*/
            }
            catch (System.Exception ex)
            {
                string msg = ex.Message;
                // if (LogError != null)
                //     LogError(String.Format("HeightBlobMeasure Warning : unable to compute height - return 0"));

                p_BlobMeasure.Z_um = 0.0;
                bSuccess = false;
            }

            return bSuccess;
        }

        private void CreateBlob(int p_iBlobIdx, HeightMeasureRun p_BlobMeasure, double p_dBackgroundHeight_um, Cluster3DDieHM p_Cluster, List<Blob> p_BlobList, DieHMResults p_Results, bool p_bComputeCoplanarity, CoplaRunCoefficient p_Copla)
        {
            //.....................
            //  Blob measure
            // 
            Blob blob = new Blob(p_iBlobIdx, p_Cluster);

            // Calcul du rectangle
            blob.pixelRect.X = p_Cluster.imageRect.X + (int)p_BlobMeasure.DataBlob_Left;
            blob.pixelRect.Y = p_Cluster.imageRect.Y + (int)p_BlobMeasure.DataBlob_Top;
            blob.pixelRect.Width = (int)(p_BlobMeasure.DataBlob_Right - p_BlobMeasure.DataBlob_Left) + 1;
            blob.pixelRect.Height = (int)(p_BlobMeasure.DataBlob_Bottom - p_BlobMeasure.DataBlob_Top) + 1;
            blob.micronQuad = p_Cluster.Layer.Matrix.pixelToMicron(blob.pixelRect);

            // Blob Characteristics
            blob.characteristics.Add(BlobCharacteristics.BOX_X_MIN, p_Cluster.imageRect.X + p_BlobMeasure.DataBlob_Left);
            blob.characteristics.Add(BlobCharacteristics.BOX_Y_MIN, p_Cluster.imageRect.Y + p_BlobMeasure.DataBlob_Top);
            blob.characteristics.Add(BlobCharacteristics.BOX_X_MAX, p_Cluster.imageRect.X + p_BlobMeasure.DataBlob_Right);
            blob.characteristics.Add(BlobCharacteristics.BOX_Y_MAX, p_Cluster.imageRect.Y + p_BlobMeasure.DataBlob_Bottom);
            blob.characteristics.Add(BlobCharacteristics.CENTER_OF_GRAVITY_X, p_Cluster.imageRect.X + p_BlobMeasure.X_px);
            blob.characteristics.Add(BlobCharacteristics.CENTER_OF_GRAVITY_Y, p_Cluster.imageRect.Y + p_BlobMeasure.Y_px);
            blob.characteristics.Add(BlobCharacteristics.MicronArea, blob.micronQuad.SurroundingRectangle.Area());
            blob.characteristics.Add(ClusterCharacteristics.RealWidth, (double)blob.micronQuad.SurroundingRectangle.Width);
            blob.characteristics.Add(ClusterCharacteristics.RealHeight, (double)blob.micronQuad.SurroundingRectangle.Height);

            double dRelativeHeightMeasure_um = p_BlobMeasure.Z_um - p_dBackgroundHeight_um;
            blob.characteristics.Add(Blob3DCharacteristics.HeightMicron, dRelativeHeightMeasure_um);
            blob.characteristics.Add(Blob3DCharacteristics.SubstrateHeightMicron, p_dBackgroundHeight_um);
            p_BlobList.Add(blob);

            p_Results.AddHeightMeasure(p_iBlobIdx, (float)dRelativeHeightMeasure_um, (int)p_BlobMeasure.X_px, (int)p_BlobMeasure.Y_px);

            if (p_bComputeCoplanarity)
            {
                p_Copla.Add(p_BlobMeasure.X_px, p_BlobMeasure.Y_px, dRelativeHeightMeasure_um);
            }
        }

        private double ComputeCoplanarity(DieHMResults p_Results, CoplaRunCoefficient p_Copla)
        {
            double coplanarity = 0.0;
            double a = 0.0, b = 0.0, c = 0.0;
            double n = (double)p_Results.NbMeasures;
            if (n <= 0)
            {
                coplanarity = 0.0;
                //if (LogError != null)
                //    LogError(String.Format("HMDieExecutor::ComputeCoplanarity warning: No measures"));
            }
            else if (n <= 3)
            {
                float fMax = p_Results.ComputeMax();
                float fMin = p_Results.ComputeMin();
                coplanarity = (double)(fMax - fMin);
            }
            else if (p_Copla.GetPlaneCoefcient(out a, out b, out c))
            {
                // mesure de la coplanarity egale au maximum des ecarts de hauteurs par rapport au plan
                //coplanarity = p_Results.ComputeDevMaxFromPlane(a, b, c); //note de rti : method used prior to 15/04/2022 request IBM albany (Dario)
                coplanarity = p_Results.ComputeDistanceFromMinMaxPlane(a, b, c); //note de rti : method used after 15/04/2022 request IBM albany (Dario)
                                                                                 // fix calculatin on 15/05/22
            }

            p_Results.Coplanarity = (float)coplanarity;
            return coplanarity;
        }

        private double ComputeSubstrateCoplanarity(List<HeightMeasure> p_substrateHeights, CoplaRunCoefficient p_SubCopla, DieHMResults p_Results)
        {
            double subcoplanarity = 0.0;
            double a = 0.0, b = 0.0, c = 0.0;
            double n = (double)p_substrateHeights.Count;
            if (n <= 0)
            {
                subcoplanarity = 0.0;
            }
            else if (n <= 3)
            {
                float fMax = p_substrateHeights.Max(mes => mes.Height_um);
                float fMin = p_substrateHeights.Min(mes => mes.Height_um); ;
                subcoplanarity = (double)(fMax - fMin);
            }
            else if (p_SubCopla.GetPlaneCoefcient(out a, out b, out c))
            {
                // mesure de la coplanarity egale au maximum des ecarts de hauteurs par rapport au plan
                //Soit z=a.x+b.y+c l'équation du plan .
                double dMax = -double.MaxValue;
                foreach (HeightMeasure hm in p_substrateHeights)
                {
                    if (IsCancellationRequested)
                        break;

                    double zi = a * (double)hm.PositionX_px + b * (double)hm.PositionY_px + c;
                    double deviation = Math.Abs(zi - hm.Height_um);
                    if (deviation > dMax)
                        dMax = deviation;
                }
                subcoplanarity = dMax;
            }

            if (IsCancellationRequested)
            {
                subcoplanarity = 0.0;
            }
            else
            {
                p_Results.SubstrateCoplanarity = (float)subcoplanarity;
                p_substrateHeights.Clear();
            }
            return subcoplanarity;
        }

        private void DebugAddRangebloblist(Cluster3DDieHM p_Cluster, List<Blob> p_blobList)
        {
            p_Cluster.blobList.AddRange(p_blobList);
        }

        public bool Measure(ref Cluster3DDieHM p_Cluster, bool p_bComputeHeightMap, bool p_bComputeCoplanarity, bool p_bComputeSubstrateCoplanarity)
        {
            MIL_ID p_DieImageId = p_Cluster.OriginalProcessingImage.GetMilImage();
            // /!\ p_DieImageId est un buffer de type flottant ie (32 + MIL.M_FLOAT) /!\ 
            DieHMResults p_Results = p_Cluster._dieHMresult;

            // calcul des hauteurs des mesures stockées en recettes
            bool bSuccess = false;
            MIL_ID milImageChildId = MIL.M_NULL;
            MIL_ID milImageChildId_BLOB = MIL.M_NULL;
            try
            {
                // on verifie si l'image passer en paramètre correspond à notre recette 
                if (p_DieImageId == MIL.M_NULL)
                {
                    throw new ApplicationException(String.Format("HMDieExecutor::Measure Error : Input Image is Null"));
                }

                _nDieImgSizeX_px = (int)MIL.MbufInquire(p_DieImageId, MIL.M_SIZE_X);
                _nDieImgSizeY_px = (int)MIL.MbufInquire(p_DieImageId, MIL.M_SIZE_Y);
                if (_nDieImgSizeX_px != m_hmRecipe.BaseImageSizeX || _nDieImgSizeY_px != m_hmRecipe.BaseImageSizeY)
                {
                    throw new ApplicationException(String.Format("HMDieExecutor::Measure Error : Input Image Size is different from recipe"));
                }

                bool bUseWholeBackground_Method = (m_hmRecipe.HeightCalcutationMethod == 0);
                _nWindowMarginX_px = m_hmRecipe.WindowMarginX;
                _nWindowMarginY_px = m_hmRecipe.WindowMarginY;

                if (IsCancellationRequested)
                    return false;

                ComputeNanMask(p_DieImageId);

                /*       MIL_ID maskBlobRecentered = MIL.M_NULL;
                       MIL_ID maskBlobRecentered_CHILD = MIL.M_NULL;
                       MIL.MbufAlloc2d(m_MilSys, m_hmRecipe.BaseImageSizeX, m_hmRecipe.BaseImageSizeY, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref maskBlobRecentered);
                       MIL.MbufClear(maskBlobRecentered, 0.0);
                       MIL.MbufChild2d(maskBlobRecentered, 0, 0, nDieImgSizeX_px, nDieImgSizeY_px, ref maskBlobRecentered_CHILD);
                       double backgroundroof_height = 0.0;
                       int nDistanceNeightbord_px = 2;*/

                // init image child window - whole image & Blob
                InitInputChilds(p_DieImageId, ref milImageChildId, ref milImageChildId_BLOB);

                double dbackgroundHeight_um = 0.0; // this is our substrate height
                CoplaRunCoefficient MeasureCopla = null;
                CoplaRunCoefficient SubtrateCopla = null;
                List<HeightMeasure> substrateHeights = null;
                if (bUseWholeBackground_Method)
                {
                    dbackgroundHeight_um = ComputeWholeBackground(p_DieImageId, p_bComputeSubstrateCoplanarity, ref substrateHeights, ref SubtrateCopla);

                }
                else if (p_bComputeSubstrateCoplanarity)
                {
                    substrateHeights = new List<HeightMeasure>(m_ardRunBlobLabels.Length);
                    SubtrateCopla = new CoplaRunCoefficient();
                }

                if (p_bComputeCoplanarity)
                {
                    MeasureCopla = new CoplaRunCoefficient();
                }

                int nbMeasure = m_arRunBlobMeasures.Length;
                List<Blob> blobList = new List<Blob>(nbMeasure);
                uint uIter = 0;
                foreach (HeightMeasureRun hmBlobMeasure in m_arRunBlobMeasures)
                {
                    if (IsCancellationRequested)
                        break;

                    int iBlobIndex = (int)m_ardRunBlobLabels[uIter]; // ici ce sont les labels des blobs au lieu de index, il peux y avoirs des trous , ne pas s'en servir comme références d'index pour les array !
                    if (!HeightBlobMeasure(hmBlobMeasure, milImageChildId_BLOB))
                    {
                        // Warning la hauteur n'a pas pu être calculé
                        CreateBlob((int)iBlobIndex, hmBlobMeasure, 0.0, p_Cluster, blobList, p_Results, p_bComputeCoplanarity, MeasureCopla);
                    }
                    else
                    {
                        if (!bUseWholeBackground_Method)
                        {
                            dbackgroundHeight_um = ComputeWindowBackground((int)iBlobIndex, milImageChildId, hmBlobMeasure.X_px, hmBlobMeasure.Y_px, p_bComputeSubstrateCoplanarity, substrateHeights, SubtrateCopla);
                        }

                        CreateBlob((int)iBlobIndex, hmBlobMeasure, dbackgroundHeight_um, p_Cluster, blobList, p_Results, p_bComputeCoplanarity, MeasureCopla);
                    }
                    ++uIter;
                } // fin foreach

                if (IsCancellationRequested)
                    return false;

                // DebugAddRangebloblist(p_Cluster, blobList); // only for performance check use the commented line below in normal behavior
                p_Cluster.blobList.AddRange(blobList);

                if (p_bComputeCoplanarity)
                {
                    p_Cluster.characteristics[Cluster3DCharacteristics.Coplanarity] = ComputeCoplanarity(p_Results, MeasureCopla);
                    MeasureCopla = null;
                }

                if (p_bComputeSubstrateCoplanarity)
                {

                    p_Cluster.characteristics[Cluster3DCharacteristics.SubstrateCoplanarity] = ComputeSubstrateCoplanarity(substrateHeights, SubtrateCopla, p_Results);
                    substrateHeights = null;
                    SubtrateCopla = null;
                }

                if (p_bComputeHeightMap)
                {
                    p_Results.UpdateStats();
                    p_Cluster.characteristics[Cluster3DCharacteristics.HeightAverage] = (double)p_Results.MeanHeight_um;
                    p_Cluster.characteristics[Cluster3DCharacteristics.HeightStdDev] = (double)p_Results.StdDev;
                    p_Cluster.characteristics[Cluster3DCharacteristics.HeightMin] = (double)p_Results.MinHeight;
                    p_Cluster.characteristics[Cluster3DCharacteristics.HeightMax] = (double)p_Results.MaxHeight;

                    p_Cluster.characteristics[SizingCharacteristics.DefectMaxSize] = (double)p_Results.MaxHeight;
                    p_Cluster.characteristics[SizingCharacteristics.TotalDefectSize] = (double)p_Results.MeanHeight_um;
                    p_Cluster.characteristics[SizingCharacteristics.SizingType] = eSizingType.ByHeight3D;
                }

                bSuccess = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (milImageChildId_BLOB != MIL.M_NULL)
                {
                    MIL.MbufFree(milImageChildId_BLOB);
                    milImageChildId_BLOB = MIL.M_NULL;
                }

                if (milImageChildId != MIL.M_NULL)
                {
                    MIL.MbufFree(milImageChildId);
                    milImageChildId = MIL.M_NULL;
                }
            }
            return bSuccess;
        }


        #region IDisposable Members and Methods
        private bool _hasDisposed = false;
        //Finalizer
        ~HMDieExecutor()
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
            if (_hasDisposed == false)
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

                _hasDisposed = true;
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
            // release des data inhérante à chaque run 
            if (m_oRunStatsContext != MIL.M_NULL)
            {
                MIL.MimFree(m_oRunStatsContext);
                m_oRunStatsContext = MIL.M_NULL;
            }

            if (m_oRunStats != MIL.M_NULL)
            {
                MIL.MimFree(m_oRunStats);
                m_oRunStats = MIL.M_NULL;
            }

            if (m_oRunNoNanMask_CHILD != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunNoNanMask_CHILD);
                m_oRunNoNanMask_CHILD = MIL.M_NULL;
            }

            if (m_oRunNoNanMask != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunNoNanMask);
                m_oRunNoNanMask = MIL.M_NULL;
            }

            if (m_oRunMaskMeasures_CHILD != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunMaskMeasures_CHILD);
                m_oRunMaskMeasures_CHILD = MIL.M_NULL;
            }

            if (m_oRunNoNanMask_Measure != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunNoNanMask_Measure);
                m_oRunNoNanMask_Measure = MIL.M_NULL;
            }

            if (m_oRunOneBlobMask != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunOneBlobMask);
                m_oRunOneBlobMask = MIL.M_NULL;
            }

            if (m_oRunMaskBackground_CHILD != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunMaskBackground_CHILD);
                m_oRunMaskBackground_CHILD = MIL.M_NULL;
            }

            if (m_oRunNoNanMask_Background != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunNoNanMask_Background);
                m_oRunNoNanMask_Background = MIL.M_NULL;
            }

            if (m_oRunMaskBackground != MIL.M_NULL)
            {
                MIL.MbufFree(m_oRunMaskBackground);
                m_oRunMaskBackground = MIL.M_NULL;
            }
        }

        private void ReleaseSharedData()
        {
            // release des data inhérante à l'ensemble du run, ces données sont "partagées" par l'ensemble des threads, elles ne sont pas copier lors d'un clonage.
            if (m_oMaskMeasure != MIL.M_NULL)
            {
                MIL.MbufFree(m_oMaskMeasure);
                m_oMaskMeasure = MIL.M_NULL;
            }

            if (m_oRunBlobResults != MIL.M_NULL)
            {
                MIL.MblobFree(m_oRunBlobResults);
                m_oRunBlobResults = MIL.M_NULL;
            }

            ///m_hmRecipe.Dispose();        ??
        }

        private void ReleaseUnmanagedResources()
        {
            ReleaseRunData();

            if (!_bIsCloned)
            {
                ReleaseSharedData();
            }
        }

        #endregion

        #region ICloneable Members and methods
        [Obsolete]
        protected void CloneData(ref HMDieExecutor cloned)
        {
            cloned.m_ardRunBlobLabels = new double[m_ardRunBlobLabels.Length];
            Array.Copy(m_ardRunBlobLabels, cloned.m_ardRunBlobLabels, m_ardRunBlobLabels.Length);

            cloned.m_arRunBlobMeasures = m_arRunBlobMeasures.Select(item => (HeightMeasureRun)item.Clone()).ToArray();

            cloned.CloneRunData();
        }

        [Obsolete]
        protected object DeepCopy()
        {
            HMDieExecutor cloned = MemberwiseClone() as HMDieExecutor;
            // ici on clone les instances nécéssaire au run (elles sont différentes et utlisé pour chaque run)
            CloneData(ref cloned);
            cloned._bIsCloned = true;
            if (!cloned.IsFree)
                cloned.Return();
            return cloned;
        }

        [Obsolete]
        public virtual object Clone()
        {
            return DeepCopy();
        }

        #endregion
    }
}
