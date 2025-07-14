using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public class CSocketClientSerialization
    {
        const int TIMEOUT_SENDMSG = 45; // 45 Secondes
        public const String ALTA_MSG_BEGIN = "ALTAMSGBEGIN";
        public const String ALTA_MSG_END = "ALTAMSGEND";
  
        CSocketClientServer m_SocketClient;
        OnSocketLog m_SocketLog;
        OnSocketLog m_SocketLogDisplay;
        OnDataExchange m_OnDataRead;
        OnDataExchange m_OnDataSent;
        OnDataExchange m_OnMessageReceived;
        OnServerConnectedDisconnected m_OnServerDisconnected;
        bool m_bExitWaitingResponse = false;
        
        String m_LastMsg="";
        String m_ServerName;
        short m_PortNum;
        String m_ClientName;
        bool m_bConnected;
        Connection m_ServerType;
        AutoResetEvent m_evtDataReceivedSending;
        bool m_bSendEventAck=false;
        String m_Response="";
        Object m_SynchroMsgReceived = new Object();
        CADCMessage m_MsgReceived;
        Object m_SynchroSendMsg = new object();

        public CSocketClientSerialization(Connection pServerType, String ServerName, short PortNum, OnSocketLog pEventLog, OnServerConnectedDisconnected pOnServerDisconnected)
        {
            m_SocketLog = pEventLog;
            m_ServerName = ServerName;
            m_ServerType = pServerType;
            m_PortNum = PortNum;
            m_OnServerDisconnected = pOnServerDisconnected;

            m_evtDataReceivedSending = new AutoResetEvent(false);

            // Define client name according to server type
            switch (m_ServerType)
            {
                case Connection.CONNECT_ACQUISITION_FRONTSIDE: m_ClientName = "[AcqFrontside client]";
                    break;
                case Connection.CONNECT_ACQUISITION_BACKSIDE: m_ClientName = "[AcqBackside client]";
                    break;
                default: m_ClientName = "[Unknown client]"; break;
            }           
        }

        public Boolean IsConnected 
        { 
            get {
                    try
                    {
                        return m_SocketClient.IsConnected;
                    }
                    catch
                    {
                        return false;
                    }
                } 
        }
        public String ClientName { get { return m_ClientName; } }
        
        public Connection ServerType
        {
            get { return m_ServerType; }
        }

        public String LastMsg
        {
            get { return m_LastMsg; }
            set { m_LastMsg = value; }
        }
               

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Echange Question/Reponse
        public int SendMsg(CADCMessage MsgObj)
        {
            String data = Win32Tools.Serialize(MsgObj);
            data = ALTA_MSG_BEGIN + data + ALTA_MSG_END;
            m_SocketClient.SendData(data);            
            return ETCPException.NO_ERROR;
        }

        public int SendMsg(CADCMessage MsgSent, out CADCMessage MsgReceived)
        {
            lock (m_SynchroSendMsg)
            {
                m_bSendEventAck = true;
                SendMsg(MsgSent);
                DateTime StartSend = DateTime.Now;
                bool bStopTimeOut = false;
                while (!m_evtDataReceivedSending.WaitOne(100, false))
                {
                    Application.DoEvents();
                    bStopTimeOut = (((TimeSpan)DateTime.Now.Subtract(StartSend)).TotalSeconds > TIMEOUT_SENDMSG);
                    if (!IsConnected || bStopTimeOut || m_bExitWaitingResponse)
                    {
                        lock (m_SynchroMsgReceived)
                        { MsgReceived = null; }
                        if (IsConnected) Disconnect();
                        return ETCPException.NO_TIMEOUT_SOCKET;
                    }
                }
                m_bSendEventAck = false;
                lock (m_SynchroMsgReceived)
                {
                    MsgReceived = m_MsgReceived;
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
                m_MsgReceived = Win32Tools.DeSerialize<CADCMessage>(pResponse);
                
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
            CADCMessage MsgSent = new CADCMessage();
            CADCMessage MsgRec;
            MsgSent.acCommand = enumCommand.acGetVersion;
            //MsgSent.Status = enumStatusExchange.saOk;
            int iError = SendMsg(MsgSent, out MsgRec);
            if ((MsgRec!=null ) && (MsgRec.Description.Length > 0))
                sVersion = MsgRec.Description;
            else
                sVersion = "Version is not available";
            return iError;        
        }

        
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int GetAcquisitionStatus(out CADCMessage Status)
        {
            Status = null;
            CADCMessage MsgSent = new CADCMessage();
            CADCMessage MsgRec;
            MsgSent.acCommand = enumCommand.acGetStatus;
            //MsgSent.Status = enumStatusExchange.saOk;
            int ErrorCode = SendMsg(MsgSent, out MsgRec);
            if ((ErrorCode == ENanotopographyException.NOERROR) || (MsgRec != null)) 
            {
                // Take Status from String                
                Status = MsgRec; 
                // Check status conformity
                return ENanotopographyException.NOERROR;                
            }
            else
                return ENanotopographyException.NO_GET_STATUS_MODULE_FAILED;
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public void Connect()
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
                Ex.DisplayError(Ex.ErrorCode) ;
            }
        }

        public void Disconnect()
        {
            try
            {
                m_SocketClient.DisconnectClient();
                m_OnServerDisconnected.Invoke(m_ServerType);  
            }
            catch (ETCPException Ex)
            {
                Ex.DisplayError(Ex.ErrorCode);
            }
        }

        public void SocketShutdown()
        {
            if (m_SocketClient != null)
            {
                m_SocketClient.ExitReceiveThread();
                m_SocketClient.ExitSendThread();
            }
            m_bExitWaitingResponse = true;
        }

    }
}

