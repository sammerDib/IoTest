using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Hardware.Chamber
{
    public class EMEChamber : ChamberBase, IEMEChamber
    {
        private EMEChamberController _controller;
        private EMEChamberConfig _chamberConfig;
        private bool _isSlitDoorOpened;

        private Dictionary<uint, InterlockMessage> _interlockPanels = new Dictionary<uint, InterlockMessage>();

        public EMEChamber(IGlobalStatusServer globalStatusServer, ILogger logger, ChamberConfig config, ChamberController chamberController) :
            base(globalStatusServer, logger, config)
        {
            _controller = (EMEChamberController)chamberController;
            _chamberConfig = (EMEChamberConfig)config;

            if (_chamberConfig.Interlocks != null && !_chamberConfig.Interlocks.InterlockPanels.IsNullOrEmpty())
            {
                foreach (var doorInterlock in _chamberConfig.Interlocks.InterlockPanels)
                {
                    _interlockPanels.Add(doorInterlock.InterlockID, new InterlockMessage());
                }
            }
        }

        public override void Init()
        {
            base.Init();
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        public bool FFUState()
        {
            return false;
        }


        public bool EMOState()
        {
            return false;
        }

        public bool RobotIsOutState()
        {
            return false;
        }

        public SlitDoorPosition SlitDoorState
        {
            get => _controller.SlitDoorState;
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
