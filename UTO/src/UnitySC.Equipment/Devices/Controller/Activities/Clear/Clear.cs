using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    /// <summary>
    /// Remove all substrates inside the Tool Supported Commands are:
    /// <list type="bullet">
    /// <item>Start the Activity</item> <item>Abort the Activity</item>
    /// </list>
    /// </summary>
    internal partial class Clear : MachineActivity
    {
        private readonly Func<string, bool> _confirmationCallback;
        private readonly Dictionary<string, SubstrateLocation> _destinations;
        private readonly string _nameGripper1;
        private readonly string _nameGripper2;
        private SubstrateLocation _destinationGripper1;
        private SubstrateLocation _destinationGripper2;
        private readonly ActivityManager _processModuleClearActivityManager;

        #region Devices

        private readonly Abstractions.Devices.Robot.Robot _robot;
        private readonly Abstractions.Devices.Aligner.Aligner _aligner;
        private readonly List<Abstractions.Devices.LoadPort.LoadPort> _loadPorts;
        private readonly List<DriveableProcessModule> _processModules;
        private readonly DriveableProcessModule _processModule1;
        private readonly DriveableProcessModule _processModule2;
        private readonly DriveableProcessModule _processModule3;

        #endregion

        /// <summary>
        /// Check if the activity can be started. It validates the context and parameters. The activity can NOT
        /// starts if:
        /// <list type="bullet">
        /// <item>Another Activity is in progress</item>
        /// <item>
        /// At least 1 device is not supported on the equipment (Robot, LP, Aligner, PM)
        /// </item>
        /// <item>Used Devices are not Idle</item> <item>A specified location does not exist</item>
        /// <item>Selected Destination slot is not empty on the equipment</item>
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

            if (_robot.State != OperatingModes.Idle)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = "Robot is not IDLE. Not allowed to start";
            }
            else if (_destinations.Count != NbSubstratesOnTool)
            {
                err.ErrorCode = ErrorCode.InsufficientParametersSpecified;
                err.ErrorText =
                    $"{_destinations.Count} destinations are specified. {NbSubstratesOnTool} wafers on tool. Not allowed to start";
            }
            else
            {
                // Check Destinations
                foreach (var destination in _destinations)
                {
                    var substrate =
                        SubstratesOnTool.FirstOrDefault(
                            s => s.Location.Name.Equals(destination.Key));

                    if (substrate == null)
                    {
                        // No substrate found on destination Location ?
                        err.ErrorCode = ErrorCode.InvalidAttributeValue;
                        err.ErrorText =
                            $"No Substrate found on location {destination.Key}. Not allowed to start";
                    }
                    else if (destination.Key != _nameGripper1
                             && destination.Key != _nameGripper2
                             && !Controller.AllOfType<IMaterialLocationContainer>()
                                 .Any(
                                     d => d.MaterialLocations.All(
                                         ml => ml.Name != destination.Key)))
                    {
                        // Destination Location known ?
                        err.ErrorCode = ErrorCode.InvalidAttributeValue;
                        err.ErrorText =
                            $"Location {destination.Key} does not exist. Not allowed to start";
                    }
                    else if (destination.Value == null)
                    {
                        // Destination Substrate Location is unknown
                        err.ErrorCode = ErrorCode.InvalidAttributeValue;
                        err.ErrorText =
                            $"No destination location associated to Location {destination.Key} provided. Not allowed to start";
                    }
                    else if (!_loadPorts.Any(
                                 lp => lp.Carrier != null
                                       && lp.InstanceId.Equals(substrate.SourcePort)))
                    {
                        // Load Port Identifier valid ?
                        err.ErrorCode = ErrorCode.InvalidAttributeValue;
                        err.ErrorText =
                            $"Destination {destination.Value.Name} does not correspond to a valid load port. Not allowed to start";
                    }
                    else
                    {
                        // Load Port Treatment
                        err = CheckLoadPort(
                            _loadPorts.Single(
                                lp => lp.Carrier != null
                                      && lp.InstanceId.Equals(substrate.SourcePort)),
                            substrate);
                    }
                }
            }

            return err;
        }

        private ErrorDescription CheckLoadPort(
            Abstractions.Devices.LoadPort.LoadPort loadPort,
            Substrate substrate)
        {
            var err = new ErrorDescription();

            // Load Port is in IDLE state?
            if (loadPort.State != OperatingModes.Idle)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText = $"{loadPort.Name} is not IDLE. Activity cannot start.";
                return err;
            }

            // Load Port is Opened ?
            if (loadPort.PhysicalState != LoadPortState.Open
                && !loadPort.Configuration.CloseDoorAfterRobotAction)
            {
                err.ErrorCode = ErrorCode.CommandNotValidForCurrentState;
                err.ErrorText =
                    $"Carrier on Load Port {loadPort.InstanceId} is not accessible. Activity cannot start.";
                return err;
            }

            // Mapping Done ?
            if (loadPort.CarrierPresence != CassettePresence.Correctly
                || loadPort.Carrier?.MappingTable == null)
            {
                err.ErrorCode = ErrorCode.InvalidAttributeValue;
                err.ErrorText = $"{loadPort.Name} is not Mapped. Activity cannot start.";
                return err;
            }

            // Slot is empty ?
            if (loadPort.Carrier.MappingTable[substrate.SourceSlot - 1] != SlotState.NoWafer)
            {
                err.ErrorCode = ErrorCode.InvalidAttributeValue;
                err.ErrorText =
                    $"Carrier {loadPort.Carrier.Id} / Slot {substrate.SourceSlot} has substrate. Activity cannot start.";
                return err;
            }

            // Check the coherence size
            if ((_robot.UpperArmLocation.Substrate != null
                 && _robot.UpperArmLocation.Substrate.MaterialDimension
                 != loadPort.Carrier.SampleSize)
                || (_robot.LowerArmLocation.Substrate != null
                    && _robot.LowerArmLocation.Substrate.MaterialDimension
                    != loadPort.Carrier.SampleSize)
                || (_aligner.Location.Substrate != null
                    && _aligner.Location.Substrate.MaterialDimension
                    != loadPort.Carrier.SampleSize))
            {
                err.ErrorCode = ErrorCode.InvalidAttributeValue;
                err.ErrorText = "Incoherent substrate sizes. Activity cannot start.";
                return err;
            }

            if (_processModules.Any(
                    pm => pm.Location.Substrate != null
                          && pm.Location.Substrate.MaterialDimension
                          != loadPort.Carrier.SampleSize))
            {
                err.ErrorCode = ErrorCode.InvalidAttributeValue;
                err.ErrorText = "Incoherent substrate sizes. Activity cannot start.";
                return err;
            }

            return err;
        }

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="Clear" /> class.</summary>
        /// <param name="controller">The controller device instance.</param>
        /// <param name="confirmationCallback">
        /// Callback called in case of step by step. If null, no step by step is required. Input parameter
        /// contains an informative string about the movement to be done. The return corresponds to the
        /// authorization. True means continue, False means Abort the activity.
        /// </param>
        public Clear(Controller controller, Func<string, bool> confirmationCallback)
            : base(nameof(Clear), controller)
        {
            _destinations = new Dictionary<string, SubstrateLocation>();
            foreach (var substrate in SubstratesOnTool)
            {
                var destination = substrate.Destination ?? substrate.Source;
                _destinations.Add(substrate.Location.Name, destination);
            }

            _processModules = Controller.AllDevices<DriveableProcessModule>().ToList();
            _processModule1 = _processModules.FirstOrDefault(pm => pm.InstanceId == 1);
            _processModule2 = _processModules.FirstOrDefault(pm => pm.InstanceId == 2);
            _processModule3 = _processModules.FirstOrDefault(pm => pm.InstanceId == 3);

            _loadPorts = Efem.AllDevices<Abstractions.Devices.LoadPort.LoadPort>().ToList();
            _aligner = Efem.TryGetDevice<Abstractions.Devices.Aligner.Aligner>();
            _robot = Efem.TryGetDevice<Abstractions.Devices.Robot.Robot>();

            _nameGripper1 = _robot.UpperArmLocation.Name;
            _nameGripper2 = _robot.LowerArmLocation.Name;

            // Do we have a substrate on RobotArm ?
            if (_destinations.Keys.Contains(_nameGripper1))
            {
                _destinationGripper1 = _destinations[_nameGripper1];
            }

            if (_destinations.Keys.Contains(_nameGripper2))
            {
                _destinationGripper2 = _destinations[_nameGripper2];
            }

            var str = new StringBuilder();
            str.AppendLine("[Details]");
            foreach (var item in _destinations)
            {
                str.Append(item.Key).Append(" => ").Append(item.Value).AppendLine();
            }

            Logger.Info($"Movement planned :{Environment.NewLine}{str}");

            _confirmationCallback = confirmationCallback;

            _processModuleClearActivityManager = new ActivityManager(Logger);
            _processModuleClearActivityManager.ActivityDone +=
                ProcessModuleClearActivityManager_ActivityDone;

            CreateStateMachine();
            StateMachine = m_Clear;
        }

        #endregion Constructor

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
            _processModuleClearActivityManager.ActivityDone -=
                ProcessModuleClearActivityManager_ActivityDone;
            m_Clear.Dispose();
            base.Dispose(disposing);
        }

        private void StepByStepConfirmation(string msg)
        {
            var isContinue = _confirmationCallback?.Invoke(msg) ?? true;
            if (!isContinue)
            {
                PostEvent(ActivityDoneEvent.OnRejected("Clean Activity stopped by Operator."));
            }
        }

        public override ErrorDescription Abort()
        {
            _processModuleClearActivityManager.Abort();

            //Abort all devices involved in the activity
            _robot.InterruptAsync(InterruptionKind.Abort);
            _aligner.InterruptAsync(InterruptionKind.Abort);

            foreach (var pm in _processModules)
            {
                pm.InterruptAsync(InterruptionKind.Abort);
            }

            foreach (var loadPort in _loadPorts)
            {
                loadPort.InterruptAsync(InterruptionKind.Abort);
            }

            return base.Abort();
        }

        #region Event Handlers

        private void ProcessModuleClearActivityManager_ActivityDone(
            object sender,
            ActivityEventArgs e)
        {
            if (e.Status == CommandStatusCode.Ok)
            {
                PostEvent(new PmDone());
                return;
            }

            //Abort the whole activity in case of error
            Abort();
        }

        #endregion

        #region Action State Machine

        private void GetStatusesEntry(Event ev)
        {
            var locations = Controller.GetSubstrateLocations();

            PostEvent(new StatusReceived(locations));
        }

        #region Aligner

        private void PickOnAligner_G1Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_aligner} with Gripper 1.
Continue ?");

            // Store destination to the Robot gripper
            _destinationGripper1 = _destinations[_aligner.Location.Name];

            PerformAction(
                () =>
                {
                    _aligner.PrepareTransfer(
                        _robot.Configuration.UpperArm.EffectorType,
                        _aligner.GetMaterialDimension(),
                        _aligner.GetMaterialType());
                    _robot.Pick(RobotArm.Arm1, _aligner, 1);
                },
                new RobotDone());
        }

        private void PickOnAligner_G2Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_aligner.Name} with Gripper 2.
Continue ?");

            // Store destination to the Robot gripper
            _destinationGripper2 = _destinations[_aligner.Location.Name];

            PerformAction(
                () =>
                {
                    _aligner.PrepareTransfer(
                        _robot.Configuration.LowerArm.EffectorType,
                        _aligner.GetMaterialDimension(),
                        _aligner.GetMaterialType());
                    _robot.Pick(RobotArm.Arm2, _aligner, 1);
                },
                new RobotDone());
        }

        #endregion

        #region ProcessModule 1 Gripper 1

        private void RunClearPm1Activity_G1Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_processModule1.Name} with Gripper 1.
Continue ?");

            _destinationGripper1 = _destinations[_processModule1.Location.Name];

            PerformAction(
                () =>
                {
                    _processModuleClearActivityManager.Run(
                        new ProcessModuleClearActivity(Controller, _processModule1, RobotArm.Arm1));
                },
                new PmDone());
        }

        #endregion

        #region ProcessModule 1 Gripper 2

        private void RunClearPm1Activity_G2Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_processModule1.Name} with Gripper 2.
Continue ?");

            _destinationGripper2 = _destinations[_processModule1.Location.Name];

            PerformAction(
                () =>
                {
                    _processModuleClearActivityManager.Run(
                        new ProcessModuleClearActivity(Controller, _processModule1, RobotArm.Arm2));
                },
                new PmDone());
        }

        #endregion

        #region ProcessModule 2 Gripper 1

        private void RunClearPm2Activity_G1Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_processModule2.Name} with Gripper 1.
Continue ?");

            _destinationGripper1 = _destinations[_processModule2.Location.Name];

            PerformAction(
                () =>
                {
                    _processModuleClearActivityManager.Run(
                        new ProcessModuleClearActivity(Controller, _processModule2, RobotArm.Arm1));
                },
                new PmDone());
        }

        #endregion

        #region ProcessModule 2 Gripper 2

        private void RunClearPm2Activity_G2Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_processModule2.Name} with Gripper 2.
Continue ?");

            _destinationGripper2 = _destinations[_processModule2.Location.Name];

            PerformAction(
                () =>
                {
                    _processModuleClearActivityManager.Run(
                        new ProcessModuleClearActivity(Controller, _processModule2, RobotArm.Arm2));
                },
                new PmDone());
        }

        #endregion

        #region ProcessModule 3 Gripper 1

        private void RunClearPm3Activity_G1Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_processModule3.Name} with Gripper 1.
Continue ?");

            _destinationGripper1 = _destinations[_processModule3.Location.Name];

            PerformAction(
                () =>
                {
                    _processModuleClearActivityManager.Run(
                        new ProcessModuleClearActivity(Controller, _processModule3, RobotArm.Arm1));
                },
                new PmDone());
        }

        #endregion

        #region ProcessModule 3 Gripper 2

        private void RunClearPm3Activity_G2Entry(Event ev)
        {
            StepByStepConfirmation(
                $@"Pick Substrate on {_processModule3.Name} with Gripper 2.
Continue ?");

            _destinationGripper2 = _destinations[_processModule3.Location.Name];

            PerformAction(
                () =>
                {
                    _processModuleClearActivityManager.Run(
                        new ProcessModuleClearActivity(Controller, _processModule3, RobotArm.Arm2));
                },
                new PmDone());
        }

        #endregion

        #region Load Ports

        private void PlaceToLP_G1Entry(Event ev)
        {
            if (_destinationGripper1 == null)
            {
                StateMachine.PostEvent(
                    ActivityDoneEvent.OnError(
                        "No Destination defined for Substrate present on Gripper 1"));
                return;
            }

            StepByStepConfirmation(
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"Place Substrate in {0} with Gripper 1.\r\nContinue ?",
                    _destinationGripper1.ToString()));

            PerformAction(
                () =>
                {
                    var substrate = _robot.UpperArmLocation.Substrate;
                    var slot = substrate.SourceSlot;
                    
                    if (_loadPorts.FirstOrDefault(
                            lp => lp.Carrier != null
                                  && lp.InstanceId.Equals(substrate.SourcePort)) is not {} loadPort)
                    {
                        throw new InvalidOperationException();
                    }

                    loadPort.PrepareForTransfer();
                    _robot.Place(RobotArm.Arm1, loadPort, slot);
                    loadPort.PostTransfer();
                },
                new RobotDone());
        }

        private void PlaceToLP_G2Entry(Event ev)
        {
            if (_destinationGripper2 == null)
            {
                StateMachine.PostEvent(
                    ActivityDoneEvent.OnError(
                        "No Destination defined for Substrate present on Gripper 2"));
                return;
            }

            StepByStepConfirmation(
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"Place Substrate in {0} with Gripper 2. \r\nContinue ?",
                    _destinationGripper2.ToString()));

            PerformAction(
                () =>
                {
                    var substrate = _robot.LowerArmLocation.Substrate;
                    var slot = substrate.SourceSlot;

                    if (_loadPorts.FirstOrDefault(
                            lp => lp.Carrier != null
                                  && lp.InstanceId.Equals(substrate.SourcePort)) is not { } loadPort)
                    {
                        throw new InvalidOperationException();
                    }

                    loadPort.PrepareForTransfer();
                    _robot.Place(RobotArm.Arm2, loadPort, slot);
                    loadPort.PostTransfer();
                },
                new RobotDone());
        }

        #endregion

        protected override void ActivityExit(Event ev)
        {
            if (NbSubstratesOnTool == 0)
            {
                foreach (var loadPort in _loadPorts.Where(lp => lp.IsDocked))
                {
                    loadPort.ReleaseCarrierAsync();
                }
            }

            base.ActivityExit(ev);
        }

        #endregion Action State Machine

        #region Conditional

        private bool AlignerOccupied_G1Empty(Event ev)
        {
            return _aligner.Location.Substrate != null && RobotEmpty() && _robot.Configuration.UpperArm.IsEnabled;
        }

        private bool AlignerOccupied_G1Occupied_G2Empty(Event ev)
        {
            return _aligner.Location.Substrate != null && RobotEmpty() && _robot.Configuration.LowerArm.IsEnabled;
        }

        private bool PM1Occupied_G2Empty(Event ev)
        {
            return _processModule1.Location.Substrate != null
                   && _aligner.Location.Substrate == null
                   && RobotEmpty()
                   && _robot.Configuration.LowerArm.IsEnabled;
        }

        private bool PM1Occupied_G2Occupied_G1Empty(Event ev)
        {
            return _processModule1.Location.Substrate != null
                   && _aligner.Location.Substrate == null
                   && RobotEmpty()
                   && _robot.Configuration.UpperArm.IsEnabled;
        }

        private bool PM2Occupied_G2Empty(Event ev)
        {
            return _processModule2?.Location.Substrate != null
                   && _aligner.Location.Substrate == null
                   && RobotEmpty()
                   && _robot.Configuration.LowerArm.IsEnabled;
        }

        private bool PM2Occupied_G2Occupied_G1Empty(Event ev)
        {
            return _processModule2?.Location.Substrate != null
                   && _aligner.Location.Substrate == null
                   && RobotEmpty()
                   && _robot.Configuration.UpperArm.IsEnabled;
        }

        private bool PM3Occupied_G2Empty(Event ev)
        {
            return _processModule3?.Location.Substrate != null
                   && _aligner.Location.Substrate == null
                   && RobotEmpty()
                   && _robot.Configuration.LowerArm.IsEnabled;
        }

        private bool PM3Occupied_G2Occupied_G1Empty(Event ev)
        {
            return _processModule3?.Location.Substrate != null
                   && _aligner.Location.Substrate == null
                   && RobotEmpty()
                   && _robot.Configuration.UpperArm.IsEnabled;
        }

        private bool G1Occupied_1SubstrateOnTool(Event ev)
        {
            return _robot.UpperArmLocation.Substrate != null
                   && _robot.Configuration.UpperArm.IsEnabled;
        }

        private bool G1Occupied(Event ev)
        {
            return _robot.UpperArmLocation.Substrate != null
                   && _robot.Configuration.UpperArm.IsEnabled;
        }

        private bool G1Empty(Event ev)
        {
            return _robot.UpperArmLocation.Substrate == null
                   || !_robot.Configuration.UpperArm.IsEnabled;
        }

        private bool G1Empty()
        {
            return (_robot.UpperArmLocation.Substrate == null
                    && _robot.Configuration.UpperArm.IsEnabled)
                   || !_robot.Configuration.UpperArm.IsEnabled;
        }

        private bool G2Empty()
        {
            return (_robot.LowerArmLocation.Substrate == null
                    && _robot.Configuration.LowerArm.IsEnabled)
                   || !_robot.Configuration.LowerArm.IsEnabled;
        }

        private bool RobotEmpty()
        {
            return G1Empty() && G2Empty();
        }

        private bool IsCleanG2Allowed(Event ev)
        {
            return _robot.LowerArmLocation.Substrate != null
                   && _robot.Configuration.LowerArm.IsEnabled;
        }

        private bool ToolClean(Event ev)
        {
            return NbSubstratesOnTool == 0
                || (NbSubstratesOnTool == 1
                    && ((_robot.UpperArmLocation.Substrate != null && !_robot.Configuration.UpperArm.IsEnabled)
                    || (_robot.LowerArmLocation.Substrate != null && !_robot.Configuration.LowerArm.IsEnabled)));
        }

        #endregion Conditional
    }
}
