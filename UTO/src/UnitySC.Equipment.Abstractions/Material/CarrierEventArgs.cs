using System;

namespace UnitySC.Equipment.Abstractions.Material
{
    /// <summary>
    /// Class responsible to hold information about carrier events.
    /// </summary>
    public class CarrierEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CarrierEventArgs"/> class.
        /// </summary>
        /// <param name="carrier"><see cref="Carrier"/></param>
        public CarrierEventArgs(Carrier carrier)
        {
            Carrier = carrier;
        }

        /// <summary>
        /// Gets the carrier object associated to the event (e.g. being placed/removed).
        /// </summary>
        public Carrier Carrier { get; }
    }
}
