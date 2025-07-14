using System.Collections.Generic;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.DataFlow.ProcessModules.Drivers.WCF
{
    public static class WcfHelper
    {
        public static Material ToWcfMaterial(Wafer wafer)
        {
            return new Material()
            {
                AcquiredID = wafer.AcquiredId,
                CarrierID = wafer.CarrierId,
                ControlJobID = wafer.ControlJobId,
                LoadportID = wafer.SourcePort,
                LotID = wafer.LotId,
                MaterialType = (int)wafer.MaterialType,
                OrientationAngle = wafer.OrientationAngle.Degrees,
                ProcessJobID = wafer.ProcessJobId,
                SlotID = wafer.SourceSlot,
                SubstrateID = wafer.SubstrateId,
                GUIDWafer = wafer.GuidWafer,
                JobPosition = wafer.JobPosition,
                EquipmentID = wafer.EquipmentID,
                DeviceID = wafer.DeviceID,
                JobStartTime = wafer.JobStartTime,
                WaferDimension = ConvertSampleDimensionToLength(wafer.MaterialDimension)
            };
        }

        public static List<SampleDimension> ConvertLengthsToSampleDimensions(List<Length> lengthList)
        {
            var sampleDimensionList = new List<SampleDimension>();

            foreach (Length length in lengthList)
            {
                switch (length.Millimeters)
                {
                    case (double)SampleDimension.S100mm:
                        sampleDimensionList.Add(SampleDimension.S100mm);
                        break;
                    case (double)SampleDimension.S150mm:
                        sampleDimensionList.Add(SampleDimension.S150mm);
                        break;
                    case (double)SampleDimension.S200mm:
                        sampleDimensionList.Add(SampleDimension.S200mm);
                        break;
                    case (double)SampleDimension.S300mm:
                        sampleDimensionList.Add(SampleDimension.S300mm);
                        break;
                    case (double)SampleDimension.S450mm:
                        sampleDimensionList.Add(SampleDimension.S450mm);
                        break;
                }
            }

            return sampleDimensionList;
        }

        public static Length ConvertSampleDimensionToLength(SampleDimension sampleDimension)
        {
            return new Length((double)sampleDimension, LengthUnit.Millimeter);
        }
    }
}
