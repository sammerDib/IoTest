using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101
{
    public partial class RV101 : UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort, IRV101
    {
        public static new readonly DeviceType Type;

        static RV101()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.RV101.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.RV101");
            }
        }

        public RV101(string name)
            : this(name, Type)
        {
        }

        protected RV101(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                default:
                    base.InternalRun(execution);
                    break;
            }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode OperationMode
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode)GetStatusValue("OperationMode"); }
            protected set { SetStatusValue("OperationMode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OriginReturnCompletion OriginReturnCompletion
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OriginReturnCompletion)GetStatusValue("OriginReturnCompletion"); }
            protected set { SetStatusValue("OriginReturnCompletion", value); }
        }

        public UnitySC.EFEM.Rorze.Drivers.Enums.CommandProcessing CommandProcessing
        {
            get { return (UnitySC.EFEM.Rorze.Drivers.Enums.CommandProcessing)GetStatusValue("CommandProcessing"); }
            protected set { SetStatusValue("CommandProcessing", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationStatus OperationStatus
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationStatus)GetStatusValue("OperationStatus"); }
            protected set { SetStatusValue("OperationStatus", value); }
        }

        public bool IsNormalSpeed
        {
            get { return (bool)GetStatusValue("IsNormalSpeed"); }
            protected set { SetStatusValue("IsNormalSpeed", value); }
        }

        public string MotionSpeedPercentage
        {
            get { return (string)GetStatusValue("MotionSpeedPercentage"); }
            protected set { SetStatusValue("MotionSpeedPercentage", value); }
        }

        public string ErrorControllerCode
        {
            get { return (string)GetStatusValue("ErrorControllerCode"); }
            protected set { SetStatusValue("ErrorControllerCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.ErrorControllerId ErrorControllerName
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.ErrorControllerId)GetStatusValue("ErrorControllerName"); }
            protected set { SetStatusValue("ErrorControllerName", value); }
        }

        public string ErrorCode
        {
            get { return (string)GetStatusValue("ErrorCode"); }
            protected set { SetStatusValue("ErrorCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
            protected set { SetStatusValue("ErrorDescription", value); }
        }

        public bool I_EmergencyStop
        {
            get { return (bool)GetStatusValue("I_EmergencyStop"); }
            protected set { SetStatusValue("I_EmergencyStop", value); }
        }

        public bool I_TemporarilyStop
        {
            get { return (bool)GetStatusValue("I_TemporarilyStop"); }
            protected set { SetStatusValue("I_TemporarilyStop", value); }
        }

        public bool I_VacuumSourcePressure
        {
            get { return (bool)GetStatusValue("I_VacuumSourcePressure"); }
            protected set { SetStatusValue("I_VacuumSourcePressure", value); }
        }

        public bool I_AirSupplySourcePressure
        {
            get { return (bool)GetStatusValue("I_AirSupplySourcePressure"); }
            protected set { SetStatusValue("I_AirSupplySourcePressure", value); }
        }

        public bool I_ProtrusionDetection
        {
            get { return (bool)GetStatusValue("I_ProtrusionDetection"); }
            protected set { SetStatusValue("I_ProtrusionDetection", value); }
        }

        public bool I_Cover
        {
            get { return (bool)GetStatusValue("I_Cover"); }
            protected set { SetStatusValue("I_Cover", value); }
        }

        public bool I_DrivePower
        {
            get { return (bool)GetStatusValue("I_DrivePower"); }
            protected set { SetStatusValue("I_DrivePower", value); }
        }

        public bool I_MappingSensor
        {
            get { return (bool)GetStatusValue("I_MappingSensor"); }
            protected set { SetStatusValue("I_MappingSensor", value); }
        }

        public bool I_ShutterOpen
        {
            get { return (bool)GetStatusValue("I_ShutterOpen"); }
            protected set { SetStatusValue("I_ShutterOpen", value); }
        }

        public bool I_ShutterClose
        {
            get { return (bool)GetStatusValue("I_ShutterClose"); }
            protected set { SetStatusValue("I_ShutterClose", value); }
        }

        public bool I_PresenceLeft
        {
            get { return (bool)GetStatusValue("I_PresenceLeft"); }
            protected set { SetStatusValue("I_PresenceLeft", value); }
        }

        public bool I_PresenceRight
        {
            get { return (bool)GetStatusValue("I_PresenceRight"); }
            protected set { SetStatusValue("I_PresenceRight", value); }
        }

        public bool I_PresenceMiddle
        {
            get { return (bool)GetStatusValue("I_PresenceMiddle"); }
            protected set { SetStatusValue("I_PresenceMiddle", value); }
        }

        public bool I_InfoPadA
        {
            get { return (bool)GetStatusValue("I_InfoPadA"); }
            protected set { SetStatusValue("I_InfoPadA", value); }
        }

        public bool I_InfoPadB
        {
            get { return (bool)GetStatusValue("I_InfoPadB"); }
            protected set { SetStatusValue("I_InfoPadB", value); }
        }

        public bool I_InfoPadC
        {
            get { return (bool)GetStatusValue("I_InfoPadC"); }
            protected set { SetStatusValue("I_InfoPadC", value); }
        }

        public bool I_InfoPadD
        {
            get { return (bool)GetStatusValue("I_InfoPadD"); }
            protected set { SetStatusValue("I_InfoPadD", value); }
        }

        public bool I_200mmPresenceLeft
        {
            get { return (bool)GetStatusValue("I_200mmPresenceLeft"); }
            protected set { SetStatusValue("I_200mmPresenceLeft", value); }
        }

        public bool I_200mmPresenceRight
        {
            get { return (bool)GetStatusValue("I_200mmPresenceRight"); }
            protected set { SetStatusValue("I_200mmPresenceRight", value); }
        }

        public bool I_150mmPresenceLeft
        {
            get { return (bool)GetStatusValue("I_150mmPresenceLeft"); }
            protected set { SetStatusValue("I_150mmPresenceLeft", value); }
        }

        public bool I_150mmPresenceRight
        {
            get { return (bool)GetStatusValue("I_150mmPresenceRight"); }
            protected set { SetStatusValue("I_150mmPresenceRight", value); }
        }

        public bool I_AccessSwitch1
        {
            get { return (bool)GetStatusValue("I_AccessSwitch1"); }
            protected set { SetStatusValue("I_AccessSwitch1", value); }
        }

        public bool I_AccessSwitch2
        {
            get { return (bool)GetStatusValue("I_AccessSwitch2"); }
            protected set { SetStatusValue("I_AccessSwitch2", value); }
        }

        public bool O_PreparationCompleted
        {
            get { return (bool)GetStatusValue("O_PreparationCompleted"); }
            protected set { SetStatusValue("O_PreparationCompleted", value); }
        }

        public bool O_TemporarilyStop
        {
            get { return (bool)GetStatusValue("O_TemporarilyStop"); }
            protected set { SetStatusValue("O_TemporarilyStop", value); }
        }

        public bool O_SignificantError
        {
            get { return (bool)GetStatusValue("O_SignificantError"); }
            protected set { SetStatusValue("O_SignificantError", value); }
        }

        public bool O_LightError
        {
            get { return (bool)GetStatusValue("O_LightError"); }
            protected set { SetStatusValue("O_LightError", value); }
        }

        public bool O_ClampMovingDirection
        {
            get { return (bool)GetStatusValue("O_ClampMovingDirection"); }
            protected set { SetStatusValue("O_ClampMovingDirection", value); }
        }

        public bool O_ClampMovingStart
        {
            get { return (bool)GetStatusValue("O_ClampMovingStart"); }
            protected set { SetStatusValue("O_ClampMovingStart", value); }
        }

        public bool O_ShutterOpen
        {
            get { return (bool)GetStatusValue("O_ShutterOpen"); }
            protected set { SetStatusValue("O_ShutterOpen", value); }
        }

        public bool O_ShutterClose
        {
            get { return (bool)GetStatusValue("O_ShutterClose"); }
            protected set { SetStatusValue("O_ShutterClose", value); }
        }

        public bool O_ShutterMotionDisabled
        {
            get { return (bool)GetStatusValue("O_ShutterMotionDisabled"); }
            protected set { SetStatusValue("O_ShutterMotionDisabled", value); }
        }

        public bool O_ShutterOpen2
        {
            get { return (bool)GetStatusValue("O_ShutterOpen2"); }
            protected set { SetStatusValue("O_ShutterOpen2", value); }
        }

        public bool O_CoverLock
        {
            get { return (bool)GetStatusValue("O_CoverLock"); }
            protected set { SetStatusValue("O_CoverLock", value); }
        }

        public bool O_CarrierPresenceSensorOn
        {
            get { return (bool)GetStatusValue("O_CarrierPresenceSensorOn"); }
            protected set { SetStatusValue("O_CarrierPresenceSensorOn", value); }
        }

        public bool O_PreparationCompleted2
        {
            get { return (bool)GetStatusValue("O_PreparationCompleted2"); }
            protected set { SetStatusValue("O_PreparationCompleted2", value); }
        }

        public bool O_CarrierProperlyPlaced
        {
            get { return (bool)GetStatusValue("O_CarrierProperlyPlaced"); }
            protected set { SetStatusValue("O_CarrierProperlyPlaced", value); }
        }

        public bool O_AccessSwitch1
        {
            get { return (bool)GetStatusValue("O_AccessSwitch1"); }
            protected set { SetStatusValue("O_AccessSwitch1", value); }
        }

        public bool O_AccessSwitch2
        {
            get { return (bool)GetStatusValue("O_AccessSwitch2"); }
            protected set { SetStatusValue("O_AccessSwitch2", value); }
        }

        public bool O_LOAD_LED
        {
            get { return (bool)GetStatusValue("O_LOAD_LED"); }
            protected set { SetStatusValue("O_LOAD_LED", value); }
        }

        public bool O_UNLOAD_LED
        {
            get { return (bool)GetStatusValue("O_UNLOAD_LED"); }
            protected set { SetStatusValue("O_UNLOAD_LED", value); }
        }

        public bool O_PRESENCE_LED
        {
            get { return (bool)GetStatusValue("O_PRESENCE_LED"); }
            protected set { SetStatusValue("O_PRESENCE_LED", value); }
        }

        public bool O_PLACEMENT_LED
        {
            get { return (bool)GetStatusValue("O_PLACEMENT_LED"); }
            protected set { SetStatusValue("O_PLACEMENT_LED", value); }
        }

        public bool O_LATCH_LED
        {
            get { return (bool)GetStatusValue("O_LATCH_LED"); }
            protected set { SetStatusValue("O_LATCH_LED", value); }
        }

        public bool O_ERROR_LED
        {
            get { return (bool)GetStatusValue("O_ERROR_LED"); }
            protected set { SetStatusValue("O_ERROR_LED", value); }
        }

        public bool O_BUSY_LED
        {
            get { return (bool)GetStatusValue("O_BUSY_LED"); }
            protected set { SetStatusValue("O_BUSY_LED", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.CarrierType CarrierDetectionMode
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.CarrierType)GetStatusValue("CarrierDetectionMode"); }
            protected set { SetStatusValue("CarrierDetectionMode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.CarrierType CarrierType
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.CarrierType)GetStatusValue("CarrierType"); }
            protected set { SetStatusValue("CarrierType", value); }
        }

        public uint CarrierTypeIndex
        {
            get { return (uint)GetStatusValue("CarrierTypeIndex"); }
            protected set { SetStatusValue("CarrierTypeIndex", value); }
        }

        public string Version
        {
            get { return (string)GetStatusValue("Version"); }
            protected set { SetStatusValue("Version", value); }
        }
    }
}
