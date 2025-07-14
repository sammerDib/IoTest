using System;
using System.Windows.Controls;
using System.Windows.Data;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.DiesSelection;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps
{
    /// <summary>
    /// Interaction logic for RecipeDiesSelectionStepsView.xaml
    /// </summary>
    /// 
    public enum SortField { Column, Row }

    public partial class RecipeDiesSelectionStepsView : UserControl
    {

        public ListCollectionView DiesList
        {
            get { return (CollectionView)CollectionViewSource.GetDefaultView(diesList.ItemsSource) as ListCollectionView; }
        }

        public RecipeDiesSelectionStepsView()
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(new Action(() => { DiesList.CustomSort = new DieSorter(SelectedSortField); })); // sort by columns by default
        }

        private SortField _isSelectedSortField;

        public SortField SelectedSortField
        {
            get { return _isSelectedSortField; }
            set
            {
                if (_isSelectedSortField != value)
                {
                    _isSelectedSortField = value;
                    DiesList.CustomSort = new DieSorter(_isSelectedSortField);
                }
            }
        }
    }
}
