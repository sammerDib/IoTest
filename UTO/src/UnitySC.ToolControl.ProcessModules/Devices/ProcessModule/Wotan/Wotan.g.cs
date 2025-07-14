using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.Wotan
{
    public partial class Wotan : UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule.ToolControlProcessModule, IWotan
    {
        public static new readonly DeviceType Type;

        static Wotan()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.Wotan.Wotan.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.Wotan.Wotan");
            }
        }

        public Wotan(string name)
            : this(name, Type)
        {
        }

        protected Wotan(string name, DeviceType type)
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
