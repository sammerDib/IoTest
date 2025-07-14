using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UnitySC.ADCAS300Like.Common
{
    public class XmlSerializableObject<T> where T:class
    {
        public static T ReadFromFile(String xmlFilePath)
        {
            StreamReader Reader = new StreamReader(xmlFilePath);
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
        public virtual bool SaveToFile(String xmlFilePath)
        {
            StreamWriter Writer = new StreamWriter(xmlFilePath);
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
