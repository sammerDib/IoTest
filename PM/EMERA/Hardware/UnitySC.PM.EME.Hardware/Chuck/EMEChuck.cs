using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.USPChuck
{
    public class EMEChuck : USPChuckBase, IChuckMaterialPresence, IChuckClamp, IChuckLoadingPosition
    {
        private EMEChuckController _controller;

        public EMEChuckConfig ChuckConfiguration
        {
            get
            {
                if (Configuration is EMEChuckConfig emeConfig)
                    return emeConfig;
                else
                    return null;
            }
        }

        public override bool IsMaterialPresenceRefreshed { get => _controller.IsMaterialPresenceRefreshed; }

        public EMEChuck(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config, IChuckController controller)
            : base(globalStatusServer, logger, config, controller)
        {
            if (config is EMEChuckConfig emeChuckConfig)
            {
                if (controller is EMEChuckController emeChuckController)
                {
                    emeChuckController.InitStatesWithChuckConfiguration(ChuckConfiguration.SubstrateSlotConfigs);
                    _controller = emeChuckController;
                }
                else
                    throw new Exception("On EMEChuck creation, EME chuck controller type is not EMEChuckController as expected. Config type is " + _controller.GetType().ToString());


                // Alert on bad configs
                if (emeChuckConfig.SubstrateSlotConfigs.Count != 3)
                    Logger.Error("Configuration ERROR: Configuration SubstrateSlotConfigs should contains exactly three SubstrateSlotConfigs");
            }
            else
                throw new Exception("Chuck configuration type is not EMEChuckConfig as expected. Config type is " + config.GetType().ToString());
        }

        public override void Init()
        {
            State = new DeviceState(DeviceStatus.Unknown, "EME Chuck initialization sarted");
            try
            {
                base.Init();
                
                State = new DeviceState(DeviceStatus.Ready, "EME chuck initialization successfully completed.");
                ChuckController.ChuckStateChangedEvent += NotifyChuckStateUpdated;
            }
            catch (Exception ex) 
            {
                State = new DeviceState(DeviceStatus.Error, "initialization failed.");
                Logger.Information(Name + " device initialization failed: " + ex.Message + ((ex.StackTrace.Length > 0) ? " - " + ex.StackTrace : ""));
                throw;
            }
        }


        public override ChuckState GetState()
        {
            try
            {
                return ChuckController.GetState();
            }
            catch
            {
                State = new DeviceState(DeviceStatus.Error, Name + " get chuck state failed");
                return CreateDefaultChuckStateFromConfig();
            }
        }
        
        private void NotifyChuckStateUpdated(ChuckState chuckState)
        {
            var chuckServiceCallback = ClassLocator.Default.GetInstance<IChuckServiceCallbackProxy>();
            chuckServiceCallback.StateChanged(chuckState);
        }

        public MaterialPresence CheckWaferPresence(Length size)
        {           
            if (ChuckConfiguration.GetSubstrateSlotConfigs().Find(x => x.Diameter == size)?.IsPresenceSensorAvailable ?? false)
                return _controller.MaterialPresences[size];               
            else
                return MaterialPresence.Unknown;
        }

        public override List<Length> GetMaterialDiametersSupported()
        {
            var config = Configuration as EMEChuckConfig;
            return config.SubstrateSlotConfigs.Select(subSlot => subSlot.Diameter).ToList();
        }

        public override bool IsSensorPresenceEnable(Length size)
        {
            return ChuckConfiguration.GetSubstrateSlotConfigs().Find(x => x.Diameter == size).IsPresenceSensorAvailable;
        }


        #region Factory
        public static USPChuckBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller = null;

            if (config.IsSimulated) return new USPDummyChuck(globalStatusServer, logger, config);

            if (config is EMEChuckConfig emeChuckConfig)
            {
                if (emeChuckConfig.ControllerID != "None")
                {
                    bool found = controllers.TryGetValue(emeChuckConfig.ControllerID, out controller);
                    if (!found || (controller == null))
                        throw new Exception("Controller of the configuration was not found [deviceID = " + emeChuckConfig.DeviceID + ", ControllerId = " + emeChuckConfig.ControllerID + "]");

                    if (controller is IChuckController plChuckController) // TODO: Change IChuckController with the controller type specific for PhotoLum HW
                        return new EMEChuck(globalStatusServer, logger, emeChuckConfig, plChuckController);
                    else
                        throw new Exception("On Chuck creation, Controller supplied is not a PLChuckController. Controller type = " + controller.GetType());
                }
                else
                    return new EMEChuck(globalStatusServer, logger, emeChuckConfig, null); // TODO: During development, no controller. At the end, Chuck must have a hardware controller. Once PhotoLumController is defined, then remove this instance creation. 
            }
            else
                throw new Exception("Unknown config class" + config.GetType());
        }

        public void ClampWafer(Length size)
        {
            try
            {
                if (ChuckController is IChuckClamp controllerClamp)
                    controllerClamp.ClampWafer(size);
                else
                    throw new NotImplementedException("No IChuckClamp implemented");
            }
            catch
            {
                State = new DeviceState(DeviceStatus.Error, Name + " clamp wafer failed");
            }
        }

        public void ReleaseWafer(Length size)
        {
            try
            {
                if (ChuckController is IChuckClamp controllerClamp)
                    controllerClamp.ReleaseWafer(size);
                else
                    throw new NotImplementedException("No IChuckClamp implemented");
            }
            catch
            {
                State = new DeviceState(DeviceStatus.Error, Name + " release wafer failed");
            }
        }                     

        public void SetChuckInLoadingPosition(bool loadingPosition)
        {
            try
            {
                if (ChuckController is IChuckLoadingPosition controller)
                    controller.SetChuckInLoadingPosition(loadingPosition);
                else
                    throw new NotImplementedException("No IChuckLoadingPosition implemented");
            }
            catch
            {
                State = new DeviceState(DeviceStatus.Error, Name + " chuck loading position failed");
            }
        }

        public bool IsInLoadingPosition()
        {
            if (ChuckController is IChuckLoadingPosition controller)
                return controller.IsInLoadingPosition();
            else
                throw new NotImplementedException("IChuckLoadingPosition is not implemented");
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        #endregion
    }
}
