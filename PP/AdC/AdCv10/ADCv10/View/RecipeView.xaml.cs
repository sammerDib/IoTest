using System.Windows;
using System.Windows.Controls;

namespace ADC.View
{
    /// <summary>
    /// Logique d'interaction pour RecipeView.xaml
    /// </summary>
    public partial class RecipeView : UserControl
    {

        public RecipeView()
        {
            InitializeComponent();
        }
    }



    public class RecipeViewHeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MenuTemplate { get; set; }
        public DataTemplate EmbeddedMenuTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item,
          DependencyObject container)
        {
            if (ViewModel.ViewModelLocator.Instance.MainWindowViewModel.IsEmbedded)
            {
                return EmbeddedMenuTemplate;
            }
            else
            {
                return MenuTemplate;
            }
        }
    }
}



