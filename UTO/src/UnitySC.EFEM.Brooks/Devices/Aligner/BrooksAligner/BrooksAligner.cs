using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using BAI.Systems.Common;
using BAI.Systems.Devices.WaferAligner;
using BAI.Systems.Modules.EFEM;

using UnitsNet;

using UnitySC.EFEM.Brooks.Devices.Aligner.BrooksAligner.Configuration;
using UnitySC.EFEM.Brooks.Devices.Aligner.BrooksAligner.Resources;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

using WaferLocation = UnitySC.Equipment.Abstractions.Material.WaferLocation;

namespace UnitySC.EFEM.Brooks.Devices.Aligner.BrooksAligner
{
    public partial class BrooksAligner : IConfigurableDevice<BrooksAlignerConfiguration>
    {
        #region Fields

        private WaferAlignerRemoteProxy _waferAlignerProxy;
        private EfemProxy _efemProxy;
        private bool _waferHasBeenAligned;

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            BAI.CTC.ClientInit.ClientLibLoader.InitializeLoader();
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        _efemProxy = Helpers.Helper.GetEfemProxy(this);

                        var waferAligner = _efemProxy.GetDevice(Configuration.BrooksAlignerName);
                        if (waferAligner is not WaferAlignerRemoteProxy waferAlignerProxy)
                        {
                            throw new InvalidOperationException(
                                Messages.AlignerNotPresentInEfemConfig);
                        }

                        _waferAlignerProxy = waferAlignerProxy;
                        _waferAlignerProxy.AlarmGenerated += WaferAlignerProxy_AlarmGenerated;
                        Location.PropertyChanged += Location_PropertyChanged;
                    }

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
            try
            {
                if (!_waferAlignerProxy.Connected)
                {
                    _waferAlignerProxy.Connect();
                }

                IsCommunicationStarted = true;
                IsCommunicating = true;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStopCommunication()
        {
            try
            {
                if (_waferAlignerProxy.Connected)
                {
                    _waferAlignerProxy.Disconnect();
                }

                IsCommunicationStarted = false;
                IsCommunicating = false;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (ExecutionMode != ExecutionMode.Real)
            {
                return;
            }

            _waferAlignerProxy.StopMotion();
        }

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                _waferHasBeenAligned = false;

                //Base init
                base.InternalInitialize(mustForceInit);

                //Device init
                _waferAlignerProxy.Initialize();
                _waferAlignerProxy.HomeWaferAligner();

                //Status update
                GetWaferPresence();

                //Check device ready
                if (!_waferAlignerProxy.IsOperable())
                {
                    throw new InvalidOperationException(Messages.AlignerNotOperable);
                }

                if (_waferAlignerProxy.IsInMaintenance())
                {
                    throw new InvalidOperationException(Messages.AlignerInMaintenance);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IAligner Commands

        protected override void InternalAlign(Angle target, AlignType alignType)
        {
            try
            {
                var alignFeature = Helpers.Helper.ConvertMaterialTypeToWaferAlignFeature(Location.Wafer.MaterialType);
                _waferAlignerProxy.AlignWafer(alignFeature, target.Degrees);
                _waferHasBeenAligned = true;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalCentering()
        {
            throw new NotImplementedException();
        }

        protected override void InternalPrepareTransfer(
            EffectorType effector,
            SampleDimension dimension,
            MaterialType materialType)
        {
            if (Location.Wafer != null
                && !_waferHasBeenAligned)
            {
                InternalAlign(Angle.FromDegrees(0), AlignType.AlignWaferWithoutCheckingSubO_FlatLocation);
            }
        }

        protected override void InternalSetDateAndTime()
        {
            try
            {
                _efemProxy.SetControllerLocalTime(DateTime.Now);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalClamp()
        {
            try
            {
                _waferAlignerProxy.ApplyWaferRestraint(Configuration.BrooksChuckName);
                GetWaferPresence();
                UpdateClampState();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalUnclamp()
        {
            try
            {
                _waferAlignerProxy.ReleaseWaferRestraint(Configuration.BrooksChuckName);
                GetWaferPresence();
                UpdateClampState();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalMoveZAxis(bool isBottom)
        {
            throw new NotImplementedException();
        }

        public override void CheckSubstrateDetectionError(bool reset = false)
        {
            GetWaferPresence();
            base.CheckSubstrateDetectionError(reset);
        }

        #endregion

        #region IConfigurableDevice

        public new BrooksAlignerConfiguration Configuration
            => ConfigurationExtension.Cast<BrooksAlignerConfiguration>(base.Configuration);

        public BrooksAlignerConfiguration CreateDefaultConfiguration()
        {
            return new BrooksAlignerConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Aligner)}/{nameof(BrooksAligner)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region Event handlers

        private void WaferAlignerProxy_AlarmGenerated(
            [BAI.Internal.DeviceName] string source,
            AlarmLevel level,
            string message)
        {
            if (_waferAlignerProxy.IsInMaintenance() || !_waferAlignerProxy.IsOperable())
            {
                Interrupt(InterruptionKind.Abort);
            }
        }

        private void Location_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(WaferLocation.Material))
            {
                return;
            }

            if (Location.Wafer != null)
            {
                //When a new wafer arrives, restart the fact that it has been aligned
                _waferHasBeenAligned = false;
            }

            CheckSubstrateDetectionError();
        }

        #endregion

        #region Private

        private void GetWaferPresence()
        {
            var upperPresence =
                _waferAlignerProxy.MapWaferPresenceOnHost(Configuration.BrooksChuckName);
            WaferPresence = Helpers.Helper.ConvertPresenceStateToWaferPresence(upperPresence);
        }

        private void UpdateClampState()
        {
            IsClamped = _waferAlignerProxy.GetWaferRestraintState(Configuration.BrooksChuckName) == LockState.Locked;
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (_waferAlignerProxy != null)
                {
                    _waferAlignerProxy.AlarmGenerated -= WaferAlignerProxy_AlarmGenerated;
                    _waferAlignerProxy.Dispose();
                    Location.PropertyChanged -= Location_PropertyChanged;
                }

                if (_efemProxy != null)
                {
                    _efemProxy.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
