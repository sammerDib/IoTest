using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedModules.ClassificationMultiLayer
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ViewModel principal de la Classification Multicouche
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ClassificationMultiLayerViewModel : ObservableRecipient
    {
        //=================================================================
        // Proriétées
        //=================================================================
        public ClassificationMultiLayerParameter Parameter { get; set; }
        public ClassificationMultiLayerModule Module { get { return (ClassificationMultiLayerModule)Parameter.Module; } }

        // Colonnes de la DataGrid
        public List<DataGridColumn> Columns { get; private set; }

        // Lignes de la DataGrid
        public ObservableCollection<MultiLayerDefectClassViewModel> RowViewModelList { get; private set; }

        // Information provenant des autres modules (classif, carac, ...)
        //..........................................................
        public HashSet<string> AvailableDefectLabelList { get; private set; }
        public List<string> BranchList { get; private set; }
        public List<KeyValuePair<int, string>> AvailableBranchListwithAutomatic { get; private set; }
        public List<Characteristic> AvailableCharacteristicList { get; private set; }

        //=================================================================
        // Constructeurs
        //=================================================================
        public ClassificationMultiLayerViewModel(ClassificationMultiLayerParameter parameter)
        {
            Columns = new List<DataGridColumn>();
            Parameter = parameter;
        }

        //=================================================================
        //
        //=================================================================
        public void Init()
        {
            bool ok = Parameter.SynchronizeDefectClassList();
            if (!ok)
                return;
            BuildLists();
            CreateDataTable();
        }


        //=================================================================
        // Récupération des infos des autres modules
        //=================================================================
        public void BuildLists()
        {
            //-------------------------------------------------------------
            // Liste des Branches
            //-------------------------------------------------------------
            ModuleBase cmc = Module.FindAncestors(mod => mod is CmcNamespace.CmcModule).FirstOrDefault();
            if (cmc == null)
                throw new ApplicationException("Can't find CMC module");

            BranchList = new List<string>();
            for (int i = 0; i < cmc.Parents.Count; i++)
            {
                ModuleBase parent = cmc.Parents[i];
                string name = "Parent Branch " + i + ":\n    " + parent.ToString() + "\n    " + GetLayerName(parent);
                BranchList.Add(name);
            }

            // Avec Automatic
            //...............
            AvailableBranchListwithAutomatic = new List<KeyValuePair<int, string>>();
            AvailableBranchListwithAutomatic.Add(new KeyValuePair<int, string>(-1, "automatic"));
            for (int i = 0; i < BranchList.Count; i++)
                AvailableBranchListwithAutomatic.Add(new KeyValuePair<int, string>(i, BranchList[i]));

            //-------------------------------------------------------------
            // Liste des Caractéristiques
            //-------------------------------------------------------------
            AvailableCharacteristicList = new List<Characteristic>(
                from c in Parameter.FindAvailableCharacteristics()
                where c.Type == typeof(double)
                select c
                );

            //-------------------------------------------------------------
            // Liste des Labels
            //-------------------------------------------------------------
            AvailableDefectLabelList = Parameter.FindAvailableDefectLabels();
        }

        //=================================================================
        // NB: ce n'est plus une vraie DataTable
        //=================================================================
        private void CreateDataTable()
        {
            CreateColumns();
            CreateLines();
        }

        //=================================================================
        //
        //=================================================================
        private void CreateColumns()
        {
            Columns.Clear();

            DataGridColumn templatecol;

            // Defect Labels ( 1ère colonne)
            //..............................
            DataGridTextColumn textcol = new DataGridTextColumn();
            textcol.Header = "Defect Class";
            textcol.Binding = new System.Windows.Data.Binding("DefectLabel");
            textcol.IsReadOnly = true;
            Columns.Add(textcol);

            // Layers (autres colonnes)
            //.........................
            for (int i = 0; i < BranchList.Count; i++)
            {
                string binding = "LayerTable[" + i + "]";
                DataGridTemplateColumn templcol = NewTemplateColumn(BranchList[i], "CheckBoxTemplate.xaml", binding);
                templcol.MinWidth = 30;
                Columns.Add(templcol);
            }

            // Mesure (deux dernières colonnes)
            //.................................
            templatecol = NewTemplateColumn("Measured Branch", "MeasuredBranchTemplate.xaml", binding: "MeasuredBranch");
            Columns.Add(templatecol);

            templatecol = NewTemplateColumn("Characteristic", "CharacteristicForAutomaticMeasureTemplate.xaml");
            Columns.Add(templatecol);
        }

        //=================================================================
        //
        //=================================================================
        private void CreateLines()
        {
            RowViewModelList = new ObservableCollection<MultiLayerDefectClassViewModel>();

            foreach (MultiLayerDefectClass multiLayerDefectClass in Parameter.DefectClassList)
            {
                MultiLayerDefectClassViewModel rowViewModel = new MultiLayerDefectClassViewModel(this, multiLayerDefectClass);
                RowViewModelList.Add(rowViewModel);
            }

            OnPropertyChanged(nameof(RowViewModelList));
        }

        //=================================================================
        //
        //=================================================================
        private DataGridTemplateColumn NewTemplateColumn(string header, string xaml, string binding = "")
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn();

            xaml = "AdvancedModules.Classification.ClassificationMultiLayer.View." + xaml;
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(xaml);
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string template_str = reader.ReadToEnd();
                template_str = template_str.Replace("BINDING", binding);
                column.CellTemplate = (DataTemplate)System.Windows.Markup.XamlReader.Parse(template_str);
            }

            column.Header = header;
            column.MinWidth = 30;

            return column;
        }

        //=================================================================
        // MoveRow
        //=================================================================
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
            // On bouge la Row dans la DataTable
            //..................................
            int idx = RowViewModelList.MoveItem(oldpos, newpos);
            if (idx < 0)
                return idx;

            // On bouge aussi le paramètre
            //............................
            Parameter.DefectClassList.MoveItem(oldpos, newpos);
            Parameter.ReportChange();

            // Sanity check
            //.............
            if (RowViewModelList[newpos].MultiLayerDefectClass != Parameter.DefectClassList[newpos])
                throw new ApplicationException("error in the data table");

            return newpos;
        }

        //=================================================================
        //
        //=================================================================
        private string GetLayerName(ModuleBase module)
        {
            List<ModuleBase> loaders = module.FindAncestors(m => m is IDataLoader);
            if (loaders.Count == 0)
                return "<no layer>";

            IDataLoader loader = (IDataLoader)loaders[0];
            return loader.LayerName;
        }


    }
}
