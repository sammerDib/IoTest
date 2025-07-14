using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Step
{
    public class StepDetailMeasureInfoVM : MetroDetailMeasureInfoVM<StepPointResult>
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

        private StepResultSettings _type;

        public StepResultSettings Settings
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

            if (Point != null && Settings != null)
            {
                if (Output == StepResultVM.StepHeightOutputName)
                {
                    Value = LengthToStringConverter.ConvertToString(Point.StepHeightStat.Mean, Digits, true, "-", LengthUnit.Micrometer);
                    State = Point.StepHeightStat.State;
                    if (State != MeasureState.NotMeasured)
                    {
                        string convertDelta = Point.StepHeightStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.StepHeightStat.Mean - Settings.StepHeightTarget, Digits, true, "-", LengthUnit.Micrometer);
                        Delta = $"{convertDelta}";
                    }
                    else
                    {
                        Delta = "Not measured";
                    }

                    Target = LengthToStringConverter.ConvertToString(Settings.StepHeightTarget, Digits, true, "-", LengthUnit.Micrometer);
                    Tolerance = LengthToleranceToStringConverter.ConvertToString(Settings.StepHeightTolerance, Digits, true, "-", LengthToleranceUnit.Micrometer);
                }
                else
                {
                    Delta = "No target defined for this output.";
                    Target = "-";
                    Tolerance = "-";
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
