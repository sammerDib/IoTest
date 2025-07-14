using System;

using Agileo.EquipmentModeling;

using BAI.Systems.Devices.SubstrateIdReader;
using BAI.Systems.Modules.EFEM;

using UnitySC.EFEM.Brooks.Devices.SubstrateIdReader.BrooksSubstrateIdReader.Configuration;
using UnitySC.EFEM.Brooks.Devices.SubstrateIdReader.BrooksSubstrateIdReader.Resources;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EFEM.Brooks.Devices.SubstrateIdReader.BrooksSubstrateIdReader
{
    public partial class BrooksSubstrateIdReader
        : IConfigurableDevice<BrooksSubstrateIdReaderConfiguration>
    {
        #region Fields

        private SubstrateIdReaderProxy _substrateIdReaderProxy;
        private EfemProxy _efemProxy;

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

                        var device = _efemProxy.GetDevice(Configuration.BrooksReaderName);
                        if (device is not SubstrateIdReaderProxy substrateIdReaderProxy)
                        {
                            throw new InvalidOperationException(
                                Messages.ReaderNotPresentInEfemConfig);
                        }

                        _substrateIdReaderProxy = substrateIdReaderProxy;
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region IConfigurableDevice

        public new BrooksSubstrateIdReaderConfiguration Configuration
            => ConfigurationExtension.Cast<BrooksSubstrateIdReaderConfiguration>(base.Configuration);

        public BrooksSubstrateIdReaderConfiguration CreateDefaultConfiguration()
        {
            return new BrooksSubstrateIdReaderConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(SubstrateIdReader)}/{nameof(BrooksSubstrateIdReader)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            try
            {
                if (!_substrateIdReaderProxy.Connected)
                {
                    _substrateIdReaderProxy.ConnectSubstrateIdReader();
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
                if (_substrateIdReaderProxy.Connected)
                {
                    _substrateIdReaderProxy.DisconnectSubstrateIdReader();
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

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                //Base init
                base.InternalInitialize(mustForceInit);

                //Device init
                _substrateIdReaderProxy.Initialize();

                //Status update
                //

                //Check device ready
                if (!_substrateIdReaderProxy.IsOperable())
                {
                    throw new InvalidOperationException(Messages.ReaderNotOperable);
                }

                if (_substrateIdReaderProxy.IsInMaintenance())
                {
                    throw new InvalidOperationException(Messages.ReaderInMaintenance);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region ISubstrateIdReader Commands

        protected override void InternalRequestRecipes()
        {
            try
            {
                //TODO check params
                var param = _substrateIdReaderProxy.GetIdReaderConfigParamNames();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalRead(string recipeName)
        {
            try
            {
                //TODO _substrateIdReaderProxy.SetIdReaderConfigParam();
                SubstrateId = _substrateIdReaderProxy.ReadSubstrateId();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGetImage(string imagePath)
        {
            throw new InvalidOperationException(Messages.CmdNotSupported);
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
                if (_substrateIdReaderProxy != null)
                {
                    _substrateIdReaderProxy.Dispose();
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
