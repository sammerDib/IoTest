using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public class ProbeLiseDllProxyDummy : ProbeLiseDllProxy
    {
        public override int Average
        {
            get; set;
        }

        public override bool AutoAirGap
        {
            get { return true; }
            set { }
        }

        public override bool AutoGain
        {
            get { return true; }
            set { }
        }

        private const int ContinuousAcquisitionPeriod = 500;
        private ILogger _logger = ClassLocator.Default.GetInstance<ILogger>();

        private const int LiseSignalLength = 32000;
        private const int RefPeakArbitraryPositionTop = 600;
        private const int RefPeakArbitraryPositionBottom = 500;

        // /!\ Must be between 0 and 1500
        private const int FirstPeakArbitraryPositionTop = 2000;

        private const int FirstPeakArbitraryPositionBottom = 1600;

        // /!\ Must be greater than 1500
        private const float GeometricToMicrometerRatio = 2.6F;

        private const double MaterialRefractionIndex = 1.4621;

        // A very ugly way to override our Singleton class.
        // Used only for simulated mode, please do not copy.
        private static ProbeLiseDllProxyDummy s_instance = null;

        private static readonly object s_padlock = new object();

        public new static ProbeLiseDllProxyDummy Instance
        {
            get
            {
                lock (s_padlock)
                {
                    if (s_instance == null)
                    {
                        s_instance = new ProbeLiseDllProxyDummy();
                    }
                    return s_instance;
                }
            }
        }

        public override IEnumerable<ProbeLiseSignal> PerformSingleAcquisition(IProbeConfig configuration, IProbeInputParams inputParams)
        {
            Average = (inputParams as SingleLiseInputParams).NbMeasuresAverage;
            var originalSignal = GetRawSignal(configuration);
            return Enumerable.Repeat(originalSignal, Average);
        }

        public override void ContinuousAcquisitionTask(IProbeConfig configuration, IProbeInputParams inputParams, CancellationToken token, Action<ProbeLiseSignal> notifier)
        {
            _logger?.Information($"[ProbeLise] A continuous acquisition is started.");
            try
            {
                CurrentAcquisitionParams = inputParams;
                CurrentAcquisitionConfig = configuration;
                Notifier = notifier;
                Status = ProbeLiseProxyStatus.Acquisition;

                while (!token.IsCancellationRequested)
                {
                    var rawSignal = GetRawSignal(configuration);
                    notifier(rawSignal);
                    Thread.Sleep(ContinuousAcquisitionPeriod);
                }
            }
            catch (Exception e)
            {
                string message = $"[ProbeLise] Continuous acquisition task failed: {e.Message}";
                _logger?.Error(message);
            }
            finally
            {
                Status = ProbeLiseProxyStatus.Free;
                CurrentAcquisitionParams = null;
                CurrentAcquisitionConfig = null;
                _logger?.Information($"[ProbeLise] A continuous acquisition is stopped.");
            }
        }

        public override IEnumerable<ProbeLiseSignal> PerformSingleAcquisitionWithInterruption(IProbeConfig configuration, IProbeInputParams inputParams)
        {
            var currentAcquisitionParamsStored = CurrentAcquisitionParams;
            var currentAcquisitionConfigStored = CurrentAcquisitionConfig;

            StopContinuousAcquisition();

            var rawSignal = PerformSingleAcquisition(configuration, inputParams);

            StartContinuousAcquisition(currentAcquisitionConfigStored, currentAcquisitionParamsStored, Notifier);

            return rawSignal;
        }

        private ProbeLiseSignal GetRawSignal(IProbeConfig configuration)
        {
            return CreateLiseSignal(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex, configuration);
        }

        private ProbeLiseSignal CreateLiseSignal(List<Length> layersThickness, double refractionIndex, IProbeConfig configuration)
        {
            var random = new Random();

            var peaksPosition = new List<int>();
            if (configuration.ModulePosition == PM.Shared.Hardware.Service.Interface.ModulePositions.Up)
            {
                peaksPosition.Add(RefPeakArbitraryPositionTop - (int)Math.Floor(random.NextDouble() * 10));
                peaksPosition.Add(FirstPeakArbitraryPositionTop + (int)Math.Floor(random.NextDouble() * 10));
            }
            else
            {
                peaksPosition.Add(RefPeakArbitraryPositionBottom - (int)Math.Floor(random.NextDouble() * 10));
                peaksPosition.Add(FirstPeakArbitraryPositionBottom + (int)Math.Floor(random.NextDouble() * 10));
            }

            foreach (var layerThickness in layersThickness)
            {
                double geometricDist = layerThickness.Micrometers / GeometricToMicrometerRatio;
                double opticalDist = geometricDist * refractionIndex;
                int peakPos = (peaksPosition[peaksPosition.Count - 1] + (int)opticalDist) + (int)Math.Floor(random.NextDouble() * 10);
                peaksPosition.Add(peakPos);
            }

            return CreateLiseSignal(configuration, peaksPosition, GeometricToMicrometerRatio, LiseSignalLength);
        }

        private ProbeLiseSignal CreateLiseSignal(IProbeConfig configuration, List<int> peaksPosition, float geometricToMicrometerRatio, int nbValues, int saturationValue = 7, int peakAmplitude = 6)
        {
            var signal = new ProbeLiseSignal
            {
                SaturationValue = saturationValue,
                StepX = (1.0f / geometricToMicrometerRatio) * 1000.0f, //nanometers
                ProbeID = configuration.DeviceID
            };

            var rnd = new Random();
            for (int i = 0; i < nbValues; i++)
            {
                signal.RawValues.Add(-3.01 + 0.05 * rnd.NextDouble());
            }

            int niter = 0;
            double stepX_Micron = (double)signal.StepX / 1000.0;
            foreach (int x_um in peaksPosition)
            {
                double rawPeakAmplitude = (double) peakAmplitude - 0.5 * rnd.NextDouble();
                int indX = (int) Math.Round((double) x_um / stepX_Micron);
                signal.RawValues[indX] = rawPeakAmplitude;
                signal.RawValues[signal.RawValues.Count - indX] = rawPeakAmplitude;

                if(niter == 0)
                    signal.ReferencePeaks.Add(new ProbePoint(indX, rawPeakAmplitude));
                else
                    signal.SelectedPeaks.Add(new ProbePoint(indX, rawPeakAmplitude));
                niter++;
            }

            var rankdiscarspeak_um = 1000.0 + 10 * rnd.NextDouble() - 5;
            var ampldiscardpeak = 0.5 + 0.1 * rnd.NextDouble();
            int indRk = (int)Math.Round((double)rankdiscarspeak_um / stepX_Micron);
            signal.RawValues[(int)indRk] = ampldiscardpeak;
            signal.DiscardedPeaks.Add(new ProbePoint(indRk, ampldiscardpeak));
            signal.DiscardedPeaks.Add(new ProbePoint((signal.RawValues.Count - indRk), ampldiscardpeak));

            return signal;
        }

        public bool Equals(ProbeLiseDllProxyDummy other)
        {
            throw new NotImplementedException();
        }
    }
}
