using System.Collections.ObjectModel;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.Models;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation
{
    /// <summary>
    /// Class representing LoadPort Model related to simulation.
    /// </summary>
    public class LoadPortSimulationData
    {
        /// <summary>
        /// Gets or sets a value indicating whether current simulated command must fail or succeed.
        /// </summary>
        public bool IsCommandExecutionFailed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tag reader is enabled.
        /// </summary>
        public bool IsTagReadEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tag read/write must fail or succeed.
        /// </summary>
        public bool IsReadWriteFailed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the carrier identifier which has been read.
        /// </summary>
        public string CarrierIdRead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the configuration of carrier which has been loaded on the load port.
        /// </summary>
        public CarrierConfiguration CarrierConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value representing the carrier mapping.
        /// </summary>
        public ReadOnlyCollection<SlotState> Mapping => new(CarrierConfiguration.MappingTable);
    }
}
