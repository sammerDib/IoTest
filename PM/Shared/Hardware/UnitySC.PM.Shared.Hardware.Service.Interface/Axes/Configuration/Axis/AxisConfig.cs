using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public enum MovingDirection
    {
        Unknown = -1,
        X = 0,
        Y = 1,
        ZTop = 2,
        XLower = 3,
        YLower = 4,
        ZBottom = 5,
        ZPiezo = 6,
        Z = 7,
        T = 8,
        Linear = 9,
        Rotation = 10,
    }

    [XmlInclude(typeof(DummyAxisConfig))]
    [XmlInclude(typeof(ACSAxisConfig))]
    [XmlInclude(typeof(MotorizedAxisConfig))]
    [XmlInclude(typeof(PiezoAxisConfig))]
    [XmlInclude(typeof(RelianceAxisConfig))]
    [XmlInclude(typeof(PhytronAxisConfig))]
    [XmlInclude(typeof(AerotechAxisConfig))]
    [XmlInclude(typeof(ThorlabsSliderAxisConfig))]
    [XmlInclude(typeof(OwisAxisConfig))]
    [XmlInclude(typeof(ParallaxAxisConfig))]
    [XmlInclude(typeof(IoAxisConfig))]
    [XmlInclude(typeof(CNCAxisConfig))]
    [DataContract]
    public class AxisConfig
    {
        #region Constructors

        public AxisConfig()
        {
            Name = string.Empty;
            AxisID = string.Empty;
            ControllerID = string.Empty;
            SpeedSlow = 500;
            SpeedNormal = 6000;
            SpeedFast = 10000;
            SpeedMeasure = 6000;
            PositionMax = 0.Millimeters();
            PositionMin = 0.Millimeters();
            PositionPark = new Length(0, LengthUnit.Millimeter);
            PositionManualLoad = new Length(0, LengthUnit.Millimeter);
            PositionHome = 0.Millimeters();
            PositionZero = 0.Millimeters();
            Initialized = false;
            MovingDirection = MovingDirection.Unknown;
            LastPos = 0.0;
            MotionOffset = new List<float>();
            MinSpeed = 0.1;
            Unbounded = false;
        }

        #endregion Constructors

        #region Properties

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual string AxisID { get; set; }

        [DataMember]
        public virtual string ControllerID { get; set; }

        ///<summary>
        ///Direction in which this axis moves
        ///</summary>
        [DataMember]
        public virtual MovingDirection MovingDirection { get; set; }

        ///<summary>
        ///Predifined slow speed (mm.s-1)
        ///</summary>
        [DataMember]
        public virtual double SpeedSlow { get; set; }

        ///<summary>
        ///Minimum speed (mm.s-1)
        ///</summary>
        [DataMember]
        public virtual double MinSpeed { get; set; }

        ///<summary>
        ///Predifined normal speed (mm.s-1)
        ///</summary>
        [DataMember]
        public virtual double SpeedNormal { get; set; }

        ///<summary>
        ///Predifined fast speed (mm.s-1)
        ///</summary>
        [DataMember]
        public virtual double SpeedFast { get; set; }

        ///<summary>
        ///Predifined speed used for measuring (mm.s-1)
        ///</summary>
        [DataMember]
        public virtual double SpeedMeasure { get; set; }

        ///<summary>
        ///Predifined max speed used for scanning (mm.s-1)
        ///</summary>
        [DataMember]
        public virtual double SpeedMaxScan { get; set; }

        ///<summary>
        ///Axis maximum coordinate
        ///</summary>
        [DataMember]
        public virtual Length PositionMax { get; set; }

        ///<summary>
        ///Axis minimum coordinate
        ///</summary>
        [DataMember]
        public virtual Length PositionMin { get; set; }

        ///<summary>
        ///Predifined park position
        ///</summary>
        [DataMember]
        public virtual Length PositionPark { get; set; }

        ///<summary>
        ///Predifined position for manual wafer load
        ///</summary>
        [DataMember]
        public virtual Length PositionManualLoad { get; set; }

        // FIXME: seems to be useless
        [DataMember]
        public virtual Length PositionTolerance { get; set; }

        /// <summary>
        /// Minimum distance travelled from which a notification to the client is triggered.
        /// </summary>
        [DataMember]
        public Length DistanceThresholdForNotification { get; set; } = 0.001.Millimeters();

        ///<summary>
        ///Predifined home position
        ///</summary>
        [DataMember]
        public Length PositionHome { get; set; }

        [DataMember]
        public virtual Length PositionZero { get; set; }

        [DataMember]
        public List<float> MotionOffset { get; set; }

        [DataMember]
        public virtual double LastPos { get; set; }

        [DataMember]
        public bool ResetOnPark { get; set; }

        [DataMember]
        public virtual bool Initialized { get; set; }

        [DataMember]
        public virtual bool IsSafe { get; set; }

        ///<summary>
        ///Define if axis has limit at extremes(mm)
        ///</summary>
        [DataMember]
        public virtual bool Unbounded { get; set; }

        #endregion Properties
    }
}
