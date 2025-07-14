using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Spectrometer;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware
{
    public class SpectrometerTest : FunctionalTest
    {
        private SpectrometerAVSController _controller;

        public override void Run()
        {
            Logger.Information("\n******************\n[SpectrometerTest] Test suite START\n******************\n");
            InitSpectrometerController();
            Init();
            DoMeasure();
            Disconnect();
            Reset();
            SerialNumberIsNotFound();
            Logger.Information("\n******************\n[SpectrometerTest] Test suite STOP\n******************\n");
        }

        public void InitSpectrometerController()
        {
            var config = HardwareManager.Spectrometers.Values.FirstOrDefault().Configuration;
            if (config == null)
            {
                Logger.Error("Config not found");
                throw new Exception("Config not found");
            }
            HardwareManager.Controllers.TryGetValue(config.ControllerID, out var controller);
            _controller = controller as SpectrometerAVSController;
            if (_controller == null)
            {
                Logger.Error("Controller not found");
                throw new Exception("Controller not found");
            }
        }

        public void Init()
        {
            Logger.Information("\n******************[Init] START******************\n");
            try
            {
                var initErrors = new List<Message>();
                _controller.Init(initErrors);
            }
            catch
            {
                throw new Exception("Test Init faillure");
            }
            Logger.Information("\n******************[Init] STOP******************\n");
        }

        public void SerialNumberIsNotFound()
        {
            Logger.Information("\n******************[SerialNumerberNotFound] START******************\n");
            var controllerConfig = _controller.ControllerConfiguration as SpectrometerAvantesControllerConfig;
            controllerConfig.SerialNumber = "foo";
            try
            {
                Init();
            }
            catch
            {
                Logger.Information("Test SerialNumberIsNotFound success");
                Logger.Information("\n******************[SerialNumerberNotFound] STOP******************\n");
                return;
            }
            throw new Exception("Test SerialNumberIsNotFound faillure");
        }

        public void Disconnect()
        {
            Logger.Information("\n******************[Disconnect] START******************\n");
            try
            {
                Init();
                _controller.Disconnect();
            }
            catch
            {
                throw new Exception("Test Disconnect faillure");
            }
            Logger.Information("\n******************[Disconnect] STOP******************\n");
        }

        public void Reset()
        {
            Logger.Information("\n******************[Reset] START******************\n");
            try
            {
                Init();
                if (!_controller.ResetController())
                {
                    Logger.Information("\n******************[Reset] STOP******************\n");
                    throw new Exception("Test ResetController faillure");
                }
            }
            catch
            {
                throw new Exception("Test ResetController faillure");
            }
            Logger.Information("\n******************[Reset] STOP******************\n");
        }

        public void DoMeasure()
        {
            Logger.Information("\n******************[DoMeasure] START******************\n");
            Init();
            _controller.DoMeasure(param:null);
            _controller.StopMeasure();
            Logger.Information("\n******************[DoMeasure] STOP******************\n");
        }
    }
}
