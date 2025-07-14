using System.Collections;
using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RA420
{
    public enum Ra420InputsOutputs
    {
        ExhaustFanRotating,
        SubstrateDetectionSensor1,
        SubstrateDetectionSensor2,
        AlignerReadyToOperate,
        TemporarilyStop,
        SignificantError,
        LightError,
        SubstrateDetection,
        AlignmentComplete,
        SpindleSolenoidValveChuckingOff,
        SpindleSolenoidValveChuckingOn,
    }

    public partial class RA420InputsOutputsControl : InputsOutputsControl
    {
        public RA420InputsOutputsControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Ra420InputsOutputs.ExhaustFanRotating, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Ra420InputsOutputs.SubstrateDetectionSensor1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Ra420InputsOutputs.SubstrateDetectionSensor2, "0: False/1: True", "0");

            paramDataGridView.Rows.Add(4, Ra420InputsOutputs.AlignerReadyToOperate, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Ra420InputsOutputs.TemporarilyStop, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Ra420InputsOutputs.SignificantError, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Ra420InputsOutputs.LightError, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Ra420InputsOutputs.SubstrateDetection, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Ra420InputsOutputs.AlignmentComplete, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Ra420InputsOutputs.SpindleSolenoidValveChuckingOff, "0: False/1: True",
                "0");
            paramDataGridView.Rows.Add(11, Ra420InputsOutputs.SpindleSolenoidValveChuckingOn, "0: False/1: True",
                "0");
        }

        public string GetStatus(Ra420InputsOutputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Ra420InputsOutputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                false,  //Bit0
                false,  //Bit1
                false,  //Bit2
                false,  //Bit3
                GetStatus(Ra420InputsOutputs.ExhaustFanRotating) == "1",  //Bit4
                false,  //Bit5
                false,  //Bit6
                false,  //Bit7
                GetStatus(Ra420InputsOutputs.SubstrateDetectionSensor1) == "1",  //Bit8
                GetStatus(Ra420InputsOutputs.SubstrateDetectionSensor2) == "1",  //Bit9
                false,  //Bit10
                false,  //Bit11
                false,  //Bit12
                false,  //Bit13
                false,  //Bit14
                false,  //Bit15
                false,  //Bit16
                false,  //Bit17
                false,  //Bit18
                false,  //Bit19
                false,  //Bit20
                false,  //Bit21
                false,  //Bit22
                false,  //Bit23
                false,  //Bit24
                false,  //Bit25
                false,  //Bit26
                false,  //Bit27
                false,  //Bit28
                false,  //Bit29
                false,  //Bit30
                false,  //Bit31
            });

            BitArray outputs = new BitArray(new[]
            {
                GetStatus(Ra420InputsOutputs.AlignerReadyToOperate) == "1",
                GetStatus(Ra420InputsOutputs.TemporarilyStop) == "1",
                GetStatus(Ra420InputsOutputs.SignificantError) == "1",
                GetStatus(Ra420InputsOutputs.LightError) == "1",
                false,
                false,
                GetStatus(Ra420InputsOutputs.SubstrateDetection) == "1",
                GetStatus(Ra420InputsOutputs.AlignmentComplete) == "1",
                GetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOff) == "1",
                GetStatus(Ra420InputsOutputs.SpindleSolenoidValveChuckingOn) == "1",
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
            });

            stringBuilder.Append(BitArrayToHexaString(inputs));
            stringBuilder.Append("/");
            stringBuilder.Append(BitArrayToHexaString(outputs));

            return stringBuilder.ToString();
        }
    }
}
