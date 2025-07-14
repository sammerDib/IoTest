using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Image;

namespace UnitySC.PM.ANA.Service.Core.PSI
{
    public class PSIFlow : FlowComponent<PSIInput, PSIResult, PSIConfiguration>
    {
        private const int WaitPiezoMotionEndTimeout_ms = 10_000;

        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        private readonly CameraBase _camera;
        private readonly PiezoController _piezoController;
        private readonly IAxis _piezoAxis;

        private readonly USPCameraImageTracker _uspCameraImageTracker;
        private readonly TopographyComputation _topographyLib;

        private PSISteps _steps;
        private Rect _oldCameraAOI = Rect.Empty;

        public PSIFlow(PSIInput input, USPCameraImageTracker cameraImageTracker = null, TopographyComputation topographyLib = null) : base(input, "PSIFlow")
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
            _uspCameraImageTracker = cameraImageTracker ?? new USPCameraImageTracker(_camera);
            _topographyLib = topographyLib ?? new TopographyComputation();
        }

        protected override void Process()
        {
            SetProgressMessage("Starting PSI Flow");
            try
            {
                InitialSetup();
                AcquireImages();
                ComputeTopographyImage();
                WriteReportIfNeeded();
            }
            finally
            {
                GoToPiezoPosition(_piezoAxis.AxisConfiguration.PositionHome);
                _camera.SetAOI(_oldCameraAOI);
                _hardwareManager.Axes.StopLanding();
            }
        }

        private void InitialSetup()
        {
            CheckCancellation();

            var signedStepSize = -Input.StepSize;
            var piezoInitialPosition = _piezoController.GetCurrentPosition();
            _steps = new PSISteps(piezoInitialPosition, Input.StepCount, signedStepSize);
            SetCameraAOI();
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
            CheckCancellation();
            Logger.Debug($"{LogHeader} Start Image acquisition for {Input.StepCount} steps");

            var isAcquiringFromOutside = _camera.IsAcquiring;
            if (!isAcquiringFromOutside)
            {
                _camera.StartContinuousGrab();
            }

            foreach (var step in _steps.GetOrderedSteps())
            {
                GoToPiezoPosition(step.StartPosition);
                _uspCameraImageTracker.StartTrackingUntil(Input.ImagesPerStep);
                step.Images.AddRange(_uspCameraImageTracker.Images);

                CheckCancellation();
            }

            if (!isAcquiringFromOutside)
            {
                _camera.StopContinuousGrab();
            }

            Logger.Debug($"{LogHeader} Image acquisition Done");
        }

        private void WriteReportIfNeeded()
        {
            CheckCancellation();

            if (Configuration.IsAnyReportEnabled())
            {
                SetProgressMessage($"{LogHeader} WriteReport saving images");
                _steps.SaveImages(ReportFolder);
                if (Result.NanoTopographyImage != null)
                {
                    Result.NanoTopographyImage.SaveToFile(Path.Combine(ReportFolder, $"NanoTopography_image.png"));
                    SaveNormmalizedTopographyImage(Result.NanoTopographyImage);
                    SaveTopographyImageAs3DA(Result.NanoTopographyImage);
                }
                Configuration.Serialize(Path.Combine(ReportFolder, $"config.txt"));
            }
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

        private void ComputeTopographyImage()
        {
            Logger.Debug($"{LogHeader} Start PSI computation");
            try
            {
                // computation phase
                var images = _steps.GetAllImages();
                var currentObjective = _hardwareManager.GetObjectiveInUseByCamera(Input.CameraId);
                var objectiveCalibration = HardwareUtils.GetObjectiveParameters(_calibrationManager, currentObjective.DeviceID);
                var addData = new MatrixFloatFile.AdditionnalHeaderData(objectiveCalibration.Image.PixelSizeX.Millimeters, objectiveCalibration.Image.PixelSizeY.Millimeters, "mm", "mm", "μm");

                var resultPSI = _topographyLib.ComputeTopography(images, Input.Wavelength, Input.StepCount, addData);
                Result.NanoTopographyImage = resultPSI.TopographyMap;
            }
            catch (Exception e)
            {
                throw new Exception($"[PSI flow] Error during NanoTopography computation: {e.Message} {e.StackTrace}");
            }
            finally
            {
                Logger.Debug($"{LogHeader} PSI computation done");
            }
        }

        private void SaveNormmalizedTopographyImage(ServiceImage topographyImage)
        {
            if (topographyImage.Depth != 32 && topographyImage.Depth != 5)
                throw new ArgumentException($"Bad Service image depth");

            float[] floatArray = null;
            using (var mff = new MatrixFloatFile(topographyImage.Data))
            {
                floatArray = MatrixFloatFile.AggregateChunks(mff.GetChunkStatus(), mff);
            }
            var normalizedFloatArray = NormalizeData(floatArray, 0, 255);
            var newBitmap = new Bitmap(topographyImage.DataWidth, topographyImage.DataHeight, PixelFormat.Format24bppRgb);
            // get source bitmap pixel format size
            int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(newBitmap.PixelFormat);
            int cCount = nDepth / 8;
            int nSzX = topographyImage.DataWidth;
            int nSzY = topographyImage.DataHeight;
            unsafe
            {
                // pour du rgb24
                BitmapData newData = newBitmap.LockBits(new Rectangle(0, 0, nSzX, nSzY), ImageLockMode.WriteOnly, newBitmap.PixelFormat);
                fixed (int* pNorm = normalizedFloatArray)
                {
                    int* ptrStartArray = pNorm;
                    System.Threading.Tasks.Parallel.For(0, nSzY, y =>
                    //for (int y = 0; y < nSzY; y++)
                    {
                        byte* pRow = (byte*)newData.Scan0 + (y * newData.Stride);
                        int* pNormRow = (int*)(ptrStartArray + (y * nSzX));

                        for (int x = 0; x < nSzX; x++)
                        {
                            pRow[2] = pRow[1] = pRow[0] = (byte)pNormRow;
                            pRow += cCount;
                            pNormRow++;
                        }
                    });
                }
                //unlock the bitmaps
                newBitmap.UnlockBits(newData);
            }
            newBitmap.Save(Path.Combine(ReportFolder, $"normalized_NanoTopography_image.png"));
        }

        private void SaveTopographyImageAs3DA(ServiceImage topographyImage)
        {
            string filename = Path.Combine(ReportFolder, $"NanoTopography_image.3da");
            topographyImage.SaveToFile(filename);
        }

        private static int[] NormalizeData(IEnumerable<float> data, int min, int max)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = dataMax - dataMin;

            return data
                .Select(d => (d - dataMin) / range) //normalizes the input to be from 0 to 1 (0 being minimum, 1 being the maximum)
                .Select(n => (int)((1 - n) * min + n * max)) // takes that normalized number, and maps it to the new minimum and maximum
                .ToArray();
        }

        private static float[] ConvertByteArrayToFloat(byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException("byteArray");

            if (byteArray.Length % 4 != 0)
                throw new ArgumentException("Byte array does not represent a sequence of floats");

            // create a float array and copy the bytes into it...
            var floatArray = new float[byteArray.Length / 4];
            Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
            return floatArray;
        }
    }
}
