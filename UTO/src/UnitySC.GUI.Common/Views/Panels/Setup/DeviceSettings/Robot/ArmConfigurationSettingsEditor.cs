using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;
using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Robot
{
    public class ArmConfigurationSettingsEditor : Notifier
    {
        #region Constructor

        static ArmConfigurationSettingsEditor()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(RobotSettingsResources)));
        }

        public ArmConfigurationSettingsEditor()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public ArmConfigurationSettingsEditor(ArmConfiguration armConfig)
        {
            ArmConfig = armConfig;
            SampleDimensionsIsSelectedFunc = SampleDimensionsIsSelected;
        }

        #endregion

        #region Properties

        public ArmConfiguration ArmConfig { get; }

        public bool IsArmEnabled
        {
            get => ArmConfig?.IsEnabled ?? false;
            set
            {
                ArmConfig.IsEnabled = value;
                OnPropertyChanged();
            }
        }

        public EffectorType EffectorType
        {
            get => ArmConfig?.EffectorType ?? EffectorType.None;
            set
            {
                ArmConfig.EffectorType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SubstrateType> SupportedSubstrateTypes
        {
            get
                => ArmConfig != null
                    ? new ObservableCollection<SubstrateType>(ArmConfig.SupportedSubstrateTypes)
                    : new ObservableCollection<SubstrateType>();
            set
            {
                ArmConfig.SupportedSubstrateTypes = value.ToList();
                OnPropertyChanged();
            }
        }

        #region Update Supported Sample Dimensions

        private DelegateCommand<SampleDimension?> _selectSampleDimension;
        public DelegateCommand<SampleDimension?> SelectSampleDimensionCommand
        {
            get
                => _selectSampleDimension
                   ??= new DelegateCommand<SampleDimension?>(SelectSampleDimensionCommandExecute);
        }

        private void SelectSampleDimensionCommandExecute(SampleDimension? obj)
        {
            if (!obj.HasValue)
            {
                return;
            }

            if (!ArmConfig.SupportedSubstrateSizes.Contains(obj.Value))
            {
                ArmConfig.SupportedSubstrateSizes.Add(obj.Value);
            }
            else
            {
                ArmConfig.SupportedSubstrateSizes.Remove(obj.Value);
            }
            SupportedDimensionsChangedFlag = !SupportedDimensionsChangedFlag;
        }

        private bool _supportedDimensionsChangedFlag;
        public bool SupportedDimensionsChangedFlag
        {
            get => _supportedDimensionsChangedFlag;
            set => SetAndRaiseIfChanged(ref _supportedDimensionsChangedFlag, value);
        }

        public Func<SampleDimension?, bool, bool> SampleDimensionsIsSelectedFunc { get; }

        public bool SampleDimensionsIsSelected(SampleDimension? dimension, bool _)
        {
            return dimension.HasValue && ArmConfig.SupportedSubstrateSizes.Contains(dimension.Value);
        }

        #endregion

        #endregion
    }
}
