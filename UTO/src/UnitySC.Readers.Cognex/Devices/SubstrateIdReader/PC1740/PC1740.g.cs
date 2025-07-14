using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740
{
    public partial class PC1740 : UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader, IPC1740
    {
        public static new readonly DeviceType Type;

        static PC1740()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.PC1740.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.PC1740");
            }
        }

        public PC1740(string name)
            : this(name, Type)
        {
        }

        protected PC1740(string name, DeviceType type)
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
