using System.Windows;
using System.Windows.Controls;



using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.DMT.CommonUI.View
{
    /// <summary>
    /// Interaction logic for RecipeEditionView.xaml
    /// </summary>
    public partial class RecipeEditionView : UserControl
    {
        public RecipeEditionView()
        {
            InitializeComponent();    
        }

        public bool IsEditingRecipeName
        {
            get { return (bool)GetValue(IsEditingRecipeNameProperty); }
            set { SetValue(IsEditingRecipeNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditingRecipeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditingRecipeNameProperty =
            DependencyProperty.Register(nameof(IsEditingRecipeName), typeof(bool), typeof(RecipeEditionView), new PropertyMetadata(false));


    
        private AutoRelayCommand _startRecipeNameEdition;

        public AutoRelayCommand StartRecipeNameEdition
        {
            get
            {
                return _startRecipeNameEdition ?? (_startRecipeNameEdition = new AutoRelayCommand(
                    () =>
                    {
                        // Code to execute
                        IsEditingRecipeName = true;
                    },
                    () => { return true; }
                ));
            }
        }

        private void TexBoxRecipeName_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditingRecipeName = false;
        }
    }
}
