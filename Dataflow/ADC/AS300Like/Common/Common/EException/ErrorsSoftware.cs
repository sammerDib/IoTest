using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Common;
using Common.PMAltasight;

namespace Common.EException
{
    public class ESoftwareException : Exception
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
        //                                          800 - 899        
        // ESoftwareException                       900 - 999
        // EPMException                            1000 - 1099
        // EEFEMEngineException                    1100 - 1199

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
        // ECommonException 900 - 999
        //===============================================================================================================
        public const int NO_ERRBASE_COMMON = 900;

        public const int NO_SYSTEM_EXCEPTION = NO_ERRBASE_COMMON + 1;
        const String MSG_NO_SYSTEM_EXCEPTION = "System Exception occurs.";

        public const int NO_OBJECT_NOT_ASSIGNED = NO_SYSTEM_EXCEPTION + 1;
        const String MSG_NO_OBJECT_NOT_ASSIGNED = "Object not assigned.";

        public const int NO_REGISTRY_ERROR = NO_OBJECT_NOT_ASSIGNED + 1;
        const String MSG_NO_REGISTRY_ERROR = "Search error in registry";

        public const int NO_INVALID_DATA_IN_MATERIAL_OBJECT = NO_REGISTRY_ERROR + 1;
        const String MSG_NO_INVALID_DATA_IN_MATERIAL_OBJECT = "Invalid data in material object";

        public const int NO_THREAD_START_WITH_ERROR = NO_INVALID_DATA_IN_MATERIAL_OBJECT + 1;
        const String MSG_NO_THREAD_START_WITH_ERROR = "Starting Thread error";

        public const int NO_SWITCH_WITH_BAD_CASE = NO_THREAD_START_WITH_ERROR + 1;
        const String MSG_NO_SWITCH_WITH_BAD_CASE = "Bad selection in switch case";

        public const int NO_INVALID_DATA_IN_CONFIGURATION = NO_SWITCH_WITH_BAD_CASE + 1;
        const String MSG_NO_INVALID_DATA_IN_CONFIGURATION = "Invalid data in configuration file";

        public const int NO_INVALID_CHARACTER_IN_FIELD = NO_INVALID_DATA_IN_CONFIGURATION + 1;
        const String MSG_NO_INVALID_CHARACTER_IN_FIELD = "Invalid character in text field. / \\ : * ? \" < > | ! <SPACE> characters can not be used.";

        public String UserMessage
        {
            get { return m_UserMessage; }
        }

        //---------------------------------------------------------------------------------------------------------------
        // Constructor
        public ESoftwareException(enumConnection ServerType, int NoError, String pUserMessage, String pLogMessage, bool bSilence)
        {
            m_UserMessage = pUserMessage;
            m_LogMessage = pLogMessage;
            m_ErrCode = NoError;
            m_bSilence = bSilence;
            m_ServerType = ServerType;
            switch (ServerType)
            {
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD1:
                    m_Title = "Brightfield1 error";
                    break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD2:
                    m_Title = "Brightfield2 error";
                    break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD3:
                    m_Title = "Brightfield3 error";
                    break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD4:
                    m_Title = "Brightfield4 error";
                    break;
                case enumConnection.CONNECT_GRAB_DARKFIELD:
                    m_Title = "Darkview error";
                    break;
                case enumConnection.CONNECT_PSD:
                    m_Title = "PSD error";
                    break;
                case enumConnection.CONNECT_GRAB_EDGE:
                    m_Title = "Edge error";
                    break;
                case enumConnection.CONNECT_PMEDGE:
                    m_Title = "PM Edge error";
                    break;
                case enumConnection.CONNECT_PMLS:
                    m_Title = "PM Light speed error";
                    break;
                default:
                    m_Title = "Socket TCP Exchange error";
                    break;
            }
        }
        // Pour de simple message d'affichage
        public ESoftwareException(int NoError, String pUserMessage, String pLogMessage, bool bSilence)
        {
            m_UserMessage = pUserMessage;
            m_LogMessage = pLogMessage;
            m_ErrCode = NoError;
            m_bSilence = bSilence;
            m_Title = "Error";
        }
        //---------------------------------------------------------------------------------------------------------------
        // Fonctions
        public static String GetMessage(int ErroCode)
        {
            String Msg;
            switch (ErroCode)
            {
                case NO_SYSTEM_EXCEPTION: Msg = MSG_NO_SYSTEM_EXCEPTION; break;
                case NO_OBJECT_NOT_ASSIGNED: Msg = MSG_NO_OBJECT_NOT_ASSIGNED; break;
                case NO_REGISTRY_ERROR: Msg = MSG_NO_REGISTRY_ERROR; break;
                case NO_INVALID_DATA_IN_MATERIAL_OBJECT: Msg = MSG_NO_INVALID_DATA_IN_MATERIAL_OBJECT; break;
                case NO_THREAD_START_WITH_ERROR: Msg = MSG_NO_THREAD_START_WITH_ERROR; break;
                case NO_SWITCH_WITH_BAD_CASE: Msg = MSG_NO_SWITCH_WITH_BAD_CASE; break;
                case NO_INVALID_DATA_IN_CONFIGURATION: Msg = MSG_NO_INVALID_DATA_IN_CONFIGURATION; break;
                case NO_INVALID_CHARACTER_IN_FIELD: Msg = MSG_NO_INVALID_CHARACTER_IN_FIELD; break;
                default: Msg = MSG_NO_ERROR_UNKNOWN; break;
            }
            return Msg;
        }
        //---------------------------------------------------------------------------------------------------------------
        public void DisplayError(IPMAltasightCallback pCallBack, int ErrorCode)
        {
            // GetMessage() will be used in PM_GUIDisplay and message will be written in logs with MsgError parameter
            if (!m_bSilence)
            {
                pCallBack.NotifyPM_GUIDisplay(m_ServerType, "ERROR#:" + ErrorCode.ToString(), m_UserMessage + " - " + m_LogMessage);
            }
        }

        public void DisplayError(int ErrorCode)
        {
            if (!m_bSilence)
            {
                String Msg = m_UserMessage;
                FrmError f = new FrmError(m_Title, Msg);
                f.BringToFront();
                f.ShowDialog();
            }
        }
        public int ErrorCode
        {
            get { return m_ErrCode; }
        }

    }
}
