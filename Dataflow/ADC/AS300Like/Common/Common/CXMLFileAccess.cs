using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Common
{
    public static class CXMLFileAccess<T> where T : new()  
    {        
       public static void WriteParameters(string filename, T objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            XmlSerializer bFormatter = new XmlSerializer(objectToSerialize.GetType());
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

       public static T ReadParameters(string filename)
        {
            T objectToDeserialize = new T();
            CheckConfigurationFileAndCreateIfNeeded(filename, objectToDeserialize);
            Stream stream = File.Open(filename, FileMode.Open);
            XmlSerializer bFormatter = new XmlSerializer(objectToDeserialize.GetType());
            objectToDeserialize = (T)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToDeserialize;
        }
       

       public static bool CheckConfigurationFileAndCreateIfNeeded(string filename, T ConfiguredObject) 
       {
           try
           {
               if (!File.Exists(filename))
               {
                   ConfiguredObject = new T();
                   WriteParameters(filename, ConfiguredObject);
               }
               return true;
           }
           catch
           {
               return false;
           }
       }
    }
}
