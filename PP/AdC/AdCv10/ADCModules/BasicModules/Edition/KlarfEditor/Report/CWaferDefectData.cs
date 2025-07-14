using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace BasicModules.KlarfEditor
{
    public class CWaferDefectData
    {
        private int m_ID;
        private int m_XIndex;
        private int m_YIndex;
        private int m_XRelatif;
        private int m_YRelatif;
        private double m_XSize;
        private double m_YSize;
        private double m_DefectArea;
        private double m_DiameterSize;
        private int m_ClusterNumber;
        private int m_RoughBinNumber;
        private CImageData m_ImageData;
        private Random m_rnd = new Random();
        private int m_WaferSize = 0;
        private PointF m_DiePitch;
        private PointF m_DieOrigin;
        private bool m_bUsingDieToDie;
        private Point m_NbDies = new Point(0, 0);


        public CWaferDefectData(int pWaferSize, String pWaferData, CWaferParameters pWaferParameters)
        {
            m_WaferSize = pWaferSize;
            m_DiePitch = pWaferParameters.DiePitch;
            m_DieOrigin = pWaferParameters.DieOrigin;
            m_NbDies.X = (int)Math.Round((double)(m_WaferSize / m_DiePitch.X), MidpointRounding.AwayFromZero);
            m_NbDies.Y = (int)Math.Round((double)(m_WaferSize / m_DiePitch.Y), MidpointRounding.AwayFromZero);
            m_bUsingDieToDie = (pWaferParameters.DiesIndexes.Count > 1);

            String[] lSeparator = new String[] { " " };
            String[] sTab = pWaferData.Trim().Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
            int iDataOrder = 0;
            for (int i = 0; i < pWaferParameters.ColumnNameList.Count; i++)
            {
                if (sTab[i].Length > 0)
                {
                    switch (pWaferParameters.ColumnNameList[i])
                    {
                        case "DEFECTID": m_ID = Convert.ToInt32(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "XREL": m_XRelatif = Convert.ToInt32(Math.Floor(Convert.ToDouble(sTab[i], CultureInfo.InvariantCulture.NumberFormat)), CultureInfo.InvariantCulture.NumberFormat); break;
                        case "YREL": m_YRelatif = Convert.ToInt32(Math.Floor(Convert.ToDouble(sTab[i], CultureInfo.InvariantCulture.NumberFormat)), CultureInfo.InvariantCulture.NumberFormat); break;
                        case "XSIZE": m_XSize = Convert.ToDouble(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "YSIZE": m_YSize = Convert.ToDouble(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "XINDEX": m_XIndex = Convert.ToInt32(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "YINDEX": m_YIndex = Convert.ToInt32(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "DEFECTAREA": m_DefectArea = Convert.ToDouble(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "DSIZE": m_DiameterSize = Convert.ToDouble(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "CLUSTERNUMBER": m_ClusterNumber = Convert.ToInt32(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "ROUGHBINNUMBER": m_RoughBinNumber = Convert.ToInt32(sTab[i], CultureInfo.InvariantCulture.NumberFormat); break;
                        case "IMAGECOUNT":
                            String[] lImageDataArray = new String[sTab.Length - i];
                            for (int j = i; j < sTab.Length; j++)
                            {
                                lImageDataArray[j - i] = sTab[j];
                            }
                            m_ImageData = new CImageData(lImageDataArray);
                            break;
                        default:
                            break;
                    }
                    iDataOrder++;
                }
            }

            //TO DO HUGO à remplir en fct des catégorie de taille ?
            //             m_Color = ; 
            //             m_Label = ;
            //             m_bEnabled = ;
        }

        public static void DeleteEmptyFile(String pFileName)
        {
            if (File.Exists(pFileName))
            {
                FileInfo finfo = new FileInfo(pFileName);
                if (finfo.Length == 0)
                    File.Delete(pFileName);
            }
        }

        public int ID
        {
            get { return m_ID; }
        }
        public int XIndex
        {
            get { return m_XIndex; }
            set { m_XIndex = value; }
        }
        public int YIndex
        {
            get { return m_YIndex; }
            set { m_YIndex = value; }
        }
        public int XRelatif
        {
            get { return m_XRelatif; }
        }

        public int YRelatif
        {
            get { return m_YRelatif; }
        }

        public int XRelatifFromWaferCenter
        {
            get
            {
                if (!m_bUsingDieToDie)
                    return m_XRelatif - Convert.ToInt32(m_WaferSize / 2, CultureInfo.InvariantCulture.NumberFormat);
                else
                {
                    double DiePosX = m_DieOrigin.X + m_XIndex * (double)m_DiePitch.X;
                    return (int)Math.Floor(DiePosX + m_XRelatif);
                }
            }
        }

        public int YRelatifFromWaferCenter
        {
            get
            {
                if (!m_bUsingDieToDie)
                    return m_YRelatif - Convert.ToInt32(m_WaferSize / 2, CultureInfo.InvariantCulture.NumberFormat);
                else
                {
                    double DiePosY = m_DieOrigin.Y + m_YIndex * (double)m_DiePitch.Y;
                    return (int)Math.Floor(DiePosY + m_YRelatif);
                }
            }
        }

        public double XSize
        {
            get { return m_XSize; }
        }


        public double YSize
        {
            get { return m_YSize; }
        }


        public double DefectArea
        {
            get { return m_DefectArea; }
        }


        public double DiameterSize
        {
            get { return m_DiameterSize; }
        }


        public int ClusterNumber
        {
            get { return m_ClusterNumber; }
        }
        public int RoughBinNumber
        {
            get { return m_RoughBinNumber; }
        }
        public CImageData ImageData
        {
            get { return m_ImageData; }
        }

        /// <summary>
        /// Retourne la position relative de X en considérent la taille du wafer à 1
        /// </summary>
        public double GetXPosBetween0and1()
        {
            if (!m_bUsingDieToDie)
            {
                return (double)XRelatif / (double)m_WaferSize;
            }
            else
            {
                return (XRelatifFromWaferCenter + (double)m_WaferSize / 2.0) / (double)m_WaferSize;
            }
        }
        /// <summary>
        /// Retourne la position relative de Y en considérent la taille du wafer à 1
        /// </summary>
        public double GetYPosBetween0and1()
        {
            if (!m_bUsingDieToDie)
            {
                return (double)YRelatif / (double)m_WaferSize;
            }
            else
            {
                return (YRelatifFromWaferCenter + (double)m_WaferSize / 2.0) / (double)m_WaferSize;
            }
        }
    }

    public class CImageData
    {
        private int m_ImageCount;
        private List<int> m_ImageList = new List<int>();

        public CImageData(String[] pImageData)
        {
            for (int i = 0; i < pImageData.Length; i++)
            {
                switch (i)
                {
                    case 0: m_ImageCount = Convert.ToInt32(pImageData.GetValue(i), CultureInfo.InvariantCulture.NumberFormat); break;
                    case 1: break;
                    default:
                        if (pImageData[i] != null)
                        {
                            m_ImageList.Add(Convert.ToInt32(pImageData[i], CultureInfo.InvariantCulture.NumberFormat));
                            i++;
                        }
                        break;
                }
            }
        }
    }

}
