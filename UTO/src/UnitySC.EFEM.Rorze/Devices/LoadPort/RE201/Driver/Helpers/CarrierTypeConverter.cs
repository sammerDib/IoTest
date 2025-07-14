using System;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.CommandConstants;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Helpers
{
    public static class CarrierTypeConverter
    {
        public static uint ToCarrierTypeDataIndex(CarrierType carrierType, uint? carrierTypeIndex)
        {
            switch (carrierType)
            {
                case CarrierType.Cassette:
                    if (carrierTypeIndex == null
                        || carrierTypeIndex > CarrierTypeConstants.NbCassetteTypes)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(carrierTypeIndex),
                            carrierTypeIndex,
                            @"Not possible to get carrier data for the given parameter.");
                    }

                    // FOUP interval in RE201 HW data table is from 0 to 9
                    return (uint)(carrierTypeIndex);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(carrierType),
                        carrierType,
                        @"Not possible to get carrier data for the given parameter.");
            }
        }
    }
}
