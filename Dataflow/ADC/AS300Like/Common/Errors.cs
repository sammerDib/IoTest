using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OHT_Service
{
    class ServerIOException : Exception
    {    
        // Variables internes
        String m_AddingMessage;
        int m_ErrCode;
                
        // Error Codes and Messages
        public const int NOERROR = 0;
        public const int NO_ERROR = 0;
        public const int NO_ERROR_UNKNOWN = 1;
        const String MSG_ERROR_UNKNOWN = "Error unknown";
        //===============================================================================================================
        // SERVER IO
        //===============================================================================================================
        public const int NO_ERRBASE_SERVERIO = 20;
        public const int NO_CONFIG_FILE_NOT_FOUND = NO_ERRBASE_SERVERIO + 1;
        const String MSG_CONFIG_FILE_NOT_FOUND = "Configuration file not found";

        public const int NO_READ_IO_FAILED = NO_ERRBASE_SERVERIO + 2;
        const String MSG_READ_IO_FAILED = "Read I/O failed";

        public const int NO_WRITE_IO_FAILED = NO_ERRBASE_SERVERIO + 3;
        const String MSG_WRITE_IO_FAILED = "Write I/O failed";

        public const int NO_CARD_DETECTION_FAILED = NO_ERRBASE_SERVERIO + 4;
        const String MSG_CARD_DETECTION_FAILED = "Card detection failed";
       

        //---------------------------------------------------------------------------------------------------------------
        // Constructor
        public ServerIOException(int NoError, String Message)
        {
            m_AddingMessage = Message;
            m_ErrCode = NoError;            
        }

        //---------------------------------------------------------------------------------------------------------------
        // Fonctions
        public String GetMessage(int ErroCode)
        {
            String Msg;
            switch (ErroCode)
            {
                case NO_CONFIG_FILE_NOT_FOUND:  Msg = MSG_CONFIG_FILE_NOT_FOUND;     break;                       
                case NO_CARD_DETECTION_FAILED:  Msg = MSG_CARD_DETECTION_FAILED;     break;
                case NO_WRITE_IO_FAILED:        Msg = MSG_WRITE_IO_FAILED; break;
                case NO_READ_IO_FAILED:         Msg = MSG_READ_IO_FAILED; break;
                default: Msg = MSG_ERROR_UNKNOWN; break;                                
            }                                                                           
            return Msg;
        }
        //---------------------------------------------------------------------------------------------------------------
        public void DisplayError(int ErrorCode)
        {
            String Msg = GetMessage(ErrorCode);
            if (m_AddingMessage.Length > 0)
                Msg += m_AddingMessage;
            MessageBox.Show(Msg);
        }

        public int ErrorCode
        {
            get { return m_ErrCode; }
        }
        
    }
}
