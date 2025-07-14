using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.HLS.Service.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.HLS.Hardware.Manager
{
    [Serializable]
    public class HlsHardwareConfiguration : HardwareConfiguration
    {
		//TODO

        public void InitializeObjectsFromIDs()
        {
          // TODO
        }

        public override void SetAllHardwareInSimulation()
        {
            var subDeviceConfigs = SubObjectFinder.GetAllSubObjectOfTypeT<IDeviceConfiguration>(this, 2);
            foreach (var deviceConfig in subDeviceConfigs)
            {
                deviceConfig.Value.IsSimulated = true;
            }
        }
    }
}
