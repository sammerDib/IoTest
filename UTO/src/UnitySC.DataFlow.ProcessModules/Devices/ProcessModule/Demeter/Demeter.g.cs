using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter
{
    public partial class Demeter : UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.UnityProcessModule, IDemeter
    {
        public static new readonly DeviceType Type;

        static Demeter()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter.Demeter.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter.Demeter");
            }
        }

        public Demeter(string name)
            : this(name, Type)
        {
        }

        protected Demeter(string name, DeviceType type)
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

        public string AcquisitionWaferId
        {
            get { return (string)GetStatusValue("AcquisitionWaferId"); }
            protected set { SetStatusValue("AcquisitionWaferId", value); }
        }

        public UnitsNet.Ratio AcquisitionProgress
        {
            get { return (UnitsNet.Ratio)GetStatusValue("AcquisitionProgress"); }
            protected set
            {
                UnitsNet.Units.RatioUnit? unit = DeviceType.AllStatuses().Get("AcquisitionProgress").Unit as UnitsNet.Units.RatioUnit?;
                UnitsNet.Ratio newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("AcquisitionProgress", newValue);
            }
        }

        public string CalculationWaferId
        {
            get { return (string)GetStatusValue("CalculationWaferId"); }
            protected set { SetStatusValue("CalculationWaferId", value); }
        }

        public UnitsNet.Ratio CalculationProgress
        {
            get { return (UnitsNet.Ratio)GetStatusValue("CalculationProgress"); }
            protected set
            {
                UnitsNet.Units.RatioUnit? unit = DeviceType.AllStatuses().Get("CalculationProgress").Unit as UnitsNet.Units.RatioUnit?;
                UnitsNet.Ratio newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("CalculationProgress", newValue);
            }
        }
    }
}
