using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{

    [Serializable]
    public enum OverlayReferenceRole
    {
        Base,
        Top,
        Bottom
    }
    [Serializable]
    public enum OverlayReferenceMatchingMethode
    {
        PrincipalDiagonalTarget,
        MinorDiagonalTarget,
        Symmetry
    }

    [DataContract(Namespace = "")]
    [Serializable]
    public class OverlayObjectiveReferenceConfiguration
    {
        [DataMember(Order = 1)]
        [Browsable(true), Category("Common")]
        public string ObjectiveName { get; set; }
        [DataMember(Order = 2)]
        [Browsable(true), Category("Common")]
        public string ImagePath { get; set; }
        [DataMember(Order = 3)]
        [Browsable(true), Category("Common")]
        public string PRConfigPath { get; set; }
        [Browsable(true), Category("Common")]
        public bool DoAutofocus { get; set; }
        [Browsable(true), Category("Common")]
        public bool DoPR { get; set; }
        //[Browsable(true), Category("Common")]
        //public ImageParams ImageParams { get; set; }
        [Browsable(true), Category("Common")]
        public OverlayReferenceMatchingMethode Method { get; set; }
    }

    [DataContract(Namespace = "")]
    [Serializable]
    public class OverlayHeadReferenceConfiguration
    {
        //[DataMember(Order = 1)]
        //[Browsable(true), Category("Common")]
        //public enSelectedProbe Location { get; set; }
        [DataMember(Order = 2)]
        [Browsable(true), Category("Objectives")]
        public List<OverlayObjectiveReferenceConfiguration> Objectives { get; set; }
        [DataMember(Order = 3)]
        [Browsable(true), Category("Camera")]
        public string Camera { get; set; }
    }

    [DataContract(Namespace = "")]
    [Serializable]
    public class OverlayReferenceConfiguration
    {
        [DataMember(Order = 1)]
        [Browsable(true), Category("Common")]
        public double DeltaX { get; set; }
        [DataMember(Order = 2)]
        [Browsable(true), Category("Common")]
        public double DeltaY { get; set; }

        public OverlayReferenceRole Role { get; set; }

        [DataMember(Order = 4)]
        [Browsable(true), Category("Head")]
        public List<OverlayHeadReferenceConfiguration> Head { get; set; }
    }
}
