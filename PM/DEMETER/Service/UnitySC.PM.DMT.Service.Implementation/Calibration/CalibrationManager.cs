using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Calibration;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Implementation.Extensions;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
using UnitySC.PM.DMT.Service.Implementation.Wrapper;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.CurvatureDynamics;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Implementation.Calibration
{
    public class CalibrationManager : ICalibrationManager
    {
        private const string DMTCalibrationInputsFileName = "DMTCalibrationInputs.xml";

        private const string FrontGlobalTopoSystemCalibFileName = "FrontGlobalTopoSystemCalibration.xml";
        private const string BackGlobalTopoSystemCalibFileName = "BackGlobalTopoSystemCalibration.xml";
        private const string FrontGlobalTopoCameraCalibFileName = "FrontGlobalTopoCameraCalibration.xml";
        private const string BackGlobalTopoCameraCalibFileName = "BackGlobalTopoCameraCalibration.xml";
        private const string FrontSystemUniformityCalibFileName = "FrontSystemUniformityCalibration.tif";
        private const string BackSystemUniformityCalibFileName = "BackSystemUniformityCalibration.tif";
        private const string FrontPerspectiveCalibrationFileName = "FrontPerspectiveCalibration.psd";
        private const string BackPerspectiveCalibrationFileName = "BackPerspectiveCalibration.psd";
        private const string FrontHighAngleDarkFieldMaskFileName = "FrontHighAngleDarkFieldMask.bmp";
        private const string BackHighAngleDarkFieldMaskFileName = "BackHighAngleDarkFieldMask.bmp";
        private const string FrontExposureTimeCalibrationFileName = "FrontExposureTimeCalibration.xml";
        private const string BackExposureTimeCalibrationFileName = "BackExposureTimeCalibration.xml";
        private const string FrontCurvatureDynamicsCalibrationFileName = "FrontCurvatureDynamicsCalibration.xml";
        private const string BackCurvatureDynamicsCalibrationFileName = "BackCurvatureDynamicsCalibration.xml";
        private const string FrontDeadPixelsCalibrationFileName = "FrontDeadPixelsCalibration.dpml";
        private const string BackDeadPixelsCalibrationFileName = "BackDeadPixelsCalibration.dpml";

        private readonly Dictionary<Side, string> _globalTopoSystemCalibFileNamesBySide;
        private readonly Dictionary<Side, string> _globalTopoCameraCalibFileNamesBySide;
        private readonly Dictionary<Side, string> _systemUniformityCalibFileNamesBySide;
        private readonly Dictionary<Side, string> _perspectiveCalibFileNamesBySide;
        private readonly Dictionary<Side, string> _highAngleDarkFieldMaskFileNamesBySide;
        private readonly Dictionary<Side, string> _exposureTimeCalibFileNamesBySide;
        private readonly Dictionary<Side, string> _curvatureDynamicsCalibFileNamesBySide;
        private readonly Dictionary<Side, string> _deadPixelsCalibFileNamesBySide;
        private readonly Dictionary<Side, GlobalTopoCameraCalibrationResult> _globalTopoCameraCalibBySide;
        private readonly Dictionary<Side, GlobalTopoSystemCalibrationResult> _globalTopoSystemCalibBySide;
        private readonly Dictionary<Side, ImageData> _systemUniformityCalibBySide;
        private readonly Dictionary<Side, ExposureCalibration> _exposureCalibBySide;
        private readonly Dictionary<Side, CurvatureDynamicsCalibrationData> _curvatureCalibBySide;
        private readonly Dictionary<Side, DMTTransform> _perspectiveCalibrationBySide;
        private readonly Dictionary<Side, USPImageMil> _highAngleDarkFieldMaskBySide;

        private readonly string[] _globalTopoCalibrationImagesName =
            {
                "Reference Position",
                "Tilt the wafer to the Right",
                "Tilt the wafer to the Left",
                "Tilt the wafer to the Top",
                "Tilt the wafer to the Bottom",
                "Change wafer height",
            };

        private readonly ILogger _logger;
        private readonly FringeManager _fringeManager;
        private readonly AlgorithmManager _algorithmManager;
        private readonly DMTHardwareManager _hardwareManager;
        private readonly IDMTServiceConfigurationManager _serviceConfigurationManager;
        private readonly IGlobalStatusServer _globalStatusServer;
        private readonly Dictionary<Side, Dictionary<string, USPImageMil>> _cameraCalibrationImages;
        private readonly Dictionary<Side, USPImageMil> _brightFieldImagesBySide;
        private readonly Dictionary<Side, DeadPixelsManager> _deadPixelsManagers;
        private readonly Dictionary<Side, Dictionary<DeadPixelTypes, ServiceImageWithDeadPixels>>  _deadPixelsImagesBySideAndType;
        private Dictionary<Side, EllipseMaskInput> _ellipseMasksBySide;
        private DMTCalibrationInputs _dmtCalibrationInputs;
        private bool _areFlowsSimulated;

        public CalibrationManager(ILogger logger, AlgorithmManager algorithmManager,
            FringeManager fringeManager, DMTHardwareManager hardwareManager, IDMTServiceConfigurationManager serviceConfigurationManager, IGlobalStatusServer globalStatusServer)
        {
            _logger = logger;
            _fringeManager = fringeManager;
            _algorithmManager = algorithmManager;
            _hardwareManager = hardwareManager;
            _serviceConfigurationManager = serviceConfigurationManager;
            _globalStatusServer = globalStatusServer;
            _areFlowsSimulated = _serviceConfigurationManager.FlowsAreSimulated;

            _deadPixelsManagers = new Dictionary<Side, DeadPixelsManager>();
            _deadPixelsImagesBySideAndType = new Dictionary<Side, Dictionary<DeadPixelTypes, ServiceImageWithDeadPixels>>
            {
                { Side.Front, new Dictionary<DeadPixelTypes, ServiceImageWithDeadPixels> 
                    { { DeadPixelTypes.WhitePixel, null }, { DeadPixelTypes.BlackPixel, null } }
                },
                { Side.Back, new Dictionary<DeadPixelTypes, ServiceImageWithDeadPixels> 
                    { { DeadPixelTypes.WhitePixel, null }, {DeadPixelTypes.BlackPixel, null } }
                }
            };
            _cameraCalibrationImages = new Dictionary<Side, Dictionary<string, USPImageMil>>(2);
            _cameraCalibrationImages[Side.Front] = new Dictionary<string, USPImageMil>(_globalTopoCalibrationImagesName.Length);
            _cameraCalibrationImages[Side.Back] = new Dictionary<string, USPImageMil>(_globalTopoCalibrationImagesName.Length);
            _brightFieldImagesBySide = new Dictionary<Side, USPImageMil>(2);
            _globalTopoCameraCalibBySide = new Dictionary<Side, GlobalTopoCameraCalibrationResult>(2);
            _globalTopoSystemCalibBySide = new Dictionary<Side, GlobalTopoSystemCalibrationResult>(2);
            _systemUniformityCalibBySide = new Dictionary<Side, ImageData>(2);
            _exposureCalibBySide = new Dictionary<Side, ExposureCalibration>(2);
            _curvatureCalibBySide = new Dictionary<Side, CurvatureDynamicsCalibrationData>(2);
            _perspectiveCalibrationBySide = new Dictionary<Side, DMTTransform>(2)
            {
                { Side.Front, null }, { Side.Back, null }
            };
            _highAngleDarkFieldMaskBySide = new Dictionary<Side, USPImageMil>(2);
            _globalTopoSystemCalibFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontGlobalTopoSystemCalibFileName }, { Side.Back, BackGlobalTopoSystemCalibFileName }
            };
            _globalTopoCameraCalibFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontGlobalTopoCameraCalibFileName }, { Side.Back, BackGlobalTopoCameraCalibFileName }
            };
            _systemUniformityCalibFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontSystemUniformityCalibFileName }, { Side.Back, BackSystemUniformityCalibFileName }
            };
            _perspectiveCalibFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontPerspectiveCalibrationFileName }, { Side.Back, BackPerspectiveCalibrationFileName }
            };
            _highAngleDarkFieldMaskFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontHighAngleDarkFieldMaskFileName }, { Side.Back, BackHighAngleDarkFieldMaskFileName }
            };
            _exposureTimeCalibFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontExposureTimeCalibrationFileName }, { Side.Back, BackExposureTimeCalibrationFileName }
            };
            _curvatureDynamicsCalibFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontCurvatureDynamicsCalibrationFileName }, { Side.Back, BackCurvatureDynamicsCalibrationFileName }
            };
            _deadPixelsCalibFileNamesBySide = new Dictionary<Side, string>
            {
                { Side.Front, FrontDeadPixelsCalibrationFileName }, { Side.Back, BackDeadPixelsCalibrationFileName }
            };
        }

        public void Init()
        {
            InitCalibrationFolders();
            InitCalibrations();
            InitCalibrationInputs();
            InitializeDeadPixelsManagers();
            CheckFringesCurvature();
        }

        public void Shutdown()
        {
            foreach (var calibration in _perspectiveCalibrationBySide.Values)
            {
                calibration.Dispose();
            }
            _perspectiveCalibrationBySide.Clear();
            
            foreach (var imageBySideDict in _cameraCalibrationImages.Values)
            {
                foreach (var image in imageBySideDict.Values)
                {
                    image.Dispose();
                }
                imageBySideDict.Clear();
            }
            _cameraCalibrationImages.Clear();

            foreach (var brightFieldImage in _brightFieldImagesBySide.Values)
            {
                brightFieldImage.Dispose();
            }
            _brightFieldImagesBySide.Clear();
        }

        public ImageData GetUniformityCorrectionCalibImageBySide(Side side)
        {
            _systemUniformityCalibBySide.TryGetValue(side, out var uniformityCalibration);
            return uniformityCalibration;
        }

        public DMTCalibrationInputs GetCalibrationInputs()
        {
            return _dmtCalibrationInputs;
        }

        public EllipseMaskInput GetEllipseMasksBySide(Side side)
        {
            _ellipseMasksBySide.TryGetValue(side, out EllipseMaskInput ellipseMaskInput);
            return ellipseMaskInput;
        }

        public DeadPixelsManager GetDeadPixelsManagerBySide(Side side)
        {
            _deadPixelsManagers.TryGetValue(side, out DeadPixelsManager deadPixelsManager);
            return deadPixelsManager;
        }

        public ExposureCalibration GetExposureCalibrationBySide(Side side)
        {
            _exposureCalibBySide.TryGetValue(side, out ExposureCalibration exposureCalib);
            return exposureCalib;
        }

        public GlobalTopoCameraCalibrationResult GetGlobalTopoCameraCalibrationResultBySide(Side side)
        {
            _globalTopoCameraCalibBySide.TryGetValue(side, out GlobalTopoCameraCalibrationResult cameraCalib);
            return cameraCalib;
        }

        public GlobalTopoSystemCalibrationResult GetGlobalTopoSystemCalibrationResultBySide(Side side)
        {
            _globalTopoSystemCalibBySide.TryGetValue(side, out GlobalTopoSystemCalibrationResult systemCalib);
            return systemCalib;
        }

        public CurvatureDynamicsCalibrationData GetCurvatureDynamicsCalibrationBySide(Side side)
        {
            _curvatureCalibBySide.TryGetValue(side, out CurvatureDynamicsCalibrationData curvatureCalib);
            return curvatureCalib;
        }

        public DMTTransform GetPerspectiveCalibrationForSide(Side side)
        {
            _perspectiveCalibrationBySide.TryGetValue(side, out var calibration);
            return calibration;
        }

        public string GetPerspectiveCalibrationFullFilePathForSide(Side side)
        {
            return Path.Combine(_serviceConfigurationManager.CalibrationOutputFolderPath, _perspectiveCalibFileNamesBySide[side]);
        }

        public void ReloadPerspectiveCalibrationForSide(Side side)
        {
            _perspectiveCalibrationBySide[side].Dispose();
            _perspectiveCalibrationBySide.Remove(side);
            string calibrationFullFilePath = Path.Combine(_serviceConfigurationManager.CalibrationOutputFolderPath, _perspectiveCalibFileNamesBySide[side]);
            LoadPerspectiveCalibrationFromFile(calibrationFullFilePath, side);
        }

        public USPImageMil GetHighAngleDarkFieldMaskForSide(Side side)
        {
            _highAngleDarkFieldMaskBySide.TryGetValue(side, out var image);
            return image;
        }

        public void SetHighAngleDarkFieldMaskForSide(Side side, USPImageMil image)
        {
            _highAngleDarkFieldMaskBySide[side] = image;
            string fullFilePath = Path.Combine(_serviceConfigurationManager.CalibrationOutputFolderPath, _highAngleDarkFieldMaskFileNamesBySide[side]);
            image.Save(fullFilePath);
        }

        public void UpdateAndSaveDeadPixels(Side side)
        {
            var deadPixelManager = _deadPixelsManagers[side];
            if (_deadPixelsImagesBySideAndType[side][DeadPixelTypes.WhitePixel] is null ||
                _deadPixelsImagesBySideAndType[side][DeadPixelTypes.BlackPixel] is null)
            {
                throw new Exception("Cannot update dead pixels since one source image is missing");
            }
            deadPixelManager.WhiteDeadPixels = _deadPixelsImagesBySideAndType[side][DeadPixelTypes.WhitePixel].DeadPixels;
            deadPixelManager.BlackDeadPixels = _deadPixelsImagesBySideAndType[side][DeadPixelTypes.BlackPixel].DeadPixels;
            string calibrationFileName = _deadPixelsCalibFileNamesBySide[side];
            string fullFilePath = Path.Combine(_serviceConfigurationManager.CalibrationOutputFolderPath, calibrationFileName);
            deadPixelManager.WriteBackXMLConfig(fullFilePath);
        }

        public bool DoesDeadPixelsCalibrationExist(Side side)
        {
            return _deadPixelsManagers.TryGetValue(side, out DeadPixelsManager deadPixelsManager)
                   && deadPixelsManager != null;
        }

        public void SetDeadPixelsImageForSideAndType(Side side, ServiceImageWithDeadPixels imageWithDeadPixels)
        {
            _deadPixelsImagesBySideAndType[side][imageWithDeadPixels.DeadPixelType] = imageWithDeadPixels;
        }

        public string[] GetGlobalTopoCameraCalibrationImagesName()
        {
            return _globalTopoCalibrationImagesName;
        }

        public async Task<float> CalibrateCurvatureDynamicsAsync(Side side, DMTDeflectometryAcquisitionFlowTask acquisitionFlowTask)
        {
            _curvatureCalibBySide[side] = await ExecuteCurvatureDynamicsCalibrationAsync(side, acquisitionFlowTask);
            SaveCurvatureDynamicsCalibration(side);

            return (float)_curvatureCalibBySide[side].DynamicsCoefficient;
        }

        public double CalibrateExposure(Side side, ExposureMatchingInputs inputs = null)
        {
            _exposureCalibBySide[side] = ExecuteExposureCalibration(side, inputs);
            SaveExposureCalibration(side);
            UpdateAndSaveCalibrationInput(side);
            return _exposureCalibBySide[side].ExposureCorrectionCoef;
        }

        public ExposureMatchingInputs GetGoldenValues(Side side)
        {
            _exposureCalibBySide[side] = ComputeGoldenValues(side);
            SaveExposureCalibration(side);
            UpdateAndSaveCalibrationInput(side);
            var inputs = new ExposureMatchingInputs
            {
                GoldenValues = _exposureCalibBySide.Values.Select(value => value.ExposureMatchingGoldenValues).ToList(),
                AcquisitionExposureTimeMs = _dmtCalibrationInputs.ExposureMatchingInputs.AcquisitionExposureTimeMs,
            };
            return inputs;
        }

        public void CalibrateGlobalTopoSystem(Side side, List<int> periods, USPImageMil brightFieldImage, DMTDeflectometryAcquisitionFlowTask acquisitionFlowTask, Length waferDiameter)
        {
            var image = ImageUtils.CreateImageDataFromUSPImageMil(brightFieldImage);
            ComputeUnwrappedPhaseMapXAndY(side, periods, acquisitionFlowTask, waferDiameter, out var unwrappedPhaseMapXImage, out var unwrappedPhaseMapYImage);
            _globalTopoSystemCalibBySide[side] = ExecuteSystemCalibration(side, periods, image, unwrappedPhaseMapXImage, unwrappedPhaseMapYImage);
            SaveGlobalTopoSystemCalibration(side);
        }

        public void SaveGlobalTopoSystemCalibrationBackup(Side side)
        {
            try
            {
                var calibBackupFolder = _serviceConfigurationManager.CalibrationOutputBackupFolderPath;
                Directory.CreateDirectory(calibBackupFolder);
                var now = DateTime.Now;
                var filename = $"{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}" + _globalTopoSystemCalibFileNamesBySide[side];
                string calibrationFile = Path.Combine(calibBackupFolder, filename);
                XML.Serialize(_globalTopoSystemCalibBySide[side], calibrationFile);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Failed to save the globalTopo camera calibration file : {ex.Message}");
                throw;
            }
        }

        public void CalibrateGlobalTopoCamera(Side side)
        {
            _globalTopoCameraCalibBySide[side] = ExecuteCameraCalibration(side);
            SaveGlobalTopoCameraCalibration(side);
        }

        public void SaveGlobalTopoCameraCalibrationBackup(Side side)
        {
            try
            {
                var calibBackupFolder = _serviceConfigurationManager.CalibrationOutputBackupFolderPath;
                Directory.CreateDirectory(calibBackupFolder);
                var now = DateTime.Now;
                var filename = $"{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}" + _globalTopoCameraCalibFileNamesBySide[side];
                string calibrationFile = Path.Combine(calibBackupFolder, filename);
                XML.Serialize(_globalTopoCameraCalibBySide[side], calibrationFile);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Failed to save the globalTopo camera calibration file : {ex.Message}");
                throw;
            }
        }

        public void CalibrateSystemUniformityAsync(Side side)
        {
            _systemUniformityCalibBySide[side] = ExecuteSystemUniformityCalibration(side);
            SaveSystemUniformityCalibrationAsync(side);
        }
        
        public void SetBrightFieldImageForSide(Side side, USPImageMil image)
        {
            if (_brightFieldImagesBySide.TryGetValue(side, out var previousImage))
            {
                previousImage.Dispose();
            }
            _brightFieldImagesBySide[side] = image;
        }

        public void RemoveBrightFieldImage(Side side)
        {
            try
            {
                _brightFieldImagesBySide[side]?.Dispose();
                _brightFieldImagesBySide.Remove(side);
            }
            catch
            {
                // Nothing to do
            }
        }

        public ServiceImage StoreGlobalTopoCameraCalibrationImage(Side side, USPImageMil calibrationImage, string imageName, double exposureTimeMs)
        {
            RemoveGlobalTopoCameraCalibrationImage(side, imageName);
            _cameraCalibrationImages[side][imageName] = calibrationImage;
            return _cameraCalibrationImages[side][imageName].ToServiceImage();
        }

        public void RemoveGlobalTopoCameraCalibrationImage(Side side, string imageName)
        {
            try
            {
                _cameraCalibrationImages[side][imageName].Dispose();
                _cameraCalibrationImages[side]?.Remove(imageName);
            }
            catch
            {
                // Nothing to do
            }
        }

        public void ClearGlobalTopoCameraCalibrationImages()
        {
            foreach (var imagesForOneSide in _cameraCalibrationImages.Values)
            {
                foreach (var image in imagesForOneSide.Values)
                {
                    image.Dispose();
                }
                imagesForOneSide.Clear();
            }
        }

        public bool DoesGlobalTopoCameraCalibrationExist(Side side)
        {
            return _globalTopoCameraCalibBySide.ContainsKey(side);
        }

        public bool DoesGlobalTopoSystemCalibrationExist(Side side)
        {
            return _globalTopoSystemCalibBySide.ContainsKey(side);
        }
        
        // CameraManager must be passed from the service call to avoid circular dependency
        public async Task<USPImageMil> AcquireBrightFieldImageForCalibration(Side side ,double exposureTimeMs, DMTCameraManager cameraManager)
        {
            var measureType = MeasureType.BrightFieldMeasure;
            var screenDisplay = AcquisitionScreenDisplayImage.Color;
            var input = new AcquireOneImageInput(side, side, exposureTimeMs, measureType, screenDisplay, Colors.White);
            var flow = new AcquireOneImageFlow(input, cameraManager, _hardwareManager);
            var flowTask = new FlowTask<AcquireOneImageInput, AcquireOneImageResult>(flow);
            flowTask.Start();
            var result = await flowTask;
            if (result.Status.State != FlowState.Success)
            {
                throw new Exception($"Failed to acquire bright-field image for system calibration");
            }

            if (CheckSaturatedPixels(result.AcquiredImage))
            {
                var message = $"Acquired image is saturated";
                _logger.Error(message);
                ClassLocator.Default.GetInstance<IGlobalStatusServer>().AddMessage(new Message(MessageLevel.Error, message));
                throw new Exception(message);
            }
            return result.AcquiredImage;
        }

        // CameraManager must be passed from the service call to avoid circular dependency
        public DMTDeflectometryAcquisitionFlowTask StartAcquireCurvatureDynamicsCalibrationImages(Side side, DMTCameraManager cameraManager)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            int minFringePeriod = _fringeManager.Fringes.Min(f => f.Period);
            var smallestFringe = _fringeManager.Fringes.First(f => f.Period == minFringePeriod);
            var firstFringeImage = _fringeManager.GetFringeImageDict(side, smallestFringe)[FringesDisplacement.X][minFringePeriod][0];
            var curvatureDynamicsCalibrationConfiguration = ClassLocator.Default.
                GetInstance<IFlowsConfiguration>()
                .Flows.OfType<CurvatureDynamicsCalibrationConfiguration>()
                .First();
            smallestFringe.NbImagesPerDirection = curvatureDynamicsCalibrationConfiguration.NumberOfPhaseShifts;
            var acquisitionFlowTask = CreateAcquisitionFlowTask(side, smallestFringe, firstFringeImage, cameraManager);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);

            return acquisitionFlowTask;
        }

        // CameraManager must be passed from the service call to avoid circular dependency
        public DMTDeflectometryAcquisitionFlowTask StartAcquirePhaseImagesForPeriods(Side side, List<int> periods, double exposureTimeMs, DMTCameraManager cameraManager)        {
            var fringe = new Fringe
            {
                FringeType = FringeType.Multi, Period = periods.Min(), Periods = periods, NbImagesPerDirection = 8
            };
            var phaseAcqFlows = periods.SelectMany(period =>
            {
                var inputX = new AcquirePhaseImagesForPeriodAndDirectionInput(side, fringe, period, FringesDisplacement.X, exposureTimeMs);
                var inputY = new AcquirePhaseImagesForPeriodAndDirectionInput(side, fringe, period, FringesDisplacement.Y, exposureTimeMs);
                return _areFlowsSimulated
                    ? new[]
                    {
                        new AcquirePhaseImagesForPeriodAndDirectionFlowDummy(inputX, _hardwareManager, cameraManager,
                            _fringeManager),
                        new AcquirePhaseImagesForPeriodAndDirectionFlowDummy(inputY, _hardwareManager, cameraManager,
                            _fringeManager)
                    }
                    : new[]
                    {
                        new AcquirePhaseImagesForPeriodAndDirectionFlow(inputX, _hardwareManager, cameraManager,
                            _fringeManager),
                        new AcquirePhaseImagesForPeriodAndDirectionFlow(inputY, _hardwareManager, cameraManager,
                            _fringeManager)
                    };
            }).ToList();
            var phaseAcqFlowTask = new DMTDeflectometryAcquisitionFlowTask(phaseAcqFlows);
            phaseAcqFlowTask.Start(new CancellationTokenSource(), (status, data) => { }, (status, data) => { });
            return phaseAcqFlowTask;
        }

        private void InitCalibrationFolders()
        {
            var currentConfiguration = ClassLocator.Default.GetInstance<IDMTServiceConfigurationManager>();
            string calibrationInputsFolderPath = currentConfiguration.CalibrationInputFolderPath;
            string calibrationOutputsFolderPath = currentConfiguration.CalibrationOutputFolderPath;

            if (Directory.Exists(calibrationInputsFolderPath))
            {
                string message = $"Calibration inputs folder: {calibrationInputsFolderPath}";
                _logger.Information(message);
            }
            else
            {
                string message = $"Calibration inputs folder doesn't exist: {calibrationInputsFolderPath}";
                _logger.Error(message);
            }

            if (!Directory.Exists(calibrationOutputsFolderPath))
            {
                Directory.CreateDirectory(calibrationOutputsFolderPath);
            }
        }
        
        private void UpdateAndSaveCalibrationInput(Side side)
        {
            var newInputs = new ExposureMatchingInputs
            {
                GoldenValues = _exposureCalibBySide.Values.Select(value => value.ExposureMatchingGoldenValues).ToList(),
                AcquisitionExposureTimeMs = _dmtCalibrationInputs.ExposureMatchingInputs.AcquisitionExposureTimeMs,
            };
            _dmtCalibrationInputs.ExposureMatchingInputs = newInputs;
            string calibrationInputsFileFullPath = Path.Combine(_serviceConfigurationManager.CalibrationInputFolderPath,
                DMTCalibrationInputsFileName);
            _dmtCalibrationInputs.Serialize(calibrationInputsFileFullPath);
        }

        private void CheckFringesCurvature()
        {
            var standardFringes = _fringeManager.Fringes.Where(f => f.FringeType == FringeType.Standard).ToList();
            var smallestFringePeriod = standardFringes.FirstOrDefault();
            var calibrationDataFs = GetCurvatureDynamicsCalibrationBySide(Side.Front);
            if (calibrationDataFs != null && calibrationDataFs.FringePeriod != smallestFringePeriod.Period)
            {
                string message = $"The smallest fringe period ({smallestFringePeriod.Period}) does not correspond to the curvature calibration period ({calibrationDataFs.FringePeriod}) for the front side";
                _logger.Error(message);
            }

            var calibrationDataBs = GetCurvatureDynamicsCalibrationBySide(Side.Back);
            if (calibrationDataBs != null && calibrationDataBs.FringePeriod != smallestFringePeriod.Period)
            {
                string message = $"The smallest fringe period ({smallestFringePeriod.Period}) does not correspond to the curvature calibration period ({calibrationDataBs.FringePeriod}) for the back side";
                _logger.Error(message);
            }
        }

        private void InitCalibrations()
        {
            var calibFolder = _serviceConfigurationManager.CalibrationOutputFolderPath;

            InitializeGlobalTopographyCameraCalibration(calibFolder);

            InitializeGlobalTopographySystemCalbration(calibFolder);

            InitializeSystemUniformityCalibration(calibFolder);

            InitializePerpestiveCalibration(calibFolder);

            InitializeHighAngleDarkFieldMasks(calibFolder);

            InitializeExposureTimeMatchingCalibration(calibFolder);
            
            InitializeCurvatureDynamicsCalibration(calibFolder);
        }

        private void InitializeHighAngleDarkFieldMasks(string calibFolder)
        {
            foreach (var entry in _highAngleDarkFieldMaskFileNamesBySide)
            {
                var side = entry.Key;
                string calibrationFullFilePath = Path.Combine(calibFolder, entry.Value);
                if (File.Exists(calibrationFullFilePath))
                {
                    _highAngleDarkFieldMaskBySide[side] = new USPImageMil(calibrationFullFilePath);
                }
                else
                {
                    var message = new Message(MessageLevel.Warning, $"Calibration file {entry.Value} is missing");
                    _logger.Warning(message.UserContent);
                    _globalStatusServer.AddMessage(message);
                }
            }
        }

        private void InitializeCurvatureDynamicsCalibration(string calibFolder)
        {
            foreach (var entry in _curvatureDynamicsCalibFileNamesBySide)
            {
                var side = entry.Key;
                var fileName = entry.Value;
                var calibrationFile = Path.Combine(calibFolder, fileName);
                if (_algorithmManager.Config.CurvatureDynamicsCalibrationConfigurations.Any(curvature =>
                        curvature.Side == side && curvature.IsEnabled))
                {
                    if (File.Exists(calibrationFile))
                    {
                        _curvatureCalibBySide[side] = XML.Deserialize<CurvatureDynamicsCalibrationData>(calibrationFile);
                    }
                    else
                    {
                        var message = $"Calibration file {fileName} is missing";
                        _logger.Warning(message);
                        _globalStatusServer.AddMessage(new Message(MessageLevel.Warning, message));
                    }
                }
                
            }
        }

        private void InitializeExposureTimeMatchingCalibration(string calibFolder)
        {
            foreach (var exposureCalibrationSettings in _algorithmManager.Config.ExposureCalibration)
            {
                var exposureCalibrationSetting =
                    _algorithmManager.Config.ExposureCalibration.FirstOrDefault(c => c.Side == exposureCalibrationSettings.Side);
                var side = exposureCalibrationSetting.Side;
                string fileName = _exposureTimeCalibFileNamesBySide[side];
                string calibrationFile = Path.Combine(calibFolder, fileName);

                if (File.Exists(calibrationFile) && exposureCalibrationSetting.IsEnabled)
                {
                    _exposureCalibBySide[side] = XML.Deserialize<ExposureCalibration>(calibrationFile);
                }
                else
                {
                    if (exposureCalibrationSetting.IsEnabled)
                    {
                        var message = new Message(MessageLevel.Warning, $"Calibration file {fileName} is missing");
                        _logger.Warning(message.UserContent);
                        _globalStatusServer.AddMessage(message);
                    }
                }
            }
        }

        private void InitializePerpestiveCalibration(string calibFolder)
        {
            foreach (var entry in _perspectiveCalibFileNamesBySide)
            {
                var side = entry.Key;
                var fileName = entry.Value;
                var calibrationFile = Path.Combine(calibFolder, fileName);
                if (_algorithmManager.Config.DMTCalTransforms.Any(conf => conf.Side == side && conf.IsEnabled))
                {
                    if (File.Exists(calibrationFile))
                    {
                        LoadPerspectiveCalibrationFromFile(calibrationFile, side);
                    }
                    else
                    {
                        var warningMessage = new Message(MessageLevel.Warning, $"Perspective calibration file {fileName} for {side} is missing");
                        _logger.Warning(warningMessage.UserContent);
                        _globalStatusServer.AddMessage(warningMessage);
                    }
                }
            }
        }

        private void LoadPerspectiveCalibrationFromFile(string calibrationFile, Side side)
        {
            if (_serviceConfigurationManager.MilIsSimulated)
            {
                var test = Activator.CreateInstance(typeof(DMTTransform), true) as DMTTransform;
                _perspectiveCalibrationBySide[side] = test;
                return;
            }
            var calibration = new DMTTransform(Mil.Instance.HostSystem, calibrationFile);
            if (_hardwareManager.CamerasBySide.TryGetValue(side, out var camera) && camera.Width == calibration.InputSizeX && camera.Height == calibration.InputSizeY)
            {
                _perspectiveCalibrationBySide[side] = calibration;
            }
            else
            {
                if (camera is null)
                {
                    var errorMessage = new Message(MessageLevel.Error,
                        $"No camera available for {side} side while a perspective calibration is enabled. Please check the configuration file.");
                    _logger.Error(errorMessage.UserContent);
                    _globalStatusServer.AddMessage(errorMessage);
                }
                else if (camera.Width != calibration.InputSizeX || camera.Height != calibration.InputSizeY)
                {
                    var errorMessage = new Message(MessageLevel.Error,
                        $"Process module calibration size ({calibration.InputSizeX}, {calibration.InputSizeY}) doesn't match camera size ({camera.Width}, {camera.Height}).");
                    _logger.Error(errorMessage.UserContent);
                    _globalStatusServer.AddMessage(errorMessage);
                }
            }
        }

        private void InitializeSystemUniformityCalibration(string calibFolder)
        {
            foreach (var entry in _systemUniformityCalibFileNamesBySide)
            {
                var side = entry.Key;
                var fileName = entry.Value;
                var calibrationFile = Path.Combine(calibFolder, fileName);
                if (File.Exists(calibrationFile))
                {
                    var correctionUSPImageMil = new USPImageMil(calibrationFile);
                    var correctionImageData = ImageUtils.CreateImageDataFromUSPImageMil(correctionUSPImageMil);
                    _systemUniformityCalibBySide[side] = correctionImageData;
                }
                else
                {
                    var message = $"Calibration file {fileName} is missing";
                    _logger.Warning(message);
                    _globalStatusServer.AddMessage(new Message(MessageLevel.Warning, message));
                }
            }
        }

        private void SaveSystemUniformityCalibrationAsync(Side side)
        {
            try
            {
                var calibFolder = _serviceConfigurationManager.CalibrationOutputFolderPath;
                Directory.CreateDirectory(calibFolder);
                string calibrationFile = Path.Combine(calibFolder, _systemUniformityCalibFileNamesBySide[side]);
                var uniformityCorrection = _systemUniformityCalibBySide[side];

                var uniformityCorrectionUSP = uniformityCorrection.ConvertToUSPImageMil(false);

                uniformityCorrectionUSP.Save(calibrationFile);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Failed to save the system uniformity calibration file : {ex.Message}");
                throw;
            }
        }

        private void InitializeGlobalTopographySystemCalbration(string calibFolder)
        {
            foreach (var entry in _globalTopoSystemCalibFileNamesBySide)
            {
                var side = entry.Key;
                var fileName = entry.Value;
                var calibrationFile = Path.Combine(calibFolder, fileName);
                if (File.Exists(calibrationFile))
                {
                    _globalTopoSystemCalibBySide[side] = XML.Deserialize<GlobalTopoSystemCalibrationResult>(calibrationFile);
                }
                else
                {
                    var message = $"Calibration file {fileName} is missing";
                    _logger.Warning(message);
                    _globalStatusServer.AddMessage(new Message(MessageLevel.Warning, message));
                }
            }
        }

        private void InitializeGlobalTopographyCameraCalibration(string calibFolder)
        {
            foreach (var entry in _globalTopoCameraCalibFileNamesBySide)
            {
                var side = entry.Key;
                var fileName = entry.Value;
                var calibrationFile = Path.Combine(calibFolder, fileName);
                if (File.Exists(calibrationFile))
                {
                    _globalTopoCameraCalibBySide[side] = XML.Deserialize<GlobalTopoCameraCalibrationResult>(calibrationFile);
                }
                else
                {
                    var message = $"Calibration file {fileName} is missing";
                    _logger.Warning(message);
                    _globalStatusServer.AddMessage(new Message(MessageLevel.Warning, message));
                }
            }
        }

        private void InitCalibrationInputs()
        {
            try
            {
                string calibrationInputsFilePath = Path.Combine(_serviceConfigurationManager.CalibrationInputFolderPath, DMTCalibrationInputsFileName);
                if (!File.Exists(calibrationInputsFilePath))
                {
                    var message = $"{DMTCalibrationInputsFileName} file missing from Calibration Input folder";
                    _logger.Error(message);
                    _globalStatusServer.AddMessage(new Message(MessageLevel.Error, message));
                }
                _dmtCalibrationInputs = XML.Deserialize<DMTCalibrationInputs>(calibrationInputsFilePath);
                _ellipseMasksBySide = _dmtCalibrationInputs.EllipseMaskInputs.ToDictionary(ellipseMask => ellipseMask.Side);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void InitializeDeadPixelsManagers()
        {
            foreach (var side in _hardwareManager.CamerasBySide.Keys)
            {
                LoadDeadPixels(side);
            }
        }

        private void LoadDeadPixels(Side side)
        {
            var camera = _hardwareManager.CamerasBySide[side];
            string deadPixelAbsoluteFilePath =
                Path.Combine(_serviceConfigurationManager.CalibrationOutputFolderPath, _deadPixelsCalibFileNamesBySide[side]);
            if (!string.IsNullOrEmpty(deadPixelAbsoluteFilePath) && File.Exists(deadPixelAbsoluteFilePath))
            {
                try
                {
                    _deadPixelsManagers.Add(side, DeadPixelsManager.ReadFromDPMLFile(deadPixelAbsoluteFilePath));
                }
                catch (Exception)
                {
                    _logger.Error($"Unable to load the dead pixels file : {deadPixelAbsoluteFilePath}");
                }
            }
            else
            {
                var message = $"Calibration file {_deadPixelsCalibFileNamesBySide[side]} is missing";
                _logger.Warning(message);
                _globalStatusServer.AddMessage(new Message(MessageLevel.Warning, message));
            }
        }

        private void SaveCurvatureDynamicsCalibration(Side side)
        {
            try
            {
                var filename = _curvatureDynamicsCalibFileNamesBySide[side];
                var calibFolder = _serviceConfigurationManager.CalibrationOutputFolderPath;
                Directory.CreateDirectory(calibFolder);
                string calibrationFile = Path.Combine(calibFolder, filename);
                XML.Serialize(_curvatureCalibBySide[side], calibrationFile);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Failed to save the curvature dynamics calibration file : {ex.Message}");
                throw;
            }
        }

        private void SaveExposureCalibration(Side side)
        {
            try
            {
                var filename = _exposureTimeCalibFileNamesBySide[side];
                var calibFolder = _serviceConfigurationManager.CalibrationOutputFolderPath;
                Directory.CreateDirectory(calibFolder);
                string calibrationFile = Path.Combine(calibFolder, filename);
                _exposureCalibBySide[side].Serialize(calibrationFile);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Failed to save the exposure calibration file : {ex.Message}");
                throw;
            }
        }

        private void SaveGlobalTopoSystemCalibration(Side side)
        {
            try
            {
                var calibFolder = _serviceConfigurationManager.CalibrationOutputFolderPath;
                Directory.CreateDirectory(calibFolder);
                string calibrationFile = Path.Combine(calibFolder, _globalTopoSystemCalibFileNamesBySide[side]);
                XML.Serialize(_globalTopoSystemCalibBySide[side], calibrationFile);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Failed to save the globalTopo camera calibration file : {ex.Message}");
                throw;
            }
        }

        private void SaveGlobalTopoCameraCalibration(Side side)
        {
            try
            {
                var calibFolder = _serviceConfigurationManager.CalibrationOutputFolderPath;
                Directory.CreateDirectory(calibFolder);
                string calibrationFile = Path.Combine(calibFolder, _globalTopoCameraCalibFileNamesBySide[side]);
                XML.Serialize(_globalTopoCameraCalibBySide[side], calibrationFile);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Failed to save the globalTopo camera calibration file : {ex.Message}");
                throw;
            }
        }
        
        private DMTDeflectometryAcquisitionFlowTask CreateAcquisitionFlowTask(Side side, Fringe smallestFringe,
            USPImageMil firstFringeImage, DMTCameraManager cameraManager)
        {
            var aeFlow = CreateAutoExposureFlow(side, smallestFringe, firstFringeImage, cameraManager);
            var apiFlows = CreateAcquirePhaseImageFlows(side, smallestFringe, cameraManager);

            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(aeFlow, apiFlows);
            return acquisitionFlowTask;
        }
        
        private AutoExposureFlow CreateAutoExposureFlow(Side side, Fringe smallestFringe, USPImageMil firstFringeImage, DMTCameraManager cameraManager)
        {
            var cameraInfo = cameraManager.GetCameraInfoBySide(side);
            var aeRoi = new ROI
            {
                RoiType = RoiType.Rectangular,
                Rect = new Rect(0, 0, cameraInfo.Width, cameraInfo.Height)
            };
            var aeInput = new AutoExposureInput(side, MeasureType.DeflectometryMeasure, aeRoi,
                AcquisitionScreenDisplayImage.FringeImage, Colors.White,
                smallestFringe, firstFringeImage, 220);

            return _areFlowsSimulated ? new AutoExposureFlowDummy(aeInput, _hardwareManager, cameraManager)
                : new AutoExposureFlow(aeInput, _hardwareManager, cameraManager);
        }
        
        private List<AcquirePhaseImagesForPeriodAndDirectionFlow> CreateAcquirePhaseImageFlows(Side side, Fringe smallestFringe, DMTCameraManager cameraManager)
        {
            var apiXFlow = CreateAcquirePhaseImageForPeriodAndDirectionFlow(side, smallestFringe, FringesDisplacement.X, cameraManager);
            var apiYFlow = CreateAcquirePhaseImageForPeriodAndDirectionFlow(side, smallestFringe, FringesDisplacement.Y, cameraManager);
            return new List<AcquirePhaseImagesForPeriodAndDirectionFlow> { apiXFlow, apiYFlow };
        }
        
        private AcquirePhaseImagesForPeriodAndDirectionFlow CreateAcquirePhaseImageForPeriodAndDirectionFlow(Side side, Fringe smallestFringe, FringesDisplacement direction, DMTCameraManager cameraManager)
        {
            var apiInput = new AcquirePhaseImagesForPeriodAndDirectionInput(side, smallestFringe, smallestFringe.Period, direction, 0.0);
            return _areFlowsSimulated ? new AcquirePhaseImagesForPeriodAndDirectionFlowDummy(apiInput, _hardwareManager, cameraManager, _fringeManager)
                : new AcquirePhaseImagesForPeriodAndDirectionFlow(apiInput, _hardwareManager, cameraManager, _fringeManager);
        }

        private void ComputeUnwrappedPhaseMapXAndY(Side side, List<int> periods, DMTDeflectometryAcquisitionFlowTask acquisitionFlowTask, Length waferDiameter, out ImageData unwrappedPhaseMapXImage,
            out ImageData unwrappedPhaseMapYImage)
        {
            var fringe = new Fringe
            {
                FringeType = FringeType.Multi,
                Period = periods.Min(),
                Periods = periods,
                NbImagesPerDirection = 8
            };
            var computePhaseMapXFlows = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>(periods.Count);
            var computePhaseMapYFlows = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>(periods.Count);
            _perspectiveCalibrationBySide.TryGetValue(side, out var perspectiveCalibration);
            foreach (var period in periods)
            {
                var computePhaseMapXInput = new ComputePhaseMapAndMaskForPeriodAndDirectionInput(fringe, period, FringesDisplacement.X, side)
                {
                    WaferDiameter = waferDiameter,
                    PerspectiveCalibration = perspectiveCalibration,
                    UseEnhancedMask = !(perspectiveCalibration is null)
                };
                var computePhaseMapXFlow = _areFlowsSimulated ? new ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy(computePhaseMapXInput)
                                                             : new ComputePhaseMapAndMaskForPeriodAndDirectionFlow(computePhaseMapXInput);

                computePhaseMapXFlows.Add(computePhaseMapXFlow);

                var computePhaseMapYInput =
                    new ComputePhaseMapAndMaskForPeriodAndDirectionInput(fringe, period, FringesDisplacement.Y, side)
                    {
                        WaferDiameter = waferDiameter,
                        PerspectiveCalibration = perspectiveCalibration,
                        UseEnhancedMask = !(perspectiveCalibration is null)
                    };
                var computePhaseMapYFlow = _areFlowsSimulated ? new ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy(computePhaseMapYInput)
                                                             : new ComputePhaseMapAndMaskForPeriodAndDirectionFlow(computePhaseMapYInput);
                computePhaseMapYFlows.Add(computePhaseMapYFlow);
            }

            var computePhaseMapFlows = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>(periods.Count * 2);
            computePhaseMapFlows.AddRange(computePhaseMapXFlows);
            computePhaseMapFlows.AddRange(computePhaseMapYFlows);

            var computeUnwrappedPhaseMapXInput = new ComputeUnwrappedPhaseMapForDirectionInput(fringe, FringesDisplacement.X, side, false, false);
            var computeUnwrappedPhaseMapXFlow =
                _areFlowsSimulated ? new ComputeUnwrappedPhaseMapForDirectionFlowDummy(computeUnwrappedPhaseMapXInput)
                                  : new ComputeUnwrappedPhaseMapForDirectionFlow(computeUnwrappedPhaseMapXInput);

            var computeUnwrappedPhaseMapYInput = new ComputeUnwrappedPhaseMapForDirectionInput(fringe, FringesDisplacement.Y, side, false, false);
            var computeUnwrappedPhaseMapYFlow =
                _areFlowsSimulated ? new ComputeUnwrappedPhaseMapForDirectionFlowDummy(computeUnwrappedPhaseMapYInput)
                                  : new ComputeUnwrappedPhaseMapForDirectionFlow(computeUnwrappedPhaseMapYInput);

            var saveImageFlowByComputeUnwrappedPhaseMapFlow = new Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
                {
                    {computeUnwrappedPhaseMapXFlow, null },
                    {computeUnwrappedPhaseMapYFlow, null },
                };
            var deflectometryCalculationFlowTask = new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, computePhaseMapFlows)
                .AddUnwrappedPhaseMapsFlows(saveImageFlowByComputeUnwrappedPhaseMapFlow);

            deflectometryCalculationFlowTask.CreateAndChainComputationContinuationTasks(null);
            deflectometryCalculationFlowTask.LastComputationTask.ToTask().Wait();

            if (computeUnwrappedPhaseMapXFlow.Result.Status.State != FlowState.Success)
            {
                throw new Exception($"Failed to acquire Unwrapped Phase Map X image for system calibration");
            }
            unwrappedPhaseMapXImage = computeUnwrappedPhaseMapXFlow.Result.UnwrappedPhaseMap;

            if (computeUnwrappedPhaseMapYFlow.Result.Status.State != FlowState.Success)
            {
                throw new Exception($"Failed to acquire Unwrapped Phase Map Y image for system calibration");
            }
            unwrappedPhaseMapYImage = computeUnwrappedPhaseMapYFlow.Result.UnwrappedPhaseMap;
        }

        private bool CheckSaturatedPixels(USPImageMil procimg)
        {
            var nbPixelsSaturated = procimg.ComputeNbSaturatedPixels();
            var nbPixelsInImage = procimg.Height * procimg.Width;
            var saturationThresholdPercent = _algorithmManager.Config.Image.SaturationThresholdPercent;
            var percentageSaturatedPixels = nbPixelsSaturated * 100 / nbPixelsInImage;
            if (percentageSaturatedPixels > saturationThresholdPercent)
            {
                _logger.Warning($"The image is saturated : the percentage of saturated pixels is {percentageSaturatedPixels}% wich is over the threshold {saturationThresholdPercent}%");
                return true;
            }
            return false;
        }

        private async Task<CurvatureDynamicsCalibrationData> ExecuteCurvatureDynamicsCalibrationAsync(Side side, DMTDeflectometryAcquisitionFlowTask acquisitionFlowTask)
        {
            int minFringePeriod = _fringeManager.Fringes.Min(f => f.Period);
            var smallestFringe = _fringeManager.Fringes.First(f => f.Period == minFringePeriod);
            var curvatureDynamicsCalibrationConfiguration = ClassLocator.Default.
                                                                        GetInstance<IFlowsConfiguration>()
                                                                        .Flows.OfType<CurvatureDynamicsCalibrationConfiguration>()
                                                                        .First();
            smallestFringe.NbImagesPerDirection = curvatureDynamicsCalibrationConfiguration.NumberOfPhaseShifts;
            var curvatureFlowTask = CreateCurvatureCalculationFlowTask(smallestFringe, acquisitionFlowTask, side);
            curvatureFlowTask.CreateAndChainComputationContinuationTasks();
            await curvatureFlowTask.LastComputationTask;
            if (curvatureFlowTask.LastComputationTask.Result.Status.State != FlowState.Success)
            {
                throw new Exception("Unable to compute a curvature dynamics coefficient");
            }

            var coefficient = curvatureFlowTask.LastComputationTask.Result.CurvatureDynamicsCoefficient;
            var calibrationData = new CurvatureDynamicsCalibrationData
            {
                Side = side,
                FringePeriod = smallestFringe.Period,
                DynamicsCoefficient = coefficient
            };
            return calibrationData;
        }

        private ExposureCalibration ExecuteExposureCalibration(Side side, ExposureMatchingInputs inputs = null)
        {
            string cameraSerialNumber = _hardwareManager.CamerasBySide[side].SerialNumber;
            
            if (inputs is null)
            {
                inputs = _dmtCalibrationInputs.ExposureMatchingInputs; 
            }
        
            uint[] goldenValues = inputs.GoldenValuesBySide[side].GetGoldenValuesArray(); 

            // Calculates the exposure correction
            var image = _brightFieldImagesBySide[side].ToServiceImage();
            int res = FocusExposureCalibrationWrapper.CalibrateExposureMatching(image.Data, (uint)image.DataWidth,
             (uint)image.DataHeight, goldenValues, out float exposureMatchingCoef);

            if (res < 0)
            {
                throw new Exception("Failed to compute the Exposure Matching coefficient");
            }

            // Save the result in the exposure calibration file
            var exposureCalibration = new ExposureCalibration
            {
                CameraSerialNumber = cameraSerialNumber,
                ExposureCorrectionCoef = exposureMatchingCoef,
                ExposureMatchingGoldenValues = new ExposureMatchingGoldenValues(side, goldenValues),
            };
            return exposureCalibration;
        }
        
        private ExposureCalibration ComputeGoldenValues(Side side)
        {
            string cameraSerialNumber = _hardwareManager.CamerasBySide[side].SerialNumber;
            var image = _brightFieldImagesBySide[side].ToServiceImage();

            ulong[] goldenLuminanceValues = FocusExposureCalibrationWrapper.GetGoldenValuesFromImage(image.Data, (uint)image.DataWidth, (uint)image.DataHeight);
            
            var exposureCalibration = new ExposureCalibration
            {
                CameraSerialNumber = cameraSerialNumber,
                ExposureCorrectionCoef = 1.0,
                ExposureMatchingGoldenValues = new ExposureMatchingGoldenValues(side, goldenLuminanceValues)
            };
            return exposureCalibration;
        }

        private GlobalTopoCameraCalibrationResult ExecuteCameraCalibration(Side side)
        {
            float pixelSize;
            if (_areFlowsSimulated)
            {
                pixelSize = 1f;
            }
            else
            {
                if (!_perspectiveCalibrationBySide.TryGetValue(side, out var transform))
                {
                    throw new Exception("Cannot retrieve informations from perspective calibration");
                }
                pixelSize = (float)transform.PixelSize.Micrometers().Millimeters;
            }

            var waferDef = _dmtCalibrationInputs.GlobalTopoCalibrationWaferDefinition;
            var cameraCalibrationImages = _cameraCalibrationImages[side].Values.ToArray();
            var input = new GlobalTopoCameraCalibrationInput(side, pixelSize, waferDef, cameraCalibrationImages);
            var flow = _areFlowsSimulated ? new GlobalTopoCameraCalibrationFlowDummy(input)
                                         : new GlobalTopoCameraCalibrationFlow(input);
            var cameraCalibrationRes = flow.Execute();
            if (cameraCalibrationRes.Status.State != FlowState.Success)
            {
                throw new Exception($"Camera calibration failed");
            }

            return cameraCalibrationRes;
        }

        private GlobalTopoSystemCalibrationResult ExecuteSystemCalibration(Side side, List<int> periods,
            ImageData brightFieldImage, ImageData unwrappedPhaseMapXImage, ImageData unwrappedPhaseMapYImage)
        {
            float pixelSize;
            if (_areFlowsSimulated)
            {
                pixelSize = 1f;
            }
            else
            {
                if (!_perspectiveCalibrationBySide.TryGetValue(side, out var transform))
                {
                    throw new Exception("Cannot retrieve informations from perspective calibration");
                }
                pixelSize = (float)transform.PixelSize.Micrometers().Millimeters;
            }

            if (!_globalTopoCameraCalibBySide.TryGetValue(side, out var cameraCalib))
            {
                throw new Exception($"Global topo camera calibration is missing for side {side}");
            }

            var waferDef = _dmtCalibrationInputs.GlobalTopoCalibrationWaferDefinition;

            var calibSystemInput = new GlobalTopoSystemCalibrationInput(
                periods, side, pixelSize, waferDef, cameraCalib, brightFieldImage, unwrappedPhaseMapXImage, unwrappedPhaseMapYImage);
            var calibSystemFlow = _areFlowsSimulated ? new GlobalTopoSystemCalibrationFlowDummy(calibSystemInput, _hardwareManager)
                                                    : new GlobalTopoSystemCalibrationFlow(calibSystemInput, _hardwareManager);
            var calibSystemRes = calibSystemFlow.Execute();
            if (calibSystemRes.Status.State != FlowState.Success)
            {
                throw new Exception($"System calibration failed");
            }

            return calibSystemRes;
        }

        private ImageData ExecuteSystemUniformityCalibration(Side side)
        {
            if (!_brightFieldImagesBySide.TryGetValue(side, out var brigthfieldImage))
            {
                throw new Exception($"Bright-field image for side {side} is missing");
            }
            if (!_hardwareManager.ScreensBySide.TryGetValue(side, out var screen))
            {
                throw new Exception($"Screen for side {side} is missing");
            }
            if (!_globalTopoCameraCalibBySide.TryGetValue(side, out var cameraCalibration))
            {
                throw new Exception($"Global topo camrea calibration for side {side} is missing");
            }
            if (!_globalTopoSystemCalibBySide.TryGetValue(side, out var systemCalibration))
            {
                throw new Exception($"Global topo system calibration for side {side} is missing");
            }
            if (!_globalTopoCameraCalibBySide.TryGetValue(side, out var cameraCalib))
            {
                throw new Exception($"Global topo camera calibration is missing for side {side}");
            }

            float pixelSizeInMm;
            if (_areFlowsSimulated)
            {
                pixelSizeInMm = 1f;
            }
            else
            {
                if (!_perspectiveCalibrationBySide.TryGetValue(side, out var transform))
                {
                    throw new Exception("Cannot retrieve informations from perspective calibration");
                }
                pixelSizeInMm = (float)transform.PixelSize.Micrometers().Millimeters;
            }

            var image = ImageUtils.CreateImageDataFromUSPImageMil(brigthfieldImage);
            var waferRadiusInMm = _dmtCalibrationInputs.GlobalTopoCalibrationWaferDefinition.WaferRadiusMm;
            var input = new SystemUniformityCalibrationInput(image, waferRadiusInMm, pixelSizeInMm, cameraCalibration, systemCalibration, _dmtCalibrationInputs.FresnelCoefficients, screen.WavelengthPeaks, screen.Polarisation);
            var flow = _areFlowsSimulated ? new SystemUniformityCalibrationFlowDummy(input)
                                         : new SystemUniformityCalibrationFlow(input);
            var systemUniformityRes = flow.Execute();
            if (systemUniformityRes.Status.State != FlowState.Success)
            {
                throw new Exception("System uniformity calibration failed");
            }
            return systemUniformityRes.UnifomityCorrection;
        }

        private DMTCurvatureCalibrationCalculationFlowTask CreateCurvatureCalculationFlowTask(
            Fringe smallestFringe, DMTDeflectometryAcquisitionFlowTask acquisitionFlowTask, Side side)
        {
            var cpmFlows = CreateComputePhaseMapFlows(smallestFringe, side);
            var crcmFlows = CreateComputeRawCurvatureMapFlows(smallestFringe, side);
            var ccdcFlow = CreateCurvatureDynamicsCalibrationFlow();

            var curvatureFlowTask = new DMTCurvatureCalibrationCalculationFlowTask(acquisitionFlowTask, cpmFlows, crcmFlows, ccdcFlow);
            return curvatureFlowTask;
        }

        private List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> CreateComputePhaseMapFlows(
            Fringe smallestFringe, Side side)
        {
            var cpmXFlow = CreateComputePhaseMapAndMaskForPeriodAndDirectionFlow(smallestFringe, FringesDisplacement.X, side);
            var cpmYFlow = CreateComputePhaseMapAndMaskForPeriodAndDirectionFlow(smallestFringe, FringesDisplacement.Y, side);
            return new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> { cpmXFlow, cpmYFlow };
        }

        private ComputePhaseMapAndMaskForPeriodAndDirectionFlow
            CreateComputePhaseMapAndMaskForPeriodAndDirectionFlow(
                Fringe smallestFringe, FringesDisplacement direction, Side side)
        {
            var waferDiameter = _hardwareManager.GetCurrentChuckMaximumDiameter();
            if (!_perspectiveCalibrationBySide.TryGetValue(side, out var perspectiveCalibration))
            {
                throw new Exception("Cannot retrieve informations from perspective calibration");
            }
            
            var cpmInput =
                new ComputePhaseMapAndMaskForPeriodAndDirectionInput(smallestFringe, smallestFringe.Period, direction, side)
                {
                    WaferDiameter = waferDiameter,
                    PerspectiveCalibration = perspectiveCalibration,
                    UseEnhancedMask = false,

                };
            return _areFlowsSimulated ? new ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy(cpmInput)
                                     : new ComputePhaseMapAndMaskForPeriodAndDirectionFlow(cpmInput);
        }

        private CurvatureDynamicsCalibrationFlow CreateCurvatureDynamicsCalibrationFlow()
        {
            var ccdcInput = new CurvatureDynamicsCalibrationInput();
            return _areFlowsSimulated ? new CurvatureDynamicsCalibrationFlowDummy(ccdcInput)
                                     : new CurvatureDynamicsCalibrationFlow(ccdcInput);
        }

        private List<ComputeRawCurvatureMapForPeriodAndDirectionFlow> CreateComputeRawCurvatureMapFlows(
            Fringe smallestFringe, Side side)
        {
            var crcmXFlow = CreateComputeRawCurvatureMapForPeriodAndDirectionFlow(smallestFringe, FringesDisplacement.X, side);
            var crcmYFlow = CreateComputeRawCurvatureMapForPeriodAndDirectionFlow(smallestFringe, FringesDisplacement.Y, side);
            return new List<ComputeRawCurvatureMapForPeriodAndDirectionFlow> { crcmXFlow, crcmYFlow };
        }

        private ComputeRawCurvatureMapForPeriodAndDirectionFlow
            CreateComputeRawCurvatureMapForPeriodAndDirectionFlow(
                Fringe smallestFringe, FringesDisplacement direction, Side side)
        {
            var crcmInput = new ComputeRawCurvatureMapForPeriodAndDirectionInput(smallestFringe, smallestFringe.Period, direction, side);
            return _areFlowsSimulated ? new ComputeRawCurvatureMapForPeriodAndDirectionFlowDummy(crcmInput) :
                                       new ComputeRawCurvatureMapForPeriodAndDirectionFlow(crcmInput);
        }
    }
}
