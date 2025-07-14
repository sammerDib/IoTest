using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public class ThicknessLayersInfoVM : ObservableObject
    {
        #region Properties

        public double ItemHeight
        {
            get
            {
                if (LayersDetails == null) return 0;
                if (LayersDetails.Count >= 4) return 30;
                if (LayersDetails.Count == 0) return 120;
                return 120 / (double)LayersDetails.Count;
            }
        }

        private List<LayerDetailInfoVM> _layersDetails;

        public List<LayerDetailInfoVM> LayersDetails
        {
            get { return _layersDetails; }
            set
            {
                SetProperty(ref _layersDetails, value);
                OnPropertyChanged(nameof(ItemHeight));
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private string _waferThickness = string.Empty;

        public string WaferThickness
        {
            get { return _waferThickness; }
            set { SetProperty(ref _waferThickness, value); }
        }

        private string _leftSideTooltip;

        public string LeftSideTooltip
        {
            get { return _leftSideTooltip; }
            set { SetProperty(ref _leftSideTooltip, value); }
        }

        private bool _showTotalArrow;

        public bool ShowTotalArrow
        {
            get { return _showTotalArrow; }
            set { SetProperty(ref _showTotalArrow, value); }
        }

        #endregion

        #region Constructors

        public ThicknessLayersInfoVM()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                var layersDetails = new List<LayerDetailInfoVM>();

                var random = new Random();

                const int itemCount = 5;

                for (int i = 0; i < itemCount; i++)
                {
                    var namedLengthOutput = new ThicknessLengthSettings
                    {
                        IsMeasured = i != 2,
                        Name = $"Layer {i + 1}",
                        Target = new Length(400, LengthUnit.Micrometer),
                        Tolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer)
                    };

                    var statsContainer = MetroStatsContainer.GenerateFromLength(new List<Tuple<Length, MeasureState>>
                    {
                        new Tuple<Length, MeasureState>(new Length(random.Next(350, 450), LengthUnit.Micrometer), MeasureState.Success),
                        new Tuple<Length, MeasureState>(new Length(random.Next(350, 450), LengthUnit.Micrometer), MeasureState.Error)
                    });

                    double percent = i / (double)itemCount;
                    layersDetails.Add(new LayerDetailInfoVM($"Layer {i + 1}", namedLengthOutput, statsContainer, 3, Color.FromRgb((byte)(percent * 255), 100, 0), LengthUnit.Micrometer));
                }

                LayersDetails = layersDetails;

                Title = "Total";
                Value = $"Value = {Environment.NewLine}4000,000 µm{Environment.NewLine}{Environment.NewLine}Deviation = {Environment.NewLine}12,05 µm";
                WaferThickness = $"Value = {Environment.NewLine}4025,000 µm{Environment.NewLine}{Environment.NewLine}Deviation = {Environment.NewLine}37,05 µm";
            }
        }

        #endregion
    }
}
