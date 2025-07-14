using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
//using System.Windows.Controls;
using AdcBasicObjects;

using ADCEngine;
using ADCEngine.Parameters;

using BasicModules;
using BasicModules.Sizing;

using UnitySC.Shared.LibMIL;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class DiameterMeasurementAnalysisModule : ModuleBase, IClassifierModule
    {

        public static readonly String _LblgMeasureMissing = "Missing Measure";
        public static readonly String _LblMeasureBadDiameter = "Diameter Measure out of tolerance";
        public static readonly String _LblMeasureBadOffset = "Offset Measure out of tolerance";
        public static readonly String _LblDieBadAvgDiameter = "Die Average Diameter out of tolerance";

        private List<string> _Die2ddmDefectClassList = new List<string>();
        public List<string> DefectClassLabelList
        {
            get
            {
                return _Die2ddmDefectClassList;
            }
        }


        //=================================================================
        // Paramètres du XML
        //=================================================================
        // Bumps Metrics
        public readonly SeparatorParameter paramSeparatorBump;
        public readonly DoubleParameter paramMeasureDiameterPassRangeMin;
        public readonly DoubleParameter paramMeasureDiameterPassRangeMax;
        public readonly DoubleParameter paramMeasureOffsetPassRangeMin;
        public readonly DoubleParameter paramMeasureOffsetPassRangeMax;

        // Die Metrics
        public readonly SeparatorParameter paramSeparatorDie;
        public readonly DoubleParameter paramAverageDieDiameterPassRangeMin;
        public readonly DoubleParameter paramAverageDieDiameterPassRangeMax;

        // Kiil Metrics
        public readonly SeparatorParameter paramSeparatorYield;
        public readonly IntParameter paramNbKillMissingMeasure;
        public readonly IntParameter paramNbKillBadDiametertMeasure;
        public readonly IntParameter paramNbKillBadOffsettMeasure;


        //=================================================================
        // Constructeur
        //=================================================================
        public DiameterMeasurementAnalysisModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ModuleProperty = eModuleProperty.Stage;

            paramSeparatorBump = new SeparatorParameter(this, "Measures");

            paramMeasureDiameterPassRangeMin = new DoubleParameter(this, "MeasureDiameterPassRangeMin");
            paramMeasureDiameterPassRangeMin.Value = 0.0;
            paramMeasureDiameterPassRangeMax = new DoubleParameter(this, "MeasureDiameterPassRangeMax");
            paramMeasureDiameterPassRangeMax.Value = 1000.0;

            paramMeasureOffsetPassRangeMin = new DoubleParameter(this, "MeasureOffsetPassRangeMin");
            paramMeasureOffsetPassRangeMin.Value = 0;
            paramMeasureOffsetPassRangeMax = new DoubleParameter(this, "MeasureOffsetPassRangeMax");
            paramMeasureOffsetPassRangeMax.Value = 1000.0;

            paramSeparatorDie = new SeparatorParameter(this, "Die");

            paramAverageDieDiameterPassRangeMin = new DoubleParameter(this, "AverageDieDiameterPassRangeMin");
            paramAverageDieDiameterPassRangeMin.Value = 0;
            paramAverageDieDiameterPassRangeMax = new DoubleParameter(this, "AverageDieDiameterPassRangeMax");
            paramAverageDieDiameterPassRangeMax.Value = 1000.0;

            paramSeparatorYield = new SeparatorParameter(this, "Yield");

            paramNbKillMissingMeasure = new IntParameter(this, "NbKillMissingMeasure");
            paramNbKillBadDiametertMeasure = new IntParameter(this, "NbKillBadDiameterMeasure");
            paramNbKillBadOffsettMeasure = new IntParameter(this, "NbKillBadOffsetMeasure");
        }


        private void UpdateParams(DiameterMeasurementDieModule DirectAncestor)
        {
            if (DirectAncestor == null)
                return;

            _Die2ddmDefectClassList.Clear();
            // Missing Measure
            _Die2ddmDefectClassList.Add(_LblgMeasureMissing);
            // Bad Diameter Measure
            _Die2ddmDefectClassList.Add(_LblMeasureBadDiameter);
            // Bad Offset Measure
            _Die2ddmDefectClassList.Add(_LblMeasureBadOffset);
            // Die average Diameter
            _Die2ddmDefectClassList.Add(_LblDieBadAvgDiameter);

        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;


            // Find ancestor of die height measurement
            List<ModuleBase> AncestorHMmodule = FindAncestors(mod => mod is DiameterMeasurementDieModule);
            if (AncestorHMmodule.Count == 0)
                return "No Diameter measurement die module has been set above this module";

            DiameterMeasurementDieModule DirectAncestor = AncestorHMmodule[0] as DiameterMeasurementDieModule;
            // check if data is coorectly computed by ancestor
            UpdateParams(DirectAncestor);

            if (paramMeasureDiameterPassRangeMin.Value > paramMeasureDiameterPassRangeMax.Value)
                return "Measure Diameter Pass Range Min should be inferior to Max";

            if (paramMeasureOffsetPassRangeMin.Value > paramMeasureOffsetPassRangeMax.Value)
                return "Measure Offset Pass Range Min should be inferior to Max";

            if (paramAverageDieDiameterPassRangeMin.Value > paramAverageDieDiameterPassRangeMax.Value)
                return "Average Die Height Passs Range Min should be inferior to Max";

            return null;
        }


        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
        }


        //=================================================================
        // 
        //=================================================================

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            if (State != eModuleState.Aborting)
            {
                logDebug("process " + obj);
                Interlocked.Increment(ref nbObjectsIn);

                if (!(obj is Cluster2DDieDM))
                {
                    throw new ApplicationException("Object received is not a Cluster 2D Die DM");
                }

                Cluster2DDieDM cluster2DDie = (Cluster2DDieDM)obj;
                MilImage dieImageProcessing = cluster2DDie.CurrentProcessingImage.GetMilImage();
                int nImgType = dieImageProcessing.Type;
                int nImgAttrib = dieImageProcessing.Attribute;

                int nDieIndexX = cluster2DDie.DieIndexX;
                int nDieIndexY = cluster2DDie.DieIndexY;
                Point DieOffsetImage = cluster2DDie.DieOffsetImage;

                int nBbBlobMeasures = cluster2DDie.blobList.Count;
                int ClusterDieIndex = cluster2DDie.Index;

                double dSumDieDiameter = 0.0;
                double dSumDieOffset = 0.0;

                uint NbMeasurePass = 0;
                uint NbMeasureFailMissing = 0;
                uint NbMeasureFailBadDiameter = 0;
                uint NbMeasureFailBadOffset = 0;

                bool bDieAvgDiameterFail = false;

                // On Check  les données du cluster die
                if (cluster2DDie.characteristics.ContainsKey(Cluster2DCharacteristics.DiameterAverage))
                {
                    double dAvgDieDiameterum = (double)cluster2DDie.characteristics[Cluster2DCharacteristics.DiameterAverage];
                    bDieAvgDiameterFail = dAvgDieDiameterum < paramAverageDieDiameterPassRangeMin.Value || dAvgDieDiameterum > paramAverageDieDiameterPassRangeMax.Value;

                    if (cluster2DDie.characteristics.ContainsKey(Cluster2DCharacteristics.isDiameterFailure))
                    {
                        cluster2DDie.characteristics[Cluster2DCharacteristics.isDiameterFailure] = bDieAvgDiameterFail;
                    }
                    else
                    {
                        cluster2DDie.characteristics.Add(Cluster2DCharacteristics.isDiameterFailure, bDieAvgDiameterFail);
                    }

                    if (bDieAvgDiameterFail)
                        cluster2DDie.defectClassList.Insert(0, _LblDieBadAvgDiameter);
                }

                double MissingFailureType = 1.0;  //"Missing";
                double BadDiameterFailureType = 2.0; // "BadDiameter";
                double BadOffsetFailureType = 3.0; // "BadOffset";

                double dMaxDiameter = double.MinValue;
                double dMinDiameter = double.MaxValue;
                double dMaxOffset = double.MinValue;
                double dMinOffset = double.MaxValue;

                List<double> StatsdevDiameter = new List<double>(cluster2DDie.blobList.Count);
                List<double> StatsdevOffset = new List<double>(cluster2DDie.blobList.Count);

                foreach (Blob blobmeasure in cluster2DDie.blobList)
                {
                    if (State == eModuleState.Aborting)
                    {
                        break;
                    }

                    if (blobmeasure.characteristics.ContainsKey(Blob2DCharacteristics.isMissing) && (double)blobmeasure.characteristics[Blob2DCharacteristics.isMissing] != 0.0)
                    {
                        NbMeasureFailMissing++;
                        blobmeasure.characteristics.Add(Blob2DCharacteristics.FailureType, MissingFailureType);
                    }
                    else
                    {
                        double dDiameterum = (double)blobmeasure.characteristics[Blob2DCharacteristics.Diameter];
                        dSumDieDiameter += dDiameterum;
                        dMaxDiameter = Math.Max(dDiameterum, dMaxDiameter);
                        dMinDiameter = Math.Min(dDiameterum, dMinDiameter);
                        StatsdevDiameter.Add(dDiameterum);

                        double dOffsetum = (double)blobmeasure.characteristics[Blob2DCharacteristics.OffsetPos];
                        dSumDieOffset += dOffsetum;
                        dMaxOffset = Math.Max(dOffsetum, dMaxOffset);
                        dMinOffset = Math.Min(dOffsetum, dMinOffset);
                        StatsdevOffset.Add(dOffsetum);


                        if (dDiameterum < paramMeasureDiameterPassRangeMin.Value || dDiameterum > paramMeasureDiameterPassRangeMax.Value)
                        {
                            NbMeasureFailBadDiameter++;
                            blobmeasure.characteristics.Add(Blob2DCharacteristics.FailureType, BadDiameterFailureType);
                        }
                        else if (dOffsetum < paramMeasureOffsetPassRangeMin.Value || dOffsetum > paramMeasureOffsetPassRangeMax.Value)
                        {
                            NbMeasureFailBadOffset++;
                            blobmeasure.characteristics.Add(Blob2DCharacteristics.FailureType, BadOffsetFailureType);
                        }
                        else
                        {
                            NbMeasurePass++;
                            blobmeasure.characteristics.Add(Blob2DCharacteristics.FailureType, 0.0);
                        }
                    }
                }

                if (State != eModuleState.Aborting)
                {
                    cluster2DDie._NbMeasuresPass = NbMeasurePass;
                    cluster2DDie._NbMeasuresMissing = NbMeasureFailMissing;
                    cluster2DDie._NbMeasuresBadDiameter = NbMeasureFailBadDiameter;
                    cluster2DDie._NbMeasuresBadOffset = NbMeasureFailBadOffset;

                    cluster2DDie._SumMeasuresDiameter = dSumDieDiameter;
                    cluster2DDie._SumMeasuresOffset = dSumDieOffset;

                    double dNbBumpInspected = (double)(NbMeasurePass + NbMeasureFailBadDiameter + NbMeasureFailBadOffset);
                    // Debug.assert(NbBumpInspected== StatsdevDiameter.count)
                    cluster2DDie._MinMeasureDiameter = dMinDiameter;
                    cluster2DDie._MaxMeasureDiameter = dMaxDiameter;
                    cluster2DDie._MeanMeasureDiameter = dSumDieDiameter / dNbBumpInspected;
                    cluster2DDie._StdDevMeasureDiameter = StatsdevDiameter.Sum(mes => Math.Pow(mes - cluster2DDie._MeanMeasureDiameter, 2));
                    cluster2DDie._StdDevMeasureDiameter /= dNbBumpInspected;
                    cluster2DDie._StdDevMeasureDiameter = Math.Sqrt(cluster2DDie._StdDevMeasureDiameter); ;

                    cluster2DDie._MinMeasureOffset = dMinOffset;
                    cluster2DDie._MaxMeasureOffset = dMaxOffset;
                    cluster2DDie._MeanMeasureOffset = dSumDieOffset / dNbBumpInspected;
                    cluster2DDie._StdDevMeasureOffset = StatsdevOffset.Sum(mes => Math.Pow(mes - cluster2DDie._MeanMeasureOffset, 2));
                    cluster2DDie._StdDevMeasureOffset /= dNbBumpInspected;
                    cluster2DDie._StdDevMeasureOffset = Math.Sqrt(cluster2DDie._StdDevMeasureOffset); ;

                    cluster2DDie._StatsDiameter = StatsdevDiameter;
                    cluster2DDie._StatsOffset = StatsdevOffset;

                    cluster2DDie.characteristics[SizingCharacteristics.DefectMaxSize] = cluster2DDie._MaxMeasureDiameter;
                    cluster2DDie.characteristics[SizingCharacteristics.TotalDefectSize] = cluster2DDie._MeanMeasureDiameter;
                    cluster2DDie.characteristics[SizingCharacteristics.SizingType] = eSizingType.ByDiameter;


                    bool isMeasureMissingFailure = (NbMeasureFailMissing > paramNbKillMissingMeasure.Value);
                    if (cluster2DDie.characteristics.ContainsKey(Cluster2DCharacteristics.isMeasureMissingFailure))
                    {
                        cluster2DDie.characteristics[Cluster2DCharacteristics.isMeasureMissingFailure] = isMeasureMissingFailure;
                    }
                    else
                    {
                        cluster2DDie.characteristics.Add(Cluster2DCharacteristics.isMeasureMissingFailure, isMeasureMissingFailure);
                    }
                    if (isMeasureMissingFailure)
                        cluster2DDie.defectClassList.Add(_LblgMeasureMissing);

                    bool isMeasureDiameterFailure = (NbMeasureFailBadDiameter > paramNbKillBadDiametertMeasure.Value);
                    if (cluster2DDie.characteristics.ContainsKey(Cluster2DCharacteristics.isMeasureDiameterFailure))
                    {
                        cluster2DDie.characteristics[Cluster2DCharacteristics.isMeasureDiameterFailure] = isMeasureDiameterFailure;
                    }
                    else
                    {
                        cluster2DDie.characteristics.Add(Cluster2DCharacteristics.isMeasureDiameterFailure, isMeasureDiameterFailure);
                    }
                    if (isMeasureDiameterFailure)
                        cluster2DDie.defectClassList.Add(_LblMeasureBadDiameter);

                    bool isMeasureOffsetFailure = (NbMeasureFailBadOffset > paramNbKillBadOffsettMeasure.Value);
                    if (cluster2DDie.characteristics.ContainsKey(Cluster2DCharacteristics.isMeasureOffsetFailure))
                    {
                        cluster2DDie.characteristics[Cluster2DCharacteristics.isMeasureOffsetFailure] = isMeasureOffsetFailure;
                    }
                    else
                    {
                        cluster2DDie.characteristics.Add(Cluster2DCharacteristics.isMeasureOffsetFailure, isMeasureOffsetFailure);
                    }
                    if (isMeasureOffsetFailure)
                        cluster2DDie.defectClassList.Add(_LblMeasureBadOffset);


                    bool bDieFailure = isMeasureMissingFailure || isMeasureDiameterFailure || isMeasureOffsetFailure || bDieAvgDiameterFail;
                    if (cluster2DDie.characteristics.ContainsKey(Cluster2DCharacteristics.isFailure))
                    {
                        cluster2DDie.characteristics[Cluster2DCharacteristics.isFailure] = bDieFailure;
                    }
                    else
                    {
                        cluster2DDie.characteristics.Add(Cluster2DCharacteristics.isFailure, bDieFailure);
                    }
                }

                ProcessChildren(obj);

            }
        }
    }
}
