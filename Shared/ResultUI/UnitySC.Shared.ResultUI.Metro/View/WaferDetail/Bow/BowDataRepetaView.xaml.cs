using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Bow
{
    public partial class BowDataRepetaView
    {
        public BowDataRepetaView()
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

        private BowDataRepetaVM ViewModel => DataContext as BowDataRepetaVM;

        private void OnGeneratedColumnsChanged()
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            var MeasureStateTemplate = FindResource("MeasureStateTemplate") as DataTemplate;

            DataTemplate GetDataTemplate(GeneratedStateColumn column)
            {
                var cellTemplate = new DataTemplate();

                var dockPanelFactory = new FrameworkElementFactory(typeof(DockPanel));

                var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                contentPresenterFactory.SetValue(MarginProperty, new System.Windows.Thickness(5, 0, 0, 0));
                contentPresenterFactory.SetValue(ContentPresenter.ContentTemplateProperty, MeasureStateTemplate);
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
