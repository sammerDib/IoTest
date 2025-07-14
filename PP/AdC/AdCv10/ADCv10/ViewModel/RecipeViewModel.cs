using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

using ADC.AdcEnum;
using ADC.Controls;
using ADC.Model;
using ADC.View.Graph;
using ADC.View.Parameters;
using ADC.View.RunTime;
using ADC.ViewModel.Ada;
using ADC.ViewModel.Graph;
using ADC.ViewModel.MergedRecipe;

using ADCEngine;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

using UnitySC.PP.Shared.Configuration;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.Data.Enum;
using Serilog.Core;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Helpers;
using UnitySC.PP.ADC.Client.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;
//using System.Data.Entity.Core.Objects;
//using AIS.Core.Globalization.Mvvm;

namespace ADC.ViewModel
{
    /// <summary>
    /// The view-model for the RecipeView
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class RecipeViewModel : ObservableRecipient
    {
        private IAdcExecutor _adcLocalExecutor = new AdcExecutor();
        private RecipeGraphViewModel _recipeGraphVM;
        static public UndoRedo.UndoRedoManager UndoRedocommandManager = new UndoRedo.UndoRedoManager();
        private const string ConfigurationManagerFileName = "ConfigurationManager.exe";
        private const string ADCConfigurationFileName = "ADCConfiguration.exe";
        private const string ADCProdFileName = "ADCProd.exe";

        private IAdcExecutor AdcExecutor
        {
            get
            {
                /*if (_adcLocalExecutor == null)
                {

                    string remoteExecutorWcf = AppParameter.Instance.Get("PPHostIAdcExecutor"); // "net.tcp://localhost:2250/IAdcExecutor"     ""; //

                    if (!string.IsNullOrEmpty(remoteExecutorWcf))
                    {

                        //ContractDescription contract = new ContractDescription("IAdcAcquisition");

                        //"net.tcp://localhost:2250/IAdcExecutor"
                        EndpointAddress address = new EndpointAddress(remoteExecutorWcf);
                        NetTcpBinding binding = new NetTcpBinding();

                        //ServiceEndpoint endpoint = new ServiceEndpoint(contract, binding, address);


                        ChannelFactory<IAdcExecutor> AdcAcquisitionChannelFactory = new ChannelFactory<IAdcExecutor>(binding, address);
                        _adcLocalExecutor = AdcAcquisitionChannelFactory.CreateChannel();



                    }
                    else
                    {

                        _adcLocalExecutor = new AdcExecutor();
                    }
                }*/



                return _adcLocalExecutor;
            }
        }

        private AcquisitionAdcExchange.RecipeId _recipeId;



        #region Internal Data Members

        public string CurrentFileName;

        public RecipeGraphViewModel RecipeGraphVM
        {
            get { return _recipeGraphVM; }
        }

        private bool _isRecipeRunning = false;
        public bool IsRecipeRunning
        {
            get { return _isRecipeRunning; }
            set
            {
                if (_isRecipeRunning == value)
                    return;
                _isRecipeRunning = value;
                OnPropertyChanged();
            }
        }

        private bool _showNodeResult = true;
        public bool ShowNodeResult
        {
            get { return _showNodeResult; }
            set
            {
                if (_showNodeResult == value)
                    return;
                _showNodeResult = value;
                OnPropertyChanged();
            }
        }

        public bool IsRecipeEdited
        {
            get { return IsRecipeEditedExpert || IsRecipeEditedSimplified; }
        }

        public bool IsRecipeEditedExpert
        {
            get { return ((EditionMode == RecipeEditionMode.ExpertRecipeEdition) && (_recipeGraphVM != null && _recipeGraphVM.GraphContainsNodes)); }
        }

        public bool IsRecipeEditedSimplified
        {
            get { return ((EditionMode == RecipeEditionMode.SimplifiedRecipeEdition) && _recipeGraphVM.GraphContainsNodes); }
        }

        public bool IsRecipeProcessingView
        {
            get { return ((EditionMode == RecipeEditionMode.RecipeProcessing) && _recipeGraphVM.GraphContainsNodes); }
        }

        public void UpdateRecipeEditedState()
        {
            OnPropertyChanged(nameof(IsRecipeEdited));
            OnPropertyChanged(nameof(IsRecipeEditedExpert));
            OnPropertyChanged(nameof(IsRecipeEditedSimplified));
            OnPropertyChanged(nameof(IsRecipeProcessingView));
            if (_recipeGraphVM != null)
                _recipeGraphVM.IsEditable = _editionMode == RecipeEditionMode.ExpertRecipeEdition;
        }

        ///
        /// The current scale at which the content is being viewed.
        /// 
        public bool IsChangeModeEnable
        {
            get
            {
                return (_recipeGraphVM.GraphContainsNodes || (EditionMode == RecipeEditionMode.RecipeProcessing));
            }
        }

        public Recipe Recipe
        {
            get { return ServiceRecipe.Instance().RecipeCurrent; }
            set
            {
                ServiceRecipe.Instance().RecipeCurrent = value;
                OnPropertyChanged(nameof(Recipe));
            }
        }


        /// <summary>
        /// Si l'application démarre en vue simplifié le mode expert est pas disponible
        /// </summary>
        public bool ExpertIsAvailable { get; private set; }

        /// <summary>
        /// Timer used in running to display information from modules 
        /// </summary>
        private DispatcherTimer _timerGraph;
        private DispatcherTimer _timerRecipeValidation;


        #endregion Internal Data Members

        #region Module Details
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Gestion de la partie droite de l'IHM
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //=================================================================
        // La partie droite est un conteneur
        //=================================================================
        private UserControl _moduleDetails;
        public UserControl ModuleDetails
        {
            get { return _moduleDetails; }
            set
            {
                if (_moduleDetails == value)
                    return;
                _moduleDetails = value;
                OnPropertyChanged();
            }
        }


        //=================================================================
        // Les vues qu'on affecte dans ModuleDetail
        //=================================================================
        private ParametersExpertView _parametersExpertView = new ParametersExpertView();
        private ParametersSimplifiedView _parametersSimplifiedView = new ParametersSimplifiedView();
        private RunTimeView _runTimeView = new RunTimeView();
        private EmptyView _emptyView = new EmptyView();

        //=================================================================
        // Les Tabs de la RunTimeView
        //=================================================================
        public class TabListItem
        {
            public string Header { get; set; }
            public UserControl TabContent { get; set; }
        }
        private ObservableCollection<TabListItem> _tabList = new ObservableCollection<TabListItem>();
        public ObservableCollection<TabListItem> TabList { get { return _tabList; } }

        // Tab sélectionné
        //.................................................................
        private TabListItem _selectedTab;
        public TabListItem SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        // Tab pour le log
        //.................................................................
        private UserControl _logControl = new LogControl();

        //=================================================================
        // Mode actif
        //=================================================================
        private RecipeEditionMode _startupEditionMode = RecipeEditionMode.ExpertRecipeEdition;
        private RecipeEditionMode _editionMode = (RecipeEditionMode)(-1);   //unintialized
        private RecipeEditionMode _previousEditionMode;
        public RecipeEditionMode EditionMode
        {
            get { return _editionMode; }
            set
            {
                if (_editionMode == value)
                    return;

                RecipeEditionMode oldmode = _editionMode;
                _editionMode = value;
                OnEditionModeChanged(_editionMode, oldmode);
                OnPropertyChanged();
                if (_recipeGraphVM != null)
                {
                    _recipeGraphVM.IsEditable = _editionMode == RecipeEditionMode.ExpertRecipeEdition;
                }
                UpdateRecipeEditedState();
            }
        }

        //-----------------------------------------------------------------
        // Changement de mode
        //-----------------------------------------------------------------
        protected virtual void OnEditionModeChanged(RecipeEditionMode newmode, RecipeEditionMode oldmode)
        {
            //.................................................................
            // Le mode RecipeProcessing est un peu spécial, puisqu'on doit
            // pouvoir revenir au mode précédent quand la recette est finie.
            //.................................................................
            if (newmode != RecipeEditionMode.RecipeProcessing)
                _previousEditionMode = newmode;

            //.................................................................
            // Gestion du ModuleDetail dans la partie de droite de l'IHM
            //.................................................................
            if (_recipeGraphVM != null)
                _recipeGraphVM.SelectedNode = null;

            _timerRecipeValidation.Stop();
            RefreshEditionModeState();
        }

        //===================================================================
        // Liste des parameters exportés par les modules
        //===================================================================
        private ObservableCollection<ParameterBase> _exportedParameterList = new ObservableCollection<ParameterBase>();
        public ObservableCollection<ParameterBase> ExportedParameterList { get { return _exportedParameterList; } }

        //=================================================================
        // Selected Module Parameters: gestion des parametres exportés en mode Simplifié
        //=================================================================

        //-----------------------------------------------------------------
        // La liste des paramètres du module sélectionnés dans le graph
        // => pour les sélectionner dans la listbox
        //-----------------------------------------------------------------
        private ObservableCollection<ParameterBase> _selectedModuleParameters;
        public ObservableCollection<ParameterBase> SelectedModuleParameters
        {
            get { return _selectedModuleParameters; }
            protected set
            {
                if (_selectedModuleParameters == value)
                    return;
                _selectedModuleParameters = value;

                OnPropertyChanged();
            }
        }

        //-----------------------------------------------------------------
        // Le paramètre sélectionné dans la listbox
        // => pour sélectionner le module correspondant dans le graph
        //-----------------------------------------------------------------
        private ParameterBase _selectedModuleParameter;
        public ParameterBase SelectedModuleParameter
        {
            get { return _selectedModuleParameter; }
            set
            {
                if (_selectedModuleParameter == value)
                    return;
                _selectedModuleParameter = value;

                OnSelectedModuleParameterChanged();
                OnPropertyChanged();
            }
        }

        //-----------------------------------------------------------------
        // OnSelectedModuleParameterChanged:
        // On selectionne le module correspondant au paramètre
        //-----------------------------------------------------------------
        protected virtual void OnSelectedModuleParameterChanged()
        {
            if (_selectedModuleParameter != null)
                SelectedModule = _selectedModuleParameter.Module;
        }


        //-----------------------------------------------------------------
        // OnSelectedNodeChanged
        //-----------------------------------------------------------------
        protected virtual void OnSelectedNodeChanged(ModuleNodeViewModel newnode, ModuleNodeViewModel oldnode)
        {
            // On selectionne le Module correspondant
            //.......................................
            SelectedModule = newnode?.Module;

            // Mise à jour de l'IHM
            //.....................
            if (EditionMode == RecipeEditionMode.ExpertRecipeEdition)
            {
                if (newnode == null || newnode.Module is RootModule)
                {
                    ModuleDetails = _emptyView;
                }
                else
                {
                    ModuleDetails = _parametersExpertView;
                    ModuleDetails.DataContext = newnode;
                }
            }
        }

        //===================================================================
        // Le module selectionné
        //===================================================================
        private ModuleBase _selectedModule;
        public ModuleBase SelectedModule
        {
            get { return _selectedModule; }
            set
            {
                if (_selectedModule == value)
                    return;
                _selectedModule = value;
                OnSelectedModuleChanged();
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModuleRenderingUI));
            }
        }

        //------------------------------------------------------------
        // OnSelectedModuleChanged:
        // On selectionne aussi le Node correspondant au module
        //------------------------------------------------------------
        protected virtual void OnSelectedModuleChanged()
        {
            // Affichage du nouveau module
            //............................
            switch (EditionMode)
            {
                case RecipeEditionMode.ExpertRecipeEdition:
                    // La sélection est faite dans OnSelectedNodeChanged()
                    break;
                case RecipeEditionMode.SimplifiedRecipeEdition:
                    if (_selectedModule == null)
                        SelectedModuleParameters = null;
                    else
                        SelectedModuleParameters = _selectedModule.ExportedParameterList;
                    break;
            }

            // Le noeud correspondant est-il déja sélectionné ?
            //.....................................................
            if (_selectedModule == null)
                _recipeGraphVM.SelectedNode = null;
            if (_recipeGraphVM.SelectedNode != null && _recipeGraphVM.SelectedNode.Module == _selectedModule)
                return;

            // Sinon parcours des noeuds
            //.....................................................
            foreach (ModuleNodeViewModel node in _recipeGraphVM.Nodes)
            {
                if (node.Module == _selectedModule)
                {
                    _recipeGraphVM.SelectedNode = node;
                    return;
                }
            }

            // Pas trouvé
            //.....................................................
            _recipeGraphVM.SelectedNode = null;
        }
        #endregion Module Details

        //=================================================================
        // Constructeur
        //=================================================================
        public RecipeViewModel(IMessenger messenger) : base(messenger)
        {

            _recipeGraphVM = new RecipeGraphViewModel();
            messenger.Register<Messages.SelectVisibleParameters>(this, (r, m) =>
            {
                ManageExports();
            });

            messenger.Register<Messages.SelectedModuleChanged>(this, (r, m) =>
            {
                OnSelectedNodeChanged(m.NewModule, m.OldModule);
            });
            CurrentFileName = "";

            ServiceRecipe.Instance().RecipeExecutedEventHandler += (s, e) => IsRecipeRunning = false;
            ServiceRecipe.Instance().RecipeChanged += (s, e) => { if (EditionMode != RecipeEditionMode.RecipeProcessing) DeferredValidateRecipe(); };

            // Timer pour rafraichir les stats (en mode Run)
            //................................
            _timerGraph = new DispatcherTimer();
            _timerGraph.Tick += new EventHandler(timerGraph_Tick);
            _timerGraph.Interval = new TimeSpan(0, 0, 0, seconds: 1);

            // Timer pour la validation de la recette
            _timerRecipeValidation = new DispatcherTimer();
            _timerRecipeValidation.Tick += (e, s) => ValidateRecipe();
            _timerRecipeValidation.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: 200);

            // Startup EditionMode
            Enum.TryParse<RecipeEditionMode>(ConfigurationManager.AppSettings["Editor.StartupMode"], true, out _startupEditionMode);
            EditionMode = _startupEditionMode;
            ExpertIsAvailable = _startupEditionMode != RecipeEditionMode.SimplifiedRecipeEdition;
        }

        /// <summary>
        /// Switch Mode entre Expert/Simplifié/Run
        /// </summary>
        public void SwitchMode()
        {
            switch (EditionMode)
            {
                case RecipeEditionMode.SimplifiedRecipeEdition:
                    if (ExpertIsAvailable)
                        EditionMode = RecipeEditionMode.ExpertRecipeEdition;
                    break;
                case RecipeEditionMode.ExpertRecipeEdition:
                    EditionMode = RecipeEditionMode.SimplifiedRecipeEdition;
                    break;
                case RecipeEditionMode.RecipeProcessing:
                    EditionMode = _previousEditionMode;
                    break;
                default:
                    throw new ApplicationException("invalid SelectedMode:" + EditionMode);
            }
        }


        private void RefreshExportedParameterList()
        {
            ExportedParameterList.Clear();
            if (Recipe == null)
                return;

            List<ParameterBase> list = Recipe.GetExportedParameterList();
            ExportedParameterList.AddRange(list);
            DeferredValidateRecipe();
        }

        /// <summary>
        /// If IsRecipeRunning == true, query if user want to stop
        /// </summary>
        /// <returns></returns>
        public bool CheckStopRunning()
        {
            if (IsRecipeRunning == true)
            {
                MessageBoxResult result = MessageBox.Show(Properties.Resources.QueryStop, Properties.Resources.ApplicationLabel, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    AbortGraph();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Create new Recipe
        /// </summary>
        public void NewRecipe()
        {
            // In fist, need to close actual recipe ?
            //if (CheckSave() == false)
            //{
            //    return;
            //}
            CloseRecipe();
            CreateRecipe();
            EditionMode = RecipeEditionMode.ExpertRecipeEdition;
            UpdateRecipeEditedState();
        }

        public bool CanSwitchSimplifiedView()
        {
            if (IsRecipeRunning)
                return false;
            else if (_recipeGraphVM != null)
                return (IsChangeModeEnable && (_recipeGraphVM.GraphContainsNodes));
            else
                return false;
        }

        /// <summary>
        /// Open a recipe
        /// </summary>
        public bool LoadRecipe()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();

            openFileDlg.Filter = "Recipe files (*.adcrcp)|*.adcrcp;|Merged recipe file (*.adcmge)|*.adcmge";
            openFileDlg.InitialDirectory = ConfigurationManager.AppSettings["Editor.RecipeFolder"];

            if (openFileDlg.ShowDialog() != true)
                return false;

            // In fist, need to close actual recipe ?
            if (!CheckSave())
            {
                return false;
            }

            string filename = openFileDlg.FileName;
            LoadRecipe(filename);
            RefreshEditionModeState();
            return true;
        }

        public void LoadRecipeFromBase(Guid recipeKey)
        {
            var _dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            var dbRecipe = _dbRecipeServiceProxy.GetLastRecipe(recipeKey);

            LoadRecipeFromXml(dbRecipe.XmlContent);
            Recipe.Name=dbRecipe.Name;
            Recipe.Key = dbRecipe.KeyForAllVersion;
            Recipe.Comment = dbRecipe.Comment;
            Recipe.StepId = dbRecipe.StepId;
            Recipe.UserId = dbRecipe.CreatorUserId;
            Recipe.CreatorChamberId = dbRecipe.CreatorChamberId;

            OnPropertyChanged(nameof(Recipe));

        }


        private void RefreshEditionModeState()
        {
            switch (_editionMode)
            {
                case RecipeEditionMode.ExpertRecipeEdition:
                    ModuleDetails = _emptyView;
                    DeferredValidateRecipe();
                    break;
                case RecipeEditionMode.SimplifiedRecipeEdition:
                    RefreshExportedParameterList();
                    ModuleDetails = _parametersSimplifiedView;
                    ModuleDetails.DataContext = this;
                    SelectedModuleParameter = null;
                    SelectedModuleParameters = null;
                    DeferredValidateRecipe();
                    break;
                case RecipeEditionMode.RecipeProcessing:
                    if (_tabList.Count() > 0)
                        SelectedTab = _tabList.First();
                    else
                        SelectedTab = null;
                    ModuleDetails = _runTimeView;
                    break;
                default:
                    throw new ApplicationException("unknown mode:" + _editionMode);
            }

            UpdateRecipeEditedState();
        }


        public void LoadRecipeFromXml(string xml)
        {
            ServiceRecipe recipe = ServiceRecipe.Instance();

            try
            {
                using (new AdcTools.Widgets.WaitCursor())
                {

                    CloseRecipe();

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(xml);

                    LoadRecipe(xmldoc);
                    ViewModelLocator.Instance.MainWindowViewModel.SubTitle = "Dataflow";
                    UpdateRecipeEditedState();
                }

                if (Recipe.HasUnknwonModules())
                    AttentionMessageBox.Show("Some modules failed to load.");

                if (!ServiceRecipe.Instance().ValidateInputData())
                {
                    if (Services.Services.Instance.PopUpService.ShowConfirmeYesNo("Invalid input data", "This merged recipe contains invalid paths." + Environment.NewLine + "Do you want to update them ?"))
                        Services.Services.Instance.PopUpService.ShowDialogWindow("Select picture paths", new PicturesSelectionViewModel(), 500, 200, true);
                }

                UndoRedocommandManager.Clear();
                RefreshEditionModeState();
            }
            catch (Exception ex)
            {
                string msg = "Failed to open Xml";
                ExceptionMessageBox.Show(msg, ex);
            }
        }


        public void LoadRecipe(string filename)
        {
            ServiceRecipe recipe = ServiceRecipe.Instance();

            try
            {
                using (new AdcTools.Widgets.WaitCursor())
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(filename);
                    LoadRecipe(xmldoc);
                    CurrentFileName = filename;
                    ViewModelLocator.Instance.MainWindowViewModel.SubTitle = CurrentFileName;
                    UpdateRecipeEditedState();
                }

                if (Recipe.HasUnknwonModules())
                    AttentionMessageBox.Show("Some modules failed to load.");

                if (!ServiceRecipe.Instance().ValidateInputData())
                {
                    if (Services.Services.Instance.PopUpService.ShowConfirmeYesNo("Invalid input data", "This merged recipe contains invalid paths." + Environment.NewLine + "Do you want to update them ?"))
                        Services.Services.Instance.PopUpService.ShowDialogWindow("Select picture paths", new PicturesSelectionViewModel(), 500, 200, true);
                }
            }
            catch (Exception ex)
            {
                string msg = "Failed to open \"" + filename + "\"";
                ExceptionMessageBox.Show(msg, ex);
            }
        }

        public void LoadRecipe(XmlDocument xmldoc)
        {
            _recipeGraphVM.LoadRecipe(xmldoc);
            OnPropertyChanged(nameof(RecipeGraphVM));
            SelectedModule = null;
            DeferredValidateRecipe();
        }


        /// <summary>
        /// Save the current document. Then , save the position of nodes in the file
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveDocument(String fileName)
        {
            try
            {
                FileExtension.Backup(fileName);
                ServiceRecipe.Instance().SaveRecipe(fileName);
                // save the position
                XmlDocument xmldoc = new XmlDocument();

                xmldoc.Load(fileName);
                XmlNode xnode = xmldoc.CreateComment("Generated by " + System.Windows.Forms.Application.ProductName);
                xmldoc.AppendChild(xnode);

                xnode = xmldoc.CreateComment("GenerationDate: " + DateTime.Now.ToString());
                xmldoc.AppendChild(xnode);

                xnode = SaveModulesPos(xmldoc);
                xmldoc.DocumentElement.AppendChild(xnode);

                xmldoc.Save(fileName);
            }
            catch (Exception ex)
            {
                string msg = "Failed to save \"" + fileName + "\"";
                ExceptionMessageBox.Show(msg, ex);
            }

        }

        public string SaveDocumentToXml()
        {
            try
            {

                XmlDocument xmldoc = new XmlDocument();
                XmlNode node = ServiceRecipe.Instance().RecipeCurrent.Save(xmldoc, false);

                xmldoc.AppendChild(node);

                XmlNode xnode = xmldoc.CreateComment("Generated by " + System.Windows.Forms.Application.ProductName);
                xmldoc.AppendChild(xnode);

                xnode = xmldoc.CreateComment("GenerationDate: " + DateTime.Now.ToString());
                xmldoc.AppendChild(xnode);

                xnode = SaveModulesPos(xmldoc);
                xmldoc.DocumentElement.AppendChild(xnode);

                var sb = new StringBuilder();
                var sw = new StringWriter(sb);
                xmldoc.Save(sw);

                string ADCEngineRecipeXml = sb.ToString();

                return ADCEngineRecipeXml;
            }
            catch (Exception ex)
            {
                string msg = "Failed to save to xml";
                ExceptionMessageBox.Show(msg, ex);
            }

            return null;
        }



        public XmlNode SaveModulesPos(XmlDocument xmldoc)
        {
            XmlNode recipeNode = xmldoc.CreateElement("GraphView");

            foreach (ModuleNodeViewModel node in _recipeGraphVM.Nodes)
            {
                XmlElement moduleNode = xmldoc.CreateElement("Module");
                moduleNode.SetAttribute("ModID", node.Module.Id.ToString());
                moduleNode.SetAttribute("Name", node.Module.Factory.ModuleName);
                moduleNode.SetAttribute("X", node.X.ToString());
                moduleNode.SetAttribute("Y", node.Y.ToString());
                recipeNode.AppendChild(moduleNode);
            }
            return recipeNode;
        }


        /// <summary>
        /// Check if saving is necessary. if yes, stop the running before save
        /// </summary>
        /// <returns></returns>
        public bool OnSave(bool questionBeforeSave = true)
        {
            MessageBoxResult result;

            if (!CheckStopRunning())
                return false;

            if (!ServiceRecipe.Instance().MustBeSaved)
                return true;

            if (questionBeforeSave)
            {
                result = MessageBox.Show(Properties.Resources.QuerySaveRecipe, Properties.Resources.ApplicationLabel, MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Cancel)
                    return false;

                if (result == MessageBoxResult.No)
                    return true;
            }

            if (String.IsNullOrEmpty(CurrentFileName) || !Path.IsPathRooted(CurrentFileName))
            {
                return OnSaveAs();
            }
            else
            {
                SaveDocument(CurrentFileName);
                return true;
            }
        }

        public bool OnSaveAs()
        {
            if (CheckStopRunning() == false)
            {
                return false;
            }

            SaveFileDialog saveFileDlg = new SaveFileDialog();

            Recipe recipe = ServiceRecipe.Instance().RecipeCurrent;

            if (recipe == null) return false;

            if (ServiceRecipe.Instance().RecipeCurrent.IsMerged)
                saveFileDlg.Filter = "Recipe files (*.adcrcp)|*.adcrcp;|Merged recipe file (*.adcmge)|*.adcmge";
            else
                saveFileDlg.Filter = "Recipe files (*.adcrcp)|*.adcrcp;";

            if (CurrentFileName == null || CurrentFileName == "")
            {
                saveFileDlg.InitialDirectory = ConfigurationManager.AppSettings["Editor.RecipeFolder"];
            }
            else
            {
                PathString pathString = new PathString(CurrentFileName);
                saveFileDlg.InitialDirectory = pathString.Directory;
                saveFileDlg.FileName = pathString.Filename;
            }

            if (saveFileDlg.ShowDialog() == true)
            {
                // Si présence de fichier externes à la recette on propose à l'utilisateur de les copiers.
                bool hasExternalRecipeFiles = recipe.ExternalRecipeFileList.Any() && Directory.Exists(recipe.InputDir);
                string oldInputDir = recipe.InputDir;
                recipe.SetInputDir(saveFileDlg.FileName);

                if (hasExternalRecipeFiles)
                {
                    try
                    {
                        if (Services.Services.Instance.PopUpService.ShowConfirmeYesNo("External recipe files", "Do you want to copy the external files linked to this recipe ?"))
                            FileExtension.DirectoryCopy(oldInputDir, recipe.InputDir, true);
                        SelectedModule = null;
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessageBox.Show("Error in external files copy", ex);
                    }
                }

                SaveDocument(saveFileDlg.FileName);
                CurrentFileName = saveFileDlg.FileName;

                ViewModelLocator.Instance.MainWindowViewModel.SubTitle = saveFileDlg.FileName;

                return true;
            }
            return false;
        }


        public bool OnSaveBdD(bool questionBeforeSave = true)
        {
            MessageBoxResult result;

            if (!CheckStopRunning())
                return false;

            if (!ServiceRecipe.Instance().MustBeSaved)
                return true;

            if (questionBeforeSave)
            {
                result = MessageBox.Show(Properties.Resources.QuerySaveRecipe, Properties.Resources.ApplicationLabel, MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Cancel)
                    return false;

                if (result == MessageBoxResult.No)
                    return true;
            }


            string xml = SaveDocumentToXml();


            bool ok = EmbeddedView.SaveRecipe(xml);


            return true;

        }


        /// <summary>
        /// Controle s'il y as qq chose a sauvegarder. (revois true si tt est sauvegardé)
        /// </summary>
        public bool CheckSave()
        {
            if (_recipeGraphVM.GraphContainsNodes)
            {
                if (!OnSave())
                {
                    return false;
                }
            }

            CloseRecipe();

            return true;
        }

        public Recipe CreateRecipe()
        {
            ServiceRecipe.Instance().CreateRecipe();
            ModuleNodeViewModel rootnode = _recipeGraphVM.CreateRootNode();
            _recipeGraphVM.VisibleRect = rootnode.NodeRect();

            SelectedModule = null;

            ViewModelLocator.Instance.MainWindowViewModel.SubTitle = "New Recipe";
            return Recipe;
        }

        public void CloseRecipe()
        {
            CloseRendering();
            _recipeGraphVM.ClearGraph();
            ServiceRecipe.Instance().CloseRecipe();
            ModuleDetails = null;
            CurrentFileName = String.Empty;
            GraphView.undoRedocommandManager.Clear();

            // On reset les pointers pour qu'ils ne gardent pas des références sur
            // la recette (et donc les objets du klarfHM qui peuvent être très nombreux)
            _parametersExpertView.DataContext = null;
            _parametersSimplifiedView.DataContext = null;
        }

        /// <summary>
        /// Run graph
        /// </summary>
        public void RunGraph()
        {
            if (!ServiceRecipe.Instance().RecipeCurrent.IsMerged)
            {
                AdcTools.AttentionMessageBox.Show("The recipe is not merged");
                return;
            }

            // Enlève les anciens tab, 
            for (int i = _tabList.Count - 1; i >= 0; i--)
                _tabList.RemoveAt(i);

            if (ShowNodeResult)
            {
                // Add a control for each Trace/Report module
                foreach (ModuleBase module in Recipe.ModuleList.Values)
                {
                    module.ClearRenderingObjects();
                    UserControl control = module.GetUI();

                    if (control != null)
                    {
                        _tabList.Add(new TabListItem() { TabContent = control, Header = module.ToString() });
                    }
                }
            }
            if (_startupEditionMode == RecipeEditionMode.ExpertRecipeEdition)
                _tabList.Add(new TabListItem() { TabContent = _logControl, Header = "Log" });
            SelectedTab = _tabList.First();

            EditionMode = RecipeEditionMode.RecipeProcessing;
            IsRecipeRunning = true;
            Recipe.IsRendering = false;
            _timerGraph.Start();

            // Execute Recipe
            //...............

            XmlDocument xmldoc = new XmlDocument();
            Recipe.SetInputDir(CurrentFileName);
            XmlNode node = Recipe.Save(xmldoc, Recipe.IsMerged);
            xmldoc.AppendChild(node);
            using (MemoryStream stream = new MemoryStream())
            {
                xmldoc.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                byte[] data = stream.ToArray();
                _recipeId = AdcExecutor.ReprocessRecipe(data);
            }


            RefreshNodeStatistics();
        }

        public string SelectedAdaFile;

        /// <summary>
        /// Sélection d'un Ada pour l'éxecution de la recette
        /// </summary>
        public void SelectAda(bool bRun = true)
        {
            UpdateRecipeEditedState();
            Services.Services.Instance.PopUpService.ShowDialogWindow("Select Ada", new SelectAdaViewModel(this, bRun), 500, 500, true);
            UpdateRecipeEditedState();
        }

        /// <summary>
        /// Updates the current seconds display and calls
        /// InvalidateRequerySuggested on the CommandManager to force 
        /// the Command to raise the CanExecuteChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerGraph_Tick(object sender, EventArgs e)
        {
            if (IsRecipeRunning == false)
                _timerGraph.Stop();

            RefreshNodeStatistics();

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// The 'Abort' command was executed.
        /// </summary>
        public void AbortGraph()
        {
            AdcExecutor.AbortRecipe(_recipeId);
        }

        /// <summary>
        /// Selectionne les paramètres visibles en mode simplifié
        /// </summary>
        /// <param name="node"></param>
        public void ManageExports(ModuleNodeViewModel node)
        {
            Window window = new ParametersExportDialog();
            window.DataContext = node.Module;
            window.ShowDialog();

            RefreshExportedParameterList();
            SelectedModuleParameters = node.Module.ExportedParameterList;
        }

        public void ManageExports()
        {
            if (_recipeGraphVM.SelectedNode == null)
                MessageBox.Show("No node selected !");
            else
                ManageExports(_recipeGraphVM.SelectedNode);
        }

        private void StartExternalExe(string exeName)
        {
            try
            {
                Process.Start((Path.Combine(PathString.GetExecutingAssemblyPath().Directory, exeName)));
            }
            catch
            {
                AdcTools.AttentionMessageBox.Show("Error during open " + exeName);
            }
        }

        public bool CheckExternalExe(string exeName)
        {
            return File.Exists(Path.Combine(PathString.GetExecutingAssemblyPath().Directory, exeName));
        }

        #region Private Methods

        //=================================================================
        // Validation de la recette
        //=================================================================
        private void DeferredValidateRecipe()
        {
            _timerRecipeValidation.Start();
        }

        private void ValidateRecipe()
        {
            _timerRecipeValidation.Stop();

            if (_recipeGraphVM == null || Recipe == null)
                return;

            foreach (ModuleNodeViewModel n in _recipeGraphVM.Nodes)
                n.Validate();

            _recipeGraphVM.ComputeStages();
        }

        private void RefreshNodeStatistics()
        {
            if (_recipeGraphVM == null)
                return;

            if (_recipeId == null)
                return;

            RecipeStat rstat = AdcExecutor.GetRecipeStat(_recipeId);
            if (rstat == null)
                return;

            RefreshNodeStatistics(rstat);
        }

        private void RefreshNodeStatistics(RecipeStat rstat)
        {
            foreach (ModuleNodeViewModel node in _recipeGraphVM.Nodes)
                node.RefreshStatistics(rstat);

            if (!rstat.IsRunning)
                IsRecipeRunning = false;
        }

        /// <summary>
        /// Notification when the current node is deselected
        /// </summary>
        public void NodeDeselected()
        {
            SelectedModuleParameters = null;
        }

        #endregion Private Methods

        #region Commands

        private AutoRelayCommand _newRecipeCommand = null;
        public AutoRelayCommand NewRecipeCommand
        {
            get
            {
                return _newRecipeCommand ?? (_newRecipeCommand = new AutoRelayCommand(() =>
                {
                    UndoRedocommandManager.Clear();
                    NewRecipe();
                },
                    () => { return EditionMode == RecipeEditionMode.ExpertRecipeEdition; }));

            }
        }

        private AutoRelayCommand _openRecipeCommand = null;
        public AutoRelayCommand OpenRecipeCommand
        {
            get
            {
                return _openRecipeCommand ?? (_openRecipeCommand = new AutoRelayCommand(() =>
                {
                    if (LoadRecipe())
                    {
                        UndoRedocommandManager.Clear();
                    }
                },
                    () => { return !IsRecipeRunning; }));
            }
        }

        private AutoRelayCommand _openRecipeFromBaseCommand = null;
        public AutoRelayCommand OpenRecipeFromBaseCommand
        {
            get
            {
                return _openRecipeFromBaseCommand ?? (_openRecipeFromBaseCommand = new AutoRelayCommand(() =>
                {

                    var _dbToolServiceProxy=ClassLocator.Default.GetInstance<DbToolServiceProxy>();

                    var products=_dbToolServiceProxy.GetProductAndSteps();

                    var _dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();

                    var recipesList=_dbRecipeServiceProxy.GetRecipeList(ActorType.ADC, 1, 1, 3);


                    



                    var recipe = _dbRecipeServiceProxy.GetLastRecipe(recipesList[0].Key);

                    LoadRecipeFromXml(recipe.XmlContent);


                },
                    () => { return !IsRecipeRunning; }));
            }
        }

        private AutoRelayCommand _saveRecipeToBaseCommand = null;
        public AutoRelayCommand SaveRecipeToBaseCommand
        {
            get
            {
                return _saveRecipeToBaseCommand ?? (_saveRecipeToBaseCommand = new AutoRelayCommand(() =>
                {
                    SaveRecipe();
                    

                },
                    () => { return !IsRecipeRunning && Recipe != null; }));
            }
        }

        private void SaveRecipeToBase()
        {
            try
            {
                var _dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
                var dtoRecipe = new UnitySC.DataAccess.Dto.Recipe();

                dtoRecipe.Type = ActorType.ADC;

                dtoRecipe.Comment = Recipe.Comment;
                dtoRecipe.Name = Recipe.Name;
                dtoRecipe.StepId = Recipe.StepId;
                dtoRecipe.CreatorUserId = Recipe.UserId ?? 0;
                dtoRecipe.CreatorChamberId = Recipe.CreatorChamberId ?? 0;

                if (Recipe.Key != Guid.Empty)
                    dtoRecipe.KeyForAllVersion = Recipe.Key;
                else
                {
                    dtoRecipe.KeyForAllVersion = Guid.NewGuid();
                    Recipe.Key = dtoRecipe.KeyForAllVersion;
                }

                dtoRecipe.XmlContent = SaveDocumentToXml();

                foreach (var dataLoader in Recipe.DataLoaders)
                {
                    foreach (var compatipleResultType in dataLoader.CompatibleResultTypes)
                    {
                        dtoRecipe.AddInput(compatipleResultType);
                    }
                }

                // TODO Add the real outputs
                // Mandatory
                dtoRecipe.AddOutput(ResultType.ADC_Klarf);

                var id = _dbRecipeServiceProxy.SetRecipe(dtoRecipe, true);
                OnPropertyChanged(nameof(Recipe));
            }
            catch (Exception ex)
            {
#if DEBUG
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Failed to save the Recipe. " + ex.ToString()); ;
#else
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Failed to save the Recipe"); ;
#endif
            }
        }

        private AutoRelayCommand _saveRecipeCommand = null;
        public AutoRelayCommand SaveRecipeCommand
        {
            get { return _saveRecipeCommand ?? (_saveRecipeCommand = new AutoRelayCommand(() => { OnSave(false); }, () => { return !IsRecipeRunning && ServiceRecipe.Instance().MustBeSaved; })); }
        }

        private AutoRelayCommand _saveAsRecipeCommand = null;
        public AutoRelayCommand SaveAsRecipeCommand
        {
            get { return _saveAsRecipeCommand ?? (_saveAsRecipeCommand = new AutoRelayCommand(() => { OnSaveAs(); }, () => { return !IsRecipeRunning; })); }
        }

        private AutoRelayCommand _saveRecipeBdDCommand = null;
        public AutoRelayCommand SaveRecipeBdDCommand
        {
            get { return _saveRecipeBdDCommand ?? (_saveRecipeBdDCommand = new AutoRelayCommand(() => { OnSaveBdD(false); }, () => { return !IsRecipeRunning && ServiceRecipe.Instance().MustBeSaved; })); }
        }

        private AutoRelayCommand _undoRecipeCommand = null;
        public AutoRelayCommand UndoRecipeCommand
        {
            get { return _undoRecipeCommand ?? (_undoRecipeCommand = new AutoRelayCommand(() => { UndoRedocommandManager.Undo(); }, () => { return EditionMode == RecipeEditionMode.ExpertRecipeEdition; })); }
        }

        private AutoRelayCommand _redoRecipeCommand = null;
        public AutoRelayCommand RedoRecipeCommand
        {
            get { return _redoRecipeCommand ?? (_redoRecipeCommand = new AutoRelayCommand(() => { UndoRedocommandManager.Redo(); }, () => { return EditionMode == RecipeEditionMode.ExpertRecipeEdition; })); }
        }

        private AutoRelayCommand _runGraphCommand = null;
        public AutoRelayCommand RunGraphCommand
        {
            get
            {
                return _runGraphCommand ?? (_runGraphCommand = new AutoRelayCommand(
              () =>
              {
                  if (IsRecipeRunning)
                      AbortGraph();
                  else
                      SelectAda();
              },
              () =>
              {
                  return _recipeGraphVM.GraphContainsNodes;
              }));
            }
        }


        private AutoRelayCommand _openMainHelpCommand;
        public AutoRelayCommand OpenMainHelpCommand
        {
            get
            {
                return _openMainHelpCommand ?? (_openMainHelpCommand = new AutoRelayCommand(
              () =>
              {
                  ADCHelpDisplay.OpenMainHelp();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _selectRunViewCommand = null;
        public AutoRelayCommand SelectRunViewCommand
        {
            get
            {
                return _selectRunViewCommand ?? (_selectRunViewCommand = new AutoRelayCommand(
            () =>
            {
                if (!IsRecipeRunning)
                    SelectAda();
                UpdateRecipeEditedState();

            },
            () =>
            {
                return _recipeGraphVM.GraphContainsNodes;
            }));
            }
        }

        private AutoRelayCommand _exitAppCommand = null;
        public AutoRelayCommand ExitAppCommand
        {
            get
            {
                return _exitAppCommand ?? (_exitAppCommand = new AutoRelayCommand(() =>
                {
                    Services.Services.Instance.ShutdownService.ShutdownApp();
                }, () => { return true; }));
            }
        }

        private AutoRelayCommand _exitEditorCommand = null;
        public AutoRelayCommand ExitEditorCommand
        {
            get
            {
                return _exitEditorCommand ?? (_exitEditorCommand = new AutoRelayCommand(() =>
                {


                    bool allowShutdown = true;




                    RecipeViewModel recipeVM = ViewModelLocator.Instance.MainWindowViewModel.MainViewViewModel as RecipeViewModel;
                    if (recipeVM != null)
                    {
                        allowShutdown = OnSaveBdD(true);
                    }

                    if (allowShutdown)
                    {
                        // arret autorisé
                        EmbeddedView.CloseEditor();

                    }
                    else
                    {

                        // arret refusé.
                    }


                }, () => { return true; }));
            }
        }

        private AutoRelayCommand _showAboutCommand = null;
        public AutoRelayCommand ShowAboutCommand
        {
            get
            {
                return _showAboutCommand ?? (_showAboutCommand = new AutoRelayCommand(() =>
                {
                    new Header.About().ShowDialog();
                }, () => { return true; }));
            }
        }

        private AutoRelayCommand _simplifiedViewCommand = null;
        public AutoRelayCommand SimplifiedViewCommand
        {
            get
            {
                return _simplifiedViewCommand ?? (_simplifiedViewCommand = new AutoRelayCommand(
            () =>
            {
                SwitchMode();
                //graphControl.EnableNodeDragging = (ViewModel.EditionMode == RecipeEditionMode.ExpertRecipeEdition); ==>
                UpdateRecipeEditedState();
            },
            () => { return CanSwitchSimplifiedView(); }));
            }
        }

        private AutoRelayCommand _replayCommand;
        public AutoRelayCommand ReplayCommand
        {
            get
            {
                return _replayCommand ?? (_replayCommand = new AutoRelayCommand(
                () =>
                {
                    RunGraph();
                },
              () => { return (Recipe != null ? Recipe.IsMerged : false) && !IsRecipeRunning; }));
            }
        }

        private AutoRelayCommand _openADCConfigurationCommand;
        public AutoRelayCommand OpenADCConfigurationCommand
        {
            get
            {
                return _openADCConfigurationCommand ?? (_openADCConfigurationCommand = new AutoRelayCommand(
              () =>
              {
                  StartExternalExe(ADCConfigurationFileName);
              },
              () => { return CheckExternalExe(ADCConfigurationFileName); }));
            }
        }


        private AutoRelayCommand _openConfigurationManagerCommand;
        public AutoRelayCommand OpenConfigurationManagerCommand
        {
            get
            {
                return _openConfigurationManagerCommand ?? (_openConfigurationManagerCommand = new AutoRelayCommand(
              () =>
              {
                  StartExternalExe(ConfigurationManagerFileName);
              },
              () => { return CheckExternalExe(ConfigurationManagerFileName); }));
            }
        }


        private AutoRelayCommand _openADCProdCommand;
        public AutoRelayCommand OpenADCProdCommand
        {
            get
            {
                return _openADCProdCommand ?? (_openADCProdCommand = new AutoRelayCommand(
              () =>
              {
                  StartExternalExe(ADCProdFileName);
              },
              () => { return CheckExternalExe(ADCProdFileName); }));
            }
        }

#endregion

        internal void NotifyUpdateRecipe()
        {
            OnPropertyChanged(nameof(Recipe));
        }

        internal bool CanCloseRecipe()
        {
            if (ServiceRecipe.Instance().MustBeSaved)
            {
                MessageBoxResult result = MessageBox.Show($"Do you want to save the Recipe : {Recipe.Name} ?", Properties.Resources.ApplicationLabel, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    SaveRecipe();
                }
            }
            return true;
        }

        private void SaveRecipe()
        {
            try
            {
                if (_recipeGraphVM.Nodes.Count() > 1) // au moins un dataloader
                {
                    if (Recipe.IsOkBase())
                    {
                        var moduleConf = ClassLocator.Default.GetInstance<ModuleConfiguration>();
                        bool isRecipeAlreadyInDb = Recipe.Key != Guid.Empty;
                        bool isNameAlreadyTaken = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>()
                            .GetRecipeList(ActorType.ADC, Recipe.StepId.GetValueOrDefault(0), moduleConf.ChamberKey,
                                moduleConf.ToolKey)
                            .Any(recipe => recipe.Name == Recipe.Name && (!isRecipeAlreadyInDb || recipe.Key != Recipe.Key));

                        if (isNameAlreadyTaken)
                        {
                            MessageBox.Show($"The recipe name '{Recipe.Name}' is already used.", "Save Recipe",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        SaveRecipeToBase();                                              
                    }
                    else
                    {
                        OnSave(false);
                    }
                    ServiceRecipe.Instance().MustBeSaved = false;
                    MessageBox.Show($"Recipe '{Recipe.Name}' has been saved successfully.", "Save Confirmation", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show($"Add a module before save the Recipe.");
                }
            } 
            // The only exception that can be caught here is CommunicationException. We explicit it for maintanability goals
            catch (Exception ex) when (ex.InnerException is CommunicationException)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>()
                    .ShowException(ex, "The recipe could not be saved. Database is unreachable.");
            }
        }
    }

}

