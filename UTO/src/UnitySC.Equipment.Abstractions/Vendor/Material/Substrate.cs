using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    public class Substrate : Agileo.EquipmentModeling.Material
    {
        private readonly List<SubstHistoryItem> _substHistory;

        /// <inheritdoc />
        public Substrate(string name)
            : base(name)
        {
            _substHistory = new List<SubstHistoryItem>();
        }

        public SampleDimension MaterialDimension { get; set; }

        /// <summary>
        /// Gets a value indicating the substrate location on which the substrate has been initially registered.
        /// </summary>
        public SubstrateLocation Source { get; private set; }

        /// <summary>
        /// Gets a value indicating the substrate source port Id on which the substrate has been initially registered
        /// </summary>
        public byte SourcePort { get; private set; }

        /// <summary>
        /// Gets a value indicating the substrate source slot Id on which the substrate has been initially registered
        /// </summary>
        public byte SourceSlot { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating the substrate location on which the substrate shall be finally restored.
        /// </summary>
        /// <remarks>If null, then same as source substrate location</remarks>
        public SubstrateLocation Destination { get; set; }

        /// <summary>
        /// Gets the simplified name of the substrate
        /// </summary>
        public string SimplifiedName => $"C{SourcePort:00}.S{SourceSlot:00}";

        /// <summary>
        /// Gets a value indicating the substrate history (series of location that the substrate has visited).
        /// </summary>
        public ReadOnlyCollection<SubstHistoryItem> SubstrateHistory
            => new ReadOnlyCollection<SubstHistoryItem>(_substHistory);


        private WaferStatus _status;

        /// <summary>
        /// Get or set a value indicating the current status of the substrate
        /// </summary>
        public WaferStatus Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;

                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        /// <summary>
        /// Set time and location on which substrate has been loaded.
        /// </summary>
        /// <param name="source">The source substrate location.</param>
        /// <param name="sourcePort">The source port.</param>
        /// <param name="sourceSlot">The source slot.</param>
        /// <param name="loadingTime">The time when substrate has been loaded on the equipment.</param>
        public void SetSource(SubstrateLocation source, byte sourcePort, byte sourceSlot, DateTime loadingTime)
        {
            _substHistory.Add(new SubstHistoryItem(source, loadingTime));
            Source = source;
            SourcePort = sourcePort;
            SourceSlot = sourceSlot;
            Destination = null; // By default slot integrity is used
        }

        /// <summary>
        /// Updates the substrate history.
        /// </summary>
        /// <param name="moveTime">The time when substrate has moved.</param>
        /// <param name="newLocation">The substrate location after the move.</param>
        public void UpdateHistory(DateTime moveTime, SubstrateLocation newLocation)
        {
            _substHistory.LastOrDefault()?.SetTimeOut(moveTime);
            if (newLocation != null)
            {
                _substHistory.Add(new SubstHistoryItem(newLocation, moveTime));
            }
        }
    }
}
