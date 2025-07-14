using System;
using System.ComponentModel.Design;
using System.Drawing;

using LightningChartLib.WinForms.Charting;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp
{
    public enum WarpType
    {
        MEDIAN,
        SURFACE
    }

    public class WarpDetailMeasureInfoVM : MetroDetailMeasureInfoVM<WarpPointResult>
    {
        #region Global properties WARP/TTV

        private string _globalOutput;
        public string GlobalOutput
        {
            get => _globalOutput;
            set => SetProperty(ref _globalOutput, value);
        }

        public double QualityScore { get; set; }

        private MeasureState _globlaState;
        public MeasureState GlobalState
        {
            get => _globlaState;
            set => SetProperty(ref _globlaState, value);
        }

        private Length _warpResultLength;
        public Length WarpResultLength
        {
            get { return _warpResultLength; }
            set { SetProperty(ref _warpResultLength, value); }
        }

        private string _ttvResult;
        public string TTVResult
        {
            get { return _ttvResult; }
            set { SetProperty(ref _ttvResult, value); }
        }

        private WarpResultSettings _settings;
        public WarpResultSettings Settings
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

        private MetroStatsContainer _globalStats;
        public MetroStatsContainer GlobalStats
        {
            get { return _globalStats; }
            set { SetProperty(ref _globalStats, value); }
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

        private bool _hasRepeta;
        public bool HasRepeta
        {
            get { return _hasRepeta; }
            set => SetProperty(ref _hasRepeta, value);
        }



        public WarpDisplayVM WarpDisplayVM { get; set; }
               

        #endregion

        #region Point site properties
        private string _pointSiteMeasure;
        public string PointSiteMeasure
        {
            get => _pointSiteMeasure;
            set => SetProperty(ref _pointSiteMeasure, value);
        }

        private string _pointSiteType;
        public string PointSiteType
        {
            get { return _pointSiteType; }
            set { SetProperty(ref _pointSiteType, value); }
        }


        #endregion properties

        public WarpDetailMeasureInfoVM()
        {
            WarpDisplayVM = new WarpDisplayVM();
        }

        #region Overrides of MetroDetailMeasureInfoVM

        protected override void OnPointChanged()
        {
            base.OnPointChanged();

            PointSiteMeasure = string.Empty;
            var warpMax = new Length(0.0, LengthUnit.Micrometer);

            if (!string.IsNullOrEmpty(Output))
            {
                WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(Output);

                PointSiteType = viewerType == WarpViewerType.WARP ? EnumUtils.GetAttribute<ChartDisplayDescriptionAttribute>(viewerType).DisplayDescription
                                        : EnumUtils.GetAttribute<CompleteDisplayNameAttribute>(viewerType).Name;


                if (Point != null && Settings != null && Settings.WarpMax != null)
                {
                    warpMax = Settings.WarpMax;

                    PointSiteMeasure = viewerType == WarpViewerType.WARP ? LengthToStringConverter.ConvertToString(Point.RPDStat?.Mean, Digits, true, "-", LengthUnit.Micrometer)
                                                                        : LengthToStringConverter.ConvertToString(Point.TotalThicknessStat?.Mean, Digits, true, "-", LengthUnit.Micrometer);


                    if (viewerType == WarpViewerType.WARP)
                    {
                        GlobalOutput = Enum.GetName(typeof(WarpViewerType), WarpViewerType.WARP);
                        GlobalOutput = (string)(Settings.IsSurfaceWarp ? GlobalOutput += " (" + WarpType.SURFACE + ")" : GlobalOutput += " (" + WarpType.MEDIAN + ")");

                        if (GlobalStats != null)
                        {
                            GlobalState = GlobalStats.State;
                            WarpResultLength = GlobalStats.Mean;
                        }
                    }
                    else
                    {
                        GlobalOutput = Enum.GetName(typeof(WarpViewerType), WarpViewerType.TTV);

                        if (GlobalStats != null)
                        {
                            GlobalState = GlobalStats.State;
                            TTVResult = LengthToStringConverter.ConvertToString(GlobalStats.Mean, Digits, true, "-", LengthUnit.Micrometer);
                        }
                    }

                    WarpDisplayVM.UpdateWarpDisplay(WarpResultLength, Digits, warpMax, GlobalState);

                }
            }
        }


        internal void Update(WarpResultSettings settings, string output, MetroStatsContainer stats, double qualityScore, bool hasRepeta)
        {
            Settings = settings;
            GlobalStats = stats;
            Output = output;
            QualityScore = qualityScore;
            HasRepeta = hasRepeta;
        }

        #endregion Overrides of MetroDetailMeasureInfoVM
    }
}
