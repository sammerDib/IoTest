using System;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.ExternalFile;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Context
{
    [DataContract]
    public class PatternRecognitionDataWithContext : PatternRecognitionData, ICloneable
    {
        public PatternRecognitionDataWithContext()
        {
        }

        public PatternRecognitionDataWithContext(ExternalImage patternReference, string cameraId, RegionOfInterest roi, double gamma, TopImageAcquisitionContext context) : base(patternReference, cameraId, roi, gamma)
        {
            Context = context;
        }

        public override InputValidity CheckInputValidity()
        {
            var validity = base.CheckInputValidity();

            if (Context is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The context is missing.");
            }

            return validity;
        }

        // Be aware that the ExternalImage BitmapSource data will not be clone
        public object Clone()
        {
            var extImgClone = PatternReference.DeepClone();
            var topImgCtx = Context as TopImageAcquisitionContext;
            return new PatternRecognitionDataWithContext(extImgClone, CameraId, RegionOfInterest, Gamma, topImgCtx);
        }

        [DataMember]
        public ImageAcquisitionContextBase Context { get; set; }
    }
}
