using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdvancedModules.Edition.Apc.View;
using AdvancedModules.Edition.Apc.ViewModel;

using UnitySC.Shared.Tools;

namespace AdvancedModules.Edition.Apc
{
    public class ApcParameter : ParameterBase
    {
        public int VidNumber { get; set; } = -1;
        public string VidLabel { get; set; } = "";

        //=================================================================
        // Constructeur
        //=================================================================
        public ApcParameter(ApcModule module, string name) :
            base(module, name)
        {
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("VID", xmlNodes);
            VidLabel = node.Attributes["VidLabel"].Value;
            VidNumber = int.Parse(node.Attributes["Value"].Value);
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "VID", VidNumber);
            node.AddAttribute("VidLabel", VidLabel);

            return node;
        }

        //=================================================================
        // IHM
        //=================================================================
        private ApcParameterViewModel _parameterViewModel;
        private ApcParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new ApcParameterViewModel(this);
                    _parameterUI = new ApcParameterView(_parameterViewModel);
                }
                return _parameterUI;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string err = base.Validate();
            if (err != null)
                return err;

            if (VidNumber < 0)
                return "Invalid VID number";

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool HasSameValue(object obj)
        {
            var parameter = obj as ApcParameter;
            return parameter != null &&
                   parameter.VidNumber == VidNumber &&
                   parameter.VidLabel == VidLabel;
        }

    }
}
