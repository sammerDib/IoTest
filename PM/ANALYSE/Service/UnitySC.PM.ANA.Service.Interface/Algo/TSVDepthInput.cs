using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(TSVAcquisitionStrategy))]
    [XmlInclude(typeof(TSVMeasurePrecision))]
    public class TSVDepthInput : IANAInputFlow
    {
        public TSVDepthInput()
        { }

        public TSVDepthInput(Length approximateDepth, Length approximateWidth, Length depthTolerance, ProbeSettings probeSettings, TSVAcquisitionStrategy acquisitionStrategy, TSVMeasurePrecision measurePrecision)
        {
            ApproximateDepth = approximateDepth;
            ApproximateWidth = approximateWidth;
            DepthTolerance = depthTolerance;
            Probe = probeSettings;
            AcquisitionStrategy = acquisitionStrategy;
            MeasurePrecision = measurePrecision;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ApproximateDepth is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"ApproximateDepth is missing.");
            }

            if (ApproximateWidth is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"ApproximateWidth is missing.");
            }

            if (Probe is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"Probe is missing.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public Length ApproximateDepth { get; set; }

        [DataMember]
        public Length DepthTolerance { get; set; }

        [DataMember]
        public Length ApproximateWidth { get; set; }

        [DataMember]
        public ProbeSettings Probe { get; set; }

        [DataMember]
        public TSVAcquisitionStrategy AcquisitionStrategy { get; set; }

        [DataMember]
        public TSVMeasurePrecision MeasurePrecision { get; set; }
    }
}
