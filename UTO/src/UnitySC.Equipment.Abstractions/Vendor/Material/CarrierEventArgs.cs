using System;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    public class CarrierEventArgs : EventArgs
    {
        public CarrierEventArgs(Carrier carrier)
        {
            Carrier = carrier;
        }

        public Carrier Carrier { get; }
    }
}
