using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Thickness
{
    /// <summary>
    /// Interaction logic for ThicknessDataRepetaView.xaml
    /// </summary>
    public partial class ThicknessDataRepetaView
    {
        public ThicknessDataRepetaView()
        {
            InitializeComponent();
        }

        #region Overrides

        protected override void OnGenerateColumnsRequested()
        {
            base.OnGenerateColumnsRequested();
            OnGeneratedColumnsChanged();
        }

        #endregion

        private ThicknessDataRepetaVM ViewModel => DataContext as ThicknessDataRepetaVM;

        private void OnGeneratedColumnsChanged()
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            var measureStateTemplate = FindResource("MeasureStateTemplate") as DataTemplate;

            DataTemplate GetDataTemplate(GeneratedStateColumn column)
            {
                var cellTemplate = new DataTemplate();

                var dockPanelFactory = new FrameworkElementFactory(typeof(DockPanel));

                var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                contentPresenterFactory.SetValue(MarginProperty, new System.Windows.Thickness(5, 0, 0, 0));
                contentPresenterFactory.SetValue(ContentPresenter.ContentTemplateProperty, measureStateTemplate);
                contentPresenterFactory.SetBinding(ContentPresenter.ContentProperty, column.StateBinding);
                contentPresenterFactory.SetValue(DockPanel.DockProperty, Dock.Right);
                dockPanelFactory.AppendChild(contentPresenterFactory);

                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetBinding(TextBlock.TextProperty, column.ValueBinding);
                textBlockFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);
                dockPanelFactory.AppendChild(textBlockFactory);

                cellTemplate.VisualTree = dockPanelFactory;
                return cellTemplate;
            }

            ColumnGenerator.GenerateColumns(GridView, viewModel.GeneratedColumns, GetDataTemplate);
        }
    }
}
