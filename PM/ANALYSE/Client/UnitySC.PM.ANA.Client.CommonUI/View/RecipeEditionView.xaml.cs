using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel;
using UnitySC.PM.ANA.Client.Proxy.Recipe;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.PM.Shared.UI.Main;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Client.CommonUI.View
{
    /// <summary>
    /// Interaction logic for RecipeEditionView.xaml
    /// </summary>
    [Export(typeof(IRecipeEditorUc))]
    [UCMetadata(ActorType = ActorType.ANALYSE)]
    public partial class RecipeEditionView : UserControl, IRecipeEditorUc
    {
        private IRecipeManager _recipeManager;
        private ANARecipeSupervisor _recipeSupervisor;

        public RecipeEditionView()
        {
            InitializeComponent();
            _recipeManager = ServiceLocator.RecipeManager;
            _recipeSupervisor = ServiceLocator.ANARecipeSupervisor;
            this.DataContext = ClassLocator.Default.GetInstance<RecipeEditionVM>();
        }

        public bool IsEditingRecipeName
        {
            get { return (bool)GetValue(IsEditingRecipeNameProperty); }
            set { SetValue(IsEditingRecipeNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditingRecipeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditingRecipeNameProperty =
            DependencyProperty.Register(nameof(IsEditingRecipeName), typeof(bool), typeof(RecipeEditionView), new PropertyMetadata(false));

        public ActorType ActorType { get; }

        public event EventHandler ExitEditor;

        #region IRecipeEditorUC implementation

        public bool CanClose()
        {
            return _recipeManager.CanClose();
        }

        public RecipeInfo CreateNewRecipe(string name, int stepId, int userId)
        {
            var recipe = _recipeSupervisor.CreateRecipe(name, stepId, userId);
            _recipeSupervisor.SaveRecipe(recipe);
            return recipe;
        }

        public void Init(bool isStandalone)
        {
            //if (isStandalone)
            //    ExecutionMode = ExecutionModes.StandAlone;
            //else
            //    ExecutionMode = ExecutionModes.Integrated;
        }

        public void LoadRecipe(Guid key)
        {
            _recipeManager.SetEditedRecipe(key, false);
        }

        private AutoRelayCommand _exitRecipeEdition;

        /// <summary>
        /// Gets the ExitRecipeEdition.
        /// </summary>
        public AutoRelayCommand ExitRecipeEdition
        {
            get
            {
                return _exitRecipeEdition
                    ?? (_exitRecipeEdition = new AutoRelayCommand(
                    () =>
                    {
                        if (_recipeManager.CanClose())
                            Exit();
                    }));
            }
        }

        public void Exit()
        {
            OnExitEditor();
        }

        protected virtual void OnExitEditor()
        {
            ExitEditor?.Invoke(this, null);
        }

        #endregion IRecipeEditorUC implementation

        private AutoRelayCommand _startRecipeNameEdition;

        public AutoRelayCommand StartRecipeNameEdition
        {
            get
            {
                return _startRecipeNameEdition ?? (_startRecipeNameEdition = new AutoRelayCommand(
                    () =>
                    {
                        // Code to execute
                        IsEditingRecipeName = true;
                    },
                    () => { return true; }
                ));
            }
        }

        private void TexBoxRecipeName_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditingRecipeName = false;
        }

        private void RecipeEdition_Loaded(object sender, RoutedEventArgs e)
        {
            // We go to the first page
            ServiceLocator.NavigationManager.NavigateToPage(ServiceLocator.NavigationManager.GetFirstPage());
        }

        public async Task ExportRecipeAsync(Guid key)
        {
            var folderBrowserDialogSettings = new FolderBrowserDialogSettings();
            bool? success = false;
            success = ServiceLocator.DialogService.ShowFolderBrowserDialog(folderBrowserDialogSettings);
            
            if (success.Value)
            {
                await Task.Run(() => _recipeManager.ExportRecipe(key, folderBrowserDialogSettings.SelectedPath));
               
                ServiceLocator.DialogService.ShowMessageBox($"Export to {folderBrowserDialogSettings.SelectedPath} successful.", "Export successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }    
        }

        public async Task<RecipeInfo> ImportRecipeAsync(int stepId, int userId)
        {
            RecipeInfo info = null;
            var settings = new OpenFileDialogSettings
            {
                Title = "Open ANALYSE recipe",
                Filter = "ANALYSE recipe (*.anarx)|*.anarx",
            };


            bool? success=false;
              success=ServiceLocator.DialogService.ShowOpenFileDialog(settings);
 
            
            if (success==true)
            {
                await Task.Run(()=> info = _recipeManager.ImportRecipe(stepId, userId, settings.FileName));
            }
            return info;
        }
      
    }
}
