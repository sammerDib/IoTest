using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace UnitySC.PM.LIGHTSPEED.Data
{
    [Serializable]
    public class PtAttenuation : IComparable
    {
        [XmlAttribute("dB")]
        public double dB { get; set; }

        [XmlAttribute("Angle")]
        public double angle_dg { get; set; }

        public PtAttenuation()
        {
            dB = 0.0;
            angle_dg = 0.0;
        }

        public PtAttenuation(double lAngle_dg, double ldB)
        {
            angle_dg = lAngle_dg;
            dB = ldB;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            PtAttenuation pt = obj as PtAttenuation;
            if (pt != null)
                return this.dB.CompareTo(pt.dB);
            else
                throw new ArgumentException("Object is not a PtAtt");
        }
    }

    [Serializable]
    public class LutAttenuation
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlArray("Lut")]
        [System.Xml.Serialization.XmlArrayItem("PtAtt")]
        public List<PtAttenuation> Lut { get; set; } = new List<PtAttenuation>();

        [XmlIgnore]
        private bool _isIncreasing = false;

        public LutAttenuation()
        {
            Name = String.Empty;
        }

        public LutAttenuation(string lName)
        {
            Name = lName;
        }

        public void Init()
        {
            if (Lut != null)
            {
                Lut.Sort();

                if (Lut.Count > 1)
                    _isIncreasing = (Lut.Last().angle_dg - Lut.First().angle_dg) > 0.0;
            }
        }

        /// <summary>
        /// Attenuation range (after Init()).
        /// </summary>
        public double StrongestAttenuation_dB => Lut[0].dB;

        public double LightestAttenuation_dB => Lut.Last().dB;
        public double StrongestAttenuationFactor => Math.Pow(10d, StrongestAttenuation_dB / 10d);
        public double LightestAttenuationFactor => Math.Pow(10d, LightestAttenuation_dB / 10d);

        public double FindAngle(double dTargetAttenuation_dB)
        {
            double fx = 0.0;
            int rk = Lut.BinarySearch(new PtAttenuation(0.0, dTargetAttenuation_dB));
            if (rk < 0)
            {
                // not found --
                int nx1 = ~rk;
                if (nx1 == 0)
                {
                    // Min Extrema
                    fx = Lut.FirstOrDefault().angle_dg;
                }
                else if (nx1 >= Lut.Count)
                {
                    // Max Extrema
                    fx = Lut.Last().angle_dg;
                }
                else
                {
                    // interpolate
                    int nx = (~rk) - 1;
                    double xp = Lut[nx].dB;
                    double xp1 = Lut[nx1].dB;
                    double yp = Lut[nx].angle_dg;
                    double yp1 = Lut[nx1].angle_dg;
                    fx = yp + (yp1 - yp) * (dTargetAttenuation_dB - xp) / (xp1 - xp);
                }
            }
            else
            {
                fx = Lut[rk].angle_dg;
            }
            return fx;
        }

        public double GetMid_dB()
        {
            return Lut[((int)(Lut.Count / 2))].dB;
        }
    }

    [Serializable]
    public class DataLutAttenuation
    {
        [System.Xml.Serialization.XmlArray("Data")]
        [System.Xml.Serialization.XmlArrayItem("LutAttenuation")]
        public List<LutAttenuation> Data { get; set; } = new List<LutAttenuation>();

        public DataLutAttenuation()
        {
        }

        public void Init()
        {
            foreach (var lut in Data)
                lut.Init();
        }

        public static DataLutAttenuation LoadData(String sXmlFilePath)
        {
            using (FileStream fs = new FileStream(sXmlFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (DataLutAttenuation)new XmlSerializer(typeof(DataLutAttenuation)).Deserialize(fs);
            }
        }

        public bool SaveData(string sXmlFilePath)
        {
            bool bSuccess = true;
            try
            {
                // sort here in case ...
                Init();

                Directory.CreateDirectory(Path.GetDirectoryName(sXmlFilePath));
                using (FileStream fs = new FileStream(sXmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    new XmlSerializer(typeof(DataLutAttenuation)).Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                bSuccess = false;
                string msg = ex.Message;
                Debug.WriteLine(msg);
            }
            return bSuccess;
        }

        public LutAttenuation GetLut(string lName)
        {
            var lut = Data.FirstOrDefault(x => x.Name == lName);
            if (lut == null || String.IsNullOrEmpty(lut.Name))
                throw new Exception($"Could not find data lut <{lName}>");

            return lut;
        }

        public double FindAngle(string lName, double dTargetAttenuation_dB)
        {
            return GetLut(lName).FindAngle(dTargetAttenuation_dB);
        }

        public double GetMid_dB(string lName)
        {
            return GetLut(lName).GetMid_dB();
        }
    }
}
