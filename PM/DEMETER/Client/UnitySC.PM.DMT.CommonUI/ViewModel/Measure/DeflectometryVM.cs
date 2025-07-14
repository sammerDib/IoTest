using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Enum;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    internal class BooleanToCurvatureDarkDynamicsHeaderConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string textOutput = null;
            if (values.All(v => v is bool))
            {
                if ((bool)values[0])
                {
                    textOutput+= "Curvature ";
                }
                if ((bool)values[1])
                {
                    textOutput += "Dark ";
                }
                if ((bool)values[2])
                {
                    textOutput += "UnWrapped ";
                }
                if (textOutput != null) 
                { 
                    textOutput += "Dynamics"; 
                    return textOutput; 
                }
                return DependencyProperty.UnsetValue;    
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class DeflectometryVM : MeasureVM
    {
        private ScreenInfo _screenInfo;
        private readonly CameraSupervisor _cameraSupervisor;
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly RecipeSupervisor _recipeSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly Mapper _mapper;
        private readonly MainRecipeEditionVM _mainRecipeEditionVM;

        public DeflectometryVM(CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, CalibrationSupervisor calibrationSupervisor,
            AlgorithmsSupervisor algorithmsSupervisor, RecipeSupervisor recipeSupervisor, IDialogOwnerService dialogService,
            Mapper mapper, MainRecipeEditionVM mainRecipeEditionVM, Side side)
        {
            Side = side;
            _cameraSupervisor = cameraSupervisor;
            _screenSupervisor = screenSupervisor;
            _calibrationSupervisor = calibrationSupervisor;
            _algorithmsSupervisor = algorithmsSupervisor;
            _recipeSupervisor = recipeSupervisor;
            _dialogService = dialogService;
            _mapper = mapper;
            _mainRecipeEditionVM = mainRecipeEditionVM;

            Title = "Deflectometry";
            CanUseEnhancedMask = _calibrationSupervisor.HasPerspectiveCalibration(Side);

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            var allfringes = _screenSupervisor.GetAvailableFringes();

            // Standard fringes
            //.................
            var standardFringes = allfringes.Where(f => f.FringeType == FringeType.Standard).ToList();
            StandardFringes = _mapper.AutoMap.Map<List<Fringe>, List<FringeVM>>(standardFringes);
            StandardFringe = StandardFringes.FirstOrDefault();

            var groups = StandardFringes.OrderBy(x => x.Period).GroupBy(x => x.Period).ToList();
            MaxSlopePrecision = groups.Count - 1;
            for (int i = 0; i < groups.Count; i++)
            {
                foreach (var f in groups[i])
                    f.SlopePrecision = i;
            }

            SlopePrecisionTickText = BuildSlopePrecisionTickText(MaxSlopePrecision);

            groups = StandardFringes.OrderBy(x => x.NbImagesPerDirection).GroupBy(x => x.NbImagesPerDirection).ToList();
            MaxPhaseShiftPrecision = groups.Count - 1;
            for (int i = 0; i < groups.Count; i++)
            {
                foreach (var f in groups[i])
                    f.PhaseShiftPrecision = i;
            }

            PhaseShiftPrecisionTickText = BuildPhaseShifPrecisionTickText(MaxPhaseShiftPrecision);

            // Multi fringes
            //..............
            MultiFringe = new FringeVM();
            MultiFringe.FringeType = FringeType.Multi;
            _screenInfo = _screenSupervisor.GetScreenInfo(Side);
            MultiSlopePrecisionList = standardFringes.Select(fringe => fringe.Period).Distinct().ToList();
            _multiNbImagePerDirectionList = standardFringes.Select(fringe => fringe.NbImagesPerDirection).Distinct().ToList();
            MultiSlopePrecisionTickText = BuildSlopePrecisionTickText(_multiSlopePrecisionList.Count - 1);
            MultiSlopePrecisionIndex = 0;
            MultiSlopePrecision = _multiSlopePrecisionList[MultiSlopePrecisionIndex];

            InitializeAvailablePeriods();
            _multiPeriodIndex = AvailablePeriods.Count / 2;
            Periods = AvailablePeriods.Keys.ElementAt(_multiPeriodIndex);
            MultiPeriodRatio = AvailablePeriods[Periods];
            MultiPeriodRatioTickText = BuildMultiPeriodRatioTickText(AvailablePeriods.Count - 1);
            MultiNbImagesPerDirection = _multiNbImagePerDirectionList[0];
        }

        private string BuildMultiPeriodRatioTickText(int max)
        {
            return BuildTickText(max, "High surface variations", "", "Smooth surface");
        }

        private string BuildSlopePrecisionTickText(int max)
        {
            return BuildTickText(max, "High (Optimal)", "Intermediate", "Low (Rough Wafer)");
        }

        private string BuildPhaseShifPrecisionTickText(int max)
        {
            return BuildTickText(max, "Hi-Speed", "Intermediate", "Hi-Precision");
        }

        private string BuildTickText(double max, string left, string middle, string right)
        {
            int nbticks = (int)max + 1;

            if (nbticks <= 1)
            {
                return "";
            }
            else
            {
                var tickTextList = Enumerable.Range(0, nbticks).Select(x => "").ToList();
                switch (nbticks)
                {
                    case 2:
                        tickTextList[0] = left;
                        tickTextList[1] = right;
                        break;

                    default:
                        int middleTickIndex = nbticks / 2;
                        tickTextList[0] = left;
                        tickTextList[middleTickIndex] = middle;
                        tickTextList[tickTextList.Count - 1] = right;
                        break;
                }
                return String.Join(",", tickTextList);
            }
        }

        //=================================================================

        #region Propriétés bindables communes

        //=================================================================

        public override HelpTag HelpTag => HelpTag.DMT_Deflecto;

        // Outputs
        private Dictionary<DeflectometryOutput, SelectableItemVM<DeflectometryOutput>> _outputs;

        public Dictionary<DeflectometryOutput, SelectableItemVM<DeflectometryOutput>> Outputs
        {
            get => _outputs;
            set
            {
                if (_outputs == value)
                    return;
                _outputs = value;
                foreach (var output in _outputs)
                {
                    output.Value.PropertyChanged += OutputSelectionChanged;
                }

                OutputSelectionChanged(null, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public bool NeedMultiPeriod
        {
            get
            {
                return Outputs[DeflectometryOutput.UnwrappedPhase].IsSelected || Outputs[DeflectometryOutput.NanoTopo].IsSelected;
            }
        }

        public DeflectometryOutput AvailableOutputs { get; set; }

        private void OutputSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsSelected")
                return;

            SelectableItemVM<DeflectometryOutput> selectableDeflectometryOutput = null;

            if (sender is SelectableItemVM<DeflectometryOutput>)
            {
                selectableDeflectometryOutput = sender as SelectableItemVM<DeflectometryOutput>;

                int nbSelected = Outputs.Count(o => o.Value.IsSelected);
                if (nbSelected == 0)
                    selectableDeflectometryOutput.IsSelected = true;

                // UnwrappedPhase and GlobalTopo can not be selected at the same time
                /*if ((selectableDeflectometryOutput.WrappedObject == DeflectometryOutput.UnwrappedPhase) && selectableDeflectometryOutput.IsSelected)
                    Outputs[DeflectometryOutput.GlobalTopo].IsSelected = false;
                if ((selectableDeflectometryOutput.WrappedObject == DeflectometryOutput.GlobalTopo) && selectableDeflectometryOutput.IsSelected)
                    Outputs[DeflectometryOutput.UnwrappedPhase].IsSelected = false;*/
            }

            if (Outputs[DeflectometryOutput.GlobalTopo].IsSelected) // GlobalTopo
            {
                AllowStandardFringes = false;
                FringeType = FringeType.Multi; // GlobalTopo is now only allowed with multi periods.
            }
            else if (Outputs[DeflectometryOutput.UnwrappedPhase].IsSelected) // Unwrapped Phases, multi periods.
            {
                AllowStandardFringes = false;
                FringeType = FringeType.Multi;
            }
            else if
                (Outputs[DeflectometryOutput.NanoTopo].IsSelected) // NanoTopo is now only allowed with multi periods.
            {
                AllowStandardFringes = false;
                FringeType = FringeType.Multi;
            }
            else
            {
                AllowStandardFringes = true;
                if (FringeType != FringeType.Standard)
                    FringeType = FringeType.Standard;
            }

            OnPropertyChanged(nameof(Outputs));
            OnPropertyChanged(nameof(IsTuneExposureNeeded));
            OnPropertyChanged(nameof(NeedMultiPeriod));
        }

        private FringeType _fringeType;

        public FringeType FringeType
        {
            get => _fringeType;
            set
            {
                if (_fringeType != value)
                {
                    _fringeType = value;
                    Fringe.FringeType = _fringeType;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     La FringeVM sélectionnée
        /// </summary>
        public FringeVM SelectedFringeVM
        {
            get
            {
                switch (FringeType)
                {
                    case FringeType.Standard: return StandardFringe;
                    case FringeType.Multi: return MultiFringe;
                    default:
                        throw new ApplicationException("unknown FringeType: " + FringeType);
                }
            }
        }

        /// <summary>
        ///     L'objet de classe Fringe correspondant à la sélection
        /// </summary>
        public override Fringe Fringe
        {
            get => _mapper.AutoMap.Map<FringeVM, Fringe>(SelectedFringeVM);
            set
            {
                if (value == null)
                    return;
                FringeType = value.FringeType;
                switch (FringeType)
                {
                    case FringeType.Standard:
                        StandardFringe = StandardFringes.FirstOrDefault(fringeVm =>
                            fringeVm.Period == value.Period &&
                            fringeVm.NbImagesPerDirection == value.NbImagesPerDirection);
                        break;

                    case FringeType.Multi:
                        MultiFringe = _mapper.AutoMap.Map<Fringe, FringeVM>(value);
                        break;
                        //default:
                        //throw new ApplicationException("unknown FringeType: " + FringeType);
                }
            }
        }

        private bool _allowStandardFringes;

        public bool AllowStandardFringes
        {
            get => _allowStandardFringes;
            set
            {
                if (_allowStandardFringes != value)
                {
                    _allowStandardFringes = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _useEnhancedMask;

        public bool UseEnhancedMask
        {
            get => _useEnhancedMask;
            set
            {
                if (CanUseEnhancedMask)
                {
                    SetProperty(ref _useEnhancedMask, value);    
                }
            }
        }

        private bool _canUseEnhancedMask;

        public bool CanUseEnhancedMask
        {
            get => _canUseEnhancedMask;
            set
            {
                SetProperty(ref _canUseEnhancedMask, value);
                if (!_canUseEnhancedMask)
                {
                    UseEnhancedMask = false;
                };
            }
        }

        #endregion Propriétés bindables communes

        //=================================================================

        #region Propriétés bindables pour les franges standards

        //=================================================================
        public List<FringeVM> StandardFringes { get; }

        private FringeVM _standardFringe;

        public FringeVM StandardFringe
        {
            get => _standardFringe;
            set
            {
                if (_standardFringe == value)
                    return;
                _standardFringe = value;
                if (value == null)
                {
                    Period = "N/A";
                    StandardNbImagesPerDirection = "N/A";
                }
                else
                {
                    PhaseShiftPrecision = value.PhaseShiftPrecision;
                    SlopePrecision = value.SlopePrecision;
                    Period = value.Period.ToString();
                    StandardNbImagesPerDirection = value.NbImagesPerDirection.ToString();
                }

                OnPropertyChanged();
            }
        }

        private int _slopePrecision;

        public int SlopePrecision
        {
            get => _slopePrecision;
            set
            {
                if (_slopePrecision != value)
                {
                    _slopePrecision = value;
                    OnPropertyChanged();
                    SelectStandardFringe();
                }
            }
        }

        private int _maxSlopePrecision;

        public int MaxSlopePrecision
        {
            get => _maxSlopePrecision;
            set
            {
                if (_maxSlopePrecision != value)
                {
                    _maxSlopePrecision = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _slopePrecisionTickText;

        public string SlopePrecisionTickText
        {
            get => _slopePrecisionTickText;
            set
            {
                if (_slopePrecisionTickText != value)
                {
                    _slopePrecisionTickText = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _phaseShiftPrecision;

        public int PhaseShiftPrecision
        {
            get => _phaseShiftPrecision;
            set
            {
                if (_phaseShiftPrecision != value)
                {
                    _phaseShiftPrecision = value;
                    OnPropertyChanged();
                    SelectStandardFringe();
                }
            }
        }

        private int _maxPhaseShiftPrecision;

        public int MaxPhaseShiftPrecision
        {
            get => _maxPhaseShiftPrecision;
            set
            {
                if (_maxPhaseShiftPrecision != value)
                {
                    _maxPhaseShiftPrecision = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _phaseShiftPrecisionTickText;

        public string PhaseShiftPrecisionTickText
        {
            get => _phaseShiftPrecisionTickText;
            set
            {
                if (_phaseShiftPrecisionTickText != value)
                {
                    _phaseShiftPrecisionTickText = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _curvatureDynamic;

        public double CurvatureDynamic
        {
            get => _curvatureDynamic;
            set
            {
                if (_curvatureDynamic != value)
                {
                    _curvatureDynamic = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _darkDynamic;

        public double DarkDynamic
        {
            get => _darkDynamic;
            set
            {
                if (_darkDynamic != value)
                {
                    _darkDynamic = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _unwrappedDynamic;

        public double UnWrappedDynamic
        {
            get => _unwrappedDynamic;
            set
            {
                if (_unwrappedDynamic != value)
                {
                    _unwrappedDynamic = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _period;

        public string Period
        {
            get => _period;
            set
            {
                if (_period != value)
                {
                    _period = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsTuneExposureNeeded));
                }
            }
        }

        private string _standardNbImagesPerDirection;

        public string StandardNbImagesPerDirection
        {
            get => _standardNbImagesPerDirection;
            set
            {
                if (_standardNbImagesPerDirection != value)
                {
                    _standardNbImagesPerDirection = value;
                    OnPropertyChanged();
                }
            }
        }

        // Selectes the standard fringe corresponding to the slopePrecion and the phaseShiftPrecision
        private void SelectStandardFringe()
        {
            StandardFringe = StandardFringes.FirstOrDefault(x =>
                x.SlopePrecision == SlopePrecision && x.PhaseShiftPrecision == PhaseShiftPrecision);
        }

        #endregion Propriétés bindables pour les franges standards

        //=================================================================

        #region Propriétés bindables pour les franges multiples

        //=================================================================

        private FringeVM _multiFringe;

        public FringeVM MultiFringe
        {
            get => _multiFringe;
            set
            {
                if (_multiFringe != value)
                {
                    _multiFringe = value;
                    if (value.Period != 0)
                    {
                        _multiSlopePrecision = value.Period;
                        OnPropertyChanged(nameof(MultiSlopePrecision));
                        var index = _multiSlopePrecisionList.IndexOf(value.Period);
                        if (index != -1)
                        {
                            _multiSlopePrecisionIndex = index;
                            OnPropertyChanged(nameof(MultiSlopePrecisionIndex));
                        }
                        InitializeAvailablePeriods();
                        Periods = String.Join("-", value.Periods);
                        MultiPeriodIndex = AvailablePeriods.Keys.ToList().IndexOf(Periods);
                        MultiPeriodRatio = AvailablePeriods[Periods];
                        MultiPeriodRatioTickText = BuildMultiPeriodRatioTickText(AvailablePeriods.Count - 1);
                        var imageIndex =
                            _multiNbImagePerDirectionList.FindIndex(nbImages => nbImages == value.NbImagesPerDirection);
                        MultiNbImagePerDirectionIndex = imageIndex;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private List<int> _multiSlopePrecisionList;

        public List<int> MultiSlopePrecisionList
        {
            get => _multiSlopePrecisionList;
            set
            {
                if (value != _multiSlopePrecisionList)
                {
                    _multiSlopePrecisionList = value;
                    MultiSlopePrecisionTickText = BuildSlopePrecisionTickText(value.Count - 1);
                    OnPropertyChanged();
                }
            }
        }

        private List<int> _multiNbImagePerDirectionList;

        private int _multiSlopePrecision;

        public int MultiSlopePrecision
        {
            get => _multiSlopePrecision;
            set
            {
                if (value != _multiSlopePrecision)
                {
                    _multiSlopePrecision = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _multiSlopePrecisionIndex;

        public int MultiSlopePrecisionIndex
        {
            get => _multiSlopePrecisionIndex;
            set
            {
                if (_multiSlopePrecisionIndex != value)
                {
                    if (null == _screenInfo)
                    {
                        _screenInfo = _screenSupervisor.GetScreenInfo(Side);
                    }

                    _multiSlopePrecisionIndex = value;
                    _multiSlopePrecision = _multiSlopePrecisionList[value];
                    InitializeAvailablePeriods();
                    MultiPeriodRatioTickText = BuildMultiPeriodRatioTickText(AvailablePeriods.Count - 1);
                    if (AvailablePeriods.Count > 0 && _multiPeriodIndex > AvailablePeriods.Count - 1)
                    {
                        MultiPeriodIndex = AvailablePeriods.Count - 1;
                    }
                    else
                    {
                        Periods = AvailablePeriods.Keys.ElementAt(MultiPeriodIndex);
                        MultiPeriodRatio = AvailablePeriods[Periods];
                        OnPropertyChanged(nameof(Periods));
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MultiSlopePrecision));
                    OnPropertyChanged(nameof(AvailablePeriods));
                }
            }
        }

        private Dictionary<string, int> _availablePeriods;

        private void InitializeAvailablePeriods()
        {
            var maxPeriod = _screenInfo.Width * 10;
            var currentPeriod = _multiSlopePrecisionList[_multiSlopePrecisionIndex];
            _availablePeriods = Enumerable.Range(1, 20)
                .TakeWhile(ratio => currentPeriod * ratio < maxPeriod)
                .Select(ratio => new KeyValuePair<List<int>, int>(DMTDeflectometryMeasureHelper.CreatePeriodsListForRatio(currentPeriod, _screenInfo.Width, _screenInfo.Height, ratio), ratio))
                .Where(kvPair => kvPair.Key.Count > 0)
                .Select(kvPair => new KeyValuePair<string, int>(String.Join("-", kvPair.Key), kvPair.Value))
                .ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);
            if (_availablePeriods.Count > 1)
            {
                _availablePeriods.Remove(_multiSlopePrecisionList[_multiSlopePrecisionIndex].ToString());
            }
        }

        public Dictionary<string, int> AvailablePeriods
        {
            get => _availablePeriods;
        }

        private string _multiSlopePrecisionTickText;

        public string MultiSlopePrecisionTickText
        {
            get => _multiSlopePrecisionTickText;
            set
            {
                if (value != _multiSlopePrecisionTickText)
                {
                    _multiSlopePrecisionTickText = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _multiPeriodIndex;

        public int MultiPeriodIndex
        {
            get => _multiPeriodIndex;
            set
            {
                if (value != _multiPeriodIndex)
                {
                    _multiPeriodIndex = value;
                    Periods = AvailablePeriods.Keys.ElementAt(value);
                    MultiPeriodRatio = AvailablePeriods[Periods];
                    OnPropertyChanged();
                }
            }
        }

        private string _periods;

        public string Periods
        {
            get => _periods;
            set
            {
                if (_periods != value)
                {
                    _periods = value;
                    MultiFringe.Period = _multiSlopePrecisionIndex;
                    MultiFringe.Periods = _periods.Split('-').Select(periodString => Int32.Parse(periodString)).ToList();
                    OnPropertyChanged();
                }
            }
        }

        private int _multiPeriodRatio;

        public int MultiPeriodRatio
        {
            get => _multiPeriodRatio;
            set
            {
                if (value != _multiPeriodRatio)
                {
                    _multiPeriodRatio = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _multiNbImagePerDirectionIndex;

        public int MultiNbImagePerDirectionIndex
        {
            get => _multiNbImagePerDirectionIndex;
            set
            {
                if (value != _multiNbImagePerDirectionIndex)
                {
                    _multiNbImagePerDirectionIndex = value;
                    MultiNbImagesPerDirection = _multiNbImagePerDirectionList[value];
                    OnPropertyChanged();
                }
            }
        }

        private int _multiNbImagesPerDirection;

        public int MultiNbImagesPerDirection
        {
            get => _multiNbImagesPerDirection;
            set
            {
                if (value != _multiNbImagesPerDirection)
                {
                    _multiNbImagesPerDirection = value;
                    MultiFringe.NbImagesPerDirection = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _multiPeriodRatioTickText;

        public string MultiPeriodRatioTickText
        {
            get => _multiPeriodRatioTickText;
            set
            {
                if (value != _multiPeriodRatioTickText)
                {
                    _multiPeriodRatioTickText = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Propriétés bindables pour les franges multiples

        //=================================================================

        #region Commandes

        //=================================================================
        private AutoRelayCommand _adjustCurvatureDynamicCommand;

        public AutoRelayCommand AdjustCurvatureDynamicCommand =>
            _adjustCurvatureDynamicCommand ?? (_adjustCurvatureDynamicCommand = new AutoRelayCommand(
                () =>
                {
                    var vm = new AdjustDynamicsVM(_recipeSupervisor, _dialogService, _mapper);
                    vm.MeasureVM = this;
                    if (Outputs[DeflectometryOutput.LowAngleDarkField].IsSelected && !Outputs[DeflectometryOutput.Curvature].IsSelected)
                    {
                        vm.Dynamic = DynamicsType.Dark;
                    }

                    _mainRecipeEditionVM.EditedRecipe.Navigate(vm);
                }));

        private int alreadyTunedExposure = -1;

        public bool IsTuneExposureNeeded => alreadyTunedExposure != GetTuneExposurePeriod();

        private int GetTuneExposurePeriod()
        {
            if (_screenInfo is null)
                _screenInfo = _screenSupervisor.GetScreenInfo(Side);

            if (Outputs[DeflectometryOutput.UnwrappedPhase].IsSelected ||
                Outputs[DeflectometryOutput.NanoTopo].IsSelected || Fringe.Period > _screenInfo.Height / 2)
                return 0; // White screen

            return Fringe.Period; // Fringe screen
        }

        private AutoRelayCommand<string> _tuneExposureTimeCommand;

        public AutoRelayCommand<string> TuneExposureTimeCommand =>
            _tuneExposureTimeCommand ?? (_tuneExposureTimeCommand = new AutoRelayCommand<string>(
                title =>
                {
                    var vm = new ManualExposureSettingsVMForDeflectometry(title, this, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor,
                                                                            _algorithmsSupervisor, _dialogService, _mapper, _mainRecipeEditionVM);
                    alreadyTunedExposure = GetTuneExposurePeriod();
                    OnPropertyChanged(nameof(IsTuneExposureNeeded));
                    _mainRecipeEditionVM.EditedRecipe.Navigate(vm);
                }));

        #endregion Commandes
    }
}
