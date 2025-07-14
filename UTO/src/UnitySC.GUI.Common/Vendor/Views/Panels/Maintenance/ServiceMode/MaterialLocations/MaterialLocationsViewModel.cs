using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.MaterialLocations
{
    public class MaterialLocationsViewModel : NamedViewModel
    {
        public override string Name => "Material Locations";

        public MaterialLocationsViewModel(IMaterialLocationContainer container)
        {
            MaterialLocations.Sort.AddSortDefinition(nameof(MaterialLocation.Name), location => location.Name);
            MaterialLocations.Sort.AddSortDefinition(nameof(MaterialLocation.Material), location => location.Material.Name);

            MaterialLocations.Reset(container.MaterialLocations);
        }

        public DataTableSource<MaterialLocation> MaterialLocations { get; } = new();
    }
}
