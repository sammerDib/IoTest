using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Interface
{
    public class DeadPixelsManager
    {
        private List<DeadPixel> _blackDeadPixels;
        private List<DeadPixel> _whiteDeadPixels;

        [XmlIgnore]
        protected String _sDPMLFilePath;
        /// <summary>
        /// Contains all the white dead pixels
        /// </summary>
        public List<DeadPixel> WhiteDeadPixels
        {
            get { return _whiteDeadPixels; }
            set { _whiteDeadPixels = value; }
        }
        /// <summary>
        /// Contains all the black dead pixels
        /// </summary>
        public List<DeadPixel> BlackDeadPixels
        {
            get { return _blackDeadPixels; }
            set { _blackDeadPixels = value; }
        }

        /// <summary>
        /// Saves the current instance into an XML file
        /// </summary>
        /// <param name="DPMLFilePath">XML file path to save the instance to</param>
        /// <returns>True if the operation succeeds, false otherwise</returns>
        public bool WriteBackXMLConfig(String DPMLFilePath)
        {
            this.Serialize(DPMLFilePath);
            return true;
        }
        /// <summary>
        /// Saves the current instance back to the XML file it comes from
        /// </summary>
        /// <returns>True if the operation succeeds, false otherwise</returns>
        public bool WriteBackXMLConfig()
        {
            return WriteBackXMLConfig(_sDPMLFilePath);
        }

        /// <summary>
        /// Restores an instance from an XML File
        /// </summary>
        /// <param name="DPMLFilePath">The file path to use</param>
        /// <returns>The instance read from the XML file</returns>
        public static DeadPixelsManager ReadFromDPMLFile(String DPMLFilePath)
        {
            if (string.IsNullOrEmpty(DPMLFilePath))
                return null;

            // Crée l'objet de sérialisation XML
            var m_InitConfig = new DeadPixelsManager();
            XmlSerializer serializer = new XmlSerializer(m_InitConfig.GetType());
            // Ouvre le fichier  
            StreamReader reader;
            reader = new StreamReader(DPMLFilePath);
            try
            {
                // Déserialise les options

                m_InitConfig = (DeadPixelsManager)serializer.Deserialize(reader);
                m_InitConfig._sDPMLFilePath = DPMLFilePath;
                //options.ValidateAppOptions();
            }
            catch (Exception Ex)
            {
                throw new Exception("Bad DPML File : " + Ex.Message + " - " + Ex.InnerException?.Message);
                // Le fichier n'existe pas ou est invalide, pour le moment levée d'une exception 
            }
            finally
            {
                reader.Close();
            }
            return m_InitConfig;
        }
    }
}

