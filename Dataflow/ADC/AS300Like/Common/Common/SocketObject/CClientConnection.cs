using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using Common.EException;

namespace Common.SocketObject
{        
    public class CClientConnection
    {
        // Private member
        const int MAX_BUFFER = 65536; // octets
        bool m_bExitReceiveThread;
        bool m_bExitSendThread;
        String m_ClientName;
        Object m_SynchroSocketServer = new Object();
        Socket m_SocketExchange;
        Object m_SocketSynchronization;
        String m_ServerName="Unknown";
        enumConnection m_ServertType;
        short m_PortNum=5000;
        DataObject m_DataReceived;
        DataObject m_DataSend;
        bool m_bShutdoneDone = false;

        bool m_Silence_NoMsg;

        // Public member        
        public Thread ReadingThread;
        public Thread SendingThread;
        public event OnSocketLog m_SocketLog;
        private OnClientConnectedDisconnected m_EvtInt_OnClientDisconnected;

        public CClientConnection(enumConnection pServerType, String pClientName, Socket pSocket, OnSocketLog pEventLog, OnDataExchange pOnDataReceived, OnDataExchange pOnDataSend, OnClientConnectedDisconnected pEvtInt_OnClientDisconnected)
        {
            m_SocketLog = pEventLog;
            m_SocketExchange = pSocket;
            m_ServertType = pServerType;
            m_ClientName = pClientName;                                  
            m_SocketSynchronization = new object();

            m_DataReceived = new DataObject(pOnDataReceived, null);
            m_DataSend = new DataObject(null, pOnDataSend);
            m_EvtInt_OnClientDisconnected = pEvtInt_OnClientDisconnected;

            // Thread d'envoi
            SendingThread = new Thread(new ParameterizedThreadStart(Thread_SendData));
            SendingThread.Name = pClientName + "Thread_SendData";
            SendingThread.Start(m_DataSend);

            // Thread d'écoute
            ReadingThread = new Thread(new ParameterizedThreadStart(Thread_ReadData));
            ReadingThread.Name = pClientName + "Thread_ReadData";
            ReadingThread.Start(m_DataReceived);
        }
         //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Properties
        public bool bShutdoneDone
        {
            get { return m_bShutdoneDone; }
        }
        public Boolean IsConnected { get { return ((m_SocketExchange != null) && m_SocketExchange.Connected); } }
        public String ClientName { get { return m_ClientName; } }
        public bool Silence_NoMsg
        {
            set { m_Silence_NoMsg = value; }
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
            if ((m_SocketExchange != null) && (m_SocketExchange.Connected))
                m_SocketExchange.Close();            
            m_bExitReceiveThread = true;
            m_DataSend.m_evtStartSendingToEFEM.Set();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Thread: Read Data From Client
        private void Thread_ReadData(Object oData)
        {            
            String Response;
            m_bExitReceiveThread = false;
            while (!m_bExitReceiveThread)
            {
                if (m_SocketExchange.Connected)
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
                                    m_DataReceived.m_pOnDataExchange(m_DataReceived.Data);
                                }
                            }
                            else
                                throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, "Not Connected", "Not Connected", false);
                        }
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.Message;
                        m_SocketLog.Invoke(m_ServertType, Msg, "");
                        if (IsConnected)
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
                        m_bExitReceiveThread=true;
                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = m_ClientName + " " + ETCPException.GetMessage(Ex.ErrorCode);
                        m_SocketLog.Invoke(m_ServertType, Msg, "");
                        if (IsConnected)
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
                        m_bExitReceiveThread=true;
                    }
                    catch (Exception E)
                    {
                        String Msg = m_ClientName + " " + E.Message;
                        m_SocketLog.Invoke(m_ServertType, Msg, "");
                        if (IsConnected)
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
                        m_bExitReceiveThread=true;
                    }
                }
                else
                    Thread.Sleep(200);
            }
            // If ExitReceiveThread => use this thread to shutdown CClientConnection Object devenu inutile
            Shutdown();
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
                        //EventSocketLog.BeginInvoke(m_ServertType, " [SEND] " + lSendDataObject.Data, "", null ,null);                         
                    }
                    catch (SocketException Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.Message;
                        m_SocketLog.Invoke(m_ServertType, Msg, "");
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
                        m_bExitSendThread = true;
                    }
                    catch (ETCPException Ex)
                    {
                        String Msg = m_ClientName + " " + ETCPException.GetMessage(Ex.ErrorCode);
                        m_SocketLog.Invoke(m_ServertType, Msg, "");
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
                        m_bExitSendThread = true;
                    }
                    catch (Exception Ex)
                    {
                        String Msg = m_ClientName + " " + Ex.Message;
                        m_SocketLog.Invoke(m_ServertType, Msg, "");
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
                        m_bExitSendThread = true;
                    }
                }
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
            m_SocketLog.Invoke(m_ServertType, Txt, "");
            
            // Connecter
            try
            {
                // SI déjà connecté ALORS
                if (IsConnected)
                {
                    Txt = m_ClientName + " Already connected";
                    m_SocketLog.Invoke(m_ServertType, Txt, "");
                    throw new ETCPException(m_ServertType, (int)ETCPException.NO_ERROR_ALREADY_CONNECTED, "", "", m_Silence_NoMsg);
                }
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                m_SocketLog.Invoke(m_ServertType, Txt, "");

                // Creation du socket
                IPAddress Ip;
                try
                {
                    Ip = IPAddress.Parse(Win32Tools.GetAdresseIP(m_ServerName));
                }
                catch
                {
                    throw new ETCPException(m_ServertType, ETCPException.NO_TCP_SERVER_NOT_EXIST, "Bad IP address", "Bad IP address", m_Silence_NoMsg);
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
                        if (IsConnected)
                        {
                            Txt = m_ClientName + "  Connected";
                            m_SocketLog.Invoke(m_ServertType, Txt, "");
                            Txt = m_ClientName + "------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
                            m_SocketLog.Invoke(m_ServertType, Txt, "");                           
                        }
                    }
                    catch (SocketException E)
                    {
                        throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, E.Message, E.Message, m_Silence_NoMsg);
                    }                    
                }
            }
            catch ( ETCPException  Ex)
            {
                String ErrMsg = Ex.Message;
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
            m_SocketLog.Invoke(m_ServertType, Txt, "");

            // Disconnect
            try
            {
                // SI déjà déconnecté ALORS
                if (!IsConnected)
                {
                    //Affichage
                    Txt = m_ClientName + " Already disconnected";
                    m_SocketLog.Invoke(m_ServertType, Txt, "");
                    return 0;
                }
                //--- Déconneter

                //Affichage
                Txt = m_ClientName + " Computer Name: " + m_ServerName + " - " + m_PortNum;
                m_SocketLog.Invoke(m_ServertType, Txt, "");

                // Fermeture du socket d'echange   
                if (m_SocketExchange != null)
                    m_SocketExchange.Close();
                
                //Affichage
                Txt = m_ClientName + " Disconnected";
                m_SocketLog.Invoke(m_ServertType, Txt, "");
                Txt = m_ClientName + "######################################################################################################################################################################################################";
                m_SocketLog.Invoke(m_ServertType, Txt, "");
            }
            catch (SocketException E)
            {
                throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, E.Message, E.Message, m_Silence_NoMsg);
            }
            catch (ETCPException Ex)
            {
                String ErrMsg = Ex.Message;
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
        public bool SendData(string strData)
        {
            try
            {
                if ((m_SocketExchange == null) || (!m_SocketExchange.Connected))
                    return true;

                byte[] iCmdPacket = new byte[MAX_BUFFER];
               
                iCmdPacket = Encoding.ASCII.GetBytes(strData + "\r");
                int iCmdPacketLength = strData.Length + 1;
                object oCmdPacket = iCmdPacket as object;
                SendData(strData, oCmdPacket, iCmdPacketLength);
            }
            catch 
            {
                throw new ETCPException(m_ServertType, ETCPException.NO_SOCKET_ERROR, "Send data with Socket failed", "Send data with Socket failed", false);
            }
            return true;
        }

        public void Shutdown()
        {
            if (m_bShutdoneDone) return;
            // Kill Thread
            ExitSendThread();
            ExitReceiveThread();

            // Kill object
            if (ReadingThread != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (ReadingThread.ThreadState == ThreadState.Stopped)
                        Thread.Sleep(500);
                    else
                        break;
                }
            }
            ReadingThread = null;
            if (SendingThread != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (SendingThread.ThreadState != ThreadState.Stopped)
                        Thread.Sleep(500);
                    else
                        break;
                }
            }
            m_SynchroSocketServer = null;
            if ((m_SocketExchange != null) && (m_SocketExchange.Connected))
                m_SocketExchange.Close();
            m_SocketExchange = null;
            m_SocketSynchronization = null;
            if(m_DataReceived!=null) m_DataReceived.DisposeObject();
            m_DataReceived = null;
            if (m_DataSend != null) m_DataSend.DisposeObject();
            m_DataSend = null;
            SendingThread = null;
            m_SocketLog = null;
            m_bShutdoneDone = true;
            m_EvtInt_OnClientDisconnected.Invoke(null);
            m_EvtInt_OnClientDisconnected = null;
        }
    }
}      
