using System;
using System.Windows;

using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;

namespace UnitySC.Shared.ResultUI.Common.View.ImageViewer
{
    /// <summary>
    /// Interaction logic for MatrixViewerView.xaml
    /// </summary>
    public partial class MatrixViewerView
    {
        public MatrixViewerView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                switch (ViewModel.Mode)
                {
                    case MatrixViewerViewModel.MatrixViewerMode.TwoDimension:
                        TabControl.Select(TwoDimensionTabItem);
                        break;
                    case MatrixViewerViewModel.MatrixViewerMode.ThreeDimension:
                        TabControl.Select(ThreeDimensionTabItem);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private MatrixViewerViewModel ViewModel => DataContext as MatrixViewerViewModel;

        private void OnTabControlSelectionChanged(object sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                if (ReferenceEquals(TabControl.SelectedItem, ThreeDimensionTabItem))
                {
                    ViewModel.Mode = MatrixViewerViewModel.MatrixViewerMode.ThreeDimension;
                }
                else
                {
                    ViewModel.Mode = MatrixViewerViewModel.MatrixViewerMode.TwoDimension;
                }
            }
        }

        #region Dependency Properties

        public static readonly DependencyProperty AdditionalContentProperty = DependencyProperty.Register(
            nameof(AdditionalContent), typeof(object), typeof(MatrixViewerView), new PropertyMetadata(default(object)));

        public object AdditionalContent
        {
            get { return (object)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }

        #endregion
    }
}
