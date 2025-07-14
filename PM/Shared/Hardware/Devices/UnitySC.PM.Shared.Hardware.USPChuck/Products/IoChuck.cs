using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck.Configuration;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.USPChuck
{
    public class IoChuck : USPChuckBase
    {
        private IoChuckController _controller;
        private PSDChuckConfig _pSDChuckConfig;
        private Dictionary<Length, MaterialPresence> _waferPresenceSensors = new Dictionary<Length, MaterialPresence>();

        public IoChuck(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config, USPChuckControllerBase controller)
            : base(globalStatusServer, logger, config, controller)
        {
            _controller = (IoChuckController)controller;
            _pSDChuckConfig = (PSDChuckConfig)config;

            foreach (var waferPresenceSensor in _pSDChuckConfig.WaferPresenceSensors.Where(x => x.IsEnabled))
            {
                _waferPresenceSensors.Add(waferPresenceSensor.Diameter, MaterialPresence.Unknown);
            }

            if (_waferPresenceSensors.Count() == 0)
            {
                logger.Warning($"{_pSDChuckConfig.DeviceID} - no wafer precence sensor");
            }
            foreach (var slotConfig in _pSDChuckConfig.SubstrateSlotConfigs)
            {
                if(slotConfig.IsPresenceSensorEnabled)
                {
                    _waferPresenceSensors.Add(slotConfig.Name, MaterialPresence.Unknown);
                }
            }
        }

        public override void Init()
        {
            base.Init();
            _controller.InitWaferPresenceSensors(_waferPresenceSensors);
        }

        public override void RefreshAllValues()
        {
            _controller.RefreshAllValues();
        }

        public override Dictionary<Length, MaterialPresence> CheckWafersPresence()
        {
            return _controller.CheckWafersPresence();
        }
    }
}
