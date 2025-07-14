using System;
using System.Collections.Generic;
using System.Xml;

namespace FormatCRW
{
    public class CRWXmlWriter
    {

        protected List<ProfilScan> m_vProfilScan;
        protected List<DataMeasure> m_vDataMeasures;

        public CRWXmlWriter()
        {
            m_vProfilScan = new List<ProfilScan>();
            m_vDataMeasures = new List<DataMeasure>();
        }

        public CRWXmlWriter(List<ProfilScan> p_vProfilScan, List<DataMeasure> p_vDataMeasures)
        {
            m_vProfilScan = p_vProfilScan;
            m_vDataMeasures = p_vDataMeasures;
        }

        public void AddProfil(ProfilScan oScan)
        {
            m_vProfilScan.Add(oScan);
        }

        public void AddData(DataMeasure oMeasure)
        {
            m_vDataMeasures.Add(oMeasure);
        }

        public bool Save(string sXmlCRWFilePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                //(1) the xml declaration is recommended, but not mandatory
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                //ADD root element
                XmlElement crwRootElement = doc.CreateElement(string.Empty, "crw", string.Empty);
                crwRootElement.SetAttribute("version", "v1.0.0");
                doc.AppendChild(crwRootElement);

                // Profiles 
                XmlElement ProfilesElement = doc.CreateElement(string.Empty, "Profiles", string.Empty);
                crwRootElement.AppendChild(ProfilesElement);
                foreach (ProfilScan scan in m_vProfilScan)
                {
                    XmlElement ScanElement = doc.CreateElement("scan");

                    // Labels
                    XmlElement LabelElt = doc.CreateElement("Label");
                    XmlText nodeLabel = doc.CreateTextNode(scan.sLabel);
                    LabelElt.AppendChild(nodeLabel);
                    ScanElement.AppendChild(LabelElt);

                    // Colors
                    XmlElement EltColor = doc.CreateElement("color");
                    XmlElement EltR = doc.CreateElement("R"); EltR.InnerText = scan.cColor.R.ToString();
                    XmlElement EltG = doc.CreateElement("G"); EltG.InnerText = scan.cColor.G.ToString();
                    XmlElement EltB = doc.CreateElement("B"); EltB.InnerText = scan.cColor.B.ToString();
                    EltColor.AppendChild(EltR);
                    EltColor.AppendChild(EltG);
                    EltColor.AppendChild(EltB);
                    ScanElement.AppendChild(EltColor);

                    //Deltaum List
                    XmlElement EltDeltaum = null;
                    foreach (double dDeltaum in scan.DeltaList)
                    {
                        EltDeltaum = doc.CreateElement("Deltaum");
                        EltDeltaum.InnerText = dDeltaum.ToString("#0.00");
                        ScanElement.AppendChild(EltDeltaum);
                    }

                    ProfilesElement.AppendChild(ScanElement);
                }

                // Data
                XmlElement DataElement = doc.CreateElement(string.Empty, "Data", string.Empty);
                crwRootElement.AppendChild(DataElement);
                foreach (DataMeasure measure in m_vDataMeasures)
                {
                    XmlElement measureElt = doc.CreateElement("measure");
                    XmlElement EltLabel = doc.CreateElement("label"); EltLabel.InnerText = measure.sLabel;
                    XmlElement EltValue = doc.CreateElement("value"); EltValue.InnerText = measure.sValue;
                    measureElt.AppendChild(EltLabel);
                    measureElt.AppendChild(EltValue);
                    DataElement.AppendChild(measureElt);
                }

                // Save XML File
                doc.Save(sXmlCRWFilePath);

            }
            catch (System.Exception ex)
            {
                String sMsg = ex.Message;
                return false;
            }

            return true;
        }

    }
}
