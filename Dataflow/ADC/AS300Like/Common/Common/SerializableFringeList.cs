using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace E95GUIFrameWork.InfoPanel
{
    /// <summary>
    /// Holds the available fringes list on the PM side, to be read by the Robot software
    /// </summary>
    public class SerializableFringeList
    {
        /// <summary>
        /// The Fringe name list
        /// </summary>
        public List<String> FringeList;

        /// <summary>
        /// Constructor. Initializes the fringe list object
        /// </summary>
        public SerializableFringeList()
        {
            FringeList = new List<string>();
        }

        /// <summary>
        /// Serializes the current instance into an XML file
        /// </summary>
        /// <param name="XmlOutputFilePath">The XML file path</param>
        /// <returns></returns>
        public bool Serialize(String XmlOutputFilePath)
        {
            XmlSerializer Serializer= new XmlSerializer(typeof(SerializableFringeList));
            
            StreamWriter SW = new StreamWriter(XmlOutputFilePath);
            try
            {
                Serializer.Serialize(SW, this);
                
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                SW.Close();
            }
            return true;
        }

        /// <summary>
        /// Deserializes an XML file representation of a SerializableFringeList instance
        /// </summary>
        /// <param name="XmlInputFilePath"></param>
        /// <returns></returns>
        public static SerializableFringeList Deserialize(String XmlInputFilePath)
        {
            SerializableFringeList FringeList = new SerializableFringeList();

            XmlSerializer Serializer = new XmlSerializer(typeof(SerializableFringeList));
            StreamReader SR = new StreamReader(XmlInputFilePath);
            try
            {
                FringeList = (SerializableFringeList) Serializer.Deserialize(SR);

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                SR.Close();
            }
            return FringeList; ;
        }
    }
}
