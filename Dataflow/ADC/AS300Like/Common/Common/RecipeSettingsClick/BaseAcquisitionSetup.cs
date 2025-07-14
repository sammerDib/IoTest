using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.Acquisition
{
    
    public class BaseAcquisitionSetup : GenericAcquisitionSetup,ICloneable
    {
        public BaseAcquisitionSetup():base()
        {
      
        }
        
        private String m_sBaseFileName;

        public String BaseFileName
        {
            get { return m_sBaseFileName; }
            set { m_sBaseFileName = value; }
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public string summary()
        {
            return "Output Folder : " + FolderPath + "\r\nBase FileName : " + m_sBaseFileName;
        }

        private int m_iNbImgX;

        public int NbImgX
        {
            get { return m_iNbImgX; }
            set { m_iNbImgX = value; }
        }
        private int m_iNbImgY;

        public int NbImgY
        {
            get { return m_iNbImgY; }
            set { m_iNbImgY = value; }
        }
        private int m_iWaferID;

        public int WaferID
        {
            get { return m_iWaferID; }
            set { m_iWaferID = value; }
        }

        private int m_iPixelNumber;

        public int PixelNumber
        {
            get { return m_iPixelNumber; }
            set { m_iPixelNumber = value; }
        }


        #region ICloneable Membres

        public object Clone()
        {
            BaseAcquisitionSetup Setup = new BaseAcquisitionSetup();
            Setup.BaseFileName = BaseFileName;
            Setup.FolderPath = FolderPath;
            Setup.NbImgX = NbImgX;
            Setup.NbImgY = NbImgY;
            Setup.PixelNumber = PixelNumber;
            Setup.SizeX = SizeX;
            Setup.SizeY = SizeY;
            Setup.Type = Type;
            Setup.WaferID = WaferID;
            return Setup;
        }

        #endregion
    }
}
