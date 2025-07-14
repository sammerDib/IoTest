using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    internal class ThicknessLayersEditorViewModel : ObservableObject, IModalDialogViewModel
    {

        private bool? _dialogResult;
        private LayersEditorViewModel _layersEditor = null;

        public LayersEditorViewModel LayersEditor
        {
            get
            {
                if (_layersEditor is null)
                    _layersEditor = new LayersEditorViewModel();
                return _layersEditor;

            }
             set { if (_layersEditor != value) { _layersEditor = value; OnPropertyChanged(); } }
        }



        private AutoRelayCommand _validate;

        public AutoRelayCommand Validate
        {
            get
            {
                return _validate ?? (_validate = new AutoRelayCommand(
                    () =>
                    {
                        DialogResult = true;
                    },
                    () => { return !LayersEditor.IsEditing; }
                ));
            }
        }




        #region IModalDialogViewModel
        public bool? DialogResult
{
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        #endregion IModalDialogViewModel
    }
}
