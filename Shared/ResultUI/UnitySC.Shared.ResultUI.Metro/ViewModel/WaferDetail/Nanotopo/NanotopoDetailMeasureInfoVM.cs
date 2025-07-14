using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo
{
    public class NanotopoDetailMeasureInfoVM : MetroDetailMeasureInfoVM<NanoTopoPointResult>
    {
        #region Properties
        
        private string _value;

        public string Value
        {
            get { return _value; }
            private set { SetProperty(ref _value, value); }
        }

        private MeasureState _state;

        public MeasureState State
        {
            get { return _state; }
            private set { SetProperty(ref _state, value); }
        }

        private string _delta;

        public string Delta
        {
            get { return _delta; }
            private set { SetProperty(ref _delta, value); }
        }

        private string _target;

        public string Target
        {
            get { return _target; }
            private set { SetProperty(ref _target, value); }
        }

        private string _tolerance;

        public string Tolerance
        {
            get { return _tolerance; }
            private set { SetProperty(ref _tolerance, value); }
        }

        private string _output;

        public string Output
        {
            get { return _output; }
            set
            {
                if (SetProperty(ref _output, value))
                {
                    OnPointChanged();
                }
            }
        }

        private NanoTopoResultSettings _type;

        public NanoTopoResultSettings Settings
        {
            get { return _type; }
            set
            {
                if (SetProperty(ref _type, value))
                {
                    OnPointChanged();
                }
            }
        }

        #endregion

        #region Overrides of MetroDetailMeasureInfoVM

        protected override void OnPointChanged()
        {
            base.OnPointChanged();

            if (Point != null && Settings != null && Output!=null)
            {
                switch (Output)
                {
                    case NanotopoResultVM.RoughnessOutputName:
                        {
                            Value = LengthToStringConverter.ConvertToString(Point.RoughnessStat.Mean, Digits, true, "-", LengthUnit.Nanometer);
                            State = Point.RoughnessStat.State;
                            if (State != MeasureState.NotMeasured)
                            {
                                string convertDelta = Point.RoughnessStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.RoughnessStat.Mean - Settings.RoughnessTarget, Digits, true, "-", LengthUnit.Nanometer);
                                Delta = $"{DifferenceWithTargetSymbole} = {convertDelta}";
                            }
                            else
                            {
                                Delta = "Not measured";
                            }

                            Target = LengthToStringConverter.ConvertToString(Settings.RoughnessTarget, Digits, true, "-", LengthUnit.Nanometer);
                            Tolerance = LengthToleranceToStringConverter.ConvertToString(Settings.RoughnessTolerance, Digits, true, "-", LengthToleranceUnit.Nanometer);
                            break;
                        }
                    case NanotopoResultVM.StepHeightOutputName:
                        {
                            Value = LengthToStringConverter.ConvertToString(Point.StepHeightStat.Mean, Digits, true, "-", LengthUnit.Nanometer);
                            State = Point.StepHeightStat.State;
                            if (State != MeasureState.NotMeasured)
                            {
                                string convertDelta = Point.StepHeightStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.StepHeightStat.Mean - Settings.StepHeightTarget, Digits, true, "-", LengthUnit.Nanometer);
                                Delta = $"{DifferenceWithTargetSymbole} = {convertDelta}";
                            }
                            else
                            {
                                Delta = "Not measured";
                            }

                            Target = LengthToStringConverter.ConvertToString(Settings.StepHeightTarget, Digits, true, "-", LengthUnit.Nanometer);
                            Tolerance = LengthToleranceToStringConverter.ConvertToString(Settings.StepHeightTolerance, Digits, true, "-", LengthToleranceUnit.Nanometer);
                            break;
                        }
                    default:
                        {
                            var output = Settings.ExternalProcessingOutputs.SingleOrDefault(epo => epo.Name.Equals(Output));
                            if (output != null)
                            {
                                Target = LengthToStringConverter.ConvertToString(output.OutputTarget, Digits);
                                Tolerance = ToleranceToStringConverter.ConvertToString(output.OutputTolerance, Digits, true, "-");
                            }
                            else
                            {
                                Delta = "No target defined for this output.";
                                Target = "-";
                                Tolerance = "-";
                            }

                            if (Point.ExternalProcessingStats.TryGetValue(Output, out var statsContainer))
                            {
                                Value = LengthToStringConverter.ConvertToString(statsContainer.Mean, Digits);
                                State = statsContainer.State;

                                if (output != null)
                                {
                                    if (State != MeasureState.NotMeasured)
                                    {
                                        string convertDelta = LengthToStringConverter.ConvertToString(statsContainer.Mean - output.OutputTarget, Digits);
                                        Delta = $"{DifferenceWithTargetSymbole} = {convertDelta}";
                                    }
                                    else
                                    {
                                        Delta = "Not measured";
                                    }
                                }
                            }
                            else
                            {
                                Value = "-";
                                State = MeasureState.NotMeasured;
                                Delta = "-";
                            }

                            break;
                        }
                }
            }
            else
            {
                Value = string.Empty;
                State = MeasureState.NotMeasured;
                Delta = string.Empty;
                Target = string.Empty;
                Tolerance = string.Empty;
            }
        }

        #endregion
    }
}
