using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using ADCControls;

using AdcRobotExchange;

using Serilog;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;


namespace AdcToRobot
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false)]
    internal class TransferToRobot : ITransferToRobot
    {
        private Dictionary<string, AdcStatus> AdcStatusMap;

        //=================================================================
        // Constructeur
        //=================================================================
        public TransferToRobot(Dictionary<string, AdcStatus> adcStatusMap)
        {
            AdcStatusMap = adcStatusMap;
        }

        //=================================================================
        // Implémentation de ITransferToRobot
        //=================================================================
        public void TransferVids(string Toolname, string UniqueID, List<VidBase> VidList)
        {
            AdcStatus adcStatus = AdcStatusMap[Toolname];

            lock (adcStatus.mutex)
            {
                Log.Information("Received VIDs for Wafer: " + UniqueID);
                CWaferReport creport = adcStatus.GetOrCreateCWaferReport(UniqueID);

                creport.DefectList = new List<CVIDProcessDefect>();
                creport.APCList = new List<CVIDProcessAPC>();
                creport.MeasurementList = new List<CVIDProcessMeasurement>();
                creport.DieCollectionList = new List<CVIDDieCollection>();


                foreach (VidBase vid in VidList)
                {
                    Log.Information("VID: " + vid.VidNumber + " " + vid.VidLabel);
                    if (vid is VidDefect)
                    {
                        VidDefect defect = (VidDefect)vid;
                        Log.Information("  size: " + defect.DefectSizePerBin[0] + "  " + defect.DefectSizePerBin[1] + "  " + defect.DefectSizePerBin[2] + "  " + defect.DefectSizePerBin[3]);
                        Log.Information("  nb: " + defect.DefectCountPerBin[0] + "  " + defect.DefectCountPerBin[1] + "  " + defect.DefectCountPerBin[2] + "  " + defect.DefectCountPerBin[3]);

                        CVIDProcessDefect cVIDProcessDefect = new CVIDProcessDefect();
                        cVIDProcessDefect.m_iVIDValue = defect.VidNumber;
                        cVIDProcessDefect.m_sVIDLabel = defect.VidLabel;
                        cVIDProcessDefect.m_iSubVIDValue = 0;
                        cVIDProcessDefect.m_sSubVIDLabel = "";
                        cVIDProcessDefect.m_lsSizeDefectBin = new List<double>(defect.DefectSizePerBin);
                        cVIDProcessDefect.m_lsCountDefefectBin = new List<double>(defect.DefectCountPerBin);

                        creport.DefectList.Add(cVIDProcessDefect);
                    }
                    else if (vid is VidEdge)
                    {
                        VidEdge measure = (VidEdge)vid;
                        Log.Information("  measure " + measure.MeasureNumber + " value: " + measure.Measure);

                        CVIDProcessMeasurement cVIDProcessMeasurement = new CVIDProcessMeasurement();
                        cVIDProcessMeasurement.m_enMeasurementType = enTypeMeasurement.en_EdgeMeasurement;
                        cVIDProcessMeasurement.m_iVIDValue = measure.VidNumber;
                        cVIDProcessMeasurement.m_sVIDLabel = measure.VidLabel;
                        cVIDProcessMeasurement.m_iSubVIDValue = 0;
                        cVIDProcessMeasurement.m_sSubVIDLabel = "";


                        cVIDProcessMeasurement.m_lfMeasurementValue = measure.Measure;
                        cVIDProcessMeasurement.m_sUnitValue = measure.UnitValue;
                        cVIDProcessMeasurement.m_iXcoordinate = measure.Xcoordinate;
                        cVIDProcessMeasurement.m_iYcoordinate = measure.Ycoordinate;
                        cVIDProcessMeasurement.m_RadiusValue = measure.RadiusValue;
                        cVIDProcessMeasurement.m_MeasurementNumber = measure.MeasureNumber;

                        creport.MeasurementList.Add(cVIDProcessMeasurement);
                    }
                    else if (vid is VidBowWarpMeasure)
                    {
                        VidBowWarpMeasure measure = (VidBowWarpMeasure)vid;
                        Log.Information("  measure " + measure.MeasureNumber + " value: " + measure.Measure);

                        CVIDProcessMeasurement cVIDProcessMeasurement = new CVIDProcessMeasurement();
                        cVIDProcessMeasurement.m_enMeasurementType = enTypeMeasurement.en_BowWarpMeasurement;
                        cVIDProcessMeasurement.m_iVIDValue = measure.VidNumber;
                        cVIDProcessMeasurement.m_sVIDLabel = measure.VidLabel;
                        cVIDProcessMeasurement.m_iSubVIDValue = 0;
                        cVIDProcessMeasurement.m_sSubVIDLabel = "";
                        cVIDProcessMeasurement.m_lfMeasurementValue = measure.Measure;
                        cVIDProcessMeasurement.m_sUnitValue = measure.UnitValue;
                        cVIDProcessMeasurement.m_iXcoordinate = measure.Xcoordinate;
                        cVIDProcessMeasurement.m_iYcoordinate = measure.Ycoordinate;
                        cVIDProcessMeasurement.m_MeasurementNumber = measure.MeasureNumber;

                        creport.MeasurementList.Add(cVIDProcessMeasurement);
                    }
                    else if (vid is VidApc)
                    {
                        VidApc apc = (VidApc)vid;
                        Log.Information("  apc module: " + string.Join(" ", apc.Modules.Select(m => m.ActorTypeId.ToString())));

                        CVIDProcessAPC cVIDProcessAPC = new CVIDProcessAPC();
                        cVIDProcessAPC.m_iVIDValue = apc.VidNumber;
                        cVIDProcessAPC.m_sVIDLabel = apc.VidLabel;
                        foreach (VidApcModule mod in apc.Modules)
                        {
                            CVIDProcessAPCModule cVIDProcessAPCModule = new CVIDProcessAPCModule();
                            cVIDProcessAPCModule.m_ModuleID = mod.ActorTypeId;
                            foreach (var kvp in mod.Dictionary)
                            {
                                cVIDProcessAPCModule.m_lsLabel.Add(kvp.Key);
                                cVIDProcessAPCModule.m_lsValue.Add(kvp.Value);
                            }
                        }

                        creport.APCList.Add(cVIDProcessAPC);
                    }
                    else if (vid is VidHaze)
                    {
                        VidHaze haze = (VidHaze)vid;
                        Log.Information("  measure " + haze.VidLabel + " value: " + haze.Measure);

                        CVIDProcessMeasurement cVIDProcessMeasurement = new CVIDProcessMeasurement();
                        cVIDProcessMeasurement.m_enMeasurementType = enTypeMeasurement.en_Haze;
                        cVIDProcessMeasurement.m_iVIDValue = haze.VidNumber;
                        cVIDProcessMeasurement.m_sVIDLabel = haze.VidLabel;
                        cVIDProcessMeasurement.m_lfMeasurementValue = haze.Measure;
                        cVIDProcessMeasurement.m_sUnitValue = haze.UnitValue;
                        cVIDProcessMeasurement.m_iXcoordinate = haze.Xcoordinate;
                        cVIDProcessMeasurement.m_iYcoordinate = haze.Ycoordinate;

                        creport.MeasurementList.Add(cVIDProcessMeasurement);
                    }
                    else if (vid is VidMeasure2D)
                    {
                        VidMeasure2D measure = (VidMeasure2D)vid;
                        Log.Information("  measure " + measure.Measure + " value: " + measure.Measure);

                        CVIDProcessMeasurement cVIDProcessMeasurement = new CVIDProcessMeasurement();
                        cVIDProcessMeasurement.m_enMeasurementType = enTypeMeasurement.en_2DMetro;
                        cVIDProcessMeasurement.m_iVIDValue = measure.VidNumber;
                        cVIDProcessMeasurement.m_sVIDLabel = measure.VidLabel;
                        cVIDProcessMeasurement.m_lfMeasurementValue = measure.Measure;
                        cVIDProcessMeasurement.m_sUnitValue = measure.UnitValue;

                        creport.MeasurementList.Add(cVIDProcessMeasurement);
                    }
                    else if (vid is VidMeasure3D)
                    {
                        VidMeasure3D measure = (VidMeasure3D)vid;
                        Log.Information("  measure " + measure.Measure + " value: " + measure.Measure);

                        CVIDProcessMeasurement cVIDProcessMeasurement = new CVIDProcessMeasurement();
                        cVIDProcessMeasurement.m_enMeasurementType = enTypeMeasurement.en_3DMetro;
                        cVIDProcessMeasurement.m_iVIDValue = measure.VidNumber;
                        cVIDProcessMeasurement.m_sVIDLabel = measure.VidLabel;
                        cVIDProcessMeasurement.m_lfMeasurementValue = measure.Measure;
                        cVIDProcessMeasurement.m_sUnitValue = measure.UnitValue;

                        creport.MeasurementList.Add(cVIDProcessMeasurement);
                    }

                    else if (vid is VidBF2DDieCollection)
                    {
                        VidBF2DDieCollection measure = (VidBF2DDieCollection)vid;
                        Log.Information("  measure " + measure.VidLabel + " value: " + measure.VidNumber);

                        CVIDDieCollection cVIDDieCollection = new CVIDDieCollection();
                        cVIDDieCollection.m_iVIDValue = measure.VidNumber;
                        cVIDDieCollection.m_sVIDLabel = measure.VidLabel;

                        SetDieCollectionTypeLabel2D(ref cVIDDieCollection);

                        cVIDDieCollection.lsDieCollectionRow = new List<CVIDDieCollectionRow>();

                        foreach (VidBF2DDieCollectionModule die in measure.lsdieCollection)
                        {
                            CVIDDieCollectionRow dieItem = new CVIDDieCollectionRow();

                            dieItem.row = new List<string>();
                            dieItem.row.Add(die.dieCol.ToString());
                            dieItem.row.Add(die.dieRow.ToString());
                            dieItem.row.Add(die.status.ToString());
                            dieItem.row.Add(die.totBumpFail.ToString());
                            dieItem.row.Add(die.bumpFailMissing.ToString());
                            dieItem.row.Add(die.bumpFailDiameter.ToString());
                            dieItem.row.Add(die.bumFailOffset.ToString());
                            dieItem.row.Add(die.averageDiameterStatus.ToString());
                            dieItem.row.Add(die.bumpDiameterMin.ToString());
                            dieItem.row.Add(die.bumpDiameterMax.ToString());
                            dieItem.row.Add(die.bumpDiameterStdDv.ToString());
                            dieItem.row.Add(die.bumpOffsetMin.ToString());
                            dieItem.row.Add(die.bumpOffsetMax.ToString());
                            dieItem.row.Add(die.bumpOffsetAverage.ToString());
                            dieItem.row.Add(die.bumpOffsetStdDv.ToString());
                            dieItem.row.Add(die.averageDiameter.ToString());

                            Log.Information(" add die  " + die.dieCol.ToString() + " column " + die.dieRow.ToString() + "row");

                            cVIDDieCollection.lsDieCollectionRow.Add(dieItem);
                        }

                        creport.DieCollectionList.Add(cVIDDieCollection);
                    }
                    else if (vid is VidBF3DDieCollection)
                    {
                        VidBF3DDieCollection measure = (VidBF3DDieCollection)vid;

                        CVIDDieCollection cVIDDieCollection = new CVIDDieCollection();
                        cVIDDieCollection.m_iVIDValue = measure.VidNumber;
                        cVIDDieCollection.m_sVIDLabel = measure.VidLabel;

                        SetDieCollectionTypeLabel3D(ref cVIDDieCollection);

                        cVIDDieCollection.lsDieCollectionRow = new List<CVIDDieCollectionRow>();

                        foreach (VidBF3DDieCollectionModule die in measure.lsdieCollection)
                        {
                            CVIDDieCollectionRow dieItem = new CVIDDieCollectionRow();
                            dieItem.row = new List<string>();
                            dieItem.row.Add(die.dieCol.ToString());
                            dieItem.row.Add(die.dieRow.ToString());
                            dieItem.row.Add(die.status.ToString());
                            dieItem.row.Add(die.totBumpFail.ToString());
                            dieItem.row.Add(die.bumpFailMissing.ToString());
                            dieItem.row.Add(die.bumpFaiHeight.ToString());
                            dieItem.row.Add(die.averageHeightStatus.ToString());
                            dieItem.row.Add(die.coplanarityStatus.ToString());
                            dieItem.row.Add(die.subtrateCoplanarityStatus.ToString());
                            dieItem.row.Add(die.bumpHeightMin.ToString());
                            dieItem.row.Add(die.bumpHeightMax.ToString());
                            dieItem.row.Add(die.bumpHeightStdDv.ToString());
                            dieItem.row.Add(die.bumpHeightMean.ToString());
                            dieItem.row.Add(die.coplanarityValue.ToString());
                            dieItem.row.Add(die.SubstrateCoplanarityValue.ToString());

                            cVIDDieCollection.lsDieCollectionRow.Add(dieItem);
                        }

                        creport.DieCollectionList.Add(cVIDDieCollection);

                    }
                    else
                    {
                        // nothing
                    }
                }
                Log.Information("");
            }
        }
        //-----------------------------------------------------------
        //-----------------------------------------------------------
        private void SetDieCollectionTypeLabel2D(ref CVIDDieCollection cVIDDieCollection)
        {
            cVIDDieCollection.lsColumnType = new List<enTypeData>();
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_string);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_string);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);

            cVIDDieCollection.lsColumnLabel = new List<string>();
            cVIDDieCollection.lsColumnLabel.Add("DieCol");
            cVIDDieCollection.lsColumnLabel.Add("DieRow");
            cVIDDieCollection.lsColumnLabel.Add("Status");
            cVIDDieCollection.lsColumnLabel.Add("TotBumpFail");
            cVIDDieCollection.lsColumnLabel.Add("bumpFailMissing");
            cVIDDieCollection.lsColumnLabel.Add("bumpFailDiameter");
            cVIDDieCollection.lsColumnLabel.Add("bumFailOffset");
            cVIDDieCollection.lsColumnLabel.Add("averageDiameterStatus");
            cVIDDieCollection.lsColumnLabel.Add("bumpDiameterMin");
            cVIDDieCollection.lsColumnLabel.Add("bumpDiameterMax");
            cVIDDieCollection.lsColumnLabel.Add("bumpDiameterStdDv");
            cVIDDieCollection.lsColumnLabel.Add("bumpOffsetMin");
            cVIDDieCollection.lsColumnLabel.Add("bumpOffsetMax");
            cVIDDieCollection.lsColumnLabel.Add("bumpOffsetAverage");
            cVIDDieCollection.lsColumnLabel.Add("bumpOffsetStdDv");
            cVIDDieCollection.lsColumnLabel.Add("averageDiameter");
        }
        //--------------------------------------------------------------------
        //--------------------------------------------------------------------
        private void SetDieCollectionTypeLabel3D(ref CVIDDieCollection cVIDDieCollection)
        {

            cVIDDieCollection.lsColumnType = new List<enTypeData>();
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_string);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_int);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_string);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_string);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_string);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);
            cVIDDieCollection.lsColumnType.Add(enTypeData.en_double);

            cVIDDieCollection.lsColumnLabel = new List<string>();
            cVIDDieCollection.lsColumnLabel.Add("DieCol");
            cVIDDieCollection.lsColumnLabel.Add("DieRow");
            cVIDDieCollection.lsColumnLabel.Add("Status");
            cVIDDieCollection.lsColumnLabel.Add("TotBumpFail");
            cVIDDieCollection.lsColumnLabel.Add("bumpFailMissing");
            cVIDDieCollection.lsColumnLabel.Add("bumpFaiHeight");
            cVIDDieCollection.lsColumnLabel.Add("averageHeightStatus");
            cVIDDieCollection.lsColumnLabel.Add("coplanarityStatus");
            cVIDDieCollection.lsColumnLabel.Add("subtrateCoplanarityStatus");
            cVIDDieCollection.lsColumnLabel.Add("bumpHeightMin;");
            cVIDDieCollection.lsColumnLabel.Add("bumpHeightMax;");
            cVIDDieCollection.lsColumnLabel.Add("bumpHeightStdDv;");
            cVIDDieCollection.lsColumnLabel.Add("bumpHeightMean;");
            cVIDDieCollection.lsColumnLabel.Add("coplanarityValue;");
            cVIDDieCollection.lsColumnLabel.Add("SubstrateCoplanarityValue;");
        }


        //=================================================================
        // Implémentation de ITransferToRobot
        //=================================================================
        public void TransferInputList(string Toolname, string UniqueID, List<AdcInput> AdcInputList)
        {
            AdcStatus adcStatus = AdcStatusMap[Toolname];
            lock (adcStatus.mutex)
            {
                Log.Information("Received InputList for Wafer: " + UniqueID);
                CWaferReport creport = adcStatus.GetOrCreateCWaferReport(UniqueID);

                creport.ADCInputDataTab = new List<CADCInputData>();
                foreach (AdcInput input in AdcInputList)
                {
                     if (!Enum.IsDefined(typeof(ResultType), input.InputResultType))
                    {
                        throw new InvalidOperationException($"Unkwon ResultType({input.InputResultType}) in TransferInputList");
                    }
                    Log.Information($"  Result Type : {(ResultType)input.InputResultType} Folder: {input.InputPictureDirectory}");

                    CADCInputData cADCInput = new CADCInputData();
                    creport.ADCInputDataTab.Add(cADCInput);
                    cADCInput.ResultType = (ResultType)input.InputResultType;
                    cADCInput.InputPictureDirectory = input.InputPictureDirectory;
                    //Obsolete
                    //cADCInput.ActorTypeId = (enModuleID)input.ActorTypeId;
                    //cADCInput.channelID = (enChannelID)input.ChannelID;

                    creport.ADCInputDataTab.Add(cADCInput);
                }
                Log.Information("");
            }
        }

        //=================================================================
        // Implémentation de ITransferToRobot
        //=================================================================
        public void TransferWaferReport(string Toolname, string UniqueID, WaferReport WaferReport)
        {
            AdcStatus adcStatus = AdcStatusMap[Toolname];
            lock (adcStatus.mutex)
            {
                Log.Information("Received WaferReport for Wafer: " + UniqueID);
                Log.Information("  Status: " + WaferReport.WaferStatus);
                
                CWaferReport creport = adcStatus.GetOrCreateCWaferReport(UniqueID);

                creport.sWaferID = WaferReport.WaferID;                
                if (Guid.TryParse(WaferReport.WaferGUID.Trim(), out Guid waferGUID))
                {
                    creport.gWaferGUID = waferGUID;
                }                            
                creport.enWaferStatus = (enWaferStatus)WaferReport.WaferStatus;
                creport.enAnalysisStatus = (enAnalysisStatus)WaferReport.AnalysisStatus;
                creport.iSlotID = int.Parse(WaferReport.SlotID);
                creport.iLoadPort = int.Parse(WaferReport.LoadPortID);
                creport.sProcessStartTime = WaferReport.ProcessStartTime;
                creport.sJobStartTime = WaferReport.JobStartTime;
                creport.JobID = WaferReport.JobID;
                creport.sLaunchingFileName = WaferReport.LaunchingFileName;
                creport.sLotID = WaferReport.LotID;
                creport.m_sFileKlarfName = WaferReport.KlarfFilename;
                creport.m_sOutputDataResultDirectory = WaferReport.OutputDirectory;
                creport.ErrorMessage = WaferReport.ErrorMessage;
                creport.defectCount_tot = int.Parse(WaferReport.defectCount_tot);

                if (WaferReport.ErrorMessage != null)
                    adcStatus.LastErrorMessage = WaferReport.ErrorMessage;

                Log.Information("Transfert wafer report :" + XML.SerializeToString(WaferReport));
            }
        }

        //=================================================================
        // Implémentation de ITransferToRobot
        //=================================================================
        public void TransferDataBaseStatus(string Toolname, eDataBaseType DBType, bool Connected)
        {
            AdcStatus adcStatus = AdcStatusMap[Toolname];
            lock (adcStatus.mutex)
            {
                Log.Information("Received DataBase Status for Tool: " + Toolname);
                Log.Information("  DB: " + DBType + " Status: " + Connected);

                switch (DBType)
                {
                    case eDataBaseType.ConfigurationDataBase:
                        adcStatus.IsConfigurationDatabaseConnected = Connected;
                        break;
                    case eDataBaseType.ResultDataBase:
                        adcStatus.IsResultDatabaseConnected = Connected;
                        break;
                    default:
                        throw new ApplicationException("unknown eDataBaseType: " + DBType);
                }

                Log.Information("");
            }
        }

    }
}
