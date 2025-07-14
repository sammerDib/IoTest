using System.Windows;
using System.Windows.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Run
{
    /// <summary>
    /// Interaction logic for RecipeRunLiveControl.xaml
    /// </summary>
    public partial class RecipeRunLiveControl : UserControl
    {
        public RecipeRunLiveControl()
        {
            InitializeComponent();
        }



        public bool IsOneRecipeAlreadyStarted
        {
            get { return (bool)GetValue(IsOneRecipeAlreadyStartedProperty); }
            set { SetValue(IsOneRecipeAlreadyStartedProperty, value); }
        }

        public static readonly DependencyProperty IsOneRecipeAlreadyStartedProperty =
            DependencyProperty.Register(nameof(IsOneRecipeAlreadyStarted), typeof(bool), typeof(RecipeRunLiveControl), new PropertyMetadata(false));


    }
}
