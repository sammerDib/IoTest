using System;
using System.Collections.Generic;

using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses
{
    /// <summary>
    /// Aims to provide speed of all system FANS.
    /// </summary>
    public class FansRotationSpeed : Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FansRotationSpeed"/> class.
        /// <param name="other">Create a deep copy of <see cref="FansRotationSpeed"/> instance</param>
        /// </summary>
        public FansRotationSpeed(FansRotationSpeed other)
        {
            Set(other);
        }

        public FansRotationSpeed(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');
            FansSpeed = new Dictionary<uint, int>(statuses.Length);

            foreach (var fan in statuses)
            {
                var splitFanData = fan.Split('|');

                if (splitFanData.Length != 2)
                {
                    throw new ArgumentException(
                        $"{nameof(FansRotationSpeed)} - At least one of the given FAN data is not valid.\n"
                        + $"FAN data: {fan}");
                }

                if (!uint.TryParse(splitFanData[0], out uint fanId))
                {
                    throw new ArgumentException(
                        $"{nameof(FansRotationSpeed)} - At least one of the given FAN data has not a valid ID.\n"
                        + $"FAN ID: {splitFanData[0]}");
                }

                if (!int.TryParse(splitFanData[1], out int rotationSpeed))
                {
                    throw new ArgumentException(
                        $"{nameof(FansRotationSpeed)} - At least one of the given FAN data has not a valid speed.\n"
                        + $"FAN ID: {splitFanData[0]}; FAN speed:{splitFanData[1]}");
                }

                FansSpeed.Add(fanId, rotationSpeed);
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Associate a rotation speed (rpm) to each FAN.
        /// </summary>
        public Dictionary<uint, int> FansSpeed { get; private set; }

        #endregion Properties+

        #region Overrides

        /// <summary>
        /// Create an object that it is a DEEP copy of current instance
        /// <returns>new object instance that it is a DEEP copy</returns>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new FansRotationSpeed(this);
        }

        #endregion Overrides

        #region Internal Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        internal void Set(FansRotationSpeed other = null)
        {
            lock (this)
            {
                FansSpeed = other == null
                    ? new Dictionary<uint, int>()
                    : new Dictionary<uint, int>(other.FansSpeed);
            }
        }

        #endregion Internal Methods
    }
}
