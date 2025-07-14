using DeepLearningSoft48.ViewModels;

using MvvmDialogs;

using UnitySC.Shared.Tools;

namespace DeepLearningSoft48.Services
{
    /// <summary>
    /// This class contains references to all the view models in the application and provides an
    /// entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Set the main window view model.
        /// </summary>
        public MainViewModel MainWindow => ClassLocator.Default.GetInstance<MainViewModel>();

        //====================================================================
        // Constructor
        //====================================================================
        public ViewModelLocator()
        {
            ClassLocator.Default.Register<MainViewModel>();
            ClassLocator.Default.Register<ApplyImageProcessesDialogViewModel>();
            ClassLocator.Default.Register<AddImageProcessDialogViewModel>();
            ClassLocator.Default.Register<AddNewDefectCategoryDialogViewModel>();
             ClassLocator.Default.Register<EditDefectCategoryDialogViewModel>(() => new EditDefectCategoryDialogViewModel(null, null, null, System.Drawing.Color.Green));
            ClassLocator.Default.Register<IDialogService>(() => new DialogService());
        }
    }
}
