using System.Collections;
using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RV101
{
    internal enum Rv101InputsOutputs
    {
        EmergencyStop,
        TemporarilyStopInput,
        VacuumSourcePressure,
        AirSupplySourcePressure,
        ProtrusionDetection,
        Cover,
        DrivePower,
        MappingSensor,
        ShutterOpenInput,
        ShutterCloseInput,
        PresenceLeft,
        PresenceRight,
        PresenceMiddle,
        InfoPadA,
        InfoPadB,
        InfoPadC,
        InfoPadD,
        PresenceLeft200Mm,
        PresenceRight200Mm,
        PresenceLeft150Mm,
        PresenceRight150Mm,
        AccessSwitch1,
        AccessSwitch2,
        PreparationCompleted,
        TemporarilyStopOutput,
        SignificantError,
        LightError,
        ClampMovingDirection,
        ClampMotionStart,
        ShutterOpenOutput,
        ShutterCloseOutput,
        ShutterMotionDisabled,
        ShutterOpenOutput2,
        CoverLock,
        CarrierPresenceSensorOn,
        PreparationCompleted2,
        CarrierProperPlaced,
        AccessSwitch1Output,
        AccessSwitch2Output,
        Load,
        Unload,
        Presence,
        Placement,
        Latch,
        Error,
        Busy
    }
    internal partial class Rv101InputsOutputsControl : InputsOutputsControl
    {
        public Rv101InputsOutputsControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Rv101InputsOutputs.EmergencyStop, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Rv101InputsOutputs.TemporarilyStopInput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Rv101InputsOutputs.VacuumSourcePressure, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Rv101InputsOutputs.AirSupplySourcePressure, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Rv101InputsOutputs.ProtrusionDetection, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Rv101InputsOutputs.Cover, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Rv101InputsOutputs.DrivePower, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Rv101InputsOutputs.MappingSensor, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Rv101InputsOutputs.ShutterOpenInput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Rv101InputsOutputs.ShutterCloseInput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(11, Rv101InputsOutputs.PresenceLeft, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(12, Rv101InputsOutputs.PresenceRight, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(13, Rv101InputsOutputs.PresenceMiddle, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(14, Rv101InputsOutputs.InfoPadA, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(15, Rv101InputsOutputs.InfoPadB, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(16, Rv101InputsOutputs.InfoPadC, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(17, Rv101InputsOutputs.InfoPadD, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(18, Rv101InputsOutputs.PresenceLeft200Mm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(19, Rv101InputsOutputs.PresenceRight200Mm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(20, Rv101InputsOutputs.PresenceLeft150Mm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(21, Rv101InputsOutputs.PresenceRight150Mm, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(22, Rv101InputsOutputs.AccessSwitch1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(23, Rv101InputsOutputs.AccessSwitch2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(24, Rv101InputsOutputs.PreparationCompleted, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(25, Rv101InputsOutputs.TemporarilyStopOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(26, Rv101InputsOutputs.SignificantError, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(27, Rv101InputsOutputs.LightError, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(28, Rv101InputsOutputs.ClampMovingDirection, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(29, Rv101InputsOutputs.ClampMotionStart, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(30, Rv101InputsOutputs.ShutterOpenOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(31, Rv101InputsOutputs.ShutterCloseOutput, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(32, Rv101InputsOutputs.ShutterMotionDisabled, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(33, Rv101InputsOutputs.ShutterOpenOutput2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(34, Rv101InputsOutputs.CoverLock, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(35, Rv101InputsOutputs.CarrierPresenceSensorOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(36, Rv101InputsOutputs.PreparationCompleted2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(37, Rv101InputsOutputs.CarrierProperPlaced, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(38, Rv101InputsOutputs.AccessSwitch1Output, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(39, Rv101InputsOutputs.AccessSwitch2Output, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(40, Rv101InputsOutputs.Load, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(41, Rv101InputsOutputs.Unload, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(42, Rv101InputsOutputs.Presence, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(43, Rv101InputsOutputs.Placement, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(44, Rv101InputsOutputs.Latch, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(45, Rv101InputsOutputs.Error, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(46, Rv101InputsOutputs.Busy, "0: False/1: True", "0");
        }

        public string GetStatus(Rv101InputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Rv101InputsOutputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                //bit 0-7
                GetStatus(Rv101InputsOutputs.EmergencyStop) == "1",
                GetStatus(Rv101InputsOutputs.TemporarilyStopInput) == "1",
                GetStatus(Rv101InputsOutputs.VacuumSourcePressure) == "1",
                GetStatus(Rv101InputsOutputs.AirSupplySourcePressure) == "1",
                false,
                false,
                false,
                GetStatus(Rv101InputsOutputs.ProtrusionDetection) == "1",

                //bit 8-15
                GetStatus(Rv101InputsOutputs.Cover) == "1",
                GetStatus(Rv101InputsOutputs.DrivePower) == "1",
                false,
                false,
                GetStatus(Rv101InputsOutputs.MappingSensor) == "1",
                false,
                false,
                false,

                //bit 16-23
                GetStatus(Rv101InputsOutputs.ShutterOpenInput) == "1",
                GetStatus(Rv101InputsOutputs.ShutterCloseInput) == "1",
                GetStatus(Rv101InputsOutputs.PresenceLeft) == "1",
                GetStatus(Rv101InputsOutputs.PresenceRight) == "1",
                GetStatus(Rv101InputsOutputs.PresenceMiddle) == "1",
                GetStatus(Rv101InputsOutputs.InfoPadA) == "1",
                GetStatus(Rv101InputsOutputs.InfoPadB) == "1",
                GetStatus(Rv101InputsOutputs.InfoPadC) == "1",

                //bit 24-31
                GetStatus(Rv101InputsOutputs.InfoPadD) == "1",
                false,
                false,
                false,
                GetStatus(Rv101InputsOutputs.PresenceLeft200Mm) == "1",
                GetStatus(Rv101InputsOutputs.PresenceRight200Mm) == "1",
                GetStatus(Rv101InputsOutputs.PresenceLeft150Mm) == "1",
                GetStatus(Rv101InputsOutputs.PresenceRight150Mm) == "1",

                //bit 32-39
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,

                //bit 40-47
                GetStatus(Rv101InputsOutputs.AccessSwitch1) == "1",
                GetStatus(Rv101InputsOutputs.AccessSwitch2) == "1",
                false,
                false,
                false,
                false,
                false,
                false,

                //bit 48-55
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,

                //bit 56-63
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false
            });

            BitArray outputs = new BitArray(new[]
            {
                //bit 0-7
                GetStatus(Rv101InputsOutputs.PreparationCompleted) == "1",
                GetStatus(Rv101InputsOutputs.TemporarilyStopOutput) == "1",
                GetStatus(Rv101InputsOutputs.SignificantError) == "1",
                GetStatus(Rv101InputsOutputs.LightError) == "1",
                false,
                false,
                false,
                false,

                //bit 8-15
                GetStatus(Rv101InputsOutputs.ClampMovingDirection) == "1",
                GetStatus(Rv101InputsOutputs.ClampMotionStart) == "1",
                false,
                false,
                false,
                false,
                false,
                false,

                //bit 16-23
                GetStatus(Rv101InputsOutputs.ShutterOpenOutput) == "1",
                GetStatus(Rv101InputsOutputs.ShutterCloseOutput) == "1",
                GetStatus(Rv101InputsOutputs.ShutterMotionDisabled) == "1",
                false,
                false,
                false,
                false,
                false,

                //bit 24-31
                GetStatus(Rv101InputsOutputs.ShutterOpenOutput2) == "1",
                GetStatus(Rv101InputsOutputs.CoverLock) == "1",
                GetStatus(Rv101InputsOutputs.CarrierPresenceSensorOn) == "1",
                GetStatus(Rv101InputsOutputs.PreparationCompleted2) == "1",
                GetStatus(Rv101InputsOutputs.CarrierProperPlaced) == "1",
                false,
                false,
                false,

                //bit 32-39
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,

                //bit 40-47
                GetStatus(Rv101InputsOutputs.AccessSwitch1Output) == "1",
                GetStatus(Rv101InputsOutputs.AccessSwitch2Output) == "1",
                GetStatus(Rv101InputsOutputs.Load) == "1",
                GetStatus(Rv101InputsOutputs.Unload) == "1",
                GetStatus(Rv101InputsOutputs.Presence) == "1",
                GetStatus(Rv101InputsOutputs.Placement) == "1",
                GetStatus(Rv101InputsOutputs.Latch) == "1",
                GetStatus(Rv101InputsOutputs.Error) == "1",

                //bit 48-55
                false,
                false,
                GetStatus(Rv101InputsOutputs.Busy) == "1",
                false,
                false,
                false,
                false,
                false,

                //bit 56-63
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false
            });

            stringBuilder.Append(BitArrayToHexaString(inputs));
            stringBuilder.Append("/");
            stringBuilder.Append(BitArrayToHexaString(outputs));

            return stringBuilder.ToString();
        }
    }
}
