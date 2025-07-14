using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper.Internal;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using LightningChartLib.WPF.ChartingMVVM;
using LightningChartLib.WPF.ChartingMVVM.Axes;
using LightningChartLib.WPF.ChartingMVVM.SeriesXY;
using LightningChartLib.WPF.ChartingMVVM.Views.ViewXY;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Display.ASO;
using UnitySC.Shared.Format.ASO;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Defect;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Helper;

namespace UnitySC.Shared.ResultUI.ASO.ViewModel
{
    public class AsoResultVM : DefectCategoriesVM
    {
        private const ResultFormat FormatToken = ResultFormat.ASO;

        #region Fields

        private readonly ImageZoneVM _imageZone = new ImageZoneVM();

        private readonly IMessenger _messenger;

        private readonly object _defthumbsSync = new object();

        private CancellationTokenSource _readTokenSource;

        #endregion

        #region Properties

        public override string FormatName => "ASO";

        private bool _hasthumbnails;
        public bool HasThumbnails
        {
            get => _hasthumbnails;
            set => SetProperty(ref _hasthumbnails, value);
        }

        private int _totalCountSelected;
        public override int TotalCountSelected
        {
            get => _totalCountSelected;
            set
            {
                if (_totalCountSelected == value) return;
                _totalCountSelected = value;
                OnPropertyChanged();
            }
        }

        private int _totalActiveDefects;
        public override int ActiveDefects
        {
            get => _totalActiveDefects;
            set => SetProperty(ref _totalActiveDefects, value);
        }

        private List<string> _selectedCategories;
        public List<string> SelectedCategories
        {
            get { return _selectedCategories; }
            set { SetProperty(ref _selectedCategories, value); }
        }

        private bool _isDisplayGrid;
        public bool IsDisplayGrid
        {
            get => _isDisplayGrid;
            set => SetProperty(ref _isDisplayGrid, value);
        }

        private bool _isDisplayGridEnabled;
        public bool IsDisplayGridEnabled
        {
            get => _isDisplayGridEnabled;
            set => SetProperty(ref _isDisplayGridEnabled, value);
        }

        private int _displayMinSize;
        public int AsoDisplayMinSize
        {
            get => _displayMinSize;
            set
            {
                if (SetProperty(ref _displayMinSize, value))
                {
                    ResultDisplay?.UpdateInternalDisplaySettingsPrm(_displayFactor, _displayMinSize);

                    IsBusy = true;
                    //need to delayed action not to overflow with task when button stay pushed

                    Task.Run(() => DrawWaferImage(_readTokenSource.Token));

                    if (Defects?.SelectedDefect != null)
                    {
                        SetRectHilite(((AsoDefectVM)Defects.SelectedDefect).AsoDefectObj);
                    }

                    IsBusy = false;
                }
            }
        }

        private double _displayFactor;
        public double AsoDisplayFactor
        {
            get => _displayFactor;
            set
            {
                if (SetProperty(ref _displayFactor, value))
                {
                    ResultDisplay?.UpdateInternalDisplaySettingsPrm(_displayFactor, _displayMinSize);
                    IsBusy = true;
                    //need to delayed actions not to overflow with task when button stay pushed

                    Task.Run(() => DrawWaferImage(_readTokenSource.Token));

                    if (Defects != null && Defects.SelectedDefect != null)
                    {
                        SetRectHilite(((AsoDefectVM)Defects.SelectedDefect).AsoDefectObj);
                    }

                    IsBusy = false;
                }
            }
        }

        private ObservableCollection<DefectCategoryVM> _defectCategories;
        public override ObservableCollection<DefectCategoryVM> DefectCategories
        {
            get { return _defectCategories; }
            set => SetProperty(ref _defectCategories, value);
        }

        // VM de la vue des imagettes
        private ThumbnailsDefectsVM _defects;
        public ThumbnailsDefectsVM Defects
        {
            get => _defects;
            set => SetProperty(ref _defects, value);
        }

        // Current lot view for container presenter.
        private ObservableRecipient _waferzoneVM;
        public ObservableRecipient WaferZoneVM
        {
            get => _waferzoneVM;
            set => SetProperty(ref _waferzoneVM, value);
        }

        private int _maxIdDefect;
        public int MaxIdDefect
        {
            get => _maxIdDefect;
            set => SetProperty(ref _maxIdDefect, value);
        }

        private int _minIdDefect;
        public int MinIdDefect
        {
            get => _minIdDefect;
            set => SetProperty(ref _minIdDefect, value);
        }

        private int _foundDefectsbyClusterNum;
        public int FoundDefectsbyClusterNum
        {
            get => _foundDefectsbyClusterNum;
            set => SetProperty(ref _foundDefectsbyClusterNum, value);
        }

        private bool _isSearchClusterDone;
        public bool IsSearchClusterDone
        {
            get => _isSearchClusterDone;
            set => SetProperty(ref _isSearchClusterDone, value);
        }
        #endregion Properties


        #region Constructor
        public AsoResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            DefectCategories = new ObservableCollection<DefectCategoryVM>();
            SelectedCategories = new List<string>();

            _customAxis = new CustomAxisTickCollection();
            _histogramBars = new BarSeriesCollection();

            _waferzoneVM = _imageZone;

            OnUpdateCategoriesCommand = new AutoRelayCommand<DefectCategoryVM>(category => UpdateCategoriesCommand(category));
        }
        #endregion Constructor

        #region Categories selection
        public override AutoRelayCommand<DefectCategoryVM> OnUpdateCategoriesCommand { get; }
        private void UpdateCategoriesCommand(DefectCategoryVM category)
        {
            SelectedCategories.Clear();

            if (category?.LabelCategory == "All")
            {
                DefectCategories.ForAll(_ => _.IsSelected = category.IsSelected);

                category.NbDefects = category.IsSelected ? TotalCountSelected : 0;
            }

            foreach (var item in DefectCategories)
            {
                if (item.IsSelected)
                {
                    SelectedCategories.Add(item.LabelCategory);
                }
            }

            Defects.SelectedCategories = SelectedCategories;

            UpdateSelectedDefect(true);

            Task.Run(() => DrawWaferImage(_readTokenSource.Token));
        }
        #endregion

        #region Methods

        #region Selections
        private void SelectDefectByClick(System.Windows.Point pt)
        {
            if (!IsBusy)
            {
                // update wafer image
                IsBusy = true;
                Task.Run(() => SearchDefectByPosition(pt));
            }
        }


        /// <summary>
        /// Gestion de la selection de la vue wafer et imagettes.
        /// </summary>
        private int _selectedZoneViewIndex;
        public int SelectedZoneViewIndex
        {
            get => _selectedZoneViewIndex;
            set
            {
                _selectedZoneViewIndex = value;
                OnPropertyChanged();

                IsBusy = false;

                IsDisplayGrid = _selectedZoneViewIndex == 1;

                if (_selectedZoneViewIndex == 0)
                {
                    WaferZoneVM = _imageZone;
                }
                else
                {
                    // Activer la selection de l'imagette
                    Defects.UpdateSelectedThumbnail();

                    WaferZoneVM = Defects;
                }
            }
        }
        #endregion Selections


        private void ResetView()
        {
            _imageZone.WaferImageSource = null;
            _imageZone.ShowHilite = false;
            SelectedZoneViewIndex = 0;

            DefectCategories.Clear();
            SelectedCategories.Clear();

            FoundDefectsbyClusterNum = 0;
            IsDisplayGrid = false;
            IsDisplayGridEnabled = false;

            HistogramBars.Clear();
            CustomAxis.Clear();
            AxisYMin = -0.001;
            AxisYMax = 0.001;
            AxisXMin = -0.001;
            AxisXMax = 0.001;
        }


        private void UpdateRectHilite(UpdateRectHiliteMessage updateRectHilite)
        {
            if (updateRectHilite.DefectRect != null)
            {
                SetRectHilite((AsoDefect)updateRectHilite.DefectRect);
            }
            ShowHilite(updateRectHilite.Show);
        }
        private void SetRectHilite(AsoDefect defect)
        {
            if (defect != null)
            {
                var rc = Rectangle.Round(defect.ApplyModifiers((float)AsoDisplayFactor, AsoDisplayMinSize));
                _imageZone.HiliteRect = new Int32Rect(rc.X, rc.Y, rc.Width, rc.Height);
            }
        }
        private void ShowHilite(bool bShow)
        {
            _imageZone.ShowHilite = bShow;
        }


        public override async void UpdateResData(IResultDataObject resdataObj)
        {
            // Unregister previous messages
            Clean();

            IsBusy = true;
            _readTokenSource = new CancellationTokenSource();

            System.Diagnostics.Debug.WriteLine($"ASO UpdateResData[{resdataObj.DBResId}] {Path.GetFileName(resdataObj.ResFilePath)}");

            _messenger.Register<ResultsDisplayChangedMessage, string>(this, ResultType.ADC_ASO.ToString(), (r, m) => Clean());
            _imageZone.OnDoubleClick += SelectDefectByClick;

            // Draw area containing the selected defect 
            _messenger.Register<UpdateRectHiliteMessage, string>(this, FormatToken.ToString(), (r, m) => UpdateRectHilite(m));

            // Commands from DefectInfoView (defect detail management)
            _messenger.Register<DefectActionMessage, string>(this, FormatToken.ToString(), (r, m) => UpdateAsoDefect(m));

            ResultDataObj = resdataObj;

            var dataAso = ResultDataObj as DataAso;
            var resAsoDisplay = ResultDisplay as AsoDisplay;

            // do not use normal property set to avoid UpdateInternalDisplaySettingsPrm call and refresh image
            _displayMinSize = (int)resAsoDisplay.DisplayMinSize;
            OnPropertyChanged(nameof(AsoDisplayMinSize));

            _displayFactor = resAsoDisplay.DisplayFactor;
            OnPropertyChanged(nameof(AsoDisplayFactor));

            if (dataAso == null)
            {
                return;
            }

            lock (_defthumbsSync)
            {
                Defects = new ThumbnailsDefectsVM();
            }

            ResetView();

            //update all attribute and redraw image
            IsDisplayGridEnabled = dataAso.UseDieGridDisplay;
            IsDisplayGrid = IsDisplayGridEnabled; // a voir si on garde le statut dans le display

            // generate histogram and categories
            GenerateHistoAndCategories(dataAso);

            var allCategory = DefectCategories.FirstOrDefault(_ => _.LabelCategory == "All");
            if (allCategory != null)
            {
                allCategory.NbDefects = DefectCategories.Sum(_ => _.NbDefects);
            }

            Defects.SelectedCategories = SelectedCategories;

            await Task.Run(() => DrawWaferImage(_readTokenSource.Token, true));

            if (dataAso.DefectViewItemList == null)
            {
                return;
            }

            if (dataAso.DefectViewItemList.ItemList != null)
            {
                TotalCountSelected = dataAso.DefectViewItemList.ItemList.Count();
                ActiveDefects = TotalCountSelected;

                if (TotalCountSelected > 0)
                {
                    MaxIdDefect = dataAso.DefectViewItemList.ItemList.Max(x => x.DefectId);
                    MinIdDefect = dataAso.DefectViewItemList.ItemList.Min(x => x.DefectId);
                }

                if (DefectCategories.Any())
                {
                    var IdsSelectedCategories = DefectCategories.Where(x => x.IsSelected).Select(x => x.LabelCategory);
                    var orderedDefects = dataAso.DefectViewItemList.ItemList.OrderBy(_ => _.DefectId);

                    // Recuperer le premier defaut des categories actives et l'afficher
                    var defectobj = orderedDefects.FirstOrDefault(x => IdsSelectedCategories.Contains(x.DefectCategory));
                    if (defectobj != null)
                    {
                        // Defect thumbnail view
                        Defects.SelectedDefect = new AsoDefectVM(this);
                        Defects.SelectedDefect.Change(defectobj);
                    }
                }

                HasThumbnails = !dataAso.HasNoThumbnails;
                if (HasThumbnails)
                {
                    string namTask = Path.GetFileNameWithoutExtension(ResultDataObj.ResFilePath);
                    System.Diagnostics.Debug.WriteLine($"ReadThumbnails ASO Start [{namTask}]..");

                    await Task.Run(() => LoadThumbnails(_readTokenSource.Token, dataAso));
                }
            }

            IsBusy = false;

            System.Diagnostics.Debug.WriteLine($"~ASO UpdateResData[{resdataObj.DBResId}] {Path.GetFileName(resdataObj.ResFilePath)} Exit ");
        }

        public bool LoadThumbnails(CancellationToken token, DataAso data)
        {
            var orderedDefects = data.DefectViewItemList.ItemList.OrderBy(_ => _.DefectId);
            Defects.TotalThumbnails = orderedDefects.Count();

            int index = 0;

            foreach (var defect in orderedDefects)
            {
                if (token != null && token.IsCancellationRequested)
                {
                    return false;
                }

                int defectId = defect.DefectId;
                string defectCategory = defect.DefectCategory;
                var defcolor = defect.Color;
                index++;

                Defects.UpdateProgressBar(index * 100 / Defects.TotalThumbnails);

                if (!string.IsNullOrEmpty(defect.Defect.ThumbnailGreyLevelFilePath))
                {
                    string sFullpathGrey = data.LocalPath + defect.Defect.ThumbnailGreyLevelFilePath;
                    if (File.Exists(sFullpathGrey))
                    {
                        var thumbnail = new Thumbnail(sFullpathGrey, defectId, defectCategory, defcolor, index);

                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            Defects.ActiveThumbnails.Add(thumbnail);
                        });
                    }
                }

                if (!string.IsNullOrWhiteSpace(defect.Defect.ThumbnailGreyLevelFilePath))
                {
                    string sFullpathBin = data.LocalPath + defect.Defect.ThumbnailBinaryFilePath;
                    if (File.Exists(sFullpathBin))
                    {
                        var thumbnail = new Thumbnail(sFullpathBin, defectId, defectCategory, defcolor, index);

                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            Defects.ActiveThumbnails.Add(thumbnail);
                        });
                    }
                }

                if (index == 1)
                {
                    _messenger.Send(new DefectActionMessage(DefectActionEnum.SelectAndDisplayFirstDefect), FormatToken.ToString());
                }
            }

            string namTask = Path.GetFileNameWithoutExtension(data.ResFilePath);
            System.Diagnostics.Debug.WriteLine($"ReadThumbnails ASO DONE [{namTask}]...");

            return true;
        }

        // Bitmap to load wafer image
        private Bitmap _waferBitmap;
        private bool DrawWaferImage(CancellationToken cancellationToken, bool bFirstDraw = false)
        {
            try
            {
                if (cancellationToken == null)
                {
                    return false;
                }

                if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                var lCat = bFirstDraw ? null : SelectedCategories;

                _waferBitmap = ResultDisplay.DrawImage(ResultDataObj, IsDisplayGrid, lCat);

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    if (_waferBitmap != null)
                    {
                        _imageZone.WaferImageSource = ImageHelper.ConvertToBitmapSource(_waferBitmap);
                    }
                });

                _waferBitmap?.Dispose();
                _waferBitmap = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // Update defect in DefectInfoView
        private void UpdateAsoDefect(DefectActionMessage asoDefectTO)
        {
            switch (asoDefectTO.Action)
            {
                // Update current defect in defectDetailView
                case DefectActionEnum.ChangeCurrentDefect:
                    var asodefect = GetDefect(asoDefectTO.Id);
                    (Defects.SelectedDefect).Change(asodefect);
                    break;

                // Navigation command to required defect
                case DefectActionEnum.NavigateToDefect:
                    UpdateSelectedDefect();
                    break;

                // Search command to required defect id
                case DefectActionEnum.SearchDefectByClusterNumber:
                    SearchDefectByClusterNumber(asoDefectTO.Id);
                    break;

                case DefectActionEnum.SelectAndDisplayFirstDefect:
                    UpdateSelectedDefect(true);
                    break;

                default:
                    break;
            }
        }


        /// <summary>
        /// Met à jour le defaut selectionné.
        /// </summary>
        /// <param name="categSelected"></param>
        public void UpdateSelectedDefect(bool categSelected = false)
        {
            if (DefectCategories == null)
            {
                return; // initialization not finalize
            }

            var data = ResultDataObj as DataAso;
            AsoDefect defect = null;

            var selectedDefect = (AsoDefectVM)Defects.SelectedDefect;

            // Liste des catégories selectionnées
            var IdsSelectedCategories = DefectCategories.Where(x => x.IsSelected).Select(x => x.LabelCategory);

            if (data.DefectViewItemList == null || !data.DefectViewItemList.ItemList.Any() || selectedDefect == null)
                return;

            var activesDefects = data.DefectViewItemList.ItemList.Where(x => IdsSelectedCategories
                                                                 .Contains(x.DefectCategory))
                                                                 .OrderBy(x => x.DefectId);

            // Selectionner uniquement les imagettes dont leurs categories de defauts sont actives
            if (categSelected)
            {
                //ActivesThumbnails = Defects.Thumbnails.Where(x => IdsSelectedCategories.Contains(x.LabelCategory)).ToList();

                if (IdsSelectedCategories.Count() == 0)
                {
                    selectedDefect.AsoDefectObj = null;
                }
                else if (!IdsSelectedCategories.Contains(selectedDefect.Class))
                {
                    // On gère ici le cas où on desactive la catégorie du defaut courant selectionné.
                    // Dans ce cas, on affiche le premier defaut actif appartenant à une categorie quelconque
                    defect = activesDefects.FirstOrDefault();
                }
                else
                {
                    defect = activesDefects.FirstOrDefault(x => x.DefectId == selectedDefect.Id);
                }

                // Update defects number
                //TotalCountSelected = activesDefects.Count();
                ActiveDefects = activesDefects.Count();
            }
            else
            {
                if (selectedDefect.NextButtonClicked)
                {
                    // On Recherche le premier defaut actif qui suit le defaut courant selectionné
                    defect = activesDefects.FirstOrDefault(x => x.DefectId > selectedDefect.Id);
                }
                else
                {
                    // On Recherche le premier defaut actif qui precède le defaut courant selectionné
                    defect = activesDefects.Where(x => x.DefectId < selectedDefect.Id)
                                           .OrderByDescending(x => x.DefectId)
                                           .FirstOrDefault();
                }
                if (defect == null)
                    return;
            }

            if (IdsSelectedCategories.Count() > 0)
            {
                selectedDefect.Change(defect);
            }
            else
            {
                ShowHilite(false);
            }
        }

        public void SearchDefectByClusterNumber(int clusterId)
        {
            var data = ResultDataObj as DataAso;
            int nFoundDefects = 0;
            var prmDefect = data.DefectViewItemList.ItemList.Find(x => x.Defect.ClusterNumber == clusterId);
            if (prmDefect != null)
            {
                var IdsSelectedCategories = DefectCategories.Where(x => x.IsSelected).Select(x => x.LabelCategory);
                if (IdsSelectedCategories.Contains(prmDefect.DefectCategory))
                {
                    nFoundDefects = 1;
                    ((AsoDefectVM)Defects.SelectedDefect).Change(prmDefect);
                }
            }
            FoundDefectsbyClusterNum = nFoundDefects;
            IsSearchClusterDone = true;
        }

        public void SearchDefectByPosition(System.Windows.Point pt)
        {
            if (SelectedCategories.Count != 0)
            {
                var data = ResultDataObj as DataAso;
                var selectedDefect = (AsoDefectVM)Defects.SelectedDefect;
                string[] CategoryStr = SelectedCategories.Select(i => i.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToArray();
                if (data.DefectViewItemList.FindItemWithModifiers((float)pt.X, (float)pt.Y, CategoryStr, out var defect, (float)AsoDisplayFactor, AsoDisplayMinSize))
                {
                    selectedDefect.Change(defect);
                }
                IsSearchClusterDone = true;
            }

            IsBusy = false;
        }

        public AsoDefect GetDefect(int id)
        {
            var data = ResultDataObj as DataAso;
            var defect = data.DefectViewItemList.ItemList.Find(x => x.DefectId == id);
            return defect;
        }

        #endregion Methods

        #region Histogram

        private BarSeriesCollection _histogramBars;
        public BarSeriesCollection HistogramBars
        {
            get => _histogramBars;
            set => SetProperty(ref _histogramBars, value);
        }

        private CustomAxisTickCollection _customAxis;
        public CustomAxisTickCollection CustomAxis
        {
            get => _customAxis;
            set => SetProperty(ref _customAxis, value);
        }

        private double _axisXMax = 100;
        public double AxisXMax
        {
            get => _axisXMax;
            set => SetProperty(ref _axisXMax, value);
        }

        private double _axisYMax = 100;
        public double AxisYMax
        {
            get => _axisYMax;
            set => SetProperty(ref _axisYMax, value);
        }

        private double _axisXMin = -1;
        public double AxisXMin
        {
            get => _axisXMin;
            set => SetProperty(ref _axisXMin, value);
        }

        private double _axisYMin = -1;
        public double AxisYMin
        {
            get => _axisYMin;
            set => SetProperty(ref _axisYMin, value);
        }


        private void GenerateHistoAndCategories(DataAso data)
        {
            AxisXMin = 1.0 - 0.5;
            AxisXMax = data.ReportDetailList.Count + 0.5;
            double dMaxY = 0.0;
            int nIdx = 0;

            DefectCategories.Add(new DefectCategoryVM() { LabelCategory = "All", IsSelected = true, EllipseColor = Color.LightBlue });

            foreach (var cat in data.ReportDetailList)
            {
                nIdx++;
                string label = cat.Label;
                var clr = data.GetColorCategory(label);

                int nbDefects = cat.Number;

                DefectCategoryVM defectCategory = new DefectCategoryVM(data.ResType.GetResultFormat(), label, clr, nbDefects);
                DefectCategories.Add(defectCategory);

                SelectedCategories.Add(label);

                // Show all categories even dummy
                //if (nbDefects <= 0)
                //{
                //    defectCategories.Last().IsSelected = false;
                //    defectCategories.Last().Enabled = false;
                //}

                if (dMaxY < nbDefects)
                    dMaxY = nbDefects;

                var barseries = InitBarseries(label, clr);
                barseries.Values = new[]
                {
                        CreateBarserieValue(nIdx, nbDefects)
                };
                _histogramBars.Add(barseries);
                AddCustomAxis(label, nIdx);
            }
            AxisYMin = 0.0;
            AxisYMax = dMaxY * 1.1;

            OnPropertyChanged(nameof(CustomAxis));
            OnPropertyChanged(nameof(HistogramBars));
        }

        private BarSeries InitBarseries(string name, Color color)
        {
            var barSeries = new BarSeries();
            barSeries.Fill.Color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            barSeries.Fill.GradientFill = GradientFill.Solid;
            barSeries.Title.Text = name;
            barSeries.IncludeInAutoFit = true;
            barSeries.Title.Visible = false;
            barSeries.BarThickness = 30;
            return barSeries;
        }

        private BarSeriesValue CreateBarserieValue(int positionX, double histoValue)
        {
            var barSerieValue = new BarSeriesValue
            {
                Location = positionX,
                Value = histoValue
            };
            return barSerieValue;
        }

        private void AddCustomAxis(string name, int positionX)
        {
            var tick = new CustomAxisTick
            {
                AxisValue = positionX,
                LabelText = name,
                Style = CustomTickStyle.TickAndGrid,
                Color = System.Windows.Media.Color.FromArgb(255, 200, 0, 255)
            };
            _customAxis.Add(tick);
        }

        #endregion Histogram


        #region Overrides of ResultWaferVM

        public void Clean()
        {
            // Cancel running tasks
            if (_readTokenSource != null && !_readTokenSource.IsCancellationRequested)
            {
                _readTokenSource.Cancel();
                Task.Delay(100).GetAwaiter().GetResult();
            }

            _imageZone.OnDoubleClick -= SelectDefectByClick;
            _messenger.Unregister<ResultsDisplayChangedMessage, string>(this, ResultType.ADC_ASO.ToString());
            _messenger.Unregister<UpdateRectHiliteMessage, string>(this, FormatToken.ToString());
            _messenger.Unregister<DefectActionMessage, string>(this, FormatToken.ToString());
        }


        public override void Dispose()
        {
            Clean();

            _imageZone.Dispose();

            _waferBitmap?.Dispose();
            _waferBitmap = null;

            Defects?.Dispose();
            DefectCategories.Clear();

            _customAxis.Clear();
            _histogramBars.Clear();

            base.Dispose();
        }

        #endregion
    }
}
