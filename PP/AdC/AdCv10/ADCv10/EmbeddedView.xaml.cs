using System;
using System.Windows;
using System.Windows.Controls;

using UnitySC.PP.Shared.Configuration;

namespace ADC
{
    /// <summary>
    /// Logique d'interaction pour EmbeddedView.xaml
    /// </summary>
    public partial class EmbeddedView : UserControl
    {
        public EmbeddedView()
        {
            Bootstrapper.Register();


            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/ADCEditor;component/Styles/GlobalStyle.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/ADCEditor;component/View/DataTemplate.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/ADCEditor;component/View/DataProvider.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/ADCEditor;component/View/Converters/Converters.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/ADCEditor;component/Styles/ImageDictionary.xaml") });

            InitializeComponent();

            new ViewModel.ViewModelLocator();

            ViewModel.ViewModelLocator.Instance.MainWindowViewModel.IsEmbedded = true;

            DataContext = ViewModel.ViewModelLocator.Instance.MainWindowViewModel;

        }



        public static void Init()
        {
            MergeContext.Context.Initializer.Init();
            ADCEngine.ADC.Instance.Init();
        }

        public static void Exit()
        {
            ADCEngine.ADC.Instance.Shutdown();
        }


        public static void InitParam(Func<string, string> getValue)
        {
            AppParameter.Instance.Init(getValue);
        }


        private static Func<bool> s_closeEditor = null;
        public static void InitCloseEditor(Func<bool> closeEditor)
        {
            s_closeEditor = closeEditor;
        }

        public static void CloseEditor()
        {
            s_closeEditor();
        }



        // Guid utilisé pour identifier la Recette au niveau du DataFlow / TC
        private static Guid s_currentRecipeKey;


        private static Func<Guid, string, bool> s_saveRecipe = null;
        public static void InitSaveRecipe(Func<Guid, string, bool> saveRecipe)
        {
            s_saveRecipe = saveRecipe;
        }

        public static bool SaveRecipe(string recipeXml)
        {
            return s_saveRecipe(s_currentRecipeKey, recipeXml);
        }


        public static bool LoadRecipe(Guid key, string recipeXml)
        {
            s_currentRecipeKey = key;

            ViewModel.RecipeViewModel rvm = (ViewModel.ViewModelLocator.Instance.MainWindowViewModel.MainViewViewModel as ViewModel.RecipeViewModel);

            if (rvm != null)
            {

                rvm.LoadRecipeFromXml(recipeXml);

                /* System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();

                 xmldoc.LoadXml(recipeXml);


                 rvm.LoadRecipe(xmldoc);

                 */
            }
            return true;
        }

    }

}
