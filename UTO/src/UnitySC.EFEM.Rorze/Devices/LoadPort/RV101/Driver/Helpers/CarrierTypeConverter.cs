using System;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.CommandConstants;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Helpers
{
    public static class CarrierTypeConverter
    {
        public static uint ToCarrierTypeDataIndex(CarrierType carrierType, uint? carrierTypeIndex)
        {
            switch (carrierType)
            {
                case CarrierType.FOUP:
                    if (carrierTypeIndex == null
                        || carrierTypeIndex == 0
                        || carrierTypeIndex > CarrierTypeConstants.NbFoupTypes)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(carrierTypeIndex),
                            carrierTypeIndex,
                            @"Not possible to get carrier data for the given parameter.");
                    }

                    // FOUP interval in RV101 HW data table is from 0 to 6
                    return (uint)(carrierTypeIndex - 1);

                case CarrierType.FOSB:
                    if (carrierTypeIndex == null
                        || carrierTypeIndex == 0
                        || carrierTypeIndex > CarrierTypeConstants.NbFosBTypes)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(carrierTypeIndex),
                            carrierTypeIndex,
                            @"Not possible to get carrier data for the given parameter.");
                    }

                    // FOSB interval in RV101 HW data table is from 7 to 11
                    return (uint)(carrierTypeIndex - 1 + CarrierTypeConstants.NbFoupTypes);

                case CarrierType.OpenCassette:
                    if (carrierTypeIndex == null
                        || carrierTypeIndex == 0
                        || carrierTypeIndex > CarrierTypeConstants.NbOpenCassetteTypes)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(carrierTypeIndex),
                            carrierTypeIndex,
                            @"Not possible to get carrier data for the given parameter.");
                    }

                    // Open cassettes interval in RV101 HW data table is from 12 to 14
                    return (uint)(carrierTypeIndex
                                  - 1
                                  + CarrierTypeConstants.NbFoupTypes
                                  + CarrierTypeConstants.NbFosBTypes);

                case CarrierType.Foup1Slot:
                    if (carrierTypeIndex == null
                        || carrierTypeIndex == 0
                        || carrierTypeIndex > CarrierTypeConstants.NbFoup1SlotTypes)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(carrierTypeIndex),
                            carrierTypeIndex,
                            @"Not possible to get carrier data for the given parameter.");
                    }

                    // FOUP with only 1 slot interval in RV101 HW data table is from 15 to 15
                    return (uint)(carrierTypeIndex
                                  - 1
                                  + CarrierTypeConstants.NbFoupTypes
                                  + CarrierTypeConstants.NbFosBTypes
                                  + CarrierTypeConstants.NbOpenCassetteTypes);

                case CarrierType.CarrierOnAdapter:
                    if (carrierTypeIndex == null
                        || carrierTypeIndex == 0
                        || carrierTypeIndex > CarrierTypeConstants.NbCarrierOnAdapterTypes)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(carrierTypeIndex),
                            carrierTypeIndex,
                            @"Not possible to get carrier data for the given parameter.");
                    }

                    // Carrier on adapter slot interval in RV101 HW data table is from 16 to 19
                    return (uint)(carrierTypeIndex
                                  - 1
                                  + CarrierTypeConstants.NbFoupTypes
                                  + CarrierTypeConstants.NbFosBTypes
                                  + CarrierTypeConstants.NbOpenCassetteTypes
                                  + CarrierTypeConstants.NbFoup1SlotTypes);

                // TODO: from there, conversion is not established. Complete or correct if needed.
                //       from tests on emulator,
                //          - from 20 to 31 carrier types are "CSTK" to "CSTV"
                //          - 32 carrier type is "OPN1"
                //          - from 33 to 63 carrier types have default values and no name.
                case CarrierType.Special:
                    if (carrierTypeIndex == null
                        || carrierTypeIndex == 0
                        || carrierTypeIndex
                        > CarrierTypeConstants.SpecialCarrierStopIndex
                        - CarrierTypeConstants.SpecialCarrierStartIndex
                        + 1)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(carrierTypeIndex),
                            carrierTypeIndex,
                            @"Not possible to get carrier data for the given parameter.");
                    }

                    return (uint)(carrierTypeIndex
                                  - 1
                                  + CarrierTypeConstants.NbFoupTypes
                                  + CarrierTypeConstants.NbFosBTypes
                                  + CarrierTypeConstants.NbOpenCassetteTypes
                                  + CarrierTypeConstants.NbFoup1SlotTypes
                                  + CarrierTypeConstants.NbCarrierOnAdapterTypes);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(carrierType),
                        carrierType,
                        @"Not possible to get carrier data for the given parameter.");
            }
        }
    }
}
