using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels
{
    public class
        MaterialLocationContainerDeviceCommandParameterViewModel : DeviceCommandParameterViewModel<
            MaterialLocationContainerDeviceCommandParameter>
    {
        public MaterialLocationContainerDeviceCommandParameterViewModel(
            MaterialLocationContainerDeviceCommandParameter model) : base(model)
        {
            MaterialLocationContainers =
                new ObservableCollection<IMaterialLocationContainer>(App.Instance.EquipmentManager.Equipment
                    .AllOfType<IMaterialLocationContainer>());
        }

        public ObservableCollection<IMaterialLocationContainer> MaterialLocationContainers { get; }

        public IMaterialLocationContainer TypedValue
        {
            get
            {
                return MaterialLocationContainers.FirstOrDefault(container => container.Name.Equals(Model.Value));
            }
            set
            {
                if (Model.Value == value?.Name)
                {
                    return;
                }

                Model.Value = value?.Name;
                OnPropertyChanged();
            }
        }

        #region Override

    }

    #endregion
}
