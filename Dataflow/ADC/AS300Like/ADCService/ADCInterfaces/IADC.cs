using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.ADCAS300Like.Common;

namespace UnitySC.ADCAS300Like.Service.ADCInterfaces
{
    public interface IADC
    {
        void RegisterCallback(IADCCB pCB);

        void UnregisterCallback(IADCCB pCB);
        int ConnectADC(ADCType serverType, bool connect);
        int SetModeAutoConnect(ADCType serverType, bool autoConnect, int delayAutoConnect);

        int IsADCConnected(ADCType serverType, out bool isConnected);

        int IsADCConnectedToDatabase(ADCType serverType, out bool isConnectedToDatabase);

    }
}
