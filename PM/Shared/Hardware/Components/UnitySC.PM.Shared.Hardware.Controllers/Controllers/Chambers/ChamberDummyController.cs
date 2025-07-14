using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers
{
    public class ChamberDummyController : ChamberController, IChamberController, IChamberFFUControl, IChamberBasics, ISlitDoor
    {
        private readonly IMessenger _messenger;
        private readonly ILogger _logger;
        private bool _ffuState;

        public ChamberDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init ChamberController as dummy");
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

        public void TurnOnFFU()
        {
            _ffuState = true;
        }

        public void TurnOffFFU()
        {
            _ffuState = false;
        }

        public bool GetFFUErrorState()
        {
            return false;
        }

        public bool CdaIsReady()
        {
            return true;
        }

        public bool IsInMaintenance()
        {
            return false;
        }

        public bool PrepareToTransferState()
        {
            return true;
        }

        public bool FFUState()
        {
            return _ffuState;
        }

        public override void TriggerUpdateEvent()
        {
             
        }

        public SlitDoorPosition SlitDoorState { get; set; }

        public void OpenSlitDoor()
        {
            SlitDoorState = SlitDoorPosition.OpenPosition;
            _messenger.Send(new SlitDoorPositionMessage { SlitDoorPosition = SlitDoorState });
        }

        public void CloseSlitDoor()
        {
            SlitDoorState = SlitDoorPosition.ClosePosition;
            _messenger.Send(new SlitDoorPositionMessage { SlitDoorPosition = SlitDoorState });
        }
    }
}
