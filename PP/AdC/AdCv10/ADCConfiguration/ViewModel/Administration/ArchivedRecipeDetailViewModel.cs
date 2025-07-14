using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Utils.ViewModel;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel.Administration
{
    public class ArchivedRecipeDetailViewModel : ValidationViewModelBase
    {
        // Liste des autres version de la recette associées à la recette courante.
        public List<ArchivedRecipeDetailViewModel> AllRecipeVersions { get; private set; }
        public Dto.Recipe Recipe { get; private set; }

        public ArchivedRecipeDetailViewModel(Dto.Recipe recipe)
        {
            Recipe = recipe;
        }

        public bool IsArchived
        {
            get => Recipe.IsArchived; set { if (Recipe.IsArchived != value) { Recipe.IsArchived = value; OnPropertyChanged(); } }
        }

        public void Init()
        {
            if (AllRecipeVersions != null)
                return;

            IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var recipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
                    AllRecipeVersions = recipeProxy.GetADCRecipes(Recipe.Name, true, true).Select(x => new ArchivedRecipeDetailViewModel(x)).ToList();

                    foreach (var recipeVersion in AllRecipeVersions)
                    {
                        recipeVersion.PropertyChanged += RecipeVersion_PropertyChanged; ;
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        OnPropertyChanged(nameof(AllRecipeVersions));
                        DisableAllCommand.NotifyCanExecuteChanged();

                    });
                    HasChanged = false;
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Refresh recipe archive detail", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Refresh error: ", ex); }));
                }
                finally
                {
                    IsBusy = false;
                    HasChanged = false;
                }
            });
        }

        private void RecipeVersion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasChanged")
            {
                HasChanged = true;
                ArchivedRecipeDetailViewModel recipeVersion = sender as ArchivedRecipeDetailViewModel;

                // Si la recette séléectionné correspond à la recette modifier au met à jour l'état de la recette séléectionné
                if (recipeVersion != null && Recipe.Id == recipeVersion.Recipe.Id)
                    IsArchived = recipeVersion.IsArchived;
            }
        }

        private void EnableAll()
        {
            AllRecipeVersions.ForEach(x => x.IsArchived = false);
        }

        private void DisableAll()
        {
            AllRecipeVersions.ForEach(x => x.IsArchived = true);
        }

        #region command

        private AutoRelayCommand _enableAllCommand;
        public AutoRelayCommand EnableAllCommand
        {
            get
            {
                return _enableAllCommand ?? (_enableAllCommand = new AutoRelayCommand(
              () =>
              {
                  EnableAll();
              },
              () => { return AllRecipeVersions != null && !AllRecipeVersions.All(x => !x.IsArchived); }));
            }
        }


        private AutoRelayCommand _disableAllCommand;
        public AutoRelayCommand DisableAllCommand
        {
            get
            {
                return _disableAllCommand ?? (_disableAllCommand = new AutoRelayCommand(
              () =>
              {
                  DisableAll();
              },
              () => { return AllRecipeVersions != null && !AllRecipeVersions.All(x => x.IsArchived); }));
            }
        }

        #endregion
    }
}
