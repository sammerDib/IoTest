using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.BorderRemoval;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using MergeContext.Context;

using UnitySC.Shared.Tools;

namespace HazeModule
{
    // DARkVIEW HAZE VERSION
    [Obsolete("Only For DarkviewModule - use Haze LS otherwise", false)]
    public class HazeMeasurementModule : ModuleBase
    {
        private static class NativeMethods
        {
            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int InitLoadCall();
            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformMedianFilter(byte[] uIn, long uOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, int iRadius, long l2CacheMemSize);
            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformLinearDynScaling(long uIn, long uOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, byte uMin, byte uMax);

        }

        static HazeMeasurementModule()
        {
            NativeMethods.InitLoadCall();
#if DEBUG
            bool bUnitarytest = false;
            if (bUnitarytest)
            {
                InputHazeConfiguration inp = new InputHazeConfiguration();
                inp.InitColorMapFromFile(new PathString(@"C:\Altasight\IniRep\Color8Map.txt"), false);
                inp.InitColorMapFromFile(new PathString(@"C:\Altasight\IniRep\Color256Map.txt"), true);
                inp.InitHazeCoreFromFile_8(new PathString(@"C:\Altasight\IniRep\HazeCoreVal8.dat"));
                inp.InitHazeCoreFromFile_256(new PathString(@"C:\Altasight\IniRep\HazeCoreVal256.dat"));
                inp.RangeName = "RANGE RTI TEST";
                inp.InitScaleMaxFromString("0.977;2.19;5.35;10.1;20.2;51.0;125.0;");


                string sxmlfile = @"c:\temp\xmlserializeinputhazadata.xml";
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(InputHazeConfiguration));
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                using (XmlWriter writer = XmlWriter.Create(sxmlfile, settings))
                {
                    ser.Serialize(writer, inp);
                    writer.Close();
                }

                int nnnn = 01;
                nnnn++;

                InputHazeConfiguration outp;
                using (System.IO.FileStream fs = new System.IO.FileStream(sxmlfile, System.IO.FileMode.Open))
                {
                    XmlReader reader = XmlReader.Create(fs);
                    outp = (InputHazeConfiguration)ser.Deserialize(reader);
                    fs.Close();
                }


                nnnn++;
            }
#endif
        }


        private int Resizepx_X = 3000;
        private int Resizepx_Y = 3000;
        private InputHazeConfiguration inputDatabaseConfigParameters = null;
        private float[] HazeRangeFactor = new float[8];

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramMargin;
        public readonly IntParameter paramCurve1_LO;
        public readonly IntParameter paramCurve1_HI;
        public readonly IntParameter paramMedianRadius;
        public readonly IntParameter paramCurve2_LO;
        public readonly IntParameter paramCurve2_HI;


        //=================================================================
        // Constructeur
        //=================================================================
        public HazeMeasurementModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMargin = new DoubleParameter(this, "EdgeExclusionMargin");
            paramMargin.Value = 0;

            paramCurve1_LO = new IntParameter(this, "LowerThresholdCurve1", 0, 255);
            paramCurve1_LO.Value = 0;

            paramCurve1_HI = new IntParameter(this, "HigherThresholdCurve1", 0, 255);
            paramCurve1_HI.Value = 255;

            paramMedianRadius = new IntParameter(this, "MedianRadius", 0, 2000);
            paramMedianRadius.Value = 0;

            paramCurve2_LO = new IntParameter(this, "LowerThresholdCurve2", 0, 255);
            paramCurve2_LO.Value = 0;

            paramCurve2_HI = new IntParameter(this, "HigherThresholdCurve2", 0, 255);
            paramCurve2_HI.Value = 255;

        }

        protected override void OnInit()
        {
            base.OnInit();

            if (paramCurve1_LO.Value >= paramCurve1_HI.Value)
                throw new ApplicationException("Lower threshold should strictly be inferior to Higher Threshold (Curve 1)");

            if (paramCurve2_LO.Value >= paramCurve2_HI.Value)
                throw new ApplicationException("Lower threshold should strictly be inferior to Higher Threshold (Curve 2)");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;

            inputDatabaseConfigParameters = image.Layer.GetContextMachine<InputHazeConfiguration>(ConfigurationType.InputHaze.ToString());

            GenerateFactorHaze();

            // ADD a 1% margin border to ROI and adjust roi limits to the image borders 
            double dPercentLimit = 0.01;
            int nROIMarginX = (int)((double)Resizepx_X * dPercentLimit);
            int nROIMarginY = (int)((double)Resizepx_Y * dPercentLimit);

            MilImage milimg_ROI = new MilImage();
            MilImage milmsk_ROI = new MilImage();

            try
            {
                System.Drawing.Rectangle WaferRoiRect = System.Drawing.Rectangle.Empty;

                //---------------------------------
                // edge remove pour obtenir le mask
                //---------------------------------
                using (ImageBase mskimage = (ImageBase)image.DeepClone())
                {
                    EdgeRemoveAlgorithm edgeRemoveAlgo = new EdgeRemoveAlgorithm();
                    edgeRemoveAlgo.Margin = paramMargin.Value;
                    edgeRemoveAlgo.PerformRemoval(mskimage);

                    MilImage milmsk = mskimage.ResultProcessingImage.GetMilImage();
                    MIL.MimBinarize(milmsk.MilId, milmsk.MilId, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL);

                    // GetEmcompassingRect
                    using (MilBlobFeatureList blobFeatureList = new MilBlobFeatureList())
                    using (MilBlobResult blobResult = new MilBlobResult())
                    {
                        //--------------------------------------------------------------
                        // Calcul des blobs
                        //--------------------------------------------------------------
                        // Features a calculer
                        blobFeatureList.Alloc();
                        blobFeatureList.SelectFeature(MIL.M_BOX_X_MIN);
                        blobFeatureList.SelectFeature(MIL.M_BOX_X_MAX);
                        blobFeatureList.SelectFeature(MIL.M_BOX_Y_MIN);
                        blobFeatureList.SelectFeature(MIL.M_BOX_Y_MAX);

                        // Calcul des features pour tous les blobs
                        blobResult.Alloc();
                        blobResult.Calculate(milmsk, null, blobFeatureList);

#if DEBUG
                        //milmsk.ImageJ();
#endif

                        blobResult.Control(MIL.M_SAVE_RUNS, MIL.M_ENABLE);

                        MIL_INT nbBlobs = blobResult.Number;
                        if (nbBlobs != 1)
                        {
                            throw new ApplicationException("Error in Mask computation");
                        }

                        // Get blob areas
                        double[] lfDataBlob_Left = blobResult.GetResult(MIL.M_BOX_X_MIN);
                        double[] lfDataBlob_Right = blobResult.GetResult(MIL.M_BOX_X_MAX);
                        double[] lfDataBlob_Top = blobResult.GetResult(MIL.M_BOX_Y_MIN);
                        double[] lfDataBlob_Bottom = blobResult.GetResult(MIL.M_BOX_Y_MAX);

                        WaferRoiRect = new System.Drawing.Rectangle(
                            (int)Math.Floor(lfDataBlob_Left[0] + 1),                                           //X
                            (int)Math.Floor(lfDataBlob_Top[0] + 1),                                            //Y
                            (int)(Math.Floor(lfDataBlob_Right[0] + 1) - Math.Floor(lfDataBlob_Left[0] + 1)),   //Width
                            (int)(Math.Floor(lfDataBlob_Bottom[0] + 1) - Math.Floor(lfDataBlob_Top[0] + 1)));  //Height
                    }

                    // On applique les margin des roi
                    int nRcNewWidth = WaferRoiRect.Width + 2 * nROIMarginX;
                    int nRcNewX = WaferRoiRect.Left - nROIMarginX;
                    if (nRcNewX < 0)
                    {
                        // on depasse à gauche
                        nRcNewWidth += nRcNewX; // on enleve le delta hors de l'image à la Width
                        nRcNewX = 0;
                    }
                    if ((WaferRoiRect.Right + nROIMarginX) >= milmsk.SizeX)
                    {
                        // on depasse à droite
                        int nDelta = (WaferRoiRect.Right + nROIMarginX) - milmsk.SizeX;
                        nRcNewWidth -= nDelta; // on enleve le delta hors de l'image à la Width 
                    }
                    int nRcNewHeight = WaferRoiRect.Height + 2 * nROIMarginY;
                    int nRcNewY = WaferRoiRect.Top - nROIMarginY;
                    if (nRcNewY < 0)
                    {
                        // on depasse en haut
                        nRcNewHeight += nRcNewY; // on enleve le delta hors de l'image à la Height 
                        nRcNewY = 0;
                    }
                    if ((WaferRoiRect.Bottom + nROIMarginY) >= milmsk.SizeY)
                    {
                        // on depasse en bas
                        int nDelta = (WaferRoiRect.Bottom + nROIMarginY) - milmsk.SizeY;
                        nRcNewHeight -= nDelta; // on enleve le delta hors de l'image à la Height 
                    }
                    WaferRoiRect = new System.Drawing.Rectangle(nRcNewX, nRcNewY, nRcNewWidth, nRcNewHeight);

                    // On recupere les buffers ROI

                    // ROI mask Image
                    using (MilImage ChildMsk = new MilImage())
                    {
                        milmsk_ROI.Alloc2d(WaferRoiRect.Width, WaferRoiRect.Height, 8 + MIL.M_UNSIGNED, MIL.M_PROC + MIL.M_IMAGE);
                        ChildMsk.Child2d(milmsk, WaferRoiRect);
                        MilImage.Copy(ChildMsk, milmsk_ROI);
                    }

#if DEBUG
                    // milmsk_ROI.ImageJ();
#endif
                    // ROI Image
                    using (MilImage ChildImg = new MilImage())
                    {
                        milimg_ROI.Alloc2d(WaferRoiRect.Width, WaferRoiRect.Height, 8 + MIL.M_UNSIGNED, MIL.M_PROC + MIL.M_IMAGE);
                        ChildImg.Child2d(image.CurrentProcessingImage.GetMilImage(), WaferRoiRect);
                        MilImage.Copy(ChildImg, milimg_ROI);
                    }

#if DEBUG
                    //milimg_ROI.ImageJ();
#endif

                }

                System.Drawing.RectangleF waferRect_um = Wafer.SurroundingRectangle;

                double dPixelSize_X = (waferRect_um.Width - 2.0 * paramMargin.Value) / (double)WaferRoiRect.Width;
                double dPixelSize_Y = (waferRect_um.Height - 2.0 * paramMargin.Value) / (double)WaferRoiRect.Height;

                // -----------------
                // Resize
                // -----------------
                if (Resizepx_X != 0 && Resizepx_Y != 0)
                {
                    MilImage aux = null;
                    MilImage imgRsize = new MilImage();
                    imgRsize.Alloc2d(Resizepx_X, Resizepx_Y, 8 + MIL.M_UNSIGNED, MIL.M_PROC + MIL.M_IMAGE);
                    MIL.MimResize(milimg_ROI.MilId, imgRsize.MilId, MIL.M_FILL_DESTINATION, MIL.M_FILL_DESTINATION, MIL.M_DEFAULT);
                    aux = milimg_ROI;
                    milimg_ROI = imgRsize;
                    aux.Dispose();
                    aux = null;

                    MilImage mskRsize = new MilImage();
                    mskRsize.Alloc2d(Resizepx_X, Resizepx_Y, 8 + MIL.M_UNSIGNED, MIL.M_PROC + MIL.M_IMAGE);
                    MIL.MimResize(milmsk_ROI.MilId, mskRsize.MilId, MIL.M_FILL_DESTINATION, MIL.M_FILL_DESTINATION, MIL.M_DEFAULT);
                    //MIL.MimBinarize(mskRsize.MilId, mskRsize.MilId, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL); // on mets à 255 les gris dû au approc resize
                    aux = milmsk_ROI;
                    milmsk_ROI = mskRsize;
                    aux.Dispose();
                    aux = null;

                    dPixelSize_X *= (double)WaferRoiRect.Width / (double)Resizepx_X;
                    dPixelSize_Y *= (double)WaferRoiRect.Height / (double)Resizepx_Y;
                }
                MIL.MimArith(milmsk_ROI.MilId, 254, milmsk_ROI.MilId, MIL.M_SUB_CONST + MIL.M_SATURATION); // on veux que du 0 et du 1 donc saturation
#if DEBUG
                // milmsk_ROI.ImageJ();
#endif
                // -----------------
                // Perfom Curve 1
                // -----------------
                if ((paramCurve1_HI.Value != 255) || (paramCurve1_LO.Value != 0))
                {
                    try
                    {
                        milimg_ROI.Lock();
                        NativeMethods.PerformLinearDynScaling(milimg_ROI.HostAddress, milimg_ROI.HostAddress, milimg_ROI.Pitch, milimg_ROI.Pitch, milimg_ROI.SizeX, milimg_ROI.SizeY, (byte)paramCurve1_LO.Value, (byte)paramCurve1_HI.Value);
                    }
                    finally
                    {
                        milimg_ROI.Unlock();
                    }
                }

                // -----------------
                // Median Filter
                // -----------------
                if (paramMedianRadius.Value > 0)
                {
                    byte[] SrcBuff = new byte[milimg_ROI.SizeX * milimg_ROI.SizeY]; // TO DO voir si  le mbufget mets les data en contigous ou si il ya un pitch
                    milimg_ROI.Get(SrcBuff);
                    try
                    {
                        long l2CacheMemSize = 512 * 1024;
                        milimg_ROI.Lock();
                        NativeMethods.PerformMedianFilter(SrcBuff, milimg_ROI.HostAddress, milimg_ROI.SizeX, milimg_ROI.Pitch, milimg_ROI.SizeX, milimg_ROI.SizeY, paramMedianRadius.Value, l2CacheMemSize);
                    }
                    finally
                    {
                        milimg_ROI.Unlock();
                    }
                }

                // -----------------
                // Perfom Curve 2
                // -----------------
                if ((paramCurve1_HI.Value != 255) || (paramCurve2_LO.Value != 0))
                {
                    try
                    {
                        milimg_ROI.Lock();
                        NativeMethods.PerformLinearDynScaling(milimg_ROI.HostAddress, milimg_ROI.HostAddress, milimg_ROI.Pitch, milimg_ROI.Pitch, milimg_ROI.SizeX, milimg_ROI.SizeY, (byte)paramCurve2_LO.Value, (byte)paramCurve2_HI.Value);
                    }
                    finally
                    {
                        milimg_ROI.Unlock();
                    }
                }

                //-------------------
                // Init HAZE Measure
                //-------------------
                using (HazeMeasure hazedata = new HazeMeasure())
                {
                    hazedata.Init(inputDatabaseConfigParameters);
                    hazedata.PixelSizeX = dPixelSize_X;
                    hazedata.PixelSizeY = dPixelSize_Y;
                    hazedata.EdgeExlusion_um = (int)Math.Round(paramMargin.Value);
                    hazedata.SizeWidth = milimg_ROI.SizeX;
                    hazedata.SizeHeight = milimg_ROI.SizeY;

                    hazedata.InputImageGL = new byte[milimg_ROI.SizeX * milimg_ROI.SizeY];
                    milimg_ROI.Get(hazedata.InputImageGL);

                    hazedata.InputWaferMask = new byte[milmsk_ROI.SizeX * milmsk_ROI.SizeY];
                    milmsk_ROI.Get(hazedata.InputWaferMask);

                    //-------------------
                    // LUTs
                    //-------------------
                    MilImage[] img8b = new MilImage[3];
                    MilImage[] img256b = new MilImage[3];
                    for (int i = 0; i < 3; i++)
                    {
                        img8b[i] = new MilImage();
                        img8b[i].Alloc2dCompatibleWith(milimg_ROI);

                        img256b[i] = new MilImage();
                        img256b[i].Alloc2dCompatibleWith(milimg_ROI);
                    }

                    MIL_ID LutKernel = MIL.M_NULL;
                    MIL.MbufAlloc1d(Mil.Instance.MilSystem, 256, 8 + MIL.M_UNSIGNED, MIL.M_LUT, ref LutKernel);
                    MilImage.checkMilError("Fail to alloc Haze LUT");
                    for (int i = 0; i < 6; i++)
                    {
                        if (i / 3 <= 0)
                        {
                            // 8 bits
                            MIL.MbufPut(LutKernel, hazedata.GetLut(i, false));
                            MIL.MimLutMap(milimg_ROI.MilId, img8b[i].MilId, LutKernel);
                            MIL.MimArith(img8b[i].MilId, milmsk_ROI.MilId, img8b[i].MilId, MIL.M_MULT);
                        }
                        else
                        {
                            // 256 bits 
                            MIL.MbufPut(LutKernel, hazedata.GetLut(i - 3, true));
                            MIL.MimLutMap(milimg_ROI.MilId, img256b[i - 3].MilId, LutKernel);
                            MIL.MimArith(img256b[i - 3].MilId, milmsk_ROI.MilId, img256b[i - 3].MilId, MIL.M_MULT);
                        }
                    }
                    MIL.MbufFree(LutKernel); LutKernel = MIL.M_NULL;

                    //-------------------
                    // Histograms
                    //-------------------
                    MIL_ID milHisto = MIL.M_NULL;
                    MIL.MimAllocResult(Mil.Instance.MilSystem, 256, MIL.M_HIST_LIST, ref milHisto);
                    MilImage.checkMilError("Fail to alloc Haze Histogram");
                    List<MIL_INT[]> ImgHistoList = new List<MIL_INT[]>(6);
                    for (int i = 0; i < 6; i++)
                    {
                        MilImage curImg = null;
                        if (i / 3 <= 0)
                        {
                            // 8 bits
                            curImg = img8b[i];
                        }
                        else
                        {
                            // 256 bits
                            curImg = img256b[i - 3];
                        }
                        MIL.MimHistogram(curImg, milHisto);

                        MIL_INT[] histobuffer = new MIL_INT[256];
                        MIL.MimGetResult(milHisto, MIL.M_VALUE, histobuffer);
                        ImgHistoList.Add(histobuffer);
                    }
                    MIL.MbufFree(milHisto); milHisto = MIL.M_NULL;

                    //---------------------
                    // Compute Haze 8 bits
                    //----------------------
                    hazedata.D8.HazeNbPixels[8] = 0.0f;
                    foreach (var Hazecore in inputDatabaseConfigParameters.CoreVal8)
                    {
                        hazedata.D8.HazeNbPixels[Hazecore.Index] = 0.0f;
                        int nCnt = 0;
                        foreach (var Coreval in Hazecore.Cores.values)
                        {
                            hazedata.D8.HazeNbPixels[Hazecore.Index] += (long)(ImgHistoList[HazeMeasure._R_])[Coreval] + (long)(ImgHistoList[HazeMeasure._G_])[Coreval] + (long)(ImgHistoList[HazeMeasure._B_])[Coreval];
                            nCnt++;
                        }
                        hazedata.D8.HazeNbPixels[Hazecore.Index] /= nCnt;
                        hazedata.D8.HazeNbPixels[8] += hazedata.D8.HazeNbPixels[Hazecore.Index];
                    }

                    if (hazedata.D8.HazeNbPixels[8] == 0.0f)
                        hazedata.D8.HazeNbPixels[8] = 1.0f; // evite cas particulier de division par zero

                    for (int i = 0; i < 9; i++)
                    {
                        hazedata.D8.HazeAeraPct[i] = (float)hazedata.D8.HazeNbPixels[i] / (float)hazedata.D8.HazeNbPixels[8];
                    }

                    hazedata.D8.Globalppm = 0.0f;
                    for (int i = 0; i < 8; i++)
                    {
                        hazedata.D8.Globalppm += HazeRangeFactor[i] * hazedata.D8.HazeAeraPct[i];
                    }

                    //---------------------
                    // Compute Haze 256 bits
                    //----------------------
                    hazedata.D256.HazeNbPixels[8] = 0.0f;
                    foreach (var Hazecore in inputDatabaseConfigParameters.CoreVal256)
                    {
                        foreach (var Coreval in Hazecore.Cores.values)
                        {
                            hazedata.D256.HazeNbPixels[Hazecore.Index] += (float)(ImgHistoList[Hazecore.Chan + 3])[Coreval];
                        }
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        hazedata.D256.HazeNbPixels[i] /= (float)hazedata.D256.Levels[i];
                        hazedata.D256.HazeNbPixels[8] += hazedata.D256.HazeNbPixels[i];
                    }

                    if (hazedata.D256.HazeNbPixels[8] == 0.0f)
                        hazedata.D256.HazeNbPixels[8] = 1.0f; // evite cas particulier de division par zero

                    for (int i = 0; i < 9; i++)
                    {
                        hazedata.D256.HazeAeraPct[i] = (float)hazedata.D256.HazeNbPixels[i] / (float)hazedata.D256.HazeNbPixels[8];
                    }

                    hazedata.D256.Globalppm = 0.0f;
                    for (int i = 0; i < 8; i++)
                    {
                        hazedata.D256.Globalppm += HazeRangeFactor[i] * hazedata.D256.HazeAeraPct[i];
                    }

                    ImgHistoList.Clear();

                    // ------------------
                    // Compute Thumbnails
                    //-------------------
                    int nThumbnailSize = 256;

                    MIL_ID colorimg = MIL.M_NULL;
                    MIL.MbufAllocColor(Mil.Instance.MilSystem, 3, hazedata.SizeWidth, hazedata.SizeHeight, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref colorimg);
                    MilImage.checkMilError("Fail to alloc Haze color image");

                    MIL_ID colorthumb = MIL.M_NULL;
                    MIL.MbufAllocColor(Mil.Instance.MilSystem, 3, nThumbnailSize, nThumbnailSize, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref colorthumb);
                    MilImage.checkMilError("Fail to alloc Haze color thumbnail");

                    // merge color - 8 bits
                    MIL.MbufTransfer(img8b[HazeMeasure._R_].MilId, colorimg, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_COPY, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL);
                    MIL.MbufTransfer(img8b[HazeMeasure._G_].MilId, colorimg, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 1, MIL.M_COPY, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL);
                    MIL.MbufTransfer(img8b[HazeMeasure._B_].MilId, colorimg, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 2, MIL.M_COPY, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL);

                    MIL.MimResize(colorimg, colorthumb, MIL.M_FILL_DESTINATION, MIL.M_FILL_DESTINATION, MIL.M_DEFAULT);

                    int nThumbPitch_8 = (int)MIL.MbufInquire(colorthumb, MIL.M_PITCH_BYTE);
                    hazedata.D8.ThumbSizeX = (int)MIL.MbufInquire(colorthumb, MIL.M_SIZE_X);
                    hazedata.D8.ThumbSizeY = (int)MIL.MbufInquire(colorthumb, MIL.M_SIZE_Y);
                    hazedata.D8.Thumbbail = new byte[hazedata.D8.ThumbSizeX * hazedata.D8.ThumbSizeY * 3];
                    MIL.MbufGetColor(colorthumb, MIL.M_PACKED + MIL.M_BGR24, MIL.M_ALL_BANDS, hazedata.D8.Thumbbail);

                    // merge color - 256 bits
                    MIL.MbufTransfer(img256b[HazeMeasure._R_].MilId, colorimg, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_COPY, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL);
                    MIL.MbufTransfer(img256b[HazeMeasure._G_].MilId, colorimg, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 1, MIL.M_COPY, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL);
                    MIL.MbufTransfer(img256b[HazeMeasure._B_].MilId, colorimg, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, 2, MIL.M_COPY, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL);

                    MIL.MimResize(colorimg, colorthumb, MIL.M_FILL_DESTINATION, MIL.M_FILL_DESTINATION, MIL.M_DEFAULT);

                    hazedata.D256.ThumbSizeX = (int)MIL.MbufInquire(colorthumb, MIL.M_SIZE_X);
                    hazedata.D256.ThumbSizeY = (int)MIL.MbufInquire(colorthumb, MIL.M_SIZE_Y);
                    hazedata.D256.Thumbbail = new byte[hazedata.D256.ThumbSizeX * hazedata.D256.ThumbSizeY * 3];
                    MIL.MbufGetColor(colorthumb, MIL.M_PACKED + MIL.M_BGR24, MIL.M_ALL_BANDS, hazedata.D256.Thumbbail);
#if DEBUG
                    //   MilImage.ImageJ(colorimg);
                    //    MilImage.ImageJ(colorthumb);
#endif
                    // ------------
                    // Clear memory
                    // ------------
                    MIL.MbufFree(colorthumb); colorthumb = MIL.M_NULL;
                    MIL.MbufFree(colorimg); colorimg = MIL.M_NULL;
                    for (int i = 3; i > 0; --i)
                    {
                        img256b[i - 1].Dispose();
                        img256b[i - 1] = null;
                        img8b[i - 1].Dispose();
                        img8b[i - 1] = null;
                    }

                    // -------------------------- 
                    // Send Haze data to childrens
                    // ---------------------------
                    ProcessChildren(hazedata);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                milimg_ROI.Dispose();
                milmsk_ROI.Dispose();
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void GenerateFactorHaze()
        {
            // le tableau de float RangeScaleMax doit être de size=7, la 1ere borne étant un zero implicite, la dernière étant elle même 
            HazeRangeFactor[0] = inputDatabaseConfigParameters.RangeScaleMax[0] * 0.5f;
            for (int i = 1; i < 7; i++)
            {
                HazeRangeFactor[i] = (inputDatabaseConfigParameters.RangeScaleMax[i] + inputDatabaseConfigParameters.RangeScaleMax[i - 1]) * 0.5f;
            }
            HazeRangeFactor[7] = inputDatabaseConfigParameters.RangeScaleMax[6];
        }
    }
}
