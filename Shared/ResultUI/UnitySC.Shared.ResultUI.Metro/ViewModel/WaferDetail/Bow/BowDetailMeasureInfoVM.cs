using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow
{
    public class BowDetailMeasureInfoVM : MetroDetailMeasureInfoVM<BowPointResult>
    {
        #region Properties

        private string _bowvalue;

        public string BowValue
        {
            get
            {
                return _bowvalue;
            }
            private set { SetProperty(ref _bowvalue, value); }
        }

        private MeasureState _bowstate;

        public MeasureState BowState
        {
            get { return _bowstate; }
            private set { SetProperty(ref _bowstate, value); }
        }

        private string _bowTargetMin;

        public string BowTargetMin
        {
            get { return _bowTargetMin; }
            private set { SetProperty(ref _bowTargetMin, value); }
        }

        private string _bowTargetMax;

        public string BowTargetMax
        {
            get { return _bowTargetMax; }
            private set { SetProperty(ref _bowTargetMax, value); }
        }

        private BowResultSettings _settings;

        public BowResultSettings Settings
        {
            get { return _settings; }
            set
            {
                if (SetProperty(ref _settings, value))
                {
                    OnPointChanged();
                }
            }
        }

        #endregion Properties

        #region Overrides of MetroDetailMeasureInfoVM

        protected override void OnPointChanged()
        {
            base.OnPointChanged();

            if (Point != null && Settings != null && Settings.BowTargetMin != null && Settings.BowTargetMax != null)
            {
                BowTargetMin = LengthToStringConverter.ConvertToString(Settings.BowTargetMin, Digits, true, "-", LengthUnit.Micrometer);
                BowTargetMax = LengthToStringConverter.ConvertToString(Settings.BowTargetMax, Digits, true, "-", LengthUnit.Micrometer);

                if (Point.BowStat != null)
                {
                    BowState = Point.BowStat.State;
                    BowValue = LengthToStringConverter.ConvertToString(Point.BowStat.Mean, Digits, true, "-", LengthUnit.Micrometer);
                }
                else
                {
                    BowValue = string.Empty;
                    BowState = MeasureState.NotMeasured;
                }
            }
            else
            {
                BowTargetMin = string.Empty;
                BowTargetMax = string.Empty;
                BowValue = string.Empty;
                BowState = MeasureState.NotMeasured;
            }
        }

        #endregion Overrides of MetroDetailMeasureInfoVM
    }
}
