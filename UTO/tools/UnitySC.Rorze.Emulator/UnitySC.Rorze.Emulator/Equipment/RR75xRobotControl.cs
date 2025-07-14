using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.Rorze.Emulator.Common;
using UnitySC.Rorze.Emulator.Controls.RR75x;

namespace UnitySC.Rorze.Emulator.Equipment
{
    internal partial class RR75xRobotControl : BaseEquipmentControl.BaseEquipmentControl
    {
        #region Fields

        private bool _upperArmWaferPresence;
        private bool _lowerArmWaferPresence;

        private string _upperArmWaferId = "000000";
        private string _lowerArmWaferid = "000000";

        private string _loadPort1Mapping = "1111111111111111111111111";
        private string _loadPort2Mapping = "1111111111111111111111111";
        private string _loadPort3Mapping = "0000000000000000000000000";
        private string _loadPort4Mapping = "0000000000000000000000000";

        private bool _mappingPerformed;

        #endregion

        #region Constructor

        public RR75xRobotControl()
        {
            InitializeComponent();

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ExhaustFan, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ExhaustFanForUpperArm, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ExhaustFanForLowerArm, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.EmergencyStopTeachPendant, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.DeadManSwitch, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.PreparationComplete, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.DrivePower, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ZAxisExcitationOnOff, "1");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.RotationAxisExcitationOnOff,
                "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmExcitationOnOff, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationMode, "1");
            rr75xStatusWordSpyControl.UpdateStatus();

            rr75xStatusWordSpyControl.StatusChanged += Rr75xStatusWordSpyControl_StatusChanged;
            rR75xInputsOutputsControl1.StatusChanged += RR75xInputsOutputsControl1_StatusChanged;

            tLP1Mapping.Text = _loadPort1Mapping;
            tLP2Mapping.Text = _loadPort2Mapping;
            tLP3Mapping.Text = _loadPort3Mapping;
            tLP4Mapping.Text = _loadPort4Mapping;
        }

        #endregion

        #region Overrides

        public override void Clean()
        {
            rr75xStatusWordSpyControl.StatusChanged -= Rr75xStatusWordSpyControl_StatusChanged;
            rR75xInputsOutputsControl1.StatusChanged -= RR75xInputsOutputsControl1_StatusChanged;

            base.Clean();
        }

        protected override void TreatResponse(string toRespondeTo, string response)
        {
            if (!string.IsNullOrEmpty(toRespondeTo))
            {
                if (toRespondeTo.Contains("LOAD"))
                {
                    var loadParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var arm = int.Parse(loadParameters[0]);
                    var locationId = int.Parse(loadParameters[1]);
                    var slot = int.Parse(loadParameters[2]);

                    if (response.Contains("eTRB1.GWID")
                        && ((arm == 1 && !_upperArmWaferPresence)
                            || (arm == 2 && !_lowerArmWaferPresence)))
                    {
                        if (arm == 1)
                        {
                            _upperArmWaferPresence = true;
                        }
                        else
                        {
                            _lowerArmWaferPresence = true;
                        }

                        OnWaferPickedByRobot(locationId, slot);
                    }
                }
                else if (toRespondeTo.Contains("UNLD"))
                {
                    var unloadParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var arm = int.Parse(unloadParameters[0]);
                    var locationId = int.Parse(unloadParameters[1]);
                    var slot = int.Parse(unloadParameters[2]);

                    if (response.Contains("eTRB1.GPIO")
                        && !GetArmSolenoidValueFromConcatenatedString(
                            response.Remove(0, 11).TrimEnd('\r'),
                            arm)
                        && ((arm == 1 && _upperArmWaferPresence)
                            || (arm == 2 && _lowerArmWaferPresence)))
                    {
                        if (arm == 1)
                        {
                            _upperArmWaferPresence = false;
                        }
                        else
                        {
                            _lowerArmWaferPresence = false;
                        }

                        OnWaferPlacedByRobot(locationId, slot);
                    }
                }
                else if (toRespondeTo.Contains("EXCH"))
                {
                    var swapParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var pickArm = int.Parse(swapParameters[0]);
                    var placeArm = 1;
                    if (pickArm == 1)
                    {
                        placeArm = 2;
                    }

                    var locationId = int.Parse(swapParameters[1]);
                    var slot = int.Parse(swapParameters[2]);

                    if (response.Contains("eTRB1.GWID")
                        && ((pickArm == 1 && !_upperArmWaferPresence)
                            || (pickArm == 2 && !_lowerArmWaferPresence)))
                    {
                        if (pickArm == 1)
                        {
                            _upperArmWaferPresence = true;
                        }
                        else
                        {
                            _lowerArmWaferPresence = true;
                        }

                        OnWaferPickedByRobot(locationId, slot);
                    }
                    else if (response.Contains("eTRB1.GPIO")
                             && !GetArmSolenoidValueFromConcatenatedString(
                                 response.Remove(0, 11).TrimEnd('\r'),
                                 placeArm)
                             && ((placeArm == 1 && _upperArmWaferPresence)
                                 || (placeArm == 2 && _lowerArmWaferPresence)))
                    {
                        if (placeArm == 1)
                        {
                            _upperArmWaferPresence = false;
                        }
                        else
                        {
                            _lowerArmWaferPresence = false;
                        }

                        OnWaferPlacedByRobot(locationId, slot);
                    }
                }
                else if (toRespondeTo.Contains("TRNS"))
                {
                    var transferParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var pickArm = int.Parse(transferParameters[0]);
                    var sourceLocation = int.Parse(transferParameters[1]);
                    var sourceSlot = int.Parse(transferParameters[2]);
                    var placeArm = int.Parse(transferParameters[3]);
                    var destinationLocation = int.Parse(transferParameters[4]);
                    var destinationSlot = int.Parse(transferParameters[5]);

                    if (response.Contains("eTRB1.GWID")
                        && ((pickArm == 1 && !_upperArmWaferPresence)
                            || (pickArm == 2 && !_lowerArmWaferPresence)))
                    {
                        if (pickArm == 1)
                        {
                            _upperArmWaferPresence = true;
                        }
                        else
                        {
                            _lowerArmWaferPresence = true;
                        }

                        OnWaferPickedByRobot(sourceLocation, sourceSlot);
                    }
                    else if (response.Contains("eTRB1.GPIO")
                             && !GetArmSolenoidValueFromConcatenatedString(
                                 response.Remove(0, 11).TrimEnd('\r'),
                                 placeArm)
                             && ((placeArm == 1 && _upperArmWaferPresence)
                                 || (placeArm == 2 && _lowerArmWaferPresence)))
                    {
                        if (placeArm == 1)
                        {
                            _upperArmWaferPresence = false;
                        }
                        else
                        {
                            _lowerArmWaferPresence = false;
                        }

                        OnWaferPlacedByRobot(destinationLocation, destinationSlot);
                    }
                }
            }
        }

        protected override List<string> GetResponses(string toRespondeTo)
        {
            var answers = new List<string>();
            if (!string.IsNullOrEmpty(toRespondeTo))
            {
                if (toRespondeTo.Contains(".STIM"))
                {
                    answers.Add("aTRB1.STIM:\r");
                }
                else if (toRespondeTo.Contains(".EVNT"))
                {
                    answers.Add("aTRB1.EVNT:\r");
                    var eventParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var evnt = int.Parse(eventParameters[0]);
                    var isActive = int.Parse(eventParameters[1]);
                    if (isActive == 1)
                    {
                        switch (evnt)
                        {
                            case 0:
                                answers.Add(
                                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                answers.Add("eTRB1.GPOS:999/999/999/999/999\r");
                                answers.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");
                                answers.Add(
                                    $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                                break;
                            case 1:
                                answers.Add(
                                    $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                                break;

                            case 2:
                                answers.Add(
                                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");
                                break;

                            case 3:
                                answers.Add("eTRB1.GPOS:999/999/999/999/999\r");
                                break;

                            case 4:
                                answers.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");
                                break;
                        }
                    }
                }
                else if (toRespondeTo.Contains(".STAT"))
                {
                    answers.Add(
                        $"aTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".GPIO"))
                {
                    answers.Add(
                        $"aTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GPOS"))
                {
                    answers.Add("aTRB1.GPOS:999/999/999/999/999\r");
                }
                else if (toRespondeTo.Contains("GWID"))
                {
                    answers.Add($"aTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");
                }
                else if (toRespondeTo.Contains(".RSTA"))
                {
                    answers.Add("aTRB1.RSTA:\r");

                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "1");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "0");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains(".INIT"))
                {
                    answers.Add("aTRB1.INIT:\r");
                    answers.AddRange(PerformInit());
                }
                else if (toRespondeTo.Contains(".CLMP"))
                {
                    answers.Add("aTRB1.CLMP:\r");
                    var clampParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var arm = int.Parse(clampParameters[0]);
                    answers.AddRange(PerformClamp(arm));
                }
                else if (toRespondeTo.Contains(".UCLM"))
                {
                    answers.Add("aTRB1.UCLM:\r");
                }
                else if (toRespondeTo.Contains(".ORGN"))
                {
                    answers.Add("aTRB1.ORGN:\r");
                    answers.AddRange(PerformOrigin());
                }
                else if (toRespondeTo.Contains(".LOAD"))
                {
                    answers.Add("aTRB1.LOAD:\r");

                    var loadParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var arm = int.Parse(loadParameters[0]);
                    var locationId = int.Parse(loadParameters[1]);
                    var slot = int.Parse(loadParameters[2]);
                    answers.AddRange(PerformLoad(arm, locationId, slot));
                }
                else if (toRespondeTo.Contains("UNLD"))
                {
                    answers.Add("aTRB1.UNLD:\r");

                    var unloadParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var arm = int.Parse(unloadParameters[0]);
                    var locationId = int.Parse(unloadParameters[1]);
                    answers.AddRange(PerformUnload(arm, locationId));
                }
                else if (toRespondeTo.Contains("EXCH"))
                {
                    answers.Add("aTRB1.EXCH:\r");
                    var swapParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var arm = int.Parse(swapParameters[0]);
                    var locationId = int.Parse(swapParameters[1]);
                    var slot = int.Parse(swapParameters[2]);
                    answers.AddRange(PerformSwap(arm, locationId, slot));
                }
                else if (toRespondeTo.Contains("TRNS"))
                {
                    answers.Add("aTRB1.TRNS:\r");
                    var swapParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var pickArm = int.Parse(swapParameters[0]);
                    var sourceLocationId = int.Parse(swapParameters[1]);
                    var sourceSlot = int.Parse(swapParameters[2]);
                    var placeArm = int.Parse(swapParameters[3]);
                    var destinationLocationId = int.Parse(swapParameters[4]);
                    answers.AddRange(
                        PerformTransfer(
                            pickArm,
                            sourceLocationId,
                            sourceSlot,
                            placeArm,
                            destinationLocationId));
                }
                else if (toRespondeTo.Contains("HOME"))
                {
                    answers.Add("aTRB1.HOME:\r");

                    var locationId = 0;

                    //Case where the location is specified in the HOME command
                    if (toRespondeTo.Length > 11)
                    {
                        var homeParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                        locationId = int.Parse(homeParameters[2]);
                    }

                    answers.AddRange(PerformHome(locationId));
                }
                else if (toRespondeTo.Contains("SSPD"))
                {
                    answers.Add("aTRB1.SSPD:\r");

                    var setSpeedParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var requestedSpeed = int.Parse(setSpeedParameters[1]);

                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "1");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

                    rr75xStatusWordSpyControl.SetStatus(
                        Rr75xStatus.MotionSpeed,
                        RorzeHelpers.RequestedSpeedToRorzeSpeed(requestedSpeed).ToString());
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "0");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("STOP"))
                {
                    answers.Add("aTRB1.STOP:\r");
                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "1");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "0");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("PAUS"))
                {
                    answers.Add("aTRB1.PAUS:\r");
                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "1");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.CommandProcessing, "0");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                    rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "2");
                    rr75xStatusWordSpyControl.UpdateStatus();
                    answers.Add(
                        $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
                }
                else if (toRespondeTo.Contains("GVER"))
                {
                    answers.Add("aTRB1.GVER:RORZE STD_TBR RR75 Ver 1.00\r");
                }
                else if (toRespondeTo.Contains("WMAP"))
                {
                    var mapParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var locationId = int.Parse(mapParameters[0]);

                    answers.Add($"aTRB1.WMAP:\r");
                    answers.AddRange(PerformMapping(locationId));
                }
                else if (toRespondeTo.Contains("GMAP"))
                {
                    var mapParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var locationId = int.Parse(mapParameters[0]);

                    switch (locationId)
                    {
                        case Constants.LoadPort1Mapping100LocationId1:
                        case Constants.LoadPort1Mapping100LocationId2:
                        case Constants.LoadPort1Mapping150LocationId1:
                        case Constants.LoadPort1Mapping150LocationId2:
                        case Constants.LoadPort1Mapping200LocationId1:
                        case Constants.LoadPort1Mapping200LocationId2:
                            answers.Add($"aTRB1.GMAP:{_loadPort1Mapping}\r");
                            break;
                        case Constants.LoadPort2Mapping100LocationId1:
                        case Constants.LoadPort2Mapping100LocationId2:
                        case Constants.LoadPort2Mapping150LocationId1:
                        case Constants.LoadPort2Mapping150LocationId2:
                        case Constants.LoadPort2Mapping200LocationId1:
                        case Constants.LoadPort2Mapping200LocationId2:
                            answers.Add($"aTRB1.GMAP:{_loadPort2Mapping}\r");
                            break;
                        case Constants.LoadPort3Mapping100LocationId1:
                        case Constants.LoadPort3Mapping100LocationId2:
                        case Constants.LoadPort3Mapping150LocationId1:
                        case Constants.LoadPort3Mapping150LocationId2:
                        case Constants.LoadPort3Mapping200LocationId1:
                        case Constants.LoadPort3Mapping200LocationId2:
                            answers.Add($"aTRB1.GMAP:{_loadPort3Mapping}\r");
                            break;
                        case Constants.LoadPort4Mapping100LocationId1:
                        case Constants.LoadPort4Mapping100LocationId2:
                        case Constants.LoadPort4Mapping150LocationId1:
                        case Constants.LoadPort4Mapping150LocationId2:
                        case Constants.LoadPort4Mapping200LocationId1:
                        case Constants.LoadPort4Mapping200LocationId2:
                            answers.Add($"aTRB1.GMAP:{_loadPort4Mapping}\r");
                            break;
                    }

                    _mappingPerformed = true;
                }
                else if (toRespondeTo.Contains("ARM1.DCMD"))
                {
                    answers.Add("aTRB1.ARM1.DCMD:\r");
                }
                else if (toRespondeTo.Contains("EXTD"))
                {
                    answers.Add("aTRB1.EXTD:\r");
                    var loadParameters = RorzeHelpers.GetCommandParameter(toRespondeTo);
                    var arm = int.Parse(loadParameters[1]);
                    var locationId = int.Parse(loadParameters[2]);
                    answers.AddRange(PerformExtend(arm, locationId));
                }
            }

            return answers;
        }

        protected override void SendConnectedMessage()
        {
            SendMessage("eTRB1.CNCT:\r");
        }

        #endregion

        #region Events

        public EventHandler<WaferMovedEventArgs> WaferPickedByRobot;

        public EventHandler<WaferMovedEventArgs> WaferPlacedByRobot;

        #endregion

        #region Event Handlers

        private void RR75xInputsOutputsControl1_StatusChanged(object sender, EventArgs e)
        {
            SendMessage($"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");
        }

        private void Rr75xStatusWordSpyControl_StatusChanged(object sender, EventArgs e)
        {
            SendMessage($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");
        }

        private void bLP1SetMapping_Click(object sender, EventArgs e)
        {
            _loadPort1Mapping = tLP1Mapping.Text;
        }

        private void bLP2SetMapping_Click(object sender, EventArgs e)
        {
            _loadPort2Mapping = tLP2Mapping.Text;
        }

        private void bLP3SetMapping_Click(object sender, EventArgs e)
        {
            _loadPort3Mapping = tLP3Mapping.Text;
        }

        private void bLP4SetMapping_Click(object sender, EventArgs e)
        {
            _loadPort4Mapping = tLP4Mapping.Text;
        }

        #endregion

        #region Private Methods

        private List<string> PerformClamp(int arm)
        {
            var clampStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            clampStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            switch (arm)
            {
                //Upper arm
                case 4:
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                        "0");
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                        "1");
                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                        "1");

                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                        _upperArmWaferPresence
                            ? "1"
                            : "0");

                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                        "1");
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                        "0");
                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                        "0");
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                        "0");
                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                    break;

                //Lower arm
                case 5:
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                        "0");
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                        "1");
                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmWaferPresence2,
                        "1");
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmWaferPresence1,
                        _lowerArmWaferPresence
                            ? "1"
                            : "0");
                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                        "1");
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                        "0");
                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmWaferPresence1,
                        "0");
                    rR75xInputsOutputsControl1.SetStatus(
                        Rr75xInputsOuputs.LowerArmWaferPresence2,
                        "0");
                    rR75xInputsOutputsControl1.UpdateStatus();
                    clampStrings.Add(
                        $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");
                    break;
            }

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            clampStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return clampStrings;
        }

        private List<string> PerformInit()
        {
            var initStrings = new List<string>();

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.PreparationComplete, "0");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmSolenoidValveOff, "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ZAxisExcitationOnOff, "0");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.RotationAxisExcitationOnOff,
                "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmExcitationOnOff, "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmExcitationOnOff, "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            initStrings.Add("eTRB1.GPOS:999/999/999/999/999\r");
            initStrings.Add("eTRB1.GWID:000000,000000\r");

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationMode, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            initStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ZAxisExcitationOnOff, "1");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.RotationAxisExcitationOnOff,
                "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmExcitationOnOff, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.PreparationComplete, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            initStrings.Add($"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationMode, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            initStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return initStrings;
        }

        private List<string> PerformOrigin()
        {
            var originStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            originStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff, "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add("eTRB1.GPOS:147/001/147/000/000\r");

            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmSolenoidValveOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                "1");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "1");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.LowerArmWaferPresence1,
                _upperArmWaferPresence
                    ? "1"
                    : "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                "0");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "0");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                "1");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "1");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.LowerArmWaferPresence1,
                _lowerArmWaferPresence
                    ? "1"
                    : "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                "0");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add("eTRB1.GPOS:999/999/999/999/999\r");
            originStrings.Add("eTRB1.GPOS:999/999/999/000/999\r");
            originStrings.Add("eTRB1.GPOS:999/999/999/000/000\r");
            originStrings.Add("eTRB1.GPOS:999/000/999/000/000\r");
            originStrings.Add("eTRB1.GPOS:999/000/000/000/000\r");
            originStrings.Add("eTRB1.GPOS:999/999/999/999/999\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff, "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ZAxisExcitationOnOff, "0");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.RotationAxisExcitationOnOff,
                "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmExcitationOnOff, "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmExcitationOnOff, "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff, "0");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.ZAxisExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff, "1");
            rR75xInputsOutputsControl1.SetStatus(
                Rr75xInputsOuputs.RotationAxisExcitationOnOff,
                "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmExcitationOnOff, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            originStrings.Add("eTRB1.GPOS:000/000/000/000/000\r");

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OriginSearchCompleteFlag, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            originStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "1");
            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmOrigin, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            originStrings.Add(
                $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            return originStrings;
        }

        private List<string> PerformHome(int locationId)
        {
            var homeStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            homeStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            if (_mappingPerformed)
            {
                _mappingPerformed = false;
                homeStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
            }

            homeStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            homeStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return homeStrings;
        }

        private List<string> PerformLoad(int arm, int locationId, int slot)
        {
            var loadStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            loadStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            if (arm == 1)
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    loadStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                loadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/000\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                loadStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/{locationId:000}/000\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _upperArmWaferId = $"{locationId:000}{slot:000}";
                loadStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                loadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/000\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                loadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");
            }
            else
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    loadStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                loadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/999\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmOrigin, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                loadStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/{locationId:000}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _lowerArmWaferid = $"{locationId:000}{slot:000}";
                loadStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                loadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/999\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmOrigin, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                loadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                loadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");
            }

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            loadStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return loadStrings;
        }

        private List<string> PerformUnload(int arm, int locationId)
        {
            var unloadStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            unloadStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            if (arm == 1)
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    unloadStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                unloadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/000\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                unloadStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/{locationId:000}/000\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _upperArmWaferId = "000000";
                unloadStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                unloadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/000\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                unloadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");
            }
            else
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    unloadStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                unloadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/999\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmOrigin, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                unloadStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/{locationId:000}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _lowerArmWaferid = "000000";
                unloadStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                unloadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/999\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmOrigin, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                unloadStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                unloadStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");
            }

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            unloadStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return unloadStrings;
        }

        private List<string> PerformSwap(int arm, int locationId, int slot)
        {
            var swapStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            swapStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            if (arm == 1)
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    swapStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                swapStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/000\r");
                swapStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/{locationId:000}/000\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _upperArmWaferId = $"{locationId:000}{slot:000}";
                swapStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                swapStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/{locationId:000}/999\r");
                swapStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/999\r");
                swapStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/{locationId:000}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "0");
                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _lowerArmWaferid = "000000";
                swapStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                swapStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");
            }
            else
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    swapStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                swapStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");
                swapStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/999\r");
                swapStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/{locationId:000}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "1");
                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _lowerArmWaferid = $"{locationId:000}{slot:000}";
                swapStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                swapStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/{locationId:000}\r");
                swapStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/000\r");
                swapStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/{locationId:000}/000\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                swapStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _upperArmWaferId = "000000";
                swapStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                swapStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");
            }

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            swapStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return swapStrings;
        }

        private List<string> PerformTransfer(
            int pickArm,
            int sourceLocationId,
            int sourceSlot,
            int placeArm,
            int destinationLocationId)
        {
            var transferStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            transferStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            if (pickArm == 1)
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    transferStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                transferStrings.Add(
                    $"eTRB1.GPOS:{sourceLocationId:000}/001/{sourceLocationId:000}/999/000\r");
                transferStrings.Add(
                    $"eTRB1.GPOS:{sourceLocationId:000}/001/{sourceLocationId:000}/{sourceLocationId:000}/000\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _upperArmWaferId = $"{sourceLocationId:000}{sourceSlot:000}";
                transferStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                transferStrings.Add(
                    $"eTRB1.GPOS:{sourceLocationId:000}/001/{sourceLocationId:000}/000/000\r");
            }
            else
            {

                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    transferStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                transferStrings.Add(
                    $"eTRB1.GPOS:{sourceLocationId:000}/001/{sourceLocationId:000}/000/999\r");
                transferStrings.Add(
                    $"eTRB1.GPOS:{sourceLocationId:000}/001/{sourceLocationId:000}/000/{sourceLocationId:000}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                    "1");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "1");
                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _lowerArmWaferid = $"{sourceLocationId:000}{sourceSlot:000}";
                transferStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                transferStrings.Add(
                    $"eTRB1.GPOS:{sourceLocationId:000}/001/{sourceLocationId:000}/000/000\r");
            }

            if (placeArm == 1)
            {
                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    transferStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                transferStrings.Add(
                    $"eTRB1.GPOS:{destinationLocationId:000}/001/{destinationLocationId:000}/999/000\r");
                transferStrings.Add(
                    $"eTRB1.GPOS:{destinationLocationId:000}/001/{destinationLocationId:000}/{destinationLocationId:000}/000\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence1,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.UpperArmFinger1WaferPresence2,
                    "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _upperArmWaferId = $"000000";
                transferStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                transferStrings.Add(
                    $"eTRB1.GPOS:{destinationLocationId:000}/001/{destinationLocationId:000}/000/000\r");
            }
            else
            {

                if (_mappingPerformed)
                {
                    _mappingPerformed = false;
                    transferStrings.Add($"eTRB1.GPOS:999/001/999/000/000\r");
                }

                transferStrings.Add(
                    $"eTRB1.GPOS:{destinationLocationId:000}/001/{destinationLocationId:000}/000/999\r");
                transferStrings.Add(
                    $"eTRB1.GPOS:{destinationLocationId:000}/001/{destinationLocationId:000}/000/{destinationLocationId:000}\r");

                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOn,
                    "0");
                rR75xInputsOutputsControl1.SetStatus(
                    Rr75xInputsOuputs.LowerArmSolenoidValveOff,
                    "1");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1, "0");
                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                transferStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                _lowerArmWaferid = $"000000";
                transferStrings.Add($"eTRB1.GWID:{_upperArmWaferId},{_lowerArmWaferid}\r");

                transferStrings.Add(
                    $"eTRB1.GPOS:{destinationLocationId:000}/001/{destinationLocationId:000}/000/000\r");
            }

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            transferStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return transferStrings;
        }

        private List<string> PerformMapping(int locationId)
        {
            var mappingStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            mappingStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            mappingStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "0");
            rR75xInputsOutputsControl1.UpdateStatus();
            mappingStrings.Add($"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            mappingStrings.Add($"eTRB1.GPOS:{locationId:000}/999/{locationId:000}/000/000\r");
            mappingStrings.Add($"eTRB1.GPOS:{locationId:000}/025/{locationId:000}/000/000\r");
            mappingStrings.Add($"eTRB1.GPOS:{locationId:000}/999/{locationId:000}/000/000\r");
            mappingStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/000\r");

            rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "1");
            rR75xInputsOutputsControl1.UpdateStatus();
            mappingStrings.Add($"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();

            mappingStrings.Add($"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return mappingStrings;
        }

        private List<string> PerformExtend(int arm, int locationId)
        {
            var extendStrings = new List<string>();

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "1");
            rr75xStatusWordSpyControl.UpdateStatus();
            extendStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            if (arm == 1)
            {
                extendStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/999/000\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.UpperArmOrigin, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                extendStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                extendStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/{locationId:000}/000\r");
            }
            else
            {
                extendStrings.Add($"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/999\r");

                rR75xInputsOutputsControl1.SetStatus(Rr75xInputsOuputs.LowerArmOrigin, "0");
                rR75xInputsOutputsControl1.UpdateStatus();
                extendStrings.Add(
                    $"eTRB1.GPIO:{rR75xInputsOutputsControl1.GetConcatenatedStatuses()}\r");

                extendStrings.Add(
                    $"eTRB1.GPOS:{locationId:000}/001/{locationId:000}/000/{locationId:000}\r");
            }

            rr75xStatusWordSpyControl.SetStatus(Rr75xStatus.OperationStatus, "0");
            rr75xStatusWordSpyControl.UpdateStatus();
            extendStrings.Add(
                $"eTRB1.STAT:{rr75xStatusWordSpyControl.GetConcatenatedStatuses()}\r");

            return extendStrings;
        }

        private void OnWaferPickedByRobot(int locationId, int slot)
        {
            Task.Run(
                () =>
                {
                    Thread.Sleep(1000);
                    WaferPickedByRobot?.Invoke(this, new WaferMovedEventArgs(locationId, slot));
                });
        }

        private void OnWaferPlacedByRobot(int locationId, int slot)
        {
            WaferPlacedByRobot?.Invoke(this, new WaferMovedEventArgs(locationId, slot));
        }

        public bool GetArmSolenoidValueFromConcatenatedString(string statuses, int arm)
        {
            var splittedStatuses = statuses.Split('/');
            var outputs = new BitArray(
                new[] { int.Parse(splittedStatuses[1], NumberStyles.HexNumber) });

            if (arm == 1)
            {
                return outputs[8];
            }

            return outputs[18];
        }

        #endregion
    }
}
