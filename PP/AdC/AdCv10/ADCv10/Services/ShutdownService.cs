using ADC.ViewModel;

namespace ADC.Services
{
    public class ShutdownService
    {
        private bool shutdowning = false;

        public void ShutdownApp()
        {
            if (shutdowning)
                return;

            bool allowShutdown = true;

            RecipeViewModel recipeVM = ViewModelLocator.Instance.MainWindowViewModel.MainViewViewModel as RecipeViewModel;
            if (recipeVM != null)
                allowShutdown = recipeVM.CheckSave();

            if (allowShutdown)
            {
                // arret autorisé
                shutdowning = true;
                App.Current.Shutdown();
            }
            else
            {
                shutdowning = false;

                // arret refusé.
            }
        }

    }
}
