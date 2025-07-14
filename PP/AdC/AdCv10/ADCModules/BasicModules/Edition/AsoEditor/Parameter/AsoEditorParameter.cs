using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.Tools;

namespace BasicModules.AsoEditor
{
    internal class AsoEditorParameter : ClassificationParameterBase
    {
        // NB: Seule l'une de ces Map est remplie:
        // - soit DefectClassToCategoryMap si on n'a pas de VID
        // - soit VidToCategoryMap si on a des VIDs
        internal CustomExceptionDictionary<string, AsoDefectClass> DefectClassToCategoryMap { get; private set; }
        internal CustomExceptionDictionary<int, AsoDefectVidCategory> VidToCategoryMap { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public AsoEditorParameter(AsoEditorModule module, string name)
            : base(module, name)
        {
            DefectClassToCategoryMap = new CustomExceptionDictionary<string, AsoDefectClass>(exceptionKeyName: "DefectClass");
            VidToCategoryMap = new CustomExceptionDictionary<int, AsoDefectVidCategory>(exceptionKeyName: "VID");
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            foreach (XmlNode node in xmlNodes)
            {
                if (node.Attributes["Label"].Value == "DefectClass")
                {
                    // Sans VID
                    AsoDefectClass defectClass = new AsoDefectClass();
                    defectClass.DefectLabel = node.GetAttributeValue("Value");
                    defectClass.DefectCategory = node.GetAttributeValue("DefectCategory");
                    defectClass.Color = node.GetAttributeValue("Color");
                    defectClass.SaveThumbnails = bool.Parse(node.GetAttributeValue("SaveThumbnails"));

                    DefectClassToCategoryMap.Add(defectClass.DefectLabel, defectClass);
                }
                else if (node.Attributes["Label"].Value == "DefectCategory")
                {
                    // Avec VID
                    AsoDefectVidCategory defectCategory = new AsoDefectVidCategory();
                    defectCategory.DefectCategory = node.GetAttributeValue("Value");
                    defectCategory.VID = Int32.Parse(node.GetAttributeValue("VID"));
                    defectCategory.Color = node.GetAttributeValue("Color");
                    defectCategory.SaveThumbnails = bool.Parse(node.GetAttributeValue("SaveThumbnails"));

                    VidToCategoryMap.Add(defectCategory.VID, defectCategory);
                }
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            // Sans VID
            foreach (AsoDefectClass defectClass in DefectClassToCategoryMap.Values)
            {
                XmlElement node = SaveParameter(xmlNode, "DefectClass", defectClass.DefectLabel);
                node.SetAttribute("DefectCategory", defectClass.DefectCategory);
                node.SetAttribute("Color", defectClass.Color);
                node.SetAttribute("SaveThumbnails", defectClass.SaveThumbnails.ToString());
            }

            // Avec VID
            foreach (AsoDefectVidCategory defectCategory in VidToCategoryMap.Values)
            {
                XmlElement node = SaveParameter(xmlNode, "DefectCategory", defectCategory.DefectCategory);
                node.SetAttribute("VID", defectCategory.VID.ToString());
                node.SetAttribute("Color", defectCategory.Color);
                node.SetAttribute("SaveThumbnails", defectCategory.SaveThumbnails.ToString());
            }

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private AsoEditorViewModel _parameterViewModel;
        private AsoControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new AsoEditorViewModel(this);
                    _parameterUI = new AsoControl(_parameterViewModel);
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

            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            CustomExceptionDictionary<int, VidReport.ReportClass> VidList = FindAvailableVids();
            bool hasVids = (VidList.Count != 0);

            //-------------------------------------------------------------
            // Pas de VID, sélectionne directement la catégorie pour chaque classe
            //-------------------------------------------------------------
            if (!hasVids)
            {
                VidToCategoryMap.Clear();

                HashSet<string> DefectLabelList = FindAvailableDefectLabelsWithDummyDefect();


                // Purge les classes non utilisées
                //................................
                List<AsoDefectClass> ClassList = DefectClassToCategoryMap.Values.ToList();

                foreach (AsoDefectClass defectClass in ClassList)
                {
                    if (!DefectLabelList.Contains(defectClass.DefectLabel))
                    {
                        DefectClassToCategoryMap.Remove(defectClass.DefectLabel);
                        changed = true;
                    }
                }

                // Ajoute les nouvelles classes
                //.............................
                foreach (string label in DefectLabelList)
                {
                    bool missing = !DefectClassToCategoryMap.ContainsKey(label);
                    if (missing)
                    {
                        AsoDefectClass asoClass = new AsoDefectClass();
                        asoClass.DefectLabel = label;
                        asoClass.DefectCategory = label;

                        DefectClassToCategoryMap.Add(asoClass.DefectLabel, asoClass);
                        changed = true;
                    }
                }
            }
            else
            //-------------------------------------------------------------
            // VID, on contruit une liste VID -> Couleur
            //-------------------------------------------------------------
            {
                DefectClassToCategoryMap.Clear();

                // Purge les VIDs non utilisés
                //............................
                List<AsoDefectVidCategory> CategoryList = VidToCategoryMap.Values.ToList();
                foreach (AsoDefectVidCategory cat in CategoryList)
                {
                    if (!VidList.ContainsKey(cat.VID))
                    {
                        VidToCategoryMap.Remove(cat.VID);
                        changed = true;
                    }
                }

                // Ajoute les nouveaux VIDs
                //.........................
                foreach (VidReport.ReportClass vidReportClass in VidList.Values)
                {
                    bool missing = !VidToCategoryMap.ContainsKey(vidReportClass.VID);
                    if (missing)
                    {
                        AsoDefectVidCategory asoCategory = new AsoDefectVidCategory();
                        asoCategory.VID = vidReportClass.VID;
                        asoCategory.DefectCategory = vidReportClass.VidLabel != null ? vidReportClass.VidLabel : "Not defined";

                        VidToCategoryMap.Add(asoCategory.VID, asoCategory);
                        changed = true;
                    }
                }
            }

            //-------------------------------------------------------------
            // Retour
            //-------------------------------------------------------------
            if (changed)
                ReportChange();
            return hasVids;
        }

        //=================================================================
        //
        //=================================================================
        public override string Validate()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            CustomExceptionDictionary<int, VidReport.ReportClass> VidList = FindAvailableVids();
            bool hasVids = (VidList.Count != 0);

            //-------------------------------------------------------------
            // Pas de VID, sélectionne directement la catégorie pour chaque classe
            //-------------------------------------------------------------
            if (!hasVids)
            {
                HashSet<string> DefectLabelList = FindAvailableDefectLabelsWithDummyDefect();

                // Est-ce que des classes ont été ajoutées ?
                //..........................................
                int count = 0;
                string error = null;

                foreach (string label in DefectLabelList)
                {
                    bool missing = !DefectClassToCategoryMap.ContainsKey(label);
                    if (missing)
                    {
                        if (count++ < 5)
                        {
                            if (count > 1)
                                error += Environment.NewLine;
                            error += "New defect class: " + label;
                        }
                        else
                            return "New defect classes, review configuration";
                    }
                }

                if (error != null)
                    return "Review configuration," + error;

                // Est-ce que des classes ont été supprimées ?
                //............................................
                foreach (AsoDefectClass defectClass in DefectClassToCategoryMap.Values)
                {
                    if (!DefectLabelList.Contains(defectClass.DefectLabel))
                        return "Defect classes have changed, review configuration";
                }
            }
            else
            //-------------------------------------------------------------
            // VID, on contruit une liste VID -> Couleur
            //-------------------------------------------------------------
            {
                // Est-ce que des VIDs ont été ajoutés ?
                //......................................
                int count = 0;
                string error = null;

                foreach (VidReport.ReportClass vidReportClass in VidList.Values)
                {
                    bool missing = !VidToCategoryMap.ContainsKey(vidReportClass.VID);
                    if (missing)
                    {
                        if (count++ < 5)
                            error += "\nnew VID: " + vidReportClass.VidLabel;
                        else
                            return "New VIDs, review configuration";
                    }
                }

                if (error != null)
                    return "Review configuration," + error;

                // Est-ce que des VIDs ont été supprimés ?
                //......................................
                foreach (AsoDefectVidCategory cat in VidToCategoryMap.Values)
                {
                    if (!VidList.ContainsKey(cat.VID))
                        return "VIDs have changed, review configuration";
                }
            }

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        //=================================================================
        //
        //=================================================================
        private CustomExceptionDictionary<int, VidReport.ReportClass> FindAvailableVids()
        {
            CustomExceptionDictionary<int, VidReport.ReportClass> VidList = new CustomExceptionDictionary<int, VidReport.ReportClass>(exceptionKeyName: "VID");

            // Récupère la liste des modules VID
            //..................................
            List<ModuleBase> vidmodules = Module.FindAncestors(mod => mod is BasicModules.VidReport.VidReportModule);

            // Récupère la liste des VIDs
            //...........................
            foreach (VidReport.VidReportModule mod in vidmodules)
            {
                foreach (VidReport.ReportClass cat in mod.paramCategories.ReportClasses.Values)
                    VidList[cat.VID] = cat;
            }

            return VidList;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as AsoEditorParameter;
            return parameter != null &&
                   DefectClassToCategoryMap.DictionaryEqual(parameter.DefectClassToCategoryMap) &&
                   VidToCategoryMap.DictionaryEqual(parameter.VidToCategoryMap);
        }

    }

}
