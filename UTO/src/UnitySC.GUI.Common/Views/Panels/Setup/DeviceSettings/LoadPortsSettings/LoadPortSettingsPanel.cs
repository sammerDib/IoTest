using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LoadPortsSettings
{
    public abstract class LoadPortSettingsPanel<T> : DeviceSettingsPanel<T>
        where T : LoadPortConfiguration, new()
    {
        #region Fields

        private readonly LoadPort _loadPort;

        private readonly UserMessage _initMessage = new(
            MessageLevel.Warning,
            new LocalizableText(nameof(LoadPortsSettingsPanelResources.S_SETUP_LOADPORTS_MESSAGE_NEED_INIT)));
        #endregion

        #region Constructors

        static LoadPortSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(LoadPortsSettingsPanelResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the
        /// <see>
        /// <cref>LoadPortSettingsPanel</cref>
        /// </see>
        /// class.
        /// </summary>
        /// <param name="loadPort">The load port</param>
        /// <param name="id">
        /// Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a
        /// localizable resource.
        /// </param>
        /// <param name="icon">
        /// Optional parameter used to define the representation of the panel inside the application.
        /// </param>
        protected LoadPortSettingsPanel(LoadPort loadPort, string id, IIcon icon = null)
            : base(id, icon)
        {
            _loadPort = loadPort;
        }

        #endregion

        #region Properties

        public CassetteType CassetteType
        {
            get => ModifiedConfig?.CassetteType ?? CassetteType.None;
            set
            {
                ModifiedConfig.CassetteType = value;
                OnPropertyChanged();
            }
        }

        public bool IsCarrierIdSupported
        {
            get => ModifiedConfig?.IsCarrierIdSupported ?? false;
            set
            {
                ModifiedConfig.IsCarrierIdSupported = value;
                OnPropertyChanged();
            }
        }

        public bool IsE84Enabled
        {
            get => ModifiedConfig?.IsE84Enabled ?? false;
            set
            {
                ModifiedConfig.IsE84Enabled = value;
                OnPropertyChanged();
            }
        }

        public int Tp1
        {
            get => ModifiedConfig?.E84Configuration.Tp1 ?? 0;
            set
            {
                ModifiedConfig.E84Configuration.Tp1 = value;
                OnPropertyChanged();
            }
        }

        public int Tp2
        {
            get => ModifiedConfig?.E84Configuration.Tp2 ?? 0;
            set
            {
                ModifiedConfig.E84Configuration.Tp2 = value;
                OnPropertyChanged();
            }
        }

        public int Tp3
        {
            get => ModifiedConfig?.E84Configuration.Tp3 ?? 0;
            set
            {
                ModifiedConfig.E84Configuration.Tp3 = value;
                OnPropertyChanged();
            }
        }

        public int Tp4
        {
            get => ModifiedConfig?.E84Configuration.Tp4 ?? 0;
            set
            {
                ModifiedConfig.E84Configuration.Tp4 = value;
                OnPropertyChanged();
            }
        }

        public int Tp5
        {
            get => ModifiedConfig?.E84Configuration.Tp5 ?? 0;
            set
            {
                ModifiedConfig.E84Configuration.Tp5 = value;
                OnPropertyChanged();
            }
        }

        public bool IsMappingSupported
        {
            get => ModifiedConfig?.IsMappingSupported ?? false;
            set
            {
                ModifiedConfig.IsMappingSupported = value;
                OnPropertyChanged();
            }
        }

        public bool IsInService
        {
            get => ModifiedConfig?.IsInService ?? false;
            set
            {
                ModifiedConfig.IsInService = value;
                OnPropertyChanged();
            }
        }

        public HandOffType HandOffType
        {
            get => ModifiedConfig?.HandOffType ?? HandOffType.Manual;
            set
            {
                ModifiedConfig.HandOffType = value;
                OnPropertyChanged();
            }
        }

        public byte AutoHandOffTimeout
        {
            get => ModifiedConfig?.AutoHandOffTimeout ?? 0;
            set
            {
                ModifiedConfig.AutoHandOffTimeout = value;
                OnPropertyChanged();
            }
        }

        public bool CloseDoorAfterRobotAction
        {
            get => ModifiedConfig?.CloseDoorAfterRobotAction ?? false;
            set
            {
                ModifiedConfig.CloseDoorAfterRobotAction = value;
                OnPropertyChanged();
            }
        }

        #region Carrier Indentification Configuration

        public bool ByPassReadId
        {
            get => ModifiedConfig?.CarrierIdentificationConfig?.ByPassReadId ?? false;
            set
            {
                ModifiedConfig.CarrierIdentificationConfig.ByPassReadId = value;
                OnPropertyChanged();
            }
        }

        public CarrierIDAcquisitionType CarrierIdAcquisition
        {
            get
                => ModifiedConfig?.CarrierIdentificationConfig?.CarrierIdAcquisition
                   ?? CarrierIDAcquisitionType.Generate;
            set
            {
                ModifiedConfig.CarrierIdentificationConfig.CarrierIdAcquisition = value;
                OnPropertyChanged();
            }
        }

        public CarrierTagLocation CarrierTagLocation
        {
            get
                => ModifiedConfig?.CarrierIdentificationConfig?.CarrierTagLocation
                   ?? CarrierTagLocation.Clamped;
            set
            {
                ModifiedConfig.CarrierIdentificationConfig.CarrierTagLocation = value;
                OnPropertyChanged();
            }
        }

        public string DefaultCarrierId
        {
            get => ModifiedConfig?.CarrierIdentificationConfig?.DefaultCarrierId ?? "Default";
            set
            {
                ModifiedConfig.CarrierIdentificationConfig.DefaultCarrierId = value;
                OnPropertyChanged();
            }
        }

        public byte MaxNumberOfRetry
        {
            get => ModifiedConfig?.CarrierIdentificationConfig?.MaxNumberOfRetry ?? 0;
            set
            {
                ModifiedConfig.CarrierIdentificationConfig.MaxNumberOfRetry = value;
                OnPropertyChanged();
            }
        }

        public int CarrierIdStartIndex
        {
            get => ModifiedConfig?.CarrierIdentificationConfig.CarrierIdStartIndex ?? 0;
            set
            {
                ModifiedConfig.CarrierIdentificationConfig.CarrierIdStartIndex = value;
                OnPropertyChanged();
            }
        }

        public int CarrierIdStopIndex
        {
            get => ModifiedConfig?.CarrierIdentificationConfig.CarrierIdStopIndex ?? 0;
            set
            {
                ModifiedConfig.CarrierIdentificationConfig.CarrierIdStopIndex = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public DataTableSource<CarrierType> DataTableCarrierTypes { get; } = new();

        private void UpdateDataTable()
        {
            DataTableCarrierTypes.Reset(ModifiedConfig.CarrierTypes ?? new List<CarrierType>(16));
        }

        public List<string> AvailableProfiles { get; private set; } = new();

        public bool IsManualCarrierTypeEnabled
        {
            get => ModifiedConfig?.IsManualCarrierType ?? false;
            set
            {
                ModifiedConfig.IsManualCarrierType = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Virtual

        protected virtual bool ChangesNeedInit()
        {
            return ObjectAreEquals(
                       ModifiedConfig.CarrierIdentificationConfig.CarrierIdAcquisition,
                       CurrentConfig.CarrierIdentificationConfig.CarrierIdAcquisition)
                   && ObjectAreEquals(
                       ModifiedConfig.IsE84Enabled,
                       CurrentConfig.IsE84Enabled)
                   && ObjectAreEquals(
                       ModifiedConfig.E84Configuration.Tp1,
                       CurrentConfig.E84Configuration.Tp1)
                   && ObjectAreEquals(
                       ModifiedConfig.E84Configuration.Tp2,
                       CurrentConfig.E84Configuration.Tp2)
                   && ObjectAreEquals(
                       ModifiedConfig.E84Configuration.Tp3,
                       CurrentConfig.E84Configuration.Tp3)
                   && ObjectAreEquals(
                       ModifiedConfig.E84Configuration.Tp4,
                       CurrentConfig.E84Configuration.Tp4)
                   && ObjectAreEquals(
                       ModifiedConfig.E84Configuration.Tp5,
                       CurrentConfig.E84Configuration.Tp5);
        }

        #endregion

        #region Override

        protected override void LoadEquipmentConfig()
        {
            OnPropertyChanged(nameof(CassetteType));
            OnPropertyChanged(nameof(IsCarrierIdSupported));
            OnPropertyChanged(nameof(IsE84Enabled));
            OnPropertyChanged(nameof(Tp1));
            OnPropertyChanged(nameof(Tp2));
            OnPropertyChanged(nameof(Tp3));
            OnPropertyChanged(nameof(Tp4));
            OnPropertyChanged(nameof(Tp5));
            OnPropertyChanged(nameof(IsMappingSupported));
            OnPropertyChanged(nameof(IsInService));
            OnPropertyChanged(nameof(HandOffType));
            OnPropertyChanged(nameof(AutoHandOffTimeout));
            OnPropertyChanged(nameof(ByPassReadId));
            OnPropertyChanged(nameof(CarrierIdAcquisition));
            OnPropertyChanged(nameof(CarrierTagLocation));
            OnPropertyChanged(nameof(DefaultCarrierId));
            OnPropertyChanged(nameof(MaxNumberOfRetry));
            OnPropertyChanged(nameof(CarrierIdStartIndex));
            OnPropertyChanged(nameof(CarrierIdStopIndex));
            OnPropertyChanged(nameof(IsManualCarrierTypeEnabled));
            OnPropertyChanged(nameof(CloseDoorAfterRobotAction));
            UpdateDataTable();
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();
            UpdateDataTable();
        }

        public override void OnHide()
        {
            base.OnHide();

            DataTableCarrierTypes.Reset(Enumerable.Empty<CarrierType>());
        }

        public override void OnShow()
        {
            base.OnShow();

            UpdateDataTable();
            AvailableProfiles.Clear();
            AvailableProfiles.Add(string.Empty);
            foreach (var carrierType in _loadPort.AvailableCarrierTypes)
            {
                AvailableProfiles.Add(carrierType);
            }

            foreach (var type in ModifiedConfig.CarrierTypes.Select(c=>c.Name))
            {
                if (!AvailableProfiles.Contains(type))
                {
                    AvailableProfiles.Add(type);
                }
            }

            OnPropertyChanged(nameof(AvailableProfiles));
        }

        protected override void SaveConfig()
        {
            base.SaveConfig();

            App.Instance.UserInterface.Messages.HideAll();

            if (!ChangesNeedInit())
            {
                return;
            }

            App.Instance.UserInterface.Messages.Show(_initMessage);
            _loadPort.CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
        }

        #endregion

        #region Event handler

        private void LoadPort_CommandExecutionStateChanged(
            object sender,
            Agileo.EquipmentModeling.CommandExecutionEventArgs e)
        {
            if (e.Execution.Context.Command.Name != nameof(IGenericDevice.Initialize))
            {
                return;
            }

            App.Instance.UserInterface.Messages.Hide(_initMessage);
            _loadPort.CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
        }

        #endregion
    }
}
