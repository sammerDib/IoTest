using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
    public struct ProbeHardwareConfig
    {
        //[MarshalAs(UnmanagedType.LPWStr)]
   


        public int LiseType;   //< pour connaitre le type de la sonde Lise
                               //dll init
        public string TypeDevice;         // Pour stocker le nom de l'appareil
        public string SerialNumber;       // pour stocker le numéro de série

        public float ProbeRange;
        public float MinimumGain;
        public float MaximumGain;
        public float GainStep;
        public float AutoGainStep;
        public int Frequency;

        //Calibration
        public double CalibWavelength;

        //Signal processing
        public float ComparisonTol;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
    public struct DualProbeHardwareConfig
    {
        //[MarshalAs(UnmanagedType.LPWStr)]
        public string TypeDevice;  //< Type du device

        public IntPtr ConfProbeUp;
    }


    internal static class ProbeLiseNativeMethods
    {
        public const string DllName = "FD_ProbeMini.dll";

        [DllImport(DllName)]
        public static extern int FPGetVersion();

        [DllImport(DllName)]
        public static extern int FPDLLInit();
        [DllImport(DllName)]
        public static extern int FPDLLClose();
        [DllImport(DllName)] //, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int FPInitialize(ref DualProbeHardwareConfig dualProbeHardwareConfig, ref ProbeHardwareConfig probeHardwareConfigTop , ref ProbeHardwareConfig probeHardwareConfigBottom, StringBuilder Name, int Param1, int Param2, int Param3);
        [DllImport(DllName)]
        public static extern int FPLampState(int ProbeID, out int itSate);
        [DllImport(DllName)]
        public static extern int FPClose(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPDoSettings(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPOpenSettingsWindow(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPUpdateSettingsWindow(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPCloseSettingsWindow(int ProbeID);

        [DllImport(DllName)]
        public static extern int FPStartSingleShotAcq(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPStopSingleShotAcq(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPStartContinuousAcq(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPStopContinuousAcq(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPGetThickness(int ProbeID, double[] ThicknessArray, double[] QualityValue, int _iNumThickness);
        [DllImport(DllName)]
        public static extern int FPGetThicknesses(int ProbeID, double[] Dates, double[] Thicknesses, double[] Quality, int NumValues);

        [DllImport(DllName)]
        public static extern int FPGetParam(int ProbeID, ref int Param, ProbeLiseParams ParamID);
        [DllImport(DllName)]
        public static extern int FPGetParam(int ProbeID, StringBuilder Param, ProbeLiseParams ParamID);
        [DllImport(DllName)]
        public static extern int FPGetParam(int ProbeID, [MarshalAsAttribute(UnmanagedType.LPArray, SizeParamIndex = 1), Out()] byte[] Param, ProbeLiseParams ParamID);
        [DllImport(DllName)]
        public static extern int FPGetParam(int ProbeID, ref double Param, ProbeLiseParams ParamID);

        [DllImport(DllName)]
        public static extern int FPSetParam(int ProbeID, ref int Param, ProbeLiseParams ParamID);
        [DllImport(DllName)]
        public static extern int FPSetParam(int ProbeID, StringBuilder Param, ProbeLiseParams ParamID);
        [DllImport(DllName)]
        public static extern int FPSetParam(int ProbeID, double[] Param, ProbeLiseParams ParamID);
        [DllImport(DllName)]
        public static extern int FPGetSystemCaps(int ProbeID, StringBuilder Type, StringBuilder SerialNumber, out double Range, out int Frequency, out double GainMin, out double GainMax, out double GainStep);
        [DllImport(DllName)]
        public static extern int FPGetRawSignal(
            int ProbeID,
            StringBuilder Password,
            [MarshalAsAttribute(UnmanagedType.LPArray, SizeParamIndex = 1), Out()] double[] I,
            ref int NbSamples,
            ref float StepX,
            int Voie,
            ref float SaturationValue,
            [MarshalAsAttribute(UnmanagedType.LPArray, SizeParamIndex = 1), Out()] double[] SelectedPeaks,
            ref int nbSelectedPeaks,
            [MarshalAsAttribute(UnmanagedType.LPArray, SizeParamIndex = 1), Out()] double[] DiscardedPeaks,
            ref int nbDiscardedPeaks);
        [DllImport(DllName)]
        public static extern int FPSetStagePositionInfo(int ProbeID, ref double XStagePosition, ref double YStagePosition, ref double ZStagePosition);
        [DllImport(DllName)]
        public static extern int FPCalibrateDark(int ProbeID);
        [DllImport(DllName)]
        public static extern int FPCalibrateThickness(int ProbeID, double Value);

        [DllImport(DllName)]
        public static extern int FPDefineSample(int ProbeID, StringBuilder Name, StringBuilder SampleInfo, double[] ThicknessArray, double[] ToleranceArray, double[] IndexArray, double[] TypeArray, int NbThickness, double Gain, double QualityThreshold);

        [DllImport(DllName)]
        public static extern int FPDefineSampleDouble(int ProbeID, StringBuilder Name, StringBuilder SampleInfo, double[] ThicknessArray, double[] ToleranceArray, double[] IndexArray, double[] TypeArray, int NbThickness, double[] Gain, double[] QualityThreshold);
        [DllImport(DllName)]
        public static extern int FPDefineSampleDoubleEx(int ProbeID, string Name, string SampleInfo, double[] ThicknessArray, double[] ToleranceArray, double[] IndexArray, double[] TypeArray, int NbThickness, double[] Gain, double[] QualityThreshold);
        [DllImport(DllName)]
        public static extern int FPGetSystemCapsDouble(int ProbeID, StringBuilder Type, StringBuilder SerialNumber, double[] Range, int[] Frequency, double[] GainMin, double[] GainMax, double[] GainStep);
    }
}
