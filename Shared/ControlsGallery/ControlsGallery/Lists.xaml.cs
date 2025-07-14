using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class TestItem
    {
        public string Name { get; set; }
        public int Value { get; set; }

    }

    /// <summary>
    /// Interaction logic for Lists.xaml
    /// </summary>
    public partial class Lists : UserControl
    {
        public ObservableCollection<TestItem> Items { get; set; }

        public Lists()
        {
            this.DataContext = this;

            var newItem = new TestItem() { Name = "Item1", Value = 11 };
            var newItem2 = new TestItem() { Name = "Item2", Value = 22 };
            var newItem3 = new TestItem() { Name = "Item3", Value = 33 };
            Items = new ObservableCollection<TestItem>();
            Items.Add(newItem);
            Items.Add(newItem2);
            Items.Add(newItem3);
            InitializeComponent();
        }
    }
}
