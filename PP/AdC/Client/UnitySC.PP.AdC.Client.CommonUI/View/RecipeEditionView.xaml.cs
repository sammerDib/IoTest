using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using ADC;

using UnitySC.PM.Shared.UC;
using UnitySC.PP.ADC.Client.Proxy.Recipe;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PP.ADC.Client.CommonUI.View
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    [Export(typeof(IRecipeEditorUc))]
    [UCMetadata(ActorType = ActorType.ADC)]
    public partial class RecipeEditionView : UserControl, IRecipeEditorUc
    {


        //private IRecipeManager _recipeManager;
        private ADCRecipeSupervisor _recipeSupervisor;


        public RecipeEditionView()
        {
            InitializeComponent();

            _recipeSupervisor = ServiceLocator.ADCRecipeSupervisor;

             EmbeddedView.InitParam(
                (p) =>
                {
                    //note de rti : marche pas car pas set
                    string tmp = ConfigurationManager.AppSettings[p];
                    return tmp;
                }
                );


            EmbeddedView.InitCloseEditor(() =>
            {
                Exit();

                return true;
            });


            EmbeddedView.InitSaveRecipe((key, xml) =>
            {
                SaveRecipe(key, xml);

                return true;
            });

            MainContentPresenter.Content = new EmbeddedView();

            EmbeddedView.Init();





            // _recipeManager = ServiceLocator.RecipeManager;
            //this.DataContext = ClassLocator.Default.GetInstance<RecipeEditionVM>();
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
            return true; // _recipeManager.CanClose();
        }

        public RecipeInfo CreateNewRecipe(string name, int stepId, int userId)
        {

            var recipeADC = _recipeSupervisor.CreateRecipe(name, stepId, userId);

            return recipeADC;
        }


        public RecipeInfo SaveRecipe(Guid key, string xml)
        {

            Service.Interface.Recipe.ADCRecipe r = _recipeSupervisor.GetRecipe(key);
            r.ADCEngineRecipeXml = xml;

            _recipeSupervisor.SaveRecipe(r);

            return r;
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

            Service.Interface.Recipe.ADCRecipe r = _recipeSupervisor.GetRecipe(key);

            string xml = r.ADCEngineRecipeXml;

            EmbeddedView.LoadRecipe(key, xml);

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
                        // if (_recipeManager.CanClose())
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
            //ServiceLocator.NavigationManager.NavigateToPage(ServiceLocator.NavigationManager.GetFirstPage());
        }

        public async Task ExportRecipeAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        public async Task<RecipeInfo> ImportRecipeAsync(int stepId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
