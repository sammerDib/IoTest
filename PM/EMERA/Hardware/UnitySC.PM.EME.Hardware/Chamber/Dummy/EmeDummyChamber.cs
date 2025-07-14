using System;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Hardware.Chamber.Dummy
{
    public class EmeDummyChamber : ChamberBase, IEMEChamber
    {
        private ILogger _logger;
        private ChamberDummyController _controller;
        public SlitDoorPosition SlitDoorState { get; }

        public EmeDummyChamber(IGlobalStatusServer globalStatusServer, ILogger logger, ChamberConfig config, ChamberDummyController controller) :
            base(globalStatusServer, logger, config)
        {
            _controller = controller;
            _logger = logger;
        }

        public override void TriggerUpdateEvent()
        {
        }
        
        public void OpenSlitDoor()
        {
            _controller.OpenSlitDoor();
        }

        public void CloseSlitDoor()
        {
            _controller.CloseSlitDoor();
        }
    }
}
