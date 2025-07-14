using System;
using System.Threading;

using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.IDSCamera
{
    public class UI324xCpNir : IDSCameraBase
    {
        protected new UI324xCpNirIDSCameraConfig Config;

        public UI324xCpNir(UI324xCpNirIDSCameraConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
            Config = config;
        }

        #region LIFECYCLE

        public override void Init()
        {
            base.Init();

            SetGPIOConfiguration(uEye.Defines.IO.GPIOConfiguration.Output);
            SetGPIOState(uEye.Defines.IO.State.Low);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion LIFECYCLE

        #region TRIGGER

        private uEye.Defines.IO.GPIO GpioLineId
        {
            get
            {
                int gpioLineId = Config.GpioLineId;
                if (gpioLineId == default)
                    throw new ApplicationException("GPIO line ID not found in the configuration.");

                switch (gpioLineId)
                {
                    case 1: return uEye.Defines.IO.GPIO.One;
                    case 2: return uEye.Defines.IO.GPIO.Two;
                    default: throw new ApplicationException("Unsupported GPIO line ID.");
                }
            }
        }

        /// <summary>
        /// Orders the camera to emit a trigger out signal of 3.3V on the GPIO line specified in the
        /// configuration (1 or 2).
        /// </summary>
        /// <param name="pulseDuration_ms">
        /// Time (in milliseconds) during which the signal is emitted. Default value is 1 ms.
        /// </param>
        /// <param name="logTriggerStartStop">Enable/Disable debug logging about trigger status.</param>
        public override void TriggerOutEmitSignal(int pulseDuration_ms = 1, bool logTriggerStartStop = false)
        {
            SetGPIOConfiguration(uEye.Defines.IO.GPIOConfiguration.Output);
            SetGPIOState(uEye.Defines.IO.State.High);
            if (logTriggerStartStop) Logger.Debug($"{Name} starts trigger OUT during {pulseDuration_ms} ms.........");
            Thread.Sleep(pulseDuration_ms);
            if (logTriggerStartStop) Logger.Debug($"{Name} stops trigger OUT");
            SetGPIOState(uEye.Defines.IO.State.Low);
        }

        private uEye.Defines.IO.GPIOConfiguration GetGPIOConfiguration()
        {
            ApiErrorHandler(Camera.IO.Gpio.GetConfiguration(GpioLineId, out _, out var configuration, out _));
            return configuration;
        }

        private void SetGPIOConfiguration(uEye.Defines.IO.GPIOConfiguration configuration)
        {
            ApiErrorHandler(Camera.IO.Gpio.SetConfiguration(GpioLineId, configuration));
        }

        private uEye.Defines.IO.GPIOConfiguration GetGPIOSupportedConfiguration()
        {
            ApiErrorHandler(Camera.IO.Gpio.GetConfiguration(GpioLineId, out var supportedConfiguration, out _, out _));
            return supportedConfiguration;
        }

        private uEye.Defines.IO.State GetGPIOState()
        {
            ApiErrorHandler(Camera.IO.Gpio.GetConfiguration(GpioLineId, out _, out _, out var gpioState));
            return gpioState;
        }

        private void SetGPIOState(uEye.Defines.IO.State state)
        {
            var currentConfiguration = GetGPIOConfiguration();
            ApiErrorHandler(Camera.IO.Gpio.SetConfiguration(GpioLineId, currentConfiguration, state));
        }

        #endregion TRIGGER
    }
}
