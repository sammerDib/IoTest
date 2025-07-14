using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Fringe_manager
{
    public class FringeFolder
    {
        public FringeFolder()
        {
            m_fringeList = new List<string>();
        }

        private String m_sFullFilePath;

        public String FullFilePath
        {
            get { return m_sFullFilePath; }
            set { m_sFullFilePath = value; }
        }
        private String m_sFringeName;

        public String FringeName
        {
            get { return m_sFringeName; }
            set { m_sFringeName = value; }
        }
        private int m_iShiftCount;

        public int ShiftCount
        {
            get { return m_iShiftCount; }
            set { m_iShiftCount = value; }
        }
        private List<String> m_fringeList;
        public string BlackImg;
        public string WhiteImg;

        public List<String> FringeList
        {
            get { return m_fringeList; }
            set { m_fringeList = value; }
        }

        private int m_iXShift;

        public int XShift
        {
            get { return m_iXShift; }
            set { m_iXShift = value; }
        }

        private int m_iYShift;

        public int YShift
        {
            get { return m_iYShift; }
            set { m_iYShift = value; }
        }
        protected int m_iPixelCount;
        public int PixelCount { get { return m_iPixelCount; } set { m_iPixelCount = value; } }

        private bool m_bDoForward;

        public bool DoForward
        {
            get { return m_bDoForward; }
            set { m_bDoForward = value; }
        }

        private String m_sRemoteName;

        public String RemoteName
        {
            get { return m_sRemoteName; }
            set { m_sRemoteName = value; }
        }     
    }
}
