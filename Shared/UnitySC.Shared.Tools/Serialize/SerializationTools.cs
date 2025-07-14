using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace UnitySC.Shared.Tools.Serialize
{
    public class SerializationTools
    {
        public static void Serialize_DataContractObject(String filePathName, Object obj)
        {
            DataContractSerializer dcSerializer;
            StreamWriter fs = null;
            using (var ms = new MemoryStream())
            {
                try
                {
                    fs = new StreamWriter(filePathName);
                    string xml = string.Empty;
                    dcSerializer = new DataContractSerializer(obj.GetType());
                    using (var xmlTextWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        xmlTextWriter.Formatting = Formatting.Indented;
                        dcSerializer.WriteObject(xmlTextWriter, obj);
                        xmlTextWriter.Flush();
                        var item = (MemoryStream)xmlTextWriter.BaseStream;
                        item.Flush();
                        xml = new UTF8Encoding().GetString(item.ToArray());
                        fs.Write(xml);
                    }
                }
                finally
                {
                    fs.Close();
                }
            }
        }
    }
}
