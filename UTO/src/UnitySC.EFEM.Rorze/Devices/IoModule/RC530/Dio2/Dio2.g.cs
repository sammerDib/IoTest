using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2
{
    public partial class Dio2 : UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.GenericRC5xx, IDio2
    {
        public static new readonly DeviceType Type;

        static Dio2()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Dio2.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Dio2");
            }
        }

        public Dio2(string name)
            : this(name, Type)
        {
        }

        protected Dio2(string name, DeviceType type)
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

        public UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
            protected set { SetStatusValue("ErrorDescription", value); }
        }

        public bool I_PM1_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM1_DoorOpened"); }
            protected set { SetStatusValue("I_PM1_DoorOpened", value); }
        }

        public bool I_PM1_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM1_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM1_ReadyToLoadUnload", value); }
        }

        public bool I_PM2_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM2_DoorOpened"); }
            protected set { SetStatusValue("I_PM2_DoorOpened", value); }
        }

        public bool I_PM2_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM2_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM2_ReadyToLoadUnload", value); }
        }

        public bool I_PM3_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM3_DoorOpened"); }
            protected set { SetStatusValue("I_PM3_DoorOpened", value); }
        }

        public bool I_PM3_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM3_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM3_ReadyToLoadUnload", value); }
        }

        public bool O_RobotArmNotExtended_PM1
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM1"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM1", value); }
        }

        public bool O_RobotArmNotExtended_PM2
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM2"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM2", value); }
        }

        public bool O_RobotArmNotExtended_PM3
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM3"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM3", value); }
        }
    }
}
