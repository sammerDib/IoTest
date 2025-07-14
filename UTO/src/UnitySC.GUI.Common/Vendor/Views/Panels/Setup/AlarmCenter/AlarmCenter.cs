using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Agileo.AlarmModeling.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;

using Microsoft.Win32;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AlarmCenter
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel view
    /// </summary>
    public class AlarmCenter : SetupNodePanel<AlarmCenterConfiguration>
    {
        #region Fields

        private readonly LocalizableText _onMemoryStorageMode = new(nameof(AlarmCenterResources.ON_MEMORY));
        private readonly LocalizableText _onDiskStorageMode = new(nameof(AlarmCenterResources.ON_DISK));

        #endregion

        #region Constructors

        static AlarmCenter()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(AlarmCenterResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SetupPanelResources)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AlarmCenter.AlarmCenter" /> class.
        /// </summary>
        public AlarmCenter()
            : this("DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AlarmCenter.AlarmCenter" /> class.
        /// </summary>
        /// <param name="id">Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public AlarmCenter(string id, IIcon icon = null)
            : base(id, icon)
        {
            Rules.Add(
                new DelegateRule(
                    nameof(StepProviderClassId),
                    () => StepProviderClassId < 1
                        ? LocalizationManager.GetString(nameof(SetupPanelResources.SETUP_MESSAGE_STEP_WARNING))
                        : string.Empty));

            Rules.Add(
                new DelegateRule(
                    nameof(StepProviderInstance),
                    () => StepProviderInstance < 1
                        ? LocalizationManager.GetString(nameof(SetupPanelResources.SETUP_MESSAGE_STEP_WARNING))
                        : string.Empty));

            Rules.Add(new DelegateRule(nameof(SelectedStorageLocation),
                () =>
                {
                    if (!SelectedStorageMode.Equals(_onMemoryStorageMode.Key, StringComparison.Ordinal)
                        && !string.IsNullOrWhiteSpace(SelectedStorageLocation)
                        && !Directory.Exists(Path.GetDirectoryName(SelectedStorageLocation)))
                    {
                        return LocalizationManager.GetString(
                            nameof(SetupPanelResources.SETUP_MESSAGE_DIRECTORY_WARNING));
                    }

                    return string.Empty;
                }));
        }

        #endregion

        #region Commands

        private DelegateCommand _cmdDefinePath;

        public ICommand DefinePath => _cmdDefinePath ??= new DelegateCommand(CmdDefinePathExecute);

        private void CmdDefinePathExecute()
        {
            var openFileDialog = new OpenFileDialog { Filter = "*.sqlite3|*.sqlite3" };
            if (File.Exists(SelectedStorageLocation))
            {
                openFileDialog.InitialDirectory = Path.GetFullPath(SelectedStorageLocation)
                    .Replace(Path.GetFileName(SelectedStorageLocation), string.Empty);
            }

            if (openFileDialog.ShowDialog() == true && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                SelectedStorageLocation = openFileDialog.FileName;
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<LocalizableText> StorageLocations
            => new() { _onMemoryStorageMode, _onDiskStorageMode };

        private string _selectedStorageLocation;

        public string SelectedStorageLocation
        {
            get => _selectedStorageLocation;

            set
            {
                if (SetAndRaiseIfChanged(ref _selectedStorageLocation, value))
                {
                    ModifiedConfigNode.Storage.DbFullPath = value;
                }
            }
        }

        public bool DisableAlarmsTextLocalization
        {
            get => ModifiedConfigNode.DisableAlarmsTextLocalization;
            set
            {
                if (ModifiedConfigNode.DisableAlarmsTextLocalization == value)
                {
                    return;
                }

                ModifiedConfigNode.DisableAlarmsTextLocalization = value;
                OnPropertyChanged();
            }
        }

        public int StepProviderClassId
        {
            get => ModifiedConfigNode.StepProviderClassId;
            set
            {
                if (ModifiedConfigNode.StepProviderClassId == value)
                {
                    return;
                }

                ModifiedConfigNode.StepProviderClassId = value;
                OnPropertyChanged();
            }
        }

        public int StepProviderInstance
        {
            get => ModifiedConfigNode.StepProviderInstance;
            set
            {
                if (ModifiedConfigNode.StepProviderInstance == value)
                {
                    return;
                }

                ModifiedConfigNode.StepProviderInstance = value;
                OnPropertyChanged();
            }
        }

        private string _selectedStorageMode;

        public string SelectedStorageMode
        {
            get => _selectedStorageMode;
            set
            {
                if (!SetAndRaiseIfChanged(ref _selectedStorageMode, value))
                {
                    return;
                }

                ModifiedConfigNode.Storage.DbFullPath =
                    _selectedStorageMode?.Equals(_onMemoryStorageMode.Key, StringComparison.Ordinal) == true
                        ? AlarmStorageConfiguration.StorageMemoryMode
                        : SelectedStorageLocation;

                OnPropertyChanged(nameof(LocationTextBoxVisibility));
            }
        }

        public Visibility LocationTextBoxVisibility
            => SelectedStorageMode.Equals(_onMemoryStorageMode.Key, StringComparison.Ordinal)
                ? Visibility.Collapsed
                : Visibility.Visible;

        #endregion Properties

        #region Override

        protected override AlarmCenterConfiguration GetNode(ApplicationConfiguration applicationConfiguration)
            => applicationConfiguration?.Alarms;

        protected override bool ConfigurationNodeAreEquals(
            AlarmCenterConfiguration configNode1,
            AlarmCenterConfiguration configNode2)
            => configNode1.Storage.DbFullPath == configNode2.Storage.DbFullPath
               && configNode1.StepProviderClassId == configNode2.StepProviderClassId
               && configNode1.StepProviderInstance == configNode2.StepProviderInstance
               && configNode1.DisableAlarmsTextLocalization == configNode2.DisableAlarmsTextLocalization;

        protected override void UndoChanges()
        {
            base.UndoChanges();
            InitSelectedStorage();
        }

        public override void OnSetup()
        {
            base.OnSetup();
            InitSelectedStorage();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StorageLocations.Clear();
                _onDiskStorageMode?.Dispose();
                _onMemoryStorageMode?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitSelectedStorage()
        {
            if (ModifiedConfigNode.Storage.DbFullPath.Equals(
                    AlarmStorageConfiguration.StorageMemoryMode,
                    StringComparison.OrdinalIgnoreCase))
            {
                _selectedStorageMode = _onMemoryStorageMode.Key;
                _selectedStorageLocation = string.Empty;
            }
            else
            {
                _selectedStorageMode = _onDiskStorageMode.Key;
                _selectedStorageLocation = ModifiedConfigNode.Storage.DbFullPath;
            }

            OnPropertyChanged(nameof(SelectedStorageMode));
            OnPropertyChanged(nameof(LocationTextBoxVisibility));
            OnPropertyChanged(nameof(SelectedStorageLocation));
        }

        #endregion Override
    }
}
