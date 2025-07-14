using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Collection;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class ComputeNanoTopoInput : IFlowInput
    {
        public List<int> Periods  { get; set; }

        public double ScreenPixelSize { get; set; }
        
        [XmlIgnore]
        public ImageData UnwrappedX { get; set; }

        [XmlIgnore]
        public ImageData UnwrappedY { get; set; }

        [XmlIgnore]
        public ImageData Mask { get; set; }

        [XmlIgnore]
        public Side Side { get; set; }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (Periods.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a NanoTopo without period");
            }
            
            if (ScreenPixelSize <= 0)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a NanoTopo with a screen pixel less than 0");
            }
            
            if (UnwrappedX == null)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a NanoTopo without an unwrapped X image");
            }
            
            if (UnwrappedY == null)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a NanoTopo without an unwrapped Y image");
            }
            
            return result;
        }
    }
}
