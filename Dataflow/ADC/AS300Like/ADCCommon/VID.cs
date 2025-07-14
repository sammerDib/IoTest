using System;
using System.Windows.Documents;

using ADCCommon;

using UnitySC.ADCAS300Like.Common;

namespace UnitySC.ADCAS300Like.Common.CVIDObj
{
    //public enum enumDataType { dtU4, dtASCII, dtF8, dtList, dtBool };

    public class CVID
    {
        private int _vidNum = 0;
        private String _vidName = "";
        private enumVidType _dataType;
        private Object _vidData = new Object();
        private String _description;

        public CVID(Int32 ID, Object pVIDData, String pDescription)
        {
            _vidNum = ID;
            _vidName = "";
            _dataType = enumVidType.dtList; // List<Object>
            _vidData = pVIDData;
            _description = ID.ToString() + "= " + pDescription;
        }
        public CVID(Int32 ID, enumVidType pDataType, Object pVIDData, String pDescription)
        {
            _vidNum = ID;
            _vidName = "";
            _dataType = pDataType;
            _vidData = pVIDData;
            _description = ID.ToString() + "= " + pDescription;
        }
        public CVID(String pName, enumVidType pDataType, Object pVIDData, String pDescription)
        {
            _vidNum = -1;
            _vidName = pName;
            _dataType = pDataType;
            _vidData = pVIDData;
            _description = pName + "= " + pDescription;
        }
        public Object VID_DataList
        {
            get { return _vidData; }
        }
        public int VID_ID
        {
            get { return _vidNum; }
        }
        public String VID_Name
        {
            get { return _vidName; }
        }
        public enumVidType VID_DataType
        {
            get { return _dataType; }
        }
    }
}
