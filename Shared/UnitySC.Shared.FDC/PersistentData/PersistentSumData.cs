using System;
using System.Collections.Generic;

namespace UnitySC.Shared.FDC.PersistentData
{
    [Serializable]
    public class PersistentSumData<T> : IPersistentFDCData where T : IConvertible
    {
        // Default constructor requested for Serialization
        public PersistentSumData() { }

        // Usual Constructor
        public PersistentSumData(string name)
        {
            FDCName = name;
            NbItems = 0uL;
            Sum = 0.0;
        }

        public string FDCName { get; set; }

        public double Sum { get; set; }

        public UInt64 NbItems { get; set; }

        public double Average() {return (NbItems > 0uL) ? Sum/(double)NbItems : 0.0;}

        public void Add(T data)
        {
            Sum += Convert.ToDouble(data);
            ++NbItems;
        }

        public void AddRange(List<T> dataRange)
        {
            if (dataRange != null)
            {
                foreach (T data in dataRange)
                    Add(data);
            }
        }

        public void Clear()
        {
            NbItems = 0uL;
            Sum = 0;
        }
    }
}
