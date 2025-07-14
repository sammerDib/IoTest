using System;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    public abstract class ParameterViewModel : NotifyDataError
    {
        public Type Type { get; }

        public Agileo.EquipmentModeling.Parameter Parameter { get; }

        public DeviceCommandViewModel DeviceCommandViewModel { get; }

        protected ParameterViewModel(Agileo.EquipmentModeling.Parameter parameter, DeviceCommandViewModel deviceCommandViewModel, Type type)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            DeviceCommandViewModel = deviceCommandViewModel ?? throw new ArgumentNullException(nameof(deviceCommandViewModel));
            Type = type;

            // [TLA] Setup default value for DataType parameters
            if (parameter.Type is CSharpType dataType)
            {
                Value = parameter.Unit == null
                    ? dataType.DefaultValue
                    : Activator.CreateInstance(dataType.PlatformType, 0, parameter.Unit);
            }
        }

        public object Value
        {
            get => DeviceCommandViewModel.CommandParameters.TryGetValue(Parameter.Name, out var value) ? value : null;
            set
            {
                DeviceCommandViewModel.CommandParameters[Parameter.Name] = value;
                OnPropertyChanged();
            }
        }

    }
}
