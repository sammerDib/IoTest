using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ADCEngine;
using AdcTools;
using AdcBasicObjects;

namespace BasicModules.DeepControl
{
	///////////////////////////////////////////////////////////////////////
	// Classe de défauts
	///////////////////////////////////////////////////////////////////////
	public class DefectClass : IValueComparer
	{
		public string label;
		public List<ComparatorBase> compartorList = new List<ComparatorBase>();

        public bool HasSameValue(object obj)
        {
            var @class = obj as DefectClass;
            return @class != null &&
                   label == @class.label &&
                    compartorList.ValuesEqual(@class.compartorList);
        }

        //=================================================================
        // 
        //=================================================================
        public void Load(XmlNode defectClassNode)
		{
			label = defectClassNode.Attributes["Value"].Value;
			XmlNodeList nodes = defectClassNode.ChildNodes;
			foreach (XmlNode node in nodes)
			{
				ComparatorBase cmp = Serializable.LoadFromXml<ComparatorBase>(node);
				compartorList.Add(cmp);
			}
		}

		//=================================================================
		// 
		//=================================================================
		public void Save(XmlNode defectClassNode)
		{
			XmlNode node = ParameterBase.SaveParameter(defectClassNode, "DefectClass", label);

			foreach (ComparatorBase cmp in compartorList)
				cmp.SerializeAsChildOf(node);
		}

		//=================================================================
		// 
		//=================================================================
		public override string ToString()
		{
			return label;
		}
	}

}
