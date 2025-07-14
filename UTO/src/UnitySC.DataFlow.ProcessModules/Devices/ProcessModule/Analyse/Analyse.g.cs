using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Analyse
{
    public partial class Analyse : UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.UnityProcessModule, IAnalyse
    {
        public static new readonly DeviceType Type;

        static Analyse()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Analyse.Analyse.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Analyse.Analyse");
            }
        }

        public Analyse(string name)
            : this(name, Type)
        {
        }

        protected Analyse(string name, DeviceType type)
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
    }
}
