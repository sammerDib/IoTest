using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.RecipeProcessModule
{
    public abstract partial class RecipeProcessModule : UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule.ProcessModule, IRecipeProcessModule
    {
        public static new readonly DeviceType Type;

        static RecipeProcessModule()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Vendor.Devices.RecipeProcessModule.RecipeProcessModule.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Vendor.Devices.RecipeProcessModule.RecipeProcessModule");
            }
        }

        public RecipeProcessModule(string name)
            : this(name, Type)
        {
        }

        protected RecipeProcessModule(string name, DeviceType type)
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

        public bool IsProgramLoaded
        {
            get { return (bool)GetStatusValue("IsProgramLoaded"); }
            protected set { SetStatusValue("IsProgramLoaded", value); }
        }

        public Agileo.ProcessingFramework.ProcessorState ProcessorState
        {
            get { return (Agileo.ProcessingFramework.ProcessorState)GetStatusValue("ProcessorState"); }
            protected set { SetStatusValue("ProcessorState", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums.ProgramExecutionState ProgramExecutionState
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums.ProgramExecutionState)GetStatusValue("ProgramExecutionState"); }
            protected set { SetStatusValue("ProgramExecutionState", value); }
        }
    }
}
