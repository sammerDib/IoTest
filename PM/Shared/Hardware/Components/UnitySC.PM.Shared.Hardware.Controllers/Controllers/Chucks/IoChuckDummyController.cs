using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chuck
{
    public class IoChuckDummyController : IoChuckController
    {
        private readonly ILogger _logger;

        private ChuckState _chuckState;
        public MaterialPresence WaferPresence { get; set; } = MaterialPresence.Unknown;

        private enum EChuckCmds
        { RaisePropertiesChanged }

        private enum EFeedbackMsgPSDChuck
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            WaferPresence_75mm_Msg = 10,
            WaferPresence_100mm_Msg,
            WaferPresence_150mm_Msg,
            WaferPresence_200mm_Msg
        }

        public IoChuckDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
            _chuckState = new ChuckState();
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init IoChuckController as dummy");
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
