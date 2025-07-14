using System;

using UnitySC.ADCAS300Like.Service.ADCInterfaces;

using UnitySC.ADCAS300Like.Common;

namespace UnitySC.ADCAS300Like.Service
{
    public class CADCService : IADC
    {
        // Membres        
        private CADCObjectList _adcList = new CADCObjectList();

        public int ConnectADC(ADCType serverType, bool bConnect)
        {
          _adcList.ConnectADC(serverType, bConnect);
           return (int)VSConstants.VS_OK; 
        }

        public int IsADCConnected(ADCType serverType, out bool isConnected)
        {
            isConnected = _adcList.GetADCConnectionStatus(serverType);
            return (int)VSConstants.VS_OK;
        }

        public int SetModeAutoConnect(ADCType serverType, bool bAutoConnect, int delayAutoConnect)
        {
            _adcList.SetAutoConnect(serverType, bAutoConnect, delayAutoConnect);
            return (int)VSConstants.VS_OK;
        }

        #region REGISTER CALLBACK
        public void RegisterCallback(IADCCB pCB)
        {
            if (pCB == null)
                throw new Exception("RegisterCallback() - InvalidArgumentException");

            if (_adcList == null)
                throw new Exception("RegisterCallback() - PointerException()");

            _adcList.RegisterCallback(pCB);
        }
        public void UnregisterCallback(IADCCB pCB)
        {
            if (pCB == null)
                throw new Exception("RegisterCallback() - InvalidArgumentException");

            if (_adcList == null)
                throw new Exception("RegisterCallback() - PointerException()");

            _adcList.UnregisterCallback(pCB);
        }
        #endregion

        #region IADC Members
        public int IsADCConnectedToDatabase(ADCType serverType, out bool isConnectedToDatabase)
        {
           _adcList.IsADCConnectedToDatabase(serverType, out isConnectedToDatabase);
           return (int)VSConstants.VS_OK;
        }
        #endregion


        public int Shutdown()
        {
            _adcList.Shutdown();
            return 0;
        }
    }
}
