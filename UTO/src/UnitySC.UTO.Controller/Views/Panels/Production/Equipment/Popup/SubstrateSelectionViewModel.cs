using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Popup
{
    public class SubstrateSelectionViewModel : Notifier, IDisposable
    {
        #region Properties

        public LoadPort LoadPort { get; }
        public ObservableCollection<IndexedSlotState> LpMappingTable { get; }
        public List<IndexedSlotState> LpSelectedSlots { get; }

        #endregion

        #region Constructor

        public SubstrateSelectionViewModel(LoadPort loadPort)
        {
            LoadPort = loadPort;
            LpMappingTable = IndexedSlotState.GetOriginalIndexedSlotStates(LoadPort);
            LpSelectedSlots = new List<IndexedSlotState>();

            if (LoadPort == null)
            {
                return;
            }

            LoadPort.CarrierPlaced += LoadPort_CarrierPlaced;
            LoadPort.CarrierRemoved += LoadPort_CarrierRemoved;

            if (LoadPort.Carrier != null)
            {
                LoadPort.Carrier.SlotMapChanged += LpCarrierOnSlotMapChanged;
            }
        }

        #endregion

        #region Event handler

        private void LoadPort_CarrierPlaced(object sender, CarrierEventArgs e)
            => e.Carrier.SlotMapChanged += LpCarrierOnSlotMapChanged;

        private void LoadPort_CarrierRemoved(object sender, CarrierEventArgs e)
            => e.Carrier.SlotMapChanged -= LpCarrierOnSlotMapChanged;

        private void LpCarrierOnSlotMapChanged(object sender, SlotMapEventArgs e) => UpdateLpMappingTable();

        #endregion

        #region Private method

        private void UpdateLpMappingTable()
            => DispatcherHelper.DoInUiThread(
                () =>
                {
                    if (LpMappingTable.Any())
                    {
                        return;
                    }

                    LpMappingTable.Clear();
                    foreach (var indexedSlotState in IndexedSlotState.GetOriginalIndexedSlotStates(LoadPort))
                    {
                        LpMappingTable.Add(indexedSlotState);
                    }
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
            if (LoadPort == null)
            {
                return;
            }

            LoadPort.CarrierPlaced -= LoadPort_CarrierPlaced;
            LoadPort.CarrierRemoved -= LoadPort_CarrierRemoved;

            if (LoadPort.Carrier != null)
            {
                LoadPort.Carrier.SlotMapChanged -= LpCarrierOnSlotMapChanged;
            }
        }

        #endregion
    }
}
