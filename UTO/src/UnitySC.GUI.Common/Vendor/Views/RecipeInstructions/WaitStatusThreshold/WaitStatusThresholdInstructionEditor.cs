using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;
using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.EquipmentTree;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitStatusThreshold.ViewModels;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitStatusThreshold.ViewModels.Operand;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitStatusThreshold.ViewModels.Operators;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitStatusThreshold
{
    public class WaitStatusThresholdInstructionEditor : InstructionEditorViewModel<WaitStatusThresholdInstruction>
    {
        #region Constructors

        static WaitStatusThresholdInstructionEditor()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ScenarioResources)));
        }

        public WaitStatusThresholdInstructionEditor() : base(null)
        {
            var newThreshold = new QuantityThresholdOperand
            {
                StatusName = "Status name",
                DeviceName = "Device Name",
                Type = "Temperature",
                Value = "24.5",
                WaitingOperator = WaitingOperator.GreaterThan,
                Quantity = new Temperature(24.5, TemperatureUnit.DegreeCelsius)
            };

            Thresholds.Add(new QuantityThresholdOperandViewModel(newThreshold));
            Thresholds.Add(new ThresholdOperatorViewModel(new ThresholdOperator { Operator = LogicalOperator.And }));
            Thresholds.Add(new QuantityThresholdOperandViewModel(newThreshold));
            Thresholds.Add(new QuantityThresholdOperandViewModel(newThreshold));
        }

        public WaitStatusThresholdInstructionEditor(WaitStatusThresholdInstruction instructionModel) : base(
            instructionModel)
        {
            EquipmentTree = new EquipmentTree(OnSelectedDeviceChanged, IsAvailableDevice);
            Thresholds = new ObservableCollection<IThresholdViewModel>(BuildThresholdViewModels(Model.Thresholds));
        }

        private IEnumerable<IThresholdViewModel> BuildThresholdViewModels(IEnumerable<Threshold> thresholds)
        {
            return thresholds.Select(NewThresholdViewModel);
        }

        private IThresholdViewModel NewThresholdViewModel(Threshold thresholdModel)
        {
            switch (thresholdModel)
            {
                case ThresholdOperator thresholdOperator:
                    return new ThresholdOperatorViewModel(thresholdOperator);
                case QuantityThresholdOperand quantityThresholdOperand:
                    return new QuantityThresholdOperandViewModel(quantityThresholdOperand);
                case EnumerableThresholdOperand enumerableThresholdOperand:
                    return new EnumerableThresholdOperandViewModel(enumerableThresholdOperand);
                case BooleanThresholdOperand booleanThresholdOperand:
                    return new BooleanThresholdOperandViewModel(booleanThresholdOperand);
                case NumericThresholdOperand numericThresholdOperand:
                    return new NumericThresholdOperandViewModel(numericThresholdOperand);
                case StringThresholdOperand stringThresholdOperand:
                    return new StringThresholdOperandViewModel(stringThresholdOperand);
                default:
                    return null;
            }
        }

        private bool IsAvailableDevice(Device arg)
        {
            return arg.DeviceType.Statuses.Count != 0;
        }

        private void OnSelectedDeviceChanged(Device device)
        {
            SelectedDevice = device == null ? null : new DeviceThresholdViewModel(device);
        }

        private DeviceThresholdViewModel _selectedDevice;

        public DeviceThresholdViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set { SetAndRaiseIfChanged(ref _selectedDevice, value); }
        }

        #endregion Constructors

        #region Properties

        public EquipmentTree EquipmentTree { get; }

        public ObservableCollection<IThresholdViewModel> Thresholds { get; set; } =
            new ObservableCollection<IThresholdViewModel>();

        public double TimeOut
        {
            get { return Model.TimeOut.Seconds; }
            set
            {
                Model.TimeOut = Duration.FromSeconds(value);
                OnPropertyChanged(nameof(TimeOut));
                UpdateLabel();
            }
        }

        public bool IsTimeoutEnabled
        {
            get
            {
                return Model.IsTimeoutEnabled;
            }
            set
            {
                Model.IsTimeoutEnabled = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region RemoveThresholdCommand

        private ICommand _removeCommand;

        public ICommand RemoveCommand =>
            _removeCommand
            ?? (_removeCommand =
                new DelegateCommand<IThresholdViewModel>(RemoveCommandExecute, RemoveCommandCanExecute));

        private bool RemoveCommandCanExecute(IThresholdViewModel threshold)
        {
            return threshold != null;
        }

        private void RemoveCommandExecute(IThresholdViewModel threshold)
        {
            var index = Thresholds.IndexOf(threshold);
            Thresholds.Remove(threshold);

            var operatorToRemoveIndex = index - 1;

            if (Thresholds.Count > 0)
            {
                if (operatorToRemoveIndex >= 0 && Thresholds.ElementAt(index - 1) is ThresholdOperatorViewModel)
                {
                    Thresholds.RemoveAt(operatorToRemoveIndex);
                }

                if (Thresholds.ElementAt(0) is ThresholdOperatorViewModel)
                {
                    Thresholds.RemoveAt(0);
                }
            }

            UpdateModel();
        }

        #endregion

        #region Add Status Command

        private ICommand _addStatusCommand;

        public ICommand AddStatusCommand =>
            _addStatusCommand
            ?? (_addStatusCommand =
                new DelegateCommand<DeviceStatus>(AddStatusCommandExecute, AddStatusCommandCanExecute));

        private bool AddStatusCommandCanExecute(DeviceStatus arg)
        {
            return arg != null && Thresholds.OfType<BooleanThresholdOperandViewModel>().Count() < 3;
        }

        #endregion

        private void AddStatusCommandExecute(DeviceStatus status)
        {
            if (!(status.Type is CSharpType statusType))
            {
                return;
            }

            var platformType = statusType.PlatformType;
            
            if (Thresholds.Count % 2 == 1)
            {
                Thresholds.Add(new ThresholdOperatorViewModel(new ThresholdOperator()));
            }

            // Boolean
            if (platformType.IsPrimitive && platformType == typeof(bool))
            {
                Thresholds.Add(new BooleanThresholdOperandViewModel(new BooleanThresholdOperand
                {
                    StatusName = status.Name,
                    DeviceName = SelectedDevice.Device.Name,
                    WaitingOperator = WaitingOperator.Equals,
                    Value = true,
                    Type = ((CSharpType)status.Type)?.PlatformType.ToString()
                }));
            }

            // Enum
            else if (platformType.IsEnum)
            {
                Thresholds.Add(new EnumerableThresholdOperandViewModel(new EnumerableThresholdOperand(status, SelectedDevice.Device.Name)));
            }

            // String
            else if (platformType == typeof(string))
            {
                Thresholds.Add(new StringThresholdOperandViewModel(new StringThresholdOperand(status, SelectedDevice.Device.Name)));
            }

            // Numeric
            else if (platformType.IsPrimitive && platformType != typeof(char))
            {
                Thresholds.Add(new NumericThresholdOperandViewModel(new NumericThresholdOperand(status, SelectedDevice.Device.Name)));
            }

            // IQuantity
            else if (status.Unit != null)
            {
                var unitType = status.Unit.GetType();
                var quantityInfo = Quantity.Infos.Single(i => i.UnitType == unitType);
                var valueType = quantityInfo.ValueType;

                var defaultUnitIndex = quantityInfo.UnitInfos.Select(x => x.Name).ToList().IndexOf(status.Unit.ToString());
                var abbreviation = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(unitType, defaultUnitIndex + 1, CultureInfo.InvariantCulture);
                var value = Quantity.Parse(CultureInfo.InvariantCulture, valueType, "0" + abbreviation);

                Thresholds.Add(new QuantityThresholdOperandViewModel(new QuantityThresholdOperand
                {
                    StatusName = status.Name,
                    DeviceName = SelectedDevice.Device.Name,
                    WaitingOperator = WaitingOperator.GreaterThan,
                    Quantity = value
                }));
            }
            else
            {
                Thresholds.Add(new QuantityThresholdOperandViewModel(new QuantityThresholdOperand
                {
                    StatusName = status.Name,
                    DeviceName = SelectedDevice.Device.Name,
                    WaitingOperator = WaitingOperator.GreaterThan,
                    Quantity = Quantity.Parse(CultureInfo.InvariantCulture, statusType.DefaultValue.GetType(), statusType.DefaultValue.ToString())
                }));
            }

            UpdateModel();
        }

        private void UpdateModel()
        {
            Model.Thresholds = Thresholds.Select(thresholdViewModel => thresholdViewModel.Model).ToList();
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            OnPropertyChanged(nameof(PrettyLabel));
            OnPropertyChanged(nameof(FormattedLabel));
        }

        public string PrettyLabel => Model.PrettyLabel;

        public List<AdvancedStringFormatDefinition> FormattedLabel => Model.FormattedLabel;
    }

    public class DeviceStatusThresholdGroupViewModel : Notifier
    {
        public string UnitName { get; }

        public List<DeviceStatus> Statuses { get; } = new List<DeviceStatus>();

        public DeviceStatusThresholdGroupViewModel(string unitName)
        {
            UnitName = unitName;
        }
    }

    public class DeviceThresholdViewModel : Notifier
    {
        public Device Device { get; }

        public DeviceThresholdViewModel(Device device)
        {
            Device = device;
            foreach (var status in Device.DeviceType.AllStatuses())
            {
                AddStatusToGroups(status);
            }
        }

        private void AddStatusToGroups(DeviceStatus status)
        {
            var groupName = status.Unit == null ? "Other" : status.Unit.GetType().Name;
            var group = StatusGroups.SingleOrDefault(g => g.UnitName == groupName);
            if (group == null)
            {
                group = new DeviceStatusThresholdGroupViewModel(groupName);
                StatusGroups.Add(group);
            }

            group.Statuses.Add(status);
        }

        public List<DeviceStatusThresholdGroupViewModel> StatusGroups { get; } =
            new List<DeviceStatusThresholdGroupViewModel>();
    }
}
