using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Globalization;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public enum ClientConnection
    {
        NONE = 0,
        AcquisitionFrontside_OK,
        AcquisitionFrontside_NC,
        AcquisitionFrontside_ABS,
        AcquisitionBackside_OK,
        AcquisitionBackside_NC,
        AcquisitionBackside_ABS,
        SL300_OK,
        SL300_NC,
        SL300_ABS
    }

    public delegate void OnConnectionEvent(ClientConnection lClientConnection);
    public delegate void OnGUIDisplay(Connection ServerType, String Msg, String MsgError);
    public delegate void OnStatuschanged(enumSide Side, Object Status);
    public delegate void OnStageStatusChange(Object Status);
    
    public enum Connection { CONNECT_ACQUISITION_FRONTSIDE = 0, CONNECT_ACQUISITION_BACKSIDE, CONNECT_SL300, CONNECT_NANOTOPO, UNKNOWN };
    public enum enumSide {msFrontside=0, msBackside, msUnknown};
    public enum enumEngineStatus { esNotConnected = 0, egDisabled, egNotInitialized, egHoming, egOnStanby, egRDY_ScanUnload, egRunning, egError };
    public enum enumPneumaticsStatus { esNotConnected = 0, egRunning, egError, egUnknown };
    public enum enumStatusList { slAcquisitionStatus = 0, slMirrorStatus, slLaserStatus, slPMT1Status, slPMT2Status, slAS300ExchangeStatus, slStageStatus, slPneumaticsStatus, slWaferPresence };
    public enum enumStatusState { siUnknown = 0, siNotReady, siReadyToStart }
    public enum enumLaserState {lsActive=0, lsStanby, lsWarmup, lsFault, lsUnknown}
    public enum enumLaserError {Ok=0, ValueOutofRange, InvalidCommand}
    public enum enumLaserFault {Ok=0, InterlockOpen, DiodeOverCurrent, DiodeTemperatureFault, BasePlateTemperatureFault, BufferOverfflow, EpromError, I2CError, CommandTimeOut,WatchDogError, FatalError}
    public enum enumLaserTEC { TEC_ON, TEC_OFF };
    public enum enumCycleMode { cmSemiAuto = 0, cmAuto };
    public enum enumCycleStep { csNotStarted, csFirstScanning, csFirstScanComplete, csRotationRunning, csRotationComplete, csSecondScanning, csSecondScanComplete, csCycleComplete, csAborted }

    
    public class CWaferParameters
    {
        public static int NbParameters = 10; 

        // From wafer or manual form
        public Int32 ID;
        public Int32  SlotID;
        public Int32  LoadportID;
        public String RecipeDarkfield;
        public String LotID;
        public String WaferID;
        public String DataProcessResultFile;
        public double  Angle;
        public Int32  Size;
        public String ProcessStartTime;
        public List<Object> ParametersList = new List<Object>();
        public String CustomerPath;

        // From equipment
        public String RecipeMain;
        public int WaferCount;
        public String EquipmentName;

        public static String[] WafersParametersName = new String[]{ 
            "ID",
            "SlotID",
            "LoadportID",
            "DarkfieldRecipe",
            "LotID",
            "WaferID",
            "DataProcessResultFile",
            "Angle",
            "Size",
            "ProcessStartTime"
        };        

        public CWaferParameters()
        {
            ID = 0;
            ParametersList.Add(ID);
            SlotID = 1;
            ParametersList.Add(SlotID);
            LoadportID = 1;
            ParametersList.Add(LoadportID);
            RecipeDarkfield = "";
            ParametersList.Add(RecipeDarkfield);
            LotID = "";
            ParametersList.Add(LotID);
            WaferID = "";
            ParametersList.Add(WaferID);
            DataProcessResultFile = "";
            ParametersList.Add(DataProcessResultFile);
            Angle = 0;
            ParametersList.Add(LoadportID);
            Size = 300;
            ParametersList.Add(Size);
            DateTimeFormatInfo myDTFI = new CultureInfo("fr-FR", false).DateTimeFormat;
            myDTFI.ShortDatePattern = @"MM-dd-yyyy";
            ProcessStartTime = DateTime.Now.ToString("g", myDTFI);
            ParametersList.Add(ProcessStartTime);
            RecipeMain = "";
            CustomerPath = "";
        }
    }
   
    public class CCommand
    {
        public Object FunctionCode;
        public List<Object> m_ListParams;

        public CCommand(Object pFunctionCode)
            : this(pFunctionCode, null, null, null, null) { }
        public CCommand( Object pFunctionCode, Object pParameter1)
            : this(pFunctionCode, pParameter1, null, null, null) { }
        public CCommand( Object pFunctionCode, Object pParameter1, Object pParameter2)
            : this(pFunctionCode, pParameter1, pParameter2, null, null) { }
        public CCommand(Object pFunctionCode, Object pParameter1, Object pParameter2, Object pParameter3)
            : this(pFunctionCode, pParameter1, pParameter2, pParameter3, null) { }

        public CCommand(Object pFunctionCode, Object pParameter1, Object pParameter2, Object pParameter3, Object pParameter4)
        {
            FunctionCode = pFunctionCode;
            m_ListParams = new List<object>();
            if (pParameter1 != null)
                m_ListParams.Add(pParameter1);
            if (pParameter2 != null)
                m_ListParams.Add(pParameter2);
            if (pParameter3 != null)
                m_ListParams.Add(pParameter3);
            if (pParameter4 != null)
                m_ListParams.Add(pParameter4);
        }
    }
}
