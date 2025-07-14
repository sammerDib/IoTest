using System.Linq;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Common.Converters;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography
{
    public class TopographyDetailMeasureInfoVM : MetroDetailMeasureInfoVM<TopographyPointResult>
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

        private TopographyResultSettings _type;

        public TopographyResultSettings Settings
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

            if (Point != null && Settings != null && Output != null)
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
                            string convertDelta =
                                LengthToStringConverter.ConvertToString(statsContainer.Mean - output.OutputTarget,
                                    Digits);
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
