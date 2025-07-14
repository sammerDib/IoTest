using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [XmlInclude(typeof(ACSAxisConfig))]
    [XmlInclude(typeof(PiezoAxisConfig))]
    [XmlInclude(typeof(AerotechAxisConfig))]
    [XmlInclude(typeof(RelianceAxisConfig))]
    [XmlInclude(typeof(PhytronAxisConfig))]
    [XmlInclude(typeof(ThorlabsSliderAxisConfig))]
    [XmlInclude(typeof(ParallaxAxisConfig))]
    [XmlInclude(typeof(IoAxisConfig))]
    [XmlInclude(typeof(CNCAxisConfig))]
    [DataContract]
    public class MotorizedAxisConfig : AxisConfig
    {
        public MotorizedAxisConfig()
            : base()
        {
            HasExternalTrigger = false;
            MotorDirection = 0;
            AccelSlow = 0;
            AccelNormal = 0;
            AccelFast = 0;
            AccelMeasure = 0;
            AccelScan = 0;
            CanStep = 1;
            PositionTolerance = 0.1.Millimeters();
            ObjectiveSlot = 0;
        }

        #region Properties

        [DataMember]
        public virtual bool HasExternalTrigger { get; set; }

        ///<summary>
        ///Motor direction normal/reverse (1 or -1)
        ///</summary>
        [DataMember]
        public virtual int MotorDirection { get; set; }

        ///<summary>
        ///Predifined acceleration used for slow speed (mm.s-2)
        ///</summary>
        [DataMember]
        public virtual double AccelSlow { get; set; }

        ///<summary>
        ///Predifined acceleration used for normal speed (mm.s-2)
        ///</summary>
        [DataMember]
        public virtual double AccelNormal { get; set; }

        ///<summary>
        ///Predifined acceleration used for fast speed (mm.s-2)
        ///</summary>
        [DataMember]
        public virtual double AccelFast { get; set; }

        ///<summary>
        ///Predifined acceleration used when measuring (mm.s-2)
        ///</summary>
        [DataMember]
        public virtual double AccelMeasure { get; set; }

        ///<summary>
        ///Predifined acceleration used when scanning (mm.s-2)
        ///</summary>
        [DataMember]
        public virtual double AccelScan { get; set; }

        [DataMember]
        public virtual double CanStep { get; set; }

        [DataMember]
        public virtual int ObjectiveSlot { get; set; }

        #endregion Properties
    }
}
