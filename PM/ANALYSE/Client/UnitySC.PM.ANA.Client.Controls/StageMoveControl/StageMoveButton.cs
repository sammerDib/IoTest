using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using UnitySC.PM.ANA.Client.Proxy.Axes;

namespace UnitySC.PM.ANA.Client.Controls.StageMoveControl
{
    public class StageMoveButton : Button
    {
        public StageMoveButton()
        {
        }

        public AxesMoveTypes MoveType
        {
            get { return (AxesMoveTypes)GetValue(MoveTypeProperty); }
            set { SetValue(MoveTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MoveTypeProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoveTypeProperty =
            DependencyProperty.Register(nameof(MoveType), typeof(AxesMoveTypes), typeof(StageMoveButton), new PropertyMetadata());

        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(StageMoveButton), new PropertyMetadata(Application.Current.FindResource(typeof(Button)) as Style));

        public Geometry ImageGeometry
        {
            get { return (Geometry)GetValue(ImageGeometryProperty); }
            set { SetValue(ImageGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryProperty =
            DependencyProperty.Register(nameof(ImageGeometry), typeof(Geometry), typeof(StageMoveButton), new PropertyMetadata(null));
    }
}
