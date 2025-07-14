using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.Acquisition
{
    public class GenericAcquisitionSetup:ICloneable
    {
        private enumSaverType m_type = 0;

        public enumSaverType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private int m_iSizeX;
        [XmlIgnore]
        public int SizeX
        {
            get { return m_iSizeX; }
            set { m_iSizeX = value; }
        }
        private int m_iSizeY;
        [XmlIgnore]
        public int SizeY
        {
            get { return m_iSizeY; }
            set { m_iSizeY = value; }
        }

        private String m_sFolderPath;

        public String FolderPath
        {
            get { return m_sFolderPath; }
            set { m_sFolderPath = value; }
        }

        #region ICloneable Membres

        public object Clone()
        {
            GenericAcquisitionSetup St = new GenericAcquisitionSetup();
            St.FolderPath = FolderPath;
            St.SizeX = SizeX;
            St.SizeY = SizeY;
            St.Type = Type;
            return St;
        }

        #endregion
    }
}
