/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ADCConfiguration"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;

using ADCConfiguration.ViewModel.Administration;
using ADCConfiguration.ViewModel.Recipe;
using ADCConfiguration.ViewModel.Tool;
using ADCConfiguration.ViewModel.Tool.TreeView;
using ADCConfiguration.ViewModel.Users;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools;

namespace ADCConfiguration.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
        }

        public static void Register()
        {
            ClassLocator.Default.Register<MainViewModel>(true);
            ClassLocator.Default.Register<MainMenuViewModel>(true);
            ClassLocator.Default.Register<UserLoginViewModel>(true);
            ClassLocator.Default.Register<UserViewModel>(true);
            ClassLocator.Default.Register<ImportRecipeViewModel>(true);
            ClassLocator.Default.Register<ExportRecipeViewModel>(true);
            ClassLocator.Default.Register<EditVidViewModel>(true);
            ClassLocator.Default.Register<EditUserViewModel>(true);
            ClassLocator.Default.Register<RecipeHistoryViewModel>(true);
            ClassLocator.Default.Register<LogsViewModel>(true);
            ClassLocator.Default.Register<ArchivedRecipeViewModel>(true);
        }

        public MainViewModel MainViewModel { get { return GetInstance<MainViewModel>(); } }

        public MainMenuViewModel MainMenuViewModel { get { return GetInstance<MainMenuViewModel>(); } }



        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        public static ObservableRecipient GetInstance(Type type)
        {
            return ClassLocator.Default.GetInstance(type) as ObservableRecipient;
        }

        public static VM GetInstance<VM>() where VM : class
        {
            return ClassLocator.Default.GetInstance<VM>();
        }

    }
}
