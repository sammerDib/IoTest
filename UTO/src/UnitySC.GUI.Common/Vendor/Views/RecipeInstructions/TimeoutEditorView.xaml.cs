using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions
{
    /// <summary>
    /// Interaction logic for TimeoutEditorView.xaml
    /// </summary>
    public partial class TimeoutEditorView : UserControl
    {
        private static readonly List<long> DefaultItemsSource = new List<long>
        {
            5,
            10,
            30,
            60,
            1000,
            2000,
            5000,
            10000,
            20000,
            60000
        };


        public TimeoutEditorView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(TimeoutEditorView), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(string), typeof(TimeoutEditorView), new PropertyMetadata(default(string)));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive), typeof(bool), typeof(TimeoutEditorView), new PropertyMetadata(true));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource), typeof(IEnumerable), typeof(TimeoutEditorView), new PropertyMetadata(DefaultItemsSource));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty IsOptionalProperty = DependencyProperty.Register(
            nameof(IsOptional), typeof(bool), typeof(TimeoutEditorView), new PropertyMetadata(default(bool)));

        public bool IsOptional
        {
            get { return (bool)GetValue(IsOptionalProperty); }
            set { SetValue(IsOptionalProperty, value); }
        }
    }
}
