using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public static class LiseMeasurement
    {
        public static SingleLiseResult DoMeasure(IProbeLise probeLise, LiseAcquisitionParams acquisitionParams, LiseSignalAnalysisParams analysisParams)
        {
            var analyzedSignal = AcquireAverageRawSignal(probeLise, acquisitionParams).Select(signal => new LISESignalAnalyzed(signal));
            var timestamp = DateTime.UtcNow;
            var config = probeLise.Configuration as ProbeLiseConfig;

            var result = new SingleLiseResult();
            try
            {
                result.LayersThickness = ComputeAverageLayersThickness(acquisitionParams.Sample, analyzedSignal, config);
                var airGapResult = ComputeAverageAirGap(analyzedSignal, config);

                result.AirGap = airGapResult.AirGap;

                var airGapQuality = airGapResult.Quality;
                var layersQuality = QualityScore.AggregateLayersQuality(result.LayersThickness);
                result.Quality = (airGapQuality + layersQuality) / 2;
            }
            catch
            {
                result.AirGap = 0.Nanometers();
                result.LayersThickness = InitializeThicknessesMeasured(acquisitionParams.Sample);
                result.Quality = 0;
            }

            result.Timestamp = timestamp;
            return result;
        }

        public static ProbeAirGapMeasure DoAirGap(IProbeLise probeLise, LiseAcquisitionParams acquisitionParams, LiseSignalAnalysisParams analysisParams)
        {
            var config = probeLise.Configuration as ProbeLiseConfig;

            try
            {
                return ComputeAverageAirGap(
                    AcquireAverageRawSignal(probeLise, acquisitionParams).Select(signal => new LISESignalAnalyzed(signal)), 
                    config);
            }
            catch
            {
                return new ProbeAirGapMeasure();
            }
        }

        public static List<SingleLiseResult> DoMultipleMeasures(IProbeLise probeLise, LiseAcquisitionParams acquisitionParams, LiseSignalAnalysisParams analysisParams, int nbMeasures, List<IEnumerable<ProbeLiseSignal>> rawSignals = null)
        {
            int thicknessMeasuresPeriodInMs = 100;

            var results = new List<SingleLiseResult>();
            while (results.Count < nbMeasures)
            {
                var measure = DoMeasure(probeLise, acquisitionParams, analysisParams);
                results.Add(measure);
                if (rawSignals != null)
                {
                    rawSignals.Add(new List<ProbeLiseSignal> { acquisitionParams.RawSignalAcquired }); 
                }
                Thread.Sleep(thicknessMeasuresPeriodInMs);
            }

            return results;
        }

        public static DualLiseResult DoUnknownLayerMeasure(IProbeDualLise probeLiseDouble, Length globalThickness, DualLiseAcquisitionParams acquisitionParams, ProbeSampleLayer unknownLayer)
        {
            var airGapUp = 0.Micrometers();
            var airGapDown = 0.Micrometers();
            var airGapQuality = 0.0;
            var unknownLayerThicknessQuality = 0.0;
            var timestamp = DateTime.UtcNow;

            var expectedUpLayersCount = acquisitionParams.LiseUpParams.Sample.Layers.Count;
            var expectedDownLayersCount = acquisitionParams.LiseDownParams.Sample.Layers.Count;
            var resultLayersThickness = new List<ProbeThicknessMeasure>();
            for (int layerId = 0; layerId < expectedUpLayersCount; layerId++)
            {
                var layer = acquisitionParams.LiseUpParams.Sample.Layers[layerId];
                resultLayersThickness.Add(new ProbeThicknessMeasure(0.Millimeters(), 0, layer.IsMandatory, layer.Name));
            }
            resultLayersThickness.Add(new ProbeThicknessMeasure(0.Millimeters(), 0, unknownLayer.IsMandatory, unknownLayer.Name));
            for (int layerId = 0; layerId < expectedDownLayersCount; layerId++)
            {
                var layer = acquisitionParams.LiseDownParams.Sample.Layers[layerId];
                resultLayersThickness.Add(new ProbeThicknessMeasure(0.Millimeters(), 0, layer.IsMandatory, layer.Name));
            }

            try
            {
                var probeUp = probeLiseDouble.ProbeLiseUp;
                var probeDown = probeLiseDouble.ProbeLiseDown;
                var configUp = probeUp.Configuration as ProbeLiseConfig;
                var configDown = probeDown.Configuration as ProbeLiseConfig;

                // Acquire signal up and down and use only one timestamp for both
                var analyzedSignalUp = AcquireAverageRawSignal(probeUp, acquisitionParams.LiseUpParams).Select(signal => new LISESignalAnalyzed(signal));
                var analyzedSignalDown = AcquireAverageRawSignal(probeDown, acquisitionParams.LiseDownParams).Select(signal => new LISESignalAnalyzed(signal));

                var layersThicknessUp = ComputeAverageLayersThickness(acquisitionParams.LiseUpParams.Sample, analyzedSignalUp, configUp);

                if (expectedUpLayersCount != layersThicknessUp.Count)
                {
                    throw new ArgumentException("The measured layers number does not correspond to the expected number.");
                }

                for (int i = 0; i < layersThicknessUp.Count; i++)
                {
                    resultLayersThickness[i] = layersThicknessUp[i];
                }


                var layersThicknessDown = ComputeAverageLayersThickness(acquisitionParams.LiseDownParams.Sample, analyzedSignalDown, configDown);

                if (expectedDownLayersCount != layersThicknessDown.Count)
                {
                    throw new ArgumentException("The measured layers number does not correspond to the expected number.");
                }

                for (int i = 0; i < layersThicknessDown.Count; i++)
                {
                    resultLayersThickness[layersThicknessUp.Count + 1 + i] = layersThicknessDown[i];
                }

                var knownLayersThicknesses = layersThicknessUp.Concat(layersThicknessDown).ToList();

                var airGapUpResult = ComputeAverageAirGap(analyzedSignalUp, configUp);
                var airGapDownResult = ComputeAverageAirGap(analyzedSignalDown, configDown);
                airGapUp = airGapUpResult.AirGap;
                airGapDown = airGapDownResult.AirGap;
                var totalAirGap = airGapUp + airGapDown;
                airGapQuality = (airGapUpResult.Quality + airGapDownResult.Quality) / 2;

                var unknownLayerThickness = ComputeUnknownLayerThickness(globalThickness, knownLayersThicknesses, totalAirGap, airGapQuality);
                var unknownLayerIndex = layersThicknessUp.Count;
                resultLayersThickness[unknownLayerIndex].Thickness = unknownLayerThickness.Thickness;
                resultLayersThickness[unknownLayerIndex].Quality = unknownLayerThickness.Quality;
                unknownLayerThicknessQuality = unknownLayerThickness.Quality;
            }
            catch
            {
            }

            return new DualLiseResult
            {
                LayersThickness = resultLayersThickness,
                AirGapUp = airGapUp,
                AirGapDown = airGapDown,
                Quality = (airGapQuality + unknownLayerThicknessQuality) / 2,
                Timestamp = timestamp
            };
        }

        public static bool AreMandatoryMeasuresValid(List<ProbeThicknessMeasure> layersThickness)
        {
            foreach (var measure in layersThickness)
            {
                if (measure.IsMandatory && measure.Quality == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static Tuple<ProbeAirGapMeasure, List<ProbeThicknessMeasure>> CalibrateLise(IProbeLise probeLise, LiseAcquisitionParams acquisitionParams, LiseSignalAnalysisParams analysisParams)
        {
            try
            {
                var config = probeLise.Configuration as ProbeLiseConfig;

                var analyzedSignal = AcquireAverageRawSignal(probeLise, acquisitionParams).Select(signal => new LISESignalAnalyzed(signal));

                var layersThickness = ComputeAverageLayersThickness(acquisitionParams.Sample, analyzedSignal, config);
                var airGap = ComputeAverageAirGap(analyzedSignal, config);

                return new Tuple<ProbeAirGapMeasure, List<ProbeThicknessMeasure>>(airGap, layersThickness);
            }
            catch (Exception e)
            {
                throw new Exception("Calibration of lise failed due to an internal exception : " + e);
            }
        }

        public static Length CalibrateDualLise(DualLiseAcquisitionParams acquisitionParams, Length airGapUp, Length airGapDown, List<ProbeThicknessMeasure> layersThicknessUp, List<ProbeThicknessMeasure> layersThicknessDown)
        {
            try
            {
                var totalAirGap = airGapUp + airGapDown;
                var globalThickness = ComputeGlobalThickness(acquisitionParams.LiseUpParams.Sample, layersThicknessUp, layersThicknessDown, totalAirGap);

                return globalThickness;
            }
            catch (Exception e)
            {
                throw new Exception("Calibration of bottom lise failed due to an internal exception : " + e);
            }
        }

        private static List<ProbeThicknessMeasure> ComputeLayersThickness(ProbeSample sample, LISESignalAnalyzed analyzedSignal, ProbeLiseConfig config)
        {
            if (analyzedSignal.SignalStatus != LISESignalAnalyzed.SignalAnalysisStatus.Valid)
            {
                throw new Exception("Lise analyzed signal is not valid.");
            }

            var layersThickness = InitializeThicknessesMeasured(sample);

            for (int layerID = 0; layerID < sample.Layers.Count; layerID++)
            {
                int entryPeakID = layerID;
                int outputPeakID = entryPeakID + 1;

                if (analyzedSignal.SelectedPeaks.Count > outputPeakID)
                {
                    if (sample.Layers[layerID].RefractionIndex == 0 || double.IsNaN(sample.Layers[layerID].RefractionIndex))
                    {
                        layersThickness[layerID].Thickness = 0.Micrometers();
                        layersThickness[layerID].Quality = 0;
                    }
                    else
                    {
                        var entryPeak = analyzedSignal.SelectedPeaks[entryPeakID];
                        var outputPeak = analyzedSignal.SelectedPeaks[outputPeakID];

                        double opticalDist = outputPeak.X - entryPeak.X;
                        double geometricDist = opticalDist / sample.Layers[layerID].RefractionIndex;
                        var thickness = (geometricDist * analyzedSignal.StepX).Nanometers();
                        layersThickness[layerID].Thickness = thickness;

                        bool thicknessIsWithinTolerance = sample.Layers[layerID].Tolerance.IsInTolerance(thickness, sample.Layers[layerID].Thickness);
                        if (thicknessIsWithinTolerance)
                        {
                            double entryPeakQuality = QualityScore.ComputeQualityScore(analyzedSignal, entryPeakID, config);
                            double outputPeakQuality = QualityScore.ComputeQualityScore(analyzedSignal, outputPeakID, config);
                            layersThickness[layerID].Quality = (entryPeakQuality + outputPeakQuality) / 2;
                        }
                    }
                }
            }

            return layersThickness;
        }

        private static List<ProbeThicknessMeasure> ComputeAverageLayersThickness(ProbeSample sample, IEnumerable<LISESignalAnalyzed> analyzedSignals, ProbeLiseConfig config)
        {
            var allThicknesses = analyzedSignals.Select(signal => ComputeLayersThickness(sample, signal, config));

            var averagedList = new List<ProbeThicknessMeasure>();
            for (int idxLayer = 0; idxLayer < allThicknesses.First().Count(); ++idxLayer)
            {
                averagedList.Add(new ProbeThicknessMeasure { 
                    Thickness = allThicknesses.Average(thicksOneSignal => thicksOneSignal[idxLayer].Thickness.Micrometers).Micrometers(),
                    Quality = allThicknesses.Average(thicksOneSignal => thicksOneSignal[idxLayer].Quality),
                    IsMandatory = allThicknesses.First()[idxLayer].IsMandatory,
                    Name = allThicknesses.First()[idxLayer].Name,
                });
            }

            return averagedList;
        }

        private static ProbeAirGapMeasure ComputeAirGap(LISESignalAnalyzed analyzedSignal, ProbeLiseConfig config)
        {
            if (analyzedSignal.SignalStatus != LISESignalAnalyzed.SignalAnalysisStatus.Valid)
            {
                throw new Exception("Lise analyzed signal is not valid.");
            }

            var result = new ProbeAirGapMeasure();
            var airGap = 0.Nanometers();

            bool analyzedSignalCanBeUsedForAirGap = analyzedSignal.ReferencePeaks.Count == 1 && analyzedSignal.SelectedPeaks.Count >= 1;
            if (analyzedSignalCanBeUsedForAirGap)
            {
                var refPeak = analyzedSignal.ReferencePeaks[0];
                var firstPeak = analyzedSignal.SelectedPeaks[0];

                double opticalDist = firstPeak.X - refPeak.X;
                double airRefractionIndex = 1;
                double geometricDist = opticalDist / airRefractionIndex;

                double airGapNanometer = geometricDist * analyzedSignal.StepX;
                airGap = airGapNanometer.Nanometers();
            }

            result.AirGap = airGap;
            result.Quality = QualityScore.ComputeQualityScore(analyzedSignal, 0, config);

            return result;
        }

        private static ProbeAirGapMeasure ComputeAverageAirGap(IEnumerable<LISESignalAnalyzed> analyzedSignals, ProbeLiseConfig config)
        {
            var res = new ProbeAirGapMeasure()
            {
                AirGap = 0.Micrometers(),
                Quality = 0.0,
            };
            int size = 0;

            foreach (var signal in analyzedSignals)
            {
                var airGap = ComputeAirGap(signal, config);
                res.AirGap += airGap.AirGap;
                res.Quality += airGap.Quality;
                ++size;
            }
            if (size > 0)
            {
                res.AirGap /= size;
                res.Quality /= size;
            }

            return res;
        }

        private static ProbeThicknessMeasure ComputeUnknownLayerThickness(Length globalThickness, List<ProbeThicknessMeasure> knownLayersThicknesses, Length totalAirGap, double airGapQuality)
        {
            if (!AreMandatoryMeasuresValid(knownLayersThicknesses))
            {
                throw new Exception($"Some mandatory layer thicknesses could not be measured or with insufficient precision.");
            }

            var unknownThickness = globalThickness - totalAirGap;
            double quality = airGapQuality;
            foreach (var layer in knownLayersThicknesses)
            {
                unknownThickness -= layer.Thickness;
                quality += layer.Quality;
            }
            quality = quality / (knownLayersThicknesses.Count + 1);

            return new ProbeThicknessMeasure(unknownThickness, quality);
        }

        private static Length ComputeGlobalThickness(ProbeSample sample, List<ProbeThicknessMeasure> layersUp, List<ProbeThicknessMeasure> layersDown, Length totalAirGap)
        {
            bool notEnoughMeasurements = sample.Layers.Count != layersUp.Count || sample.Layers.Count != layersDown.Count;
            if (notEnoughMeasurements)
            {
                throw new ArgumentException("Some layer thickness measurements are missing.");
            }

            var globalThickness = totalAirGap;
            for (int i = 0; i < sample.Layers.Count; i++)
            {
                int downIndex = sample.Layers.Count - 1 - i;
                if (!sample.Layers[i].Tolerance.IsInTolerance(layersUp[i].Thickness, sample.Layers[i].Thickness) || !sample.Layers[i].Tolerance.IsInTolerance(layersDown[downIndex].Thickness, sample.Layers[i].Thickness))
                {
                    throw new Exception("Compute global thickness failed because at least one layer measured is outside tolerance.");
                }
                globalThickness += (layersUp[i].Thickness + layersDown[downIndex].Thickness) / 2;
            }

            return globalThickness;
        }

        private static List<ProbeThicknessMeasure> InitializeThicknessesMeasured(ProbeSample sample)
        {
            int nbLayers = sample.Layers.Count;

            var layersThickness = new List<ProbeThicknessMeasure>();
            for (int layerId = 0; layerId < nbLayers; layerId++)
            {
                var layer = sample.Layers[layerId];
                layersThickness.Add(new ProbeThicknessMeasure(0.Millimeters(), 0, layer.IsMandatory, layer.Name));
            }

            return layersThickness;
        }
    }
}
