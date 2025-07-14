using System;
using System.Windows;

namespace UnitySC.ADCAS300Like.Common.EException
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
        private String _userMessage;
        private String _logMessage;
        private int _errCode;
        private String _title;
        private bool _bSilence;
        private ADCType _serverType;

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
        private const String MSG_NO_TIMEOUT_SOCKET = "Sending data Timeout. It was not connected or connexion interrupted.";

        public const int NO_TOO_EARLY_GETSTATUS = NO_TIMEOUT_SOCKET + 1;
        private const String MSG_NO_TOO_EARLY_GETSTATUS = "GetStatus command sent too early. Wait delay to request it again";


        //---------------------------------------------------------------------------------------------------------------
        // Constructor
        public ETCPException(ADCType serverType, int noError, String pUserMessage, String pLogMessage, bool bSilence)
        {
            _userMessage = pUserMessage;
            _logMessage = pLogMessage;
            _errCode = noError;
            _bSilence = bSilence;
            _serverType = serverType;
            switch (_serverType)
            {
                case ADCType.atPSD_FRONTSIDE:
                    _title = "ADC Frontside error";
                    break;
                case ADCType.atPSD_BACKSIDE:
                    _title = "ADC Backside error";
                    break;
                case ADCType.atEDGE:
                    _title = "ADC Edge error";
                    break;
                case ADCType.atLIGHTSPEED:
                    _title = "ADC Light Speed error";
                    break;
                default: 
                    _title = "ADC TCP Exchange error"; 
                    break;
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        // Fonctions
        public static String GetMessage(int erroCode)
        {
            String Msg;
            switch (erroCode)
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
        

        public int ErrorCode
        {
            get { return _errCode; }
        }

        public bool bSilence { get => _bSilence; }
    }
}
