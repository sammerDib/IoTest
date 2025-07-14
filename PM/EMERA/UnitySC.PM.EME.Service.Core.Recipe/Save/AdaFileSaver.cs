using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Shared.DateTimeProvider;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Data.Ada;
using UnitySC.Shared.Data;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class AdaFileSaver : IAdaFileSaver
    {
        private const string DefaultValue = "default";
        private const int PhotoLumModuleId = 9;
        private readonly ICalibrationManager _calibrationManager;
        private readonly IRecipeAcquisitionTemplateComposer _composer;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly PMConfiguration _pmConfiguration;

        public AdaFileSaver(PMConfiguration pmConfiguration, IRecipeAcquisitionTemplateComposer composer,
            IDateTimeProvider dateTimeProvider, ICalibrationManager calibrationManager)
        {
            _pmConfiguration = pmConfiguration;
            _composer = composer;
            _dateTimeProvider = dateTimeProvider;
            _calibrationManager = calibrationManager;
        }

        public void GenerateFile(RecipeAdapter recipe, List<IAcquisitionImageResult> acquisitionResults)
        {
            string adaFileName = _composer.GetAdaFileName(recipe);

            if (File.Exists(adaFileName))
                File.Delete(adaFileName);

            var writer = new AdaWriter(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey, adaFileName);

            WriteInfo(recipe, writer);
            WriteDistortionCalibration(writer);
            
            foreach (var item in acquisitionResults.Select((result, index) => new { index,  result }))
            {
                switch (item.result)
                {
                    case VignetteImageResult vignetteResult:
                        writer.AddMosaicImage(vignetteResult.ResultType, vignetteResult.NbColumns, vignetteResult.NbLines, vignetteResult.FolderName, vignetteResult.BaseName);
                        break;
                    case FullImageResult fullImageResult:
                        writer.AddFullImage(fullImageResult.ResultType, fullImageResult.FolderName + fullImageResult.BaseName + ".tiff");
                        break;
                }
                
                writer.WriteMetaData(item.result.ResultType, "ModuleID", PhotoLumModuleId);
                writer.WriteMetaData(item.result.ResultType, "ChannelID", item.index.ToString());
                writer.WriteMetaData(item.result.ResultType, "ChamberID", _pmConfiguration.ChamberId);
                writer.WriteRectangularMatrix(item.result.ResultType, item.result.PixelSize.Micrometers, item.result.PixelSize.Micrometers, item.result.WaferCenter, item.result.WaferCenter, 0);
            }

            writer.Close();
        }

        private void WriteInfo(RecipeAdapter recipe, AdaWriter writer)
        {
            var material = recipe.RemoteProductionInfo?.ProcessedMaterial;

            writer.WriteInfoWafer("StartProcess",
                _dateTimeProvider.GetDateTimeNow().ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture));

            writer.WriteInfoWafer("ADCOutputDataFilePath", DefaultValue);
            writer.WriteInfoWafer("SummaryFile", DefaultValue);
            writer.WriteInfoWafer("CarrierStatus",
                material != null ? ((int)material.JobPosition).ToString() : JobPosition.FirstAndLast.ToString());
            writer.WriteInfoWafer("WaferID", material?.WaferBaseName ?? "0-000.00");
            writer.WriteInfoWafer("SlotID", material?.SlotID.ToString() ?? "1");
            writer.WriteInfoWafer("LoadPortID", material?.LoadportID.ToString() ?? "1");
            writer.WriteInfoWafer("StepID", recipe.StepId);
            writer.WriteInfoWafer("DeviceID", material?.DeviceID ?? DefaultValue);
            writer.WriteInfoWafer("JobID", material?.ProcessJobID ?? DefaultValue);
            writer.WriteInfoWafer("LotID", material?.LotID ?? DefaultValue);
            writer.WriteInfoWafer("ToolRecipe", recipe.RemoteProductionInfo?.DFRecipeName ?? DefaultValue);
            writer.WriteInfoWafer("ADCRecipeFileName", DefaultValue);
            writer.WriteInfoWafer("CorrectorsEnabled", recipe.RunOptions.RunBwa ? 1 : 0);
            writer.WriteInfoWafer("WaferType", recipe?.WaferCategory ?? "Notch 200mm"); 
        }

        private void WriteDistortionCalibration(AdaWriter writer)
        {
            var distortionData = _calibrationManager.GetDistortion();

            WriteMatrixInfo("NewOptimalCameraMatrix", distortionData.NewOptimalCameraMat, writer);
            WriteMatrixInfo("CameraMatrix", distortionData.CameraMat, writer);
            WriteMatrixInfo("DistortionMatrix", distortionData.DistortionMat, writer);
            WriteMatrixInfo("TranslationVector", distortionData.TranslationVec, writer);
            WriteMatrixInfo("RotationVector", distortionData.RotationVec, writer);
        }

        private static void WriteMatrixInfo(string prefix, IEnumerable<double> matrix, AdaWriter writer)
        {
            matrix
                .Select((value, i) => new { Index = i + 1, Value = value })
                .ToList()
                .ForEach(x => writer.WriteInfoWafer($"{prefix}_{x.Index}", x.Value));
        }
    }
}
