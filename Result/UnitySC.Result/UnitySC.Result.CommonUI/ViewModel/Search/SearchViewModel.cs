using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.Shared.UI.ViewModel.Navigation;
using UnitySC.Shared.ResultUI.Common.Message;

namespace UnitySC.Result.CommonUI.ViewModel.Search
{
    public class SearchViewModel : PageNavigationVM
    {
        public const string ConnectionErrorMessage = "ConnectionError";

        #region Properties

        // Private
        private static readonly ResultQuery s_defaultResult = new ResultQuery() { Id = -1, Name = "All" };
        private static readonly ResultQuery s_defaultFilterTag = new ResultQuery() { Id = -1, Name = "All" };
        private static readonly ResultChamberQuery s_defaultChamberResult = new ResultChamberQuery() { Id = -1, Name = "All", ActorType = ActorType.Unknown, ToolIdOwner = -1 };

        private readonly ILogger _logger;
        private readonly DuplexServiceInvoker<IResultService> _resultService;
        private List<ResultQuery> _tools;
        private List<string> _lots;
        private List<ResultQuery> _products;
        private List<ResultChamberQuery> _chambers;
        private List<string> _recipes;
        private List<string> _resultStates;
        private List<string> _resultFilterTags;
        private static bool s_selectedValueContentsFilterToApply;
        private IMessenger _messenger;

        // Public
        public override string PageName => "Search page";     // Nom de la page

        public ICollectionView Lots { get; private set; }
        public ICollectionView Products { get; private set; }
        public ICollectionView Chambers { get; private set; }
        public ICollectionView Recipes { get; private set; }

        public List<string> ResultStates
        {
            get => _resultStates;
            set
            {
                if (_resultStates != value)
                {
                    _resultStates = value; OnPropertyChanged();
                }
            }
        }

        public List<string> ResultFilterTags
        {
            get => _resultFilterTags;
            set
            {
                if (_resultFilterTags != value)
                {
                    _resultFilterTags = value; OnPropertyChanged();
                }
            }
        }

        public event Action<int> OnDisplaySettings;

        //Selections and filters
        //*******Tool*****************
        public List<ResultQuery> Tools
        {
            get => _tools;

            set
            {
                if (_tools != value)
                {
                    _tools = value;
                    OnPropertyChanged();
                }
            }
        }

        private ResultQuery _selectedTool;

        public ResultQuery SelectedTool
        {
            get => _selectedTool;
            set
            {
                if (_selectedTool != value)
                {
                    _selectedTool = value; OnPropertyChanged();
                }
            }
        }

        private int? _uniqueToolId;

        public int? UniqueToolId
        {
            get => _uniqueToolId;
            set
            {
                if (_uniqueToolId != value)
                {
                    _uniqueToolId = value; OnPropertyChanged();
                }
            }
        }


        //*******Lot*****************
        private string _lotFilter;

        public string LotFilter
        {
            get => _lotFilter;
            set
            {
                if (_lotFilter != value)
                {
                    if (_lotFilter != null)
                    {
                        bool success =_lots.Remove(MakeStarFilter(_lotFilter));
                        // if (success) System.Diagnostics.Debug.WriteLine($" --- remove lotfilter <{_lotFilter}>");
                    }

                    _lotFilter = value;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(_lotFilter))
                    {
                        _lots.Insert(0, MakeStarFilter(_lotFilter));
                        //System.Diagnostics.Debug.WriteLine($" +++ add lotfilter <{_lots[0]}>");
                    }

                    Lots.Refresh();
                }
            }
        }

        private bool LotResultFilter(object obj)
        {
            string result = obj as string;
            bool match = false;
            if (_lotFilter != null)
            {
                if (_lotFilter.Contains("*") || _lotFilter.Contains("%"))
                {
                    string pattern = "^" + _lotFilter.Replace("*", ".*?").Replace("%", ".*?") + "$";
                    match = Regex.IsMatch(result, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                else
                    match = result.ToLower().Contains(_lotFilter.ToLower());
            }
            return _lotFilter == null || match || result == s_defaultResult.Name || result == _lotFilter;
        }

        private bool _lotSelectionIsOpen;

        public bool LotSelectionIsOpen
        {
            get => _lotSelectionIsOpen;
            set
            {
                if (_lotSelectionIsOpen != value)
                {
                    _lotSelectionIsOpen = value;
                    OnPropertyChanged();
                    if (!_lotSelectionIsOpen)
                    {
                        if(string.IsNullOrEmpty(SelectedLot) || string.IsNullOrWhiteSpace(SelectedLot))
                           SelectedLot = s_defaultResult.Name;

                        if (LotFilter != null && SelectedLot == s_defaultResult.Name)
                            LotFilter = null;
                    }
                }
            }
        }

        private string _selectedLot;

        public string SelectedLot
        {
            get => _selectedLot;
            set
            {
                if (_selectedLot != value)
                {
                    _selectedLot = value;
                    OnPropertyChanged();

                    //if(!string.IsNullOrEmpty(_selectedLot))
                    //    System.Diagnostics.Debug.WriteLine($"@SelectLot = <{_selectedLot}>");
                    //else
                    //    System.Diagnostics.Debug.WriteLine($"@SelectLot = #NULL#");

                    if(!string.IsNullOrEmpty(_selectedLot))
                        LotSelectionIsOpen = false;
                }
            }
        }

        //*******Product*****************
        private string _productFilter;

        public string ProductFilter
        {
            get => _productFilter;
            set
            {
                if (_productFilter != value)
                {
                    _productFilter = value;
                    OnPropertyChanged();
                    Products.Refresh();
                }
            }
        }

        private bool ProductResultFilter(object obj)
        {
            var result = obj as ResultQuery;
            if (_productFilter != null)
            {
                s_selectedValueContentsFilterToApply = SelectedProduct.Name.ToLower().Contains(_productFilter.ToLower());
                if (!s_selectedValueContentsFilterToApply)
                    SelectedProduct = s_defaultResult;
            }

            return _productFilter == null || result.Name.ToLower().Contains(_productFilter.ToLower()) || result.Id == s_defaultResult.Id;
        }

        private bool _prodSelectionIsOpen;

        public bool ProdSelectionIsOpen
        {
            get => _prodSelectionIsOpen;
            set
            {
                if (_prodSelectionIsOpen != value)
                {
                    _prodSelectionIsOpen = value;
                    OnPropertyChanged();
                    if (!_prodSelectionIsOpen)
                        ProductFilter = null;
                }
            }
        }

        private ResultQuery _selectedProduct;

        public ResultQuery SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    OnPropertyChanged();
                    if (s_selectedValueContentsFilterToApply || _selectedProduct != s_defaultResult)
                        ProdSelectionIsOpen = false;
                }
            }
        }

        //*******Recipe*****************
        private string _recipeFilter;

        public string RecipeFilter
        {
            get => _recipeFilter;
            set
            {
                if (_recipeFilter != value)
                {
                    if (_recipeFilter != null)
                    {
                        bool success = _recipes.Remove(MakeStarFilter(_recipeFilter));
                        // if (success) System.Diagnostics.Debug.WriteLine($" --- remove recipeFilter <{_recipeFilter}>");
                    }

                    _recipeFilter = value;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(_recipeFilter))
                    {
                        _recipes.Insert(0, MakeStarFilter(_recipeFilter));
                        //System.Diagnostics.Debug.WriteLine($" +++ add recipeFilter <{_recipes[0]}>");
                    }

                    Recipes.Refresh();
                }
            }
        }

        private bool RecipeResultFilter(object obj)
        {
            string result = obj as string;
            bool match = false;
            if (_recipeFilter != null)
            {
                if (_recipeFilter.Contains("*") || _recipeFilter.Contains("%"))
                {
                    string pattern = "^" + _recipeFilter.Replace("*", ".*?").Replace("%", ".*?") + "$";
                    match = Regex.IsMatch(result, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                else
                    match = result.ToLower().Contains(_recipeFilter.ToLower());
            }

            return _recipeFilter == null || match || result == s_defaultResult.Name || result == _recipeFilter;
        }

        private bool _recipeSelectionIsOpen;

        public bool RecipeSelectionIsOpen
        {
            get => _recipeSelectionIsOpen;
            set
            {
                if (_recipeSelectionIsOpen != value)
                {
                    _recipeSelectionIsOpen = value;
                    OnPropertyChanged();
                    if (!_recipeSelectionIsOpen)
                    {
                        if (string.IsNullOrEmpty(SelectedRecipe) || string.IsNullOrWhiteSpace(SelectedRecipe))
                            SelectedRecipe = s_defaultResult.Name;

                        if (RecipeFilter != null && SelectedRecipe == s_defaultResult.Name)
                            RecipeFilter = null;
                    }
                }
            }
        }

        private string _selectedRecipe;

        public string SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    OnPropertyChanged();

                    //if(!string.IsNullOrEmpty(_selectedRecipe))
                    //    System.Diagnostics.Debug.WriteLine($"@_selectedRecipe = <{_selectedRecipe}>");
                    //else
                    //    System.Diagnostics.Debug.WriteLine($"@_selectedRecipe = #NULL#");

                    if (!string.IsNullOrEmpty(_selectedRecipe))
                        RecipeSelectionIsOpen = false;
                }
            }
        }

        //*******Chamber*****************
        private string _chamberFilter;

        public string ChamberFilter
        {
            get => _chamberFilter;
            set
            {
                if (_chamberFilter != value)
                {
                    _chamberFilter = value;
                    OnPropertyChanged();
                    Chambers.Refresh();
                }
            }
        }

        private bool ChamberResultFilter(object obj)
        {
            var result = obj as ResultChamberQuery;
            if (_chamberFilter != null)
            {
                bool selectedPMExists = SelectedPM.ActorType.GetLabelName().ToLower().Contains(_chamberFilter.ToLower());
                if (!selectedPMExists)
                    SelectedPM = s_defaultChamberResult;
            }
            return _chamberFilter == null || result.ActorType.GetLabelName().ToLower().Contains(_chamberFilter.ToLower()) || result == _selectedPM;
        }

        private bool _chamberSelectionIsOpen;

        public bool ChamberSelectionIsOpen
        {
            get => _chamberSelectionIsOpen;
            set
            {
                if (_chamberSelectionIsOpen != value)
                {
                    _chamberSelectionIsOpen = value;
                    OnPropertyChanged();
                    if (!_chamberSelectionIsOpen)
                        ChamberFilter = null;
                }
            }
        }

        private ResultChamberQuery _selectedPM;

        public ResultChamberQuery SelectedPM
        {
            get => _selectedPM;
            set
            {
                if (_selectedPM != value)
                {
                    _selectedPM = value;
                    OnPropertyChanged();
                    if (s_selectedValueContentsFilterToApply || _selectedPM.Id != s_defaultResult.Id)
                        ChamberSelectionIsOpen = false;
                }
            }
        }

        //*******Liste des jobs repondant aux critères de recherche *****************
        private List<Job> _jobResultsList;

        public List<Job> JobResultsList
        {
            get => _jobResultsList;
            set
            {
                if (_jobResultsList != value)
                {
                    _jobResultsList = value;
                    OnPropertyChanged();
                }
            }
        }

        //*******Result state*****************
        private string _resultState;

        public string SelectedResultState
        {
            get => _resultState;
            set
            {
                if (_resultState != value)
                {
                    _resultState = value;
                    OnPropertyChanged();
                }
            }
        }

        //*******Result Filter tag*****************
        private string _selresultfilterTag;

        public string SelectedResultFilterTag
        {
            get => _selresultfilterTag;
            set
            {
                if (_selresultfilterTag != value)
                {
                    _selresultfilterTag = value;
                    OnPropertyChanged();
                }
            }
        }

        //*******Start date*****************
        private DateTime? _selectedStartDate;

        public DateTime? SelectedStartDate
        {
            get => _selectedStartDate;
            set
            {
                if (_selectedStartDate != value)
                {
                    _selectedStartDate = value;

                    OnPropertyChanged();
                }
            }
        }

        //*******End date*****************
        private DateTime? _selectedEndDate;

        public DateTime? SelectedEndDate
        {
            get => _selectedEndDate;
            set
            {
                if (_selectedEndDate != value)
                {
                    _selectedEndDate = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resultService"></param>
        /// <param name="logger"></param>
        public SearchViewModel(DuplexServiceInvoker<IResultService> resultService, ILogger logger)
        {
            _resultService = resultService;
            _logger = logger;
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            //_dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
        }

        #endregion Constructor

        #region Commands

        /// <summary>
        /// Search button command
        /// </summary>
        private AutoRelayCommand _searchCommand;

        public AutoRelayCommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new AutoRelayCommand(() =>
                {
                    try
                    {
                        var sqlQueryParams = GetSearchParams();
                        JobResultsList = _resultService.Invoke(x => x.GetSearchJobs(sqlQueryParams));
                        SearchCommand.NotifyCanExecuteChanged();
                    }
                    catch (Exception)
                    {
                        _messenger.Send(new DisplayErrorMessage(ConnectionErrorMessage)); // A Valider
                    }
                },
                () => true));
            }
        }

        /// <summary>
        /// Reset button command.
        /// </summary>
        private AutoRelayCommand _commandReset;

        public AutoRelayCommand CommandReset
        {
            get
            {
                return _commandReset ?? (_commandReset = new AutoRelayCommand(
              () =>
              {
                  ResetSearchParams();
                  CommandReset.NotifyCanExecuteChanged();
              },
              () => { return true; }));
            }
        }

        /// <summary>
        /// Permet de fermer la zone d'édition d'un dropdownButton par un double click lorsqu'il contient déja l'element selectionné par le user.
        /// </summary>
        private AutoRelayCommand _closeCommand;

        public AutoRelayCommand DropDownButtonCloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new AutoRelayCommand(
              () =>
              {
                  ProdSelectionIsOpen = false;
                  LotSelectionIsOpen = false;
                  ChamberSelectionIsOpen = false;
                  RecipeSelectionIsOpen = false;
              },
              () => { return true; }));
            }
        }

        /// <summary>
        /// Disaplay viewer settings page command.
        /// </summary>
        private AutoRelayCommand _commandViewerSettings;

        public AutoRelayCommand CommandViewerSettings
        {
            get
            {
                return _commandViewerSettings ?? (_commandViewerSettings = new AutoRelayCommand(
              () =>
              {
                  OnDisplaySettings?.Invoke(0);
              },
              () => { return true; }));
            }
        }

        /// <summary>
        /// Disaplay viewer settings page command.
        /// </summary>
        private AutoRelayCommand _updateFiltersCommand;

        public AutoRelayCommand UpdateFiltersCommand
        {
            get
            {
                return _updateFiltersCommand ?? (_updateFiltersCommand = new AutoRelayCommand(
              () =>
              {
                  RefreshSearchFilters(UniqueToolId);
                  UpdateFiltersCommand.NotifyCanExecuteChanged();
              },
              () => { return true; }));
            }
        }

        

        #endregion Commands

        #region Methods

        /// <summary>
        /// Initialisation des données des combobox.
        /// </summary>
        internal void Init(int? pToolKey = null)
        {
            try
            {
                UniqueToolId = null;
                if(pToolKey.HasValue)
                    UniqueToolId = _resultService.Invoke(x => x.RetrieveToolIdFromToolKey(pToolKey.Value)).FirstOrDefault().Id;

                // Récuperation des données
                var tools = _resultService.Invoke(x => x.GetTools());
                tools.Insert(0, s_defaultResult);
                Tools = tools;
                SelectedTool = s_defaultResult;

                _products = _resultService.Invoke(x => x.GetProducts());
                _products.Insert(0, s_defaultResult);
                SelectedProduct = s_defaultResult;

                _lots = _resultService.Invoke(x => x.GetLots(pToolKey));
                _lots.Insert(0, s_defaultResult.Name);
                SelectedLot = s_defaultResult.Name;

                _chambers = _resultService.Invoke(x => x.GetChambers(pToolKey, UniqueToolId, true));
                _chambers.Insert(0, s_defaultChamberResult);
                SelectedPM = s_defaultChamberResult;

                _recipes = _resultService.Invoke(x => x.GetRecipes(pToolKey));
                _recipes.Insert(0, s_defaultResult.Name);
                SelectedRecipe = s_defaultResult.Name;
                // Liste des jobs
                Lots = CollectionViewSource.GetDefaultView(_lots);
                Lots.Filter = LotResultFilter;
                // Liste des produits
                Products = CollectionViewSource.GetDefaultView(_products);
                Products.Filter = ProductResultFilter;
                //Liste des recettes
                Recipes = CollectionViewSource.GetDefaultView(_recipes);
                Recipes.Filter = RecipeResultFilter;
                //Liste des process modules (chambres)
                Chambers = CollectionViewSource.GetDefaultView(_chambers);
                Chambers.Filter = ChamberResultFilter;
                //Liste des états du résultat
                var resStateList = Enum.GetNames(typeof(ResultState)).ToList();
                //resStateList.Sort();
                resStateList.Insert(0, s_defaultResult.Name);
                ResultStates = resStateList;
                SelectedResultState = ResultStates[0];
                //Liste des results filter tags
                var resTagList = Enum.GetNames(typeof(ResultFilterTag)).ToList();
                resTagList.Insert(0, s_defaultFilterTag.Name);
                ResultFilterTags = resTagList;
                SelectedResultFilterTag = ResultFilterTags[0];
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Init Search VM");
                ClassLocator.Default.GetInstance<NotifierVM>().AddMessage(new Message(MessageLevel.Error, "Access to result service error : Check the connection"));
            }
        }

        internal void RefreshSearchFilters(int? pToolKey = null)
        {
            ResetSearchParams();

            try
            {       
                _products.RemoveRange(1, _products.Count() - 1);
                var products =_resultService.Invoke(x => x.GetProducts());
                _products.AddRange(products);
                Products.Refresh();
                SelectedProduct = s_defaultResult;

                var lots = _resultService.Invoke(x => x.GetLots(pToolKey));
                _lots.RemoveRange(1, _lots.Count() - 1);
                _lots.AddRange(lots);
                Lots.Refresh();
                SelectedLot = s_defaultResult.Name;

                var recipes = _resultService.Invoke(x => x.GetRecipes(pToolKey));
                _recipes.RemoveRange(1, _recipes.Count() - 1);
                _recipes.AddRange(recipes);
                Recipes.Refresh();
                SelectedRecipe = s_defaultResult.Name;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Refresh Search Filters");
                ClassLocator.Default.GetInstance<NotifierVM>().AddMessage(new Message(MessageLevel.Error, "Access to result service error : Check the connection"));
            }
        }


        /// <summary>
        /// Récuperation des données de recherche selectionnées.
        /// </summary>
        /// <returns></returns>
        protected SearchParam GetSearchParams()
        {
            var paramQuery = new SearchParam();

            // Attention: startDate doit etre inférieure à EndDate  TO DO

            if (SelectedStartDate != null)
            {
                paramQuery.StartDate = SelectedStartDate;
            }
            if (SelectedEndDate != null)
            {
                paramQuery.EndDate = SelectedEndDate;
            }
            if (SelectedTool != null && SelectedTool.Id > -1)
            {
                paramQuery.ToolId = SelectedTool.Id;
            }
            if (SelectedProduct != null && SelectedProduct.Id > -1)
            {
                paramQuery.ProductId = SelectedProduct.Id;
            }
            if (SelectedLot != null && SelectedLot.ToUpper() != "ALL")
            {
                string lotSearch = SelectedLot.Replace('*', '%').Trim();
                paramQuery.LotName = lotSearch;
            }
            if (SelectedRecipe != null && SelectedRecipe.ToUpper() != "ALL")
            {
                string recipeSearch = SelectedRecipe.Replace('*', '%').Trim();
                paramQuery.RecipeName = recipeSearch;
            }
            if (SelectedPM != null && SelectedPM.Id > -1)
            {
                paramQuery.ActorType = SelectedPM.ActorType;
            }
            if (SelectedResultState != null && SelectedResultState.ToUpper() != "ALL")
            {
                paramQuery.ResultState = (ResultState)Enum.Parse(typeof(ResultState), SelectedResultState);
            }
            if (SelectedResultFilterTag != null && SelectedResultFilterTag.ToUpper() != "ALL")
            {
                paramQuery.ResultFilterTag = (ResultFilterTag)Enum.Parse(typeof(ResultFilterTag), SelectedResultFilterTag);
            }

            return paramQuery;
        }

        /// <summary>
        /// Renitialise les données des champs de recherche.
        /// </summary>
        private void ResetSearchParams()
        {
            SelectedStartDate = null;
            SelectedEndDate = null;
            SelectedTool = Tools[0];
            SelectedLot = s_defaultResult.Name;
            SelectedPM = s_defaultChamberResult;
            SelectedProduct = s_defaultResult;
            SelectedRecipe = s_defaultResult.Name;
            JobResultsList = null;
            SelectedResultState = s_defaultResult.Name;
            SelectedResultFilterTag = s_defaultFilterTag.Name;

            ChamberFilter = null;
            ProductFilter = null;
            LotFilter = null;
            RecipeFilter = null;
        }

        private static string MakeStarFilter(string filter)
        {
            if (filter.Contains('*'))
                return filter;

            string starfilter = filter;
            if (!filter.StartsWith("*"))
                starfilter = "*" + starfilter;
            if (!filter.EndsWith("*"))
                starfilter += "*";
            return starfilter;
        }

        #endregion Methods
    }
}
