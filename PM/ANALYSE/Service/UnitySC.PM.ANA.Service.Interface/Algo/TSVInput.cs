using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(TSVAcquisitionStrategy))]
    [XmlInclude(typeof(TSVMeasurePrecision))]
    public class TSVInput : IANAInputFlow
    {
        // For serialization
        public TSVInput()
        {
        }

        public TSVInput(string cameraId, ProbeSettings probeSettings, TSVShape shape, Length approximateDepth, Length approximateLength, Length approximateWidth, Length depthTolerance, Length lengthTolerance, Length widthTolerance, TSVAcquisitionStrategy acquisitionStrategy, TSVMeasurePrecision measurePrecision, CenteredRegionOfInterest roi, List<LayerSettings> physicalLayers, ShapeDetectionModes shapeDetectionMode)
        {
            CameraId = cameraId;
            Probe = probeSettings;
            Shape = shape;
            ShapeDetectionMode = shapeDetectionMode;
            ApproximateDepth = approximateDepth;
            ApproximateLength = approximateLength;
            ApproximateWidth = approximateWidth;
            DepthTolerance = depthTolerance;
            LengthTolerance = lengthTolerance;
            WidthTolerance = widthTolerance;
            AcquisitionStrategy = acquisitionStrategy;
            MeasurePrecision = measurePrecision;
            RegionOfInterest = roi;
            PhysicalLayers = physicalLayers;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (CameraId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"CameraId is missing.");
            }

            if (Probe is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"Probe is missing.");
            }

            if (ApproximateDepth is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"ApproximateDepth is missing.");
            }

            if (ApproximateLength is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"ApproximateLength is missing.");
            }

            if (ApproximateWidth is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"ApproximateWidth is missing.");
            }

            if (DepthTolerance is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"DepthTolerance is missing.");
            }

            if (LengthTolerance is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"LengthTolerance is missing.");
            }

            if (WidthTolerance is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"WidthTolerance is missing.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public TSVShape Shape { get; set; }

        [DataMember]
        public ShapeDetectionModes ShapeDetectionMode { get; set; }

        [DataMember]
        public Length ApproximateDepth { get; set; }

        [DataMember]
        public Length ApproximateLength { get; set; }

        [DataMember]
        public Length ApproximateWidth { get; set; }

        [DataMember]
        public Length DepthTolerance { get; set; }

        [DataMember]
        public Length LengthTolerance { get; set; }

        [DataMember]
        public Length WidthTolerance { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public ProbeSettings Probe { get; set; }

        [DataMember]
        public List<LayerSettings> PhysicalLayers { get; set; }

        [DataMember]
        public CenteredRegionOfInterest RegionOfInterest { get; set; }

        [DataMember]
        public TSVAcquisitionStrategy AcquisitionStrategy { get; set; }

        [DataMember]
        public TSVMeasurePrecision MeasurePrecision { get; set; }

    }
}
