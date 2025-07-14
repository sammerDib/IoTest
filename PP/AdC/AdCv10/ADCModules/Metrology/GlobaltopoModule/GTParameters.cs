using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using GlobaltopoModule.View;
using GlobaltopoModule.ViewModel;

namespace GlobaltopoModule
{
    public class GTParameters : ParameterBase
    {
        public GTInputPrmClass InputPrmClass { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public GTParameters(GTMeasurementModule module, string name) :
            base(module, name)
        {
            InputPrmClass = new GTInputPrmClass();
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("InPrm", xmlNodes);
            XmlNode childnode = node.FirstChild;
            InputPrmClass = Serializable.LoadFromXml<GTInputPrmClass>(childnode);
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "InPrm", this);
            InputPrmClass.SerializeAsChildOf(node);
            return null;
        }

        // =================================================================
        //  IHM
        // =================================================================
        private GTInputPrmViewModel _parameterViewModel;
        private GTInputPrmControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new GTInputPrmViewModel(this);
                    _parameterUI = new GTInputPrmControl();
                    _parameterUI.DataContext = _parameterViewModel;
                }
                return _parameterUI;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override bool HasSameValue(object obj)
        {
            return InputPrmClass.HasSameValue(obj);
        }

    }
}
