using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RR75x
{
    public enum Rr75xInputsOuputs
    {
        EmergencyStop,
        PauseInput,
        VacuumSourcePressure,
        AirSourcePressure,
        ExhaustFan,
        ExhaustFanForUpperArm,
        ExhaustFanForLowerArm,
        UpperArmFinger1WaferPresence1,
        UpperArmFinger1WaferPresence2,
        UpperArmFinger2WaferPresence1,
        UpperArmFinger2WaferPresence2,
        UpperArmFinger3WaferPresence1,
        UpperArmFinger3WaferPresence2,
        UpperArmFinger4WaferPresence1,
        UpperArmFinger4WaferPresence2,
        UpperArmFinger5WaferPresence1,
        UpperArmFinger5WaferPresence2,
        LowerArmWaferPresence1,
        LowerArmWaferPresence2,
        EmergencyStopTeachPendant,
        DeadManSwitch,
        ModeKey,
        InterlockInput0,
        InterlockInput1,
        InterlockInput2,
        InterlockInput3,
        Sensor1ForTeaching,
        Sensor2ForTeaching,
        PreparationComplete,
        PauseOutput,
        FatalError,
        LightError,
        ZAxisBrakeOff,
        BatteryVoltageTooLow,
        DrivePower,
        TorqueLimitation,
        UpperArmFinger1SolenoidValveOn,
        UpperArmFinger1SolenoidValveOff,
        UpperArmFinger2SolenoidValveOn,
        UpperArmFinger2SolenoidValveOff,
        UpperArmFinger3SolenoidValveOn,
        UpperArmFinger3SolenoidValveOff,
        UpperArmFinger4SolenoidValveOn,
        UpperArmFinger4SolenoidValveOff,
        UpperArmFinger5SolenoidValveOn,
        UpperArmFinger5SolenoidValveOff,
        LowerArmSolenoidValveOn,
        LowerArmSolenoidValveOff,
        XAxisExcitationOnOff,
        ZAxisExcitationOnOff,
        RotationAxisExcitationOnOff,
        UpperArmExcitationOnOff,
        LowerArmExcitationOnOff,
        UpperArmOrigin,
        LowerArmOrigin
    }

    public partial class RR75xInputsOutputsControl : InputsOutputsControl
    {
        public RR75xInputsOutputsControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Rr75xInputsOuputs.EmergencyStop, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Rr75xInputsOuputs.PauseInput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Rr75xInputsOuputs.VacuumSourcePressure, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Rr75xInputsOuputs.AirSourcePressure, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Rr75xInputsOuputs.ExhaustFan, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Rr75xInputsOuputs.ExhaustFanForUpperArm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Rr75xInputsOuputs.ExhaustFanForLowerArm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Rr75xInputsOuputs.UpperArmFinger1WaferPresence1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Rr75xInputsOuputs.UpperArmFinger1WaferPresence2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Rr75xInputsOuputs.UpperArmFinger2WaferPresence1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(11, Rr75xInputsOuputs.UpperArmFinger2WaferPresence2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(12, Rr75xInputsOuputs.UpperArmFinger3WaferPresence1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(13, Rr75xInputsOuputs.UpperArmFinger3WaferPresence2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(14, Rr75xInputsOuputs.UpperArmFinger4WaferPresence1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(15, Rr75xInputsOuputs.UpperArmFinger4WaferPresence2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(16, Rr75xInputsOuputs.UpperArmFinger5WaferPresence1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(17, Rr75xInputsOuputs.UpperArmFinger4WaferPresence2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(18, Rr75xInputsOuputs.LowerArmWaferPresence1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(19, Rr75xInputsOuputs.LowerArmWaferPresence2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(20, Rr75xInputsOuputs.EmergencyStopTeachPendant, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(21, Rr75xInputsOuputs.DeadManSwitch, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(22, Rr75xInputsOuputs.ModeKey, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(23, Rr75xInputsOuputs.InterlockInput0, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(24, Rr75xInputsOuputs.InterlockInput1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(25, Rr75xInputsOuputs.InterlockInput2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(26, Rr75xInputsOuputs.InterlockInput3, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(27, Rr75xInputsOuputs.Sensor1ForTeaching, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(28, Rr75xInputsOuputs.Sensor2ForTeaching, "0: False/1: True", "0");

            paramDataGridView.Rows.Add(29, Rr75xInputsOuputs.PreparationComplete, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(30, Rr75xInputsOuputs.PauseOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(31, Rr75xInputsOuputs.FatalError, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(32, Rr75xInputsOuputs.LightError, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(33, Rr75xInputsOuputs.ZAxisBrakeOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(34, Rr75xInputsOuputs.BatteryVoltageTooLow, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(35, Rr75xInputsOuputs.DrivePower, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(36, Rr75xInputsOuputs.TorqueLimitation, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(37, Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(38, Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(39, Rr75xInputsOuputs.UpperArmFinger2SolenoidValveOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(40, Rr75xInputsOuputs.UpperArmFinger2SolenoidValveOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(41, Rr75xInputsOuputs.UpperArmFinger3SolenoidValveOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(42, Rr75xInputsOuputs.UpperArmFinger3SolenoidValveOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(43, Rr75xInputsOuputs.UpperArmFinger4SolenoidValveOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(44, Rr75xInputsOuputs.UpperArmFinger4SolenoidValveOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(45, Rr75xInputsOuputs.UpperArmFinger5SolenoidValveOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(46, Rr75xInputsOuputs.UpperArmFinger5SolenoidValveOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(47, Rr75xInputsOuputs.LowerArmSolenoidValveOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(48, Rr75xInputsOuputs.LowerArmSolenoidValveOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(49, Rr75xInputsOuputs.XAxisExcitationOnOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(50, Rr75xInputsOuputs.ZAxisExcitationOnOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(51, Rr75xInputsOuputs.RotationAxisExcitationOnOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(52, Rr75xInputsOuputs.UpperArmExcitationOnOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(53, Rr75xInputsOuputs.LowerArmExcitationOnOff, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(54, Rr75xInputsOuputs.UpperArmOrigin, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(55, Rr75xInputsOuputs.LowerArmOrigin, "0: False/1: True", "0");
        }

        public string GetStatus(Rr75xInputsOuputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Rr75xInputsOuputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                GetStatus(Rr75xInputsOuputs.EmergencyStop) == "1",
                GetStatus(Rr75xInputsOuputs.PauseInput) == "1",
                GetStatus(Rr75xInputsOuputs.VacuumSourcePressure) == "1",
                GetStatus(Rr75xInputsOuputs.AirSourcePressure) == "1",
                false,
                GetStatus(Rr75xInputsOuputs.ExhaustFan) == "1",
                GetStatus(Rr75xInputsOuputs.ExhaustFanForUpperArm) == "1",
                GetStatus(Rr75xInputsOuputs.ExhaustFanForLowerArm) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger1WaferPresence1) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger1WaferPresence2) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger2WaferPresence1) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger2WaferPresence2) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger3WaferPresence1) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger3WaferPresence2) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger4WaferPresence1) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger4WaferPresence2) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger5WaferPresence1) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger5WaferPresence2) == "1",
                GetStatus(Rr75xInputsOuputs.LowerArmWaferPresence1) == "1",
                GetStatus(Rr75xInputsOuputs.LowerArmWaferPresence2) == "1",
                false,
                GetStatus(Rr75xInputsOuputs.EmergencyStopTeachPendant) == "1",
                GetStatus(Rr75xInputsOuputs.DeadManSwitch) == "1",
                GetStatus(Rr75xInputsOuputs.ModeKey) == "1",
                GetStatus(Rr75xInputsOuputs.InterlockInput0) == "1",
                GetStatus(Rr75xInputsOuputs.InterlockInput1) == "1",
                GetStatus(Rr75xInputsOuputs.InterlockInput2) == "1",
                GetStatus(Rr75xInputsOuputs.InterlockInput3) == "1",
                GetStatus(Rr75xInputsOuputs.Sensor1ForTeaching) == "1",
                GetStatus(Rr75xInputsOuputs.Sensor2ForTeaching) == "1",
                false,
                false
            });

            BitArray outputs = new BitArray(new[]
            {
                GetStatus(Rr75xInputsOuputs.PreparationComplete) == "1",
                GetStatus(Rr75xInputsOuputs.PauseOutput) == "1",
                GetStatus(Rr75xInputsOuputs.FatalError) == "1",
                GetStatus(Rr75xInputsOuputs.LightError) == "1",
                GetStatus(Rr75xInputsOuputs.ZAxisBrakeOff) == "1",
                GetStatus(Rr75xInputsOuputs.BatteryVoltageTooLow) == "1",
                GetStatus(Rr75xInputsOuputs.DrivePower) == "1",
                GetStatus(Rr75xInputsOuputs.TorqueLimitation) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOn) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger1SolenoidValveOff) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger2SolenoidValveOn) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger2SolenoidValveOff) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger3SolenoidValveOn) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger3SolenoidValveOff) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger4SolenoidValveOn) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger4SolenoidValveOff) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger5SolenoidValveOn) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmFinger5SolenoidValveOff) == "1",
                GetStatus(Rr75xInputsOuputs.LowerArmSolenoidValveOn) == "1",
                GetStatus(Rr75xInputsOuputs.LowerArmSolenoidValveOff) == "1",
                false,
                false,
                false,
                false,
                GetStatus(Rr75xInputsOuputs.XAxisExcitationOnOff) == "1",
                GetStatus(Rr75xInputsOuputs.ZAxisExcitationOnOff) == "1",
                GetStatus(Rr75xInputsOuputs.RotationAxisExcitationOnOff) == "1",
                GetStatus(Rr75xInputsOuputs.UpperArmExcitationOnOff) == "1",
                GetStatus(Rr75xInputsOuputs.LowerArmExcitationOnOff) == "1",
                false,
                GetStatus(Rr75xInputsOuputs.UpperArmOrigin) == "1",
                GetStatus(Rr75xInputsOuputs.LowerArmOrigin) == "1",
            });

            stringBuilder.Append(BitArrayToHexaString(inputs));
            stringBuilder.Append("/");
            stringBuilder.Append(BitArrayToHexaString(outputs));

            return stringBuilder.ToString();
        }
    }
}
