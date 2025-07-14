using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.UIComponents
{
    /// <summary>
    /// Interaction logic for IconVisualizer.xaml
    /// </summary>
    public partial class IconVisualizer
    {
        /// <summary>
        /// PathGeometry Icons visualizer
        /// </summary>
        public IconVisualizer()
        {
            InitializeComponent();
            PathGeometries = ResourcesHelper.GetAll<PathGeometry>();
        }

        public static readonly DependencyProperty PathGeometriesProperty = DependencyProperty.Register(
            nameof(PathGeometries), typeof(Dictionary<string, PathGeometry>), typeof(IconVisualizer), new PropertyMetadata(default(Dictionary<string, PathGeometry>)));

        public Dictionary<string, PathGeometry> PathGeometries
        {
            get => (Dictionary<string, PathGeometry>)GetValue(PathGeometriesProperty);
            set => SetValue(PathGeometriesProperty, value);
        }
    }
}
