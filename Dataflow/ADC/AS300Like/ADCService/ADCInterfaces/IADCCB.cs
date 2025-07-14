using System;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.ADC;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Logger;

namespace UnitySC.ADCAS300Like.Service.ADCInterfaces
{
    public interface IADCCB
    {
        void ADC_GUIDisplay(ADCType serverType, string msg, string msgError);

        void ADC_Connected(ADCType serverType, bool bConnected, bool bDatabaseConnected);

        void ADC_SetWaferReport(ADCType serverType, CWaferReport waferReport, LocalLogger resultLogger);

        void ADC_JobStatus( string status);

        void ADC_AbortProcess(int errorStatus);

        void ADC_WaferCompleted(ADCType serverType, Object material, int errorStatus);

        void ADC_SetEventResult(ADCType serverType, Object material, CWaferReport waferReport, EvenID eventID, int errorID,  string msgError, LocalLogger resultsLogger);
    }
}
