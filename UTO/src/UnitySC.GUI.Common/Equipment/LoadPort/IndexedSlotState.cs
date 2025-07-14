using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Material;

using Carrier = UnitySC.Equipment.Abstractions.Material.Carrier;

namespace UnitySC.GUI.Common.Equipment.LoadPort
{
    public class IndexedSlotState
    {
        public SlotState State { get; set; }

        public Substrate Substrate { get; set; }

        public int Index { get; }

        public IndexedSlotState(SlotState state, Substrate substrate, int index)
        {
            State = state;
            Substrate = substrate;
            Index = index;
        }

        public static ObservableCollection<IndexedSlotState> GetIndexedSlotsState(UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort loadPort)
        {
            var mappingTable = new ObservableCollection<IndexedSlotState>();
            if (loadPort?.Carrier?.MappingTable != null)
            {
                for (var i = loadPort.Carrier.MappingTable.Count - 1; i >= 0; i--)
                {
                    mappingTable.Add(
                        new IndexedSlotState(
                            loadPort.Carrier.MappingTable[i],
                            (Substrate)loadPort.Carrier.MaterialLocations[i].Material,
                            i + 1));
                }
            }

            return mappingTable;
        }

        public static ObservableCollection<IndexedSlotState> GetOriginalIndexedSlotStates(
            UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort loadPort)
        {
            var mappingTable = new ObservableCollection<IndexedSlotState>();
            if (loadPort?.Carrier?.OriginalMappingTable != null)
            {
                for (var i = loadPort.Carrier.OriginalMappingTable.Count - 1; i >= 0; i--)
                {
                    var substrate = GetSubstrate(
                        loadPort.Carrier,
                        Carrier.GetSubstrateName(loadPort.Carrier.Id, i));
                    mappingTable.Add(
                        new IndexedSlotState(
                            loadPort.Carrier.OriginalMappingTable[i],
                            substrate,
                            i + 1));
                }
            }

            return mappingTable;
        }

        public static Substrate GetSubstrate(Carrier carrier, string substrateName)
        {
            //First look if the substrate exist in the carrier
            var substrate = carrier.MaterialLocations.Cast<SubstrateLocation>()
                .Select(sl => sl.Substrate)
                .Where(s => s != null)
                .FirstOrDefault(s => s.Name == substrateName);

            if (substrate == null)
            {
                //If substrate has not been found in the carrier, it means that it is moving in the equipment
                substrate = App.Instance.EquipmentManager.Equipment
                    .AllOfType<IMaterialLocationContainer>()
                    .SelectMany(mlc => mlc.MaterialLocations.OfType<SubstrateLocation>())
                    .Select(sl => sl.Substrate)
                    .Where(s => s != null)
                    .FirstOrDefault(s => s.Name == substrateName);
            }

            return substrate;
        }
    }
}
