using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.OpticalPowermeter
{
    public class StateMessage
    {
        public PowerIlluminationFlow Flow;
        public string State;
    }

    public class PowerMessage
    {
        public PowerIlluminationFlow Flow;
        public double Power;
        public double PowerCal_mW;
        public double RFactor;
    }

    public class MaxPowerMessage
    {
        public PowerIlluminationFlow Flow;
        public double MaximumPower;
    }

    public class MinPowerMessage
    {
        public PowerIlluminationFlow Flow;
        public double MinimumPower;
    }

    public class WavelengthMessage
    {
        public PowerIlluminationFlow Flow;
        public uint Wavelength;
    }

    public class RangesMessage
    {
        public PowerIlluminationFlow Flow;
        public string PowermeterRange;
    }

    public class BeamDiameterMessage
    {
        public PowerIlluminationFlow Flow;
        public uint BeamDiameter;
    }

    public class WavelengthRangeMessage
    {
        public PowerIlluminationFlow Flow;
        public double WavelengthRange;
    }

    public class IdentifierMessage
    {
        public PowerIlluminationFlow Flow;
        public string Identifier;
    }

    public class CustomMessage
    {
        public PowerIlluminationFlow Flow;
        public string Custom;
    }

    public class CurrentMessage
    {
        public PowerIlluminationFlow Flow;
        public double Current_mA;
    }

    public class AvailableWavelengthsMessage
    {
        public List<string> Wavelengths;
    }

    public class DarkAdjustStateMessage
    {
        public PowerIlluminationFlow Flow;
        public string DarkAdjustState;
    }

    public class DarkOffsetMessage
    {
        public PowerIlluminationFlow Flow;
        public double DarkOffset;
    }

    public class ResponsivityMessage
    {
        public PowerIlluminationFlow Flow;
        public double Responsivity;
    }

    public class SensorTypeMessage
    {
        public PowerIlluminationFlow Flow;
        public string SensorType;
    }

    public class SensorNameMessage
    {
        public PowerIlluminationFlow Flow;
        public string SensorName;
    }

    public class SensorAttenuationMessage
    {
        public PowerIlluminationFlow Flow;
        public uint SensorAttenuation;
    }

    public class RFactorsCalibMessage
    {
        public PowerIlluminationFlow Flow;
        public double RFactorS;
        public double RFactorP;
    }
}
