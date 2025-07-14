using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public static class LiseSignalReport
    {
        private static ILogger Logger => ClassLocator.Default.GetInstance<ILogger>();
        private static string LogHeader => $"[LiseSignalReport]";


        public static void WriteRawSignalInCSVFormat(List<double> values, string filePath)
        {
            try
            {
                var csv = new CSVStringBuilder();
                int i = 0;
                foreach (double p in values)
                {
                    csv.AppendLine(i.ToString(), p.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture));
                    i++;
                }
                File.WriteAllText(filePath, csv.ToString());
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting Raw Signal failed : {e.Message}");
            }
        }

        public static void WriteRawSignalInCSVFormat(ProbeLiseSignal rawSignal, string filePath, string header = null)
        {
            if (rawSignal == null)
                return;

            try
            {     
                var csv = new CSVStringBuilder();
                double x = 0.0;
                double stepx = (double)rawSignal.StepX / 1000.0;
                csv.AppendLine("X", "Y");
                foreach (double y in rawSignal.RawValues)
                {
                    csv.AppendLine(x.ToString(System.Globalization.CultureInfo.InvariantCulture), y.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    x += stepx;
                }
                File.WriteAllText(filePath, csv.ToString());
            }
            catch (Exception e)
            {
                Logger.Error($"{header ?? LogHeader} Reporting Raw Signal failed : {e.Message}");
            }
        }

        public static void WriteSignalAnalysisInCSVFormat(LISESignalAnalyzed analyzedSignal, LiseSignalAnalysisParams algoParams, string filepath)
        {
            try
            {
                int signalSize = analyzedSignal.RawValues.Count();
                double[] referencePeaks = new double[signalSize];
                foreach (Peak peak in analyzedSignal.ReferencePeaks)
                {
                    referencePeaks[(int)peak.X] = peak.Y;
                }

                double[] selectedPeaks = new double[signalSize];
                foreach (Peak peak in analyzedSignal.SelectedPeaks)
                {
                    selectedPeaks[(int)peak.X] = peak.Y;
                }

                var sbCSV = new CSVStringBuilder();
                sbCSV.AppendLine("Raw signal", "Threshold", "Reference peak", "Peak of interest");
                for (int i = 0; i < signalSize; i++)
                {
                    double threshold = analyzedSignal.Means[i] + analyzedSignal.StdDev[i] * algoParams.DetectionCoef;
                    sbCSV.AppendLine(
                         analyzedSignal.RawValues[i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                         threshold.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                         referencePeaks[i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                         selectedPeaks[i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)  );
                }
                File.WriteAllText(filepath, sbCSV.ToString());   
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting Signal Analysis failed  : {e.Message}");
            }
        }

        public static void WriteSignalAtfocusPositionInCSVFormat(LISESignalAnalyzed analyzedSignalAtFocus, int focusPosition, string filepath)
        {
            try
            {
                int signalSize = analyzedSignalAtFocus.RawValues.Count();
                double[] focus = new double[signalSize];
                focus[focusPosition] = analyzedSignalAtFocus.SaturationValue;

                var sbCSV = new CSVStringBuilder();
                sbCSV.AppendLine("Raw signal", "Focus position");
                for (int i = 0; i < signalSize; i++)
                {
                    sbCSV.AppendLine(analyzedSignalAtFocus.RawValues[i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                                     focus[i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) );

                }
                File.WriteAllText(filepath, sbCSV.ToString());
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting Signal at focus failed : {e.Message}");
            }
        }
    }
}
