using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.LIGHTSPEED.Client.Proxy.RotatorsKitCalibration
{
    public class PowermeterPowerChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double Power;
        public double PowerCal_mW;
        public double RFactor;
    }

    public class PowermeterCurrentChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double Current_mA;
    }

    public class PowermeterIdentifierChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string Identifier;
    }

    public class PowermeterWavelengthChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint Wavelength;
    }

    public class PowermeterRangesChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string PowermeterRange;
    }

    public class PowermeterBeamDiameterChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint BeamDiameter;
    }

    public class PowermeterDarkAdjustStateChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string DarkAdjustState;
    }

    public class PowermeterDarkOffsetChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double DarkOffset;
    }

    public class PowermeterResponsivityChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double Responsivity;
    }

    public class PowermeterSensorTypeChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public string SensorType;
    }

    public class PowermeterSensorAttenuationChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public uint SensorAttenuation;
    }

    public class PowermeterRFactorsCalibChangedMessage
    {
        public PowerIlluminationFlow Flow;
        public double RFactorS;
        public double RFactorP;
    }

    public class PowerLaserChangedMessage
    {
        public double Power;
    }

    public class InterlockStatusChangedMessage
    {
        public string InterlockStatus;
    }

    public class LaserTemperatureChangedMessage
    {
        public double LaserTemperature;
    }

    public class PsuTemperatureChangedMessage
    {
        public double PsuTemperature;
    }

    public class AttenuationPositionChangedMessage
    {
        public double AttenuationPosition;
    }

    public class PolarisationPositionChangedMessage
    {
        public double PolarisationPosition;
    }

    public class PolarisationAngleCalibChangedMessage
    {
        public double PolarAngleHsS;
        public double PolarAngleHsP;
        public double PolarAngleHtS;
        public double PolarAngleHtP;
    }

    public class ShutterIrisPositionChangedMessage
    {
        public string ShutterIrisPosition;
    }

    public class MppcStateSignalsChangedMessage
    {
        public MppcCollector Collector;
        public MppcStateModule StateSignals;
    }

    public class MppcOutputVoltageChangedMessage
    {
        public MppcCollector Collector;
        public double OutputVoltage;
    }

    public class MppcOutputCurrentChangedMessage
    {
        public MppcCollector Collector;
        public double OutputCurrent;
    }

    public class MppcSensorTemperatureChangedMessage
    {
        public MppcCollector Collector;
        public double SensorTemperature;
    }

    public class DataAttributesChangedMessages
    {
        public List<DataAttribute> DataAttributes;
    }
}
