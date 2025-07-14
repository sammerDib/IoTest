using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201
{
    public partial class RE201 : UnitySC.Equipment.Abstractions.Devices.SmifLoadPort.SmifLoadPort, IRE201
    {
        public static new readonly DeviceType Type;

        static RE201()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.RE201.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.RE201");
            }
        }

        public RE201(string name)
            : this(name, Type)
        {
        }

        protected RE201(string name, DeviceType type)
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

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.ErrorControllerId ErrorControllerName
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.ErrorControllerId)GetStatusValue("ErrorControllerName"); }
            protected set { SetStatusValue("ErrorControllerName", value); }
        }

        public string ErrorCode
        {
            get { return (string)GetStatusValue("ErrorCode"); }
            protected set { SetStatusValue("ErrorCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
            protected set { SetStatusValue("ErrorDescription", value); }
        }

        public bool I_SubstrateDetection
        {
            get { return (bool)GetStatusValue("I_SubstrateDetection"); }
            protected set { SetStatusValue("I_SubstrateDetection", value); }
        }

        public bool I_MotionProhibited
        {
            get { return (bool)GetStatusValue("I_MotionProhibited"); }
            protected set { SetStatusValue("I_MotionProhibited", value); }
        }

        public bool I_ClampRightClose
        {
            get { return (bool)GetStatusValue("I_ClampRightClose"); }
            protected set { SetStatusValue("I_ClampRightClose", value); }
        }

        public bool I_ClampLeftClose
        {
            get { return (bool)GetStatusValue("I_ClampLeftClose"); }
            protected set { SetStatusValue("I_ClampLeftClose", value); }
        }

        public bool I_ClampRightOpen
        {
            get { return (bool)GetStatusValue("I_ClampRightOpen"); }
            protected set { SetStatusValue("I_ClampRightOpen", value); }
        }

        public bool I_ClampLeftOpen
        {
            get { return (bool)GetStatusValue("I_ClampLeftOpen"); }
            protected set { SetStatusValue("I_ClampLeftOpen", value); }
        }

        public bool I_CarrierPresenceMiddle
        {
            get { return (bool)GetStatusValue("I_CarrierPresenceMiddle"); }
            protected set { SetStatusValue("I_CarrierPresenceMiddle", value); }
        }

        public bool I_CarrierPresenceLeft
        {
            get { return (bool)GetStatusValue("I_CarrierPresenceLeft"); }
            protected set { SetStatusValue("I_CarrierPresenceLeft", value); }
        }

        public bool I_CarrierPresenceRight
        {
            get { return (bool)GetStatusValue("I_CarrierPresenceRight"); }
            protected set { SetStatusValue("I_CarrierPresenceRight", value); }
        }

        public bool I_AccessSwitch
        {
            get { return (bool)GetStatusValue("I_AccessSwitch"); }
            protected set { SetStatusValue("I_AccessSwitch", value); }
        }

        public bool I_ProtrusionDetection
        {
            get { return (bool)GetStatusValue("I_ProtrusionDetection"); }
            protected set { SetStatusValue("I_ProtrusionDetection", value); }
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

        public bool I_PositionForReadingId
        {
            get { return (bool)GetStatusValue("I_PositionForReadingId"); }
            protected set { SetStatusValue("I_PositionForReadingId", value); }
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

        public bool O_LaserStop
        {
            get { return (bool)GetStatusValue("O_LaserStop"); }
            protected set { SetStatusValue("O_LaserStop", value); }
        }

        public bool O_InterlockCancel
        {
            get { return (bool)GetStatusValue("O_InterlockCancel"); }
            protected set { SetStatusValue("O_InterlockCancel", value); }
        }

        public bool O_CarrierClampCloseRight
        {
            get { return (bool)GetStatusValue("O_CarrierClampCloseRight"); }
            protected set { SetStatusValue("O_CarrierClampCloseRight", value); }
        }

        public bool O_CarrierClampOpenRight
        {
            get { return (bool)GetStatusValue("O_CarrierClampOpenRight"); }
            protected set { SetStatusValue("O_CarrierClampOpenRight", value); }
        }

        public bool O_CarrierClampCloseLeft
        {
            get { return (bool)GetStatusValue("O_CarrierClampCloseLeft"); }
            protected set { SetStatusValue("O_CarrierClampCloseLeft", value); }
        }

        public bool O_CarrierClampOpenLeft
        {
            get { return (bool)GetStatusValue("O_CarrierClampOpenLeft"); }
            protected set { SetStatusValue("O_CarrierClampOpenLeft", value); }
        }

        public bool O_GreenIndicator
        {
            get { return (bool)GetStatusValue("O_GreenIndicator"); }
            protected set { SetStatusValue("O_GreenIndicator", value); }
        }

        public bool O_RedIndicator
        {
            get { return (bool)GetStatusValue("O_RedIndicator"); }
            protected set { SetStatusValue("O_RedIndicator", value); }
        }

        public bool O_LoadIndicator
        {
            get { return (bool)GetStatusValue("O_LoadIndicator"); }
            protected set { SetStatusValue("O_LoadIndicator", value); }
        }

        public bool O_UnloadIndicator
        {
            get { return (bool)GetStatusValue("O_UnloadIndicator"); }
            protected set { SetStatusValue("O_UnloadIndicator", value); }
        }

        public bool O_AccessSwitchIndicator
        {
            get { return (bool)GetStatusValue("O_AccessSwitchIndicator"); }
            protected set { SetStatusValue("O_AccessSwitchIndicator", value); }
        }

        public bool O_CarrierOpen
        {
            get { return (bool)GetStatusValue("O_CarrierOpen"); }
            protected set { SetStatusValue("O_CarrierOpen", value); }
        }

        public bool O_CarrierClamp
        {
            get { return (bool)GetStatusValue("O_CarrierClamp"); }
            protected set { SetStatusValue("O_CarrierClamp", value); }
        }

        public bool O_PodPresenceSensorOn
        {
            get { return (bool)GetStatusValue("O_PodPresenceSensorOn"); }
            protected set { SetStatusValue("O_PodPresenceSensorOn", value); }
        }

        public bool O_CarrierProperPlaced
        {
            get { return (bool)GetStatusValue("O_CarrierProperPlaced"); }
            protected set { SetStatusValue("O_CarrierProperPlaced", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.CarrierType CarrierDetectionMode
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.CarrierType)GetStatusValue("CarrierDetectionMode"); }
            protected set { SetStatusValue("CarrierDetectionMode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.CarrierType CarrierType
        {
            get { return (UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.CarrierType)GetStatusValue("CarrierType"); }
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
