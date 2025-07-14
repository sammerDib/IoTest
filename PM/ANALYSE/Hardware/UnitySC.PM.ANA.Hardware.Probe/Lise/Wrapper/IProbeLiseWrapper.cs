using System;

using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public interface IProbeLiseWrapper
    {
        int FPGetVersion();

        void FPDLLInit();

        void FPDLLClose();

        void FPInitialize(String name, int param1, int param2, int param3, ProbeLiseConfig config);

        void FPLampState(out int itSate);

        void FPClose();

        void FPDoSettings();

        void FPOpenSettingsWindow();

        void FPUpdateSettingsWindow();

        void FPCloseSettingsWindow();

        void FPStartSingleShotAcq();

        void FPStopSingleShotAcq();

        void FPStartContinuousAcq();

        void FPStopContinuousAcq();

        ProbeLiseError FPGetThickness(double[] thicknessArray, double[] qualityValue, int iNumThickness);

        void FPGetThicknesses(double[] dates, double[] thicknesses, double[] quality, int numValues);

        int FPGetParamInt(ProbeLiseParams paramID);

        bool FPGetParamBool(ProbeLiseParams paramID);

        string FPGetParamString(ProbeLiseParams paramID);

        void FPGetParam(byte[] param, ProbeLiseParams paramID);

        double FPGetParamDouble(ProbeLiseParams paramID);

        void FPSetParam(int param, ProbeLiseParams paramID);

        void FPSetParam(bool param, ProbeLiseParams paramID);

        void FPSetParam(string param, ProbeLiseParams paramID);

        void FPSetParam(double[] param, ProbeLiseParams paramID);

        void FPGetSystemCaps(out string type, out string serialNumber, out double range, out int frequency, out double gainMin, out double gainMax, out double gainStep);

        void FPGetRawSignal(string password, double[] signal, ref int nbSamples, ref float stepX, int voie, ref float saturationValue, double[] selectedPeaks, ref int nbSelectedPeaks, double[] discardedPeaks, ref int nbDiscardedPeaks);

        void FPSetStagePositionInfo(ref double xStagePosition, ref double yStagePosition, ref double zStagePosition);

        void FPCalibrateDark();

        void FPCalibrateThickness(double value);

        void FPDefineSample(String name, String sampleInfo, double[] thicknessArray, double[] toleranceArray, double[] indexArray, double[] typeArray, int nbThickness, double gain, double qualityThreshold);

        void FPDefineSampleDouble(String name, String sampleInfo, double[] thicknessArray, double[] toleranceArray, double[] indexArray, double[] typeArray, int nbThickness, double[] gain, double[] qualityThreshold);

        void FPDefineSampleDoubleEx(String name, String sampleInfo, double[] thicknessArray, double[] toleranceArray, double[] indexArray, double[] typeArray, int nbThickness, double[] gain, double[] qualityThreshold);

        void FPGetSystemCapsDouble(out String type, out String serialNumber, double[] range, int[] frequency, double[] gainMin, double[] gainMax, double[] gainStep);

        void WrapEntryParamsForDll(in IProbeInputParams inputParams, out double[] thicknesses, out double[] tolerances, out double[] indexes, out double[] types);
    }
}
