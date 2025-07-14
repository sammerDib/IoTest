using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.ADC;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.ADCAS300Like.Service.ADCInterfaces;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Data;

namespace UnitySC.ADCAS300Like.Service
{
    public class CADCCallBack : IADCCallBack
    {
        //Membres 
        #region Protected Member Variables
        private System.Object _callbackSynchronizationObject = new System.Object();
        private LinkedList<object> _callbackList = new LinkedList<object>();
        #endregion
        
        // Constructor
        public CADCCallBack()
        {
        }

        #region Public Properties
        //Properties Callback
        public LinkedList<object> CallbackList
        {
            get { return _callbackList; }
        }        
        public object AddCallback
        {
            set
            {
                lock (_callbackSynchronizationObject)
                {
                    _callbackList.AddLast(value);
                }
            }
        }
        public object RemoveCallback
        {
            set
            {
                lock (_callbackSynchronizationObject)
                {
                    _callbackList.Remove(value);
                }
            }
        }
        #endregion

        public void NotifyADC_GUIDisplay(ADCType serverType, string msg, string msgError)
        {
            lock (_callbackSynchronizationObject)
            {
                foreach (object oCallbackInterface in _callbackList)
                {
                    try
                    {
                        IADCCB pCallback = oCallbackInterface as IADCCB;
                        if (pCallback != null)
                        {
                            pCallback.ADC_GUIDisplay(serverType, msg, msgError);
                        }
                    }
                    catch 
                    {
                    }
                }
            }
        }

        public void NotifyADC_Connected(ADCType serverType, bool bConnected, bool pbDatabaseConnected)
        {
            lock (_callbackSynchronizationObject)
            {
                foreach (object oCallbackInterface in _callbackList)
                {
                    try
                    {
                        IADCCB pCallback = oCallbackInterface as IADCCB;
                        if (pCallback != null)
                        {
                            pCallback.ADC_Connected(serverType, bConnected, pbDatabaseConnected);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        public void NotifyADC_SetWaferReport(ADCType serverType, CWaferReport waferReport, LocalLogger resultsLogger)
        {
            lock (_callbackSynchronizationObject)
            {
                foreach (object oCallbackInterface in _callbackList)
                {
                    try
                    {
                        IADCCB pCallback = oCallbackInterface as IADCCB;
                        if (pCallback != null)
                        {
                            pCallback.ADC_SetWaferReport(serverType, waferReport, resultsLogger);
                        }
                    }
                    catch 
                    {
                    }
                }
            }
        }

        public void NotifyADC_SetEventResult(ADCType pServerType, Object material, CWaferReport waferReport, EvenID pEventID, int pErrorID, String pMsgError, LocalLogger resultsLogger)
        {
            lock (_callbackSynchronizationObject)
            {
                foreach (object oCallbackInterface in _callbackList)
                {
                    try
                    {
                        IADCCB pCallback = oCallbackInterface as IADCCB;
                        if (pCallback != null)
                        {
                            pCallback.ADC_SetEventResult(pServerType, material, waferReport, pEventID, pErrorID, pMsgError, resultsLogger);
                        }
                    }
                    catch 
                    {
                    }
                }
            }
        }

        public void NotifyADC_WaferCompleted(ADCType serverType, Material material, int pErrorStatus)
        {
            lock (_callbackSynchronizationObject)
            {
                foreach (object oCallbackInterface in _callbackList)
                {
                    try
                    {
                        IADCCB pCallback = oCallbackInterface as IADCCB;
                        if (pCallback != null)
                        {
                            pCallback.ADC_WaferCompleted(serverType, material, pErrorStatus);
                        }
                    }
                    catch (COMException comException)
                    {
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }
        }
        public void NotifyADC_JobStatus(String status)
        {
            lock (_callbackSynchronizationObject)
            {
                foreach (object oCallbackInterface in _callbackList)
                {
                    try
                    {
                        IADCCB pCallback = oCallbackInterface as IADCCB;
                        if (pCallback != null)
                        {
                            pCallback.ADC_JobStatus(status);
                        }
                    }
                    catch 
                    {
                    }
                }
            }
        }

        public void NotifyADC_AbortProcess(int errorStatus)
        {
            lock (_callbackSynchronizationObject)
            {
                foreach (object oCallbackInterface in _callbackList)
                {
                    try
                    {
                        IADCCB pCallback = oCallbackInterface as IADCCB;
                        if (pCallback != null)
                        {
                            pCallback.ADC_AbortProcess(errorStatus);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
