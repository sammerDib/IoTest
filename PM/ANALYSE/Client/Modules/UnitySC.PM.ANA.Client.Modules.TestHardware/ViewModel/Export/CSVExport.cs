using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel.Export
{
    public static class CsvExport 
    {
        public static void CompleteCsvWithSelectedPoint(StreamWriter file, List<List<ProbePoint>> selectedPoints)
        {
            string separator = CSVStringBuilder.GetCSVSeparator();

            var listOfSelectedPeaks = selectedPoints;
            var valuesOfSelectedPoint = new string[2];
            for (int i = 0; i < listOfSelectedPeaks.Count; i++)
            {
                //for each acquisition, we set header
                file.WriteLine($"Selected peaks for acquisition {i + 1}");
                file.WriteLine($"Position{separator}Volts");
                var selectedPeaks = listOfSelectedPeaks[i];
                //we complete line by line with pointX and pointY
                foreach (var peaks in selectedPeaks)
                {
                    valuesOfSelectedPoint[0] = Math.Round(peaks.X, 3).ToString(CultureInfo.InvariantCulture);
                    valuesOfSelectedPoint[1] = Math.Round(peaks.Y, 2).ToString(CultureInfo.InvariantCulture);
                    string rowOfSelectedPoints = string.Join(separator, valuesOfSelectedPoint);
                    file.WriteLine(rowOfSelectedPoints);
                }
                file.WriteLine("");
            }
        }

        public static void CompleteCsvWithRawData
            (StreamWriter file, List<float> xRawAcquisition, List<List<double>> yRawAcquisition)
        {
            string separator = CSVStringBuilder.GetCSVSeparator();

            var pointsX = xRawAcquisition;
            var listOfAcquisition = yRawAcquisition;
            var valuesOfAcquisition = new string[listOfAcquisition.Count + 1];
            var label = new string[yRawAcquisition.Count + 1];
            label[0] = "Position";

            //header for each acquisition
            for (int l = 0; l < yRawAcquisition.Count; l++)
            {
                label[l + 1] = $"Acquisition {l + 1}";
            }
            string labelRaw = string.Join(separator, label);
            file.WriteLine(labelRaw);

            //we complete line by line with pointX and all pointY
            for (int i = 0; i < pointsX.Count; i++)
            {
                valuesOfAcquisition[0] = Math.Round(pointsX[i],3).ToString(CultureInfo.InvariantCulture);

                for (int k = 0; k < listOfAcquisition.Count; k++)
                {
                    var pointsY = listOfAcquisition[k];
                    valuesOfAcquisition[k + 1] = i < pointsY.Count ? Math.Round(pointsY[i], 2).ToString(CultureInfo.InvariantCulture) : "";
                }

                string rowRawData = string.Join(separator, valuesOfAcquisition);
                file.WriteLine(rowRawData);
            }
            file.WriteLine("");
        }
    }
}
