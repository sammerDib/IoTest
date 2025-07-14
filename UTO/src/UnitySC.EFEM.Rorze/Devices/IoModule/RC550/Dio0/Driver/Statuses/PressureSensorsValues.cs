using System;
using System.Collections.Generic;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses
{
    /// <summary>
    /// Aims to provide pressure of all system pressure sensors.
    /// </summary>
    public class PressureSensorsValues : Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PressureSensorsValues"/> class.
        /// <param name="other">Create a deep copy of <see cref="PressureSensorsValues"/> instance</param>
        /// </summary>
        public PressureSensorsValues(PressureSensorsValues other)
        {
            Set(other);
        }

        public PressureSensorsValues(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');
            PressureValuesPerSensor = new Dictionary<uint, Pressure>(statuses.Length);

            foreach (var pressureSensor in statuses)
            {
                var splitSensorData = pressureSensor.Split('|');

                if (splitSensorData.Length != 2)
                {
                    throw new ArgumentException(
                        $"{nameof(PressureSensorsValues)} - At least one of the given pressure sensor data is not valid.\n"
                        + $"Pressure sensor data: {pressureSensor}");
                }

                if (!uint.TryParse(splitSensorData[0], out uint sensorId))
                {
                    throw new ArgumentException(
                        $"{nameof(PressureSensorsValues)} - At least one of the given pressure sensor data has not a valid ID.\n"
                        + $"Pressure sensor ID: {splitSensorData[0]}");
                }

                if (!int.TryParse(splitSensorData[1], out int pressure))
                {
                    throw new ArgumentException(
                        $"{nameof(PressureSensorsValues)} - At least one of the given pressure sensor data has not a valid value.\n"
                        + $"Pressure sensor ID: {splitSensorData[0]}; Pressure sensor data: {splitSensorData[1]}");
                }

                // TODO: see if useful to manage units from captor types
                // For now, according to the logs, we can consider that sensors in presence are in mPa.
                // According to RC550 documentation, there are captors units per captor type:
                // - Differential pressure sensor:      [mPa]
                // - Vacuum pressure sensor:            [Pa]
                // - Compressed air pressure sensor:    [kPa]
                // - Others (SB080): Depends on a sensor connected to SB080.
                var pressureWithUnit = Pressure.FromMillipascals(pressure);

                PressureValuesPerSensor.Add(sensorId, pressureWithUnit);
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Associate a pressure value to each pressure sensor.
        /// </summary>
        public Dictionary<uint, Pressure> PressureValuesPerSensor { get; private set; }

        #endregion Properties+

        #region Overrides

        /// <summary>
        /// Create an object that it is a DEEP copy of current instance
        /// <returns>new object instance that it is a DEEP copy</returns>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new PressureSensorsValues(this);
        }

        #endregion Overrides

        #region Internal Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        internal void Set(PressureSensorsValues other = null)
        {
            lock (this)
            {
                PressureValuesPerSensor = other == null
                    ? new Dictionary<uint, Pressure>()
                    : new Dictionary<uint, Pressure>(other.PressureValuesPerSensor);
            }
        }

        #endregion Internal Methods
    }
}
