using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets ;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Common.EException;

namespace Common.SocketObject
{    
    //  Socket Object Class - to exchange PC <-> PC  
    public class CServerMultiClients
    {
        // Private member                
        const int MAX_BUFFER = 65536; // 64ko
        const int TIMEOUT_SENDMSG = 45; // 45 Secondes
        public const String ALTA_MSG_BEGIN = "ALTAMSGBEGIN";
        public const String ALTA_MSG_END = "ALTAMSGEND";
        IAsyncResult m_AsyncResult;
        int m_iConnectionNbr = 0;
        bool m_bListening;

        TcpListener m_TcpListenerServer;
		bool m_TcpListnerStopRequested = false;
        List<CClientConnection> m_SocketExchangeList = new List<CClientConnection>();

        // Identification server
        String m_ServerName;
        enumConnection m_ServertType;
        short m_PortNum;
        
        // Event members        
        OnSocketLog m_EventLog;
        public OnClientConnectedDisconnected m_EvtExt_OnClientDisconnected;
        public OnClientConnectedDisconnected m_EvtExt_OnClientConnected;
        public OnServerConnectedDisconnected m_EvtExt_OnServerDisconnected;
        public OnDataExchange m_EVT_OnDataReceived;
        public OnDataExchange m_EVT_OnDataSend;

        OnClientConnectedDisconnected m_pEvtInt_OnClientConnected;
        OnClientConnectedDisconnected m_pEvtInt_OnClientDisconnected;
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Constructor
        public CServerMultiClients(enumConnection pServerType, String ServerName, short PortNum, OnSocketLog pEventLog, OnDataExchange pOnDataReceived, OnDataExchange pOnDataSend, OnClientConnectedDisconnected pEvtExt_OnClientConnected, OnClientConnectedDisconnected pEvtExt_OnClientDisconnected, OnServerConnectedDisconnected pEvtExt_OnServerDisconnected)
        {
            m_EventLog = pEventLog;
            m_ServerName = ServerName;
            m_PortNum = PortNum;
            m_ServertType = pServerType;
            m_EVT_OnDataReceived = pOnDataReceived;
            m_EVT_OnDataSend = pOnDataSend;
            m_EvtExt_OnClientDisconnected = pEvtExt_OnClientDisconnected;
            m_EvtExt_OnClientConnected = pEvtExt_OnClientConnected;
            m_EvtExt_OnServerDisconnected = pEvtExt_OnServerDisconnected;

            m_pEvtInt_OnClientConnected = new OnClientConnectedDisconnected(OnClientConnect);
            m_pEvtInt_OnClientDisconnected = new OnClientConnectedDisconnected(OnClientDisconnect);
            ListenNetwork();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //Properties
        public bool IsListening
        {
            get { return m_bListening; }
        }

        public List<CClientConnection> ClientList
        {
            get { return m_SocketExchangeList; }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------------------------------------------------------------------
        // Server Mode       
        public void ListenNetwork()
        {
            m_EventLog.Invoke(m_ServertType, "Server waiting connection...", "");
            // create the socket
            if (m_TcpListenerServer != null)
            {
                m_TcpListenerServer.Stop();
                m_TcpListenerServer = null;
            }
            try
            {
                m_TcpListenerServer = new TcpListener(IPAddress.Any, m_PortNum);
				m_TcpListnerStopRequested = false;
				// start listening
				m_iConnectionNbr = 0;
                m_TcpListenerServer.Start();

                // create the call back for any client connections...
                m_TcpListenerServer.BeginAcceptSocket(new AsyncCallback(m_pEvtInt_OnClientConnected), null);
                m_bListening = true;

            }
            catch (SocketException se)
            {
                String ErrMsg = se.Message;
                MessageBox.Show(ErrMsg);
            }
        
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
				if (m_TcpListnerStopRequested) return;
                String stClientName;
				m_bListening = false;
                m_AsyncResult = asyn;
                m_EventLog.Invoke(m_ServertType, "Incoming connection.", "");

                Socket lNewSocketConnected = m_TcpListenerServer.EndAcceptSocket(m_AsyncResult);
                switch (m_ServertType)
                {
                    case enumConnection.CONNECT_GRAB_DARKFIELD: stClientName = "[Darkfield client]"; break;
                    case enumConnection.CONNECT_GRAB_BRIGHTFIELD1: stClientName = "[BrightField1 client]"; break;
                    case enumConnection.CONNECT_GRAB_BRIGHTFIELD2: stClientName = "[BrightField2 client]"; break;
                    case enumConnection.CONNECT_GRAB_BRIGHTFIELD3: stClientName = "[BrightField3 client]"; break;
                    case enumConnection.CONNECT_GRAB_BRIGHTFIELD4: stClientName = "[BrightField4 client]"; break;
                    case enumConnection.CONNECT_PSD: stClientName = "[PSD client]"; break;
                    case enumConnection.CONNECT_GRAB_REVIEW: stClientName = "[Review client]"; break;
                    case enumConnection.CONNECT_PMEDGE: stClientName = "[PMEdge client]"; break;
                    case enumConnection.CONNECT_GRAB_EDGE: stClientName = "[PMEdge client]"; break;
                    case enumConnection.CONNECT_PMLS: stClientName = "[PMLS client]"; break;
                    default: stClientName = "[Unknown client]"; break;
                }
                stClientName = "Client#" + m_iConnectionNbr.ToString() +":"+ stClientName;
                CClientConnection lNewClient = new CClientConnection(m_ServertType, stClientName, lNewSocketConnected, m_EventLog, m_EVT_OnDataReceived, m_EVT_OnDataSend, m_pEvtInt_OnClientDisconnected);
                if (lNewClient.IsConnected)
                {
                    m_iConnectionNbr++;
                    m_SocketExchangeList.Add(lNewClient);
                    m_EventLog.Invoke(m_ServertType, m_iConnectionNbr.ToString() + " clients connected.", "");
                    if (m_iConnectionNbr >= 1)
                        if (m_EvtExt_OnClientConnected!=null) m_EvtExt_OnClientConnected.Invoke(asyn);
                    // Prepare for a new connection
                    m_TcpListenerServer.BeginAcceptSocket(new AsyncCallback(m_pEvtInt_OnClientConnected), null);
                    m_bListening = true;
                }
            }           
            catch
            {
                m_EventLog.Invoke(m_ServertType, "Incoming connection failed.", "");
                //throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", false);
            }
        }

        public void OnClientDisconnect(IAsyncResult asyn)
        {
            try
            {                
                m_EventLog.Invoke(m_ServertType, "Client connection closed.", "");

                if (m_iConnectionNbr>=1)                    
                    m_EvtExt_OnClientDisconnected.Invoke(asyn);

                m_iConnectionNbr--;
                int lIndexFound = -1;
                for (int i = 0; i < m_SocketExchangeList.Count; i++)
                {
                    if (m_SocketExchangeList[i].bShutdoneDone)
                    {
                        lIndexFound = i;
                        break;
                    }
                }
                m_SocketExchangeList.RemoveAt(lIndexFound);
                if (m_iConnectionNbr == 0)
                {
                    m_EventLog.Invoke(m_ServertType, " No client currently connected.", "");
                }
                else
                    m_EventLog.Invoke(m_ServertType, m_iConnectionNbr.ToString() + " clients still connected.", "");
            }
            catch
            {
                m_EventLog.Invoke(m_ServertType, "Incoming connection failed.", "");
                //throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", false);
            }
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Stop Serveur       
        public Int32 StopServeur()
        {            
            String Txt;

            // Affichage
            Txt = "Stop Server";
            m_EventLog.Invoke(m_ServertType, Txt, "");
            
            // Disconnect
            try
            {       
                // SI déjà stoppe ALORS
                if (!m_bListening)
                {
                    //Affichage               
                    Txt = "Server already stopped";
                    m_EventLog.Invoke(m_ServertType, Txt, "");

                    //Etat deconnecté
                    m_bListening = false;
                    m_iConnectionNbr = 0;
                    return 0;

                }
                //--- Déconneter                
                //Affichage
                Txt = "Stop server [ Computer Name: " + m_ServerName + " - " + m_PortNum + "]";
                m_EventLog.Invoke(m_ServertType, Txt, "");

				// Fermeture du tcpListener => du port  
				m_TcpListnerStopRequested = true;
				if (m_TcpListenerServer != null)
                    m_TcpListenerServer.Stop();

                //Affichage
                Txt = "Server stopped";
                m_EventLog.Invoke(m_ServertType, Txt, "");
                              
                //Etat deconnecté
                m_bListening = false;
                m_iConnectionNbr = 0;  
            }                     
            catch (ETCPException Ex)
            {
                String ErrMsg = Ex.Message;
                throw;
            }
            catch (Exception E)
            {
                throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, E.Message, E.Message, false);
            }   
            return 0;
        }


        public bool IsConnected 
        {
            get
            {
                for (int i = 0; i < m_SocketExchangeList.Count; i++)
                {
                    if (m_SocketExchangeList[i].IsConnected)
                        return true;
                }
                return false;
            }
        }

        public void SendDataAllClients(string strData)
        {
            try
            {
                for (int i = 0; i < m_SocketExchangeList.Count; i++)
                {
                    m_SocketExchangeList[i].SendData(strData);
                }
            }
            catch 
            {
                throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, "Send data with Socket failed", "Send data with Socket failed", false);
            }
        }

        internal void DisconnectClients()
        {
            try
            {
                for (int i = 0; i < m_SocketExchangeList.Count; i++)
                {
                    m_SocketExchangeList[i].DisconnectClient();
                }
            }
            catch
            {
                throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, "Disconnect clients failed", "Disconnect clients failed", false);
            }
        }

        internal void ExitReceiveThread()
        {

        }
    }
}
      
