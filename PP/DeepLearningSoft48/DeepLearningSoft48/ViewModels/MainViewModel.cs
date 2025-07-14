using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Utils.Enums;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// DataContext for the MainWindow.
    /// </summary>
    public class MainViewModel : ObservableRecipient
    {
        //====================================================================
        // Initialization of the 2 main view models (tabs views)
        //====================================================================
        public LearningTabViewModel LearningTabViewModel { get; }
        public TestTabViewModel TestTabViewModel { get; }

        private ImageAnnotationToolsViewModel _imageAnnotationToolsViewModel;

        //====================================================================
        // Constructor
        //====================================================================
        public MainViewModel()
        {
            _imageAnnotationToolsViewModel = new ImageAnnotationToolsViewModel();

            LearningTabViewModel = new LearningTabViewModel(TabType.Learning, _imageAnnotationToolsViewModel);
            TestTabViewModel = new TestTabViewModel(TabType.Test, _imageAnnotationToolsViewModel);
        }
    }
}
