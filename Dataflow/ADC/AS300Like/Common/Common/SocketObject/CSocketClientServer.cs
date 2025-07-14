using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using Common;
using Common.EException;


namespace Common.SocketObject
{
    public delegate void OnDataExchange(String Data);
    public delegate void OnClientConnectedDisconnected(IAsyncResult Async);
    public delegate void OnServerConnectedDisconnected(enumConnection ServerType);

    public enum DataType { dtReceivedData = 0, dtSendData = 1 };

    // Data Exchange Object Class 
    public class DataObject
    {
        public String Data;
        public String LastData;
        public Object oPacket;
        public int iPacketLenght;
        public DataType m_DataType;
        public System.Object m_SynchronizationObject = new System.Object();
        public OnDataExchange m_pOnDataExchange;
        public AutoResetEvent m_evtStartSendingToEFEM;

        // Constructor
        public DataObject(OnDataExchange pOnRead, OnDataExchange pOnSend)
        {
            if (pOnRead != null)
            {
                m_DataType = DataType.dtReceivedData;
                m_pOnDataExchange = pOnRead;
            }
            if (pOnSend != null)
            {
                m_DataType = DataType.dtSendData;
                m_evtStartSendingToEFEM = new AutoResetEvent(false);
                m_pOnDataExchange = pOnSend;
            }
        }

        public void DisposeObject()
        {
            oPacket = null;
            m_SynchronizationObject = null;
            m_pOnDataExchange = null;
            m_evtStartSendingToEFEM = null;
        }

    }
    //  Socket Object Class - to exchange PC <-> PC  
    public class CSocketClientServer
    {
        // Private member
        const int MAX_BUFFER = 65536; // 64ko
        bool m_bExitReceiveThread;
        bool m_bExitSendThread;
        Boolean m_bConnected;
        String m_ClientName;
        Socket m_SocketServer;
        bool m_bServerMode;
        Socket m_SocketExchange;
        Object m_SocketSynchronization;
        String m_ServerName;
        enumConnection m_ServerType;
        short m_PortNum;
        DataObject m_DataReceived;
        DataObject m_DataSend;
        bool m_Silence_NoMsg;
        bool m_bSimulation;
        bool m_bDisconnecting;
        bool m_bConnecting;
        IAsyncResult m_AsyncResult;
        int m_IncValue;

        // Public member        
        public Thread ReadingThread;
        public Thread SendingThread;
        public event OnSocketLog EventSocketLog;
        public OnClientConnectedDisconnected m_Evt_OnClientDisconnected;
        public OnClientConnectedDisconnected m_Evt_OnClientConnected;
        public OnServerConnectedDisconnected m_Evt_OnServerDisconnected;


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Constructor
        public CSocketClientServer(enumConnection pServerType, bool bServerMode, String ServerName, short PortNum, String pName, OnSocketLog pEventLog, OnDataExchange pOnDataReceived, OnDataExchange pOnDataSend, OnClientConnectedDisconnected pOnClientConnected, OnClientConnectedDisconnected pOnClientDisconnected, OnServerConnectedDisconnected pOnServerDisconnected, bool pbSimulation)
        {
#if SIMULATION_EN_LOCAL
            MessageBox.Show(pServerType.ToString() + " Simulation Mode: Network parameter for local use in CSocketClientServer");
#endif
            EventSocketLog = pEventLog;
            m_bServerMode = bServerMode;
            m_ServerName = ServerName;
            m_PortNum = PortNum;
            m_ServerType = pServerType;
            m_bSimulation = pbSimulation;
            m_bConnected = false;
            m_ClientName = pName;
            m_SocketSynchronization = new object();
            m_DataReceived = new DataObject(pOnDataReceived, null);
            m_DataSend = new DataObject(null, pOnDataSend);
            m_Evt_OnClientDisconnected = pOnClientDisconnected;
            m_Evt_OnClientConnected = pOnClientConnected;
            m_Evt_OnServerDisconnected = pOnServerDisconnected;

            // Thread d'envoi
            SendingThread = new Thread(new ParameterizedThreadStart(Thread_SendData));
            SendingThread.Name = pName + "Thread_SendData";
            SendingThread.Start(m_DataSend);

            // Thread d'écoute
            ReadingThread = new Thread(new ParameterizedThreadStart(Thread_ReadData));
            ReadingThread.Name = pName + "Thread_ReadData";
            ReadingThread.Start(m_DataReceived);

        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //Properties
        public bool IsListening
        {
            get { return m_bConnecting; }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Exit Threads Correctly
        public void ExitSendThread()
        {
            m_bExitSendThread = true;
            for (int i = 0; i < 5; i++)
            {                
                m_DataSend.m_evtStartSendingToEFEM.Set();
                Thread.Sleep(100);
            }
        }
        public void ExitReceiveThread()
        {
            if ((m_SocketExchange != null) && (m_SocketExchange.Connected))
                m_SocketExchange.Close();
            m_bExitReceiveThread = true;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Thread: Read Data From Client
        private void Thread_ReadData(Object oData)
        {
            String Response;
            m_bExitReceiveThread = false;
            while (!m_bExitReceiveThread)
            {
                if (m_bConnected)
                {
                    try
                    {
                        // Receive data
                        int bytes = 0;
                        Response = "";
                        byte[] bytesReceived = new byte[MAX_BUFFER];
                        //lock (m_SocketSynchronization)
                        {
                            bytes = m_SocketExchange.Receive(bytesReceived, bytesReceived.Length, 0);
                            if (bytes > 0)
                            {
                                // Reponse recu
                                Response = Response + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                                lock (m_DataReceived.m_SynchronizationObject)
                                {
                                    m_DataReceived.Data = Response.Trim();
                                    //EventSocketLog.Invoke(m_ServertType, " [RECV] " + m_DataReceived.Data, "");
                                    //}
                                    // SI c'est un Accuse ALORS 
                                    //if (m_DataReceived.Data.Contains("ACK#:"))
                                    //{
                                    // Verifie Si la donnée est retournée
                                    //    String data = m_DataReceived.Data.Replace("ACK#:", "");
                                    //    lock (m_DataSend.m_SynchronizationObject)
                                    //    {
                                    //        if (data != m_DataSend.LastData)
                                    //        {
                                    //            EventSocketLog.Invoke(m_ServertType, "Bad reception", "Error Bad reception");
                                    //        }
                                    //    }
                                    //}
                                    //else // SINON Envoi accusé reception et Callback Msg recu
                                    //{
                                    //    SendData("ACK#:" + m_DataReceived.Data);
                                    m_DataReceived.m_pOnDataExchange(m_DataReceived.Data);
                                }
                            }
                            else
                                throw new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Not Connected", "in fct Thread_ReadData", false);
                        }
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.Message;
                        EventSocketLog.Invoke(m_ServerType, Msg, "");
                        if (m_bServerMode)
                        {
                            m_bDisconnecting = true;
                            m_Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (m_SocketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    Ex2.DisplayError(Ex2.ErrorCode);
                                    EventSocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
                                }
                            }
                        }
                        m_bConnected = false;
                        if (m_Evt_OnServerDisconnected!=null)
                            m_Evt_OnServerDisconnected.Invoke(m_ServerType);
                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = m_ClientName + " " + ETCPException.GetMessage(Ex.ErrorCode);
                        EventSocketLog.Invoke(m_ServerType, Msg, "");
                        if (m_bServerMode)
                        {
                            m_bDisconnecting = true;
                            m_Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (m_SocketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    Ex2.DisplayError(Ex2.ErrorCode);
                                    EventSocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
                                }
                            }
                        }
                        m_bConnected = false;

                        if (m_Evt_OnServerDisconnected != null) m_Evt_OnServerDisconnected.Invoke(m_ServerType);
                    }
                    catch (Exception E)
                    {
                        String Msg = m_ClientName + " " + E.Message;
                        EventSocketLog.Invoke(m_ServerType, Msg, "");
                        if (m_bServerMode)
                        {
                            m_bDisconnecting = true;
                            m_Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (m_SocketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    Ex2.DisplayError(Ex2.ErrorCode);
                                    EventSocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
                                }
                            }
                        }
                        m_bConnected = false;

                        if (m_Evt_OnServerDisconnected != null) m_Evt_OnServerDisconnected.Invoke(m_ServerType);
                    }
                }
                else
                    Thread.Sleep(200);
                //{
                //    if (m_bServerMode && m_bConnected)
                //    {
                //        m_bDisconnecting = true;
                //        m_Evt_OnClientDisconnected.Invoke(null);

                //        m_bConnected = false;
                //        m_bExitReceiveThread = true;
                //    }
                //}               
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Thread : Send Data to Client
        public void Thread_SendData(Object Data)
        {
            DataObject lSendDataObject = (DataObject)Data;
            m_bExitSendThread = false;
            while (!m_bExitSendThread)
            {
                lSendDataObject.m_evtStartSendingToEFEM.WaitOne(-1, true);
                lock (lSendDataObject.m_SynchronizationObject)
                {
                    if (m_bExitSendThread)
                        break;
                    try
                    {
                        int DtSent;
                        lSendDataObject.LastData = lSendDataObject.Data;
                        DtSent = m_SocketExchange.Send((byte[])lSendDataObject.oPacket, lSendDataObject.iPacketLenght, SocketFlags.None);
                        //EventSocketLog.Invoke(m_ServertType, " [SEND] " + lSendDataObject.Data, "");                         
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.Message;
                        EventSocketLog.Invoke(m_ServerType, Msg, "");
                        if (m_bServerMode)
                        {
                            m_bDisconnecting = true;
                            m_Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (m_SocketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    Ex2.DisplayError(Ex2.ErrorCode);
                                    EventSocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
                                }
                            }
                        }

                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = m_ClientName + " " + ETCPException.GetMessage(Ex.ErrorCode);
                        EventSocketLog.Invoke(m_ServerType, Msg, "");
                        if (m_bServerMode)
                        {
                            m_bDisconnecting = true;
                            m_Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (m_SocketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    Ex2.DisplayError(Ex2.ErrorCode);
                                    EventSocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        String Msg = m_ClientName + " " + E.Message;
                        EventSocketLog.Invoke(m_ServerType, Msg, "");
                        if (m_bServerMode)
                        {
                            m_bDisconnecting = true;
                            m_Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (m_SocketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    Ex2.DisplayError(Ex2.ErrorCode);
                                    EventSocketLog.Invoke(m_ServerType, "ErrorCleared#:", "");
                                }
                            }
                        }
                    }
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Properties
        public Boolean IsConnected { get { return m_bConnected; } }
        public String ClientName { get { return m_ClientName; } }
        public bool Silence_NoMsg
        {
            set { m_Silence_NoMsg = value; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------
        // Server Mode       
        public void ListenNetwork(short PortNum)
        {
            EventSocketLog.Invoke(m_ServerType, "Server waiting connection...", "");
            m_PortNum = PortNum;
            // create the socket
            if (m_SocketServer != null && m_SocketServer.IsBound)
                m_SocketServer.Close();
            m_SocketServer = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream,
                                  ProtocolType.Tcp);
            try
            {
                // bind the listening socket to the port

                IPEndPoint ep = new IPEndPoint(IPAddress.Any, m_PortNum);
                m_SocketServer.Bind(ep);

                // start listening
                m_SocketServer.Listen(1);

                // create the call back for any client connections...
                m_SocketServer.BeginAccept(new AsyncCallback(m_Evt_OnClientConnected), null);
                m_bConnecting = true;
                m_bDisconnecting = false;

            }
            catch (SocketException se)
            {
                ETCPException Ex2 = new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed. Error = " + se.Message, "in fct ListenNetwork() - Error = " + se.Message, false);
                Ex2.DisplayError(Ex2.ErrorCode);
            }

        }

        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                if (m_bDisconnecting)
                {
                    m_bDisconnecting = false;
                    m_bConnecting = false;
                    return;
                }
                if (!m_bConnecting && m_SocketServer.IsBound)
                    return;
                m_AsyncResult = asyn;
                EventSocketLog.Invoke(m_ServerType, "Incoming connection.", "");
                m_SocketExchange = m_SocketServer.EndAccept(m_AsyncResult);
                m_bConnected = true;
                m_bConnecting = false;
            }
            catch (ObjectDisposedException)
            {
                EventSocketLog.Invoke(m_ServerType, "Incoming connection failed.", "");
                throw new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", "in fct OnClientConnect()", false);
            }
            catch (SocketException se)
            {
                string strMess = se.Message;
                EventSocketLog.Invoke(m_ServerType, "Incoming connection failed.", "");
                throw new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", "in fct OnClientConnect()", false);
            }
        }
        
        //---------------------------------------------------------------------------------------------------------------------------------
        // Connection Managment
        // Create connection to Serveur
        public Int32 Connect()
        {
            //Affichage
            String Txt;
            Txt = m_ClientName + " Trying to connect";
            EventSocketLog.Invoke(m_ServerType, Txt, "");

            // Connecter
            try
            {
                if (m_bSimulation)
                    return ETCPException.NO_ERROR;

                // SI déjà connecté ALORS
                if (m_bConnected)
                {
                    Txt = m_ClientName + " Already connected";
                    EventSocketLog.Invoke(m_ServerType, Txt, "");
                    throw new ETCPException(m_ServerType, ETCPException.NO_ERROR_ALREADY_CONNECTED, "Already connected", "in fct CSocketClientServer.Connect()", m_Silence_NoMsg);
                }
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                EventSocketLog.Invoke(m_ServerType, Txt, "");

                // Creation du socket
                IPAddress Ip;
                try
                {
                    Ip = IPAddress.Parse(Win32Tools.GetAdresseIP(m_ServerName));
                }
                catch
                {
                    throw new ETCPException(m_ServerType, ETCPException.NO_TCP_SERVER_NOT_EXIST, "Bad IP address", "in fct CSocketClientServer.Connect()", m_Silence_NoMsg);
                }
                IPEndPoint ipEnd = new IPEndPoint(Ip, m_PortNum);
                lock (m_SocketSynchronization)
                {
                    if (m_SocketExchange != null)
                    {
                        m_SocketExchange.Close();
                        m_SocketExchange = null;
                    }
                    m_SocketExchange = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    m_SocketExchange.ReceiveTimeout = 0;
                    m_SocketExchange.SendTimeout = 5000;
                    try
                    {
                        m_SocketExchange.Connect(ipEnd);
                        if (m_SocketExchange.Connected)
                        {
                            Txt = m_ClientName + "  Connected";
                            m_bConnected = true;
                            EventSocketLog.Invoke(m_ServerType, Txt, "");
                            Txt = m_ClientName + "------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
                            EventSocketLog.Invoke(m_ServerType, Txt, "");
                        }
                    }
                    catch (SocketException E)
                    {
                        throw new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Starting connection failed. Error = " + E.Message, "in fct CSocketClientServer.Connect()", m_Silence_NoMsg);
                    }
                }
            }
            catch (ETCPException Ex)
            {
                string strMess = Ex.Message;
                throw;
            }
            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Stop connection to EFEM Serveur       
        public Int32 DisconnectServeur()
        {
            String Txt;

            // Affichage
            Txt = "Stop Server";
            EventSocketLog.Invoke(m_ServerType, Txt, "");

            // Disconnect
            try
            {
                // SI déjà déconnecté ALORS
                if (!m_bConnected)
                {
                    //Affichage
                    m_bDisconnecting = true;
                    Txt = "Server stopped";
                    EventSocketLog.Invoke(m_ServerType, Txt, "");
                    if (m_SocketServer != null && m_SocketServer.IsBound)
                        m_SocketServer.Close();
                    if ((m_SocketExchange != null) && (m_SocketExchange.Connected))
                        m_SocketExchange.Disconnect(false);
                    //Etat deconnecté
                    m_bConnecting = false;
                    m_bDisconnecting = false;
                    return 0;

                }
                //--- Déconneter                
                //Affichage
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                EventSocketLog.Invoke(m_ServerType, Txt, "");

                // Fermeture du socket serveur                   
                m_bDisconnecting = true;
                if (m_SocketServer.Connected)
                {
                    m_SocketServer.Disconnect(true);
                    if (m_SocketServer != null && m_SocketServer.IsBound)
                        m_SocketServer.Close();
                }
                if (m_SocketExchange.Connected)
                    m_SocketExchange.Disconnect(false);
                //Affichage
                Txt = "Server stopped";
                EventSocketLog.Invoke(m_ServerType, Txt, "");

                //Etat deconnecté
                m_bConnected = false;
                m_bConnecting = false;
                m_bDisconnecting = false;
            }
            catch (SocketException E)
            {
                throw new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Disconnection failed. Error =" + E.Message, "in fct DisconnectServeur()", m_Silence_NoMsg);
            }
            catch (ETCPException Ex)
            {
                string strMess = Ex.Message;
                throw;
            }
            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Stop connection to EFEM Serveur       
        public Int32 DisconnectClient()
        {
            String Txt;

            // Affichage
            Txt = m_ClientName + " Trying to disconnect";
            EventSocketLog.Invoke(m_ServerType, Txt, "");

            // Disconnect
            try
            {
                // SI déjà déconnecté ALORS
                if (!m_bConnected)
                {
                    //Affichage
                    Txt = m_ClientName + " Already disconnected";
                    EventSocketLog.Invoke(m_ServerType, Txt, "");

                    // Avertissement Already disconnected
                    throw new ETCPException(m_ServerType, ETCPException.NO_ERROR_ALREADY_DISCONNECTED, "Already disconnected.", "in fct DisconnectClient()", m_Silence_NoMsg);
                }
                //--- Déconneter

                //Affichage
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                EventSocketLog.Invoke(m_ServerType, Txt, "");

                // Fermeture du socket d'echange   
                if (m_SocketExchange != null)
                    m_SocketExchange.Close();

                //Affichage
                Txt = m_ClientName + " Disconnected";
                EventSocketLog.Invoke(m_ServerType, Txt, "");
                Txt = m_ClientName + "######################################################################################################################################################################################################";
                EventSocketLog.Invoke(m_ServerType, Txt, "");

                //Etat deconnecté
                m_bConnected = false;
            }
            catch (SocketException E)
            {
                throw new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Disconnection failed. Error = " + E.Message, "in fct DisconnectClient()", m_Silence_NoMsg);
            }
            catch (ETCPException Ex)
            {
                string strMess = Ex.Message;
                throw;
            }
            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        // Send Data 
        private void SendData(String Data, object oPacket, int iPacketLength)
        {
            lock (m_SocketSynchronization)
            {
                m_DataSend.Data = Data;
                m_DataSend.oPacket = oPacket;
                m_DataSend.iPacketLenght = iPacketLength;
                m_DataSend.m_evtStartSendingToEFEM.Set();
            }
            Thread.Sleep(5);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public void SendData(string strData)
        {
            try
            {
                if ((m_SocketExchange == null) || (!m_SocketExchange.Connected))
                    throw new Exception("Socket not connected.");

                byte[] iCmdPacket = new byte[MAX_BUFFER];
                int iCmdPacketLength = 0;

                //////////////////////////
                //Make a copy of the command to the command packet buffer.
                int iDataLength = strData.Length;
                for (iCmdPacketLength = 0; iCmdPacketLength < iDataLength; ++iCmdPacketLength)
                {
                    char cChar = strData[iCmdPacketLength];
                    iCmdPacket[iCmdPacketLength] = (byte)Convert.ToInt32(cChar);
                }

                //Terminate the packet.
                iCmdPacket[iCmdPacketLength++] = (int)0x0d;

                object oCmdPacket = iCmdPacket as object;
                SendData(strData, oCmdPacket, iCmdPacketLength);
            }
            catch (Exception ex)
            {
                throw new ETCPException(m_ServerType, ETCPException.NO_SOCKET_ERROR, "Send data with Socket failed", $"in fct CSocketClientServer.SendData() - {ex.Message} - {ex.StackTrace}", false);
            }
        }

        public void OnClientDisconnect()
        {
            m_IncValue = m_IncValue + 1;
            //ListenNetwork(m_PortNum);
            m_IncValue = m_IncValue + 1;
        }

        public void ChangeServer(string pServerName, short pPortNum)
        {
            m_ServerName = pServerName;
            m_PortNum = pPortNum;
        }
    }
}

