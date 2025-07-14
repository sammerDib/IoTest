using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware;
using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Hardware
{
    public class DMTChamber : ChamberBase, IPSDChamber
    {
        public const String OPENED = "OPENED";
        private PSDChamberController _controller;

        public DMTChamberConfig DMTChamberConfiguration
        {
            get
            {
                if (Configuration is DMTChamberConfig dmtConfig)
                    return dmtConfig;
                else
                    return null;
            }
        }

        public DMTChamber(IGlobalStatusServer globalStatusServer, ILogger logger, ChamberConfig config, IChamberController chamberController) :
            base(globalStatusServer, logger, config)
        {
            if (Configuration is DMTChamberConfig dmtChamberConfig)
            {
                if (chamberController is PSDChamberController psdChamberController)
                {
                    _controller = psdChamberController;
                    Messenger.Register<InterlockMessage>(this, (r, m) => { UpdateInterlocks(m); }); // InterlockMessage = message sent from beckoff about PSD chamber panels state
                    Messenger.Register<IsInMaintenanceMessage>(this, (r, m) => { UpdateIsInMaintenance(m); });
                }
            }
            else
                throw new Exception("Invalid chamber configuration type");
        }

        public void UpdateInterlocks(InterlockMessage interlockMessage)
        {
            // Catch panel state changed
            string msg = $"{interlockMessage.Description} is {interlockMessage.State}";
            bool active = interlockMessage.State.ToUpper().Contains(OPENED);
            SetDeviceErrorMessage(ErrorID.PanelInterlockedError, msg, active);
        }

        public void UpdateIsInMaintenance(IsInMaintenanceMessage maintenanceMessage)
        {
            // trigger Error panel opened
            string msg = $"PM Chamber is in maintenance";
            bool active = (maintenanceMessage.IsInMaintenance);
            SetDeviceErrorMessage(ErrorID.MaintenanceStateError, msg, active);
        }

        private void SetDeviceErrorMessage(ErrorID errorId, String message, bool active)
        {
            // trigger Error panel opened
            DeviceErrorMessage deviceErrorMessage = new DeviceErrorMessage();
            deviceErrorMessage.ErrorID = errorId;
            deviceErrorMessage.Message = message;
            deviceErrorMessage.Active = active;
            if (deviceErrorMessage.Active)
                Messenger.Send<DeviceErrorMessage>(deviceErrorMessage);
        }

        public void CheckInterlocksClosedState()
        {
            if (_controller.InterlockPanels.IsNullOrEmpty()) return; // No panels => no check

            // Check if panels are opened
            foreach (var panelId in _controller.InterlockPanels.Keys)
            {
                // trigger Error panel opened
                string msg = $"{_controller.InterlockPanels[panelId].Name} is {_controller.InterlockPanels[panelId].State}";
                bool active = (_controller.InterlockPanels[panelId].State.ToUpper().Contains(OPENED));
                SetDeviceErrorMessage(ErrorID.PanelInterlockedError, msg, active);
            }
        }

        public override void Init()
        {
            base.Init();
            TriggerUpdateEvent(); // To receive all previous controller notifications potentially missed until now
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        public void TurnOffFFU()
        {
        }

        public void TurnOnFFU()
        {
        }

        public bool FFUState()
        {
            return false;
        }

        public bool GetFFUErrorState()
        {
            //No sensor connected to see if the ffu is in error
            throw new NotImplementedException();
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

        public bool ValveIsOpened
        {
            get => _controller.ValveIsOpened;
        }

        public void OpenCdaPneumaticValve()
        {
            _controller.OpenCdaPneumaticValve();
        }

        public void CloseCdaPneumaticValve()
        {
            _controller.CloseCdaPneumaticValve();
        }

        #region Factory

        public static ChamberBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, ChamberConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller = null;

            if (!(config.ControllerID == "None"))
            {
                bool found = controllers.TryGetValue(config.ControllerID, out controller);

                if (!found || (controller == null))
                    throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID + ", ControllerId = " + config.ControllerID + "]");
            }

            if (config.IsSimulated)
            {
                if (controller is IChamberController chamberController)
                    return new DummyChamber(globalStatusServer, logger, config, chamberController);
            }

            if (config is DMTChamberConfig psdChuckConfig)
            {
                if (controller is PSDChamberController psdChuckController)
                    return new DMTChamber(globalStatusServer, logger, psdChuckConfig, psdChuckController);
                else
                    throw new Exception("On PSDChuck creation, Controller supplied is not a PSDChuckController. Controller type = " + controller.GetType());
            }
            else
                throw new Exception("Unknown config class" + config.GetType());
        }

        #endregion Factory
    }
}
