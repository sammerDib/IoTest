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

namespace ControlsGallery
{
    /// <summary>
    /// Interaction logic for Buttons.xaml
    /// </summary>
    public partial class Buttons : UserControl
    {


        public Buttons()
        {
            InitializeComponent();
            CurrentValue = 0;
            this.DataContext = this;
        }


        public int CurrentValue { get; set; }

        private void ImageRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentValue++;
            UpdateCurrentValueDisplay();
        }

        private void ImageRepeatMinusButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentValue--;
            UpdateCurrentValueDisplay();
        }


        private void UpdateCurrentValueDisplay()
        {
            BindingExpression binding = DisplayCurrentValue.GetBindingExpression(TextBlock.TextProperty);
            binding.UpdateTarget();

        }


    }
}
