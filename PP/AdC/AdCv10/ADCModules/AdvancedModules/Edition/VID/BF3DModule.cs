using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcRobotExchange;

using AdcTools;

using HeightMeasurementDieModule;

using UnitySC.Shared.Tools;

namespace AdvancedModules.Edition.VID
{
    public class BF3DVIDModule : ModuleBase
    {
        public readonly BF3DParameter DataCollect_3Dion;

        private CustomExceptionDictionary<int, VidMeasure3D> vidMap;
        private List<VidBase> vidReturnList;

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
        // Constructeur
        //=================================================================
        public BF3DVIDModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            DataCollect_3Dion = new BF3DParameter(this, "DataCollect_3D");
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

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("BF3D3DVID",
                () =>
                {
                    try
                    {
                        ProcessBF3D3DVID();
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
        private void ProcessBF3D3DVID()
        {
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

            //------ calcul stats ---///
            double dPctWaferYield = 100.0 * (double)_DiePass / (double)_DieInspected;
            double dPctInspectedYield = 100.0; //100.0*(Nb Dies Inspected – Nb dies inspection error (could not be computed)  / Nb Dies Inspected 
            double dPctBumpMeasureYield = 100.0 * (double)_BumpPass / (double)_BumpInspected;
            double dBumpMean = _dBumpMeanCalc / (double)_BumpInspected;
            double dBumpStd = 0.0;
            foreach (Cluster3DDieHM cluster in _clusterList)
                dBumpStd += cluster._dieHMresult.ComputeDieMeasuresVariance(dBumpMean);
            dBumpStd /= (double)(_BumpInspected);
            dBumpStd = Math.Sqrt(dBumpStd);
            double dDieAvgMean = _dDieAvgMeanCalc / (double)_DieInspected;
            double dDieAvgStd = Math.Sqrt(_dDieStdCalc.Average(d => Math.Pow(d - dDieAvgMean, 2))); ;

            double dDieCoplaMean = 0.0;
            double dDieCoplaStd = 0.0;
            double dDieSubCoplaMean = 0.0;
            double dDieSubCoplaStd = 0.0;

            if (bHasMapCopla)
            {
                dDieCoplaMean = _dDieCoplaMeanCalc / (double)_DieInspected;
                dDieCoplaStd = Math.Sqrt(_dDieCoplaStdCalc.Average(d => Math.Pow(d - dDieCoplaMean, 2)));
            }
            if (bHasMapSubCopla)
            {
                dDieSubCoplaMean = _dDieSubCoplaMeanCalc / (double)_DieInspected;
                dDieSubCoplaStd = Math.Sqrt(_dDieSubCoplaStdCalc.Average(d => Math.Pow(d - dDieSubCoplaMean, 2)));
            }
            //------------------------//
            SetVIDVariable(enDataType_3D.bumpsperdie, NbMeasuresPerDie, "NA");
            SetVIDVariable(enDataType_3D.TotalWaferYield, dPctWaferYield, "NA");
            SetVIDVariable(enDataType_3D.DiesfailedError, 0, "NA"); // OPI : demander à romain pour les fail sont dans les reject et les fail à 0
            SetVIDVariable(enDataType_3D.diesInspected, _DieInspected, "NA");
            SetVIDVariable(enDataType_3D.diesPassed, _DiePass, "NA");
            SetVIDVariable(enDataType_3D.diesRejected, _DieFail, "NA");
            SetVIDVariable(enDataType_3D.BumpsInspected, _BumpInspected, "NA");
            SetVIDVariable(enDataType_3D.BumpsPassed, _BumpPass, "NA");
            SetVIDVariable(enDataType_3D.BumpsFailed, _BumpFail, "NA");
            SetVIDVariable(enDataType_3D.InspectedYield, dPctInspectedYield, "NA");
            SetVIDVariable(enDataType_3D.BumpMetrologyYield, dPctWaferYield, "NA");
            SetVIDVariable(enDataType_3D.diesfailedMissing, _DieFailDetailsBumpMissing, "NA");
            SetVIDVariable(enDataType_3D.diesfailedHeight, _DieFailDetailsBumpBadHeigth, "NA");
            SetVIDVariable(enDataType_3D.diesfailedAverageHeight, _DieFailDetailsAvgHeight, "NA");
            SetVIDVariable(enDataType_3D.diesfailedCoplanarity, _DieFailDetailsCopla, "NA");
            SetVIDVariable(enDataType_3D.diesfailedSubstrateCoplanarity, _DieFailDetailsSubCopla, "NA");
            SetVIDVariable(enDataType_3D.BumpsfailedMissing, _BumpFailDetailsMissing, "NA");
            SetVIDVariable(enDataType_3D.BumpsfailedHeight, _BumpFailDetailsBadHeight, "NA");
            SetVIDVariable(enDataType_3D.BumpHeightMin, _dBumpMinCalc, "NA");
            SetVIDVariable(enDataType_3D.BumpHeightMean, _dBumpMeanCalc, "µ");
            SetVIDVariable(enDataType_3D.BumpHeightMax, _dBumpMaxCalc, "µ");
            SetVIDVariable(enDataType_3D.BumpHeightStd, dBumpStd, "µ");
            SetVIDVariable(enDataType_3D.BumpAverageHeightMin, _dDieAvgMinCalc, "µ");
            SetVIDVariable(enDataType_3D.BumpAverageHeightMean, dDieAvgMean, "µ");
            SetVIDVariable(enDataType_3D.BumpAverageHeightMax, _dDieAvgMaxCalc, "µ");
            SetVIDVariable(enDataType_3D.BumpAverageHeightStd, dDieAvgStd, "µ");
            SetVIDVariable(enDataType_3D.CoplanarityMin, _dDieCoplaMinCalc, "µ");
            SetVIDVariable(enDataType_3D.CoplanarityMean, dDieCoplaMean, "µ");
            SetVIDVariable(enDataType_3D.CoplanarityMax, _dDieCoplaMaxCalc, "µ");
            SetVIDVariable(enDataType_3D.CoplanarityStd, dDieCoplaStd, "µ");
            SetVIDVariable(enDataType_3D.SubstrateCoplanarityMin, _dDieSubCoplaMinCalc, "µ");
            SetVIDVariable(enDataType_3D.SubstrateCoplanarityMean, dDieSubCoplaMean, "µ"); // //100.0*(Nb Dies Inspected – Nb dies inspection error (could not be computed)  / Nb Dies Inspected 
            SetVIDVariable(enDataType_3D.SubstrateCoplanarityMax, _dDieSubCoplaMaxCalc, "µ");
            SetVIDVariable(enDataType_3D.SubstrateCoplanarityStd, dDieSubCoplaStd, "µ");
            //------------------------------------------------------------------
            //---- Ajout de la liste des Die
            //-------------------------------------------------------------------
            VidBF3DDieCollection VidBF3DDieCollection_var = new VidBF3DDieCollection();
            VidBF3DDieCollection_var.lsdieCollection = new List<VidBF3DDieCollectionModule>();

            foreach (Cluster3DDieHM cluster in _clusterList)
            {
                AddClusterInDieCollectionList(ref VidBF3DDieCollection_var.lsdieCollection, cluster);
            }
            SetVIDVariable(enDataType_3D.dieCollection, VidBF3DDieCollection_var);
            //-------------------------------------------------------------------

            ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", vidReturnList);
        }
        //----------------------------------------------------------------------
        //------ AddClusterInDieCollectionList : creation d'un enregistrement die
        //----------------------------------------------------------------------
        private void AddClusterInDieCollectionList(ref List<VidBF3DDieCollectionModule> lsdieCollection, Cluster3DDieHM cluster)
        {
            VidBF3DDieCollectionModule die = new VidBF3DDieCollectionModule();

            String sDieFailStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isFailure] ? "Fail" : "Pass";
            String sAvgHeightStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isHeightFailure] ? "Fail" : "Pass";
            String sCoplaStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isCoplaFailure] ? "Fail" : "Pass";
            String sSubCoplaStatus = (bool)cluster.characteristics[Cluster3DCharacteristics.isSubCoplaFailure] ? "Fail" : "Pass";


            die.dieCol = cluster.DieIndexX;
            die.dieRow = cluster.DieIndexY;

            die.status = sDieFailStatus;
            die.totBumpFail = (int)cluster._NbMeasuresMissing + (int)cluster._NbMeasuresBadHeight;
            die.bumpFailMissing = (int)cluster._NbMeasuresMissing;
            die.bumpFaiHeight = (int)cluster._NbMeasuresBadHeight;

            die.averageHeightStatus = sAvgHeightStatus;
            die.coplanarityStatus = sCoplaStatus;
            die.subtrateCoplanarityStatus = sSubCoplaStatus;
            die.bumpHeightMin = cluster._dieHMresult.MinHeight;
            die.bumpHeightMax = cluster._dieHMresult.MaxHeight;
            die.bumpHeightStdDv = cluster._dieHMresult.StdDev;
            die.bumpHeightMean = cluster._dieHMresult.MeanHeight_um;
            die.coplanarityValue = cluster._dieHMresult.Coplanarity;
            die.SubstrateCoplanarityValue = cluster._dieHMresult.SubstrateCoplanarity;

            lsdieCollection.Add(die);
        }

        //----------------------------------------------------------------------
        //------ SetVIDVariable : inititlaise la iste des VID à retourner
        //----------------------------------------------------------------------

        private void SetVIDVariable(enDataType_3D dtaMetro, double measure, string s_unit)
        {
            //-------------------------------------------------------------
            // Vid
            //-------------------------------------------------------------
            DataCollect_3D dtaCategory = new DataCollect_3D();
            bool found = DataCollect_3Dion.DataCollect_3DList.TryGetValue(dtaMetro.ToString(), out dtaCategory);

            if (found)
            {
                VidMeasure3D vid = vidMap[dtaCategory.VID];

                if (vid.VidNumber <= 0)
                {
                    logDebug("DataCollect_3D: " + dtaMetro.ToString() + " VID:" + vid.VidNumber + " " + vid.VidLabel);
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

        private void SetVIDVariable(enDataType_3D dtaMetro, VidBF3DDieCollection VidBF3DDieCollection)
        {
            //-------------------------------------------------------------
            // Vid
            //-------------------------------------------------------------
            DataCollect_3D dtaCategory = new DataCollect_3D();
            bool found = DataCollect_3Dion.DataCollect_3DList.TryGetValue(dtaMetro.ToString(), out dtaCategory);


            if (found)
            {
                VidMeasure3D vid = vidMap[dtaCategory.VID];

                if (vid.VidNumber <= 0)
                {
                    logDebug("DataCollect: " + dtaMetro.ToString() + " VID:" + vid.VidNumber + " " + vid.VidLabel);
                    return;   // Variable pas voulu
                }

                //if (dtaCategory.DataType == enDataType_3D.dieCollection)
                if (true)
                {
                    VidBF3DDieCollection.VidNumber = vid.VidNumber;
                    VidBF3DDieCollection.VidLabel = vid.VidLabel;

                    vidReturnList.Add(VidBF3DDieCollection);
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
            vidMap = new CustomExceptionDictionary<int, VidMeasure3D>(exceptionKeyName: "DataCollect_3D");
            foreach (DataCollect_3D dtaCatagory in DataCollect_3Dion.DataCollect_3DList.Values)
            {
                VidMeasure3D vid;
                bool exists = vidMap.TryGetValue(dtaCatagory.VID, out vid);
                if (!exists)
                {
                    vid = new VidMeasure3D();
                    vid.VidNumber = dtaCatagory.VID;
                    vid.VidLabel = dtaCatagory.VidLabel;
                    vidMap[dtaCatagory.VID] = vid;
                }
            }
        }
    }
}
