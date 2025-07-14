using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using MathNet.Numerics.Statistics;

using UnitySC.PM.ANA.Service.Core.AutofocusV2;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Flows.AFCameraV2
{
    // This test is working on tool NST7 2238 and NST6 2229.
    // For 100 executions, the test will run about 1 hours.
    // Focus positions store in _zTopFocusPosition could change a little bit (few µm) according temperature, moment of the day, ...
    // Light intensities store in _whiteLedIntensityList came from autolight flow except for INT objectives
    // Report location : ANALYSE\Hardware\UnitySC.PM.ANA.Hardware.FunctionalTests\bin\Logs\Tests\TestAFV2Camera
    public class AFV2CameraTest : FunctionalTest
    {
        private const int NbExecution = 100;
        private readonly string _cameraId = "1";
        private readonly string _whiteLedId = "VIS_WHITE_LED";
        private readonly XYPosition _refCamPosition;
        private readonly ScanRangeType _scanRangeType = ScanRangeType.Large;
        private readonly string[] _objectiveIdList;
        private readonly double[] _whiteLedIntensityList;
        private readonly double[] _zTopFocusPosition;

        private double _scanRangeCoeff;
        private AFCameraConfiguration _aFCameraConfig;
        private string _prevObjectiveId;
        private string _currentObjectiveId;
        private double _currentLightIntensity;
        private double _currentZStartPosition;

        private double[] _zPositionsResults = new double[NbExecution];
        private double[] _executionTime = new double[NbExecution];


        public AFV2CameraTest(string toolId)
        {
            // Warning, 5X INT objective must be the last objective to test
            if (toolId == "4MET2238")
            {
                _refCamPosition = new XYPosition(new StageReferential(), -207.556, 140.884);
                _objectiveIdList = new string[] { "ID-2XVIS01", "ID-5XNIR01", "ID-10XNIR01", "ID-20XNIR01", "ID-50XNIR01", "ID-50X INT01", "ID-5X INT01" };
                _whiteLedIntensityList = new double[] { 6, 6.5, 9, 10, 14.5, 15, 7 };
                _zTopFocusPosition = new double[] { 18.928, 13.580, 7.240, 1.228, 1.434, -8.033, -3.716 };
            }
            else if (toolId == "4MET2229")
            {
                _refCamPosition = new XYPosition(new StageReferential(), -208.536, 142.186);
                _objectiveIdList = new string[] { "ID-2XVIS01", "ID-5XNIR01", "ID-10XNIR01", "ID-20XNIR01", "ID-50XNIR01", "ID-50XVIS01", "ID-50X INT01", "ID-5X INT01" };
                _whiteLedIntensityList = new double[] { 8.5, 10, 12.5, 13, 40, 18.5, 40, 12 };
                _zTopFocusPosition = new double[] { 19.509, 14.176, 8.283, 2.219, 2.100, 2.115, -4.449, -3.191 };
            }
        }

        public override void Run()
        {
            InitConfigAndScanRangeCoeff();

            for (int i = 0; i < _objectiveIdList.Length; i++)
            {
                _prevObjectiveId = _currentObjectiveId;
                _currentObjectiveId = _objectiveIdList[i];
                _currentLightIntensity = _whiteLedIntensityList[i];
                _currentZStartPosition = _zTopFocusPosition[i];
                Logger.Information("\n\n\n" + "AFV2 Camera test starting for objective " + _currentObjectiveId);
                ExecuteAFV2CameraFlow();
                DisplayResults();
                writeReport();
                _zPositionsResults = new double[NbExecution];
                _executionTime = new double[NbExecution];
            }
            Logger.Information("AFV2 Camera test Done");
        }

        private void ExecuteAFV2CameraFlow()
        {
            var AFCameraInput = new AFCameraInput()
            {
                CameraId = _cameraId,
                RangeType = _scanRangeType,
                UseCurrentZPosition = true,
            };

            var stopWatch = new Stopwatch();
            double percentStep = ((1.0 / _objectiveIdList.Length) / NbExecution) * 100.0;
            double percentDone = Array.IndexOf(_objectiveIdList, _currentObjectiveId) * NbExecution * percentStep;

            for (int i = 0; i < NbExecution; i++)
            {
                PrepareHardware();
                var AFV2CameraFlow = new AFV2CameraFlow(AFCameraInput);
                stopWatch.Restart();
                var result = AFV2CameraFlow.Execute();
                _executionTime[i] = stopWatch.ElapsedMilliseconds;
                _zPositionsResults[i] = result.ZPosition;
                Logger.Information($"---------- OBJECTIVE {_currentObjectiveId} - EXEC N°{i + 1} ({Math.Round(percentStep * i + percentDone, 2)}%) ----------");
            }
        }

        private void DisplayResults()
        {
            Logger.Information("AFV2 Camera test report for objective " + _currentObjectiveId + " : ");
            Logger.Information("White Light intensity =  " + _currentLightIntensity);
            Logger.Information("Min    Z    =  " + _zPositionsResults.Min());
            Logger.Information("Max    Z    =  " + _zPositionsResults.Max());
            Logger.Information("Max - Min   =  " + (_zPositionsResults.Max() - _zPositionsResults.Min()));
            Logger.Information("Mean   Z    =  " + _zPositionsResults.Mean());
            Logger.Information("Median Z    =  " + _zPositionsResults.Median());
            Logger.Information("StandardDeviation Z =  " + _zPositionsResults.StandardDeviation());
            Logger.Information("3Sigma              =  " + 3 * _zPositionsResults.StandardDeviation());
            Logger.Information("Variance          Z =  " + _zPositionsResults.Variance());
            Logger.Information("RootMeanSquare    Z =  " + _zPositionsResults.RootMeanSquare());
        }

        private void writeReport()
        {
            try
            {
                string reportFolder = Path.Combine(Logger.LogDirectory, "Tests", "TestAFV2Camera");
                Directory.CreateDirectory(reportFolder);
                string fileName = ("report_" + _currentObjectiveId + ".txt").Replace(" ", "");
                string filepath = Path.Combine(reportFolder, fileName);

                var objectiveConfig = HardwareManager.GetObjectiveConfigs().Find(_ => _.DeviceID == _currentObjectiveId);
                if (objectiveConfig == null)
                {
                    throw new Exception("Objective Configuration is null");
                }
                
                var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
                var objCalibration = calibrationManager.GetObjectiveCalibration(_currentObjectiveId);
                var scanRange = _aFCameraConfig.AutoFocusScanRange * _scanRangeCoeff;
                var stepSize = (objectiveConfig.DepthOfField * _aFCameraConfig.FactorBetweenDepthOfFieldAndStepSize).Millimeters;
                stepSize = Math.Max(_aFCameraConfig.MinZStep.Millimeters, stepSize);

                using (StreamWriter file = new StreamWriter(filepath))
                {
                    file.WriteLine("AFV2 Camera test report for objective " + _currentObjectiveId + " : ");
                    file.WriteLine("Position track period : " + _aFCameraConfig.PositionTrackingPeriod_ms);
                    file.WriteLine("Framerate Limiter     : " + _aFCameraConfig.CameraFramerateLimiter);
                    file.WriteLine("Piezo Step            : " + _aFCameraConfig.PiezoStep);
                    file.WriteLine("Nb execution          : " + NbExecution);
                    file.WriteLine("Mean execution time   : " + _executionTime.Mean() / 1000 + " s");
                    file.WriteLine("Scan range size/type  : " + scanRange + "/" + _scanRangeType);
                    file.WriteLine("Step size             : " + stepSize + " mm");
                    file.WriteLine("White Light intensity :  " + _currentLightIntensity + "%");
                    file.WriteLine("Min    Z  (mm) =  " + _zPositionsResults.Min());
                    file.WriteLine("Max    Z  (mm) =  " + _zPositionsResults.Max());
                    file.WriteLine("Max - Min (mm) =  " + (_zPositionsResults.Max() - _zPositionsResults.Min()));
                    file.WriteLine("Mean   Z  (mm) =  " + _zPositionsResults.Mean());
                    file.WriteLine("Median Z  (mm) =  " + _zPositionsResults.Median());
                    file.WriteLine("StandardDeviation (mm) =  " + _zPositionsResults.StandardDeviation());
                    file.WriteLine("3Sigma            (mm) =  " + 3 * _zPositionsResults.StandardDeviation());
                    file.WriteLine("Variance          (mm) =  " + _zPositionsResults.Variance());
                    file.WriteLine("\nZPositions results :");
                    foreach (var zPos in _zPositionsResults)
                    {
                        file.WriteLine(zPos);
                    }
                }
            }
            catch
            {
                Logger.Error("Error in file reporting.");
            }
        }

        private void PrepareHardware()
        {
            SelectObjective();
            GoToPosition();
            SetLightIntensity();
        }

        private void GoToPosition()
        {
            var startPosition = _refCamPosition.ToXYZTopZBottomPosition();
            startPosition.ZTop = _currentZStartPosition;

            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            if (_prevObjectiveId != null)
            {
                var prevObjCalib = calibrationManager.GetObjectiveCalibration(_prevObjectiveId);
                startPosition.X -= prevObjCalib?.Image?.XOffset?.Millimeters ?? 0.0;
                startPosition.Y -= prevObjCalib?.Image?.YOffset?.Millimeters ?? 0.0;
            }

            var objCalibration = calibrationManager.GetObjectiveCalibration(_currentObjectiveId);
            if (!(objCalibration is null))
            {
                // New offset
                startPosition.X += objCalibration?.Image?.XOffset?.Millimeters ?? 0.0;
                startPosition.Y += objCalibration?.Image?.YOffset?.Millimeters ?? 0.0;
            }

            HardwareManager.Axes.GotoPosition(startPosition, Shared.Hardware.Service.Interface.Axes.AxisSpeed.Fast);
            HardwareManager.Axes.WaitMotionEnd(20000);
        }

        private void SelectObjective()
        {
            HardwareManager.ObjectivesSelectors.TryGetValue("ObjectiveSelector01", out var objectiveSelector);
            var objectiveConfig = objectiveSelector.Config.FindObjective(_currentObjectiveId);
            objectiveSelector.SetObjective(objectiveConfig);
            objectiveSelector.WaitMotionEnd();
            HardwareManager.Axes.WaitMotionEnd(20000);
        }

        private void SetLightIntensity()
        {
            HardwareManager.Lights.TryGetValue(_whiteLedId, out var whiteLed);
            whiteLed.SetIntensity(_currentLightIntensity);
        }

        private void InitConfigAndScanRangeCoeff()
        {
            var flowConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();
            _aFCameraConfig = flowConfiguration?.Flows?.OfType<AFCameraConfiguration>().FirstOrDefault();
            if (_aFCameraConfig == null)
            {
                throw new Exception("Autofocus camera Configuration is null");
            }

            switch (_scanRangeType)
            {
                case ScanRangeType.Small:
                    _scanRangeCoeff = _aFCameraConfig.SmallRangeCoeff;
                    break;

                case ScanRangeType.Medium:
                    _scanRangeCoeff = _aFCameraConfig.MediumRangeCoeff;
                    break;

                case ScanRangeType.Large:
                    _scanRangeCoeff = _aFCameraConfig.LargeRangeCoeff;
                    break;

                default:
                    throw new ArgumentException($"Scan range type {_scanRangeType} is not supported for Autofocus camera Test");
            }
        }
    }
}
