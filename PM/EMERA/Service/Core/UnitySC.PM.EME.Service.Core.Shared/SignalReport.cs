using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public static class SignalReport
    {
        private static ILogger Logger => ClassLocator.Default.GetInstance<ILogger>();
        private static string LogHeader => "[SignalReport]";

        private static void WriteSignalInCSVFormat(string label1, string label2, List<double> values1, List<double> values2, string filepath)
        {
            try
            { 
                string separator = CSVStringBuilder.GetCSVSeparator();

                using (var file = new StreamWriter(filepath))
                {
                    file.Write(label1 + separator + label2);

                    int signalSize = values1.Count();
                    for (int i = 0; i < signalSize; i++)
                    {
                        string lightPowerStr = values1[i].ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
                        string contrastStr = values2[i].ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);

                        file.Write(Environment.NewLine);
                        file.Write(lightPowerStr + separator + contrastStr);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting failed : {e.Message}");
            }
        }

        public static void WriteSignalInCSVFormat(string label1, string label2, List<Tuple<double, double>> values, string filepath)
        {
            var firstValues = values.Select(v => v.Item1).ToList();
            var secondValues = values.Select(v => v.Item2).ToList();
            WriteSignalInCSVFormat(label1, label2, firstValues, secondValues, filepath);
        }
    }
}
