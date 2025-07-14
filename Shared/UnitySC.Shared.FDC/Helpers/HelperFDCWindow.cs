using System;
using System.Linq;

using UnitySC.Shared.FDC.PersistentData;

namespace UnitySC.Shared.FDC.Helpers
{

    static public class HelperFDCWindow
    {
        // NOTE DE RTI doit on retourn un averag si la windw n'est pas full ?
        // choix actuel : on retourne la moyenne de la window même si incomplete

        public static double ComputeDblAverage<T> (PersistentWindowData<T> data) where T : IConvertible
        {
            if ((data.WindowData?.Count ?? 0) == 0)
                return double.NaN;

            double sum = 0.0;

            var dataList= data.WindowData.ToList();
            foreach (var val in dataList)
                sum += Convert.ToDouble(val);
            sum /= (double)dataList.Count;

            return sum;
        }

        public static int ComputeIntAverage<T>(PersistentWindowData<T> data) where T : IConvertible
        {
            if ((data.WindowData?.Count ?? 0) == 0)
                return 0;

            long sum = 0;
            var dataList = data.WindowData.ToList();
            foreach (var val in dataList)
            {
                sum += (long) Convert.ToInt32(val);
            }
            sum /= (long)dataList.Count;
            return (int) sum;
        }

        public static long ComputeLongAverage<T>(PersistentWindowData<T> data) where T : IConvertible
        {
            if((data.WindowData?.Count ?? 0) == 0)
                return 0;

            long sum = 0;
            var dataList = data.WindowData.ToList();
            foreach (var val in dataList)
                sum += Convert.ToInt64(val);
            sum /= (long)dataList.Count;
            return sum;
        }

    }

    static public class HelperFDCWindowTime
    {
        // NOTE DE RTI doit on retourné un average si la window ne couvre pas une period suffisante ou si il y a eu un gap trop important ?
        // choix acule on retourn la moyenne de la window peut importe les gaps ou le nimbr d'elements (sauf si vide)

        public static double ComputeDblAverage<T>(PersistentWindowTimeData<T> data) where T : IConvertible
        {
            if ((data.WindowTimeData?.Count ?? 0) == 0)
                return double.NaN;

            double sum = 0.0;
            var timeDataList = data.WindowTimeData.ToList();
            foreach (var val in timeDataList.ToList())
                sum += Convert.ToDouble(val);
            sum /= (double)timeDataList.Count;
            return sum;
        }

        public static int ComputeIntAverage<T>(PersistentWindowTimeData<T> data) where T : IConvertible
        {
            if ((data.WindowTimeData?.Count ?? 0) == 0)
                return 0;

            long sum = 0;
            var timeDataList = data.WindowTimeData.ToList();
            foreach (var val in timeDataList)
                sum += (long)Convert.ToInt32(val);
            sum /= (long)timeDataList.Count;
            return (int)sum;
        }

        public static long ComputeLongAverage<T>(PersistentWindowTimeData<T> data) where T : IConvertible
        {
            if ((data.WindowTimeData?.Count ?? 0) == 0)
                return 0;

            long sum = 0;
            var timeDataList = data.WindowTimeData.ToList();
            foreach (var val in timeDataList)
                sum += Convert.ToInt64(val);
            sum /= (long)timeDataList.Count;
            return sum;
        }

    }
}
