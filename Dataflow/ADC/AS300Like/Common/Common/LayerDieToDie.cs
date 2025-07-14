using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Common.SocketMessage;

namespace Common
{
    //public static class DeSerializerDieInfoInPixelsList
    //{
    //    public static CDieInfoInPixelsList DeSerializeObject(string filename)
    //    {
    //        CDieInfoInPixelsList objectToSerialize = new CDieInfoInPixelsList();
    //        Stream stream = File.Open(filename, FileMode.Open);
    //        XmlSerializer bFormatter = new XmlSerializer(objectToSerialize.GetType());
    //        objectToSerialize = (CDieInfoInPixelsList)bFormatter.Deserialize(stream);
    //        stream.Close();
    //        return objectToSerialize;
    //    }

    //}

    //[Serializable]
    //public class CDieInfoInPixelsList
    //{
    //    private List<CDieInfoInPixels> m_DieInfoInPixelsList = new List<CDieInfoInPixels>();

    //    public List<CDieInfoInPixels> DieInfoInPixelsList { get { return m_DieInfoInPixelsList; } }

    //    public CDieInfoInPixelsList()
    //    {
    //    }



    //}
    //[Serializable]

    //public class CDieInfoInPixels
    //{
    //    public int IndexX { get; set; }
    //    public int IndexY { get; set; }

    //    public String IndexX_s { get; set; }
    //    public String IndexY_s { get; set; }

    //    public int PositionX { get; set; }
    //    public int PositionY { get; set; }

    //    public int SizeX { get; set; }
    //    public int SizeY { get; set; }

    //    public CDieInfoInPixels()
    //    {
    //    }
    //}
    public class CLayerDieToDie
    {
        public class CDieInfoInPixels
        {
            public int IndexX { get; set; }
            public int IndexY { get; set; }

            public String IndexX_s { get; set; }
            public String IndexY_s { get; set; }

            public int PositionX { get; set; }
            public int PositionY { get; set; }

            public int SizeX { get; set; }
            public int SizeY { get; set; }

            public CDieInfoInPixels()
            {
            }
        }

        public class CDescriptorDie
        {

            // Index Die
            int m_iDieXIndex;
            int m_iDieYIndex;
            public int DieXIndex { get { return m_iDieXIndex; } }
            public int DieYIndex { get { return m_iDieYIndex; } }

            // Position Die
            double m_dPositionXum;
            double m_dPositionYum;
            public double PositionXum { get { return m_dPositionXum; } set { m_dPositionXum = value; } }
            public double PositionYum { get { return m_dPositionYum; } set { m_dPositionYum = value; } }            

            // Size Die                 
            double m_dDieSizeXum;
            double m_dDieSizeYum;
            public double DieSizeXum { get { return m_dDieSizeXum; } }
            public double DieSizeYum { get { return m_dDieSizeYum; } }

            public CDescriptorDie(double pPixelSizeX, double pPixelSizeY, CDieInfoInPixels pDieInfoPx, PointF pDelta)
            {
                // Index position
                m_iDieXIndex = pDieInfoPx.IndexX;
                m_iDieYIndex = pDieInfoPx.IndexY;
                // Position est expripmé en um dans repere orthonormé direct XY. Mais les données de DieInfo sont des données absolues dans un repere indirect => donc on applique un - pour opposer les données de l'axe Y
                // On utilise les données de dieInfo pour connaitre l'espacement relatif à la die origine que l'on convertit en um et on ajoute à la position de la die origine pour obtenir la position absolue de chaque die
                //m_dPositionXum = pDieOriginXum + Math.Sign(pDieInfoPx.IndexX) * (pDieInfoPx.PositionX - pDieOriginXPx) * m_dPixelSizeX;
                //m_dPositionYum = pDieOriginYum + Math.Sign(pDieInfoPx.IndexY) * (pDieInfoPx.PositionY - pDieOriginYPx) * m_dPixelSizeY + pDieInfoPx.SizeY * m_dPixelSizeY;
                m_dPositionXum = pDieInfoPx.PositionX * pPixelSizeX + pDelta.X;
                m_dPositionYum = (pDieInfoPx.PositionY + pDieInfoPx.SizeY) * pPixelSizeY + pDelta.Y;
                
                // Size in um
                m_dDieSizeXum = pDieInfoPx.SizeX * pPixelSizeX;
                m_dDieSizeYum = pDieInfoPx.SizeY * pPixelSizeY;
            }
            public CDescriptorDie(int pDieOriginXum, int pDieOriginYum, Point pDiesIndexes, PointF pDiePitchSize)
            {
                // Index position
                m_iDieXIndex = pDiesIndexes.X;
                m_iDieYIndex = pDiesIndexes.Y;
                // Position est expripmé en um dans repere orthonormé direct XY. Mais les données de DieInfo sont des données absolues dans un repere indirect => donc on applique un - pour opposer les données de l'axe Y
                // On utilise les données de dieInfo pour connaitre l'espacement relatif à la die origine que l'on convertit en um et on ajoute à la position de la die origine pour obtenir la position absolue de chaque die
                m_dPositionXum = pDieOriginXum + Math.Sign(m_iDieXIndex) * Math.Abs(m_iDieXIndex)  * pDiePitchSize.X;
                m_dPositionYum = pDieOriginYum + Math.Sign(m_iDieYIndex) * Math.Abs(m_iDieYIndex) * pDiePitchSize.Y;
                // Size in um no notion DieSize
                m_dDieSizeXum = pDiePitchSize.X ;
                m_dDieSizeYum = pDiePitchSize.Y ;
            }                   
        }

        //------------------
        #region Members
        List<CDescriptorDie> m_lsDieLayout = new List<CDescriptorDie>();

        int m_iMinDieIndex_X=0;
        int m_iMaxDieIndex_X=0; // not used

        int m_iMinDieIndex_Y=0; // not used
        int m_iMaxDieIndex_Y=0; 

        int m_iXNumberOfDie=0;    // not used
        int m_iYNumberOfDie=0;    // not used
        
        public int getNumberOfDie { get { return m_lsDieLayout.Count; } }
        public int getNumberOfDie_X { get { return m_iXNumberOfDie; } }  // not used
        public int getNumberOfDie_Y { get { return m_iYNumberOfDie; } }  // not used
    
        public int getMinDieXIndex { get { return m_iMinDieIndex_X; } } // cf LoadInfoBloc (calcul virtual index)
        public int getMinDieYIndex { get { return m_iMinDieIndex_Y; } } // not used

        public int getMaxDieXIndex { get { return m_iMaxDieIndex_X; } } // not used
        public int getMaxDieYIndex { get { return m_iMaxDieIndex_Y; } } // cf LoadInfoBloc (calcul virtual index)

        //public double DieOriginPosX_um { get; set; }
        //public double DieOriginPosY_um { get; set; }
        private enumDieOriginReference m_DieOriginPosReference;
        private String m_XMLFilePathName = String.Empty;
        private List<Point> m_DiesIndexes = null;
        private PointF m_DiePitchSize ;
        private PointF m_SampleCenter; // unity in micron
        private PointF m_DieOriginPos; //unity in micron
        private PointF m_RationPixtToMicronForXML;
        private int m_WaferSize;

        
        #endregion
        public CLayerDieToDie()
        {

        }
         ~CLayerDieToDie()
        {
            lstDieLayout.Clear();
            if (m_DiesIndexes != null)
                m_DiesIndexes.Clear();
        }
        public List<CDescriptorDie> lstDieLayout { get { return m_lsDieLayout; } }
        public void SetParametersForXMLFile(PointF pDieOriginPos, PointF pRationPixelToMicron, String pXMLFilePathName, int pWaferSize, PointF pSampleCenter, enumDieOriginReference pDieOriginPosReference)
        {
            m_DieOriginPos = pDieOriginPos;
            m_RationPixtToMicronForXML = pRationPixelToMicron;
            m_XMLFilePathName = pXMLFilePathName;
            m_WaferSize = pWaferSize;
            m_SampleCenter = pSampleCenter;
            m_DieOriginPosReference = pDieOriginPosReference;

            m_DiesIndexes = null;
        }
        public void SetParametersWithoutXMLFile(PointF pDieOriginPos, PointF pDiePitchSize, List<Point> pDiesIndexes, int pWaferSize, PointF pSampleCenter, enumDieOriginReference pDieOriginPosReference)
        {
            m_DieOriginPos = pDieOriginPos;
            m_DiePitchSize = pDiePitchSize;
            m_DiesIndexes = pDiesIndexes;
            m_WaferSize = pWaferSize;
            m_SampleCenter = pSampleCenter;
            m_DieOriginPosReference = pDieOriginPosReference;

            m_XMLFilePathName = String.Empty;
        }
        //==================================================================================
        // Result : Dies liste with dimension unity micron, coordinate origin is (Left, Top)
        //==================================================================================
        public List<CDescriptorDie> BuildingListDies()
        {
            if (m_DieOriginPosReference == enumDieOriginReference.FromCoordinatesOrigin)
            {
                m_DieOriginPos.X = m_DieOriginPos.X - m_SampleCenter.X;
                m_DieOriginPos.Y = m_DieOriginPos.Y - m_SampleCenter.Y;

            }
            double lHalfWaferSize = (double)(m_WaferSize / 2);

            m_lsDieLayout.Clear();
            if ((m_XMLFilePathName == String.Empty) && (m_DiesIndexes != null))
            {
                //Construir nous même les Dies
               
                for (int i = 0; i < m_DiesIndexes.Count; i++)
                {
                    CDescriptorDie lNewDie = new CDescriptorDie((int)m_DieOriginPos.X, (int)m_DieOriginPos.Y, m_DiesIndexes[i], m_DiePitchSize);
                    //================================================================================
                    // Change coordinate origin to (Left, Top), the coordinates is (Left, Down) of Die
                    //================================================================================
                    lNewDie.PositionXum = (double)(lNewDie.PositionXum + lHalfWaferSize);
                    lNewDie.PositionYum = (double)(lHalfWaferSize - lNewDie.PositionYum);

                    m_lsDieLayout.Add(lNewDie);

                }

            }
            else
            {
                List<CLayerDieToDie.CDieInfoInPixels> lDieInfoPxList = CLayerDietToDieFile.ReaderXmlFile(m_XMLFilePathName);

                CDieInfoInPixels lDieOrigin = null;
                for (int i = 0; i < lDieInfoPxList.Count; i++)
                {
                    if ((lDieInfoPxList[i].IndexX == 0) && (lDieInfoPxList[i].IndexY == 0))
                    {
                        lDieOrigin = lDieInfoPxList[i];
                        break;
                    }
                }
                //Calcul the delta between the doordinate
                PointF Delta = new PointF(); 
                Delta.X = (float)((lHalfWaferSize + m_DieOriginPos.X ) - (lDieOrigin.PositionX * m_RationPixtToMicronForXML.X));
                Delta.Y = (float)((lHalfWaferSize - m_DieOriginPos.Y) - (lDieOrigin.PositionY * m_RationPixtToMicronForXML.Y));
                for (int i = 0; i < lDieInfoPxList.Count; i++)
                {
                    CDescriptorDie lNewDie = new CDescriptorDie(m_RationPixtToMicronForXML.X, m_RationPixtToMicronForXML.Y, lDieInfoPxList[i], Delta);
                    
                    m_lsDieLayout.Add(lNewDie);
                   
                }
                lDieInfoPxList.Clear();
            }
            
            return m_lsDieLayout;
        }
        //--------------------------------------------------------
        //public CLayerDieToDie(double pDieOriginXum, double pDieOriginYum, double pPixelSizeX, double pPixelSizeY, String pFilePathName, List<Point> pDiesIndexes, PointF pDiePitchSize)
        //{
        //    if (pFilePathName == String.Empty)
        //    {
        //        //Construir nous même les Dies
        //        m_lsDieLayout.Clear();
        //        for (int i = 0; i < pDiesIndexes.Count; i++)
        //        {
        //            CDescriptorDie lNewDie = new CDescriptorDie((int)pDieOriginXum, (int)pDieOriginYum, pDiesIndexes[i], pDiePitchSize, pPixelSizeX, pPixelSizeY);
        //            if (m_iMinDieIndex_X > lNewDie.DieXIndex)
        //                m_iMinDieIndex_X = lNewDie.DieXIndex;
        //            if (m_iMinDieIndex_Y > lNewDie.DieYIndex)
        //                m_iMinDieIndex_Y = lNewDie.DieYIndex;
        //            if (m_iMaxDieIndex_X < lNewDie.DieXIndex)
        //                m_iMaxDieIndex_X = lNewDie.DieXIndex;
        //            if (m_iMaxDieIndex_Y < lNewDie.DieYIndex)
        //                m_iMaxDieIndex_Y = lNewDie.DieYIndex;
        //            m_iXNumberOfDie = m_iMaxDieIndex_X - m_iMinDieIndex_X;
        //            m_iYNumberOfDie = m_iMaxDieIndex_Y - m_iMinDieIndex_Y;
        //            m_lsDieLayout.Add(lNewDie);

        //            if ((lNewDie.DieXIndex == 0) && (lNewDie.DieYIndex == 0))
        //            {
        //                DieOriginPosX_um = lNewDie.PositionXum;
        //                DieOriginPosY_um = lNewDie.PositionYum;
        //            }
        //        }

        //    }
        //    else
        //    {
        //        List<CLayerDieToDie.CDieInfoInPixels> lDieInfoPxList = CLayerDietToDieFile.ReaderXmlFile(pFilePathName);

        //        CDieInfoInPixels lDieOrigin = null;
        //        for (int i = 0; i < lDieInfoPxList.Count; i++)
        //        {
        //            if ((lDieInfoPxList[i].IndexX == 0) && (lDieInfoPxList[i].IndexY == 0))
        //            {
        //                lDieOrigin = lDieInfoPxList[i];
        //                break;
        //            }
        //        }

        //        for (int i = 0; i < lDieInfoPxList.Count; i++)
        //        {
        //            CDescriptorDie lNewDie = new CDescriptorDie(pDieOriginXum, pDieOriginYum, lDieOrigin.PositionX, lDieOrigin.PositionY, pPixelSizeX, pPixelSizeY, lDieInfoPxList[i]);
        //            if (m_iMinDieIndex_X > lNewDie.DieXIndex)
        //                m_iMinDieIndex_X = lNewDie.DieXIndex;
        //            if (m_iMinDieIndex_Y > lNewDie.DieYIndex)
        //                m_iMinDieIndex_Y = lNewDie.DieYIndex;
        //            if (m_iMaxDieIndex_X < lNewDie.DieXIndex)
        //                m_iMaxDieIndex_X = lNewDie.DieXIndex;
        //            if (m_iMaxDieIndex_Y < lNewDie.DieYIndex)
        //                m_iMaxDieIndex_Y = lNewDie.DieYIndex;
        //            m_iXNumberOfDie = m_iMaxDieIndex_X - m_iMinDieIndex_X;
        //            m_iYNumberOfDie = m_iMaxDieIndex_Y - m_iMinDieIndex_Y;
        //            m_lsDieLayout.Add(lNewDie);

        //            if ((lNewDie.DieXIndex == 0) && (lNewDie.DieYIndex == 0))
        //            {
        //                DieOriginPosX_um = lNewDie.PositionXum;
        //                DieOriginPosY_um = lNewDie.PositionYum;
        //            }
        //        }
        //    }
        //}        
        //--------------------------------------------------------
        public CDescriptorDie GetDieDescriptor(int pDieXIndex, int pDieYIndex)
        {
            for (int i = 0; i < m_lsDieLayout.Count; i++)
            {
                if ((m_lsDieLayout[i].DieXIndex == pDieXIndex) && (m_lsDieLayout[i].DieYIndex == pDieYIndex))
                {
                    return m_lsDieLayout[i];
                }
            }
            return null;
        }
        public CDescriptorDie GetDieDescriptor(int pIndex)
        {
            return m_lsDieLayout[pIndex];
        }

        public Rectangle GetLimits()
        {
            double lX1Pos_BottomRight = -1999999999;
            double lY1Pos_BottomRight = -1999999999;
            double lX2Pos_TopLeft = 1999999999;
            double lY2Pos_TopLeft = 1999999999;

            for (int i = 0; i < m_lsDieLayout.Count; i++)
			{
                if (lX1Pos_BottomRight < m_lsDieLayout[i].PositionXum + m_lsDieLayout[i].DieSizeXum)
                    lX1Pos_BottomRight = m_lsDieLayout[i].PositionXum + m_lsDieLayout[i].DieSizeXum;
                if (lY1Pos_BottomRight < m_lsDieLayout[i].PositionYum + m_lsDieLayout[i].DieSizeYum)
                    lY1Pos_BottomRight = m_lsDieLayout[i].PositionYum + m_lsDieLayout[i].DieSizeYum;
                if (lX2Pos_TopLeft > m_lsDieLayout[i].PositionXum + m_lsDieLayout[i].DieSizeXum)
                    lX2Pos_TopLeft = m_lsDieLayout[i].PositionXum + m_lsDieLayout[i].DieSizeXum;
                if (lY2Pos_TopLeft > m_lsDieLayout[i].PositionYum + m_lsDieLayout[i].DieSizeYum)
                    lY2Pos_TopLeft = m_lsDieLayout[i].PositionYum + m_lsDieLayout[i].DieSizeYum;
			}
            return new Rectangle(Convert.ToInt32(lX2Pos_TopLeft), Convert.ToInt32(lY2Pos_TopLeft), Convert.ToInt32(lX1Pos_BottomRight - lX2Pos_TopLeft), Convert.ToInt32(lY1Pos_BottomRight - lY2Pos_TopLeft));
            
        }
    }

    public static class CLayerDietToDieFile
    {

        //------------------------------------------------
        public static List<CLayerDieToDie.CDieInfoInPixels> ReaderXmlFile(String pDieToDieFileName)
        {
            List<CLayerDieToDie.CDieInfoInPixels> lDieToDieList = new List<CLayerDieToDie.CDieInfoInPixels>();

            XmlDocument doc = new XmlDocument();
            doc.Load(pDieToDieFileName);

            XmlNodeList subnodelist = doc.SelectNodes("//root");

            foreach (XmlNode node in subnodelist)
            {
                LoadDieListFromXML(node, ref lDieToDieList);
            }
            return lDieToDieList;
        }
        //=================================================================
        // Get a value from an XML subnode
        //=================================================================
        public static string getXmlStringValue(XmlNode node, String field)
        {
            XmlNode subnode = node.SelectSingleNode(".//" + field);
            if (subnode == null)
                throw new ApplicationException("Missing field " + field);

            XmlNode valuenode = subnode.Attributes.GetNamedItem("value");
            if (valuenode == null)
                throw new ApplicationException("Missing value in field " + field);

            return valuenode.Value;
        }
        //=================================================================
        // Get a value from an XML subnode
        //=================================================================
        public static int getXmlValue(XmlNode node, String field)
        {
            string str = getXmlStringValue(node, field);

            int valueInt;
            if (!int.TryParse(str, out valueInt))
                throw new ApplicationException("Invalid value in field " + field);

            return valueInt;
        }
        //=================================================================
        // Load from the XML config file a list of dies to extract
        //=================================================================
        public static void LoadDieListFromXML(XmlNode node, ref List<CLayerDieToDie.CDieInfoInPixels> pDieToDieList)
        {
            //-------------------------------------------------------------
            // Get all <die> items in XML
            //-------------------------------------------------------------
            XmlNodeList xmlnodelist = node.SelectNodes(".//Die");
            if (xmlnodelist.Count == 0)
                throw new ApplicationException("No dies to output");

            //-------------------------------------------------------------
            // Create the list of dies
            //-------------------------------------------------------------
            foreach (XmlNode xmldienode in xmlnodelist)
            {
                LoadDieFromXML(xmldienode, ref pDieToDieList);
            }
        }
        //=================================================================
        // 
        //=================================================================
        public static bool LoadDieFromXML(XmlNode node, ref List<CLayerDieToDie.CDieInfoInPixels> pDieToDieList)
        {
            //-------------------------------------------------------------
            // Read values in XML
            //-------------------------------------------------------------
            CLayerDieToDie.CDieInfoInPixels pDieInfo = new CLayerDieToDie.CDieInfoInPixels();

            pDieInfo.PositionX = getXmlValue(node, "X");
            pDieInfo.PositionY = getXmlValue(node, "Y"); 

            pDieInfo.SizeX = getXmlValue(node, "W"); 
            pDieInfo.SizeY = getXmlValue(node, "H"); 

            pDieInfo.IndexX_s = getXmlStringValue(node, "IndexX");
            pDieInfo.IndexY_s = getXmlStringValue(node, "IndexY");

            pDieInfo.IndexX = Convert.ToInt32(pDieInfo.IndexX_s);
            pDieInfo.IndexY = Convert.ToInt32(pDieInfo.IndexY_s);

            pDieToDieList.Add(pDieInfo);

            return true;
        }


    }
}
