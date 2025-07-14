using System;
using System.IO;
using System.Xml.Serialization;

namespace UnitySC.Shared.Data.ColorMap
{
    [Serializable]
    public class ColorMapSettings
    {
        public string ColorMapName { get; set; }

        public static bool SaveToXml(ColorMapSettings currentSettings, string filePath)
        {
            var serializer = new XmlSerializer(typeof(ColorMapSettings));

            try
            {
                using (var strWriter = new StreamWriter(filePath))
                {
                    serializer.Serialize(strWriter, currentSettings);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ColorMapSettings ReadFromXml(string filePath)
        {
            ColorMapSettings colorMapSettings;
            var sr = new XmlSerializer(typeof(ColorMapSettings));

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    colorMapSettings = (ColorMapSettings)sr.Deserialize(fs);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return colorMapSettings;
        }
    }
}
