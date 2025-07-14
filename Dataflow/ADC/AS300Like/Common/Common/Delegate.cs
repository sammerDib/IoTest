using System;
using System.Collections.Generic;
using System.Text;
using CIM300EXPERTLib;
using Common2017;

namespace Common
{
    public delegate void OnSocketLog(enumConnection ServerType, String Msg, String MsgError);

    public delegate void OnDisplayChange(List<Object> Params);

    public delegate void OnRecipeValueChange(String pFileName, String pSection, String pKey, String pNewKeyValue, String pLastKeyValue);

    public delegate void EventNoParam();

    public delegate void EventHandlerString(String pParam);

    public delegate void EventHandlerBoolBool(bool pParam1, bool pParam2);

    public delegate void EventHandlerIntInt(int pParam1, int pParam2);
    public delegate void EventHandlerIntString(int pPortID, string pMsg);

    public delegate void EventHandlerStationID(EFEMStationID pStationID);
    public delegate void EventHandlerStationIDCarrierID(EFEMStationID pStationID, String pCarrierID);

    public delegate void OnCarrierTypeChange(EFEMStationID pPortID, CCarrierType pCarrierType);

    public delegate void OnRequestToChangeWorkingSize(int pSize_Inches);

    public delegate void OnWorkingSizeChange(int pSize_Inches);

    public delegate void OnSet_IOChanged(EFEMStationID LoadPortID, IOTabIndex NumIO, int NewValue);

    public delegate void OnSet_InfoStatusText(string Msg);

    public delegate void OnSet_InfoStatusInt(int Value);

    public delegate void OnSet_InfoStatusTextLoadport(EFEMStationID pPortID, string Msg);

    public delegate void EventStatusChange(EFEMStationID pPortId, Object pEventData);

    public delegate void EventIOStateChanged(Object pIOName, Object pEventData);
    public delegate void EventFrameInputStateChanged(int pPortId, Object pIOName, Object pEventData);

    public delegate void OnCancelCarrier(int pPortID);

    public delegate void OnUpdateInit(String title, String name, String Subtitle, List<String> SubNameList);
    public delegate void OnUpdateItem(String name, String status);
    public delegate void OnUpdateSubItem(String name, String subName, String status, List<String> SubNameList2);
    public delegate void OnUpdateSubItem2(String name, String subName, String SubName2, String status2);
    public delegate void OnUpdateCJ(CJData cJData);
    public delegate void OnUpdateCJPJ(String cjName, String pjName, String status, enumPPStatus ThreadStatus);

    public delegate void SetDataVariable(string VarName, VALUELib.CxValueObject Value);
    public delegate void TriggerEvent(AllEventID eventID, String pMsgError, String pErrorID);

}