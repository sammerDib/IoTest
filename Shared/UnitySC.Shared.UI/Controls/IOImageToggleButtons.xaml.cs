using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls
{
    public partial class IOImageToggleButtons : UserControl
    {

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public static readonly DependencyProperty LeftButtonContentProperty =
            DependencyProperty.Register(nameof(LeftButtonContent), typeof(string),
                typeof(IOImageToggleButtons), new PropertyMetadata(default(string)));

        public string LeftButtonContent
        {
            get => (string)GetValue(LeftButtonContentProperty);
            set
            {
                SetValue(LeftButtonContentProperty, value);
            }
        }

        public static readonly DependencyProperty LeftButtonCommandProperty =
            DependencyProperty.Register(nameof(LeftButtonCommand), typeof(ICommand),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public ICommand LeftButtonCommand
        {
            get => (ICommand)GetValue(LeftButtonCommandProperty);
            set
            {
                SetValue(LeftButtonCommandProperty, value);
            }
        }

        public static readonly DependencyProperty LeftButtonImageProperty =
            DependencyProperty.Register(nameof(LeftButtonImage), typeof(ImageSource),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public ImageSource LeftButtonImage
        {
            get => (ImageSource)GetValue(LeftButtonImageProperty);
            set
            {
                SetValue(LeftButtonImageProperty, value);
            }
        }

        public static readonly DependencyProperty LeftButtonImageGeometryProperty =
            DependencyProperty.Register(nameof(LeftButtonImageGeometry), typeof(Geometry),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public Geometry LeftButtonImageGeometry
        {
            get => (Geometry)GetValue(LeftButtonImageGeometryProperty);
            set
            {
                SetValue(LeftButtonImageGeometryProperty, value);
            }
        }

        public static readonly DependencyProperty LeftButtonImageGeometryBrushProperty =
            DependencyProperty.Register(nameof(LeftButtonImageGeometryBrush), typeof(Brush),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public Brush LeftButtonImageGeometryBrush
        {
            get => (Brush)GetValue(LeftButtonImageGeometryBrushProperty);
            set
            {
                SetValue(LeftButtonImageGeometryBrushProperty, value);
            }
        }

        public static readonly DependencyProperty LeftButtonVisibilityProperty =
            DependencyProperty.Register(nameof(LeftButtonVisibility), typeof(Visibility),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public Visibility LeftButtonVisibility
        {
            get => (Visibility)GetValue(LeftButtonVisibilityProperty);
            set
            {
                SetValue(LeftButtonVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty RightButtonContentProperty =
            DependencyProperty.Register(nameof(RightButtonContent), typeof(string),
                typeof(IOImageToggleButtons), new PropertyMetadata(default(string)));

        public string RightButtonContent
        {
            get => (string)GetValue(RightButtonContentProperty);
            set
            {
                SetValue(RightButtonContentProperty, value);
            }
        }

        public static readonly DependencyProperty RightButtonCommandProperty =
            DependencyProperty.Register(nameof(RightButtonCommand), typeof(ICommand),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public ICommand RightButtonCommand
        {
            get => (ICommand)GetValue(RightButtonCommandProperty);
            set
            {
                SetValue(RightButtonCommandProperty, value);
            }
        }

        public static readonly DependencyProperty RightButtonImageProperty =
            DependencyProperty.Register(nameof(RightButtonImage), typeof(ImageSource),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public ImageSource RightButtonImage
        {
            get => (ImageSource)GetValue(RightButtonImageProperty);
            set
            {
                SetValue(RightButtonImageProperty, value);
            }
        }

        public static readonly DependencyProperty RightButtonImageGeometryProperty =
            DependencyProperty.Register(nameof(RightButtonImageGeometry), typeof(Geometry),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public Geometry RightButtonImageGeometry
        {
            get => (Geometry)GetValue(RightButtonImageGeometryProperty);
            set
            {
                SetValue(RightButtonImageGeometryProperty, value);
            }
        }

        public static readonly DependencyProperty RightButtonImageGeometryBrushProperty =
            DependencyProperty.Register(nameof(RightButtonImageGeometryBrush), typeof(Brush),
                typeof(IOImageToggleButtons), new PropertyMetadata(null));

        public Brush RightButtonImageGeometryBrush
        {
            get => (Brush)GetValue(RightButtonImageGeometryBrushProperty);
            set
            {
                SetValue(RightButtonImageGeometryBrushProperty, value);
            }
        }

        public static readonly DependencyProperty IsIOStatusMatchingLeftButtonProperty =
            DependencyProperty.Register(nameof(IsIOStatusMatchingLeftButton), typeof(bool),
                typeof(IOImageToggleButtons), new PropertyMetadata(true));

        public bool IsIOStatusMatchingLeftButton
        {
            get => (bool)GetValue(IsIOStatusMatchingLeftButtonProperty);
            set
            {
                SetValue(IsIOStatusMatchingLeftButtonProperty, value);
            }
        }

        public IOImageToggleButtons()
        {
            InitializeComponent();
        }

    }
}
