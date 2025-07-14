using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv
{
    public class TsvDetailMeasureInfoVM : MetroDetailMeasureInfoVM<TSVPointResult>
    {
        private TSVResultSettings _settings;

        public TSVResultSettings Settings
        {
            get { return _settings; }
            set { SetProperty(ref _settings, value); }
        }

        private string _deltaDepth;

        public string DeltaDepth
        {
            get => _deltaDepth;
            set => SetProperty(ref _deltaDepth, value);
        }

        private string _deltaWidth;

        public string DeltaWidth
        {
            get { return _deltaWidth; }
            set
            {
                SetProperty(ref _deltaWidth, value);
                OnPropertyChanged(nameof(IsRectangularShaped));
                OnPropertyChanged(nameof(IsCircleShaped));
                OnPropertyChanged(nameof(LengthLabel));
            }
        }

        private string _deltaLength;

        public string DeltaLength
        {
            get { return _deltaLength; }
            set { SetProperty(ref _deltaLength, value); }
        }

        private Length _copla;

        public Length Copla
        {
            get { return _copla; }
            set { SetProperty(ref _copla, value); }
        }

        public bool IsRectangularShaped => Settings?.Shape == TSVShape.Rectangle;
        public bool IsCircleShaped => Settings?.Shape == TSVShape.Circle;
        public string LengthLabel => IsCircleShaped ? "Diameter" : "Length";

        protected override void OnPointChanged()
        {
            base.OnPointChanged();

            if (Point != null && Settings != null)
            {
                Copla = Point.CoplaInWaferValue == null ? Point.CoplaInDieValue : Point.CoplaInWaferValue;

                if (!(Point.DepthTsvStat is null) && Point.DepthTsvStat.State != MeasureState.NotMeasured)
                {
                    string convertDelta = Point.DepthTsvStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.DepthTsvStat.Mean - Settings.DepthTarget, Digits, true, "-", LengthUnit.Micrometer);
                    DeltaDepth = $"{DifferenceWithTargetSymbole} = {convertDelta}";
                }
                else
                {
                    DeltaDepth = "Not measured";
                }

                if (!(Point.WidthTsvStat is null) && Point.WidthTsvStat.State != MeasureState.NotMeasured)
                {
                    string convertDelta = Point.WidthTsvStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.WidthTsvStat.Mean - Settings.WidthTarget, Digits, true, "-", LengthUnit.Micrometer);
                    DeltaWidth = $"{DifferenceWithTargetSymbole} = {convertDelta}";
                }
                else
                {
                    DeltaWidth = "Not measured";
                }

                if (!(Point.LengthTsvStat is null) && Point.LengthTsvStat.State != MeasureState.NotMeasured)
                {
                    string convertDelta = Point.LengthTsvStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.LengthTsvStat.Mean - Settings.LengthTarget, Digits, true, "-", LengthUnit.Micrometer);
                    DeltaLength = $"{DifferenceWithTargetSymbole} = {convertDelta}";
                }
                else
                {
                    DeltaLength = "Not measured";
                }
            }
            else
            {
                DeltaDepth = string.Empty;
                DeltaWidth = string.Empty;
                DeltaLength = string.Empty;
                Copla = null;
            }
        }
    }
}
