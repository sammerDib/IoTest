using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Common;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [XmlInclude(typeof(Valve))]
    [DataContract]
    public class WaferClampConfig
    {
        #region Constructors

        public WaferClampConfig()
        {
            Available = false;
            WaferSize = 200.Millimeters();
            ValveName = "WaferClamp";
        }

        #endregion Constructors

        #region Properties

        [DataMember]
        public virtual bool Available { get; set; }

        [DataMember]
        public virtual Length WaferSize { get; set; }

        [DataMember]
        public virtual string ValveName { get; set; }

        [DataMember]
        public bool IsFilmFrame { get; set; }

        [DataMember]
        public Valve Valve = new Valve();

        #endregion Properties
    }

    public class Valve
    {
        [DataMember]
        [XmlAttribute("VacuumValve1")]
        public bool VacuumValve1 { get; set; }

        [DataMember]
        [XmlAttribute("VacuumValve2")]
        public bool VacuumValve2 { get; set; }

        [DataMember]
        [XmlAttribute("Vacuum1")]
        public bool Vacuum1 { get; set; }

        [DataMember]
        [XmlAttribute("Vacuum2")]
        public bool Vacuum2 { get; set; }
    }
    [Serializable]
    [DataContract]
    public class ACSAxisIDLink
    {
        [DataMember]
        public String ACSID { get; set; }

        [DataMember]
        public String AxisID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ACSControllerConfig : IOControllerConfig
    {
        #region Fields

        [DataMember]
        public Length PositionLimitEpsilon { get; set; } = (0.01).Millimeters();

        [Category("EthernetCom")]
        [DisplayName("EthernetCom")]
        [DataMember]
        public EthernetCom EthernetCom { get; set; }

        public List<ACSAxisIDLink> ACSAxisIDLinks { get; set; }

        [DataMember]
        [Browsable(false)]
        public List<WaferClampConfig> WaferClampList
        {
            get;
            set;
        }
        #endregion Fields

        #region Constructors

        public ACSControllerConfig()
          : base()
        {
            EthernetCom = new EthernetCom();
        }

        #endregion Constructors
    }
}
