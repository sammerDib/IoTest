using System;
using System.Collections;
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

using Xceed.Wpf.Toolkit;

namespace ControlsGallery
{
    /// <summary>
    /// Interaction logic for Geometries.xaml
    /// </summary>
    public partial class Geometries : UserControl
    {



        public Color CurSelectedColor
        {
            get { return (Color)GetValue(CurSelectedColorProperty); }
            set { SetValue(CurSelectedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurSelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurSelectedColorProperty =
            DependencyProperty.Register(nameof(CurSelectedColor), typeof(Color), typeof(Geometries), new PropertyMetadata());




        public ObservableCollection<String> GeometriesList { get; set; }


        public Geometries()
        {
            InitializeComponent();

            List<string> geometries = new List<string>();

            foreach (var mergedDictionnary in Application.Current.Resources.MergedDictionaries)
            {
                foreach (DictionaryEntry dictionaryEntry in mergedDictionnary)
                {
                    if (dictionaryEntry.Value is Geometry)
                    {
                        geometries.Add((string)dictionaryEntry.Key);
                    }
                }
            }

          

            GeometriesList = new ObservableCollection<string>(geometries.OrderBy(x => x).ToList());
            CurSelectedColor = Colors.Black;
            this.DataContext = this;
        }
    }
}
