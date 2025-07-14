using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.OpticalPowermeter
{
    public class StateChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string State;
    }

    public class PowerChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double Power;
        public double PowerCal_mW;
        public double RFactor;
    }

    public class MaxPowerChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double MaximumPower;
    }

    public class MinPowerChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double MinimumPower;
    }

    public class WavelengthChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint Wavelength;
    }

    public class BeamDiameterChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint BeamDiameter;
    }

    public class WavelengthRangeChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double WavelengthRange;
    }

    public class IdentifierChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string Identifier;
    }

    public class CustomChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string Custom;
    }

    public class AvailableWavelengthsChangedMessage
    {
        public List<string> Wavelengths;
    }

    public class CurrentChangedMessage
    {
        public PowerIlluminationFlow Flow;        
        public double Current_mA;
    }    

    public class DarkAdjustStateChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string DarkAdjustState;
    }

    public class DarkOffsetChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double DarkOffset_mW;
    }

    public class ResponsivityChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double Responsivity;
    }

    public class SensorTypeChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string SensorType;
    }

    public class SensorAttenuationChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint SensorAttenuation;
    }
}
