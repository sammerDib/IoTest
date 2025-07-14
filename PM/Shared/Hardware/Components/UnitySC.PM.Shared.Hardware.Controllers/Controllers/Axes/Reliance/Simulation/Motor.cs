using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance.Simulation
{
    /// <summary>
    /// Simulated motor, with its current state.
    /// </summary>
    internal class Motor
    {
        private readonly string _id;
        private readonly ILogger _logger;

        // TODO: Extract constants, and expose it in production code
        public string Status = "8";

        // Unit: pulse
        // Origin: beginning of the axis.
        // Always positive
        public int CurrentPosition = 0;

        public readonly Dictionary<string, int> SearchOriginParameters = new Dictionary<string, int>
        {
            { "K42", 100 },
            { "K43", 1_000 },
            { "K45", 1 },
            { "K46", 2 },
            { "K47", 100 },
            { "K48", 0 },
        };

        public readonly MotionParameters PendingMotionParameters = new MotionParameters
        {
            Position = 0, Speed = 100, Acceleration = 200,
        };

        public TimeSpan MotionDelay;

        public Motor(string id, TimeSpan motionDelay)
        {
            _id = id;
            _logger = ClassLocator.Default.GetInstance<ILogger<Motor>>();
            MotionDelay = motionDelay;
        }

        public async Task MoveASync()
        {
            await MoveASync(PendingMotionParameters);
        }

        private CancellationTokenSource _motionCancellationToken;

        private Task MoveASync(MotionParameters parameters)
        {
            if (parameters.Speed == 0)
            {
                throw new ZeroSpeedException();
            }
            _motionCancellationToken = new CancellationTokenSource();
            Status = "0";
            return Task.Run(() =>
                    {
                        Thread.Sleep(MotionDelay);
                        _motionCancellationToken.Token.ThrowIfCancellationRequested();

                        CurrentPosition = parameters.Position;
                        Status = "8";
                        _motionCancellationToken = null;
                        _logger.Debug($"Motor {_id}: moved to position {CurrentPosition}");
                    },
                    _motionCancellationToken.Token
                )
                .ContinueWith(_ => _motionCancellationToken = null);
        }

        public async Task SearchOriginAsync()
        {
            int speed = SearchOriginParameters["K42"];
            int acceleration = SearchOriginParameters["K43"];
            await MoveASync(new MotionParameters { Position = 0, Speed = speed, Acceleration = acceleration });
            _logger.Debug($"Motor {_id}: origin searched");
        }

        public void Stop()
        {
            _motionCancellationToken?.Cancel();
        }

        public void Reset()
        {
            Status = "8";
        }
    }

    internal class ZeroSpeedException : Exception
    {
    }
}
