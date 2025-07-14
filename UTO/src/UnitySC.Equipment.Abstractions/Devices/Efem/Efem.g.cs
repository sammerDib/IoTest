using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.Efem
{
    public abstract partial class Efem : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IEfem
    {
        public static new readonly DeviceType Type;

        static Efem()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.Efem.Efem.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.Efem.Efem");
            }
        }

        public Efem(string name)
            : this(name, Type)
        {
        }

        protected Efem(string name, DeviceType type)
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

        public UnitySC.Equipment.Abstractions.Devices.Efem.Enums.OperationMode OperationMode
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Efem.Enums.OperationMode)GetStatusValue("OperationMode"); }
            protected set { SetStatusValue("OperationMode", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.Efem.Enums.RobotStatus RobotStatus
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Efem.Enums.RobotStatus)GetStatusValue("RobotStatus"); }
            protected set { SetStatusValue("RobotStatus", value); }
        }

        public UnitsNet.Ratio RobotSpeed
        {
            get { return (UnitsNet.Ratio)GetStatusValue("RobotSpeed"); }
            protected set
            {
                UnitsNet.Units.RatioUnit? unit = DeviceType.AllStatuses().Get("RobotSpeed").Unit as UnitsNet.Units.RatioUnit?;
                UnitsNet.Ratio newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("RobotSpeed", newValue);
            }
        }

        public UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus LoadPortStatus1
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus)GetStatusValue("LoadPortStatus1"); }
            protected set { SetStatusValue("LoadPortStatus1", value); }
        }

        public bool IsLoadPort1CarrierPresent
        {
            get { return (bool)GetStatusValue("IsLoadPort1CarrierPresent"); }
            protected set { SetStatusValue("IsLoadPort1CarrierPresent", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus LoadPortStatus2
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus)GetStatusValue("LoadPortStatus2"); }
            protected set { SetStatusValue("LoadPortStatus2", value); }
        }

        public bool IsLoadPort2CarrierPresent
        {
            get { return (bool)GetStatusValue("IsLoadPort2CarrierPresent"); }
            protected set { SetStatusValue("IsLoadPort2CarrierPresent", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus LoadPortStatus3
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus)GetStatusValue("LoadPortStatus3"); }
            protected set { SetStatusValue("LoadPortStatus3", value); }
        }

        public bool IsLoadPort3CarrierPresent
        {
            get { return (bool)GetStatusValue("IsLoadPort3CarrierPresent"); }
            protected set { SetStatusValue("IsLoadPort3CarrierPresent", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus LoadPortStatus4
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Efem.Enums.LoadPortStatus)GetStatusValue("LoadPortStatus4"); }
            protected set { SetStatusValue("LoadPortStatus4", value); }
        }

        public bool IsLoadPort4CarrierPresent
        {
            get { return (bool)GetStatusValue("IsLoadPort4CarrierPresent"); }
            protected set { SetStatusValue("IsLoadPort4CarrierPresent", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.Efem.Enums.AlignerStatus AlignerStatus
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Efem.Enums.AlignerStatus)GetStatusValue("AlignerStatus"); }
            protected set { SetStatusValue("AlignerStatus", value); }
        }

        public bool IsAlignerCarrierPresent
        {
            get { return (bool)GetStatusValue("IsAlignerCarrierPresent"); }
            protected set { SetStatusValue("IsAlignerCarrierPresent", value); }
        }

        public bool SafetyDoorSensor
        {
            get { return (bool)GetStatusValue("SafetyDoorSensor"); }
            protected set { SetStatusValue("SafetyDoorSensor", value); }
        }

        public bool VacuumSensor
        {
            get { return (bool)GetStatusValue("VacuumSensor"); }
            protected set { SetStatusValue("VacuumSensor", value); }
        }

        public bool AirSensor
        {
            get { return (bool)GetStatusValue("AirSensor"); }
            protected set { SetStatusValue("AirSensor", value); }
        }

        public bool FfuAlarm
        {
            get { return (bool)GetStatusValue("FfuAlarm"); }
            protected set { SetStatusValue("FfuAlarm", value); }
        }

        public bool IonizerAirState
        {
            get { return (bool)GetStatusValue("IonizerAirState"); }
            protected set { SetStatusValue("IonizerAirState", value); }
        }

        public bool IonizerAlarm
        {
            get { return (bool)GetStatusValue("IonizerAlarm"); }
            protected set { SetStatusValue("IonizerAlarm", value); }
        }

        public bool LightCurtainBeam
        {
            get { return (bool)GetStatusValue("LightCurtainBeam"); }
            protected set { SetStatusValue("LightCurtainBeam", value); }
        }

        public bool Interlock
        {
            get { return (bool)GetStatusValue("Interlock"); }
            protected set { SetStatusValue("Interlock", value); }
        }
    }
}
