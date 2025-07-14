using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.UTO.Controller.Views.Panels.Maintenance.Wafer
{
    public class WaferPanel : BusinessPanel
    {
        #region Constructors

        static WaferPanel()
        {
            DataTemplateGenerator.Create(typeof(WaferPanel), typeof(WaferPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(WaferPanelResources)));
        }

        public WaferPanel()
            : this($"{nameof(WaferPanel)} DesignTime Constructor")
        {
        }

        public WaferPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            Wafers = new DataTableSource<Equipment.Abstractions.Material.Wafer>();

            Wafers.Search.AddSearchDefinition(
                nameof(WaferPanelResources.CARRIER_ID),
                wafer => wafer.CarrierId,
                true);

            Wafers.Search.AddSearchDefinition(
                nameof(WaferPanelResources.PROCESSJOB_ID),
                wafer => wafer.ProcessJobId,
                true);

            Wafers.Search.AddSearchDefinition(
                nameof(WaferPanelResources.CONTROLJOB_ID),
                wafer => wafer.ControlJobId,
                true);

            Wafers.Search.AddSearchDefinition(
                nameof(WaferPanelResources.LOT_ID),
                wafer => wafer.LotId,
                true);

            Wafers.Search.AddSearchDefinition(
                nameof(WaferPanelResources.SUBSTRATE_ID),
                wafer => wafer.SubstrateId,
                true);

            Wafers.Search.AddSearchDefinition(
                nameof(WaferPanelResources.ACQUIRED_ID),
                wafer => wafer.AcquiredId,
                true);

            Wafers.Sort.AddSortDefinition(
                nameof(Equipment.Abstractions.Material.Wafer.SubstrateId),
                wafer => wafer.SubstrateId);

            var controller = App.ControllerInstance.ControllerEquipmentManager.Equipment
                .AllOfType<Equipment.Devices.Controller.Controller>()
                .First();
            controller.MaterialManager.MaterialMoved += MaterialManager_MaterialMoved;
        }

        #endregion

        #region Properties

        public DataTableSource<Equipment.Abstractions.Material.Wafer> Wafers { get; }

        #endregion

        #region Event Handlers

        private void MaterialManager_MaterialMoved(object sender, MaterialMovedEventArgs e)
        {
            if (e.Material is not Carrier carrier)
            {
                return;
            }

            //Carrier has been placed
            if (e.OldLocation == null)
            {
                carrier.SlotMapChanged += Carrier_SlotMapChanged;
            }
            //Carrier has been removed
            else if (e.NewLocation == null)
            {
                carrier.SlotMapChanged -= Carrier_SlotMapChanged;
                Wafers.Reset(new List<Equipment.Abstractions.Material.Wafer>());
            }
        }

        private void Carrier_SlotMapChanged(object sender, SlotMapEventArgs e)
        {
            if (sender is not Carrier carrier)
            {
                return;
            }

            Wafers.Reset(new List<Equipment.Abstractions.Material.Wafer>());
            foreach (var materialLocation in carrier.MaterialLocations)
            {
                if (materialLocation.Material is Equipment.Abstractions.Material.Wafer wafer)
                {
                    wafer.PropertyChanged += Wafer_PropertyChanged;
                    Wafers.Add(wafer);
                }
            }
            Wafers.UpdateCollection();
        }

        private void Wafer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not Equipment.Abstractions.Material.Wafer wafer)
            {
                return;
            }

            UpdateWafer(wafer);
        }

        #endregion

        #region Private Methods

        private object _lockWafer = new();

        private void UpdateWafer(Equipment.Abstractions.Material.Wafer wafer)
        {
            lock (_lockWafer)
            {
                var displayedWafer = Wafers.FirstOrDefault(w => w.SubstrateId == wafer.SubstrateId);
                if (displayedWafer != null)
                {
                    int waferIndex = Wafers.IndexOf(displayedWafer);
                    Wafers[waferIndex] = wafer;
                }
                else
                {
                    Wafers.Add(wafer);
                }
                Wafers.UpdateCollection();
            }
        }

        #endregion
    }
}
