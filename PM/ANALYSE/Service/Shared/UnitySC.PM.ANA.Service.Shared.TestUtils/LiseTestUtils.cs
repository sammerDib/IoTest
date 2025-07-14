using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public static class LiseTestUtils
    {
        public const int LiseSignalLength = 32000;
        public const int RefPeakArbitraryPosition = 1400; // /!\ Must be between 0 and 1500
        public const int FirstPeakArbitraryPosition = 5000; // /!\ Must be greater than 1500
        public static LengthTolerance Tolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer);
        public static Length AirGapUp = 500.Micrometers();
        public static Length AirGapDown = 600.Micrometers();
        public static Length Thickness50 = 50.Micrometers();
        public static Length Thickness750 = 750.Micrometers();
        public static Length Thickness200 = 200.Micrometers();
        public static Length Thickness180 = 180.Micrometers();
        public static Length Thickness25 = 25.Micrometers();
        public const float StepXInNano = 384.615F; // match stepX of LisED raw signal in Nanometers as is it acquire 
        public const float GeometricToMicrometerRatio = StepXInNano / 1000.0F; //GeoToMicronRatio = match stepX of LisED raw signal in Micrometer (in real signal ~ 384.6nm) Micron = Geo * GeoToMicronRatio
        public const float MicrometertoGeometricRatio = 1.0F / GeometricToMicrometerRatio; //MicronToGeo = 1/stepxinmicron | Geo = Micron * MicronToGeo
        public static double MaterialRefractionIndex = 1.43;
        public static double MaterialRefractionIndex2 = 1.125;
        public static double AirRefractionIndex = 1.0;

        public static ProbeSampleLayer UnknownLayer = new ProbeSampleLayer(0.Micrometers(), Tolerance, 1);
        public static ProbeSampleLayer Layer750 = new ProbeSampleLayer(Thickness750, Tolerance, MaterialRefractionIndex);
        public static ProbeSampleLayer Layer200 = new ProbeSampleLayer(Thickness200, Tolerance, MaterialRefractionIndex);
        public static ProbeSampleLayer Layer180 = new ProbeSampleLayer(Thickness180, Tolerance, MaterialRefractionIndex2);
        public static ProbeSampleLayer Layer25 = new ProbeSampleLayer(Thickness25, Tolerance, MaterialRefractionIndex2);

        public struct SampleAndSignal
        {
            public ProbeSample Sample;
            public ProbeLiseSignal SignalLiseUp;
            public ProbeLiseSignal SignalLiseDown;
        }

        public static ProbeLiseSignal CreateFakeLiseSignalCorrespondingToOneTSVOf750MicrometersDepth()
        {
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, AirRefractionIndex);

            double geometricDist = Thickness750.Micrometers * MicrometertoGeometricRatio;
            double opticalDist = geometricDist * AirRefractionIndex;
            int secondPeakPos = FirstPeakArbitraryPosition + (int)opticalDist;

            var analyzedSignal = new ProbeLiseSignal();
            analyzedSignal.RawValues = signal.RawValues.ToList();
            analyzedSignal.Means = new List<double>();
            analyzedSignal.StdDev = new List<double>();
            analyzedSignal.ReferencePeaks = new List<ProbePoint>() { new ProbePoint(RefPeakArbitraryPosition, 6) };
            analyzedSignal.SelectedPeaks = new List<ProbePoint>() { new ProbePoint(FirstPeakArbitraryPosition, 6), new ProbePoint(secondPeakPos, 6) };
            analyzedSignal.StepX =  StepXInNano; // aka (1000 * StepXInNano) ou (1 / (1000 * MicrometertoGeometricRatio))

            return analyzedSignal;
        }

        public static ProbeLiseSignal CreateFakeLiseSignal(double[] rawSignal, int referencePeakPosition, Length airGap, double micrometersToGeometricRatio)
        {
            var signal = new ProbeLiseSignal();
            signal.RawValues = rawSignal.ToList();
            signal.Means = new List<double>();
            signal.StdDev = new List<double>();
            signal.ReferencePeaks = new List<ProbePoint>();
            signal.SelectedPeaks = new List<ProbePoint>();
            if (referencePeakPosition > 0)
            {
                signal.ReferencePeaks.Add(new ProbePoint(referencePeakPosition, 6));
            }
            if (airGap != null)
            {
                signal.SelectedPeaks.Add(new ProbePoint(airGap.Micrometers * micrometersToGeometricRatio + referencePeakPosition, 6));
            }
            signal.StepX = (float)(1.0 / micrometersToGeometricRatio * 1000.0); // pour nano

            return signal;
        }

        public static SampleAndSignal CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(ProbeSampleLayer layerUp, ProbeSampleLayer layerDown, Length airGapUp, Length airGapDown)
        {
            return new SampleAndSignal()
            {
                Sample = new ProbeSample(new List<ProbeSampleLayer> { layerUp, UnknownLayer, layerDown }, "Name", "SampleInfo"),
                SignalLiseUp = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { layerUp }, airGapUp.Micrometers, GeometricToMicrometerRatio, LiseSignalLength),
                SignalLiseDown = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { layerDown }, airGapDown.Micrometers, GeometricToMicrometerRatio, LiseSignalLength)
            };
        }

        public static SampleAndSignal CreateSampleAndItsAssociatedSplitSignalForLayersSeparatedByOneUnknownLayer(List<ProbeSampleLayer> layersUp, List<ProbeSampleLayer> layersDown, Length airGapUp, Length airGapDown)
        {
            var allLayers = new List<ProbeSampleLayer>();
            foreach (var layer in layersUp)
            {
                allLayers.Add(layer);
            }
            allLayers.Add(UnknownLayer);
            foreach (var layer in layersDown)
            {
                allLayers.Add(layer);
            }

            return new SampleAndSignal()
            {
                Sample = new ProbeSample(allLayers, "Name", "SampleInfo"),
                SignalLiseUp = CreateLiseSignalFromSampleLayers(layersUp, airGapUp.Micrometers, GeometricToMicrometerRatio, LiseSignalLength),
                SignalLiseDown = CreateLiseSignalFromSampleLayers(layersDown, airGapDown.Micrometers, GeometricToMicrometerRatio, LiseSignalLength)
            };
        }

        public static SampleAndSignal CreateSampleAndItsAssociatedSignalForOneLayer(ProbeSampleLayer layer, Length airGapUp, Length airGapDown)
        {
            return new SampleAndSignal()
            {
                Sample = new ProbeSample(new List<ProbeSampleLayer> { layer }, "Name", "SampleInfo"),
                SignalLiseUp = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { layer }, airGapUp.Micrometers, GeometricToMicrometerRatio, LiseSignalLength),
                SignalLiseDown = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { layer }, airGapDown.Micrometers, GeometricToMicrometerRatio, LiseSignalLength)
            };
        }

        public static ProbeSample CreateProbeSample(List<Length> layersThickness, double refractionIndex)
        {
            var probeSampleLayers = new List<ProbeSampleLayer>();

            foreach (var layerThickness in layersThickness)
            {
                var probeSampleLayer = new ProbeSampleLayer(layerThickness, Tolerance, refractionIndex);
                probeSampleLayers.Add(probeSampleLayer);
            }

            return CreateProbeSample(probeSampleLayers);
        }

        public static ProbeSample CreateProbeSample(List<ProbeSampleLayer> probeSampleLayers)
        {
            var sample = new ProbeSample(probeSampleLayers, "SampleName", "SampleInfo");

            return sample;
        }

        public static ProbeLiseSignal CreateNullLiseSignal()
        {
            return new ProbeLiseSignal();
        }

        public static ProbeLiseSignal CreateLiseSignalWithoutPeak(float geometricToMicrometerRatio, int nbValues, int saturationValue = 7)
        {
            var signal = new ProbeLiseSignal
            {
                SaturationValue = saturationValue,
                StepX = geometricToMicrometerRatio * 1000 //nanometers
            };

            for (int i = 0; i < nbValues; i++)
            {
                signal.RawValues.Add(-3.01);
                signal.Means.Add(-3.01);
                signal.StdDev.Add(0);
            }

            return signal;
        }

        public static ProbeLiseSignal CreateLiseSignalWithOnlyAirGap(Length airGapThickness)
        {
            double geometricDist = airGapThickness.Micrometers * MicrometertoGeometricRatio;
            double opticalDist = geometricDist * AirRefractionIndex;
            int peakPosition = RefPeakArbitraryPosition + (int)opticalDist;

            return CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition, peakPosition }, GeometricToMicrometerRatio, LiseSignalLength);
        }

        public static ProbeLiseSignal CreateLiseSignalFromLayerThicknesses(List<Length> layersThickness, double refractionIndex)
        {
            var peaksPosition = new List<int>();
            peaksPosition.Add(RefPeakArbitraryPosition);
            peaksPosition.Add(FirstPeakArbitraryPosition);

            foreach (var layerThickness in layersThickness)
            {
                double geometricDist = layerThickness.Micrometers / GeometricToMicrometerRatio;
                double opticalDist = geometricDist * refractionIndex;
                int peakPos = peaksPosition[peaksPosition.Count - 1] + (int)opticalDist;
                peaksPosition.Add(peakPos);
            }

            return CreateLiseSignalFromPeakPositions(peaksPosition, GeometricToMicrometerRatio, LiseSignalLength);
        }

        public static ProbeLiseSignal CreateLiseSignalFromSamples(ProbeSample sample)
        {
            var peaksPosition = new List<int>();
            peaksPosition.Add(RefPeakArbitraryPosition);
            peaksPosition.Add(FirstPeakArbitraryPosition);

            foreach (var layer in sample.Layers)
            {
                var layerThickness = layer.Thickness;
                var refractionIndex = layer.RefractionIndex;
                double geometricDist = layerThickness.Micrometers / GeometricToMicrometerRatio;
                double opticalDist = geometricDist * refractionIndex;
                int peakPos = peaksPosition[peaksPosition.Count - 1] + (int)opticalDist;
                peaksPosition.Add(peakPos);
            }

            return CreateLiseSignalFromPeakPositions(peaksPosition, GeometricToMicrometerRatio, LiseSignalLength);
        }

        public static ProbeLiseSignal CreateLiseSignalWithInterferences(List<Length> layersThickness, double refractionIndex)
        {
            var peaksPosition = new List<int>();
            peaksPosition.Add(RefPeakArbitraryPosition);
            peaksPosition.Add(FirstPeakArbitraryPosition);

            foreach (var layerThickness in layersThickness)
            {
                double geometricDist = layerThickness.Micrometers / GeometricToMicrometerRatio;
                double opticalDist = geometricDist * refractionIndex;
                int peakPos = peaksPosition[peaksPosition.Count - 1] + (int)opticalDist;
                peaksPosition.Add(peakPos);
            }

            var interferencePeaksPosition = new List<int>();
            peaksPosition.Add(FirstPeakArbitraryPosition + (LiseSignalLength - FirstPeakArbitraryPosition) / 20);
            peaksPosition.Add(FirstPeakArbitraryPosition + (LiseSignalLength - FirstPeakArbitraryPosition) / 10);

            return CreateLiseSignalFromPeakPositions(peaksPosition, GeometricToMicrometerRatio, LiseSignalLength, interferencePeaksPosition);
        }

        public static ProbeLiseSignal CreateLiseSignalFromItsSubparts(List<int> peaksPositionInFirstPartOfSignal, List<int> peaksPositionInSecondPartOfSignal, float geometricToMicrometerRatio, int nbValues)
        {
            var goingsSignal = CreateLiseSignalFromPeakPositions(peaksPositionInFirstPartOfSignal, geometricToMicrometerRatio, nbValues);
            var comingsSignal = CreateLiseSignalFromPeakPositions(peaksPositionInSecondPartOfSignal, geometricToMicrometerRatio, nbValues);

            var fullSignal = new ProbeLiseSignal();
            fullSignal.StepX = goingsSignal.StepX;
            fullSignal.SaturationValue = goingsSignal.SaturationValue;

            var part1 = goingsSignal.RawValues.GetRange(0, goingsSignal.RawValues.Count / 2);
            var part2 = comingsSignal.RawValues.GetRange(comingsSignal.RawValues.Count / 2, comingsSignal.RawValues.Count / 2);
            fullSignal.RawValues = part1.Concat(part2).ToList();

            return fullSignal;
        }

        public static ProbeLiseSignal CreateLiseSignalFromPeakPositions(List<int> peaksPosition, float geometricToMicrometerRatio, int nbValues, List<int> interferencePeaksPositions = null, int saturationValue = 7, int peakAmplitude = 6)
        {
            var signal = CreateLiseSignalWithoutPeak(geometricToMicrometerRatio, nbValues, saturationValue);

            signal.ReferencePeaks = new List<ProbePoint>();
            signal.SelectedPeaks = new List<ProbePoint>();
            signal.DiscardedPeaks = new List<ProbePoint>();

            foreach (int x in peaksPosition)
            {
                bool isReferencePeak = signal.ReferencePeaks.Count == 0;
                if (isReferencePeak)
                {
                    signal.ReferencePeaks.Add(new ProbePoint(x, peakAmplitude));
                }
                else
                {
                    signal.SelectedPeaks.Add(new ProbePoint(x, peakAmplitude));
                }
                signal.RawValues[x] = peakAmplitude;
                signal.RawValues[signal.RawValues.Count - x] = peakAmplitude;
            }

            if (interferencePeaksPositions != null)
            {
                foreach (int x in interferencePeaksPositions)
                {
                    signal.DiscardedPeaks.Add(new ProbePoint(x, peakAmplitude));

                    signal.RawValues[x] = peakAmplitude;
                    signal.RawValues[signal.RawValues.Count - x] = peakAmplitude;
                }
            }

            return signal;
        }

        public static ProbeLiseSignal CreateLiseSignalFromSampleLayers(List<ProbeSampleLayer> probeSampleLayers, double airGap, float geometricToMicrometerRatio, int nbValues)
        {
            var signal = CreateLiseSignalWithoutPeak(geometricToMicrometerRatio, nbValues);

            int refPeakPosition = RefPeakArbitraryPosition;
            signal.RawValues[refPeakPosition] = 6;
            signal.RawValues[signal.RawValues.Count - refPeakPosition] = 6;

            signal.ReferencePeaks.Add(new ProbePoint(RefPeakArbitraryPosition, 6));

            int airRefractionIndex = 1;
            int firstPeakPosition = (int)(RefPeakArbitraryPosition + airGap / geometricToMicrometerRatio * airRefractionIndex);
            signal.RawValues[firstPeakPosition] = 6;
            signal.RawValues[signal.RawValues.Count - firstPeakPosition] = 6;

            signal.SelectedPeaks.Add(new ProbePoint(firstPeakPosition, 6));

            int previousPeakPos = firstPeakPosition;
            foreach (var layer in probeSampleLayers)
            {
                double geometricDist = layer.Thickness.Micrometers / geometricToMicrometerRatio;
                double opticalDist = geometricDist * layer.RefractionIndex;
                int nextPeakPos = previousPeakPos + (int)opticalDist;
                signal.RawValues[nextPeakPos] = 6;
                signal.RawValues[signal.RawValues.Count - nextPeakPos] = 6;
                previousPeakPos = nextPeakPos;
                signal.SelectedPeaks.Add(new ProbePoint(nextPeakPos, 6));
            }

            return signal;
        }

        public static ProbeLiseSignal CreateLiseSignalFromPeaks(List<Peak> peaks, float geometricToMicrometerRatio, int nbValues)
        {
            var signal = CreateLiseSignalWithoutPeak(geometricToMicrometerRatio, nbValues);

            signal.ReferencePeaks = new List<ProbePoint>();
            signal.SelectedPeaks = new List<ProbePoint>();
            signal.DiscardedPeaks = new List<ProbePoint>();

            foreach (var peak in peaks)
            {
                bool isReferencePeak = signal.ReferencePeaks.Count == 0;
                if (isReferencePeak)
                {
                    signal.ReferencePeaks.Add(new ProbePoint(peak.X, peak.Y));
                }
                else
                {
                    signal.SelectedPeaks.Add(new ProbePoint(peak.X, peak.Y));
                }
                signal.RawValues[(int)peak.X] = peak.Y;
                signal.RawValues[signal.RawValues.Count - (int)peak.X] = peak.Y;
            }

            return signal;
        }

        public static LISESignalAnalyzed CreateLISESignalAnalyzed(ProbeLiseSignal rawSignal, List<Peak> refPeaks, List<Peak> interestPeaks, float saturationValue = 7, double stepX = 1, LISESignalAnalyzed.SignalAnalysisStatus signalStatus = LISESignalAnalyzed.SignalAnalysisStatus.Valid)
        {
            int signalLength = rawSignal.RawValues.Count;

            var signalAnalyzed = new LISESignalAnalyzed
            {
                RawValues = rawSignal.RawValues.ToList(),
                Means = new List<double>(),
                StdDev = new List<double>(),
                ReferencePeaks = refPeaks,
                SelectedPeaks = interestPeaks,
                SaturationValue = saturationValue,
                StepX = stepX,
                SignalStatus = signalStatus
            };

            for (int i = 0; i < signalLength; i++)
            {
                signalAnalyzed.Means.Add(rawSignal.RawValues.ElementAt(0));
                signalAnalyzed.StdDev.Add(0);
            }

            return signalAnalyzed;
        }

        public static List<double> CreateLiseRawSignalFromCSV(string csvName)
        {
            string csvPath = DirTestUtils.GetWorkingDirectoryDataPath("\\data\\LISESignal\\" + csvName);

            return ReadRawSignalFromCSV(csvPath);
        }

        private static List<double> ReadRawSignalFromCSV(string csvPath)
        {
            string separator = CSVStringBuilder.GetCSVSeparator();
            var cSep = new char[] { separator[0] };

            using (var reader = new StreamReader(csvPath))
            {
                var rawSignal = new List<double>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line?.Split(cSep);
                    if (values != null)
                    {
                        rawSignal.Add(double.Parse(values[0], System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                return rawSignal;
            }
        }
    }
}
