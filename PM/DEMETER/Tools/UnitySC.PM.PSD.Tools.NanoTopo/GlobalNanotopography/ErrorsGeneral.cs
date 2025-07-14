using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public class EGeneralException: Exception
    {
        // --- Recapitule les No d'erreurs
        // Global error                               0 - 99
        // Darkfield Main                           100 - 199
        // ETCPException                            200 - 299        
        // EACQException                            300 - 399
        // EAS300EchangeException                   400 - 499
        // EPneumaticException                      500 - 599
        // EMachineException                        600 - 699
        // EEngineException                         700 - 799

        // Variables internes
        String m_AddingMessage;
        int m_ErrCode;
        String m_Title;
        bool m_bSilence;

        // Error Codes and Messages
        public const int NOERROR = 0;
        public const int NO_ERROR = 0;
        public const int NO_ERROR_UNKNOWN = 1;
        const String MSG_NO_ERROR_UNKNOWN = "Error unknown";

        public const int NO_ERROR_OBJECT_NOT_ASSIGNED = 2;
        const String MSG_NO_ERROR_OBJECT_NOT_ASSIGNED = "Object not assigned";

        public const int NO_SWITCH_CASE_INDETERMINATE = NO_ERROR_OBJECT_NOT_ASSIGNED + 1;
        const String MSG_NO_SWITCH_CASE_INDETERMINATE = "Invalid Value in switch case";

        public const int NO_ERROR_OBJECT_ALREADY_EXIST = NO_SWITCH_CASE_INDETERMINATE + 1;
        const String MSG_NO_ERROR_OBJECT_ALREADY_EXIST = "Object already exist";

        public const int NO_FILE_NOT_FOUND = NO_ERROR_OBJECT_ALREADY_EXIST + 1;
        const String MSG_NO_FILE_NOT_FOUND = "File not found or does not exist.";

        //===============================================================================================================
        // Darkfield main 100 - 199
        //===============================================================================================================
        public const int NO_ERRBASE_DF_MAIN = 100;



        //========================================================================================================================================================================================
        // Constructor
        public EGeneralException(String StrTitle, int NoError, String Message, bool bSilence)
        {
            m_AddingMessage = Message;
            m_ErrCode = NoError;
            m_bSilence = bSilence;
            m_Title = StrTitle;                 
        }

        //---------------------------------------------------------------------------------------------------------------
        // Fonctions
        public String GetMessage(int ErroCode)
        {
            String Msg;
            switch (ErroCode)
            {
                case NO_ERROR_UNKNOWN: Msg = MSG_NO_ERROR_UNKNOWN; break;
                case NO_ERROR_OBJECT_NOT_ASSIGNED: Msg = MSG_NO_ERROR_OBJECT_NOT_ASSIGNED; break;
                case NO_ERROR_OBJECT_ALREADY_EXIST: Msg = MSG_NO_ERROR_OBJECT_ALREADY_EXIST; break;
                case NO_FILE_NOT_FOUND: Msg = MSG_NO_FILE_NOT_FOUND; break;
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
                    MessageBox.Show(Msg);
                    //FrmError f = new FrmError(m_Title, Msg);
                    //f.BringToFront();
                    //f.ShowDialog();
                }                
            }            
        }

        public int ErrorCode
        {
            get { return m_ErrCode; }
        }

    }
}

