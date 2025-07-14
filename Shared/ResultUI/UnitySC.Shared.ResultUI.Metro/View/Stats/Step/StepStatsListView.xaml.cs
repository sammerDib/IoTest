using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Step;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.Shared.ResultUI.Metro.View.Stats.Step
{
    /// <summary>
    /// Interaction logic for ThicknessStatsListView.xaml
    /// </summary>
    public partial class StepStatsListView
    {
        public StepStatsListView()
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

        private StepStatsVM ViewModel => DataContext as StepStatsVM;

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
