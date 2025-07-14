using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class PSIInput : IANAInputFlow
    {
        public enum PhaseUnwrappingAlgo
        {
            Goldstein,
            ReliabilityHistogram,
            QualityGuidedByPseudoCorrelation,
            QualityGuidedByVariance,
            QualityGuidedByGradient
        }

        public enum PhaseCalculationAlgo
        {
            RobustCMP,
            Hariharan
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public Length Wavelength { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public Length StepSize { get; set; }

        [DataMember]
        public int StepCount { get; set; }

        [DataMember]
        public int ImagesPerStep { get; set; }

        [DataMember]
        public CenteredRegionOfInterest ROI { get; set; }

        [DataMember]
        public PhaseCalculationAlgo PhaseCalculation { get; set; }

        [DataMember]
        public PhaseUnwrappingAlgo PhaseUnwrapping { get; set; }

        public PSIInput()
        {
        }

        public PSIInput(string objectiveId, string cameraId, Length step, int stepCount, int imagesPerStep, CenteredRegionOfInterest roi, PhaseCalculationAlgo phaseCalculation, PhaseUnwrappingAlgo phaseUnwrapping, Length wavelength)
            : this(null, objectiveId, cameraId, step, stepCount, imagesPerStep, roi, phaseCalculation, phaseUnwrapping, wavelength)
        {
        }

        public PSIInput(ANAContextBase initialContext, string objectiveId, string cameraId, Length step, int stepCount, int imagesPerStep, CenteredRegionOfInterest roi, PhaseCalculationAlgo phaseCalculation, PhaseUnwrappingAlgo phaseUnwrapping, Length wavelength)
        {
            InitialContext = initialContext;
            ObjectiveId = objectiveId;
            CameraId = cameraId;
            StepSize = step;
            StepCount = stepCount;
            ImagesPerStep = imagesPerStep;
            ROI = roi;
            PhaseCalculation = phaseCalculation;
            PhaseUnwrapping = phaseUnwrapping;
            Wavelength = wavelength;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);
            var messages = new List<string>();

            if (StepSize is null)
            {
                validity.IsValid = false;
                messages.Add("Step is missing.");
            }

            if (StepCount <= 0)
            {
                validity.IsValid = false;
                messages.Add("StepCount should be trictly positive.");
            }

            if (CameraId is null)
            {
                validity.IsValid = false;
                messages.Add("CameraId is missing.");
            }

            if (ImagesPerStep <= 0)
            {
                validity.IsValid = false;
                messages.Add("ImagesPerStep should be trictly positive.");
            }

            return validity;
        }
    }
}
