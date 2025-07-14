using System;
using System.Collections.Generic;

using ADCCommon;

using UnitySC.ADCAS300Like.Common.CVIDObj;

namespace UnitySC.ADCAS300Like.Common
{
    public delegate void OnSocketLog(ADCType serverType, String msg, String msgError);
    public delegate void OnDisplayChange(List<Object> parameters);
    public delegate void EventNoParam();
    public delegate void EventHandlerString(String pParam);
    public delegate void EventHandlerBoolBool(bool pParam1, bool pParam2);
    public delegate void EventHandlerIntInt(int pParam1, int pParam2);
    public delegate void EventHandlerIntString(int pPortID, string pMsg);

    public delegate void OnUpdateInit(String title, String name, String subtitle, List<String> subNameList);
    public delegate void OnUpdateItem(String name, String status);
    public delegate void OnUpdateSubItem(String name, String subName, String status, List<String> subNameList2);
    public delegate void OnUpdateSubItem2(String name, String subName, String subName2, String status2);

    public delegate void SetDataVariable(string varName, CVID value);
    public delegate void TriggerEvent(AllEventID eventID, String pMsgError, String pErrorID);

}
