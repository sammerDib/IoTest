using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Hardware
{
    public class ANAChamber : ChamberBase, IANAChamber
    {
        private ACSController _controller;
        private bool _isFFUOn;

        private ANAChamberConfig ChamberConfig
        {
            get
            {
                if (Configuration is ANAChamberConfig anaConfig)
                    return anaConfig;
                else
                    return null;
            }
        }
        public ANAChamber(IGlobalStatusServer globalStatusServer, ILogger logger, ChamberConfig config, IChamberController chamberController) :
            base(globalStatusServer, logger, config)
        {
            if (Configuration is ANAChamberConfig anaChamberConfig)
            {
                if (chamberController is ACSController acsChambercontroller)                
                    _controller = acsChambercontroller;                
            }else            
                throw new Exception("Invalid chmaber configuration type");           
        }

        public override void Init()
        {
            base.Init();
        }

        public void TurnOffFFU()
        {
            if (_controller is IChamberFFUControl ffuController)
            {
                ffuController.TurnOffFFU();
                Thread.Sleep(ChamberConfig.StabilisationFFUSwitchOff_ms);
                _isFFUOn = false;
            }
        }

        public void TurnOnFFU()
        {
            if (_controller is IChamberFFUControl ffuController)
            {
                ffuController.TurnOnFFU();
                Thread.Sleep(ChamberConfig.StabilisationFFUSwitchOn_ms);
            }
        }

        public  bool GetFFUErrorState()
        {
            if (_controller is IChamberFFUControl ffuController)                
                return ffuController.GetFFUErrorState();
            else 
                throw new NotImplementedException("Get FFU error state not implemented");
        }

        public bool FFUState()
        {
            bool ffuState = false;
            if (_controller is IChamberFFUControl ffuController)            
                ffuState = ffuController.FFUState();
            return ffuState;
        }

        public bool PrepareToTransferState()
        {
            return _controller.GetPrepareToTransfertValue();
        }

        #region Input

        public bool EMOState()
        {
            return _controller.GetEMOPushValue();
        }

        public bool RobotIsOutState()
        {
            return _controller.GetRobotIsOutValue();
        }

        public void InitProcess()
        {
            _controller.InitProcess();
        }

        public void SetChamberLightState(bool value)
        {
            _controller.ManagePrincipalChamberLight(value);
        }

        #endregion Input
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

            if (config is ANAChamberConfig psdChuckConfig)
            {
                if (controller is ACSController acsChamberController)
                    return new ANAChamber(globalStatusServer, logger, psdChuckConfig, acsChamberController);
                else
                    throw new Exception("On ANAChuck creation, Controller supplied is not a ACSController. Controller type = " + controller.GetType());
            }
            else
                throw new Exception("Unknown config class" + config.GetType());
        }
        // Heritage des devices avec PLC - not used
        public override void TriggerUpdateEvent()
        {
           
        }
        #endregion
    }
}
