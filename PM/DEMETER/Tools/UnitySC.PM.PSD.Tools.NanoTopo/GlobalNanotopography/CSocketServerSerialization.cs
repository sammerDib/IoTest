using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public delegate void OnClientDisconnected(Connection ServerType);
    public enum enumStatus
    {
         stUnknown=0,
         stNotInitialized,
         stReadyToStart,
         stProcessing,
         stProcessComplete,
         stError
    };    

    public delegate void OnSocketLog(Connection ServerType, String Msg, String MsgError);
    public class CSocketServer
    {
        const int TIMEOUT_SENDMSG = 45; // 45 Secondes
        public const String ALTA_MSG_BEGIN = "ALTAMSGBEGIN";
        public const String ALTA_MSG_END = "ALTAMSGEND";
  
        CSocketClientServer m_SocketServer;
        OnSocketLog m_SocketLog;
        OnSocketLog m_SocketLogDisplay;
        OnDataExchange m_OnDataRead;
        OnDataExchange m_OnDataSent;
        OnDataExchange m_OnMessageReceived;
        OnServerConnectedDisconnected m_pOnServerDisconnected;
        OnClientConnectedDisconnected m_pOnClientDisconnected;
        OnClientConnectedDisconnected m_pOnClientConnected;
        
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

        public CSocketServer(Connection pServerType, String ServerName, short PortNum, OnSocketLog pEventLog, OnClientConnectedDisconnected pOnClientConnected, OnClientConnectedDisconnected pOnClientDisconnected, OnServerConnectedDisconnected pOnServerDisconnected, OnDataExchange pOnMessageReceived)
        {
            m_SocketLog = pEventLog;
            m_ServerName = ServerName;
            m_ServerType = pServerType;
            m_PortNum = PortNum;
            m_pOnServerDisconnected = pOnServerDisconnected;
            m_pOnClientDisconnected = pOnClientDisconnected;
            m_pOnClientConnected = pOnClientConnected;
            m_OnDataRead = new OnDataExchange(OnDataRead);
            m_OnDataSent = new OnDataExchange(OnDataSent);
            m_OnMessageReceived = pOnMessageReceived;
            m_evtDataReceivedSending = new AutoResetEvent(false);

            // Define client name according to server type
            switch (m_ServerType)
            {
                case Connection.CONNECT_SL300: m_ClientName = "[AS300 client]";
                    break;
                default: m_ClientName = "[Unknown client]"; break;
            }
            m_SocketServer = new CSocketClientServer(m_ServerType, true, m_ServerName, m_PortNum, m_ServerName, m_SocketLog, m_OnDataRead, m_OnDataSent, m_pOnClientConnected, m_pOnClientDisconnected, m_pOnServerDisconnected, false);
            m_SocketServer.ListenNetwork(m_PortNum);

        }

        public Boolean IsConnected 
        { 
            get {
                    try
                    {
                        return m_SocketServer.IsConnected;
                    }
                    catch
                    {
                        return false;
                    }
                } 
        }

        public bool IsListening
        {
            get { return m_SocketServer.IsListening ; }
        }
        public String ServerName { get { return m_ServerName; } }
        
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
            m_SocketServer.SendData(data);            
            return ETCPException.NO_ERROR;
        }

        public int SendMsg(CADCMessage MsgSent, out CADCMessage MsgReceived)
        {
            m_bSendEventAck = true;
            SendMsg(MsgSent);
            DateTime StartSend = DateTime.Now;
            bool bStopTimeOut = false;
            while (!m_evtDataReceivedSending.WaitOne(100, false))
            {
                Application.DoEvents();
                bStopTimeOut = (((TimeSpan)DateTime.Now.Subtract(StartSend)).TotalSeconds > TIMEOUT_SENDMSG);
                if (!IsConnected || bStopTimeOut)
                {
                    MsgReceived = null;
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
            if (m_bSendEventAck)
            {
                lock (m_SynchroMsgReceived)
                {
                    m_MsgReceived = Win32Tools.DeSerialize<CADCMessage>(pResponse);
                    m_evtDataReceivedSending.Set();
                }
            }
            else
            {                
                m_OnMessageReceived.Invoke(pResponse);
            }
        }
        public void OnDataSent(String Data)
        {

        }        

        public void OnClientDisconnect()
        {
            m_SocketServer.OnClientDisconnect();
        }                                                                                      

        public void Disconnect()
        {
            try
            {
                m_SocketServer.DisconnectClient();
                m_pOnServerDisconnected.Invoke(m_ServerType);  
            }
            catch (ETCPException Ex)
            {
                Ex.DisplayError(Ex.ErrorCode);
            }
        }

        public void SocketShutdown()
        {
            if (m_SocketServer != null)
            {                
                m_SocketServer.DisconnectServeur();
                m_SocketServer.ExitReceiveThread();
                m_SocketServer.ExitSendThread();
            }
        }


        public void OnClientConnect(IAsyncResult async)
        {

            m_SocketServer.OnClientConnect(async);
        }
    }
}

