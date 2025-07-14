using System;

namespace UnitySC.Shared.FDC.PersistentData
{
    [Serializable]
    public class PersistentFDCDate : IPersistentFDCData
    {
        // Default constructor requested for Serialization
        public PersistentFDCDate() { }

        public PersistentFDCDate(string name) : this(name, DateTime.Now) { }

        public PersistentFDCDate(string name, DateTime dt)
        {
            FDCName = name;
            Date = dt;
        }

        public string FDCName { get; set; }

        public DateTime Date { get; set; }
    }
}
