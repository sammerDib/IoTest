using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public enum FctCode
    {
        ALTAMSG_GETVERSION = 0,
        ALTAMSG_SETPARAM,
        ALTAMSG_INIT,
        ALTAMSG_SETUP,
        ALTAMSG_CONTROL_START,
        ALTAMSG_CONTROL_GETSTATUS,
        ALTAMSG_CLOSE,
        ALTAMSG_RESULT_GET_STATUS,
        ALTAMSG_RESULT_WAFER,
        ALTAMSG_RESULT_SET_STATUS,
        ALTAMSG_GET_STATUS_START_PROCESS,
        ALTAMSG_GET_STATUS_SAVING_COMPLETE,
        ALTAMSG_ACK_STATUS_SAVING_COMPLETE,
        ALTAMSG_GET_STATUS_PROCESS,
        ALTAMSG_SET_ADA_PATH     			
    }

    public enum enumMsgStatus
    {
        ALTAMSG_STATUS_0_NONE = 100,
        ALTAMSG_STATUS_GRABBING = 101,
        ALTAMSG_STATUS_GRAB_FINISHED = 105,
        ALTAMSG_STATUS_GRAB_ERR = 106,
        ALTAMSG_STATUS_EXTRACTING_IMAGE = 107,
        ALTAMSG_STATUS_EXTRACTION_IMAGE_OK = 108,
        ALTAMSG_STATUS_IMAGES_SAVING = 109,
        ALTAMSG_STATUS_IMAGES_SAVED_OK = 110,
        ALTAMSG_STATUS_IMAGES_SAVED_ERR = 111,


        ALTAMSG_STATUS_DO_NOT_START_PROCESS = 200,
        ALTAMSG_STATUS_START_PROCESS = 201,
        ALTAMSG_STATUS_START_PROCESS_ERROR = 202,

        ALTAMSG_STATUS_PROCESS_IN_PROGRESS = 300,
        ALTAMSG_STATUS_PROCESS_COMPLETE = 301,
        ALTAMSG_STATUS_PROCESS_ERROR = 302,

        ALTAMSG_STATUS_SAVING_IN_PROGRESS = 400,
        ALTAMSG_STATUS_SAVING_COMPLETE = 401,
        ALTAMSG_STATUS_SAVING_ERROR = 402
    }

    public class CSocketClientAscii
    {
        const int TIMEOUT_SENDMSG = 45; // 45 Secondes

        CSocketClientServer m_SocketClient;
        OnSocketLog m_SocketLog;
        OnSocketLog m_SocketLogDisplay;
        OnDataExchange m_OnDataRead;
        OnDataExchange m_OnDataSent;
        OnServerConnectedDisconnected m_OnServerDisconnected;
        
        String m_LastMsg="";
        int m_iMessageCount=0;
        String m_ServerName;
        short m_PortNum;
        String m_ClientName;
        bool m_bConnected;
        Connection m_ServerType;
        AutoResetEvent m_evtDataReceivedSending;
        bool m_bSendEventAck=false;
        String m_Response="";

        public CSocketClientAscii(Connection pServerType, String ServerName, short PortNum, OnSocketLog pEventLog, OnServerConnectedDisconnected pOnServerDisconnected)
        {
            m_SocketLog = pEventLog;
            m_ServerName = ServerName;
            m_ServerType = pServerType;
            m_PortNum = PortNum;
            m_OnServerDisconnected = pOnServerDisconnected;

            m_ClientName = "Unknown";
            m_iMessageCount = 0;

            m_evtDataReceivedSending = new AutoResetEvent(false);

            // Define client name according to server type
            switch (m_ServerType)
            {
                case Connection.CONNECT_ACQUISITION_FRONTSIDE: m_ClientName = "[Acquisition Frontside client]";
                    break;
                case Connection.CONNECT_ACQUISITION_BACKSIDE: m_ClientName = "[Acquisition Backside client]";
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
        public int SendMsg(FctCode FunctionCode, String Question)
        {
            String MsgCode = "#" + m_iMessageCount.ToString("D4") + "R" + ((int)FunctionCode).ToString("D4") + "=" + Question;
            m_SocketClient.SendData(MsgCode);            
            m_iMessageCount = (m_iMessageCount + 1) % 10;
            return ETCPException.NO_ERROR;
        }

        public int SendMsg(FctCode FunctionCode, String Question, out String Response)
        {             
            m_bSendEventAck = true;            
            SendMsg(FunctionCode, Question);
            DateTime StartSend = DateTime.Now;            
            bool bStopTimeOut = false;
            while (!m_evtDataReceivedSending.WaitOne(100, false))
            {
                Application.DoEvents();
                bStopTimeOut = (((TimeSpan)DateTime.Now.Subtract(StartSend)).TotalSeconds > TIMEOUT_SENDMSG);
                if (!IsConnected || bStopTimeOut)
                {
                    Response = "";
                    if (IsConnected) Disconnect(); 
                    return ETCPException.NO_TIMEOUT_SOCKET;
                }
            }
            m_bSendEventAck = false;
            lock (m_Response)
            {
                Response = m_Response;
                m_Response = "";                
            }
            return ETCPException.NO_ERROR;
        }
        public void OnDataRead(String Data)
        {
            lock (m_Response)
            {
                if (m_bSendEventAck)
                {
                    m_evtDataReceivedSending.Set();
                }
                if (m_Response != "")
                    Thread.Sleep(2000);
                
                m_Response = ReceiveMsg(Data);
            }
        }

        public void OnDataSent(String Data)
        {

        }        

        public void OnClientDisconnect()
        {
            
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------
        // Extract Received Msg
        public String ReceiveMsg(String Msg)
        {
            // Receive data            
            String[] sTab = Msg.Split('=');
            String Response = sTab[1];            
            return Response;
        }
                      

        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int GetCaptureVersion( out String sVersion)
        {
            return SendMsg(FctCode.ALTAMSG_GETVERSION, "", out sVersion);            
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int GetGrabberStatus(FctCode Fct, out enumMsgStatus Status)
        {
            String sReponse;
            short iStatus = 0;
            Status = enumMsgStatus.ALTAMSG_STATUS_0_NONE;  
            int ErrorCode = SendMsg(Fct, "", out sReponse);
            if ((ErrorCode == ENanotopographyException.NOERROR) || (sReponse!="")) 
            {
                // Take Status from String
                String[] sTab = sReponse.Split('-');
                iStatus = Convert.ToInt16(sTab[0].Trim()); 
                // Check status conformity
                if ((iStatus > (int)enumMsgStatus.ALTAMSG_STATUS_0_NONE) && (iStatus < (int)enumMsgStatus.ALTAMSG_STATUS_SAVING_ERROR))
                {
                    Status = (enumMsgStatus)iStatus;
                    return ENanotopographyException.NOERROR;
                }
                else
                    return ENanotopographyException.NO_GET_STATUS_MODULE_FAILED;
            }
            else
                return ENanotopographyException.NO_GET_STATUS_MODULE_FAILED;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int SendStartProcess(String sParameters, out String sReponse)
        {
            return SendMsg(FctCode.ALTAMSG_CONTROL_START, sParameters, out sReponse);            
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int SendKillGrabber( out String sReponse)
        {
            String sParameters = "";
            return SendMsg(FctCode.ALTAMSG_CLOSE, sParameters, out sReponse);            
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        public int SendInitGrabber(out String sReponse)
        {
            String sParameters = "";
            return SendMsg(FctCode.ALTAMSG_INIT, sParameters, out sReponse);            
        }

        public void Connect()
        {
            try
            {
                if (m_SocketClient == null)
                {
                    m_OnDataRead += new OnDataExchange(OnDataRead);
                    m_OnDataSent += new OnDataExchange(OnDataSent);
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
    }
}

