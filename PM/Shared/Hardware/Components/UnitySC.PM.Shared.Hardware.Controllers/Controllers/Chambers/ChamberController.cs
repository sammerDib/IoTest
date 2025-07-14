using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers
{
    public abstract class ChamberController : ControllerBase, IControllerIO
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        protected ControllerConfig ControllerConfig;

        internal string DeviceName = "Chamber";

        public Dictionary<string, Input> NameToInput { get; set; }
        public Dictionary<string, Output> NameToOutput { get; set; }

        protected ChamberController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            ControllerConfig = controllerConfig;
        }

        public abstract void TriggerUpdateEvent();

        private Task GetTask(string name, Dictionary<string, Task> dico)
        {
            bool succes = dico.TryGetValue(name, out var task);
            if (!succes)
            {
                throw new InvalidOperationException($"Could not get task {name}.");
            }
            return task;
        }

        public bool DigitalRead(DigitalInput input)
        {
            //Nothing oO'
            return false;
        }

        public void DigitalWrite(DigitalOutput output, bool value)
        {
            //Nothing oO'
        }

        public double AnalogRead(AnalogInput input)
        {
            //Nothing oO'
            return 0.0;
        }

        public void AnalogWrite(AnalogOutput output, double value)
        {
            //Nothing oO'
        }

        public Input GetInput(string name)
        {
            if (!NameToInput.ContainsKey(name))
            {
                string message = $"{name} is not known in the configuration. Check in the AnaHardwareConfiguration file.";
                Logger.Error(message);
                throw new InvalidOperationException(message);
            }
            return NameToInput[name];
        }

        public Output GetOutput(string name)
        {
            if (!NameToOutput.ContainsKey(name))
            {
                string message = $"A device tried to get an Output named: {name} but it does not exist in the {ControllerConfiguration.Name} controller configuration";
                Logger.Error(message);
                throw new InvalidOperationException(message);
            }
            return NameToOutput[name];
        }

        public System.Threading.Tasks.Task StartRefreshIOStatesTask()
        {
            //Nothing
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void StopRefreshIOStatesTask()
        {
            //Nothing
        }
    }
}
