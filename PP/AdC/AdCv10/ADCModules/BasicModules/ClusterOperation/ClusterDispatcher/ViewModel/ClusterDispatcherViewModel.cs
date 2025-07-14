using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.ClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // View Model
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ClusterDispatcherViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public ClusterDispatcherParameter Parameter;

        // DataTable pour la DataGrid
        //...........................
        public DataTable DataTable { get; private set; }

        // Colonnes de la datagrid
        public ObservableCollection<DataGridColumn> ColumnCollection { get; private set; }

        // Lignes de la grille
        public List<DispatcherDefectClassViewModel> RowViewModelList;


        // Information provenant des autres modules (classif, carac, ...)
        //..........................................................
        public HashSet<string> AvailableDefectClassList { get; private set; }

        //=================================================================
        // Constructor
        //=================================================================
        public ClusterDispatcherViewModel(ClusterDispatcherParameter parameter)
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
            AvailableDefectClassList = Parameter.FindAvailableDefectLabels();

            //-------------------------------------------------------------
            // Création de la DataTable
            //-------------------------------------------------------------
            SynchronizeDefectClassList();
            CreateDataTable();
        }

        //=================================================================
        // Verifie que toutes les classes de défauts sont bien gérées par
        // l'object Parameter.
        //=================================================================
        private void SynchronizeDefectClassList()
        {
            foreach (string defectClass in AvailableDefectClassList)
            {
                DispatcherDefectClass dispatcherDefectClass = Parameter.DefectClassList.Find(x => x.DefectClass == defectClass);
                if (dispatcherDefectClass == null)
                {
                    dispatcherDefectClass = new DispatcherDefectClass();
                    dispatcherDefectClass.DefectClass = defectClass;

                    Parameter.DefectClassList.Add(dispatcherDefectClass);
                }
            }
        }

        //=================================================================
        //
        //=================================================================
        private void CreateDataTable()
        {
            DataTable = new DataTable();
            ColumnCollection = new ObservableCollection<DataGridColumn>();
            RowViewModelList = new List<DispatcherDefectClassViewModel>();

            //-------------------------------------------------------------
            // Création des colonnes
            //-------------------------------------------------------------
            DataGridColumn templatecol; // Template pour la DataGrid
            DataColumn col;             // Colonne de la DataTable

            //TODO usefull ? table.PrimaryKey = new DataColumn[] { col };

            col = DataTable.Columns.Add("Defect Class", typeof(DispatcherDefectClassViewModel));
            templatecol = NewTemplateColumn(col.ColumnName, "TextBoxTemplate.xaml", binding: col.ColumnName + ".DefectClass");
            ColumnCollection.Add(templatecol);

            for (int i = 0; i < Parameter.Module.Children.Count; i++)
            {
                ModuleBase module = Parameter.Module.Children[i];
                if (module is TerminationModule)
                    continue;

                string branch = "Branch" + i;
                col = DataTable.Columns.Add(branch, typeof(BranchBooleanViewModel));

                string colunmHeader = "Child branch " + i + ":\n  " + module.ToString() + "  ";
                templatecol = NewTemplateColumn(colunmHeader, "CheckBoxTemplate.xaml", binding: col.ColumnName + ".Bool");
                ColumnCollection.Add(templatecol);
            }

            //-------------------------------------------------------------
            // Création des lignes
            //-------------------------------------------------------------
            foreach (DispatcherDefectClass dispatcherDefectClass in Parameter.DefectClassList)
            {
                DataTableAddRow(dispatcherDefectClass);
            }

            //-------------------------------------------------------------
            // Mise à jour de DataTable
            //-------------------------------------------------------------
            OnPropertyChanged(nameof(DataTable));
        }

        //=================================================================
        //
        //=================================================================
        private DataGridTemplateColumn NewTemplateColumn(string header, string xaml, string binding = "")
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn();

            xaml = "BasicModules.ClusterOperation.ClusterDispatcher.View." + xaml;
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(xaml);
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string template_str = reader.ReadToEnd();
                template_str = template_str.Replace("BINDING", binding);
                column.CellTemplate = (DataTemplate)System.Windows.Markup.XamlReader.Parse(template_str);
            }

            column.Header = header;
            column.CanUserSort = true;
            column.MinWidth = 30;

            return column;
        }

        //=================================================================
        // 
        //=================================================================
        public void DataTableAddRow(DispatcherDefectClass dispatcherDefectClass)
        {
            DispatcherDefectClassViewModel rowViewModel = new DispatcherDefectClassViewModel(this, dispatcherDefectClass);

            DataRow row = DataTable.Rows.Add();

            int c = 0;
            row[c++] = rowViewModel;
            for (int i = 0; i < rowViewModel.branchTable.Count(); i++)
                row[c++] = rowViewModel.branchTable[i];

            RowViewModelList.Add(rowViewModel);
        }

    }
}
