using AdcBasicObjects.Rendering;

using UnitySC.Shared.UI.AutoRelayCommandExt;


namespace BasicModules.DataLoader
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DataLoaderRenderingViewModel : ImageRenderingViewModel
    {
        public DataLoaderRenderingViewModel(DataLoaderBase module) :
            base(module)
        {
        }

        public bool IsDataloader => true;

        private AutoRelayCommand _selectRenderingImagesCommand;
        public AutoRelayCommand SelectRenderingImagesCommand
        {
            get
            {
                return _selectRenderingImagesCommand ?? (_selectRenderingImagesCommand = new AutoRelayCommand(
                    () =>
                    {
                        SelectRenderingImageView view = new SelectRenderingImageView();
                        view.DataContext = new SelectRenderingImageViewModel((DataLoaderBase)Module);
                        view.ShowDialog();
                    }));
            }
        }

    }
}
