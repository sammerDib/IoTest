using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp
{
    public class WarpDisplayVM : ObservableObject
    {
        #region warp display
        private Color _resultColor;
        public Color ResultColor
        {
            get { return _resultColor; }
            set { SetProperty(ref _resultColor, value); }
        }

        private double _totalHeight;
        public double TotalHeight
        {
            get { return _totalHeight; }
            set { SetProperty(ref _totalHeight, value); }
        }

        private double _heightMeasure;
        public double HeightMeasure
        {
            get { return _heightMeasure; }
            set { SetProperty(ref _heightMeasure, value); }
        }

        private double _heightText;
        public double HeightText
        {
            get { return _heightText; }
            set { SetProperty(ref _heightText, value); }
        }


        private MeasureState _globlaState;
        public MeasureState GlobalState
        {
            get => _globlaState;
            set => SetProperty(ref _globlaState, value);
        }

        private string _warpResult;
        public string WarpResult
        {
            get { return _warpResult; }
            set { SetProperty(ref _warpResult, value); }
        }
        
        private Length _warpMax;
        public Length WarpMax
        {
            get { return _warpMax; }
            set { SetProperty(ref _warpMax, value); }
        }

        private Length _warpResultLength;
        public Length WarpResultLength
        {
            get { return _warpResultLength; }
            set { SetProperty(ref _warpResultLength, value); }
        }
        #endregion


        private static Color s_validResultColor = Color.FromArgb(8, 180, 8);
        private static Color s_invalidResultColor = Color.FromArgb(216, 18, 18);
        public void UpdateWarpDisplay(Length warpResultLength, int digits, Length warpMax, MeasureState state)
        {
            int sizeArrow = 13;
            int minTextMargin = 8;
            int textMargin = 5;
            WarpResult = string.Empty;
            TotalHeight = 120;
            HeightMeasure = 0;
            HeightText = 0;

            GlobalState = state;
            WarpMax = warpMax;
            WarpResultLength = warpResultLength;

           if (WarpResultLength == null) return;

            WarpResult = LengthToStringConverter.ConvertToString(WarpResultLength, digits, true, "-", LengthUnit.Micrometer);

            if (0 < WarpResultLength.Value && WarpResultLength.Value < warpMax.Value)
            {
                ResultColor = s_validResultColor;

                HeightMeasure = WarpResultLength.Value * TotalHeight / warpMax.Value;
                HeightText = HeightMeasure;

                if (TotalHeight - HeightMeasure < minTextMargin)
                {
                    HeightText = HeightMeasure + textMargin;
                }
                else if (HeightText < minTextMargin)
                {
                    HeightText = sizeArrow;
                }
                else
                    HeightText = HeightMeasure + minTextMargin;
            }
            else
            {
                ResultColor = s_invalidResultColor;
                HeightMeasure = (WarpResultLength.Value < 0) ? 0 : TotalHeight;
            }
        }


    }
}
