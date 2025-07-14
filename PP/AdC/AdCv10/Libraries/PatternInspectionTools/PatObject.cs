using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Matrox.MatroxImagingLibrary;

using OStorageTools.Ole;

namespace PatternInspectionTools
{
    public sealed class PatObject : IDisposable, ICloneable
    {
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int compress(byte[] dest, out uint destlen, byte[] source, uint sourcelen);
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(byte[] dest, uint[] destlen, byte[] source, uint sourcelen);

        public const int _DARK = 0;
        public const int _BRIGHT = 1;
        public const int _BOTH = 2;

        private MIL_ID m_MilSys = MIL.M_NULL;

        #region Pattern File Data Member
        public String m_sPatFilePath = String.Empty;

        // Pattern Settings
        private int m_nTrainPatternSize_X = 0;
        private int m_nTrainPatternSize_Y = 0;
        public int TrainSize_X
        {
            get { return m_nTrainPatternSize_X; }
            private set { m_nTrainPatternSize_X = value; }
        }
        public int TrainSize_Y
        {
            get { return m_nTrainPatternSize_Y; }
            private set { m_nTrainPatternSize_Y = value; }
        }

        private int m_nLocType;	            // 0 : no align required, 1 : use align
        private int m_bUseSobel;            // use sobel pré-filter
        public bool UseSobel
        {
            get { return (m_bUseSobel != 0); }
        }
        private int m_bDeleteUnlocalized;   // Delete unlocalized die image (DEPRECATED)
        private uint m_uNbFileToTrain;      // Nb images used for train

        // Advanced Localisation Settings 
        private PatternLocAdvSettings m_oLocSettings = null; // settings for internal die alignement
        public PatternLocAdvSettings LocSettings
        {
            get { return m_oLocSettings; }
            private set { m_oLocSettings = value; }
        }

        private int m_nLocSizeW;
        private int m_nLocSizeH;
        private MIL_ID m_oLocImg = MIL.M_NULL;
        private MIL_ID m_oLocImgMsk = MIL.M_NULL;

        // Pattern Buffers
        private int m_nSizeW;
        private int m_nSizeH;

        private MIL_ID m_oMean = MIL.M_NULL;
        private MIL_ID m_oStdDev = MIL.M_NULL;
        private MIL_ID m_oMask = MIL.M_NULL;
        public MIL_ID BufMean { get { return m_oMean; } }
        public MIL_ID BufStdDev { get { return m_oStdDev; } }
        public MIL_ID BufMask { get { return m_oMask; } }

        // Inpection settings
        private PatternInspectSettings m_oInspSettings = null;
        public PatternInspectSettings InspSettings
        {
            get { return m_oInspSettings; }
            private set { m_oInspSettings = value; }
        }

        public void FlagClone() { m_bIsCloned = true; }
        private bool m_bIsCloned = false;

        #endregion
        public int LocType
        {
            get { return m_nLocType; }
            private set { m_nLocType = value; }
        }

        static public PatObject OpenPatFile(MIL_ID p_milSys, string p_sPatFilePath)
        {
            PatObject patobj = new PatObject(p_milSys);
            String sErrMsg;
            if (!patobj.LoadFromPatFile(p_sPatFilePath, out sErrMsg))
            {
                patobj = null;
                throw new Exception("OpenPatFile Exception : " + sErrMsg);
            }
            return patobj;
        }

        public PatObject(MIL_ID p_milSys)
        {
            m_MilSys = p_milSys;


        }

        public bool LoadFromPatFile(string p_sPatFilePath, out string p_sErrMsg)
        {
            string sShortFileName = System.IO.Path.GetFileNameWithoutExtension(p_sPatFilePath);
            p_sErrMsg = String.Empty;
            bool bSuccess = true;
            try
            {
                // check if file exist
                if (!File.Exists(p_sPatFilePath))
                {
                    p_sErrMsg = "This Pattern file path {" + p_sPatFilePath + "} doesn't exist !";
                    return false;
                }
                // check if extension is correct
                string sExt = System.IO.Path.GetExtension(p_sPatFilePath);
                if (false == (sExt.Equals(".pat", StringComparison.InvariantCultureIgnoreCase)))
                {
                    p_sErrMsg = "This Pattern file extension cannot be read {" + p_sPatFilePath + "} !";
                    return false;
                }
                if (false == OleStorage.IsAStorageFile(p_sPatFilePath))
                {
                    p_sErrMsg = "This file is not a Pattern storage file {" + p_sPatFilePath + "} !";
                    return false;
                }

                using (OleStorage storage = OleStorage.CreateInstance(p_sPatFilePath))
                {
                    if (storage != null)
                    {
                        try
                        {
                            // [ROOT]
                            // ->LocSettings
                            // ->PatSettings
                            // ->LocSize
                            // ->LocImg
                            // ->LocMsk
                            // ->Size
                            // ->MeanData
                            // ->StdData
                            // ->MskData
                            // ->InspectionParameters

                            int nLength = 0;
                            byte[] strbuff = null;
                            uint nBufLen = 0;
                            byte[] buffertoDecompress = null;
                            byte[] byteArray = null;
                            OleStream oStream = null;

                            /*********************** PatSettings  ********************************/
                            oStream = storage.OpenStream("PatSettings");

                            m_nTrainPatternSize_X = oStream.ReadInt();
                            m_nTrainPatternSize_Y = oStream.ReadInt();
                            m_nLocType = oStream.ReadInt();
                            m_bUseSobel = oStream.ReadInt();

                            m_bDeleteUnlocalized = oStream.ReadInt(); // if !=0 Delete files that cannot be Localized (Use for Pattern Training) Not Use Here
                            uint uNbFileUseForTraining = oStream.ReadUInt32(); // Nb Fles use by training to obtain such data (Not used here and needed for iterative statistical training (possible next Dev))
                            m_uNbFileToTrain = uNbFileUseForTraining;

                            oStream.Close(); oStream = null;

                            if (m_nLocType == 1) // Loc settings are read only if it is usefull
                            {

                                m_oLocSettings = new PatternLocAdvSettings();

                                /*********************** LocSettings  ********************************/
                                oStream = storage.OpenStream("LocSettings");

                                m_oLocSettings.m_nMethodType = oStream.ReadInt();
                                m_oLocSettings.m_nRefPt_X = oStream.ReadInt();
                                m_oLocSettings.m_nRefPt_Y = oStream.ReadInt();
                                m_oLocSettings.m_nROIOrigin_X = oStream.ReadInt();
                                m_oLocSettings.m_nROIOrigin_Y = oStream.ReadInt();
                                m_oLocSettings.m_nROISize_sx = oStream.ReadInt();
                                m_oLocSettings.m_nROISize_sy = oStream.ReadInt();
                                m_oLocSettings.m_nUseMask = oStream.ReadInt();
                                m_oLocSettings.m_nMilPrmSpeed = oStream.ReadInt();
                                m_oLocSettings.m_nMilPrmAccuracy = oStream.ReadInt();
                                m_oLocSettings.m_nMilPrmPolarity = oStream.ReadInt();

                                m_oLocSettings.m_dMilPrmAcceptance = oStream.ReadDouble();
                                m_oLocSettings.m_dMilPrmCertainty = oStream.ReadDouble();
                                m_oLocSettings.m_dMilPrmScaleFactor_Min = oStream.ReadDouble();
                                m_oLocSettings.m_dMilPrmScaleFactor_Max = oStream.ReadDouble();

                                m_oLocSettings.m_nOpencvPrmThreshold = oStream.ReadInt();

                                nLength = oStream.ReadInt();
                                strbuff = oStream.ReadBuffer(nLength);
                                m_oLocSettings.m_csLocImagePath = ASCIIEncoding.ASCII.GetString(strbuff);

                                nLength = oStream.ReadInt();
                                strbuff = oStream.ReadBuffer(nLength);
                                m_oLocSettings.m_csLocMaskPath = ASCIIEncoding.ASCII.GetString(strbuff);

                                oStream.Close(); oStream = null;

                                /*********************** LocSize  ********************************/
                                oStream = storage.OpenStream("LocSize");

                                m_nLocSizeW = oStream.ReadInt(); //Width 
                                m_nLocSizeH = oStream.ReadInt(); //Height

                                oStream.Close(); oStream = null;

                                /*********************** LocImg  ********************************/
                                oStream = storage.OpenStream("LocImg");

                                MIL.MbufAlloc2d(m_MilSys, (MIL_INT)m_nLocSizeW, (MIL_INT)m_nLocSizeH, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_oLocImg);

                                nBufLen = oStream.ReadUInt32();
                                try
                                {
                                    buffertoDecompress = oStream.ReadBuffer((int)nBufLen);
                                    byteArray = new byte[(m_nLocSizeH * m_nLocSizeW) * sizeof(byte)];
                                    uint[] destlen = new uint[1];
                                    destlen[0] = (uint)((m_nLocSizeH * m_nLocSizeW) * sizeof(byte));
                                    int nzres = uncompress(byteArray, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                                    MIL.MbufPut2d(m_oLocImg, 0, 0, m_nLocSizeW, m_nLocSizeH, byteArray);
                                }
                                catch (Exception Ex)
                                {
                                    if (m_oLocImg != MIL.M_NULL)
                                    {
                                        MIL.MbufFree(m_oLocImg);
                                        m_oLocImg = MIL.M_NULL;
                                    }

                                    p_sErrMsg = "Error in Pattern storage file {" + sShortFileName + "} - LocImg Stream - " + Ex.Message;
                                    bSuccess = false;
                                }
                                finally
                                {
                                    buffertoDecompress = null;
                                    byteArray = null;
                                }
                                oStream.Close(); oStream = null;

                                /*********************** LocMsk  ********************************/
                                if (m_oLocSettings.m_nUseMask != 0)
                                {
                                    oStream = storage.OpenStream("LocMsk");

                                    MIL.MbufAlloc2d(m_MilSys, (MIL_INT)m_nLocSizeW, (MIL_INT)m_nLocSizeH, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_oLocImgMsk);

                                    nBufLen = oStream.ReadUInt32();
                                    try
                                    {
                                        buffertoDecompress = oStream.ReadBuffer((int)nBufLen);
                                        byteArray = new byte[(m_nLocSizeH * m_nLocSizeW) * sizeof(byte)];
                                        uint[] destlen = new uint[1];
                                        destlen[0] = (uint)((m_nLocSizeH * m_nLocSizeW) * sizeof(byte));
                                        int nzres = uncompress(byteArray, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                                        MIL.MbufPut2d(m_oLocImgMsk, 0, 0, m_nLocSizeW, m_nLocSizeH, byteArray);
                                    }
                                    catch (Exception Ex)
                                    {
                                        if (m_oLocImgMsk != MIL.M_NULL)
                                        {
                                            MIL.MbufFree(m_oLocImgMsk);
                                            m_oLocImgMsk = MIL.M_NULL;
                                        }

                                        p_sErrMsg = "Error in Pattern storage file {" + sShortFileName + "} - LocMsk Stream - " + Ex.Message;
                                        bSuccess = false;
                                    }
                                    finally
                                    {
                                        buffertoDecompress = null;
                                        byteArray = null;
                                    }
                                    oStream.Close(); oStream = null;
                                }
                                else
                                    m_oLocImgMsk = MIL.M_NULL;
                            }

                            /*********************** Size  ********************************/
                            oStream = storage.OpenStream("Size");

                            m_nSizeW = oStream.ReadInt(); //Width 
                            m_nSizeH = oStream.ReadInt(); //Height

                            oStream.Close(); oStream = null;

                            /*********************** MeanData  ********************************/
                            oStream = storage.OpenStream("MeanData");

                            MIL.MbufAlloc2d(m_MilSys, (MIL_INT)m_nSizeW, (MIL_INT)m_nSizeH, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oMean);

                            nBufLen = oStream.ReadUInt32();
                            try
                            {
                                buffertoDecompress = oStream.ReadBuffer((int)nBufLen);
                                byteArray = new byte[(m_nSizeH * m_nSizeW) * sizeof(float)];
                                uint[] destlen = new uint[1];
                                destlen[0] = (uint)((m_nSizeH * m_nSizeW) * sizeof(float));
                                int nzres = uncompress(byteArray, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                                MIL.MbufPut2d(m_oMean, 0, 0, m_nSizeW, m_nSizeH, byteArray);
                            }
                            catch (Exception Ex)
                            {
                                if (m_oMean != MIL.M_NULL)
                                {
                                    MIL.MbufFree(m_oMean);
                                    m_oMean = MIL.M_NULL;
                                }

                                p_sErrMsg = "Error in Pattern storage file {" + sShortFileName + "} - MeanData Stream - " + Ex.Message;
                                bSuccess = false;
                            }
                            finally
                            {
                                buffertoDecompress = null;
                                byteArray = null;
                            }
                            oStream.Close(); oStream = null;

                            /*********************** StdData  ********************************/
                            oStream = storage.OpenStream("StdData");

                            MIL.MbufAlloc2d(m_MilSys, (MIL_INT)m_nSizeW, (MIL_INT)m_nSizeH, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref m_oStdDev);

                            nBufLen = oStream.ReadUInt32();
                            try
                            {
                                buffertoDecompress = oStream.ReadBuffer((int)nBufLen);
                                byteArray = new byte[(m_nSizeH * m_nSizeW) * sizeof(float)];
                                uint[] destlen = new uint[1];
                                destlen[0] = (uint)((m_nSizeH * m_nSizeW) * sizeof(float));
                                int nzres = uncompress(byteArray, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                                MIL.MbufPut2d(m_oStdDev, 0, 0, m_nSizeW, m_nSizeH, byteArray);
                            }
                            catch (Exception Ex)
                            {
                                if (m_oStdDev != MIL.M_NULL)
                                {
                                    MIL.MbufFree(m_oStdDev);
                                    m_oStdDev = MIL.M_NULL;
                                }

                                p_sErrMsg = "Error in Pattern storage file {" + sShortFileName + "} - StdData Stream - " + Ex.Message;
                                bSuccess = false;
                            }
                            finally
                            {
                                buffertoDecompress = null;
                                byteArray = null;
                            }
                            oStream.Close(); oStream = null;

                            /*********************** MskData  ********************************/
                            oStream = storage.OpenStream("MskData");

                            MIL.MbufAlloc2d(m_MilSys, (MIL_INT)m_nSizeW, (MIL_INT)m_nSizeH, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref m_oMask);

                            nBufLen = oStream.ReadUInt32();
                            try
                            {
                                buffertoDecompress = oStream.ReadBuffer((int)nBufLen);
                                byteArray = new byte[(m_nSizeH * m_nSizeW) * sizeof(byte)];
                                uint[] destlen = new uint[1];
                                destlen[0] = (uint)((m_nSizeH * m_nSizeW) * sizeof(byte));
                                int nzres = uncompress(byteArray, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                                MIL.MbufPut2d(m_oMask, 0, 0, m_nSizeW, m_nSizeH, byteArray);
                            }
                            catch (Exception Ex)
                            {
                                if (m_oMask != MIL.M_NULL)
                                {
                                    MIL.MbufFree(m_oMask);
                                    m_oMask = MIL.M_NULL;
                                }

                                p_sErrMsg = "Error in Pattern storage file {" + sShortFileName + "} - MskData Stream - " + Ex.Message;
                                bSuccess = false;
                            }
                            finally
                            {
                                buffertoDecompress = null;
                                byteArray = null;
                            }
                            oStream.Close(); oStream = null;

                            /*********************** InspectionParameters  ********************************/
                            m_oInspSettings = new PatternInspectSettings();
                            oStream = storage.OpenStream("InspectionParameters");
                            if (oStream == null)
                            {
                                // probably an old and obsolete format -- use some Standard inspection settings to avoid ABORTION
                                // Signal it to the user
                                p_sErrMsg = "Obsolete pat file format {" + sShortFileName + "} : Inspection parameters are missing - use default parameter";
                                m_oInspSettings.m_nType = 0;
                                m_oInspSettings.m_nNbROI = 1;
                                m_oInspSettings.m_fBlobExElongation[_DARK] = 0;
                                m_oInspSettings.m_nBlobExArea[_DARK] = 9;
                                m_oInspSettings.m_nBlobExBreadth[_DARK] = 1;

                                m_oInspSettings.m_vnNormalizeMethod.Clear();
                                m_oInspSettings.m_vfK[_DARK].Clear(); m_oInspSettings.m_vfK[_DARK].Clear();

                                m_oInspSettings.m_vnNormalizeMethod.Add(0);
                                m_oInspSettings.m_vfK[_DARK].Add(2.0f);
                                m_oInspSettings.m_vfK[_BRIGHT].Add(3.0f);
                                m_oInspSettings.m_vnTh[_DARK].Add(5);
                                m_oInspSettings.m_vnTh[_BRIGHT].Add(10);
                                m_oInspSettings.m_bInspectionByROI = 0;
                            }
                            else
                            {
                                m_oInspSettings.m_nType = oStream.ReadInt();    //0: dark; 1: bright; 2: both (dark & bright)
                                m_oInspSettings.m_nNbROI = oStream.ReadInt();   //nb de ROI used

                                m_oInspSettings.m_fBlobExElongation[_DARK] = oStream.ReadFloat();
                                m_oInspSettings.m_fBlobExElongation[_BRIGHT] = oStream.ReadFloat();

                                m_oInspSettings.m_nBlobExArea[_DARK] = oStream.ReadInt();
                                m_oInspSettings.m_nBlobExArea[_BRIGHT] = oStream.ReadInt();

                                m_oInspSettings.m_nBlobExBreadth[_DARK] = oStream.ReadInt();
                                m_oInspSettings.m_nBlobExBreadth[_BRIGHT] = oStream.ReadInt();

                                m_oInspSettings.m_vnNormalizeMethod.Clear();
                                m_oInspSettings.m_vfK[_DARK].Clear(); m_oInspSettings.m_vfK[_DARK].Clear();
                                m_oInspSettings.m_vnTh[_DARK].Clear(); m_oInspSettings.m_vnTh[_BRIGHT].Clear();
                                for (int i = 0; i < m_oInspSettings.m_nNbROI + 1; i++)
                                {
                                    m_oInspSettings.m_vnNormalizeMethod.Add(oStream.ReadInt());
                                    m_oInspSettings.m_vfK[_DARK].Add(oStream.ReadFloat());
                                    m_oInspSettings.m_vfK[_BRIGHT].Add(oStream.ReadFloat());
                                    m_oInspSettings.m_vnTh[_DARK].Add(oStream.ReadInt());
                                    m_oInspSettings.m_vnTh[_BRIGHT].Add(oStream.ReadInt());
                                }
                                m_oInspSettings.m_bInspectionByROI = oStream.ReadInt();
                                oStream.Close(); oStream = null;
                            }

                        }
                        catch (Exception Ex)
                        {
                            p_sErrMsg = String.Format("Load File Pattern Exception for file <{0}> : {1}", sShortFileName, Ex);
                            bSuccess = false;
                        }
                        finally
                        {
                            storage.Close();
                        }
                    } //endif(storage != null)
                } // end using
            }
            catch (Exception Ex)
            {
                p_sErrMsg = String.Format("Load File Pattern UNKNOWN Exception for file <{0}> : {1}", sShortFileName, Ex);
                bSuccess = false;
            }
            finally
            {
                if (!bSuccess)
                {
                    // on clean
                    Dispose();
                }
            }
            return bSuccess;
        }

        public bool SavePatFile(string sPatFilePath)
        {
            throw new NotImplementedException("Save Pat Object (not implemented) - not used ?");
            //bool bSuccess = false;
            //return bSuccess;
        }

        public MIL_ID InitAlign()
        {
            MIL_ID oPatAlign = MIL.M_NULL;
            if (m_oLocSettings.m_nMethodType == 0)
            {
                // MIL Alignement
                if (m_oLocImg == MIL.M_NULL)
                    return oPatAlign;

                // Allocate a Geometric Model Finder context.
                MIL.MmodAlloc(m_MilSys, MIL.M_GEOMETRIC_CONTROLLED, MIL.M_DEFAULT, ref oPatAlign);

                // Define the model.
                MIL.MmodDefine(oPatAlign, MIL.M_IMAGE, m_oLocImg, 0, 0, m_nLocSizeW, m_nLocSizeH);

                MIL.MmodControl(oPatAlign, MIL.M_CONTEXT, MIL.M_SPEED, m_oLocSettings.SpeedToMil());
                MIL.MmodControl(oPatAlign, MIL.M_CONTEXT, MIL.M_ACCURACY, m_oLocSettings.AccuracyToMil());
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_SCALE_MIN_FACTOR, m_oLocSettings.m_dMilPrmScaleFactor_Min);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_SCALE_MAX_FACTOR, m_oLocSettings.m_dMilPrmScaleFactor_Max);
                MIL.MmodControl(oPatAlign, MIL.M_CONTEXT, MIL.M_SEARCH_ANGLE_RANGE, MIL.M_DISABLE);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_ANGLE_DELTA_NEG, 0.00);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_ANGLE_DELTA_POS, 0.00);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_ACCEPTANCE, m_oLocSettings.m_dMilPrmAcceptance);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_CERTAINTY, m_oLocSettings.m_dMilPrmCertainty);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_POLARITY, m_oLocSettings.PolarityToMil());

                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_NUMBER, 1);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_REFERENCE_X, m_oLocSettings.m_nRefPt_X);
                MIL.MmodControl(oPatAlign, MIL.M_DEFAULT, MIL.M_REFERENCE_Y, m_oLocSettings.m_nRefPt_Y);

                if ((m_oLocSettings.m_nUseMask != 0) && (m_oLocImgMsk != MIL.M_NULL))
                {
                    MIL.MmodMask(oPatAlign, MIL.M_DEFAULT, m_oLocImgMsk, MIL.M_DONT_CARE, MIL.M_DEFAULT);
                }

                // Preprocess the search context.
                MIL.MmodPreprocess(oPatAlign, MIL.M_DEFAULT);
            }
            else
            {
                // OPEN CV Alignment : Not Implemented
                throw new Exception("InitAlign Error : Open CV Localization Alignment is depracted and no longer available for this software version");
            }
            return oPatAlign;
        }

        // IDisposable Mechanism
        #region IDisposable Members and Methods

        private bool m_hasDisposed = false;

        //Finalizer
        ~PatObject()
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

        private void Dispose(bool disposing)
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

        private void ReleaseUnmanagedResources()
        {
            m_oLocSettings = null;
            m_oInspSettings = null;

            if (!m_bIsCloned)
            {
                // ces données là sont partagée entre les threads
                if (m_oLocImg != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oLocImg);
                    m_oLocImg = MIL.M_NULL;
                }

                if (m_oLocImgMsk != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oLocImgMsk);
                    m_oLocImgMsk = MIL.M_NULL;
                }

                if (m_oMean != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oMean);
                    m_oMean = MIL.M_NULL;
                }

                if (m_oStdDev != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oStdDev);
                    m_oStdDev = MIL.M_NULL;
                }

                if (m_oMask != MIL.M_NULL)
                {
                    MIL.MbufFree(m_oMask);
                    m_oMask = MIL.M_NULL;
                }
            }
        }
        #endregion

        #region ICloneable Members and methods

        private void CloneData(ref PatObject cloned)
        {
            CopyInitialData(ref cloned);
        }

        private void CopyInitialData(ref PatObject cloned)
        {
            /* if (this.m_oMean != MIL.M_NULL)
             {
                 MIL.MbufAlloc2d(m_pView.getMilSystem, (MIL_INT)m_nSizeW, (MIL_INT)m_nSizeH, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref cloned.m_oMean);
                 MIL.MbufCopy(this.m_oMean, cloned.m_oMean);
             }
             if (this.m_oStdDev != MIL.M_NULL)
             {
                 MIL.MbufAlloc2d(m_pView.getMilSystem, (MIL_INT)m_nSizeW, (MIL_INT)m_nSizeH, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref cloned.m_oStdDev);
                 MIL.MbufCopy(this.m_oStdDev, cloned.m_oStdDev);
             }
             if (this.m_oMask != MIL.M_NULL)
             {
                 MIL.MbufAlloc2d(m_pView.getMilSystem, (MIL_INT)m_nSizeW, (MIL_INT)m_nSizeH, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref cloned.m_oMask);
                 MIL.MbufCopy(this.m_oMask, cloned.m_oMask);
             }

             cloned.m_ListMilRoiMaskID = new List<MIL_ID>();
             foreach (MIL_ID oMilMaskId in this.m_ListMilRoiMaskID)
             {
                 if (oMilMaskId != MIL.M_NULL)
                 {
                     MIL_ID MiClonedRoiID = new MIL_ID();
                     MIL.MbufAlloc2d(m_pView.getMilSystem, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref MiClonedRoiID);
                     MIL.MbufCopy(oMilMaskId, MiClonedRoiID);
                     cloned.m_ListMilRoiMaskID.Add(MiClonedRoiID);
                 }
             }

             if (this.m_oPrediff_DARK != MIL.M_NULL)
             {
                 MIL.MbufAlloc2d(m_pView.getMilSystem, m_nSizeW, m_nSizeH, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref cloned.m_oPrediff_DARK);
                 MIL.MbufCopy(this.m_oPrediff_DARK, cloned.m_oPrediff_DARK);
             }
             if (this.m_oPrediff_BRIGHT != MIL.M_NULL)
             {
                 MIL.MbufAlloc2d(m_pView.getMilSystem, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref cloned.m_oPrediff_BRIGHT);
                 MIL.MbufCopy(this.m_oPrediff_BRIGHT, cloned.m_oPrediff_BRIGHT);
             }
             if (this.m_oPreParams_DARK != MIL.M_NULL)
             {
                 MIL.MbufAlloc2d(m_pView.getMilSystem, m_nSizeW, m_nSizeH, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref cloned.m_oPreParams_DARK);
                 MIL.MbufCopy(this.m_oPreParams_DARK, cloned.m_oPreParams_DARK);
             }
             if (this.m_oPreParams_BRIGHT != MIL.M_NULL)
             {
                 MIL.MbufAlloc2d(m_pView.getMilSystem, m_nTrainPatternSize_X, m_nTrainPatternSize_Y, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC + g_nAllocOption, ref cloned.m_oPreParams_BRIGHT);
                 MIL.MbufCopy(this.m_oPreParams_BRIGHT, cloned.m_oPreParams_BRIGHT);
             }

             cloned.m_oLocSettings = (PatternLocAdvSettings)this.m_oLocSettings.Clone();
             cloned.m_oInspSettings = (PatternInspectSettings)this.m_oInspSettings.Clone();
             cloned.m_dListNormalizationMean = new List<double>();
             cloned.m_dListNormalizationMean.AddRange(this.m_dListNormalizationMean);
             cloned.m_dListNormalizationStdDev = new List<double>();
             cloned.m_dListNormalizationStdDev.AddRange(this.m_dListNormalizationStdDev);
             cloned.m_nListNormalizationMaxPeak = new List<int>();
             cloned.m_nListNormalizationMaxPeak.AddRange(this.m_nListNormalizationMaxPeak);*/
        }

        private object DeepCopy()
        {
            PatObject cloned = MemberwiseClone() as PatObject;
            // ici on clone les instances nécéssaire au run (elles sont différentes et utlisé pour chaque run)
            // CloneData(ref cloned);
            cloned.FlagClone();
            return cloned;
        }

        public object Clone()
        {
            return DeepCopy();
        }

        #endregion

    }
}
