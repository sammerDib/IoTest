using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser.Leukos;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser
{
    public class Piano450LaserDummyController : LaserController
    {
        private readonly ILogger _logger;
        private CancellationTokenSource _cancelationTokenSrc;

        private Task _pollTask;

        public Piano450LaserDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;

        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init Piano450LaserController as dummy");
            Connect();
        }

        public override bool ResetController()
        {
            try
            {
                Disconnect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void Connect()
        {
            _cancelationTokenSrc?.Cancel();
            _cancelationTokenSrc = new CancellationTokenSource();
            _pollTask = new Task(async () => { 
                await TaskLaserPollingAsync(); },   TaskCreationOptions.LongRunning);
            _pollTask.Start();
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect()
        {
            if (_cancelationTokenSrc != null)
            {
                _cancelationTokenSrc.Cancel();
                _pollTask.Wait(500);
                _cancelationTokenSrc = null;
            }
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override void PowerOn()
        {
        }

        public override void PowerOff()
        {
        }

        public override void ReadPower()
        {
            throw new NotImplementedException();
        }

        public override void SetPower(double power)
        {
            throw new NotImplementedException();
        }

        public override void TriggerUpdateEvent()
        {
        }

        public override void CustomCommand(string custom)
        {
        }

        private async Task TaskLaserPollingAsync()
        {
            var cancelToken = _cancelationTokenSrc.Token;

            var rnd = new Random();
            double laserTemp = 100.0;
            double laserNoise = 8.5;

            double crystalTemp = 30.0;
            double crystalNoise = 2.5;

            double laserTemperature = laserTemp;
            double crystalTemperature = crystalTemp;

            while (cancelToken.IsCancellationRequested == false)
            {
                try
                {
                    Messenger.Send(new LaserTemperatureMessage() { LaserTemperature = laserTemperature });
                    laserTemperature = laserTemp + laserNoise * (rnd.NextDouble() - 0.5);
           
                    Messenger.Send(new CrystalTemperatureMessage() { CrystalTemperature = crystalTemperature });
                    crystalTemperature = crystalTemp + crystalNoise * (rnd.NextDouble() - 0.5);
                }
                catch (AggregateException aex)
                {                 
                    _logger?.Error($"Dummy Laser aggregexception : {aex.Message}");
                }
                catch (Exception Ex)
                {
                    _logger?.Error($"Dummy Laser exception : {Ex.Message}");
                }

                await Task.Delay(250, cancelToken);
            }
        }

    }
}
