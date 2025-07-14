using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.CommonUI.View
{
    /// <summary>
    /// Logique d'interaction pour RecipeSummary.xaml
    /// </summary>
    [Export(typeof(IRecipeSummaryUc))]
    [UCMetadata(ActorType = ActorType.ANALYSE)]
    public partial class RecipeSummary : UserControl, IRecipeSummaryUc
    {
        #region Private Fields

        private ANARecipeSupervisor _recipeSupervisor;
        private ILogger _logger;

        #endregion Private Fields

        //private IRecipeManager _recipeManager;

        #region Public Constructors

        public RecipeSummary()
        {
            InitializeComponent();
            _recipeSupervisor = ServiceLocator.ANARecipeSupervisor;
            _logger = ClassLocator.Default.GetInstance<ILogger>();

            DataContext = new RecipeSummaryVM();
        }

        #endregion Public Constructors

        #region Public Properties

        public ActorType ActorType => ActorType.ANALYSE;

        #endregion Public Properties

        #region Public Methods

        public void Init(bool isStandalone)
        {
            // TODO
        }

        public void LoadRecipe(Guid key)
        {
            var viewModel = (this.DataContext as RecipeSummaryVM);
            viewModel.IsBusy = true;

            Task.Run(() =>
            {
                ANARecipe recipe = null;

                try
                {
                    recipe = _recipeSupervisor.GetRecipe(key, false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during get recipe");
                }
                finally
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (recipe == null)
                            {
                                MessageBox.Show("Failed to load the recipe", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                viewModel.IsBusy = false;
                                return;
                            }

                            viewModel.Update(recipe);
                            viewModel.IsBusy = false;
                        }));
                    }
                }
            });
        }

        public void Refresh()
        {
            var viewModel = (this.DataContext as RecipeSummaryVM);
            LoadRecipe(viewModel.DisplayedRecipe.Key);
        }

        #endregion Public Methods
    }
}
