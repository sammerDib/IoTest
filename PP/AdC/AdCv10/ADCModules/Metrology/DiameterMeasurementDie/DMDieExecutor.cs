using System;
using System.Collections.Generic;
using System.Drawing;

using AdcBasicObjects;

using AdcTools;
using AdcTools.Collection;

using BasicModules.Sizing;

using FormatRDM;

using Matrox.MatroxImagingLibrary;

namespace DiameterMeasurementDieModule
{
    public class DMDieExecutor : ITakeable, IDisposable, ICloneable
    {
        public class ScoreParams
        {
            public double dMin = 0.0;
            public double dLow = 0.0;
            public double dHig = 0.0;
            public double dMax = MIL.M_MAX_POSSIBLE_VALUE;
            public double dScoreOffset = MIL.M_DEFAULT;

            public ScoreParams()
            {

            }
            public ScoreParams(double p_dMin, double p_dLow, double p_dHig, double p_dMax)
            {
                dMin = p_dMin; dLow = p_dLow; dHig = p_dHig; dMax = p_dMax;
            }
        }
        public class DMExecParams
        {
            public double dSearchRadMin = 0.0;
            public double dSearchRadMax = 3.0;
            public double dpolarity = MIL.M_ANY;
            public double dNbSubRegion = -1.0;// default disable (<0.0)
            public double dMaxAssocDist_px = -1.0; // default disable (<0.0)
            public double dEdgeThreshold = -1.0;  // default disable (<0.0) in grayscale level pctage
            public double dSubRegionChordAngle = 10.0; // en degres
            public ScoreParams RadiusScore = null;
            public ScoreParams StrengthScore = null;
            public DMExecParams()
            {

            }

            public DMExecParams(double p_dSearchRadMin, double p_dSearchRadMax)
            {
                dSearchRadMin = p_dSearchRadMin;
                dSearchRadMax = p_dSearchRadMax;
            }
        }

        private bool m_bIsCloned = false;

        // data à reférence unique pour l'ensemble de la mesure, non copier lors du clonage
        private MIL_ID m_MilSys = MIL.M_NULL;
        private DiameterMapRecipe m_dmRecipe = null;
        private DMExecParams m_ExecParams = null;

        #region RunData
        // data utile au run, elles sont copié par clonage si multithreading
        #endregion RunData

        private MIL_ID m_oRunCircleMarker = MIL.M_NULL;

        private int m_nDieImgSizeX_px = 0;
        private int m_nDieImgSizeY_px = 0;
        private int m_nWindowMarginX_px = 0;
        private int m_nWindowMarginY_px = 0;

        public DMDieExecutor()
        {
            m_bIsCloned = false;
            m_MilSys = MIL.M_NULL;
        }

        public DMDieExecutor(MIL_ID p_MilSys)
        {
            m_bIsCloned = false;
            m_MilSys = p_MilSys;
        }

        public bool LoadFromFile(string p_sPathRecipeFile, DMExecParams p_ExecParams)
        {
            // Load HM Recipe
            m_dmRecipe = new DiameterMapRecipe();
            String sErrorMsg;
            bool bSuccess = m_dmRecipe.ReadFromFile(p_sPathRecipeFile, out sErrorMsg);
            if (!bSuccess)
            {
                throw new ApplicationException(String.Format("DMDieExecutor:LoadFromFile FAIL = {0}", sErrorMsg));
            }
            else
            {
                m_ExecParams = p_ExecParams;
            }

            return bSuccess;
        }

        public bool InitMeasureMarker(float p_lfPixelSizeX_um, float p_lfPixelSizeY_um)
        {
            bool bSuccess = false;
            try
            {
                int nChildW = m_dmRecipe.ModelSizeX + 2 * m_dmRecipe.WindowMarginX;
                int nChildH = m_dmRecipe.ModelSizeY + 2 * m_dmRecipe.WindowMarginY;

                float ChildOffsetX = ((float)nChildW) / 2.0f;
                float ChildOffsetY = ((float)nChildH) / 2.0f;

                m_oRunCircleMarker = MIL.MmeasAllocMarker(m_MilSys, MIL.M_CIRCLE, MIL.M_DEFAULT, MIL.M_NULL);

                MIL.MmeasSetMarker(m_oRunCircleMarker, MIL.M_SUB_REGIONS_CHORD_ANGLE, m_ExecParams.dSubRegionChordAngle, MIL.M_NULL); // default 10.0f
                MIL.MmeasSetMarker(m_oRunCircleMarker, MIL.M_SEARCH_REGION_CLIPPING, MIL.M_ENABLE, MIL.M_NULL); // default Disabled //Sets whether to allow MIL to automatically clip the search region (box or ring) when it falls outside the image. In this case, the automatically computed search region is the best possible area that is both inscribed by the defined search region and the boundaries of the image. Note that this modified (clipped) region is internally created by MIL; the actual search region that you defined is not modified. 

                //MIL.MmeasSetMarker(MilCircleMarker, MIL.M_CIRCLE_ACCURACY, MIL.M_LOW, MIL.M_NULL); // Mil 10 only

                float pixelSizeAvg = (p_lfPixelSizeX_um + p_lfPixelSizeY_um) * 0.5f;

                MIL.MmeasSetMarker(m_oRunCircleMarker, MIL.M_RING_CENTER, ChildOffsetX, ChildOffsetX);
                MIL.MmeasSetMarker(m_oRunCircleMarker, MIL.M_RING_RADII, m_ExecParams.dSearchRadMin / pixelSizeAvg, m_ExecParams.dSearchRadMax / pixelSizeAvg);
                MIL.MmeasSetMarker(m_oRunCircleMarker, MIL.M_POLARITY, m_ExecParams.dpolarity, MIL.M_NULL); // = MIL.M_ANY ou MIL.M_POSITIVE; ou MIL.M_NEGATIVE;

                if (m_ExecParams.dNbSubRegion > 0.0)
                    MIL.MmeasSetMarker(m_oRunCircleMarker, MIL.M_SUB_REGIONS_NUMBER, m_ExecParams.dNbSubRegion, MIL.M_NULL); // default 8

                if (m_ExecParams.dMaxAssocDist_px >= 0.0)
                    MIL.MmeasSetMarker(m_oRunCircleMarker, MIL.M_MAX_ASSOCIATION_DISTANCE, m_ExecParams.dMaxAssocDist_px, MIL.M_NULL);

                if (m_ExecParams.dEdgeThreshold >= 0.0)
                    MIL.MmeasSetMarker(m_oRunCircleMarker, 87040 /*MIL.M_EDGE_THRESHOLD*/, m_ExecParams.dEdgeThreshold, MIL.M_NULL);


                if (m_ExecParams.RadiusScore != null)
                {
                    // Maxposs = MIL.M_MAX_POSSIBLE_VALUE
                    // smallest circle = 0,0,0,MaxPoss
                    // biggest circle = 0,MaxPoss,MaxPoss,MaxPoss 

                    MIL.MmeasSetScore(m_oRunCircleMarker, MIL.M_RADIUS_SCORE, m_ExecParams.RadiusScore.dMin, m_ExecParams.RadiusScore.dLow, m_ExecParams.RadiusScore.dHig, m_ExecParams.RadiusScore.dMax, m_ExecParams.RadiusScore.dScoreOffset, MIL.M_DEFAULT, MIL.M_DEFAULT);
                }

                if (m_ExecParams.StrengthScore != null)
                {
                    // Flat = 0,0,MaxPoss,MaxPoss
                    // Creaneau Up = val,val,MaxPoss,MaxPoss
                    // Creaneau Down = 0,0,val,val
                    // Creneau up/down = val1,val1,val2,val2
                    // Pyramide = val0,val1,val3,val4

                    MIL.MmeasSetScore(m_oRunCircleMarker, MIL.M_STRENGTH_SCORE, m_ExecParams.StrengthScore.dMin, m_ExecParams.StrengthScore.dLow, m_ExecParams.StrengthScore.dHig, m_ExecParams.StrengthScore.dMax, m_ExecParams.StrengthScore.dScoreOffset, MIL.M_DEFAULT, MIL.M_DEFAULT);
                }
                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(String.Format("DMDieExecutor:InitRunData FAIL = {0}", ex.Message));
            }
            finally
            {

            }
            return bSuccess;
        }
        public bool Measure(ref Cluster2DDieDM p_Cluster)
        {
            MIL_ID p_DieImageId = p_Cluster.OriginalProcessingImage.GetMilImage();
            // /!\ p_DieImageId est un buffer de type Grayscale ou Binaire en 8 bits !! ie (8 + MIL.M_UNSIGNED) /!\ 

            // calcul des mesures de diameter et de delta Offest a partie des targets 
            bool bSuccess = false;
            MIL_ID milImageChildId = MIL.M_NULL;

            try
            {
                // on verifie si l'image passer en paramètre correspond à notre recette 
                if (p_DieImageId == MIL.M_NULL)
                {
                    throw new ApplicationException(String.Format("DMDieExecutor::Measure Error : Input Image is Null"));
                }

                m_nDieImgSizeX_px = (int)MIL.MbufInquire(p_DieImageId, MIL.M_SIZE_X);
                m_nDieImgSizeY_px = (int)MIL.MbufInquire(p_DieImageId, MIL.M_SIZE_Y);
                if (m_nDieImgSizeX_px != m_dmRecipe.BaseImageSizeX || m_nDieImgSizeY_px != m_dmRecipe.BaseImageSizeY)
                {
                    throw new ApplicationException(String.Format("DMDieExecutor::Measure Error : Input Image Size is different from recipe"));
                }

                float lfPixelSizeX_um = 1.0f;
                float lfPixelSizeY_um = 1.0f;
                if (p_Cluster.Layer.Matrix is RectangularMatrix)
                {
                    RectangularMatrix mat = p_Cluster.Layer.Matrix as RectangularMatrix;
                    lfPixelSizeX_um = mat.PixelWidth;
                    lfPixelSizeY_um = mat.PixelHeight;
                }

                if (!InitMeasureMarker(lfPixelSizeX_um, lfPixelSizeY_um))
                {
                    throw new ApplicationException(String.Format("DMDieExecutor::Measure Error : Init error"));
                }

                m_nWindowMarginX_px = m_dmRecipe.WindowMarginX;
                m_nWindowMarginY_px = m_dmRecipe.WindowMarginY;

                int nChildW = m_dmRecipe.ModelSizeX + 2 * m_dmRecipe.WindowMarginX;
                int nChildH = m_dmRecipe.ModelSizeY + 2 * m_dmRecipe.WindowMarginY;

                float ChildOffsetX = ((float)nChildW * 0.5f);
                float ChildOffsetY = ((float)nChildH * 0.5f);

                if (IsCancellationRequested)
                    return false;

                MIL.MbufChild2d(p_DieImageId, 0, 0, nChildW, nChildH, ref milImageChildId);

                double dDieAvgDiameter_um = 0.0;
                double dNbDiameterCompute = 0.0;
                List<Blob> blobList = new List<Blob>(m_dmRecipe._ListTargetPos.Count);
                foreach (PointF ptFMeasure in m_dmRecipe._ListTargetPos)
                {
                    if (IsCancellationRequested)
                        break;

                    float fPosTLX = Math.Min(Math.Max(0, ptFMeasure.X - ChildOffsetX), (float)m_nDieImgSizeX_px - 1.0f);
                    float fPosTLY = Math.Min(Math.Max(0, ptFMeasure.Y - ChildOffsetY), (float)m_nDieImgSizeY_px - 1.0f);

                    float fPosBRX = Math.Min(Math.Max(0, fPosTLX + (float)nChildW), (float)m_nDieImgSizeX_px - 1.0f);
                    float fPosBRY = Math.Min(Math.Max(0, fPosTLY + (float)nChildH), (float)m_nDieImgSizeY_px - 1.0f);

                    int nChldW = (int)(fPosBRX - fPosTLX);
                    int nChldH = (int)(fPosBRY - fPosTLY);

                    MIL.MbufChildMove(milImageChildId, (int)fPosTLX, (int)fPosTLY, nChldW, nChldH, MIL.M_DEFAULT);

                    Blob blob = new Blob(blobList.Count, p_Cluster);

                    // Calcul du rectangle
                    blob.pixelRect.X = p_Cluster.imageRect.X + (int)fPosTLX;
                    blob.pixelRect.Y = p_Cluster.imageRect.Y + (int)fPosTLY;
                    blob.pixelRect.Width = nChldW;
                    blob.pixelRect.Height = nChldH;
                    blob.micronQuad = p_Cluster.Layer.Matrix.pixelToMicron(blob.pixelRect);

                    // Blob Characteristics
                    blob.characteristics.Add(BlobCharacteristics.BOX_X_MIN, p_Cluster.imageRect.X + fPosTLX);
                    blob.characteristics.Add(BlobCharacteristics.BOX_Y_MIN, p_Cluster.imageRect.Y + fPosTLY);
                    blob.characteristics.Add(BlobCharacteristics.BOX_X_MAX, p_Cluster.imageRect.X + fPosBRX);
                    blob.characteristics.Add(BlobCharacteristics.BOX_Y_MAX, p_Cluster.imageRect.Y + fPosBRY);
                    blob.characteristics.Add(BlobCharacteristics.MicronArea, blob.micronQuad.SurroundingRectangle.Area());
                    blob.characteristics.Add(ClusterCharacteristics.RealWidth, (double)blob.micronQuad.SurroundingRectangle.Width);
                    blob.characteristics.Add(ClusterCharacteristics.RealHeight, (double)blob.micronQuad.SurroundingRectangle.Height);

                    // Find the circle and measure its position and radius.
                    MIL.MmeasFindMarker(MIL.M_DEFAULT, milImageChildId, m_oRunCircleMarker, MIL.M_DEFAULT);

                    // If occurrence is found, show the results.
                    MIL_INT NumberOccurrencesFound = 0;
                    MIL.MmeasGetResult(m_oRunCircleMarker, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref NumberOccurrencesFound);

                    if (NumberOccurrencesFound >= 1)
                    {
                        double CircleCenterX = 0.0;
                        double CircleCenterY = 0.0;
                        double CircleRadius = 0.0;

                        MIL.MmeasGetResult(m_oRunCircleMarker, MIL.M_POSITION, ref CircleCenterX, ref CircleCenterY);
                        MIL.MmeasGetResult(m_oRunCircleMarker, MIL.M_RADIUS, ref CircleRadius);

                        blob.characteristics.Add(BlobCharacteristics.CENTER_OF_GRAVITY_X, p_Cluster.imageRect.X + CircleCenterX);
                        blob.characteristics.Add(BlobCharacteristics.CENTER_OF_GRAVITY_Y, p_Cluster.imageRect.Y + CircleCenterY);

                        double dDiameterum = 2.0 * CircleRadius * (double)((lfPixelSizeX_um + lfPixelSizeY_um) * 0.5f);
                        double dDeltaXum = (CircleCenterX - ChildOffsetX) * lfPixelSizeX_um;
                        double dDeltaYum = (CircleCenterY - ChildOffsetY) * lfPixelSizeY_um;
                        double dDistanceum = Math.Sqrt(dDeltaXum * dDeltaXum + dDeltaYum * dDeltaYum);

                        dDieAvgDiameter_um += dDiameterum;
                        dNbDiameterCompute++;
                        blob.characteristics[Blob2DCharacteristics.Diameter] = dDiameterum;

                        blob.characteristics[Blob2DCharacteristics.DeltaTargetX] = dDeltaXum;
                        blob.characteristics[Blob2DCharacteristics.DeltaTargetY] = dDeltaYum;
                        blob.characteristics[Blob2DCharacteristics.OffsetPos] = dDistanceum;
                    }
                    else
                    {
                        // Missing bump

                        blob.characteristics.Add(BlobCharacteristics.CENTER_OF_GRAVITY_X, p_Cluster.imageRect.X + fPosTLX + ChildOffsetX);
                        blob.characteristics.Add(BlobCharacteristics.CENTER_OF_GRAVITY_Y, p_Cluster.imageRect.Y + fPosTLX + ChildOffsetX);

                        blob.characteristics[Blob2DCharacteristics.isMissing] = 1.0;
                    }

                    blobList.Add(blob);
                }

                if (IsCancellationRequested)
                    return false;

                if (dNbDiameterCompute != 0.0)
                    dDieAvgDiameter_um /= dNbDiameterCompute;

                p_Cluster.characteristics[Cluster2DCharacteristics.DiameterAverage] = dDieAvgDiameter_um;

                p_Cluster.characteristics[SizingCharacteristics.DefectMaxSize] = dDieAvgDiameter_um;
                p_Cluster.characteristics[SizingCharacteristics.TotalDefectSize] = dDieAvgDiameter_um;
                p_Cluster.characteristics[SizingCharacteristics.SizingType] = eSizingType.ByDiameter;

                p_Cluster.blobList.AddRange(blobList);

                bSuccess = true;
            }
            catch
            {
                throw;
            }
            finally
            {

                if (m_oRunCircleMarker != MIL.M_NULL)
                {
                    MIL.MmeasFree(m_oRunCircleMarker);
                    m_oRunCircleMarker = MIL.M_NULL;
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
        private bool m_hasDisposed = false;
        //Finalizer
        ~DMDieExecutor()
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
            // release des data inhérante à chaque run 
            if (m_oRunCircleMarker != MIL.M_NULL)
            {
                MIL.MmeasFree(m_oRunCircleMarker);
                m_oRunCircleMarker = MIL.M_NULL;
            }
        }

        private void ReleaseSharedData()
        {

        }

        private void ReleaseUnmanagedResources()
        {
            ReleaseRunData();

            if (!m_bIsCloned)
            {
                ReleaseSharedData();
            }
        }

        #endregion

        #region ICloneable Members and methods
        protected void CloneData(ref DMDieExecutor cloned)
        {
            /*  try
              {
                  // on clone le circle marker seuleument

                  // Save
                  MIL_INT BufferSize = 0;
                  MIL.MmeasStream(MIL.M_NULL, m_MilSys, MIL.M_INQUIRE_SIZE_BYTE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_oRunCircleMarker, ref BufferSize);
                  byte[] Buffer = new byte[BufferSize];
                  MIL.MmeasStream(Buffer, m_MilSys, MIL.M_SAVE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_oRunCircleMarker, ref BufferSize);
                  // restore buffer
                  MIL_INT BufferSizeLoad = 0;
                  MIL.MmeasStream(Buffer, m_MilSys, MIL.M_RESTORE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref cloned.m_oRunCircleMarker, ref BufferSizeLoad);

               }
              catch (System.Exception ex)
              {
                  throw new ApplicationException(String.Format("DMDieExecutor:CloneData FAIL = {0}", ex.Message));
              }
              finally
              {

              }*/
        }

        protected object DeepCopy()
        {
            DMDieExecutor cloned = MemberwiseClone() as DMDieExecutor;
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
