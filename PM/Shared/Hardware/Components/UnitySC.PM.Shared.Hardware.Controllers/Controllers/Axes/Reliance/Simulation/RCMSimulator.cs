using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Communication;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance.Simulation
{
    /// <summary>
    /// Simulator of a RCM hardware controller, exposed through a serial port.
    /// </summary>
    public class RCMSimulator : ISerialPort
    {
        public string PortName { get; }

        public event DataReceivedEventHandler DataReceived;

        public bool IsOpen { get; private set; } = false;

        private Dictionary<string, Motor> _motors;
        private ConcurrentQueue<string> _reponsesBuffer = new ConcurrentQueue<string>();

        private IEnumerable<Handler> _handlers;

        public TimeSpan MotionDelay
        {
            set
            {
                foreach (var motor in _motors.Values)
                {
                    motor.MotionDelay = value;
                }
            }
        }

        public RCMSimulator(string portName, IEnumerable<RelianceAxis> axes, TimeSpan motionDelay)
        {
            PortName = portName;

            var setParameterHandler = new Handler(
                @"^(K\d\d).(\d+)=(-?\d+)$",
                param =>
                {
                    string motorId = param[2];
                    _motors[motorId].SearchOriginParameters[param[1]] = int.Parse(param[3]);
                }
            );
            var setPositionHandler = new Handler(
                @"^P.(\d+)=(-?\d+)$",
                param =>
                {
                    string motorId = param[1];
                    _motors[motorId].PendingMotionParameters.Position = int.Parse(param[2]);
                }
            );
            var setSpeedHandler = new Handler(
                @"^S.(\d+)=(\d+)$",
                param =>
                {
                    string motorId = param[1];
                    _motors[motorId].PendingMotionParameters.Speed = int.Parse(param[2]);
                }
            );
            var setAccelerationHandler = new Handler(
                @"^A.(\d+)=(\d+)$",
                param =>
                {
                    string motorId = param[1];
                    _motors[motorId].PendingMotionParameters.Acceleration = int.Parse(param[2]);
                }
            );
            var motionHandler = new Handler(
                @"^\^.(\d+)$",
                param =>
                {
                    string motorId = param[1];
                    try
                    {
                        //"_ =" to disable warning CS4014
                        _ = _motors[motorId].MoveASync();
                    }
                    catch (ZeroSpeedException)
                    {
                        _reponsesBuffer.Enqueue($"invalid speed data S0.{motorId}=0");
                    }
                }
            );
            var searchOriginHandler = new Handler(
                @"^\|.(\d+)$",
                param =>
                {
                    string motorId = param[1];
                    try
                    {
                        //"_ =" to disable warning CS4014
                        _ = _motors[motorId].SearchOriginAsync();
                    }
                    catch (ZeroSpeedException e)
                    {
                        _reponsesBuffer.Enqueue($"invalid speed data S0.{motorId}=0. " + e);
                    }
                }
            );
            var queryCurrentPositionHandler = new Handler(
                @"^?96.(\d+)$",
                param =>
                {
                    string motorId = param[1];
                    int position = _motors[motorId].CurrentPosition;
                    _reponsesBuffer.Enqueue($"Px.{motorId}={position}");
                    DataReceived?.Invoke();
                }
            );
            var queryMotorStatusHandler = new Handler(
                @"^?99.(\d+)$",
                param =>
                {
                    string motorId = param[1];
                    string status = _motors[motorId].Status;
                    _reponsesBuffer.Enqueue($"Ux.{motorId}={status}");
                    DataReceived?.Invoke();
                }
            );
            var stopHandler = new Handler(
                @"^\*$",
                _ =>
                {
                    foreach (var motor in _motors.Values)
                    {
                        motor.Stop();
                    }
                }
            );
            var resetHandler = new Handler(
                @"^\*1$",
                _ =>
                {
                    foreach (var motor in _motors.Values)
                    {
                        motor.Reset();
                    }
                }
            );
            var enableHandler = new Handler(@"^\($", _ => { });
            _handlers = new HashSet<Handler>
            {
                setParameterHandler,
                setPositionHandler,
                setSpeedHandler,
                setAccelerationHandler,
                motionHandler,
                searchOriginHandler,
                queryCurrentPositionHandler,
                queryMotorStatusHandler,
                stopHandler,
                resetHandler,
                enableHandler
            };

            _motors = axes
                .Select(axis => axis.Config.RelianceAxisID)
                .ToDictionary(axisId => axisId, axisId => new Motor(axisId, motionDelay));
        }

        public void Open()
        {
            if (IsOpen)
            {
                throw new Exception("Already opened");
            }

            IsOpen = true;
        }

        public void Close()
        {
            if (!IsOpen)
            {
                throw new Exception("Cannot close not opened connection");
            }

            IsOpen = false;
        }

        public string ReadExisting()
        {
            if (!IsOpen)
            {
                throw new Exception("Connection not opened");
            }

            var aggregatedResponses = new StringBuilder();
            while (_reponsesBuffer.TryDequeue(out string response))
            {
                aggregatedResponses.Append(response + "\r\n");
            }

            return aggregatedResponses.ToString();
        }

        public void Dispose()
        {
        }

        public void Write(string message)
        {
            if (!IsOpen)
            {
                throw new Exception("Connection not opened");
            }

            if (!message.EndsWith("\r\n"))
            {
                throw new Exception(@"Unknown message: does not end with \r\n");
            }

            string[] messages = message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string msg in messages)
            {
                Handle(msg);
            }
        }

        public void Write(byte[] message)
        {
            Write(Encoding.UTF8.GetString(message));
        }

        private void Handle(string message)
        {
            foreach (var handler in _handlers)
            {
                var match = handler.Matches(message);
                if (!match.Success) continue;
                string[] matchingValues = (
                    from Group @group in match.Groups
                    select @group.Value
                ).ToArray();
                handler.Handle(matchingValues);
                return;
            }

            throw new Exception($"Unsupported message '{message}'");
        }

        /// <param name="position">Requested position, in hardware referential (unit: pulses)</param>
        public void MoveTo(int position, string motorId)
        {
            _motors[motorId].CurrentPosition = position;
        }
    }
}
