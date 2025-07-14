using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions; // for registry keys
using Microsoft.Win32;// for registry keys
using System.IO; // for DIRECTORIES
using System.Runtime.InteropServices;
using Common.EException;


namespace Common
{    
    public class UserTools
    {                
        // --------------------------------------------------------------------------------
        #region Types - EnumRights        
        public enum EnumRights
        {
            UNKNOWN = 0,
            TOOL_OPERATOR = 1,
            TOOL_TECHNICIAN=3,
            TOOL_ENGINEER = 7,
            TOOL_ADMINISTRATOR = 15
        }
        #endregion        

        public static int CheckInvalidCharacterInFieldForLotID(String pText)
        {
            pText = pText.Trim();

            int lRet = CheckBasicInvalidCharacterInField(pText);
            if (lRet == EEFEMException.NO_ERROR && !pText.Contains("_") && !pText.Contains("#"))
                return EEFEMException.NO_ERROR;
            else
                return EEFEMException.NO_INVALID_CHARCATER_LOTID;
        }

        public static int CheckBasicInvalidCharacterInField(String pText)
        {
            pText = pText.Trim();
            Byte[] test = ASCIIEncoding.ASCII.GetBytes(pText);
            String test2 = ASCIIEncoding.ASCII.GetString(test);
            if (test2.Contains("?"))
            {
                return EEFEMException.NO_INVALID_CHARCATER_LOTID;
            }

            if (!pText.Contains("/") && !pText.Contains("\\") &&
                !pText.Contains(":") && !pText.Contains("?") &&
                !pText.Contains("*") && !pText.Contains("\"") &&
                !pText.Contains("<") && !pText.Contains(">") &&
                !pText.Contains("|") && 
                !pText.Contains("!") && !pText.Contains(" "))
                return EEFEMException.NO_ERROR;
            else
                return EEFEMException.NO_INVALID_CHARCATER_LOTID;

        }
    }
}
