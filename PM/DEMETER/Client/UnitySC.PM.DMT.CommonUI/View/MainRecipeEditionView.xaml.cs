using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.CommonUI.View
{
    /// <summary>
    ///     Interaction logic for MainRecipeEditionView.xaml
    /// </summary>
    [Export(typeof(IRecipeEditorUc))]
    [UCMetadata(ActorType = ActorType.DEMETER)]
    public partial class MainRecipeEditionView : UserControl, IRecipeEditorUc
    {
        // Using a DependencyProperty as the backing store for UsageMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExecutionModeProperty =
            DependencyProperty.Register("ExecutionMode", typeof(ExecutionModes), typeof(MainRecipeEditionView),
                new PropertyMetadata(ExecutionModes.Integrated, OnExecutionModeChanged));

        private readonly IDialogOwnerService _dialogService;
        private readonly IRecipeManager _recipeManager;
        private readonly RecipeSupervisor _recipeSupervisor;

        public MainRecipeEditionView()
        {
            Console.WriteLine("MainRecipeEditionView Constructor");
            DataContext = ClassLocator.Default.GetInstance<MainRecipeEditionVM>();
            InitializeComponent();
            _recipeManager = ClassLocator.Default.GetInstance<IRecipeManager>();
            _recipeSupervisor = ClassLocator.Default.GetInstance<RecipeSupervisor>();
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
        }

        public ExecutionModes ExecutionMode
        {
            get => (ExecutionModes)GetValue(ExecutionModeProperty);
            set => SetValue(ExecutionModeProperty, value);
        }

        public event EventHandler ExitEditor;

        public ActorType ActorType { get; }

        private static void OnExecutionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                try
                {
                    var executionModeService = ClassLocator.Default.GetInstance<IExecutionMode>();
                    executionModeService.CurrentExecutionMode = (ExecutionModes)e.NewValue;
                }
                catch (Exception)
                {
                }
            }
        }

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
        }

        public void LoadRecipe(Guid key)
        {
            _recipeManager.SetEditedRecipe(key, false);
        }

        private AutoRelayCommand _exitRecipeEdition;

        /// <summary>
        ///     Gets the ExitRecipeEdition.
        /// </summary>
        public AutoRelayCommand ExitRecipeEdition
        {
            get
            {
                return _exitRecipeEdition ?? (_exitRecipeEdition = new AutoRelayCommand(() =>
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

        public async Task ExportRecipeAsync(Guid key)
        {
            var folderBrowserDialogSettings = new FolderBrowserDialogSettings();
            bool? success = _dialogService.ShowFolderBrowserDialog(folderBrowserDialogSettings);
            if (success.GetValueOrDefault(false))
            {
                await Task.Run(() => _recipeManager.ExportRecipe(key, folderBrowserDialogSettings.SelectedPath));
                _dialogService.ShowMessageBox($"Export to {folderBrowserDialogSettings.SelectedPath} successful.", "Export successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async Task<RecipeInfo> ImportRecipeAsync(int stepId, int userId)
        {
            RecipeInfo info = null;
            var settings = new OpenFileDialogSettings
            {
                Title = "Open DEMETER recipe", Filter = "DEMETER recipe (*.dmtrcp)|*.dmtrcp", Multiselect = false
            };

            bool? success = _dialogService.ShowOpenFileDialog(settings);
            if (success.GetValueOrDefault(false))
            {
                try
                {
                    await Task.Run(() => info = _recipeManager.ImportRecipe(stepId, userId, settings.FileName));
                }
                catch (Exception e)
                {
                    _dialogService.ShowMessageBox(e.Message, "Error while importing recipe", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }

            return info;
        }

        #endregion IRecipeEditorUC implementation
    }
}
