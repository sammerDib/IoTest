using System;
using System.Linq;

using ADCControls;

using Serilog;


namespace AdcToRobot
{
    internal class SocketRobot
    {
        protected CSocketServer<CMessageADC> SocketServer;
        private AdcStatus AdcStatus;

        //=================================================================
        // Constructeur
        //=================================================================
        public SocketRobot(AdcStatus AdcStatus)
        {
            this.AdcStatus = AdcStatus;
        }

        //=================================================================
        // Start / Stop
        //=================================================================
        public void Start(string servername, int port)
        {
            SocketServer = new CSocketServer<CMessageADC>(servername, (short)port, OnSocketLog, OnClientConnected, OnClientDisconnected, OnServerDisconnected, OnMessageReceived);
            SocketServer.Listen();
        }

        public void Stop()
        {
            if (SocketServer.IsConnected)
                SocketServer.Disconnect();
            SocketServer.SocketShutdown();
        }

        //=================================================================
        // Callbacks
        //=================================================================
        private void OnSocketLog(String Msg, String MsgError)
        {
            log(Msg);
            logError(MsgError);
        }

        private void OnMessageReceived(String data)
        {
            //-------------------------------------------------------------
            // Décodage du message reçu
            //-------------------------------------------------------------
            CMessageADC MsgReceived;
            try
            {
                MsgReceived = ToolsClass_Utilities.DeSerialize<CMessageADC>(data);
            }
            catch (Exception Ex)
            {
                log("Invalid message received from Robot:\n" + Ex);
                return;
            }

            //-------------------------------------------------------------
            // Envoi de la réponse
            //-------------------------------------------------------------
            try
            {
                CMessageADC MsgToSend = CreateReply(MsgReceived);
                SocketServer.SendMsg(MsgToSend);
            }
            catch (Exception ex)
            {
                logError("Failed to send response:\n" + ex);
            }
        }

        private void OnClientConnected(IAsyncResult Async)
        {
            log("Client connected");
        }

        private void OnServerDisconnected()
        {
            log("Server disconnected");
        }

        private void OnClientDisconnected(IAsyncResult Async)
        {
            log("Client disconnected");
        }

        //=================================================================
        // Création de la réponse
        //=================================================================
        private CMessageADC CreateReply(CMessageADC MsgReceived)
        {
            lock (AdcStatus.mutex)
            {
                CMessageADC MsgSend = new CMessageADC();
                MsgSend.Status = enumStatusExchangeADC.saOk;
                MsgSend.Command = enumCommandExchangeADC.caAcknowledge;

                MsgSend.Error = new CErrorParameters();
                MsgSend.Error.DataBaseIsConnected = AdcStatus.IsConfigurationDatabaseConnected & AdcStatus.IsResultDatabaseConnected;
                MsgSend.Error.iClassError = enClassName.en_SpecificClass;
                if (AdcStatus.LastErrorMessage == null || AdcStatus.LastErrorMessage == "")
                {
                    MsgSend.Error.sErrorMessage = "";
                    MsgSend.Error.iErrorNumber = enError.en_NoError;
                }
                else
                {
                    MsgSend.Error.sErrorMessage = AdcStatus.LastErrorMessage;
                    MsgSend.Error.iErrorNumber = enError.en_SpecificError;
                }

                switch (MsgReceived.Command)
                {
                    case enumCommandExchangeADC.caUpdateFDCInfo:
                        PCRessourcesMonitor pCRessourcesMonitor = new PCRessourcesMonitor();
                        MsgSend.dataFDC = pCRessourcesMonitor.GetInfo_Snapshot();
                        break;
                    case enumCommandExchangeADC.caGetVersion:
                        break;
                    //---------------------------
                    case enumCommandExchangeADC.caGetStatus:
                        {
                            MsgSend.lsCompletedUniqueId = AdcStatus.WaferReportList.Keys.ToList();
                        }
                        break;
                    //------------------------------
                    case enumCommandExchangeADC.caGetResult:
                        {
                            string uniqueId = MsgReceived.aUniqueIDSearch;
                            bool ok = AdcStatus.WaferReportList.TryGetValue(uniqueId, out MsgSend.pCWaferReport);
                            if (!ok)
                                logWarning("Can't find wafer, UniqueId: " + uniqueId);
                        }
                        break;
                    //------------------------------
                    case enumCommandExchangeADC.caClear:
                        {
                            string uniqueId = MsgReceived.aUniqueIDSearch;
                            bool ok = AdcStatus.WaferReportList.Remove(uniqueId);
                            if (!ok)
                                logWarning("Can't find wafer, UniqueId: " + uniqueId);

                            MsgSend.Error.DataBaseIsConnected = AdcStatus.IsConfigurationDatabaseConnected = AdcStatus.IsResultDatabaseConnected = true;
                            MsgSend.Error.sErrorMessage = AdcStatus.LastErrorMessage = "";
                            MsgSend.Error.iErrorNumber = enError.en_NoError;
                        }
                        break;
                    //------------------------------
                    default:
                        logWarning("Invalid message received from Robot: unknown MsgReceived.Command = \n" + MsgReceived.Command);
                        MsgSend.Status = enumStatusExchangeADC.saError;
                        break;
                }

                return MsgSend;
            }
        }

        //=================================================================
        // Log
        //=================================================================
        private void log(string msg)
        {
            if (msg != null && msg != "")
                Log.Information(SocketServer.ServerName + ": " + msg);
        }

        private void logWarning(string msg)
        {
            if (msg != null && msg != "")
                Log.Warning(SocketServer.ServerName + ": " + msg);
        }

        private void logError(string msg)
        {
            if (msg != null && msg != "")
                Log.Error(SocketServer.ServerName + ": " + msg);
        }

    }
}
