using System;

namespace UnitySC.Shared.FDC.PersistentData
{
    [Serializable]
    public class PersistentFDCRate<T> : IPersistentFDCData where T : IConvertible
    {
        public string FDCName { get; set; }

        public T Amount { get; set; }

        public T Total { get; set; }

        // Default constructor requested for Serialization
        public PersistentFDCRate() { }

        public PersistentFDCRate(string name)
        {
            FDCName = name;
        }

        public double GetPercentageRate()
        {
            return GetRate() * 100;
        }

        public double GetRate()
        {
            double numericAmount = Convert.ToDouble(Amount);
            double numericTotal = Convert.ToDouble(Total);
            if (numericTotal == 0)
            {
                return 0.0;
            }
            else
            {
                return (numericAmount / numericTotal);
            }
        }

        public void Clear()
        {
            Amount = default;
            Total = default;
        }

    }
}
