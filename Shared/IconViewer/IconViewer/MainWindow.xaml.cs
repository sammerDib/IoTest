using System;
using System.Collections;
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

namespace IconViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
            List<string> images = new List<string>();
            foreach(DictionaryEntry dictionaryEntry in Application.Current.Resources.MergedDictionaries[0])
            {
                if(dictionaryEntry.Value is DrawingImage)
                {
                    images.Add((string)dictionaryEntry.Key);
                }
            }

            this.DataContext = images.OrderBy(x => x).ToList();
        }
    }
}
