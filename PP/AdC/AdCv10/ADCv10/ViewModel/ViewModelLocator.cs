/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ADC"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
###
/!\ mvvmlight nuget are now depracated, now please used CommunityToolkit.Mvvm
###
*/

using UnitySC.Shared.Tools;

namespace ADC.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; private set; }


        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            Instance = this;
        }


        public static void Register()
        {
            ClassLocator.Default.Register<MainWindowViewModel>(true);
            ClassLocator.Default.Register<RecipeViewModel>(true);
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return ClassLocator.Default.GetInstance<MainWindowViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
