using System;
using System.Globalization;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.ANA.Client.Proxy.Axes.Models
{
    [XmlRoot("Scenario")]
    public class Scenario : ObservableObject

    {
        #region Fields
        private string _fileName;
        private StateScenario _state;
        private string _name;
        private int _numberOfLines;
        private int _numberOfCycles;
        private int _numberOfLinesTreated;
        #endregion


        #region Constructors
        #endregion

        #region Public methods

        [XmlArray("ScenarioItemList")]
        public ScenarioItem[] ScenarioItemList { get; set; }
        [XmlElement("Name")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        [XmlElement(ElementName = "GlobalSpeedX")]
        public string GlobalSpeedXItem
        {
            get
            {
                return GlobalSpeedX.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalSpeedX = Double.Parse(value, CultureInfo.InvariantCulture);

            }
        }
        [XmlElement(ElementName = "GlobalSpeedY")]
        public string GlobalSpeedYItem
        {
            get
            {
                return GlobalSpeedY.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalSpeedY = Double.Parse(value, CultureInfo.InvariantCulture);

            }
        }
        [XmlElement(ElementName = "GlobalSpeedZTop")]
        public string GlobalSpeedZTopItem
        {
            get
            {
                return GlobalSpeedZTop.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalSpeedZTop = Double.Parse(value, CultureInfo.InvariantCulture);

            }
        }
        [XmlElement(ElementName = "GlobalSpeedZBottom")]
        public string GlobalSpeedZBottomItem
        {
            get
            {
                return GlobalSpeedZBottom.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalSpeedZBottom = Double.Parse(value, CultureInfo.InvariantCulture);

            }
        }
        [XmlElement("GlobalAccelX")]
        public string GlobalAccelXItem
        {
            get
            {
                return GlobalAccelX.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalAccelX = Double.Parse(value, CultureInfo.InvariantCulture);

            }
        }
        [XmlElement("GlobalAccelY")]
        public string GlobalAccelYItem
        {
            get
            {
                return GlobalAccelY.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalAccelY = Double.Parse(value, CultureInfo.InvariantCulture);


            }
        }
        [XmlElement("GlobalAccelZTop")]
        public string GlobalAccelZTopItem
        {
            get
            {
                return GlobalAccelZTop.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalAccelZTop = Double.Parse(value, CultureInfo.InvariantCulture);

            }
        }
        [XmlElement("GlobalAccelZBottom")]
        public string GlobalAccelZBottomItem
        {
            get
            {
                return GlobalAccelZBottom.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                GlobalAccelZBottom = Double.Parse(value, CultureInfo.InvariantCulture);

            }
        }
        [XmlIgnore]
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
        [XmlIgnore]
        public StateScenario State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }
        [XmlIgnore]
        public int NumberOfLines
        {
            get
            {
                return _numberOfLines;
            }
            set
            {
                _numberOfLines = value;
                OnPropertyChanged();
            }
        }
        [XmlIgnore]
        public int NumberOfCycles
        {
            get
            {
                return _numberOfCycles;
            }
            set
            {
                _numberOfCycles = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public int NumberOfLinesTreated
        {
            get
            {
                return _numberOfLinesTreated;
            }
            set
            {
                _numberOfLinesTreated = value;
                OnPropertyChanged();
            }
        }
        [XmlIgnore]
        public double GlobalSpeedX
        {
            get; set;
        }
        [XmlIgnore]
        public double GlobalSpeedY
        {
            get; set;
        }
        [XmlIgnore]
        public double GlobalSpeedZTop
        {
            get; set;
        }
        [XmlIgnore]
        public double GlobalSpeedZBottom
        {
            get; set;
        }
        [XmlIgnore]
        public double GlobalAccelX
        {
            get; set;
        }
        [XmlIgnore]
        public double GlobalAccelY
        {
            get; set;
        }
        [XmlIgnore]
        public double GlobalAccelZTop
        {
            get; set;
        }
        [XmlIgnore]
        public double GlobalAccelZBottom
        {
            get; set;
        }
        #endregion
    }
}
