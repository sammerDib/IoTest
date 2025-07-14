using System.ComponentModel.Composition;
using System.Windows.Controls;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;
using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.ANA.Client.CommonUI.View
{
    /// <summary>
    /// Logique d'interaction pour ANARecipeRunLiveUC.xaml
    /// </summary>
    [Export(typeof(IRecipeRunLiveViewUc))]
    [UCMetadata(ActorType = ActorType.ANALYSE)]
    public partial class ANARecipeRunLiveUC : UserControl, IRecipeRunLiveViewUc
    {
        #region Private Fields
        private ANARecipeRunLiveVM _recipeRunLiveViewModel;

        #endregion Private Fields


        #region Public Constructors

        public ANARecipeRunLiveUC()
        {
            InitializeComponent();
            _recipeRunLiveViewModel=new ANARecipeRunLiveVM();
            DataContext = _recipeRunLiveViewModel;
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

        public void Display()
        {
              _recipeRunLiveViewModel.StartCamera();
        }

        public void Hide()
        {
            _recipeRunLiveViewModel.StopCamera();
        }
        #endregion Public Methods
    }
}
