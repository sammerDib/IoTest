using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Permissions; // for registry keys
using Microsoft.Win32;// for registry keys
using System.IO; // for DIRECTORIES
using System.Runtime.InteropServices;
using UnitySC.ADCAS300Like.Common.EException;


namespace UnitySC.ADCAS300Like.Common
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

    }
}
