using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common.Protocol_Robot_ADC;

namespace Common.Protocol_Robot_ADC
{
    #region ENUM_TYPE_DATA

    public enum enTypeData
    {
        en_int = 0,
        en_double,
        en_string
    }

    #endregion ENUM_TYPE_DATA

    #region ENUM_CLASS

    public enum enClassName
    {
        en_ADCViewMother = 0,
        en_MotherAdaEngine,
        en_MotherFactoryAccess,
        en_MotherRecipeAcces,
        en_MotherAutomationAccess,
        en_MotherClassificationAccess,
        en_MatrixConvertClass = 50,
        en_MatrixConvertClass_Topographie,
        en_MatrixConvertClass_LedMaxHightResolution,
        en_MatrixConvertClass_LedMaxLowResolution,
        en_MatrixConvertClass_DarkFiled,
        en_MatrixConvertClass_TopoMires,
        en_MatrixConvertClass_Edge,
        en_MatrixConvertClass_DieToDie,
        en_MatrixDataInterface,
        en_MatrixConvertClass_TopoPSD,
        en_LayerClassFoup = 100,
        en_LayerClassWafer,
        en_LayerClassCluster,
        en_LayerClassBlob,
        en_LayerClassDieToDie,
        en_LayerClassPicture,
        en_LayerClassCMC,
        en_EditorFoup = 200,
        en_EditorWafer,
        en_EditorWafer_Edge,
        en_EditorCluster,
        en_EditorCluster_Edge,
        en_EditorBlob,
        en_EditorKla,
        en_EditorKla_DieToDie,
        en_EditorKla_Edge,
        en_EditorDieTodie,
        en_FilterClass_Mother = 300,
        en_FilterClass_MotherCluster,
        en_FilterClass_MotherDefect,
        en_FilterClass_MotherImage,
        en_BlocFilterManager = 400,
        en_BlocFilterManager_Wafer,
        en_BlocFilterManager_cluster,
        en_ADAFileClass = 500,
        en_ObjectClass_Image = 600,
        en_ObjectClass_WaferReportVid,
        en_ObjectClass_DescrImage,
        en_ObjectClass_Caracteristic,
        en_SpecificClass,
    }

    #endregion ENUM_CLASS

    #region ENUM_ERROR

    public enum enError
    {
        // base
        en_NoError = 0,

        en_FunctionNotImplement,

        // erreur d'acces aux handle d'application
        en_HandleError_View = 1000,

        en_HandleError_ADA,
        en_HandleError_Wafer,
        en_HandleError_Image,
        en_HandleError_Cluster,
        en_HandleError_Module,
        en_BadTabIndex,
        en_BadPictureArrayIndex,

        // erreur lecture fichier
        en_FileError_Read = 2000,

        en_FileError_Read_Recipe_File,
        en_FileError_Read_Classification_File,
        en_FileError_Read_Automation_File,
        en_FileError_Read_Caracteristic_File,
        en_fileError_Read_WaferTypeFile,

        // erreur presence fichier
        en_FileError_Access = 2100,

        en_FileError_Access_Recipe_File,
        en_FileError_Access_Classification_File,
        en_FileError_Access_Automation_File,

        // erreur ecriture ou presence fichier
        en_FileError_Write = 2500,

        en_FileError_WriteEditFoup,
        en_FileError_WriteEditWafer,
        en_FileError_WriteEditCluster,
        en_FileError_WriteEditBlob,
        en_FileError_WriteEditKla,
        en_FileError_RenameADAFile,
        en_FileError_DeletePreCarFile,
        en_FileError_DeletePostCarFile,
        en_FileError_WriteMultiTiffPage,
        en_FileError_XmlReading,
        en_FileError_ReportDieToDie,
        en_FileError_ReportDieToDie_Card,
        en_FileError_ReadDieToDie_EntryFile,

        // erreur fichier ADA
        en_FileError_AdaInformation,

        en_FileError_BrightFieldExtension,
        en_FileError_ReadAdaType,

        // exception non intercepté
        en_Exception_StartAPI_Foup = 3000,

        en_Exception_StartNewWafer,
        en_Exception_EndWafer,

        // erreur d'acces au file d'object
        en_ManagerError_Wafer = 4000,

        // Acces MIL
        en_MIL_CreateImageBuffer = 5000,

        en_MIL_CreateCalculateBuffer,
        en_MIL_LoadImageBuffer,
        en_MIL_LoadImageBuffer_Mask,
        en_MIL_SaveImageBuffer,
        en_MIL_CreateMaskBuffer,
        en_MIL_APIApplication,
        en_MIL_APISystem,
        en_MIL_OperationDLL_Export,
        en_MIL_CreateChildImageBuffer,

        // Dans les bloc
        en_BlocCreate_Issue,

        en_BlocExecute_Issue,

        // Dans les filtre
        en_FilterCreate_Issue,

        en_FilterExecute_Issue,
        en_FilterInvalidParameters,

        // Cluster
        en_ClusterInvalidCaracteristic,

        en_ClusterClassificationParameters,
        en_ClusterInvalidPostBlocSelection,

        // matrice
        en_MIL_UnableTocreateMatrixBuffer,

        en_Matrix_NoMatrixForIndex,
        en_Matrix_BadMatrixForIndex,
        en_MatrixConvertRead,

        // dictonary
        en_Dictonary_Create,

        // base de donnée
        en_UnableToConnectDataBase,

        // Sockect
        en_SocketError_DeSerialization,

        en_SocketError_Serialization,
        en_SocketError_UnknowMessage,

        // coorector
        en_GeneralCorrectorError,

        //Eye Edge errors
        en_EdgeNotchNotFound,

        // nouveau message erreur en queue pour garder la compatibilité avec le robot sans mise à jour robot
        en_FileError_WriteXmlSortingFile,

        en_APCAccesFile,
        en_FileError_WriteHMRFile,
        en_SpecificError,
        en_FileError_ReadReviewRecipeFile
    }

    #endregion ENUM_ERROR

    #region ENUM_STATUS

    public enum enWaferStatus
    {
        en_unprocessed = 0,
        en_processing,
        en_complete,
        en_error
    }

    public enum enAnalysisStatus
    {
        en_unanalyzed = 0,
        en_aborted,
        en_success,

        /// <summary>
        /// Too much defects
        /// </summary>
        en_partial,

        /// <summary>
        /// Grading classified the wafer as reject
        /// </summary>
        GradingReject,

        /// <summary>
        /// Grading classified the wafer as rework
        /// </summary>
        GradingRework
    }

    #endregion ENUM_STATUS

    #region ENUM_BIN

    public enum EnBin
    {
        en_BIN1 = 0,
        en_BIN2 = 1,
        en_BIN3 = 2
    }

    #endregion ENUM_BIN

    #region ENUM_MODULE

    public enum enModuleID
    {
        en_Topography = 0,
        en_BrightField2D,
        en_Darkfield,
        en_BrightFieldPattern,
        en_EyeEdge,
        en_NanoTopography,
        en_Lightspeed,           // nouveau nom pour le velociraptor
        en_BrightField3D,
        en_EdgeInspect,          // ATTENTION => Gestion des anciens Edge
        en_Multi = 255

        // Topography = 0, BrightField2D = 1, Darkfield = 2, BrightFieldPattern = 3, EyeEdge = 4, Nanotopography = 5, en_Lightspeed = 6, BrightField3D = 7, OldEdgeInspect (3 capteurs) = 8, Topography_BackSide = 9
    }

    #endregion ENUM_MODULE

    #region ENUM CHANNEL

    public enum enChannelID
    {
        en_TopoDeflect_Front = 0,       // deflectivity module image (amplitude + curvature on each axis (sinuosidal fringes) ) on FRONTSIDE module
        en_TopoDeflect_Back = 1,        // deflectivity module image (amplitude + curvature on each axis (sinuosidal fringes) ) on Backside module
        en_TopoReflect_Front = 2,       // Reflection module image (a simple photo of reflection of wafer with a blank screen light) on FRONTSIDE module
        en_TopoReflect_Back = 3,        // Reflection module image (a simple photo of reflection of wafer with a blank screen light) on Backside module

        en_TopoDeflect_Die_Front = 4,   // Die to die -- cutupdie is called --
        en_TopoDeflect_Die_Back = 5,
        en_TopoReflect_Die_Front = 6,
        en_TopoReflect_Die_Back = 7,

        en_TopoDarkPSD_Front = 8,       //  image PSD en mode DarkField - FrontSide
        en_TopoDarkPSD_Back = 9,        //  image PSD en mode DarkField - BackSide

        en_TopoGlobalTopo_Front = 10,   // Global topography 3D heightmap Row/Warp measures - FrontSide
        en_TopoGlobalTopo_Back = 11,    // Global topography 3D heightmap Row/Warp measures - BackSide

        en_TopoCustomReflect_Front = 12, //  image PSD en Reflectivité Custom (projection image sur ecran) - FrontSide
        en_TopoCustomReflect_Back = 13,  //  image PSD en Reflectivité Custom (projection image sur ecran) - BackSide

        en_BrightField2D = 0,
        en_Darkfield = 0,
        en_BrightFieldPattern = 0,
        en_BrightField3D = 0,
        en_BrightField3D_DieToDie = 1,

        en_EyeEdge_Up = 0,
        en_EyeEdge_UpBevel = 1,
        en_EyeEdge_Apex = 2,
        en_EyeEdge_BottomBevel = 3,
        en_EyeEdge_Bottom = 4,

        en_NanoTopography = 0,
        en_Lightspeed = 0           // nouveau nom pour le velociraptor
    }

    #endregion ENUM CHANNEL

    #region ENUM TYPE MESURE

    public enum enTypeMeasurement
    {
        en_EdgeMeasurement = 0,
        en_BowWarpMeasurement,
        en_PolishedMeasurement, 
        en_2DMetro, 
        en_3DMetro, 
        en_Haze
    }

    #endregion ENUM TYPE MESURE

    public enum enumCommandExchangeADC { caGetVersion = 0, caGetStatus, caAcknowledge, caGetResult, caClear, caUpdateFDCInfo };

    public enum enumStatusExchangeADC { saUnknown = 0, saOk, saError };

    [Serializable]
    public class CVIDProcessMeasurement
    {
        public enTypeMeasurement m_enMeasurementType;
        public int m_iVIDValue;
        public String m_sVIDLabel;
        public double m_lfMeasurementValue;
        public String m_sUnitValue;
        public int m_iXcoordinate;
        public int m_iYcoordinate;
        public int m_RadiusValue;
        public int m_MeasurementNumber;
        public int m_iSubVIDValue; // => numero de mesure
        public String m_sSubVIDLabel;
    }

    [Serializable]
    public class CVIDProcessDefect
    {
        public int m_iVIDValue;
        public String m_sVIDLabel;
        public int m_iSubVIDValue;
        public String m_sSubVIDLabel;
        public List<double> m_lsSizeDefectBin;
        public List<double> m_lsCountDefefectBin;
    }

    [Serializable]
    public class CVIDProcessAPCModule
    {
        public int m_ModuleID;
        public List<String> m_lsLabel;
        public List<double> m_lsValue;
    }

    [Serializable]
    public class CVIDProcessAPC
    {
        public int m_iVIDValue;
        public String m_sVIDLabel;
        public List<CVIDProcessAPCModule> m_DataAPCModule;
    }

    [Serializable]
    public class CVIDDieCollectionRow
    {
        public List<string> row;
    }

    [Serializable]
    public class CVIDDieCollection
    {
        public int m_iVIDValue;
        public String m_sVIDLabel;
        public List<CVIDDieCollectionRow> lsDieCollectionRow;
        public List<string> lsColumnLabel;
        public List<enTypeData> lsColumnType;
    }

    //[Serializable]
    //public class CVIDProcessAPC
    //{
    //    public int m_iVIDValue;
    //    public String m_sVIDLabel;
    //    public List<String> m_lsLabel;
    //    public List<double> m_lsValue;
    //}

    [Serializable]
    public class CADCInputData
    {
        public String sInputPictureDirectory;
        public enChannelID channelID;
        public enModuleID ModuleID;
    }

    [Serializable]
    public class CErrorParameters
    {
        public String sErrorMessage;
        public enError iErrorNumber;
        public enClassName iClassError;

        public bool DataBaseIsConnected;
    }

    [Serializable]
    [XmlInclude(typeof(CVIDProcessDefect))]
    [XmlInclude(typeof(CVIDProcessMeasurement))]
    [XmlInclude(typeof(CVIDProcessAPC))]
    [XmlInclude(typeof(CVIDDieCollection))]
    public class CWaferReport
    {
        public enWaferStatus enWaferStatus;
        public int iSlotID;
        public int iLoadPort;
        public String sProcessStartTime;
        public String sJobStartTime;
        public String sLaunchingFileName;  // chemin et nom fichier ADC
        public String sLotID;
        public String sWaferID;
        public String JobID;
        public enAnalysisStatus enAnalysisStatus;
        public string ErrorMessage;

        public String m_sFileKlarfName;
        public String m_sOutputDataResultDirectory;

        [XmlArray("ADCInputDataList")]
        [XmlArrayItem("ADCInputDataItem", typeof(CADCInputData))]
        public List<CADCInputData> ADCInputDataTab;

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // OPF_BECARREFULL : ATTENTION, ACHTUNG  !!!!!!!!!!!!!!!
        // Pas de serialisation possible de Dictionaire. On passe donc par deux listes pour simuler (le dictionnaire bien sur, pas autre chose .... )
        // CRI_BECAREFULLL : ATTENTION !!
        // Pour bien enfoncer le clou... et pas autre chose....je confirme !!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // pour les defauts
        [XmlArray("VIDProcessDefectList")]
        [XmlArrayItem("VIDProcessDefectObject", typeof(CVIDProcessDefect))]
        public List<CVIDProcessDefect> DefectList;  // Clef = SubVID

        // pour les mesure
        [XmlArray("VIDProcessMeasurementList")]
        [XmlArrayItem("VIDProcessMeasurementObject", typeof(CVIDProcessMeasurement))]
        public List<CVIDProcessMeasurement> MeasurementList; // Clef = VID

        // pour les APC
        [XmlArray("VIDProcessAPCList")]
        [XmlArrayItem("VIDProcessAPCObject", typeof(CVIDProcessAPC))]
        public List<CVIDProcessAPC> APCList; // Clef = VID

        // pour les Die Collection
        [XmlArray("CVIDDieCollectionList")]
        [XmlArrayItem("CVIDDieCollectionObject", typeof(CVIDDieCollection))]
        public List<CVIDDieCollection> DieCollectionList; // Clef = VID
    }

    [Serializable]
    public class CMessageADC : CADCBaseMessage
    {
        public static String ADCGetRevision()
        {
            return "V9R1200";
        }

        public CWaferReport pCWaferReport;

        [XmlArray("CompleteUniqueIDList")]
        [XmlArrayItem("CompletedUniqueIdValue", typeof(int))]
        public List<int> lsCompletedUniqueId;

        public int aUniqueIDSearch;

        public override String MessageLog
        {
            get
            {
                String strError = " | ERR= Database connected - ";
                if (Error != null)
                {
                    if (!Error.DataBaseIsConnected)
                        strError = " | ERR= Database not connected - ";
                    if ((strError == " | ERR= Database not connected - ") && ((Error.iErrorNumber == enError.en_NoError) || (pCWaferReport == null)))
                        strError = " | ERR= Database not connected";
                    else
                        if ((strError == " | ERR= Database connected - ") && ((Error.iErrorNumber != enError.en_NoError) && (pCWaferReport != null)))
                        strError += Error.iErrorNumber.ToString().Remove(0, 3);
                    if (Error.DataBaseIsConnected && ((Error.iErrorNumber == enError.en_NoError) || ((Error.iErrorNumber != enError.en_NoError) && (pCWaferReport == null))))
                        strError = "";
                }
                else
                    strError = "";
                String lstrComment = ((Description!=null && (Description.Length > 0)) ? " | CMT=" + Description : "");
                String lstrCommand = "CD=" + Command.ToString().Remove(0, 2);
                String lstrStatus = " | S=" + Status.ToString().Remove(0, 2);
                switch (Command)
                {
                    case enumCommandExchangeADC.caGetStatus:
                        strError = "";
                        goto default;
                    default:
                        return lstrCommand + lstrStatus + strError + lstrComment;
                }
            }
        }
    }
}