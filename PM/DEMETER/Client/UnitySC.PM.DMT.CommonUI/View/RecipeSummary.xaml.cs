using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.UC;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.CommonUI.View
{
    /// <summary>
    /// Logique d'interaction pour RecipeSummary.xaml
    /// </summary>
    [Export(typeof(IRecipeSummaryUc))]
    [UCMetadata(ActorType = ActorType.DEMETER)]
    public partial class RecipeSummary : UserControl, IRecipeSummaryUc
    {
        #region Private Fields

        private RecipeSupervisor _recipeSupervisor;

        #endregion Private Fields

        #region Public Constructors

        public RecipeSummary()
        {
            InitializeComponent();
            _recipeSupervisor = ClassLocator.Default.GetInstance<RecipeSupervisor>();
            var userSupervisor = ClassLocator.Default.GetInstance<UserSupervisor>();
            var toolServiceLogger = ClassLocator.Default.GetInstance<ILogger<IToolService>>();
            DataContext = new RecipeSummaryVM(userSupervisor, toolServiceLogger);
        }

        #endregion Public Constructors

        #region Public Properties

        public ActorType ActorType => ActorType.DEMETER;

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
                //Stopwatch chrono = new Stopwatch();
                //chrono.Start();

                DMTRecipe recipe = null;

                try
                {
                    recipe = _recipeSupervisor.GetLastRecipeWithProductAndStep(key);
                }
                catch (Exception ex) when (ex.Data.Contains(RecipeLoadImportExceptionDataKeys.LoadCheckErrors) ||
                                           ex.Data.Contains(RecipeLoadImportExceptionDataKeys.LoadErrors))
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(MainRecipeEditionVM.GetMessageFromRecipeLoadImportException(ex), "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                }
                catch (Exception ex)
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (recipe == null)
                                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                    }
                }
                //chrono.Stop();
                //Console.WriteLine($"GetRecipe executed in {chrono.ElapsedMilliseconds}ms");
                finally
                {
                    viewModel.DisplayedRecipe = recipe;
                    viewModel.IsBusy = false;
                }
            });
        }

        public void Refresh()
        {
            var viewModel = (this.DataContext as RecipeSummaryVM);
            if (viewModel.DisplayedRecipe is null)
                return;
            LoadRecipe(viewModel.DisplayedRecipe.Key);
        }

        #endregion Public Methods

        private void CollectionViewSource_FilterFront(object sender, FilterEventArgs e)
        {
            var measure = e.Item as MeasureSummaryVM;
            if (measure != null)
            {
                if (measure.Side == Side.Front)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void CollectionViewSource_FilterBack(object sender, FilterEventArgs e)
        {
            var measure = e.Item as MeasureSummaryVM;
            if (measure != null)
            {
                if (measure.Side == Side.Back)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }
    }
}
