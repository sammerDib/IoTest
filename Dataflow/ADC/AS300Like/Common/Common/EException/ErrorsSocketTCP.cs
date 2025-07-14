using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Common;
using Common.PMAltasight;

namespace Common.EException
{
    public class ETCPException : Exception
    {
        // --- Recapitule les No d'erreurs
        // COHTService.ServerIO                     20  - 99
        // CADCService.ADC                          100 - 199
        // ETCPException                            200 - 299
        // EPMAException                            300 - 399
        // EEdgeException                           400 - 499
        // EServerIO                                500 - 599
        // EDarkfieldException                      600 - 699
        // EBrightFieldException                    700 - 799

        // Variables internes
        String m_UserMessage;
        String m_LogMessage;
        int m_ErrCode;
        String m_Title;
        bool m_bSilence;
        enumConnection m_ServerType;

        // Error Codes and Messages
        public const int NOERROR = 0;
        public const int NO_ERROR = 0;
        public const int NO_ERROR_UNKNOWN = 1;
        const String MSG_NO_ERROR_UNKNOWN = "Error unknown";
        //===============================================================================================================
        // TCP 200 - 299
        //===============================================================================================================
        public const int NO_ERRBASE_TCP = 200;
        public const int NO_CONFIG_FILE_NOT_FOUND = NO_ERRBASE_TCP + 1;
        const String MSG_NO_CONFIG_FILE_NOT_FOUND = "Configuration file not found";

        public const int NO_ERROR_ALREADY_CONNECTED = NO_CONFIG_FILE_NOT_FOUND + 1;
        const String MSG_NO_ERROR_ALREADY_CONNECTED = "Client already connected";

        public const int NO_ERROR_ALREADY_DISCONNECTED = NO_ERROR_ALREADY_CONNECTED + 1;
        const String MSG_NO_ERROR_ALREADY_DISCONNECTED = "Client already disconnected";

        public const int NO_SOCKET_ERROR = NO_ERROR_ALREADY_DISCONNECTED + 1;
        const String MSG_NO_SOCKET_ERROR = "Socket error. Now, client is disconnected.";

        public const int NO_THREAD_START_WITH_ERROR = NO_SOCKET_ERROR + 1;
        const String MSG_NO_THREAD_START_WITH_ERROR = "Starting Thread error";

        public const int NO_TCP_SERVER_NOT_EXIST = NO_THREAD_START_WITH_ERROR + 1;
        const String MSG_NO_TCP_SERVER_NOT_EXIST = "This server does not exist.";

        public const int NO_TIMEOUT_SOCKET = NO_TCP_SERVER_NOT_EXIST + 1;
        const String MSG_NO_TIMEOUT_SOCKET = "Sending data Timeout. It was not connected or connexion interrupted.";

        public const int NO_TOO_EARLY_GETSTATUS = NO_TIMEOUT_SOCKET + 1;
        const String MSG_NO_TOO_EARLY_GETSTATUS = "GetStatus command sent too early. Wait delay to request it again";


        //---------------------------------------------------------------------------------------------------------------
        // Constructor
        public ETCPException(enumConnection ServerType, int NoError, String pUserMessage, String pLogMessage, bool bSilence)
        {
            m_UserMessage = pUserMessage;
            m_LogMessage = pLogMessage;
            m_ErrCode = NoError;
            m_bSilence = bSilence;
            m_ServerType = ServerType;
            switch (m_ServerType)
            {
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD1:
                    m_Title = "Brightfield PM1 error";
                    break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD2:
                    m_Title = "Brightfield PM2 error";
                    break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD3:
                    m_Title = "Brightfield PM3 error";
                    break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD4:
                    m_Title = "Brightfield PM4 error";
                    break;
                case enumConnection.CONNECT_GRAB_DARKFIELD:
                    m_Title = "Darkview PM error";
                    break;
                case enumConnection.CONNECT_PSD:
                    m_Title = "PSD PM error";
                    break;
                case enumConnection.CONNECT_PMEDGE:
                    m_Title = "Edge PM error";
                    break;
                case enumConnection.CONNECT_GRAB_EDGE:
                    m_Title = "Edge error";
                    break;
                case enumConnection.CONNECT_GRAB_REVIEW:
                    m_Title = "Review PM error";
                    break;
                case enumConnection.CONNECT_ADC_FRONT:
                    m_Title = "ADC Frontside error";
                    break;
                case enumConnection.CONNECT_ADC_BACK:
                    m_Title = "ADC Backside error";
                    break;
                case enumConnection.CONNECT_ADC_EDGE:
                    m_Title = "ADC Edge error";
                    break;
                case enumConnection.CONNECT_NANOTOPO:
                    m_Title = "Nanotopography error";
                    break;
                case enumConnection.CONNECT_ADC_DF:
                    m_Title = "ADC Darkview error";
                    break;
                case enumConnection.CONNECT_ADC_BF1:
                    m_Title = "ADC Brightfield 1 error";
                    break;
                case enumConnection.CONNECT_ADC_BF2:
                    m_Title = "ADC Brightfield 2 error";
                    break;
                case enumConnection.CONNECT_PMLS:
                    m_Title = "Light speed error";
                    break;
                case enumConnection.CONNECT_ADC_LS:
                    m_Title = "ADC Light Speed error";
                    break;
                default: 
                    m_Title = "Socket TCP Exchange error"; 
                    break;
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        // Fonctions
        public static String GetMessage(int ErroCode)
        {
            String Msg;
            switch (ErroCode)
            {
                case NO_CONFIG_FILE_NOT_FOUND: Msg = MSG_NO_CONFIG_FILE_NOT_FOUND; break;
                case NO_ERROR_ALREADY_CONNECTED: Msg = MSG_NO_ERROR_ALREADY_CONNECTED; break;
                case NO_ERROR_ALREADY_DISCONNECTED: Msg = MSG_NO_ERROR_ALREADY_DISCONNECTED; break;
                case NO_SOCKET_ERROR: Msg = MSG_NO_SOCKET_ERROR; break;
                case NO_THREAD_START_WITH_ERROR: Msg = MSG_NO_THREAD_START_WITH_ERROR; break;
                case NO_TCP_SERVER_NOT_EXIST: Msg = MSG_NO_TCP_SERVER_NOT_EXIST; break;
                case NO_TOO_EARLY_GETSTATUS: Msg = MSG_NO_TOO_EARLY_GETSTATUS; break;
                case NO_TIMEOUT_SOCKET: Msg = MSG_NO_TIMEOUT_SOCKET; break;
                
                default: Msg = MSG_NO_ERROR_UNKNOWN; break;
            }
            return Msg;
        }
        //---------------------------------------------------------------------------------------------------------------
        public void DisplayError(IPMAltasightCallback pCallBack, int ErrorCode)
        {
            if (!m_bSilence)
            {
                pCallBack.NotifyPM_GUIDisplay(m_ServerType, "ERROR#:" + ErrorCode.ToString(), m_UserMessage + " - " + m_LogMessage);
            }
        }
        //---------------------------------------------------------------------------------------------------------------
        public void DisplayError(int ErrorCode)
        {
            if (!m_bSilence)
            {
                String Msg = m_UserMessage;                
                if (!m_bSilence)
                {
                    FrmError f = new FrmError(m_Title, Msg);
                    f.BringToFront();
                    f.ShowDialog();
                }                
            }            
        }

        public int ErrorCode
        {
            get { return m_ErrCode; }
        }

    }
}
