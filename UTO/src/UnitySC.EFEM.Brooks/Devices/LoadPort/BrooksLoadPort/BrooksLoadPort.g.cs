using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort
{
    public partial class BrooksLoadPort : UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort, IBrooksLoadPort
    {
        public static new readonly DeviceType Type;

        static BrooksLoadPort()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort.BrooksLoadPort.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort.BrooksLoadPort");
            }
        }

        public BrooksLoadPort(string name)
            : this(name, Type)
        {
        }

        protected BrooksLoadPort(string name, DeviceType type)
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

        public Agileo.SemiDefinitions.LightState PlacementLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("PlacementLightState"); }
            protected set { SetStatusValue("PlacementLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState PodLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("PodLightState"); }
            protected set { SetStatusValue("PodLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState PresenceLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("PresenceLightState"); }
            protected set { SetStatusValue("PresenceLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState ReadyLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("ReadyLightState"); }
            protected set { SetStatusValue("ReadyLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState ServiceLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("ServiceLightState"); }
            protected set { SetStatusValue("ServiceLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led1LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led1LightState"); }
            protected set { SetStatusValue("Led1LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led2LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led2LightState"); }
            protected set { SetStatusValue("Led2LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led3LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led3LightState"); }
            protected set { SetStatusValue("Led3LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led4LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led4LightState"); }
            protected set { SetStatusValue("Led4LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led5LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led5LightState"); }
            protected set { SetStatusValue("Led5LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led6LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led6LightState"); }
            protected set { SetStatusValue("Led6LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led7LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led7LightState"); }
            protected set { SetStatusValue("Led7LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led8LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led8LightState"); }
            protected set { SetStatusValue("Led8LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led9LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led9LightState"); }
            protected set { SetStatusValue("Led9LightState", value); }
        }

        public Agileo.SemiDefinitions.LightState Led10LightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("Led10LightState"); }
            protected set { SetStatusValue("Led10LightState", value); }
        }

        public bool InfoPadA
        {
            get { return (bool)GetStatusValue("InfoPadA"); }
            protected set { SetStatusValue("InfoPadA", value); }
        }

        public bool InfoPadB
        {
            get { return (bool)GetStatusValue("InfoPadB"); }
            protected set { SetStatusValue("InfoPadB", value); }
        }

        public bool InfoPadC
        {
            get { return (bool)GetStatusValue("InfoPadC"); }
            protected set { SetStatusValue("InfoPadC", value); }
        }

        public bool InfoPadD
        {
            get { return (bool)GetStatusValue("InfoPadD"); }
            protected set { SetStatusValue("InfoPadD", value); }
        }

        public bool InfoPadE
        {
            get { return (bool)GetStatusValue("InfoPadE"); }
            protected set { SetStatusValue("InfoPadE", value); }
        }

        public bool InfoPadF
        {
            get { return (bool)GetStatusValue("InfoPadF"); }
            protected set { SetStatusValue("InfoPadF", value); }
        }
    }
}
