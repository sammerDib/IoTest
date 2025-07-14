using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitySC.Shared.FDC.PersistentData
{

    [Serializable]
    public class PersistentWindowData<T> : IPersistentFDCData where T : IConvertible
    {
        // Default constructor requested for Serialization
        public PersistentWindowData() {}

        // Usual Constructor
        public PersistentWindowData( string name, int winSize)
        {
            FDCName = name;
            WindowSize = winSize;
            WindowData = new List<T> (winSize);
        }

        public string FDCName { get; set; }

        public int WindowSize { get; set; }

        // DO NOT ADD or REMOVE Data from WindowData use AddData and AddDataRange
        public List<T> WindowData { get; set; }

        public bool IsWindowFull() { return WindowSize <= (WindowData?.Count ?? -1 ); }

        public void AddData(T singledata)
        {
            if (IsWindowFull())
            {
                WindowData?.RemoveAt(0); 
            }
            WindowData?.Add(singledata);
        }

        public void AddDataRange(List<T> rangeData)
        {
            var currentWindowSize = (WindowData?.Count ?? 0);
            var total = currentWindowSize + rangeData.Count;
            if (total <= WindowSize)
            {
                WindowData?.AddRange(rangeData);
            }
            else
            {
                if (currentWindowSize > 0)
                {
                    int toRemove = Math.Max(1, Math.Min(currentWindowSize, total - WindowSize));
                    WindowData?.RemoveRange(0, toRemove);
                }
                WindowData?.AddRange(rangeData.Skip(Math.Max(0, rangeData.Count - WindowSize)));
            }
        }
    }
}
