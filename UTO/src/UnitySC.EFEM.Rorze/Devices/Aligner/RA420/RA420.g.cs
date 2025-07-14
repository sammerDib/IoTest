using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420
{
    public partial class RA420 : UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner, IRA420
    {
        public static new readonly DeviceType Type;

        static RA420()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.Aligner.RA420.RA420.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.Aligner.RA420.RA420");
            }
        }

        public RA420(string name)
            : this(name, Type)
        {
        }

        protected RA420(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "GetStatuses":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGetStatuses();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGetStatuses(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GetStatuses interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                default:
                    base.InternalRun(execution);
                    break;
            }
        }

        public void GetStatuses()
        {
            CommandExecution execution = new CommandExecution(this, "GetStatuses");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GetStatusesAsync()
        {
            CommandExecution execution = new CommandExecution(this, "GetStatuses");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OperationMode OperationMode
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OperationMode)GetStatusValue("OperationMode"); }
            protected set { SetStatusValue("OperationMode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OriginReturnCompletion OriginReturnCompletion
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OriginReturnCompletion)GetStatusValue("OriginReturnCompletion"); }
            protected set { SetStatusValue("OriginReturnCompletion", value); }
        }

        public UnitySC.EFEM.Rorze.Drivers.Enums.CommandProcessing CommandProcessing
        {
            get { return (UnitySC.EFEM.Rorze.Drivers.Enums.CommandProcessing)GetStatusValue("CommandProcessing"); }
            protected set { SetStatusValue("CommandProcessing", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OperationStatus OperationStatus
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OperationStatus)GetStatusValue("OperationStatus"); }
            protected set { SetStatusValue("OperationStatus", value); }
        }

        public bool IsNormalSpeed
        {
            get { return (bool)GetStatusValue("IsNormalSpeed"); }
            protected set { SetStatusValue("IsNormalSpeed", value); }
        }

        public string MotionSpeedPercentage
        {
            get { return (string)GetStatusValue("MotionSpeedPercentage"); }
            protected set { SetStatusValue("MotionSpeedPercentage", value); }
        }

        public string ErrorControllerCode
        {
            get { return (string)GetStatusValue("ErrorControllerCode"); }
            protected set { SetStatusValue("ErrorControllerCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.ErrorControllerId ErrorControllerName
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.ErrorControllerId)GetStatusValue("ErrorControllerName"); }
            protected set { SetStatusValue("ErrorControllerName", value); }
        }

        public string ErrorCode
        {
            get { return (string)GetStatusValue("ErrorCode"); }
            protected set { SetStatusValue("ErrorCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
            protected set { SetStatusValue("ErrorDescription", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.XAxisPosition XAxisPosition
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.XAxisPosition)GetStatusValue("XAxisPosition"); }
            protected set { SetStatusValue("XAxisPosition", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.YAxisPosition YAxisPosition
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.YAxisPosition)GetStatusValue("YAxisPosition"); }
            protected set { SetStatusValue("YAxisPosition", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.ZAxisPosition ZAxisPosition
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.ZAxisPosition)GetStatusValue("ZAxisPosition"); }
            protected set { SetStatusValue("ZAxisPosition", value); }
        }

        public uint SelectedSize
        {
            get { return (uint)GetStatusValue("SelectedSize"); }
            protected set { SetStatusValue("SelectedSize", value); }
        }

        public UnitySC.Equipment.Abstractions.Enums.MaterialType SelectedMaterialType
        {
            get { return (UnitySC.Equipment.Abstractions.Enums.MaterialType)GetStatusValue("SelectedMaterialType"); }
            protected set { SetStatusValue("SelectedMaterialType", value); }
        }

        public Agileo.SemiDefinitions.SampleDimension SelectedSubstrateSize
        {
            get { return (Agileo.SemiDefinitions.SampleDimension)GetStatusValue("SelectedSubstrateSize"); }
            protected set { SetStatusValue("SelectedSubstrateSize", value); }
        }

        public bool I_ExhaustFanRotating
        {
            get { return (bool)GetStatusValue("I_ExhaustFanRotating"); }
            protected set { SetStatusValue("I_ExhaustFanRotating", value); }
        }

        public bool I_SubstrateDetectionSensor1
        {
            get { return (bool)GetStatusValue("I_SubstrateDetectionSensor1"); }
            protected set { SetStatusValue("I_SubstrateDetectionSensor1", value); }
        }

        public bool I_SubstrateDetectionSensor2
        {
            get { return (bool)GetStatusValue("I_SubstrateDetectionSensor2"); }
            protected set { SetStatusValue("I_SubstrateDetectionSensor2", value); }
        }

        public bool O_AlignerReadyToOperate
        {
            get { return (bool)GetStatusValue("O_AlignerReadyToOperate"); }
            protected set { SetStatusValue("O_AlignerReadyToOperate", value); }
        }

        public bool O_TemporarilyStop
        {
            get { return (bool)GetStatusValue("O_TemporarilyStop"); }
            protected set { SetStatusValue("O_TemporarilyStop", value); }
        }

        public bool O_SignificantError
        {
            get { return (bool)GetStatusValue("O_SignificantError"); }
            protected set { SetStatusValue("O_SignificantError", value); }
        }

        public bool O_LightError
        {
            get { return (bool)GetStatusValue("O_LightError"); }
            protected set { SetStatusValue("O_LightError", value); }
        }

        public bool O_SubstrateDetection
        {
            get { return (bool)GetStatusValue("O_SubstrateDetection"); }
            protected set { SetStatusValue("O_SubstrateDetection", value); }
        }

        public bool O_AlignmentComplete
        {
            get { return (bool)GetStatusValue("O_AlignmentComplete"); }
            protected set { SetStatusValue("O_AlignmentComplete", value); }
        }

        public bool O_SpindleSolenoidValveChuckingOFF
        {
            get { return (bool)GetStatusValue("O_SpindleSolenoidValveChuckingOFF"); }
            protected set { SetStatusValue("O_SpindleSolenoidValveChuckingOFF", value); }
        }

        public bool O_SpindleSolenoidValveChuckingON
        {
            get { return (bool)GetStatusValue("O_SpindleSolenoidValveChuckingON"); }
            protected set { SetStatusValue("O_SpindleSolenoidValveChuckingON", value); }
        }
    }
}
