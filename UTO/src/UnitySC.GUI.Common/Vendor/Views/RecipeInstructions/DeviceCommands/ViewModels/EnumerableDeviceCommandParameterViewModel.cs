using System;
using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels
{
    public class EnumerableDeviceCommandParameterViewModel : DeviceCommandParameterViewModel<EnumerableDeviceCommandParameter>
    {
        public EnumerableDeviceCommandParameterViewModel(EnumerableDeviceCommandParameter model)
            : base(model)
        {
            Literals = new ObservableCollection<IComparable>(EnumLoader.GetEnumValues(model.EnumType, model.AssemblyName).OfType<IComparable>());
        }

        #region Properties

        public ObservableCollection<IComparable> Literals { get; }

        public IComparable TypedValue
        {
            get
            {
                return Literals.FirstOrDefault(l => l.ToString().Equals(Model.Value, StringComparison.Ordinal));
            }
            set
            {
                if (string.Equals(value?.ToString(), Model.Value, StringComparison.Ordinal))
                {
                    return;
                }

                Model.Value = value?.ToString() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}
