using System;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Chamber
{
    public class DummyChamber : ChamberBase, IChamberInterlocks, IChamberFFUControl
    {
        private IChamberController _controller;
        private bool _ffuState;

        public DummyChamber(IGlobalStatusServer globalStatusServer, ILogger logger, ChamberConfig config, IChamberController controller) :
            base(globalStatusServer, logger, config)
        {
            _controller = controller;
        }

        public override void Init()
        {
            base.Init();
            Logger.Information("Starting dummy chamber");
        }


        public  void TurnOnFFU()
        {
            _ffuState = true;
        }

        public void TurnOffFFU()
        {
            _ffuState = false;
        }

        public bool GetFFUErrorState()
        {
            return true; // FFU Ok
        }

        public void CheckInterlocksClosedState()
        {                   
            // TEST trigger Error panel opened 
            //DeviceErrorMessage deviceErrorMessage = new DeviceErrorMessage();
            //deviceErrorMessage.ErrorID = ErrorID.PanelInterlockedError;
            //deviceErrorMessage.Message = $"Left panel is Opened";
            //Messenger.Send<DeviceErrorMessage>(deviceErrorMessage);
              
        }

        public bool FFUState()
        {
            return _ffuState;
        }

        public override void TriggerUpdateEvent()
        {
             
        }
    }
}
