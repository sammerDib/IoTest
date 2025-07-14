using System;
using System.IO;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

using Path = AgilController.GUI.Configuration.Path;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AppPathConfig
{
    public class AppPathConfigPanel : SetupNodePanel<Path>
    {
        #region Constructors

        static AppPathConfigPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(AppPathConfigPanelResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SetupPanelResources)));
        }

        public AppPathConfigPanel()
        {
        }

        public AppPathConfigPanel(string id, IIcon icon = null)
            : base(id, icon)
        {
            Rules.Add(
                new DelegateRule(
                    nameof(AccessRightsConfigurationPath),
                    () =>
                    {
                        if (string.IsNullOrEmpty(AccessRightsConfigurationPath)
                            || !File.Exists(AccessRightsConfigurationPath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_ACCESS_RIGHT_PATH_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(AccessRightsSchemaPath),
                    () =>
                    {
                        if (string.IsNullOrEmpty(AccessRightsSchemaPath)
                            || !File.Exists(AccessRightsSchemaPath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_ACCESS_RIGHT_PATH_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(LocalizationPath),
                    () =>
                    {
                        if (string.IsNullOrEmpty(LocalizationPath)
                            || !File.Exists(LocalizationPath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_LOCALISATION_PATH_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(UserManualPath),
                    () =>
                    {
                        if (string.IsNullOrEmpty(UserManualPath)
                            || !File.Exists(UserManualPath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_MANUAL_PATH_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(AlarmAnalysisCapturePath),
                    () =>
                    {
                        if (string.IsNullOrEmpty(AlarmAnalysisCapturePath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_DIRECTORY_WARNING));
                        }

                        return string.Empty;
                    }));
        }

        #endregion Constructors

        #region Properties

        private ApplicationPath ModifiedApplicationPathConfig => ModifiedConfig?.ApplicationPath;

        public string UserManualPath
        {
            get => ModifiedApplicationPathConfig.UserManualPath;
            set
            {
                if (string.Equals(ModifiedApplicationPathConfig.UserManualPath, value, StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedApplicationPathConfig.UserManualPath = value;
                OnPropertyChanged();
            }
        }

        public string AccessRightsConfigurationPath
        {
            get => ModifiedConfigNode.AccessRightsConfigurationPath;
            set
            {
                if (string.Equals(ModifiedConfigNode.AccessRightsConfigurationPath, value, StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedConfigNode.AccessRightsConfigurationPath = value;
                OnPropertyChanged();
            }
        }

        public string AccessRightsSchemaPath
        {
            get => ModifiedConfigNode.AccessRightsSchemaPath;
            set
            {
                if (string.Equals(ModifiedConfigNode.AccessRightsSchemaPath, value, StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedConfigNode.AccessRightsSchemaPath = value;
                OnPropertyChanged();
            }
        }

        public string LocalizationPath
        {
            get => ModifiedConfigNode.LocalizationPath;
            set
            {
                if (string.Equals(ModifiedConfigNode.LocalizationPath, value, StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedConfigNode.LocalizationPath = value;
                OnPropertyChanged();
            }
        }

        public string AlarmAnalysisCapturePath
        {
            get => ModifiedApplicationPathConfig.AlarmAnalysisCaptureStoragePath;
            set
            {
                if (string.Equals(ModifiedApplicationPathConfig.AlarmAnalysisCaptureStoragePath, value, StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedApplicationPathConfig.AlarmAnalysisCaptureStoragePath = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Commands

        private DelegateCommand _defineUserManualPathCommand;

        public ICommand DefineUserManualPathCommand
            => _defineUserManualPathCommand ??= new DelegateCommand(DefineUserManualPathExecute);

        private void DefineUserManualPathExecute()
            => ShowOpenFileDialog<AppPathConfigPanel>(p => p.UserManualPath, "XPS files (*.xps)|*.xps");

        private DelegateCommand _defineAccessRightsPathCommand;

        public ICommand DefineAccessRightsPathCommand
            => _defineAccessRightsPathCommand ??= new DelegateCommand(DefineAccessRightsPathExecute);

        private void DefineAccessRightsPathExecute()
            => ShowOpenFileDialog<AppPathConfigPanel>(p => p.AccessRightsConfigurationPath, "XML files (*.xml)|*.xml");

        private DelegateCommand _defineAccessRightsSchemaPathCommand;

        public ICommand DefineAccessRightsSchemaPathCommand
            => _defineAccessRightsSchemaPathCommand ??= new DelegateCommand(DefineAccessRightsSchemaPathExecute);

        private void DefineAccessRightsSchemaPathExecute()
            => ShowOpenFileDialog<AppPathConfigPanel>(p => p.AccessRightsSchemaPath, "XSD files (*.xsd)|*.xsd");

        private DelegateCommand _defineLocalizationPathCommand;

        public ICommand DefineLocalizationPathCommand
            => _defineLocalizationPathCommand ??= new DelegateCommand(DefineLocalizationPathExecute);

        private void DefineLocalizationPathExecute()
            => ShowOpenFileDialog<AppPathConfigPanel>(p => p.LocalizationPath, "XML files (*.xml)|*.xml");

        private ICommand _defineAlarmAnalysisCapturePathCommand;

        public ICommand DefineAlarmAnalysisCapturePathCommand
            => _defineAlarmAnalysisCapturePathCommand ??= new DelegateCommand(DefineAlarmAnalysisCapturePathCommandExecute);

        private void DefineAlarmAnalysisCapturePathCommandExecute() => ShowOpenFolderDialog<AppPathConfigPanel>(p => p.AlarmAnalysisCapturePath);

        #endregion Commands

        #region Overrides

        protected override bool ConfigurationEqualsCurrent()
            => base.ConfigurationEqualsCurrent()
               && ObjectAreEquals(ModifiedApplicationPathConfig, CurrentConfig?.ApplicationPath);

        protected override bool ConfigurationEqualsLoaded()
            => base.ConfigurationEqualsLoaded()
               && ObjectAreEquals(ModifiedApplicationPathConfig, LoadedConfig?.ApplicationPath);

        protected override Path GetNode(ApplicationConfiguration applicationConfiguration)
            => applicationConfiguration?.Path;

        #endregion
    }
}
