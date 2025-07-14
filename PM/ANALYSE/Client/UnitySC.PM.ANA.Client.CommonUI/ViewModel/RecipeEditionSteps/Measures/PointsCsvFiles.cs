using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    static class PointsCsvFiles
    {
        public static List<XYZTopZBottomPosition> LoadPointsFromFile(string filePathToImport)
        {
            if (filePathToImport is null)
                return null;

            if (File.Exists(filePathToImport))
            {
                try
                {
                    string separator = CSVStringBuilder.GetCSVSeparator();
                    var cSep = new char[] { separator[0] };

                    var point = new UnitySC.Shared.Data.Geometry.PointUnits();
                    var pointPositions = new List<XYZTopZBottomPosition>();
                    using (var reader = new StreamReader(filePathToImport))
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(cSep);

                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            values = line.Split(cSep);

                            var pointPosition = new XYZTopZBottomPosition()
                            {
                                X = Convert.ToDouble(values[0]),
                                Y = Convert.ToDouble(values[1]),
                                ZTop = Convert.ToDouble(values[2]),
                                ZBottom = Convert.ToDouble(values[3]),
                                Referential = new WaferReferential()
                            };
                            pointPositions.Add(pointPosition);
                        }
                    }

                    return pointPositions;
                }
                catch (Exception ex)
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "ImportFromFile: Error during import csv file");
                    });
                    
                }
                
            }
            return null;
        }

        public static void SavePointsToFile(string filePath, List<XYZTopZBottomPosition> pointPositions, WaferDimensionalCharacteristic waferDimentionalCharacteristic )
        {
            string separator = CSVStringBuilder.GetCSVSeparator();

  

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            
            //Header for file                               

            var sbCSV = new CSVStringBuilder();
            sbCSV.AppendLine("X mm", "Y mm", "ZTop mm", "ZBottom mm", "Die Index Row", "X(mm)", "Y(mm)", $"Wafer Diameter mm{separator} {waferDimentionalCharacteristic.Diameter.Millimeters}");
            foreach (var position in pointPositions)
            {
                sbCSV.Append($"{position.X}");
                sbCSV.Append($"{position.Y}");
                sbCSV.Append($"{position.ZTop}");
                sbCSV.Append($"{position.ZBottom}");
                sbCSV.AppendLine();
            }

            File.WriteAllText(filePath, sbCSV.ToString());
        }
    }
}
