using System;
using System.Collections.ObjectModel;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.EquipmentHandling.Clean
{
    public class CleanLoadPort : IDisposable
    {
        #region Properties

        public LoadPort LoadPort { get; }

        public ObservableCollection<IndexedSlotState> MappingTable { get; }

        public CleanLoadPort(LoadPort loadPort)
        {
            LoadPort = loadPort;
            MappingTable = IndexedSlotState.GetIndexedSlotsState(LoadPort);

            LoadPort.CarrierPlaced += Lp_CarrierPlaced;
            LoadPort.CarrierRemoved += Lp_CarrierRemoved;
            if (LoadPort.Carrier != null)
            {
                LoadPort.Carrier.SlotMapChanged += Carrier_SlotMapChanged;
            }
        }

        #endregion

        #region Event Handlers

        private void Carrier_SlotMapChanged(object sender, UnitySC.Equipment.Abstractions.Material.SlotMapEventArgs e)
        {
            UpdateMappingTables();
        }

        private void Lp_CarrierRemoved(
            object sender,
            UnitySC.Equipment.Abstractions.Material.CarrierEventArgs e)
            => e.Carrier.SlotMapChanged -= Carrier_SlotMapChanged;

        private void Lp_CarrierPlaced(
            object sender,
            UnitySC.Equipment.Abstractions.Material.CarrierEventArgs e)
            => e.Carrier.SlotMapChanged += Carrier_SlotMapChanged;


        #endregion

        #region Private Methods

        private void UpdateMappingTables()
            => DispatcherHelper.DoInUiThreadAsynchronously(
                () =>
                {
                    MappingTable.Clear();
                    MappingTable.AddRange(IndexedSlotState.GetIndexedSlotsState(LoadPort));
                });

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoadPort.CarrierPlaced -= Lp_CarrierPlaced;
                LoadPort.CarrierRemoved -= Lp_CarrierRemoved;
                if (LoadPort.Carrier != null)
                {
                    LoadPort.Carrier.SlotMapChanged -= Carrier_SlotMapChanged;
                }
            }
        }

        #endregion
    }
}
