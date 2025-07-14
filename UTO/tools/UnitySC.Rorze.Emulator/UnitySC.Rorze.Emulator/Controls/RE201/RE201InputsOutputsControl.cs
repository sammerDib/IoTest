using System.Collections;
using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RE201
{
    internal enum Re201InputsOutputs
    {
        SubstrateDetection,
        MotionProhibited,
        ClampRightClose,
        ClampLeftClose,
        ClampRightOpen,
        ClampLeftOpen,
        CarrierPresenceMiddle,
        CarrierPresenceLeft,
        CarrierPresenceRight,
        AccessSwitch,
        ProtrusionDetection,
        InfoPadA,
        InfoPadB,
        InfoPadC,
        InfoPadD,
        PositionForReadingId,
        PreparationCompletedSignalNotConnected,
        TemporarilyStopSignalNotConnected,
        SignificantErrorSignalNotConnected,
        LightErrorSignalNotConnected,
        LaserStop,
        InterlockCancel,
        CarrierClampCloseRight,
        CarrierClampOpenRight,
        CarrierClampCloseLeft,
        CarrierClampOpenLeft,
        GreenIndicator,
        RedIndicator,
        LoadIndicator,
        UnloadIndicator,
        AccessSwitchIndicator,
        CarrierOpen,
        CarrierClamp,
        PodPresenceSensorOn,
        PreparationCompleted,
        CarrierProperPlaced
    }
    internal partial class Re201InputsOutputsControl : InputsOutputsControl
    {
        public Re201InputsOutputsControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Re201InputsOutputs.SubstrateDetection, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Re201InputsOutputs.MotionProhibited, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Re201InputsOutputs.ClampRightClose, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Re201InputsOutputs.ClampLeftClose, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Re201InputsOutputs.ClampRightOpen, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Re201InputsOutputs.ClampLeftOpen, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Re201InputsOutputs.CarrierPresenceMiddle, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Re201InputsOutputs.CarrierPresenceLeft, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Re201InputsOutputs.CarrierPresenceRight, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Re201InputsOutputs.AccessSwitch, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(11, Re201InputsOutputs.ProtrusionDetection, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(12, Re201InputsOutputs.InfoPadA, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(13, Re201InputsOutputs.InfoPadB, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(14, Re201InputsOutputs.InfoPadC, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(15, Re201InputsOutputs.InfoPadD, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(16, Re201InputsOutputs.PositionForReadingId, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(17, Re201InputsOutputs.PreparationCompletedSignalNotConnected, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(18, Re201InputsOutputs.TemporarilyStopSignalNotConnected, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(19, Re201InputsOutputs.SignificantErrorSignalNotConnected, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(20, Re201InputsOutputs.LightErrorSignalNotConnected, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(21, Re201InputsOutputs.LaserStop, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(22, Re201InputsOutputs.InterlockCancel, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(23, Re201InputsOutputs.CarrierClampCloseRight, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(24, Re201InputsOutputs.CarrierClampOpenRight, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(25, Re201InputsOutputs.CarrierClampCloseLeft, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(26, Re201InputsOutputs.CarrierClampOpenLeft, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(27, Re201InputsOutputs.GreenIndicator, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(28, Re201InputsOutputs.RedIndicator, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(29, Re201InputsOutputs.LoadIndicator, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(30, Re201InputsOutputs.UnloadIndicator, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(31, Re201InputsOutputs.AccessSwitchIndicator, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(32, Re201InputsOutputs.CarrierOpen, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(33, Re201InputsOutputs.CarrierClamp, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(34, Re201InputsOutputs.PodPresenceSensorOn, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(35, Re201InputsOutputs.PreparationCompleted, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(36, Re201InputsOutputs.CarrierProperPlaced, "0: False/1: True", "0");
        }

        public string GetStatus(Re201InputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Re201InputsOutputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                false,
                false,
                false,
                false,
                false,
                false,
                GetStatus(Re201InputsOutputs.SubstrateDetection) == "1",
                GetStatus(Re201InputsOutputs.MotionProhibited) == "1",
                GetStatus(Re201InputsOutputs.ClampRightClose) == "1",
                GetStatus(Re201InputsOutputs.ClampLeftClose) == "1",
                GetStatus(Re201InputsOutputs.ClampRightOpen) == "1",
                GetStatus(Re201InputsOutputs.ClampLeftOpen) == "1",
                false,
                false,
                false,
                false,
                false,
                GetStatus(Re201InputsOutputs.CarrierPresenceMiddle) == "1",
                GetStatus(Re201InputsOutputs.CarrierPresenceLeft) == "1",
                GetStatus(Re201InputsOutputs.CarrierPresenceRight) == "1",
                GetStatus(Re201InputsOutputs.AccessSwitch) == "1",
                GetStatus(Re201InputsOutputs.ProtrusionDetection) == "1",
                false,
                false,
                GetStatus(Re201InputsOutputs.InfoPadA) == "1",
                GetStatus(Re201InputsOutputs.InfoPadB) == "1",
                GetStatus(Re201InputsOutputs.InfoPadC) == "1",
                GetStatus(Re201InputsOutputs.InfoPadD) == "1",
                GetStatus(Re201InputsOutputs.PositionForReadingId) == "1",
                false,
                false,
                false
            });

            BitArray outputs = new BitArray(new[]
            {
                GetStatus(Re201InputsOutputs.PreparationCompletedSignalNotConnected) == "1",
                GetStatus(Re201InputsOutputs.TemporarilyStopSignalNotConnected) == "1",
                GetStatus(Re201InputsOutputs.SignificantErrorSignalNotConnected) == "1",
                GetStatus(Re201InputsOutputs.LightErrorSignalNotConnected) == "1",
                false,
                false,
                GetStatus(Re201InputsOutputs.LaserStop) == "1",
                GetStatus(Re201InputsOutputs.InterlockCancel) == "1",
                GetStatus(Re201InputsOutputs.CarrierClampCloseRight) == "1",
                GetStatus(Re201InputsOutputs.CarrierClampOpenRight) == "1",
                GetStatus(Re201InputsOutputs.CarrierClampCloseLeft) == "1",
                GetStatus(Re201InputsOutputs.CarrierClampOpenLeft) == "1",
                false,
                false,
                false,
                false,
                GetStatus(Re201InputsOutputs.GreenIndicator) == "1",
                GetStatus(Re201InputsOutputs.RedIndicator) == "1",
                GetStatus(Re201InputsOutputs.LoadIndicator) == "1",
                GetStatus(Re201InputsOutputs.UnloadIndicator) == "1",
                GetStatus(Re201InputsOutputs.AccessSwitchIndicator) == "1",
                false,
                false,
                false,
                GetStatus(Re201InputsOutputs.CarrierOpen) == "1",
                GetStatus(Re201InputsOutputs.CarrierClamp) == "1",
                GetStatus(Re201InputsOutputs.PodPresenceSensorOn) == "1",
                GetStatus(Re201InputsOutputs.PreparationCompleted) == "1",
                GetStatus(Re201InputsOutputs.CarrierProperPlaced) == "1",
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
