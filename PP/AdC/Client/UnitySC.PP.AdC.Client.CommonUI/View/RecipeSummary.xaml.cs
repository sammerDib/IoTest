using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

using UnitySC.PM.Shared.UC;
using UnitySC.PP.ADC.Client.Proxy.Recipe;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PP.ADC.Client.CommonUI.View
{
    /// <summary>
    /// Logique d'interaction pour RecipeSummary.xaml
    /// </summary>
    [Export(typeof(IRecipeSummaryUc))]
    [UCMetadata(ActorType = ActorType.ADC)]
    public partial class RecipeSummary : UserControl, IRecipeSummaryUc
    {
        #region Private Fields

        private ADCRecipeSupervisor _recipeSupervisor;

        #endregion Private Fields

        //private IRecipeManager _recipeManager;

        #region Public Constructors

        public RecipeSummary()
        {
            InitializeComponent();
            _recipeSupervisor = ServiceLocator.ADCRecipeSupervisor;

            //DataContext = new RecipeSummaryVM();
        }

        #endregion Public Constructors

        #region Public Properties

        public ActorType ActorType => ActorType.ADC;

        #endregion Public Properties

        #region Public Methods

        public void Init(bool isStandalone)
        {
            // TODO
        }

        public void LoadRecipe(Guid key)
        {
            /*
                var viewModel = (this.DataContext as RecipeSummaryVM);
                viewModel.IsBusy = true;

                Task.Run(() =>
                {

                    ANARecipe recipe = null;

                    try

                    {
                        recipe = _recipeSupervisor.GetRecipe(key);
                    }
                    catch (Exception)
                    {
                    }
                    //chrono.Stop();
                    //Console.WriteLine($"GetRecipe executed in {chrono.ElapsedMilliseconds}ms");
                    finally
                    {
                        if (Application.Current != null)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                if (recipe == null)
                                    MessageBox.Show("Failed to load the recipe", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                                viewModel.DisplayedRecipe = recipe;
                                viewModel.IsBusy = false;
                            }));
                        }
                    }
                });
            */
        }

        public void Refresh()
        {

            /*
             * var viewModel = (this.DataContext as RecipeSummaryVM);
                LoadRecipe(viewModel.DisplayedRecipe.Key);
            */
        }



        #endregion Public Methods


    }
}

