using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public enum PathType
    {
        File,
        Folder
    }

    public class PathBox : Control
    {
        static PathBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathBox), new FrameworkPropertyMetadata(typeof(PathBox)));
        }

        public static readonly DependencyProperty DefinePathCommandProperty = DependencyProperty.Register(nameof(DefinePathCommand), typeof(ICommand), typeof(PathBox), new PropertyMetadata(default(ICommand)));

        public ICommand DefinePathCommand
        {
            get => (ICommand)GetValue(DefinePathCommandProperty);
            set => SetValue(DefinePathCommandProperty, value);
        }

        public static readonly DependencyProperty IsDefinePathButtonVisibleProperty = DependencyProperty.Register(nameof(IsDefinePathButtonVisible), typeof(bool), typeof(PathBox), new PropertyMetadata(true));

        public bool IsDefinePathButtonVisible
        {
            get => (bool)GetValue(IsDefinePathButtonVisibleProperty);
            set => SetValue(IsDefinePathButtonVisibleProperty, value);
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(string), typeof(PathBox), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(PathBox), new PropertyMetadata(true));

        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        public static readonly DependencyProperty PathTypeProperty = DependencyProperty.Register(nameof(PathType), typeof(PathType), typeof(PathBox), new PropertyMetadata(PathType.File));

        public PathType PathType
        {
            get => (PathType)GetValue(PathTypeProperty);
            set => SetValue(PathTypeProperty, value);
        }
    }
}
