using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Service.Core.Recipe
{
    public class RecipeAdapterBuilder
    {
        private readonly ICalibrationManager _calibrationManager;
        private readonly RecipeConfiguration _recipeConfiguration;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly EmeHardwareManager _hardware;
        private readonly IEmeraCamera _camera;
        private RemoteProductionInfo _remoteProductionInfo;

        public RecipeAdapterBuilder(ILogger logger, IMessenger messenger, IEmeraCamera camera = null,
            EmeHardwareManager hardware = null, ICalibrationManager calibrationManager = null,
            RecipeConfiguration recipeConfiguration = null)
        {
            _logger = logger;
            _messenger = messenger;
            _camera = camera ?? ClassLocator.Default.GetInstance<EmeraCamera>();
            _hardware = hardware ?? ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _recipeConfiguration = recipeConfiguration ?? ClassLocator.Default.GetInstance<RecipeConfiguration>();
            _calibrationManager = calibrationManager ?? ClassLocator.Default.GetInstance<ICalibrationManager>();
            _remoteProductionInfo = null;
        }

        public RecipeAdapter ValidateAndBuild(EMERecipe recipe, string customSavePath = null, RemoteProductionInfo remoteProductionInfo = null)
        {
            _remoteProductionInfo = remoteProductionInfo;
            ValidateCalibrations();
            ValidateFilters(recipe.Acquisitions);
            ValidateLights(recipe.Acquisitions.Select(acquisition => acquisition.LightDeviceId));

            var adaptedRecipe = new RecipeAdapter(recipe, _recipeConfiguration, _hardware, _camera, _calibrationManager, remoteProductionInfo);
            if (!customSavePath.IsNullOrEmpty())
            {
                adaptedRecipe.CustomSavePath = customSavePath;
            }

            return adaptedRecipe;
        }

        private void ValidateCalibrations()
        {
            if (_calibrationManager.Calibrations.Count() >= _calibrationManager.GetCalibrationTypes().Count)
            {
                return;
            }

            string message = "Not all calibration data are found.";
            _logger.Error(message);
            _messenger.Send(RecipeExecutionMessageFactory.CreateFailed(message, _remoteProductionInfo));
            throw new Exception(message);
        }

        private void ValidateFilters(List<Acquisition> acquisitions)
        {
            var availableFilters = _calibrationManager.GetFilters();
            var invalidAcquisition = acquisitions.FirstOrDefault(acquisition =>
            {
                return availableFilters.All(filter => filter.Type != acquisition.Filter);
            });
            if (invalidAcquisition == null)
            {
                return;
            }

            string message =
                $"The filter {invalidAcquisition.Filter} in the acquisition does not match any of the available filters.";
            _logger.Error(message);
            _messenger.Send(RecipeExecutionMessageFactory.CreateFailed(message, _remoteProductionInfo));
            throw new Exception(message);
        }

        private void ValidateLights(IEnumerable<string> lights)
        {
            var availableLights = _hardware.EMELights;
            foreach (string lightId in lights)
            {
                if (!availableLights.ContainsKey(lightId))
                {
                    string message =
                        $"The light {lightId} in the acquisition does not match any of the available lights.";
                    _logger.Error(message);
                    _messenger.Send(RecipeExecutionMessageFactory.CreateFailed(message,_remoteProductionInfo));
                    throw new Exception(message);
                }
            }
        }
    }
}
