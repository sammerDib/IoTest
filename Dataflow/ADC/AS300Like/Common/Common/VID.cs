using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VALUELib;

namespace Common2017.CVIDObj
{
    //public enum enumDataType { dtU4, dtASCII, dtF8, dtList, dtBool };

    public class CVID
    {
        int m_VIDNum = 0;
        String m_VIDName = "";
        enumVidType m_DataType;
        CxValueObjectClass m_VIDData = new CxValueObjectClass();
        String m_Description;
        public CVID(Int32 ID, CxValueObjectClass pVIDData, String pDescription)
        {
            m_VIDNum = ID;
            m_VIDName = "";
            m_DataType = enumVidType.dtList;
            m_VIDData = pVIDData;
            m_Description = ID.ToString() + "= " + pDescription;
        }
        public CVID(Int32 ID, enumVidType pDataType, CxValueObjectClass pVIDData, String pDescription)
        {
            m_VIDNum = ID;
            m_VIDName = "";
            m_DataType = pDataType;
            m_VIDData = pVIDData;
            m_Description = ID.ToString() + "= " + pDescription;
        }
        public CVID(String pName, enumVidType pDataType, CxValueObjectClass pVIDData, String pDescription)
        {
            m_VIDNum = -1;
            m_VIDName = pName;
            m_DataType = pDataType;
            m_VIDData = pVIDData;
            m_Description = pName + "= " + pDescription;
        }
        public CxValueObjectClass VID_DataList
        {
            get { return m_VIDData; }
        }
        public int VID_ID
        {
            get { return m_VIDNum; }
        }
        public String VID_Name
        {
            get { return m_VIDName; }
        }
        public enumVidType VID_DataType
        {
            get { return m_DataType; }
        }
    }
}
