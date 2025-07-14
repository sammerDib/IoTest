using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{

    [DataContract]
    public class LiseHFCalibrationData :ICalibrationData
    {
        public static string CurrentFileVersion = "1.0.0";

        [DataMember]
        [XmlAttribute]
        public string FileVersion { get; set; } = CurrentFileVersion;

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        [XmlArray("SpectroIntegrationTimes")]
        [XmlArrayItem("Objective")]
        public List<LiseHFObjectiveIntegrationTimeCalibration> IntegrationTimes { get; set; }

        [DataMember]
        [XmlArray("SpotPosition")]
        [XmlArrayItem("Objective")]
        public List<LiseHFObjectiveSpotCalibration> SpotPositions{ get; set; }

        public LiseHFCalibrationData()
        {
            IntegrationTimes = new List<LiseHFObjectiveIntegrationTimeCalibration>();

            SpotPositions = new List<LiseHFObjectiveSpotCalibration>();
        }

        public string Information => string.Format($"Last saved by {User} on {CreationDate}");
    }

    [DataContract]
    public class LiseHFObjectiveIntegrationTimeCalibration
    {
        public LiseHFObjectiveIntegrationTimeCalibration() { }

        public LiseHFObjectiveIntegrationTimeCalibration(string objectiveDeviceId, FlowStatus flowStatus)
        {
            ObjectiveDeviceId = objectiveDeviceId;
            Status = flowStatus;
        }

        [DataMember]
        [XmlIgnore]
        public FlowStatus Status { get; set; } = new FlowStatus();

        [DataMember]
        [XmlAttribute]
        public DateTime Date { get; set; }

        [DataMember]
        [XmlElement("Id")]
        public string ObjectiveDeviceId { get; set; }

        [DataMember]
        [XmlElement("StdIllum_ms")]
        public double StandardFilterIntegrationTime_ms { get; set; }

        [DataMember]
        [XmlElement("StdIllum_Count")]
        public double StandardFilterBaseCount{ get; set; }
        
        [DataMember]
        [XmlIgnore]
        [XmlArrayItem("y")]
        public List<double> StandardSignal { get; set; }

        [DataMember]
        [XmlElement("LowIllum_ms")]
        public double LowIllumFilterIntegrationTime_ms { get; set; }

        [DataMember]
        [XmlElement("LowIllum_Count")]
        public double LowIllumFilterBaseCount{ get; set; }
        
        [DataMember]
        [XmlIgnore]
        [XmlArrayItem("y")]
        public List<double> LowIllumSignal { get; set; }

        [DataMember]
        [XmlIgnore]
        [XmlArrayItem("x")]
        public List<double> WaveSignal { get; set; }
    }

    [DataContract]
    public class LiseHFObjectiveSpotCalibration : IProbeSpotCalibration
    {
        public LiseHFObjectiveSpotCalibration(){  }

        public LiseHFObjectiveSpotCalibration(string objectiveDeviceId, FlowStatus flowStatus)
        {
            ObjectiveDeviceId = objectiveDeviceId;
            Status = flowStatus;
        }

        [DataMember]
        [XmlIgnore]
        public FlowStatus Status { get; set; } = new FlowStatus();

        [DataMember]
        [XmlAttribute]
        public DateTime Date { get; set; }

        [DataMember]
        [XmlElement("Id")]
        public string ObjectiveDeviceId { get; set; }
        
        [DataMember]
        public Length XOffset { get; set; } = new Length(0.0, LengthUnit.Micrometer);

        [DataMember]
        public Length YOffset { get; set; } = new Length(0.0, LengthUnit.Micrometer);

        [DataMember]
        public Length PixelSizeX { get; set; } = new Length(0.0, LengthUnit.Micrometer);

        [DataMember]
        public Length PixelSizeY { get; set; } = new Length(0.0, LengthUnit.Micrometer);

        [DataMember]
        public double CamExposureTime_ms { get; set; } = 0.0;

        // advanced paramters
        [DataMember]
        [XmlIgnore]
        public double SpotShape { get; set; }

        [DataMember]
        [XmlIgnore]
        public Length SpotDiameter { get; set; }

        [DataMember]
        [XmlIgnore]
        public int SpotIntensity { get; set; }
    }
}
