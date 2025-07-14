using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions
{
    public interface IReaderPositionerIos
    {
        #region Properties

        bool I_OCRWaferReaderLimitSensor1 { get; }

        bool I_OCRWaferReaderLimitSensor2 { get; }

        bool IsCommunicationStarted { get; }

        bool IsCommunicating { get; }

        OperatingModes State { get; }

        #endregion

        #region Methods

        void StartCommunication();

        void StopCommunication();

        void SetReaderPosition(SampleDimension dimension);

        #endregion
        #region Events

        event EventHandler<StatusChangedEventArgs> StatusValueChanged;

        #endregion
    }
}
