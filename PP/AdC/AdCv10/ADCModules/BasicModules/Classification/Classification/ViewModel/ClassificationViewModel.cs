using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using AdcBasicObjects;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Classification
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ClassificationViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public DataTable DataTable { get; private set; }

        private ClassificationParameter _parameter;

        //=================================================================
        // Constructor
        //=================================================================
        public ClassificationViewModel(ClassificationParameter parameter)
        {
            _parameter = parameter;
        }

        //=================================================================
        //
        //=================================================================
        public void Init()
        {
            ReleaseDataTable();
            CreateDataTable();
            _parameter.Synchronize();
        }

        //=================================================================
        //
        //=================================================================
        public void Release()
        {
            ReleaseDataTable();
        }

        //=================================================================
        //
        //=================================================================
        private void CreateDataTable()
        {
            DataTable table = new DataTable();
            List<Characteristic> caracs = _parameter.FindAvailableCharacteristics().ToList();

            // Création des Colonnes
            //......................
            var col = table.Columns.Add("Defect Class", typeof(ClassificationDefectClassViewModel));
            //TODO usefull ? table.PrimaryKey = new DataColumn[] { col };
            foreach (Characteristic carac in caracs)
            {
                col = table.Columns.Add(carac.Name, typeof(ComparatorBase));
                table.ExtendedProperties.Add(col.Ordinal, carac);
            }

            // Remplissage des lignes
            //.......................
            foreach (DefectClass defectClass in _parameter.DefectClassList)
            {
                DataRow row = table.Rows.Add();
                ClassificationDefectClassViewModel defectClassViewModel = new ClassificationDefectClassViewModel(_parameter, defectClass);
                row[0] = defectClassViewModel;
                for (int i = 0; i < caracs.Count(); i++)
                {
                    Characteristic carac = caracs[i];
                    foreach (ComparatorBase cmp in defectClass.compartorList)
                    {
                        if (cmp.characteristic == carac)
                            row[i + 1] = cmp;
                    }
                }
            }

            // Gestion des events
            //...................
            table.TableNewRow += Table_RowCreated;
            table.RowChanged += Table_RowChanged;
            table.RowDeleting += Table_RowDeleting;

            DataTable = table;
            OnPropertyChanged(nameof(DataTable));
        }

        //=================================================================
        // 
        //=================================================================
        private ClassificationDefectClassViewModel GetRowViewModel(DataRow row)
        {
            return (ClassificationDefectClassViewModel)row[0];
        }

        //=================================================================
        // Unsubscribe des events pour éviter les memory leaks
        //=================================================================
        private void ReleaseDataTable()
        {
            if (DataTable != null)
            {
                DataTable.TableNewRow -= Table_RowCreated;
                DataTable.RowChanged -= Table_RowChanged;
                DataTable.RowDeleting -= Table_RowDeleting;
                DataTable = null;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public bool AddDefectClass()
        {
            // Selection d'un label
            //.....................
            List<string> labels = SelectDefectClass(multiple: true);
            if (labels.Count == 0)
                return false;

            // Création d'une ligne dans la DataTable
            //.......................................
            foreach (string label in labels)
            {
                DataTable table = DataTable;
                DataRow row = table.Rows.Add();
                ClassificationDefectClassViewModel defectClassViewModel = GetRowViewModel(row);
                defectClassViewModel.Label = label;
            }

            return true;
        }

        //=================================================================
        // 
        //=================================================================
        private List<string> SelectDefectClass(bool multiple)
        {
            // Liste des labels déjà utilisés
            //...............................
            var alreadyUsedLabels = from d in _parameter.DefectClassList select d.label;

            // Dialogue pour sélectionner le label
            //....................................
            List<string> list = DefectLabelStore.Classes.Popup(alreadyUsedLabels.ToList());
            return list;
        }

        //=================================================================
        // Events de la DataTable
        //=================================================================
        private void Table_RowCreated(object sender, DataTableNewRowEventArgs e)
        {
            if (isMovingRows)
                return;

            // Création de la DefectClass dans le Parameter
            //.............................................
            var defectClass = new DefectClass();
            defectClass.label = "xxx";
            _parameter.DefectClassList.Add(defectClass);
            _parameter.ReportChange();

            // Ajout dans la DataTable
            //........................
            var row = e.Row;
            ClassificationDefectClassViewModel defectClassViewModel = new ClassificationDefectClassViewModel(_parameter, defectClass);
            row[0] = defectClassViewModel;
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (isMovingRows)
                return;

            if (e.Action == DataRowAction.Add)
            {
                DataRow row = e.Row;
                if (!(row[0] is ClassificationDefectClassViewModel))
                {
                    // Création de la DefectClass dans le Parameter
                    //.............................................
                    DefectClass defectClass = new DefectClass();
                    defectClass.label = "xxx";
                    _parameter.DefectClassList.Add(defectClass);
                    _parameter.ReportChange();

                    // Ajout dans la DataTable
                    //........................
                    ClassificationDefectClassViewModel defectClassViewModel = new ClassificationDefectClassViewModel(_parameter, defectClass);
                    row[0] = defectClassViewModel;
                }
            }
        }

        private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (isMovingRows)
                return;

            ClassificationDefectClassViewModel defectClassViewModel = GetRowViewModel(e.Row);
            _parameter.DefectClassList.Remove(defectClassViewModel.DefectClass);
            _parameter.ReportChange();
        }

        //=================================================================
        // MoveRow
        //=================================================================
        private bool isMovingRows;

        public int MoveRowUp(int index)
        {
            return MoveRow(index, index - 1);
        }

        public int MoveRowDown(int index)
        {
            return MoveRow(index, index + 1);
        }

        private int MoveRow(int oldpos, int newpos)
        {
            isMovingRows = true;

            try
            {
                // On bouge la Row dans la DataTable
                //..................................
                int idx = DataTable.MoveRow(oldpos, newpos);
                if (idx < 0)
                    return idx;

                // On bouge aussi dans le Paramètre
                //.................................
                _parameter.DefectClassList.MoveItem(oldpos, newpos);
                _parameter.ReportChange();

                // Sanity check
                //.............
                ClassificationDefectClassViewModel defectClassViewModel2 = GetRowViewModel(DataTable.Rows[newpos]);
                if (_parameter.DefectClassList[newpos] != defectClassViewModel2.DefectClass)
                    throw new ApplicationException("error in the datatable order");
            }
            finally
            {
                isMovingRows = false;
            }

            return newpos;
        }

        //=================================================================
        // On a notre propre éditeur.
        // <returns> true si la cellule a un éditeur custom
        // false si'il faut utiliser l'éditeur par défaut</returns>
        //=================================================================
        public bool EditCell(DataRow row, int columnIndex)
        {
            if (columnIndex == 0)
                EditLabelCell(row);
            else
                EditComparatorCell(row, columnIndex);

            return true;    // traité = true
        }

        //=================================================================
        // Sélection de la Defect Class
        //=================================================================
        public void EditLabelCell(DataRow row)
        {
            ClassificationDefectClassViewModel defectClassViewModel = GetRowViewModel(row);
            List<string> labels = SelectDefectClass(multiple: false);
            if (labels.Count != 1)
                return;

            defectClassViewModel.Label = labels[0];
            _parameter.ReportChange();

            // Mise à jour de l'IHM.
            // NB: il n'est pas possible de notifier car la cellule affiche 
            // le ToString(). Et il est compliqué d'utiliser le DisplayMember.
            row[0] = defectClassViewModel;
        }

        //=================================================================
        //
        //=================================================================
        public void EditComparatorCell(DataRow row, int columnIndex)
        {
            //-------------------------------------------------------------
            // Récupération des infos
            //-------------------------------------------------------------
            ClassificationDefectClassViewModel defectClassViewModel = GetRowViewModel(row);
            DefectClass defectClass = defectClassViewModel.DefectClass;
            var item = row[columnIndex];
            if (item == DBNull.Value)
                item = null;
            Characteristic carac = (Characteristic)DataTable.ExtendedProperties[columnIndex];
            ComparatorBase cmp = (ComparatorBase)item;

            //-------------------------------------------------------------
            // Affichage du dialog
            //-------------------------------------------------------------
            ComparatorViewModelBase viewmodel = ComparatorViewModelBase.GetEditorViewModel(carac.Type);
            viewmodel.Comparator = cmp;

            Window dlg = viewmodel.GetUI();
            dlg.Title = "Configure characteristic: " + carac;
            dlg.DataContext = viewmodel;
            dlg.ShowDialog();

            //-------------------------------------------------------------
            // Modification du comparateur
            //-------------------------------------------------------------
            if (dlg.DialogResult == true)
            {
                if (viewmodel.IsNull)
                {
                    if (cmp != null)
                        defectClass.compartorList.Remove(cmp);
                    row[columnIndex] = null;
                }
                else
                {
                    cmp = viewmodel.Comparator;
                    if (cmp.characteristic == null) // nouvelle carac
                    {
                        cmp.characteristic = carac;
                        defectClass.compartorList.Add(cmp);
                    }
                    row[columnIndex] = cmp;
                }
                _parameter.ReportChange();
            }
        }

        //=================================================================
        //
        //=================================================================
        private DataGridTemplateColumn NewTemplateColumn(string header, string xaml, string binding = "")
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn();

            xaml = "BasicModules.Classification.MilClassification." + xaml;
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


    }
}
