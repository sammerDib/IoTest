using System;
using System.Globalization;
using System.Xml.Serialization;

namespace UnitySC.PM.ANA.Client.Proxy.Axes.Models
{

    public class ScenarioItem 
    {
        [XmlAttribute("XPos")]
        public string XPosItem
        { 
            get {
                return XPos.ToString();
            }
            set {
                XPos = Double.Parse(value, CultureInfo.InvariantCulture);
            }
        }
        [XmlAttribute("YPos")]
        public string YPosItem
        {
            get {
                return YPos.ToString();
            }
            set {
                YPos = Double.Parse(value, CultureInfo.InvariantCulture);
            }
        }
        [XmlAttribute("ZTopPos")]
        public string ZTopPosItem
        {
            get {
                return ZTopPos.ToString();
            }
            set {
                ZTopPos = Double.Parse(value, CultureInfo.InvariantCulture);
            }
        }
        [XmlAttribute("ZBottomPos")]
        public string ZBottomPosItem
        {
            get {
                return ZBottomPos.ToString();
            }
            set {
                ZBottomPos = Double.Parse(value, CultureInfo.InvariantCulture);
            }
        }
        [XmlAttribute(AttributeName ="SpeedX")]
        public string SpeedXItem
        {
            get
            {
                return SpeedX.ToString();
            }
            set
            {
                if (value != null)
                {
                    SpeedX = Double.Parse(value, CultureInfo.InvariantCulture);
                }
              

            }
        }
        [XmlAttribute(AttributeName = "SpeedY")]
        public string SpeedYItem
        {
            get
            {
                return SpeedY.ToString();
            }
            set
            {
                if (value != null)
                {
                    SpeedY = Double.Parse(value, CultureInfo.InvariantCulture);
                }


            }
        }
        [XmlAttribute(AttributeName = "SpeedZTop")]
        public string SpeedZTopItem
        {
            get
            {
                return SpeedZTop.ToString();
            }
            set
            {
                if (value != null)
                {
                    SpeedZTop = Double.Parse(value, CultureInfo.InvariantCulture);
                }


            }
        }
        [XmlAttribute(AttributeName = "SpeedZBottom")]
        public string SpeedZBottomItem
        {
            get
            {
                return SpeedZBottom.ToString();
            }
            set
            {
                if (value != null)
                {
                    SpeedZBottom = Double.Parse(value, CultureInfo.InvariantCulture);
                }


            }
        }
        [XmlAttribute(AttributeName = "AccelX")]
        public string AccelXItem
        {
            get
            {
                return AccelX.ToString();
            }
            set
            {
                if (value != null)
                {
                    AccelX = Double.Parse(value, CultureInfo.InvariantCulture);
                }


            }
        }
        [XmlAttribute(AttributeName = "AccelY")]
        public string AccelYItem
        {
            get
            {
                return AccelY.ToString();
            }
            set
            {
                if (value != null)
                {
                    AccelY = Double.Parse(value, CultureInfo.InvariantCulture);
                }


            }
        }
        [XmlAttribute(AttributeName = "AccelZTop")]
        public string AccelZTopItem
        {
            get
            {
                return AccelZTop.ToString();
            }
            set
            {
                if (value != null)
                {
                    AccelZTop = Double.Parse(value, CultureInfo.InvariantCulture);
                }


            }
        }
        [XmlAttribute(AttributeName = "AccelZBottom")]
        public string AccelZBottomItem
        {
            get
            {
                return AccelZBottom.ToString();
            }
            set
            {
                if (value != null)
                {
                    AccelZBottom = Double.Parse(value, CultureInfo.InvariantCulture);
                }


            }
        }
        [XmlIgnore]
        public double? XPos
        {
            get; set;
        }

        [XmlIgnore]
        public double? YPos
        {
            get; set;
        }
        [XmlIgnore]
        public double? ZTopPos
        {
            get; set;
        }
        [XmlIgnore]
        public double? ZBottomPos
        {
            get; set;
        }
        [XmlIgnore]
        public double? SpeedX
        {
            get; set;
        }
        [XmlIgnore]
        public double? SpeedY
        {
            get; set;
        }
        [XmlIgnore]
        public double? SpeedZTop
        {
            get; set;
        }
        [XmlIgnore]
        public double? SpeedZBottom
        {
            get; set;
        }
        [XmlIgnore]
        public double? AccelX
        {
            get; set;
        }
        [XmlIgnore]
        public double? AccelY
        {
            get; set;
        }
        [XmlIgnore]
        public double? AccelZTop
        {
            get; set;
        }
        [XmlIgnore]
        public double? AccelZBottom
        {
            get; set;
        }
    }
}
