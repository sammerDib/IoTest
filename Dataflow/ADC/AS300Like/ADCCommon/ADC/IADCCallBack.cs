using System;

using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Logger;

namespace UnitySC.ADCAS300Like.Common.ADC
{
    public enum EvenID { eEventResultAvailable = 0, eEventResultError, eEventPostProcessStarted, eEventPostProcessComplete };
    
    public interface IADCCallBack
    {                        
        object AddCallback { set; }
        object RemoveCallback { set; }        

        void NotifyADC_GUIDisplay(ADCType serverType, string msg, string msgError);        
        void NotifyADC_Connected(ADCType serverType, bool bConnected, bool bDatabaseConnected);        
        void NotifyADC_SetWaferReport(ADCType serverType, CWaferReport waferReport, LocalLogger resultLogger);
        //void NotifyADC_SetEventResult(ADCType pServerType, Material material, EvenID pEventID, int pErrorID, String pMsgError);
        void NotifyADC_SetEventResult(ADCType pServerType, Object material, CWaferReport waferReport, EvenID pEventID, int pErrorID, String pMsgError, LocalLogger resultsLogger);
        void NotifyADC_JobStatus(String status);
        void NotifyADC_AbortProcess(int errorStatus);
    }

}
