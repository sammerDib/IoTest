using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Common.EException;
using Common.SocketMessage;

namespace Common.SocketObject
{
    public class CServerMultiClientsSerialization<TMessage>
        where TMessage : CBaseMessage
    {
        const int TIMEOUT_SENDMSG = 45; // 45 Secondes
        public const String ALTA_MSG_BEGIN = "ALTAMSGBEGIN";
        public const String ALTA_MSG_END = "ALTAMSGEND";

        CServerMultiClients m_ServerMultiClients;
        OnSocketLog m_SocketLog;
        OnDataExchange m_OnDataRead;
        OnDataExchange m_OnDataSent;
        OnDataExchange m_OnMessageReceived;
        OnServerConnectedDisconnected m_pEvtExt_OnServerDisconnected;
        OnClientConnectedDisconnected m_pEvtExt_OnClientDisconnected;
        OnClientConnectedDisconnected m_pEvtExt_OnClientConnected;
        
        String m_LastMsg="";
        String m_ServerName;
        short m_PortNum;
        enumConnection m_ServerType;
        AutoResetEvent m_evtDataReceivedSending;
        bool m_bSendEventAck=false;
        String m_Response="";
        Object m_SynchroMsgReceived = new Object();
        TMessage m_MsgReceived;
        enumSerializationType m_SerializationType;

        public CServerMultiClientsSerialization(enumConnection pServerType, String ServerName, short PortNum, OnSocketLog pEventLog, OnClientConnectedDisconnected pEvtExt_OnClientConnected, OnClientConnectedDisconnected pEvtExt_OnClientDisconnected, OnServerConnectedDisconnected pEvtExt_OnServerDisconnected, OnDataExchange pEvtExt_OnMessageReceived, enumSerializationType pSerializationType)
        {
            m_SocketLog = pEventLog;
            m_ServerName = ServerName;
            m_ServerType = pServerType;
            m_PortNum = PortNum;
            m_pEvtExt_OnServerDisconnected = pEvtExt_OnServerDisconnected;
            m_pEvtExt_OnClientDisconnected = pEvtExt_OnClientDisconnected;
            m_pEvtExt_OnClientConnected = pEvtExt_OnClientConnected;
            m_OnDataRead = new OnDataExchange(OnDataRead);
            m_OnDataSent = new OnDataExchange(OnDataSent);
            m_OnMessageReceived = pEvtExt_OnMessageReceived;
            m_evtDataReceivedSending = new AutoResetEvent(false);
            m_SerializationType = pSerializationType;

            m_ServerMultiClients = new CServerMultiClients(m_ServerType, m_ServerName, m_PortNum, m_SocketLog, m_OnDataRead, m_OnDataSent, m_pEvtExt_OnClientConnected, m_pEvtExt_OnClientDisconnected, m_pEvtExt_OnServerDisconnected);            
        }

        public Boolean IsConnected 
        { 
            get {
                    try
                    {
                        return m_ServerMultiClients.IsConnected;
                    }
                    catch
                    {
                        return false;
                    }
                } 
        }

        public bool IsListening
        {
            get { return m_ServerMultiClients.IsListening ; }
        }
        public String ServerName { get { return m_ServerName; } }

        public int NumberOfConnectedClients
        {
            get
            {
                int i = 0;
                foreach (CClientConnection client in m_ServerMultiClients.ClientList)
                {
                    if (client.IsConnected)
                        i++;
                }
                return i;
            }
        }

        public enumConnection ServerType
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
        public int SendMsg(TMessage MsgObj)
        {
            String lData;
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
            //m_SocketLog.Invoke(m_ServerType, "[SEND] = " + lData, "");
            m_SocketLog.Invoke(m_ServerType, "[SEND] " + MsgObj.MessageLog, "");
            m_ServerMultiClients.SendDataAllClients(lData);            
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
            if (m_bSendEventAck) // Toujours false. A utiliser dans method SendMSG avec attente de reponse dans ce server. Mais actuellement pas implementer - A faire au besoin
            {
                lock (m_SynchroMsgReceived)
                {
                    m_MsgReceived = Win32Tools.DeSerialize<TMessage>(pResponse);
                    if (m_MsgReceived != null)
                    {
                        m_SocketLog.Invoke(m_ServerType, m_MsgReceived.MessageLog, "");
                        m_evtDataReceivedSending.Set();
                    }
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
            
        }                                                                                      

        public void Disconnect()
        {
            try
            {
                m_ServerMultiClients.DisconnectClients();
                m_pEvtExt_OnServerDisconnected.Invoke(m_ServerType);  
            }
            catch (ETCPException Ex)
            {
                Ex.DisplayError(Ex.ErrorCode);
            }
        }

        public void Shutdown()
        {
            if (m_ServerMultiClients != null)
            {
                m_ServerMultiClients.DisconnectClients();
                m_ServerMultiClients.StopServeur();
            }
        }


        public  void OnClientConnect(IAsyncResult async)
        {
            m_ServerMultiClients.OnClientConnect(async);
        }
    }
}

