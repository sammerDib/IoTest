using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.DMT.Service.Interface;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class FocusDataVM : ObservableObject
    {
        private uint _id = 0;

        public uint Id
        {
            get => _id; set { if (_id != value) { _id = value; OnPropertyChanged(); } }
        }

        private int _top = 0;

        public int Top
        {
            get => _top; set { if (_top != value) { _top = value; OnPropertyChanged(); } }
        }

        private int _left = 0;

        public int Left
        {
            get => _left; set { if (_left != value) { _left = value; OnPropertyChanged(); } }
        }

        private uint _focusZoneX = 0;

        public uint FocusZoneX
        {
            get => _focusZoneX; set { if (_focusZoneX != value) { _focusZoneX = value; OnPropertyChanged(); } }
        }

        private uint _focusZoneY = 0;

        public uint FocusZoneY
        {
            get => _focusZoneY; set { if (_focusZoneY != value) { _focusZoneY = value; OnPropertyChanged(); } }
        }

        private uint focusZoneWidth = 0;

        public uint FocusZoneWidth
        {
            get => focusZoneWidth; set { if (focusZoneWidth != value) { focusZoneWidth = value; OnPropertyChanged(); } }
        }

        private uint _focusZoneHeight = 0;

        public uint FocusZoneHeight
        {
            get => _focusZoneHeight;
            set
            {
                if (_focusZoneHeight != value)
                {
                    _focusZoneHeight = value;
                    FontSize = _focusZoneHeight / 4;
                    GaugeWidth = (int)(_focusZoneHeight / 4);
                    BorderThickness = Math.Max((int)(_focusZoneHeight / 20), 10);
                    OnPropertyChanged();
                }
            }
        }

        private double _focusQuality = 0;

        public double FocusQuality
        {
            get => _focusQuality;
            set
            {
                if (_focusQuality != value)
                {
                    _focusQuality = value;
                    if (_focusQuality > _focusQualityMax)
                        FocusQualityMax = _focusQuality;
                    OnPropertyChanged();
                }
            }
        }

        private double _focusQualityMax = 5;

        public double FocusQualityMax
        {
            get => _focusQualityMax; set { if (_focusQualityMax != value) { _focusQualityMax = value; OnPropertyChanged(); } }
        }

        private int _gaugeWidth = 300;

        public int GaugeWidth
        {
            get => _gaugeWidth; set { if (_gaugeWidth != value) { _gaugeWidth = value; OnPropertyChanged(); } }
        }

        private double _fontSize = 300;

        public double FontSize
        {
            get => _fontSize; set { if (_fontSize != value) { _fontSize = value; OnPropertyChanged(); } }
        }

        private int _borderThickness = 30;

        public int BorderThickness
        {
            get => _borderThickness; set { if (_borderThickness != value) { _borderThickness = value; OnPropertyChanged(); } }
        }

        internal void UpdateFrom(SubImageProperties sSubImProp)
        {
            Id = sSubImProp.SubImageNumber;
            FocusQuality = sSubImProp.ComputedFocusQuality;
            FocusZoneX = sSubImProp.SubImagePositionX;
            FocusZoneY = sSubImProp.SubImagePositionY;
            Left = (int)sSubImProp.SubImagePositionX;
            Top = (int)sSubImProp.SubImagePositionY;
            FocusZoneWidth = sSubImProp.SubImageWidth;
            FocusZoneHeight = sSubImProp.SubImageHeight;

            Console.WriteLine($"X : {FocusZoneX}  Y : {FocusZoneY}  Width = {FocusZoneWidth}  Height = {FocusZoneHeight}");
        }

        internal void Reset()
        {
            FocusQualityMax = 5;
        }
    }
}
