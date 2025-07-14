using System;
using System.Collections.Generic;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;

using UnitySC.ADCAS300Like.Common.CVIDObj;
using UnitySC.Shared.Logger;
using System.Text;

namespace UnitySC.ADCAS300Like.Service
{
    public abstract class CADCResults
    {
        public StringBuilder LogResult { get; set; }
        //----------------------------------------------------------------------------------------------------------------
        // Type
        //----------------------------------------------------------------------------------------------------------------
        public enum EVIDNameTab
        {
            Results_DataFileName = 0,
            Results_SlotID,
            Results_LoadPortSource,
            Results_ProcessStartTime,
            Results_LaunchingFileName,
            Results_LotID,
            Results_WaferID,
            ResultsFileOutputPathName,
            ImagesFilesInputPathName_PSDDeflect_FS,
            ImagesFilesInputPathName_PSDDeflect_BS,
            ImagesFilesInputPathName_PSDReflect_FS,
            ImagesFilesInputPathName_PSDReflect_BS,
            ImagesFilesInputPathName_PSDDeflect_Die2Die_FS,
            ImagesFilesInputPathName_PSDDeflect_Die2Die_BS,
            ImagesFilesInputPathName_PSDReflect_Die2Die_FS,
            ImagesFilesInputPathName_PSDReflect_Die2Die_BS,
            ImagesFilesInputPathName_PSDDark_FS,
            ImagesFilesInputPathName_PSDDark_BS,
            ImagesFilesInputPathName_Brightfield2D,
            ImagesFilesInputPathName_Darkfield,
            ImagesFilesInputPathName_BrightfieldPattern,
            ImagesFilesInputPathName_Brightfield3D,
            ImagesFilesInputPathName_EyeEdge_Up,
            ImagesFilesInputPathName_EyeEdge_UpBevel,
            ImagesFilesInputPathName_EyeEdge_Apex,
            ImagesFilesInputPathName_EyeEdge_BottomBevel,
            ImagesFilesInputPathName_EyeEdge_Bottom,
            ImagesFilesInputPathName_Nanotopography,
            ImagesFilesInputPathName_Lightspeed,
            ImagesFilesInputPathName_PSDGlobalT_FS,
            ImagesFilesInputPathName_PSDGlobalT_BS,
            ImagesFilesInputPathName_PSDCustomReflect_FS,
            ImagesFilesInputPathName_PSDCustomReflect_BS,
            Results_JobStartTime,
            Results_JobID
        }

        public class EdgeDefect
        {
            public int Slot;
            public int Loadport;
            public String LotID;
            public List<CVID> EdgeVID = new List<CVID>();
        }

        protected ADCType _serverType;
        protected List<EdgeDefect> _edgeDefectList;
        public static string EDGE_RESULTS_PATH = @"C:\CIMConnectProjects\Equipment1\EdgeResultsData\";

        protected LocalLogger ResultLogger { get; set; }
        public CADCResults(ADCType pServerType, LocalLogger resultsLogger)
        {
            _serverType = pServerType;
            ResultLogger = resultsLogger;
        }

        public abstract Object this[int index]
        {
            get;
            set;
        }

        public List<EdgeDefect> EdgeDefectList
        {
            get { return _edgeDefectList; }
            set { _edgeDefectList = value; }
        }
        public void WriteADCLog(String data, String path)
        {
            ResultLogger.Information(data);
        }
    }
}
