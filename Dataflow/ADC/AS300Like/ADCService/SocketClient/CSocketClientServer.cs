using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.EException;


namespace UnitySC.ADCAS300Like.Service.SocketClient
{
    public delegate void OnDataExchange(String Data);
    public delegate void OnClientConnectedDisconnected(IAsyncResult Async);
    public delegate void OnServerConnectedDisconnected(ADCType ServerType);

    public enum DataType { dtReceivedData = 0, dtSendData = 1 };
    
    // Data Exchange Object Class 
    public class DataObject
    {
        public String Data;
        public String LastData;
        public Object oPacket;
        public int iPacketLenght;
        public DataType DataType;
        public System.Object SynchronizationObject = new System.Object();
        public OnDataExchange OnDataExchange;        
        public AutoResetEvent EvtStartSendingToEFEM;
        
        // Constructor
        public DataObject(OnDataExchange pOnRead, OnDataExchange pOnSend)
        {
            if (pOnRead != null)
            {
                DataType = DataType.dtReceivedData ;
                OnDataExchange = pOnRead;     
            }
            if (pOnSend != null)            
            {         
                DataType = DataType.dtSendData;
                EvtStartSendingToEFEM = new AutoResetEvent(false);
                OnDataExchange = pOnSend;
            }
        }
        
    }
    //  Socket Object Class - to exchange PC <-> PC  
    public class CSocketClientServer
    {
        // Private member
        private const int MAX_BUFFER = 1024000; //65536; // 64ko
        private bool _bExitReceiveThread;
        private bool _bExitSendThread;
        private Boolean _bConnected;
        private String _clientName;
        private Socket _socketServer;
        private bool _bServerMode;
        private Socket _socketExchange;
        private Object _socketSynchronization;
        private String _serverName;
        private ADCType _serverType;
        private short _portNum;
        private DataObject _dataReceived;
        private DataObject _dataSend;
        private bool _silence_NoMsg;
        private bool _bSimulation;
        private bool _bDisconnecting;
        private bool _bConnecting;
        private IAsyncResult _asyncResult;
        private int _incValue;
        
        // Public member        
        public Thread ReadingThread;
        public Thread SendingThread;
        public event OnSocketLog EventSocketLog;
        public OnClientConnectedDisconnected Evt_OnClientDisconnected;
        public OnClientConnectedDisconnected Evt_OnClientConnected;
        public OnServerConnectedDisconnected Evt_OnServerDisconnected;
        

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Constructor
        public CSocketClientServer(ADCType pServerType, bool bServerMode, String serverName, short portNum, String pName, OnSocketLog pEventLog, OnDataExchange pOnDataReceived, OnDataExchange pOnDataSend, OnClientConnectedDisconnected pOnClientConnected, OnClientConnectedDisconnected pOnClientDisconnected, OnServerConnectedDisconnected pOnServerDisconnected, bool pbSimulation)
        {
            EventSocketLog = pEventLog;

            _bServerMode = bServerMode;
            _serverName = serverName;
            _portNum = portNum;
            _serverType = pServerType;
            _bSimulation = pbSimulation;
            _bConnected = false;
            _clientName = pName;                                  
            _socketSynchronization = new object();
            _dataReceived = new DataObject(pOnDataReceived, null);
            _dataSend = new DataObject(null, pOnDataSend);
            Evt_OnClientDisconnected = pOnClientDisconnected;
            Evt_OnClientConnected = pOnClientConnected;
            Evt_OnServerDisconnected = pOnServerDisconnected;

            string Txt = _clientName + " Computer Name: " + _serverName + " - " + _portNum;
            EventSocketLog.Invoke(_serverType, Txt, "");
            // Thread d'envoi
            SendingThread = new Thread(new ParameterizedThreadStart(Thread_SendData));
            SendingThread.Name = pName + "Thread_SendData";
            SendingThread.Start(_dataSend);

            // Thread d'écoute
            ReadingThread = new Thread(new ParameterizedThreadStart(Thread_ReadData));
            ReadingThread.Name = pName + "Thread_ReadData";
            ReadingThread.Start(_dataReceived);
            
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        //Properties
        public bool IsListening
        {
            get { return _bConnecting; }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Exit Threads Correctly
        public void ExitSendThread()
        {
            _bExitSendThread = true;
            _dataSend.EvtStartSendingToEFEM.Set(); 
        }     
        public void ExitReceiveThread()
        {
            if ((_socketExchange != null) && (_socketExchange.Connected))
                _socketExchange.Close();
            _bExitReceiveThread = true;
            _dataSend.EvtStartSendingToEFEM.Set();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Thread: Read Data From Client
        private void Thread_ReadData(Object oData)
        {            
            String Response;
            _bExitReceiveThread = false;
            while (!_bExitReceiveThread)
            {
                if (_bConnected)
                {
                    try
                    {
                        // Receive data
                        int bytes = 0;
                        Response = "";
                        byte[] bytesReceived = new byte[MAX_BUFFER];
                        //lock (m_SocketSynchronization)
                        {
                            bytes = _socketExchange.Receive(bytesReceived, bytesReceived.Length, 0);
                            if (bytes > 0)
                            {
                                // Reponse recu
                                Response = Response + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                                lock (_dataReceived.SynchronizationObject)
                                {
                                    _dataReceived.Data = Response.Trim();
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
                                    _dataReceived.OnDataExchange(_dataReceived.Data);
                                }
                            }
                            else
                                throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Not Connected", "in Thread_ReadData - Not connected", false);
                        }
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = _clientName + " " + Ex.Message;
                        EventSocketLog.Invoke(_serverType, Msg, "");
                        if (_bServerMode)
                        {
                            _bDisconnecting = true;
                            Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (_socketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    EventSocketLog.Invoke(_serverType, "Communication error", ETCPException.GetMessage(Ex2.ErrorCode));
                                }
                            }
                        }
                        _bConnected = false;
                        Evt_OnServerDisconnected.Invoke(_serverType ); 
                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = _clientName + " " + ETCPException.GetMessage(Ex.ErrorCode);
                        EventSocketLog.Invoke(_serverType, Msg, "");
                        if (_bServerMode)
                        {
                            _bDisconnecting = true;
                            Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (_socketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    EventSocketLog.Invoke(_serverType, "Communication error", ETCPException.GetMessage(Ex2.ErrorCode));
                                }
                            }
                        }
                        _bConnected = false;
                        Evt_OnServerDisconnected.Invoke(_serverType); 
                    }
                    catch (Exception E)
                    {
                        String Msg = _clientName + " " + E.Message;
                        EventSocketLog.Invoke(_serverType, Msg, "");
                        if (_bServerMode)
                        {
                            _bDisconnecting = true;
                            Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (_socketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    EventSocketLog.Invoke(_serverType, "Communication error", ETCPException.GetMessage(Ex2.ErrorCode));
                                }
                            }
                        }
                        _bConnected = false;
                        Evt_OnServerDisconnected.Invoke(_serverType); 
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
            _bExitSendThread = false;
            while (!_bExitSendThread)
            {
                lSendDataObject.EvtStartSendingToEFEM.WaitOne(-1, true);
                lock (lSendDataObject.SynchronizationObject)
                {
                    if (_bExitSendThread)
                        break;
                    try
                    {
                        int DtSent;
                        lSendDataObject.LastData = lSendDataObject.Data;
                        DtSent = _socketExchange.Send((byte[])lSendDataObject.oPacket, lSendDataObject.iPacketLenght, SocketFlags.None);
                        //EventSocketLog.Invoke(m_ServertType, " [SEND] " + lSendDataObject.Data, "");                         
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = _clientName + " " + Ex.Message;
                        EventSocketLog.Invoke(_serverType, Msg, "");
                        if (_bServerMode)
                        {
                            _bDisconnecting = true;
                            Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (_socketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    EventSocketLog.Invoke(_serverType, "Communication error", ETCPException.GetMessage(Ex2.ErrorCode));
                                }
                            }
                        }

                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = _clientName + " " + ETCPException.GetMessage(Ex.ErrorCode);
                        EventSocketLog.Invoke(_serverType, Msg, "");
                        if (_bServerMode)
                        {
                            _bDisconnecting = true;
                            Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (_socketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    EventSocketLog.Invoke(_serverType, "Communication error", ETCPException.GetMessage(Ex2.ErrorCode));
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        String Msg = _clientName + " " + E.Message;
                        EventSocketLog.Invoke(_serverType, Msg, "");
                        if (_bServerMode)
                        {
                            _bDisconnecting = true;
                            Evt_OnClientDisconnected.Invoke(null);
                        }
                        else
                        {
                            if (_socketExchange.Connected)
                            {
                                try
                                {
                                    DisconnectClient();
                                }
                                catch (ETCPException Ex2)
                                {
                                    EventSocketLog.Invoke(_serverType, "Communication error", ETCPException.GetMessage(Ex2.ErrorCode));
                                }
                            }
                        }
                    }
        }
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Properties
        public Boolean IsConnected { get { return _bConnected; } }
        public String ClientName { get { return _clientName; } }
        public bool Silence_NoMsg
        {
            set { _silence_NoMsg = value; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------
        // Server Mode       
        public void ListenNetwork_HostEntry(short PortNum)
        {
            EventSocketLog.Invoke(_serverType, "Server waiting connection...", "");
            _portNum = PortNum;
            // create the socket
            if (_socketServer != null && _socketServer.IsBound)
                _socketServer.Close();
            _socketServer = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream,
                                  ProtocolType.Tcp);
            try
            {
                // bind the listening socket to the port
                IPAddress[] ipv4Addresses = Array.FindAll(Dns.GetHostEntry(string.Empty).AddressList,a => a.AddressFamily == AddressFamily.InterNetwork);

                //IPAddress[] llist = Dns.GetHostEntry(IPAddress.Any.ToString()).AddressList;
                IPAddress hostIP = ipv4Addresses[0];
                IPEndPoint ep = new IPEndPoint(hostIP, _portNum);
                _socketServer.Bind(ep);

                // start listening
                _socketServer.Listen(1);
                // create the call back for any client connections...
                _socketServer.BeginAccept(new AsyncCallback(Evt_OnClientConnected), null);
                _bConnecting = true;
                _bDisconnecting = false;

            }
            catch (SocketException se)
            {
                string strMess = se.Message;
                throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", "in ListenNetwork() -  SocketException: " + se.Message, false);
            }
        
        }

        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                if (_bDisconnecting)
                {
                    _bDisconnecting = false;
                    _bConnecting = false;
                    return;
                }
                if (!_bConnecting && _socketServer.IsBound)
                    return;
                _asyncResult = asyn;
                EventSocketLog.Invoke(_serverType, "Incoming connection.", "");
                _socketExchange = _socketServer.EndAccept(_asyncResult);
                _bConnected = true;
                _bConnecting = false;
            }
            catch (ObjectDisposedException Ex)
            {
                EventSocketLog.Invoke(_serverType, "Incoming connection failed.", "");
                throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", "in OnClientConnect() - ObjectDisposedException: " + Ex.Message, false);            
            }
            catch (SocketException se)
            {
                string strMess = se.Message;
                EventSocketLog.Invoke(_serverType, "Incoming connection failed.", "");
                throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Listen incoming connection failed", "in OnClientConnect() -  SocketException: " + se.Message, false);
            }
        }
        



        


        //---------------------------------------------------------------------------------------------------------------------------------
        // Connection Managment
        // Create connection to Serveur
        public Int32 Connect()
        {            
            //Affichage
            String Txt;
            
            // Connecter
            try
            {
                if (_bSimulation)
                    return ETCPException.NO_ERROR;

                // SI déjà connecté ALORS
                if (_bConnected)
                {
                    Txt = _clientName + " Already connected";
                    EventSocketLog.Invoke(_serverType, Txt, "");
                    throw new ETCPException(_serverType, ETCPException.NO_ERROR_ALREADY_CONNECTED, "Already connected", "", _silence_NoMsg);
                }

                // Creation du socket
                IPAddress Ip;
                try
                {
                    if (_serverName.ToUpper() == "LOCALHOST")
                        _serverName = "127.0.0.1";
                    Ip = IPAddress.Parse(Win32Tools.GetAdresseIP(_serverName));
                }
                catch
                {
                    throw new ETCPException(_serverType, ETCPException.NO_TCP_SERVER_NOT_EXIST, "Bad IP address", "in Connect() - Exception in GetAdresseIP", _silence_NoMsg);
                }
                IPEndPoint ipEnd = new IPEndPoint(Ip, _portNum);
                lock (_socketSynchronization)  
                {
                    if (_socketExchange != null)
                    {
                        _socketExchange.Close();
                        _socketExchange = null;
                    }
                    _socketExchange = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                       
                    _socketExchange.ReceiveTimeout = 0;
                    _socketExchange.SendTimeout = 5000;
                    try
                    {
                        _socketExchange.Connect(ipEnd);
                        if (_socketExchange.Connected)
                        {
                            Txt = _clientName + " Connected";
                            _bConnected = true;
                            EventSocketLog.Invoke(_serverType, Txt, "");
                            Txt = _clientName + "------------------------------------------------------------------------";
                            EventSocketLog.Invoke(_serverType, Txt, "");                           
                        }
                    }
                    catch (SocketException E)
                    {
                        throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Connection failed", "in Connect() - SocketException: " + E.Message, _silence_NoMsg);
                    }                    
                }
            }
            catch ( ETCPException  Ex)
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
            EventSocketLog.Invoke(_serverType, Txt, "");
            
            // Disconnect
            try
            {       
                // SI déjà déconnecté ALORS
                if (!_bConnected)
                {
                    //Affichage
                    _bDisconnecting = true;                    
                    Txt = "Server stopped";
                    EventSocketLog.Invoke(_serverType, Txt, "");
                    if (_socketServer != null && _socketServer.IsBound)
                        _socketServer.Close();
                    if ((_socketExchange != null) && (_socketExchange.Connected))
                        _socketExchange.Disconnect(false);
                    //Etat deconnecté
                    _bConnecting = false;
                    _bDisconnecting = false;    
                    return 0;

                }
                //--- Déconneter                
                //Affichage
                Txt = _clientName + " Computer Name: " + _serverName + " - " + _portNum;
                EventSocketLog.Invoke(_serverType, Txt, "");

                // Fermeture du socket serveur                   
                _bDisconnecting = true;
                if (_socketServer.Connected)
                {
                    _socketServer.Disconnect(true);
                    if (_socketServer != null && _socketServer.IsBound)
                        _socketServer.Close();
                }
                if (_socketExchange.Connected)
                    _socketExchange.Disconnect(false);
                //Affichage
                Txt = "Server stopped";
                EventSocketLog.Invoke(_serverType, Txt, "");
                              
                //Etat deconnecté
                _bConnected = false;
                _bConnecting = false;
                _bDisconnecting = false;    
            }
            catch (SocketException E)
            {
                throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Disconnection failed.", "DisconnectServeur() - SocketException: " + E.Message, _silence_NoMsg);
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

            // Disconnect
            try
            {
                // SI déjà déconnecté ALORS
                if (!_bConnected)
                {
                    //Affichage
                    Txt = _clientName + " Already disconnected";
                    EventSocketLog.Invoke(_serverType, Txt, "");

                    // Avertissement Already disconnected
                    throw new ETCPException(_serverType, ETCPException.NO_ERROR_ALREADY_DISCONNECTED, "Already disconnected", "in DisconnectClient() - Already disconnected.",  _silence_NoMsg);
                }
                //--- Déconneter

                //Affichage
                Txt = _clientName + " Computer Name: " + _serverName + " - " + _portNum;
                EventSocketLog.Invoke(_serverType, Txt, "");

                // Fermeture du socket d'echange   
                if (_socketExchange != null)
                    _socketExchange.Close();
                
                //Affichage
                Txt = _clientName + " Disconnected";
                EventSocketLog.Invoke(_serverType, Txt, "");
                Txt = _clientName + "######################################################################################################################################################################################################";
                EventSocketLog.Invoke(_serverType, Txt, "");                

                //Etat deconnecté
                _bConnected = false;
            }
            catch (SocketException E)
            {
                throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Disconnection failed.", "in DisconnectClient() - SocketException: " + E.Message, _silence_NoMsg);
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
            lock (_socketSynchronization)
            {
                _dataSend.Data = Data;
                _dataSend.oPacket = oPacket;
                _dataSend.iPacketLenght = iPacketLength;
                _dataSend.EvtStartSendingToEFEM.Set();
            }                          
            Thread.Sleep(5);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public bool SendData(string strData)
        {
            try
            {
                if ((_socketExchange == null) || (!_socketExchange.Connected))
                    return true;

                int iCmdPacketLength = 0;

                //////////////////////////
                //Make a copy of the command to the command packet buffer.
                int iDataLength = strData.Length;
                byte[] iCmdPacket = new byte[iDataLength + 10];
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
                throw new ETCPException(_serverType, ETCPException.NO_SOCKET_ERROR, "Communication error. Sending data failed.","in SendData() - Send data with Socket failed.", false);
            }
            return true;
        }

        public void OnClientDisconnect()
        {
           _incValue = _incValue + 1;
           //ListenNetwork(m_PortNum);
           _incValue = _incValue + 1;
        }

        public void ChangeServer(string pServerName, short pPortNum)
        {
            _serverName = pServerName;
            _portNum = pPortNum;
        }
    }
}
      
