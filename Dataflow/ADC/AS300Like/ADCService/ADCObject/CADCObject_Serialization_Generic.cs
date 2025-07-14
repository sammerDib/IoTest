using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ADCCommon;

using COHTServiceObject;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.ADC;
using UnitySC.ADCAS300Like.Common.CVIDObj;
using UnitySC.ADCAS300Like.Common.EException;
using UnitySC.ADCAS300Like.Common.ProcessModule;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.ADCAS300Like.Service.SocketClient;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.ADCAS300Like.Service
{
    public class CADCObject_Serialization_Generic : CADCObject
    {
        private OnServerConnectedDisconnected _onServerDisconnected;
        private CSocketClient_Serialization<CMessageADC> _socketClient;
        private bool _lastDatabaseConnectionState = true; // Connected by default
        private int _adcFormatVersionNumber;
        private ADCsConfigs _adcConfig;
        private ILogger _logger;

        public CADCObject_Serialization_Generic(ADCType serverType, string serverName, short portNum, OnSocketLog pSocketLog, CADCCallBack adcCallbacks, object pSynchroADCObject, LocalLogger resultLogger)
            : base(serverType, serverName, portNum, pSocketLog, adcCallbacks, pSynchroADCObject, resultLogger)
        {
            _adcConfig = ClassLocator.Default.GetInstance<ADCsConfigs>();
            _onServerDisconnected = new OnServerConnectedDisconnected(OnServerDisconnect);
            _socketClient = new CSocketClient_Serialization<CMessageADC>(serverType, base.ServerName, base.PortNum, pSocketLog, _onServerDisconnected);
            Finalize_Construction();
            _adcFormatVersionNumber = CConfiguration.ADC_FormatVersionNumber;
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            ResultsLogger = resultLogger;
        }

        // Overrides Methods
        public override bool IsADCConnected
        {
            get { return _socketClient.IsConnected; }
        }

        public override void ADCThread_Execute(Object oData)
        {
            // Cast entry parameter in CCommandObject
            CCommandObject Commands = oData as CCommandObject;
            //Init Thread
            ExitThread = false;
            // Infinity loop
            while (!ExitThread)
            {
                // Waiting Event received function call
                EvtStartReceiving.WaitOne(-1, true);
                if (ExitThread)
                    break;

                // Protected Commands Object data
                //lock (Commands.m_SynchronizationObject)
                while (Commands.CommandList.Count > 0)
                {
                    // Call function according to data received
                    switch ((FunctionCode)Commands.CommandList[0].Fct)
                    {
                        // Connect ADC
                        case FunctionCode.Fct_ConnectADC:
                            _socketClient.Connect();
                            break;
                        // Disconnect ADC
                        case FunctionCode.Fct_DisconnectADC:
                            _socketClient.Disconnect();
                            break;

                        default:
                            break;
                    }

                    Commands.CommandList.RemoveAt(0);
                }
            }
        }

        public override void ADCThread_Status(Object oData)
        {
            CMessageADC lResponse;
            CMessageADC lQuestion;
            String lTxt;
            bool lbLastDatabaseConnection = false;
            bool lbLastConnection = false;

            bool lbDeconnectionDetected = false;
            ExitThreadStatus = false;

            while (!ExitThreadStatus)
            {
                Thread.Sleep(SleepTime);
                if (_socketClient.IsConnected)
                {
                    _socketClient.Silence_NoMsg = false;
                    SleepTime = 3000;
                    lQuestion = new CMessageADC();
                    lQuestion.Error = new CErrorParameters();
                    lQuestion.Error.iErrorNumber = enError.en_NoError;
                    lQuestion.Error.DataBaseIsConnected = true;
                    lQuestion.Status = enumStatusExchangeADC.saOk;
                    lQuestion.Description = "Check status";
                    lQuestion.Command = enumCommandExchangeADC.caGetStatus;
                    _socketClient.SendMsg(lQuestion, out lResponse);
                    try
                    {
                        if (lResponse != null)
                        {
                            _lastDatabaseConnectionState = lResponse.Error.DataBaseIsConnected;
                            bool bStatusChanged = CheckWaferStatus(lResponse);
                            if (bStatusChanged)
                                WaferStatusChanged(lResponse);
                            else
                            if (lResponse.Error.iErrorNumber != enError.en_NoError)
                                WaferStatusError(lResponse);
                        }

                        if (!lbLastConnection || (lbLastDatabaseConnection != _lastDatabaseConnectionState))
                            AdcCallbacks.NotifyADC_Connected(ServerType, _socketClient.IsConnected, _lastDatabaseConnectionState);
                        lbLastDatabaseConnection = _lastDatabaseConnectionState;
                        lbLastConnection = true;
                    }
                    catch (Exception Ex)
                    {
                        EADCException Ex2 = new EADCException(ServerType, EADCException.NO_INVALID_ADC_RESPONSE, "Exception raised : Invalid ADC response", "in fct ADCThread_Status() - Error: " + Ex.Message, false);
                        EvtSocketLog.Invoke(ServerType, "Communication error", ETCPException.GetMessage(Ex2.ErrorCode));
                    }
                    lbDeconnectionDetected = true; // Si deconnexion realisée, signal connexion interrompu
                }
                else
                {
                    ADCItemConfig config = _adcConfig.ADCItemsConfig.Find(c => c.ADCType == ServerType);

                    if (lbDeconnectionDetected) 
                    {
                        if (config.Enabled)
                            AdcCallbacks.NotifyADC_Connected(ServerType, false, false);                        
                    }
                    if (AutoConnect)
                    {
                        lTxt = _socketClient.ClientName + " Automatic try connect";
                        if (lbLastConnection) EvtSocketLog.Invoke(ServerType, lTxt, "");
                        _socketClient.Silence_NoMsg = true;
                        _socketClient.Connect(); // En Synchrone pour avoir la variable m_SocketClient.IsConnected à jour

                        if ((config.EnabledCheckingConnection) && (config.Enabled))
                            AdcCallbacks.NotifyADC_Connected(ServerType, _socketClient.IsConnected, _lastDatabaseConnectionState);
                        if (!_socketClient.IsConnected)
                        {
                            lTxt = _socketClient.ClientName + " Loop connection available [retry every " + DelayAutoConnect_Sec.ToString() + "sec]";
                            if (lbLastConnection) EvtSocketLog.Invoke(ServerType, lTxt, "");
                            Thread.Sleep(DelayAutoConnect_Sec);
                        }
                    }
                    lbLastConnection = false;
                    lbDeconnectionDetected = false; // Reset de la detection de la deconnexion
                }
                if (ExitThread)
                    break;
            }
        }

        private void WaferStatusError(CMessageADC lResponse)
        {
            if (lResponse != null)
            {
                CWaferReport lCurrentWaferReport = lResponse.pCWaferReport;
                String lTxt = _socketClient.ClientName + " status error.";
                EvtSocketLog.Invoke(ServerType, lTxt, "");
                String strSlot = "NULL";
                String strLoadPortID = "NULL";
                String strLotID = "NULL";
                String strWaferID = "NULL";
                Material material = new Material();
                if (lCurrentWaferReport != null)
                {
                    strSlot = lCurrentWaferReport.iSlotID.ToString();
                    strLoadPortID = lCurrentWaferReport.iLoadPort.ToString();
                    strLotID = lCurrentWaferReport.sLotID;
                    strWaferID = lCurrentWaferReport.sWaferID;

                    material.LoadportID = lCurrentWaferReport.iLoadPort;
                    material.SlotID = lCurrentWaferReport.iSlotID;
                    material.LotID = lCurrentWaferReport.sLotID;
                    material.ProcessJobID = lCurrentWaferReport.JobID;
                    material.SubstrateID = lCurrentWaferReport.sWaferID;
                }

                String strDataResult = GetDataResultFromWaferStatus(lCurrentWaferReport);
                lTxt = _socketClient.ClientName + "WID=" + strWaferID + " - Results LP=" + strLoadPortID + " W=" + strSlot + ": " + strDataResult;
                EvtSocketLog.Invoke(ServerType, lTxt, "");

                CADCResultsList lADCResults = new CADCResultsList(ServerType, ResultsLogger);

                String lMsgError2 = "ERROR: " + strDataResult + " ClassError=" + lResponse.Error.iClassError + " - ErrorNumber=" + lResponse.Error.iErrorNumber + " - ErrorMessage=" + lResponse.Error.sErrorMessage;

                try
                {
                    // Recuperation données du Message
                    lADCResults.LogResult = new StringBuilder();
                    lADCResults.LogResult.AppendLine("========================================================================================================================");
                    lADCResults.LogResult.AppendLine("Results wafer | LP=" + strLoadPortID + " | Slot=" + strSlot);
                    lADCResults.LogResult.AppendLine("------------------------------------------------------------------------");
                    lADCResults.LogResult.AppendLine("Data received =" + strDataResult);
                    lADCResults.LogResult.AppendLine("------------------------------------------------------------------------");                    
                    lADCResults.LogResult.AppendLine(lMsgError2);
                    WriteADCLog(lADCResults.LogResult.ToString(), _logger.LogDirectory);
                }
                catch (Exception Ex)
                {
                    String lMsgError = "Exception raised: " + Ex.Message + " - " + Ex.StackTrace;
                    WriteADCLog(lMsgError, _logger.LogDirectory);
                    lTxt = _socketClient.ClientName + " LotID " + strLotID + " - W" + strSlot + " error during data traitment: SetDataResults().";
                    EvtSocketLog.Invoke(ServerType, lTxt, "");
                }
                lock (SynchroADCObject)
                {
                    try
                    {
                        // Sauvegarde des donnees recues
                        AdcCallbacks.NotifyADC_SetWaferReport(ServerType, lCurrentWaferReport, ResultsLogger);
                    }
                    catch
                    {
                        lTxt = _socketClient.ClientName + " LotID " + strLotID + " - W" + strSlot + " error during data traitment: SendADCResultsToCIMConnect().";
                        EvtSocketLog.Invoke(ServerType, lTxt, "");
                    }
                }

                // Display to GUI
                lTxt = _socketClient.ClientName + lMsgError2;
                EvtSocketLog.Invoke(ServerType, lTxt, "");

                // Event Error
                lTxt = _socketClient.ClientName + "Event sent: RESULTS ERROR";
                EvtSocketLog.Invoke(ServerType, lTxt, "");

                // Trigger Event Result_Error
                AdcCallbacks.NotifyADC_SetEventResult(ServerType, null, lCurrentWaferReport, EvenID.eEventResultError, (int)lResponse.Error.iErrorNumber, lMsgError2, ResultsLogger);

                // Clear le status
                CMessageADC lClearResponse;
                CMessageADC lClearWaferStatus = new CMessageADC();
                lClearWaferStatus.Command = enumCommandExchangeADC.caClear;
                lClearWaferStatus.aUniqueIDSearch = lResponse.aUniqueIDSearch;
                lClearWaferStatus.Description = "Clear an error";
                lClearWaferStatus.Status = enumStatusExchangeADC.saOk;
                lClearWaferStatus.Error = new CErrorParameters();
                lClearWaferStatus.Error.iErrorNumber = enError.en_NoError;
                lClearWaferStatus.Error.DataBaseIsConnected = true;
                _socketClient.SendMsg(lClearWaferStatus, out lClearResponse);
            }
        }

        public void WaferStatusChanged(CMessageADC pResponse)
        {
            String lTxt = "";
            for (int iCurrent = 0; iCurrent < pResponse.lsCompletedUniqueId.Count; iCurrent++)
            {
                CMessageADC lAskWaferReport = new CMessageADC();
                lAskWaferReport.Command = enumCommandExchangeADC.caGetResult;
                lAskWaferReport.Error = new CErrorParameters();
                lAskWaferReport.Error.iErrorNumber = enError.en_NoError;
                lAskWaferReport.Error.DataBaseIsConnected = true;
                lAskWaferReport.aUniqueIDSearch = pResponse.lsCompletedUniqueId[iCurrent];
                lAskWaferReport.Description = "Ask a wafer report";
                lAskWaferReport.Status = enumStatusExchangeADC.saOk;
                CMessageADC lResponse;
                _socketClient.SendMsg(lAskWaferReport, out lResponse);
                if (lResponse != null)
                {
                    CWaferReport lCurrentWaferReport = lResponse.pCWaferReport;
                    lTxt = "=======================================================================================================================================================";
                    WriteADCLog(lTxt, _logger.LogDirectory);
                    lTxt = _socketClient.ClientName + " status changed.";
                    WriteADCLog(lTxt, _logger.LogDirectory);
                    String strSlot = lCurrentWaferReport.iSlotID.ToString();
                    String strLoadPortID = lCurrentWaferReport.iLoadPort.ToString();
                    String strLotID = lCurrentWaferReport.sLotID;
                    String strState = ((int)(lCurrentWaferReport.enWaferStatus)).ToString();
                    Material material = new Material();
                    material.LoadportID = lCurrentWaferReport.iLoadPort;
                    material.SlotID = lCurrentWaferReport.iSlotID;
                    material.LotID= lCurrentWaferReport.sLotID;
                    material.ProcessJobID = lCurrentWaferReport.JobID;
                    material.SubstrateID = lCurrentWaferReport.sWaferID;

                    switch (lCurrentWaferReport.enWaferStatus) // Status du wafer
                    {
                        case enWaferStatus.en_unprocessed: break;
                        case enWaferStatus.en_processing:
                            lTxt = _socketClient.ClientName + " LotID " + strLotID + " - W" + strSlot + " is beeing processed ";
                            EvtSocketLog.Invoke(ServerType, lTxt, "");

                            AdcCallbacks.NotifyADC_SetEventResult(ServerType, material, lCurrentWaferReport, EvenID.eEventPostProcessStarted, 0, "", ResultsLogger);
                            break;

                        case enWaferStatus.en_complete:
                            lTxt = _socketClient.ClientName + " LotID " + strLotID + " - W" + strSlot + " finished.";
                            EvtSocketLog.Invoke(ServerType, lTxt, "");

                            int ret = SetDataFromWaferReport(lCurrentWaferReport);

                            if (ret < 0)
                                goto default;
                           
                            bool dataDetected = false;                            
                            if ((lCurrentWaferReport.DefectList != null) && (lCurrentWaferReport.DefectList.Count > 0))
                            {
                                lTxt = _socketClient.ClientName + " Results available: DEFECT RESULTS";
                                WriteADCLog(lTxt, _logger.LogDirectory);
                                    dataDetected = true;
                            }

                            if ((lCurrentWaferReport.MeasurementList != null) && (lCurrentWaferReport.MeasurementList.Count > 0))
                            {
                                lTxt = _socketClient.ClientName + " Results available: MEASUREMENT RESULTS";
                                WriteADCLog(lTxt, _logger.LogDirectory);
                                    dataDetected = true;
                            }
                            if ((lCurrentWaferReport.APCList != null) && (lCurrentWaferReport.APCList.Count > 0))
                            {
                                lTxt = _socketClient.ClientName + " Results available: APC RESULTS";
                                WriteADCLog(lTxt, _logger.LogDirectory);
                                    dataDetected = true;
                             }    
                            if (!dataDetected)
                            {
                                lTxt = _socketClient.ClientName + " No data in report detected";
                                WriteADCLog(lTxt, _logger.LogDirectory);
                                goto default;
                            }


                            AdcCallbacks.NotifyADC_SetEventResult(ServerType, material, lCurrentWaferReport, EvenID.eEventResultAvailable, 0, "", ResultsLogger);
                            AdcCallbacks.NotifyADC_WaferCompleted(ServerType, material, (int)enError.en_NoError);

                            // Clear le status
                            lAskWaferReport.Command = enumCommandExchangeADC.caClear;
                            lAskWaferReport.Error = new CErrorParameters();
                            lAskWaferReport.Error.iErrorNumber = enError.en_NoError;
                            lAskWaferReport.Error.DataBaseIsConnected = true;
                            lAskWaferReport.aUniqueIDSearch = pResponse.lsCompletedUniqueId[iCurrent];
                            lAskWaferReport.Description = "Clear a wafer report";
                            lAskWaferReport.Status = enumStatusExchangeADC.saOk;
                            _socketClient.SendMsg(lAskWaferReport, out lResponse);
                            break;

                        case enWaferStatus.en_error:
                            String lMsgError2 = "ERROR: ClassError=" + pResponse.Error.iClassError + " - ErrorNumber=" + pResponse.Error.iErrorNumber + " - ErrorMessage=" + pResponse.Error.sErrorMessage;
                            lTxt = _socketClient.ClientName + " LotID " + strLotID + " - W" + strSlot + " finished with ERROR - " + lMsgError2;
                            EvtSocketLog.Invoke(ServerType, lTxt, "");

                            SetDataFromWaferReport(lCurrentWaferReport);

                            goto default;
                        default:
                            // Event Error
                            String lMsgError3 = "ERROR: ClassError=" + pResponse.Error.iClassError + " - ErrorNumber=" + pResponse.Error.iErrorNumber + " - ErrorMessage=" + pResponse.Error.sErrorMessage;

                            // Trigger Event Result_Error
                            AdcCallbacks.NotifyADC_SetEventResult(ServerType, material, lCurrentWaferReport, EvenID.eEventResultError, (int)pResponse.Error.iErrorNumber, lMsgError3, ResultsLogger);
                            AdcCallbacks.NotifyADC_WaferCompleted(ServerType, material, (int)lResponse.Error.iErrorNumber);

                            // Clear le status
                            lAskWaferReport.Command = enumCommandExchangeADC.caClear;
                            lAskWaferReport.Error = new CErrorParameters();
                            lAskWaferReport.Error.iErrorNumber = enError.en_NoError;
                            lAskWaferReport.Error.DataBaseIsConnected = true;
                            lAskWaferReport.aUniqueIDSearch = pResponse.lsCompletedUniqueId[iCurrent];
                            lAskWaferReport.Description = "Clear a wafer report";
                            lAskWaferReport.Status = enumStatusExchangeADC.saOk;
                            _socketClient.SendMsg(lAskWaferReport, out lResponse);
                            break;
                    }
                }
            }
        }

        private int SetDataFromWaferReport(CWaferReport currentWaferReport)
        {
            CADCResults lADCResults;

            if (currentWaferReport == null) return -1;

            String strSlot = currentWaferReport.iSlotID.ToString();
            String strLoadPortID = currentWaferReport.iLoadPort.ToString();
            String strLotID = currentWaferReport.sLotID;
            String strState = ((int)(currentWaferReport.enWaferStatus)).ToString();
            String lTxt;
            // Recupere les infos du wafer concerné
            try
            {
                String strDataResult = GetDataResultFromWaferStatus(currentWaferReport);
                lTxt = _socketClient.ClientName + "WID=" + currentWaferReport.sWaferID + " - Results LP" + strLoadPortID + "W" + strSlot + ": " + strDataResult;
                WriteADCLog(lTxt, _logger.LogDirectory);
                
                switch (_adcFormatVersionNumber)
                {
                    case 1:
                        lADCResults = new CADCResultsList_FVN1(ServerType, ResultsLogger);
                        break;
                    case 2:
                    default:
                        lADCResults = new CADCResultsList_FVN2(ServerType, ResultsLogger);
                        break;
                }
                // Recuperation données du Message
                lADCResults.LogResult = new StringBuilder();
                lADCResults.LogResult.AppendLine("========================================================================");
                lADCResults.LogResult.AppendLine("Results wafer | LP=" + strLoadPortID + " | Slot=" + strSlot);
                lADCResults.LogResult.AppendLine("------------------------------------------------------------------------");
                lADCResults.LogResult.AppendLine("Data received =" + strDataResult);
                lADCResults.LogResult.AppendLine("------------------------------------------------------------------------");
                WriteADCLog(lADCResults.LogResult.ToString(), _logger.LogDirectory);
            }
            catch (Exception Ex)
            {
                String lMsgError = "Exception raised: " + Ex.Message + " - " + Ex.StackTrace;
                WriteADCLog(lMsgError, _logger.LogDirectory);
                lTxt = _socketClient.ClientName + " LotID " + strLotID + " - W" + strSlot + " error during data traitment: SetDataResults().";
                EvtSocketLog.Invoke(ServerType, lTxt, "");
                return -1;
            }
            lock (SynchroADCObject)
            {
                try
                {
                    // Sauvegarde des donnees recues
                    AdcCallbacks.NotifyADC_SetWaferReport(ServerType, currentWaferReport, ResultsLogger);
                }
                catch
                {
                    lTxt = _socketClient.ClientName + " LotID " + strLotID + " - W" + strSlot + " error during data traitment: SendADCResultsToCIMConnect().";
                    EvtSocketLog.Invoke(ServerType, lTxt, "");
                    return -1;
                }

            }
            return 0;
        }

        // Disconnection
        private void OnServerDisconnect(ADCType serverType)
        {
            try
            {
                switch (serverType)
                {
                    case ADCType.atEDGE:
                        if (_socketClient.IsConnected) // Assure la deconnexion
                            _socketClient.Disconnect();
                        break;

                    default:
                        break;
                }
            }
            catch (EADCException Ex)
            {
                Ex.DisplayError(AdcCallbacks, Ex.ErrorCode);
            }
        }

        private bool CheckWaferStatus(CMessageADC response)
        {
            // Check Wafer status exist
            if (response.lsCompletedUniqueId.Count == 0)
            {
                return false;
            }
            return true;
        }

        private string GetDataResultFromWaferStatus(CWaferReport pCurrentWaferReport)
        {
            String lResult = String.Empty;
            String lResult_LastValidation = String.Empty;

            if (pCurrentWaferReport != null)
            {
                try
                {
                    if (pCurrentWaferReport.JobID == null)
                        lResult = "#" + pCurrentWaferReport.sWaferID + ";State=" + pCurrentWaferReport.enWaferStatus.ToString() + ";LotID=" + pCurrentWaferReport.sLotID + ";S=" + pCurrentWaferReport.iSlotID.ToString() + ";LP=" + pCurrentWaferReport.iLoadPort.ToString() + ";";
                    else
                        lResult = "#" + pCurrentWaferReport.sWaferID + ";State=" + pCurrentWaferReport.enWaferStatus.ToString() + ";LotID=" + pCurrentWaferReport.sLotID + ";S=" + pCurrentWaferReport.iSlotID.ToString() + ";LP=" + pCurrentWaferReport.iLoadPort.ToString() + ";" + "JobID=" + pCurrentWaferReport.JobID + ";" + "JobStartTime=" + pCurrentWaferReport.sJobStartTime + ";";
                    lResult_LastValidation = lResult;

                    if (pCurrentWaferReport.DefectList != null)
                    {
                        for (int i = 0; i < pCurrentWaferReport.DefectList.Count; i++)
                        {
                            lResult += pCurrentWaferReport.DefectList[i].m_sVIDLabel + "=" + pCurrentWaferReport.DefectList[i].m_iVIDValue.ToString() + ";";
                            List<CVIDProcessDefect> lDefectList = pCurrentWaferReport.DefectList;
                            for (int k = 0; k < lDefectList[i].m_lsCountDefefectBin.Count; k++)
                            {
                                lResult += "Count_bin" + Convert.ToString(k + 1) + "=" + lDefectList[i].m_lsCountDefefectBin[k] + ";";
                            }
                            for (int l = 0; l < lDefectList[i].m_lsSizeDefectBin.Count; l++)
                            {
                                lResult += "Size_bin" + Convert.ToString(l + 1) + "=" + lDefectList[i].m_lsSizeDefectBin[l] + ";";
                            }
                        }
                    }

                    lResult_LastValidation = lResult;


                    if (pCurrentWaferReport.MeasurementList != null)
                    {
                        for (int k = 0; k < pCurrentWaferReport.MeasurementList.Count; k++)
                        {
                            lResult += pCurrentWaferReport.MeasurementList[k].m_sVIDLabel + "=" + pCurrentWaferReport.MeasurementList[k].m_iVIDValue.ToString() + ";";
                            List<CVIDProcessMeasurement> lMeasurementList = pCurrentWaferReport.MeasurementList;

                            //for (int k = 0; k < lMeasurementList.Count; k++)
                            //{
                            try
                            {
                                enTypeMeasurement lTryTypeExist = lMeasurementList[k].m_enMeasurementType;
                                switch (lTryTypeExist)
                                {
                                    case enTypeMeasurement.en_EdgeMeasurement:
                                    default:
                                        lResult += "MeasureLabel=" + lMeasurementList[k].m_sSubVIDLabel + ";MeasureValue=" + lMeasurementList[k].m_lfMeasurementValue.ToString() + " " + lMeasurementList[k].m_sUnitValue + ";MeasurePos=(" + lMeasurementList[k].m_iXcoordinate.ToString() + "," + lMeasurementList[k].m_iYcoordinate.ToString() + ");MeasureNbr=" + lMeasurementList[k].m_iSubVIDValue.ToString() + ";";
                                        break;

                                    case enTypeMeasurement.en_BowWarpMeasurement:
                                        lResult += "MeasureLabel=" + lMeasurementList[k].m_sSubVIDLabel + ";MeasureValue=" + lMeasurementList[k].m_lfMeasurementValue.ToString() + " " + lMeasurementList[k].m_sUnitValue + ";MeasureRadius=" + lMeasurementList[k].m_RadiusValue.ToString() + ";MeasureNbr=" + lMeasurementList[k].m_MeasurementNumber.ToString() + ";";
                                        break;
                                    case enTypeMeasurement.en_2DMetro:
                                    case enTypeMeasurement.en_3DMetro:
                                    case enTypeMeasurement.en_Haze:
                                        lResult += "MeasureLabel=" + lMeasurementList[k].m_sSubVIDLabel + ";MeasureValue=" + lMeasurementList[k].m_lfMeasurementValue.ToString() + ";";
                                        break;
                                }
                            }
                            catch
                            {
                                // Type edge par defaut
                                lResult += "MeasureLabel=" + lMeasurementList[k].m_sSubVIDLabel + ";MeasureValue=" + lMeasurementList[k].m_lfMeasurementValue.ToString() + " " + lMeasurementList[k].m_sUnitValue + ";MeasurePos=(" + lMeasurementList[k].m_iXcoordinate.ToString() + "," + lMeasurementList[k].m_iYcoordinate.ToString() + ");MeasureNbr=" + lMeasurementList[k].m_iSubVIDValue.ToString() + ";";
                            }
                            //}
                        }
                    }

                    lResult_LastValidation = lResult;
                    if (pCurrentWaferReport.APCList != null)
                    {
                        List<CVIDProcessAPC> lAPCList = pCurrentWaferReport.APCList;
                        for (int i = 0; i < lAPCList.Count; i++)
                        {
                            lResult += lAPCList[i].m_sVIDLabel + "=" + lAPCList[i].m_iVIDValue.ToString() + ";";
                            List<CVIDProcessAPCModule> lAPCModuleList = lAPCList[i].m_DataAPCModule;
                            if (lAPCModuleList != null)
                            {
                                for (int j = 0; j < lAPCModuleList.Count; j++)
                                {
                                    lResult += "Module ID =" + lAPCModuleList[j].m_ModuleID.ToString() + ";";
                                    List<String> lLabelList = lAPCModuleList[i].m_lsLabel;
                                    List<double> lValueList = lAPCModuleList[i].m_lsValue;
                                    if ((lLabelList != null) && (lValueList != null))
                                    {
                                        for (int k = 0; k < lLabelList.Count; k++)
                                        {
                                            lResult += lLabelList[k].ToString() + "=" + lValueList[k].ToString() + ";";
                                        }
                                    }
                                    else
                                        lResult += "LabelList or lValueList is null";
                                }
                            }
                            else
                                lResult += "APCModuleList is null";
                        }
                    }

                    lResult_LastValidation = lResult;
                    if (pCurrentWaferReport.DieCollectionList != null)
                    {
                        List<CVIDDieCollection> lDieCollList = pCurrentWaferReport.DieCollectionList;
                        for (int i = 0; i < lDieCollList.Count; i++)
                        {
                            lResult += lDieCollList[i].m_sVIDLabel + "=" + lDieCollList[i].m_iVIDValue.ToString() + ";";
                            List<String> lColNameList = lDieCollList[i].lsColumnLabel;
                            List<enTypeData> lColTypeList = lDieCollList[i].lsColumnType;
                            List<CVIDDieCollectionRow> lDieColRowList = lDieCollList[i].lsDieCollectionRow;
                            if (lColNameList != null)
                            {
                                lResult += "Title=";
                                for (int j = 0; j < lColNameList.Count; j++)
                                {
                                    lResult += lColNameList[j] + "#";
                                }
                                lResult = lResult.Remove(lResult.Length - 1, 1);

                                for (int j = 0; j < lDieCollList.Count; j++)
                                {
                                    lResult += ";Row" + j.ToString() + "=";
                                    for (int k = 0; k < lDieColRowList[j].row.Count; k++)
                                    {
                                        lResult += lDieColRowList[j].row[k].ToString() + "#";
                                    }
                                    lResult = lResult.Remove(lResult.Length - 1, 1);
                                }
                            }
                            else
                                lResult += "DieCollection List is null";
                        }
                    }
                }
                catch
                {
                    return lResult_LastValidation;
                }
            }
            else
                lResult = "# No results - Wafer report empty (NULL)";
            return lResult;
        }

        public override bool IsADCConnectedToDatabase
        {
            get { return _lastDatabaseConnectionState; }
        }

        public LocalLogger ResultsLogger { get; set; }

        public override void ShutdownSocket()
        {
            _socketClient.SocketShutdown();
        }

        public void WriteADCLog (String data, String path)
        {
            ResultsLogger.Information(data);
        }
    }
}
