using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public class ProbeLiseWrapper : Singleton<ProbeLiseWrapper>, IProbeLiseWrapper
    {
        public int ProbeID;
        private bool _initialized = false;
        private object _lockDll = new object();

        public int FPGetVersion()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetVersion();
            Monitor.Exit(_lockDll);
            return result;
        }

        public void FPDLLInit()
        {
            if (_initialized)
                return;

            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPDLLInit();
            Monitor.Exit(_lockDll);
            throwExceptionIfError
                (result);
        }

        public void FPDLLClose()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPDLLClose();
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPInitialize(String name, int param1, int param2, int param3, ProbeLiseConfig config)
        {
            if (_initialized)
                return;

            var dualHardwareConfig = toDualProbeHardwareConfig(config);
            var probeUpHardwareConfig = toProbeHardwareConfig(config);
            var probeDownHardwareConfig = toProbeHardwareConfig(config);

            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPInitialize(ref dualHardwareConfig, ref probeUpHardwareConfig, ref probeDownHardwareConfig, new StringBuilder(name), param1, param2, param3);
            Monitor.Exit(_lockDll);
            if (ProbeID < 0)
                throwExceptionIfError(result);
            ProbeID = result;
            _initialized = true;
        }

        public void FPLampState(out int itSate)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPLampState(ProbeID, out itSate);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPClose()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPClose(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
            ProbeID = -1;
        }

        public void FPDoSettings()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPDoSettings(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPOpenSettingsWindow()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPOpenSettingsWindow(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPUpdateSettingsWindow()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPUpdateSettingsWindow(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPCloseSettingsWindow()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPCloseSettingsWindow(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPStartSingleShotAcq()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPStartSingleShotAcq(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPStopSingleShotAcq()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPStopSingleShotAcq(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPStartContinuousAcq()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPStartContinuousAcq(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPStopContinuousAcq()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPStopContinuousAcq(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public ProbeLiseError FPGetThickness(double[] thicknessArray, double[] qualityValue, int iNumThickness)
        {
            if (thicknessArray.Length != qualityValue.Length)
                throw new ArgumentException("GetThickness called with inconsistent array lengths");

            Monitor.Enter(_lockDll);
            var result = (ProbeLiseError)ProbeLiseNativeMethods.FPGetThickness(ProbeID, thicknessArray, qualityValue, iNumThickness);
            Monitor.Exit(_lockDll);
            return result;
        }

        public void FPGetThicknesses(double[] dates, double[] thicknesses, double[] quality, int numValues)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetThicknesses(ProbeID, dates, thicknesses, quality, numValues);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public int FPGetParamInt(ProbeLiseParams paramID)
        {
            int val = 0;

            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetParam(ProbeID, ref val, paramID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);

            return val;
        }

        public bool FPGetParamBool(ProbeLiseParams paramID)
        {
            int val = FPGetParamInt(paramID);
            return val == 1;
        }

        public string FPGetParamString(ProbeLiseParams paramID)
        {
            StringBuilder sb = new StringBuilder();

            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetParam(ProbeID, sb, paramID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);

            return sb.ToString();
        }

        public void FPGetParam(byte[] param, ProbeLiseParams paramID)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetParam(ProbeID, param, paramID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public double FPGetParamDouble(ProbeLiseParams paramID)
        {
            double d = double.NaN;

            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetParam(ProbeID, ref d, paramID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);

            return d;
        }

        public void FPSetParam(int param, ProbeLiseParams paramID)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPSetParam(ProbeID, ref param, paramID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPSetParam(bool param, ProbeLiseParams paramID)
        {
            int val = param ? 1 : 0;
            FPSetParam(val, paramID);
        }

        public void FPSetParam(string param, ProbeLiseParams paramID)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPSetParam(ProbeID, new StringBuilder(param), paramID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPSetParam(double[] param, ProbeLiseParams paramID)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPSetParam(ProbeID, param, paramID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPGetSystemCaps(out string type, out string serialNumber, out double range, out int frequency, out double gainMin, out double gainMax, out double gainStep)
        {
            StringBuilder builderType = new StringBuilder(256);
            StringBuilder builderSN = new StringBuilder(256);

            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetSystemCaps(ProbeID, builderType, builderSN, out range, out frequency, out gainMin, out gainMax, out gainStep);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);

            type = builderType.ToString();
            serialNumber = builderSN.ToString();
        }

        public void FPGetRawSignal(string password, double[] signal, ref int nbSamples, ref float stepX, int voie, ref float saturationValue, double[] selectedPeaks, ref int nbSelectedPeaks, double[] discardedPeaks, ref int nbDiscardedPeaks)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetRawSignal(ProbeID, new StringBuilder(password), signal, ref nbSamples, ref stepX, voie, ref saturationValue, selectedPeaks, ref nbSelectedPeaks, discardedPeaks, ref nbDiscardedPeaks);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPSetStagePositionInfo(ref double xStagePosition, ref double yStagePosition, ref double zStagePosition)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPSetStagePositionInfo(ProbeID, ref xStagePosition, ref yStagePosition, ref zStagePosition);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPCalibrateDark()
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPCalibrateDark(ProbeID);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPCalibrateThickness(double value)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPCalibrateThickness(ProbeID, value);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPDefineSample(String name, String sampleInfo, double[] thicknessArray, double[] toleranceArray, double[] indexArray, double[] typeArray, int nbThickness, double gain, double qualityThreshold)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPDefineSample(ProbeID, new StringBuilder(name), new StringBuilder(sampleInfo), thicknessArray, toleranceArray, indexArray, typeArray, nbThickness, gain, qualityThreshold);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPDefineSampleDouble(String name, String sampleInfo, double[] thicknessArray, double[] toleranceArray, double[] indexArray, double[] typeArray, int nbThickness, double[] gain, double[] qualityThreshold)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPDefineSampleDouble(ProbeID, new StringBuilder(name), new StringBuilder(sampleInfo), thicknessArray, toleranceArray, indexArray, typeArray, nbThickness, gain, qualityThreshold);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPDefineSampleDoubleEx(String name, String sampleInfo, double[] thicknessArray, double[] toleranceArray, double[] indexArray, double[] typeArray, int nbThickness, double[] gain, double[] qualityThreshold)
        {
            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPDefineSampleDoubleEx(ProbeID, name, sampleInfo, thicknessArray, toleranceArray, indexArray, typeArray, nbThickness, gain, qualityThreshold);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);
        }

        public void FPGetSystemCapsDouble(out String type, out String serialNumber, double[] range, int[] frequency, double[] gainMin, double[] gainMax, double[] gainStep)
        {
            StringBuilder builderType = new StringBuilder(256);
            StringBuilder builderSN = new StringBuilder(256);

            Monitor.Enter(_lockDll);
            var result = ProbeLiseNativeMethods.FPGetSystemCapsDouble(ProbeID, builderType, builderSN, range, frequency, gainMin, gainMax, gainStep);
            Monitor.Exit(_lockDll);
            throwExceptionIfError(result);

            type = builderType.ToString();
            serialNumber = builderSN.ToString();
        }

        public void WrapEntryParamsForDll(in IProbeInputParams inputParams, out double[] thicknesses, out double[] tolerances, out double[] indexes, out double[] types)
        {
            // We have to provide something to the dll but the values ​​don't matter

            var probeSampleLayers = createSampleWithAirGaps(inputParams);

            int nbLayers = probeSampleLayers.Count;
            thicknesses = new double[nbLayers];
            tolerances = new double[nbLayers];
            indexes = new double[nbLayers];
            types = new double[nbLayers];

            for (int layer = 0; layer < probeSampleLayers.Count; layer++)
            {
                thicknesses[layer] = probeSampleLayers[layer].Thickness.Nanometers;
                tolerances[layer] = probeSampleLayers[layer].Tolerance.GetAbsoluteTolerance(probeSampleLayers[layer].Thickness).Nanometers;
                indexes[layer] = probeSampleLayers[layer].RefractionIndex;
                types[layer] = probeSampleLayers[layer].Type;
            }
        }

        private List<ProbeSampleLayer> createSampleWithAirGaps(IProbeInputParams inputParams)
        {
            // We have to provide something to the dll but the values ​​don't matter
            var probeSampleLayers = new List<ProbeSampleLayer>(((ILiseInputParams)inputParams).ProbeSample.Layers);
            var tolerance = new LengthTolerance(3000, LengthToleranceUnit.Nanometer);
            var probeSampleLayerAirGap = new ProbeSampleLayer(3000.Nanometers(), tolerance, 1.0);
            probeSampleLayers.Insert(0, probeSampleLayerAirGap);
            probeSampleLayers.Add(probeSampleLayerAirGap);

            return probeSampleLayers;
        }

        private void throwExceptionIfError(int error)
        {
            ProbeLiseError eError = (ProbeLiseError)error;
            if (eError != ProbeLiseError.FP_OK)
            {
                string descr = eError.GetDescription();
                throw new ApplicationException($"FogaleProbe error, ID={ProbeID}, error={descr}");
            }
        }

        private DualProbeHardwareConfig toDualProbeHardwareConfig(ProbeLiseConfig config)
        {
            var dualHardwareConfig = new DualProbeHardwareConfig();
            dualHardwareConfig.TypeDevice = config.DeviceType;
            return dualHardwareConfig;
        }

        private ProbeHardwareConfig toProbeHardwareConfig(ProbeLiseConfig probeLiseConfig)
        {
            var probeHardwareConfig = new ProbeHardwareConfig();
            probeHardwareConfig.TypeDevice = probeLiseConfig.DeviceType;
            probeHardwareConfig.SerialNumber = probeLiseConfig.SerialNumber;
            probeHardwareConfig.ProbeRange = probeLiseConfig.ProbeRange;
            probeHardwareConfig.MinimumGain = probeLiseConfig.MinimumGain;
            probeHardwareConfig.MaximumGain = probeLiseConfig.MaximumGain;
            probeHardwareConfig.GainStep = probeLiseConfig.GainStep;
            probeHardwareConfig.AutoGainStep = probeLiseConfig.AutoGainStep;
            probeHardwareConfig.Frequency = probeLiseConfig.Frequency;
            probeHardwareConfig.CalibWavelength = probeLiseConfig.CalibWavelength;
            probeHardwareConfig.ComparisonTol = (float)probeLiseConfig.ComparisonTol.Micrometers;

            return probeHardwareConfig;
        }
    }
}
