using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.BorderRemoval;

using FormatHAZE;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Data.Enum;

namespace HazeLSModule
{
    public class HazeLSMeasurementModule : ModuleBase
    {

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramMargin;


        // Histogram
        public readonly DoubleParameter paramHistoMin;
        public readonly DoubleParameter paramHistoMax;
        public readonly IntParameter paramHistoNbStep;

        // Ranges Binning
        private const int PRMBIN_NB_RANGE = 8;
        // there is PRMBIN_NB_RANGE-1 MaxRanges
        public readonly DoubleParameter paramBinMaxRange0;
        public readonly DoubleParameter paramBinMaxRange1;
        public readonly DoubleParameter paramBinMaxRange2;
        public readonly DoubleParameter paramBinMaxRange3;
        public readonly DoubleParameter paramBinMaxRange4;
        public readonly DoubleParameter paramBinMaxRange5;
        public readonly DoubleParameter paramBinMaxRange6;



        //=================================================================
        // Constructeur
        //=================================================================
        public HazeLSMeasurementModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMargin = new DoubleParameter(this, "EdgeExclusionMargin");
            paramMargin.Value = 0;

            //HistoGram
            paramHistoMin = new DoubleParameter(this, "HistoMin");
            paramHistoMin.Value = 0.0;
            paramHistoMax = new DoubleParameter(this, "HistoMax");
            paramHistoMax.Value = 10.0;
            paramHistoNbStep = new IntParameter(this, "HistoNbStep", 3, 2048);
            paramHistoNbStep.Value = 1024;

            // Ranges Binning
            float fStep = ((float)paramHistoMax.Value - (float)paramHistoMin.Value) / (float)PRMBIN_NB_RANGE;
            if (fStep == 0.0f)
                fStep = 0.001f;
            int i = 1;
            paramBinMaxRange0 = new DoubleParameter(this, "MaxRange0");
            paramBinMaxRange0.Value = (float)paramHistoMin.Value + fStep * i++;
            paramBinMaxRange1 = new DoubleParameter(this, "MaxRange1");
            paramBinMaxRange1.Value = (float)paramHistoMin.Value + fStep * i++;
            paramBinMaxRange2 = new DoubleParameter(this, "MaxRange2");
            paramBinMaxRange2.Value = (float)paramHistoMin.Value + fStep * i++;
            paramBinMaxRange3 = new DoubleParameter(this, "MaxRange3");
            paramBinMaxRange3.Value = (float)paramHistoMin.Value + fStep * i++;
            paramBinMaxRange4 = new DoubleParameter(this, "MaxRange4");
            paramBinMaxRange4.Value = (float)paramHistoMin.Value + fStep * i++;
            paramBinMaxRange5 = new DoubleParameter(this, "MaxRange5");
            paramBinMaxRange5.Value = (float)paramHistoMin.Value + fStep * i++;
            paramBinMaxRange6 = new DoubleParameter(this, "MaxRange6");
            paramBinMaxRange6.Value = (float)paramHistoMin.Value + fStep * i++;

        }

        protected override void OnInit()
        {
            base.OnInit();
        }


        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            if (paramHistoMin.Value >= paramHistoMax.Value)
                return "Histo Min should strictly inferior to Histo Max";

            if (paramBinMaxRange0.Value >= paramBinMaxRange1.Value)
                return "Range 0 should strictly inferior to the next Ranges";
            if (paramBinMaxRange1.Value >= paramBinMaxRange2.Value)
                return "Range 1 should strictly inferior to the next Ranges";
            if (paramBinMaxRange2.Value >= paramBinMaxRange3.Value)
                return "Range 2 should strictly inferior to the next Ranges";
            if (paramBinMaxRange3.Value >= paramBinMaxRange4.Value)
                return "Range 3 should strictly inferior to the next Ranges";
            if (paramBinMaxRange4.Value >= paramBinMaxRange5.Value)
                return "Range 4 should strictly inferior to the next Ranges";
            if (paramBinMaxRange5.Value >= paramBinMaxRange6.Value)
                return "Range 5 should strictly inferior to the next Ranges";
            if (paramBinMaxRange6.Value <= paramBinMaxRange5.Value)
                return "Range 6 should strictly superior to the previous Ranges";
            return null;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;

            try
            {
                ImageLayerBase imgLayer = (ImageLayerBase)image.Layer;
                if (imgLayer.ResultType.GetActorType() != ActorType.LIGHTSPEED)
                {
                    throw new ApplicationException("Bad Layer actor (expecting : Lightspeed) : " + imgLayer.ResultType.GetActorType());
                }

                // rti need to use tjose moduel as helios module , lightspeed should not be integrated in UPS for the moment

                var restyp = imgLayer.ResultType;
                if (!(restyp == ResultType.HLS_Haze_WideFW || restyp == ResultType.HLS_Haze_NarrowBW || restyp == ResultType.HLS_Haze_Total))
                {
                    throw new ApplicationException($"Wrong Result Type (expecting : Haze_Forward OR Haze_Backward OR Haze_Tot) : {restyp}");
                }

                if (!double.TryParse(imgLayer.MetaData[AcquisitionAdcExchange.LayerMetaData.hazeMin_ppmCal], out double adaHazeMinppm))
                {
                    adaHazeMinppm = 0.0;
                    logWarning($"hazeMin_ppmCal not found in layer - use default value = <{adaHazeMinppm}>");
                }

                if (!double.TryParse(imgLayer.MetaData[AcquisitionAdcExchange.LayerMetaData.hazeMax_ppmCal], out double adaHazeMaxppm))
                {
                    adaHazeMaxppm = 65535.0;
                    logWarning($"hazeMax_ppmCal not found in layer - use default value = <{adaHazeMaxppm}>");
                }


                System.Drawing.Rectangle WaferRoiRect = System.Drawing.Rectangle.Empty;

                float lfPixelSizeX_um = 1.0f;
                float lfPixelSizeY_um = 1.0f;
                if (imgLayer.Matrix is RectangularMatrix)
                {
                    RectangularMatrix mat = imgLayer.Matrix as RectangularMatrix;
                    lfPixelSizeX_um = mat.PixelWidth;
                    lfPixelSizeY_um = mat.PixelHeight;
                    // on doit avoir ici lfPixelSizeX_um==lfPixelSizeY_um (pixel size carre)
                }

                MilImage imgIN_ppm = image.ResultProcessingImage.GetMilImage();

                //---------------------------------
                // edge remove pour obtenir le mask
                //---------------------------------

                using (ImageBase mskimage = (ImageBase)image.DeepClone())
                {
                    MilImage milmsk = mskimage.ResultProcessingImage.GetMilImage();
                    // on passe en 8 bits
                    MIL.MbufFree(milmsk.DetachMilId());
                    milmsk.Alloc2d(imgIN_ppm.SizeX, imgIN_ppm.SizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                    MIL.MimBinarize(imgIN_ppm.MilId, milmsk.MilId, MIL.M_IN_RANGE, 0, float.MaxValue); // on s'affranchi des négatif et des NaNs

                    EdgeRemoveAlgorithm edgeRemoveAlgo = new EdgeRemoveAlgorithm();
                    edgeRemoveAlgo.Margin = paramMargin.Value;
                    edgeRemoveAlgo.PerformRemoval(mskimage);

                    milmsk = mskimage.ResultProcessingImage.GetMilImage();
                    // MIL.MimBinarize(milmsk.MilId, milmsk.MilId, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL);
                    milmsk.Arith(255.0, MIL.M_DIV_CONST);

                    // On applique le mask à l'image
                    // MilImage.Arith(imgIN_ppm, milmsk, imgIN_ppm, MIL.M_MULT);
                    imgIN_ppm.Arith(milmsk.MilId, MIL.M_MULT);
                }

                //-------------------
                // Init HAZE Measure
                //-------------------
                using (HazeLSMeasure hazeMeasure = new HazeLSMeasure())
                {
                    hazeMeasure.EdgeExlusion_um = (int)Math.Round(paramMargin.Value);
                    hazeMeasure.HazeMin_ppmCal = adaHazeMinppm;
                    hazeMeasure.HazeMax_ppmCal = adaHazeMaxppm;

                    LSHazeData dt = new LSHazeData();

                    dt.nId = (int)restyp; // channel id 
                    dt.fPixelSize_um = lfPixelSizeX_um;
                    dt.nWidth = image.Width;
                    dt.nHeigth = image.Height;

                    using (var StatResult = new MilImageResult())
                    {
                        StatResult.AllocResult(imgIN_ppm.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                        StatResult.Stat(imgIN_ppm, MilTo.StatList(MIL.M_STAT_MIN, MIL.M_STAT_MAX, MIL.M_STAT_MEAN, MIL.M_STAT_STANDARD_DEVIATION), MIL.M_NOT_EQUAL, 0.0);

                        //stats
                        dt.min_ppm = (float)StatResult.GetResult(MIL.M_STAT_MIN);
                        dt.max_ppm = (float)StatResult.GetResult(MIL.M_STAT_MAX);
                        dt.mean_ppm = (float)StatResult.GetResult(MIL.M_STAT_MEAN);
                        dt.stddev_ppm = (float)StatResult.GetResult(MIL.M_STAT_STANDARD_DEVIATION);
                    }

                    float[] buffer = new float[dt.nWidth * dt.nHeigth];
                    imgIN_ppm.Get(buffer);
                    dt.HazeMeasures = buffer;

                    List<float> values = new List<float>(buffer.Length);
                    foreach (float fval in buffer)
                    {
                        // on skip les NaN
                        if (float.IsNaN(fval) == false)
                        {
                            // On skip les zeros purs
                            if (fval != 0.0f)
                            {
                                values.Add(fval);
                            }
                        }
                    }
                    values.Sort();

                    double dPtsUsed = (double)values.Count();
                    int nMid = (int)(dPtsUsed / 2.0f);
                    if (dPtsUsed % 2.0f == 0.0f)
                    {
                        // le nb d'elt est pair on prends la moyenne des 2 valeurs du milieu  
                        float a = values[nMid - 1];
                        float b = values[nMid];
                        dt.median_ppm = (a + b) / 2.0f;
                    }
                    else
                    {
                        //le nb d'elt est impair on prends la valeur du milieu 
                        dt.median_ppm = values[nMid];
                    }

                    // On recupere les parametres Histo
                    dt.HistLimitMax = (float)paramHistoMax.Value;
                    dt.HistLimitMin = (float)paramHistoMin.Value;
                    dt.HistNbStep = (float)paramHistoNbStep.Value;

                    float[] BinMaxRanges = new float[PRMBIN_NB_RANGE - 1];
                    BinMaxRanges[0] = (float)paramBinMaxRange0.Value;
                    BinMaxRanges[1] = (float)paramBinMaxRange1.Value;
                    BinMaxRanges[2] = (float)paramBinMaxRange2.Value;
                    BinMaxRanges[3] = (float)paramBinMaxRange3.Value;
                    BinMaxRanges[4] = (float)paramBinMaxRange4.Value;
                    BinMaxRanges[5] = (float)paramBinMaxRange5.Value;
                    BinMaxRanges[6] = (float)paramBinMaxRange6.Value;


                    // Compute range Bining && Histogram
                    if (values.Count > 0)
                    {
                        // Hist internal var
                        float stepval = (dt.HistLimitMax - dt.HistLimitMin) / (dt.HistNbStep - 1);
                        float stepval_HistoCalc = (dt.HistLimitMax - dt.HistLimitMin) / (dt.HistNbStep - 2);
                        int curclassid = 0;
                        int maxid = (int)dt.HistNbStep;
                        float curclassval = dt.HistLimitMin;
                        UInt32[] barclass = new UInt32[(int)dt.HistNbStep];

                        // Bin internal var
                        int BINcurclassid = 0;
                        int BINmaxid = (int)PRMBIN_NB_RANGE;
                        int BINmaxRangeid = BinMaxRanges.Length;
                        float BINcurclassval = BinMaxRanges[0];
                        ulong[] binclass = new ulong[PRMBIN_NB_RANGE];
                        ulong uBINTotal = 0;
                        bool LastRangeReached = false;

                        foreach (float val in values)
                        {
                            // Histo
                            while ((val > curclassval) && (curclassid < maxid - 1))
                            {
                                curclassval += stepval_HistoCalc;
                                curclassid++;
                            }
                            barclass[curclassid]++;

                            // Bin
                            while (!LastRangeReached && (val > BINcurclassval) && (BINcurclassid < BINmaxid - 1))
                            {
                                BINcurclassid++;
                                if (BINcurclassid < BINmaxRangeid)
                                    BINcurclassval = BinMaxRanges[BINcurclassid];
                                else
                                    LastRangeReached = true;
                            }
                            binclass[BINcurclassid]++;
                            uBINTotal++;
                        }

                        dt.Ranges = new List<LSHazeRange>(binclass.Length);
                        for (int k = 0; k < binclass.Length; k++)
                        {
                            LSHazeRange rg = new LSHazeRange();
                            rg.nrank = k;
                            rg.min_ppm = (k > 0) ? BinMaxRanges[k - 1] : float.NegativeInfinity;
                            rg.max_ppm = (k < binclass.Length - 1) ? BinMaxRanges[k] : float.PositiveInfinity;
                            rg.area_pct = 100.0f * (float)((double)binclass[k] / (double)uBINTotal);
                            rg.nbCount = binclass[k];

                            dt.Ranges.Add(rg);
                        }

                        dt.HistMaxYBar = barclass.Max();
                        dt.Histo = barclass;
                    }
                    else
                    {
                        dt.Ranges = new List<LSHazeRange>(PRMBIN_NB_RANGE);
                        for (int k = 0; k < PRMBIN_NB_RANGE; k++)
                        {
                            LSHazeRange rg = new LSHazeRange();
                            rg.nrank = k;
                            rg.min_ppm = (k > 0) ? BinMaxRanges[k - 1] : float.NegativeInfinity;
                            rg.max_ppm = (k < PRMBIN_NB_RANGE - 1) ? BinMaxRanges[k] : float.PositiveInfinity;
                            rg.area_pct = 0.0f;
                            rg.nbCount = (ulong)0;

                            dt.Ranges.Add(rg);
                        }

                        dt.HistMaxYBar = 0;
                        dt.Histo = null;
                    }

                    hazeMeasure.Data = dt;

                    // -------------------------- 
                    // Send Haze data to childrens
                    // ---------------------------
                    ProcessChildren(hazeMeasure);
                }

            }
            catch
            {
                throw;
            }
            finally
            {

            }
        }
    }
}
