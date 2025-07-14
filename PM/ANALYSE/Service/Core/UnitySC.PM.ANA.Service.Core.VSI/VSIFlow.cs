using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.VSI
{
    public class VSIFlow : FlowComponent<VSIInput, VSIResult, VSIConfiguration>
    {
        private const int WaitPiezoMotionEndTimeout_ms = 10_000;

        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        private readonly PiezoController _piezoController;
        private readonly CameraBase _camera;
        private readonly IAxis _piezoAxis;

        private VSISteps _steps;
        private Length _piezoStartPosition;
        private Rect _oldCameraAOI = Rect.Empty;

        public VSIFlow(VSIInput input) : base(input, "VSIFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();

            if (!_hardwareManager.Cameras.TryGetValue(Input.CameraId, out _camera))
            {
                throw new InvalidOperationException($"Camera with ID '{Input.CameraId}' cannot be found.");
            }

            try
            {
                _piezoController = _hardwareManager.GetPiezoController(Input.ObjectiveId);
            }
            catch
            {
                throw new InvalidOperationException($"Piezo controller for objective with ID '{Input.ObjectiveId}' cannot be found.");
            }

            _piezoAxis = _piezoController.AxesList?.First(axis => axis.AxisID == _hardwareManager.GetPiezoAxisID(Input.ObjectiveId));
            if (_piezoAxis is null)
            {
                throw new InvalidOperationException($"Piezo axis for objective with ID '{Input.ObjectiveId}' cannot be found.");
            }
        }

        protected override void Process()
        {
            SetProgressMessage("Starting VSI Flow");
            try
            {
                InitialSetup();
                AcquireImages();
                ComputeVSI();
            }
            finally
            {
                WriteReportIfNeeded();
                GoToPiezoPosition(_piezoAxis.AxisConfiguration.PositionHome);
                _camera.SetAOI(_oldCameraAOI);
                _hardwareManager.Axes.StopLanding();
            }
        }

        private void InitialSetup()
        {
            CheckCancellation();

            var signedStepSize = -Input.StepSize;
            _piezoStartPosition = _piezoAxis.AxisConfiguration.PositionMax;
            _steps = new VSISteps(_piezoStartPosition, Input.StepCount, signedStepSize);
            SetCameraAOI();
            MoveInStartPosition();
            _hardwareManager.Axes.Land();
        }

        private void SetCameraAOI()
        {
            if (Input.ROI == null)
            {
                return;
            }

            var objectiveCalibration = HardwareUtils.GetObjectiveParameters(_calibrationManager, Input.ObjectiveId);

            _oldCameraAOI = _camera.GetAOI();

            int nbLines = (int)Input.ROI.Height.ToPixels(objectiveCalibration.Image.PixelSizeY);
            int nbCols = (int)Input.ROI.Width.ToPixels(objectiveCalibration.Image.PixelSizeX);

            int topLeftY = (_camera.Height / 2) - (nbLines / 2);
            int topLeftX = (_camera.Width / 2) - (nbCols / 2);
            var aoi = new Rect(topLeftX, topLeftY, nbCols, nbLines);

            _camera.SetAOI(aoi);
        }

        private void AcquireImages()
        {
            Logger.Debug($"{LogHeader} Start Image acquisition for {Input.StepCount} steps");
            CheckCancellation();

            var isAcquiringFromOutside = _camera.IsAcquiring;
            if (!isAcquiringFromOutside)
            {
                _camera.StartContinuousGrab();
            }

            int cntmax = _steps.GetOrderedSteps().Count;
            int onepercent = Math.Max(1, cntmax / 100);
            int cnt = 0;
            foreach (var step in _steps.GetOrderedSteps())
            {
                GoToPiezoPosition(step.StartPosition);
                step.Image = HardwareUtils.AcquireCameraImage(_hardwareManager, ClassLocator.Default.GetInstance<ICameraManager>(), Input.CameraId);
                CheckCancellation();

                if (++cnt % onepercent == 0)
                {
                    Logger.Debug($"{LogHeader} progress : {cnt} / {cntmax} steps");
                }
            }

            if (!isAcquiringFromOutside)
            {
                _camera.StopContinuousGrab();
            }

            Logger.Debug($"{LogHeader} Image acquisition Done");
        }

        // AcquireImages version with piezo trigger IN for tests
        /*private void AcquireImages()
        {
            var isAcquiringFromOutside = _camera.IsAcquiring;
            if (!isAcquiringFromOutside)
            {
                _camera.StartContinuousGrab();
            }

            _piezoController.ConfigureTriggerIn(_piezoStartPosition, _signedStepSize);

            try
            {
                foreach (var step in _steps.GetOrderedSteps())
                {
                    step.Image = HardwareUtils.AcquireCameraImage(_hardwareManager, ClassLocator.Default.GetInstance<ICameraManager>(), Input.CameraId);

                    _hardwareManager.Plc?.StartTriggerOutEmitSignal();
                    waitPiezoReachPosition(step.EndPosition);
                }
            }
            finally
            {
                _piezoController.DisableTriggerIn();
                if (!isAcquiringFromOutside)
                {
                    _camera.StopContinuousGrab();
                }
            }
        }*/

        private void ComputeVSI()
        {
            Logger.Debug($"{LogHeader} Start VSI computation");
            CheckCancellation();

            var images = _steps.GetAllImages();

            var dataWidth = images[0].DataWidth;   // pixel, number of columnns
            var dataHeight = images[0].DataHeight; // pixel, number of row

            List<byte[]> byteArrayList = new List<byte[]>();
            foreach (var img in images)
            {
                byteArrayList.Add(img.Data);
            }

            double ruleStep = Input.StepSize.Meters;
            double lambdaCenter = Configuration.LambdaCenter.Meters;     // wavelength [m] /!\ require IN METERS by the algorithm (SI) (default 621e-09)
            double fwhmLambda = Configuration.FwhmLambda.Meters;    // spectral bandwidth [m] /!\ require IN METERS by the algorithm (SI) (default 124e-09)
            double noiseLevel = Configuration.NoiseLevel;
            double maskThreshold = Configuration.MaskThreshold;

            var imagesBytesArray = byteArrayList.ToArray();

            // clean memory
            byteArrayList.Clear();
            images.Clear();

            CheckCancellation();

            var vsiOutput = UnitySCPMSharedAlgosVSIWrapper.VSI.ComputeTopography(imagesBytesArray, dataWidth, dataHeight, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);
            if (vsiOutput.Status == UnitySCPMSharedAlgosVSIWrapper.StatusCode.OK)
            {
                var objectiveCalibration = HardwareUtils.GetObjectiveParameters(_calibrationManager, Input.ObjectiveId);

                double pxSizeX = objectiveCalibration.Image.PixelSizeX.Micrometers;
                double pxSizeY = objectiveCalibration.Image.PixelSizeY.Micrometers;
                using (var mff = new MatrixFloatFile())
                {
                    bool useCompression = false;

                    var addData = new MatrixFloatFile.AdditionnalHeaderData(pxSizeX, pxSizeY, "um", "um", "um");
                    var header = new MatrixFloatFile.HeaderData(dataHeight, dataWidth, addData);
                    byte[] byteArrayOutput = mff.WriteInMemory(header, vsiOutput.ResultArray, useCompression);

                    var serviceImg = new ServiceImage();
                    serviceImg.Type = ServiceImage.ImageType._3DA;
                    serviceImg.DataHeight = dataHeight;
                    serviceImg.DataWidth = dataWidth;
                    serviceImg.Data = byteArrayOutput;

                    Result.TopographyImage = serviceImg;
                }
            }
            else
            {
                throw new Exception($"{vsiOutput.Status} during VSI topography computation.");
            }

            Logger.Debug($"{LogHeader} VSI computation done");
        }

        private void WriteReportIfNeeded()
        {
            try
            {
                CheckCancellation();

                if (Configuration.IsAnyReportEnabled())
                {
                    SetProgressMessage($"{LogHeader} WriteReport saving images");
                    _steps.SaveImages(ReportFolder);
                    if (Result.TopographyImage != null)
                    {
                        SaveTopographyImageAs3DA(Result.TopographyImage);
                    }
                    Configuration.Serialize(Path.Combine(ReportFolder, $"config.txt"));
                }
            }
            catch (Exception)
            {
                Logger.Debug($"Failed to generate the report for the VSIFlow");
            }
        }

        private void SaveTopographyImageAs3DA(ServiceImage topographyImage)
        {
            string filename = Path.Combine(ReportFolder, $"Topography_image.3da");
            topographyImage.SaveToFile(filename);
        }

        private void MoveInStartPosition()
        {
            var intialPosition = _hardwareManager.Axes.GetPos().ToXYZTopZBottomPosition();
            XYZTopZBottomPosition startPosition = (XYZTopZBottomPosition)intialPosition.Clone();
            var referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
            startPosition = referentialManager.ConvertTo(startPosition, Input.StartPosition.Referential.Tag).ToXYZTopZBottomPosition();

            Length piezoCurrentPos = _piezoController.GetCurrentPosition();
            startPosition.ZTop = Input.StartPosition.ZTop + (_piezoStartPosition - piezoCurrentPos).Millimeters;
            startPosition.X = double.NaN;
            startPosition.Y = double.NaN;
            startPosition.ZBottom = double.NaN;

            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, startPosition);
            GoToPiezoPosition(_piezoStartPosition);
        }

        private void GoToPiezoPosition(Length piezoPosition)
        {
            var speed = _piezoAxis.AxisConfiguration.SpeedNormal;
            _piezoController.SetPosAxisWithSpeedAndAccel(
                new List<double> { piezoPosition.Millimeters },
                new List<IAxis> { _piezoAxis },
                new List<double> { speed },
                new List<double>()
            );
            waitPiezoReachPosition(piezoPosition);
        }

        private void waitPiezoReachPosition(Length position)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            _piezoController.WaitMotionEnd(WaitPiezoMotionEndTimeout_ms);

            var timeToSleep = (int)(Configuration.PiezoMinStabilisationTime_ms - stopWatch.ElapsedMilliseconds);
            if (timeToSleep > 0)
            {
                Thread.Sleep(timeToSleep);
            }

            if (!SpinWait.SpinUntil(() => PiezoIsAtPosition(position), WaitPiezoMotionEndTimeout_ms))
            {
                throw new Exception($"{LogHeader} Piezo is not at a valid position.");
            }
        }

        private bool PiezoIsAtPosition(Length position)
        {
            var piezoPosition = _piezoController.GetCurrentPosition();

            bool result = piezoPosition.Micrometers.Near(position.Micrometers, Configuration.PiezoPositionStabilisationAccuracy.Micrometers);
            if (!result)
            {
                Logger.Verbose($"{LogHeader} piezoPosition {piezoPosition} is at position {position} => {result}");
            }
            else
            {
                Logger.Verbose($"{LogHeader} piezoPosition {piezoPosition} is at position {position} => {result}");
            }

            return result;
        }
    }
}
