using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Common.Acquisition
{
    #region objects from C++ Wrapper
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]

    public struct ALTASIGHT_FACTORY
    {
        public int use_y_axe_for_Curvature_X;
        public int use_y_axe_for_Amplitude_X;
        public int mask_map;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct REPORT_CONFIG
    {
        [XmlIgnore]
        public double min_curvature;
        [XmlIgnore]
        public double max_curvature;
        public double min_amplitude;
        public double max_amplitude;
        public int used_default;
    };
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CALCULATE_CONFIG
    {
        public int phase_map;
        public string phase_map_path;
    };
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct FILTER_FACTORY
    {
        public int Contraste_min_amplitude;
        public int Intensite_min_amplitude;

        public int Contraste_min_curvature;
        public int Intensite_min_curvature;
    };
    #endregion

    public class CxAxAcquisitionSetup : BaseAcquisitionSetup,ICloneable
    {
        private ALTASIGHT_FACTORY m_altasightFactorySettings;

        public ALTASIGHT_FACTORY AltasightFactorySettings
        {
            get { return m_altasightFactorySettings; }
            set { m_altasightFactorySettings = value; }
        }

        private enumSensorID m_sensorID;

        public enumSensorID SensorID
        {
            get { return m_sensorID; }
            set { m_sensorID = value; }
        }

        private REPORT_CONFIG m_reportConfigSettings;

        public REPORT_CONFIG ReportConfigSettings
        {
            get { return m_reportConfigSettings; }
            set { m_reportConfigSettings = value; }
        }

        private CALCULATE_CONFIG m_calculateConfigSettings;

        public CALCULATE_CONFIG CalculateConfigSettings
        {
            get { return m_calculateConfigSettings; }
            set { m_calculateConfigSettings = value; }
        }

        private FILTER_FACTORY m_filterFactorySettings;

        public FILTER_FACTORY FilterFactorySettings
        {
            get { return m_filterFactorySettings; }
            set { m_filterFactorySettings = value; }
        }

        private String m_sXLogFilePath;

        public String XLogFilePath
        {
            get { return m_sXLogFilePath; }
            set { m_sXLogFilePath = value; }
        }
        private String m_sYLogFilePath;

        public String YLogFilePath
        {
            get { return m_sYLogFilePath; }
            set { m_sYLogFilePath = value; }
        }

        private Byte m_byTypeOfFrame;
        
        public Byte TypeOfFrame
        {
            get { return m_byTypeOfFrame; }
            set { m_byTypeOfFrame = value; }
        }

        private Byte m_byTypeOfCutUp;

        public Byte TypeOfCutUp
        {
            get { return m_byTypeOfCutUp; }
            set { m_byTypeOfCutUp = value; }
        }

        private string m_sCutUpRecipePath;

        public string CutUpRecipePath
        {
            get { return m_sCutUpRecipePath; }
            set { m_sCutUpRecipePath = value; }
        }

        private bool m_bNormalised;

        public bool Normalised
        {
            get { return m_bNormalised; }
            set { m_bNormalised = value; }
        }
        private String m_sLotID;

        [XmlIgnore]
        public String LotID
        {
            get { return m_sLotID; }
            set { m_sLotID = value; }
        }
        private int m_iSigma;

        public int Sigma
        {
            get { return m_iSigma; }
            set { m_iSigma = value; }
        }
        private int m_iFilterIndex;

        [XmlIgnore]
        public int FilterIndex
        {
            get { return m_iFilterIndex; }
            set { m_iFilterIndex = value; }
        }
        public CxAxAcquisitionSetup()
            : base()
        {
        }
        public new object Clone()
        {
            CxAxAcquisitionSetup St = (CxAxAcquisitionSetup)this.MemberwiseClone();
            return St;
        }
    }
}
