using System.Linq;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    public class StringParameterViewModel : ParameterViewModel
    {
        public StringParameterViewModel(Agileo.EquipmentModeling.Parameter p, DeviceCommandViewModel deviceCommandViewModel)
            : base(p, deviceCommandViewModel, typeof(string))
        {
            Rules.Add(new DelegateRule(nameof(Value),
                () => Value.ToString().Any(c => !char.IsLetterOrDigit(c)) ? "Invalid name: containing special characters" : null));
        }
    }
}
