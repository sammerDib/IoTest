using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class OpcDummyController : ControllerBase
    {
        public const bool DebugOpc = false;
        private ILogger _logger;

        public Dictionary<uint, DataAttribute> HandleDataAttributes { get; set; } = new Dictionary<uint, DataAttribute>();

        public ClientSessionChannel Channel { get; set; }
        public List<MonitoredItemCreateRequest> MonitoredItems = new List<MonitoredItemCreateRequest>();

        public OpcDevice OpcDevice { get; set; } = new OpcDevice();

        public OpcDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServeur, ILogger logger)
            : base(opcControllerConfig, globalStatusServeur, logger)
        {
            Init(opcControllerConfig, logger);
        }

        private void Init(OpcControllerConfig opcControllerConfig, ILogger logger, DeliverMessagesDelegate deliverMessagesDelegate = null)
        {
            _logger = logger;
            logger.Information("Init OpcController as dummy");
            string hostname;
            if (opcControllerConfig.IsSimulated)
            {
                _logger.Warning("Simulated mode activated");
                hostname = "localhost";
            }
            else
            {
                hostname = opcControllerConfig.OpcCom.Hostname;
            }
        }

        public override void Init(List<Message> initErrors)
        {
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void Connect()
        {
        }

        public override void Connect(string deviceId)
        {
        }

        public override void Disconnect()
        {
        }

        public override void Disconnect(string deviceID)
        {
        }

    }
}
