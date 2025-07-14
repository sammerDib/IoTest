using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public abstract class ControllerBase : DeviceBase
    {
        public override DeviceFamily Family => DeviceFamily.Controller;
        public ControllerConfig ControllerConfiguration { get; set; }
        public ILogger Logger { get; set; }

        public ControllerBase(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(globalStatusServer, logger)
        {
            if (controllerConfig == null) throw new Exception("Invalid Controller configuration (NULL)");
            if (string.IsNullOrEmpty(controllerConfig.Name)) throw new Exception("Invalid Name for controller in configuration");
            if (string.IsNullOrEmpty(controllerConfig.DeviceID)) throw new Exception("Invalid DeviceID for controller in configuration");

            Name = controllerConfig.Name;
            DeviceID = controllerConfig.DeviceID;

            ControllerConfiguration = controllerConfig;
            Logger = logger;

        }

        #region methods

        /// <summary>
        /// Connect to hardware device.
        /// </summary>
        /// <param name="initErrors">List of errors, which are appended during initialization</param>
        public abstract void Init(List<Message> initErrors);

        public abstract bool ResetController();

        public abstract void Connect();

        public abstract void Connect(string deviceId);

        public abstract void Disconnect();

        public abstract void Disconnect(string deviceID);

        #endregion methods
    }
}
