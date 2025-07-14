using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem
{
    public partial class BrooksEfem : UnitySC.Equipment.Abstractions.Devices.Efem.Efem, IBrooksEfem
    {
        public static new readonly DeviceType Type;

        static BrooksEfem()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem.BrooksEfem.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem.BrooksEfem");
            }
        }

        public BrooksEfem(string name)
            : this(name, Type)
        {
        }

        protected BrooksEfem(string name, DeviceType type)
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
    }
}
