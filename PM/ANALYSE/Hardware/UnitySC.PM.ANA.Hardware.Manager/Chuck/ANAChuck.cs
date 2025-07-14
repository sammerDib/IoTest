using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware
{
    /// <summary>
    /// Handles the communication, the polling and the output activations of a Wago I/O device
    ///
    /// </summary>
    public class ANAChuck : USPChuckBase, IChuckClamp, IChuckMaterialPresence, IChuckAirBearing, IChuckInitialization
    {
        private IMessenger _messenger;

        protected IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                {
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                }

                return _messenger;
            }
        }
        private ANAChuckConfig ChuckConfig
        {
            get
            {
                if (Configuration is ANAChuckConfig anaConfig)
                {
                    return anaConfig;
                }
                else
                {
                    return null;
                }
            }
        }

        public override bool IsMaterialPresenceRefreshed { get => false; }
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverConfigFilePath">the XML configuration file path</param>
        /// <param name="SystemConfig">the global configuration object</param>
        public ANAChuck(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config,
            IChuckController controller)
            : base(globalStatusServer, logger)
        {
            ChuckController = controller;
            Configuration = config;
            if (config is ANAChuckConfig anaChuckConfig)
            {
                if (anaChuckConfig.SubstrateSlotConfigs.Count == 0)
                {
                    Logger.Error("Configuration ERROR: Configuration SubstrateSlotConfigs should contain one SubstrateSlotConfig AT LEAST !");
                }

                if (anaChuckConfig.SubstrateSlotConfigs.Count > 1)
                {
                    Logger.Warning("Configuration WARNING: Configuration SubstrateSlotConfigs should contain one SubstrateSlotConfig ONLY ! FIRST config in list is currently used");
                }
            }
            else
            {
                throw new Exception("Chuck configuration type is not ANAChuckConfig as expected. Config type is " +
                                    Configuration.GetType());
            }
        }

        #endregion Constructor

        #region implement ChuckBase

        /// <summary>
        /// Initializes the Wago connection and the I/O values
        /// </summary>
        /// <param name="serverConfigFilePath">the XML configuration file path</param>
        public override void Init()
        {
            State = new DeviceState(DeviceStatus.Unknown, "ANA Chuck initialization sarted");
            try
            {
                base.Init();

                if (ChuckController.State.Status != DeviceStatus.Ready)
                {
                    throw new Exception("Controller is not ready");
                }

                if (!ChuckController.IsChuckStateChangedEventSet)
                {
                    ChuckController.ChuckStateChangedEvent += NotifyChuckStateUpdated;
                }

                State = new DeviceState(DeviceStatus.Ready, "ANA chuck initialization successfully completed.");
            }
            catch (Exception Ex)
            {
                State = new DeviceState(DeviceStatus.Error, "initialization failed.");
                Logger.Information(Name + " device initialization failed: " + Ex.Message +
                                   ((Ex.StackTrace.Length > 0) ? " - " + Ex.StackTrace : ""));
                throw;
            }
        }

        public override bool IsSensorPresenceEnable(Length size)
        {
            return ChuckConfig.GetSubstrateSlotConfigs().Find(x => x.Diameter == size).IsPresenceSensorAvailable;
        }

        public void ClampWafer(Length size)
        {
            try
            {
                if (ChuckController is IChuckClamp controllerClamp)
                {
                    controllerClamp.ClampWafer(size);
                }
                else
                {
                    throw new NotImplementedException("No IChuckClamp implemented");
                }
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
                {
                    controllerClamp.ReleaseWafer(size);
                }
                else
                {
                    throw new NotImplementedException("No IChuckClamp implemented");
                }
            }
            catch
            {
                State = new DeviceState(DeviceStatus.Error, Name + " release wafer failed");
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
            Messenger.Send(new DataAttributesChuckMessage() { State = chuckState });
        }


        public MaterialPresence CheckWaferPresence(Length size)
        {
            if (Configuration.IsSimulated)
            {
                return MaterialPresence.Unknown;
            }

            if (ChuckConfig.GetSubstrateSlotConfigs().Find(x => x.Diameter == size)?.IsPresenceSensorAvailable ?? false)
            {
                if (ChuckController is IChuckMaterialPresence controllerClamp)
                {
                    return controllerClamp.CheckWaferPresence(size);
                }
                else
                {
                    return MaterialPresence.Unknown;
                }
            }
            else
            {
                return MaterialPresence.Unknown;
            }
        }

        public void InitAirbearing()
        {
            if (ChuckController is IChuckAirBearing controllerAirBearing)
            {
                controllerAirBearing.InitAirbearing();
            }
        }


        public Dictionary<string, float> GetAirBearingPressuresValues()
        {
            if (ChuckController is IChuckAirBearing controllerAirBearing)
            {
                return controllerAirBearing.GetAirBearingPressuresValues();
            }
            else
            {
                return null;
            }
        }

        public override List<Length> GetMaterialDiametersSupported()
        {
            var sizes = new List<Length>();
            foreach (var substSlot in ChuckConfig.SubstrateSlotConfigs)
            {
                sizes.Add(substSlot.Diameter);
            }

            return sizes;
        }

        #endregion implement ChuckBase

        #region implement ChuckInitialization

        public void InitWaferStage()
        {
            if (ChuckController is ACSController acsController)
            {
                acsController.InitTableAxes();
            }
        }

        #endregion

        #region Factory

        public static USPChuckBase Create(IGlobalStatusServer globalStatusServer, ILogger logger,
            ChuckBaseConfig config, Dictionary<string, ControllerBase> controllers)
        {
            ControllerBase controller = null;

            if (!(config.ControllerID == "None"))
            {
                bool found = controllers.TryGetValue(config.ControllerID, out controller);
                if (!found || (controller == null))
                {
                    throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID +
                                        ", ControllerId = " + config.ControllerID + "]");
                }
            }

            if (config.IsSimulated)
            {
                return new USPDummyChuck(globalStatusServer, logger, config);
            }

            if (config is ANAChuckConfig anaChuckConfig)
            {
                if (controller is ACSController acsController)
                {
                    return new ANAChuck(globalStatusServer, logger, anaChuckConfig, acsController);
                }
                else
                {
                    throw new Exception( "On ANAChuck creation, Controller supplied is not a ACSController. Controller type = " +
                                        controller.GetType());
                }
            }
            else
            {
                throw new Exception("Unknown config class" + config.GetType());
            }
                
        }
        // Heritage des devices avec PLC - not used  
        public override void TriggerUpdateEvent()
        {
        }

        #endregion
    }
}
