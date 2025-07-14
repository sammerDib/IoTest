using ADCEngine;
using AdcTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Xml;
using GlobaltopoModule.View;
using GlobaltopoModule.ViewModel;

namespace GlobaltopoModule
{
    public class GTVidBowWarpParameter : ParameterBase
    {
        public CustomExceptionDictionary<string, GTVidBowWarpPrmClass> BowWarpReportClasses { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public GTVidBowWarpParameter(GTVidBowWarpReportModule module, string name) :
            base(module, name)
        {
            BowWarpReportClasses = new CustomExceptionDictionary<string, GTVidBowWarpPrmClass>(exceptionKeyName: "Label");
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("Class", xmlNodes);
            foreach (XmlNode childnode in node)
            {
                GTVidBowWarpPrmClass category = Serializable.LoadFromXml<GTVidBowWarpPrmClass>(childnode);
                BowWarpReportClasses.Add(category.Label, category);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "Class", this);

            foreach (GTVidBowWarpPrmClass measurereport in BowWarpReportClasses.Values)
                measurereport.SerializeAsChildOf(node);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private VidBowWarpReportViewModel _parameterViewModel;
        private VidBowWarpControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new VidBowWarpReportViewModel(this);
                    _parameterUI = new VidBowWarpControl(_parameterViewModel);
                }
                return _parameterUI;
            }
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as GTVidBowWarpParameter;
            return parameter != null &&
                   BowWarpReportClasses.DictionaryEqual(parameter.BowWarpReportClasses);
        }
    }
}
