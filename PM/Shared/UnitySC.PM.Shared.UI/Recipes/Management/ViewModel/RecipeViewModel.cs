using System;
using System.Windows;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.UC;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class RecipeViewModel : TreeViewItemViewModel
    {
        private RecipeInfo _recipeInfo;
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;

        public RecipeInfo RecipeInfo 
        {
            get { return _recipeInfo; } 
            set { _recipeInfo = value;  Update(value);  }
        }
        public RecipeViewModel() : base()
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
        }
        public RecipeViewModel(RecipeInfo recipeInfo) : base()
        {
            _recipeInfo = recipeInfo;
            Init(recipeInfo);
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
        }
        public RecipeViewModel(TreeViewItemViewModel parent, RecipeInfo recipeInfo) : base(parent, false)
        {
            _recipeInfo = recipeInfo;
            Init(recipeInfo);
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
        }

        private void Init(RecipeInfo recipeInfo)
        {
            Text = recipeInfo?.Name;
            if (recipeInfo.IsTemplate)
                RecipeState = RecipeState.Template;
            else if (recipeInfo.IsShared)
                RecipeState = RecipeState.Shared;
            else
                RecipeState = RecipeState.Local;
        }
        internal void Update(RecipeInfo recipeInfo)
        {
            var oldRecipe = _recipeInfo;
            _recipeInfo = recipeInfo;
            if (oldRecipe.Name != _recipeInfo.Name)
                OnPropertyChanged(nameof(Name));
            if (oldRecipe.Key != _recipeInfo.Key)
                OnPropertyChanged(nameof(Key));
            HasChanged = false;
        }


        public void NotifiyNameUpdate()
        {
            OnPropertyChanged(nameof(Name));
        }


        public string Name => _recipeInfo.Name;

        public Guid Key => _recipeInfo.Key;

        public ActorType ActorType => _recipeInfo.ActorType;

        public string Text { get; set; }
        public RecipeState RecipeState { get; set; }

        private bool _isShared;

        public bool IsShared
        {
            get => _isShared;
            set
            {
                if (_isShared != value)
                {
                    _isShared = value;
                    bool hasChangedBefore = HasChanged;
                    OnPropertyChanged();
                    HasChanged = hasChangedBefore;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public IRecipeSummaryUc CurrentRecipeSummaryUC
        {
            get
            {
                var recipeSummary = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeSummary(_recipeInfo.ActorType);
                if (recipeSummary != null)
                {
                    recipeSummary.Init(true);
                    recipeSummary.LoadRecipe(_recipeInfo.Key);
                }

                return recipeSummary;
            }
        }

        //
        public void RefreshRecipeSummary()
        {
            var recipeSummary = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeSummary(_recipeInfo.ActorType);
            if (recipeSummary != null)
            {
                recipeSummary.Refresh();
            }
        }

        public override void Archive()
        {
            if (_recipeInfo.Key != Guid.Empty)
                _dbRecipeService.Invoke(x => x.ArchiveAllVersionOfRecipe(_recipeInfo.Key, ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id));
        }
        public override bool CanUserRemove()
        {
            try
            {
                var recipe = _dbRecipeService.Invoke(x => x.GetLastRecipe(this.Key, false, false));
                // TODO Add User management
                //var user = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser;
                //if (recipe.CreatorUserId != user.Id
                //    || !user.Rights.Contains(UserRights.RecipeEdition))
                //{
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Impossible to remove this recipe: " + Name);
                return false;
            }
            return true;
        }
    }
}
