using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.Edition.DataBase;

using UnitySC.Shared.Tools;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class DMStatsReportModule : DatabaseEditionModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Autres Champs
        //=================================================================
        private List<Cluster2DDieDM> _clusterList = new List<Cluster2DDieDM>();
        protected PathString _Filename;

        protected int _nSampleSize_mm = 0;

        protected uint _DieInspected = 0;
        protected uint _DiePass = 0;
        protected uint _DieFail = 0;
        protected uint _DieFailDetailsAvgDiameter = 0;
        protected uint _DieFailDetailsBumpMissing = 0;
        protected uint _DieFailDetailsBumpBadDiameter = 0;
        protected uint _DieFailDetailsBumpBadOffset = 0;

        protected uint _BumpInspected = 0;
        protected uint _BumpPass = 0;
        protected uint _BumpFail = 0;
        protected uint _BumpFailDetailsMissing = 0;
        protected uint _BumpFailDetailsBadDiameter = 0;
        protected uint _BumpFailDetailsBadOffset = 0;

        protected double _dDieAvgMeanCalc;
        protected double _dDieAvgMinCalc;
        protected double _dDieAvgMaxCalc;
        protected List<double> _dDieStdCalc = new List<double>();

        protected double _dBumpMeanCalc;
        protected double _dBumpMinCalc;
        protected double _dBumpMaxCalc;

        protected double _dBumpMeanCalcOffset;
        protected double _dBumpMinCalcOffset;
        protected double _dBumpMaxCalcOffset;


        protected bool _WaferisFailure = false;

        //=================================================================
        // Database results registration - in this specific cas there will be no registration 
        //=================================================================
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.HeightMeasurement_AHM); // en réalité c'est un csv,  mais ce type là n'est pas register
            return Rtypes;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public DMStatsReportModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {


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
            List<ModuleBase> AncestorHMmodule = FindAncestors(mod => mod is DiameterMeasurementAnalysisModule);
            if (AncestorHMmodule.Count == 0)
                return "No Diameter measurement die ANALYSIS module has been set above this module";

            DiameterMeasurementDieModule DirectAncestor = AncestorHMmodule[0] as DiameterMeasurementDieModule;
            // check if data is coorectly computed by ancestor

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            String sFileName = String.Format("{0}_DMStats_{1}{2}", Wafer.Basename, RunIter, ".csv");
            _Filename = DestinationDirectory / sFileName;

            _dBumpMeanCalc = 0.0;
            _dBumpMinCalc = Double.MaxValue;
            _dBumpMaxCalc = Double.MinValue;

            _dBumpMeanCalcOffset = 0.0;
            _dBumpMinCalcOffset = Double.MaxValue;
            _dBumpMaxCalcOffset = Double.MinValue;

            _dDieAvgMeanCalc = 0.0;
            _dDieAvgMinCalc = Double.MaxValue;
            _dDieAvgMaxCalc = Double.MinValue;
            _dDieStdCalc.Clear();

            if (Wafer is NotchWafer)
            {
                _nSampleSize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                _nSampleSize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                _nSampleSize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            if ((obj is Cluster2DDieDM) == false)
                throw new ApplicationException("Wrong type of parent data sent, Cluster2DDieDM is expected");

            Cluster2DDieDM cluster = (Cluster2DDieDM)obj;
            //-------------------------------------------------------------
            // Stockage des clusters
            //-------------------------------------------------------------
            cluster.AddRef();
            lock (_clusterList)
            {

                _DieInspected++;
                if (true == (bool)cluster.characteristics[Cluster2DCharacteristics.isFailure])
                {
                    _DieFail++;

                    _WaferisFailure = true;

                    if (true == (bool)cluster.characteristics[Cluster2DCharacteristics.isDiameterFailure])
                        _DieFailDetailsAvgDiameter++;

                    if (true == (bool)cluster.characteristics[Cluster2DCharacteristics.isMeasureDiameterFailure])
                        _DieFailDetailsBumpBadDiameter++;

                    if (true == (bool)cluster.characteristics[Cluster2DCharacteristics.isMeasureDiameterFailure])
                        _DieFailDetailsBumpBadOffset++;

                    if (true == (bool)cluster.characteristics[Cluster2DCharacteristics.isMeasureMissingFailure])
                        _DieFailDetailsBumpMissing++;
                }
                else
                {
                    _DiePass++;
                }

                _BumpInspected += (uint)cluster.blobList.Count;
                _BumpPass += cluster._NbMeasuresPass;
                _BumpFail += (cluster._NbMeasuresMissing + cluster._NbMeasuresBadDiameter + cluster._NbMeasuresBadOffset);
                _BumpFailDetailsMissing += cluster._NbMeasuresMissing;
                _BumpFailDetailsBadDiameter += cluster._NbMeasuresBadDiameter;
                _BumpFailDetailsBadOffset += cluster._NbMeasuresBadOffset;

                _dBumpMeanCalc += cluster._SumMeasuresDiameter;
                if (_dBumpMaxCalc < (double)cluster._MaxMeasureDiameter)
                    _dBumpMaxCalc = (double)cluster._MaxMeasureDiameter;
                if (_dBumpMinCalc > (double)cluster._MinMeasureDiameter)
                    _dBumpMinCalc = (double)cluster._MinMeasureDiameter;

                _dBumpMeanCalcOffset += cluster._SumMeasuresOffset;
                if (_dBumpMaxCalcOffset < (double)cluster._MaxMeasureOffset)
                    _dBumpMaxCalcOffset = (double)cluster._MaxMeasureOffset;
                if (_dBumpMinCalcOffset > (double)cluster._MinMeasureOffset)
                    _dBumpMinCalcOffset = (double)cluster._MinMeasureOffset;

                _dDieAvgMeanCalc += cluster._MeanMeasureDiameter;
                _dDieStdCalc.Add(cluster._MeanMeasureDiameter);
                if (_dDieAvgMaxCalc < cluster._MeanMeasureDiameter)
                    _dDieAvgMaxCalc = cluster._MeanMeasureDiameter;
                if (_dDieAvgMinCalc > cluster._MeanMeasureDiameter)
                    _dDieAvgMinCalc = cluster._MeanMeasureDiameter;


                _clusterList.Add(cluster);
            }
        }


        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");

            Scheduler.StartSingleTask("ProcessCSV", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                        ProcessCSV();
                }
                catch (Exception ex)
                {
                    string msg = "CSV generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    PurgeCSV();
                    base.OnStopping(oldState);
                }
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessCSV()
        {
            //-------------------------------------------------------------
            // Write CSV file - reporting all Height Measure
            //-------------------------------------------------------------
            log("Creating Die 2D Measure Statical report file " + _Filename);

            int nFooter = 1024;
            int NbDies = _clusterList.Count;
            int NbMeasuresPerDie = 0;
            if (NbDies > 0)
            {
                NbMeasuresPerDie = _clusterList[0].blobList.Count;

                // on sort les die cluster de manière à les enregistré s dans le même ordre
                _clusterList.Sort((a, b) =>
                {
                    var n = b.DieIndexY.CompareTo(a.DieIndexY);
                    if (n == 0)
                        n = a.DieIndexX.CompareTo(b.DieIndexX);
                    return n;
                });
            }

            StringBuilder sb = new StringBuilder(NbDies * NbMeasuresPerDie * 128 + nFooter);

            DateTime dtNow = DateTime.Now;
            sb.AppendLine("Wafer Report");
            sb.AppendFormat("Job ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.ToolRecipe));
            sb.AppendFormat("Lot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.LotID));
            sb.AppendFormat("Wafer ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.WaferID));
            int nSlotId = 0;
            if (int.TryParse(Wafer.GetWaferInfo(eWaferInfo.SlotID), out nSlotId))
            {
                sb.AppendFormat("Slot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.SlotID));

            }
            sb.AppendFormat("Recipe;{0}\n", Wafer.GetWaferInfo(eWaferInfo.ADCRecipeFileName));
            sb.AppendFormat("Unique ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.Basename));
            sb.AppendFormat("Wafer Diameter (mm);{0}\n", _nSampleSize_mm);
            sb.AppendLine();

            sb.AppendFormat("Total # testable dies;{0}\n", NbDies);
            sb.AppendFormat("# bumps per die;{0}\n", NbMeasuresPerDie);

            sb.AppendFormat("Wafer Start Time;{0}\n", Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.StartProcess));
            sb.AppendFormat("Wafer End Time;{0:dd-MM-yyyy HH:mm:ss}\n", dtNow);
            sb.AppendFormat("Equipment ID;{0}-{1}-{2}\n", "UNITYSC", "4See", Recipe.Toolname);
            sb.AppendFormat("Wafer Inspection Result;{0}\n", _WaferisFailure ? "Fail" : "Pass");
            sb.AppendLine();

            double dPctWaferYield = 100.0 * (double)_DiePass / (double)_DieInspected;
            sb.AppendFormat("Total Wafer Yield %;{0:#0.00}\n", dPctWaferYield);
            sb.AppendLine("# Dies failed (Die Inspection Error);0");
            sb.AppendFormat("# dies Inspected;{0};# Bumps Inspected;{1}\n", _DieInspected, _BumpInspected);
            sb.AppendFormat("# dies Passed;{0};# Bumps Passed;{1}\n", _DiePass, _BumpPass);
            sb.AppendFormat("# dies Rejected;{0};# Bumps Failed;{1}\n", _DieFail, _BumpFail);
            sb.AppendLine();

            double dPctInspectedYield = 100.0; //100.0*(Nb Dies Inspected – Nb dies inspection error (could not be computed)  / Nb Dies Inspected 
            sb.AppendFormat("Inspected Yield %;{0:#0.00}\n", dPctInspectedYield);
            double dPctBumpMeasureYield = 100.0 * (double)_BumpPass / (double)_BumpInspected;
            sb.AppendFormat("Bump Metrology Yield %;{0:#0.00}\n", dPctBumpMeasureYield);
            sb.AppendLine();

            sb.AppendFormat("# dies failed (Missing/Not Located);{0};# Bumps failed (Missing/Not Located);{1}\n", _DieFailDetailsBumpMissing, _BumpFailDetailsMissing);
            sb.AppendFormat("# dies failed (Diameter);{0};# Bumps failed (Diameter);{1}\n", _DieFailDetailsBumpBadDiameter, _BumpFailDetailsBadDiameter);
            sb.AppendFormat("# dies failed (Bump-Offset);{0};# Bumps failed (Bump-Offset);{1}\n", _DieFailDetailsBumpBadOffset, _BumpFailDetailsBadOffset);
            sb.AppendFormat("# dies failed (Average Diameter);{0}\n", _DieFailDetailsAvgDiameter);
            sb.AppendLine();

            // All measures : diameter and Offset statistics computation 
            double dBumpMeanDiameter = _dBumpMeanCalc / (double)_BumpInspected;
            double dBumpStdDiameter = 0.0;
            double dBumpMeanOffset = _dBumpMeanCalcOffset / (double)_BumpInspected;
            double dBumpStdOffset = 0.0;
            foreach (Cluster2DDieDM cluster in _clusterList)
            {
                dBumpStdDiameter += cluster._StatsDiameter.Sum(mes => Math.Pow(mes - dBumpMeanDiameter, 2));
                dBumpStdOffset += cluster._StatsOffset.Sum(mes => Math.Pow(mes - dBumpMeanOffset, 2)); ;
            }
            dBumpStdDiameter /= (double)(_BumpInspected);
            dBumpStdOffset /= (double)(_BumpInspected);
            dBumpStdDiameter = Math.Sqrt(dBumpStdDiameter);
            dBumpStdOffset = Math.Sqrt(dBumpStdOffset);

            double dDieAvgMean = _dDieAvgMeanCalc / (double)_DieInspected;
            double dDieAvgStd = Math.Sqrt(_dDieStdCalc.Average(d => Math.Pow(d - dDieAvgMean, 2))); ;

            sb.AppendLine(" ;Min;Mean;Max;Std;");
            sb.AppendFormat("Bump Diameter (um);{0:#0.00};{1:#0.00};{2:#0.00};{3:#0.00}\n", _dBumpMinCalc, dBumpMeanDiameter, _dBumpMaxCalc, dBumpStdDiameter);
            sb.AppendFormat("Bump Bump-Offset (um);{0:#0.00};{1:#0.00};{2:#0.00};{3:#0.00}\n", _dBumpMinCalcOffset, dBumpMeanOffset, _dBumpMaxCalcOffset, dBumpStdOffset);
            sb.AppendFormat("Bump Average Diameter (um);{0:#0.00};{1:#0.00};{2:#0.00};{3:#0.00}\n", _dDieAvgMinCalc, dDieAvgMean, _dDieAvgMaxCalc, dDieAvgStd);

            ///-------
            sb.AppendLine("Die Details");

            sb.Append("Die Col;Die Row;Status;# Bumps failed;# Bump failed (Missing/Not Located);# Bump failed (Diameter);Bump failed (Offset);" +
                "Average Diameter Status;Bump Diameter Min (um);Bump Diameter Max (um);Bump Diameter Std (um);" +
                "Bump Offset Min (um);Bump Offset Mean (um);Bump Offset Max (um);Bump Offset Std (um);Average Diameter (um);");
            sb.AppendLine();

            foreach (Cluster2DDieDM cluster in _clusterList)
            {
                if (State == eModuleState.Aborting)
                {
                    break;
                }

                int nDieIdxX = cluster.DieIndexX;
                int nDieIdxY = cluster.DieIndexY;
                String sDieFailStatus = (bool)cluster.characteristics[Cluster2DCharacteristics.isFailure] ? "Fail" : "Pass";
                String sAvgDiameterStatus = (bool)cluster.characteristics[Cluster2DCharacteristics.isDiameterFailure] ? "Fail" : "Pass";

                sb.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8:#0.00};{9:#0.00};{10:#0.00};{11:#0.00};{12:#0.00};{13:#0.00};{14:#0.00};{15:#0.00};",
                         nDieIdxX, nDieIdxY, sDieFailStatus, cluster._NbMeasuresMissing + cluster._NbMeasuresBadDiameter + cluster._NbMeasuresBadOffset, cluster._NbMeasuresMissing, cluster._NbMeasuresBadDiameter, cluster._NbMeasuresBadOffset,
                         sAvgDiameterStatus, cluster._MinMeasureDiameter, cluster._MaxMeasureDiameter, cluster._StdDevMeasureDiameter,
                         cluster._MinMeasureOffset, cluster._MeanMeasureOffset, cluster._MaxMeasureOffset, cluster._StdDevMeasureOffset, cluster._MeanMeasureDiameter);

                sb.AppendLine();

                Interlocked.Increment(ref nbObjectsOut);
            }
            sb.AppendLine();

            if (State != eModuleState.Aborting)
            {
                using (StreamWriter SW = new StreamWriter(_Filename, false))
                {
                    SW.Write(sb.ToString());
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeCSV()
        {
            // Purge de la liste interne de clusters
            //......................................
            foreach (Cluster2DDieDM cluster in _clusterList)
                cluster.DelRef();
            _clusterList.Clear();
        }
    }
}
