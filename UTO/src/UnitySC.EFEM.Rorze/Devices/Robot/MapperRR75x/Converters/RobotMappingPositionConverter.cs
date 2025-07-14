using System;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Converters;

namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Converters
{
    public class RobotMappingPositionConverter : IMappingPositionConverter
    {
        #region Fields

        private readonly MapperRR75x _robot;
        private readonly IStoppingPositionConverterCallBack _callback;
        private readonly ILogger _logger;

        #endregion Fields

        #region Constructor

        public RobotMappingPositionConverter(MapperRR75x robot, ILogger logger = null)
        {
            _robot = robot;
            _logger = logger ?? Logger.GetLogger(nameof(RobotMappingPositionConverter));

            var callback = robot.GetTopDeviceContainer()
                .AllDevices()
                .FirstOrDefault(d => d is IStoppingPositionConverterCallBack);

            if (callback is not IStoppingPositionConverterCallBack converterCallBack)
            {
                throw new InvalidOperationException(
                    $"Mandatory device of type {nameof(IStoppingPositionConverterCallBack)} is not found in equipment model tree.");
            }

            _callback = converterCallBack;
        }

        #endregion Constructor

        #region IMappingPositionConverter

        /// <inheritdoc cref="ToTransferLocation"/>
        public TransferLocation ToTransferLocation(uint mappingPosition, bool is0BasedIndexing)
        {
            foreach (var sampleSizeMappingPositions in _robot.Configuration.MappingPositionPerSampleSize.Values)
            {
                foreach (var tupleModuleMappingPositions in sampleSizeMappingPositions.MappingPositionsPerModule)
                {
                    var position = tupleModuleMappingPositions.Value;

                    // Mapping positions are stored in 1..400 validity range and not in 0..399
                    // If we want the 0 based value, we have to get value minus 1 (x - 1)
                    if ((is0BasedIndexing ? position.ArmFirstMappingPosition - 1 : position.ArmFirstMappingPosition) == mappingPosition)
                    {
                        return tupleModuleMappingPositions.Key;
                    }

                    if ((is0BasedIndexing ? position.ArmSecondMappingPosition - 1 : position.ArmSecondMappingPosition) == mappingPosition)
                    {
                        return tupleModuleMappingPositions.Key;
                    }
                }
            }

            throw new InvalidOperationException(
                $"{nameof(RobotMappingPositionConverter)} - {nameof(ToTransferLocation)} - "
                + $"The given stopping position (\"{mappingPosition}\") has not been found in robot configuration.");
        }

        /// <inheritdoc cref="ToMappingPosition"/>
        public uint ToMappingPosition(TransferLocation transferLocation, RobotArm arm, bool isFirstPosition, bool is0BasedIndexing)
        {
            var waferDimension = _callback.GetSampleDimension(transferLocation, arm);

            if (!_robot.Configuration.MappingPositionPerSampleSize.ContainsKey(waferDimension))
            {
                var firstDimension = _robot.Configuration.MappingPositionPerSampleSize.Keys.First();
                _logger.Warning(
                    $"Could not convert transfer location to mapping position for current wafer size: {waferDimension}. First available size used: {firstDimension}.");
                waferDimension = firstDimension;
            }

            var sampleDimensionMappingPositionConfig = _robot.Configuration
                .MappingPositionPerSampleSize[waferDimension]
                .MappingPositionsPerModule;

            if (!sampleDimensionMappingPositionConfig.ContainsKey(transferLocation))
            {
                throw new InvalidOperationException(
                    $"Could not convert transfer location to mapping position for given location: {transferLocation}");
            }

            var mappingPosition = sampleDimensionMappingPositionConfig[transferLocation];
            var position = isFirstPosition ? mappingPosition.ArmFirstMappingPosition : mappingPosition.ArmSecondMappingPosition;

            // Mapping positions are stored in 1..400 validity range and not in 0..399
            // If we want the 0 based value, we have to get value minus 1 (x - 1)
            return is0BasedIndexing
                ? position - 1
                : position;
        }

        #endregion IMappingPositionConverter
    }
}
