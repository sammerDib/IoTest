using System.Windows.Controls;

using ADCEngine;

using Utils.ViewModel;

namespace ADCConfiguration.ViewModel.Recipe
{
    public class RecipeHistoryParameterViewModel : ValidationViewModelBase
    {
        public UserControl ParameterUI { get; private set; }

        /// <summary>
        /// Défini si identique au paramétre comparé
        /// </summary>
        public bool Same { get; private set; }

        public RecipeHistoryParameterViewModel(ParameterBase parameter, bool same)
        {
            Same = same;
            ParameterUI = parameter.ParameterUI;
        }
    }
}
