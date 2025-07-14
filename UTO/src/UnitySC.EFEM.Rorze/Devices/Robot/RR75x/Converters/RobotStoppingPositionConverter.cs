using System;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Converters;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Converters
{
    public class RobotStoppingPositionConverter : IStoppingPositionConverter
    {
        #region Fields

        private readonly RR75x _robot;
        private readonly IRorzeStoppingPositionConverterCallBack _callback;
        private readonly ILogger _logger;

        #endregion Fields

        #region Constructor

        public RobotStoppingPositionConverter(RR75x robot, ILogger logger = null)
        {
            _logger = logger ?? Logger.GetLogger(nameof(RobotStoppingPositionConverter));
            _robot = robot;

            var callback = robot.GetTopDeviceContainer()
                .AllDevices()
                .FirstOrDefault(d => d is IRorzeStoppingPositionConverterCallBack);

            if (callback is not IRorzeStoppingPositionConverterCallBack converterCallBack)
            {
                throw new InvalidOperationException(
                    $"Mandatory device of type {nameof(IRorzeStoppingPositionConverterCallBack)} is not found in equipment model tree.");
            }

            _callback = converterCallBack;
        }

        #endregion Constructor

        #region IStoppingPositionConverter

        /// <inheritdoc cref="ToTransferLocation"/>
        public TransferLocation ToTransferLocation(double stoppingPosition, bool is0BasedIndexing)
        {
            foreach (var moduleStoppingPositions in _robot.Configuration.StoppingPositionPerSampleSize.Values)
            {
                foreach (var tupleModuleStoppingPositions in moduleStoppingPositions.StoppingPositionsPerModule)
                {
                    foreach (var stoppingPositionInConfig in tupleModuleStoppingPositions.Value.StoppingPositions.Values)
                    {
                        // Stopping positions are stored in 1..400 validity range and not in 0..399
                        // If we want the 0 based value, we have to get value minus 1 (x - 1)
                        if (Math.Abs((is0BasedIndexing ? stoppingPositionInConfig - 1 : stoppingPositionInConfig) - stoppingPosition) < 1)
                        {
                            return tupleModuleStoppingPositions.Key;
                        }
                    }
                }
            }

            throw new InvalidOperationException(
                $"{nameof(RobotStoppingPositionConverter)} - {nameof(ToTransferLocation)} - "
                + $"The given stopping position (\"{stoppingPosition}\") has not been found in robot configuration.");
        }

        /// <inheritdoc cref="ToStoppingPosition"/>
        public double ToStoppingPosition(TransferLocation transferLocation, RobotArm arm, bool is0BasedIndexing)
        {
            var waferDimension = _callback.GetSampleDimension(transferLocation, arm);
            var innerLocationType = _callback.GetInnerLocation(transferLocation);

            if (!_robot.Configuration.StoppingPositionPerSampleSize.ContainsKey(waferDimension))
            {
                var firstDimension = _robot.Configuration.StoppingPositionPerSampleSize.Keys.First();
                _logger.Warning(
                    $"Could not convert transfer location to stopping position for current wafer size: {waferDimension}. First available size used: {firstDimension}.");
                waferDimension = firstDimension;
            }

            var sampleDimensionStoppingPositionConfig = _robot.Configuration
                .StoppingPositionPerSampleSize[waferDimension]
                .StoppingPositionsPerModule;

            if (!sampleDimensionStoppingPositionConfig.ContainsKey(transferLocation))
            {
                throw new InvalidOperationException(
                    $"Could not convert transfer location to stopping position for given location: {transferLocation}");
            }

            var locationStoppingPositionConfig =
                sampleDimensionStoppingPositionConfig[transferLocation].StoppingPositions;

            if (!locationStoppingPositionConfig.ContainsKey(innerLocationType))
            {
                throw new InvalidOperationException(
                    $"Could not convert transfer location to stopping position for given inner location: {innerLocationType}");
            }

            // Stopping positions are stored in 1..400 validity range and not in 0..399
            // If we want the 0 based value, we have to get value minus 1 (x - 1)
            return is0BasedIndexing
                ? locationStoppingPositionConfig[innerLocationType] - 1
                : locationStoppingPositionConfig[innerLocationType];
        }

        #endregion IStoppingPositionConverter
    }
}
