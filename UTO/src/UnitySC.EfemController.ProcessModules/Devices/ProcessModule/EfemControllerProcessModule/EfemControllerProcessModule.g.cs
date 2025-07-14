using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EfemController.ProcessModules.Devices.ProcessModule.EfemControllerProcessModule
{
    public partial class EfemControllerProcessModule : UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule, IEfemControllerProcessModule
    {
        public static new readonly DeviceType Type;

        static EfemControllerProcessModule()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EfemController.ProcessModules.Devices.ProcessModule.EfemControllerProcessModule.EfemControllerProcessModule.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EfemController.ProcessModules.Devices.ProcessModule.EfemControllerProcessModule.EfemControllerProcessModule");
            }
        }

        public EfemControllerProcessModule(string name)
            : this(name, Type)
        {
        }

        protected EfemControllerProcessModule(string name, DeviceType type)
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
