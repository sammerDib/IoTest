using System;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    public class SubstHistoryItem
    {
        /// <summary>
        /// Gets a value indicating the equipment substrate location or carrier substrate location on which the substrate was registered or has visited.
        /// </summary>
        public SubstrateLocation Location { get; }

        /// <summary>
        /// Gets a value indicating the actual arrival time of the substrate on the substrate location
        /// </summary>
        public DateTime TimeIn { get; }

        /// <summary>
        /// Gets a value indicating the actual departure time of the substrate from the substrate location.
        /// </summary>
        /// <remarks> This will equals <see cref="TimeIn"/> until the substrate has departed the location.</remarks>
        public DateTime? TimeOut { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstHistoryItem"/>
        /// </summary>
        /// <param name="location">The location identifier</param>
        /// <param name="timeIn">Arrival time of substrate on the specified location.</param>
        public SubstHistoryItem(SubstrateLocation location, DateTime timeIn)
        {
            Location = location;
            TimeIn = timeIn;
            TimeOut = null; // [E90] Table 3: TimeOut should be an empty item until the substrate has departed the location.
        }

        /// <summary>
        /// Sets the substrate departure time
        /// </summary>
        /// <param name="timeOut"></param>
        public void SetTimeOut(DateTime timeOut)
        {
            if (TimeOut.HasValue) { return; }

            TimeOut = timeOut;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TimeOut.HasValue ? $"{Location.Name}: {TimeIn} --> {TimeOut.Value}" : $"{Location.Name}: {TimeIn} --> -";
        }
    }
}
