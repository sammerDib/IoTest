using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Common.SocketMessage;

namespace Common
{
    public enum enumDarkfieldMode
    { MODE_NONE = 0, FRONTSIDE, BACKSIDE, FRONTSIDE_ET_BACKSIDE };

    public struct PMFailure
    {
        public bool bInitializeFail;
        public bool bShutdownFail;
        public bool bPreloadFail;
        public bool bPostloadFail;
        public bool bPreUnloadFail;
        public bool bPostUnloadFail;
        public bool bStartProcessFail;
        public bool bProcessFail;
        public bool bStopProcessFail;
        public bool bAbortProcessFail;
        public bool bPauseProcessFail;
        public bool bResumeProcessFail;
        public bool bValidateRecipeFail;
        public bool bRecipeUpdateFail;
    };

    public class CWaferSideParameters
    {
        public String DataProcessResultFile;
        public String DataViewerResultFile;
        public String ADCAdaPath;
        public String SummaryFileName;
        public String ADCConfigurationFile;
    }

    public class CCarrierType
    {
        private String m_TypeName;

        private enumCarrierType m_Type;

        private int m_WaferSizeInCarrier;

        private enumWaferThicknessType m_WaferThicknessType;

        public CCarrierType(String pTypeName, enumCarrierType pType, int pWaferSizeInCarrier, enumWaferThicknessType pWaferThicknessType)
        {
            m_TypeName = pTypeName;
            m_Type = pType;
            m_WaferSizeInCarrier = pWaferSizeInCarrier;
            m_WaferThicknessType = pWaferThicknessType;
        }

        public String TypeName
        {
            get { return m_TypeName; }
            set { m_TypeName = value; }
        }

        public enumCarrierType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public int WaferSizeInCarrier
        {
            get { return m_WaferSizeInCarrier; }
            set { m_WaferSizeInCarrier = value; }
        }

        public enumWaferThicknessType WaferThicknessType
        {
            get { return m_WaferThicknessType; }
            set { m_WaferThicknessType = value; }
        }

        public CCarrierType Clone()
        {
            CCarrierType lClone = new CCarrierType("", enumCarrierType.crType1, 0, enumWaferThicknessType.wttNORMAL);
            lClone.TypeName = m_TypeName;
            lClone.Type = m_Type;
            lClone.WaferSizeInCarrier = m_WaferSizeInCarrier;
            lClone.WaferThicknessType = m_WaferThicknessType;
            return lClone;
        }
    }

    public class CWaferParametersDarkview
    {
        public Int32 ID;
        public Int32 SlotID;
        public Int32 LoadportID;
        public String RecipeDarkfield;
        public String LotID;
        public String WaferID;
        public double Angle;
        public Int32 Size;
        public String ProcessStartTime;
        public String CustomerPath;
        public String StepID;
        public String DeviceID;
        public String CustomerEquipmentID;
        public enumOrientationMarkType OrientationMarkType;

        public CWaferSideParameters WaferFrontSide = new CWaferSideParameters();
        public CWaferSideParameters WaferBackSide = new CWaferSideParameters();

        // From equipment
        public List<String> SequenceProcessModules = new List<String>();

        public String RecipeMain;
        public int WaferCount;
        public String EquipmentName;

        public String ADCChamberID;
        public String ADCWaferTypeName;
        public String JobStartTime;

        public String JobID;

        public CWaferParametersDarkview()
        {
            ID = 0;
            SlotID = 1;
            LoadportID = 1;
            RecipeDarkfield = "";
            LotID = "";
            WaferID = "";
            Angle = 0;
            Size = 300;
            //DateTimeFormatInfo myDTFI = new CultureInfo("fr-FR", false).DateTimeFormat;
            //myDTFI.ShortDatePattern = @"MM-dd-yyyy";
            ProcessStartTime = DateTime.Now.ToString("MM-dd-yyyy  HH:mm:ss");
            RecipeMain = "";
            CustomerPath = "";
            // Front
            WaferFrontSide.ADCAdaPath = "";
            WaferFrontSide.DataProcessResultFile = "";
            WaferFrontSide.DataViewerResultFile = "";
            WaferFrontSide.SummaryFileName = "";
            // Back
            WaferBackSide.ADCAdaPath = "";
            WaferBackSide.DataProcessResultFile = "";
            WaferBackSide.DataViewerResultFile = "";
            WaferBackSide.SummaryFileName = "";

            ADCChamberID = "0";
            ADCWaferTypeName = "";
            JobStartTime = "";
        }
    }

    public class CLoadportDefinition
    {
        public int PortID;
        public int SubstrateType;
        public bool TagReaderAvailable;
        public int LoadportType;
    }

}