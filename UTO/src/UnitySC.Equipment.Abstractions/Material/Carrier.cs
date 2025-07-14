using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions.Material
{
    public class Carrier : Agileo.EquipmentModeling.Material, IMaterialLocationContainer, IDisposable
    {
        private List<SlotState> _slotMap = new();

        public string Id
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of substrates a carrier can hold.
        /// </summary>
        public byte Capacity { get; }

        /// <summary>
        /// Date and Time when the Carrier was placed on the LoadPort.
        /// </summary>
        /// <remarks>
        /// PutTimeStamp can only be changed internally, using the constructor
        /// </remarks>
        public DateTime PutTimeStamp { get; }

        /// <summary>
        /// Indicates the size of wafers, <see cref="Agileo.SemiDefinitions.SampleDimension"/>, contained within the Carrier.
        /// </summary>
        public Agileo.SemiDefinitions.SampleDimension SampleSize { get; }

        public Collection<SlotState> GetMappingTable() => new(_slotMap.ToList());

        public Collection<SlotState> MappingTable { get; private set; }

        public Collection<SlotState> OriginalMappingTable { get; private set; }

        public event EventHandler<SlotMapEventArgs> SlotMapChanged;

        /// <inheritdoc />
        public Carrier(string name, byte capacity, Agileo.SemiDefinitions.SampleDimension size)
            : base(string.IsNullOrEmpty(name) ? " " : name)
        {
            PutTimeStamp      = DateTime.Now;
            Capacity          = capacity;
            SampleSize        = size;
            MaterialLocations = ReferenceFactory.OneToManyComposition<MaterialLocation>(
                nameof(MaterialLocations), this);

            OnPropertyChanged(nameof(SampleSize));
        }

        #region IMaterialLocationContainer

        /// <inheritdoc />
        public OneToManyComposition<MaterialLocation> MaterialLocations { get; }

        #endregion IMaterialLocationContainer

        public void SetSlotMap(IList<SlotState> slotMap, byte sourcePort, MaterialType materialType)
        {
            if (slotMap.Count != Capacity)
            {
                throw new InvalidOperationException("Provided slot map does not match the carrier capacity.");
            }

            MaterialLocations.Clear();
            for (int slotIndex = 0; slotIndex < slotMap.Count; slotIndex++)
            {
                //// [AGr] From "Sematech 300mm Operation FlowChart and scenario v10", §3.2.4.1 Carrier Substrate Location objects are instantiated after the Slot Map Verification OK event.
                //// Create SubstrateLocations one by one because type of MaterialLocation needs to be 'CarrierSubstrateLocation'
                var substrateLoc = new CarrierSubstrateLocation($"{Id}.{slotIndex + 1:00}");
                MaterialLocations.Add(substrateLoc);

                // Create Substrate depending on SlotState
                if (slotMap[slotIndex] != SlotState.NoWafer)
                {
                    // TODO We'll need a way to determine if carrier holds substrates or frames (maybe the type of LP or InfoPads can help?)
                    var substrate = new Wafer(GetSubstrateName(Id, slotIndex)) { MaterialDimension = SampleSize };
                    substrate.SetSource(substrateLoc, sourcePort, (byte)(slotIndex + 1), PutTimeStamp);
                    substrate.SetLocation(MaterialLocations[slotIndex]);
                    substrate.CarrierId = Id;
                    substrate.MaterialType = materialType;
                }
            }

            _slotMap = slotMap.ToList();
            MappingTable = new Collection<SlotState>(_slotMap);
            OriginalMappingTable = new Collection<SlotState>(new List<SlotState>(_slotMap));

            foreach (var materialLocation in MaterialLocations)
            {
                materialLocation.PropertyChanged += MaterialLocation_PropertyChanged;
            }

            SlotMapChanged?.Invoke(this, new SlotMapEventArgs(MappingTable.ToList().AsReadOnly()));
        }

        private void MaterialLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MaterialLocation.Material))
            {
                var materialLoc = MaterialLocations.SingleOrDefault(ml => ml == sender);
                _slotMap[MaterialLocations.IndexOf(materialLoc)] =
                    materialLoc?.Material == null ? SlotState.NoWafer : SlotState.HasWafer;

                SlotMapChanged?.Invoke(this, new SlotMapEventArgs(_slotMap.AsReadOnly()));
            }
        }

        public static string GetSubstrateName(string carrierId, int slotIndex)
        {
            return $"{carrierId}.{slotIndex + 1}";
        }

        #region IDisposable

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // When carrier is removed, all its location and material on it must be destroyed.
                    for (int slotIndex = 0; slotIndex < MaterialLocations.Count; slotIndex++)
                    {
                        MaterialLocations[slotIndex].Material?.SetLocation(null);
                        MaterialLocations[slotIndex].PropertyChanged -= MaterialLocation_PropertyChanged;
                        MaterialLocations[slotIndex] = null;
                    }

                    MaterialLocations.Clear();
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
