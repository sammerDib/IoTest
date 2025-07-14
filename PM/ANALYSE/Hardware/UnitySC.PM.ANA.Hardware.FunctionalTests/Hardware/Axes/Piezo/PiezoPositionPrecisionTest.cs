using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes.Piezo
{
    public class PiezoPositionPrecisionTest : FunctionalTest
    {
        private const string FileName = "report_piezo_positions_over_time.csv";
        private const string PiezoControllerID = "PI-E709-1";
        private readonly PIE709Controller _piezoController;
        private double _speed = 2000;
        private Length _signedStepSize = -40.Nanometers();
        private Length _initialPos = 100.Micrometers();
        private Length _finalPos = 99.8.Micrometers();
        private int _positionRecordTime_ms = 3000;

        private Dictionary<double, List<double>> _targetedPositions = new Dictionary<double, List<double>>(); // <target position, recorded positions>
        private Dictionary<double, List<double>> _targetedTimes = new Dictionary<double, List<double>>(); // <target position, recorded times>

        public PiezoPositionPrecisionTest()
        {
            _piezoController = (PIE709Controller)HardwareManager.Axes.AxesControllers.First(controller => controller.DeviceID == PiezoControllerID);
        }

        public override void Run()
        {
            PrepareHardware();
            TestPiezoPositionPrecision();
            WriteReport();
            Logger.Information("PiezoPositionPrecisionTest done.");
        }

        private void PrepareHardware()
        {
            _piezoController.SetSpeed(_speed);
            _piezoController.MoveTo(_initialPos);
        }

        private void TestPiezoPositionPrecision()
        {
            var targetPosition = _initialPos + _signedStepSize;

            while (targetPosition < _finalPos)
            {
                _piezoController.MoveTo(targetPosition);

                Logger.Debug($"Record position {targetPosition} for {_positionRecordTime_ms} ms");
                (_targetedPositions[targetPosition.Nanometers], _targetedTimes[targetPosition.Nanometers]) = recordCurrentPositionsDuring(_positionRecordTime_ms);

                targetPosition += _signedStepSize;
            }
        }

        // TestPiezoPositionPrecision2 version with piezo trigger IN
        /*private void TestPiezoPositionPrecision()
        {
            _piezoController.ConfigureTriggerIn(_initialPos, _stepSize);

            try
            {
                var targetPosition = _initialPos + _stepSize;

                while (targetPosition < _finalPos)
                {
                    HardwareManager.Plc?.StartTriggerOutEmitSignal();

                    Logger.Debug($"Record position {targetPosition} for {_positionRecordTime_ms} ms");
                    (_targetedPositions[targetPosition.Nanometers], _targetedTimes[targetPosition.Nanometers]) = recordCurrentPositionsDuring(_positionRecordTime_ms);

                    targetPosition += _stepSize;
                }
            }
            finally
            {
                _piezoController.DisableTriggerIn();
            }
        }*/

        private (List<double>, List<double>) recordCurrentPositionsDuring(int milliseconds)
        {
            var positions = new List<double>();
            var times = new List<double>();
            var s_stopwatch = new Stopwatch();
            s_stopwatch.Restart();

            SpinWait.SpinUntil(() =>
            {
                bool timeUp = s_stopwatch.ElapsedMilliseconds > milliseconds;
                if (!timeUp)
                {
                    positions.Add(_piezoController.GetCurrentPosition().Nanometers);
                    times.Add(s_stopwatch.ElapsedMilliseconds);
                }
                return timeUp;
            });
            return (positions, times);
        }

        private void WriteReport()
        {
            try
            {

                string reportFolder = Path.Combine(Logger.LogDirectory, "Tests", "TestPiezoPositionPrecision");
                Directory.CreateDirectory(reportFolder);
                string filepath = Path.Combine(reportFolder, FileName);

                using (StreamWriter file = new StreamWriter(filepath))
                {
                    foreach (double position in _targetedPositions.Keys)
                    {
                        file.WriteLine("Test : " + position);
                        file.WriteLine("Time (ms);Position (nm)");

                        var positions = _targetedPositions[position];
                        var times = _targetedTimes[position];

                        for (int i = 0; i < positions.Count; i++)
                        {
                            file.WriteLine(times[i] + ";" + positions[i]);
                        }
                        file.WriteLine();
                    }
                }
            }
            catch
            {
                Logger.Error("Error in file reporting.");
            }
        }
    }
}
