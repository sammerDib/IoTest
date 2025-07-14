using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ADCControls
{
    internal class ToolsClass_Utilities
    {
        //---------------------------------------------------
        public static String Serialize<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, obj);


            //bf.Serialize(ms, obj);
            byte[] buff = ms.ToArray();
            return ASCIIEncoding.ASCII.GetString(buff);
        }
        //---------------------------------------------------
        public static T DeSerialize<T>(String Response)
        {
            byte[] buff = ASCIIEncoding.ASCII.GetBytes(Response);
            T result;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(buff);
            result = (T)serializer.Deserialize(ms);
            return result;
        }
    }
}
