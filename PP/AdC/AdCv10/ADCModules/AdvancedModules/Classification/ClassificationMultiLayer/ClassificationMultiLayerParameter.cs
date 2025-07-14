using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using BasicModules;

namespace AdvancedModules.ClassificationMultiLayer
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ClassificationMultiLayerParameter : ClassificationParameterBase
    {
        private List<MultiLayerDefectClass> _defectClassList = new List<MultiLayerDefectClass>();
        public List<MultiLayerDefectClass> DefectClassList
        {
            get { return _defectClassList; }
            set { _defectClassList = value; }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public ClassificationMultiLayerParameter(ClassificationMultiLayerModule module, string name)
            : base(module, name)
        {
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
            XmlNode defectClassNode = ReadParameter(Name, parameterNodes);
            foreach (XmlNode node in defectClassNode.ChildNodes)
            {
                MultiLayerDefectClass defectClass = Serializable.LoadFromXml<MultiLayerDefectClass>(node);
                _defectClassList.Add(defectClass);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement elem = SaveParameter(xmlNode, Name, this);

            foreach (MultiLayerDefectClass defectClass in _defectClassList)
                defectClass.SerializeAsChildOf(elem);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private ClassificationMultiLayerControl _parameterUI;
        private ClassificationMultiLayerViewModel _parameterViewModel;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new ClassificationMultiLayerViewModel(this);
                    _parameterUI = new ClassificationMultiLayerControl(_parameterViewModel);
                }

                return _parameterUI;
            }
        }

        ///=================================================================<summary>
        /// Vérifie que toutes les classes de défault sont bien gérées par
        /// l'object Parameter.
        ///</summary>=================================================================
        public bool SynchronizeDefectClassList()
        {
            bool changed = false;

            //-------------------------------------------------------------
            // Récupération du module CMC
            //-------------------------------------------------------------
            ModuleBase cmc = Module.FindAncestors(mod => mod is CmcNamespace.CmcModule).FirstOrDefault();
            if (cmc == null)
                return false;

            //-------------------------------------------------------------
            // Récupération des Classifiers et des Layers
            //-------------------------------------------------------------
            List<ModuleBase> classifiers = new List<ModuleBase>();
            List<List<ModuleBase>> classifiersByBranch = new List<List<ModuleBase>>();
            foreach (ModuleBase parent in cmc.Parents)
            {
                List<ModuleBase> list = parent.FindAncestors(mod => mod is IClassifierModule);
                if (parent is IClassifierModule)
                    list.Add(parent);
                classifiersByBranch.Add(list);
                classifiers.UnionWith(list);
            }
            if (classifiers.Count == 0)
                return false;

            //-------------------------------------------------------------
            // Suppression des classes inutiles
            //-------------------------------------------------------------
            HashSet<string> availableDefectLabelList = FindAvailableDefectLabels();

            for (int i = DefectClassList.Count - 1; i >= 0; i--)
            {
                MultiLayerDefectClass defectClass = DefectClassList[i];
                bool needed = availableDefectLabelList.Any(l => l == defectClass.DefectLabel);
                if (!needed)
                {
                    DefectClassList.RemoveAt(i);
                    changed = true;
                }
            }

            //-------------------------------------------------------------
            // Création des classes manquantes
            //-------------------------------------------------------------
            foreach (string label in availableDefectLabelList)
            {
                MultiLayerDefectClass mldclass = DefectClassList.Find(x => x.DefectLabel == label);
                if (mldclass != null)
                {
                    // On vérifie qu'on a le bon nombre de branches
                    //.............................................
                    mldclass.DefectBranchList.Resize(classifiersByBranch.Count);
                    if (mldclass.DefectBranchList.Count != classifiersByBranch.Count)
                        changed = true;

                    // Vérifie que les branches sont bien activées/desactivées
                    //........................................................
                    for (int branch = 0; branch < classifiersByBranch.Count; branch++)
                    {
                        DefectTestType test = IsClassified(mldclass.DefectLabel, classifiersByBranch[branch]);
                        if (test == DefectTestType.DefectClassNotUsed)
                            mldclass.DefectBranchList[branch] = test;
                        else if (mldclass.DefectBranchList[branch] == DefectTestType.DefectClassNotUsed)
                            mldclass.DefectBranchList[branch] = IsClassified(mldclass.DefectLabel, classifiersByBranch[branch]);
                    }
                }
                else
                {
                    // Crée une classe de défaut multicouche
                    //......................................
                    mldclass = new MultiLayerDefectClass();
                    mldclass.DefectLabel = label;
                    DefectClassList.Add(mldclass);

                    changed = true;

                    // On cherche le défaut dans toutes les branches où il est classé
                    //...............................................................
                    mldclass.DefectBranchList.Resize(classifiersByBranch.Count);
                    for (int branch = 0; branch < classifiersByBranch.Count; branch++)
                        mldclass.DefectBranchList[branch] = IsClassified(mldclass.DefectLabel, classifiersByBranch[branch]);
                }
            }

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
            // Récupération du module CMC
            //-------------------------------------------------------------
            ModuleBase cmc = Module.FindAncestors(mod => mod is CmcNamespace.CmcModule).FirstOrDefault();
            if (cmc == null)
                return "Can't find module: Multi Layer Cluster Creation";

            //-------------------------------------------------------------
            // Récupération des Classifiers
            //-------------------------------------------------------------
            List<ModuleBase> classifiers = new List<ModuleBase>();
            List<List<ModuleBase>> classifiersByBranch = new List<List<ModuleBase>>();
            for (int i = 0; i < cmc.Parents.Count; i++)
            {
                ModuleBase parent = cmc.Parents[i];

                List<ModuleBase> list = parent.FindAncestors(mod => mod is IClassifierModule);
                if (parent is IClassifierModule)
                    list.Add(parent);
                classifiersByBranch.Add(list);
                classifiers.UnionWith(list);

                if (list.Count == 0)
                    return "Can't find Layer for branch: " + i;
            }

            //-------------------------------------------------------------
            // TODO: Est-ce qu'on a changé/permuté les branches ?
            //-------------------------------------------------------------

            //-------------------------------------------------------------
            // Est-ce que toutes les classes de défaut sont gérées ?
            //-------------------------------------------------------------
            HashSet<string> availableDefectLabelList = FindAvailableDefectLabels();

            string error = null;
            int errcount = 0;
            foreach (string label in availableDefectLabelList)
            {
                MultiLayerDefectClass mldclass = DefectClassList.Find(x => x.DefectLabel == label);
                if (mldclass == null)
                    return "New defect classes, review the classification";
                if (mldclass.DefectBranchList.Count != cmc.Parents.Count)
                    return "Branches have changed, review the classification";

                // Vérifie que les branches sont bien activées/desactivées
                //........................................................
                for (int branch = 0; branch < cmc.Parents.Count; branch++)
                {
                    DefectTestType test = IsClassified(mldclass.DefectLabel, classifiersByBranch[branch]);
                    bool ok = (test == DefectTestType.DefectClassNotUsed) == (mldclass.DefectBranchList[branch] == DefectTestType.DefectClassNotUsed);
                    if (!ok)
                    {
                        if (errcount++ > 5)
                        {
                            return "Defect classes have changed, review the classification";
                        }
                        else
                        {
                            if (errcount > 1)
                                error += Environment.NewLine;
                            error += "Defect class has changed, review the classification: " + mldclass.DefectLabel;
                            break;
                        }
                    }
                }
            }
            if (error != null)
                return error;

            //-------------------------------------------------------------
            // Est-ce qu'il y a des classes inutiles ?
            //-------------------------------------------------------------
            if (DefectClassList.Count != availableDefectLabelList.Count)
                return "Defect classes have changed, review the classification";

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        ///=================================================================
        ///<summary>
        /// Détermine le type de test à faire pour un défaut
        ///</summary>
        ///=================================================================
        private DefectTestType IsClassified(string defectLabel, List<ModuleBase> classifiers)
        {
            foreach (ModuleBase classifier in classifiers)
            {
                if (((IClassifierModule)classifier).DefectClassLabelList.Contains(defectLabel))
                    return DefectTestType.DefectMustBePresent;
            }

            return DefectTestType.DefectClassNotUsed;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as ClassificationMultiLayerParameter;
            return parameter != null &&
                   DefectClassList.ValuesEqual(parameter.DefectClassList);
        }

    }
}
