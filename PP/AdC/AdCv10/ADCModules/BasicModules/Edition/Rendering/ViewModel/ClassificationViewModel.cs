using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using ADCEngine;

using BasicModules.Edition.Rendering.ViewModel;

namespace BasicModules.Edition.Rendering
{
    /// <summary>
    /// ViewModel to display all the defect by class
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ClassificationViewModel : RenderingViewModelBase
    {
        private System.Windows.Threading.DispatcherTimer _refreshTimer;
        private object _lock = new object();
        private TimeSpan _refreshTimeSpan = new TimeSpan(0, 0, 0, 1, 100);
        private List<DefectViewModel> _defectDisplayBuffer = new List<DefectViewModel>();

        public ClassificationViewModel(ModuleBase module) : base(module)
        {
            InitWaferProfile();
        }

        /// <summary>
        /// List of classes to class defect
        /// </summary>
        public ObservableCollection<ClassViewModel> Classes { get; private set; }

        /// <summary>
        /// List of defects for the selected classes
        /// </summary>
        public ObservableCollection<DefectViewModel> Defects { get; private set; }

        /// <summary>
        /// Wafer height in zoombox
        /// </summary>
        private double _waferHeight = 10000;
        public double WaferHeight
        {
            get => _waferHeight; set { if (_waferHeight != value) { _waferHeight = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// If false : Force la mise à jour de l'affichage de tous les défauts sur la wafer
        /// </summary>
        private bool _waferIsUpToDate = true;
        public bool WaferIsUpToDate
        {
            get => _waferIsUpToDate;
            set
            {
                _waferIsUpToDate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// <summary>
        /// Wafer width in zoombox
        /// </summary>
        private double _waferWidth = 10000;
        public double WaferWidth
        {
            get => _waferWidth; set { if (_waferWidth != value) { _waferWidth = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Real wafer Height
        /// </summary>
        private double _realWaferHeightmm;
        public double RealWaferHeightmm
        {
            get => _realWaferHeightmm; set { if (_realWaferHeightmm != value) { _realWaferHeightmm = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Real wafer width
        /// </summary>
        private double _realWaferWidthmm;
        public double RealWaferWidthmm
        {
            get => _realWaferWidthmm; set { if (_realWaferWidthmm != value) { _realWaferWidthmm = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// True if the wafer is square
        /// </summary>
        private bool _isSquareWafer;
        public bool IsRectangularWafer
        {
            get => _isSquareWafer; set { if (_isSquareWafer != value) { _isSquareWafer = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Wafer margin in zoombox
        /// </summary>
        private int _waferMargin = 300;
        public int WaferMargin
        {
            get => _waferMargin; set { if (_waferMargin != value) { _waferMargin = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Pixel size for the wafer in the ZoomBox
        /// </summary>
        public double viewPixelSizeX = 10.0;
        public double viewPixelSizeY = 10.0;


        /// <summary>
        /// Magnification factor
        /// </summary>
        private int _defectFactorSize = 1;
        public int DefectFactorSize
        {
            get => _defectFactorSize; set { if (_defectFactorSize != value) { _defectFactorSize = value; OnPropertyChanged(); DefectSizeChange(); } }
        }

        /// <summary>
        /// Minimun defect size
        /// </summary>
		private int _defectMinSize = 2000;
        public int DefectMinSize
        {
            get => _defectMinSize; set { if (_defectMinSize != value) { _defectMinSize = value; OnPropertyChanged(); DefectSizeChange(); } }
        }

        /// <summary>
        /// Update defect when DefectFactorSize or DefectMinSize change
        /// </summary>
		private void DefectSizeChange()
        {
            foreach (var defect in Defects.Where(x => x.IsVisible))
            {
                UpdateDefectSize(defect);
            }

            RefreshUI();
        }

        /// <summary>
        /// Numbers of defects
        /// </summary>
        public int NbDefects => Defects != null ? Defects.Count(x => x.IsVisible) : 0;

        /// <summary>
        /// True if at leat one class is selected
        /// </summary>
        public bool ClassesAreSelected
        {
            get
            {
                if (!Classes.Any())
                    return false;
                else
                    return Classes.All(x => x.IsSelected);
            }

            set
            {
                foreach (var currentClass in Classes)
                {
                    currentClass.SelectWithoutNotification(value);
                }

                RefreshUI();
            }
        }


        /// <summary>
        /// Init the ViewModel
        /// </summary>
        public void Init(List<ClassDefectResult> resultClasses)
        {
            Classes = new ObservableCollection<ClassViewModel>();
            InitClasses(resultClasses);
            Defects = new ObservableCollection<DefectViewModel>();
            OnPropertyChanged(nameof(Defects));
            UpdateClassColor();
            RefreshUI();
        }

        /// <summary>
        /// Init defect classes
        /// </summary>
		private void InitClasses(List<ClassDefectResult> classes)
        {
            Classes.Clear();
            foreach (var classDefect in classes)
            {
                Classes.Add(new ClassViewModel(this, classDefect.ClassName, true, classDefect.RoughBinNum));
            }
            UpdateClassColor();
            OnPropertyChanged(nameof(ClassesAreSelected));
        }

        /// <summary>
        /// Refresh UI 
        /// </summary>
        public void RefreshUI()
        {
            if (Defects != null)
            {
                List<ClassViewModel> selectedClasses = Classes.Where(x => x.IsSelected).ToList();

                foreach (var defect in Defects)
                {
                    defect.IsVisible = selectedClasses.Contains(defect.ClassVM);
                }
            }
            OnPropertyChanged(nameof(ClassesAreSelected));
            OnPropertyChanged(nameof(NbDefects));
            WaferIsUpToDate = false;
        }

        private List<System.Windows.Media.Color> _colorList;
        private List<System.Windows.Media.Color> ColorList
        {
            get
            {
                if (_colorList == null)
                {
                    _colorList = new List<System.Windows.Media.Color>();
                    for (int i = 1; i < 6; i++)
                        _colorList.Add(((System.Windows.Media.Color)Application.Current.FindResource(string.Format("Class{0}DefectColor", i))));

                    Type type = typeof(System.Windows.Media.Colors);
                    var list = new List<System.Windows.Media.Color>(
                        from p in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                        where typeof(System.Windows.Media.Color).IsAssignableFrom(p.PropertyType)
                        select (System.Windows.Media.Color)p.GetValue(null)
                        );
                    _colorList.AddRange(list);
                }
                return _colorList;
            }
        }

        /// <summary>
        /// Update the class with some color define in the resouces dictionnary
        /// </summary>
        public void UpdateClassColor()
        {
            System.Windows.Media.Color emptyColor = new System.Windows.Media.Color();
            foreach (ClassViewModel classVm in Classes.Where(x => x.Color.Equals(emptyColor)))
            {
                // First available color
                foreach (var color in ColorList)
                {
                    if (!Classes.Any(x => x.Color.Equals(color)))
                    {
                        classVm.Color = color;
                        break;
                    }
                }
                if (classVm.Color.A == 0)
                    classVm.Color = System.Windows.Media.Colors.Red;
            }
        }

        /// <summary>
        /// Update defect size : Real size / pixel size
        /// </summary>o
        /// <param name="defectVM"></param>
		private void UpdateDefectSize(DefectViewModel defectVM)
        {
            defectVM.Width = Math.Max(defectVM.MicronRect.Width * DefectFactorSize, DefectMinSize) / viewPixelSizeX;
            defectVM.Height = Math.Max(defectVM.MicronRect.Height * DefectFactorSize, DefectMinSize) / viewPixelSizeY;
            defectVM.TopPosition = WaferHeight / 2.0 - (defectVM.MicronRect.Y / viewPixelSizeY) - defectVM.Height;
            defectVM.LeftPosition = WaferWidth / 2.0 + (defectVM.MicronRect.X / viewPixelSizeX);
        }

        public void AddDefect(DefectResult defect)
        {
            lock (_lock)
            {
                ClassViewModel classVM = GetClassVM(defect);
                if (classVM == null)
                    classVM = null;
                DefectViewModel newDefect = new DefectViewModel(defect, classVM);
                UpdateDefectSize(newDefect);
                newDefect.IsVisible = true;
                _defectDisplayBuffer.Add(newDefect);
                if (_refreshTimer == null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _refreshTimer = new System.Windows.Threading.DispatcherTimer();
                        _refreshTimer.Interval = _refreshTimeSpan;
                        _refreshTimer.Tick += TimerRendering_Tick;
                        _refreshTimer.Start();
                    });
                }
            }
        }

        private ClassViewModel GetClassVM(DefectResult resultDefect)
        {
            return Classes.FirstOrDefault(x => x.ClassName == resultDefect.ClassName);
        }

        private void TimerRendering_Tick(object sender, EventArgs e)
        {
            List<DefectViewModel> defectToDisplay;
            lock (_lock)
            {
                defectToDisplay = _defectDisplayBuffer.ToList();
                _defectDisplayBuffer.Clear();
            }

            if (defectToDisplay.Any())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var defect in defectToDisplay)
                    {
                        if (defect.ClassVM != null)
                            defect.ClassVM.Defects.Add(defect);
                        Defects.Add(defect);
                    }

                    foreach (var currentClass in Classes)
                    {
                        currentClass.UpdateNbDefects();
                    }
                });
            }
        }

        public void InitWaferProfile()
        {
            WaferBase waferBase = Module.Recipe.Wafer;
            WaferHeight = waferBase.SurroundingRectangle.Height / viewPixelSizeY;
            WaferWidth = waferBase.SurroundingRectangle.Width / viewPixelSizeX;
            RealWaferHeightmm = waferBase.SurroundingRectangle.Height / 1000;
            RealWaferWidthmm = waferBase.SurroundingRectangle.Width / 1000;
            IsRectangularWafer = waferBase is RectangularWafer;
        }

        public virtual void Clean()
        {
            if (_refreshTimer != null)
                _refreshTimer.Tick -= TimerRendering_Tick;
        }
    }
}
