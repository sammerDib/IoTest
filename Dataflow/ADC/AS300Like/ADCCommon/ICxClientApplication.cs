using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.ADCAS300Like.Common;

namespace ADCCommon
{
    public interface ICxClientApplication
    {

        string appName
        {
            get;
            set;
        }

        void GetEquipmentID(out int equipID);


        void UnregisterCommandHandler(string name);

        void DisableComm(int connectionID);


        void EnableComm(int connectionID);

        void DisableSpooling(int connectionID);

        void EnableSpooling(int connectionID);

        void SpoolingOverwrite(int connectionID, bool overwrite);

        void GoOnline(int connectionID);

        void GoOffline(int connectionID);

        void GoLocal(int connectionID);


        void GoRemote(int connectionID);



        void GetValue(int connectionID, ref int varid, ref string name, CxValueObject value, out VariableResults result);



        void GetWellKnownValue(int connectionID, string name, CxValueObject value, out VariableResults result);



        void GetValues(int connectionID, ref object varIDs, ref object names, out object values, out object results);


        void SetValue(int connectionID, VarType VarType, ref int varid, ref string name, CxValueObject value, out VariableResults result);


        void SetWellKnownValue(int connectionID, string name, CxValueObject value, out VariableResults result);

        void SetValues(int connectionID, VarType VarType, ref object varIDs, ref object names, object values, out object results);







        void UnregisterValueChangedHandler(int connectionID, int varid);




        void SendTerminalMsg(int connectionID, int termID, string msg);



        void RecognizeTerminalMsg(int connectionID);





        void PPLoadInquire(int connectionID, string name, int length);



        void PPSend(int connectionID, string filename, CxValueObject recipe);



        void PPRequest(int connectionID, string name);





        void SetAlarm(ref int alarmID, ref string name);



        void ClearAlarm(ref int alarmID, ref string name);



        void GetAlarmState(ref int alarmID, ref string name, out int state);



        void TriggerEvent(int connectionID, ref int eventID, ref string name);



        void TriggerWellKnownEvent(int connectionID, string name);



        void WaitOnEvent(int connectionID, ref int eventID, ref string name, int timeout, out int result);




        void Log(string logString);



        void StartLogging();



        void StopLogging();





        void GetLoggingProperties(out string strDirectory, out string strFilename, out int lNumberOfFiles, out int lMaximumFileSize, out int lTimePeriod, out int Logging);



        void SetLoggingProperties(string strDirectory, string strFilename, int lNumberOfFiles, int lMaximumFileSize, int lTimePeriod, int Logging);



        void ProcessingStateChange(int newState, ref string stateString, int eventID, ref string eventName);



        void GetCommState(int connectionID, out int state, out int substate);



        void AsyncTriggerEvent(int connectionID, ref int eventID, ref string name);



        void AsyncTriggerWellKnownEvent(int connectionID, string name);



        void SyncSendMessage(int connectionID, int msgID, CxValueObject msg, bool msgBody, int replyID, out CxValueObject reply, out int ErrorID, out int lExtra);



        void SetValueFromByteBuffer(int connectionID, VarType VarType, ref int varid, ref string name, object ValueBuffer, out VariableResults result);



        void GetValueToByteBuffer(int connectionID, ref int varid, ref string name, out object pValueBuffer, out VariableResults result);



        void SetValuesFromByteBuffer(int connectionID, VarType VarType, ref object varIDs, ref object names, object ValueBuffers, out object results);



        void GetValuesToByteBuffer(int connectionID, ref object varIDs, ref object names, out object pValueBuffers, out object results);




        void RestoreNVS();




        void SetAlarmAndText(int alarmID, string name, string text);







        void Log2(string msg, uint logLevel);





        void SetValuesTriggerEvent(int connectionID, VarType VarType, ref object varIDs, ref object names, object values, ref int eventID, ref string eventName, out object results);



        void AsyncSetValuesTriggerEvent(int connectionID, VarType VarType, ref object varIDs, ref object names, object values, ref int eventID, ref string eventName, out object results);


        bool IsLogging
        {


            get;
        }



        CxValueObject CreateValueObject();



        void SetAlarm2(int alarmID, string name, int alcd, string altx, int connectionID, ref object varIDs, ref object names, object values, out object results);



        void ClearAlarm2(int alarmID, string name, int alcd, string altx, int connectionID, ref object varIDs, ref object names, object values, out object results);
    }
}
