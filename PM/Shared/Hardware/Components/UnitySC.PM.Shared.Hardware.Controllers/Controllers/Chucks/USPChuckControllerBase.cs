using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chuck
{
    public abstract class USPChuckControllerBase : ControllerBase, IChuckController
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        protected ControllerConfig ControllerConfig;

        internal string DeviceName = "Chuck";

        public event ChuckStateChangedDelegate ChuckStateChangedEvent;
        public bool IsChuckStateChangedEventSet { get => (ChuckStateChangedEvent != null); }

        protected USPChuckControllerBase(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            ControllerConfig = controllerConfig;
        }

        public abstract ChuckState GetState();
        protected void RaiseStateChangedEvent(ChuckState chuckState)
        {
            ChuckStateChangedEvent?.Invoke(chuckState);
        }
    }
}
