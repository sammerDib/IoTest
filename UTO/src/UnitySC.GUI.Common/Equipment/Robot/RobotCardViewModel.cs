using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.SmifLoadPort;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.GUI.Common.Equipment.Popup;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Equipment.Robot
{
    public class RobotCardViewModel : UnityDeviceCardViewModel
    {
        static RobotCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(RobotCardViewModel), typeof(RobotCard));
        }

        public RobotCardViewModel(
            UnitySC.Equipment.Abstractions.Devices.Robot.Robot robot)
        {
            Robot = robot;
        }

        #region Properties

        public UnitySC.Equipment.Abstractions.Devices.Robot.Robot Robot { get; }

        private IMaterialLocationContainer _selectedSource;

        public IMaterialLocationContainer SelectedSource
        {
            get => _selectedSource;
            set => SetAndRaiseIfChanged(ref _selectedSource, value);
        }

        private IMaterialLocationContainer _selectedDestination;

        public IMaterialLocationContainer SelectedDestination
        {
            get => _selectedDestination;
            set => SetAndRaiseIfChanged(ref _selectedDestination, value);
        }

        private byte _selectedSlotSource;

        public byte SelectedSlotSource
        {
            get => _selectedSlotSource;
            set => SetAndRaiseIfChanged(ref _selectedSlotSource, value);
        }

        private byte _selectedSlotDestination;

        public byte SelectedSlotDestination
        {
            get => _selectedSlotDestination;
            set => SetAndRaiseIfChanged(ref _selectedSlotDestination, value);
        }

        private RobotArm _arm;

        public RobotArm Arm
        {
            get => _arm;
            set
            {
                if (value != RobotArm.Undefined)
                {
                    SetAndRaiseIfChanged(ref _arm, value);
                }
            }
        }

        private Ratio _currentSpeed;

        public string CurrentSpeedAsString
        {
            get => _currentSpeed.Percent.ToString(CultureInfo.InvariantCulture);
            set
            {
                var newValue = Ratio.Zero;
                if (int.TryParse(value, out var result))
                {
                    newValue = Ratio.FromPercent(result);
                }

                _currentSpeed = newValue;
                SetAndRaiseIfChanged(ref _currentSpeed, newValue);
            }
        }

        #endregion

        #region Commands

        #region Init

        private SafeDelegateCommandAsync _initCommand;

        public SafeDelegateCommandAsync InitCommand
            => _initCommand ??= new SafeDelegateCommandAsync(
                InitializeRobotCommandExecute,
                InitializeRobotCommandCanExecute);

        private Task InitializeRobotCommandExecute()
        {
            return Robot.InitializeAsync(false);
        }

        private bool InitializeRobotCommandCanExecute()
        {
            if (Robot == null)
            {
                return false;
            }

            var context = Robot.NewCommandContext(nameof(Robot.Initialize))
                .AddArgument("mustForceInit", false);
            return Robot.CanExecute(context);
        }

        #endregion

        #region Home

        private SafeDelegateCommandAsync _homeCommand;

        public SafeDelegateCommandAsync HomeCommand
            => _homeCommand ??= new SafeDelegateCommandAsync(
                HomeRobotCommandExecute,
                HomeRobotCommandCanExecute);

        private Task HomeRobotCommandExecute()
        {
            return Robot.GoToHomeAsync();
        }

        private bool HomeRobotCommandCanExecute()
        {
            if (Robot == null)
            {
                return false;
            }

            var context = Robot.NewCommandContext(nameof(Robot.GoToHome));
            return Robot.CanExecute(context);
        }

        #endregion

        #region Pick

        private SafeDelegateCommand _pickCommand;

        public SafeDelegateCommand PickCommand
            => _pickCommand ??= new SafeDelegateCommand(
                PickRobotCommandExecute,
                PickRobotCommandCanExecute);

        private void PickRobotCommandExecute()
        {
            Task.Run(() => Pick(SelectedSource, SelectedSlotSource, Arm));
        }

        private bool PickRobotCommandCanExecute()
        {
            if (Robot == null)
            {
                return false;
            }

            if (SelectedSource is UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort lp)
            {
                var carrier = lp.Carrier;

                if (carrier == null)
                {
                    return false;
                }

                if (SelectedSource is SmifLoadPort smifLoadPort)
                {
                    var loadPortContext = smifLoadPort.NewCommandContext(nameof(ISmifLoadPort.GoToSlot))
                        .AddArgument("slot", SelectedSlotSource);

                    if (!smifLoadPort.CanExecute(loadPortContext))
                    {
                        return false;
                    }
                }

                var context = Robot.NewCommandContext(nameof(Robot.Pick))
                    .AddArgument("arm", Arm)
                    .AddArgument("sourceDevice", SelectedSource)
                    .AddArgument("sourceSlot", SelectedSlotSource);

                return Robot.CanExecute(context);
            }
            else
            {
                if (SelectedSource is UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner
                    aligner)
                {
                    //In case of aligner, PrepareTransfer command must be called on Aligner before calling Pick/Place command
                    //So CanExecute will always be false in case of aligner
                    //We check conditions here to allow to prepare the aligner
                    if (!Robot.HasBeenInitialized
                        || Robot.State != OperatingModes.Idle
                        || aligner.State != OperatingModes.Idle
                        || aligner.Location.Material == null)
                    {
                        return false;
                    }

                    if (Arm == RobotArm.Arm1)
                    {
                        if (Robot.UpperArmLocation.Material != null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Robot.LowerArmLocation.Material != null)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else if (SelectedSource is UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule processModule)
                {
                    if (_waitPmReadyInProgress)
                    {
                        return false;
                    }

                    var pmContext = processModule
                        .NewCommandContext(nameof(processModule.PrepareTransfer))
                        .AddArgument("transferType", TransferType.Pick)
                        .AddArgument("arm", Arm)
                    .AddArgument("materialType", processModule.Location.Wafer?.MaterialType ?? MaterialType.Unknown)
                    .AddArgument("dimension", processModule.Location.Wafer?.MaterialDimension ?? SampleDimension.NoDimension); 
                    return processModule.CanExecute(pmContext);
                }

                var context = Robot.NewCommandContext(nameof(Robot.Pick))
                    .AddArgument("arm", Arm)
                    .AddArgument("sourceDevice", SelectedSource)
                    .AddArgument("sourceSlot", SelectedSlotSource);

                return Robot.CanExecute(context);
            }
        }

        #endregion

        #region Place

        private SafeDelegateCommand _placeCommand;

        public SafeDelegateCommand PlaceCommand
            => _placeCommand ??= new SafeDelegateCommand(
                PlaceRobotCommandExecute,
                PlaceRobotCommandCanExecute);

        private void PlaceRobotCommandExecute()
        {
            Task.Run(() => Place(SelectedDestination, SelectedSlotDestination, Arm));
        }

        private bool PlaceRobotCommandCanExecute()
        {
            return PlaceCanExecute(false);
        }

        #endregion

        #region Transfer

        private SafeDelegateCommand _transferCommand;

        public SafeDelegateCommand TransferCommand
            => _transferCommand ??= new SafeDelegateCommand(
                TransferRobotCommandExecute,
                TransferRobotCommandCanExecute);

        private void TransferRobotCommandExecute()
        {
            Task.Run(
                () =>
                {
                    var selectedDestination = SelectedDestination;
                    var selectedSlotDestination = SelectedSlotDestination;
                    var arm = Arm;

                    Pick(SelectedSource, SelectedSlotSource, Arm);
                    Place(selectedDestination, selectedSlotDestination, arm);
                });
        }

        private bool TransferRobotCommandCanExecute()
        {
            return PickRobotCommandCanExecute() && PlaceCanExecute(true);
        }

        #endregion

        #region SetSpeed

        private SafeDelegateCommandAsync _setMotionSpeedCommand;

        public SafeDelegateCommandAsync SetMotionSpeedCommand
            => _setMotionSpeedCommand ??= new SafeDelegateCommandAsync(
                SetSpeedCommandExecute,
                SetSpeedCommandCanExecute);

        private Task SetSpeedCommandExecute()
        {
            return Robot.SetMotionSpeedAsync(_currentSpeed);
        }

        private bool SetSpeedCommandCanExecute()
        {
            if (Robot == null)
            {
                return false;
            }

            var context = Robot.NewCommandContext(nameof(Robot.SetMotionSpeed))
                .AddArgument("percentage", _currentSpeed);
            return Robot.CanExecute(context);
        }

        #endregion

        #region Abort

        private SafeDelegateCommandAsync _abortCommand;

        public SafeDelegateCommandAsync AbortCommand
            => _abortCommand ??= new SafeDelegateCommandAsync(
                AbortCommandExecute,
                AbortCommandCanExecute);

        private Task AbortCommandExecute()
        {
            return Robot.InterruptAsync(InterruptionKind.Abort);
        }

        private bool AbortCommandCanExecute()
        {
            if (Robot == null)
            {
                return false;
            }

            return Robot.State != OperatingModes.Maintenance && Robot.State != OperatingModes.Idle;
        }

        #endregion

        #region Set Upper Arm Wafer Presence

        private SafeDelegateCommand _setUpperArmWaferPresenceCommand;

        public SafeDelegateCommand SetUpperArmWaferPresenceCommand
            => _setUpperArmWaferPresenceCommand ??= new SafeDelegateCommand(
                SetUpperArmWaferPresenceCommandExecute,
                SetUpperArmWaferPresenceCommandCanExecute);

        private void SetUpperArmWaferPresenceCommandExecute()
        {
            var popupContent = new SetWaferPresencePopup(Robot.UpperArmLocation);
            var popup = new Agileo.GUI.Services.Popups.Popup(
                new LocalizableText(nameof(EquipmentResources.POPUP_SET_WAFER_PRESENCE)))
            {
                Content = popupContent
            };

            popup.Commands.Add(
                new PopupCommand(
                    Agileo.GUI.Properties.Resources.S_OK,
                    new DelegateCommand(
                        () =>
                        {
                            popupContent.ValidateModifications(); 
                            Robot.CheckSubstrateDetectionError(RobotArm.Arm1, true);
                        })));
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
        }

        private bool SetUpperArmWaferPresenceCommandCanExecute()
        {
            return Robot.State == OperatingModes.Idle || Robot.State == OperatingModes.Maintenance;
        }

        #endregion

        #region Set Lower Arm Wafer Presence

        private SafeDelegateCommand _setLowerArmWaferPresenceCommand;

        public SafeDelegateCommand SetLowerArmWaferPresenceCommand
            => _setLowerArmWaferPresenceCommand ??= new SafeDelegateCommand(
                SetLowerArmWaferPresenceCommandExecute,
                SetLowerArmWaferPresenceCommandCanExecute);

        private void SetLowerArmWaferPresenceCommandExecute()
        {
            var popupContent = new SetWaferPresencePopup(Robot.LowerArmLocation);

            var popup = new Agileo.GUI.Services.Popups.Popup(
                new LocalizableText(nameof(EquipmentResources.POPUP_SET_WAFER_PRESENCE)))
            {
                Content = popupContent
            };

            popup.Commands.Add(
                new PopupCommand(
                    Agileo.GUI.Properties.Resources.S_OK,
                    new DelegateCommand(
                        () =>
                        {
                            popupContent.ValidateModifications();
                            Robot.CheckSubstrateDetectionError(RobotArm.Arm2, true);
                        })));
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
        }

        private bool SetLowerArmWaferPresenceCommandCanExecute()
        {
            return Robot.State == OperatingModes.Idle || Robot.State == OperatingModes.Maintenance;
        }

        #endregion

        #endregion

        #region setup

        public void Setup()
        {
            _currentSpeed = Robot.Speed;
        }

        #endregion

        #region Private Methods

        private bool _waitPmReadyInProgress;

        private void Pick(
            IMaterialLocationContainer selectedSource,
            byte selectedSlotSource,
            RobotArm arm)
        {
            if (selectedSource is UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner aligner)
            {
                var tasksToWait = new List<Task>
                {
                    Robot.GoToSpecifiedLocationAsync(selectedSource, selectedSlotSource, arm, true),
                    aligner.PrepareTransferAsync(
                        arm == RobotArm.Arm1
                            ? Robot.Configuration.UpperArm.EffectorType
                            : Robot.Configuration.LowerArm.EffectorType,
                        aligner.GetMaterialDimension(),
                        aligner.GetMaterialType())
                };

                Task.WaitAll(tasksToWait.ToArray());

                Robot.Pick(arm, selectedSource, selectedSlotSource);
            }
            else if (selectedSource is SmifLoadPort loadPort)
            {
                var tasksToWait = new List<Task>
                {
                    Robot.GoToSpecifiedLocationAsync(selectedSource, 1, arm, true),
                    loadPort.GoToSlotAsync(selectedSlotSource)
                };

                Task.WaitAll(tasksToWait.ToArray());

                //In case of SMIF Load port, robot will always access to slot 1
                //So we do a particular case by setting the slot to maxValue
                Robot.Pick(arm, selectedSource, byte.MaxValue);
            }
            else if (selectedSource is UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule processModule)
            {
                var pmTask = Task.Run(
                    () =>
                    {
                        try
                        {
                            processModule.PrepareTransfer(TransferType.Pick, arm, processModule.Location.Wafer.MaterialType, processModule.Location.Wafer.MaterialDimension);
                        }
                        catch (Exception)
                        {
                            _waitPmReadyInProgress = true;
                            var startTime = DateTime.Now;
                            ArmConfiguration armConfig = new ArmConfiguration();
                            switch (arm)
                            {
                                case RobotArm.Arm1:
                                    armConfig = Robot.Configuration.UpperArm;
                                    break;
                                case RobotArm.Arm2:
                                    armConfig = Robot.Configuration.LowerArm;
                                    break;
                            }
                            while (!processModule.IsReadyForTransfer(armConfig.EffectorType, out _)
                                   && (DateTime.Now - startTime).TotalSeconds
                                   < processModule.Configuration.InitializationTimeout)
                            {
                                Thread.Sleep(100);
                            }
                        }
                        finally
                        {
                            _waitPmReadyInProgress = false;
                        }
                    });

                var tasksToWait = new List<Task>
                {
                    Robot.GoToSpecifiedLocationAsync(selectedSource, 1, arm, true), pmTask
                };

                Task.WaitAll(tasksToWait.ToArray());

                Robot.Pick(arm, selectedSource, 1);

                processModule.PostTransfer();
            }
            else
            {
                Robot.Pick(arm, selectedSource, selectedSlotSource);
            }
        }

        private void Place(
            IMaterialLocationContainer selectedDestination,
            byte selectedSlotDestination,
            RobotArm arm)
        {
            if (selectedDestination is UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner
                aligner)
            {
                var tasksToWait = new List<Task>
                {
                    Robot.GoToSpecifiedLocationAsync(
                        selectedDestination,
                        selectedSlotDestination,
                        arm,
                        false),
                    aligner.PrepareTransferAsync(
                        arm == RobotArm.Arm1
                            ? Robot.Configuration.UpperArm.EffectorType
                            : Robot.Configuration.LowerArm.EffectorType,
                        arm == RobotArm.Arm1
                            ? Robot.UpperArmWaferDimension
                            : Robot.LowerArmWaferDimension,
                        arm == RobotArm.Arm1
                            ? Robot.UpperArmLocation.Wafer.MaterialType
                            : Robot.LowerArmLocation.Wafer.MaterialType)
                };

                Task.WaitAll(tasksToWait.ToArray());

                Robot.Place(arm, selectedDestination, selectedSlotDestination);
            }
            else if (selectedDestination is SmifLoadPort loadPort)
            {
                var tasksToWait = new List<Task>
                {
                    Robot.GoToSpecifiedLocationAsync(selectedDestination, 1, arm, false),
                    loadPort.GoToSlotAsync(selectedSlotDestination)
                };

                Task.WaitAll(tasksToWait.ToArray());

                //In case of SMIF Load port, robot will always access to slot 1
                //So we do a particular case by setting the slot to maxValue
                Robot.Place(arm, selectedDestination, byte.MaxValue);
            }
            else if (selectedDestination is UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule processModule)
            {
                var pmTask = Task.Run(
                    () =>
                    {
                        try
                        {
                            processModule.PrepareTransfer(
                                TransferType.Place,
                                arm,
                                arm == RobotArm.Arm1
                                ? Robot.UpperArmLocation.Wafer.MaterialType
                                : Robot.LowerArmLocation.Wafer.MaterialType,
                                arm == RobotArm.Arm1
                                ? Robot.UpperArmWaferDimension
                                : Robot.LowerArmWaferDimension);
                        }
                        catch (Exception)
                        {
                            _waitPmReadyInProgress = true;
                            var startTime = DateTime.Now;
                            ArmConfiguration armConfig;
                            Material armMaterial;
                            switch (arm)
                            {
                                case RobotArm.Arm1:
                                    armConfig = Robot.Configuration.UpperArm;
                                    armMaterial = Robot.UpperArmLocation.Material;
                                    break;
                                case RobotArm.Arm2:
                                    armConfig = Robot.Configuration.LowerArm;
                                    armMaterial = Robot.LowerArmLocation.Material;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(arm));
                            }
                            while (!processModule.IsReadyForTransfer(armConfig.EffectorType, out _, armMaterial, selectedSlotDestination)
                                   && (DateTime.Now - startTime).TotalSeconds
                                   < processModule.Configuration.InitializationTimeout)
                            {
                                Thread.Sleep(100);
                            }
                        }
                        finally
                        {
                            _waitPmReadyInProgress = false;
                        }
                    });

                var tasksToWait = new List<Task>
                {
                    Robot.GoToSpecifiedLocationAsync(selectedDestination, 1, arm, true), pmTask
                };

                Task.WaitAll(tasksToWait.ToArray());

                Robot.Place(arm, selectedDestination, selectedSlotDestination);

                processModule.PostTransfer();
            }
            else
            {
                Robot.Place(arm, selectedDestination, selectedSlotDestination);
            }
        }

        private bool PlaceCanExecute(bool isTransfer)
        {
            if (SelectedDestination == null)
            {
                return false;
            }

            if (Robot == null)
            {
                return false;
            }

            if (SelectedDestination is UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort lp)
            {
                var carrier = lp.Carrier;

                if (carrier == null)
                {
                    return false;
                }

                if (SelectedDestination is SmifLoadPort smifLoadPort)
                {
                    var loadPortContext = smifLoadPort.NewCommandContext(nameof(ISmifLoadPort.GoToSlot))
                        .AddArgument("slot", SelectedSlotDestination);

                    if (!smifLoadPort.CanExecute(loadPortContext))
                    {
                        return false;
                    }
                }

                if (!isTransfer)
                {
                    return Robot.CanExecute(
                        Robot.NewCommandContext(nameof(Robot.Place))
                            .AddArgument("arm", Arm)
                            .AddArgument("destinationDevice", SelectedDestination)
                            .AddArgument("destinationSlot", SelectedSlotDestination));
                }
            }

            if (SelectedDestination is UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner
                aligner)
            {
                //In case of aligner, PrepareTransfer command must be called on Aligner before calling Pick/Place command
                //So CanExecute will always be false in case of aligner
                //We check conditions here to allow to prepare the aligner
                if (!Robot.HasBeenInitialized
                    || Robot.State != OperatingModes.Idle
                    || aligner.State != OperatingModes.Idle
                    || aligner.Location.Material != null)
                {
                    return false;
                }

                if (!isTransfer)
                {
                    if (Arm == RobotArm.Arm1)
                    {
                        if (Robot.UpperArmLocation.Material == null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Robot.LowerArmLocation.Material == null)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (SelectedDestination is UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule processModule)
            {
                if (_waitPmReadyInProgress)
                {
                    return false;
                }

                var waferMaterialType = (Arm == RobotArm.Arm1 ? Robot.UpperArmLocation.Wafer?.MaterialType : Robot.LowerArmLocation.Wafer?.MaterialType) ?? MaterialType.Unknown;
                var waferMaterialDimension = (Arm == RobotArm.Arm1 ? Robot.UpperArmLocation.Wafer?.MaterialDimension : Robot.LowerArmLocation.Wafer?.MaterialDimension) ?? SampleDimension.NoDimension;
                var pmContext = processModule
                    .NewCommandContext(nameof(processModule.PrepareTransfer))
                    .AddArgument("transferType", TransferType.Place)
                    .AddArgument("arm", Arm)
                    .AddArgument("materialType", waferMaterialType )
                    .AddArgument("dimension", waferMaterialDimension);
                if (!processModule.CanExecute(pmContext))
                {
                    return false;
                }

                if (!isTransfer)
                {
                    if (Arm == RobotArm.Arm1)
                    {
                        if (Robot.UpperArmLocation.Material == null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Robot.LowerArmLocation.Material == null)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (isTransfer)
            {
                return true;
            }

            return Robot.CanExecute(
                Robot.NewCommandContext(nameof(Robot.Place))
                    .AddArgument("arm", Arm)
                    .AddArgument("destinationDevice", SelectedDestination)
                    .AddArgument("destinationSlot", SelectedSlotDestination));
        }


        #endregion
    }
}
