using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Common;
using Common.ADC;

namespace Common.EException
{
    public class EADCException : Exception
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
        String m_ADCServerTitle;
        bool m_bSilence;
        enumConnection m_ServerType;

        // Error Codes and Messages
        public const int NOERROR = 0;
        public const int NO_ERROR = 0;
        public const int NO_ERROR_UNKNOWN = 1;
        const String MSG_NO_ERROR_UNKNOWN = "Error unknown";
        //===============================================================================================================
        // ADC 100 -199
        //===============================================================================================================
        public const int NO_ERRBASE_ADC = 100;

        public const int NO_SYSTEM_EXCEPTION = NO_ERRBASE_ADC + 1;
        const String MSG_NO_SYSTEM_EXCEPTION = "System Exception occurs.";

        public const int NO_CONFIG_FILE_NOT_FOUND = NO_SYSTEM_EXCEPTION + 1;
        const String MSG_NO_CONFIG_FILE_NOT_FOUND = "Configuration file not found";

        public const int NO_ERROR_ALREADY_CONNECTED = NO_CONFIG_FILE_NOT_FOUND + 1;
        const String MSG_NO_ERROR_ALREADY_CONNECTED = "Client already connected";

        public const int NO_ERROR_ALREADY_DISCONNECTED = NO_ERROR_ALREADY_CONNECTED + 1;
        const String MSG_NO_ERROR_ALREADY_DISCONNECTED = "Client already disconnected";

        public const int NO_SOCKET_ERROR = NO_ERROR_ALREADY_DISCONNECTED + 1;
        const String MSG_NO_SOCKET_ERROR = "Socket error. Now, client is disconnected.";

        public const int NO_THREAD_START_WITH_ERROR = NO_SOCKET_ERROR + 1;
        const String MSG_NO_THREAD_START_WITH_ERROR = "Starting Thread error";

        public const int NO_ADC_SERVER_NOT_EXIST = NO_THREAD_START_WITH_ERROR + 1;
        const String MSG_NO_ADC_SERVER_NOT_EXIST = "This server does not exist.";

        public const int NO_TIMEOUT_SOCKET = NO_ADC_SERVER_NOT_EXIST + 1;
        const String MSG_NO_TIMEOUT_SOCKET = "Sending data Timeout. ADC was not connected or connexion interrupted.";

        public const int NO_CIMCONNECT_SETVARIABLE_FAILED = NO_TIMEOUT_SOCKET + 1;
        const String MSG_NO_CIMCONNECT_SETVARIABLE_FAILED = "Set Result to CIMConnect failed";

        public const int NO_NOT_CONNECTED = NO_CIMCONNECT_SETVARIABLE_FAILED + 1;
        const String MSG_NO_NOT_CONNECTED = "ADC is not connected";

        public const int NO_ADC_RESULTS_INCORRECT = NO_NOT_CONNECTED + 1;
        const String MSG_NO_ADC_RESULTS_INCORRECT = "ADC data results is incorrect.";

        public const int NO_ADC_RESERVED_DEFECT_VID = NO_ADC_RESULTS_INCORRECT + 1;
        const String MSG_NO_ADC_RESERVED_DEFECT_VID = "First/Last defect VID detection failed";

        public const int NO_ADC_RESERVED_MEAS2D_VID = NO_ADC_RESERVED_DEFECT_VID + 1;
        const String MSG_NO_ADC_RESERVED_MEAS2D_VID = "First/Last measurement 2D VID detection failed";

        public const int NO_ADC_RESERVED_MEAS3D_VID = NO_ADC_RESERVED_MEAS2D_VID + 1;
        const String MSG_NO_ADC_RESERVED_MEAS3D_VID = "First/Last measurement 3D VID detection failed";

        public const int NO_ADC_EDGE_DATA_ERROR = NO_ADC_RESERVED_MEAS3D_VID + 1;
        const String MSG_NO_ADC_EDGE_DATA_ERROR = "ADC Edge data sending failed.";

        public const int NO_ADC_NOT_CONNECTED_TO_DATABASE = NO_ADC_EDGE_DATA_ERROR + 1;
        const String MSG_NO_ADC_NOT_CONNECTED_TO_DATABASE = "ADC is not connected to database.";

        public const int NO_INVALID_ADC_RESPONSE = NO_ADC_NOT_CONNECTED_TO_DATABASE + 1;
        const String MSG_NO_INVALID_ADC_RESPONSE = "Invalid ADC response.";

        public const int NO_ADC_RECIPE_NOT_FOUND = NO_INVALID_ADC_RESPONSE + 1;
        const String MSG_NO_ADC_RECIPE_NOT_FOUND = "ADC recipe not found.";

        ///////// ATTENTION AJOUTER AUSSI LES ERREURS DANS E95GUIFrameWork.ErrorsADC ///////////


        //---------------------------------------------------------------------------------------------------------------
        // Constructor
        public EADCException(enumConnection ServerType, int NoError, String pUserMessage, String pLogMessage, bool bSilence)
        {
            m_UserMessage = pUserMessage;
            m_LogMessage = pLogMessage;
            m_ErrCode = NoError;
            m_bSilence = bSilence;
            m_ServerType = ServerType;
            switch (ServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT: m_ADCServerTitle = "ADC Frontside error";
                    break;
                case enumConnection.CONNECT_ADC_BACK: m_ADCServerTitle = "ADC Backside error";
                    break;
                case enumConnection.CONNECT_ADC_EDGE: m_ADCServerTitle = "ADC Edge error";
                    break;
                case enumConnection.CONNECT_NANOTOPO: m_ADCServerTitle = "Nanotopography error";
                    break;
                case enumConnection.CONNECT_ADC_DF: m_ADCServerTitle = "ADC Darkview error";
                    break;
                case enumConnection.CONNECT_ADC_BF1: m_ADCServerTitle = "ADC Brightfield 1 error";
                    break;
                case enumConnection.CONNECT_ADC_BF2: m_ADCServerTitle = "ADC Brightfield 2 error";
                    break;
                case enumConnection.CONNECT_ADC_LS: m_ADCServerTitle = "ADC Light Speed error";
                    break;
                case enumConnection.CONNECT_ROBOT:
                    m_ADCServerTitle = "Data collection error";
                    break;
                default: m_ADCServerTitle = "ADC Error";
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
                case NO_SYSTEM_EXCEPTION: Msg = MSG_NO_SYSTEM_EXCEPTION; break;
                case NO_CONFIG_FILE_NOT_FOUND: Msg = MSG_NO_CONFIG_FILE_NOT_FOUND; break;
                case NO_ERROR_ALREADY_CONNECTED: Msg = MSG_NO_ERROR_ALREADY_CONNECTED; break;
                case NO_ERROR_ALREADY_DISCONNECTED: Msg = MSG_NO_ERROR_ALREADY_DISCONNECTED; break;
                case NO_SOCKET_ERROR: Msg = MSG_NO_SOCKET_ERROR; break;
                case NO_THREAD_START_WITH_ERROR: Msg = MSG_NO_THREAD_START_WITH_ERROR; break;
                case NO_ADC_SERVER_NOT_EXIST: Msg = MSG_NO_ADC_SERVER_NOT_EXIST; break;
                case NO_TIMEOUT_SOCKET: Msg = MSG_NO_TIMEOUT_SOCKET; break;
                case NO_CIMCONNECT_SETVARIABLE_FAILED: Msg = MSG_NO_CIMCONNECT_SETVARIABLE_FAILED; break;
                case NO_NOT_CONNECTED: Msg = MSG_NO_NOT_CONNECTED; break;
                case NO_ADC_RESULTS_INCORRECT: Msg = MSG_NO_ADC_RESULTS_INCORRECT; break;
                case NO_ADC_RESERVED_DEFECT_VID: Msg = MSG_NO_ADC_RESERVED_DEFECT_VID; break;
                case NO_ADC_RESERVED_MEAS2D_VID: Msg = MSG_NO_ADC_RESERVED_MEAS2D_VID; break;
                case NO_ADC_RESERVED_MEAS3D_VID: Msg = MSG_NO_ADC_RESERVED_MEAS3D_VID; break;
                case NO_ADC_EDGE_DATA_ERROR: Msg = MSG_NO_ADC_EDGE_DATA_ERROR; break;
                case NO_ADC_NOT_CONNECTED_TO_DATABASE: Msg = MSG_NO_ADC_NOT_CONNECTED_TO_DATABASE; break;
                case NO_INVALID_ADC_RESPONSE: Msg = MSG_NO_INVALID_ADC_RESPONSE; break;
                case NO_ADC_RECIPE_NOT_FOUND: Msg = MSG_NO_ADC_RECIPE_NOT_FOUND; break;
                default: Msg = MSG_NO_ERROR_UNKNOWN; break;
            }
            return Msg;
        }
        //---------------------------------------------------------------------------------------------------------------
        public void DisplayError(int ErrorCode)
        {
            String Msg = m_UserMessage;            
            if (!m_bSilence)
            {
                FrmError f = new FrmError(m_ADCServerTitle, Msg);
                f.BringToFront();
                f.ShowDialog();
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        public void DisplayError(IADCCallBack pCallBack, int ErrorCode)
        {
            if (!m_bSilence)
            {
                pCallBack.NotifyADC_GUIDisplay(m_ServerType, "ERROR#:" + ErrorCode.ToString(), GetMessage(ErrorCode) + " - " + m_LogMessage );
            }
        }

        public int ErrorCode
        {
            get { return m_ErrCode; }
        }
        public String AddingMessage
        {
            get { return m_UserMessage; }
        }

        public enumConnection ServerType
        {
            get { return m_ServerType; }
        }
    }
}
