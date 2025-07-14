using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    public class Carrier : Agileo.EquipmentModeling.Material, IMaterialLocationContainer, IDisposable
    {
        private List<SlotState> _slotMap = new List<SlotState>();

        public string ID
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        /// Gets or sets the Maximum number of substrates a carrier can hold.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Date and Time when the Carrier was placed on the LoadPort.
        /// </summary>
        /// <remarks>
        /// PutTimeStamp can only be changed internally, using the constructor
        /// </remarks>
        public DateTime PutTimeStamp
        {
            get;
        }

        /// <summary>
        /// Indicates the size of wafers, <see cref="SampleDimension"/>, contained within the Carrier.
        /// </summary>
        public SampleDimension SampleSize
        {
            get;
        }

        public Collection<SlotState> MappingTable
        {
            get { return new Collection<SlotState>(_slotMap.ToList()); }
        }

        public event EventHandler<SlotMapEventArgs> SlotMapChanged;

        /// <inheritdoc />
        public Carrier(string name, int capacity, SampleDimension size) : base(string.IsNullOrEmpty(name) ? " " : name)
        {
            PutTimeStamp = DateTime.Now;
            Capacity = capacity;
            SampleSize = size;
            MaterialLocations = ReferenceFactory.OneToManyComposition<MaterialLocation>(nameof(MaterialLocations), this);
        }

        #region IMaterialLocationContainer
        /// <inheritdoc />
        public OneToManyComposition<MaterialLocation> MaterialLocations
        {
            get;
        }
        #endregion IMaterialLocationContainer

        internal void SetSlotMap(IList<SlotState> slotMap, byte sourcePort)
        {
            if (slotMap.Count != Capacity)
            {
                throw new InvalidOperationException("Provided slot map does not match the carrier capacity.");
            }

            for (int slotIndex = 0; slotIndex < slotMap.Count; slotIndex++)
            {
                //// [AGr] From "Sematech 300mm Operation FlowChart and scenario v10", §3.2.4.1 Carrier Substrate Location objects are instantiated after the Slot Map Verification OK event.
                //// Create SubstrateLocations one by one because type of MaterialLocation needs to be 'CarrierSubstrateLocation'
                var substrateLoc = new CarrierSubstrateLocation($"{ID}.{slotIndex + 1}");
                MaterialLocations.Add(substrateLoc);

                // Create Substrate depending on SlotState
                if (slotMap[slotIndex] != SlotState.Empty)
                {
                    var substrate = new Substrate($"{ID}.{slotIndex + 1}") { MaterialDimension = SampleSize };
                    substrate.SetSource(substrateLoc, sourcePort, (byte)(slotIndex + 1), PutTimeStamp);
                    substrate.SetLocation(MaterialLocations[slotIndex]);
                }
            }

            _slotMap = slotMap.ToList();

            foreach (var materialLocation in MaterialLocations)
            {
                materialLocation.PropertyChanged += MaterialLocation_PropertyChanged;
            }

            SlotMapChanged?.Invoke(this, new SlotMapEventArgs(_slotMap.AsReadOnly()));
        }

        private void MaterialLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MaterialLocation.Material))
            {
                var materialLoc = MaterialLocations.SingleOrDefault(ml => ml == sender);
                _slotMap[MaterialLocations.IndexOf(materialLoc)] =
                    materialLoc?.Material == null ? SlotState.Empty : SlotState.Correct;

                SlotMapChanged?.Invoke(this, new SlotMapEventArgs(_slotMap.AsReadOnly()));
            }
        }

        internal void UpdateContentMap()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
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
    }
}
