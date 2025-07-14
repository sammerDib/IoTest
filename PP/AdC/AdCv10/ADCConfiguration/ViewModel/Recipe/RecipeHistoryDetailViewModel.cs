using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

using ADC.Messages;
using ADC.ViewModel;
using ADC.ViewModel.Graph;

using ADCEngine;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.DataAccess.Dto;
using Dto = UnitySC.DataAccess.Dto;

using Utils.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Proxy;

namespace ADCConfiguration.ViewModel.Recipe
{
    public class RecipeHistoryDetailViewModel : ValidationViewModelBase
    {
        // [Base] ViewModel du graph
        private RecipeGraphViewModel _recipeGraphBaseVM;
        public RecipeGraphViewModel RecipeGraphBaseVM => _recipeGraphBaseVM;

        // [Compare] ViewModel du graph
        private RecipeGraphViewModel _recipeGraphCompareVM;
        public RecipeGraphViewModel RecipeGraphCompareVM => _recipeGraphCompareVM;

        // [Base] ViewModel des parmatéres du module séléctionnée
        private ObservableCollection<RecipeHistoryParameterViewModel> _parametersBaseVM;
        public ObservableCollection<RecipeHistoryParameterViewModel> ParametersBaseVM => _parametersBaseVM;

        // [Compare] ViewModel des paramétres du module séléctionnée
        private ObservableCollection<RecipeHistoryParameterViewModel> _parametersCompareVM;
        public ObservableCollection<RecipeHistoryParameterViewModel> ParametersCompareVM => _parametersCompareVM;

        // Determine si le dernier noeud séléctionné vient de la partie [Compare]
        private bool _oldSelectedNodeIsCompare = false;

        private const string GrahCompareName = "Compare";
        private const string GrahBaseName = "Base";

        private bool _currentRecipesContainsDifferences = false;


        /// <summary>
        /// Liste des différentes versions des recettes
        /// </summary>
        private List<Dto.Recipe> _recipes;
        public List<Dto.Recipe> Recipes
        {
            get => _recipes; set { if (_recipes != value) { _recipes = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// [Base] Recette séléectionné 
        /// </summary>
        private Dto.Recipe _selectedRecipeBase;
        public Dto.Recipe SelectedRecipeBase
        {
            get => _selectedRecipeBase;
            set
            {
                if (_selectedRecipeBase != value)
                {
                    _selectedRecipeBase = value;
                    OnPropertyChanged();

                    // Mise à jour du graph en focntion de la version de la recette séléectionnéee
                    InitGraph(_recipeGraphBaseVM, _selectedRecipeBase);

                    // Mise à jour de l'état des noeud de la recette
                    UpdateNodeState(_recipeGraphBaseVM, _recipeGraphCompareVM);

                    if (_recipeGraphBaseVM.Nodes.Any())
                        _recipeGraphBaseVM.SelectedNode = (ModuleNodeViewModel)_recipeGraphBaseVM.Nodes.First();
                }
            }
        }
        private Dto.Recipe _selectedRecipeCompare;
        public Dto.Recipe SelectedRecipeCompare
        {
            get => _selectedRecipeCompare;
            set
            {
                if (_selectedRecipeCompare != value)
                {
                    _selectedRecipeCompare = value;
                    OnPropertyChanged();

                    // Mise à jour du graph en focntion de la version de la recette séléectionnéee
                    InitGraph(_recipeGraphCompareVM, _selectedRecipeCompare);

                    // Mise à jour de l'état des noeud de la recette
                    UpdateNodeState(_recipeGraphBaseVM, _recipeGraphCompareVM);

                    _recipeGraphCompareVM.SelectedNode = (ModuleNodeViewModel)_recipeGraphCompareVM.Nodes.First();
                }
            }
        }

        public RecipeHistoryDetailViewModel(IMessenger messenger)
        {
            SplitHorizontal = false;

            // [Base]
            _recipeGraphBaseVM = new RecipeGraphViewModel() { Name = GrahBaseName };
            _parametersBaseVM = new ObservableCollection<RecipeHistoryParameterViewModel>();

            // [Compare]
            _recipeGraphCompareVM = new RecipeGraphViewModel() { Name = GrahCompareName };
            _parametersCompareVM = new ObservableCollection<RecipeHistoryParameterViewModel>();

            // On se connect au message de changement de module
            messenger.Register<SelectedModuleChanged>(this, (r, m) =>
            {
                OnSelectedModuleChanged(m.NewModule, (RecipeGraphViewModel)m.Sender);
            });
        }

        /// <summary>
        /// Nom courant de la recette 
        /// </summary>
        private string _currentRecipeName;
        public string CurrentRecipeName
        {
            get => _currentRecipeName;
            set
            {
                if (_currentRecipeName != value)
                {
                    _currentRecipeName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initalisation global en fonction du nom de la recette
        /// </summary>
        /// <param name="recipeName"></param>
        public void Init(string recipeName)
        {
            if (string.IsNullOrEmpty(recipeName))
            {
                CurrentRecipeName = null;
                return;
            }
            CurrentRecipeName = recipeName;
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Récupération des différentes version de la recette en base
                    var recipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
                    Recipes = recipeProxy.GetADCRecipes(recipeName, true).OrderByDescending(x => x.Version).ToList();
                    _selectedRecipeBase = Recipes.First();
                    _selectedRecipeCompare = Recipes.Count > 1 ? Recipes[1] : _selectedRecipeBase;

                    // Mise à jour des éléments connecté à la vue
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        InitGraph(_recipeGraphBaseVM, _selectedRecipeBase);
                        InitGraph(_recipeGraphCompareVM, _selectedRecipeCompare);
                        UpdateNodeState(_recipeGraphBaseVM, _recipeGraphCompareVM);
                        _recipeGraphBaseVM.SelectedNode = (ModuleNodeViewModel)_recipeGraphBaseVM.Nodes.First();
                        OnPropertyChanged(nameof(SelectedRecipeBase));
                        OnPropertyChanged(nameof(SelectedRecipeCompare));
                    }));
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Refresh recipe history", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Refresh recipe  history error: ", ex); }));
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        /// <summary>
        /// Initalisation du graphVm en fonction d'une recette
        /// </summary>
        /// <param name="graphVm"></param>
        /// <param name="recipe"></param>
        private void InitGraph(RecipeGraphViewModel graphVm, Dto.Recipe recipe)
        {
            try
            {
                graphVm.ClearGraph();
                if (recipe != null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(recipe.XmlContent);
                    graphVm.LoadRecipe(xmlDoc);
                    UpdateFileParameterVersion(graphVm, recipe);
                    UpdateNodeState(_recipeGraphBaseVM, _recipeGraphCompareVM);
                }
            }
            catch (Exception ex)
            {
                Services.Services.Instance.LogService.LogError("Load recipe error", ex);
                System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Load recipe error: ", ex); }));
            }
        }

        /// <summary>
        ///  Lors du changement du noeud séléctionné dans un des graphs [Base] ou [Compare]
        /// </summary>
        /// <param name="selectedModule"></param>
        /// <param name="sender"></param>
        private void OnSelectedModuleChanged(ModuleNodeViewModel selectedModule, RecipeGraphViewModel sender)
        {
            if (sender.Name == GrahCompareName) // Changement du noeud séléctionné sur la dans le graph [Compare]
            {
                // Mise à jour des paramétres
                _parametersCompareVM.Clear();
                if (_recipeGraphCompareVM.SelectedNode != null)
                {
                    ModuleNodeViewModel sameModuleInBase = _recipeGraphBaseVM.FindNode(selectedModule.Module.Id);
                    foreach (var param in _recipeGraphCompareVM.SelectedNode.Module.ParameterList)
                    {
                        bool same = false;
                        if (sameModuleInBase != null)
                        {
                            int compareParamIndex = _recipeGraphCompareVM.SelectedNode.Module.ParameterList.IndexOf(param);
                            if (sameModuleInBase.Module.ParameterList.Count > compareParamIndex)
                                same = param.HasSameValue(sameModuleInBase.Module.ParameterList[compareParamIndex]);
                        }
                        _parametersCompareVM.Add(new RecipeHistoryParameterViewModel(param, same));
                    }

                    // Mise à jour du noeud sélectionné dans [Base] 
                    if (sameModuleInBase == null)
                    {
                        _recipeGraphBaseVM.SelectedNode = null;
                    }
                    else if (_recipeGraphBaseVM.SelectedNode != sameModuleInBase)
                    {
                        _recipeGraphBaseVM.SelectedNode = sameModuleInBase;
                    }

                    _recipeGraphCompareVM.RefreshSelectedNode();
                }
            }
            else // Changement du noeud séléctionné sur la dans le graph [Base]
            {
                // Mise à jour des paramétres
                _parametersBaseVM.Clear();
                if (_recipeGraphBaseVM.SelectedNode != null)
                {
                    ModuleNodeViewModel sameModuleInCompare = _recipeGraphCompareVM.FindNode(selectedModule.Module.Id);
                    foreach (var param in _recipeGraphBaseVM.SelectedNode.Module.ParameterList)
                    {
                        bool same = false;
                        if (sameModuleInCompare != null)
                        {
                            int baseParamIndex = _recipeGraphBaseVM.SelectedNode.Module.ParameterList.IndexOf(param);
                            if (sameModuleInCompare.Module.ParameterList.Count > baseParamIndex)
                                same = param.HasSameValue(sameModuleInCompare.Module.ParameterList[baseParamIndex]);
                        }
                        _parametersBaseVM.Add(new RecipeHistoryParameterViewModel(param, same));
                    }

                    // Mise à jour du noeud sélectionné dans [Compare]                    
                    if (sameModuleInCompare == null)
                    {
                        _recipeGraphCompareVM.SelectedNode = null;
                    }
                    else if (_recipeGraphCompareVM.SelectedNode != sameModuleInCompare)
                    {
                        _recipeGraphCompareVM.SelectedNode = sameModuleInCompare;
                    }

                    _recipeGraphBaseVM.RefreshSelectedNode();
                }
            }
        }

        /// <summary>
        /// Définit si sépare les graphs de leurs paramétres de facon horizontal ou vertical
        /// </summary>
        private bool _splitHorizontal;
        public bool SplitHorizontal
        {
            get => _splitHorizontal; set { if (_splitHorizontal != value) { _splitHorizontal = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Définit si les graphs sont synchronisés (Zoom et Positions)
        /// </summary>
        private bool _graphsAreSynchro = true;
        public bool GraphsAreSynchro
        {
            get => _graphsAreSynchro;
            set
            {
                if (_graphsAreSynchro != value)
                {
                    _graphsAreSynchro = value;
                    OnPropertyChanged();
                    if (_graphsAreSynchro)
                    {
                        OnPropertyChanged(nameof(Scale));
                        OnPropertyChanged(nameof(OffsetX));
                        OnPropertyChanged(nameof(OffsetY));
                    }
                }
            }
        }

        /// <summary>
        /// Niveau de zoom
        /// </summary>
        private double _scale = 0.6;
        public double Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    if (GraphsAreSynchro)
                        OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Décalage du graph dans la vue en X
        /// </summary>
        private double _offsetX = 560.0;
        public double OffsetX
        {
            get => _offsetX;
            set
            {
                if (_offsetX != value)
                {
                    _offsetX = value;
                    if (GraphsAreSynchro)
                        OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Décalage du graph dans la vue en Y
        /// </summary>
        private double _offsetY = 0;
        public double OffsetY
        {
            get => _offsetY;
            set
            {
                if (_offsetY != value)
                {
                    _offsetY = value;
                    if (GraphsAreSynchro)
                        OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Mise à jour des noeuds des graphs 
        /// </summary>
        /// <param name="graphBase"></param>
        /// <param name="graphCompare"></param>
        private void UpdateNodeState(RecipeGraphViewModel graphBase, RecipeGraphViewModel graphCompare)
        {

            _currentRecipesContainsDifferences = false;
            // Mise à jour du graph de base
            foreach (ModuleNodeViewModel nodeBase in graphBase.Nodes)
            {
                ModuleNodeViewModel nodeCompare = graphCompare.Nodes.OfType<ModuleNodeViewModel>().SingleOrDefault(x => x.Name == nodeBase.Name);
                // Noeud supprimé
                if (nodeCompare == null)
                {
                    SetVisualNodeState(nodeBase, eModuleVisualState.Removed);
                    _currentRecipesContainsDifferences = true;
                }
                else
                {
                    // Comparaison des valeurs des paramétres
                    if (ParamsAreEqual(nodeBase, nodeCompare))
                    {
                        SetVisualNodeState(nodeBase, eModuleVisualState.Same);
                    }
                    else
                    {
                        SetVisualNodeState(nodeBase, eModuleVisualState.Different);
                        _currentRecipesContainsDifferences = true;
                    }
                }
            }

            // Mise à jour du grah comparé
            foreach (ModuleNodeViewModel nodeCompare in graphCompare.Nodes)
            {
                ModuleNodeViewModel nodeBase = graphBase.Nodes.OfType<ModuleNodeViewModel>().SingleOrDefault(x => x.Name == nodeCompare.Name);

                // Noeud Ajouter
                if (nodeBase == null)
                {
                    SetVisualNodeState(nodeCompare, eModuleVisualState.Added);
                    _currentRecipesContainsDifferences = true;
                }
                else
                {
                    // Comparaison des valeurs des paramétres
                    if (ParamsAreEqual(nodeBase, nodeCompare))
                    {
                        SetVisualNodeState(nodeCompare, eModuleVisualState.Same);
                    }
                    else
                    {
                        SetVisualNodeState(nodeCompare, eModuleVisualState.Different);
                        _currentRecipesContainsDifferences = true;
                    }
                }
            }

            // Mise à jour de l'état des boutons previous et next difference
            NextCommand.NotifyCanExecuteChanged();
            PreviousCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Remplacement du paramétre FileParameter par FileParameterHistoryViewModel qui permet la gestion de la comparaison avec un numéro de version
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <param name="recipe"></param>
        private void UpdateFileParameterVersion(RecipeGraphViewModel graph, Dto.Recipe recipe)
        {
            foreach (ModuleNodeViewModel node in graph.Nodes)
            {
                if (recipe.RecipeFiles.Any())
                {
                    foreach (FileParameter fileParamerter in node.Module.ParameterList.OfType<FileParameter>().ToList())
                    {
                        RecipeFile recipeFile = recipe.RecipeFiles.SingleOrDefault(x => x.FileName == fileParamerter.SelectedOption);
                        int? version = recipeFile != null ? recipeFile.Version : null;

                        node.Module.ParameterList[node.Module.ParameterList.IndexOf(fileParamerter)] = new FileParameterHistoryViewModel(node.Module, fileParamerter.Label, version, fileParamerter.SelectedOption);
                    }
                }
            }
        }

        /// <summary>
        /// Définit l'état visuel d'un noeud
        /// </summary>
        /// <param name="node"></param>
        /// <param name="state"></param>
        /// <param name="customMessage"></param>
        private void SetVisualNodeState(ModuleNodeViewModel node, eModuleVisualState state, string customMessage = null)
        {
            node.State = state;
            node.BackgroundColorIndex = (int)state;

            if (customMessage != null)
                node.Message = customMessage;
            else
                node.Message = state.ToString();
        }

        /// <summary>
        /// Comapraison des valeurs des paramétres entre 2 modules
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <param name="nodeCompare"></param>
        /// <returns></returns>
        private bool ParamsAreEqual(ModuleNodeViewModel nodeBase, ModuleNodeViewModel nodeCompare)
        {
            return nodeBase.Module.ParametersValuesAreEquals(nodeCompare.Module);
        }

        /// <summary>
        /// Sélection du précedent module qui contient des paramétres différents
        /// </summary>
        private void PreviousDifference()
        {
            MoveToDifference(true);
        }

        /// <summary>
        /// Sélection du module suivant qui contient des paramétres différents
        /// </summary>
        private void NextDifference()
        {
            MoveToDifference(false);
        }

        /// <summary>
        /// Sélection du module suivant/précédent qui contient des paramétres différents
        /// </summary>
        /// <param name="backward"></param>
        private void MoveToDifference(bool backward)
        {
            ModuleNodeViewModel selectedNode = null;
            if (_oldSelectedNodeIsCompare && _recipeGraphCompareVM.SelectedNode != null)
                selectedNode = _recipeGraphCompareVM.SelectedNode;
            else
                selectedNode = _recipeGraphBaseVM.SelectedNode != null ? _recipeGraphBaseVM.SelectedNode : _recipeGraphCompareVM.SelectedNode;

            // Listes des différences [Base] + [Compare]
            IEnumerable<ModuleNodeViewModel> baseDifference = _recipeGraphBaseVM.Nodes.OfType<ModuleNodeViewModel>().Where(x => x.State == eModuleVisualState.Removed || x.State == eModuleVisualState.Different);
            IEnumerable<ModuleNodeViewModel> compareDifference = _recipeGraphCompareVM.Nodes.OfType<ModuleNodeViewModel>().Where(x => x.State == eModuleVisualState.Added);
            List<ModuleNodeViewModel> differences = baseDifference.Concat(compareDifference).OrderBy(node => node.Y).ToList();

            // on détermine l'index suivant
            int nextIndex = 0;
            if (selectedNode != null &&
                (!backward && differences.IndexOf(_recipeGraphBaseVM.SelectedNode) < differences.Count - 1)
                || (backward && differences.IndexOf(_recipeGraphBaseVM.SelectedNode) > 0))
            {
                nextIndex = differences.IndexOf(selectedNode) + (backward ? -1 : 1);
            }

            if (!differences.Any() || differences.Count - 1 < nextIndex)
                return;

            ModuleNodeViewModel nextNode = differences[nextIndex];

            // Sélection du module
            if (_recipeGraphBaseVM.Nodes.Contains(nextNode))
            {
                _recipeGraphBaseVM.SelectedNode = nextNode;
                _oldSelectedNodeIsCompare = false;
            }
            else
            {
                _recipeGraphCompareVM.SelectedNode = nextNode;
                _oldSelectedNodeIsCompare = true;
            }
        }

        #region commands

        private AutoRelayCommand _previousCommand;
        public AutoRelayCommand PreviousCommand
        {
            get
            {
                return _previousCommand ?? (_previousCommand = new AutoRelayCommand(
              () =>
              {
                  PreviousDifference();
              },
              () => { return _currentRecipesContainsDifferences; }));
            }
        }


        private AutoRelayCommand _nextCommand;
        public AutoRelayCommand NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new AutoRelayCommand(
              () =>
              {
                  NextDifference();
              },
              () => { return _currentRecipesContainsDifferences; }));
            }
        }

        #endregion
    }
}
