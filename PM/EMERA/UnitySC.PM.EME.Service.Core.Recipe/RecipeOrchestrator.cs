using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Recipe.Save;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Data;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.EME.Service.Core.Recipe
{
    public class RecipeOrchestrator
    {
        private readonly ILogger<RecipeOrchestrator> _logger;
        private readonly IMessenger _messenger;

        private readonly ICalibrationManager _calibrationManager;
        private readonly ImageProcessing _imageProcessing;
        private readonly RecipeExecution _recipeExecution;
        private readonly RecipeSaving _recipeSaving;
        private readonly IRecipeAcquisitionTemplateComposer _composer;        
        private readonly IReferentialManager _referentialManager;               

        private CancellationTokenSource _tokenSource;

        private int _imageCount;
        private int _totalImages;
        private ConcurrentBag<PositionImageData> _imagesForStitching = new ConcurrentBag<PositionImageData>();
        private RemoteProductionInfo _remoteProductionInfo;
        private IPMDFService _dfSupervisor;

        public RecipeOrchestrator(IMessenger messenger, ILogger<RecipeOrchestrator> logger,
            ICalibrationManager calibrationManager, IReferentialManager referentialManager,RecipeExecution recipeExecution, RecipeSaving recipeSaving,
            IPMDFService dfSupervisor, IRecipeAcquisitionTemplateComposer composer)
        {
            _messenger = messenger;
            _logger = logger;
            _recipeSaving = recipeSaving;
            _recipeExecution = recipeExecution;
            _calibrationManager = calibrationManager;
            _referentialManager = referentialManager;
            _imageProcessing = new ImageProcessing(calibrationManager.GetDistortion());
            _remoteProductionInfo = null;
            _dfSupervisor = dfSupervisor;
            _composer = composer;
            _calibrationManager = calibrationManager;
        }

        public void Start(RecipeAdapter recipe, Identity pmIdentity = null, RemoteProductionInfo remoteProductionInfo = null)
        {
            _remoteProductionInfo = remoteProductionInfo;
            using (_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    _recipeSaving.DeletePreviousImages(recipe);

                    _messenger.Send(RecipeExecutionMessageFactory.CreateStarted(_remoteProductionInfo));
                    _logger.Information($"Recipe {recipe.Name} Execution Started");
                  
                    if (remoteProductionInfo != null)
                    {
                        var waferDiameter = remoteProductionInfo.ProcessedMaterial.WaferDimension;
                        var waferReferentialSettings = _calibrationManager.GetWaferReferentialSettings(waferDiameter);
                        _referentialManager.SetSettings(waferReferentialSettings);
                    }

                    _recipeExecution.PrepareRecipe(recipe);

                    var acquisitionConfiguration = recipe.GetAcquisitionConfiguration();
                    _imageCount = 0;
                    _totalImages = recipe.GetTotalImages();

                    _logger.Information($"{_totalImages} images queued " +
                                       $"({acquisitionConfiguration.NbImagesX} x {acquisitionConfiguration.NbImagesY} images x {recipe.Acquisitions.Count} acquisitions) " +
                                       $"using a {acquisitionConfiguration.MarginPercentage} margin");
                    _messenger.Send(RecipeExecutionMessageFactory.CreateExecuting(_totalImages, _imageCount, _remoteProductionInfo));

                    var acquisitionResults = new List<IAcquisitionImageResult>();
                    recipe.Acquisitions.ForEach(acquisition =>
                    {
                        _recipeExecution.PrepareAcquisition(acquisition, recipe);
                        CheckForCancellationAndThrow(_tokenSource.Token);
                        if (recipe.RunOptions.RunStitchFullImages)
                        {
                            var acquisitionResult = MakeFullImageAcquisition(recipe, acquisition, _tokenSource.Token);
                            acquisitionResults.Add(acquisitionResult);
                        }
                        else
                        {
                            var acquisitionResult = MakeVignettesAcquisition(recipe, acquisition, _tokenSource.Token);
                            acquisitionResults.Add(acquisitionResult);
                        }
                        
                    });

                    if (recipe.IsSaveResultsEnabled)
                    {
                        _logger.Information("Saving result in database.");
                        _recipeSaving.SaveInDatabase(recipe, acquisitionResults);
                    }

                    _recipeSaving.SaveAdaFile(recipe, acquisitionResults);

                    if (remoteProductionInfo != null)
                    {
                        var fileName = _composer.GetAdaFileName(recipe);
                        string adaContent = File.ReadAllText(fileName);
                        _dfSupervisor.SendAda(pmIdentity,
                            remoteProductionInfo.ProcessedMaterial,
                            adaContent, fileName);
                    }
                    _logger.Information($"Recipe {recipe.Name} Execution Finished Successfully");
                    _messenger.Send(RecipeExecutionMessageFactory.CreateFinished(_remoteProductionInfo));
                }
                catch (RecipeCanceledException)
                {
                    _logger.Error($"Recipe {recipe.Name} Execution has been canceled");
                    _messenger.Send(RecipeExecutionMessageFactory.CreateCanceled(_totalImages, _imageCount, _remoteProductionInfo));
                }
                catch (OperationCanceledException)
                {
                    _logger.Error($"Recipe {recipe.Name} Execution has been canceled");
                    _messenger.Send(RecipeExecutionMessageFactory.CreateCanceled(_totalImages, _imageCount, _remoteProductionInfo));
                }
                catch (Exception e)
                {
                    _logger.Error($"Recipe {recipe.Name} Execution failed : {e.Message}");
                    _messenger.Send(RecipeExecutionMessageFactory.CreateFailed(e.Message, _remoteProductionInfo));
                }
                finally
                {
                    _recipeExecution.PostRecipe();
                }
            }
        }

        private VignetteImageResult MakeVignettesAcquisition(RecipeAdapter recipe, AcquisitionSettings acquisition,
            CancellationToken token)
        {
            var pixelSize = acquisition.Filter.PixelSize;
            _logger.Information($"Current filter pixel size : {pixelSize}");

            var acquisitionPath = recipe.GetAcquisitionPath(pixelSize);

            _logger.Information($"Starting {recipe.Strategy} acquisition path");
            var tasks = new List<Task>();
            while (!acquisitionPath.IsLastPosition())
            {
                _logger.Information($"Processing image {_imageCount + 1}/{_totalImages}");
                var newPos = acquisitionPath.NextPosition();
                var image = _recipeExecution.CaptureImage(newPos.Position.Item1, newPos.Position.Item2);

                tasks.Add(Task.Run(() =>
                {
                    var serviceImage = _imageProcessing.Process(recipe.ImageProcessingTypes, image);
                    string imageFilePath = _recipeSaving.SaveImage(recipe, acquisition, serviceImage, newPos);
                    _imageCount++;
                    var thumbnail = ImageProcessing.CreateThumbnail(image);
                    _messenger.Send(RecipeExecutionMessageFactory.CreateExecuting(_totalImages, _imageCount, thumbnail, imageFilePath, _remoteProductionInfo));
                }, token));

                CheckForCancellationAndThrow(token);
            }

            try
            {
                Task.WaitAll(tasks.ToArray(), token);
            }
            catch (AggregateException e)
            {
                throw new Exception(e.InnerExceptions.First().Message);
            }

            return new AcquisitionImageResultBuilder()
                .AddFolderAndBaseName(_recipeSaving.ImageFolderAndBaseName(recipe, acquisition))
                .AddFilter(acquisition.Filter.Type, pixelSize, recipe.ImageProcessingTypes.Contains(ImageProcessingType.ReduceResolution),
                    ImageProcessing.Scale)
                .AddWaferDiameter(recipe.WaferDiameter)
                .AddLightType(acquisition.Light.Type)
                .AddAcquisitionLabel(acquisition.Name)
                .BuildVignetteImageResult(acquisitionPath.NbImagesY, acquisitionPath.NbImagesX);
        }

        private FullImageResult MakeFullImageAcquisition(RecipeAdapter recipe, AcquisitionSettings acquisition,
            CancellationToken token)
        {
            var pixelSize = acquisition.Filter.PixelSize;
            double xAngle = _calibrationManager.GetAxisOrthogonalityCalibrationData().AngleX.Degrees;
            double yAngle = _calibrationManager.GetAxisOrthogonalityCalibrationData().AngleY.Degrees;

            _logger.Information($"Current filter pixel size : {pixelSize}");

            var acquisitionPath = recipe.GetAcquisitionPath(pixelSize);

            _logger.Information($"Starting {recipe.Strategy} acquisition path");
            var tasks = new List<Task>();
            var imagesForStitching = new ConcurrentBag<PositionImageData>();
            while (!acquisitionPath.IsLastPosition())
            {
                _logger.Information($"Processing image {_imageCount + 1}/{_totalImages}");
                var newPos = acquisitionPath.NextPosition();

                var rotatedPos = new XYPosition(new WaferReferential(), newPos.Position.Item1, newPos.Position.Item2);
                Angle rotAngle = new Angle((xAngle + yAngle) / 2, AngleUnit.Degree);
                MathTools.ApplyAntiClockwiseRotation(rotAngle, rotatedPos, new XYPosition(new WaferReferential(), 0, 0));

                var image = _recipeExecution.CaptureImage(rotatedPos.X, rotatedPos.Y);

                tasks.Add(Task.Run(() =>
                {
                    var serviceImage = _imageProcessing.Process(recipe.ImageProcessingTypes, image);
                    var imageData = AlgorithmLibraryUtils.CreateImageData(serviceImage);
                    var posImageData = new PositionImageData(imageData.ByteArray, imageData.Width, imageData.Height, imageData.Type);
                    //Centroid / Position should be the same unit as the Scale / Pixel size
                    posImageData.Centroid = new Point2d(newPos.Position.Item1, newPos.Position.Item2);
                    posImageData.Scale = new PixelSize(pixelSize.Millimeters / ImageProcessing.Scale, pixelSize.Millimeters / ImageProcessing.Scale);
                    imagesForStitching.Add(posImageData);
                    _imageCount++;
                    _messenger.Send(RecipeExecutionMessageFactory.CreateExecuting(_totalImages, _imageCount, _remoteProductionInfo));
                }, token));

                CheckForCancellationAndThrow(token);
            }

            try
            {
                Task.WaitAll(tasks.ToArray(), token);
                var stitchedPosImageData = Stitcher.StitchImages(imagesForStitching.ToList());
                var stitchedImage = AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(stitchedPosImageData);
                var thumbnail = ImageProcessing.CreateThumbnail(stitchedImage);
                string imageFilePath = _recipeSaving.SaveImage(recipe, acquisition, stitchedImage);
                _messenger.Send(RecipeExecutionMessageFactory.CreateExecuting(_totalImages, _imageCount, thumbnail, imageFilePath, _remoteProductionInfo));
            }
            catch (AggregateException e)
            {
                throw new Exception(e.InnerExceptions.First().Message);
            }

            return new AcquisitionImageResultBuilder()
                .AddFolderAndBaseName(_recipeSaving.ImageFolderAndBaseName(recipe, acquisition))
                .AddFilter(acquisition.Filter.Type, pixelSize, recipe.ImageProcessingTypes.Contains(ImageProcessingType.ReduceResolution),
                    ImageProcessing.Scale)
                .AddWaferDiameter(recipe.WaferDiameter)
                .AddLightType(acquisition.Light.Type)
                .AddAcquisitionLabel(acquisition.Name)
                .BuildFullImageResult();
        }
            public void Cancel()
        {
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
        }

        private static void CheckForCancellationAndThrow(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new RecipeCanceledException();
        }
    }
}
