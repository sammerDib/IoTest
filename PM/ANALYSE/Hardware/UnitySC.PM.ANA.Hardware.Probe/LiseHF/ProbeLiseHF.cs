using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.ANA.Service.Interface.Probe.Configuration.ProbeLiseHF;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLiseHF;
using UnitySC.PM.ANA.Service.Interface.ProbeLiseHF;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCPMSharedAlgosLiseHFWrapper;
using System.Linq;

namespace UnitySC.PM.ANA.Hardware.Probe.LiseHF
{

    public class ProbeLiseHF : ProbeBase, IProbeWithStatus
    {
        private const int cst_waitShutterTimeOutms = 2500;      // shutter open
        private const int cst_waitShutterCloseTimeOutms = 800;  // shutter close

        private readonly double _normalizeScaleFactor;  // Scale Factor use for signale Normalisation

        public ProbeStatus Status { get; set; }

        private readonly ProbeLiseHFFDCProvider _probeLiseHFFDCProvider;

        private Task _handleTaskAcquisition;
        private CancellationTokenSource _cancellationTokenSrc;

        public ProbeLiseHFDevices ProbeDevices { get; set; }

        private string _lastShutterPosition = string.Empty;

        public static  Dictionary<LHFFilterType, int> SliderIDs = new Dictionary<LHFFilterType, int>(4);

        public static int IlluminationToSliderID(bool isLowIlluminationPower)
        { return isLowIlluminationPower ? SliderIDs[LHFFilterType.LowIllum] : SliderIDs[LHFFilterType.Standard]; }

        public double GetNormScaleFactor() { return _normalizeScaleFactor; }
        public ProbeLiseHF(IProbeConfig config, ProbeLiseHFDevices probeDevices, ILogger logger = null) : base(config,logger)
        {
            Status = ProbeStatus.Uninitialized;
            ProbeDevices = probeDevices;

            var lhfcfg = Configuration as ProbeLiseHFConfig;
            foreach (var filter in lhfcfg.Filters)
            {
                if (SliderIDs.ContainsKey(filter.Type))
                {
                    SliderIDs[filter.Type] = filter.SliderIndex;
                }
                else
                {
                    SliderIDs.Add(filter.Type, filter.SliderIndex);
                }
            }

            _normalizeScaleFactor = lhfcfg?.NormalScaleFactor ?? 1.0 ;

            _probeLiseHFFDCProvider = ClassLocator.Default.GetInstance<ProbeLiseHFFDCProvider>();
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ShutterIrisPositionMessage>(this, (r, m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });
        }

        ~ProbeLiseHF()
        {
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Unregister<ShutterIrisPositionMessage>(this);
        }

        private void UpdateShutterIrisPosition(string shutterIrisPosition)
        { _lastShutterPosition = shutterIrisPosition; }

        public override void Init()
        {
            if (ProbeDevices == null)
                throw new Exception("ProbeDevices are not set");
            if (ProbeDevices.Shutter == null)
                throw new Exception("HW Shutter is Null");
            if (ProbeDevices.Laser == null)
                throw new Exception("HW Laser is Null");
            if (ProbeDevices.OpticalRackAxes == null)
                throw new Exception("HW OpticalRack MotionAxes is Null");
            if (ProbeDevices.Spectrometer == null)
                throw new Exception("HW Spectrometer is Null");

            //check if all Filter type are set
            foreach (var filtertype in (LHFFilterType[])Enum.GetValues(typeof(LHFFilterType)))
            {
                if (!SliderIDs.ContainsKey(filtertype))
                    throw new Exception($"Missing LiseHF filter definition [{filtertype}] in HWConfig");
            }


            ProbeDevices.Shutter.CloseIris(); // no wait
            SetDBSlider(LHFFilterType.Standard);

            ProbeDevices.Shutter.TriggerUpdateEvent();
            //ProbeDevices.Laser.SetPower(500); ?
            ProbeDevices.Laser.PowerOn();

            Status = ProbeStatus.Initialized;
        }

        public override void Shutdown()
        {
            ProbeDevices.Spectrometer.StopContinuousAcquisition();
            ProbeDevices.Shutter.CloseIris(); // no wait
        }

        public RawCalibrationSignal DarkCalibration (string objectiveID, bool lowIlluminationPower, double integrationTime_ms, int calibrationNbAverage)
        {
            var darkCalibrationRawSignal = MakeCalibration(lowIlluminationPower,integrationTime_ms, calibrationNbAverage);
            _probeLiseHFFDCProvider.CreateFDCLiseHFDarkCalibration(objectiveID, lowIlluminationPower, darkCalibrationRawSignal);
            return darkCalibrationRawSignal;
        }

        public RawCalibrationSignal RefCalibration(string objectiveID, bool lowIlluminationPower, double integrationTime_ms, int calibrationNbAverage)
        {
            var refCalibrationRawSignal = MakeCalibration(lowIlluminationPower,integrationTime_ms, calibrationNbAverage);
            _probeLiseHFFDCProvider.CreateFDCLiseHFRefCalibration(objectiveID, lowIlluminationPower, refCalibrationRawSignal);
            return refCalibrationRawSignal;
        }

        // Methods to move slider to a specific position
        // Info : those methods are not yet spread across the code, but feel free to use them

        public void SetStandardSlider()
        {
            SetDBSlider(LHFFilterType.Standard);
        }

        public void SetLowIllumSlider()
        {
            SetDBSlider(LHFFilterType.LowIllum);
        }

        public void Set0dDBSlider()
        {
            SetDBSlider(LHFFilterType.Zero);
        }

        public void SetCustomDBSlider()
        {
            SetDBSlider(LHFFilterType.Custom);
        }

        // use to save slider context
        public int GetSliderIndex()
        {
            return ProbeDevices.OpticalRackAxes.GetCurrentIndex();
        }

        // Use to restore slider context
        public void SetSliderIndex(int index)
        {
            if (ProbeDevices.OpticalRackAxes.GetCurrentIndex() != index)
            {
                ProbeDevices.OpticalRackAxes.MoveToIndex(index);
                Thread.Sleep(500);
            }
        }

        private void SetDBSlider(LHFFilterType filtertype)
        {
            int indexSlider = SliderIDs[filtertype];
            SetSliderIndex(indexSlider);
        }

        private RawCalibrationSignal MakeCalibration(bool lowIlluminationPower, double integrationTime_ms, int nbAverage)
        {
            int indexSlider = IlluminationToSliderID(lowIlluminationPower);
            if (ProbeDevices.OpticalRackAxes.GetCurrentIndex() != indexSlider)
                ProbeDevices.OpticalRackAxes.MoveToIndex(indexSlider);

            if (!OpenShutterAndWait(cst_waitShutterTimeOutms))
                throw new Exception("Shutter is not Openned during rawcalibration, check safety key if your are in Maintenance mode");

            var spectrosig = ProbeDevices.Spectrometer.DoMeasure(new SpectrometerParamBase(integrationTime_ms, nbAverage));
            //Close shutter
            if (!CloseShutterAndWait(cst_waitShutterCloseTimeOutms))
                Logger.Warning($"ProbeLiseHF shutter not closed in time during rawcalibration");

            //Calibration RawValues are normalized via integrationTime_ms
            NormalizeWithValue(spectrosig.RawValues, _normalizeScaleFactor);
            return new RawCalibrationSignal() { WaveLength_nm = spectrosig.Wave, RawSignal = spectrosig.RawValues };
        }

        private void NormalizeWithValue(List<double> rawData, double divisorFactor)
        {
            if (rawData == null || divisorFactor == 0.0)
                return;

            double factorNorm = 1.0 / divisorFactor;
            for (int i = 0; i < rawData.Count; i++)
            {
                rawData[i] *= factorNorm;
            }
        }

        private bool WaitShutterOpen(int timeout_ms)
        {
            int waitDelay_ms = 250;
            int nIter = 0;
            int IterMax = (int)Math.Ceiling((double)(timeout_ms / waitDelay_ms) + 0.0001);
            while (_lastShutterPosition.ToLowerInvariant() != "open" && nIter < IterMax)
            {
                Thread.Sleep(waitDelay_ms);
                ProbeDevices.Shutter.TriggerUpdateEvent();
                ++nIter;
            }
            return (nIter < IterMax);
        }

        public bool OpenShutterAndWait(int timeout_ms)
        {
            try
            {
                ProbeDevices.Shutter.OpenIris();
                Thread.Sleep(550); // delay to avoid laser dazzle

                if (!WaitShutterOpen(timeout_ms))
                    return false;

            }
            catch
            {
                return false;
            }
            return true;

        }

        public bool CloseShutterAndWait(int timeout_ms)
        {
            try
            {
                ProbeDevices.Shutter.CloseIris(); // no-wait here, nominal behavior
                if (!WaitShutterClose(timeout_ms))
                    return false;
            }
            catch
            {
                return false;
            }
            return true;

        }

        private bool WaitShutterClose(int timeout_ms)
        {
            int waitDelay_ms = 250;
            int nIter = 0;
            int IterMax = (int)Math.Ceiling((double)(timeout_ms / waitDelay_ms) + 0.0001);
            while (_lastShutterPosition.ToLowerInvariant() != "close" && nIter < IterMax)
            {
                Thread.Sleep(waitDelay_ms);
                ProbeDevices.Shutter.TriggerUpdateEvent();
                ++nIter;
            }
            return (nIter < IterMax);
        }

        private SpectroSignal AcquireSpectrum(LiseHFInputParams inputsAcq, ProbeLiseHFCalibResult probeCalibration, bool normalizedSignal = false)
        {
            if (probeCalibration == null)
            {
                Logger.Error($"ProbeLiseHF calibration has not been set or was invalid");
                throw new Exception($"ProbeLiseHF calibration has not been set or was invalid");
            }

            if (!probeCalibration.Success)
            {
                Logger.Error($"ProbeLiseHF calibration is invalid");
                throw new Exception($"ProbeLiseHF calibration is invalid");
            }

            int indexSlider = IlluminationToSliderID(inputsAcq.IsLowIlluminationPower);
            if (ProbeDevices.OpticalRackAxes.GetCurrentIndex() != indexSlider)
                ProbeDevices.OpticalRackAxes.MoveToIndex(indexSlider);

            if (!OpenShutterAndWait(cst_waitShutterTimeOutms))
                throw new Exception("Shutter is not Openned, check safety key if your are in Maintenance mode");

            SpectroSignal spectrosig = null;
            try
            {
                double integrationTimeMeasure = probeCalibration.IntegrationTime_ms * inputsAcq.IntensityFactor;
                spectrosig = ProbeDevices.Spectrometer.DoMeasure(new SpectrometerParamBase(integrationTimeMeasure, inputsAcq.NbMeasuresAverage));
                Logger.Verbose($"DoMeasure Signal Acquired");

                double maxSatValue = 65534.0;
                if (spectrosig.RawValues.Max() > maxSatValue)
                    throw new Exception("Oversaturated Spectrum - Lower Intensity factor or Check ZFocus or LiseHF IT Calibration");

                if (normalizedSignal)
                {
                    NormalizeWithValue(spectrosig.RawValues, _normalizeScaleFactor * inputsAcq.IntensityFactor);
                    Logger.Verbose($"Spectro signal normalize with ITfactor {_normalizeScaleFactor * inputsAcq.IntensityFactor}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"ProbeLiseHF Spectro exception : {ex.Message}");
                Logger.Verbose($"ProbeLiseHF Spectro exception : {ex.StackTrace}");
                throw new Exception($"ProbeLiseHF Acquisition error - {ex.Message}");
            }
            finally
            {
                //Close shutter
                CloseShutterAndWait(cst_waitShutterCloseTimeOutms);
            }
            return spectrosig;
        }

        public override IProbeResult DoMeasure(IProbeInputParams inputParams)
        {
            if (Status != ProbeStatus.Initialized)
            {
                Logger.Error($"ProbeLiseHF measaure is not in initialized status");
                throw new Exception($"ProbeLiseHF measure is not in initialized status");
            }

            try
            {
                Status = ProbeStatus.Measuring;
                Logger.Verbose($"DoMeasure started");
                
                var inputsAcq = inputParams as LiseHFInputParams;
                if (inputsAcq == null)
                {
                    Logger.Error($"ProbeLiseHF input params are not coherent or empty");
                    throw new Exception($"ProbeLiseHF input params are not coherent or empty");
                }

                ProbeLiseHFCalibResult probeLiseHFCalibResult = (ProbeLiseHFCalibResult)CalibrationManager.GetCalibration(true, DeviceID, inputParams, null, null);
                if (probeLiseHFCalibResult == null)
                {
                    Logger.Error($"ProbeLiseHF Calib result are null or invalid - check LiseHF calibration or LiseHF Hardware status");
                    throw new Exception($"ProbeLiseHF Calib result are null or invalid - check LiseHF calibration or LiseHF Hardware status");
                }

                var spectrosig = AcquireSpectrum(inputsAcq, probeLiseHFCalibResult, normalizedSignal:true);
             
                ProbeLiseHFResult result = null;
                if (inputsAcq is LiseHFInputParamsForTSV)
                {
                    var analyzedSignal = ComputeLiseHFSignal(inputsAcq, spectrosig, probeLiseHFCalibResult);

                    var signal = analyzedSignal.Item1;
                    if (signal != null)
                    {
                        NotifyRawSignalUpdated(signal);
                    }
                    else
                        throw new Exception("LiseHF Signal is null"); // Should normally not happenned when ComputeLiseHFSignal return without exception

                    var outputmeasure = analyzedSignal.Item2;
                    if (outputmeasure == null)
                    {
                        // Could not analyse layers from FFTSignal and calib signals
                        result = new ProbeLiseHFResult()
                        {
                            Timestamp = DateTime.Now,
                            Quality = 0.0,
                            Message = signal.Message,
                        };
                        if (inputsAcq.SaveFFTSignal)
                        {
                            result.FFTSignal = SaveDepthFFTSignalMemory(signal);
                        }
                        result.LayersThickness = null;
                        return result;
                        //throw new Exception($"LiseHF Fail : {signal.Message}");
                    }

                    _probeLiseHFFDCProvider.CreateFDCLiseHFMeasure(outputmeasure);

                    double globalquality = outputmeasure.Quality;
                    result = new ProbeLiseHFResult()
                    {
                        Timestamp = DateTime.Now,
                        Quality = globalquality,
                        Message = signal.Message,
                    };

                    foreach (var depth in outputmeasure.MeasuredDepths)
                    {
                        // Les layers peuvent être negative !!! dans le cas de layer negative on les mets à 0.0 afin de ne pas les comptabiliser
                        var layerdepth = depth.Micrometers();
                        if (layerdepth.Micrometers < 0.0)
                            layerdepth = 0.Micrometers();
                        result.LayersThickness.Add(new ProbeThicknessMeasure(layerdepth, globalquality));
                    }

                    if (inputsAcq.SaveFFTSignal)
                    {
                        result.FFTSignal = SaveDepthFFTSignalMemory(outputmeasure);
                    }
                }

                Logger.Verbose($"DoMeasure Signal Computed");
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Status = ProbeStatus.Initialized;
            }
        }

        private byte[] SaveDepthFFTSignalMemory(LiseHFAlgoOutputs outputs)
        {
            int nCnt = Math.Min(outputs.FTTx.Count, outputs.FTTy.Count);

            float[] memfloat = new float[2 * nCnt];
            for (int i = 0; i < nCnt; i++)
            {
                memfloat[2 * i] = (float)outputs.FTTx[i];
                memfloat[2 * i + 1] = (float)outputs.FTTy[i];
            }

            byte[] memBytes = new byte[sizeof(float) * memfloat.Length + 2];
            memBytes[0] = 0; // version buffer for lter compatibilty
            memBytes[1] = (byte)((int)Math.Round(outputs.SaturationPercentage)); // saturation percentage
            Buffer.BlockCopy(memfloat, 0, memBytes, 2, memBytes.Length - 2);

            return memBytes;
        }

        private byte[] SaveDepthFFTSignalMemory(ProbeLiseHFSignal signalFFT)
        {
            int nCnt = signalFFT.RawValues.Count;

            float[] memfloat = new float[2 * nCnt];
            double scaledStepX = signalFFT.StepX / 1000.0;
            for (int i = 0; i < nCnt; i++)
            {
                memfloat[2 * i] = (float) ((double) i * scaledStepX);
                memfloat[2 * i + 1] = (float)signalFFT.RawValues[i];
            }

            byte[] memBytes = new byte[sizeof(float) * memfloat.Length + 2];
            memBytes[0] = 0; // version buffer for lter compatibilty
            memBytes[1] = (byte) (signalFFT.SaturationLevel % 256); // saturation percentage
            Buffer.BlockCopy(memfloat, 0, memBytes, 2, memBytes.Length - 2);

            return memBytes;
        }

        public override IProbeSignal DoSingleAcquisition(IProbeInputParams inputParams)
        {
            if (Status != ProbeStatus.Initialized)
            {
                Logger.Error($"ProbeLiseHF is not in initialized status");
                throw new Exception($"ProbeLiseHF is not in initialized status");
            }
            try
            {
                Status = ProbeStatus.Measuring;
                Logger.Verbose($"DoSingleAcquisition started");
 
                var inputsAcq = inputParams as LiseHFInputParams;
                if (inputsAcq == null)
                {
                    Logger.Error($"ProbeLiseHF input params are not coherent or empty");
                    return null;
                }

                ProbeLiseHFCalibResult probeLiseHFCalibResult = (ProbeLiseHFCalibResult)CalibrationManager.GetCalibration(false, DeviceID, inputParams, null, null);
                if (probeLiseHFCalibResult == null)
                {
                    Logger.Error($"ProbeLiseHF Calib result are null or invalid - check LiseHF calibration or LiseHF Hardware status");
                    return null;
                }

                var spectrosig = AcquireSpectrum(inputsAcq, probeLiseHFCalibResult, normalizedSignal:true);
                if (spectrosig == null)
                    return null;

                Logger.Verbose($"DoSingleAcquisition Signal Acquired");
                Status = ProbeStatus.Measuring;

                var signal = ComputeLiseHFSignal(inputsAcq, spectrosig, probeLiseHFCalibResult).Item1;

                Logger.Verbose($"DoSingleAcquisition Signal Computed");
                return signal;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Status = ProbeStatus.Initialized;
            }
        }

        public override IEnumerable<IProbeSignal> DoMultipleAcquisitions(IProbeInputParams inputParameters)
        {
            if (!(inputParameters is LiseHFInputParams inputsAcq))
            {
                Logger.Error($"ProbeLiseHF input params are not coherent or empty");
                yield break;
            }

            for (int i = 0; i < inputsAcq.NbMeasuresAverage; ++i)
            {
                yield return DoSingleAcquisition(inputParameters);
            }
        }

        public bool FindSpotPosition(ServiceImage image, double pixelSizeX_um, double pixelSizeY_um, out double spotPositionX_um, out double spotPositionY_um, 
                                     out double spotShape, out double spotDiameter_um, out double spotIntensity, out string errmessage)
        {
            spotPositionX_um = 0.0;
            spotPositionY_um = 0.0;
            spotShape = 1.0;
            spotDiameter_um = 0.0;
            spotIntensity = 0.0;

            LiseHFBeamPositionInputs algoBeamInputs = new LiseHFBeamPositionInputs(image.Data, image.DataWidth, image.DataHeight, pixelSizeX_um,  pixelSizeY_um);
            var ret = Olovia_Algos.ComputeBeamPosition(algoBeamInputs);
            errmessage = ret.ErrorMessage;

            if (ret.IsSuccess == true)
            {
                spotPositionX_um = ret.xSpotPosition_um;
                spotPositionY_um = ret.ySpotPosition_um;
                spotShape = ret.dRatioOfAxisOfEllipse;
                spotDiameter_um = 2.0 * ret.dRadius;
                spotIntensity = ret.dAmpl;
            }
            return ret.IsSuccess;
        }

        protected (ProbeLiseHFSignal, LiseHFAlgoOutputs) ComputeLiseHFSignal(LiseHFInputParams inputs, SpectroSignal spectroRawSignal, ProbeLiseHFCalibResult probeCalibration, bool FFTOnly = false)
        {

            double calibrateIntegretionTimems = (probeCalibration != null) ? probeCalibration.IntegrationTime_ms : 10.0;
            double spectorIntegrationTimems = calibrateIntegretionTimems * inputs.IntensityFactor;  // new verion to implement

            int indexSlider = IlluminationToSliderID(inputs.IsLowIlluminationPower);
            int nbAverage = inputs.NbMeasuresAverage;
            double thresholdPeak = inputs.ThresholdPeak;
            double tolmargin_um = 0.0; // tolerance search margin

            // Missing data in case of Measure and analysis of signal
            LiseHFLayers liseHFLayers = null;
            var depth = (inputs is LiseHFInputParamsForTSV) ? (inputs as LiseHFInputParamsForTSV).TSVDepth : inputs.DepthTarget;
            var depthtol = (inputs is LiseHFInputParamsForTSV) ? (inputs as LiseHFInputParamsForTSV).TSVDepthTolerance : inputs.DepthTolerance.GetAbsoluteTolerance(inputs.DepthTarget);
            var diameter = (inputs is LiseHFInputParamsForTSV) ? (inputs as LiseHFInputParamsForTSV).TSVDiameter : 2.0.Micrometers();

            ProbeLiseHFConfig probeLiseHFConfig = ((Configuration as ProbeLiseHFConfig) ?? new ProbeLiseHFConfig());
            double maxQualityScore = probeLiseHFConfig.QualityNonNormalizedMax;

            if (maxQualityScore <= 0.0)
                maxQualityScore = 0.00001; //avoid zero-division

            bool hasbreak = false;
            if ((inputs.PhysicalLayers != null) && (inputs.PhysicalLayers.Count > 0))
            {
                var depthPlusTolerance = depth + depthtol;
                var availableOpticalLayers = new List<LayerWithToleranceSettings>(inputs.PhysicalLayers.Count);
                var currentDepth = 0.Micrometers();
                //from up to down layer in order to adjust last layer of tsv
                foreach (var physicalLayer in inputs.PhysicalLayers)
                {
                    if (currentDepth + physicalLayer.Thickness > depthPlusTolerance)
                    {
                        availableOpticalLayers.Add(new LayerWithToleranceSettings()
                        {
                            Name = physicalLayer.Name,
                            Thickness = depth - currentDepth,
                            ThicknessTolerance = physicalLayer.ThicknessTolerance,
                            RefractiveIndex = physicalLayer.RefractiveIndex
                        });
                        hasbreak = true;
                        break;
                    }
                    currentDepth += physicalLayer.Thickness;
                    availableOpticalLayers.Add(physicalLayer);
                }

                if (!hasbreak)
                {
                    throw new Exception($"lise HF Depth to measure is greater than the last layer defined - please define correctly layers in Product / Step");
                }

                // we should iterate from bottom to Top layer for algorithm. so inverse it
                availableOpticalLayers.Reverse();
                double tolerance_um = depthtol.Micrometers + tolmargin_um;
                int i = 0; int lastidx = availableOpticalLayers.Count - 1;
                liseHFLayers = new LiseHFLayers();

                int UseVersion = 3;
                switch (UseVersion)
                {
                    case 1:
                        // la premiere layer (celle bottom) est dans l'air donc RI = 1.0
                        foreach (var layer in availableOpticalLayers)
                        {
                            liseHFLayers.AddNewDepthLayer(layer.Thickness.Micrometers, (i == 0 || i == lastidx) ? tolerance_um : 0.0, (i == 0) ? 1.0 : (layer.RefractiveIndex ?? 3.68));
                            ++i;
                        }
                        break;

                    default:
                    case 3:
                    case 2:
                        // la premiere layer (celle bottom) est dans l'air donc RI = 1.0
                        foreach (var layer in availableOpticalLayers)
                        {
                            liseHFLayers.AddNewDepthLayer(layer.Thickness.Micrometers, (i == 0) ? tolerance_um : layer.ThicknessTolerance.Micrometers, (i == 0) ? 1.0 : (layer.RefractiveIndex ?? 3.68));
                            ++i;
                        }
                        break;
                }
                liseHFLayers.ComputeNative();
            }
            else
            {
                throw new Exception($"No layers - please define correctly layers in Product / Step");
            }

            LiseHFAlgoInputs algoInputs = new LiseHFAlgoInputs();
            algoInputs.Threshold_signal_pct = inputs.Threshold;
            algoInputs.Threshold_peak_pct = thresholdPeak;
            algoInputs.OpMode = FFTOnly ? LiseHFMode.FFTOnly : LiseHFMode.GridSearch;
            algoInputs.Wavelength_nm = spectroRawSignal.Wave;
            algoInputs.Spectrum = new LiseHFRawSignal(spectroRawSignal.RawValues, spectorIntegrationTimems, indexSlider);

            algoInputs.DarkSpectrum = (probeCalibration != null) ? new LiseHFRawSignal(probeCalibration.DarkRawSignal.RawSignal, probeCalibration.IntegrationTime_ms, probeCalibration.AttenuationId) : null;
            algoInputs.RefSpectrum = (probeCalibration != null) ? new LiseHFRawSignal(probeCalibration.RefRawSignal.RawSignal, probeCalibration.IntegrationTime_ms, probeCalibration.AttenuationId) : null;
            algoInputs.TSVDiameter_um = diameter.Micrometers;
            algoInputs.DepthLayers = liseHFLayers;

            algoInputs.PeakDetectionOnRight = probeLiseHFConfig.ComputePeakDetectionOnRight; ;
            algoInputs.NewPeakDetection = probeLiseHFConfig.ComputeNewPeakDetection; ;

            var ret = Olovia_Algos.Compute(algoInputs);

            WriteReports(inputs, algoInputs, ret);

            string verb = ret.FFTDone ? "analyse" : "compute FFT";
            if (!ret.IsSuccess)
            {
                Logger.Error($"Fail to {verb} LiseHF Signal : {ret.ErrorMessage}");
                if (ret.FFTDone)
                {
                    return (new ProbeLiseHFSignal(ret.Outputs.FTTy, ret.Outputs.FTTx, (int)ret.Outputs.SaturationPercentage)
                    {
                        Threshold = ret.Outputs.ThresholdSignal,
                        ThresholdPeak = ret.Outputs.ThresholdPeak,
                        ProbeID = DeviceID,
                        Quality = ret.Outputs.Quality / maxQualityScore,
                        Message = ret.ErrorMessage
                    }, null);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ret.ErrorMessage))
                    Logger.Warning($"LiseHF Calcul Signal : {ret.ErrorMessage}");

                ProbeLiseHFSignal signal = null;
                if (ret.FFTDone)
                    signal = new ProbeLiseHFSignal(ret.Outputs.FTTy, ret.Outputs.FTTx, (int)ret.Outputs.SaturationPercentage)
                    {
                        Threshold = ret.Outputs.ThresholdSignal,
                        ThresholdPeak = ret.Outputs.ThresholdPeak,
                        ProbeID = DeviceID,
                        Quality = ret.Outputs.Quality / maxQualityScore,
                        Message = ret.ErrorMessage
                    };
                return (signal, ret.Outputs);
            }

            throw new Exception($"Fail to {verb} LiseHF Signal : {ret.ErrorMessage}");
        }

        private void NotifyRawSignalUpdated(ProbeLiseHFSignal newProbeRawSignal)
        {
            var probeServiceCallback = ClassLocator.Default.GetInstance<IProbeServiceCallbackProxy>();
            probeServiceCallback.ProbeRawMeasuresCallback(newProbeRawSignal);
        }

        private void WriteReportSignals(string filepath, LiseHFAlgoInputs algoInputs)
        {
            try
            {
                if (System.Threading.Thread.CurrentThread.CurrentCulture != System.Globalization.CultureInfo.InvariantCulture)
                    System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

                var csv = new StringBuilder(1024 * 256);
                string sep = CSVStringBuilder.GetCSVSeparator();
                csv.AppendLine($"# ProbeLiseHF signals (c) UNITY SC{sep}");
                csv.AppendLine($"# {sep}");
                csv.AppendLine($"# {sep}Signal Integ. Time(ms){sep}{algoInputs.Spectrum.IntegrationTime_ms}{sep}Dark Integ. Time(ms){sep}{algoInputs.DarkSpectrum.IntegrationTime_ms}{sep}Ref Integ. Time(ms){sep}{algoInputs.RefSpectrum.IntegrationTime_ms}{sep}");
                csv.AppendLine($"# {sep}Attenuation ID{sep}{algoInputs.Spectrum.Attenuation_ID}{sep}");
                csv.AppendLine($"# {sep}Th1{sep}{algoInputs.Threshold_signal_pct:F4}{sep}Th2{sep}{algoInputs.Threshold_peak_pct:F4}{sep}");

                if (algoInputs.TSVDiameter_um != 0.0)
                {
                    csv.AppendLine($"# {sep}TSV Diameter (um){sep}{algoInputs.TSVDiameter_um}{sep}");
                }
                if (algoInputs.DepthLayers != null)
                {
                    csv.AppendLine($"# {sep}Layer No{sep}Depth(um){sep}SearchTolerance(um){sep}RI{sep}");
                    int maxk = algoInputs.DepthLayers.Depths_um.Count;
                    for (int k = 0; k < maxk; k++)
                    {
                        csv.AppendLine($"# {sep}{k + 1}{sep}{algoInputs.DepthLayers.Depths_um[k]:F4}{sep}{algoInputs.DepthLayers.DepthsToleranceSearch_um[k]:F4}{sep}{algoInputs.DepthLayers.DepthsRefractiveIndex[k]:F6}{sep}");
                    }
                }
                csv.AppendLine($"#   {sep}");
                csv.AppendLine($"Wave(nm){sep}Signal{sep}Dark Calib{sep}Ref Calib{sep}");
                int maxwv = algoInputs.Wavelength_nm.Count;
                for (int i = 0; i < maxwv; i++)
                {
                    csv.AppendLine($"{algoInputs.Wavelength_nm[i]}{sep}{algoInputs.Spectrum.RawSignal[i]}{sep}{algoInputs.DarkSpectrum.RawSignal[i]}{sep}{algoInputs.RefSpectrum.RawSignal[i]}{sep}");
                }
                File.WriteAllText(filepath, csv.ToString());
            }
            catch (Exception ex)
            {
                Logger?.Error($"Fail to Report LiseHF({DeviceID}) Signals : {ex.Message})");
                Logger?.Verbose($"Fail to Report LiseHF({DeviceID}) Signals to {filepath}\nStackTrace : {ex.StackTrace})");
            }
        }

        private void WriteReportOutputs(string filepath, LiseHFAlgoReturns algoret)
        {
            try
            {
                if (System.Threading.Thread.CurrentThread.CurrentCulture != System.Globalization.CultureInfo.InvariantCulture)
                    System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

                var success = algoret.IsSuccess ? "Success" : "FAIL";
                var fft = algoret.FFTDone ? "FFT Done" : "FFT FAIL";
                var analysis = "Analyze" + (algoret.AnalysisDone ? " OK" : " NOK");

                var csv = new StringBuilder(1024 * 256);
                string sep = CSVStringBuilder.GetCSVSeparator();
                csv.AppendLine($"# ProbeLiseHF outputs (c) UNITY SC{sep}");
                csv.AppendLine($"# {sep}");
                csv.AppendLine($"# {sep}{success}{sep}{fft}{sep}{fft}{analysis}");
                csv.AppendLine($"# {sep}{algoret.ErrorMessage}{sep}");
                csv.AppendLine($"# {sep}");

                if (algoret.Outputs != null)
                {
                    var o = algoret.Outputs;
                    csv.AppendLine($"# {sep}Sat %{sep}{o.SaturationPercentage:F1}{sep}Th S{sep}{o.ThresholdSignal}{sep}Th P{sep}{o.ThresholdPeak}");
                    csv.AppendLine($"# {sep}");

                    int fftmax = (o.FTTy != null) ? o.FTTy.Count : 0;
                    int pmax = (o.PeaksY != null) ? o.PeaksY.Count : 0;
                    int dmax = (o.MeasuredDepths != null) ? o.MeasuredDepths.Count : 0; ;
                    int gmax = Math.Max(fftmax, Math.Max(pmax, dmax));

                    if (gmax > 0)
                    {
                        csv.AppendLine($"X{sep}Y{sep} - {sep}Depths (um){sep} - {sep}Px{sep}Py{sep}-Quality ={sep}{o.Quality}");
                        for (int i = 0; i < gmax; i++)
                        {
                            if (i < fftmax)
                                csv.Append($"{o.FTTx[i]:F4}{sep}{o.FTTy[i]:F4}{sep}{sep}");
                            else
                                csv.Append($"{sep}{sep}{sep}");

                            if (i < dmax)
                                csv.Append($"{o.MeasuredDepths[i]:F6}{sep}{sep}");
                            else
                                csv.Append($"{sep}{sep}");

                            if (i < pmax)
                                csv.Append($"{o.PeaksX[i]:F4}{sep}{o.PeaksY[i]:F4}{sep}");
                            else
                                csv.Append($"{sep}{sep}");

                            csv.AppendLine();
                        }
                    }
                }
                File.WriteAllText(filepath, csv.ToString());
            }
            catch (Exception ex)
            {
                Logger?.Error($"Fail to Report LiseHF({DeviceID}) Signals : {ex.Message})");
                Logger?.Verbose($"Fail to Report LiseHF({DeviceID}) Signals to {filepath}\nStackTrace : {ex.StackTrace})");
            }
        }

        private void WriteReports(LiseHFInputParams inputsParam, LiseHFAlgoInputs algoInputs, LiseHFAlgoReturns algoret)
        {
            try
            {
                string TempPath = @"C:\temp\LastProbeLiseHF\";
                if (!Directory.Exists(TempPath))
                {
                    Directory.CreateDirectory(TempPath);
                }

                //INPUTS
                if (inputsParam.ReportSignals)
                {
                    var inReportPath = Path.Combine(inputsParam.ReportPath, $"LiseHFSignals.csv");
                    WriteReportSignals(inReportPath, algoInputs);

                    File.Copy(inReportPath, Path.Combine(TempPath, $"LiseHFSignals.csv"), true);
                }
                else
                    WriteReportSignals(Path.Combine(TempPath, $"LiseHFSignals.csv"), algoInputs);

                // OUTPUTS
                if (inputsParam.ReportOutputs)
                {
                    var outReportPath = Path.Combine(inputsParam.ReportPath, $"LiseHFOutputs.csv");
                    WriteReportOutputs(outReportPath, algoret);

                    File.Copy(outReportPath, Path.Combine(TempPath, $"LiseHFOutputs.csv"), true);
                }
                else
                    WriteReportOutputs(Path.Combine(TempPath, $"LiseHFOutputs.csv"), algoret);
            }
            catch (Exception ex)
            {
                // low error so dont bother to make too visible
                Logger.Verbose($"#Warning# Fail to Report LiseHF files : <{ex.Message}>");
            }
        }

        public override void StartContinuousAcquisition(IProbeInputParams inputParams)
        {
            switch (Status)
            {
                case ProbeStatus.Uninitialized:
                    {
                        string message = $"[ProbeLiseHF] Tries to perform an acquisition on an uninitialized probe LISEHF.";
                        Logger?.Error(message);
                        throw new Exception(message);
                    }
                case ProbeStatus.Initialized:
                    {
                        DoStartContinuousAcquisition(inputParams);
                        return;
                    }
                case ProbeStatus.Acquisition:
                    {
                        StopContinuousAcquisition();
                        DoStartContinuousAcquisition(inputParams);
                        string message = $"[ProbeLiseHF] The acquisition was already in progress. Restarted with new parameters ";
                        Logger?.Information(message);
                        return;
                    }
                default:
                    {
                        string message = $"[ProbeLiseHF] The state machine is in an illegal state.";
                        Logger?.Error(message);
                        throw new Exception(message);
                    }
            }
        }

        public override void StopContinuousAcquisition()
        {
            switch (Status)
            {
                case ProbeStatus.Uninitialized:
                    {
                        string message = $"[ProbeLiseHF] Tries stop an acquisition on an uninitialized probe LISE.";
                        Logger?.Error(message);
                        throw new Exception(message);
                    }
                case ProbeStatus.Calibration:
                case ProbeStatus.Initialized:
                    {
                        string message = $"[ProbeLiseHF] The acquisition is already stopped.";
                        Logger?.Information(message);
                        return;
                    }
                case ProbeStatus.Measuring:
                case ProbeStatus.Acquisition:
                    {
                        string message = $"[ProbeLiseHF] Try to stop the current acquisition.";
                        Logger?.Information(message);
                        if (_cancellationTokenSrc != null)
                            _cancellationTokenSrc.Cancel();
                        waitForAcquisitionStop();
                        return;
                    }
                default:
                    {
                        string message = $"[ProbeLiseHF] The state machine is in an illegal state.";
                        Logger?.Error(message);
                        throw new Exception(message);
                    }
            }
        }

        private void PollContinousMeasure(IProbeInputParams inputParams)
        {
            Status = ProbeStatus.Acquisition;
            var token = _cancellationTokenSrc.Token;

            try
            {
                var inputsAcq = inputParams as LiseHFInputParams;
                if (inputsAcq == null)
                    throw new Exception($"ProbeLiseHF input params are not coherent or empty");


                double integrationTimeFromCalibration_ms = 0.0;
                ProbeLiseHFCalibResult probeLiseHFCalibResult = (ProbeLiseHFCalibResult)CalibrationManager.GetCalibration(false, DeviceID, inputParams, null, null);
                if (probeLiseHFCalibResult != null)
                    integrationTimeFromCalibration_ms = probeLiseHFCalibResult.IntegrationTime_ms;
                else
                    integrationTimeFromCalibration_ms = inputsAcq.IntegrationTimems; ///!\ specific continuous polling for liseHF


                double spectroIntegrationTime_ms = inputsAcq.IntensityFactor * integrationTimeFromCalibration_ms;
                ProbeDevices.Spectrometer.StartContinuousAcquisition(new SpectrometerParamBase(spectroIntegrationTime_ms, inputsAcq.NbMeasuresAverage));


                int indexSlider = IlluminationToSliderID(inputsAcq.IsLowIlluminationPower);
                if (ProbeDevices.OpticalRackAxes.GetCurrentIndex() != indexSlider)
                    ProbeDevices.OpticalRackAxes.MoveToIndex(indexSlider);

                if (!OpenShutterAndWait(cst_waitShutterTimeOutms))
                    throw new Exception("Shutter is not Openned, check safety key if your are in Maintenance mode");

                bool flagCalibrationWarning_Raised = false;
                bool flagInLoopError_Raised = false;
                while (!token.IsCancellationRequested)
                {

                    try
                    {
                        if (probeLiseHFCalibResult != null)
                        {
                            if (flagCalibrationWarning_Raised)
                            {
                                // reset flag
                                flagCalibrationWarning_Raised = false;
                            }

                            // get spectrometer signal
                            var spectrosig = ProbeDevices.Spectrometer.GetLastMeasure();
                            if (spectrosig == null)
                                throw new Exception($"PollContinousMeasure InLoop : LiseHF measure spectrum acquisition error");

                            if (flagInLoopError_Raised)
                            {
                                // reset Error exception flag
                                flagInLoopError_Raised = false;
                            }

                            if (token.IsCancellationRequested)
                                break;

                            var computedSignal = ComputeLiseHFSignal(inputsAcq, spectrosig, probeLiseHFCalibResult, false);

                            //Signals
                            var rawSignal = computedSignal.Item1; // ftt signal
                            LastRawSignal = rawSignal;
                            NotifyRawSignalUpdated(rawSignal);

                            //outputs
                            //var outputs = computedSignal.Item2;
                        }
                        else if (!flagCalibrationWarning_Raised)
                        {
                            // calibration is nulll or has not been successfully performed
                            flagCalibrationWarning_Raised = true;
                            Logger?.Warning($"PollContinousMeasure InLoop : Calibration is required to compute Signals");
                        }

                    }
                    catch (Exception ex)
                    {
                        if (!flagInLoopError_Raised)
                        {
                            flagInLoopError_Raised = true;
                            Logger?.Error($"PollContinousMeasure InLoop : {ex}");
                        }
                    }

                    var cancellationTriggered = token.WaitHandle.WaitOne(500);
                    if (cancellationTriggered)
                    {
                        ProbeDevices.Spectrometer.StopContinuousAcquisition();
                        // Cleanup
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error($"PollContinousMeasure : {ex}");
            }
            finally
            {
                _cancellationTokenSrc.Cancel();

                ProbeDevices.Spectrometer.StopContinuousAcquisition();
                CloseShutterAndWait(cst_waitShutterCloseTimeOutms);
                Status = ProbeStatus.Initialized;
            }
        }

        private void DoStartContinuousAcquisition(IProbeInputParams inputParams)
        {
            string message = $"[ProbeLiseHF] Starts acquisition in a separate thread.";
            Logger?.Information(message);
            _cancellationTokenSrc = new CancellationTokenSource();
            _handleTaskAcquisition = new Task(() => { PollContinousMeasure(inputParams); }, TaskCreationOptions.LongRunning);
            _handleTaskAcquisition.Start();
            waitForAcquisitionStart();
        }

        private void waitForAcquisitionStop()
        {
            if (!SpinWait.SpinUntil(() => Status != ProbeStatus.Acquisition, 2000))
                throw new TimeoutException();
        }

        private void waitForAcquisitionStart()
        {
            if (!SpinWait.SpinUntil(() => Status == ProbeStatus.Acquisition, 2000))
                throw new TimeoutException();
        }
    }

    [Serializable]
    public class ProbeLiseHFLayerTolerances
    {
        [XmlIgnore]
        public const double Default_um = 0.7;

        [XmlElement]
        public int UseVersion { get; set; } = 2;

        [XmlArray("LayersTolerances")]
        [XmlArrayItem("Tolerance")]
        public List<Length> Tolerances { get; set; } = new List<Length>();

        public void FillDefaultLayers(int layersNb)
        {
            if (layersNb > Tolerances.Count)
            {
                for (int i = (Tolerances.Count - 1); i < layersNb; i++)
                    Tolerances.Add(Default_um.Micrometers());
            }
        }
    }
}
