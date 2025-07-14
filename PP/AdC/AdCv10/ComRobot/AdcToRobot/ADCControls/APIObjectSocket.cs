using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ADCControls
{
    public delegate void OnDataExchange(String Data);
    public delegate void OnClientConnectedDisconnected(IAsyncResult Async);
    public delegate void OnServerConnectedDisconnected();

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

    }
    //  Socket Object Class - to exchange PC <-> PC  
    public class CSocketClientServer
    {
        // Private member
        private const int MAX_BUFFER = 1024000; // 1024 + 512 octets
        private bool m_bExitReceiveThread;
        private bool m_bExitSendThread;
        private Boolean m_bConnected;
        private String m_ClientName;
        private Socket m_SocketServer;
        private Object m_SynchroSocketServer = new Object();
        private bool m_bServerMode;
        private Socket m_SocketExchange;
        private Object m_SocketSynchronization;
        private String m_ServerName;
        private short m_PortNum;
        private DataObject m_DataReceived;
        private DataObject m_DataSend;
        private bool m_Silence_NoMsg;
        private bool m_bSimulation;
        private bool m_bWaitingConnection;
        private IAsyncResult m_AsyncResult;
        private int m_iConnectionNbr = 0;

        // Public member        
        public Thread ReadingThread;
        public Thread SendingThread;
        public event OnSocketLog EventSocketLog;
        public OnClientConnectedDisconnected m_Evt_OnClientDisconnected;
        public OnClientConnectedDisconnected m_Evt_OnClientConnected;
        public OnServerConnectedDisconnected m_Evt_OnServerDisconnected;


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Constructor
        public CSocketClientServer(bool bServerMode, String ServerName, short PortNum, String pName, OnSocketLog pEventLog, OnDataExchange pOnDataReceived, OnDataExchange pOnDataSend, OnClientConnectedDisconnected pOnClientConnected, OnClientConnectedDisconnected pOnClientDisconnected, /*OnServerConnectedDisconnected pOnServerDisconnected, /**/ bool pbSimulation)
        {
            EventSocketLog = pEventLog;
            m_bServerMode = bServerMode;
            m_ServerName = ServerName;
            m_PortNum = PortNum;
            m_bSimulation = pbSimulation;
            m_bConnected = false;
            m_ClientName = pName;
            m_SocketSynchronization = new object();
            m_DataReceived = new DataObject(pOnDataReceived, null);
            m_DataSend = new DataObject(null, pOnDataSend);
            m_Evt_OnClientDisconnected = pOnClientDisconnected;
            m_Evt_OnClientConnected = pOnClientConnected;

            //m_Evt_OnServerDisconnected = pOnServerDisconnected;

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
            get { return m_bWaitingConnection; }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Exit Threads Correctly
        public void ExitSendThread()
        {
            m_bExitSendThread = true;
            m_DataSend.m_evtStartSendingToEFEM.Set();
        }

        public void ExitReceiveThread()
        {
            if (m_bServerMode)
            {
                if ((m_SocketServer != null) && (m_SocketServer.Connected))
                    m_SocketServer.Close();
            }
            else
            {
                if ((m_SocketExchange != null) && (m_SocketExchange.Connected))
                    m_SocketExchange.Close();
            }
            m_bExitReceiveThread = true;
            //TODO Faut-il faire ? m_DataReceived.m_evtStartSendingToEFEM.Set();
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
                        //m_ClientName
                        // Receive data
                        int bytes = 0;
                        //if (m_ClientName == "EyeEdge-rob")
                        //    bytes = 1;
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

                                    m_DataReceived.m_pOnDataExchange(m_DataReceived.Data);
                                }
                            }
                            else
                                throw new ETCPException(ETCPException.NO_SOCKET_ERROR, "Not Connected", false);
                        }
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.Message;
                        EventSocketLog.Invoke(Msg, "");
                        if (m_bServerMode)
                        {
                            OnClientDisconnect();
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
                                }
                            }
                        }
                        //m_Evt_OnServerDisconnected.Invoke(); 
                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.GetMessage(Ex.ErrorCode);
                        EventSocketLog.Invoke(Msg, "");
                        if (m_bServerMode)
                        {
                            OnClientDisconnect();
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
                                }
                            }
                        }
                        //m_Evt_OnServerDisconnected.Invoke(); 
                    }
                    catch (Exception E)
                    {
                        String Msg = m_ClientName + " " + E.Message;
                        EventSocketLog.Invoke(Msg, "");
                        if (m_bServerMode)
                        {
                            OnClientDisconnect();
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
                                }
                            }
                        }
                        //m_Evt_OnServerDisconnected.Invoke(); 
                    }
                }
                else
                    Thread.Sleep(200);
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
                        //EventSocketLog.BeginInvoke(" [SEND] " + lSendDataObject.Data, "", null ,null);                         
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.Message;
                        EventSocketLog.Invoke(Msg, "");
                        if (m_bServerMode)
                        {
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
                                }
                            }
                        }

                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.GetMessage(Ex.ErrorCode);
                        EventSocketLog.Invoke(Msg, "");
                        if (m_bServerMode)
                        {
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
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        String Msg = m_ClientName + " " + E.Message;
                        EventSocketLog.Invoke(Msg, "");
                        if (m_bServerMode)
                        {
                            m_Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if ((m_SocketExchange != null) && m_SocketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    Ex2.DisplayError(Ex2.ErrorCode);
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
            m_bWaitingConnection = false;
            EventSocketLog.Invoke("Server waiting connection...", "");
            m_PortNum = PortNum;
            // create the socket
            if (m_SocketServer != null && m_SocketServer.IsBound)
            {
                if (m_SocketServer.Connected)
                    m_SocketServer.Disconnect(false);
                m_SocketServer.Close();
            }
            m_SocketServer = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream,
                                  ProtocolType.Tcp);
            try
            {
                // bind the listening socket to the port
                //IPAddress hostIP = Dns.GetHostEntry("localhost").AddressList[0];
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, m_PortNum);
                m_SocketServer.Bind(ep);

                // start listening
                m_iConnectionNbr = 0;
                m_SocketServer.Listen(2);

                // create the call back for any client connections...
                m_SocketServer.BeginAccept(new AsyncCallback(OnClientConnect), null);
                Thread.Sleep(100);
                m_bWaitingConnection = true;

            }
            catch (SocketException se)
            {
                string serrmsg = se.Message;
                //m_SocketServer.Dispose();
                // throw new ETCPException(ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", false);
            }

        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                if (!m_bWaitingConnection)
                    return;
                if (!m_SocketServer.IsBound)
                    return;
                m_iConnectionNbr++;
                m_AsyncResult = asyn;
                EventSocketLog.Invoke("Incoming connection.", "");

                m_SocketExchange = m_SocketServer.EndAccept(m_AsyncResult);

                if (m_iConnectionNbr < 2)
                {
                    m_SocketServer.BeginAccept(new AsyncCallback(OnClientConnect), null);
                    m_bConnected = true;
                    m_bWaitingConnection = true;
                    EventSocketLog.Invoke("Client connected.", "");
                }
                else
                {
                    m_bConnected = true;
                    m_bWaitingConnection = false;
                    EventSocketLog.Invoke("Client connected.", "");
                }
            }
            catch
            {
                m_iConnectionNbr--;
                //EventSocketLog.Invoke("Incoming connection failed.", ""); /// OPF : Disfonctionnement du socket. evt_onClientConnect sur déconnexion. 
                EventSocketLog.Invoke("Server waiting connection...", "");
                //throw new ETCPException(ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", false);
            }

            m_Evt_OnClientConnected(asyn);
        }


        public void OnClientDisconnect()
        {
            if (m_iConnectionNbr > 0)
                m_iConnectionNbr--;
            if (m_iConnectionNbr >= 1)
                m_bConnected = true;
            else
                m_bConnected = false;

            if (!m_bExitReceiveThread && (m_iConnectionNbr > 0) && (m_iConnectionNbr < 2))
            {
                EventSocketLog.Invoke("Server waiting connection...", "");
                m_SocketServer.BeginAccept(new AsyncCallback(OnClientConnect), null);
                m_bWaitingConnection = true;
            }
            else
                if (!m_bExitReceiveThread && (m_iConnectionNbr == 0))
            {
                ListenNetwork(m_PortNum);
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
            EventSocketLog.Invoke(Txt, "");

            // Connecter
            try
            {
                if (m_bSimulation)
                    return ETCPException.NO_ERROR;

                // SI déjà connecté ALORS
                if (m_bConnected)
                {
                    Txt = m_ClientName + " Already connected";
                    EventSocketLog.Invoke(Txt, "");
                    throw new ETCPException(ETCPException.NO_ERROR_ALREADY_CONNECTED, "", m_Silence_NoMsg);
                }
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                EventSocketLog.Invoke(Txt, "");

                // Creation du socket
                IPAddress Ip;
                try
                {
                    Ip = IPAddress.Parse(GetAdresseIP());
                }
                catch
                {
                    throw new ETCPException(ETCPException.NO_TCP_SERVER_NOT_EXIST, "Bad IP address", m_Silence_NoMsg);
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
                            EventSocketLog.Invoke(Txt, "");
                            Txt = m_ClientName + "------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
                            EventSocketLog.Invoke(Txt, "");
                        }
                    }
                    catch (SocketException E)
                    {
                        throw new ETCPException(ETCPException.NO_SOCKET_ERROR, E.Message, m_Silence_NoMsg);
                    }
                }
            }
            catch (ETCPException Ex)
            {
                string serrmsg = Ex.GetMessage(Ex.ErrorCode);
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
            EventSocketLog.Invoke(Txt, "");

            // Disconnect
            try
            {
                // SI déjà déconnecté ALORS
                if (!m_bConnected)
                {
                    //Affichage               
                    Txt = "Server Stopped";
                    EventSocketLog.Invoke(Txt, "");

                    //Etat deconnecté
                    m_bWaitingConnection = false;

                    if (m_SocketServer != null && m_SocketServer.IsBound)
                        m_SocketServer.Close();
                    if ((m_SocketExchange != null) && (m_SocketExchange.IsBound))
                        m_SocketExchange.Close();
                    if ((m_SocketExchange != null) && (m_SocketExchange.Connected))
                        m_SocketExchange.Disconnect(true);
                    //Etat deconnecté
                    m_bWaitingConnection = false;
                    m_iConnectionNbr = 0;
                    return 0;

                }
                //--- Déconneter                
                //Affichage
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                EventSocketLog.Invoke(Txt, "");


                //Etat deconnecté
                m_bWaitingConnection = false;

                // Fermeture du socket => du port  
                if (m_SocketServer != null)
                    m_SocketServer.Close();
                if (m_SocketExchange != null)
                    m_SocketExchange.Close();

                //Affichage
                Txt = "Server stopped";
                EventSocketLog.Invoke(Txt, "");

                //Etat deconnecté
                m_bConnected = false;
                m_bWaitingConnection = false;
                m_iConnectionNbr = 0;
            }
            catch (SocketException E)
            {
                throw new ETCPException(ETCPException.NO_SOCKET_ERROR, E.Message, m_Silence_NoMsg);
            }
            catch (ETCPException Ex)
            {
                string serrmsg = Ex.GetMessage(Ex.ErrorCode);
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
            EventSocketLog.Invoke(Txt, "");

            // Disconnect
            try
            {
                // SI déjà déconnecté ALORS
                if (!m_bConnected)
                {
                    //Affichage
                    Txt = m_ClientName + " Already disconnected";
                    EventSocketLog.Invoke(Txt, "");

                    // Avertissement Already disconnected
                    throw new ETCPException(ETCPException.NO_ERROR_ALREADY_DISCONNECTED, "", m_Silence_NoMsg);
                }
                //--- Déconneter

                //Affichage
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                EventSocketLog.Invoke(Txt, "");

                // Fermeture du socket d'echange   
                if (m_SocketExchange != null)
                    m_SocketExchange.Close();

                //Affichage
                Txt = m_ClientName + " Disconnected";
                EventSocketLog.Invoke(Txt, "");
                Txt = m_ClientName + "######################################################################################################################################################################################################";
                EventSocketLog.Invoke(Txt, "");

                //Etat deconnecté
                m_bConnected = false;
            }
            catch (SocketException E)
            {
                throw new ETCPException(ETCPException.NO_SOCKET_ERROR, E.Message, m_Silence_NoMsg);
            }
            catch (ETCPException Ex)
            {
                string serrmsg = Ex.GetMessage(Ex.ErrorCode);
                throw;
            }
            return 0;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        // Get IP address according to the server name
        private String GetAdresseIP()
        {
            try
            {
                IPHostEntry IpHostEntry = Dns.GetHostEntry(m_ServerName);
                String IPStr = "";
                if (IpHostEntry.AddressList.Length != 0)
                {
                    for (int i = 0; i < IpHostEntry.AddressList.Length; i++)
                    {
                        Regex MyExp = new Regex("[0-9][0-9]?[0-9]?[.][0-9][0-9]?[0-9]?[.][0-9][0-9]?[0-9]?[.][0-9][0-9]?[0-9]?");
                        if (MyExp.IsMatch(IpHostEntry.AddressList[i].ToString()))
                            IPStr = IpHostEntry.AddressList[i].ToString();
                    }
                    return IPStr;
                }
            }
            catch
            {
                //MessageBox.Show(E.Message);
            }
            return "";
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
        public bool SendData(string strData)
        {
            try
            {
                if ((m_SocketExchange == null) || (!m_SocketExchange.Connected))
                    return true;

                int iDataLength = strData.Length;
                byte[] iCmdPacket = new byte[iDataLength + 1]; // +1 pour ajouter le caractère terminal.
                int iCmdPacketLength = 0;

                //////////////////////////
                //Make a copy of the command to the command packet buffer.

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
            catch
            {
                throw new ETCPException(ETCPException.NO_SOCKET_ERROR, "Send data with Socket failed", false);
            }
            return true;
        }

        public void ChangeServer(string pServerName, short pPortNum)
        {
            m_ServerName = pServerName;
            m_PortNum = pPortNum;
        }
    }
}

