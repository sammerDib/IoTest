using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.DMT.Client.View
{
    /// <summary>
    /// Interaction logic for RecipeListView.xaml
    /// </summary>
    public partial class RecipeListView : UserControl
    {
        public RecipeListView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The <see cref="SelectedRecipe" /> dependency property's name.
        /// </summary>
        public const string SelectedRecipePropertyName = "SelectedRecipe";

        /// <summary>
        /// Gets or sets the value of the <see cref="SelectedRecipe" />
        /// property. This is a dependency property.
        /// </summary>
        public RecipeInfo SelectedRecipe
        {
            get
            {
                return (RecipeInfo)GetValue(SelectedRecipeProperty);
            }
            set
            {
                SetValue(SelectedRecipeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedRecipe" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedRecipeProperty = DependencyProperty.Register(
            SelectedRecipePropertyName,
            typeof(RecipeInfo),
            typeof(RecipeListView),
            new UIPropertyMetadata(null));
    }
}
