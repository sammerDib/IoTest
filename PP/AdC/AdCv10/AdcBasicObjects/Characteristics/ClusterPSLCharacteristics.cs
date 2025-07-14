namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // List des caracteristiques des clusters
    ///////////////////////////////////////////////////////////////////////
    public static class ClusterPSLCharacteristics
    {
        public static Characteristic IsSaturatedCharacteristic { get; private set; }

        internal static void Init()
        {
            IsSaturatedCharacteristic = new Characteristic(typeof(bool), "IsSaturated");
        }
    }
}
