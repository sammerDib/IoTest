using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Agileo.EquipmentModeling;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Emera
{
    public partial class Emera : UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.UnityProcessModule, IEmera
    {
        public static new readonly DeviceType Type;

        static Emera()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Emera.Emera.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Emera.Emera");
            }
        }

        public Emera(string name)
            : this(name, Type)
        {
        }

        protected Emera(string name, DeviceType type)
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
