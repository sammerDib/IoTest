using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.UI.Recipes.Management;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace TestRecipeRunLiveView
{
    public class TestLiveMainViewModel : ObservableRecipient
    {






        private RecipeRunLiveManagementViewModel _recipeRunLive;

        public RecipeRunLiveManagementViewModel RecipeRunLive
        {
            get => _recipeRunLive; set { if (_recipeRunLive != value) { _recipeRunLive = value; OnPropertyChanged(); } }
        }



     
        public TestLiveMainViewModel()
        {
            RecipeRunLive= ClassLocator.Default.GetInstance<RecipeRunLiveManagementViewModel>();
            RecipeRunLive.Actor = ActorType.ANALYSE;
            RecipeRunLive.Display();
        }
    }
}
