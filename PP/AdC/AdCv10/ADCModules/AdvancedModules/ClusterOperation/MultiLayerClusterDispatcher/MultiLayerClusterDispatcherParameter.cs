using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using BasicModules;

namespace AdvancedModules.MultiLayerClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // Parameter
    ///////////////////////////////////////////////////////////////////////
    public class MultiLayerClusterDispatcherParameter : ClassificationParameterBase
    {
        public List<DispatcherDefectClass> DefectClassList { get; private set; }

        //=================================================================
        // Constructor
        //=================================================================
        public MultiLayerClusterDispatcherParameter(MultiLayerClusterDispatcherModule module, string name)
            : base(module, name)
        {
            DefectClassList = new List<DispatcherDefectClass>();
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
            XmlNode defectClassNode = ReadParameter(Name, parameterNodes);
            foreach (XmlNode node in defectClassNode.ChildNodes)
            {
                DispatcherDefectClass defectClass = Serializable.LoadFromXml<DispatcherDefectClass>(node);
                DefectClassList.Add(defectClass);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement elem = SaveParameter(xmlNode, Name, this);

            foreach (DispatcherDefectClass defectClass in DefectClassList)
                defectClass.SerializeAsChildOf(elem);

            return null;
        }

        //=================================================================
        //
        //=================================================================
        private MultiLayerClusterDispatcherViewModel _parameterViewModel;
        private MultiLayerClusterDispatcherControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new MultiLayerClusterDispatcherViewModel(this);
                    _parameterUI = new MultiLayerClusterDispatcherControl(_parameterViewModel);
                }
                return _parameterUI;
            }
        }

        //=================================================================
        //
        //=================================================================
        public override string Validate()
        {
            for (int branchIdx = 0; branchIdx < Module.Children.Count; branchIdx++)
            {
                bool found = false;
                foreach (DispatcherDefectClass defectClass in DefectClassList)
                {
                    if (defectClass.BranchIndex == branchIdx)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return "No defect for branch " + branchIdx + " (" + Module.Children[branchIdx].Name + ")";
            }

            return null;
        }

        //=================================================================
        // Récupération des infos des autres modules
        //=================================================================
        public void Synchronize()
        {
            //-------------------------------------------------------------
            // Récuprération de la liste des défauts / branches
            //-------------------------------------------------------------

            // Liste des classes de défaut de la classif multi layer
            //......................................................
            ModuleBase ancestor = Module.FindAncestors(m => m is ClassificationMultiLayer.ClassificationMultiLayerModule).FirstOrDefault();
            ClassificationMultiLayer.ClassificationMultiLayerModule classifModule = (ClassificationMultiLayer.ClassificationMultiLayerModule)ancestor;
            if (classifModule == null)
                return;
            ClassificationMultiLayer.ClassificationMultiLayerParameter classifParameter = classifModule.paramClassification;

            // Listes des layers du CMC
            //.........................
            ModuleBase cmcModule = Module.FindAncestors(m => m is CmcNamespace.CmcModule).FirstOrDefault();
            if (cmcModule == null)
                return;

            Dictionary<int, string> layerMap = new Dictionary<int, string>();
            for (int i = 0; i < cmcModule.Parents.Count; i++)
            {
                ModuleBase parent = cmcModule.Parents[i];
                IDataLoader dataloder = (IDataLoader)(parent.FindAncestors(mod => mod is IDataLoader).FirstOrDefault());
                if (dataloder != null)
                    layerMap.Add(i, dataloder.LayerName);
            }

            //-------------------------------------------------------------
            // Supression des DispatcherDefectClass inutiles
            //-------------------------------------------------------------
            for (int i = DefectClassList.Count - 1; i >= 0; i--)
            {
                DispatcherDefectClass dispatcherDefectClass = DefectClassList[i];
                bool ok = false;
                var classifDefectClass = classifParameter.DefectClassList.FirstOrDefault(c => c.DefectLabel == dispatcherDefectClass.DefectLabel);
                if (classifDefectClass != null)
                {
                    int layerIndex = layerMap.FirstOrDefault(x => x.Value == dispatcherDefectClass.DefectLayer).Key;
                    if (classifDefectClass.MeasuredBranch < 0)
                    {   // Sélection automatique de la branche à mesurer
                        var test = classifDefectClass.DefectBranchList[layerIndex];
                        ok = (test == ClassificationMultiLayer.DefectTestType.DefectMustBePresent);
                    }
                    else
                    {   // Sélection manuelle de la branche à mesurer
                        ok = (classifDefectClass.MeasuredBranch == layerIndex);
                    }
                }

                if (!ok)
                    DefectClassList.RemoveAt(i);
            }

            //-------------------------------------------------------------
            // Ajout des nouvelles class/branches
            //-------------------------------------------------------------
            foreach (var classifDefectClass in classifParameter.DefectClassList)
            {
                if (classifDefectClass.MeasuredBranch < 0)
                {   // Sélection automatique de la branche à mesurer
                    for (int i = 0; i < cmcModule.Parents.Count; i++)
                    {
                        if (classifDefectClass.DefectBranchList[i] == ClassificationMultiLayer.DefectTestType.DefectMustBePresent)
                        {
                            if (layerMap.ContainsKey(i))
                            {
                                string layer = layerMap[i];
                                SynchronizeDefectClass(classifDefectClass.DefectLabel, layer);
                            }
                        }
                    }
                }
                else
                {
                    if (layerMap.ContainsKey(classifDefectClass.MeasuredBranch))
                    {
                        // Sélection manuelle de la branche à mesurer
                        string layer = layerMap[classifDefectClass.MeasuredBranch];
                        SynchronizeDefectClass(classifDefectClass.DefectLabel, layer);
                    }
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void SynchronizeDefectClass(string defectLabel, string layer)
        {
            DispatcherDefectClass dispatcherDefectClass = DefectClassList.Find(x => x.DefectLabel == defectLabel && x.DefectLayer == layer);
            if (dispatcherDefectClass == null)
            {
                dispatcherDefectClass = new DispatcherDefectClass();
                dispatcherDefectClass.DefectLabel = defectLabel;
                dispatcherDefectClass.DefectLayer = layer;

                DefectClassList.Add(dispatcherDefectClass);
            }
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as MultiLayerClusterDispatcherParameter;
            return parameter != null &&
                   DefectClassList.ValuesEqual(parameter.DefectClassList);
        }

    }
}
