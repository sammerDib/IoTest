using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    /// <summary>
    /// Material location choice ViewModel.
    /// </summary>
    public sealed class MaterialLocationChoiceViewModel : ParameterViewModel, IDisposable
    {
        private readonly MaterialManager _materialManager;

        /// <summary>
        /// Initializes a new instance of <see cref="MaterialLocationChoiceViewModel"/>.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <param name="commandViewModel">The command ViewModel.</param>
        public MaterialLocationChoiceViewModel(Agileo.EquipmentModeling.Parameter parameter, DeviceCommandViewModel commandViewModel)
            : base(parameter, commandViewModel, typeof(MaterialLocation))
        {
            _materialManager = MaterialManager.GetMaterialManagerFrom(commandViewModel.Device);
            if (_materialManager != null)
            {
                _materialManager.MaterialMoved += MaterialManager_MaterialMoved;
                RefreshLocations();
            }
            else
            {
                Value = null;
            }
        }

        /// <summary>
        /// Gets the available material locations.
        /// </summary>
        public ObservableCollection<MaterialLocation> Locations { get; } = new();

        private void MaterialManager_MaterialMoved(object sender, MaterialMovedEventArgs e)
        {
            RefreshLocations();
        }

        private void RefreshLocations()
        {
            Locations.Clear();

            Locations.AddRange(_materialManager.LocationPicker.PickSafeLocations(
                DeviceCommandViewModel.Device,
                (DeviceCommand)Parameter.Container,
                Parameter));

            var selectedLocation = Locations.FirstOrDefault(loc =>
                loc.QualifiedName.Equals((Value as MaterialLocation)?.QualifiedName, StringComparison.Ordinal));

            Value = selectedLocation ?? Locations.FirstOrDefault();
        }

        public void Dispose()
        {
            if (_materialManager != null)
            {
                _materialManager.MaterialMoved -= MaterialManager_MaterialMoved;
            }
        }
    }
}
