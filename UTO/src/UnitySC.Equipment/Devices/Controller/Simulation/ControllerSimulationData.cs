using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.Equipment.Devices.Controller.Simulation
{
    public class ControllerSimulationData : SimulationData
    {
        #region Constructor

        public ControllerSimulationData(Controller controller)
            : base(controller)
        {

        }

        #endregion
    }
}
