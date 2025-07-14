using System;
using System.Configuration;
using System.IO;
using System.Linq;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.StandaloneClient.ViewModel.Common
{
    public class Settings
    {
        public static readonly string AppPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

        public static readonly string KlarfRoughBinPath = Path.Combine(AppPath,
            @"Settings\KlarfRoughBinSettings.xml");

        public static readonly string KlarfSizeBinPath = Path.Combine(AppPath,
            @"Settings\KlarfSizeBinSettings.xml");

        public static readonly string HazeConfigPath = Path.Combine(AppPath,
            @"Settings\HazeSettings.xml");

        public static readonly string ThumbnailConfigPath = Path.Combine(AppPath,
            @"Settings\ThumbnailSettings.xml");

        public static readonly string DefaultResultDirectory = ConfigurationManager.AppSettings["DefaultResultDirectory"];

        public KlarfSettingsData KlarfSettings { get; set; }

        public ColorMapSettings HazeSettings { get; set; }

        public ColorMapSettings ThumbnailSettings { get; set; }

        public static KlarfSettingsData LoadKlarfSettings(string defectBinsPath, string sizeBinPath)
        {
            DefectBins defectBins = null;
            SizeBins sizeBins = null;
            try
            {
                defectBins = DefectBins.ImportFromXml(defectBinsPath);
                sizeBins = SizeBins.ImportFromXml(sizeBinPath);
            }
            catch (Exception ex)
            {
                var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVM.AddMessage(new Message(MessageLevel.Error, "An error occurred while loading the defect or size bins files. Error : " + ex.Message));
            }

            return new KlarfSettingsData { RoughBins = defectBins, SizeBins = sizeBins };
        }

        public static void LoadThumbnailColorMapSettings(string filePath)
        {
            ColorMapHelper.ThumbnailColorMap = ColorMapHelper.ColorMaps.FirstOrDefault(x => x.Name == ColorMapSettings.ReadFromXml(filePath)?.ColorMapName);
        }
    }
}
