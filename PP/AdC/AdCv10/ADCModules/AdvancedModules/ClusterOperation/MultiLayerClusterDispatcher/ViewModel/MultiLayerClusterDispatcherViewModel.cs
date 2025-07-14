using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedModules.MultiLayerClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // View Model
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class MultiLayerClusterDispatcherViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public MultiLayerClusterDispatcherParameter Parameter;

        // Colonnes de la datagrid
        public List<DataGridColumn> ColumnList { get; private set; } = new List<DataGridColumn>();

        // Lignes de la grille
        private ObservableCollection<DispatcherDefectClassViewModel> RowViewModelList;
        public ICollectionView CollectionView { get; private set; }

        // Information provenant des autres modules (classif, carac, ...)
        //..........................................................
        public HashSet<string> AvailableDefectClassList { get; private set; }
        public List<string> Branches { get; private set; }

        private List<string> bindings;

        //=================================================================
        // Constructor
        //=================================================================
        public MultiLayerClusterDispatcherViewModel(MultiLayerClusterDispatcherParameter parameter)
        {
            Parameter = parameter;
        }

        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            Parameter.Synchronize();

            //-------------------------------------------------------------
            // Création de la DataTable (NB: ce n'est plus une vraie DataTable)
            //-------------------------------------------------------------
            CreateDataTableColumns();
            CreateDataTableRows();

            OnPropertyChanged(nameof(ColumnList));
        }

        //=================================================================
        //
        //=================================================================
        private void CreateDataTableColumns()
        {
            ColumnList = new List<DataGridColumn>();
            bindings = new List<string>();
            Branches = new List<string>();

            //-------------------------------------------------------------
            // Colonnes pour les labels/layers
            //-------------------------------------------------------------
            DataGridColumn templatecol;

            templatecol = NewTemplateColumn("Defect Class", "TextBoxTemplate.xaml", binding: "DefectLabel");
            ColumnList.Add(templatecol);
            bindings.Add("DefectLabel");

            templatecol = NewTemplateColumn("Layer", "TextBoxTemplate.xaml", binding: "DefectLayer");
            ColumnList.Add(templatecol);
            bindings.Add("DefectLayer");

            //-------------------------------------------------------------
            // Colonnes pour les branches
            //-------------------------------------------------------------
            for (int i = 0; i < Parameter.Module.Children.Count; i++)
            {
                ModuleBase module = Parameter.Module.Children[i];
                if (module is TerminationModule)
                    continue;

                string binding = "BranchTable[" + i + "].Bool";
                string colunmHeader = "Child branch " + i + ":\n  " + module.ToString() + "  ";
                templatecol = NewTemplateColumn(colunmHeader, "CheckBoxTemplate.xaml", binding);
                ColumnList.Add(templatecol);
                bindings.Add(binding);
                Branches.Add(colunmHeader.Replace("\n", "\t"));
            }
        }

        //=================================================================
        //
        //=================================================================
        private void CreateDataTableRows()
        {
            if (RowViewModelList == null)
            {
                RowViewModelList = new ObservableCollection<DispatcherDefectClassViewModel>();
                CollectionView = CollectionViewSource.GetDefaultView(RowViewModelList);
                OnPropertyChanged(nameof(CollectionView));
            }
            RowViewModelList.Clear();

            //-------------------------------------------------------------
            // Création des lignes
            //-------------------------------------------------------------
            foreach (DispatcherDefectClass dispatcherDefectClass in Parameter.DefectClassList)
            {
                DispatcherDefectClassViewModel rowViewModel = new DispatcherDefectClassViewModel(this, dispatcherDefectClass);
                RowViewModelList.Add(rowViewModel);
            }

            //-------------------------------------------------------------
            // Tri
            //-------------------------------------------------------------
            GroupBy(columnIndex: 0);
        }

        //=================================================================
        //
        //=================================================================
        private DataGridTemplateColumn NewTemplateColumn(string header, string xaml, string binding = "")
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn();
            column.Header = header;
            column.CanUserSort = true;
            column.MinWidth = 30;

            xaml = "AdvancedModules.ClusterOperation.MultiLayerClusterDispatcher.View." + xaml;
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(xaml);
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string template_str = reader.ReadToEnd();
                template_str = template_str.Replace("BINDING", binding);
                column.CellTemplate = (DataTemplate)System.Windows.Markup.XamlReader.Parse(template_str);
            }

            return column;
        }

        //=================================================================
        //
        //=================================================================
        public void GroupBy(int columnIndex)
        {
            //-------------------------------------------------------------
            // Tri / Filtrage
            //-------------------------------------------------------------
            CollectionView.SortDescriptions.Clear();
            CollectionView.GroupDescriptions.Clear();

            if (columnIndex < 2)
            {
                CollectionView.SortDescriptions.Add(new SortDescription(bindings[columnIndex], ListSortDirection.Ascending));
                CollectionView.SortDescriptions.Add(new SortDescription(bindings[1 - columnIndex], ListSortDirection.Ascending));
            }
            else
            {
                CollectionView.SortDescriptions.Add(new SortDescription(bindings[columnIndex], ListSortDirection.Descending));
                for (int i = 2; i < bindings.Count; i++)
                {
                    if (i != columnIndex)
                        CollectionView.SortDescriptions.Add(new SortDescription(bindings[i], ListSortDirection.Descending));
                }
                CollectionView.SortDescriptions.Add(new SortDescription(bindings[0], ListSortDirection.Ascending));
                CollectionView.SortDescriptions.Add(new SortDescription(bindings[1], ListSortDirection.Ascending));
            }

            CollectionView.Refresh();

            //-------------------------------------------------------------
            // Colorisation
            //-------------------------------------------------------------
            string old = null;
            int idx = 0;
            Brush[] brushes = { Brushes.LightGray, Brushes.WhiteSmoke };
            foreach (DispatcherDefectClassViewModel item in CollectionView)
            {
                string neww;
                switch (columnIndex)
                {
                    case 0:
                        neww = item.DefectLabel;
                        break;
                    case 1:
                        neww = item.DefectLayer;
                        break;
                    default:
                        neww = item.BranchIndex.ToString();
                        break;
                }

                if (neww != old)
                {
                    old = neww;
                    idx = (idx + 1) % brushes.Length;
                }
                item.Brush = brushes[idx];
            }
        }

    }
}
