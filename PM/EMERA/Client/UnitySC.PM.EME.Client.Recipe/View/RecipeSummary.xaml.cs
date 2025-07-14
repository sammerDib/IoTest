using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.PM.EME.Client.Proxy.Recipe;
using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Recipe.View
{
    /// <summary>
    /// Interaction logic for RecipeSummary.xaml
    /// </summary>
    [Export(typeof(IRecipeSummaryUc))]
    [UCMetadata(ActorType = ActorType.EMERA)]
    public partial class RecipeSummary : UserControl, IRecipeSummaryUc
    {
        #region Private Fields

        private IEMERecipeService _recipeSupervisor;
        private ILogger _logger;

        #endregion Private Fields
        public RecipeSummary()
        {
            InitializeComponent();
            _recipeSupervisor = ClassLocator.Default.GetInstance<IEMERecipeService>();
            _logger = ClassLocator.Default.GetInstance<ILogger>();

            DataContext = new RecipeSummaryVM();
        }
        public ActorType ActorType => ActorType.EMERA;

        public void Init(bool isStandalone)
        {
        }

        public void LoadRecipe(Guid key)
        {
            var viewModel = (this.DataContext as RecipeSummaryVM);
            viewModel.IsBusy = true;

            Task.Run(() =>
            {
                EMERecipe recipe = null;

                try
                {
                    recipe = _recipeSupervisor.GetRecipeFromKey(key)?.Result;
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
    }
}
