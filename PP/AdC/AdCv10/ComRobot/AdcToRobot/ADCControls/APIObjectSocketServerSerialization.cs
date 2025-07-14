using System;
using System.Threading;
using System.Windows.Forms;

namespace ADCControls
{
    public delegate void OnClientDisconnected();
    public enum enumStatus
    {
        stUnknown = 0,
        stNotInitialized,
        stReadyToStart,
        stProcessing,
        stProcessComplete,
        stError
    };

    public delegate void OnSocketLog(String Msg, String MsgError);

    public class CSocketServer<T> where T : class, new()
    {
        private const int TIMEOUT_SENDMSG = 45; // 45 Secondes
        public const String ALTA_MSG_BEGIN = "ALTAMSGBEGIN";
        public const String ALTA_MSG_END = "ALTAMSGEND";
        private CSocketClientServer m_SocketServer;
        private OnSocketLog m_SocketLog;
        private OnDataExchange m_OnDataRead;
        private OnDataExchange m_OnDataSent;
        private OnDataExchange m_OnMessageReceived;
        private OnServerConnectedDisconnected m_pOnServerDisconnected;
        private OnClientConnectedDisconnected m_pOnClientDisconnected;
        private OnClientConnectedDisconnected m_pOnClientConnected;
        private String m_LastMsg = "";
        private String m_ServerName;
        private short m_PortNum;
        private AutoResetEvent m_evtDataReceivedSending;
        private bool m_bSendEventAck = false;
        private String m_Response = "";
        private Object m_SynchroMsgReceived = new Object();
        private T m_MsgReceived;

        public CSocketServer(String ServerName, short PortNum, OnSocketLog pEventLog, OnClientConnectedDisconnected pOnClientConnected, OnClientConnectedDisconnected pOnClientDisconnected, OnServerConnectedDisconnected pOnServerDisconnected, OnDataExchange pOnMessageReceived)
        {
            m_SocketLog = pEventLog;
            m_ServerName = ServerName;
            m_PortNum = PortNum;
            m_pOnServerDisconnected = pOnServerDisconnected;
            m_pOnClientDisconnected = pOnClientDisconnected;
            m_pOnClientConnected = pOnClientConnected;
            m_OnDataRead = new OnDataExchange(OnDataRead);
            m_OnDataSent = new OnDataExchange(OnDataSent);
            m_OnMessageReceived = pOnMessageReceived;
            m_evtDataReceivedSending = new AutoResetEvent(false);

            // Define client name according to server type
            m_SocketServer = new CSocketClientServer(true, m_ServerName, m_PortNum, m_ServerName, m_SocketLog, m_OnDataRead, m_OnDataSent, m_pOnClientConnected, m_pOnClientDisconnected, /*m_pOnServerDisconnected, /**/false);
        }

        public Boolean IsConnected
        {
            get
            {
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
            get { return m_SocketServer.IsListening; }
        }
        public String ServerName { get { return m_ServerName; } }

        public String LastMsg
        {
            get { return m_LastMsg; }
            set { m_LastMsg = value; }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Echange Question/Reponse
        public int SendMsg(T MsgObj)
        {
            String data = ToolsClass_Utilities.Serialize(MsgObj);
            data = ALTA_MSG_BEGIN + data + ALTA_MSG_END;
            //m_SocketLog.Invoke("[SEND] " + MsgObj.MessageLog, "");
            m_SocketServer.SendData(data);
            return ETCPException.NO_ERROR;
        }

        public int SendMsg(T MsgSent, out T MsgReceived)
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
                    if (m_Response.Contains(ALTA_MSG_END))
                    {
                        pos = m_Response.IndexOf(ALTA_MSG_END);
                        m_Response = m_Response.Substring(0, pos);
                        OnMessageReceived(m_Response);
                        m_Response = "";
                    }
                }
            }
        }

        public void OnMessageReceived(String pRequest)
        {
            if (m_bSendEventAck)
            {
                lock (m_SynchroMsgReceived)
                {
                    m_MsgReceived = ToolsClass_Utilities.DeSerialize<T>(pRequest);
                    //m_SocketLog.Invoke(m_MsgReceived.MessageLog, "");
                    m_evtDataReceivedSending.Set();
                }
            }
            else
            {
                m_OnMessageReceived.Invoke(pRequest);
            }
        }

        public void OnDataSent(String Data)
        {

        }

        public void Listen()
        {
            m_SocketServer.ListenNetwork(m_PortNum);
        }

        public void Disconnect()
        {
            try
            {
                m_SocketServer.DisconnectClient();
                m_pOnServerDisconnected.Invoke();
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


    }
}

