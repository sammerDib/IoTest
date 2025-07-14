using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.Efem.MediumSizeEfem
{
    public partial class MediumSizeEfem : UnitySC.Equipment.Abstractions.Devices.Efem.Efem, IMediumSizeEfem
    {
        public static new readonly DeviceType Type;

        static MediumSizeEfem()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.Efem.MediumSizeEfem.MediumSizeEfem.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.Efem.MediumSizeEfem.MediumSizeEfem");
            }
        }

        public MediumSizeEfem(string name)
            : this(name, Type)
        {
        }

        protected MediumSizeEfem(string name, DeviceType type)
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
