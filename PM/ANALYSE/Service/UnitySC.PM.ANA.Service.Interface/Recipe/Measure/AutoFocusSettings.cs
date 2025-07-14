using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class AutoFocusSettings
    {
        [DataMember]
        public AutoFocusType Type { get; set; }

        [DataMember]
        public double LiseGain { get; set; }

        [DataMember]
        public string ProbeId { get; set; }

        [DataMember]
        public ScanRange LiseScanRange { get; set; }

        [DataMember]
        public ObjectiveContext LiseAutoFocusContext { get; set; }

        [DataMember]
        public Length LiseOffsetX { get; set; }

        [DataMember]
        public Length LiseOffsetY { get; set; }

        [DataMember]
        public ScanRangeType CameraScanRange { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public ScanRangeWithStep CameraScanRangeConfigured { get; set; }

        [DataMember]
        public ImageAcquisitionContextBase ImageAutoFocusContext { get; set; }

        [DataMember]
        public bool UseCurrentZPosition { get; set; }

        /// <summary>
        /// Parameter used only for Objective Calibration to tell the AutofocusFlow to
        /// use another algorithm that works well on the REF CAM (and only on the REF CAM!).
        /// </summary>
        [DataMember]
        public bool AutofocusModifiedLaplacien { get; set; } = false;

        public virtual InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);
            if (Type == AutoFocusType.Lise || Type == AutoFocusType.LiseAndCamera)
            {
                var liseInput = new AFLiseInput(LiseAutoFocusContext, ProbeId, LiseGain);
                validity.ComposeWith(liseInput.CheckInputValidity());
            }
            if (Type == AutoFocusType.Camera || Type == AutoFocusType.LiseAndCamera)
            {
                var cameraInput = new AFCameraInput(ImageAutoFocusContext, CameraId, CameraScanRange, CameraScanRangeConfigured);
                validity.ComposeWith(cameraInput.CheckInputValidity());
            }
            // Lise X/Y Offsets can be null
            return validity;
        }
    }
}
