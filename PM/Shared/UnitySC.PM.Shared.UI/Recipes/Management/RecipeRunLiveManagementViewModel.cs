using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Recipes.Management
{
    public class RecipeRunLiveManagementViewModel : ObservableRecipient
    {
        private RecipeRunLiveViewModel _recipeRunLiveViewModel;

        public RecipeRunLiveManagementViewModel()
        {
        }

        private ActorType _actor;

        public ActorType Actor
        {
            get => _actor;
            set
            {
                if (_actor != value)
                {
                    _actor = value;
                    RecipeRunLiveViewModel.Actor = _actor;
                    OnPropertyChanged();
                }
            }
        }

        public RecipeRunLiveViewModel RecipeRunLiveViewModel
        {
            get
            {
                if (_recipeRunLiveViewModel == null)
                {
                    _recipeRunLiveViewModel = ClassLocator.Default.GetInstance<RecipeRunLiveViewModel>();
                }
                return _recipeRunLiveViewModel;
            }
        }

        public void Display()
        {
            RecipeRunLiveViewModel.Display();
        }

        public void Hide()
        {
            RecipeRunLiveViewModel.Hide();
        }
    }
}
