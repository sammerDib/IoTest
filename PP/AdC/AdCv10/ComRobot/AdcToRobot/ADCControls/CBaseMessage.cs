using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using Common.Communication;
using Common.FDC;

namespace Common.SocketMessage
{
    public enum enumCommandExchangeAS300 { caGetVersion = 0, caGetStatus, caGetLoadingStatus, caInit, caAskingToLoadUnload, caLoadUnloadFinished, caSetup, caStart, caAbort, caAcknowledge, caLiftUnlift, caClampWafer, caUnclampWafer, caCheckingWaferPresent, caGetSetup, caOpenDoor, caCloseDoor, caSetLoadingWaferType, caUpdateFDCInfo};
    public enum enumStatusExchangeAS300 { saUnknown = 0, saOk, saError };
    public enum enumStatusLoadingAS300 { slUnknown = 0, slReadyToLoad, slReadyToUnload, slReadyToLoadDoorClosed, slReadyToUnloadDoorClosed, slNotReady };
    public enum enumInitializationStatusAS300 { isUnknown, isNotInitialized, isInitOk, isInitError };
    public enum enumControleModeAS300 { cmUnknown = 0, cmLocal, cmRemote };
    public enum enumCycleStep { csNotStarted = 0, csCycleInProgress, csCycleComplete, csCycleAborted, csCycleError };
    public enum enumOrientationMarkType { omtNotch, omtFlat, omtDoubleFlat };
    public enum enumPresenceStatusAS300 { psUnknown = 0, psPresent, psNotPresent };
    public enum enumDoorStatusAS300 { dsUnknown = 0, dsOpened, dsClosed};
    public enum enumWaferType { wtCircular, wtRectangular};
    public enum enumLoadWaferType { lwStandard = 0, lwFilmFrame };
    [Flags]
    public enum enumLoadUnloadResult { lrOK = 0, lrUnloadFailed = 1, lrLoadFailed = 2 }
    
    public class AcquisitionUpdate
    {
        public AcquisitionUpdate()
        {
            StepDescription = "";
            CurrentProgress = 0;
            Total = 1;
        }

        public AcquisitionUpdate(String pStepDescription, long pCurrentProgress, long pTotal)
        {
            StepDescription = pStepDescription;
            CurrentProgress = pCurrentProgress;
            Total = pTotal;
        }

        public String StepDescription;
        public long CurrentProgress;
        public long Total;
    }

    /// <summary>
    /// Holds the wafer information
    /// </summary>
    [Serializable]
    public class CWaferParameters
    {
        /// <summary>
        /// The wafer ID
        /// </summary>
        public Int32 ID;
        /// <summary>
        /// The Wafer Slot ID
        /// </summary>
        public Int32 SlotID;
        /// <summary>
        /// the Load Port ID
        /// </summary>
        public Int32 LoadportID;
        /// <summary>
        /// the wafer Lot ID
        /// </summary>
        public String LotID;
        /// <summary>
        /// the wafer name
        /// </summary>
        public String WaferID;
        /// <summary>
        /// The data process result file path
        /// </summary>
        public String DataProcessResultFile;
        /// <summary>
        /// The viewer result file path
        /// </summary>
        public String DataViewerResultFile;
        /// <summary>
        /// The wafer angle applied by the aligner
        /// </summary>
        public double Angle;
        /// <summary>
        /// The wafer size [mm]
        /// </summary>
        public Int32 Size;
        /// <summary>
        /// The process start time
        /// </summary>
        public String ProcessStartTime;
        /// <summary>
        /// The customer file path
        /// </summary>
        public String CustomerPath;
        /// <summary>
        /// The recipe name
        /// </summary>
        public String RecipeName;
        /// <summary>
        /// The wafer count
        /// </summary>
        public int WaferCount;
        /// <summary>
        /// The name of the equipment
        /// </summary>
        public String EquipmentName;
        /// <summary>
        /// The ADC ADA Filepath
        /// </summary>
        public String ADCAdaPath;
        /// <summary>
        /// The base file name
        /// </summary>
        public String BaseFileName;
        /// <summary>
        /// The step ID
        /// </summary>
        public String StepID;
        /// <summary>
        /// The device ID
        /// </summary>
        public String DeviceID;
        /// <summary>
        /// The Equipment ID
        /// </summary>
        public String CustomerEquipmentID;
        /// <summary>
        /// The orientation mark type (notch, flat etc)
        /// </summary>
        public enumOrientationMarkType OrientationMarkType;
        /// <summary>
        /// Give a position status in carrier
        /// </summary>
        public int CarrierStatus;
        /// <summary>
        /// Wafer type cicular or rectangular
        /// </summary>
        public enumWaferType WaferType;
        /// <summary>
        /// True if the wafer is a slot contained in a holder
        /// </summary>
        public bool IsHolderSlot;
        /// <summary>
        /// Holder Config File Name if wafer is a holder slot  (not all the path)
        /// </summary>
        public String HolderConfigFileName;
        /// <summary>
        /// Slot Index if wafer is a holder slot
        /// </summary>
        public int HolderSlotIndex;
        public String ADCChamberID;
        public String ADCWaferTypeName ;
        public String JobStartTime ;
        public String JobID;

        /// <summary>
        /// Constructor
        /// </summary>
        public CWaferParameters()
        {
            ID = 0;
            SlotID = 1;
            LoadportID = 1;
            WaferID = "";
            DataProcessResultFile = "";
            Angle = 0;
            Size = 300;
            DateTimeFormatInfo myDTFI = new CultureInfo("fr-FR", false).DateTimeFormat;
            myDTFI.ShortDatePattern = @"MM-dd-yyyy";
            ProcessStartTime = DateTime.Now.ToString("g", myDTFI);
            RecipeName = "";
            CustomerPath = "";
            WaferCount = 0;
            EquipmentName = "";
            BaseFileName = "";
            OrientationMarkType = enumOrientationMarkType.omtFlat;
            HolderConfigFileName = "";
        }
    }
    public abstract class CBaseMessage : IMessage
    {
        /// <summary>
        /// Returned by the process module to tell the Robot its Conrol mode (remote or local)
        /// </summary>
        public enumControleModeAS300 ControleMode;
        /// <summary>
        /// Command sent by the Robot to the process module
        /// </summary>
        public enumCommandExchangeAS300 Command;
        /// <summary>
        /// Returned by the process module to tell the Robot its global status
        /// </summary>
        public enumStatusExchangeAS300 Status;
        /// <summary>
        /// Returned by the process module to tell the Robot its loading status
        /// </summary>
        public enumStatusLoadingAS300 StatusLoading;
        /// <summary>
        /// Returned by the process module to tell the Robot its initialization status
        /// </summary>
        public enumInitializationStatusAS300 InitializationStatus;
        /// <summary>
        /// String field to describe the errors/status of the process module
        /// </summary>
        public String Description;

        public AcquisitionUpdate AcquisitionProgress;
        /// <summary>
        /// Returned by the process module to tell the Robot its door status
        /// </summary>
        public enumDoorStatusAS300 DoorStatus;

        public enumLoadUnloadResult LastLoadUnloadStatus;
        /// <summary>
        /// Defines the wafer type that is being loaded. Useful for hybrid chucks
        /// </summary>
        public enumLoadWaferType LoadedWaferType;
        /// <summary>
        /// Define if wafer is present or not in PM support
        /// </summary>
        public enumPresenceStatusAS300 WaferPresenceStatus;

        /// <summary>
        /// Describes briefly the process module status for logging purpose on the Robot side
        /// </summary>
        public abstract String MessageLog { get; }

        public abstract String ErrorStatusText { get;}

        public FDCInfo FdcInfoData { get; set; }
    }
}
