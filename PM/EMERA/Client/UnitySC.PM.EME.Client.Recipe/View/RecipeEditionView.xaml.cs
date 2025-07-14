using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Recipe.View
{
    /// <summary>
    /// Interaction logic for RecipeEditionView.xaml
    /// </summary>
    [Export(typeof(IRecipeEditorUc))]
    [UCMetadata(ActorType = ActorType.EMERA)]
    public partial class RecipeEditionView : UserControl, IRecipeEditorUc
    {
        private readonly IRecipeManager _recipeManager;
        private readonly IEMERecipeService _recipeSupervisor;
        private readonly IDialogOwnerService _dialogService;
        public RecipeEditionView()
        {
            InitializeComponent();
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            _recipeManager = ClassLocator.Default.GetInstance<IRecipeManager>();
            _recipeSupervisor = ClassLocator.Default.GetInstance<IEMERecipeService>();
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

        public bool CanClose()
        {
            return _recipeManager.CanClose();
        }

        public RecipeInfo CreateNewRecipe(string name, int stepId, int userId)
        {
            var recipe = _recipeSupervisor.CreateRecipe(name, stepId, userId)?.Result;
            _recipeSupervisor.SaveRecipe(recipe, true, userId);
            return recipe;
        }

        public async Task ExportRecipeAsync(Guid key)
        {
            var folderBrowserDialogSettings = new FolderBrowserDialogSettings();
            var success = _dialogService.ShowFolderBrowserDialog(folderBrowserDialogSettings);
            if (success.Value)
            {
                await Task.Run(() => _recipeManager.ExportRecipe(key, folderBrowserDialogSettings.SelectedPath));
                _dialogService.ShowMessageBox($"Export to {folderBrowserDialogSettings.SelectedPath} successful.", "Export sucessful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async Task<RecipeInfo> ImportRecipeAsync(int stepId, int userId)
        {
            RecipeInfo info = null;
            var settings = new OpenFileDialogSettings
            {
                Title = "Open EMERA recipe",
                Filter = "EMERA recipe (*.emer)|*.emer",
            };

            var success = _dialogService.ShowOpenFileDialog(settings);
            if (success.Value)
            {
                await Task.Run(() => info = _recipeManager.ImportRecipe(stepId, userId, settings.FileName));
            }
            return info;
        }

        public void Init(bool isStandalone)
        {
        }

        public void LoadRecipe(Guid key)
        {
            try
            {
                _recipeManager.SetEditedRecipe(key, false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }           
        }
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
    }
}
