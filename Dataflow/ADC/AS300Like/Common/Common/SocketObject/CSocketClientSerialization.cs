using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using Common;
using Common.SocketMessage;
using Common.EException;
using Common.SocketObject;
using Common.FDC;

namespace Common.SocketObject
{
    public class CSocketClientSerialization<T> where T:CBaseMessage, new()
    {
        const int TIMEOUT_SENDMSG = 45; // 45 Secondes
        public const String ALTA_MSG_BEGIN = "ALTAMSGBEGIN";
        public const String ALTA_MSG_END = "ALTAMSGEND";
  
        CSocketClientServer m_SocketClient;
        OnSocketLog m_SocketLog;
        OnDataExchange m_OnDataRead;
        OnDataExchange m_OnDataSent;
        OnDataExchange m_OnMessageReceived;
        OnServerConnectedDisconnected m_OnServerDisconnected;

        String m_ServerName;
        short m_PortNum;
        String m_ClientName;
        enumConnection m_ServerType;
        AutoResetEvent m_evtDataReceivedSending;
        bool m_bSendEventAck=false;
        String m_Response="";
        Object m_SynchroMsgReceived = new Object();
        T m_MsgReceived;
        Object m_SynchroSendMsg = new object();
        String m_ModuleName;

        bool m_Silence_NoMsg;
        enumSerializationType m_SerializationType;
        DateTime m_LastGetStatustime; 
        public Boolean Silence_NoMsg
        {
            set { m_Silence_NoMsg = value; }
        }

        public CSocketClientSerialization(enumConnection pServerType, String ServerName, short PortNum, OnSocketLog pEventLog, OnServerConnectedDisconnected pOnServerDisconnected, enumSerializationType pSerializationType)
        {
            m_SocketLog = pEventLog;
            m_ServerName = ServerName;
            m_ServerType = pServerType;
            m_PortNum = PortNum;
            m_OnServerDisconnected = pOnServerDisconnected;
            m_SerializationType = pSerializationType;
            m_evtDataReceivedSending = new AutoResetEvent(false);

            // Define client name according to server type
            switch (m_ServerType)
            {
                case enumConnection.CONNECT_GRAB_DARKFIELD: m_ModuleName = "Darkview"; break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD1: m_ModuleName = "BrightField1"; break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD2: m_ModuleName = "BrightField2"; break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD3: m_ModuleName = "BrightField3"; break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD4: m_ModuleName = "BrightField4"; break;
                case enumConnection.CONNECT_PSD: m_ModuleName = "PSD"; break;
				case enumConnection.CONNECT_GRAB_REVIEW: m_ModuleName = "Review"; break;
                case enumConnection.CONNECT_PMLS: m_ModuleName = "LightSpeed"; break;
                default: m_ModuleName = "Unknown"; break;
            }           
            m_ClientName = "[" + m_ModuleName + " client]";
            m_LastGetStatustime = DateTime.Now;
        }

        public Boolean IsConnected 
        { 
            get {
                    try
                    {
                        if (m_SocketClient != null)
                            return m_SocketClient.IsConnected;
                        else
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                } 
        }
        public String ClientName { get { return m_ClientName; } }
        
        public enumConnection ServerType
        {
            get { return m_ServerType; }
        }               

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Echange Question/Reponse
        public int SendMsg(T MsgObj)
        {
            try
            {
                String lData = "";
                switch (m_SerializationType)
	            {
                    case enumSerializationType.stCBaseMessage:
                        lData = Win32Tools.SerializeCBaseMessage((CBaseMessage)MsgObj);
                        break;
                    case enumSerializationType.stCBaseMessageBrightfield:
                        CBaseMessageBrightField lMsgBF = MsgObj as CBaseMessageBrightField;
                        lData = Win32Tools.SerializeCBaseMessageBrightField(lMsgBF);                        
                        break;
                    default:
                    case enumSerializationType.stNature: 
                        lData = Win32Tools.Serialize(MsgObj);
                     break;
	            }
                lData = ALTA_MSG_BEGIN + lData + ALTA_MSG_END;
                if (m_SocketClient != null)
                {
                    m_SocketLog.Invoke(m_ServerType, " [SEND] " + MsgObj.MessageLog, "");
                    m_SocketClient.SendData(lData);
                }
            }
            catch 
            {
            }
            return ETCPException.NO_ERROR;
        }

        public int SendMsg(T MsgSent, out T MsgResponse)
        {
            MsgResponse = null;
            lock (m_SynchroSendMsg)
            {
                if (m_SocketClient == null) 
                    return ETCPException.NO_TCP_SERVER_NOT_EXIST;
                m_bSendEventAck = true;
                int Err = SendMsg(MsgSent);
                if (Err != ETCPException.NOERROR)
                    return Err;
                DateTime StartSend = DateTime.Now;
                bool bStopTimeOut = false;
                while (!m_evtDataReceivedSending.WaitOne(100, false))
                {
                    Application.DoEvents();
                    bStopTimeOut = (((TimeSpan)DateTime.Now.Subtract(StartSend)).TotalSeconds > TIMEOUT_SENDMSG);
                    if (!IsConnected || bStopTimeOut)
                    {
                        MsgResponse = null;
                        if (IsConnected) Disconnect_Threaded();
                        return ETCPException.NO_TIMEOUT_SOCKET;
                    }
                }
                m_bSendEventAck = false;
                lock (m_SynchroMsgReceived)
                {
                    MsgResponse = m_MsgReceived;
                    m_MsgReceived = null;
                }
            }
            return ETCPException.NO_ERROR;
        }

        public void OnDataRead(String pData)
        {                            
            lock (m_Response)
            {
                if (m_Response != "")
                    Thread.Sleep(2000);

                m_Response += pData;
                if (m_Response.Contains(ALTA_MSG_BEGIN))
                {
                    int pos = m_Response.IndexOf(ALTA_MSG_BEGIN);
                    m_Response = m_Response.Substring(pos + ALTA_MSG_BEGIN.Length);
                    if(m_Response.Contains(ALTA_MSG_END))
                    {
                        pos = m_Response.IndexOf(ALTA_MSG_END);
                        m_Response = m_Response.Substring(0, pos);
                        OnMessageReceived(m_Response);
                        m_Response = "";
                    }                    
                }
            }
        }

        public void OnMessageReceived(String pResponse)
        {
            lock (m_SynchroMsgReceived)
            {
                try
                {
                    switch (m_SerializationType)
                    {
                        case enumSerializationType.stCBaseMessage:
                            m_MsgReceived = Win32Tools.DeSerializeCBaseMessage<T>(pResponse);
                            break;
                        case enumSerializationType.stCBaseMessageBrightfield:
                            m_MsgReceived = Win32Tools.DeSerializeCBaseMessageBrightField<T>(pResponse);
                            break;
                        case enumSerializationType.stNature:
                            m_MsgReceived = Win32Tools.DeSerialize<T>(pResponse);
                            break;
                        default:
                            break;
                    }                    
                
                    m_SocketLog.Invoke(m_ServerType, " [RECV] " + m_MsgReceived.MessageLog, "");
                }
                catch(Exception Ex)
                {
                    MessageBox.Show(Ex.Message + " - " + Ex.StackTrace + " - " + Ex.InnerException.Message); 
                }
            }
            if (m_bSendEventAck)
            {
                m_evtDataReceivedSending.Set();
            }
        }
        public void OnDataSent(String Data)
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
            MsgSent.Command = enumCommandExchangeAS300.caGetVersion;
            MsgSent.Status = enumStatusExchangeAS300.saOk;
            int iError = SendMsg(MsgSent, out MsgRec);
            if (MsgRec.Status == enumStatusExchangeAS300.saOk)
                sVersion = MsgRec.Description;
            else
                sVersion = "Version is not available";
            return iError;        
        }

        
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int GetModuleStatus(out T Status)
        {
            Status = null;
            T MsgSent = new T();
            T MsgRec;
            MsgSent.Command = enumCommandExchangeAS300.caGetStatus;
            MsgSent.Status = enumStatusExchangeAS300.saOk;

            if (DateTime.Now.Subtract(m_LastGetStatustime).TotalMilliseconds <= 400)
                Thread.Sleep(1000);
            m_LastGetStatustime = DateTime.Now;
            int ErrorCode = SendMsg(MsgSent, out MsgRec);
            if ((ErrorCode == 0) || (MsgRec != null))
            {
                // Take Status from String                
                Status = MsgRec;
                // Check status conformity
                return ErrorCode;
            }
            else
                return ErrorCode;
           
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public FDCInfo RequestToUpdateFDCData()
        {
#if !WITHOUT_FDC
                T MsgSent = new T();
                T MsgRec;
                MsgSent.Command = enumCommandExchangeAS300.caUpdateFDCInfo;
                MsgSent.Status = enumStatusExchangeAS300.saOk;
            
                int ErrorCode = SendMsg(MsgSent, out MsgRec);
                if ((ErrorCode == 0) || (MsgRec != null))
                    return  (FDCInfo)MsgRec.FdcInfoData.Clone();
                else
                    return null;
#else
            return null;
#endif
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int SendStartProcess(out T MsgResponse)
        {
            T MsgCommand = new T();
            MsgCommand.Command = enumCommandExchangeAS300.caStart;
            MsgCommand.Description = "Start " +  m_ModuleName + " process";
            MsgCommand.Status = enumStatusExchangeAS300.saOk;
            return SendMsg(MsgCommand, out MsgResponse);
        }

        public int SendAbortProcess(out T MsgResponse)
        {
            T MsgCommand = new T();
            MsgCommand.Command = enumCommandExchangeAS300.caAbort;
            MsgCommand.Description = "Abort " + m_ModuleName + " process";
            MsgCommand.Status = enumStatusExchangeAS300.saOk;
            return SendMsg(MsgCommand, out MsgResponse);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int SendKillGrabber()
        {
            return 0;
        }        

        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public void Connect()
        {
            Connect(false);
        }
        public void Connect(bool bSilence)
        {
            try
            {
                if (m_SocketClient == null)
                {
                    m_OnDataRead += new OnDataExchange(OnDataRead);
                    m_OnDataSent += new OnDataExchange(OnDataSent);
                    m_OnMessageReceived = new OnDataExchange(OnMessageReceived);
                    m_SocketClient = new CSocketClientServer(m_ServerType, false, m_ServerName, m_PortNum, m_ClientName, m_SocketLog, m_OnDataRead, m_OnDataSent, null, null ,m_OnServerDisconnected, false);
                }
                else
                    m_SocketClient.ChangeServer(m_ServerName, m_PortNum);
                m_SocketClient.Connect();
            }
            catch (ETCPException Ex)
            {    
                if(!m_Silence_NoMsg)
                    Ex.DisplayError(Ex.ErrorCode) ;
                m_SocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
            }
        }
        public void Disconnect_Threaded()
        {
            Thread lThread = new Thread(new ParameterizedThreadStart(Disconnect_Execute));
            lThread.Name = "Disconnect_Execute";
            lThread.Start(null);

        }
        private void Disconnect_Execute(Object pData)
        {
            Disconnect();
        }
        public void Disconnect()
        {
            try
            {
                if(m_SocketClient != null)
                    m_SocketClient.DisconnectClient();
                m_OnServerDisconnected.Invoke(m_ServerType);  
            }
            catch (ETCPException Ex)
            {
                if (!m_Silence_NoMsg)
                    Ex.DisplayError(Ex.ErrorCode);
                m_SocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
            }
        }

        public void SocketShutdown()
        {
            if (m_SocketClient != null)
            {
                m_SocketClient.ExitReceiveThread();
                m_SocketClient.ExitSendThread();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int ClampWafer(bool bClamp)
        {
            T MsgSent = new T();
            T MsgRec;
            if(bClamp)
                MsgSent.Command = enumCommandExchangeAS300.caClampWafer;
            else
                MsgSent.Command = enumCommandExchangeAS300.caUnclampWafer;
            MsgSent.Status = enumStatusExchangeAS300.saOk;
            int ErrorCode = SendMsg(MsgSent, out MsgRec);
            if ((ErrorCode == 0) && (MsgRec != null) && (MsgRec.Status == enumStatusExchangeAS300.saOk))            
                return 0;            
            else
                return ErrorCode;
        }
    }
}

