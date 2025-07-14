using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public class GridStatsVM : ObservableObject
    {
        private GridView _gridView;

        public GridView GridView
        {
            get => _gridView;
            set
            {
                if (_gridView != value) { _gridView = value; OnPropertyChanged(); }
            }
        }

        public DataView _dataView;

        public DataView DataView
        {
            get => _dataView;
            set
            {
                if (_dataView != value) { _dataView = value; OnPropertyChanged(); }
            }
        }

        #region Methods

        /// <summary>
        /// Generate lot statistics data.
        /// </summary>
        /// <param name="resultValues"></param>
        public void Generate(List<WaferStatsData> lotStatsData, Dictionary<string, DefectBin> legends)
        {
            if (lotStatsData.Count > 0)
            {
                var slotsIds = lotStatsData.Select(x => x.SlotId).Distinct().ToList();
                var defecClasses = lotStatsData.OrderBy(x => x.ResultValue.Name).Select(x => x.ResultValue.Name).Distinct().ToList();
                var defectsLabels = legends.Values.OrderBy(x => x.Label).Select(x => x.Label).ToList();
                // Create gridView columns
                CreateGridViewColumns(defectsLabels);

                // Create dataTable
                var dt = new DataTable();
                // Création des colonnes du dataTable
                dt.Columns.Add("SlotId");
                dt.Columns.Add("State");
                foreach (string defectClass in defectsLabels)
                {
                    dt.Columns.Add(defectClass.ToString());
                }
                dt.Columns.Add("Total");

                // On recupere les données de chaque slot et on remplit la ligne
                foreach (int slotId in slotsIds)
                {
                    var row = dt.NewRow();
                    row["SlotId"] = slotId;
                    row["State"] = lotStatsData.Where(x => x.SlotId == slotId).Select(x => x.State).FirstOrDefault();
                    foreach (string defectClass in defecClasses)
                    {
                        double defectTotalAccount = lotStatsData.Where(x => x.SlotId == slotId && x.ResultValue.Name == defectClass)
                                                             .Select(x => x.ResultValue.Value)
                                                             .ToList()
                                                             .DefaultIfEmpty(0)
                                                             .FirstOrDefault();
                        string colName = legends[defectClass].Label;
                        row[colName] = defectTotalAccount;
                    }

                    dt.Rows.Add(row);
                    row["Total"] = lotStatsData.Where(x => x.SlotId == slotId).Sum(x => x.ResultValue.Value);
                }
                var lastRow = dt.NewRow();
                lastRow["SlotId"] = "Total";
                foreach (string defectClass in defecClasses)
                {
                    string colName = legends[defectClass].Label;
                    lastRow[colName] = lotStatsData.Where(x => x.ResultValue.Name == defectClass).Sum(x => x.ResultValue.Value);
                }
                lastRow["Total"] = lotStatsData.Sum(x => x.ResultValue.Value);
                dt.Rows.Add(lastRow);
                // Update dataTable
                DataView = dt.DefaultView;
            }
            else
            {
                GridView = null;
                DataView = null;
            }
        }

        /// <summary>
        /// Create gridView columns dynamically.
        /// </summary>
        /// <param name="defecClasses"></param>
        private void CreateGridViewColumns(List<string> defecClasses)
        {
            var gridView = new GridView();

            gridView.Columns.Add(new GridViewColumn() { Header = "SlotId", DisplayMemberBinding = new Binding("SlotId"), Width = double.NaN });
            gridView.Columns.Add(new GridViewColumn() { Header = "State", DisplayMemberBinding = new Binding("State"), Width = double.NaN });
            foreach (string defectCcass in defecClasses)
            {
                gridView.Columns.Add(new GridViewColumn() { Header = defectCcass, DisplayMemberBinding = new Binding(defectCcass), Width = double.NaN });
            }
            gridView.Columns.Add(new GridViewColumn() { Header = "Total", DisplayMemberBinding = new Binding("Total"), Width = double.NaN });
            GridView = gridView;
        }

        #endregion Methods
    }
}
