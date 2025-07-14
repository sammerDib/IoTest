using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.UIComponents.Controls.Abstract;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class CollapsableHorizontalPanel : ExtendedControlBase
    {
        #region Fields

        private Button _expandButton;

        private RowDefinition _firstRow;
        private RowDefinition _secondRow;

        #endregion

        #region Overrides of FrameworkElement

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_ExpandButton", out _expandButton))
            {
                _expandButton.Click += OnExpandSecondRowClick;
            }

            GetTemplateChild("PART_FirstRow", out _firstRow);
            GetTemplateChild("PART_SecondRow", out _secondRow);
        }

        #endregion

        public static readonly DependencyProperty FirstRowLengthProperty = DependencyProperty.Register(
          nameof(FirstRowLength),
          typeof(GridLength),
          typeof(CollapsableHorizontalPanel),
          new PropertyMetadata(default(GridLength)));

        public GridLength FirstRowLength
        {
            get => (GridLength)GetValue(FirstRowLengthProperty);
            set => SetValue(FirstRowLengthProperty, value);
        }

        public static readonly DependencyProperty SecondRowLengthProperty = DependencyProperty.Register(
            nameof(SecondRowLength),
            typeof(GridLength),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(default(GridLength)));

        public GridLength SecondRowLength
        {
            get => (GridLength)GetValue(SecondRowLengthProperty);
            set => SetValue(SecondRowLengthProperty, value);
        }

        public static readonly DependencyProperty FirstRowMinHeightProperty = DependencyProperty.Register(
            nameof(FirstRowMinHeight),
            typeof(double),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(default(double)));

        public double FirstRowMinHeight
        {
            get => (double)GetValue(FirstRowMinHeightProperty);
            set => SetValue(FirstRowMinHeightProperty, value);
        }

        public static readonly DependencyProperty SecondRowMinHeightProperty = DependencyProperty.Register(
            nameof(SecondRowMinHeight),
            typeof(double),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(default(double)));

        public double SecondRowMinHeight
        {
            get => (double)GetValue(SecondRowMinHeightProperty);
            set => SetValue(SecondRowMinHeightProperty, value);
        }

        public static readonly DependencyProperty FirstRowContentProperty = DependencyProperty.Register(
            nameof(FirstRowContent),
            typeof(object),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(default(object)));

        public object FirstRowContent
        {
            get => GetValue(FirstRowContentProperty);
            set => SetValue(FirstRowContentProperty, value);
        }

        public static readonly DependencyProperty SecondRowContentProperty = DependencyProperty.Register(
            nameof(SecondRowContent),
            typeof(object),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(default(object)));

        public object SecondRowContent
        {
            get => GetValue(SecondRowContentProperty);
            set => SetValue(SecondRowContentProperty, value);
        }

        public static readonly DependencyProperty SecondRowIsExpandedProperty = DependencyProperty.Register(
            nameof(SecondRowIsExpanded),
            typeof(bool),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(true, OnSecondRowIsExpandedChanged));

        private static void OnSecondRowIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CollapsableHorizontalPanel self)
            {
                if (self.SecondRowIsExpanded)
                {
                    self._firstRow.Height = self.FirstRowLength;
                    self._secondRow.Height = self.SecondRowLength;
                    self._secondRow.MinHeight = self.SecondRowMinHeight;
                }
                else
                {
                    self.FirstRowLength = self._firstRow.Height;
                    self.SecondRowLength = self._secondRow.Height;

                    self._firstRow.Height = new GridLength(1, GridUnitType.Star);
                    self._secondRow.Height = GridLength.Auto;
                    self._secondRow.MinHeight = 0;
                }
            }
        }

        public bool SecondRowIsExpanded
        {
            get => (bool)GetValue(SecondRowIsExpandedProperty);
            set => SetValue(SecondRowIsExpandedProperty, value);
        }

        private void OnExpandSecondRowClick(object sender, RoutedEventArgs e)
        {
            SecondRowIsExpanded = true;
        }

        public static readonly DependencyProperty SecondRowContentTitleProperty = DependencyProperty.Register(
            nameof(SecondRowContentTitle),
            typeof(object),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(default(object)));

        public object SecondRowContentTitle
        {
            get => GetValue(SecondRowContentTitleProperty);
            set => SetValue(SecondRowContentTitleProperty, value);
        }

        public static readonly DependencyProperty SecondRowContentIconProperty = DependencyProperty.Register(
            nameof(SecondRowContentIcon),
            typeof(Geometry),
            typeof(CollapsableHorizontalPanel),
            new PropertyMetadata(default(Geometry)));

        public Geometry SecondRowContentIcon
        {
            get => (Geometry)GetValue(SecondRowContentIconProperty);
            set => SetValue(SecondRowContentIconProperty, value);
        }
    }
}
