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

namespace ControlsGallery
{
    /// <summary>
    /// Interaction logic for Icons.xaml
    /// </summary>
    public partial class Icons : UserControl
    {
        public Icons()
        {
            InitializeComponent();
            List<string> images = new List<string>();
            foreach (DictionaryEntry dictionaryEntry in Application.Current.Resources.MergedDictionaries[0])
            {
                if (dictionaryEntry.Value is DrawingImage)
                {
                    images.Add((string)dictionaryEntry.Key);
                }
            }

            this.DataContext = images.OrderBy(x => x).ToList();
        }
    }
}
