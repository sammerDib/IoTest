using System;

namespace UnitySC.Shared.TC.Shared.Data
{
     [Obsolete("to be deleleted NOT USED")]
    public class PMMaterial
    {
        public string Recipe { get; set; }
        public string CarrierID { get; set; }
        public string SlotID { get; set; }
        public string MaterialType { get; set; }
        public string ProcessJob { get; set; }
        public string ControlJob { get; set; }
        public string LotId { get; set; }
        public double OrientationAngle { get; set; }
        public string WaferID { get; set; }
    }
}
