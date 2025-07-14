using System;
using System.Collections.Generic;
using System.Linq;

using AdcBasicObjects.DefectLabels.View;
using AdcBasicObjects.DefectLabels.ViewModel;

using AdcTools;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;


namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// List de labels pour les classes/catégories de défauts. Ces listes 
    /// sont stockées sur disque (en XML).
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class DefectLabelStore
    {
        /// <summary>
        /// Catégories de défauts que le client veut détecter. 
        /// En général, une catégorie regroupe plusieurs classes de défaut.
        /// </summary>
        public static DefectLabelStore Categories = LoadSettingsAndVids("DefectCategories.xml");

        /// <summary>
        /// Classes de défauts que l'ADC peut détecter.
        /// Ne pas confondre avec les catégories qui sont pour le client.
        /// </summary>
        public static DefectLabelStore Classes = LoadSettings("DefectClasses.xml");

        //=================================================================
        // 
        //=================================================================
        private PathString path;

        public List<string> LabelList = new List<string>();

        //=================================================================
        // 
        //=================================================================
        public bool AddLabel(string label)
        {
            bool added = LabelList.TryAdd(label);
            if (added)
                SaveSettings();
            return added;
        }

        //=================================================================
        // 
        //=================================================================
        public void RemoveLabel(string label)
        {
            LabelList.Remove(label);
            SaveSettings();
        }

        //=================================================================
        // 
        //=================================================================
        public bool RenameLabel(string oldlabel, string newlabel)
        {
            if (LabelList.Contains(newlabel))
                return false;

            int idx = LabelList.IndexOf(oldlabel);
            LabelList[idx] = newlabel;
            SaveSettings();
            return true;
        }

        //=================================================================
        // 
        //=================================================================
        private static DefectLabelStore LoadSettings(PathString path)
        {
            path = PathString.GetExecutingAssemblyPath().Directory / path;

            DefectLabelStore df;
            try
            {
                df = XML.Deserialize<DefectLabelStore>(path);
            }
            catch
            {
                df = new DefectLabelStore();
            }

            df.path = path;
            return df;
        }

        //=================================================================
        // 
        //=================================================================
        private static DefectLabelStore LoadSettingsAndVids(PathString path)
        {
            DefectLabelStore df = LoadSettings(path);
            LoadVids(df);
            return df;
        }

        //=================================================================
        // 
        //=================================================================
        private static void LoadVids(DefectLabelStore df)
        {
            var dbToolServiceForVid = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
            var vidlist = dbToolServiceForVid.GetAllVid().OrderBy(v => v.Id).ToList();
            foreach (var vid in vidlist)
                df.LabelList.TryAdd(vid.Label);
        }

        //=================================================================
        // 
        //=================================================================
        private void SaveSettings()
        {
            try
            {
                this.Serialize(path);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox.Show("Failed to save: " + path, ex);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public string Popup()
        {
            DefectLabelStoreView dlg = new DefectLabelStoreView();
            DefectLabelStoreViewModel vm = new DefectLabelStoreViewModel(this);
            dlg.MultipleSelectionMode = true;
            dlg.DataContext = vm;

            bool? res = dlg.ShowDialog();
            if (res != true)
                return null;

            return vm.SelectedDefectClasses[0];
        }

        //=================================================================
        // 
        //=================================================================
        public List<string> Popup(List<string> DisabledClassList = null)
        {
            DefectLabelStoreView dlg = new DefectLabelStoreView();
            DefectLabelStoreViewModel vm = new DefectLabelStoreViewModel(this);
            vm.FilterDefectClassList(DisabledClassList);
            dlg.DataContext = vm;
            dlg.MultipleSelectionMode = true;

            bool? res = dlg.ShowDialog();
            if (res != true)
                return new List<string>();

            return vm.SelectedDefectClasses;
        }

    }
}
