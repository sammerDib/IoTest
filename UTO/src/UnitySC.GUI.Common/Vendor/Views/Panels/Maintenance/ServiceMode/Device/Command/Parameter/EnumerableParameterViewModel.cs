using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    public class EnumerableParameterViewModel : ParameterViewModel
    {
        public EnumerableParameterViewModel(Agileo.EquipmentModeling.Parameter p, DeviceCommandViewModel deviceCommandViewModel)
            : base(p, deviceCommandViewModel, ((CSharpType)p.Type).PlatformType)
        {
            foreach (IComparable item in Enum.GetValues(Type))
            {
                Literals.Add(item);
            }

            Rules.Add(new DelegateRule(nameof(Value), () => Literals.Any(x => Equals(x, TypedValue)) ? null : "Invalid Data"));
        }

        public List<IComparable> Literals { get; } = new();

        public IComparable TypedValue
        {
            get => Value as IComparable;
            set => Value = value;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Value))
            {
                base.OnPropertyChanged(nameof(TypedValue));
            }
        }
    }
}
