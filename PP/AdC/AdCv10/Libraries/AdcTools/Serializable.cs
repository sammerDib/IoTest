using System;
using System.Xml;

using UnitySC.Shared.Tools;

namespace AdcTools
{
    [Serializable]
    public class Serializable
    {
        //=================================================================
        // Création à partir du XML
        //=================================================================
        public static T LoadFromXml<T>(XmlNode node) where T : Serializable
        {
            string className = node.Attributes["Type"].Value;
            Type type = DynamicType.GetType(className);
            if (type == null)
                throw new ApplicationException("Type not found: " + className);

            bool ok = (type == typeof(T));
            ok = ok || type.IsSubclassOf(typeof(T));
            if (!ok)
                throw new Exception("Invalid type: " + type + " Expecting a subclass of: " + typeof(T));

            T obj = (T)(node.DeserializeAs(type));
            obj.OnLoaded(node);
            return obj;
        }

        //=================================================================
        // Fonction à redéfinir pour des traitements spéciaux.
        //=================================================================
        protected virtual Serializable OnLoaded(XmlNode node)
        {
            return this;
        }

        //=================================================================
        // Sauvegarde XML
        //=================================================================
        public virtual XmlNode SerializeAsChildOf(XmlNode parent)
        {
            XmlNode node = parent.SerializeAndAppend(this);
            return node;
        }

    }
}
