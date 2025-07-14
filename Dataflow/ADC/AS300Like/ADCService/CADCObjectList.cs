using System;
using System.Collections.Generic;
using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Service.ADCInterfaces;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.ADCAS300Like.Service
{

    // Liste des objects ADC connectable
    public class CADCObjectList
    {
        // Membres 
        private ADCsConfigs _adcConfig;
        private int _nbADC;
        private List<CADCObject> _adcList;
        private List<ADCType> _adcConnectionList;
        private OnSocketLog _onSocketLog;
        private CADCCallBack _adcCallbacks = new CADCCallBack();
        private Object _synchroADCObject = new Object();

        public OnSocketLog OnSocketLog { get => _onSocketLog; set => _onSocketLog = value; }
        public CADCCallBack AdcCallbacks { get => _adcCallbacks; set => _adcCallbacks = value; }
        public object SynchroADCObject { get => _synchroADCObject; set => _synchroADCObject = value; }


        // Constructeur
        public CADCObjectList()
        {
            Shared.Logger.ILogger appLogger = ClassLocator.Default.GetInstance<Shared.Logger.ILogger>();    
            _adcConfig = ClassLocator.Default.GetInstance<ADCsConfigs>();
            // Creation ADC list
            _nbADC = _adcConfig.ADCItemsConfig.Count;
            _adcList = new List<CADCObject> ();
            _adcConnectionList = new List<ADCType>();
            CADCObject NewADC = null;
            for (short i = 0; i < (int)ADCType.atCOUNT; i++)
            {
                String ServerHostName = "";
                int PortNum = 0;
                ADCType currentADCType = (ADCType)i;
                ADCItemConfig config = _adcConfig.ADCItemsConfig.Find(c => c.ADCType == currentADCType);
                if (!(config is null) && config.Enabled)
                {
                    ServerHostName = config.ServerName;
                    PortNum = config.PortNum;
                
                    // Creation ADC
                    OnSocketLog = new OnSocketLog(Do_OnSocketLog);
                    LocalLogger localLogger = new LocalLogger($"ADC\\{_adcConfig.ADCResultsLogPrefix}_{config.ADCType.ToString().Remove(0, 2)}");
                    NewADC = new CADCObject_Serialization_Generic(currentADCType, ServerHostName, (short)PortNum, OnSocketLog, AdcCallbacks, SynchroADCObject, localLogger);
                   
                    if (NewADC != null)
                    {
                        _adcList.Add(NewADC);
                        _adcConnectionList.Add(currentADCType);
                    }
                }                
            }                        
        }

        // Log du socket
        public void Do_OnSocketLog(ADCType serverType, String msg, String msgError)
        {
            AdcCallbacks.NotifyADC_GUIDisplay(serverType, msg, msgError); 
        }

        public void ConnectADC(ADCType serverType, bool bConnect)
        {
            int lIndex;
            if (bConnect)
            {
                lIndex = _adcConnectionList.IndexOf(serverType);
                if (lIndex >= 0)
                    // Demande de connexion
                    _adcList[lIndex].ConnectADC();
            }
            else
            {
                lIndex = _adcConnectionList.IndexOf(serverType);
                if (lIndex >= 0)
                    // Demande de deconnexion 
                    _adcList[lIndex].DisconnectADC();
            }
        }

        public void SetAutoConnect(ADCType serverType, bool bAutoConnect, int delayAutoConnect)
        {
            int lIndex = _adcConnectionList.IndexOf(serverType);
            if (lIndex >= 0)
            {
                _adcList[lIndex].AutoConnect = bAutoConnect;
                _adcList[lIndex].DelayAutoConnect = delayAutoConnect;
            }
        }


        //---------------------------------------------------------------------------------------------------------
        // ADC Callback 
        #region ADCCallback registering function
        public void RegisterCallback(IADCCB pCB)
        {
            if (pCB == null)
                throw new Exception("RegisterCallback() - InvalidArgumentException");

            if (AdcCallbacks == null)
                throw new Exception("RegisterCallback() - PointerException()");

            if (AdcCallbacks.CallbackList.Contains(pCB as object))
                throw new Exception("RegisterCallback() - AccessDeniedException()");

            AdcCallbacks.AddCallback = pCB as object;
        }
        public void UnregisterCallback(IADCCB pCB)
        {
            if (pCB == null)
                throw new Exception("RegisterCallback() - InvalidArgumentException");

            if (AdcCallbacks == null)
                throw new Exception("RegisterCallback() - PointerException()");

            if (!AdcCallbacks.CallbackList.Contains(pCB as object))
                throw new Exception("RegisterCallback() - AccessDeniedException()");

            AdcCallbacks.RemoveCallback = pCB as object;
        }
        #endregion

        internal bool GetADCConnectionStatus(ADCType serverType)
        {
            int lIndex = _adcConnectionList.IndexOf(serverType);
            if (lIndex >= 0)
            {
                return _adcList[lIndex].IsADCConnected;
            }
            else
                return false;
        }

        internal void IsADCConnectedToDatabase(ADCType serverType, out bool isConnectedToDatabase)
        {
            isConnectedToDatabase = false;
            int lIndex = _adcConnectionList.IndexOf(serverType);
            if (lIndex >= 0)
            {
                isConnectedToDatabase = _adcList[lIndex].IsADCConnectedToDatabase;
            }
        }

        internal void Shutdown()
        {
            for (int i = 0; i < _adcConnectionList.Count; i++)
            {
                _adcList[i].Shutdown();
            }
        }
    }
}
