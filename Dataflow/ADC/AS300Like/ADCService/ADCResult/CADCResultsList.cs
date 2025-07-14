using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using ADCCommon;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.EException;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;

using UnitySC.ADCAS300Like.Common.CVIDObj;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using System.Text;

namespace UnitySC.ADCAS300Like.Service
{
    public class CADCResultsList : CADCResults
    {
        public String[] VIDNameTab = new string[]{
             "Results_DataFileName",
             "Results_SlotID",
             "Results_LoadPortSource",
             "Results_ProcessStartTime",
             "Results_LaunchingFileName",
             "Results_LotID",
             "Results_WaferID",
             "ResultsFileOutputPathName",
             "ImagesFilesInputPathName_PSDDeflect_FS",
             "ImagesFilesInputPathName_PSDDeflect_BS",
             "ImagesFilesInputPathName_PSDReflect_FS",
             "ImagesFilesInputPathName_PSDReflect_BS",
             "ImagesFilesInputPathName_PSDDeflect_Die2Die_FS",
             "ImagesFilesInputPathName_PSDDeflect_Die2Die_BS",
             "ImagesFilesInputPathName_PSDReflect_Die2Die_FS",
             "ImagesFilesInputPathName_PSDReflect_Die2Die_BS",
             "ImagesFilesInputPathName_PSDDark_FS",
             "ImagesFilesInputPathName_PSDDark_BS",
             "ImagesFilesInputPathName_Brightfield2D",
             "ImagesFilesInputPathName_Darkfield",
             "ImagesFilesInputPathName_BrightfieldPattern",
             "ImagesFilesInputPathName_Brightfield3D",
             "ImagesFilesInputPathName_EyeEdge_Up",
             "ImagesFilesInputPathName_EyeEdge_UpBevel",
             "ImagesFilesInputPathName_EyeEdge_Apex",
             "ImagesFilesInputPathName_EyeEdge_BottomBevel",
             "ImagesFilesInputPathName_EyeEdge_Bottom",
             "ImagesFilesInputPathName_Nanotopography",
             "ImagesFilesInputPathName_Lightspeed",
             "ImagesFilesInputPathName_PSDGlobalT_FS",
             "ImagesFilesInputPathName_PSDGlobalT_BS",
             "ImagesFilesInputPathName_PSDCustomReflect_FS",
             "ImagesFilesInputPathName_PSDCustomReflect_BS",
             "Results_JobStartTime",
             "Results_JobID"
        };

        private CWaferReport _waferReport;


        //----------------------------------------------------------------------------------------------------------------
        // Members
        //----------------------------------------------------------------------------------------------------------------
        protected List<CVID> _vidObjectList = new List<CVID>();

        private int _fFirstReservedDefectVID;
        private int _lastReservedDefectVID;
        private int _firstReservedMeas2DVID;
        private int _lastReservedMeas2DVID;
        private int _firstReservedMeas3DVID;
        private int _lastReservedMeas3DVID;
        private bool _bClearAllVID = false;
        private ILogger _logger;
        public CADCResultsList(ADCType pServerType, LocalLogger resultLogger)
            : base(pServerType, resultLogger)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
        }

        //----------------------------------------------------------------------------------------------------------------
        // Properties
        //----------------------------------------------------------------------------------------------------------------
        public override Object this[int index]
        {
            set { _vidObjectList.Add((CVID)value); }
            get { return (Object)_vidObjectList[index]; }
        }

        public int Count { get { return _vidObjectList.Count; } }

        public CWaferReport WaferReport { get => _waferReport; set => _waferReport = value; }
        
        protected void ClearADCDataVid(enumVidType vidType, int startVidNummber, int endVidNumber)
        {
            for (int i = startVidNummber; i <= endVidNumber; i++)
            {
                // VID Num
                int iVID = i;
                CxValueObject lNewVIDData = new CxValueObject();
                switch (vidType)
                {
                    case enumVidType.dtASCII:
                        lNewVIDData.SetDataType(0, 0, VarType.A);
                        break;
                    case enumVidType.dtBool:
                        lNewVIDData.SetDataType(0, 0, VarType.Bo);
                        break;
                    case enumVidType.dtU4:
                        lNewVIDData.SetDataType(0, 0, VarType.U4);
                        break;
                    case enumVidType.dtF8:
                        lNewVIDData.SetDataType(0,0, VarType.F8);
                        break;
                    case enumVidType.dtList:
                        lNewVIDData.SetDataType(0, 0, VarType.L);
                        break;
                    default:
                        break;
                }
                // Create VID
                CVID lNewVID = new CVID(iVID, vidType, lNewVIDData, "Clear with empty value");
                _vidObjectList.Add(lNewVID);
            }
        }
        protected void ClearADCDataVid(enumVidType vidType, int pVidNummber)
        {            
            // VID double
            CxValueObject lNewVIDData = new CxValueObject();
            switch (vidType)
            {
                case enumVidType.dtASCII:
                    lNewVIDData.SetDataType(0, 0, VarType.A);
                    break;
                case enumVidType.dtBool:
                    lNewVIDData.SetDataType(0, 0, VarType.Bo);
                    break;
                case enumVidType.dtU4:
                    lNewVIDData.SetDataType(0, 0, VarType.U4);
                    break;
                case enumVidType.dtF8:
                    lNewVIDData.SetDataType(0, 0, VarType.F8);
                    break;
                case enumVidType.dtList:
                    lNewVIDData.SetDataType(0, 0, VarType.L);
                    break;
                default:
                    break;
            }
            // Create VID
            CVID lNewVID = new CVID(pVidNummber, vidType, lNewVIDData, "Clear with empty value");
            _vidObjectList.Add(lNewVID);            
        }
    }
}
