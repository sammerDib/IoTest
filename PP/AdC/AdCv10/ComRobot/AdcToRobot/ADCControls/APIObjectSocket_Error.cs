using System;
using System.Windows.Forms;

namespace ADCControls
{
    internal class ETCPException : Exception
    {
        // --- Recapitule les No d'erreurs
        // COHTService.ServerIO                     20  - 99
        // CADCService.ADC                          100 - 199
        // ETCPException                            200 - 299
        // EPMAException                            300 - 399

        // Variables internes
        private String m_AddingMessage;
        private int m_ErrCode;
        private bool m_bSilence;

        // Error Codes and Messages
        public const int NOERROR = 0;
        public const int NO_ERROR = 0;
        public const int NO_ERROR_UNKNOWN = 1;
        private const String MSG_NO_ERROR_UNKNOWN = "Error unknown";
        //===============================================================================================================
        // TCP 200 - 299
        //===============================================================================================================
        public const int NO_ERRBASE_TCP = 200;
        public const int NO_CONFIG_FILE_NOT_FOUND = NO_ERRBASE_TCP + 1;
        private const String MSG_NO_CONFIG_FILE_NOT_FOUND = "Configuration file not found";

        public const int NO_ERROR_ALREADY_CONNECTED = NO_CONFIG_FILE_NOT_FOUND + 1;
        private const String MSG_NO_ERROR_ALREADY_CONNECTED = "Client already connected";

        public const int NO_ERROR_ALREADY_DISCONNECTED = NO_ERROR_ALREADY_CONNECTED + 1;
        private const String MSG_NO_ERROR_ALREADY_DISCONNECTED = "Client already disconnected";

        public const int NO_SOCKET_ERROR = NO_ERROR_ALREADY_DISCONNECTED + 1;
        private const String MSG_NO_SOCKET_ERROR = "Socket error. Now, client is disconnected.";

        public const int NO_THREAD_START_WITH_ERROR = NO_SOCKET_ERROR + 1;
        private const String MSG_NO_THREAD_START_WITH_ERROR = "Starting Thread error";

        public const int NO_TCP_SERVER_NOT_EXIST = NO_THREAD_START_WITH_ERROR + 1;
        private const String MSG_NO_TCP_SERVER_NOT_EXIST = "This server does not exist.";

        public const int NO_TIMEOUT_SOCKET = NO_TCP_SERVER_NOT_EXIST + 1;
        private const String MSG_NO_TIMEOUT_SOCKET = "Sending data Timeout. Communication with ADC interrupted";


        //---------------------------------------------------------------------------------------------------------------
        // Constructor
        public ETCPException(int NoError, String Message, bool bSilence)
        {
            m_AddingMessage = Message;
            m_ErrCode = NoError;
            m_bSilence = bSilence;
        }

        //---------------------------------------------------------------------------------------------------------------
        // Fonctions
        public String GetMessage(int ErroCode)
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
                case NO_TIMEOUT_SOCKET: Msg = MSG_NO_TIMEOUT_SOCKET; break;

                default: Msg = MSG_NO_ERROR_UNKNOWN; break;
            }
            return Msg;
        }
        //---------------------------------------------------------------------------------------------------------------
        public void DisplayError(int ErrorCode)
        {
            if (!m_bSilence)
            {
                String Msg = GetMessage(ErrorCode);
                if (m_AddingMessage.Length > 0)
                    Msg += "#" + m_AddingMessage;
                if (!m_bSilence)
                {
                    MessageBox.Show(Msg + " : " + ErrorCode.ToString());
                }
            }
        }

        public int ErrorCode
        {
            get { return m_ErrCode; }
        }

    }
}
