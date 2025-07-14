using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Fringe_manager;
using Common.Camera;
using System.Xml.Serialization;
using System.IO;

namespace Common.Acquisition
{
    [XmlInclude(typeof(BaseAcquisitionSetup))]
    [XmlInclude(typeof(CxAxAcquisitionSetup))]
    [XmlInclude(typeof(ReflectivityAcquisitionSetup))]
    public class AcquisitionSetup : ICloneable
    {
        private List<GenericAcquisitionSetup> m_fringeSaversParameters;

        public List<GenericAcquisitionSetup> FringeSaversParameters
        {
            get { return m_fringeSaversParameters; }
            set { m_fringeSaversParameters = value; }
        }

        private List<GenericAcquisitionSetup> m_reflectivitySaversParameters;

        private enumSensorID m_sensorID;

        public enumSensorID SensorID
        {
            get { return m_sensorID; }
            set { m_sensorID = value; }
        }

        public List<GenericAcquisitionSetup> ReflectivitySaversParameters
        {
            get { return m_reflectivitySaversParameters; }
            set { m_reflectivitySaversParameters = value; }
        }

        private String m_sFringeRemoteName;

        public String FringeRemoteName
        {
            get { return m_sFringeRemoteName; }
            set { m_sFringeRemoteName = value; }
        }

        private bool m_bDoPSDCal;

        public bool DoPSDCal
        {
            get { return m_bDoPSDCal; }
            set { m_bDoPSDCal = value; }
        }

        private double m_dImageDynamic;

        public double ImageDynamic
        {
            get { return m_dImageDynamic; }
            set { m_dImageDynamic = value; }
        }

        private FringeFolder m_fringes;
        [XmlIgnore]
        public FringeFolder Fringes
        {
            get { return m_fringes; }
            set
            {
                m_fringes = value;
                m_sFringeRemoteName = m_fringes.RemoteName;
            }
        }

        private int m_iStabilizationTime;

        public int StabilizationTime
        {
            get { return m_iStabilizationTime; }
            set { m_iStabilizationTime = value; }
        }

        private ExposureGainPair m_exposureGainValues;

        public ExposureGainPair ExposureAndGainValues
        {
            get { return m_exposureGainValues; }
            set { m_exposureGainValues = value; }
        }

        public AcquisitionSetup()
        {
            m_fringeSaversParameters = new List<GenericAcquisitionSetup>();
            m_reflectivitySaversParameters = new List<GenericAcquisitionSetup>();
        }

        private bool m_bDoCycling;

        public bool DoCycling
        {
            get { return m_bDoCycling; }
            set { m_bDoCycling = value; }
        }

        private int m_iCyclingQty;

        public int CyclingCount
        {
            get { return m_iCyclingQty; }
            set { m_iCyclingQty = value; }
        }

        private bool m_bDoReflectivity;

        public bool DoReflectivity
        {
            get { return m_bDoReflectivity; }
            set { m_bDoReflectivity = value; }
        }

        public static AcquisitionSetup Deserialize(String XmlFilePath)
        {
            AcquisitionSetup St = new AcquisitionSetup();
            XmlSerializer Serializer = new XmlSerializer(typeof(AcquisitionSetup));
            StreamReader SR = new StreamReader(XmlFilePath);
            try
            {
                St = (AcquisitionSetup)Serializer.Deserialize(SR);
            }
            catch
            {
                return null;
            }
            finally
            {
                SR.Close();
            }
            return St;
        }

        public bool Serialize(String XmlFilePath)
        {
            XmlSerializer Serializer = new XmlSerializer(typeof(AcquisitionSetup));
            StreamWriter SW = new StreamWriter(XmlFilePath);
            try
            {
                Serializer.Serialize(SW, this);
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
            finally
            {
                SW.Close();
            }
        }

        #region ICloneable Membres

        public object Clone()
        {
            // WARNING !!! All sub objects aren't cloned !
            AcquisitionSetup Setup = new AcquisitionSetup();
            Setup.CyclingCount = CyclingCount;
            Setup.DoCycling = DoCycling;
            Setup.ExposureAndGainValues = ExposureAndGainValues;
            Setup.Fringes = Fringes;
            Setup.DoReflectivity = DoReflectivity;
            Setup.DoFringes = DoFringes;
            Setup.DoPSDCal = DoPSDCal;
            Setup.SensorID = SensorID;
            Setup.ImageDynamic = ImageDynamic;
            //Setup.SaversParameters = SaversParameters;
            foreach (GenericAcquisitionSetup St in FringeSaversParameters)
            {
                switch (St.Type)
                {
                    case enumSaverType.esRawImages:
                        Setup.FringeSaversParameters.Add((BaseAcquisitionSetup)((BaseAcquisitionSetup)St).Clone());
                        break;
                    case enumSaverType.esCxAx:
                        Setup.FringeSaversParameters.Add((CxAxAcquisitionSetup)((CxAxAcquisitionSetup)St).Clone());
                        break;
                    case enumSaverType.esReflectivity:
                        Setup.ReflectivitySaversParameters.Add((ReflectivityAcquisitionSetup)((ReflectivityAcquisitionSetup)St).Clone());
                        break;
                }
            }
            foreach (GenericAcquisitionSetup St in ReflectivitySaversParameters)
            {
                switch (St.Type)
                {
                    case enumSaverType.esReflectivity:
                        Setup.ReflectivitySaversParameters.Add((ReflectivityAcquisitionSetup)((ReflectivityAcquisitionSetup)St).Clone());
                        break;
                }
            }
            Setup.StabilizationTime = StabilizationTime;
            return Setup;
        }

        #endregion
        private bool m_bDoFringes;
        public bool DoFringes { get { return m_bDoFringes; } set { m_bDoFringes = value; } }
    }
}
