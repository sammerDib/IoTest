using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar
{
    public class PillarDetailMeasureInfoVM : MetroDetailMeasureInfoVM<PillarPointResult>
    {
        #region Properties

        #region Height 
        private string _heightvalue;

        public string HeightValue
        {
            get { return _heightvalue; }
            private set { SetProperty(ref _heightvalue, value); }
        }

        private MeasureState _heightstate;

        public MeasureState HeightState
        {
            get { return _heightstate; }
            private set { SetProperty(ref _heightstate, value); }
        }

        private string _heightdelta;

        public string HeightDelta
        {
            get { return _heightdelta; }
            private set { SetProperty(ref _heightdelta, value); }
        }

        private string _heighttarget;

        public string HeightTarget
        {
            get { return _heighttarget; }
            private set { SetProperty(ref _heighttarget, value); }
        }

        private string _heighttolerance;

        public string HeightTolerance
        {
            get { return _heighttolerance; }
            private set { SetProperty(ref _heighttolerance, value); }
        }
        #endregion  //  Height 
 
        #region  Width 
        private string _widthvalue;

        public string WidthValue
        {
            get { return _widthvalue; }
            private set { SetProperty(ref _widthvalue, value); }
        }

        private MeasureState _widthstate;

        public MeasureState WidthState
        {
            get { return _widthstate; }
            private set { SetProperty(ref _widthstate, value); }
        }

        private string _widthdelta;

        public string WidthDelta
        {
            get { return _widthdelta; }
            private set { SetProperty(ref _widthdelta, value); }
        }

        private string _widthtarget;

        public string WidthTarget
        {
            get { return _widthtarget; }
            private set { SetProperty(ref _widthtarget, value); }
        }

        private string _widthtolerance;

        public string WidthTolerance
        {
            get { return _widthtolerance; }
            private set { SetProperty(ref _widthtolerance, value); }
        }
        #endregion //Pillar Width

        private PillarResultSettings _settings;

        public PillarResultSettings Settings
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

        #endregion

        #region Overrides of MetroDetailMeasureInfoVM

        protected override void OnPointChanged()
        {
            base.OnPointChanged();

            if (Point != null && Settings != null)
            {
                if (Settings.HeightTarget != null)
                {
                    HeightTarget = LengthToStringConverter.ConvertToString(Settings.HeightTarget, Digits, true, "-", LengthUnit.Micrometer);
                    HeightTolerance = LengthToleranceToStringConverter.ConvertToString(Settings.HeightTolerance, Digits, true, "-", LengthToleranceUnit.Micrometer);

                    if (Point.HeightStat != null)
                    {
                        HeightState = Point.HeightStat.State;
                        HeightValue = LengthToStringConverter.ConvertToString(Point.HeightStat.Mean, Digits, true, "-", LengthUnit.Micrometer);
                        string convertDelta = Point.HeightStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.HeightStat.Mean - Settings.HeightTarget, Digits, true, "-", LengthUnit.Micrometer);
                        HeightDelta = $"{convertDelta}";
                    }
                    else
                    {
                        HeightValue = string.Empty;
                        HeightDelta = string.Empty;
                        HeightState = MeasureState.NotMeasured;
                    }
                }
                else
                {
                    HeightTarget = string.Empty;
                    HeightTolerance = string.Empty;

                    HeightValue = string.Empty;
                    HeightDelta = string.Empty;
                    HeightState = MeasureState.NotMeasured;
                }

                if (Settings.WidthTarget != null)
                {
                    WidthTarget = LengthToStringConverter.ConvertToString(Settings.WidthTarget, Digits, true, "-", LengthUnit.Micrometer);
                    WidthTolerance = LengthToleranceToStringConverter.ConvertToString(Settings.WidthTolerance, Digits, true, "-", LengthToleranceUnit.Micrometer);

                    if (Point.WidthStat != null)
                    {
                        WidthState = Point.WidthStat.State;
                        WidthValue = LengthToStringConverter.ConvertToString(Point.WidthStat.Mean, Digits, true, "-", LengthUnit.Micrometer);
                        string convertDelta = Point.WidthStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.WidthStat.Mean - Settings.WidthTarget, Digits, true, "-", LengthUnit.Micrometer);
                        WidthDelta = $"{convertDelta}";
                    }
                    else
                    {
                        WidthValue = string.Empty;
                        WidthDelta = string.Empty;
                        WidthState = MeasureState.NotMeasured;
                    }
                }
                else
                {
                    WidthTarget = string.Empty;
                    WidthTolerance = string.Empty;

                    WidthValue = string.Empty;
                    WidthDelta = string.Empty;
                    WidthState = MeasureState.NotMeasured;
                }

            }
            else
            {
                HeightValue = string.Empty;
                HeightState = MeasureState.NotMeasured;
                HeightDelta = string.Empty;
                HeightTarget = string.Empty;
                HeightTolerance = string.Empty;

                WidthValue = string.Empty;
                WidthState = MeasureState.NotMeasured;
                WidthDelta = string.Empty;
                WidthTarget = string.Empty;
                WidthTolerance = string.Empty;

            }
        }

        #endregion
    }
}
