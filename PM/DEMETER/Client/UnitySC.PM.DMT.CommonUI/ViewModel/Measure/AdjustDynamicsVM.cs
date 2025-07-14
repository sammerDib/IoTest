using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using AutoMapper.Internal;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    public class AdjustDynamicsVM : PageNavigationVM
    {
        private Dictionary<CurvatureImageType, ServiceImage> _imagesDictionary;
        private readonly RecipeSupervisor _recipeSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly Mapper _mapper;

        public AdjustDynamicsVM(RecipeSupervisor recipeSupervisor, IDialogOwnerService dialogService, Mapper mapper)
        {
            _recipeSupervisor = recipeSupervisor;
            _dialogService = dialogService;
            _mapper = mapper;
            ComputeSliderTicks();
        }

        public DeflectometryVM MeasureVM { get; set; }

        public override void Loaded()
        {
            base.Loaded();
            _curvatureSliderTickIndex = FindClosestSliderTickIndex(CurvatureDynamic);
            _darkSliderTickIndex = FindClosestSliderTickIndex(DarkDynamic);
            _unwrappedSliderTickIndex = FindClosestSliderTickIndex(UnWrappedDynamic);
            OnPropertyChanged(nameof(CurvatureSliderTickIndex));
            OnPropertyChanged(nameof(DarkSliderTickIndex));
            OnPropertyChanged(nameof(UnWrappedSliderTickIndex));
        }

        public override void Unloading()
        {
            base.Unloading();
            _recipeSupervisor.DisposeCurvatureDynamicAdjustmentMeasureExecution();
        }

        private CurvatureImageType GetImageDictionaryKey()
        {
            var key = CurvatureImageType.CurvatureMapX;
            switch (Dynamic)
            {
                case DynamicsType.Curvature:
                    switch (Direction)
                    {
                        case Orientation.Horizontal:
                            key = CurvatureImageType.CurvatureMapX;
                            break;

                        case Orientation.Vertical:
                            key = CurvatureImageType.CurvatureMapY;
                            break;
                    }

                    break;

                case DynamicsType.Dark:
                    key = CurvatureImageType.Dark;
                    break;
            }

            return key;
        }

        #region Proprités bindables

        public override string PageName => "Curvature, dark and unwrapped dynamics";
        public double UnWrappedDynamic
        {
            get => MeasureVM.UnWrappedDynamic;
            set
            {
                if (MeasureVM.UnWrappedDynamic != value)
                {
                    MeasureVM.UnWrappedDynamic = value;
                    OnPropertyChanged();
                    _unwrappedSliderTickIndex = FindClosestSliderTickIndex(value);
                    OnPropertyChanged(nameof(UnWrappedSliderTickIndex));
                }
            }
        }
        public double CurvatureDynamic
        {
            get => MeasureVM.CurvatureDynamic;
            set
            {
                if (MeasureVM.CurvatureDynamic != value)
                {
                    MeasureVM.CurvatureDynamic = value;
                    OnPropertyChanged();
                    // On n'utilise pas la propriété pour éviter les mises à jour récursives
                    _curvatureSliderTickIndex = FindClosestSliderTickIndex(value);
                    OnPropertyChanged(nameof(CurvatureSliderTickIndex));
                }
            }
        }

        public double DarkDynamic
        {
            get => MeasureVM.DarkDynamic;
            set
            {
                if (MeasureVM.DarkDynamic != value)
                {
                    MeasureVM.DarkDynamic = value;
                    OnPropertyChanged();
                    _darkSliderTickIndex = FindClosestSliderTickIndex(value);
                    OnPropertyChanged(nameof(DarkSliderTickIndex));
                }
            }
        }

        private BitmapSource _displayedImage;

        public BitmapSource DisplayedImage
        {
            get => _displayedImage;
            set
            {
                if (_displayedImage != value)
                {
                    _displayedImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private Orientation _direction;

        public Orientation Direction
        {
            get => _direction;
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    OnPropertyChanged();
                    if (_imagesDictionary != null)
                    {
                        DisplayedImage = _imagesDictionary[GetImageDictionaryKey()].WpfBitmapSource;
                    }
                }
            }
        }

        private bool _isBaseAcquisitionDone;

        public bool IsBaseAcquisitionDone
        {
            get => _isBaseAcquisitionDone;
            set
            {
                if (_isBaseAcquisitionDone != value)
                {
                    _isBaseAcquisitionDone = value;
                    OnPropertyChanged();
                }
            }
        }

        private DynamicsType _dynamic;

        public DynamicsType Dynamic
        {
            get => _dynamic;
            set
            {
                if (_dynamic != value)
                {
                    _dynamic = value;
                    OnPropertyChanged();
                    if (_imagesDictionary != null && IsBaseAcquisitionDone)
                    {
                        DisplayedImage = _imagesDictionary[GetImageDictionaryKey()].WpfBitmapSource;
                    }
                }
            }
        }

        private string _busyContent = "Please wait while base images are acquired and computed ...";

        public string BusyContent
        {
            get => _busyContent;
            set
            {
                if (_busyContent != value)
                {
                    _busyContent = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Proprités bindables

        #region Commands

        private AsyncRelayCommand _recalculateCurvatureDynamicCommand;

        public AsyncRelayCommand RecalculateCurvatureDynamicCommand =>
            _recalculateCurvatureDynamicCommand ?? (_recalculateCurvatureDynamicCommand = new AsyncRelayCommand(
                 async () =>
                 {
                     try
                     {
                         BusyContent = "Please wait while computing new image(s) ...";
                         var measure = _mapper.AutoMap.Map<DeflectometryMeasure>(MeasureVM);
                         switch (Dynamic)
                         {
                             case DynamicsType.Curvature:
                                 var curvatureTask = _recipeSupervisor.RecalculateCurvatureDynamic(measure);
                                 await curvatureTask;
                                 curvatureTask.Result.Where(kvPair => kvPair.Key != CurvatureImageType.Dark)
                                              .ForAll(kvPair => _imagesDictionary[kvPair.Key] = kvPair.Value);
                                 break;

                             case DynamicsType.Dark:
                                 var darkTask = _recipeSupervisor.RecalculateDarkDynamics(measure);
                                 await darkTask;
                                 _imagesDictionary[CurvatureImageType.Dark] = darkTask.Result;
                                 break;
                         }

                         if (_imagesDictionary == null)
                         {
                             _dialogService.ShowMessageBox("No valid acquisition.\r\nPlease execute the recipe once to get images.");
                         }
                         else
                         {
                             var key = GetImageDictionaryKey();
                             DisplayedImage = _imagesDictionary[key].WpfBitmapSource;
                         }
                    }
                    catch
                    {
                         _dialogService.ShowMessageBox("Error during curvature dynamic recalculation.");
                    }
                }));

        private AsyncRelayCommand _acquireBaseCurvatureImagesCommand;

        public AsyncRelayCommand AcquireBaseCurvatureImagesCommand =>
            _acquireBaseCurvatureImagesCommand ?? (_acquireBaseCurvatureImagesCommand = new AsyncRelayCommand(
                 async () =>
                 {
                     try
                     {
                         BusyContent = "Please wait while base images are acquired and computed ...";
                         var measure = _mapper.AutoMap.Map<DeflectometryMeasure>(MeasureVM);
                         bool isDarkFieldRequired = MeasureVM.Outputs[DeflectometryOutput.LowAngleDarkField].IsSelected;
                         var diameter = MeasureVM.WaferDimensions.Diameter;
                         _imagesDictionary =
                             await _recipeSupervisor.BaseCurvatureDynamicAcquisition(measure, diameter, isDarkFieldRequired);
                         if (_imagesDictionary is null || _imagesDictionary.Count == 0)
                         {
                             _dialogService
                                          .ShowMessageBox("Unable to acquire base images for curvature computations.");
                         }
                         else
                         {
                             var key = GetImageDictionaryKey();
                             DisplayedImage = _imagesDictionary[key].WpfBitmapSource;
                             IsBaseAcquisitionDone = true;
                         }
                     }
                     catch
                     {
                        _dialogService.ShowMessageBox("Error during base images acquisition for curvature computations.");
                     }
                 },
                 () => !IsBaseAcquisitionDone));

        #endregion Commands

        #region Slider

        public List<double> SliderValues { get; set; }

        public int MaxSliderIndex { get; set; } = 200;
        public string CurvatureSliderTickText { get; set; }

        public string DarkSliderTickText { get; set; }

        public string UnWrappedSliderTickText { get; set; }

        private int _curvatureSliderTickIndex;

        public int CurvatureSliderTickIndex
        {
            get => _curvatureSliderTickIndex;
            set
            {
                if (_curvatureSliderTickIndex != value)
                {
                    _curvatureSliderTickIndex = value;
                    OnPropertyChanged();
                    // On n'utilise pas la propriété pour éviter les mises à jour récursives
                    MeasureVM.CurvatureDynamic = SliderValues[value];
                    OnPropertyChanged(nameof(CurvatureDynamic));
                }
            }
        }
        private int _unwrappedSliderTickIndex;
        public int UnWrappedSliderTickIndex
        {
            get => _unwrappedSliderTickIndex;
            set
            {
                if (_unwrappedSliderTickIndex != value)
                {
                    _unwrappedSliderTickIndex = value;
                    OnPropertyChanged();
                    // On n'utilise pas la propriété pour éviter les mises à jour récursives
                    MeasureVM.UnWrappedDynamic = SliderValues[value];
                    OnPropertyChanged(nameof(UnWrappedDynamic));
                }
            }
        }
        private int _darkSliderTickIndex;

        public int DarkSliderTickIndex
        {
            get => _darkSliderTickIndex;
            set
            {
                if (_darkSliderTickIndex != value)
                {
                    _darkSliderTickIndex = value;
                    OnPropertyChanged();
                    // On n'utilise pas la propriété pour éviter les mises à jour récursives
                    MeasureVM.DarkDynamic = SliderValues[value];
                    OnPropertyChanged(nameof(DarkDynamic));
                }
            }
        }

        private void ComputeSliderTicks()
        {
            SliderValues = new List<double>();

            for (double i = 0.005; i < 0.04; i += 0.001)
                SliderValues.Add(i);
            for (double i = 0.04; i < 0.05; i += 0.002)
                SliderValues.Add(i);
            for (double i = 0.05; i < 0.1; i += 0.01)
                SliderValues.Add(i);
            for (double i = 0.1; i < 0.5; i += 0.05)
                SliderValues.Add(i);
            for (double i = 0.5; i < 1; i += 0.1)
                SliderValues.Add(i);
            for (double i = 1; i < 5; i += 0.5)
                SliderValues.Add(i);
            for (double i = 5; i < 10; i += 1)
                SliderValues.Add(i);
            for (double i = 10; i < 20; i += 5)
                SliderValues.Add(i);
            for (double i = 20; i < 101; i += 10)
                SliderValues.Add(i);

            MaxSliderIndex = SliderValues.Count() - 1;

            var labels = new List<string>();
            labels.Resize((MaxSliderIndex + 4) / 5);
            labels[1] = "Hi-Sensitivity";
            labels[labels.Count() - 1] = "Noise-Proof";

            CurvatureSliderTickText = string.Join(",", labels);
            DarkSliderTickText = string.Join(",", labels);
            UnWrappedSliderTickText = string.Join(",", labels);
        }

        private int FindClosestSliderTickIndex(double value)
        {
            int i;
            for (i = 0; i < SliderValues.Count(); i++)
            {
                if (SliderValues[i] >= value)
                    break;
            }

            if (i >= SliderValues.Count())
                return i - 1;
            return i;
        }

        #endregion Slider
    }
}
