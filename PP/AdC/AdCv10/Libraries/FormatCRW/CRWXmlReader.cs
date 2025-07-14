using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace FormatCRW
{
    public class CRWXmlReader
    {
        protected List<ProfilScan> m_vProfilScan;
        protected List<DataMeasure> m_vDataMeasures;

        public List<ProfilScan> ProfilScans { get { return m_vProfilScan; } }
        public List<DataMeasure> DataMeasures { get { return m_vDataMeasures; } }

        private string m_sLastErrorMsg = "";

        public CRWXmlReader(string sXmlCRWFilePath)
        {
            m_vProfilScan = new List<ProfilScan>();
            m_vDataMeasures = new List<DataMeasure>();

            if (false == Load(sXmlCRWFilePath))
                throw new SystemException(String.Format("CRWXmlReader Exception : Could not Load CRW File <{0}> \n[{1}]>", sXmlCRWFilePath, m_sLastErrorMsg));
        }

        public bool Load(string sXmlCRWFilePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(sXmlCRWFilePath);

                // Init ProfilScan List
                XmlNode ProfilesNode = doc.SelectSingleNode(".//Profiles");
                if (ProfilesNode != null)
                {
                    XmlNodeList ScanNodeList = ProfilesNode.SelectNodes("scan");
                    foreach (XmlNode ScanNode in ScanNodeList)
                    {
                        ProfilScan CurrentScanProfil = new ProfilScan();
                        CurrentScanProfil.sLabel = "";
                        CurrentScanProfil.cColor = Color.Black;
                        CurrentScanProfil.DeltaList = new List<double>();

                        XmlNode LableNode = ScanNode.SelectSingleNode(".//Label");
                        if (LableNode != null)
                            CurrentScanProfil.sLabel = LableNode.InnerText.Trim();

                        XmlNode ColorNode = ScanNode.SelectSingleNode(".//color");
                        if (ColorNode != null)
                        {
                            int r = Convert.ToInt32(ColorNode.SelectSingleNode(".//R").InnerText);
                            int g = Convert.ToInt32(ColorNode.SelectSingleNode(".//G").InnerText);
                            int b = Convert.ToInt32(ColorNode.SelectSingleNode(".//B").InnerText);
                            CurrentScanProfil.cColor = Color.FromArgb(r, g, b);
                        }

                        XmlNodeList DeltaNodeList = ScanNode.SelectNodes("Deltaum");
                        foreach (XmlNode deltanode in DeltaNodeList)
                        {
                            double dDeltaum = Convert.ToDouble(deltanode.InnerText.Trim());
                            CurrentScanProfil.DeltaList.Add(dDeltaum);
                        }

                        m_vProfilScan.Add(CurrentScanProfil);
                    }
                }

                // Init DataMeasure List
                XmlNode DataNode = doc.SelectSingleNode(".//Data");
                if (DataNode != null)
                {

                    XmlNodeList MeasureNodeList = DataNode.SelectNodes("measure");
                    foreach (XmlNode MeasureNode in MeasureNodeList)
                    {
                        DataMeasure CurrentData = new DataMeasure();
                        CurrentData.sLabel = "";
                        CurrentData.sValue = "";

                        XmlNode lableNode = MeasureNode.SelectSingleNode(".//label");
                        if (lableNode != null)
                            CurrentData.sLabel = lableNode.InnerText.Trim();

                        XmlNode valueNode = MeasureNode.SelectSingleNode(".//value");
                        if (valueNode != null)
                            CurrentData.sValue = valueNode.InnerText.Trim();

                        m_vDataMeasures.Add(CurrentData);
                    }
                }
            }
            catch (System.Exception ex)
            {
                String sMsg = ex.Message;
                m_sLastErrorMsg = sMsg;
                return false;
            }

            return true;
        }
    }
}
