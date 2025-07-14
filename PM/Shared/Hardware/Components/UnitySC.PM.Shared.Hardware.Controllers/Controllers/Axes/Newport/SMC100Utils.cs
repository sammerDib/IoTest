using System;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    /// <summary>
    /// Lists the possible statuses of a Newport sub drive
    /// </summary>
    public enum SimpleControllerState { UNKNOWN, NOT_REFERENCED, CONFIGURATION, HOMING, MOVING, READY, DISABLE, JOGGING };
    
    /// <summary>
    /// Lists the simplified controller statuses
    /// </summary>
    public enum NewportControllerState { ENABLED, READY, MOVING, HOMING, UNKNOWN };
    
    /// <summary>
    /// Lists all the possible error faults returned by the controller
    /// </summary>
    [Flags]
    public enum FullPositionnerState { None = 0, Negative_End_Of_Run = 1, Positive_End_Of_Run = 2, Peak_Current_Limit = 4, RMS_Current_Limit = 8, Short_Circuit_Detection = 16, Following_Error = 32, Homing_TimeOut = 64, Wrong_ESP_Stage = 128, DC_Voltage_Too_Low = 256, EightyWatts_Output_Power_Exceeded = 512 };

    public class NewPortSubDrive
    {
        public NewPortSubDrive(NewPortSubDriveConfig driveConfig)
        {
            Config = driveConfig;
            NewDiagArrived = new AutoResetEvent(false);
            BeforeDiag = new AutoResetEvent(false);
        }
        public readonly NewPortSubDriveConfig Config;
        public double LowerTravelLimit;
        public double UpperTravelLimit;
        public double Position;
        public double Speed;
        public NewportControllerState DriveState;
        public readonly AutoResetEvent NewDiagArrived;
        public readonly AutoResetEvent BeforeDiag;
    }
    
    public class NewPortStatusUpdate:ICloneable
    {
        /// <summary>
        /// The drive function
        /// </summary>
        public EnumDriveAxisId DriveFunction { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NewPortStatusUpdate() { }
        
        /// <summary>
        /// Get/sets the drive's current speed
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Gets/sets the drive's current position
        /// </summary>
        public double Position { get; set; }

        /// <summary>
        /// Gets/sets the drive state
        /// </summary>
        public SimpleControllerState State { get; set; }

        #region ICloneable Membres
        /// <summary>
        /// provide a method to clone the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            NewPortStatusUpdate update = new NewPortStatusUpdate();
            update.DriveFunction = DriveFunction;
            update.Position = Position;
            update.Speed = Speed;
            update.State = State;
            return update;
        }

        #endregion
    }
    
    public class SMC100Command
    {
        private SMC100Command(string value) { Code = value; }

        public string Code { get; private set; }

        public static SMC100Command GetState   { get { return new SMC100Command("TS"); } }
        public static SMC100Command GetPosition   { get { return new SMC100Command("TP"); } }
        public static SMC100Command SetGetSpeed   { get { return new SMC100Command("VA"); } }
        public static SMC100Command LowerLimit   { get { return new SMC100Command("SL"); } }
        public static SMC100Command UpperLimit    { get { return new SMC100Command("SR"); } }
        public static SMC100Command StopMotion { get { return new SMC100Command("ST"); } }
        public static SMC100Command MoveRelative   { get { return new SMC100Command("PA"); } }
        public static SMC100Command MoveAbsolute   { get { return new SMC100Command("PR"); } }
        public static SMC100Command Home   { get { return new SMC100Command("OR?"); } }
    }
}
