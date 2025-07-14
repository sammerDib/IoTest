using System.Collections.Generic;

using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Wheel;

namespace UnitySC.PM.EME.Hardware.FilterWheel
{
    public class FilterWheelConfig : WheelConfig
    {
        public string ControllerID { get; set; }
        public AxisConfig AxisConfig { get; set; }
        public List<FilterSlot> FilterSlots { get; set; }
    }
}
