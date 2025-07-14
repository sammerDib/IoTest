using System;
using System.Linq;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.ADC;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.ADCAS300Like.Service.ADCInterfaces;
using UnitySC.Dataflow.Configuration;
using UnitySC.PM.Shared;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.DataCollectionConverter;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.ADCAS300Like.Service
{
    public class ADCsConnectionManager: IADCCB
    {
        private String _lastMsgSent;
        private String _lastMsgRecv;

        private IADC _adcService;
        private ADCsConfigs _adcConfig;
        private ILogger _logger;
        private ICommonEventOperations _commonEventService;
        private IDFPostProcessCB _dfPostProcessCB;
        private DFServerConfiguration _dfServiceConfiguration;

        public ADCsConnectionManager() 
        { 

        }


        public void Init()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _commonEventService = ClassLocator.Default.GetInstance<ICommonEventOperations>();
            _dfPostProcessCB = ClassLocator.Default.GetInstance<IDFPostProcessCB>();
            _dfServiceConfiguration = ClassLocator.Default.GetInstance<DFServerConfiguration>();

            _adcConfig = ADCsConfigs.Init(ClassLocator.Default.GetInstance<IADCConfiguration>().ADCConfigurationFilePath);
            if ((_adcConfig.ADCItemsConfig == null) || (_adcConfig.ADCItemsConfig.Count == 0))
                return;
            else
            {
                if (_adcConfig.ADCItemsConfig.FindAll(c => c.Enabled).Count > 0)
                {
                    _adcService = new CADCService();
                    _adcService.RegisterCallback(this);

                    foreach (var adcconfig in _adcConfig.ADCItemsConfig)
                    {
                        if (adcconfig.Enabled)
                        {
                            _adcService.SetModeAutoConnect(adcconfig.ADCType, adcconfig.AutoConnection, adcconfig.DelayAutoConnect_Sec);
                        }
                        bool OneADCTypeInConfig = (_adcConfig.ADCItemsConfig.FindAll(c => c.ADCType == adcconfig.ADCType).Count == 1);
                        if (!OneADCTypeInConfig)
                            LogObject.ADCCommunicationLog(adcconfig.ADCType, $"ATTENTION !!!! Several same ADCType detected in ADCs configuration !!! ", _logger.LogDirectory + "\\ADC");
                    }
                }
            }
        }

        public void Shutdown()
        {
            _adcService?.UnregisterCallback(this);
        }
        public void ADC_AbortProcess(int errorStatus)
        {
            
        }

        public void ADC_Connected(ADCType serverType, bool bConnected, bool bDatabaseConnected)
        {
            if (!bConnected)
            {
                _lastMsgSent = "";
                _lastMsgRecv = "";
            }
        }

        public void ADC_GUIDisplay(ADCType serverType, string msg, string msgError)
        {                 
            if (!msgError.IsNullOrEmpty())
                _logger.Information($"ADC {serverType.ToString()} : {msg} - {msgError}");
            else
            {
                if ((_lastMsgSent != msg) && (_lastMsgRecv != msg))
                {
                    if (msg.Contains("[SEND]") || msg.Contains("[RECV]"))
                    {
                        if (msg.Contains("[SEND]"))
                        {
                            _lastMsgRecv = ""; // always update msg received if msg sent changed
                            _lastMsgSent = msg;
                        }
                        if (msg.Contains("[RECV]"))
                            _lastMsgRecv = msg;

                        LogObject.ADCCommunicationLog(serverType, $"{msg}", _logger.LogDirectory + "\\ADC");
                    }
                    else
                        _logger.Information($"ADC {serverType.ToString()} : {msg}");
                }
            }            
        }

        public void ADC_JobStatus(string status)
        {
            _logger.Information($"ADC status {status}");
        }

        public void ADC_SetEventResult(ADCType serverType, Object material, CWaferReport waferReport, EvenID eventID, int errorID, string msgError, LocalLogger resultsLogger)
        {
            int chamberID = _adcConfig.ADCItemsConfig.FirstOrDefault(r => r.ADCType == serverType).ChamberID;
            Identity _identity = new Identity(_dfServiceConfiguration.ToolKey, ActorType.ADC, chamberID);
            SecsVariableList results = null;
            Guid dfMaterialGuidFound = new Guid();
            try
            {
                DMTDataCollection dataCollection = new DMTDataCollection(waferReport, resultsLogger);
                var dcConvert = ClassLocator.Default.GetInstance<IDataCollectionConvert>();
                results = dcConvert.ConvertToSecsVariableList(dataCollection);
            }
            catch (Exception ex)
            {
                resultsLogger.Error($"Convert wafer report failed. wafer slot={waferReport?.iSlotID} - exception = {ex.Message} - {ex.StackTrace}");
            }
            switch (eventID)
            {
                case EvenID.eEventPostProcessStarted:
                    // Demarage => EvenID.eEventPostProcessStarted
                    switch (serverType)
                    {
                        case ADCType.atPSD_FRONTSIDE:
                            _commonEventService.Fire_CE(CEName.PostProcessStarted_Frontside, results);
                            _dfPostProcessCB.DFPostProcessStarted(_identity, Side.Front, waferReport.JobID, dfMaterialGuidFound);
                            break;
                        case ADCType.atPSD_BACKSIDE:
                            _commonEventService.Fire_CE(CEName.PostProcessStarted_Backside, results);
                            _dfPostProcessCB.DFPostProcessStarted(_identity, Side.Back, waferReport.JobID, dfMaterialGuidFound);
                            break;
                        default:
                            break;
                    }
                    break;
                case EvenID.eEventResultAvailable:
                    // Complete => EvenID.eEventResultAvailable
                    
                    if (results != null)
                    {
                        switch (serverType)
                        {
                            case ADCType.atPSD_FRONTSIDE:
                                _commonEventService.Fire_CE(CEName.WaferResultsAvailable_Frontside, results);
                                _dfPostProcessCB.DFPostProcessComplete(_identity, Side.Front, waferReport.JobID, dfMaterialGuidFound, DataflowRecipeStatus.Terminated);
                                break;
                            case ADCType.atPSD_BACKSIDE:
                                _commonEventService.Fire_CE(CEName.WaferResultsAvailable_Backside, results);
                                _dfPostProcessCB.DFPostProcessComplete(_identity, Side.Back, waferReport.JobID, dfMaterialGuidFound, DataflowRecipeStatus.Terminated);
                                break;
                            default:
                                break;
                        }
                        resultsLogger.Information("WaferMeasurementResults event sent to UTO with SecsVariableList data");
                    }
                    else
                    {
                        resultsLogger.Error("Failed to convert a wafer report into a SecsVariableList");
                    }
                    break;
                case EvenID.eEventResultError:
                    switch (serverType)
                    {
                        case ADCType.atPSD_FRONTSIDE:
                            _commonEventService.Fire_CE(CEName.WaferResultsError_Frontside, results);
                            _dfPostProcessCB.DFPostProcessComplete(_identity, Side.Front, waferReport.JobID, dfMaterialGuidFound, DataflowRecipeStatus.Error);
                            break;
                        case ADCType.atPSD_BACKSIDE:
                            _commonEventService.Fire_CE(CEName.WaferResultsError_Backside, results);
                            _dfPostProcessCB.DFPostProcessComplete(_identity, Side.Back, waferReport.JobID, dfMaterialGuidFound, DataflowRecipeStatus.Error);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }        

            //              EvenID.eEventResultError
        }

        public void ADC_SetWaferReport(ADCType serverType, CWaferReport waferReport, LocalLogger resultsLogger)
        {
            
        }

        public void ADC_WaferCompleted(ADCType serverType, Object material, int errorStatus)
        {
        }
    }
}
