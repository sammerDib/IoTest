using System;
using System.Collections.Generic;

using ADCCommon;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Logger;

namespace UnitySC.ADCAS300Like.Service
{
    public class CADCResultsList_FVN2 : CADCResultsList
    {
        public CADCResultsList_FVN2(ADCType pServerType, LocalLogger resulLogger)
            : base(pServerType, resulLogger)
        {
        }
    }
}
