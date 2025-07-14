using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
//using System.Windows.Controls;
using AdcBasicObjects;

using ADCEngine;
using ADCEngine.Parameters;

using BasicModules;

using UnitySC.Shared.LibMIL;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class HeightMeasurementAnalysisModule : ModuleBase, IClassifierModule
    {
        public static readonly String _LblMeasureMissing = "Missing Measure";
        public static readonly String _LblMeasureBadHeight = "Height Measure out of tolerance";
        public static readonly String _LblDieBadAvgHeight = "Die Average Height out of tolerance";
        public static readonly String _LblDieBadCoplanarity = "Die Coplanarity out of tolerance";
        public static readonly String _LblDieBadSubstrateCopla = "Die Substrate Coplanarity out of tolerance";

        private List<string> _Die3dHmDefectClassList = new List<string>();
        public List<string> DefectClassLabelList
        {
            get
            {
                return _Die3dHmDefectClassList;
            }
        }


        //=================================================================
        // Paramètres du XML
        //=================================================================
        // Bumps Metrics
        public readonly SeparatorParameter paramSeparatorBump;
        public readonly BoolParameter paramIsMeasuringCavity;
        public readonly DoubleParameter paramMeasureMissingLimit;
        public readonly DoubleParameter paramMeasureHeightPassRangeMin;
        public readonly DoubleParameter paramMeasureHeightPassRangeMax;
        // Die Metrics
        public readonly SeparatorParameter paramSeparatorDie;
        public readonly DoubleParameter paramAverageDieHeightPassRangeMin;
        public readonly DoubleParameter paramAverageDieHeightPassRangeMax;
        public readonly DoubleParameter paramDieCoplanarityPassRangeMin;
        public readonly DoubleParameter paramDieCoplanarityPassRangeMax;
        public readonly DoubleParameter paramDieSubstrateCoplanarityPassRangeMin;
        public readonly DoubleParameter paramDieSubstrateCoplanarityPassRangeMax;
        // Kil Metrics
        public readonly SeparatorParameter paramSeparatorYield;
        public readonly IntParameter paramNbKillMissingMeasure;
        public readonly IntParameter paramNbKillBadHeightMeasure;


        //=================================================================
        // Constructeur
        //=================================================================
        public HeightMeasurementAnalysisModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ModuleProperty = eModuleProperty.Stage;

            paramSeparatorBump = new SeparatorParameter(this, "Measures");

            paramIsMeasuringCavity = new BoolParameter(this, "IsMeasuringCavity");
            paramIsMeasuringCavity.Value = false;

            paramMeasureMissingLimit = new DoubleParameter(this, "MeasureMissingLimit");
            paramMeasureMissingLimit.Value = 1;

            paramMeasureHeightPassRangeMin = new DoubleParameter(this, "MeasureHeightPassRangeMin");
            paramMeasureHeightPassRangeMin.Value = 0;
            paramMeasureHeightPassRangeMax = new DoubleParameter(this, "MeasureHeightPassRangeMax");
            paramMeasureHeightPassRangeMax.Value = 1;

            paramSeparatorDie = new SeparatorParameter(this, "Die");

            paramAverageDieHeightPassRangeMin = new DoubleParameter(this, "AverageDieHeightPassRangeMin");
            paramAverageDieHeightPassRangeMin.Value = 0;
            paramAverageDieHeightPassRangeMax = new DoubleParameter(this, "AverageDieHeightPassRangeMax");
            paramAverageDieHeightPassRangeMax.Value = 1;

            paramDieCoplanarityPassRangeMin = new DoubleParameter(this, "DieCoplanarityPassRangeMin");
            paramDieCoplanarityPassRangeMin.Value = 0;
            paramDieCoplanarityPassRangeMax = new DoubleParameter(this, "DieCoplanarityPassRangeMax");
            paramDieCoplanarityPassRangeMax.Value = 1;

            paramDieSubstrateCoplanarityPassRangeMin = new DoubleParameter(this, "DieSubstrateCoplanarityPassRangeMin");
            paramDieSubstrateCoplanarityPassRangeMin.Value = 0;
            paramDieSubstrateCoplanarityPassRangeMax = new DoubleParameter(this, "DieSubstrateCoplanarityPassRangeMax");
            paramDieSubstrateCoplanarityPassRangeMax.Value = 1;

            paramSeparatorYield = new SeparatorParameter(this, "Yield");

            paramNbKillMissingMeasure = new IntParameter(this, "NbKillMissingMeasure");

            paramNbKillBadHeightMeasure = new IntParameter(this, "NbKillBadHeightMeasure");

        }


        private void UpdateParams(HeightMeasurementDieModule DirectAncestor)
        {
            if (DirectAncestor == null)
                return;

            _Die3dHmDefectClassList.Clear();
            // Missing Measure
            _Die3dHmDefectClassList.Add(_LblMeasureMissing);
            // Bad Height Measure
            _Die3dHmDefectClassList.Add(_LblMeasureBadHeight);

            bool bMapHeight = DirectAncestor.paramUseHeightMap;
            bool bMapCopla = DirectAncestor.paramUseCoplanarityMap;
            bool bMapSubCopla = DirectAncestor.paramUseSubstrateCoplanarity;

            paramAverageDieHeightPassRangeMin.IsEnabled = bMapHeight;
            paramAverageDieHeightPassRangeMax.IsEnabled = bMapHeight;
            // Die Average Height
            if (bMapHeight)
                _Die3dHmDefectClassList.Add(_LblDieBadAvgHeight);

            paramDieCoplanarityPassRangeMin.IsEnabled = bMapCopla;
            paramDieCoplanarityPassRangeMax.IsEnabled = bMapCopla;
            // Die Coplanarity
            if (bMapCopla)
                _Die3dHmDefectClassList.Add(_LblDieBadCoplanarity);

            paramDieSubstrateCoplanarityPassRangeMin.IsEnabled = bMapSubCopla;
            paramDieSubstrateCoplanarityPassRangeMax.IsEnabled = bMapSubCopla;

            // Die Substrate Coplanarity
            if (bMapSubCopla)
                _Die3dHmDefectClassList.Add(_LblDieBadSubstrateCopla);

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
            List<ModuleBase> AncestorHMmodule = FindAncestors(mod => mod is HeightMeasurementDieModule);
            if (AncestorHMmodule.Count == 0)
                return "No Height measurement die module has been set above this module";

            HeightMeasurementDieModule DirectAncestor = AncestorHMmodule[0] as HeightMeasurementDieModule;
            // check if data is coorectly computed by ancestor
            UpdateParams(DirectAncestor);


            if (Math.Abs(paramMeasureHeightPassRangeMin.Value) > Math.Abs(paramMeasureHeightPassRangeMax.Value))
                return "Measure Pass Range Min should be inferior to Max";

            if (!paramIsMeasuringCavity.Value)
            {
                if (paramMeasureMissingLimit.Value > paramMeasureHeightPassRangeMin.Value)
                    return "Measure Missing Limit Range Min should be inferior to Measure Pass Range Min";
            }
            else
            {
                if (Math.Abs(paramMeasureMissingLimit.Value) > Math.Abs(paramMeasureHeightPassRangeMax.Value))
                    return "Measure Missing Limit Range Min should be superior to Measure Pass Range Max (Cavity)";
            }

            if (Math.Abs(paramAverageDieHeightPassRangeMin.Value) > Math.Abs(paramAverageDieHeightPassRangeMax.Value))
                return "Average Die Height Passs Range Min should be inferior to Max";

            if (paramDieCoplanarityPassRangeMin.Value > paramDieCoplanarityPassRangeMax.Value)
                return "Die Coplanarity Passs Range Min should be inferior to Max";

            if (paramDieSubstrateCoplanarityPassRangeMin.Value > paramDieSubstrateCoplanarityPassRangeMax.Value)
                return "Die Substrate Coplanarity Passs Range Min should be inferior to Max";




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

                if (!(obj is Cluster3DDieHM))
                {
                    throw new ApplicationException("Object received is not a Cluster 3D Die HM");
                }

                Cluster3DDieHM cluster3DDie = (Cluster3DDieHM)obj;
                MilImage dieImageProcessing = cluster3DDie.CurrentProcessingImage.GetMilImage();
                int nImgType = dieImageProcessing.Type;
                int nImgAttrib = dieImageProcessing.Attribute;

                int nDieIndexX = cluster3DDie.DieIndexX;
                int nDieIndexY = cluster3DDie.DieIndexY;
                Point DieOffsetImage = cluster3DDie.DieOffsetImage;

                int nBbBlobMeasures = cluster3DDie.blobList.Count;
                int ClusterDieIndex = cluster3DDie.Index;

                bool bDieAvgHeightFail = false;
                bool bDieCoplaFail = false;
                bool bDieSubCoplaFail = false;
                double dSumDieHeights = 0.0;
                uint NbMeasurePass = 0;
                uint NbMeasureFailMissing = 0;
                uint NbMeasureFailBadHeight = 0;


                // On Check  les données du cluster die
                if (cluster3DDie.characteristics.ContainsKey(Cluster3DCharacteristics.HeightAverage))
                {
                    // Map Height is enable
                    double dAvgDieHeightum = (double)cluster3DDie.characteristics[Cluster3DCharacteristics.HeightAverage];
                    bDieAvgHeightFail = dAvgDieHeightum < paramAverageDieHeightPassRangeMin.Value || dAvgDieHeightum > paramAverageDieHeightPassRangeMax.Value;

                    cluster3DDie.characteristics[Cluster3DCharacteristics.isHeightFailure] = bDieAvgHeightFail;
                    if (bDieAvgHeightFail)
                        cluster3DDie.defectClassList.Insert(0, _LblDieBadAvgHeight);
                }

                if (cluster3DDie.characteristics.ContainsKey(Cluster3DCharacteristics.Coplanarity))
                {
                    // Map Copla is enable
                    double dDieCopla = (double)cluster3DDie.characteristics[Cluster3DCharacteristics.Coplanarity];
                    bDieCoplaFail = dDieCopla < paramDieCoplanarityPassRangeMin.Value || dDieCopla > paramDieCoplanarityPassRangeMax.Value;

                    cluster3DDie.characteristics[Cluster3DCharacteristics.isCoplaFailure] = bDieCoplaFail;
                    if (bDieCoplaFail)
                        cluster3DDie.defectClassList.Add(_LblDieBadCoplanarity);
                }

                if (cluster3DDie.characteristics.ContainsKey(Cluster3DCharacteristics.SubstrateCoplanarity))
                {
                    // Map Copla is enable
                    double dDieSubCopla = (double)cluster3DDie.characteristics[Cluster3DCharacteristics.SubstrateCoplanarity];
                    bDieSubCoplaFail = dDieSubCopla < paramDieSubstrateCoplanarityPassRangeMin.Value || dDieSubCopla > paramDieSubstrateCoplanarityPassRangeMax.Value;

                    cluster3DDie.characteristics[Cluster3DCharacteristics.isSubCoplaFailure] = bDieSubCoplaFail;
                    if (bDieSubCoplaFail)
                        cluster3DDie.defectClassList.Add(_LblDieBadSubstrateCopla);
                }

                double MissingFailureType = 1.0;  //"Missing";
                double BadHeightFailureType = 2.0; // "BadHeight";

                double dModCavity = paramIsMeasuringCavity.Value ? -1.0 : 1.0;
                foreach (Blob blobmeasure in cluster3DDie.blobList)
                {
                    if (State == eModuleState.Aborting)
                    {
                        break;
                    }

                    double dHeightum = (double)blobmeasure.characteristics[Blob3DCharacteristics.HeightMicron];
                    dSumDieHeights += dHeightum;
                    if (dModCavity * dHeightum < dModCavity * paramMeasureMissingLimit.Value)
                    {
                        NbMeasureFailMissing++;
                        blobmeasure.characteristics.Add(Blob3DCharacteristics.FailureType, MissingFailureType);
                    }
                    else if (Math.Abs(dHeightum) < Math.Abs(paramMeasureHeightPassRangeMin.Value) || Math.Abs(dHeightum) > Math.Abs(paramMeasureHeightPassRangeMax.Value))
                    {
                        NbMeasureFailBadHeight++;
                        blobmeasure.characteristics.Add(Blob3DCharacteristics.FailureType, BadHeightFailureType);
                    }
                    else
                    {
                        NbMeasurePass++;
                        blobmeasure.characteristics.Add(Blob3DCharacteristics.FailureType, 0.0);
                    }
                }

                if (State != eModuleState.Aborting)
                {
                    cluster3DDie._NbMeasuresPass = NbMeasurePass;
                    cluster3DDie._NbMeasuresMissing = NbMeasureFailMissing;
                    cluster3DDie._NbMeasuresBadHeight = NbMeasureFailBadHeight;
                    cluster3DDie._SumMeasuresHeight = dSumDieHeights;

                    bool isMeasureMissingFailure = (NbMeasureFailMissing > paramNbKillMissingMeasure.Value);
                    cluster3DDie.characteristics[Cluster3DCharacteristics.isMeasureMissingFailure] = isMeasureMissingFailure;
                    if (isMeasureMissingFailure)
                        cluster3DDie.defectClassList.Add(_LblMeasureMissing);

                    bool isMeasureHeightFailure = (NbMeasureFailBadHeight > paramNbKillBadHeightMeasure.Value);
                    cluster3DDie.characteristics[Cluster3DCharacteristics.isMeasureHeightFailure] = isMeasureHeightFailure;
                    if (isMeasureHeightFailure)
                        cluster3DDie.defectClassList.Add(_LblMeasureBadHeight);

                    bool bDieFailure = isMeasureMissingFailure || isMeasureHeightFailure || bDieAvgHeightFail || bDieCoplaFail || bDieSubCoplaFail;
                    cluster3DDie.characteristics[Cluster3DCharacteristics.isFailure] = bDieFailure;
                }

                ProcessChildren(obj);

            }
        }

    }
}
