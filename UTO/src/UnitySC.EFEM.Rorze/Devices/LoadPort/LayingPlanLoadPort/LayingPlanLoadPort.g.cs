using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort
{
    public partial class LayingPlanLoadPort : UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort, ILayingPlanLoadPort
    {
        public static new readonly DeviceType Type;

        static LayingPlanLoadPort()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort.LayingPlanLoadPort.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort.LayingPlanLoadPort");
            }
        }

        public LayingPlanLoadPort(string name)
            : this(name, Type)
        {
        }

        protected LayingPlanLoadPort(string name, DeviceType type)
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

        public bool PlacementSensorA
        {
            get { return (bool)GetStatusValue("PlacementSensorA"); }
            protected set { SetStatusValue("PlacementSensorA", value); }
        }

        public bool PlacementSensorB
        {
            get { return (bool)GetStatusValue("PlacementSensorB"); }
            protected set { SetStatusValue("PlacementSensorB", value); }
        }

        public bool PlacementSensorC
        {
            get { return (bool)GetStatusValue("PlacementSensorC"); }
            protected set { SetStatusValue("PlacementSensorC", value); }
        }

        public bool PlacementSensorD
        {
            get { return (bool)GetStatusValue("PlacementSensorD"); }
            protected set { SetStatusValue("PlacementSensorD", value); }
        }

        public bool WaferProtrudeSensor1
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor1"); }
            protected set { SetStatusValue("WaferProtrudeSensor1", value); }
        }

        public bool WaferProtrudeSensor2
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor2"); }
            protected set { SetStatusValue("WaferProtrudeSensor2", value); }
        }

        public bool WaferProtrudeSensor3
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor3"); }
            protected set { SetStatusValue("WaferProtrudeSensor3", value); }
        }

        public bool MappingRequested
        {
            get { return (bool)GetStatusValue("MappingRequested"); }
            protected set { SetStatusValue("MappingRequested", value); }
        }
    }
}
