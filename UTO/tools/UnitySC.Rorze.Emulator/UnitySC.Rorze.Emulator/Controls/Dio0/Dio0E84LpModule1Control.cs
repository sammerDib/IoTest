using System;
using System.Collections;
using System.Linq;
using System.Text;

using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls.Dio0
{
    public enum Dio0E84LpModule1InputsOuputs
    {
        Valid,
        Cs0,
        Cs1,
        TrReq,
        Busy,
        Compt,
        Cont,
        LReq,
        UReq,
        Ready
    }

    public partial class Dio0E84LpModule1Control : InputsOutputsControl
    {
        public Dio0E84LpModule1Control()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio0E84LpModule1InputsOuputs.Valid, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(2, Dio0E84LpModule1InputsOuputs.Cs0, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(3, Dio0E84LpModule1InputsOuputs.Cs1, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(4, Dio0E84LpModule1InputsOuputs.TrReq, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(5, Dio0E84LpModule1InputsOuputs.Busy, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(6, Dio0E84LpModule1InputsOuputs.Compt, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(7, Dio0E84LpModule1InputsOuputs.Cont, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(8, Dio0E84LpModule1InputsOuputs.LReq, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(9, Dio0E84LpModule1InputsOuputs.UReq, "0: False/1: True", "0");
            paramDataGridView.Rows.Add(10, Dio0E84LpModule1InputsOuputs.Ready, "0: False/1: True", "0");
        }

        public string GetStatus(Dio0E84LpModule1InputsOuputs status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio0E84LpModule1InputsOuputs status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            BitArray inputs = new BitArray(new[]
            {
                GetStatus(Dio0E84LpModule1InputsOuputs.Valid) == "1",
                GetStatus(Dio0E84LpModule1InputsOuputs.Cs0) == "1",
                GetStatus(Dio0E84LpModule1InputsOuputs.Cs1) == "1",
                false,
                GetStatus(Dio0E84LpModule1InputsOuputs.TrReq) == "1",
                GetStatus(Dio0E84LpModule1InputsOuputs.Busy) == "1",
                GetStatus(Dio0E84LpModule1InputsOuputs.Compt) == "1",
                GetStatus(Dio0E84LpModule1InputsOuputs.Cont) == "1",
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
                GetStatus(Dio0E84LpModule1InputsOuputs.LReq) == "1",
                GetStatus(Dio0E84LpModule1InputsOuputs.UReq) == "1",
                false,
                GetStatus(Dio0E84LpModule1InputsOuputs.Ready) == "1",
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
                    Dio0E84LpModule1InputsOuputs.LReq,
                    outputs[0]
                        ? "1"
                        : "0");

                SetStatus(
                    Dio0E84LpModule1InputsOuputs.UReq,
                    outputs[1]
                        ? "1"
                        : "0");

                SetStatus(
                    Dio0E84LpModule1InputsOuputs.Ready,
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
                        case 0:
                            SetStatus(
                                Dio0E84LpModule1InputsOuputs.LReq,
                                value[iMask]
                                    ? "1"
                                    : "0");

                            break;

                        case 1:
                            SetStatus(
                                Dio0E84LpModule1InputsOuputs.UReq,
                                value[iMask]
                                    ? "1"
                                    : "0");
                            break;

                        case 3:
                            SetStatus(
                                Dio0E84LpModule1InputsOuputs.Ready,
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
