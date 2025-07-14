using System;
using System.Threading;

using ADCCommon;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.EException;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;

namespace UnitySC.ADCAS300Like.Service.SocketClient
{
    public class CSocketClient_Serialization<T> where T : CADCBaseMessage, new()
    {
        public const String ALTA_MSG_BEGIN = "ALTAMSGBEGIN";
        public const String ALTA_MSG_END = "ALTAMSGEND";

        private const int TIMEOUT_SENDMSG = 45; // 45 Secondes
        private CSocketClientServer _socketClient;
        private OnSocketLog _socketLog;
        private OnDataExchange _onDataRead;
        private OnDataExchange _onDataSent;
        private OnDataExchange _onMessageReceived;
        private OnServerConnectedDisconnected _onServerDisconnected;

        private String _serverName;
        private short _portNum;
        private String _clientName;
        private ADCType _serverType;
        private AutoResetEvent _evtDataReceivedSending;
        private bool _bSendEventAck=false;
        private String _response ="";
        private Object _synchroMsgReceived = new Object();
        private T _msgReceived;
        private Object _synchroSendMsg = new object();
        private bool _diconnectionAlreadyDetected = false;

        public CSocketClient_Serialization(ADCType pServerType, String serverName, short portNum, OnSocketLog pEventLog, OnServerConnectedDisconnected pOnServerDisconnected)
        {
            _socketLog = pEventLog;
            _serverName = serverName;
            _serverType = pServerType;
            _portNum = portNum;
            _onServerDisconnected = pOnServerDisconnected;

            _evtDataReceivedSending = new AutoResetEvent(false);

            // Define client name according to server type
            switch (_serverType)
            {

                case ADCType.atEDGE:       _clientName = "[ADC Edge client]";             break;
                case ADCType.atPSD_FRONTSIDE:      _clientName = "[ADC Frontside client]";        break;
                case ADCType.atPSD_BACKSIDE:       _clientName = "[ADC Backside client]";         break;
                case ADCType.atLIGHTSPEED:         _clientName = "[ADC Light Speed client]";      break;
                default: _clientName = "[Unknown client]"; break;
            }           
        }

        public Boolean IsConnected 
        { 
            get {
                    try
                    {
                        if (_socketClient != null)
                            return _socketClient.IsConnected;
                        else
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                } 
        }
        public String ClientName { get { return _clientName; } }
        
        public ADCType ServerType
        {
            get { return _serverType; }
        }

        public Boolean Silence_NoMsg
        {
            set { 
                    if(_socketClient != null) 
                        _socketClient.Silence_NoMsg = value; 
            }
        }    
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Echange Question/Reponse
        public int SendMsg(T msgObj)
        {
            String data = Win32Tools.Serialize(msgObj);
            data = ALTA_MSG_BEGIN + data + ALTA_MSG_END;
            if (_socketClient != null)
            {
                try
                {
                    //m_SocketLog.Invoke(m_ServerType, " [SEND] = " + data, "");
                    _socketLog.Invoke(_serverType, "[SEND] " + msgObj.MessageLog, "");
                }
                catch
                {
                } 
                _socketClient.SendData(data);
            }
            else
                return ETCPException.NO_SOCKET_ERROR;
            return ETCPException.NO_ERROR;
        }


        public int SendMsg(T msgSent, out T msgReceived)
        {
            msgReceived = default(T);
            lock (_synchroSendMsg)
            {
                _bSendEventAck = true;
                int Err = SendMsg(msgSent);
                if (Err != ETCPException.NOERROR)
                    return Err;
                DateTime StartSend = DateTime.Now;
                bool bStopTimeOut = false;
                while (!_evtDataReceivedSending.WaitOne(100, false))
                {
                    bStopTimeOut = (((TimeSpan)DateTime.Now.Subtract(StartSend)).TotalSeconds > TIMEOUT_SENDMSG);
                    if (!IsConnected || bStopTimeOut)
                    {
                        lock (_synchroMsgReceived)
                        { msgReceived = default(T); }
                        if (IsConnected) Disconnect();
                        return ETCPException.NO_TIMEOUT_SOCKET;
                    }
                }
                _bSendEventAck = false;
                lock (_synchroMsgReceived)
                {
                    msgReceived = _msgReceived;
                    _msgReceived = default(T);
                }
            }
            return ETCPException.NO_ERROR;
        }

        public void OnDataRead(String pData)
        {
            lock (_response)
            {
                
                if (_response != "")
                    Thread.Sleep(2000);

                _response += pData;
                //m_SocketLog.Invoke(m_ServerType, " [RECV] = " + m_Response, "");
                if (_response.Contains(ALTA_MSG_BEGIN))
                {
                    int pos = _response.IndexOf(ALTA_MSG_BEGIN);
                    _response = _response.Substring(pos + ALTA_MSG_BEGIN.Length);
                    if(_response.Contains(ALTA_MSG_END))
                    {
                        pos = _response.IndexOf(ALTA_MSG_END);
                        _response = _response.Substring(0, pos);
                        OnMessageReceived(_response);
                        _response = "";
                    }                    
                }
            }
        }


        public void OnMessageReceived(String pResponse)
        {
            try
            {
                lock (_synchroMsgReceived)
                {
                    _msgReceived = Win32Tools.DeSerialize<T>(pResponse);
                    _socketLog.Invoke(_serverType, "[RECV] " + _msgReceived.MessageLog, "");
                }
                if (_bSendEventAck)
                {
                    _evtDataReceivedSending.Set();
                }
            }
            catch (Exception ex)
            {
                _socketLog.Invoke(_serverType, "Message received. DeSerialization failed.", "Exception: " + ex.Message + " - " + ex.StackTrace);                
            }

        }
        public void OnDataSent(String data)
        {

        }        

        public void OnClientDisconnect()
        {
            
        }                              

        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int GetCaptureVersion( out String sVersion)
        {
            T MsgSent = new T();
            T MsgRec;
            MsgSent.Command = enumCommandExchangeADC.caGetVersion;            
            int iError = SendMsg(MsgSent, out MsgRec);
            if (MsgRec != null)
                sVersion = MsgRec.Description;
            else
                sVersion = "Version is not available";
            return iError;        
        }

        internal FDCInfo RequestToUpdateFDCData()
        {
#if !WITHOUT_FDC
            T MsgSent = new T();
            T MsgRec;
            MsgSent.Command = enumCommandExchangeADC.caUpdateFDCInfo;
            int ErrorCode = SendMsg(MsgSent, out MsgRec);

            if ((ErrorCode == 0) && (MsgRec != null) && (MsgRec.FdcInfoData!=null))
                return (FDCInfo)MsgRec.FdcInfoData.Clone();
            else
                return null;
#else
            return null;
#endif
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public void Connect()
        {
            try
            {
                if (_socketClient == null)
                {
                    if(!_diconnectionAlreadyDetected) _socketLog.Invoke(_serverType, $"Try connection", "");
                    _onDataRead += new OnDataExchange(OnDataRead);
                    _onDataSent += new OnDataExchange(OnDataSent);
                    _onMessageReceived = new OnDataExchange(OnMessageReceived);
                    _socketClient = new CSocketClientServer(_serverType, false, _serverName, _portNum, _clientName, _socketLog, _onDataRead, _onDataSent, null, null , _onServerDisconnected, false);
                }
                else
                    _socketClient.ChangeServer(_serverName, _portNum);
                _socketClient.Connect();
                _diconnectionAlreadyDetected = false;
            }
            catch (ETCPException Ex)
            {
                if (!_diconnectionAlreadyDetected)
                {
                    _socketLog.Invoke(ServerType, "Communication error", ETCPException.GetMessage(Ex.ErrorCode));
                    _diconnectionAlreadyDetected = true;
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                if(_socketClient != null)
                    _socketClient.DisconnectClient();
                _onServerDisconnected.Invoke(_serverType);
            }
            catch (ETCPException Ex)
            {
                if (!Ex.bSilence)
                    _socketLog.Invoke(ServerType, "Communication error", ETCPException.GetMessage(Ex.ErrorCode));
            }
        }

        public void SocketShutdown()
        {
            if (_socketClient != null)
            {
                _socketClient.ExitReceiveThread();
                _socketClient.ExitSendThread();
            }
        }        
    }
}

