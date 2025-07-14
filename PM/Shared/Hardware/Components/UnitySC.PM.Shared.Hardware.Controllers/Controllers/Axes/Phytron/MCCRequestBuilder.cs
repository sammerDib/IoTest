using System;
using System.Text.RegularExpressions;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Communication;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Phytron
{
    /// <summary>
    /// Used by MCC Controller to build all commands and queries required for serial port communication.
    /// </summary>
    public class MCCRequestBuilder
    {
        private const int MinSpeedStepsPerSecond = 1;
        private const int MaxSpeedStepsPerSecond = 40_000;
        private const int MinAccelerationStepsPerSecondSquared = 1;
        private const int MaxAccelerationStepsPerSecondSquared = 500_000;

        private readonly Regex _defaultCommandAck = new Regex($"{NPC.STX}{NPC.ACK}{NPC.ETX}");

        private readonly int _address;
        private readonly ILogger<MCCRequestBuilder> _logger;

        public MCCRequestBuilder(int address)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<MCCRequestBuilder>>();
            _address = address;
        }

        public SerialPortCommand<string> SetSpeed(PhytronAxis axis, int speedStepsPerSecond)
        {
            var speed = Math.Max(speedStepsPerSecond, MinSpeedStepsPerSecond);
            if (speed != speedStepsPerSecond)
            {
                _logger.Warning(
                    $"Speed ({speedStepsPerSecond}) below minimum ({MinSpeedStepsPerSecond}). Speed used: {speed}"
                );
            }

            speed = Math.Min(speedStepsPerSecond, MaxSpeedStepsPerSecond);
            if (speed != speedStepsPerSecond)
            {
                _logger.Warning(
                    $"Speed ({speedStepsPerSecond}) above maximum ({MaxSpeedStepsPerSecond}). Speed used: {speed}"
                );
            }

            return new SerialPortCommand<string>
            {
                Message = $"{NPC.STX}{_address}{axis.Config.PhytronAxisID}P14S{speed}{NPC.ETX}",
                AcknowlegedResponsePattern = _defaultCommandAck,
            };
        }

        public SerialPortCommand<string> SetAcceleration(PhytronAxis axis, int accelerationStepsPerSecondSquared)
        {
            var speed = Math.Max(accelerationStepsPerSecondSquared, MinAccelerationStepsPerSecondSquared);
            if (speed != accelerationStepsPerSecondSquared)
            {
                _logger.Warning(
                    $"Speed ({accelerationStepsPerSecondSquared}) below minimum ({MinAccelerationStepsPerSecondSquared}). Speed used: {speed}"
                );
            }

            speed = Math.Min(accelerationStepsPerSecondSquared, MaxAccelerationStepsPerSecondSquared);
            if (speed != accelerationStepsPerSecondSquared)
            {
                _logger.Warning(
                    $"Speed ({accelerationStepsPerSecondSquared}) above maximum ({MaxAccelerationStepsPerSecondSquared}). Speed used: {speed}"
                );
            }

            return new SerialPortCommand<string>()
            {
                Message =
                    $"{NPC.STX}{_address}{axis.Config.PhytronAxisID}P15S{accelerationStepsPerSecondSquared}{NPC.ETX}",
                AcknowlegedResponsePattern = _defaultCommandAck,
            };
        }

        public SerialPortCommand<string> SetPosition(PhytronAxis axis, int positionSteps)
        {
            return new SerialPortCommand<string>()
            {
                Message = $"{NPC.STX}{_address}{axis.Config.PhytronAxisID}A{positionSteps}{NPC.ETX}",
                AcknowlegedResponsePattern = _defaultCommandAck,
            };
        }

        public SerialPortQuery<string> GetMotorStatus()
        {
            return new SerialPortQuery<string>()
            {
                Message = $"{NPC.STX}{_address}SH{NPC.ETX}",
                ResponsePattern = new Regex($"{NPC.STX}{NPC.ACK}([EN]){NPC.ETX}"),
            };
        }

        public SerialPortQuery<string> GetCurrentPosition(PhytronAxis axis)
        {
            return new SerialPortQuery<string>()
            {
                Message = $"{NPC.STX}{_address}{axis.Config.PhytronAxisID}P20R{NPC.ETX}",
                ResponsePattern = new Regex($@"{NPC.STX}{NPC.ACK}(-?\d+){NPC.ETX}"),
            };
        }

        public SerialPortCommand<string> StopMotor(PhytronAxis axis)
        {
            return new SerialPortCommand<string>
            {
                Message = $"{NPC.STX}{_address}{axis.Config.PhytronAxisID}S{NPC.ETX}",
                AcknowlegedResponsePattern = _defaultCommandAck,
            };
        }
    }
}
