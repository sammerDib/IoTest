using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.Edition.DataBase;

using UnitySC.Shared.Tools;


namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class HMStatsReportModule : DatabaseEditionModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Autres Champs
        //=================================================================
        private List<Cluster3DDieHM> _clusterList = new List<Cluster3DDieHM>();
        protected PathString _Filename;

        protected int _nSampleSize_mm = 0;

        protected uint _DieInspected = 0;
        protected uint _DiePass = 0;
        protected uint _DieFail = 0;
        protected uint _DieFailDetailsAvgHeight = 0;
        protected uint _DieFailDetailsCopla = 0;
        protected uint _DieFailDetailsSubCopla = 0;
        protected uint _DieFailDetailsBumpMissing = 0;
        protected uint _DieFailDetailsBumpBadHeigth = 0;


        protected uint _BumpInspected = 0;
        protected uint _BumpPass = 0;
        protected uint _BumpFail = 0;
        protected uint _BumpFailDetailsMissing = 0;
        protected uint _BumpFailDetailsBadHeight = 0;

        protected double _dDieAvgMeanCalc;
        protected double _dDieAvgMinCalc;
        protected double _dDieAvgMaxCalc;
        protected List<double> _dDieStdCalc = new List<double>();

        protected double _dBumpMeanCalc;
        protected double _dBumpMinCalc;
        protected double _dBumpMaxCalc;

        protected double _dDieCoplaMeanCalc;
        protected double _dDieCoplaMinCalc;
        protected double _dDieCoplaMaxCalc;
        protected List<double> _dDieCoplaStdCalc = new List<double>();

        protected double _dDieSubCoplaMeanCalc;
        protected double _dDieSubCoplaMinCalc;
        protected double _dDieSubCoplaMaxCalc;
        protected List<double> _dDieSubCoplaStdCalc = new List<double>();

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
        public HMStatsReportModule(IModuleFactory factory, int id, Recipe recipe)
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
            List<ModuleBase> AncestorHMmodule = FindAncestors(mod => mod is HeightMeasurementAnalysisModule);
            if (AncestorHMmodule.Count == 0)
                return "No Height measurement die ANALYSIS module has been set above this module";

            HeightMeasurementDieModule DirectAncestor = AncestorHMmodule[0] as HeightMeasurementDieModule;
            // check if data is coorectly computed by ancestor

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            String sFileName = String.Format("{0}_HMStats_{1}{2}", Wafer.Basename, RunIter, ".csv");
            _Filename = DestinationDirectory / sFileName;

            _dBumpMeanCalc = 0.0;
            _dBumpMinCalc = Double.MaxValue;
            _dBumpMaxCalc = Double.MinValue;

            _dDieAvgMeanCalc = 0.0;
            _dDieAvgMinCalc = Double.MaxValue;
            _dDieAvgMaxCalc = Double.MinValue;
            _dDieStdCalc.Clear();

            _dDieCoplaMeanCalc = 0.0;
            _dDieCoplaMinCalc = Double.MaxValue;
            _dDieCoplaMaxCalc = Double.MinValue;
            _dDieCoplaStdCalc.Clear();

            _dDieSubCoplaMeanCalc = 0.0;
            _dDieSubCoplaMinCalc = Double.MaxValue;
            _dDieSubCoplaMaxCalc = Double.MinValue;
            _dDieSubCoplaStdCalc.Clear();

            float width_mm = Wafer.SurroundingRectangle.Width / 1000.0f;
            float Height_mm = Wafer.SurroundingRectangle.Height / 1000.0f;
            _nSampleSize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            if ((obj is Cluster3DDieHM) == false)
                throw new ApplicationException("Wrong type of parent data sent, Cluster3DDieHM is expected");

            Cluster3DDieHM cluster = (Cluster3DDieHM)obj;


            bool bHasMapHeight = cluster.characteristics.ContainsKey(Cluster3DCharacteristics.HeightAverage);
            bool bHasMapCopla = cluster.characteristics.ContainsKey(Cluster3DCharacteristics.Coplanarity);
            bool bHasMapSubCopla = cluster.characteristics.ContainsKey(Cluster3DCharacteristics.SubstrateCoplanarity);

            //-------------------------------------------------------------
            // Stockage des clusters
            //-------------------------------------------------------------
            cluster.AddRef();
            lock (_clusterList)
            {

                _DieInspected++;
                if (true == (bool)cluster.characteristics[Cluster3DCharacteristics.isFailure])
                {
                    _DieFail++;

                    _WaferisFailure = true;

                    if (bHasMapHeight)
                    {
                        if (true == (bool)cluster.characteristics[Cluster3DCharacteristics.isHeightFailure])
                            _DieFailDetailsAvgHeight++;
                    }

                    if (bHasMapCopla)
                    {
                        if (true == (bool)cluster.characteristics[Cluster3DCharacteristics.isCoplaFailure])
                            _DieFailDetailsCopla++;
                    }

                    if (bHasMapSubCopla)
                    {
                        if (true == (bool)cluster.characteristics[Cluster3DCharacteristics.isSubCoplaFailure])
                            _DieFailDetailsSubCopla++;
                    }

                    if (true == (bool)cluster.characteristics[Cluster3DCharacteristics.isMeasureHeightFailure])
                        _DieFailDetailsBumpBadHeigth++;

                    if (true == (bool)cluster.characteristics[Cluster3DCharacteristics.isMeasureMissingFailure])
                        _DieFailDetailsBumpMissing++;
                }
                else
                {
                    _DiePass++;
                }

                _BumpInspected += (uint)cluster.blobList.Count;
                _BumpPass += cluster._NbMeasuresPass;
                _BumpFail += (cluster._NbMeasuresMissing + cluster._NbMeasuresBadHeight);
                _BumpFailDetailsMissing += cluster._NbMeasuresMissing;
                _BumpFailDetailsBadHeight += cluster._NbMeasuresBadHeight;

                _dBumpMeanCalc += cluster._SumMeasuresHeight;
                if (_dBumpMaxCalc < (double)cluster._dieHMresult.MaxHeight)
                    _dBumpMaxCalc = (double)cluster._dieHMresult.MaxHeight;
                if (_dBumpMinCalc > (double)cluster._dieHMresult.MinHeight)
                    _dBumpMinCalc = (double)cluster._dieHMresult.MinHeight;

                _dDieAvgMeanCalc += cluster._dieHMresult.MeanHeight_um;
                _dDieStdCalc.Add(cluster._dieHMresult.MeanHeight_um);
                if (_dDieAvgMaxCalc < cluster._dieHMresult.MeanHeight_um)
                    _dDieAvgMaxCalc = cluster._dieHMresult.MeanHeight_um;
                if (_dDieAvgMinCalc > cluster._dieHMresult.MeanHeight_um)
                    _dDieAvgMinCalc = cluster._dieHMresult.MeanHeight_um;

                if (bHasMapCopla)
                {
                    _dDieCoplaMeanCalc += cluster._dieHMresult.Coplanarity;
                    _dDieCoplaStdCalc.Add(cluster._dieHMresult.Coplanarity);
                    if (_dDieCoplaMaxCalc < cluster._dieHMresult.Coplanarity)
                        _dDieCoplaMaxCalc = cluster._dieHMresult.Coplanarity;
                    if (_dDieCoplaMinCalc > cluster._dieHMresult.Coplanarity)
                        _dDieCoplaMinCalc = cluster._dieHMresult.Coplanarity;
                }

                if (bHasMapSubCopla)
                {
                    _dDieSubCoplaMeanCalc += cluster._dieHMresult.SubstrateCoplanarity;
                    _dDieSubCoplaStdCalc.Add(cluster._dieHMresult.SubstrateCoplanarity);
                    if (_dDieSubCoplaMaxCalc < cluster._dieHMresult.SubstrateCoplanarity)
                        _dDieSubCoplaMaxCalc = cluster._dieHMresult.SubstrateCoplanarity;
                    if (_dDieSubCoplaMinCalc > cluster._dieHMresult.SubstrateCoplanarity)
                        _dDieSubCoplaMinCalc = cluster._dieHMresult.SubstrateCoplanarity;
                }

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
            log("Creating Die Height Measure Statical report file " + _Filename);

            int NbDies = _clusterList.Count;
            int NbMeasuresPerDie = 0;
            bool bHasMapHeight = false;
            bool bHasMapCopla = false;
            bool bHasMapSubCopla = false;
            if (NbDies > 0)
            {
                NbMeasuresPerDie = _clusterList[0]._dieHMresult.NbMeasures;
                bHasMapHeight = _clusterList[0].characteristics.ContainsKey(Cluster3DCharacteristics.HeightAverage);
                bHasMapCopla = _clusterList[0].characteristics.ContainsKey(Cluster3DCharacteristics.Coplanarity);
                bHasMapSubCopla = _clusterList[0].characteristics.ContainsKey(Cluster3DCharacteristics.SubstrateCoplanarity);

                // on sort les die cluster de manière à les enregistré s dans le même ordre
                _clusterList.Sort((a, b) =>
                {
                    var n = b.DieIndexY.CompareTo(a.DieIndexY);
                    if (n == 0)
                        n = a.DieIndexX.CompareTo(b.DieIndexX);
                    return n;
                });
            }

            using (StreamWriter stream = new StreamWriter(_Filename, false))
            {
                DateTime dtNow = DateTime.Now;
                stream.WriteLine("Wafer Report");
                stream.WriteFormat("Job ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.JobID));
                stream.WriteFormat("Lot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.LotID));
                stream.WriteFormat("Wafer ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.WaferID));
                int nSlotId = 0;
                if (int.TryParse(Wafer.GetWaferInfo(eWaferInfo.SlotID), out nSlotId))
                    stream.WriteFormat("Slot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.SlotID));

                stream.WriteFormat("Recipe;{0}\n", Wafer.GetWaferInfo(eWaferInfo.ADCRecipeFileName));
                stream.WriteFormat("BaseName;{0}\n", Wafer.GetWaferInfo(eWaferInfo.Basename));
                stream.WriteFormat("Wafer Diameter (mm);{0}\n", _nSampleSize_mm);
                stream.WriteLine();

                stream.WriteFormat("Total # testable dies;{0}\n", NbDies);
                stream.WriteFormat("# bumps per die;{0}\n", NbMeasuresPerDie);

                stream.WriteFormat("Wafer Start Time;{0}\n", Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.StartProcess));
                stream.WriteFormat("Wafer End Time;{0:dd-MM-yyyy HH:mm:ss}\n", dtNow);
                stream.WriteFormat("Equipment ID;{0}-{1}-{2}\n", "UNITYSC", "4See", Recipe.Toolname);
                stream.WriteFormat("Wafer Inspection Result;{0}\n", _WaferisFailure ? "Fail" : "Pass");
                stream.WriteLine();

                double dPctWaferYield = 100.0 * (double)_DiePass / (double)_DieInspected;
                stream.WriteFormat("Total Wafer Yield %;{0:#0.00}\n", dPctWaferYield);
                stream.WriteLine("# Dies failed (Die Inspection Error);0");
                stream.WriteFormat("# dies Inspected;{0};# Bumps Inspected;{1}\n", _DieInspected, _BumpInspected);
                stream.WriteFormat("# dies Passed;{0};# Bumps Passed;{1}\n", _DiePass, _BumpPass);
                stream.WriteFormat("# dies Rejected;{0};# Bumps Failed;{1}\n", _DieFail, _BumpFail);
                stream.WriteLine();

                double dPctInspectedYield = 100.0; //100.0*(Nb Dies Inspected – Nb dies inspection error (could not be computed)  / Nb Dies Inspected 
                stream.WriteFormat("Inspected Yield %;{0:#0.00}\n", dPctInspectedYield);
                double dPctBumpMeasureYield = 100.0 * (double)_BumpPass / (double)_BumpInspected;
                stream.WriteFormat("Bump Metrology Yield %;{0:#0.00}\n", dPctBumpMeasureYield);
                stream.WriteLine();

                stream.WriteFormat("# dies failed (Missing/Not Located);{0};# Bumps failed (Missing/Not Located);{1}\n", _DieFailDetailsBumpMissing, _BumpFailDetailsMissing);
                stream.WriteFormat("# dies failed (Height);{0};# Bumps failed (Height);{1}\n", _DieFailDetailsBumpBadHeigth, _BumpFailDetailsBadHeight);
                stream.WriteFormat("# dies failed (Average Height);{0}\n", _DieFailDetailsAvgHeight);
                stream.WriteFormat("# dies failed (Coplanarity);{0}\n", _DieFailDetailsCopla);
                stream.WriteFormat("# dies failed (Substrate Coplanarity);{0}\n", _DieFailDetailsSubCopla);
                stream.WriteLine();

                double dBumpMean = _dBumpMeanCalc / (double)_BumpInspected;
                double dBumpStd = 0.0;
                foreach (Cluster3DDieHM cluster in _clusterList)
                    dBumpStd += cluster._dieHMresult.ComputeDieMeasuresVariance(dBumpMean);
                dBumpStd /= (double)(_BumpInspected);
                dBumpStd = Math.Sqrt(dBumpStd);
                double dDieAvgMean = _dDieAvgMeanCalc / (double)_DieInspected;
                double dDieAvgStd = Math.Sqrt(_dDieStdCalc.Average(d => Math.Pow(d - dDieAvgMean, 2))); ;

                stream.WriteLine(" ;Min;Mean;Max;Std;");
                stream.WriteFormat("Bump Height (um);{0:#0.00};{1:#0.00};{2:#0.00};{3:#0.00}\n", _dBumpMinCalc, dBumpMean, _dBumpMaxCalc, dBumpStd);
                stream.WriteFormat("Bump Average Height (um);{0:#0.00};{1:#0.00};{2:#0.00};{3:#0.00}\n", _dDieAvgMinCalc, dDieAvgMean, _dDieAvgMaxCalc, dDieAvgStd);
                if (bHasMapCopla)
                {
                    double dDieCoplaMean = _dDieCoplaMeanCalc / (double)_DieInspected;
                    double dDieCoplaStd = Math.Sqrt(_dDieCoplaStdCalc.Average(d => Math.Pow(d - dDieCoplaMean, 2))); ;
                    stream.WriteFormat("Coplanarity (um);{0:#0.00};{1:#0.00};{2:#0.00};{3:#0.00}\n", _dDieCoplaMinCalc, dDieCoplaMean, _dDieCoplaMaxCalc, dDieCoplaStd);
                }
                if (bHasMapSubCopla)
                {
                    double dDieSubCoplaMean = _dDieSubCoplaMeanCalc / (double)_DieInspected;
                    double dDieSubCoplaStd = Math.Sqrt(_dDieSubCoplaStdCalc.Average(d => Math.Pow(d - dDieSubCoplaMean, 2))); ;
                    stream.WriteFormat("Substrate Coplanarity (um);{0:#0.00};{1:#0.00};{2:#0.00};{3:#0.00}\n", _dDieSubCoplaMinCalc, dDieSubCoplaMean, _dDieSubCoplaMaxCalc, dDieSubCoplaStd);
                }

                ///-------
                stream.WriteLine("Die Details");

                stream.Write("Die Col;Die Row;Status;# Bumps failed;# Bump failed (Missing/Not Located);# Bump failed (Height);" +
                    "Average Height Status;Coplanarity Status;Bump Height Min (um);Bump Height Max (um);Bump Height Std (um);Average Height (um);Coplanarity (um);");
                if (bHasMapSubCopla)
                    stream.Write("Substrate Coplanarity Status; Substrate Coplanarity (um);");
                stream.WriteLine();

                foreach (Cluster3DDieHM cluster in _clusterList)
                {
                    if (State == eModuleState.Aborting)
                        break;

                    int nDieIdxX = cluster.DieIndexX;
                    int nDieIdxY = cluster.DieIndexY;
                    String sDieFailStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isFailure] ? "Fail" : "Pass";
                    String sAvgHeightStatus = "-";
                    if (bHasMapHeight)
                        sAvgHeightStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isHeightFailure] ? "Fail" : "Pass";
                    String sCoplaStatus = "-";
                    if (bHasMapCopla)
                        sCoplaStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isCoplaFailure] ? "Fail" : "Pass";
                    String sSubCoplaStatus = "-";
                    if (bHasMapSubCopla)
                        sSubCoplaStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isSubCoplaFailure] ? "Fail" : "Pass";

                    stream.WriteFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8:#0.00};{9:#0.00};{10:#0.00};{11:#0.00};{12:#0.00};",
                            nDieIdxX, nDieIdxY, sDieFailStatus, cluster._NbMeasuresMissing + cluster._NbMeasuresBadHeight, cluster._NbMeasuresMissing, cluster._NbMeasuresBadHeight,
                            sAvgHeightStatus, sCoplaStatus, cluster._dieHMresult.MinHeight, cluster._dieHMresult.MaxHeight, cluster._dieHMresult.StdDev, cluster._dieHMresult.MeanHeight_um,
                            cluster._dieHMresult.Coplanarity);
                    if (bHasMapSubCopla)
                        stream.WriteFormat("{0};{1:#0.00};", sSubCoplaStatus, cluster._dieHMresult.SubstrateCoplanarity);
                    stream.WriteLine();

                    Interlocked.Increment(ref nbObjectsOut);
                }
                stream.WriteLine();
            }
        }


        //=================================================================
        // 
        //=================================================================
        private void PurgeCSV()
        {
            // Purge de la liste interne de clusters
            //......................................
            foreach (Cluster3DDieHM cluster in _clusterList)
                cluster.DelRef();
            _clusterList.Clear();
        }
    }
}
