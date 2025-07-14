using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.ProcessModule
{
    public abstract partial class ProcessModule : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IProcessModule
    {
        public static new readonly DeviceType Type;

        static ProcessModule()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule");
            }
        }

        public ProcessModule(string name)
            : this(name, Type)
        {
        }

        protected ProcessModule(string name, DeviceType type)
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

        public bool IsOutOfService
        {
            get { return (bool)GetStatusValue("IsOutOfService"); }
            protected set { SetStatusValue("IsOutOfService", value); }
        }

        public bool IsDoorOpen
        {
            get { return (bool)GetStatusValue("IsDoorOpen"); }
            protected set { SetStatusValue("IsDoorOpen", value); }
        }

        public bool IsReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("IsReadyToLoadUnload"); }
            protected set { SetStatusValue("IsReadyToLoadUnload", value); }
        }

        public Agileo.SemiDefinitions.SampleDimension WaferDimension
        {
            get { return (Agileo.SemiDefinitions.SampleDimension)GetStatusValue("WaferDimension"); }
            protected set { SetStatusValue("WaferDimension", value); }
        }

        public string SimplifiedWaferId
        {
            get { return (string)GetStatusValue("SimplifiedWaferId"); }
            protected set { SetStatusValue("SimplifiedWaferId", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus WaferStatus
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus)GetStatusValue("WaferStatus"); }
            protected set { SetStatusValue("WaferStatus", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence WaferPresence
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence)GetStatusValue("WaferPresence"); }
            protected set { SetStatusValue("WaferPresence", value); }
        }

        public bool SubstrateDetectionError
        {
            get { return (bool)GetStatusValue("SubstrateDetectionError"); }
            protected set { SetStatusValue("SubstrateDetectionError", value); }
        }
    }
}
