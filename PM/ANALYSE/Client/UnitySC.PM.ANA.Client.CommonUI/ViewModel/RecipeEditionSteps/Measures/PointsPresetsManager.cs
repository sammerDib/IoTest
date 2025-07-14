using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    public class PointsPresetsManager
    {
        private const string PointsPresetsFolderName = "PointsPresets";

        public List<string> GetPresets(MeasureType measureType, WaferCategory waferCategory)
        {
            var presets = new List<string>();
            string presetsFolder = GetPresetsFolderPath(measureType, waferCategory);

            if (Directory.Exists(presetsFolder))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(presetsFolder, "*.csv", SearchOption.AllDirectories))
                    {
                        presets.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }
                catch (Exception)
                {
                    var logger = ClassLocator.Default.GetInstance<ILogger>();
                    logger.Error($"Failed to load the presets from the folder {presetsFolder}");
                }
            }
            return presets;
        }

        private static string GetPresetsFolderPath(MeasureType measureType, WaferCategory waferCategory)
        {
            var presetsFolderBase = Path.Combine(ClassLocator.Default.GetInstance<IClientConfigurationManager>().ConfigurationFolderPath, PointsPresetsFolderName);
            var presetsFolder = Path.Combine(presetsFolderBase, measureType.ToString(), waferCategory.Name);
            return presetsFolder;
        }

        private static string GetPresetFilePath(MeasureType measureType, WaferCategory waferCategory, string presetName)
        {
            string presetsFolder = GetPresetsFolderPath(measureType, waferCategory);
            string presetFullPath = Path.Combine(presetsFolder, presetName + ".csv");
            return presetFullPath;
        }

        public List<XYZTopZBottomPosition> GetPresetPoints(MeasureType measureType, WaferCategory waferCategory, string presetName)
        {
            string presetFullPath = GetPresetFilePath(measureType, waferCategory, presetName);

            return PointsCsvFiles.LoadPointsFromFile(presetFullPath);
        }

        public void DeletePreset(MeasureType measureType, WaferCategory waferCategory, string presetName)
        {

            string presetFullPath = GetPresetFilePath(measureType, waferCategory, presetName);
            try
            {
                if (File.Exists(presetFullPath))
                    File.Delete(presetFullPath);
            }
            catch (System.Exception)
            {

                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Error($"Failed to delete the preset : {presetFullPath}");
            }

        }

        public void SavePointsPreset(MeasureType measureType, WaferCategory waferCategory, string preset, List<MeasurePointVM> points)
        {
            string presetFullPath = GetPresetFilePath(measureType, waferCategory, preset);
            List<XYZTopZBottomPosition> pointsPositions= points.Select(point => point.PointPosition).ToList();


            try
            {
                PointsCsvFiles.SavePointsToFile(presetFullPath, pointsPositions, waferCategory.DimentionalCharacteristic);
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Preset saved with success.", "Preset Save", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Failed to save the preset", "Preset Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    var logger = ClassLocator.Default.GetInstance<ILogger>();
                    logger.Error($"Failed to save the preset {ex.Message}");
                });
            }

            
        }
    }
}
