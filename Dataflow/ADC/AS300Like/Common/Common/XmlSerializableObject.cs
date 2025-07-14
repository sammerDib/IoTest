using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    public class XmlSerializableObject<T> where T:class
    {
        public static T ReadFromFile(String XmlFilePath)
        {
            StreamReader Reader = new StreamReader(XmlFilePath);
            try
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                T Conf = Serializer.Deserialize(Reader) as T;
                
                return Conf;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                Reader.Close();
            }
        }
        public virtual bool SaveToFile(String XmlFilePath)
        {
            StreamWriter Writer = new StreamWriter(XmlFilePath);
            try
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                Serializer.Serialize(Writer, this);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Writer.Close();
            }
        }
    }
}
