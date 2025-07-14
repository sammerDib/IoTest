using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Acquisition
{
    public class ReflectivityAcquisitionSetup : BaseAcquisitionSetup, ICloneable
    {
        private enumScreenColor m_ScreenColor;

        public enumScreenColor ScreenColor
        {
            get { return m_ScreenColor; }
            set { m_ScreenColor = value; }
        }

        private double m_dExposureTime;

        public double ExposureTime
        {
            get { return m_dExposureTime; }
            set { m_dExposureTime = value; }
        }

        private int m_iStabTime;

        public int StabTime
        {
            get { return m_iStabTime; }
            set { m_iStabTime = value; }
        }
        private int m_iGain;

        public int Gain
        {
            get { return m_iGain; }
            set { m_iGain = value; }
        }

        private bool m_bDoCutUp;

        public bool DoCutUp
        {
            get { return m_bDoCutUp; }
            set { m_bDoCutUp = value; }
        }

        private string m_sCutUpRecipePath;

        public string CutUpRecipePath
        {
            get { return m_sCutUpRecipePath; }
            set { m_sCutUpRecipePath = value; }
        }

        public ReflectivityAcquisitionSetup()
            : base()
        {
            Type = enumSaverType.esReflectivity;
        }
        public new object Clone()
        {
            return (ReflectivityAcquisitionSetup)this.MemberwiseClone();
        }
    }
}
