using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class VSIInput : IANAInputFlow
    {
        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public XYZTopZBottomPosition StartPosition { get; set; }

        [DataMember]
        public Length StepSize { get; set; }

        [DataMember]
        public int StepCount { get; set; }

        [DataMember]
        public CenteredRegionOfInterest ROI { get; set; }

        public VSIInput()
        {
        }

        public VSIInput(string objectiveId, string cameraId, XYZTopZBottomPosition startPosition, Length stepSize, int stepCount)
            : this(null, objectiveId, cameraId, startPosition, stepSize, stepCount)
        {
        }

        public VSIInput(ANAContextBase initialContext, string objectiveId, string cameraId, XYZTopZBottomPosition startPosition, Length stepSize, int stepCount)
        {
            InitialContext = initialContext;
            ObjectiveId = objectiveId;
            CameraId = cameraId;
            StartPosition = startPosition;
            StepSize = stepSize;
            StepCount = stepCount;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);
            var messages = new List<string>();

            if (StartPosition is null)
            {
                validity.IsValid = false;
                messages.Add("Step is missing.");
            }

            if (StepCount <= 0)
            {
                validity.IsValid = false;
                messages.Add("StepCount should be strictly positive.");
            }

            if (StepSize?.Value <= 0)
            {
                validity.IsValid = false;
                messages.Add("StepSize should be strictly positive.");
            }

            if (CameraId is null)
            {
                validity.IsValid = false;
                messages.Add("CameraId is missing.");
            }

            if (ObjectiveId is null)
            {
                validity.IsValid = false;
                messages.Add("ObjectiveId is missing.");
            }

            return validity;
        }
    }
}
