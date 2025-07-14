using System.Collections;
using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.Dio0
{
    public enum Dio0FanDetection
    {
        FanDetection1,
        FanDetection2,
        DriveAlarm
    }

    public partial class Dio0FanDetectionControl : InputsOutputsControl
    {
        public Dio0FanDetectionControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio0FanDetection.FanDetection1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Dio0FanDetection.FanDetection2, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Dio0FanDetection.DriveAlarm, "0: False/1: True", "0");
        }

        public string GetStatus(Dio0FanDetection status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio0FanDetection status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                GetStatus(Dio0FanDetection.FanDetection1) == "1",
                GetStatus(Dio0FanDetection.FanDetection2) == "1",
                GetStatus(Dio0FanDetection.DriveAlarm) == "1",
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
                false
            });

            BitArray outputs = new BitArray(new[]
            {
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
                false
            });

            stringBuilder.Append(BitArrayToHexaString(inputs));
            stringBuilder.Append(BitArrayToHexaString(outputs));

            return stringBuilder.ToString();
        }
    }
}
