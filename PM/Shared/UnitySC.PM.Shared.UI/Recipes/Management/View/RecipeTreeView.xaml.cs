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

namespace UnitySC.PM.Shared.UI.Recipes.Management.View
{
    /// <summary>
    /// Interaction logic for RecipeTreeView.xaml
    /// </summary>
    public partial class RecipeTreeView : UserControl
    {
        public RecipeTreeView()
        {
            InitializeComponent();
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem =
                      VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }
        }

        private static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            DependencyObject returnVal = source;

            while (returnVal != null && !(returnVal is T))
            {
                DependencyObject tempReturnVal = null;
                if (returnVal is Visual)
                {
                    tempReturnVal = VisualTreeHelper.GetParent(returnVal);
                }
                if (tempReturnVal == null)
                {
                    returnVal = LogicalTreeHelper.GetParent(returnVal);
                }
                else returnVal = tempReturnVal;
            }

            return returnVal as T;
        }

        public bool CanAddProduct
        {
            get { return (bool)GetValue(CanAddProductProperty); }
            set { SetValue(CanAddProductProperty, value); }
        }

        public static readonly DependencyProperty CanAddProductProperty =
            DependencyProperty.Register(nameof(CanAddProduct), typeof(bool), typeof(RecipeTreeView), new PropertyMetadata(true));

        public bool CanAddStep
        {
            get { return (bool)GetValue(CanAddStepProperty); }
            set { SetValue(CanAddStepProperty, value); }
        }

        public static readonly DependencyProperty CanAddStepProperty =
            DependencyProperty.Register(nameof(CanAddStep), typeof(bool), typeof(RecipeTreeView), new PropertyMetadata(true));



    }
}
