using System;

namespace UnitySC.Shared.FDC.PersistentData
{
    [Serializable]
    public class PersistentFDCCounter<T> : IPersistentFDCData where T : IConvertible
    {
        // Default constructor requested for Serialization
        public PersistentFDCCounter() { }

        public PersistentFDCCounter(string name) 
        {
            FDCName = name;
        }

        public string FDCName { get; set; }

        public T Counter { get; set; }

    }
}
