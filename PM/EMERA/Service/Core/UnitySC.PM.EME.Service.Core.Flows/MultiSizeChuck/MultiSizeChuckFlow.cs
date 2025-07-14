using System;
using System.IO;
using System.Threading;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.MultiSizeChuck
{
    public class
        MultiSizeChuckFlow : FlowComponent<MultiSizeChuckInput, MultiSizeChuckResult, MultiSizeChuckConfiguration>
    {
        private readonly AutoExposureFlow _autoExposureFlow;
        private readonly IEmeraCamera _camera;
        private readonly EMEChuckConfig _emeChuckConfig;
        private readonly PhotoLumAxes _motionAxes;

        private readonly PatternRecFlow _patternRecFlow;
        private readonly IReferentialService _referentialService;
        private SubstSlotWithPositionConfig _waferSlotConfig;

        public MultiSizeChuckFlow(MultiSizeChuckInput input, IEmeraCamera camera, PatternRecFlow patternRecFlow = null,
            AutoExposureFlow autoExposureFlow = null) : base(input, "MultiSizeChuckFlow")
        {
            _camera = camera;
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _referentialService = ClassLocator.Default.GetInstance<IReferentialService>();
            _patternRecFlow = patternRecFlow ?? new PatternRecFlow(new PatternRecInput(), _camera);
            _autoExposureFlow = autoExposureFlow ?? new AutoExposureFlow(new AutoExposureInput(), _camera);

            if (hardwareManager?.Chuck?.Configuration is EMEChuckConfig emeChuckConfig)
            {
                _emeChuckConfig = emeChuckConfig;
            }
            else
            {
                throw new Exception("The chuck config should be of type EMEChuckConfig");
            }

            if (hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception("MotionAxes should be PhotoLumAxes");
            }
        }

        protected override void Process()
        {
            double initialExposureTime = _camera.GetCameraExposureTime();
            var initialPosition = _motionAxes.GetPosition() as XYZPosition;
            var initialWaferReferentialSettings =
                _referentialService.GetSettings(ReferentialTag.Wafer)?.Result as WaferReferentialSettings;
            try
            {
                SetToEmptyReferentialWafer();
                _waferSlotConfig = GetWaferSlotConfiguration();
                var centerPosition = GetCenterPosition(_waferSlotConfig);
                MoveToCenterPosition(centerPosition);
                LoadReferenceImage();
                //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
                //double exposureTime = GetExposureTime();
                //_camera.SetCameraExposureTime(exposureTime);
                ComputeShift();
                //TODO: to be tested when a calibration Wafer is available.
                //ComputeAngle();
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Error during the Multi size chuck Flow : {ex.Message}");
                throw;
            }
            finally
            {
                _motionAxes.GoToPosition(initialPosition);
                _camera.SetCameraExposureTime(initialExposureTime);
                _referentialService.SetSettings(initialWaferReferentialSettings);
            }
        }

        private double GetExposureTime()
        {
            var result = _autoExposureFlow.Execute();
            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Exposure time Failed.");
            }

            CheckCancellation();
            return result.ExposureTime;
        }

        public void SetToEmptyReferentialWafer()
        {
            var waferReferentialSettings = new WaferReferentialSettings
            {
                ShiftX = 0.Millimeters(), ShiftY = 0.Millimeters()
            };
            _referentialService.SetSettings(waferReferentialSettings);
        }

        public SubstSlotWithPositionConfig GetWaferSlotConfiguration()
        {
            var configs = _emeChuckConfig.SubstrateSlotConfigs;
            return configs?.Find(c => c.Diameter == Input.WaferDiameter);
        }

        public static XYZPosition GetCenterPosition(SubstSlotWithPositionConfig waferSlotConfig)
        {
            if (waferSlotConfig != null)
            {
                return waferSlotConfig.PositionSensor.ToXYZPosition();
            }

            return new XYZPosition(); // Default position if wafer configuration is not available
        }

        private void MoveToCenterPosition(XYZPosition centerPosition)
        {
            _motionAxes.GoToPosition(centerPosition);
            _motionAxes.WaitMotionEnd(3000);
            Thread.Sleep(500);
        }

        private PatternRecInput CreatePatternRecInput(ServiceImage refImage, GetZFocusInput getZFocusInput = null)
        {
            bool runAutoFocus = !(getZFocusInput is null);

            var patternRecData = new PatternRecognitionData
            {
                RegionOfInterest = Configuration.RegionOfInterest,
                PatternReference = refImage.ToExternalImage(),
                Gamma = 0.9
            };

            return new PatternRecInput(patternRecData, runAutoFocus, getZFocusInput);
        }

        private void LoadReferenceImage()
        {
            string pathImageRef =
                Path.Combine(ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>().CalibrationFolderPath,
                    Configuration.WaferCenterImageCalibrationPath);
            try
            {
                if (!File.Exists(pathImageRef))
                {
                    throw new FileNotFoundException($"[MultiSizeChuckFlow] File not found: {pathImageRef}");
                }

                var refImage = new ServiceImage();
                refImage.LoadFromFile(pathImageRef);
                _patternRecFlow.Input = CreatePatternRecInput(refImage, Input.GetZFocusInput);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception($"LoadFromFile failed in MultiSizeChuckFlow: {ex.Message}");
            }
        }

        private void ComputeShift()
        {
            var result = _patternRecFlow.Execute();
            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Pattern rec failed in MultiSizeChuckFlow");
            }

            var positionSensor = _waferSlotConfig.PositionSensor as XYPosition;
            if (positionSensor == null) return;
            Result.ShiftX = positionSensor.X.Millimeters() + result.ShiftX;
            Result.ShiftY = positionSensor.Y.Millimeters() + result.ShiftY;
        }
    }
}
