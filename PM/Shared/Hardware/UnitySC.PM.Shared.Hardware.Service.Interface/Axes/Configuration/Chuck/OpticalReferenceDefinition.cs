using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    /* Class allowing to define references to use for calibration -> linked to current chuck selection */

    [DataContract(Namespace = "")]
    [Serializable]
    public class OpticalReferenceDefinition
    {
        [DataMember(Order = 1)]
        [Browsable(true), Category("Common")]
        public int ID { get; set; }

        [DataMember(Order = 10)]
        [Browsable(true), Category("Common")]
        public Length PositionX { get; set; }

        [DataMember(Order = 20)]
        [Browsable(true), Category("Common")]
        public Length PositionY { get; set; }

        [DataMember(Order = 30)]
        [Browsable(true), Category("Common")]
        public Length PositionZ { get; set; }

        [DataMember(Order = 35)]
        [Browsable(true), Category("Common")]
        public Length PositionZLower { get; set; }

        [DataMember(Order = 40)]
        [Browsable(true), Category("Common")]
        public string PositionObjectiveID { get; set; }

        [DataMember(Order = 60)]
        [Browsable(true), Category("Common")]
        public Length RefThickness { get; set; }

        [DataMember(Order = 70)]
        [Browsable(true), Category("Common")]
        public Length RefTolerance { get; set; }

        [DataMember(Order = 80)]
        [Browsable(true), Category("Common")]
        public float RefRefrIndex { get; set; }

        [DataMember(Order = 90)]
        [Browsable(true), Category("Common")]
        public String ReferenceName { get; set; }

        [DataMember(Order = 110)]
        [Browsable(true), Category("Common")]
        public OverlayReferenceConfiguration OverlayConfiguration { get; set; }

        /*Deprecated for compatibility*/
        public int OldIndex;

        public OpticalReferenceDefinition()
        {
            // ProbesSettings = new List<cProbeSettings>();
            RefThickness = 3000.0.Micrometers();
            RefTolerance = 3000.0.Micrometers();
            RefRefrIndex = 1.0f;
            ReferenceName = "";
        }
    }
}
