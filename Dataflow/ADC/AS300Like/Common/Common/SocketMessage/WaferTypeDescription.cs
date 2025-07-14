using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Communication
{
    /// <summary>
    /// Lists the sub types linked to a wafer type.
    /// </summary>
    public class WaferTypeDescription
    {
        /// <summary>
        /// Wafer type.
        /// </summary>
        [XmlAttribute("Type")]
        public string Type;

        /// <summary>
        /// Sub types.
        /// Key = value.
        /// </summary>
        [XmlIgnore]
        public readonly SortedDictionary<string, string> SubTypes = new SortedDictionary<string, string>();
        [XmlArray("SubTypes")]
        [XmlArrayItem("SubType")]
        public string[] SubTypesUnsorted
        {
            get
            {
                string[] ret = new string[SubTypes.Count];
                SubTypes.Values.CopyTo(ret, 0);
                return ret;
            }

            set
            {
                SubTypes.Clear();

                foreach (string subType in value)
                {
                    SubTypes[subType] = subType;
                }
            }
        }

        public override string ToString()
        {
            return Type;
        }
    }
}
