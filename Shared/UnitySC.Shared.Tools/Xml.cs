using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UnitySC.Shared.Tools
{
    public static class XML
    {
        // Pour avoir "." et non "," comme signe de decimal:
        public static System.Globalization.NumberFormatInfo Nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;

        #region XML DOC Functions to manipulate XML in the form <name value="xxxx">

        /// <summary>
        ///  Remove all nodes corresponding to an xpath
        /// </summary>
        /// <param name="parentnode"></param>
        /// <param name="xpath"></param>
        public static void RemoveNodes(this XmlNode parentnode, string xpath)
        {
            var xmlnodelist = parentnode.SelectNodes(xpath);
            foreach (XmlNode xmlnode in xmlnodelist)
                xmlnode.ParentNode.RemoveChild(xmlnode);
        }

        public static void AddAttribute(this XmlNode node, string name, object value)
        {
            var xmldoc = node.OwnerDocument;
            var xmlattr = xmldoc.CreateAttribute(name);
            node.Attributes.Append(xmlattr);
            xmlattr.Value = value.ToString();
        }

        public static string GetAttributeValue(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                throw new ApplicationException("missing attribute \"" + name + "\" on field \"" + node.Name + "\"");

            return attr.Value;
        }

        /// <summary>
        /// Get a string
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetStringValue(this XmlNode node, string field, string defaultValue = null)
        {
            var subnode = node.SelectSingleNode(".//" + field);
            if (subnode == null)
            {
                if (defaultValue != null)
                    return defaultValue;
                else
                    throw new ApplicationException("XML file error, Missing field: " + field);
            }

            var valuenode = subnode.Attributes.GetNamedItem("Value");
            if (valuenode == null)
                valuenode = subnode.Attributes.GetNamedItem("value");
            if (valuenode == null)
            {
                if (defaultValue != null)
                    return defaultValue;
                else
                    throw new ApplicationException("XML file error, Missing value in field: " + field);
            }

            return valuenode.Value;
        }

        /// <summary>
        /// Get a bool
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool GetBoolValue(this XmlNode node, string field)
        {
            string str = GetStringValue(node, field);

            if (!bool.TryParse(str, out bool value))
                throw new ApplicationException("XML file error, Invalid value in field: " + field);

            return value;
        }

        /// <summary>
        /// Get an int
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static int GetIntValue(this XmlNode node, string field)
        {
            string str = GetStringValue(node, field);

            if (!int.TryParse(str, out int value))
                throw new ApplicationException("XML file error, Invalid value in field: " + field);

            return value;
        }

        /// <summary>
        /// Get an int
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetIntValue(this XmlNode node, string field, int defaultValue)
        {
            string str = GetStringValue(node, field, "");
            if (str == "")
                return defaultValue;

            if (!int.TryParse(str, out int value))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Get a double
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static double GetDoubleValue(this XmlNode node, string field)
        {
            string str = GetStringValue(node, field);

            if (!double.TryParse(str, System.Globalization.NumberStyles.Any, Nfi, out double value))
                throw new ApplicationException("XML file error, Invalid value in field " + field);

            return value;
        }

        /// <summary>
        /// Get a double
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double GetDoubleValue(this XmlNode node, string field, double defaultValue)
        {
            string str = GetStringValue(node, field, "");
            if (str == "")
                return defaultValue;

            if (!double.TryParse(str, System.Globalization.NumberStyles.Any, Nfi, out double value))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Get a float
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static float GetFloatValue(this XmlNode node, string field)
        {
            string str = GetStringValue(node, field);

            if (!float.TryParse(str, System.Globalization.NumberStyles.Any, Nfi, out float value))
                throw new ApplicationException("XML file error, Invalid value in field " + field);

            return value;
        }

        /// <summary>
        /// Get a float
        /// </summary>
        /// <param name="node"></param>
        /// <param name="field"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetFloatValue(this XmlNode node, string field, float defaultValue)
        {
            string str = GetStringValue(node, field, "");
            if (str == "")
                return defaultValue;

            if (!float.TryParse(str, System.Globalization.NumberStyles.Any, Nfi, out float value))
                return defaultValue;

            return value;
        }

        /// <summary>
        ///  Create an XML element with a "value=" attribute.
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XmlNode CreateValueElement(this XmlDocument xmldoc, string name, object value)
        {
            if (value == null)
                value = "";
            XmlNode node = xmldoc.CreateElement(name);
            node.AddAttribute("Value", value.ToString());

            return node;
        }

        public static XmlNode AppendValueElement(this XmlNode parent, string name, object value)
        {
            var node = parent.OwnerDocument.CreateValueElement(name, value);
            parent.AppendChild(node);

            return node;
        }

        #endregion XML DOC Functions to manipulate XML in the form <name value="xxxx">

        ///////////////////////////////////////////////////////////////////
        //
        // Functions to manipulate XML in the form
        // <name> value </name>
        //
        ///////////////////////////////////////////////////////////////////

        #region XML DOC Functions to manipulate XML in the form <name> value </name>

        /// <summary>
        /// Create an XML element with the value as text.
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XmlNode CreateTextElement(this XmlDocument xmldoc, string name, object value)
        {
            XmlNode node = xmldoc.CreateElement(name);
            node.InnerText = value.ToString();

            return node;
        }

        /// <summary>
        /// Create an XML element with the value as text and append it the the node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XmlNode AppendTextElement(this XmlNode node, string name, object value)
        {
            var childnode = node.OwnerDocument.CreateTextElement(name, value);
            node.AppendChild(childnode);

            return childnode;
        }

        /// <summary>
        /// Return child node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetTextElement(this XmlNode node, string name)
        {
            var textNode = node.SelectSingleNode("//" + name);
            if (textNode == null)
                throw new Exception("missing node \"" + name + "\"");

            return textNode.InnerText;
        }

        #endregion XML DOC Functions to manipulate XML in the form <name> value </name>

        ///////////////////////////////////////////////////////////////////
        //
        // Serialization
        //
        ///////////////////////////////////////////////////////////////////

        #region Sérialisation [DataContract]

        /// <summary>
        /// Sérialise un [DataContract(Namespace = "")] dans un fichier.
        /// </summary>
        public static void DatacontractSerialize(this object value, string fileName)
        {
            Stream fs = new FileStream(fileName, FileMode.Create);
            using (var writer = XmlDictionaryWriter.CreateTextWriter(fs))
            {
                var ser = new NetDataContractSerializer();
                ser.WriteObject(writer, value);
            }
        }

        /// <summary>
        /// Sérialise un [DataContract(Namespace = "")] dans une string
        /// </summary>
        public static string DatacontractSerializeToString(this object value, bool indent = false)
        {
            var serializer = new DataContractSerializer(value.GetType());
            var settings = new XmlWriterSettings()
            {
                Indent = indent,
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize
            };
            //sb.Append($"<!-- Generated on {DateTime.Now}-->\n");
            using (var stringWriter = new Utf8StringWriter())
            using (var writer = XmlWriter.Create(stringWriter, settings))
            {
                serializer.WriteObject(writer, value);
                writer.Flush();
                return stringWriter.ToString();
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
            public Utf8StringWriter() { }
        }

        /// <summary>
        /// Deserialize a Datacontract depuis un fichier
        /// </summary>
        public static T DatacontractDeserialize<T>(string fileName, bool useMaxReaderQuota = false)
        {
            T t;

            var fs = new FileStream(fileName, FileMode.Open);
            using (var reader = XmlDictionaryReader.CreateTextReader(fs, useMaxReaderQuota ? XmlDictionaryReaderQuotas.Max : new XmlDictionaryReaderQuotas()))
            {
                var ser = new NetDataContractSerializer();
                t = (T)ser.ReadObject(reader, true);
            }

            return t;
        }

        /// <summary>
        /// Deserialize a Datacontract depuis une string
        /// </summary>
        public static T DatacontractDeserializeFromString<T>(string xmldata, bool useMaxReaderQuota = false)
        {
            byte[] data = System.Text.Encoding.Unicode.GetBytes(xmldata);
            Stream ms = new MemoryStream(data);
            T t;
            using (var reader = XmlDictionaryReader.CreateTextReader(ms, useMaxReaderQuota ? XmlDictionaryReaderQuotas.Max : new XmlDictionaryReaderQuotas()))
            {
                var ser = new DataContractSerializer(typeof(T));
                t = (T)ser.ReadObject(reader, true);
            }
            return t;
        }

        #endregion Sérialisation [DataContract]

        #region Sérialisation [Serializable]

        /// <summary>
        /// Serialize a [Serializable] in a file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filename"></param>
        public static void Serialize(this object obj, string filename)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        ///  Serialize a [Serializable] in an XmlNode
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static XmlNode SerializeAndAppend(this XmlNode parent, object obj)
        {
            //-------------------------------------------------------------
            // Serialize
            //-------------------------------------------------------------
            var xmlDocument = new XmlDocument();
            using (var writer = xmlDocument.CreateNavigator().AppendChild())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
            }

            //-------------------------------------------------------------
            // Append
            //-------------------------------------------------------------
            var node = parent.OwnerDocument.ImportNode(xmlDocument.DocumentElement, true);
            node.CleanXmlns();
            node.AddAttribute("Type", obj.GetType().ToString());

            parent.AppendChild(node);
            return node;
        }

        private static void CleanXmlns(this XmlNode node)
        {
            if (node.Attributes != null)
            {
                node.Attributes.RemoveNamedItem("xmlns:xsd");
                node.Attributes.RemoveNamedItem("xmlns:xsi");
            }

            foreach (XmlNode n in node.ChildNodes)
                n.CleanXmlns();
        }

        /// <summary>
        /// Deserialize a [Serializable]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string filename)
        {
            object obj;

            var deserializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StreamReader(filename))
            {
                obj = deserializer.Deserialize(reader);
            }

            return (T)obj;
        }

        public static T Deserialize<T>(this XmlNode node)
        {
            object obj;

            var deserializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(node.InnerText))
            {
                obj = deserializer.Deserialize(reader);
            }

            return (T)obj;
        }

        public static object DeserializeAs(this XmlNode node, Type type)
        {
            object obj;

            var deserializer = new XmlSerializer(type);
            using (TextReader reader = new StringReader(node.OuterXml))
            {
                obj = deserializer.Deserialize(reader);
            }

            return obj;
        }

        public static object DeserializeAsSavedType(this XmlNode node)
        {
            string className = node.Attributes["Type"].Value;
            var type = DynamicType.GetType(className);
            if (type == null)
                throw new ApplicationException("Type not found: " + className);

            if (!type.HasAttribute(new SerializableAttribute()))
            {
                throw new Exception("Invalid type: " + type + " is not [Serializable]");
            }

            object obj = node.DeserializeAs(type);
            return obj;
        }

        public static string SerializeToString<T>(T obj)
        {
            using (var outStream = new StringWriter())
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(outStream, obj);
                return outStream.ToString();
            }
        }

        public static T DeserializeFromString<T>(string serialized)
        {
            using (var inStream = new StringReader(serialized))
            {
                var ser = new XmlSerializer(typeof(T));
                return (T)ser.Deserialize(inStream);
            }
        }

        public static T DeserializeDataContract<T>(string fileName, bool useMaxReaderQuota = false)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, useMaxReaderQuota ? XmlDictionaryReaderQuotas.Max : new XmlDictionaryReaderQuotas());
            try
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                // Deserialize the data and read it from the instance.
                return (T)ser.ReadObject(reader, true);
            }
            finally
            {
                reader.Close();
                fs.Close();
            }
        }
        public static void SerializeDataContract<T>(string fileName, T obj)
        {
            FileStream writer = new FileStream(fileName, FileMode.Create);
            DataContractSerializer ser = new DataContractSerializer(typeof(T));
            try
            {
                ser.WriteObject(writer, obj);
            }
            finally
            {
                writer.Close();
            }
        }


        #endregion Sérialisation [Serializable]
    }
}
