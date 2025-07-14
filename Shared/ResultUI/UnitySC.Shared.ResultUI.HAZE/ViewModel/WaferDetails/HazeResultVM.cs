using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Display.HAZE;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.HAZE;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.UI.Helper;

namespace UnitySC.Shared.ResultUI.HAZE.ViewModel.WaferDetails
{
    public class HazeResultVM : ResultWaferVM
    {
        public HazeResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            ColorMap = ColorMapHelper.ColorMaps.FirstOrDefault();

            // TODO : à verifie comportement à chaque affichage avec (_resDisplay as HazeDisplay).TableColorMap

            HazeRangeToStringFunc = range =>
            {
                if (float.IsNegativeInfinity(range.Min_ppm))
                {
                    return $"Below {range.Max_ppm}";
                }
                if (float.IsPositiveInfinity(range.Max_ppm))
                {
                    return $"Above {range.Min_ppm}";
                }
                return $"{range.Min_ppm} ... {range.Max_ppm}";
            };
            HazeRangeToAreaFunc = range => $"{range.Area_pct:#0.000}";
            HazeRangeToAreaDoubleFunc = range => range.Area_pct;
            HazeMapIndexIsEnabled = i => DataHaze?.HazeMaps != null && i <= DataHaze.HazeMaps.Count - 1;

            HazeProfileChart = new HazeProfileChartVM();
            HazeMapVm = new HazeMapVM(() => SelectedHazeMap, HazeProfileChart);
            HazeHistogramChart = new HazeHistogramChartVM();

            HazeProfileChart.MarkerPositionChanged += HazeProfileChart_MarkerPositionChanged;
        }

        #region Properties

        public HazeProfileChartVM HazeProfileChart { get; }

        public HazeHistogramChartVM HazeHistogramChart { get; }

        public HazeMapVM HazeMapVm { get; }

        public Func<HazeRange, string> HazeRangeToStringFunc { get; }

        public Func<HazeRange, string> HazeRangeToAreaFunc { get; }

        public Func<HazeRange, double> HazeRangeToAreaDoubleFunc { get; }

        public Func<int, bool> HazeMapIndexIsEnabled { get; }

        private DataHaze DataHaze => ResultDataObj as DataHaze;
        
        public HazeMap SelectedHazeMap
        {
            get
            {
                if (DataHaze?.HazeMaps == null) return null;
                if (_selectedHazeMapIndex < 0 || _selectedHazeMapIndex >= DataHaze.HazeMaps.Count) return null;
                return DataHaze.HazeMaps[_selectedHazeMapIndex];
            }
        }

        public List<HazeRange> SelectedHazeMapRanges => SelectedHazeMap?.Ranges ?? new List<HazeRange>();

        private int _selectedHazeMapIndex;

        public int SelectedHazeMapIndex
        {
            get { return _selectedHazeMapIndex; }
            set
            {
                if (SetProperty(ref _selectedHazeMapIndex, value))
                {
                    OnSelectedHazeMapChanged();
                }
            }
        }

        public List<int> HazeMapsIndexSource { get; } = new List<int> { 0, 1, 2 };
        
        private ColorMap _colorMap;

        public ColorMap ColorMap
        {
            get { return _colorMap; }
            set
            {
                if (SetProperty(ref _colorMap, value) && _colorMap != null)
                {
                    ResultDisplay.UpdateInternalDisplaySettingsPrm(_colorMap.Name);
                    UpdateHazeImageAndHistogram();
                }
            }
        }

        private Bitmap _bitmap;
        
        private float _minValue;

        public float MinValue
        {
            get { return _minValue; }
            set
            {
                if (SetProperty(ref _minValue, value))
                {
                    UpdateHazeImageAndHistogram();
                }
            }
        }

        private float _maxValue;

        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (SetProperty(ref _maxValue, value))
                {
                    UpdateHazeImageAndHistogram();
                }
            }
        }
        
        #region Global Stats

        public float MaxPpm => SelectedHazeMap?.Max_ppm ?? float.NaN;
        public float MinPpm => SelectedHazeMap?.Min_ppm ?? float.NaN;      
        public float MeanPpm => SelectedHazeMap?.Mean_ppm ?? float.NaN;
        public float StdDevPpm => SelectedHazeMap?.Stddev_ppm ?? float.NaN;
        public float MedianPpm => SelectedHazeMap?.Median_ppm ?? float.NaN;
        public int? Height => SelectedHazeMap?.Heigth;
        public int? Width => SelectedHazeMap?.Width;
        
        #endregion

        #endregion

        #region Event Handlers

        private void HazeProfileChart_MarkerPositionChanged(object sender, EventArgs e)
        {
            HazeMapVm.UpdateMarkerPosition(HazeProfileChart.MarkerIndex, HazeProfileChart.HorizontalMarkerIndex, HazeProfileChart.VerticalMarkerIndex);
        }

        #endregion

        #region Private Methods

        private void UpdateHazeImageAndHistogram()
        {
            if (SelectedHazeMap == null || ColorMap == null) return;
            
            UpdateHazeImage();
            HazeHistogramChart.UpdateColors(MaxValue, MinValue, SelectedHazeMap, ColorMap);
        }
        
        private void UpdateHazeImage()
        {
            if (ResultDisplay == null || DataHaze == null || ColorMap == null || SelectedHazeMap == null)
            {
                if (HazeMapVm != null)
                {
                    HazeMapVm.HazeImage = null;
                }
                return;
            }

            IsBusy = true;

            HazeMapVm.HazeImage = null;
            _bitmap?.Dispose();
            _bitmap = null;
            GC.Collect();

            var imageConfiguration = new HazeImageConfiguration
            {
                MinValue = MinValue,
                MaxValue = MaxValue,
                HazeMapIndex = _selectedHazeMapIndex
            };
            
            Task.Run(() =>
            {
                _bitmap = ResultDisplay.DrawImage(ResultDataObj, imageConfiguration);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    HazeMapVm.HazeImage = ImageHelper.ConvertToBitmapSource(_bitmap);
                    IsBusy = false;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                });
            });

        }

        private void OnSelectedHazeMapChanged()
        {
            UpdateHazeImage();
            OnPropertyChanged(nameof(SelectedHazeMap));
            OnPropertyChanged(nameof(SelectedHazeMapRanges));
            OnPropertyChanged(nameof(MaxPpm));
            OnPropertyChanged(nameof(MinPpm));
            OnPropertyChanged(nameof(MeanPpm));
            OnPropertyChanged(nameof(StdDevPpm));
            OnPropertyChanged(nameof(MedianPpm));

            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(Width));

            HazeMapVm.OnSelectedHazeMapChanged();
            UpdateHistogramChart();
        }

        private void UpdateHistogramChart()
        {
            if (SelectedHazeMap == null || ColorMap == null) return;
            HazeHistogramChart.ResetChart(MaxValue, MinValue, SelectedHazeMap, ColorMap);
        }
        
        #endregion

        #region Commands

        private ICommand _dynamicCommand;

        public ICommand DynamicCommand => _dynamicCommand ?? (_dynamicCommand = new AutoRelayCommand(DynamicCommandExecute));

        private void DynamicCommandExecute()
        {
            if (SelectedHazeMap != null)
            {
                SetProperty( ref _minValue, SelectedHazeMap.Min_ppm,nameof(MinValue));
                SetProperty( ref _maxValue, SelectedHazeMap.Max_ppm,nameof(MaxValue));
                UpdateHazeImageAndHistogram();
            }
        }

        private ICommand _medianCommand;

        public ICommand MedianCommand => _medianCommand ?? (_medianCommand = new AutoRelayCommand(MedianCommandExecute));
        
        private void MedianCommandExecute()
        {
            if (SelectedHazeMap != null)
            {
                SetProperty( ref _minValue, SelectedHazeMap.Median_ppm - SelectedHazeMap.Stddev_ppm,nameof(MinValue));
                SetProperty( ref _maxValue, SelectedHazeMap.Median_ppm + SelectedHazeMap.Stddev_ppm,nameof(MaxValue));
                UpdateHazeImageAndHistogram();
            }
        }

        private ICommand _meanStdCommand;

        public ICommand MeanStdCommand => _meanStdCommand ?? (_meanStdCommand = new AutoRelayCommand(MeanStdCommandExecute));

        private void MeanStdCommandExecute()
        {
            if (SelectedHazeMap != null)
            {
                SetProperty( ref _minValue, SelectedHazeMap.Mean_ppm - SelectedHazeMap.Stddev_ppm,nameof(MinValue));
                SetProperty( ref _maxValue, SelectedHazeMap.Mean_ppm + SelectedHazeMap.Stddev_ppm,nameof(MaxValue));
                UpdateHazeImageAndHistogram();
            }
        }
        
        #endregion
        
        #region Overrides

        public override void UpdateResData(IResultDataObject resdataObj)
        {
            base.UpdateResData(resdataObj);

            if (ResultDisplay is HazeDisplay hazeDisplay)
            {
                // Define the ColorMap without going through the property setter because it initiates the generation of a new image.
                SetProperty(ref _colorMap, hazeDisplay.TableColorMap, nameof(ColorMap));
            }

            HazeMapVm.ClearProfile();

            if (SelectedHazeMapIndex < 0 || SelectedHazeMapIndex >= DataHaze.HazeMaps.Count)
            {
                //Set(nameof(SelectedHazeMapIndex), ref _selectedHazeMapIndex); // TODO Tester
                SetProperty(ref _selectedHazeMapIndex, default, nameof(SelectedHazeMapIndex));
            }

            if (SelectedHazeMap != null)
            {
                // Init Min & Max with HazeMap values
                SetProperty( ref _minValue, SelectedHazeMap.Min_ppm,nameof(MinValue));
                SetProperty( ref _maxValue, SelectedHazeMap.Max_ppm,nameof(MaxValue));
            }

            OnPropertyChanged(nameof(HazeMapIndexIsEnabled));
            OnSelectedHazeMapChanged();
            HazeMapVm.RaiseDataChanged();
        }

        public override string FormatName => "HAZE";

        protected override void OnChangeSelectedWaferDetaillName(DisplaySelectedWaferDetaillNameMessage msg)
        {
            base.OnChangeSelectedWaferDetaillName(msg);
            HazeMapVm.ExportFileName = msg.SelectedWaferDetaillName;
        }
        
        #endregion

        #region Overrides of ResultWaferVM

        public override void Dispose()
        {
            HazeProfileChart.MarkerPositionChanged -= HazeProfileChart_MarkerPositionChanged;
            HazeProfileChart.Dispose();
            HazeHistogramChart.Dispose();

            _bitmap?.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
