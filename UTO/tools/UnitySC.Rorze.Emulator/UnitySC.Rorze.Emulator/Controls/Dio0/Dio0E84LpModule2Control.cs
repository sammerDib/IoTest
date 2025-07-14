using System.Collections;
using System.Text;

using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls.Dio0
{
    public enum Dio0E84LpModule2InputsOuputs
    {
        HoAvbl,
        Es
    }

    public partial class Dio0E84LpModule2Control : InputsOutputsControl
    {
        public Dio0E84LpModule2Control()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio0E84LpModule2InputsOuputs.HoAvbl, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Dio0E84LpModule2InputsOuputs.Es, "0: False/1: True", "0");
        }

        public string GetStatus(Dio0E84LpModule2InputsOuputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio0E84LpModule2InputsOuputs status, string value)
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
                GetStatus(Dio0E84LpModule2InputsOuputs.HoAvbl) == "1",
                GetStatus(Dio0E84LpModule2InputsOuputs.Es) == "1",
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



        public void SetOutputsFromConcatenatedString(string concatenatedStatus)
        {
            //No mask defined
            if (concatenatedStatus.Length == 4)
            {
                BitArray outputs = new BitArray(RorzeHelpers.StringToByteArray(concatenatedStatus));
                SetStatus(
                    Dio0E84LpModule2InputsOuputs.HoAvbl,
                    outputs[2]
                        ? "1"
                        : "0");

                SetStatus(
                    Dio0E84LpModule2InputsOuputs.Es,
                    outputs[3]
                        ? "1"
                        : "0");
            }
            else
            {
                var status = concatenatedStatus.Split('/');
                var value = new BitArray(RorzeHelpers.StringToByteArray(status[0]));
                var mask = new BitArray(RorzeHelpers.StringToByteArray(status[0]));

                for (int iMask = 0; iMask < mask.Length; iMask++)
                {
                    if (!mask[iMask])
                    {
                        continue;
                    }

                    switch (iMask)
                    {
                        case 2:
                            SetStatus(
                                Dio0E84LpModule2InputsOuputs.HoAvbl,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;

                        case 3:
                            SetStatus(
                                Dio0E84LpModule2InputsOuputs.Es,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;
                    }
                }
            }
        }
    }
}
