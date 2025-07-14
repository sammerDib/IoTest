using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201
{
    public partial class RV201 : UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort, IRV201
    {
        public static new readonly DeviceType Type;

        static RV201()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.RV201.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.RV201");
            }
        }

        public RV201(string name)
            : this(name, Type)
        {
        }

        protected RV201(string name, DeviceType type)
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

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.ErrorControllerId ErrorControllerName
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.ErrorControllerId)GetStatusValue("ErrorControllerName"); }
            protected set { SetStatusValue("ErrorControllerName", value); }
        }

        public string ErrorCode
        {
            get { return (string)GetStatusValue("ErrorCode"); }
            protected set { SetStatusValue("ErrorCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
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

        public bool I_ExhaustFan1
        {
            get { return (bool)GetStatusValue("I_ExhaustFan1"); }
            protected set { SetStatusValue("I_ExhaustFan1", value); }
        }

        public bool I_ExhaustFan2
        {
            get { return (bool)GetStatusValue("I_ExhaustFan2"); }
            protected set { SetStatusValue("I_ExhaustFan2", value); }
        }

        public bool I_Protrusion
        {
            get { return (bool)GetStatusValue("I_Protrusion"); }
            protected set { SetStatusValue("I_Protrusion", value); }
        }

        public bool I_Protrusion2
        {
            get { return (bool)GetStatusValue("I_Protrusion2"); }
            protected set { SetStatusValue("I_Protrusion2", value); }
        }

        public bool I_FOUPDoorLeftClose
        {
            get { return (bool)GetStatusValue("I_FOUPDoorLeftClose"); }
            protected set { SetStatusValue("I_FOUPDoorLeftClose", value); }
        }

        public bool I_FOUPDoorLeftOpen
        {
            get { return (bool)GetStatusValue("I_FOUPDoorLeftOpen"); }
            protected set { SetStatusValue("I_FOUPDoorLeftOpen", value); }
        }

        public bool I_FOUPDoorRightClose
        {
            get { return (bool)GetStatusValue("I_FOUPDoorRightClose"); }
            protected set { SetStatusValue("I_FOUPDoorRightClose", value); }
        }

        public bool I_FOUPDoorRightOpen
        {
            get { return (bool)GetStatusValue("I_FOUPDoorRightOpen"); }
            protected set { SetStatusValue("I_FOUPDoorRightOpen", value); }
        }

        public bool I_MappingSensorContaining
        {
            get { return (bool)GetStatusValue("I_MappingSensorContaining"); }
            protected set { SetStatusValue("I_MappingSensorContaining", value); }
        }

        public bool I_MappingSensorPreparation
        {
            get { return (bool)GetStatusValue("I_MappingSensorPreparation"); }
            protected set { SetStatusValue("I_MappingSensorPreparation", value); }
        }

        public bool I_UpperPressureLimit
        {
            get { return (bool)GetStatusValue("I_UpperPressureLimit"); }
            protected set { SetStatusValue("I_UpperPressureLimit", value); }
        }

        public bool I_LowerPressureLimit
        {
            get { return (bool)GetStatusValue("I_LowerPressureLimit"); }
            protected set { SetStatusValue("I_LowerPressureLimit", value); }
        }

        public bool I_CarrierClampOpen
        {
            get { return (bool)GetStatusValue("I_CarrierClampOpen"); }
            protected set { SetStatusValue("I_CarrierClampOpen", value); }
        }

        public bool I_CarrierClampClose
        {
            get { return (bool)GetStatusValue("I_CarrierClampClose"); }
            protected set { SetStatusValue("I_CarrierClampClose", value); }
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

        public bool I_Presence
        {
            get { return (bool)GetStatusValue("I_Presence"); }
            protected set { SetStatusValue("I_Presence", value); }
        }

        public bool I_FOSBIdentificationSensor
        {
            get { return (bool)GetStatusValue("I_FOSBIdentificationSensor"); }
            protected set { SetStatusValue("I_FOSBIdentificationSensor", value); }
        }

        public bool I_ObstacleDetectingSensor
        {
            get { return (bool)GetStatusValue("I_ObstacleDetectingSensor"); }
            protected set { SetStatusValue("I_ObstacleDetectingSensor", value); }
        }

        public bool I_DoorDetection
        {
            get { return (bool)GetStatusValue("I_DoorDetection"); }
            protected set { SetStatusValue("I_DoorDetection", value); }
        }

        public bool I_OpenCarrierDetectingSensor
        {
            get { return (bool)GetStatusValue("I_OpenCarrierDetectingSensor"); }
            protected set { SetStatusValue("I_OpenCarrierDetectingSensor", value); }
        }

        public bool I_StageRotationBackward
        {
            get { return (bool)GetStatusValue("I_StageRotationBackward"); }
            protected set { SetStatusValue("I_StageRotationBackward", value); }
        }

        public bool I_StageRotationForward
        {
            get { return (bool)GetStatusValue("I_StageRotationForward"); }
            protected set { SetStatusValue("I_StageRotationForward", value); }
        }

        public bool I_BcrLifting
        {
            get { return (bool)GetStatusValue("I_BcrLifting"); }
            protected set { SetStatusValue("I_BcrLifting", value); }
        }

        public bool I_BcrLowering
        {
            get { return (bool)GetStatusValue("I_BcrLowering"); }
            protected set { SetStatusValue("I_BcrLowering", value); }
        }

        public bool I_CoverLock
        {
            get { return (bool)GetStatusValue("I_CoverLock"); }
            protected set { SetStatusValue("I_CoverLock", value); }
        }

        public bool I_CoverUnlock
        {
            get { return (bool)GetStatusValue("I_CoverUnlock"); }
            protected set { SetStatusValue("I_CoverUnlock", value); }
        }

        public bool I_CarrierRetainerLowering
        {
            get { return (bool)GetStatusValue("I_CarrierRetainerLowering"); }
            protected set { SetStatusValue("I_CarrierRetainerLowering", value); }
        }

        public bool I_CarrierRetainerLifting
        {
            get { return (bool)GetStatusValue("I_CarrierRetainerLifting"); }
            protected set { SetStatusValue("I_CarrierRetainerLifting", value); }
        }

        public bool I_External_SW1_ACCESS
        {
            get { return (bool)GetStatusValue("I_External_SW1_ACCESS"); }
            protected set { SetStatusValue("I_External_SW1_ACCESS", value); }
        }

        public bool I_External_SW2_TEST
        {
            get { return (bool)GetStatusValue("I_External_SW2_TEST"); }
            protected set { SetStatusValue("I_External_SW2_TEST", value); }
        }

        public bool I_External_SW3_UNLOAD
        {
            get { return (bool)GetStatusValue("I_External_SW3_UNLOAD"); }
            protected set { SetStatusValue("I_External_SW3_UNLOAD", value); }
        }

        public bool I_PFA_L
        {
            get { return (bool)GetStatusValue("I_PFA_L"); }
            protected set { SetStatusValue("I_PFA_L", value); }
        }

        public bool I_PFA_R
        {
            get { return (bool)GetStatusValue("I_PFA_R"); }
            protected set { SetStatusValue("I_PFA_R", value); }
        }

        public bool I_Dsc300mm
        {
            get { return (bool)GetStatusValue("I_Dsc300mm"); }
            protected set { SetStatusValue("I_Dsc300mm", value); }
        }

        public bool I_Dsc200mm
        {
            get { return (bool)GetStatusValue("I_Dsc200mm"); }
            protected set { SetStatusValue("I_Dsc200mm", value); }
        }

        public bool I_Dsc150mm
        {
            get { return (bool)GetStatusValue("I_Dsc150mm"); }
            protected set { SetStatusValue("I_Dsc150mm", value); }
        }

        public bool I_CstCommon
        {
            get { return (bool)GetStatusValue("I_CstCommon"); }
            protected set { SetStatusValue("I_CstCommon", value); }
        }

        public bool I_Cst200mm
        {
            get { return (bool)GetStatusValue("I_Cst200mm"); }
            protected set { SetStatusValue("I_Cst200mm", value); }
        }

        public bool I_Cst150mm
        {
            get { return (bool)GetStatusValue("I_Cst150mm"); }
            protected set { SetStatusValue("I_Cst150mm", value); }
        }

        public bool I_Adapter
        {
            get { return (bool)GetStatusValue("I_Adapter"); }
            protected set { SetStatusValue("I_Adapter", value); }
        }

        public bool I_CoverClosed
        {
            get { return (bool)GetStatusValue("I_CoverClosed"); }
            protected set { SetStatusValue("I_CoverClosed", value); }
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

        public bool O_Protrusion2Enabled
        {
            get { return (bool)GetStatusValue("O_Protrusion2Enabled"); }
            protected set { SetStatusValue("O_Protrusion2Enabled", value); }
        }

        public bool O_AdapterClamp
        {
            get { return (bool)GetStatusValue("O_AdapterClamp"); }
            protected set { SetStatusValue("O_AdapterClamp", value); }
        }

        public bool O_AdapterPower
        {
            get { return (bool)GetStatusValue("O_AdapterPower"); }
            protected set { SetStatusValue("O_AdapterPower", value); }
        }

        public bool O_ObstacleDetectionCancel
        {
            get { return (bool)GetStatusValue("O_ObstacleDetectionCancel"); }
            protected set { SetStatusValue("O_ObstacleDetectionCancel", value); }
        }

        public bool O_CarrierClampClose
        {
            get { return (bool)GetStatusValue("O_CarrierClampClose"); }
            protected set { SetStatusValue("O_CarrierClampClose", value); }
        }

        public bool O_CarrierClampOpen
        {
            get { return (bool)GetStatusValue("O_CarrierClampOpen"); }
            protected set { SetStatusValue("O_CarrierClampOpen", value); }
        }

        public bool O_FOUPDoorLockOpen
        {
            get { return (bool)GetStatusValue("O_FOUPDoorLockOpen"); }
            protected set { SetStatusValue("O_FOUPDoorLockOpen", value); }
        }

        public bool O_FOUPDoorLockClose
        {
            get { return (bool)GetStatusValue("O_FOUPDoorLockClose"); }
            protected set { SetStatusValue("O_FOUPDoorLockClose", value); }
        }

        public bool O_MappingSensorPreparation
        {
            get { return (bool)GetStatusValue("O_MappingSensorPreparation"); }
            protected set { SetStatusValue("O_MappingSensorPreparation", value); }
        }

        public bool O_MappingSensorContaining
        {
            get { return (bool)GetStatusValue("O_MappingSensorContaining"); }
            protected set { SetStatusValue("O_MappingSensorContaining", value); }
        }

        public bool O_ChuckingOn
        {
            get { return (bool)GetStatusValue("O_ChuckingOn"); }
            protected set { SetStatusValue("O_ChuckingOn", value); }
        }

        public bool O_ChuckingOff
        {
            get { return (bool)GetStatusValue("O_ChuckingOff"); }
            protected set { SetStatusValue("O_ChuckingOff", value); }
        }

        public bool O_CoverLock
        {
            get { return (bool)GetStatusValue("O_CoverLock"); }
            protected set { SetStatusValue("O_CoverLock", value); }
        }

        public bool O_CoverUnlock
        {
            get { return (bool)GetStatusValue("O_CoverUnlock"); }
            protected set { SetStatusValue("O_CoverUnlock", value); }
        }

        public bool O_DoorOpen_Ext
        {
            get { return (bool)GetStatusValue("O_DoorOpen_Ext"); }
            protected set { SetStatusValue("O_DoorOpen_Ext", value); }
        }

        public bool O_CarrierClamp_Ext
        {
            get { return (bool)GetStatusValue("O_CarrierClamp_Ext"); }
            protected set { SetStatusValue("O_CarrierClamp_Ext", value); }
        }

        public bool O_CarrierPresenceOn_Ext
        {
            get { return (bool)GetStatusValue("O_CarrierPresenceOn_Ext"); }
            protected set { SetStatusValue("O_CarrierPresenceOn_Ext", value); }
        }

        public bool O_PreparationCompleted_Ext
        {
            get { return (bool)GetStatusValue("O_PreparationCompleted_Ext"); }
            protected set { SetStatusValue("O_PreparationCompleted_Ext", value); }
        }

        public bool O_CarrierProperlyPlaced_Ext
        {
            get { return (bool)GetStatusValue("O_CarrierProperlyPlaced_Ext"); }
            protected set { SetStatusValue("O_CarrierProperlyPlaced_Ext", value); }
        }

        public bool O_StageRotationBackward
        {
            get { return (bool)GetStatusValue("O_StageRotationBackward"); }
            protected set { SetStatusValue("O_StageRotationBackward", value); }
        }

        public bool O_StageRotationForward
        {
            get { return (bool)GetStatusValue("O_StageRotationForward"); }
            protected set { SetStatusValue("O_StageRotationForward", value); }
        }

        public bool O_BcrLifting
        {
            get { return (bool)GetStatusValue("O_BcrLifting"); }
            protected set { SetStatusValue("O_BcrLifting", value); }
        }

        public bool O_BcrLowering
        {
            get { return (bool)GetStatusValue("O_BcrLowering"); }
            protected set { SetStatusValue("O_BcrLowering", value); }
        }

        public bool O_CarrierRetainerLowering
        {
            get { return (bool)GetStatusValue("O_CarrierRetainerLowering"); }
            protected set { SetStatusValue("O_CarrierRetainerLowering", value); }
        }

        public bool O_CarrierRetainerLifting
        {
            get { return (bool)GetStatusValue("O_CarrierRetainerLifting"); }
            protected set { SetStatusValue("O_CarrierRetainerLifting", value); }
        }

        public bool O_SW1_LED
        {
            get { return (bool)GetStatusValue("O_SW1_LED"); }
            protected set { SetStatusValue("O_SW1_LED", value); }
        }

        public bool O_SW3_LED
        {
            get { return (bool)GetStatusValue("O_SW3_LED"); }
            protected set { SetStatusValue("O_SW3_LED", value); }
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

        public bool O_MANUAL_LED
        {
            get { return (bool)GetStatusValue("O_MANUAL_LED"); }
            protected set { SetStatusValue("O_MANUAL_LED", value); }
        }

        public bool O_ERROR_LED
        {
            get { return (bool)GetStatusValue("O_ERROR_LED"); }
            protected set { SetStatusValue("O_ERROR_LED", value); }
        }

        public bool O_CLAMP_LED
        {
            get { return (bool)GetStatusValue("O_CLAMP_LED"); }
            protected set { SetStatusValue("O_CLAMP_LED", value); }
        }

        public bool O_DOCK_LED
        {
            get { return (bool)GetStatusValue("O_DOCK_LED"); }
            protected set { SetStatusValue("O_DOCK_LED", value); }
        }

        public bool O_BUSY_LED
        {
            get { return (bool)GetStatusValue("O_BUSY_LED"); }
            protected set { SetStatusValue("O_BUSY_LED", value); }
        }

        public bool O_AUTO_LED
        {
            get { return (bool)GetStatusValue("O_AUTO_LED"); }
            protected set { SetStatusValue("O_AUTO_LED", value); }
        }

        public bool O_RESERVED_LED
        {
            get { return (bool)GetStatusValue("O_RESERVED_LED"); }
            protected set { SetStatusValue("O_RESERVED_LED", value); }
        }

        public bool O_CLOSE_LED
        {
            get { return (bool)GetStatusValue("O_CLOSE_LED"); }
            protected set { SetStatusValue("O_CLOSE_LED", value); }
        }

        public bool O_LOCK_LED
        {
            get { return (bool)GetStatusValue("O_LOCK_LED"); }
            protected set { SetStatusValue("O_LOCK_LED", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.CarrierType CarrierDetectionMode
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.CarrierType)GetStatusValue("CarrierDetectionMode"); }
            protected set { SetStatusValue("CarrierDetectionMode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.CarrierType CarrierType
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.CarrierType)GetStatusValue("CarrierType"); }
            protected set { SetStatusValue("CarrierType", value); }
        }

        public string Version
        {
            get { return (string)GetStatusValue("Version"); }
            protected set { SetStatusValue("Version", value); }
        }
    }
}
