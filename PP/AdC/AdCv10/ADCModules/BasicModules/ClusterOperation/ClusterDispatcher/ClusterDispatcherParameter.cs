using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

namespace BasicModules.ClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // Parameter
    ///////////////////////////////////////////////////////////////////////
    public class ClusterDispatcherParameter : ClassificationParameterBase
    {
        public List<DispatcherDefectClass> DefectClassList { get; private set; }

        //=================================================================
        // Constructor
        //=================================================================
        public ClusterDispatcherParameter(ClusterDispatcherModule module, string name)
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
        private ClusterDispatcherViewModel _parameterViewModel;
        private ClusterDispatcherControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new ClusterDispatcherViewModel(this);
                    _parameterUI = new ClusterDispatcherControl(_parameterViewModel);
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

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as ClusterDispatcherParameter;
            return parameter != null &&
                   DefectClassList.ValuesEqual(parameter.DefectClassList);
        }

    }
}
