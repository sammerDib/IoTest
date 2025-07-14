using System;
using System.Drawing;
using System.Linq;
using System.Xml;

using AcquisitionAdcExchange;

using AdcTools;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // Classe de base des wafers
    ///////////////////////////////////////////////////////////////////////
    public abstract class WaferBase
    {
        public CustomExceptionDictionary<eWaferInfo, string> waferInfo = new CustomExceptionDictionary<eWaferInfo, string>();
        public string Basename { get { return waferInfo[eWaferInfo.Basename]; } }

        //=================================================================
        // 
        //=================================================================
        public string GetWaferInfo(eWaferInfo info)
        {
            if (waferInfo.ContainsKey(info))
                return waferInfo[info];
            else
                return String.Empty;
        }

        public abstract bool IsPointInside(PointF p, double margin = 0);
        public abstract eCompare IsRectInside(RectangleF r, double margin = 0);
        public abstract eCompare IsQuadInside(QuadF q, double margin = 0);

        /// <summary> En µm </summary>
		public abstract RectangleF SurroundingRectangle { get; }
        public abstract RectangleF SurroundingRectangleWithFlats { get; }

        //=================================================================
        // Load/Save
        //=================================================================
        public abstract void Init(XmlNode node);

        //=================================================================
        // Creation d'un wafer à partir du XML
        //=================================================================
        public static WaferBase LoadWafer(XmlNode node)
        {
            WaferBase wafer = (WaferBase)DynamicType.LoadXmlObject(node);

            var enumvalues = Enum.GetValues(typeof(eWaferInfo)).Cast<eWaferInfo>();
            foreach (eWaferInfo e in enumvalues)
            {
                string info = node.GetStringValue(e.ToString(), "");
                wafer.waferInfo.Add(e, info);
            }

            wafer.Init(node);

            return wafer;
        }

        //=================================================================
        // 
        //=================================================================
        public virtual XmlNode Save(XmlDocument xmldoc)
        {
            XmlElement waferNode = xmldoc.CreateElement("Wafer");
            waferNode.SetAttribute("Type", GetType().ToString());

            foreach (var kvp in waferInfo)
                waferNode.AppendValueElement(kvp.Key.ToString(), kvp.Value);

            return waferNode;
        }
    }
}
