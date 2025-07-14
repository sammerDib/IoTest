using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitySC.Shared.FDC.PersistentData
{
    [Serializable]
    public class PersistentWindowTimeData<T> : IPersistentFDCData where T : IConvertible
    {
        // Default constructor requested for Serialization
        public PersistentWindowTimeData() { }

        // Usual Constructor
        public PersistentWindowTimeData(string name, TimeSpan windowPeriod)
        {
            FDCName = name;
            WinPeriod = windowPeriod;
            WindowTimeData = new List< TimedValue<T> >();
        }

        public string FDCName { get; set; }

        public TimeSpan WinPeriod { get; set; }

        // DO NOT ADD or REMOVE Data from WindowTimeData use AddData or AddDataRange
        public List<TimedValue<T> > WindowTimeData { get; set; }

        public void AddData(T singledata)
        {
            AddData(new TimedValue<T>(singledata));
        }

        public void AddData(T singledata, DateTime datetime)
        {
            AddData(new TimedValue<T>(singledata, datetime));
        }

        public void AddData(TimedValue<T> singletimedata)
        {
            if (WindowTimeData.Count > 0)
            {
                if (WindowTimeData.Last().Date > singletimedata.Date)
                {
                    // do not add older value to window
                    return;
                }
                DateTime oldest = singletimedata.Date - WinPeriod;
                WindowTimeData.RemoveAll(t => t.Date < oldest);
            }
            WindowTimeData.Add(singletimedata);
        }

        public void AddDataRange(List<TimedValue<T>> rangeData, bool autoSort = false)
        {
            if (rangeData == null || rangeData.Count == 0)
                return;

            // autosort is need when rangeData are not sorted in Time order
            if(autoSort)
                rangeData.Sort((x, y) => x.Date.CompareTo(y.Date));

            DateTime oldest = rangeData.Last().Date - WinPeriod;
            rangeData.RemoveAll(t => t.Date < oldest);
            WindowTimeData.RemoveAll(t => t.Date < oldest);
            WindowTimeData.AddRange(rangeData);
        }
    }
}
