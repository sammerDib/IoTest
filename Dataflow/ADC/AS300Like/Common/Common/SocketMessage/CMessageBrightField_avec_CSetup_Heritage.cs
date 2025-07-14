using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Globalization;
using System.Xml.Serialization;


namespace Common.SocketMessage
{
    
    [Flags]
    public enum enumErrorDescriptionAS300 { edNoError = 0, Unknown = 1, edStageError = 2, edIOError = 4, edCameraError = 8, edFrameGrabberError = 16, edSensorMotorError = 32, edMicroscopeError = 64, edFFUError = 128, edInterlockError = 256, edMaintenanceMode = 512, edUserAssistanceRequired=1024 };
    public enum enumTriggerType { ttAreaTrigger, ttLineTrigger };
    public enum enumOrientationMarkPosition { mpUnknown = 0, mp0, mp90, mp180, mp270 }
    public enum enumBrighfieldSetupType { bst2D, bst3D, bstWaferIDReading }

    [Serializable]
    public class CRecipe2DParameters
    {
        /// <summary>
        /// The exposure time
        /// </summary>
        public int IntegrationTime;
        /// <summary>
        /// The gain
        /// </summary>
        public double Gain;
        /// <summary>
        /// The frequency
        /// </summary>
        public int Frequency;
        /// <summary>
        /// The acquisition height
        /// </summary>
        public double SensorHeight;
        /// <summary>
        /// the delta value around the sensor height for the autotune module
        /// </summary>
        public double SearchDelta;
        /// <summary>
        /// the step to use to acquire values for the autotune module
        /// </summary>
        public double SearchStep;
        /// <summary>
        /// CutUp recipe file name (not all the path)
        /// </summary>
        public string CutUpRecipe;
        public double HeightSearchPositionY;
        public double HeightSearchPositionX;
    }

    [Serializable]
    public class CRecipeDMCParameters
    {
        /// <summary>
        /// The DMC Recipe 
        /// </summary>
        public String DMCRecipeName;
    }

    [Serializable]
    public class CRecipe3DParameters
    {
        /// <summary>
        /// The Light intensity
        /// </summary>
        public int LightIntensity;
        /// <summary>
        /// The exposure time
        /// </summary>
        public int Frequency;
        /// <summary>
        /// The focus depth assumed
        /// </summary>
        public int FocusDepth;
        /// <summary>
        /// Threshold to filter bad peaks
        /// </summary>
        public int Threshold;
        /// <summary>
        /// Calibration file path name
        /// </summary>
        public String CalibrationPathFileName;
        /// <summary>
        /// The acquisition height
        /// </summary>
        public double SensorHeight;
        /// <summary>
        /// the delta value around the sensor height for the autotune module
        /// </summary>
        public double SearchDelta;
        /// <summary>
        /// the step to use to acquire values for the autotune module
        /// </summary>
        public double SearchStep;
        public double HeightSearchPositionY;
        public double HeightSearchPositionX;
    }

    /// <summary>
    /// Holds the recipe parameters for the following acquisition
    /// </summary>
    [Serializable]
    public class CBFRecipeParameters
    {
        [XmlIgnore]
        public enumBrighfieldSetupType SetupType = enumBrighfieldSetupType.bst2D;
        /// <summary>
        /// Process 2D BrightField must be or not preformed
        /// </summary>
        public bool bBrightField2DEnabled = true;
        /// <summary>
        /// Process 3D BrightField must be or not preformed
        /// </summary>
        public bool bBrightField3DEnabled=false;
        /// <summary>
        /// Process DMC reading must be or not performed
        /// </summary>
        public bool bDMCReadingEnabled = false;
        /// <summary>
        /// Recipe parameters set used for 2D process
        /// </summary>
        public CRecipe2DParameters Recipe2DParameters = new CRecipe2DParameters();        
        /// <summary>
        /// Recipe parameters set used for 3D process
        /// </summary>
        public CRecipe3DParameters Recipe3DParameters= new CRecipe3DParameters();
        /// <summary>
        /// Recipe parameters set used for DMC 
        /// </summary>
        public CRecipeDMCParameters RecipeDMCParameters = new CRecipeDMCParameters();
        /// <summary>
        /// wether the wafer is patterned or not
        /// </summary>
        public bool WaferPattern;
        /// <summary>
        /// the image reduction ratio
        /// </summary>
        public int ImageCompressingRate;
        /// <summary>
        /// The configuration file name to put in the ADA file
        /// </summary>
        public String ADCConfigurationFile;
        /// <summary>
        /// The acquisition offset angle to put
        /// </summary>
        public double OffsetAngle;
        /// <summary>
        /// The pattern mask filename
        /// </summary>
        public String PatternMaskFileName;
        /// <summary>
        /// True if a review recipe is selected
        /// </summary>
        public bool bReviewEnabled;
        /// <summary>
        /// the zoom value
        /// </summary>
        public int iZoomValue;
        /// <summary>
        /// the keyence height
        /// </summary>
        public long dKeyenceHeight;
        /// <summary>
        /// Orientation mark position in a Frame
        /// </summary>
        public enumOrientationMarkPosition OrientationMarkPosition;
        public bool IsAPC;
    }
    
    /// <summary>
    /// Object shared with the Robot software to handle communication with the module
    /// </summary>
    [Serializable]
    public class CMessageBrightField : CBaseMessage
    {
        [Serializable]
        public class CReviewSetup : CSetup
        {            
            /// <summary>
            /// Recipe parameters
            /// </summary>
            public CRecipePMReviewParameters RecipeParameters;

            /// <summary>
            /// Constructor
            /// </summary>
            public CReviewSetup() : base()
            { }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pWaferParameters">Wafer information</param>
            /// <param name="pRecipeParameters">Recipe information</param>
            public CReviewSetup(CWaferParameters pWaferParameters, CRecipePMReviewParameters pRecipeParameters)
                : base(pWaferParameters)
            {
                RecipeParameters = pRecipeParameters;
            }
        }
        [Serializable]
        public class CBFSetup : CSetup
        {
            /// <summary>
            /// Recipe parameters
            /// </summary>
            public CBFRecipeParameters RecipeParameters;

            /// <summary>
            /// Constructor
            /// </summary>
            public CReviewSetup()
                : base()
            { }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pWaferParameters">Wafer information</param>
            /// <param name="pRecipeParameters">Recipe information</param>
            public CReviewSetup(CWaferParameters pWaferParameters, CRecipePMReviewParameters pRecipeParameters)
                : base(pWaferParameters)
            {
                RecipeParameters = pRecipeParameters;
            }
        }
        /// <summary>
        /// Contains the setup parameters for an acquisition
        /// </summary>
        [Serializable]
        public abstract class CSetup
        {
            /// <summary>
            /// Tool name
            /// </summary>
            public String MachineName;
            /// <summary>
            /// The file path where to save the presentation image for the robot to display
            /// </summary>
            public String PresentationImageFilePath;
            /// <summary>
            /// Wafer parameters
            /// </summary>
            public CWaferParameters WaferParameters;
            /// <summary>
            /// Whole module sequence for ADA generation
            /// </summary>
            public String ModuleSequence = "";

            /// <summary>
            /// Constructor
            /// </summary>
            public CSetup()
            { }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pWaferParameters">Wafer information</param>
            public CSetup(CWaferParameters pWaferParameters)
            {
                WaferParameters = pWaferParameters;
            }
        }

        /// <summary>
        /// Returned by the process module to tell the Robot its error status
        /// </summary>
        public enumErrorDescriptionAS300 ErrorStatus;
        /// <summary>
        ///Returned by the process module to tell the Robot its cycle status (analyzing, error etc)
        /// </summary>
        public enumCycleStep CycleStatus;
        /// <summary>
        /// The value read with the DMCReader feature
        /// </summary>
        public String DMCValue;
        /// <summary>
        ///Acquisition parameters
        /// </summary>
        public List<CSetup> SetupList=new List<CSetup>();
        /// <summary>
        /// If only one setup is sent (used to keep compatibility with previous version)
        /// </summary>
        public CSetup Setup
        {
            get
            {
                if (SetupList.Count == 0)
                {
                    SetupList.Add(new CSetup());
                }
                return SetupList[0];  
            }
            set 
            {
                if (SetupList.Count == 0)
                {
                    SetupList.Add(new CSetup());
                }
                SetupList[0] = value;
            }
        }

        /// <summary>
        /// Serialization method
        /// </summary>
        /// <returns>The serialized instance of the object</returns>
        public override string ToString()
        {
            return Win32Tools.Serialize<CMessageBrightField>(this);
        }

        /// <summary>
        /// Describes briefly the process module status for logging purpose on the Robot side
        /// </summary>
        public override String MessageLog
        {
            get
            {
                switch (Command)
                {
                    case enumCommandExchangeAS300.caAcknowledge:
                        return "CD=" + Command.ToString().Remove(0, 2) + " | S=" + Status.ToString().Remove(0, 2) + " | CM=" + ControleMode.ToString().Remove(0, 2) + " | ERR=" + ErrorStatus.ToString().Remove(0, 2) + " | LS=" + StatusLoading.ToString().Remove(0, 2) + " | CS=" + CycleStatus.ToString().Remove(0, 2) + " | CMT=" + Description + " | INIT=" + InitializationStatus.ToString().Remove(0, 2); //+ " | WPS=" + WaferPresenceStatus.ToString().Remove(0, 2)
                    default:
                        return "CD=" + Command.ToString().Remove(0, 2) + " | ERR=" + ErrorStatus.ToString().Remove(0, 2) + " | CMT=" + Description; //+ " | WPS=" + WaferPresenceStatus.ToString().Remove(0, 2)
                }
            }
        }
        public override string ErrorStatusText
        {
            get { return ErrorStatus.ToString(); }
        }
    }
}
