using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.CommandConstants;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status
{
    public class CarrierTypeStatus: Equipment.Abstractions.Drivers.Common.Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CarrierTypeStatus"/> class.
        /// <param name="other">Create a deep copy of <see cref="CarrierTypeStatus"/> instance</param>
        /// </summary>
        public CarrierTypeStatus(CarrierTypeStatus other)
        {
            Set(other);
        }

        public CarrierTypeStatus(string messageStatusData)
        {
            var status = messageStatusData.Replace(":", string.Empty);

            var carrierType  = CarrierTypeConstants.ToCarrierTypeAndIndex(status);
            CarrierType      = carrierType.Item1;
            CarrierTypeIndex = carrierType.Item2;
            CarrierProfileName = status;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The type of the carrier.
        /// </summary>
        public CarrierType CarrierType { get; internal set; }

        /// <summary>
        /// The profile name of the carrier.
        /// </summary>
        public string CarrierProfileName { get; internal set; }

        /// <summary>
        /// The index of the carrier type, if any.
        /// There are several configuration for each carrier type. The index allows to retrieve the right one in configuration.
        /// Value is null when carrier is not identified or when detection mode is automatic.
        /// </summary>
        public uint? CarrierTypeIndex { get; internal set; }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(CarrierTypeStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    CarrierType      = CarrierType.NotIdentified;
                    CarrierTypeIndex = null;
                    CarrierProfileName = string.Empty;
                }
                else
                {
                    CarrierType      = other.CarrierType;
                    CarrierTypeIndex = other.CarrierTypeIndex;
                    CarrierProfileName = other.CarrierProfileName;
                }
            }
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new CarrierTypeStatus(this);
        }

        #endregion Status Override
    }
}
