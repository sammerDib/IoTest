using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.Tools;

namespace AdvancedModules.CmcNamespace
{
    public class CmcParameter : ParameterBase
    {
        //=================================================================
        // Constructor
        //=================================================================
        public CmcParameter(CmcModule module, string name)
            : base(module, name)
        {
        }

        public List<bool> IntraClusterization { get; set; } = new List<bool>();

        //=================================================================
        // Lecture du XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            IntraClusterization.Clear();

            foreach (XmlNode node in xmlNodes)
            {
                if (node.Attributes["Label"].Value == "IntraClusterization")
                {
                    int branch = Int32.Parse(node.GetAttributeValue("Branch"));
                    bool enabled = Boolean.Parse(node.GetAttributeValue("Value"));

                    IntraClusterization.AddAndResize(branch, enabled);
                }
            }
        }

        //=================================================================
        //
        //=================================================================
        public override XmlElement Save(XmlNode xmlNode)
        {
            for (int i = 0; i < IntraClusterization.Count; i++)
            {
                XmlElement node = SaveParameter(xmlNode, "IntraClusterization", IntraClusterization[i]);
                node.SetAttribute("Branch", i.ToString());
            }
            return null;
        }

        //=================================================================
        //
        //=================================================================
        public override string Validate()
        {
            if (IntraClusterization.Count() != Module.Parents.Count)
                return "Graph has changed, review the parameters";

            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as CmcParameter;
            return parameter != null &&
                   IntraClusterization.SequenceEqual(parameter.IntraClusterization);
        }

        //=================================================================
        //
        //=================================================================
        private UserControl _parameterUI;
        private CmcViewModel _parameterViewModel;

        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new CmcViewModel(this);
                    _parameterUI = new CmcLayersControl(_parameterViewModel);
                }

                return _parameterUI;
            }
        }

    }
}
