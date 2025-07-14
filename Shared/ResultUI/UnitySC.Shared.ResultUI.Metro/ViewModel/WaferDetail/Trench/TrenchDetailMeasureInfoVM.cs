using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench
{
    public class TrenchDetailMeasureInfoVM : MetroDetailMeasureInfoVM<TrenchPointResult>
    {
        #region Properties

        #region Depth 
        private string _depthvalue;

        public string DepthValue
        {
            get { return _depthvalue; }
            private set { SetProperty(ref _depthvalue, value); }
        }

        private MeasureState _depthstate;

        public MeasureState DepthState
        {
            get { return _depthstate; }
            private set { SetProperty(ref _depthstate, value); }
        }

        private string _depthdelta;

        public string DepthDelta
        {
            get { return _depthdelta; }
            private set { SetProperty(ref _depthdelta, value); }
        }

        private string _depthtarget;

        public string DepthTarget
        {
            get { return _depthtarget; }
            private set { SetProperty(ref _depthtarget, value); }
        }

        private string _depthtolerance;

        public string DepthTolerance
        {
            get { return _depthtolerance; }
            private set { SetProperty(ref _depthtolerance, value); }
        }
        #endregion  //  Depth 
 
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
        #endregion //Trench Width

        private TrenchResultSettings _settings;

        public TrenchResultSettings Settings
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
                if (Settings.DepthTarget != null)
                {
                    DepthTarget = LengthToStringConverter.ConvertToString(Settings.DepthTarget, Digits, true, "-", LengthUnit.Micrometer);
                    DepthTolerance = LengthToleranceToStringConverter.ConvertToString(Settings.DepthTolerance, Digits, true, "-", LengthToleranceUnit.Micrometer);

                    if (Point.DepthStat != null)
                    {
                        DepthState = Point.DepthStat.State;
                        DepthValue = LengthToStringConverter.ConvertToString(Point.DepthStat.Mean, Digits, true, "-", LengthUnit.Micrometer);
                        string convertDelta = Point.DepthStat.Mean == null ? "-" : LengthToStringConverter.ConvertToString(Point.DepthStat.Mean - Settings.DepthTarget, Digits, true, "-", LengthUnit.Micrometer);
                        DepthDelta = $"{convertDelta}";
                    }
                    else
                    {
                        DepthValue = string.Empty;
                        DepthDelta = string.Empty;
                        DepthState = MeasureState.NotMeasured;
                    }
                }
                else
                {
                    DepthTarget = string.Empty;
                    DepthTolerance = string.Empty;

                    DepthValue = string.Empty;
                    DepthDelta = string.Empty;
                    DepthState = MeasureState.NotMeasured;
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
                DepthValue = string.Empty;
                DepthState = MeasureState.NotMeasured;
                DepthDelta = string.Empty;
                DepthTarget = string.Empty;
                DepthTolerance = string.Empty;

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
