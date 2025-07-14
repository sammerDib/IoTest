using System;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class CorrectorInput : IFlowInput
    {
        [XmlIgnore]
        public USPImageMil AcquiredImage { get; set; }

        public WaferDimensionalCharacteristic ProductCharacteristics { get; set; }

        public Side Side { get; set; }

        [XmlIgnore]
        public DMTTransform Transform { get; set; }

        public CorrectorInput(WaferDimensionalCharacteristic productCharacteristics, DMTTransform transform, Side side)
        {
            ProductCharacteristics = productCharacteristics;
            Transform = transform;
            Side = side;
        }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (AcquiredImage is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot compute correctors without an image");
            }

            if (ProductCharacteristics is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot compute correctors without product dimensional characteristics");
            }

            if (Transform is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot compute correctors without applying perspective calibration");
            }
            
            return result;
        }

        public CorrectorInput()
        {
        }
    }
}
