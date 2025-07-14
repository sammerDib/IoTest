using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Newtonsoft.Json;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.Shared.Tools.Collection;
using Point = UnitySC.PM.Shared.MathLib.Geometry.Point;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.PM.Shared;
using UnitySC.Shared.Format.Helper;
using UnitySC.PM.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Calibration
{
    public class LiseGainCalibration
    {
        private readonly ILogger _logger;
        private readonly AnaHardwareManager _hardwareManager;
        private readonly double _minValueOfSignal = -5;
        private readonly double _maxValueOfSignal = 6.7;
        private readonly string _siliciumJawFilename = "Silicium_jaw.nk";
        private readonly double _coefA = 3.97454152544329;
        private readonly double _coefC = 0.164686225566167;
        private readonly string _linearRegFilename = "linearReg";

        public class LinearReg
        {
            public double A { get; set; }
            public double B { get; set; }
        }

        public class CalibrationResult
        {
            public string ToolName;
            public LinearReg LinearReg;
            public List<Point> LambdaFromGain;
            public Dictionary<double, List<Length>> MeasuredThicknessesByGain;
        }

        public LiseGainCalibration()
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _logger = ClassLocator.Default.GetInstance<ILogger<LiseCalibration>>();
        }

        public Tuple<double, double> FindAppropriateGainRangeToLiseGainCalibration(ProbeSample sample, string probeID)
        {
            var LISE = _hardwareManager.Probes[probeID] as IProbeLise;

            double minGain = 0;
            double maxGain = 5.5;
            double gainStep = 0.1;

            var maxPeakThreeshold = _maxValueOfSignal - gainStep;
            var minPeakThreeshold = _minValueOfSignal + gainStep;

            double minGainCalibrated = minGain;
            double maxGainCalibrated = maxGain;
            bool gainMinCalibrated = false;
            bool gainMaxCalibrated = false;

            double medianGainValue = Math.Round((minGain + maxGain) / 2, 1);
            double currentGainValue = medianGainValue;

            var numberOfSteps = Math.Abs(medianGainValue - maxGain) / gainStep;
            var stepNumber = 0;

            _logger.Debug($"[GainCalibration] Minimum gain calibration begins.");
            bool validPeaksHaveBeenDetected = false;
            List<Tuple<double, double>> stddevAtGain = new List<Tuple<double, double>>();

            while (!gainMinCalibrated && stepNumber <= numberOfSteps)
            {
                _logger.Debug($"[GainCalibration] - current gain value : {currentGainValue}.");

                if (currentGainValue < minGain)
                {
                    minGainCalibrated = minGain;
                    gainMinCalibrated = true;
                    _logger.Warning($"[GainCalibration] The search range of the gain is exceeded, the calibration of the min gain takes by default the minimum value of the search range but a better gain could possibly have been obtained beyond this range.");
                }

                var acquisitionParams = new LiseAcquisitionParams(currentGainValue, 1, sample);
                var rawSignal = AcquireRawSignal(LISE, acquisitionParams);
                var analyzedSignal = new LISESignalAnalyzed(rawSignal);

                var areEnoughPeaks = analyzedSignal.SelectedPeaks.Count() >= 2;
                var peaksAreBigEnough = analyzedSignal.SelectedPeaks.IsEmpty() ? false : analyzedSignal.SelectedPeaks.Min(p => p.Y) >= minPeakThreeshold;
                var peaksAreNotSaturated = analyzedSignal.SelectedPeaks.IsEmpty() ? false : analyzedSignal.SelectedPeaks.Max(p => p.Y) < maxPeakThreeshold;
                if (areEnoughPeaks && peaksAreBigEnough)
                {
                    validPeaksHaveBeenDetected = true;
                    if (peaksAreNotSaturated)
                    {
                        double currentStddev = rawSignal.RawValues.StandardDeviation();
                        stddevAtGain.Add(new Tuple<double, double>(currentGainValue, currentStddev));
                    }
                }
                else if (validPeaksHaveBeenDetected)
                {
                    var previousGainValue = currentGainValue + gainStep;
                    minGainCalibrated = previousGainValue;
                    gainMinCalibrated = true;
                }

                currentGainValue = Math.Round(currentGainValue - gainStep, 1);
                stepNumber++;
            }

            if (!gainMinCalibrated)
            {
                var msg = "Not enough peaks detected. Check that you are well positioned on the wafer and that the top probe is well positioned !";
                _logger.Warning($"[GainCalibration] " + msg);
                throw new Exception("Minimum gain calibration failed : " + msg);
            }

            _logger.Debug($"[GainCalibration] Minimum gain calibration was successful at : {minGainCalibrated}.");
            _logger.Debug($"[GainCalibration] Maximum gain calibration begins.");

            stddevAtGain.Reverse();
            int maxStddevId = stddevAtGain.Select((value, index) => new { Value = value, Index = index })
                                        .Aggregate((a, b) => (a.Value.Item2 > b.Value.Item2) ? a : b)
                                        .Index;
            Tuple<double, double> maxStddev = stddevAtGain.ElementAt(maxStddevId);

            maxGainCalibrated = maxStddev.Item1;
            if (maxGainCalibrated < medianGainValue)
            {
                _logger.Debug($"[GainCalibration] Maximum gain calibration was successful at : {maxGainCalibrated}.");
                return new Tuple<double, double>(minGainCalibrated, maxGainCalibrated);
            }

            currentGainValue = medianGainValue + gainStep;
            double previousStddev = maxStddev.Item2;
            stepNumber = 0;

            while (!gainMaxCalibrated && stepNumber <= numberOfSteps)
            {
                _logger.Debug($"[GainCalibration] - current gain value : {currentGainValue}.");

                if (currentGainValue > maxGain)
                {
                    maxGainCalibrated = maxGain;
                    _logger.Warning($"[GainCalibration] The search range of the gain is exceeded, the calibration of the max gain takes by default the maximum value of the search range but a better gain could possibly have been obtained beyond this range.");
                    _logger.Debug($"[GainCalibration] Maximum gain calibration was successful at : {maxGainCalibrated}.");
                    return new Tuple<double, double>(minGainCalibrated, maxGainCalibrated);
                }

                var acquisitionParams = new LiseAcquisitionParams(currentGainValue, 1, sample);
                var rawSignal = AcquireRawSignal(LISE, acquisitionParams);
                var analyzedSignal = new LISESignalAnalyzed(rawSignal);

                double currentStddev = rawSignal.RawValues.StandardDeviation();

                if (analyzedSignal.SelectedPeaks.Count() >= 2)
                {
                    var maxPeak = analyzedSignal.SelectedPeaks.Max(p => p.Y);
                    var atLeastOnePeakIsSaturated = maxPeak >= maxPeakThreeshold;
                    if (atLeastOnePeakIsSaturated)
                    {
                        stddevAtGain.Add(new Tuple<double, double>(currentGainValue, currentStddev));
                        _logger.Debug($"[GainCalibration] Maximum gain calibration was successful at : {maxGainCalibrated}.");
                        return new Tuple<double, double>(minGainCalibrated, maxGainCalibrated);
                    }
                    else if (currentStddev > previousStddev)
                    {
                        maxGainCalibrated = currentGainValue;
                    }
                }

                previousStddev = currentStddev;
                currentGainValue = Math.Round(currentGainValue + gainStep, 1);
                stepNumber++;
            }

            _logger.Debug($"[GainCalibration] Maximum gain calibration failed.");
            throw new Exception("Maximum gain calibration failed.");
        }

        public CalibrationResult Calibration(string toolName, Length theoricalThickness, ProbeSample sample, string probeID, double minGain = 0, double maxGain = 5.5, double gainStep = 0.05)
        {
            var probeLise = _hardwareManager.Probes[probeID] as IProbeLise;

            var calibrationResult = new CalibrationResult();
            calibrationResult.ToolName = toolName;

            _logger.Debug($"[Calibration] Begin LISE ED gain calibration.");

            try
            {
                if (theoricalThickness.Micrometers == 0)
                {
                    throw new Exception("Theorical thickness should not be zero.");
                }

                var measuredThicknessesByGain = new Dictionary<double, List<Length>>();
                var lambdaFromGain = new List<Point>();

                for (double gain = minGain; gain <= maxGain; gain = Math.Round(gain + gainStep, 2))
                {
                    _logger.Debug($"[Calibration] Gain : " + gain);

                    var thicknesses = new List<Length>();

                    for (int i = 0; i < 10; i++)
                    {
                        var acquisitionParams = new LiseAcquisitionParams(gain, HighPrecisionMeasurement, sample);
                        var rawSignal = AcquireRawSignal(probeLise, acquisitionParams);
                        var analyzedSignal = new LISESignalAnalyzed(rawSignal);

                        if (analyzedSignal.SelectedPeaks.Count >= 2)
                        {
                            analyzedSignal.SelectedPeaks.OrderBy(p => p.Y);
                            analyzedSignal.SelectedPeaks.Reverse();
                            double refractiveIndex = 1;
                            double opticalDist = Math.Abs((analyzedSignal.SelectedPeaks[1].X - analyzedSignal.SelectedPeaks[0].X)) / refractiveIndex;
                            var thickness = (opticalDist * analyzedSignal.StepX).Nanometers();

                            thicknesses.Add(thickness);
                            _logger.Debug($"[Calibration] Measured thickness : " + thickness);
                        }
                    }

                    if (thicknesses.IsEmpty())
                    {
                        _logger.Debug("Any thickness measured at gain : " + gain);
                    }
                    else
                    {
                        thicknesses = RemoveOutLiers(thicknesses);
                        measuredThicknessesByGain.Add(gain, thicknesses);

                        var averageThickness = thicknesses.Average(item => item.Micrometers).Micrometers();
                        _logger.Debug($"[Calibration] Average thickness : " + averageThickness);
                        var apparentGroupRefractiveIndex = averageThickness.Micrometers / theoricalThickness.Micrometers;
                        _logger.Debug($"[Calibration] Apparent group refractive index : " + apparentGroupRefractiveIndex);
                        var lambda = Math.Sqrt((_coefA - apparentGroupRefractiveIndex) / _coefC);
                        _logger.Debug($"[Calibration] Lambda : " + lambda);
                        lambdaFromGain.Add(new Point(gain, lambda));
                    }
                }
                var a = MathTools.Variance(lambdaFromGain, p => p.X, p => p.Y) / MathTools.Variance(lambdaFromGain, p => p.X, p => p.X);
                var b = lambdaFromGain.Average(p => p.Y) - a * lambdaFromGain.Average(p => p.X);
                var linearReg = new LinearReg() { A = a, B = b };

                calibrationResult.LinearReg = linearReg;
                calibrationResult.LambdaFromGain = lambdaFromGain;
                calibrationResult.MeasuredThicknessesByGain = measuredThicknessesByGain;
            }
            catch (Exception ex)
            {
                _logger.Information($"[Calibration] " + ex.Message);
            }

            _logger.Information($"[Calibration] Calibration was successful.");
            return calibrationResult;
        }

        public Dictionary<double, Length> TestCalibration(string toolName, ProbeSample sample, string probeID, double minGain = 0, double maxGain = 5.5, double gainStep = 0.05)
        {
            var measuredThicknessesByGain = new Dictionary<double, Length>();
            _logger.Debug($"[TestCalibration] Begin calibration test.");

            try
            {
                if (sample == null || sample.Layers.IsEmpty() || sample.Layers[0] == null)
                {
                    throw new Exception("Valid layer must be provided to compute calibration test.");
                }

                var path = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath + "\\" + _siliciumJawFilename;
                var materialData = MaterialData.LoadMaterialDataFromFile(path, "silicium");
                sample.Layers[0].RefractionIndex = materialData.DefaultGroupRefractiveIndex;

                var linearReg = ReadLinearRegFile(toolName);

                var probeLise = _hardwareManager.Probes[probeID] as IProbeLise;

                for (double gain = minGain; gain <= maxGain; gain = Math.Round(gain + gainStep, 2))
                {
                    _logger.Debug($"[TestCalibration] Gain : " + gain);

                    if (linearReg != null)
                    {
                        var lambda = linearReg.B + linearReg.A * gain;
                        var groupRefractionIndex = materialData.CalculateGroupRefractiveIndexAccordingToWavelength(lambda);
                        sample.Layers[0].RefractionIndex = groupRefractionIndex;
                    }
                    else
                    {
                        _logger.Warning($"[TestCalibration] No linearReg file be found.");
                    }

                    var acquisitionParams = new LiseAcquisitionParams(gain, HighPrecisionMeasurement, sample);
                    var rawSignal = AcquireRawSignal(probeLise, acquisitionParams);
                    var analyzedSignal = new LISESignalAnalyzed(rawSignal);

                    if (analyzedSignal.SelectedPeaks.Count >= 2)
                    {
                        double opticalDist = Math.Abs((analyzedSignal.SelectedPeaks[1].X - analyzedSignal.SelectedPeaks[0].X)) / sample.Layers[0].RefractionIndex;
                        var thickness = (opticalDist * analyzedSignal.StepX).Nanometers();
                        measuredThicknessesByGain.Add(gain, thickness);
                        _logger.Debug($"[TestCalibration] Measured thickness : " + thickness);
                    }
                    else
                    {
                        throw new Exception("Unable to measure thickness with the refraction index : " + sample.Layers[0].RefractionIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Information($"[TestCalibration] " + ex.Message);
            }

            return measuredThicknessesByGain;
        }

        private List<Length> RemoveOutLiers(List<Length> allNumbers)
        {
            List<Length> normalNumbers = new List<Length>();
            List<Length> outLierNumbers = new List<Length>();
            Length average = allNumbers.Average(item => item.Micrometers).Micrometers();
            Length standardDeviation = Math.Sqrt(allNumbers.Average(item => Math.Pow(item.Micrometers - average.Micrometers, 2))).Micrometers();
            foreach (Length number in allNumbers)
            {
                if ((Math.Abs(number.Micrometers - average.Micrometers)) > (2 * standardDeviation.Micrometers))
                    outLierNumbers.Add(number);
                else
                    normalNumbers.Add(number);
            }

            return normalNumbers;
        }

        private void WriteLookUpTableFile(string toolName, LinearReg linearReg)
        {
            string jsonSerializedObj = JsonConvert.SerializeObject(linearReg);
            var path = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath + "\\" + _linearRegFilename + "_" + toolName + ".json";
            File.WriteAllText(path, jsonSerializedObj);
        }

        private LinearReg ReadLinearRegFile(string toolName)
        {
            var path = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath + "\\" + _linearRegFilename + "_" + toolName + ".json";
            if (!File.Exists(path))
            {
                return null;
            }

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                LinearReg items = JsonConvert.DeserializeObject<LinearReg>(json);
                return items;
            }
        }

        private void WriteLambdaFromGainCsvFile(string toolName, List<Point> lambdaFromGain)
        {
            string separator = CSVStringBuilder.GetCSVSeparator();

            String csv = String.Join(Environment.NewLine, lambdaFromGain.Select(d => $"{d.X}{separator}{d.Y}{separator}"));
            File.WriteAllText(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath + "\\" + "CalculatedLambdaForEachGain.csv", csv);
        }

        private void WriteThicknessesFromGainCsvFile(string toolName, Dictionary<double, List<Length>> thicknessesFromGain)
        {
            var sbCSV = new CSVStringBuilder();
            foreach (var thicknessByGain in thicknessesFromGain)
            {
                foreach (var thickness in thicknessByGain.Value)
                {
                    sbCSV.AppendLine(thicknessByGain.Key.ToString(), thickness.Micrometers.ToString() );
                }
            }
            File.WriteAllText(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath + "\\" + "MeasuredThicknessesAtEAchGain.csv", sbCSV.ToString()); ;
        }

        private void WriteThicknessFromGainCsvFile(string toolName, Dictionary<double, Length> thicknessFromGain, string filename)
        {
            string separator = CSVStringBuilder.GetCSVSeparator();

            String csv = String.Join(Environment.NewLine, thicknessFromGain.Select(d => $"{d.Key}{separator}{d.Value.Micrometers}{separator}"));
            File.WriteAllText(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath + "\\" + filename, csv);
        }
    }
}
