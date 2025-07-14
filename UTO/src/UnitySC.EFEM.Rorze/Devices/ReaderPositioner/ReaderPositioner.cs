using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.EFEM.Rorze.Devices.ReaderPositioner
{
    public partial class ReaderPositioner
    {
        #region Fields

        private IReaderPositionerIos IoModule { get; set; }

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    var ioModule = this.GetTopDeviceContainer()
                        .AllDevices()
                        .FirstOrDefault(d => d is IReaderPositionerIos);
                    if (ioModule is not IReaderPositionerIos readerPositionerIos)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(IReaderPositionerIos)} is not found in equipment model tree.");
                    }

                    IoModule = readerPositionerIos;
                    IoModule.StatusValueChanged += IoModule_StatusValueChanged;
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            IoModule.StartCommunication();
        }

        protected override void InternalStopCommunication()
        {
            IoModule.StopCommunication();
        }

        #endregion

        #region Commands

        protected override void InternalSetPosition(SampleDimension dimension)
        {
            try
            {
                IoModule.SetReaderPosition(dimension);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region Event Handlers

        private void IoModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                case nameof(IReaderPositionerIos.I_OCRWaferReaderLimitSensor1):
                case nameof(IReaderPositionerIos.I_OCRWaferReaderLimitSensor2):
                    if (IoModule.I_OCRWaferReaderLimitSensor1
                        && !IoModule.I_OCRWaferReaderLimitSensor2)
                    {
                        CurrentPosition = SampleDimension.S300mm;
                    }
                    else if (!IoModule.I_OCRWaferReaderLimitSensor1
                             && IoModule.I_OCRWaferReaderLimitSensor2)
                    {
                        CurrentPosition = SampleDimension.S200mm;
                    }
                    else
                    {
                        CurrentPosition = SampleDimension.NoDimension;
                    }

                    break;
                case nameof(IReaderPositionerIos.IsCommunicating):
                    IsCommunicating = IoModule.IsCommunicating;
                    break;
                case nameof(IReaderPositionerIos.IsCommunicationStarted):
                    IsCommunicationStarted = IoModule.IsCommunicationStarted;
                    break;
            }

            if (State is OperatingModes.Maintenance or OperatingModes.Idle)
            {
                SetState(
                    !IsCommunicating || IoModule.State == OperatingModes.Maintenance
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
        }

        #endregion Event Handlers

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing && IoModule != null)
            {
                IoModule.StatusValueChanged -= IoModule_StatusValueChanged;
                IoModule = null;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
