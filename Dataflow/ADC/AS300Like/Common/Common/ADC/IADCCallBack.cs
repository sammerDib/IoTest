using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Common;

namespace Common.ADC
{
    public enum EvenID { eEventDefectResultAvailable = 0, eEventMeasurementResultAvailable, eEventResultError, eEventAPCResultAvailable };
    
    public interface IADCCallBack
    {                        
        object AddCallback { set; }
        object RemoveCallback { set; }        

        void NotifyADC_GUIDisplay(enumConnection ServerType, string Msg, string MsgError);        
        void NotifyADC_Connected(enumConnection ServerType, bool bConnected, bool bDatabaseConnected);        
        void NotifyADC_SetVariable(enumConnection ServerType, int VarFormatUsed, String VarName, VALUELib.CxValueObject Value);
        void NotifyADC_SetEventResult(enumConnection pServerType, EvenID pEventID, int pErrorID, String pMsgError);
        void NotifyADC_SetEventResult(enumConnection pServerType, EvenID pEventID);
        void NotifyADC_JobStatus(String Status);
        void NotifyADC_AbortProcess(int ErrorStatus);
    }

}
