using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware.Shared.Algos;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public class ProbeLiseDllProxy : Singleton<ProbeLiseDllProxy>
    {
        public IProbeLiseWrapper ProbeLiseDll { get; } = ProbeLiseWrapper.Instance;
        public ProbeLiseProxyStatus Status { get; set; }
        private const int StandAloneDllKey = 9972;
        private const int TimeoutAcquisitionStart = 10000;
        private const int TimeoutAcquisitionStop = 10000;
        private const int ContinuousAcquisitionPeriod = 200;
        private object _acquisitionRequestSync = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private ILogger _logger = ClassLocator.Default.GetInstance<ILogger>();
        private bool _isInitialized;

        protected IProbeInputParams CurrentAcquisitionParams;
        protected IProbeConfig CurrentAcquisitionConfig;
        protected Action<ProbeLiseSignal> Notifier;

        public virtual int Average
        {
            get { return ProbeLiseDll.FPGetParamInt(ProbeLiseParams.FPID_I_AVERAGEPARAM); }
            set { ProbeLiseDll.FPSetParam(value, ProbeLiseParams.FPID_I_AVERAGEPARAM); }
        }

        public virtual bool AutoAirGap
        {
            get { return ProbeLiseDll.FPGetParamBool(ProbeLiseParams.FPID_B_AUTOAIRGAPCONFIG); }
            set { ProbeLiseDll.FPSetParam(value, ProbeLiseParams.FPID_B_AUTOAIRGAPCONFIG); }
        }

        public virtual bool AutoGain
        {
            get { return ProbeLiseDll.FPGetParamBool(ProbeLiseParams.FPID_B_AUTOGAIN); }
            set { ProbeLiseDll.FPSetParam(value, ProbeLiseParams.FPID_B_AUTOGAIN); }
        }

        public enum ProbeLiseProxyStatus
        {
            Free,
            Acquisition,
        }

        private enum PositionChannel
        {
            upChannel = 0,
            downChannel = 1
        }

        public IEnumerable<ProbeLiseSignal> DoSingleAcquisition(IProbeConfig configuration, IProbeInputParams inputParams)
        {
            lock (_acquisitionRequestSync)
            {
                switch (Status)
                {
                    case ProbeLiseProxyStatus.Free:
                        {
                            string message = $"[ProbeLise] Run a single acquisition.";
                            _logger?.Information(message);
                            return PerformSingleAcquisition(configuration, inputParams);
                        }
                    case ProbeLiseProxyStatus.Acquisition:
                        {
                            string message = $"[ProbeLise] A continuous acquisition is already started.";
                            _logger?.Information(message);
                            return PerformSingleAcquisitionWithInterruption(configuration, inputParams);
                        }
                    default:
                        {
                            string message = $"[ProbeLise] The state machine is in an illegal state.";
                            _logger?.Error(message);
                            throw new Exception(message);
                        }
                }
            }
        }

        public void StartContinuousAcquisition(IProbeConfig configuration, IProbeInputParams inputParams, Action<ProbeLiseSignal> notifier)
        {
            lock (_acquisitionRequestSync)
            {
                switch (Status)
                {
                    case ProbeLiseProxyStatus.Free:
                        {
                            string message = $"[ProbeLise] Start acquisition in a separate thread.";
                            _logger?.Information(message);

                            _cts.Dispose();
                            _cts = new CancellationTokenSource();

                            Task.Factory.StartNew(
                                () => ContinuousAcquisitionTask(configuration, inputParams, _cts.Token, notifier),
                                TaskCreationOptions.LongRunning
                            );

                            waitForAcquisitionStart();
                            return;
                        }
                    case ProbeLiseProxyStatus.Acquisition:
                        {
                            if (CurrentAcquisitionConfig.ModulePosition == configuration.ModulePosition && (inputParams as SingleLiseInputParams)?.Gain == (CurrentAcquisitionParams as SingleLiseInputParams)?.Gain)
                            {
                                string message = $"[ProbeLise] The acquisition is already in progress.";
                                _logger?.Information(message);
                                return;
                            }
                            else
                            {
                                StopContinuousAcquisition();

                                _cts.Dispose();
                                _cts = new CancellationTokenSource();

                                Task.Factory.StartNew(
                                    () => ContinuousAcquisitionTask(configuration, inputParams, _cts.Token, notifier),
                                    TaskCreationOptions.LongRunning
                                );

                                waitForAcquisitionStart();
                                return;
                            }
                        }
                    default:
                        {
                            string message = $"[ProbeLise] The state machine is in an illegal state.";
                            _logger?.Error(message);
                            throw new Exception(message);
                        }
                }
            }
        }

        public void StopContinuousAcquisition()
        {
            lock (_acquisitionRequestSync)
            {
                switch (Status)
                {
                    case ProbeLiseProxyStatus.Free:
                        {
                            string message = $"[ProbeLise] The acquisition is already stopped.";
                            _logger?.Information(message);
                            return;
                        }
                    case ProbeLiseProxyStatus.Acquisition:
                        {
                            string message = $"[ProbeLise] Try to stop the current acquisition.";
                            _logger?.Information(message);

                            _cts.Cancel();
                            waitForAcquisitionStop();
                            return;
                        }
                    default:
                        {
                            string message = $"[ProbeLise] The state machine is in an illegal state.";
                            _logger?.Error(message);
                            throw new Exception(message);
                        }
                }
            }
        }

        public void InitProbeLiseDll(string configTextFilePath, ProbeLiseConfig config)
        {
            ProbeLiseDll.FPDLLInit();
            ProbeLiseDll.FPInitialize(configTextFilePath, 0, StandAloneDllKey, 0, config);
            ProbeLiseDll.FPSetParam(0, ProbeLiseParams.FPID_B_STANDBY);
            _isInitialized = true;
        }

        public void Shutdown()
        {
            if (_isInitialized)
            {
                try
                {
                    StopContinuousAcquisition();
                }
                catch { } // Nothing to do

                try
                {
                    ProbeLiseDll.FPClose();
                }
                catch { } // Nothing to do

                try
                {
                    ProbeLiseDll.FPDLLClose();
                }
                catch { } // Nothing to do

                _isInitialized = false;
            }
        }

        public void SetAirGapThreshold(IProbeInputParams inputParams)
        {
            var inputParamsLiseSimple = (SingleLiseInputParams)inputParams;
            var array = new double[] { inputParamsLiseSimple.DetectionThreshold };
            ProbeLiseDll.FPSetParam(array, ProbeLiseParams.FPID_D_AIRGAP_THRESHOLD);
        }

        private void ConfigureProbeForAcquisition(IProbeConfig configuration, IProbeInputParams inputParams)
        {
            var parametersWithSample = DefineSample(inputParams, configuration);
            SetCurrentProbe(configuration);

            var name = parametersWithSample.ProbeSample.Name;
            var info = parametersWithSample.ProbeSample.Info;
            double gain = parametersWithSample.Gain;
            double qualityThreshold = parametersWithSample.QualityThreshold;

            ProbeLiseDll.WrapEntryParamsForDll(parametersWithSample, out double[] thicknesses, out double[] tolerances, out double[] indexes, out double[] types);
            ProbeLiseDll.FPDefineSample(name, info, thicknesses, tolerances, indexes, types, thicknesses.Length, gain, qualityThreshold);
        }

        private class ReferenceSignal
        {
            public Peak ReferencePeakGo;
            public Peak ReferencePeakCome;
            public int LagLength;
            public int GoLength;
            public int ComeLength;
        }

        private IEnumerable<ProbeLiseSignal> GetDllSignal(IProbeConfig configuration, IProbeInputParams inputParams)
        {
            // TODO find a way to only get the raw signal without dll processing (and therefore without having to pass all these parameters ...)
            string password = "SiTuTrouvesLePassTuEsFort";
            int maxNbRawValues = 60000;
            int nbRawValues = maxNbRawValues;
            int channel = 1;
            int maxNbPeaks = 28;
            int nbPeaks = maxNbPeaks;
            int nbDiscardedPeaks = maxNbPeaks;
            float stepX = 0;
            float saturationValue = 0;

            var probeLiseConfig = configuration as ProbeLiseConfig;
            var analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef, probeLiseConfig.PeakInfluence);
            var analyzer = new LiseSignalAnalyzer();

            double[] rawValues = new double[maxNbRawValues];
            double[] selectedPeaks = new double[nbPeaks];
            double[] discardedPeaks = new double[nbDiscardedPeaks];

            var allSignals = new List<ProbeLiseSignal>();
            ReferenceSignal referenceSignal = null;
            int NAverage = Average;
            int extraTries = NAverage / 3;
            int totalAverageTries = NAverage + extraTries;

            if (NAverage < 2)
            {
                ProbeLiseDll.FPGetRawSignal(password, rawValues, ref nbRawValues, ref stepX, channel, ref saturationValue, selectedPeaks, ref nbPeaks, discardedPeaks, ref nbDiscardedPeaks);
                var signal = buildRawSignalObject(configuration, rawValues, nbRawValues, stepX, saturationValue, selectedPeaks, nbPeaks, discardedPeaks, nbDiscardedPeaks);
                allSignals.Add(signal);
            }
            else
            {

                for (int n = 1; n <= totalAverageTries; ++n)
                {
                    if (allSignals.Count >= NAverage)
                    {
                        // we have reach our desired average exit loot
                        break;
                    }

                    rawValues.Fill(0.0, 0, nbRawValues);
                    selectedPeaks.Fill(0.0, 0, nbPeaks);
                    discardedPeaks.Fill(0.0, 0, nbDiscardedPeaks);

                    ProbeLiseDll.FPGetRawSignal(password, rawValues, ref nbRawValues, ref stepX, channel, ref saturationValue, selectedPeaks, ref nbPeaks, discardedPeaks, ref nbDiscardedPeaks);
                    var signal = buildRawSignalObject(configuration, rawValues, nbRawValues, stepX, saturationValue, selectedPeaks, nbPeaks, discardedPeaks, nbDiscardedPeaks);
                    var subParts = analyzer.AnalyzeSubPartsOfSignal(signal, analysisParams);

                    if (subParts.GoingsPart.Peaks.IsEmpty() || subParts.ComingsPart.Peaks.IsEmpty())
                    {
                        // signal is flat or noisiy, cannot found ref peaks, analysis cannot be performed trys another signal
                        if (allSignals.IsEmpty() && n > totalAverageTries)
                        {
                            // this is the last try - add one signal at least
                            allSignals.Add(signal);
                        }
                        continue;
                    }

                    if (referenceSignal is null)
                    {
                        referenceSignal = new ReferenceSignal
                        {
                            ReferencePeakGo = subParts.GoingsPart.Peaks[0],
                            ReferencePeakCome = subParts.ComingsPart.Peaks[0],
                            LagLength = subParts.LagAtStart,
                            GoLength = subParts.GoingsPart.Signal.Length,
                            ComeLength = subParts.ComingsPart.Signal.Length,
                        };

                        allSignals.Add(signal);
                    }
                    else
                    {
                        AlignRawSignalOnReferencePeak(subParts.GoingsPart.Signal, subParts.GoingsPart.Peaks[0], referenceSignal.ReferencePeakGo);
                        double[] goingExtended = AdaptSignalToLength(subParts.GoingsPart.Signal, referenceSignal.GoLength);
                        AlignRawSignalOnReferencePeak(subParts.ComingsPart.Signal, subParts.ComingsPart.Peaks[0], referenceSignal.ReferencePeakCome);
                        double[] comingExtended = AdaptSignalToLength(subParts.ComingsPart.Signal, referenceSignal.ComeLength);

                        double lagValue = rawValues[0];
                        int nbRawValuesAligned = referenceSignal.LagLength + goingExtended.Length + comingExtended.Length;

                        double[] alignedRawSignal = new double[maxNbRawValues];
                        alignedRawSignal.Fill(lagValue, 0, referenceSignal.LagLength);
                        Array.Copy(goingExtended, 0, alignedRawSignal, referenceSignal.LagLength, goingExtended.Length);
                        Array.Reverse(comingExtended);
                        Array.Copy(comingExtended, 0, alignedRawSignal, referenceSignal.LagLength + goingExtended.Length, comingExtended.Length);
                        alignedRawSignal.Fill(0.0, nbRawValuesAligned, maxNbRawValues - nbRawValuesAligned);

                        allSignals.Add(buildRawSignalObject(configuration, alignedRawSignal, nbRawValuesAligned, stepX, saturationValue, selectedPeaks, nbPeaks, discardedPeaks, nbDiscardedPeaks));
                    }
                }

            }


            return allSignals;
        }

        private double[] AdaptSignalToLength(double[] signal, int targetLength)
        {
            double lastValue = signal.LastOrDefault();
            double[] adaptedSignal = new double[targetLength];
            int copyLength = Math.Min(signal.Length, targetLength);

            Array.Copy(signal, adaptedSignal, copyLength);
            if (copyLength < targetLength)
            {
                adaptedSignal.Fill(lastValue, copyLength, targetLength - copyLength);
            }

            return adaptedSignal;
        }

        private void AlignRawSignalOnReferencePeak(double[] rawSignal, Peak currentReferencePeak , Peak referenceReferencePeak)
        {
            int xCurrentPeak = (int)currentReferencePeak.X;
            int xReferencePeak = (int)referenceReferencePeak.X;

            int nbPointToActOn = xReferencePeak - xCurrentPeak;
            if (nbPointToActOn < 0)
            {
                Array.Copy(rawSignal, xCurrentPeak, rawSignal, xReferencePeak, rawSignal.Length - xCurrentPeak);
            }
            else
            {
                Array.Copy(rawSignal, xCurrentPeak, rawSignal, xReferencePeak, rawSignal.Length - xReferencePeak);

                double valueToDuplicate = rawSignal[xCurrentPeak - 1];
                rawSignal.Fill(valueToDuplicate, xCurrentPeak, nbPointToActOn);
            }
        }

        private ProbeLiseSignal buildRawSignalObject(IProbeConfig configuration, double[] rawValues, int nbMeasures, float stepX, float saturationValue, double[] selectedPeaksIndexes, int nbSelectedPeaks, double[] discardedPeaksIndexes, int nbDiscardedPeaks)
        {
            ProbeLiseSignal probeRawSignal = new ProbeLiseSignal();

            probeRawSignal.ProbeID = configuration.DeviceID;

            for (int measureIndex = 0; measureIndex < nbMeasures; measureIndex++)
            {
                probeRawSignal.RawValues.Add(rawValues[measureIndex]);
            }

            probeRawSignal.StepX = stepX;

            probeRawSignal.SaturationValue = saturationValue;

            //// Skip Fogale Probe Return since it will be not used afterwards
            //// Keep Code below in order to debug with fogale probe dll return 
            //double stepXMicrometer = stepX / 1000.0
            //for (int i = 0; i < nbSelectedPeaks; i++)
            //{
            //    double selectedPeakIndex = selectedPeaksIndexes[i];
            //    if ((int)selectedPeakIndex < rawValues.Length)
            //        probeRawSignal.SelectedPeaks.Add(new ProbePoint(selectedPeakIndex * stepXMicrometer), rawValues[(int)selectedPeakIndex]));
            //}

            //// Skip Fogale Probe Return discard peaks since it will be not used afterwards
            //// Keep Code below in order to debug with fogale probe dll return 
            //for (int i = 0; i < nbDiscardedPeaks; i++)
            //{
            //    double discardedPeakIndex = discardedPeaksIndexes[i];
            //    if ((int)discardedPeakIndex < rawValues.Length)
            //    {
            //        probeRawSignal.DiscardedPeaks.Add(new ProbePoint(discardedPeakIndex, rawValues[(int)discardedPeakIndex]));
            //    }
            //}

            return probeRawSignal;
        }

        public virtual IEnumerable<ProbeLiseSignal> PerformSingleAcquisition(IProbeConfig configuration, IProbeInputParams inputParams)
        {
            ConfigureProbeForAcquisition(configuration, inputParams);
            ProbeLiseDll.FPStartSingleShotAcq();
            try
            {
                return GetDllSignal(configuration, inputParams).Select(signal => analyzeSignal(signal, configuration, inputParams));
            }
            finally
            {
                ProbeLiseDll.FPStopSingleShotAcq();
            }
        }

        public virtual void ContinuousAcquisitionTask(IProbeConfig configuration, IProbeInputParams inputParams, CancellationToken token, Action<ProbeLiseSignal> notifier)
        {
            _logger?.Information($"[ProbeLise] A continuous acquisition is started.");
            try
            {
                var parametersWithSample = DefineSample(inputParams, configuration);
                ConfigureProbeForAcquisition(configuration, parametersWithSample);
                CurrentAcquisitionParams = inputParams;
                CurrentAcquisitionConfig = configuration;
                Notifier = notifier;
                ProbeLiseDll.FPStartContinuousAcq();
                Status = ProbeLiseProxyStatus.Acquisition;

                while (!token.IsCancellationRequested)
                {
                    var dllSignal = GetDllSignal(configuration, inputParams).First();
                    var analyzedSignal = analyzeSignal(dllSignal, configuration, inputParams);
                    notifier(analyzedSignal);
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
                ProbeLiseDll.FPStopContinuousAcq();
                Status = ProbeLiseProxyStatus.Free;
                CurrentAcquisitionParams = null;
                CurrentAcquisitionConfig = null;
                _logger?.Information($"[ProbeLise] A continuous acquisition is stopped.");
            }
        }

        public virtual IEnumerable<ProbeLiseSignal> PerformSingleAcquisitionWithInterruption(IProbeConfig configuration, IProbeInputParams inputParams)
        {
            var currentAcquisitionParamsStored = CurrentAcquisitionParams;
            var currentAcquisitionConfigStored = CurrentAcquisitionConfig;

            StopContinuousAcquisition();

            var signal = PerformSingleAcquisition(configuration, inputParams);

            StartContinuousAcquisition(currentAcquisitionConfigStored, currentAcquisitionParamsStored, Notifier);

            return signal;
        }

        private void SetCurrentProbe(IProbeConfig configuration)
        {
            int position = (configuration.ModulePosition == ModulePositions.Up) ? (int)PositionChannel.upChannel : (int)PositionChannel.downChannel;
            ProbeLiseDll.FPSetParam(position, ProbeLiseParams.FPID_I_SETVISIBLEPROBE);
            ProbeLiseDll.FPSetParam(position, ProbeLiseParams.FPID_I_SETCURRENTPROBE_ONLY);
        }

        private void waitForAcquisitionStop()
        {
            if (!SpinWait.SpinUntil(() => Status != ProbeLiseProxyStatus.Acquisition, TimeoutAcquisitionStop))
                throw new TimeoutException();
        }

        private void waitForAcquisitionStart()
        {
            if (!SpinWait.SpinUntil(() => Status == ProbeLiseProxyStatus.Acquisition, TimeoutAcquisitionStart))
                throw new TimeoutException();
        }

        private static ProbeLiseSignal analyzeSignal(ProbeLiseSignal signal, IProbeConfig configuration, IProbeInputParams inputParams)
        {
            if (!(inputParams is ILiseInputParams) || !(configuration is ProbeLiseConfig))
            {
                return signal;
            }

            var probeLiseConfig = configuration as ProbeLiseConfig;
            var input = inputParams as ILiseInputParams;
            var sample = input.ProbeSample != null ? input.ProbeSample as ProbeSample : null;

            var analyzer = new LiseSignalAnalyzer();
            var signalAnalyzed = new LISESignalAnalyzed();

            if (sample != null && !sample.Layers.IsNullOrEmpty())
            {
                LiseSignalAnalysisAccordingSampleParams analysisParams = new LiseSignalAnalysisAccordingSampleParams(new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef, probeLiseConfig.PeakInfluence), probeLiseConfig.AcceptanceThreshold);
                signalAnalyzed = analyzer.AnalyzeRawSignalAccordingSample(signal, analysisParams, sample);
            }
            else
            {
                LiseSignalAnalysisParams analysisParams = new LiseSignalAnalysisParams(probeLiseConfig.Lag, probeLiseConfig.DetectionCoef, probeLiseConfig.PeakInfluence);
                signalAnalyzed = analyzer.AnalyzeRawSignal(signal, analysisParams);
            }

            signal.ReferencePeaks.Clear();
            signal.SelectedPeaks.Clear();
            signal.DiscardedPeaks.Clear();
            foreach (Peak p in signalAnalyzed.ReferencePeaks)
            {
                signal.ReferencePeaks.Add(new ProbePoint(p.X, p.Y));
            }
            foreach (Peak p in signalAnalyzed.SelectedPeaks)
            {
                signal.SelectedPeaks.Add(new ProbePoint(p.X, p.Y));
            }
            foreach (Peak p in signalAnalyzed.DiscardedPeaks)
            {
                signal.DiscardedPeaks.Add(new ProbePoint(p.X, p.Y));
            }

            signal.StdDev = signalAnalyzed.StdDev;
            signal.Means = signalAnalyzed.Means;

            return signal;
        }

        protected SingleLiseInputParams DefineSample(IProbeInputParams inputParameters, IProbeConfig configuration)
        {
            var simpleParams = inputParameters;
            if (simpleParams is DualLiseInputParams)
            {
                var dualParams = simpleParams as DualLiseInputParams;

                if (configuration.ModulePosition == ModulePositions.Up)
                {
                    simpleParams = dualParams.ProbeUpParams;
                }
                else
                {
                    simpleParams = dualParams.ProbeDownParams;
                }
            }

            // We have to provide something to the dll but the values ​​don't matter
            SingleLiseInputParams parametersWithSample = new SingleLiseInputParams(simpleParams as SingleLiseInputParams);
            Average = parametersWithSample.NbMeasuresAverage;

            if (parametersWithSample.ProbeSample == null)
            {
                var tolerance = new LengthTolerance(500, LengthToleranceUnit.Micrometer);
                var probeSampleLayerMeasured = new ProbeSampleLayer(750.Micrometers(), tolerance, 1.4621);
                var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
                var defaultSample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");
                parametersWithSample.ProbeSample = defaultSample;
            }

            if ((parametersWithSample.ProbeSample.Name == null) || (parametersWithSample.ProbeSample.Info == null))
                throw (new Exception($"[ProbeLise] Sample name and sample info must not be null to define Lise sample"));

            return parametersWithSample;
        }
    }
}
