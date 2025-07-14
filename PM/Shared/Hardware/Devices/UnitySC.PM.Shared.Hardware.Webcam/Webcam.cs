using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Webcam
{
    public class Webcam : DeviceBase
    {
        private ILogger _logger;

        public WebcamConfig Config { get; private set; }

        public Webcam(WebcamConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
            _logger = logger;
            Name = config.Name;
            DeviceID = config.DeviceID;
            Config = config;
        }

        public override DeviceFamily Family => DeviceFamily.Webcam;
    }
}
