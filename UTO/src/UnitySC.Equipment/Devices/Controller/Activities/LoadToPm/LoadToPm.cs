using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;
using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    /// <summary>
    /// Transfer a Substrate From a Load Stage to the Process Module Supported Commands are:
    /// <list type="bullet">
    /// <item>Start the Activity</item> <item>Abort the Activity</item>
    /// </list>
    /// </summary>
    internal partial class LoadToPm : MachineActivity
    {
        #region Fields

        private readonly LoadToPmConfiguration _config;
        private readonly EquipmentSubstrateLocation _robotArmLocation;
        private readonly Abstractions.Devices.Robot.Robot _robot;
        private readonly Abstractions.Devices.Aligner.Aligner _aligner;

        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="LoadToPm" /> class.</summary>
        /// <param name="configuration">The activity configuration.</param>
        public LoadToPm(LoadToPmConfiguration configuration)
            : base(nameof(LoadToPm), configuration.Equipment.TryGetDevice<Controller>())
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration),
                    "Provided configuration cannot be null. Activity cannot start.");
            }

            _config = configuration;
            _robot = Efem.TryGetDevice<Abstractions.Devices.Robot.Robot>();
            _aligner = Efem.TryGetDevice<Abstractions.Devices.Aligner.Aligner>();

            _robotArmLocation = _config.RobotArm == RobotArm.Arm1
                ? _robot.UpperArmLocation
                : _robot.LowerArmLocation;

            CreateStateMachine();
            StateMachine = m_LoadToPm;
        }

        #endregion Constructor

        #region Activity Overrides

        /// <summary>
        /// In order to connect the State Machine to equipment events, it is required to finalize the Start
        /// Command for each sub-activities. This is the aim of the overridden method
        /// </summary>
        /// <returns>OK if succeeded, ErrorCode otherwise</returns>
        protected override ErrorDescription StartFinalizer()
        {
            return new();
        }

        /// <summary>
        /// Check if the activity can be started. It validates the context and parameters. The activity can NOT
        /// starts if:
        /// <list type="bullet">
        /// <item>Another Activity is in progress</item>
        /// <item>
        /// At least 1 device is not supported on the equipment (Robot, LP, Aligner, PM)
        /// </item>
        /// <item>Used Devices are not Idle</item>
        /// <item>Selected substrate is not correctly placed on the equipment</item>
        /// <item>Wafers are already in the equipment and do not allow the sequence</item>
        /// </list>
        /// </summary>
        /// <param name="context">
        /// Activity Manager is provided to provide the context of the required execution
        /// </param>
        /// <returns>Error description. In case of succeed, Status = OK.</returns>
        public override ErrorDescription Check(ActivityManager context)
        {
            var err = base.Check(context);
            if (err.ErrorCode != ErrorCode.Ok)
            {
                return err;
            }

            if (_robot.State != OperatingModes.Idle
                || _config.LoadPort.State != OperatingModes.Idle
                || _aligner.State != OperatingModes.Idle)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = "At least one Device is not IDLE. Activity cannot start";
            }
            else if (_config.LoadPort.PhysicalState != LoadPortState.Open
                     && !_config.LoadPort.Configuration.CloseDoorAfterRobotAction)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText =
                    $"Carrier on Load Port {_config.LoadPort.InstanceId} is not accessible. Activity cannot start.";
            }
            else if (_config.LoadPort.Carrier == null
                     || _config.LoadPort.CarrierPresence != CassettePresence.Correctly)
            {
                err.ErrorCode = ErrorCode.UnknownObjectInstance;
                err.ErrorText =
                    $"Carrier on Load Port {_config.LoadPort.InstanceId} is absent or misplaced. Activity cannot start.";
            }
            else if (_config.SourceSlot < 1
                     || _config.SourceSlot > _config.LoadPort.Carrier.Capacity)
            {
                err.ErrorCode = ErrorCode.UnknownObjectInstance;
                err.ErrorText =
                    $"Provided source slot {_config.SourceSlot} does not match carrier capacity '{_config.LoadPort.Carrier.Capacity}. Activity cannot start.";
            }
            else if (_config.LoadPort.Carrier.MaterialLocations.ElementAt(_config.SourceSlot - 1)
                     == null
                     || _config.LoadPort.Carrier.MappingTable[_config.SourceSlot - 1]
                     != SlotState.HasWafer)
            {
                err.ErrorCode = ErrorCode.UnknownObjectInstance;
                err.ErrorText =
                    $"Slot {_config.SourceSlot} doesn't have any substrate correctly placed. Activity cannot start.";
            }
            else if (_robotArmLocation.Substrate != null)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText =
                    $"Material is present on {_robotArmLocation.Name}. Activity cannot start.";
            }
            else if (_aligner.Location.Material != null)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = "Material is present on Aligner. Activity cannot start.";
            }
            else if (_config.ProcessModule.Location.Material != null)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = "Material is present on Process Module. Activity cannot start.";
            }
            else if (!_config.ProcessModule.SupportedSampleDimensions.Contains(_config.LoadPort.Carrier.SampleSize))
            {
                err.ErrorCode = ErrorCode.ValidationError;
                err.ErrorText =
                    $"Material Sizes ({_config.LoadPort.Carrier.SampleSize}) is not supported on Process module. Activity cannot start.";
            }
            else if (_config.ProcessModule.IsOutOfService)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = "Process module is out of service. Activity cannot start.";
            }

            return err;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">
        /// indicates whether the method call comes from a Dispose method (its value is true) or from a
        /// destructor (its value is false).
        /// </param>
        protected override void Dispose(bool disposing)
        {
            m_LoadToPm.Dispose();
            base.Dispose(disposing);
        }

        public override ErrorDescription Abort()
        {
            //Abort all devices involved in the activity
            _robot.InterruptAsync(InterruptionKind.Abort);
            _aligner.InterruptAsync(InterruptionKind.Abort);
            _config.ProcessModule.InterruptAsync(InterruptionKind.Abort);
            _config.LoadPort.InterruptAsync(InterruptionKind.Abort);

            return base.Abort();
        }

        #endregion

        #region Action State Machine

        private void PickFromLoadPortEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    _config.LoadPort.PrepareForTransfer();
                    _robot.Pick(_config.RobotArm, _config.LoadPort, _config.SourceSlot);
                    _config.LoadPort.PostTransfer();
                },
                new RobotDone());
        }

        private void PrepareAlignerPlaceEntry(Event ev)
        {
            PerformAction(
                () => _aligner.PrepareTransfer(
                    _config.RobotArm == RobotArm.Arm1
                        ? _robot.Configuration.UpperArm.EffectorType
                        : _robot.Configuration.LowerArm.EffectorType,
                    _config.RobotArm == RobotArm.Arm1
                        ? _robot.UpperArmWaferDimension
                        : _robot.LowerArmWaferDimension,
                    _config.RobotArm == RobotArm.Arm1
                        ? _robot.UpperArmLocation.Wafer.MaterialType
                        : _robot.LowerArmLocation.Wafer.MaterialType),
                new AlignerDone());
        }

        private void PrepareAlignerPickEntry(Event ev)
        {
            PerformAction(
                () => _aligner.PrepareTransfer(
                    _config.RobotArm == RobotArm.Arm1
                        ? _robot.Configuration.UpperArm.EffectorType
                        : _robot.Configuration.LowerArm.EffectorType,
                    _aligner.GetMaterialDimension(),
                    _aligner.GetMaterialType()),
                new AlignerDone());
        }

        private void PlaceToAlignerEntry(Event ev)
        {
            PerformAction(() => { _robot.Place(_config.RobotArm, _aligner, 1); }, new RobotDone());
        }

        private void AlignEntry(Event ev)
        {
            PerformAction(
                () => _aligner.Align(_config.AlignAngle, _config.AlignType),
                new AlignerDone());
        }

        private void PickFromAlignerEntry(Event ev)
        {
            PerformAction(() => _robot.Pick(_config.RobotArm, _aligner, 1), new RobotDone());
        }

        private void PrepareForTransferEntry(Event ev)
        {
            PerformAction(
                () => _config.ProcessModule.PrepareTransferAsync(TransferType.Place, _config.RobotArm,
                    _config.RobotArm == RobotArm.Arm1
                    ? _robot.UpperArmLocation.Wafer.MaterialType
                    : _robot.LowerArmLocation.Wafer.MaterialType,
                    _config.RobotArm == RobotArm.Arm1
                        ? _robot.UpperArmWaferDimension
                        : _robot.LowerArmWaferDimension),
                new PMDone());
        }

        private void GoInFrontOfPmEntry(Event ev)
        {
            PerformAction(
                () => _robot.GoToSpecifiedLocation(_config.ProcessModule, 1, _config.RobotArm, false),
                new RobotDone());
        }

        private void PrepareTransferOnPmEntry(Event ev)
        {
            try
            {
                _config.ProcessModule.PrepareTransfer(TransferType.Place, _config.RobotArm,
                    _config.RobotArm == RobotArm.Arm1
                        ? _robot.UpperArmLocation.Wafer.MaterialType
                        : _robot.LowerArmLocation.Wafer.MaterialType,
                    _config.RobotArm == RobotArm.Arm1
                        ? _robot.UpperArmWaferDimension
                        : _robot.LowerArmWaferDimension);
                PostEvent(new ReadyToTransfer());
            }
            catch
            {
                PostEvent(new NotReadyToTransfer());
            }
        }

        private void WaitReadyToTransferEntry(Event ev)
        {
            _config.ProcessModule.ReadyToTransfer += ProcessModule_ReadyToTransfer;
        }

        private void PlaceToPmEntry(Event ev)
        {
            PerformAction(
                () => _robot.Place(_config.RobotArm, _config.ProcessModule, 1),
                new RobotDone());
        }

        private void PostTransferEntry(Event ev)
        {
            PerformAction(() => _config.ProcessModule.PostTransfer(), new PMDone());
        }

        #endregion Action State Machine

        #region Event Handler

        private void ProcessModule_ReadyToTransfer(object sender, System.EventArgs e)
        {
            _config.ProcessModule.ReadyToTransfer -= ProcessModule_ReadyToTransfer;
            PostEvent(new ReadyToTransfer());
        }

        #endregion
    }
}
