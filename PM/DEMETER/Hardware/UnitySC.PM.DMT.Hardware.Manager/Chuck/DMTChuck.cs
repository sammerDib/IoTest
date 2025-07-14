using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Hardware
{
    public class DMTChuck : USPChuckBase, IChuckMaterialPresence, IChuckLoadingPosition, IRemovableChuck
    {
        private ChuckState _chuckState;
        private PSDChuckController _controller;
        private Length _chuckSizeDetected;

        public DMTChuckConfig ChuckConfiguration
        {
            get
            {
                if (Configuration is DMTChuckConfig dmtConfig)
                    return dmtConfig;
                else
                    return null;
            }
        }

        public Length ChuckSizeDetected { get => _chuckSizeDetected; }

        public DMTChuck(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config, IChuckController controller)
            : base(globalStatusServer, logger, config, controller)
        {            
            if (config is DMTChuckConfig dmtChuckConfig)
            {
                Configuration = dmtChuckConfig;
                if (controller is PSDChuckController psdChuckController)
                {
                    psdChuckController.InitStatesWithChuckConfiguration(ChuckConfiguration.SubstrateSlotConfigs);
                    _controller = psdChuckController;
                }
                else
                    throw new Exception("On DMTChuck creation, DMT chuck controller type is not PSDChuckController as expected. Config type is " + _controller.GetType().ToString());


                if (!_controller.IsChuckStateChangedEventSet) _controller.ChuckStateChangedEvent += NotifyChuckStateUpdated;
                // Alert on bad configs
                if (ChuckConfiguration.SubstrateSlotConfigs.Count == 0)
                    Logger.Error("Configuration ERROR: Configuration SubstrateSlotConfigs should contains one SubstrateSlotConfig AT LEAST !");
                
                SubstrateSlotConfig ssConfig = ChuckConfiguration.GetSlotConfig(); // Not initialized yet, 1st config is taken as default config
                if(ssConfig is null )
                    throw new Exception("On DMTChuck creation, Chuck configuration not found. Config type is " + config.GetType().ToString());

                _chuckSizeDetected = ssConfig.Diameter;
                _chuckState = CreateChuckState(ssConfig.Diameter, false, MaterialPresence.Unknown);

            }
            else
                throw new Exception("Chuck configuration type is not DMTChuckConfig as expected. Config type is " + config.GetType().ToString());
        }

        private void NotifyChuckStateUpdated(ChuckState chuckState)
        {
            var chuckServiceCallback = ClassLocator.Default.GetInstance<IChuckServiceCallbackProxy>();
            chuckServiceCallback.StateChanged(chuckState);
        }

        public override bool IsSensorPresenceEnable(Length size)
        {            
            return ChuckConfiguration?.GetSubstrateSlotConfigByWafer(size)?.IsPresenceSensorAvailable ?? false; // Only one slot on PSD chuck
        }
        public override void Init()
        {
            base.Init();
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }
        public override bool IsMaterialPresenceRefreshed { get => _controller.IsMaterialPresenceRefreshed; }

        public override ChuckState GetState()
        {
            try
            {
                MaterialPresence materialPresence = CheckWaferPresence(ChuckSizeDetected);
                ChuckState chuckState = CreateChuckState(ChuckSizeDetected, false, materialPresence);
                return chuckState;
            }
            catch (Exception ex)
            {
                State = new DeviceState(DeviceStatus.Error, Name + " get chuck state failed");
                return CreateDefaultChuckStateFromConfig();
            }
        }

        public MaterialPresence CheckWaferPresence(Length size)
        {
            if(IsSensorPresenceEnable(size))                 
                return _controller.MaterialPresences[size];
            else
                return MaterialPresence.Unknown;
        }

        public override List<Length> GetMaterialDiametersSupported()
        {
            var sizes = new List<Length>();
            foreach (var substSlot in ChuckConfiguration.SubstrateSlotConfigs)
            {
                sizes.Add(substSlot.Diameter);
            }
            return sizes;
        }
        public ChuckState CreateChuckState(Length size, bool clamped, MaterialPresence presence)
        {
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(size, clamped);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(size, presence);
            return new ChuckState(clampStates, presenceStates);
        }

        #region Factory
        public static USPChuckBase Create(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config, Dictionary<string, ControllerBase> controllers)
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
                return new USPDummyChuck(globalStatusServer, logger, config);
            }

            if (config is DMTChuckConfig psdChuckConfig)
            {
                if (controller is PSDChuckController psdChuckController)
                    return new DMTChuck(globalStatusServer, logger, psdChuckConfig, psdChuckController);
                else
                    throw new Exception("On PSDChuck creation, Controller supplied is not a PSDChuckController. Controller type = " + controller.GetType());
            }
            else
                throw new Exception("Unknown config class" + config.GetType());
        }
        #endregion

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

        public void InitChuckSizeDetected(Length size)
        {
            _chuckSizeDetected = size;
        }
    }
}
