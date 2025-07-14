using System.Collections.ObjectModel;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Core
{
    /// <summary>
    /// ViewModel that regroup all <see cref="DataSourceViewModel"/> with the same unit abbreviation.
    /// </summary>
    public class UnitTreeNode : Notifier
    {
        /// <summary>
        /// Create an <see cref="UnitTreeNode"/> for the given unit abbreviation.
        /// </summary>
        /// <param name="unitAbbreviation">The root element of the current <see cref="UnitTreeNode"/>.</param>
        public UnitTreeNode(string unitAbbreviation)
        {
            UnitAbbreviation = unitAbbreviation;
            DataSources = new ObservableCollection<DataSourceViewModel>();
        }

        /// <summary>
        /// Get or set the root unit abbreviation.
        /// </summary>
        public string UnitAbbreviation { get; set; }

        /// <summary>
        /// The <see cref="ObservableCollection{T}"/> of <see cref="DataSourceViewModel"/> that are categorized as <see cref="UnitAbbreviation"/>.
        /// </summary>
        public ObservableCollection<DataSourceViewModel> DataSources { get; set; }
    }
}
