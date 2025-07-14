using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdvancedModules.Edition.VID.View;
using AdvancedModules.Edition.VID.ViewModel;

using UnitySC.Shared.Tools;

namespace AdvancedModules.Edition.VID.GTVID
{
    public class GTParameter : ParameterBase
    {
        public int VidNumber { get; set; } = -1;
        public string VidLabel { get; set; } = "";



        //=================================================================
        // Constructeur
        //=================================================================
        public GTParameter(GTVIDModule module, string name) :
            base(module, name)
        {
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter(Name, xmlNodes);
            VidLabel = node.Attributes["VidLabel"].Value;
            VidNumber = int.Parse(node.Attributes["Value"].Value);
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, Name, VidNumber);
            node.AddAttribute("VidLabel", VidLabel);

            return node;
        }

        //=================================================================
        // IHM
        //=================================================================
        private GTParameterViewModel _parameterViewModel;
        private GTParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new GTParameterViewModel(this);
                    _parameterUI = new GTParameterView(_parameterViewModel);
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
            var parameter = obj as GTParameter;
            return parameter != null &&
                   parameter.VidNumber == VidNumber &&
                   parameter.VidLabel == VidLabel;
        }

    }
}
