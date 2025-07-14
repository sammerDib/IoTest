using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.Tools;

namespace BasicModules.KlarfEditor
{
    public class KlarfDefectColorParameters : ClassificationParameterBase
    {
        // NB: map des defectlabel venant des autres modules:       
        internal CustomExceptionDictionary<string, KlarfDefectColorCategory> LabelToCategoryMap { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public KlarfDefectColorParameters(KlarfEditorModule module, string name)
            : base(module, name)
        {
            LabelToCategoryMap = new CustomExceptionDictionary<string, KlarfDefectColorCategory>(exceptionKeyName: "DefectLabel");
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            foreach (XmlNode node in xmlNodes)
            {
                if (node.Attributes["Label"].Value == "DefectCategory")
                {
                    // restore de la recette
                    KlarfDefectColorCategory defectCategory = new KlarfDefectColorCategory();
                    defectCategory.DefectLabel = node.GetAttributeValue("DefectLabel");
                    defectCategory.Color = node.GetAttributeValue("Color");

                    LabelToCategoryMap.Add(defectCategory.DefectLabel, defectCategory);
                }
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            // Sauvegarde en recette de la config
            foreach (KlarfDefectColorCategory defectCategory in LabelToCategoryMap.Values)
            {
                XmlElement node = SaveParameter(xmlNode, "DefectCategory", defectCategory.DefectLabel);
                node.SetAttribute("DefectLabel", defectCategory.DefectLabel.ToString());
                node.SetAttribute("Color", defectCategory.Color);
            }
            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private KlarfEditorColorViewModel _parameterViewModel;
        private KlarfEditorColorControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new KlarfEditorColorViewModel(this);
                    _parameterUI = new KlarfEditorColorControl(_parameterViewModel);
                }
                return _parameterUI;
            }
        }

        ///=================================================================
        /// <summary>
        /// Synchronise les classes/catégories de défaut avec les modules 
        /// parents (Classification).
        /// </summary>
        /// <returns>True if the module uses VIDs, false otherwise</returns>
        ///=================================================================
        public bool Synchronize()
        {
            bool changed = false;


            HashSet<string> DefectLabelList = new HashSet<string>();
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            DefectLabelList = FindAvailableDefectLabels();

            ModuleBase DeepAncestor = Module.FindAncestors(mod => mod.Name.Contains("DeepControl")).FirstOrDefault();

            if (DeepAncestor != null)
            {
                DefectLabelList.UnionWith(((DeepControl.DeepControlModule)DeepAncestor).DefectClassLabelList);
            }

            #region PLSEditor Obsolete to refacto full feature
            // var plsAncestor = (PLSEditor.PLSEditorModule)Module.FindAncestors(mod => mod is PLSEditor.PLSEditorModule).FirstOrDefault();
            // if (plsAncestor == null) { DefectLabelList = FindAvailableDefectLabels(); }
            // else
            // //-------------------------------------------------------------
            // // Récupération des infos du Pls s'il est présent
            // //-------------------------------------------------------------
            // //if (!(Module is PLSEditorModule))
            // {
            //     DefectLabelList.UnionWith(plsAncestor.ListDefectLabel);
            // }
            #endregion

            //-------------------------------------------------------------
            // On contruit une liste label -> Couleur
            //-------------------------------------------------------------               

            // Purge les labels non utilisés
            //............................
            List<KlarfDefectColorCategory> CategoryList = LabelToCategoryMap.Values.ToList();
            foreach (KlarfDefectColorCategory cat in CategoryList)
            {
                if (!DefectLabelList.Contains(cat.DefectLabel))
                {
                    LabelToCategoryMap.Remove(cat.DefectLabel);
                    changed = true;
                }
            }

            // Ajoute les nouveaux Labels
            //.........................

            foreach (string label in DefectLabelList)
            {
                bool missing = !LabelToCategoryMap.ContainsKey(label);
                if (missing)
                {
                    KlarfDefectColorCategory klarfDefect = new KlarfDefectColorCategory();
                    klarfDefect.DefectLabel = label;
                    klarfDefect.Color = "white";

                    LabelToCategoryMap.Add(klarfDefect.DefectLabel, klarfDefect);
                    changed = true;
                }
            }

            //-------------------------------------------------------------
            // Retour
            //-------------------------------------------------------------
            if (changed)
                ReportChange();
            return true;
        }

        //=================================================================
        //
        //=================================================================
        public override string Validate()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            HashSet<string> DefectLabelList = FindAvailableDefectLabelsWithDummyDefect();

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as KlarfDefectColorParameters;
            return parameter != null &&
                   LabelToCategoryMap.DictionaryEqual(parameter.LabelToCategoryMap);
        }
    }
}
