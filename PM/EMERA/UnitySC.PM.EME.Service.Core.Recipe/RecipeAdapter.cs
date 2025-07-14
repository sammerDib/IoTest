using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Recipe.AcquisitionPath;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Recipe
{
    public class RecipeAdapter
    {
        private readonly RecipeConfiguration _recipeConfiguration;
        private readonly CameraInfo _cameraInfo;

        public RecipeAdapter(EMERecipe recipe, RecipeConfiguration recipeConfiguration, EmeHardwareManager hardware,
            IEmeraCamera camera = null, ICalibrationManager calibrationManager = null, RemoteProductionInfo remoteProductionInfo = null)
        {
            Info = recipe;
            _recipeConfiguration = recipeConfiguration;
            _cameraInfo = (camera ?? ClassLocator.Default.GetInstance<IEmeraCamera>()).GetCameraInfo();

            Step = recipe.Step.Name;
            StepId = recipe.StepId.HasValue ? recipe.StepId.Value.ToString() : "default";
            Product = recipe.Step.Product.Name;
            WaferCategory = recipe.Step.Product.WaferCategory.Name;
            WaferDiameter = recipe.Step.Product.WaferCategory.DimentionalCharacteristic.Diameter;

            RunOptions = new RunOptions(recipe.Execution.RunAutoFocus, recipe.Execution.RunBwa, recipe.Execution.RunStitchFullImage);
            ImageProcessingTypes = new HashSet<ImageProcessingType>();
            if (recipe.Execution.ReduceResolution)
                ImageProcessingTypes.Add(ImageProcessingType.ReduceResolution);
            if (recipe.Execution.CorrectDistortion)
                ImageProcessingTypes.Add(ImageProcessingType.CorrectDistortion);
            if (recipe.Execution.ConvertTo8Bits)
                ImageProcessingTypes.Add(ImageProcessingType.ConvertTo8Bits);
            if (recipe.Execution.ReduceResolution)
                ImageProcessingTypes.Add(ImageProcessingType.ReduceResolution);

            Strategy = recipe.Execution.Strategy;
            calibrationManager = calibrationManager ?? ClassLocator.Default.GetInstance<ICalibrationManager>();
            Acquisitions = recipe.Acquisitions
                .Select(acquisition => new AcquisitionSettings(acquisition, calibrationManager, hardware)).ToList();

            IsSaveResultsEnabled = recipe.IsSaveResultsEnabled;
            RecipeExecutionDateTime = DateTime.Now;
            RemoteProductionInfo = remoteProductionInfo;
        }

        public RecipeInfo Info { get; }

        public string Name => Info.Name;

        public string ActorType => Info.ActorType.ToString();

        public string Step { get; set; }
        public string StepId { get; }
        public string Product { get; set; }
        public string WaferCategory { get; set; }
        public Length WaferDiameter { get; }

        public RunOptions RunOptions { get; }
        public HashSet<ImageProcessingType> ImageProcessingTypes { get; }
        public AcquisitionStrategy Strategy  { get; }
        public List<AcquisitionSettings> Acquisitions { get; set; }
        public DateTime RecipeExecutionDateTime { get; set; } = DateTime.MinValue;

        private string _customSavePath;

        public string CustomSavePath
        {
            get => _customSavePath;
            set => _customSavePath = Path.Combine(value, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        }

        public bool IsSaveResultsEnabled { get; set; }
        public RemoteProductionInfo RemoteProductionInfo { get; set; }


        public AcquisitionConfiguration GetAcquisitionConfiguration()
        {
            return _recipeConfiguration.GetAcquisitionConfiguration(WaferDiameter.Millimeters);
        }

        public int GetTotalImages()
        {
            var acquisitionConfiguration = GetAcquisitionConfiguration();
            return acquisitionConfiguration.NbImagesX * acquisitionConfiguration.NbImagesY * Acquisitions.Count;
        }

        public AcquisitionPath.AcquisitionPath GetAcquisitionPath(Length pixelSize)
        {
            double cameraWidthInMm = _cameraInfo.Width * pixelSize.Millimeters;
            double cameraHeightInMm = _cameraInfo.Height * pixelSize.Millimeters;

            var configuration = GetAcquisitionConfiguration();
            double totalAcquisitionSideInMm = WaferDiameter.Millimeters * (1.0 + configuration.MarginPercentage);

            if (Strategy == AcquisitionStrategy.Serpentine)
                return new SerpentinePath(totalAcquisitionSideInMm, cameraWidthInMm, cameraHeightInMm,
                    configuration.NbImagesX, configuration.NbImagesY);
            return new TypewriterPath(totalAcquisitionSideInMm, cameraWidthInMm, cameraHeightInMm,
                configuration.NbImagesX, configuration.NbImagesY);
        }
    }
}
