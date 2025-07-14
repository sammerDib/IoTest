using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Bow;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.Shared.ResultUI.Metro.View.Stats.Bow
{
    /// <summary>
    /// Interaction logic for ThicknessStatsListView.xaml
    /// </summary>
    public partial class BowStatsListView
    {
        public BowStatsListView()
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

        private BowStatsVM ViewModel => DataContext as BowStatsVM;

        private void OnGeneratedColumnsChanged()
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            DataTemplate GetDataTemplate(GeneratedColumn column)
            {
                var cellTemplate = new DataTemplate();

                if (column is GeneratedStateColumn stateColumn)
                {
                    var toleranceDisplayerFactory = new FrameworkElementFactory(typeof(ToleranceDisplayer));
                    toleranceDisplayerFactory.SetBinding(ToleranceDisplayer.ValueProperty, stateColumn.ValueBinding);
                    toleranceDisplayerFactory.SetBinding(ToleranceDisplayer.ToleranceProperty, stateColumn.StateBinding);

                    cellTemplate.VisualTree = toleranceDisplayerFactory;
                }
                else
                {

                    var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetBinding(TextBlock.TextProperty, column.ValueBinding);
                    textBlockFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);

                    cellTemplate.VisualTree = textBlockFactory;
                }

                return cellTemplate;
            }

            ColumnGenerator.GenerateColumns(GridView, viewModel.GeneratedColumns, GetDataTemplate);
        }
    }
}
