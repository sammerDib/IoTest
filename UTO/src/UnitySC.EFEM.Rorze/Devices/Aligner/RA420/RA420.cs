using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Configuration;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Resources;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

using OperationMode = UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420
{
    public partial class RA420 : IConfigurableDevice<RA420Configuration>
    {
        #region Fields

        private const int NbSpeedLevels = 20;
        private AlignerDriver Driver { get; set; }

        private DriverWrapper DriverWrapper { get; set; }

        private List<Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader> Readers
        {
            get;
            set;
        }

        #endregion Fields

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    Readers = this.GetTopDeviceContainer()
                        .AllOfType<
                            Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader>()
                        .ToList();
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        Driver = new AlignerDriver(
                            Logger,
                            Configuration.CommunicationConfig.ConnectionMode,
                            aliveBitPeriod: Configuration.CommunicationConfig.AliveBitPeriod);
                        Driver.Setup(
                            Configuration.CommunicationConfig.IpAddress,
                            Configuration.CommunicationConfig.TcpPort,
                            Configuration.CommunicationConfig.AnswerTimeout,
                            Configuration.CommunicationConfig.ConfirmationTimeout,
                            Configuration.CommunicationConfig.InitTimeout,
                            maxNbRetry: Configuration.CommunicationConfig.MaxNbRetry,
                            connectionRetryDelay: Configuration.CommunicationConfig
                                .ConnectionRetryDelay);
                        Driver.StatusReceived += Driver_StatusReceived;
                        Driver.GpioReceived += Driver_GpioReceived;
                        Driver.GposReceived += Driver_GposReceived;
                        Driver.SubstratePresenceReceived += Driver_SubstratePresenceReceived;
                        Driver.SubstrateSizeReceived += Driver_SubstrateSizeReceived;
                        Driver.CommunicationEstablished += Driver_CommunicationEstablished;
                        Driver.CommunicationClosed += Driver_CommunicationClosed;
                        Driver.CommunicationStarted += Driver_CommunicationStarted;
                        Driver.CommunicationStopped += Driver_CommunicationStopped;
                        CommandExecutionStateChanged += RorzeAligner_CommandExecutionStateChanged;
                        DriverWrapper = new DriverWrapper(Driver, Logger);
                    }
                    else
                    {
                        SetUpSimulatedMode();
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                base.InternalInitialize(mustForceInit);
                if (OriginReturnCompletion == OriginReturnCompletion.Completed && !mustForceInit)
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.QuickInit(); },
                        AlignerCommand.Initialization);
                    foreach (var reader in this
                                 .AllDevices<
                                     Equipment.Abstractions.Devices.SubstrateIdReader.
                                     SubstrateIdReader>()
                                 .Where(r => r.State != OperatingModes.Idle))
                    {
                        reader.Initialize(true);
                    }

                    return;
                }

                DriverWrapper.RunCommand(
                    delegate { Driver.Initialization(); },
                    AlignerCommand.Initialization);
                foreach (var reader in this
                             .AllDevices<Equipment.Abstractions.Devices.SubstrateIdReader.
                                 SubstrateIdReader>())
                {
                    reader.Initialize(true);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IGenericDevice Commands

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            Driver.EnableCommunications();
        }

        protected override void InternalStopCommunication()
        {
            Driver.Disconnect();
        }

        #endregion ICommunicatingDevice Commands

        #region IAligner Commands

        protected override void InternalAlign(Angle target, AlignType alignType)
        {
            try
            {
                TreatSize(Location.Wafer.MaterialDimension, Location.Wafer.MaterialType);
                DriverWrapper.RunCommand(
                    delegate { Driver.Align(target.Degrees); },
                    AlignerCommand.Align);
                if (Location.Material != null)
                {
                    ((Wafer)Location.Material).OrientationAngle = target;
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalCentering()
        {
            try
            {
                TreatSize(Location.Wafer.MaterialDimension, Location.Wafer.MaterialType);
                DriverWrapper.RunCommand(
                    delegate { Driver.Align(0, AlignmentMode.PerformsSubstrateCenteringOnly); },
                    AlignerCommand.Align);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalPrepareTransfer(
            EffectorType effector,
            SampleDimension dimension,
            MaterialType materialType)
        {
            try
            {
                TreatSize(dimension, materialType);

                ZAxisMovement movement;
                switch (effector)
                {
                    case EffectorType.VacuumI:
                        movement = ZAxisMovement.MoveZAxisToVeryTop;
                        break;
                    case EffectorType.VacuumU:
                        movement = ZAxisMovement.MoveZAxisToVeryBottom;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(effector), effector, null);
                }

                DriverWrapper.RunCommand(
                    delegate { Driver.CancelSubstrateChuck(movement); },
                    AlignerCommand.CancelSubstrateChuck);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalClamp()
        {
            try
            {
                if (O_LightError)
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.ResetError(ResetErrorParameter.ResumeOrRetry); },
                        AlignerCommand.ResetError);
                }

                DriverWrapper.RunCommand(
                    delegate
                    {
                        // Substrate needs to be on the spindle, bottom position where vacuum holes are, in order to be clamped
                        Driver.ChuckSubstrate(ChuckSubstrateZAxisMovement.MoveZAxisToVeryBottom);
                    },
                    AlignerCommand.ChuckSubstrate);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalUnclamp()
        {
            try
            {
                if (O_LightError)
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.ResetError(ResetErrorParameter.ResumeOrRetry); },
                        AlignerCommand.ResetError);
                }

                DriverWrapper.RunCommand(
                    delegate { Driver.CancelSubstrateChuck(ZAxisMovement.NoZAxisMove); },
                    AlignerCommand.CancelSubstrateChuck);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalMoveZAxis(bool isBottom)
        {
            try
            {
                if (O_LightError)
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.ResetError(ResetErrorParameter.ResumeOrRetry); },
                        AlignerCommand.ResetError);
                }

                if (isBottom)
                {
                    DriverWrapper.RunCommand(
                        delegate
                        {
                            Driver.CancelSubstrateChuck(ZAxisMovement.MoveZAxisToVeryBottom);
                        },
                        AlignerCommand.CancelSubstrateChuck);
                }
                else
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.CancelSubstrateChuck(ZAxisMovement.MoveZAxisToVeryTop); },
                        AlignerCommand.CancelSubstrateChuck);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override void InternalSetDateAndTime()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.SetDateAndTime(); },
                    AlignerCommand.SetDateAndTime);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IAligner Commands

        #region IRA420 Commands

        protected virtual void InternalGetStatuses()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.GetStatuses(); },
                    AlignerCommand.GetStatuses);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IRA420 Commands

        #region Configuration

        public new RA420Configuration Configuration
            => ConfigurationExtension.Cast<RA420Configuration>(base.Configuration);

        public RA420Configuration CreateDefaultConfiguration()
        {
            return new RA420Configuration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.Aligner.Aligner)}/{nameof(RA420)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion Configuration

        #region EventHandlers

        private void Driver_StatusReceived(object sender, StatusEventArgs<AlignerStatus> args)
        {
            Logger.Debug(
                $"Driver's {nameof(Driver.StatusReceived)} event received. "
                + $"Source={args.SourceName}, Status={args.Status}.");

            // Update Statuses HW
            OperationMode = args.Status.OperationMode;
            OriginReturnCompletion = args.Status.OriginReturnCompletion;
            CommandProcessing = args.Status.CommandProcessing;
            OperationStatus = args.Status.OperationStatus;
            if (args.Status.MotionSpeed == 0)
            {
                IsNormalSpeed = true;
                MotionSpeedPercentage = "100%";
            }
            else
            {
                IsNormalSpeed = false;
                MotionSpeedPercentage = $"{NbSpeedLevels * args.Status.MotionSpeed}%";
            }

            ErrorControllerCode = ((int)args.Status.ErrorControllerId).ToString("X2");
            ErrorControllerName = args.Status.ErrorControllerId;
            if (args.Status.ErrorCode != Aligner.RA420.Driver.Enums.ErrorCode.None && CurrentCommand != nameof(Initialize))
            {
                //New alarm detected
                SetAlarmById(((int)args.Status.ErrorCode + 1000).ToString());
            }
            else
            {
                //Clear the previously set alarm
                ClearAlarmById(((int)ErrorDescription + 1000).ToString());
            }

            ErrorCode = ((int)args.Status.ErrorCode).ToString("X2");
            ErrorDescription = args.Status.ErrorCode;
            /* Update Generic Statuses from HW data */
            // While a command or a macro-command is executing, we just want to know that it is executing
            // We do not want the detail of knowing each time a part of the command ended
            // (e.g.: Device Initialize = Driver STIM+4*EVNT+... = 1 command)
            if (State != OperatingModes.Executing
                || (OperationStatus == OperationStatus.Stop
                    && CommandProcessing == CommandProcessing.Stop))
            {
                UpdateDeviceState();
            }
        }

        private void Driver_GpioReceived(object sender, StatusEventArgs<AlignerGpioStatus> args)
        {
            Logger.Debug(
                $"Driver's {nameof(Driver.GpioReceived)} event received. "
                + $"Source={args.SourceName}, Status={args.Status}.");

            // Inputs
            I_ExhaustFanRotating = args.Status.I_ExhaustFanRotating;
            I_SubstrateDetectionSensor1 = args.Status.I_SubstrateDetectionSensor1;
            I_SubstrateDetectionSensor2 = args.Status.I_SubstrateDetectionSensor2;

            // Outputs
            O_AlignerReadyToOperate = args.Status.O_AlignerReadyToOperate;
            O_TemporarilyStop = args.Status.O_TemporarilyStop;
            O_SignificantError = args.Status.O_SignificantError;
            O_LightError = args.Status.O_LightError;
            O_SubstrateDetection = args.Status.O_SubstrateDetection;
            O_AlignmentComplete = args.Status.O_AlignmentComplete;
            O_SpindleSolenoidValveChuckingOFF = args.Status.O_SpindleSolenoidValveChuckingOFF;
            O_SpindleSolenoidValveChuckingON = args.Status.O_SpindleSolenoidValveChuckingON;

            // Hypothesis:
            // Wafer is present if both aligner presence sensors indicate it's presence.
            // This is true at least for 300mm and 200mm wafers.
            // Aligner HW do not detect substrate when chucking is off: ignore detection inputs in such case
            var isWaferDetected = (I_SubstrateDetectionSensor1 && I_SubstrateDetectionSensor2)
                                  || O_SubstrateDetection;
            WaferPresence = isWaferDetected
                ? WaferPresence.Present
                : WaferPresence.Absent;

            IsClamped = O_SpindleSolenoidValveChuckingON;
        }

        private void Driver_GposReceived(object sender, StatusEventArgs<AlignerGposStatus> e)
        {
            if (e == null)
            {
                return;
            }

            XAxisPosition = e.Status.XAxisPosition;
            YAxisPosition = e.Status.YAxisPosition;
            ZAxisPosition = e.Status.ZAxisPosition;
        }

        private void Driver_SubstratePresenceReceived(
            object sender,
            StatusEventArgs<AlignerSubstratePresenceStatus> args)
        {
            Logger.Debug(
                $"Driver's {nameof(Driver.SubstratePresenceReceived)} event received. "
                + $"Source={args.SourceName}, Status={args.Status}.");
            /* NOTE:
             *  Do not update wafer presence because:
             *   - The presence is not updated by the RA420 event GWID when configured for 300mm wafers
             *   - However, event correctly worked using 200mm wafers
             *   - Bindings and code are not removed for history and to keep a quick access in case we would reuse this code.
             */
        }

        private void Driver_SubstrateSizeReceived(
            object sender,
            StatusEventArgs<AlignerSubstrateSizeStatus> args)
        {
            if (args == null)
            {
                return;
            }

            SelectedSize = args.Status.SubstrateSize;
            SelectedSubstrateSize = Configuration.SubstrateInformationsPerPositions.TryGetValue(
                args.Status.SubstrateSize,
                out var position)
                ? position.SubstrateSize
                : SampleDimension.NoDimension;
            SelectedMaterialType = Configuration.SubstrateInformationsPerPositions.TryGetValue(
                args.Status.SubstrateSize,
                out var size)
                ? size.MaterialType
                : MaterialType.SiliconWithNotch;
        }

        private void Driver_CommunicationEstablished(object sender, EventArgs e)
        {
            IsCommunicationStarted = IsCommunicating = true;
            Task.Factory.StartNew(
                () =>
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.InitializeCommunication(); },
                        AlignerCommand.InitializeCommunication);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        AlignerCommand.GetStatuses);
                });
        }

        private void Driver_CommunicationClosed(object sender, EventArgs e)
        {
            IsCommunicating = false;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStopped(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStarted(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
        }

        private void RorzeAligner_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            // Update device state on command ended
            if (e.PreviousState != ExecutionState.Running)
            {
                return;
            }

            UpdateDeviceState();
        }

        #endregion EventHandlers

        #region Other Methods

        public override void CheckSubstrateDetectionError(bool reset = false)
        {
            if ((WaferPresence == WaferPresence.Present && Location.Wafer != null)
                || (WaferPresence == WaferPresence.Absent && Location.Wafer == null)
                || reset)
            {
                SubstrateDetectionError = false;
            }
            else
            {
                if (ZAxisPosition == ZAxisPosition.ZAxisAtVeryBottom
                    || WaferPresence == WaferPresence.Unknown)
                {
                    SubstrateDetectionError = true;
                }
            }
        }

        public override bool IsReadyForTransfer(
            EffectorType effector,
            out List<string> errorMessages,
            Material armMaterial = null,
            byte slot = 1)
        {
            // Generic check of state and material presence
            base.IsReadyForTransfer(effector, out errorMessages, armMaterial, slot);

            // Check material is allowed on aligner
            if (armMaterial is Frame _)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.MaterialNotAllowed,
                        armMaterial.GetType().Name));
            }

            // Check selected size on aligner
            var dimension = (armMaterial as Substrate)?.MaterialDimension ?? GetMaterialDimension();
            if (dimension != SelectedSubstrateSize)
            {
                errorMessages.Add(Messages.WrongSize);
            }

            // Check selected size on aligner
            var materialType = (armMaterial as Wafer)?.MaterialType ?? GetMaterialType();
            if (materialType != SelectedMaterialType)
            {
                errorMessages.Add(Messages.WrongMaterialType);
            }

            // Check pin state depending on end-effector
            switch (effector)
            {
                case EffectorType.VacuumI:
                    if (ZAxisPosition != ZAxisPosition.ZAxisAtVeryTop)
                    {
                        errorMessages.Add(Messages.PinsNotUp);
                    }

                    break;
                case EffectorType.VacuumU:
                    if (ZAxisPosition != ZAxisPosition.ZAxisAtVeryBottom)
                    {
                        errorMessages.Add(Messages.PinsNotDown);
                    }

                    break;
                default:
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.EndEffectorNotAllowed,
                            effector.ToString()));
                    break;
            }

            return errorMessages.Count == 0;
        }

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (ExecutionMode == ExecutionMode.Real
                && (OperationStatus == OperationStatus.Moving
                    || CommandProcessing == CommandProcessing.Processing))
            {
                //Call STOP on driver only if the device is moving
                DriverWrapper?.InterruptTask();
                Driver.EmergencyStop();
            }
        }

        private void UpdateDeviceState()
        {
            if (!IsCommunicating)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (ErrorDescription != Aligner.RA420.Driver.Enums.ErrorCode.None)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (OperationMode == OperationMode.Initializing)
            {
                SetState(OperatingModes.Initialization);
            }
            else if (OperationStatus == OperationStatus.Moving
                     || CommandProcessing == CommandProcessing.Processing)
            {
                SetState(OperatingModes.Executing);
            }
            else if (OperationStatus == OperationStatus.Stop
                     && CommandProcessing == CommandProcessing.Stop)
            {
                SetState(
                    OriginReturnCompletion != OriginReturnCompletion.Completed
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
            else if (OperationStatus == OperationStatus.Pause)
            {
                SetState(OperatingModes.Executing); // Maybe we'll need an additional 'Pause' status
            }
            else
            {
                SetState(
                    OriginReturnCompletion == OriginReturnCompletion.Completed
                        ? OperatingModes.Idle
                        : OperatingModes.Maintenance);
            }
        }

        private void TreatSize(SampleDimension dimension, MaterialType materialType)
        {
            if (O_LightError)
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.ResetError(ResetErrorParameter.ResumeOrRetry); },
                    AlignerCommand.ResetError);
            }

            DriverWrapper.RunCommand(
                delegate { Driver.GetStatuses(); },
                AlignerCommand.GetStatuses);

            if (SelectedSubstrateSize != dimension || SelectedMaterialType != materialType)
            {
                //Retrieve configuration to find size according to wafer's dimension and type
                var sampleDimensionPerPosition =
                    Configuration.SubstrateInformationsPerPositions.FirstOrDefault(
                        x => x.Value.SubstrateSize == dimension
                             && x.Value.MaterialType == materialType);

                if (sampleDimensionPerPosition.Equals(default(KeyValuePair<uint, SubstrateInformations>)))
                {
                    throw new InvalidOperationException(
                        $"Substrate informations with dimension = {Location.Wafer.MaterialDimension} and material type = {Location.Wafer.MaterialType} have not been configured in Aligner.");
                }

                DriverWrapper.RunCommand(delegate { Driver.SetSize(sampleDimensionPerPosition.Key); }, AlignerCommand.SetSize);
                DriverWrapper.RunCommand(delegate { Driver.GoHome(); }, AlignerCommand.GoHome);
            }

            DriverWrapper.RunCommand(
                delegate { Driver.GetStatuses(); },
                AlignerCommand.GetStatuses);

            //Used to set the reader to the correct position too
            if (Readers.Count > 0
                && Readers[0].Positioner != null
                && Readers[0].Positioner.CurrentPosition != dimension)
            {
                Readers[0].Positioner.SetPosition(dimension);
            }
        }

        #endregion Other Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Driver != null)
                {
                    Driver.StatusReceived -= Driver_StatusReceived;
                    Driver.GpioReceived -= Driver_GpioReceived;
                    Driver.GposReceived -= Driver_GposReceived;
                    Driver.SubstratePresenceReceived -= Driver_SubstratePresenceReceived;
                    Driver.SubstrateSizeReceived -= Driver_SubstrateSizeReceived;
                    Driver.CommunicationEstablished -= Driver_CommunicationEstablished;
                    Driver.CommunicationClosed -= Driver_CommunicationClosed;
                    Driver.CommunicationStarted -= Driver_CommunicationStarted;
                    Driver.CommunicationStopped -= Driver_CommunicationStopped;
                    Driver = null;
                }

                if (SimulationData != null)
                {
                    DisposeSimulatedMode();
                }

                CommandExecutionStateChanged -= RorzeAligner_CommandExecutionStateChanged;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
