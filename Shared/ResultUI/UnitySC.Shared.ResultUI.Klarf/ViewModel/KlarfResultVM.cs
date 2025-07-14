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
using UnitySC.Shared.Display._001;
using UnitySC.Shared.Format._001;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Defect;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Helper;

namespace UnitySC.Shared.ResultUI.Klarf.ViewModel
{
    public class KlarfResultVM : DefectCategoriesVM
    {
        private const ResultFormat FormatToken = ResultFormat.Klarf;

        private CancellationTokenSource _readTokenSource;
        private const string TiffExtension = ".t01";

        #region Properties
        public override string FormatName => "Klarf";

        private readonly IMessenger _messenger;

        private bool _hasthumbnails;
        public bool HasThumbnails
        {
            get => _hasthumbnails;
            set { SetProperty(ref _hasthumbnails, value); }
        }

        public string RoughBintoClass(int roughbin)
        {
            string rbclass = string.Empty;
            if (ResultDisplay is KlarfDisplay klarfDisplay)
            {
                rbclass = klarfDisplay.GetCategoryName(roughbin);
            }
            return rbclass;
        }

        public Color RoughBintoColor(int roughbin)
        {
            var rbcolor = Color.Transparent;
            if (ResultDisplay is KlarfDisplay klarfDisplay)
            {
                rbcolor = klarfDisplay.GetColorCategory(roughbin);
            }
            return rbcolor;
        }


        private int _totalCountSelected;
        public override int TotalCountSelected
        {
            get => _totalCountSelected;
            set
            {
                if (_totalCountSelected != value)
                {
                    _totalCountSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalActiveDefects;
        public override int ActiveDefects
        {
            get => _totalActiveDefects;
            set => SetProperty(ref _totalActiveDefects, value);
        }

        private List<int> _selectedCategories;
        public List<int> SelectedCategories
        {
            get => _selectedCategories;
            set { SetProperty(ref _selectedCategories, value); }
        }

        private bool _isDisplayGrid;
        public bool IsDisplayGrid
        {
            get => _isDisplayGrid; set { if (_isDisplayGrid != value) { _isDisplayGrid = value; OnPropertyChanged(); } }
        }

        private bool _isDisplayGridEnabled;
        public bool IsDisplayGridEnabled
        {
            get => _isDisplayGridEnabled; set { if (_isDisplayGridEnabled != value) { _isDisplayGridEnabled = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<DefectCategoryVM> _defectCategories;
        public override ObservableCollection<DefectCategoryVM> DefectCategories
        {
            get { return _defectCategories; }
            set => SetProperty(ref _defectCategories, value);
        }

        private readonly ImageZoneVM _imageZone = new ImageZoneVM();

        private void UpdateRectHilite(UpdateRectHiliteMessage updateRectHilite)
        {
            SetRectHilite((KlarfDefect)updateRectHilite.DefectRect);
            ShowHilite(updateRectHilite.Show);
        }
        public void ShowHilite(bool bShow)
        {
            _imageZone.ShowHilite = bShow;
        }

        public void SetRectHilite(KlarfDefect defect)
        {
            if (defect != null)
            {
                var rc = Rectangle.Round(defect.Rectpx);
                _imageZone.HiliteRect = new Int32Rect(rc.X, rc.Y, rc.Width, rc.Height);
            }
        }


        private readonly object _defthumbs_sync = new object();


        // VM de la vue des imagettes
        private ThumbnailsDefectsVM _defects;
        public ThumbnailsDefectsVM Defects
        {
            get => _defects;
            set
            {
                if (_defects != value)
                {
                    _defects = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Current lot view for container presenter.
        /// </summary>
        private ObservableRecipient _waferzoneVM;
        public ObservableRecipient WaferZoneVM
        {
            get => _waferzoneVM; set { if (_waferzoneVM != value) { _waferzoneVM = value; OnPropertyChanged(); } }
        }

        private int _maxIdDefect;
        public int MaxIdDefect
        {
            get => _maxIdDefect;
            set
            {
                if (_maxIdDefect != value)
                {
                    _maxIdDefect = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _minIdDefect;
        public int MinIdDefect
        {
            get => _minIdDefect;
            set
            {
                if (_minIdDefect != value)
                {
                    _minIdDefect = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _foundDefectsbyClusterNum;
        public int FoundDefectsbyClusterNum
        {
            get => _foundDefectsbyClusterNum;
            set
            {
                if (_foundDefectsbyClusterNum != value)
                {
                    _foundDefectsbyClusterNum = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSearchClusterDone;
        public bool IsSearchClusterDone
        {
            get => _isSearchClusterDone;
            set
            {
                if (_isSearchClusterDone != value)
                {
                    _isSearchClusterDone = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructor
        public KlarfResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            DefectCategories = new ObservableCollection<DefectCategoryVM>();
            SelectedCategories = new List<int>();

            _customAxis = new CustomAxisTickCollection();
            _histogramBars = new BarSeriesCollection();

            _waferzoneVM = _imageZone;
            _totalCountSelected = 0;

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
                if (item.IsSelected && item.IDcode.HasValue)
                {
                    SelectedCategories.Add(item.IDcode.Value);
                }
            }

            Defects.SelectedCategories = SelectedCategories.Select(_ => Convert.ToString(_)).ToList();

            UpdateSelectedDefect(true);

            //           IsBusy = true;
            Task.Run(() => DrawWaferImage(_readTokenSource.Token));
        }

        #endregion

        #region Methods

        #region Selections
        public void SelectDefectByClick(System.Windows.Point pt)
        {
            if (!IsBusy)
            {
                // update wafer image
                IsBusy = true;
                Task.Run(() => SearchDefectByPosition(pt));
            }
        }


        // Gestion de la selection de la vue wafer et imagettes.
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

        #endregion

        public async override void UpdateResData(IResultDataObject resdataObj)
        {
            // Unregister previous messages
            Clean();

            IsBusy = true;
            _readTokenSource = new CancellationTokenSource();

            System.Diagnostics.Debug.WriteLine($"KLARF UpdateResData[{resdataObj.DBResId}] {Path.GetFileName(resdataObj.ResFilePath)}");

            _messenger.Register<ResultsDisplayChangedMessage, string>(this, ResultType.ADC_Klarf.ToString(), (r, m) => Clean());
            _imageZone.OnDoubleClick += SelectDefectByClick;

            // Draw area containing the selected defect 
            _messenger.Register<UpdateRectHiliteMessage, string>(this, FormatToken.ToString(), (r, m) => UpdateRectHilite(m));

            // Commands from DefectInfoView (defect detail management)
            _messenger.Register<DefectActionMessage, string>(this, FormatToken.ToString(), (r, m) => UpdateKlarfDefect(m));

            ResultDataObj = resdataObj;

            var dataKlarf = ResultDataObj as DataKlarf;
            var resKlarfDisplay = ResultDisplay as KlarfDisplay;

            if (dataKlarf == null)
            {
                return;
            }

            lock (_defthumbs_sync)
            {
                Defects = new ThumbnailsDefectsVM();
            }

            ResetView();

            //update all attribute and redraw image
            IsDisplayGridEnabled = dataKlarf.SampleTestPlan.NbDies > 1;
            IsDisplayGrid = IsDisplayGridEnabled; // a voir si on garde le statut dans le display

            GenerateHistoAndCategories(dataKlarf, resKlarfDisplay);

            var allCategory = DefectCategories.FirstOrDefault(_ => _.LabelCategory == "All");
            if (allCategory != null)
            {
                allCategory.NbDefects = DefectCategories.Sum(_ => _.NbDefects);
            }

            Defects.SelectedCategories = SelectedCategories.Select(_ => Convert.ToString(_)).ToList();

            await Task.Run(() => DrawWaferImage(_readTokenSource.Token, true));

            if (dataKlarf.DefectViewItemList == null)
            {
                return;
            }

            if (dataKlarf.DefectViewItemList.ItemList != null)
            {
                TotalCountSelected = dataKlarf.DefectViewItemList.ItemList.Count;
                ActiveDefects = TotalCountSelected;

                if (TotalCountSelected != 0)
                {
                    MaxIdDefect = dataKlarf.DefectViewItemList.ItemList.Max(x => GetDefectId(x));
                    MinIdDefect = dataKlarf.DefectViewItemList.ItemList.Min(x => GetDefectId(x));
                }

                if (DefectCategories.Any())
                {
                    var IdsSelectedCategoriesg = DefectCategories.Where(x => x.IsSelected).Select(x => x.IDcode);
                    var orderedDefects = dataKlarf.DefectViewItemList.ItemList.OrderBy(_ => GetDefectId(_));

                    // Recuperer le premier defaut des categories actives et l'afficher
                    var defectobj = orderedDefects.FirstOrDefault(x => IdsSelectedCategoriesg.Contains((int)x.Defect.Get("ROUGHBINNUMBER")));
                    if (defectobj != null)
                    {
                        // Defect thumbnail view
                        Defects.SelectedDefect = new KlarfDefectVM(this);
                        Defects.SelectedDefect.Change(defectobj);
                    }
                }


                string tiffPath = Path.GetFileNameWithoutExtension(dataKlarf.ResFilePath);
                string tiffFileName = Path.Combine(Path.GetDirectoryName(dataKlarf.ResFilePath), String.Concat(tiffPath, ".t01"));

                HasThumbnails = !string.IsNullOrEmpty(dataKlarf.TiffFileName) && File.Exists(tiffFileName);
                if (HasThumbnails)
                {
                    await Task.Run(() => LoadThumbnails(_readTokenSource.Token, resdataObj, resKlarfDisplay));
                }
            }

            IsBusy = false;

            System.Diagnostics.Debug.WriteLine($"~KLARF UpdateResData[{resdataObj.DBResId}] {Path.GetFileName(resdataObj.ResFilePath)} Exit ");
        }

        public bool LoadThumbnails(CancellationToken token, IResultDataObject resultData, IResultDisplay display = null)
        {
            var dataKlarf = resultData as DataKlarf;

            string tiffPath = Path.GetFileNameWithoutExtension(dataKlarf.ResFilePath);
            string tiffFileName = Path.Combine(Path.GetDirectoryName(dataKlarf.ResFilePath), String.Concat(tiffPath, TiffExtension));

            var defectsList = dataKlarf.DefectList;
            var resKlarfDisplay = display as KlarfDisplay;
            int index = 0;

            Defects.TotalThumbnails = defectsList.Count;

            // A revoir
            //lock (_defthumbs_sync)
            //{
            foreach (var defect in defectsList)
            {
                if (token != null && token.IsCancellationRequested)
                {
                    return false;
                }

                int id = (int)defect.Get("DEFECTID");
                var defectThumbnails = (PrmImageData)defect.Get("IMAGELIST");
                int roughBin = (int)defect.Get("ROUGHBINNUMBER");
                //string label = resKlarfDisplay.GetCategoryName(roughBin);
                string label = roughBin.ToString();
                var color = resKlarfDisplay.GetColorCategory(roughBin);

                index++;

                Defects.UpdateProgressBar(index * 100 / Defects.TotalThumbnails);


                if (defectThumbnails.List.Count > 0)
                {
                    // Dans ACDV9,les indexs des imagettes sont numérotés de 1 à n et dans le fichier tif de 0 à n-1 donc on soustrait 1
                    // pour etre coherent avec le fichier tif.
                    string greyLevelImageTitle = $"Grey {id}  ";
                    string binaryImageTitle = $"Binary {id}";

                    var thumbnail = new Thumbnail(greyLevelImageTitle, tiffFileName, id, label, color, defectThumbnails.List[0] - 1);

                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        Defects.ActiveThumbnails.Add(thumbnail);
                    });

                    if (defectThumbnails.List.Count > 1)
                    {
                        thumbnail = new Thumbnail(binaryImageTitle, tiffFileName, id, label, color, defectThumbnails.List[1] - 1);

                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            Defects.ActiveThumbnails.Add(thumbnail);
                        });
                    }
                    ;
                }

                if (index == 1)
                {
                    _messenger.Send(new DefectActionMessage(DefectActionEnum.SelectAndDisplayFirstDefect), FormatToken.ToString());
                }
            }
            //}

            System.Diagnostics.Debug.WriteLine($"LoadThumbnails Klarf DONE [{tiffPath}]...");

            return true;
        }


        private void UpdateKlarfDefect(DefectActionMessage klarfDefectTO)
        {
            switch (klarfDefectTO.Action)
            {
                case DefectActionEnum.ChangeCurrentDefect:
                    var klarfdefect = GetDefect(klarfDefectTO.Id);
                    (Defects.SelectedDefect).Change(klarfdefect);
                    break;
                case DefectActionEnum.NavigateToDefect:
                    UpdateSelectedDefect();
                    break;
                case DefectActionEnum.SearchDefectByClusterNumber:
                    SearchDefectByClusterNum(klarfDefectTO.Id);
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
            /* Remarque
             * Cete méthode devrait dans le futur prendre en compte le cas où on zoome sur le wafer et selectionne un defaut.
             * Il serait peut etre mieux dans ce cas de passer directement l'id du defaut à rechercher en paramètre ce qui implique
             * de faire une partie du code hors de cette méthode pour le bouton Next et Previous(exple: quand on clique sur Next,on
             * recherche d'abord l'id du defaut suivant et on le passe en paramètre à cette méthode et cette dernière se contentera
             * uniquement de rechercher le defaut dans la liste et le retourner)
             */

            var data = ResultDataObj as DataKlarf;
            KlarfDefect defect = null;

            // Note de rti: commenter ci -dessous, voir si vraiment utilse en pratique
            //data.DefectViewItemList.ItemList.OrderBy(x => GetDefectId(x));

            var selectedDefect = (KlarfDefectVM)Defects.SelectedDefect;
            // Liste des roughBin des catégories selectionnées
            var IdsSelectedCategories = DefectCategories.Where(x => x.IsSelected).Select(x => x.IDcode).ToList();

            if (data.DefectList == null || data.DefectViewItemList == null || !data.DefectViewItemList.ItemList.Any() || selectedDefect == null)
            {
                return;
            }

            var activesDefects = data.DefectViewItemList.ItemList
                                                        .Where(x => IdsSelectedCategories.Contains((int)x.Defect.Get("ROUGHBINNUMBER")))
                                                        .OrderBy(x => GetDefectId(x)).ToList();

            if (categSelected)
            {
                // Selectionner uniquement les imagettes dont leurs categories de defauts sont actives
                if (IdsSelectedCategories.Count == 0)
                    selectedDefect.InitDefectData();
                else if (!IdsSelectedCategories.Contains(selectedDefect.IDcode))
                    /* On gère ici le cas où on desactive la catégorie du defaut courant selectionné.
                     * Dans ce cas, on affiche le premier defaut actif appartenant à une categorie quelconque*/
                    defect = activesDefects.FirstOrDefault();
                else
                    defect = activesDefects.Where(x => GetDefectId(x) == selectedDefect.Id).FirstOrDefault();

                // Update defects number
                //TotalCountSelected = activesDefects.Count;
                ActiveDefects = activesDefects.Count;
            }
            else
            {
                if (selectedDefect.NextButtonClicked)
                    // On Recherche le premier defaut actif qui suit le defaut courant selectionné
                    defect = activesDefects.Where(x => GetDefectId(x) > selectedDefect.Id).FirstOrDefault();
                else
                    // On Recherche le premier defaut actif qui precède le defaut courant selectionné
                    defect = activesDefects.Where(x => GetDefectId(x) < selectedDefect.Id)
                                           .OrderByDescending(x => GetDefectId(x))
                                           .FirstOrDefault();
                if (defect == null)
                    return;
            }

            if (IdsSelectedCategories.Count > 0)
            {
                //int defectId = (int)defect.Get("DEFECTID");
                selectedDefect.Change(defect);
            }
            else
                ShowHilite(false);
        }

        public void SearchDefectByClusterNum(int clusterNum)
        {
            string sDefectIDCat = "DEFECTID";
            string sQueryCat = "CLUSTERNUMBER";
            var data = ResultDataObj as DataKlarf;
            int nFoundDefects = 0;
            var prmDefects = data.DefectList.Where(x => clusterNum == (int)x.Get(sQueryCat)).OrderBy(x => (int)x.Get(sDefectIDCat));
            var prmDefect = prmDefects.FirstOrDefault();
            if (prmDefect != null)
            {
                var IdsSelectedCategories = DefectCategories.Where(x => x.IsSelected).Select(x => x.IDcode).ToList();
                int rbin = (int)prmDefect.Get("ROUGHBINNUMBER");
                if (IdsSelectedCategories.Contains(rbin))
                {
                    nFoundDefects = prmDefects.Count();
                    int defectId = (int)prmDefect.Get(sDefectIDCat);
                    var selectedDefect = (KlarfDefectVM)Defects.SelectedDefect;
                    selectedDefect.Change(defectId);
                }
            }

            FoundDefectsbyClusterNum = nFoundDefects;
            IsSearchClusterDone = true;
        }

        public void SearchDefectByPosition(System.Windows.Point pt)
        {
            if (SelectedCategories.Count != 0)
            {
                var data = ResultDataObj as DataKlarf;
                var selectedDefect = (KlarfDefectVM)Defects.SelectedDefect;
                string[] RBinStr = SelectedCategories.Select(i => i.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToArray();
                if (true == data.DefectViewItemList.FindItem((float)pt.X, (float)pt.Y, RBinStr, out var defect))
                {
                    selectedDefect.Change(defect);
                }
                IsSearchClusterDone = true;
            }

            IsBusy = false;
        }

        /// <summary>
        /// Retourne le défaut associé à l'id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public KlarfDefect GetDefect(int id)
        {
            var data = ResultDataObj as DataKlarf;
            var defect = data.DefectViewItemList.ItemList.Where(x => GetDefectId(x) == id).FirstOrDefault();
            return defect;
        }

        /// <summary>
        /// Retourne l'id du defaut.
        /// </summary>
        /// <param name="klarfDefect"></param>
        /// <returns></returns>
        private int GetDefectId(KlarfDefect klarfDefect)
        {
            return (int)klarfDefect.Defect.Get("DEFECTID");
        }

        private void GenerateHistoAndCategories(DataKlarf dataKlarf, KlarfDisplay resKlarfDisplay)
        {
            AxisXMin = 1.0 - 0.5;
            AxisXMax = dataKlarf.RBinKeys.Count + 0.5;
            double dMaxY = 0.0;
            int nIdx = 0;

            DefectCategories.Add(new DefectCategoryVM() { IDcode = null, LabelCategory = "All", IsSelected = true, EllipseColor = Color.LightBlue });

            foreach (int rbin in dataKlarf.RBinKeys)
            {
                nIdx++;
                string label = resKlarfDisplay.GetCategoryName(rbin);
                var clr = resKlarfDisplay.GetColorCategory(rbin);
                long nbDefects = dataKlarf.GetCategoryNbDefect(rbin);

                DefectCategoryVM defectCategory = new DefectCategoryVM(dataKlarf.ResType.GetResultFormat(), label, clr, (int)nbDefects, rbin);
                DefectCategories.Add(defectCategory);

                SelectedCategories.Add(rbin);

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

            TotalCountSelected = 0;
            MaxIdDefect = 0;
            MinIdDefect = 0;
        }

        // Bitmap to load wafer image
        private Bitmap _waferBitmap;
        private bool DrawWaferImage(CancellationToken cancellationToken, bool bFirstDraw = false)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
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

        #endregion Methods

        #region Histogram

        private BarSeriesCollection _histogramBars;
        public BarSeriesCollection HistogramBars
        {
            get => _histogramBars; set { if (_histogramBars != value) { _histogramBars = value; OnPropertyChanged(); } }
        }

        private CustomAxisTickCollection _customAxis;
        public CustomAxisTickCollection CustomAxis
        {
            get => _customAxis; set { if (_customAxis != value) { _customAxis = value; OnPropertyChanged(); } }
        }

        private double _axisXMax = 100;
        public double AxisXMax
        {
            get => _axisXMax; set { if (_axisXMax != value) { _axisXMax = value; OnPropertyChanged(); } }
        }

        private double _axisYMax = 100;
        public double AxisYMax
        {
            get => _axisYMax; set { if (_axisYMax != value) { _axisYMax = value; OnPropertyChanged(); } }
        }

        private double _axisXMin = -1;
        public double AxisXMin
        {
            get => _axisXMin; set { if (_axisXMin != value) { _axisXMin = value; OnPropertyChanged(); } }
        }

        private double _axisYMin = -1;
        public double AxisYMin
        {
            get => _axisYMin; set { if (_axisYMin != value) { _axisYMin = value; OnPropertyChanged(); } }
        }

        protected BarSeries InitBarseries(string name, Color color)
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

        protected BarSeriesValue CreateBarserieValue(int positionX, double histoValue)
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
            if (_readTokenSource != null && !_readTokenSource.IsCancellationRequested)
            {
                _readTokenSource.Cancel();
                Task.Delay(100).GetAwaiter().GetResult();
            }

            _imageZone.OnDoubleClick -= SelectDefectByClick;
            _messenger.Unregister<ResultsDisplayChangedMessage, string>(this, ResultType.ADC_Klarf.ToString());
            _messenger.Unregister<UpdateRectHiliteMessage, string>(this, FormatToken.ToString());
            _messenger.Unregister<DefectActionMessage, string>(this, FormatToken.ToString());
        }

        public override void Dispose()
        {
            Clean();

            _imageZone.Dispose();
            Defects?.Dispose();

            _waferBitmap?.Dispose();
            _waferBitmap = null;

            DefectCategories.Clear();
            SelectedCategories.Clear();

            _customAxis.Clear();
            _histogramBars.Clear();

            base.Dispose();
        }

        #endregion
    }
}
