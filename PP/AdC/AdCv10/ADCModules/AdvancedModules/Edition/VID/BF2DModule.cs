using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcRobotExchange;

using AdcTools;

using DiameterMeasurementDieModule;

using UnitySC.Shared.Tools;

namespace AdvancedModules.Edition.VID
{
    public class BF2DVIDModule : ModuleBase
    {
        public readonly BF2DParameter dataCollection;

        private CustomExceptionDictionary<int, VidMeasure2D> vidMap;
        private List<VidBase> vidReturnList;

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
        // Constructeur
        //=================================================================
        public BF2DVIDModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            dataCollection = new BF2DParameter(this, "DataCollection");
            vidReturnList = new List<VidBase>();
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            InitVariableList();

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
            logDebug("cluster " + obj);
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

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("BF2D3DVID",
                () =>
                {
                    try
                    {
                        ProcessBF2D3DVID();
                    }
                    catch (Exception ex)
                    {
                        string msg = "GT VID failed: " + ex.Message;
                        HandleException(new ApplicationException(msg, ex));
                    }
                    finally
                    {
                        base.OnStopping(oldState);
                    }
                });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessBF2D3DVID()
        {
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

            //------ calcul stats ---///
            double dPctWaferYield = 100.0 * (double)_DiePass / (double)_DieInspected;
            double dPctBumpMeasureYield = 100.0 * (double)_BumpPass / (double)_BumpInspected;

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
            //------------------------//

            SetVIDVariable(enDataType.bumpperdie, NbMeasuresPerDie, "NA");
            SetVIDVariable(enDataType.totalWaferYield, dPctWaferYield, "NA");
            SetVIDVariable(enDataType.nbDieFailed, 0, "NA"); // OPI : demander à romain pourquoi les fail sont dans les reject et la valeur fail à 0
            SetVIDVariable(enDataType.nbDieInspected, _DieInspected, "NA");
            SetVIDVariable(enDataType.nbDiePassed, _DiePass, "NA");
            SetVIDVariable(enDataType.nbDieRejected, _DieFail, "NA");
            SetVIDVariable(enDataType.nbDieFailedMissing, _DieFailDetailsBumpMissing, "NA");
            SetVIDVariable(enDataType.nbDieFailedDiameter, _DieFailDetailsBumpBadDiameter, "NA");
            SetVIDVariable(enDataType.nbDieFailedOffset, _DieFailDetailsBumpBadOffset, "NA");
            SetVIDVariable(enDataType.nbDieFailedAverage, _DieFailDetailsAvgDiameter, "NA");
            SetVIDVariable(enDataType.nbBumpInspected, _BumpInspected, "NA");
            SetVIDVariable(enDataType.nbBumpPassed, _BumpPass, "NA");
            SetVIDVariable(enDataType.nbBumpFailed, _BumpFail, "NA");
            SetVIDVariable(enDataType.nbBumpFailedMissing, _BumpFailDetailsMissing, "NA");
            SetVIDVariable(enDataType.nbBumpFailedDiameter, _BumpFailDetailsBadDiameter, "NA");
            SetVIDVariable(enDataType.nbBumpFailedOffset, _BumpFailDetailsBadOffset, "NA");
            SetVIDVariable(enDataType.statsBumpDiameterMin, _dBumpMinCalc, "µ");
            SetVIDVariable(enDataType.statsBumpDiameterMean, dBumpMeanDiameter, "µ");
            SetVIDVariable(enDataType.statsBumpDiameterMax, _dBumpMaxCalc, "µ");
            SetVIDVariable(enDataType.statsBumpDiameterStdDev, dBumpStdDiameter, "µ");
            SetVIDVariable(enDataType.statsBumpOffsetMin, _dBumpMinCalcOffset, "µ");
            SetVIDVariable(enDataType.statsBumpOffsetMean, dBumpMeanOffset, "µ");
            SetVIDVariable(enDataType.statsBumpOffsetMax, _dBumpMaxCalcOffset, "µ");
            SetVIDVariable(enDataType.statsBumpOffsetStdDev, dBumpStdOffset, "µ");
            SetVIDVariable(enDataType.statsBumpAverageDiameterMin, _dDieAvgMinCalc, "µ");
            SetVIDVariable(enDataType.statsBumpAverageDiameterMean, dDieAvgMean, "µ");
            SetVIDVariable(enDataType.statsBumpAverageDiameterMax, _dDieAvgMaxCalc, "µ");
            SetVIDVariable(enDataType.statsBumpAverageDiameterStdDev, dDieAvgStd, "µ");
            SetVIDVariable(enDataType.inspectedYield, 100.0, "%"); // //100.0*(Nb Dies Inspected – Nb dies inspection error (could not be computed)  / Nb Dies Inspected 
            SetVIDVariable(enDataType.bumpMetrologyYield, dPctBumpMeasureYield, "%");
            //------------------------------------------------------------------
            //---- Ajout de la liste des Die
            //-------------------------------------------------------------------
            VidBF2DDieCollection VidBF2DDieCollection_var = new VidBF2DDieCollection();
            VidBF2DDieCollection_var.lsdieCollection = new List<VidBF2DDieCollectionModule>();

            foreach (Cluster2DDieDM cluster in _clusterList)
            {
                AddClusterInDieCollectionList(ref VidBF2DDieCollection_var.lsdieCollection, cluster);
            }
            SetVIDVariable(enDataType.dieCollection, VidBF2DDieCollection_var);
            //-------------------------------------------------------------------

            ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", vidReturnList);
        }

        private void AddClusterInDieCollectionList(ref List<VidBF2DDieCollectionModule> lsDieCollection, Cluster2DDieDM cluster)
        {
            VidBF2DDieCollectionModule die = new VidBF2DDieCollectionModule();

            String sDieFailStatus = (bool)cluster.characteristics[Cluster2DCharacteristics.isFailure] ? "Fail" : "Pass";
            String sAvgDiameterStatus = (bool)cluster.characteristics[Cluster2DCharacteristics.isDiameterFailure] ? "Fail" : "Pass";


            die.dieCol = cluster.DieIndexX;
            die.dieRow = cluster.DieIndexY;

            die.status = sDieFailStatus;
            die.totBumpFail = (int)cluster._NbMeasuresMissing + (int)cluster._NbMeasuresBadDiameter + (int)cluster._NbMeasuresBadOffset;
            die.bumpFailMissing = (int)cluster._NbMeasuresMissing;
            die.bumpFailDiameter = (int)cluster._NbMeasuresBadDiameter;
            die.bumFailOffset = (int)cluster._NbMeasuresBadOffset;
            die.averageDiameterStatus = sAvgDiameterStatus;
            die.bumpDiameterMin = cluster._MinMeasureDiameter;
            die.bumpDiameterMax = cluster._MaxMeasureDiameter;
            die.bumpDiameterStdDv = cluster._StdDevMeasureDiameter;
            die.bumpOffsetMin = cluster._MinMeasureOffset;
            die.bumpOffsetMax = cluster._MaxMeasureOffset;
            die.bumpOffsetAverage = cluster._MeanMeasureOffset;
            die.bumpOffsetStdDv = cluster._StdDevMeasureOffset;
            die.averageDiameter = cluster._MeanMeasureDiameter;


            lsDieCollection.Add(die);
        }

        //----------------------------------------------------------------------
        //------ SetVIDVariable : inititlaise la iste des VID à retourner
        //----------------------------------------------------------------------

        private void SetVIDVariable(enDataType dtaMetro, double measure, string s_unit)
        {
            //-------------------------------------------------------------
            // Vid
            //-------------------------------------------------------------
            DataCollect dtaCategory = new DataCollect();
            bool found = dataCollection.dataCollectList.TryGetValue(dtaMetro.ToString(), out dtaCategory);

            if (found)
            {
                VidMeasure2D vid = vidMap[dtaCategory.VID];

                if (vid.VidNumber <= 0)
                {
                    logDebug("DataCollect: " + dtaMetro.ToString() + " VID:" + vid.VidNumber + " " + vid.VidLabel);
                    return;   // Variable pas voulu
                }

                vid.Measure = measure;
                vid.UnitValue = s_unit;

                vidReturnList.Add(vid);
            }
        }
        //----------------------------------------------------------------------
        //------ SetVIDVariable : inititlaise la iste des die à retourner
        //----------------------------------------------------------------------

        private void SetVIDVariable(enDataType dtaMetro, VidBF2DDieCollection VidBF2DDieCollection)
        {
            //-------------------------------------------------------------
            // Vid
            //-------------------------------------------------------------
            DataCollect dtaCategory = new DataCollect();
            bool found = dataCollection.dataCollectList.TryGetValue(dtaMetro.ToString(), out dtaCategory);

            if (found)
            {
                VidMeasure2D vid = vidMap[dtaCategory.VID];

                if (vid.VidNumber <= 0)
                {
                    logDebug("DataCollect: " + dtaMetro.ToString() + " VID:" + vid.VidNumber + " " + vid.VidLabel);
                    return;   // Variable pas voulu
                }

                //if (dtaCategory.DataType == enDataType.dieCollection)
                //if (dtaCategory.DataType == enDataType.dieCollection)
                if (true)
                {
                    VidBF2DDieCollection.VidNumber = vid.VidNumber;
                    VidBF2DDieCollection.VidLabel = vid.VidLabel;

                    vidReturnList.Add(VidBF2DDieCollection);
                }
            }

        }
        //----------------------------------------------------------------------
        //
        //----------------------------------------------------------------------

        private void InitVariableList()
        {
            //-------------------------------------------------------------
            // Création de la liste des VIDs
            //-------------------------------------------------------------
            vidMap = new CustomExceptionDictionary<int, VidMeasure2D>(exceptionKeyName: "DataCollect");
            foreach (DataCollect dtaCatagory in dataCollection.dataCollectList.Values)
            {
                VidMeasure2D vid;
                bool exists = vidMap.TryGetValue(dtaCatagory.VID, out vid);
                if (!exists)
                {
                    vid = new VidMeasure2D();
                    vid.VidNumber = dtaCatagory.VID;
                    vid.VidLabel = dtaCatagory.VidLabel;
                    vidMap[dtaCatagory.VID] = vid;
                }
            }
        }
    }
}
