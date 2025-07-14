using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public class ENanotopographyException : Exception
    {
        // --- Recapitule les No d'erreurs
        // Global error                               0 - 99
        // Nanotopgraphy error                      100 - 199
        
        // Variables internes
        String m_AddingMessage;
        int m_ErrCode;
        String m_Title;
        bool m_bSilence;
        Connection m_ServerType;

        // Error Codes and Messages
        public const int NOERROR = 0;
        public const int NO_ERROR = 0;
        public const int NO_ERROR_UNKNOWN = 1;
        const String MSG_NO_ERROR_UNKNOWN = "Error unknown";
        //===============================================================================================================
        // Nanotopgraphy Exception 100 - 199
        //===============================================================================================================
        public const int NO_ERRBASE_MODULE = 100;
        public const int NO_RECIPE_FILE_NOT_FOUND = NO_ERRBASE_MODULE + 1;
        const String MSG_NO_RECIPE_FILE_NOT_FOUND = "Recipe file not found";

        public const int NO_OBJECT_NOT_ASSIGNED = NO_RECIPE_FILE_NOT_FOUND + 1;
        const String MSG_NO_OBJECT_NOT_ASSIGNED = "Object not assigned.";

        public const int NO_REGISTRY_ERROR = NO_OBJECT_NOT_ASSIGNED + 1;
        const String MSG_NO_REGISTRY_ERROR = "Search error in registry";

        public const int NO_RECIPE_LOADING_ERROR = NO_REGISTRY_ERROR + 1;
        const String MSG_NO_RECIPE_LOADING_ERROR = "Loading recipe failed";

        public const int NO_INVALID_DATA_IN_MATERIAL_OBJECT = NO_RECIPE_LOADING_ERROR + 1;
        const String MSG_NO_INVALID_DATA_IN_MATERIAL_OBJECT = "Invalid data in material object";

        public const int NO_ERROR_ALREADY_CONNECTED = NO_INVALID_DATA_IN_MATERIAL_OBJECT + 1;
        const String MSG_NO_ERROR_ALREADY_CONNECTED = "Server already connected";

        public const int NO_ERROR_ALREADY_DISCONNECTED = NO_ERROR_ALREADY_CONNECTED + 1;
        const String MSG_NO_ERROR_ALREADY_DISCONNECTED = "Server already disconnected";

        public const int NO_SOCKET_ERROR = NO_ERROR_ALREADY_DISCONNECTED + 1;
        const String MSG_NO_SOCKET_ERROR = "Socket error. Now, client is disconnected.";

        public const int NO_THREAD_START_WITH_ERROR = NO_SOCKET_ERROR + 1;
        const String MSG_NO_THREAD_START_WITH_ERROR = "Starting Thread error";

        public const int NO_TIMEOUT_SOCKET = NO_THREAD_START_WITH_ERROR + 1; // Error 20
        const String MSG_NO_TIMEOUT_SOCKET = "Sending data Timeout. Communication with  Module interrupted";

        public const int NO_NOT_CONNECTED = NO_TIMEOUT_SOCKET + 1;
        const String MSG_NO_NOT_CONNECTED = "Not connected with external supervisor application";

        public const int NO_GET_VERSION_MODULE_FAILED = NO_NOT_CONNECTED + 1;
        const String MSG_NO_GET_VERSION_MODULE_FAILED = "Exchange with module failed: Check communication failed";

        public const int NO_SEND_SOCKET_MODULE_FAILED = NO_GET_VERSION_MODULE_FAILED + 1;
        const String MSG_NO_SEND_SOCKET_MODULE_FAILED = "Send socket to external supervisor application failed.";

        public const int NO_GET_STATUS_MODULE_FAILED = NO_SEND_SOCKET_MODULE_FAILED + 1;
        const String MSG_NO_GET_STATUS_MODULE_FAILED = "Exchange with external supervisor application failed: Can not get status.";

        public const int NO_START_PROCESS_FAILED = NO_GET_STATUS_MODULE_FAILED + 1;
        const String MSG_NO_START_PROCESS_FAILED = "Start Process Failed.";

        public const int NO_PROGRAM_DOES_NOT_EXIST = NO_START_PROCESS_FAILED + 1; // Error 26
        const String MSG_NO_PROGRAM_DOES_NOT_EXIST = "Program does not exist.";


        //========================================================================================================================================================================================
        // Constructor
        public ENanotopographyException(int NoError, String Message, bool bSilence)
        {
            m_AddingMessage = Message;
            m_ErrCode = NoError;
            m_bSilence = bSilence;
            m_Title = "Nanatopography Module error";                 
        }        

        //---------------------------------------------------------------------------------------------------------------
        // Fonctions
        public static String GetMessage(int ErroCode)
        {
            String Msg;
            switch (ErroCode)
            {
                case NO_RECIPE_FILE_NOT_FOUND: Msg = MSG_NO_RECIPE_FILE_NOT_FOUND; break;
                case NO_OBJECT_NOT_ASSIGNED: Msg = MSG_NO_OBJECT_NOT_ASSIGNED; break;
                case NO_REGISTRY_ERROR: Msg = MSG_NO_REGISTRY_ERROR; break;
                case NO_RECIPE_LOADING_ERROR: Msg = MSG_NO_RECIPE_LOADING_ERROR; break;
                case NO_INVALID_DATA_IN_MATERIAL_OBJECT: Msg = MSG_NO_INVALID_DATA_IN_MATERIAL_OBJECT; break;
                case NO_ERROR_ALREADY_CONNECTED: Msg = MSG_NO_ERROR_ALREADY_CONNECTED; break;
                case NO_ERROR_ALREADY_DISCONNECTED: Msg = MSG_NO_ERROR_ALREADY_DISCONNECTED; break;
                case NO_SOCKET_ERROR: Msg = MSG_NO_SOCKET_ERROR; break;
                case NO_THREAD_START_WITH_ERROR: Msg = MSG_NO_THREAD_START_WITH_ERROR; break;
                case NO_TIMEOUT_SOCKET: Msg = MSG_NO_TIMEOUT_SOCKET; break;
                case NO_NOT_CONNECTED: Msg = MSG_NO_NOT_CONNECTED; break;
                case NO_GET_VERSION_MODULE_FAILED: Msg = MSG_NO_GET_VERSION_MODULE_FAILED; break;
                case NO_SEND_SOCKET_MODULE_FAILED: Msg = MSG_NO_SEND_SOCKET_MODULE_FAILED; break;
                case NO_GET_STATUS_MODULE_FAILED: Msg = MSG_NO_GET_STATUS_MODULE_FAILED; break;
                case NO_START_PROCESS_FAILED: Msg = MSG_NO_START_PROCESS_FAILED; break;
                case NO_PROGRAM_DOES_NOT_EXIST: Msg = MSG_NO_PROGRAM_DOES_NOT_EXIST; break;
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
                    Msg += m_AddingMessage;

                MessageBox.Show(Msg);
                // Affichage dans une fenetre
                //FrmError f = new FrmError(m_Title, Msg);
                //f.BringToFront();
                //f.ShowDialog();                
            }
        }

        public int ErrorCode
        {
            get { return m_ErrCode; }
        }

    }
}
