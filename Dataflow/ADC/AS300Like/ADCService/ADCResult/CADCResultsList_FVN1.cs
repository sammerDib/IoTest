using System;
using System.Collections.Generic;

using ADCCommon;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Logger;

namespace UnitySC.ADCAS300Like.Service
{
    public class CADCResultsList_FVN1 : CADCResultsList
    {
        public CADCResultsList_FVN1(ADCType pServerType, LocalLogger resultLogger)
            : base(pServerType, resultLogger)
        {
        }
    }
}
