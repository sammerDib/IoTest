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
    /// Transfer a Substrate From the Process Module to a load stage Supported Commands are:
    /// <list type="bullet">
    /// <item>Start the Activity</item> <item>Abort the Activity</item>
    /// </list>
    /// </summary>
    internal partial class UnloadFromPm : MachineActivity
    {
        #region Fields

        private readonly UnloadFromPmConfiguration _config;
        private readonly EquipmentSubstrateLocation _robotArmLocation;
        private readonly Abstractions.Devices.Robot.Robot _robot;

        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="UnloadFromPm" /> class.</summary>
        /// <param name="configuration">The activity configuration.</param>
        public UnloadFromPm(UnloadFromPmConfiguration configuration)
            : base(nameof(UnloadFromPm), configuration.Equipment.TryGetDevice<Controller>())
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration),
                    "Provided configuration cannot be null. Activity cannot start.");
            }

            _config = configuration;
            _robot = Efem.TryGetDevice<Abstractions.Devices.Robot.Robot>();

            _robotArmLocation = _config.RobotArm == RobotArm.Arm1
                ? _robot.UpperArmLocation
                : _robot.LowerArmLocation;

            CreateStateMachine();
            StateMachine = m_UnloadFromPm;
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
                || _config.LoadPort.State != OperatingModes.Idle)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = "At least one Device is not IDLE. Activity cannot start.";
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
            else if (_config.DestinationSlot < 1
                     || _config.DestinationSlot > _config.LoadPort.Carrier.Capacity)
            {
                err.ErrorCode = ErrorCode.UnknownObjectInstance;
                err.ErrorText =
                    $"Provided destination slot {_config.DestinationSlot} does not match carrier capacity '{_config.LoadPort.Carrier.Capacity}. Activity cannot start.";
            }
            else if (_config.LoadPort.Carrier.MaterialLocations
                         .ElementAt(_config.DestinationSlot - 1)
                         .Material
                     != null
                     || _config.LoadPort.Carrier.MappingTable[_config.DestinationSlot - 1]
                     != SlotState.NoWafer)
            {
                err.ErrorCode = ErrorCode.UnknownObjectInstance;
                err.ErrorText =
                    $"Slot {_config.DestinationSlot} is not empty. Activity cannot start.";
            }
            else if (_robotArmLocation.Material != null)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText =
                    $"Material is present on {_config.RobotArm}. Activity cannot start.";
            }
            else if (_config.ProcessModule.Location.Material == null)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = "Material is not present on Process Module. Activity cannot start.";
            }
            else if (_config.LoadPort.Carrier.SampleSize
                     != _config.ProcessModule.Location.Substrate.MaterialDimension)
            {
                err.ErrorCode = ErrorCode.ValidationError;
                err.ErrorText =
                    $"Material Sizes between PM ({_config.ProcessModule.Location.Substrate.MaterialDimension}) and LP ({_config.LoadPort.Carrier.SampleSize}) are different. Activity cannot start.";
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
            m_UnloadFromPm.Dispose();
            base.Dispose(disposing);
        }

        public override ErrorDescription Abort()
        {
            //Abort all devices involved in the activity
            _robot.InterruptAsync(InterruptionKind.Abort);
            _config.ProcessModule.InterruptAsync(InterruptionKind.Abort);
            _config.LoadPort.InterruptAsync(InterruptionKind.Abort);

            return base.Abort();
        }

        #endregion

        #region Action State Machine

        private void PrepareForTransferEntry(Event ev)
        {
            PerformAction(
                () => _config.ProcessModule.PrepareTransferAsync(TransferType.Pick, _config.RobotArm, _config.ProcessModule.Location.Wafer.MaterialType, _config.ProcessModule.Location.Wafer.MaterialDimension),
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
                _config.ProcessModule.PrepareTransfer(TransferType.Pick, _config.RobotArm, _config.ProcessModule.Location.Wafer.MaterialType, _config.ProcessModule.Location.Wafer.MaterialDimension);
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

        private void PickFromPmEntry(Event ev)
        {
            PerformAction(
                () => _robot.Pick(_config.RobotArm, _config.ProcessModule, 1),
                new RobotDone());
        }

        private void PostTransferEntry(Event ev)
        {
            PerformAction(() => _config.ProcessModule.PostTransfer(), new PMDone());
        }

        private void PlaceToLpEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    _config.LoadPort.PrepareForTransfer();
                    _robot.Place(_config.RobotArm, _config.LoadPort, _config.DestinationSlot);
                    _config.LoadPort.PostTransfer();
                },
                new RobotDone());
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
