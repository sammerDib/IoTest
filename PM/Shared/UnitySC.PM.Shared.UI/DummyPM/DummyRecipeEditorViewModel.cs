using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UI.DummyPM
{
    public class DummyRecipeEditorViewModel : ObservableObject
    {
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;
        private UnitySC.DataAccess.Dto.Recipe _recipe;


        public DummyRecipeEditorViewModel(ServiceInvoker<IDbRecipeService> dbRecipeService)
        {
            _dbRecipeService = dbRecipeService;
        }

        public void LoadRecipe(Guid key)
        {
            _recipe = _dbRecipeService.Invoke(x => x.GetLastRecipe(key, false, false));
            OnPropertyChanged(nameof(Version));
            OnPropertyChanged(nameof(RecipeName));
        }

        private void ReloadRecipe()
        {
            _recipe = _dbRecipeService.Invoke(x => x.GetLastRecipe(_recipe.KeyForAllVersion, false, false));
            OnPropertyChanged(nameof(Version));
            OnPropertyChanged(nameof(RecipeName));
        }

        private AutoRelayCommand _saveNewVersionCommand;
        public AutoRelayCommand SaveNewVersionCommand
        {
            get
            {
                return _saveNewVersionCommand ?? (_saveNewVersionCommand = new AutoRelayCommand(
              () =>
              {
                  _dbRecipeService.Invoke(x => x.SetRecipe(_recipe, true));
                  ReloadRecipe();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _saveSameVersionCommand;
        public AutoRelayCommand SaveSameVersionCommand
        {
            get
            {
                return _saveSameVersionCommand ?? (_saveSameVersionCommand = new AutoRelayCommand(
              () =>
              {
                  _dbRecipeService.Invoke(x => x.SetRecipe(_recipe, false));
                  ReloadRecipe();
              },
              () => { return true; }));
            }
        }


        public string RecipeName
        {
            get => _recipe?.Name; set { if (_recipe?.Name != value) { _recipe.Name = value; OnPropertyChanged(); } }
        }

        public int? Version => _recipe?.Version;
    }
}
